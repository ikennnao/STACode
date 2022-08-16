using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class SMSNotificationsPresenter
    {
        private CommonMethods commonMethods = new CommonMethods();

        public void CreateAndSendSMSesFromTemplates(CommonWorkFlowExtensions cmWorkFlowObject, SMSNotificationInputParams objNotificationParams)
        {
            SMSObjInputParams smsObjInputParams = null;

            if (objNotificationParams != null)
            {
                smsObjInputParams = GetNotificationConfigRcrdDetails(cmWorkFlowObject, objNotificationParams);

                if (smsObjInputParams != null)
                {
                    smsObjInputParams.entTemplate = commonMethods.GetEmailTemplateDetails(cmWorkFlowObject.srvContextUsr, smsObjInputParams.strTemplateName, smsObjInputParams.intTemplateTypeCode, smsObjInputParams.intTemplateLanguageCode);

                    if (smsObjInputParams.entTemplate != null && smsObjInputParams.entrefRegarding != null)
                    {
                        CreateSMSRecord(cmWorkFlowObject, smsObjInputParams);
                    }
                }
            }
        }

        private SMSObjInputParams GetNotificationConfigRcrdDetails(CommonWorkFlowExtensions cmWorkFlowObject, SMSNotificationInputParams objNotificationParams)
        {
            SMSObjInputParams smsObjProperities = new SMSObjInputParams();
            OptionSetValue optsetSMSLanguage = null;

            if (objNotificationParams.entTargetRcrdDetails != null)
            {
                smsObjProperities.entrefRegarding = objNotificationParams.entTargetRcrdDetails.ToEntityReference();

                #region Get the Related Customer Reference from Target Record

                string[] strAryAttrNames = null;

                if (!string.IsNullOrWhiteSpace(objNotificationParams.strCustomerAttrName))
                {
                    strAryAttrNames = new string[] { objNotificationParams.strCustomerAttrName };

                    if (!string.IsNullOrWhiteSpace(objNotificationParams.strSMSLanguageAttrName))
                    {
                        strAryAttrNames = new string[] { objNotificationParams.strCustomerAttrName, objNotificationParams.strSMSLanguageAttrName };
                    }
                }
                else if (!string.IsNullOrWhiteSpace(objNotificationParams.strSMSLanguageAttrName))
                {
                    strAryAttrNames = new string[] { objNotificationParams.strSMSLanguageAttrName };
                }

                if (strAryAttrNames.Length > 0)
                {
                    objNotificationParams.entTargetRcrdDetails = cmWorkFlowObject.srvContextUsr.Retrieve(objNotificationParams.entTargetRcrdDetails.LogicalName, objNotificationParams.entTargetRcrdDetails.Id, new ColumnSet(strAryAttrNames));

                    if (!string.IsNullOrWhiteSpace(objNotificationParams.strCustomerAttrName))
                    {
                        EntityReference entrefCustomer = commonMethods.GetAttributeValFromTargetEntity(objNotificationParams.entTargetRcrdDetails, objNotificationParams.strCustomerAttrName);
                        if (entrefCustomer != null && !string.IsNullOrWhiteSpace(entrefCustomer.LogicalName) && entrefCustomer.Id != Guid.Empty)
                        {
                            smsObjProperities.entTo = entrefCustomer;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(objNotificationParams.strSMSLanguageAttrName))
                    {
                        optsetSMSLanguage = commonMethods.GetAttributeValFromTargetEntity(objNotificationParams.entTargetRcrdDetails, objNotificationParams.strSMSLanguageAttrName);
                    }
                }

                #endregion

                #region Set the Template Type Code/Target Rcrd ETC

                switch (objNotificationParams.entTargetRcrdDetails.LogicalName)
                {
                    case CaseEntityAttributeNames.EntityLogicalName:
                        smsObjProperities.intTemplateTypeCode = (int)EntityTypeCode.Case;
                        break;
                    default:
                        smsObjProperities.intTemplateTypeCode = (int)EntityTypeCode.SystemUser; //Global Temaplate Type Code
                        break;
                }

                #endregion
            }

            #region Get the Required Template Name from Related Notification Configuration from Target Record

            if (objNotificationParams.entrefNotificationConfig != null)
            {
                Entity entNotificationConfigDetails = null;

                if (objNotificationParams.entrefNotificationConfig != null && !string.IsNullOrWhiteSpace(objNotificationParams.entrefNotificationConfig.LogicalName) && objNotificationParams.entrefNotificationConfig.Id != Guid.Empty)
                {
                    entNotificationConfigDetails = new Entity(objNotificationParams.entrefNotificationConfig.LogicalName, objNotificationParams.entrefNotificationConfig.Id);
                }

                if (entNotificationConfigDetails != null)
                {
                    string strTemplateAttrName = string.Empty;

                    switch (cmWorkFlowObject.workflowContext.MessageName)
                    {
                        case PluginHelperStrigs.CreateMsgName:
                            strTemplateAttrName = NotificationConfigEntityAttributeNames.SMSTemplateCreate;
                            break;
                        case PluginHelperStrigs.UpdateMsgName:
                            strTemplateAttrName = NotificationConfigEntityAttributeNames.SMSTemplateResolve;
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(strTemplateAttrName))
                    {
                        entNotificationConfigDetails = cmWorkFlowObject.srvContextUsr.Retrieve(entNotificationConfigDetails.LogicalName, entNotificationConfigDetails.Id, new ColumnSet(strTemplateAttrName, NotificationConfigEntityAttributeNames.SendFrom, NotificationConfigEntityAttributeNames.FromUser, NotificationConfigEntityAttributeNames.FromTeam));
                    }

                    smsObjProperities.strTemplateName = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, strTemplateAttrName);

                    bool boolSendFrom = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.SendFrom) != null ? commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.SendFrom) : null;

                    if (boolSendFrom != null)
                    {
                        if (boolSendFrom)
                        {
                            // Send From --> 'Team'
                            smsObjProperities.entrefOwner = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromTeam);

                            if (smsObjProperities.entrefOwner != null && smsObjProperities.entrefOwner.LogicalName == TeamEntityAttributeNames.EntityLogicalName && smsObjProperities.entrefOwner.Id != Guid.Empty)
                            {
                                Entity entRetrieveTeamQueue = null;
                                entRetrieveTeamQueue = cmWorkFlowObject.srvContextUsr.Retrieve(TeamEntityAttributeNames.EntityLogicalName, smsObjProperities.entrefOwner.Id, new ColumnSet(TeamEntityAttributeNames.DefaultQueue));

                                if (entRetrieveTeamQueue != null && entRetrieveTeamQueue.Attributes.Count > 0)
                                {
                                    smsObjProperities.entrefFrom = commonMethods.GetAttributeValFromTargetEntity(entRetrieveTeamQueue, TeamEntityAttributeNames.DefaultQueue);
                                }
                            }
                        }
                        else
                        {
                            // Send From --> 'User'
                            smsObjProperities.entrefOwner = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromUser);
                            smsObjProperities.entrefFrom = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromUser);
                        }
                    }

                    if (smsObjProperities.entrefFrom == null)
                    {
                        smsObjProperities.entrefOwner = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromUser);
                        smsObjProperities.entrefFrom = new EntityReference(SystemUserEntityAttributeNames.EntityLogicalName, cmWorkFlowObject.workflowContext.UserId); //System Admin
                    }
                }
            }

            #endregion            

            #region Set the Notification Language

            if (optsetSMSLanguage != null && optsetSMSLanguage.Value != int.MinValue && (optsetSMSLanguage.Value == (int)NotificationLanguage.Arabic || optsetSMSLanguage.Value == (int)NotificationLanguage.English))
            {
                smsObjProperities.intTemplateLanguageCode = optsetSMSLanguage.Value;
            }
            else
            {
                smsObjProperities.intTemplateLanguageCode = (int)NotificationLanguage.English;
            }

            #endregion            

            #region Set the Input Contact Number as 'SMS Recipient Number'

            if (!string.IsNullOrWhiteSpace(objNotificationParams.strContactNo))
            {
                smsObjProperities.strRecipientNo = objNotificationParams.strContactNo;
            }

            #endregion

            return smsObjProperities;
        }

        private void CreateSMSRecord(CommonWorkFlowExtensions cmWorkFlowObject, SMSObjInputParams smsObjInputParams)
        {
            bool checkMandatAttrs = true;

            if (smsObjInputParams != null)
            {
                string strSMSSubject = string.Empty, strSMSBody = string.Empty;

                #region Generate Email Subject && Email Body Details

                if (smsObjInputParams.entTemplate != null && smsObjInputParams.entrefRegarding != null)
                {
                    InstantiateTemplateRequest rqtInstantiateTemplate = new InstantiateTemplateRequest
                    {
                        TemplateId = smsObjInputParams.entTemplate.Id,
                        ObjectId = smsObjInputParams.entrefRegarding.Id,
                        ObjectType = smsObjInputParams.entrefRegarding.LogicalName
                    };

                    InstantiateTemplateResponse rspInstantiateTemplate = (InstantiateTemplateResponse)cmWorkFlowObject.srvContextUsr.Execute(rqtInstantiateTemplate);

                    if (rspInstantiateTemplate != null && rspInstantiateTemplate.EntityCollection != null && rspInstantiateTemplate.EntityCollection.Entities != null
                        && rspInstantiateTemplate.EntityCollection.Entities.Count > 0)
                    {
                        Entity entEmailTemplate = rspInstantiateTemplate.EntityCollection.Entities.FirstOrDefault();

                        strSMSSubject = commonMethods.GetAttributeValFromTargetEntity(entEmailTemplate, "subject");
                        strSMSBody = commonMethods.GetAttributeValFromTargetEntity(entEmailTemplate, "description");
                    }
                }

                #endregion

                Entity[] entAryFromPartyList = null, entAryToPartyList = null;
                EntityReference entrefRegardingObject = null;

                #region Set From Party List

                if (smsObjInputParams.entrefFrom != null)
                {
                    Entity entFromAP = new Entity(ActivityPartyEntityAttributeNames.EntityLogicalName);
                    entFromAP.Attributes[ActivityPartyEntityAttributeNames.PartyId] = smsObjInputParams.entrefFrom;
                    entAryFromPartyList = new Entity[] { entFromAP };
                }

                #endregion

                #region Set To Party List

                if (smsObjInputParams.entTo != null)
                {
                    Entity entToAP = new Entity(ActivityPartyEntityAttributeNames.EntityLogicalName);
                    entToAP.Attributes[ActivityPartyEntityAttributeNames.PartyId] = smsObjInputParams.entTo;
                    entAryToPartyList = new Entity[] { entToAP };
                }

                #endregion

                #region Set Regarding Object Details

                if (smsObjInputParams.entrefRegarding != null)
                {
                    entrefRegardingObject = smsObjInputParams.entrefRegarding;
                }

                #endregion

                #region Set the Required SMS Entity & its related Attribute Values

                Entity entCreateSMS = new Entity(SMSActivityEntityAttributeNames.EntityLogicalName);

                if (entAryFromPartyList != null && entAryFromPartyList.Length == 1)
                {
                    entCreateSMS.Attributes[SMSActivityEntityAttributeNames.From] = entAryFromPartyList;
                }
                if (entAryToPartyList != null && entAryToPartyList.Length > 0)
                {
                    entCreateSMS.Attributes[SMSActivityEntityAttributeNames.To] = entAryToPartyList;
                }
                if (!string.IsNullOrWhiteSpace(strSMSSubject))
                {
                    entCreateSMS.Attributes[SMSActivityEntityAttributeNames.Subject] = strSMSSubject;
                }
                if (!string.IsNullOrWhiteSpace(strSMSBody))
                {
                    Regex rgxExp = new Regex("<[^>]*>");
                    strSMSBody = rgxExp.Replace(strSMSBody, "");
                    entCreateSMS.Attributes[SMSActivityEntityAttributeNames.SMSMessage] = strSMSBody;
                }
                else
                {
                    checkMandatAttrs = false;
                }
                if (!string.IsNullOrWhiteSpace(smsObjInputParams.strRecipientNo))
                {
                    entCreateSMS.Attributes[SMSActivityEntityAttributeNames.RecipientNo] = smsObjInputParams.strRecipientNo;
                }
                else
                {
                    checkMandatAttrs = false;
                }
                if (entrefRegardingObject != null)
                {
                    entCreateSMS.Attributes[SMSActivityEntityAttributeNames.RegardingObject] = entrefRegardingObject;
                }

                entCreateSMS.Attributes[SMSActivityEntityAttributeNames.Owner] = smsObjInputParams.entrefOwner;

                #endregion

                #region Create the SMS Entity

                if (checkMandatAttrs && entCreateSMS != null && entCreateSMS.Attributes.Count > 0)
                {
                    entCreateSMS.Id = cmWorkFlowObject.srvContextUsr.Create(entCreateSMS);
                }

                #endregion                
            }
        }

        public class SMSNotificationInputParams
        {
            public Entity entTargetRcrdDetails { get; set; }
            public EntityReference entrefNotificationConfig { get; set; }
            public string strContactNo { get; set; }
            public string strSMSLanguageAttrName { get; set; }
            public string strCustomerAttrName { get; set; }
        }

        public class SMSObjInputParams
        {
            public EntityReference entrefOwner { get; set; }
            public EntityReference entrefFrom { get; set; }
            public EntityReference entTo { get; set; }
            public EntityReference entrefRegarding { get; set; }
            public string strRecipientNo { get; set; }
            public Entity entTemplate { get; set; }
            public string strTemplateName { get; set; }
            public int intTemplateTypeCode { get; set; }
            public int intTemplateLanguageCode { get; set; }
        }
    }
}