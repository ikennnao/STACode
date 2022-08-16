using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class PopulateConversationInfoPresenter
    {
        private CommonMethods commonMethods = new CommonMethods();
        private CRMFetchXML_s cRMFetchXML_S = new CRMFetchXML_s();

        public void SetConversationRequiredInfo_OnPreOperation(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetConversation = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetConversation != null)
            {
                Entity entRequiredCustomer = null;
                EntityReference entrefCustomer = null, entrefExistingCase = null, entrefCustCategory = null;
                string strEmailAddress = string.Empty, strContactNo = string.Empty, strFirstName = string.Empty, strLastName = string.Empty;

                entrefCustomer = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.Customer);
                entrefCustCategory = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.CustomerCategory);
                entrefExistingCase = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.ExistingCase);
                strEmailAddress = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.EmailAddress);
                strContactNo = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.ContactNumber);
                strFirstName = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.FirstName);
                strLastName = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.LastName);

                #region Retrieve and Populate Customer Info. in Target Conversation

                if (entrefCustomer == null || entrefCustomer.Id == Guid.Empty || string.IsNullOrWhiteSpace(entrefCustomer.LogicalName))
                {
                    #region Populate Customer from Existing Case field in Target Conversation

                    if (entrefExistingCase != null && entrefExistingCase.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefExistingCase.LogicalName))
                    {
                        Entity entRelatedCase = null;
                        entRelatedCase = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefExistingCase.LogicalName, entrefExistingCase.Id, new ColumnSet(CaseEntityAttributeNames.Customer));

                        if (entRelatedCase != null && entRelatedCase.Attributes != null && entRelatedCase.Attributes.Count > 0)
                        {
                            entrefCustomer = commonMethods.GetAttributeValFromTargetEntity(entRelatedCase, CaseEntityAttributeNames.Customer);
                        }

                        entTargetConversation.Attributes[ConversationEntityAttributeNames.IsCaseExisting] = new OptionSetValue(1);
                        entTargetConversation.Attributes[ConversationEntityAttributeNames.DoesAnyCaseAssociated] = true;
                    }

                    #endregion

                    #region Retrieve Customer from Email Address/Contact Number field in Target Conversation

                    else if (!string.IsNullOrWhiteSpace(strEmailAddress) || !string.IsNullOrWhiteSpace(strContactNo))
                    {
                        entRequiredCustomer = RetrieveCustomerInfo(objCommonForAllPlugins.srvContextUsr, strEmailAddress, strContactNo);

                        if (entRequiredCustomer != null)
                        {
                            entrefCustomer = entRequiredCustomer.ToEntityReference();
                        }
                    }

                    #endregion

                    #region Retrieve Anonymous Customer in Target Conversation

                    if (entrefCustomer == null)
                    {
                        entRequiredCustomer = commonMethods.RetrieveAnonymousCustomer(objCommonForAllPlugins.srvContextUsr);

                        if (entRequiredCustomer != null)
                        {
                            entrefCustomer = entRequiredCustomer.ToEntityReference();
                        }
                    }

                    #endregion
                }

                #region Populate Customer Info. in Target Conversation

                if (entrefCustomer != null && entrefCustomer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefCustomer.LogicalName))
                {
                    if (entrefCustomer.LogicalName.ToLower() == ContactEntityAttributeNames.EntityLogicalName)
                    {
                        entRequiredCustomer = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefCustomer.LogicalName, entrefCustomer.Id, new ColumnSet(ContactEntityAttributeNames.PrimaryEmail, ContactEntityAttributeNames.PrimaryContactNo, ContactEntityAttributeNames.SecondaryEmail, ContactEntityAttributeNames.SecondaryContactNo, ContactEntityAttributeNames.CustomerCategory, ContactEntityAttributeNames.FirstName, ContactEntityAttributeNames.LastName));

                        entTargetConversation.Attributes[ConversationEntityAttributeNames.Customer] = entrefCustomer;

                        if (entRequiredCustomer != null && entRequiredCustomer.Attributes != null && entRequiredCustomer.Attributes.Count > 0)
                        {
                            if (string.IsNullOrWhiteSpace(strEmailAddress))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.EmailAddress] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.PrimaryEmail) != null ? commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.PrimaryEmail) : commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.SecondaryEmail);
                            }
                            if (string.IsNullOrWhiteSpace(strContactNo))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.ContactNumber] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.PrimaryContactNo) != null ? commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.PrimaryContactNo) : commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.SecondaryContactNo);
                            }
                            if (entrefCustCategory == null || entrefCustCategory.Id == Guid.Empty)
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.CustomerCategory] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.CustomerCategory);
                            }
                            if (string.IsNullOrWhiteSpace(strFirstName))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.FirstName] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.FirstName);
                            }
                            if (string.IsNullOrWhiteSpace(strLastName))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.LastName] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.LastName);
                            }
                        }
                    }
                    else if (entrefCustomer.LogicalName.ToLower() == AccountEntityAttributeNames.EntityLogicalName)
                    {
                        entRequiredCustomer = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefCustomer.LogicalName, entrefCustomer.Id, new ColumnSet(AccountEntityAttributeNames.EmailAddress, AccountEntityAttributeNames.ContactNumber, AccountEntityAttributeNames.AccountName));

                        entTargetConversation.Attributes[ConversationEntityAttributeNames.Customer] = entrefCustomer;

                        if (entRequiredCustomer != null && entRequiredCustomer.Attributes != null && entRequiredCustomer.Attributes.Count > 0)
                        {
                            if (string.IsNullOrWhiteSpace(strEmailAddress))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.EmailAddress] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.PrimaryEmail);
                            }
                            if (string.IsNullOrWhiteSpace(strContactNo))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.ContactNumber] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, ContactEntityAttributeNames.PrimaryContactNo);
                            }
                            if (entrefCustCategory == null || entrefCustCategory.Id == Guid.Empty)
                            {
                                Entity entB2BCategory = commonMethods.RetrieveRelatedCustomerCategory(objCommonForAllPlugins.srvContextUsr, "1");

                                if (entB2BCategory != null && entB2BCategory.Id != Guid.Empty)
                                {
                                    entTargetConversation.Attributes[ConversationEntityAttributeNames.CustomerCategory] = entB2BCategory.ToEntityReference();
                                }
                            }
                            if (string.IsNullOrWhiteSpace(strFirstName))
                            {
                                entTargetConversation.Attributes[ConversationEntityAttributeNames.FirstName] = commonMethods.GetAttributeValFromTargetEntity(entRequiredCustomer, AccountEntityAttributeNames.AccountName);
                            }
                        }
                    }
                }

                #endregion

                #endregion
            }
        }

        public void SetConversationInActive_OnPostOperation(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetConversation = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetConversation != null)
            {
                EntityReference entrefExistingCase = null;

                entrefExistingCase = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.ExistingCase);


                if (entrefExistingCase != null)
                {
                    #region If Target Conversation contains existing case, then Set Status as 'InActive'

                    // Create the Request Object
                    SetStateRequest rqtSetState = new SetStateRequest()
                    {
                        // Set the Request Object's Properties
                        State = new OptionSetValue((int)EntityStateCode.InActive),
                        Status = new OptionSetValue((int)EntityStatusCode.InActive),

                        // Point the Request to the case whose state is being changed
                        EntityMoniker = entTargetConversation.ToEntityReference()
                    };

                    // Execute the Request
                    SetStateResponse rspSetState = (SetStateResponse)(objCommonForAllPlugins.srvContextUsr.Execute(rqtSetState));

                    #endregion

                    #region Update the Target Conversation as 'Most Recent Conversation' to 'Related Case'

                    Entity entUpdateCase = new Entity(entrefExistingCase.LogicalName, entrefExistingCase.Id);
                    entUpdateCase.Attributes[CaseEntityAttributeNames.RelatedConversation] = entTargetConversation.ToEntityReference();
                    entUpdateCase.Attributes[CaseEntityAttributeNames.ConversationIncomingMsg] = commonMethods.GetAttributeValFromTargetEntity(entTargetConversation, ConversationEntityAttributeNames.IncomingMessage);
                    objCommonForAllPlugins.srvContextUsr.Update(entUpdateCase);

                    #endregion
                }
            }
        }

        private Entity RetrieveCustomerInfo(IOrganizationService orgService, string strEmailAddress, string strContactNo)
        {
            Entity entRetrieveCustomer = null;
            string strFetchToGetCustomer = cRMFetchXML_S.GetContactInfoDetailsFromPrimaryInfo(strEmailAddress, strContactNo);

            if (!string.IsNullOrWhiteSpace(strFetchToGetCustomer))
            {
                EntityCollection entcolRequiredCustomer = null;
                entcolRequiredCustomer = orgService.RetrieveMultiple(new FetchExpression(strFetchToGetCustomer));

                if (entcolRequiredCustomer != null && entcolRequiredCustomer.Entities != null && entcolRequiredCustomer.Entities.Count > 0)
                {
                    entRetrieveCustomer = entcolRequiredCustomer.Entities[0];
                }
                else
                {
                    strFetchToGetCustomer = cRMFetchXML_S.GetContactInfoDetailsFromSecondaryInfo(strEmailAddress, strContactNo);

                    if (!string.IsNullOrWhiteSpace(strFetchToGetCustomer))
                    {
                        entcolRequiredCustomer = null;
                        entcolRequiredCustomer = orgService.RetrieveMultiple(new FetchExpression(strFetchToGetCustomer));

                        if (entcolRequiredCustomer != null && entcolRequiredCustomer.Entities != null && entcolRequiredCustomer.Entities.Count > 0)
                        {
                            entRetrieveCustomer = entcolRequiredCustomer.Entities[0];
                        }
                        else
                        {
                            strFetchToGetCustomer = cRMFetchXML_S.GetAccountInfoDetailsFromPrimaryInfo(strEmailAddress, strContactNo);

                            if (!string.IsNullOrWhiteSpace(strFetchToGetCustomer))
                            {
                                entcolRequiredCustomer = null;
                                entcolRequiredCustomer = orgService.RetrieveMultiple(new FetchExpression(strFetchToGetCustomer));

                                if (entcolRequiredCustomer != null && entcolRequiredCustomer.Entities != null && entcolRequiredCustomer.Entities.Count > 0)
                                {
                                    entRetrieveCustomer = entcolRequiredCustomer.Entities[0];
                                }
                            }
                        }
                    }
                }
            }

            return entRetrieveCustomer;
        }
    }
}