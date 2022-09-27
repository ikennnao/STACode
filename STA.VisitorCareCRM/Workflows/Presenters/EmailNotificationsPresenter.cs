using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using System;
using System.Linq;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class EmailNotificationsPresenter
    {
        private CommonMethods commonMethods = new CommonMethods();
        private const string SurveyURLPlaceholder = "Case_Record_GUID";

        public void CreateAndSendEmailsFromTemplates(CommonWorkFlowExtensions cmWorkFlowObject, EmailNotificationInputParams objNotificationParams)
        {
            EmailObjInputParams emailObjInputParams = null;

            if (objNotificationParams != null)
            {
                emailObjInputParams = GetNotificationConfigRcrdDetails(cmWorkFlowObject, objNotificationParams);

                if (emailObjInputParams != null)
                {
                    emailObjInputParams.entTemplate = commonMethods.GetEmailTemplateDetails(cmWorkFlowObject.srvContextUsr, emailObjInputParams.strTemplateName, emailObjInputParams.intTemplateTypeCode, emailObjInputParams.intTemplateLanguageCode);

                    if (emailObjInputParams.entTemplate != null && emailObjInputParams.entrefRegarding != null)
                    {
                        CreateAndSendEmail(cmWorkFlowObject, emailObjInputParams);
                    }
                }
            }
        }

        private EmailObjInputParams GetNotificationConfigRcrdDetails(CommonWorkFlowExtensions cmWorkFlowObject, EmailNotificationInputParams objNotificationParams)
        {
            EmailObjInputParams emailObjProperities = new EmailObjInputParams();
            OptionSetValue optsetEmailLanguage = null;

            if (objNotificationParams.entTargetRcrdDetails != null)
            {
                emailObjProperities.entrefRegarding = objNotificationParams.entTargetRcrdDetails.ToEntityReference();

                #region Get the Related Customer Reference from Target Record

                string[] strAryAttrNames = null;

                if (!string.IsNullOrWhiteSpace(objNotificationParams.strCustomerAttrName))
                {
                    strAryAttrNames = new string[] { objNotificationParams.strCustomerAttrName };

                    if (!string.IsNullOrWhiteSpace(objNotificationParams.strEmailLanguageAttrName))
                    {
                        strAryAttrNames = new string[] { objNotificationParams.strCustomerAttrName, objNotificationParams.strEmailLanguageAttrName };
                    }
                }
                else if (!string.IsNullOrWhiteSpace(objNotificationParams.strEmailLanguageAttrName))
                {
                    strAryAttrNames = new string[] { objNotificationParams.strEmailLanguageAttrName };
                }

                if (strAryAttrNames.Length > 0)
                {
                    objNotificationParams.entTargetRcrdDetails = cmWorkFlowObject.srvContextUsr.Retrieve(objNotificationParams.entTargetRcrdDetails.LogicalName, objNotificationParams.entTargetRcrdDetails.Id, new ColumnSet(strAryAttrNames));

                    if (!string.IsNullOrWhiteSpace(objNotificationParams.strCustomerAttrName))
                    {
                        EntityReference entrefCustomer = commonMethods.GetAttributeValFromTargetEntity(objNotificationParams.entTargetRcrdDetails, objNotificationParams.strCustomerAttrName);

                        if (entrefCustomer != null && !string.IsNullOrWhiteSpace(entrefCustomer.LogicalName) && entrefCustomer.Id != Guid.Empty)
                        {
                            emailObjProperities.entTo = entrefCustomer;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(objNotificationParams.strEmailLanguageAttrName))
                    {
                        optsetEmailLanguage = commonMethods.GetAttributeValFromTargetEntity(objNotificationParams.entTargetRcrdDetails, objNotificationParams.strEmailLanguageAttrName);
                    }
                }

                #endregion

                #region Set the Template Type Code/Target Rcrd ETC

                switch (objNotificationParams.entTargetRcrdDetails.LogicalName)
                {
                    case CaseEntityAttributeNames.EntityLogicalName:
                        emailObjProperities.intTemplateTypeCode = (int)EntityTypeCode.Case;
                        break;
                    default:
                        emailObjProperities.intTemplateTypeCode = (int)EntityTypeCode.SystemUser;
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
                            strTemplateAttrName = NotificationConfigEntityAttributeNames.EmailTemplateCreate;
                            break;
                        case PluginHelperStrigs.UpdateMsgName:
                            strTemplateAttrName = NotificationConfigEntityAttributeNames.EmailTemplateResolve;
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(strTemplateAttrName))
                    {
                        entNotificationConfigDetails = cmWorkFlowObject.srvContextUsr.Retrieve(entNotificationConfigDetails.LogicalName, entNotificationConfigDetails.Id, new ColumnSet(strTemplateAttrName, NotificationConfigEntityAttributeNames.SendFrom, NotificationConfigEntityAttributeNames.FromUser, NotificationConfigEntityAttributeNames.FromTeam));
                    }

                    emailObjProperities.strTemplateName = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, strTemplateAttrName);

                    bool ?boolSendFrom = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.SendFrom) != null ? commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.SendFrom) : null;

                    if (boolSendFrom != null)
                    {
                        if (boolSendFrom == true)
                        {
                            // Send From --> 'Team'
                            emailObjProperities.entrefOwner = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromTeam);

                            if (emailObjProperities.entrefOwner != null && emailObjProperities.entrefOwner.LogicalName == TeamEntityAttributeNames.EntityLogicalName && emailObjProperities.entrefOwner.Id != Guid.Empty)
                            {
                                Entity entRetrieveTeamQueue = null;
                                entRetrieveTeamQueue = cmWorkFlowObject.srvContextUsr.Retrieve(TeamEntityAttributeNames.EntityLogicalName, emailObjProperities.entrefOwner.Id, new ColumnSet(TeamEntityAttributeNames.DefaultQueue));

                                if (entRetrieveTeamQueue != null && entRetrieveTeamQueue.Attributes.Count > 0)
                                {
                                    emailObjProperities.entrefFrom = commonMethods.GetAttributeValFromTargetEntity(entRetrieveTeamQueue, TeamEntityAttributeNames.DefaultQueue);
                                }
                            }
                        }
                        else
                        {
                            // Send From --> 'User'
                            emailObjProperities.entrefOwner = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromUser);
                            emailObjProperities.entrefFrom = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromUser);
                        }
                    }

                    if (emailObjProperities.entrefFrom == null)
                    {
                        emailObjProperities.entrefOwner = commonMethods.GetAttributeValFromTargetEntity(entNotificationConfigDetails, NotificationConfigEntityAttributeNames.FromUser);
                        emailObjProperities.entrefFrom = new EntityReference(SystemUserEntityAttributeNames.EntityLogicalName, cmWorkFlowObject.workflowContext.UserId); //System Admin
                    }
                }
            }

            #endregion

            #region Set the Custom Email Address as 'Address Used'

            if (!string.IsNullOrWhiteSpace(objNotificationParams.strEmailAddress))
            {
                emailObjProperities.strAddressUsed = objNotificationParams.strEmailAddress;
            }

            #endregion

            #region Set the Notification Language

            if (optsetEmailLanguage != null && optsetEmailLanguage.Value != int.MinValue && (optsetEmailLanguage.Value == (int)NotificationLanguage.Arabic || optsetEmailLanguage.Value == (int)NotificationLanguage.English))
            {
                emailObjProperities.intTemplateLanguageCode = optsetEmailLanguage.Value;
            }
            else
            {
                emailObjProperities.intTemplateLanguageCode = (int)NotificationLanguage.English;
            }

            #endregion            

            return emailObjProperities;
        }

        private void CreateAndSendEmail(CommonWorkFlowExtensions cmWorkFlowObject, EmailObjInputParams emailObjInputParams)
        {
            bool checkMandatAttrs = true; string newEmailBody = string.Empty;

            if (emailObjInputParams != null)
            {
                string strEmailSubject = string.Empty, strEmailBody = string.Empty;

                #region Generate Email Subject && Email Body Details

                if (emailObjInputParams.entTemplate != null && emailObjInputParams.entrefRegarding != null)
                {
                    InstantiateTemplateRequest rqtInstantiateTemplate = new InstantiateTemplateRequest
                    {
                        TemplateId = emailObjInputParams.entTemplate.Id,
                        ObjectId = emailObjInputParams.entrefRegarding.Id,
                        ObjectType = emailObjInputParams.entrefRegarding.LogicalName
                    };

                    InstantiateTemplateResponse rspInstantiateTemplate = (InstantiateTemplateResponse)cmWorkFlowObject.srvContextUsr.Execute(rqtInstantiateTemplate);

                    if (rspInstantiateTemplate != null && rspInstantiateTemplate.EntityCollection != null && rspInstantiateTemplate.EntityCollection.Entities != null
                        && rspInstantiateTemplate.EntityCollection.Entities.Count > 0)
                    {
                        Entity entEmailTemplate = rspInstantiateTemplate.EntityCollection.Entities.FirstOrDefault();

                        strEmailSubject = commonMethods.GetAttributeValFromTargetEntity(entEmailTemplate, "subject");
                        strEmailBody = commonMethods.GetAttributeValFromTargetEntity(entEmailTemplate, "description");
                        newEmailBody = strEmailBody.Contains(SurveyURLPlaceholder) ? strEmailBody.Replace(SurveyURLPlaceholder, emailObjInputParams.entrefRegarding.Id.ToString()) : strEmailBody;
                    
                    }
                }

                #endregion

                Entity[] entAryFromPartyList = null, entAryToPartyList = null;
                EntityReference entrefRegardingObject = null;

                #region Set From Party List

                if (emailObjInputParams.entrefFrom != null)
                {
                    Entity entFromAP = new Entity(ActivityPartyEntityAttributeNames.EntityLogicalName);
                    entFromAP.Attributes[ActivityPartyEntityAttributeNames.PartyId] = emailObjInputParams.entrefFrom;
                    entAryFromPartyList = new Entity[] { entFromAP };
                }

                #endregion

                #region Set To Party List

                if (emailObjInputParams.entTo != null)
                {
                    Entity entToAP = new Entity(ActivityPartyEntityAttributeNames.EntityLogicalName);
                    entToAP.Attributes[ActivityPartyEntityAttributeNames.PartyId] = emailObjInputParams.entTo;
                    if (!string.IsNullOrWhiteSpace(emailObjInputParams.strAddressUsed))
                    {
                        entToAP.Attributes[ActivityPartyEntityAttributeNames.AddressUsed] = emailObjInputParams.strAddressUsed;
                    }
                    entAryToPartyList = new Entity[] { entToAP };
                }

                #endregion

                #region Set Regarding Object Details

                if (emailObjInputParams.entrefRegarding != null)
                {
                    entrefRegardingObject = emailObjInputParams.entrefRegarding;
                }

                #endregion

                #region Set the Required Email Entity & its related Attribute Values

                Entity entCreateEmail = new Entity(EmailEntityAttributeNames.EntityLogicalName);

                if (entAryFromPartyList != null && entAryFromPartyList.Length > 0)
                {
                    entCreateEmail.Attributes[EmailEntityAttributeNames.From] = entAryFromPartyList;
                }
                else
                {
                    checkMandatAttrs = false;
                }
                if (entAryToPartyList != null && entAryToPartyList.Length > 0)
                {
                    entCreateEmail.Attributes[EmailEntityAttributeNames.To] = entAryToPartyList;
                }
                else
                {
                    checkMandatAttrs = false;
                }
                if (!string.IsNullOrWhiteSpace(strEmailSubject))
                {
                    entCreateEmail.Attributes[EmailEntityAttributeNames.Subject] = strEmailSubject;
                }
                else
                {
                    checkMandatAttrs = false;
                }
                if (!string.IsNullOrWhiteSpace(newEmailBody))
                {
                    entCreateEmail.Attributes[EmailEntityAttributeNames.Description] = newEmailBody;
                }
                else
                {
                    checkMandatAttrs = false;
                }
                if (entrefRegardingObject != null)
                {
                    entCreateEmail.Attributes[EmailEntityAttributeNames.RegardingObject] = entrefRegardingObject;
                }
                entCreateEmail.Attributes[EmailEntityAttributeNames.DueDate] = DateTime.UtcNow;
                entCreateEmail.Attributes[EmailEntityAttributeNames.Owner] = emailObjInputParams.entrefOwner;

                #endregion

                #region Create the Email Entity

                if (checkMandatAttrs && entCreateEmail != null && entCreateEmail.Attributes.Count > 0)
                {
                    entCreateEmail.Id = cmWorkFlowObject.srvContextUsr.Create(entCreateEmail);
                }

                #endregion

                #region Trigger the Send Email Request

                if (entCreateEmail.Id != Guid.Empty)
                {
                    SendEmailRequest rqtSendEmail = new SendEmailRequest
                    {
                        IssueSend = true,
                        EmailId = entCreateEmail.Id
                    };

                    SendEmailResponse rspSendEmail = (SendEmailResponse)cmWorkFlowObject.srvContextUsr.Execute(rqtSendEmail);
                }

                #endregion
            }
        }
    }

    public class EmailNotificationInputParams
    {
        public Entity entTargetRcrdDetails { get; set; }
        public EntityReference entrefNotificationConfig { get; set; }
        public string strEmailAddress { get; set; }
        public string strEmailLanguageAttrName { get; set; }
        public string strCustomerAttrName { get; set; }
    }

    public class EmailObjInputParams
    {
        public EntityReference entrefOwner { get; set; }
        public EntityReference entrefFrom { get; set; }
        public EntityReference entTo { get; set; }
        public EntityReference entrefRegarding { get; set; }
        public Entity entTemplate { get; set; }
        public string strTemplateName { get; set; }
        public int intTemplateTypeCode { get; set; }
        public int intTemplateLanguageCode { get; set; }
        public string strAddressUsed { get; set; }
    }
}