using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Configuration;
using System.Linq;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Utility
{
    public class CRMCommonMethods
    {
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CRMFetchXML_s cRMFetchXML_S = new CRMFetchXML_s();

        public class RqtObjSocialHandleDetails
        {
            public string UserName { get; set; }
            public string UserId { get; set; }
            public string HandleType { get; set; }
        }

        public class RspObjSocialHandleDetails
        {
            public string UserName { get; set; }
            public string UserId { get; set; }
            public string NetworkId { get; set; }
            public CustomEntityReference HandleType { get; set; }
            public CustomEntityReference Customer { get; set; }
        }

        public Guid PostValidateSessionConditions(OrganizationServiceProxy orgService, AuthenticateRequest rqtAuthenicateDetails)
        {
            Guid guidLoggedInUser = Guid.Empty;

            try
            {
                if (rqtAuthenicateDetails != null)
                {
                    if (!string.IsNullOrWhiteSpace(rqtAuthenicateDetails.UserName))
                    {
                        guidLoggedInUser = GetLoggedInUserId(orgService, rqtAuthenicateDetails.UserName, guidLoggedInUser);

                        if (guidLoggedInUser == Guid.Empty)
                        {
                            throw new Exception(customErrorMsgs.missingLoggedInUserId);
                        }
                    }
                    else
                    {
                        throw new Exception(customErrorMsgs.missingUserName);
                    }
                }
                else
                {
                    throw new Exception(customErrorMsgs.missingAuthenticateDetails);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return guidLoggedInUser;
        }

        public Guid GetLoggedInUserId(OrganizationServiceProxy orgService, string strUser, Guid guidLoggedInUser)
        {
            Guid guidTryParsed = Guid.Empty;

            bool isVaildGuid = false;

            if (!string.IsNullOrWhiteSpace(strUser))
            {
                isVaildGuid = Guid.TryParse(strUser, out guidTryParsed);
            }

            QueryExpression qrySystemUser = new QueryExpression(SystemUserEntityAttributeNames.EntityLogicalName);
            qrySystemUser.ColumnSet = new ColumnSet(false);
            FilterExpression filterExp = null;
            qrySystemUser.Criteria.AddCondition(SystemUserEntityAttributeNames.IsDisabled, ConditionOperator.Equal, false);

            if (!isVaildGuid)
            {
                filterExp = new FilterExpression();
                filterExp.FilterOperator = LogicalOperator.Or;

                if (!string.IsNullOrWhiteSpace(strUser))
                {
                    string strCRMLoginDomain = ConfigurationManager.AppSettings["CRMLoginDomain"];

                    if (!string.IsNullOrWhiteSpace(strCRMLoginDomain))
                    {
                        filterExp.AddCondition(SystemUserEntityAttributeNames.DomainName, ConditionOperator.Equal, string.Format(@"{0}\{1}", strCRMLoginDomain, strUser));
                    }
                    filterExp.AddCondition(SystemUserEntityAttributeNames.PrimaryEmail, ConditionOperator.Equal, strUser);
                }
            }
            else
            {
                if (guidTryParsed != Guid.Empty)
                {
                    qrySystemUser.Criteria.AddCondition(SystemUserEntityAttributeNames.SystemUserId, ConditionOperator.Equal, guidTryParsed);
                }
            }

            if (qrySystemUser.Criteria.Conditions.Count > 0)
            {
                if (filterExp != null && filterExp.Conditions.Count > 0)
                {
                    qrySystemUser.Criteria.AddFilter(filterExp);
                }

                EntityCollection entcolSystemUsers = orgService.RetrieveMultiple(qrySystemUser);

                if (entcolSystemUsers != null && entcolSystemUsers.Entities != null && entcolSystemUsers.Entities.Count > 0)
                {
                    guidLoggedInUser = entcolSystemUsers.Entities[0].Id;
                }
            }

            return guidLoggedInUser;
        }

        public dynamic GetAttributeValFromTargetEntity(Entity entTarget, string strAttributeName)
        {
            dynamic otptAttributeVale = null;
            if (!string.IsNullOrWhiteSpace(strAttributeName) && entTarget != null && entTarget.Attributes.Contains(strAttributeName))
            {
                otptAttributeVale = entTarget.Attributes[strAttributeName];
            }
            return otptAttributeVale;
        }

        public dynamic GetAttributeFormattedValFromTargetEntity(Entity entTarget, string strAttributeName)
        {
            dynamic otptAttributeVale = null;
            if (!string.IsNullOrWhiteSpace(strAttributeName) && entTarget != null && entTarget.FormattedValues.Contains(strAttributeName))
            {
                otptAttributeVale = entTarget.FormattedValues[strAttributeName];
            }
            return otptAttributeVale;
        }

        public int RetrieveDefaultPriority(OrganizationServiceProxy orgService, Guid guidSubCatgry)
        {
            int defaultPriorityVal = int.MinValue;

            Entity entSubCatgry = null;
            entSubCatgry = orgService.Retrieve(CaseSubCategoryEntityAttributeNames.EntityLogicalName, guidSubCatgry, new ColumnSet(CaseSubCategoryEntityAttributeNames.DefaultPriority));

            if (entSubCatgry != null && entSubCatgry.Attributes != null && entSubCatgry.Attributes.Count > 0)
            {
                OptionSetValue optstDefaultPriority = GetAttributeValFromTargetEntity(entSubCatgry, CaseSubCategoryEntityAttributeNames.DefaultPriority);

                if (optstDefaultPriority != null && optstDefaultPriority.Value != int.MinValue)
                {
                    defaultPriorityVal = optstDefaultPriority.Value;
                }
            }

            return defaultPriorityVal;
        }

        public EntityReference RetrieveAnonymousCustomer(OrganizationServiceProxy orgService)
        {
            EntityReference entrefAnonymousCust = null;

            string strQueryForAnonymousCust = cRMFetchXML_S.GetQueryForAnonymousCustomer();

            if (!string.IsNullOrWhiteSpace(strQueryForAnonymousCust))
            {
                EntityCollection entcolAnonymousCust = null;
                entcolAnonymousCust = orgService.RetrieveMultiple(new FetchExpression(strQueryForAnonymousCust));

                if (entcolAnonymousCust != null && entcolAnonymousCust.Entities != null && entcolAnonymousCust.Entities.Count > 0)
                {
                    entrefAnonymousCust = new EntityReference(entcolAnonymousCust.Entities[0].LogicalName, entcolAnonymousCust.Entities[0].Id);
                }
            }

            return entrefAnonymousCust;
        }

        public Entity RetrieveCaseFromCaseRefNo(OrganizationServiceProxy orgService, string strCaseRefNo)
        {
            Entity entRequiredCase = null;
            EntityCollection entcolCases = null;

            string strFetchToRetrieveCase = cRMFetchXML_S.GetCaseFromCaseRefNo(strCaseRefNo);

            if (!string.IsNullOrWhiteSpace(strFetchToRetrieveCase))
            {
                entcolCases = orgService.RetrieveMultiple(new FetchExpression(strFetchToRetrieveCase));
            }

            if (entcolCases != null && entcolCases.Entities != null && entcolCases.Entities.Count > 0)
            {
                entRequiredCase = entcolCases.Entities[0];
            }

            return entRequiredCase;
        }

        public Entity RetrieveChannelOriginFromOriginCode(OrganizationServiceProxy orgService, string strOriginCode)
        {
            Entity entChannelOrigin = null;

            RqtObjChannelOrigin rqtObjChannelOrigin = new RqtObjChannelOrigin();
            rqtObjChannelOrigin.ChannelOriginCode = strOriginCode;

            string strFetchToRetrieveChannelOrigins = cRMFetchXML_S.GetChannelOrigin(rqtObjChannelOrigin);

            if (!string.IsNullOrWhiteSpace(strFetchToRetrieveChannelOrigins))
            {
                EntityCollection entcolChannelOrigins = orgService.RetrieveMultiple(new FetchExpression(strFetchToRetrieveChannelOrigins));

                if (entcolChannelOrigins != null && entcolChannelOrigins.Entities != null && entcolChannelOrigins.Entities.Count > 0)
                {
                    entChannelOrigin = entcolChannelOrigins.Entities[0];
                }
            }

            return entChannelOrigin;
        }

        public Entity RetrieveCustomerCategoryFromCategoryId(OrganizationServiceProxy orgService, string strCategoryId)
        {
            Entity entChannelOrigin = null;

            RetrieveCustomerCategoryObj retrieveCustomerCategoryObj = new RetrieveCustomerCategoryObj();
            retrieveCustomerCategoryObj.CategoryId = strCategoryId;

            string strFetchToRetrieveCustCategry = cRMFetchXML_S.GetCustomerCategory(retrieveCustomerCategoryObj);

            if (!string.IsNullOrWhiteSpace(strFetchToRetrieveCustCategry))
            {
                EntityCollection entcolCustCategrys = orgService.RetrieveMultiple(new FetchExpression(strFetchToRetrieveCustCategry));

                if (entcolCustCategrys != null && entcolCustCategrys.Entities != null && entcolCustCategrys.Entities.Count > 0)
                {
                    entChannelOrigin = entcolCustCategrys.Entities[0];
                }
            }

            return entChannelOrigin;
        }

        public Entity RetrieveSMCFromNetworkId(OrganizationServiceProxy orgService, RqtObjSocialHandleDetails objSocialHandleDetails)
        {
            Entity entSocialMediaHandle = null;

            if (objSocialHandleDetails != null)
            {
                string strFetchToRetrieveCase = cRMFetchXML_S.GetSocialMediaHandle(objSocialHandleDetails);

                if (!string.IsNullOrWhiteSpace(strFetchToRetrieveCase))
                {
                    EntityCollection entcolSCHs = orgService.RetrieveMultiple(new FetchExpression(strFetchToRetrieveCase));

                    if (entcolSCHs != null && entcolSCHs.Entities != null && entcolSCHs.Entities.Count > 0)
                    {
                        entSocialMediaHandle = entcolSCHs.Entities[0];
                    }
                }
            }

            return entSocialMediaHandle;
        }

        public Entity CreateSMCFromNetworkId(OrganizationServiceProxy orgService, RqtObjSocialHandleDetails objSocialHandleDetails, EntityReference entrefSocialHandle, EntityReference entrefCustomer)
        {
            Entity entSocialMediaHandle = null;

            if (objSocialHandleDetails != null && entrefSocialHandle != null && entrefSocialHandle.Id != Guid.Empty)
            {
                entSocialMediaHandle = new Entity(SocialMediaHandleEntityAttributesNames.EntityLogicalName);
                entSocialMediaHandle.Attributes[SocialMediaHandleEntityAttributesNames.SocialUserName] = objSocialHandleDetails.UserName;
                entSocialMediaHandle.Attributes[SocialMediaHandleEntityAttributesNames.SocialUserID] = objSocialHandleDetails.UserId;
                entSocialMediaHandle.Attributes[SocialMediaHandleEntityAttributesNames.SocialHandle] = entrefSocialHandle;

                if (entrefCustomer != null && entrefCustomer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefCustomer.LogicalName))
                {
                    entSocialMediaHandle.Attributes[SocialMediaHandleEntityAttributesNames.Customer] = new EntityReference(entrefCustomer.LogicalName, entrefCustomer.Id);
                }

                entSocialMediaHandle.Id = orgService.Create(entSocialMediaHandle);
            }

            return entSocialMediaHandle;
        }

        public void UpdateSMCFromNetworkId(OrganizationServiceProxy orgService, Entity entSocialMediaHandle, EntityReference entrefCustomer)
        {
            if (entSocialMediaHandle != null && entSocialMediaHandle.Id != Guid.Empty)
            {
                if (entrefCustomer != null && entrefCustomer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefCustomer.LogicalName))
                {
                    entSocialMediaHandle.Attributes[SocialMediaHandleEntityAttributesNames.Customer] = new EntityReference(entrefCustomer.LogicalName, entrefCustomer.Id);
                }
                orgService.Update(entSocialMediaHandle);
            }
        }

        public Entity GetOwningTeam(string owningTeamName, OrganizationServiceProxy service)
        {
            Entity Value = null;
            ColumnSet columnSet = new ColumnSet(true);
            QueryExpression queryExpression = new QueryExpression(TeamEntityAttributeNames.EntityLogicalName);
            ConditionExpression conditionExpression = new ConditionExpression(TeamEntityAttributeNames.TeamName, ConditionOperator.Equal, owningTeamName);
            queryExpression.Criteria.Conditions.Add(conditionExpression);
            queryExpression.ColumnSet = columnSet;
            var team = service.RetrieveMultiple(queryExpression);

            if (team != null && team.Entities != null && team.Entities.Count > 0)
            {
                Value = team.Entities[0];

            }
            return Value;
        }

        #region Un-Used Methods

        public class OptionsetCustom
        {
            public string text { get; set; }
            public int? value { get; set; }
        }

        public OptionsetCustom GetOptionsetCustomValue(Entity entTarget, string strAttributeName)
        {
            OptionsetCustom optionsetCustom = null;

            try
            {
                if (entTarget != null && !string.IsNullOrWhiteSpace(strAttributeName) && entTarget.Attributes.ContainsKey(strAttributeName) && entTarget.GetAttributeValue<OptionSetValue>(strAttributeName) != null)
                {
                    optionsetCustom = new OptionsetCustom();
                    optionsetCustom.value = entTarget.GetAttributeValue<OptionSetValue>(strAttributeName).Value;
                    optionsetCustom.text = entTarget.FormattedValues.ContainsKey(strAttributeName) ? entTarget.FormattedValues[strAttributeName] : null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return optionsetCustom;
        }

        public bool IsUserPartofProductTeam(OrganizationServiceProxy orgService, Guid guidUserId)
        {
            bool _checkUserIsProductTeam = false;

            if (guidUserId != Guid.Empty)
            {
                // Execute the query.
                EntityCollection entcolUserRoles = GetUserRoles(orgService, guidUserId);

                if (entcolUserRoles != null && entcolUserRoles.Entities != null && entcolUserRoles.Entities.Count > 0)
                {
                    string strProductTeamRoleName = string.Empty;
                    strProductTeamRoleName = ConfigurationManager.AppSettings["ProductTeamRole"];

                    if (!string.IsNullOrWhiteSpace(strProductTeamRoleName))
                    {
                        _checkUserIsProductTeam = entcolUserRoles.Entities.Any(x => x.Attributes.Contains(RoleEntityAttributeNames.RoleName) && Convert.ToString(x.Attributes[RoleEntityAttributeNames.RoleName]).ToLower() == strProductTeamRoleName.ToLower());
                    }
                }
            }

            return _checkUserIsProductTeam;
        }

        public EntityCollection GetUserRoles(OrganizationServiceProxy orgService, Guid guidUserId)
        {
            EntityCollection entcolUserRoles = null;

            try
            {
                if (guidUserId != Guid.Empty)
                {
                    // Create a QueryExpression.
                    QueryExpression qryExpRoles = new QueryExpression(RoleEntityAttributeNames.EntityLogicalName);
                    qryExpRoles.ColumnSet = new ColumnSet(RoleEntityAttributeNames.RoleName);

                    // Set up the join between the role entity
                    // and the intersect table systemuserroles.
                    LinkEntity linkEntity1 = new LinkEntity();
                    linkEntity1.LinkFromEntityName = RoleEntityAttributeNames.EntityLogicalName;
                    linkEntity1.LinkFromAttributeName = RoleEntityAttributeNames.RoleId;
                    linkEntity1.LinkToEntityName = SystemUserRoleEntityAttributeNames.EntityLogicalName;
                    linkEntity1.LinkToAttributeName = SystemUserRoleEntityAttributeNames.RoleId;

                    // Set up the join between the intersect table
                    // systemuserroles and the systemuser entity.
                    LinkEntity linkEntity2 = new LinkEntity();
                    linkEntity2.LinkFromEntityName = SystemUserRoleEntityAttributeNames.EntityLogicalName;
                    linkEntity2.LinkFromAttributeName = SystemUserRoleEntityAttributeNames.SystemUserId;
                    linkEntity2.LinkToEntityName = SystemUserEntityAttributeNames.EntityLogicalName;
                    linkEntity2.LinkToAttributeName = SystemUserEntityAttributeNames.SystemUserId;

                    // The condition is to find the user ID.
                    ConditionExpression conExpUserId = new ConditionExpression(SystemUserEntityAttributeNames.SystemUserId, ConditionOperator.Equal, guidUserId);
                    linkEntity2.LinkCriteria.Conditions.Add(conExpUserId);

                    linkEntity1.LinkEntities.Add(linkEntity2);
                    qryExpRoles.LinkEntities.Add(linkEntity1);

                    // Execute the query.
                    entcolUserRoles = orgService.RetrieveMultiple(qryExpRoles);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entcolUserRoles;
        }

        #endregion
    }
}