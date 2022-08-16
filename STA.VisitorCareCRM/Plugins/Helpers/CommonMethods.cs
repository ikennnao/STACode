using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Resources;
using System;
using System.Collections.Generic;
using System.Xml;

namespace STA.TouristCareCRM.Plugins.Helpers
{
    public class CommonMethods
    {
        CRMFetchXML_s cRMFetchXML_S = new CRMFetchXML_s();

        public Entity RetrieveTargetEntityFromContext(IPluginExecutionContext exeContext)
        {
            Entity entTarget = null;
            if (exeContext.InputParameters.Contains(PluginHelperStrigs.Target) && exeContext.InputParameters[PluginHelperStrigs.Target] is Entity)
            {
                entTarget = (Entity)exeContext.InputParameters[PluginHelperStrigs.Target];
            }
            return entTarget;
        }

        public Entity RetrievePreImageEntityFromContext(IPluginExecutionContext exeContext)
        {
            Entity entPreImage = null;
            if (exeContext.PreEntityImages.Contains(PluginHelperStrigs.PreImageName) && exeContext.PreEntityImages[PluginHelperStrigs.PreImageName] is Entity)
            {
                entPreImage = exeContext.PreEntityImages[PluginHelperStrigs.PreImageName];
            }
            return entPreImage;
        }

        public Entity RetrievePostImageEntityFromContext(IPluginExecutionContext exeContext)
        {
            Entity entPostImage = null;
            if (exeContext.PostEntityImages.Contains(PluginHelperStrigs.PostImageName) && exeContext.PostEntityImages[PluginHelperStrigs.PostImageName] is Entity)
            {
                entPostImage = exeContext.PostEntityImages[PluginHelperStrigs.PostImageName];
            }
            return entPostImage;
        }

        public EntityReference RetrieveEntityMonikerEntRefFromContext(IPluginExecutionContext exeContext)
        {
            EntityReference entrefTarget = null;
            if (exeContext.InputParameters.Contains(PluginHelperStrigs.EntityMoniker) && exeContext.InputParameters[PluginHelperStrigs.EntityMoniker] is Entity)
            {
                entrefTarget = (EntityReference)exeContext.InputParameters[PluginHelperStrigs.EntityMoniker];
            }
            return entrefTarget;
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

        public dynamic GetAttributeValFromTargetOrImageEntities(Entity entTarget, Entity entImage, string strAttributeName)
        {
            dynamic otptAttributeVale = null;
            if (!string.IsNullOrWhiteSpace(strAttributeName))
            {
                if (entTarget != null && entTarget.Attributes.Contains(strAttributeName))
                {
                    otptAttributeVale = entTarget.Attributes[strAttributeName];
                }
                else if (entImage != null && entImage.Attributes.Contains(strAttributeName))
                {
                    otptAttributeVale = entImage.Attributes[strAttributeName];
                }
            }
            return otptAttributeVale;
        }

        public dynamic RetrieveAliasAttributeValue(Entity entTarget, string strAttributeName, string strAlias)
        {
            dynamic otptAttributeVale = null;

            if (!string.IsNullOrWhiteSpace(strAttributeName) && !string.IsNullOrWhiteSpace(strAlias) && entTarget.Attributes.Contains(strAlias + strAttributeName))
            {
                AliasedValue otptAliasedValue = null;
                otptAliasedValue = (AliasedValue)entTarget.Attributes[strAlias + strAttributeName];

                if (otptAliasedValue != null)
                {
                    otptAttributeVale = otptAliasedValue.Value;
                }
            }
            return otptAttributeVale;
        }

        public string RetrieveConfigParaValFromConfigParams(IOrganizationService orgService, string strConfigParamKey)
        {
            string strConfigParamVal = string.Empty;

            string strRetrieveConfigParamRecord = cRMFetchXML_S.GetRequiredConfigParamRecord(strConfigParamKey);

            if (!string.IsNullOrWhiteSpace(strConfigParamKey))
            {
                EntityCollection entcolConfigParams = null;
                entcolConfigParams = orgService.RetrieveMultiple(new FetchExpression(strRetrieveConfigParamRecord));

                if (entcolConfigParams != null && entcolConfigParams.Entities != null && entcolConfigParams.Entities.Count == 1)
                {
                    strConfigParamVal = GetAttributeValFromTargetEntity(entcolConfigParams.Entities[0], ConfigParamsEntityAttributeNames.ConfigParamValue);
                }
            }

            return strConfigParamVal;
        }

        public List<string> FormatConfigParaValtoListofStrings(string strConfigParamsValue)
        {
            List<string> lststrConfigParamVals = null;

            if (!string.IsNullOrWhiteSpace(strConfigParamsValue))
            {
                if (strConfigParamsValue.Contains(","))
                {
                    string[] strAryConfigParamVals = new string[] { };
                    strAryConfigParamVals = strConfigParamsValue.Split(',', (char)StringSplitOptions.RemoveEmptyEntries);
                    lststrConfigParamVals = new List<string>(strAryConfigParamVals);
                }
                else
                {
                    lststrConfigParamVals = new List<string>();
                    lststrConfigParamVals.Add(strConfigParamsValue);
                }
            }

            return lststrConfigParamVals;
        }

        public EntityCollection GetUserRoles(IOrganizationService orgService, Guid guidUserId)
        {
            EntityCollection entcolUserRoles = null;

            if (guidUserId != Guid.Empty)
            {
                // Create a QueryExpression.
                QueryExpression qryExpRoles = new QueryExpression(RoleEntityAttributeNames.EntityLogicalName);
                qryExpRoles.ColumnSet = new ColumnSet(RoleEntityAttributeNames.Name);

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

            return entcolUserRoles;
        }

        public EntityCollection RetrieveDuplicateRecrdsFromOOBDetectionRules(IOrganizationService orgService, Entity entTargetRcrd)
        {
            EntityCollection entcolDuplicateRcrds = null;
            bool checkDuplicateRcrdFound = false;
            int intPageNo = 1;

            if (entTargetRcrd != null && !string.IsNullOrWhiteSpace(entTargetRcrd.LogicalName))
            {
                entcolDuplicateRcrds = new EntityCollection();

                do
                {
                    RetrieveDuplicatesRequest rqtRetrieveDuplicateRules = new RetrieveDuplicatesRequest
                    {
                        //Entity Object to be searched with the values filled for the attributes to check
                        BusinessEntity = entTargetRcrd,
                        //Logical Name of the Entity to check Matching Entity
                        MatchingEntityName = entTargetRcrd.LogicalName,
                        PagingInfo = new PagingInfo() { PageNumber = intPageNo, Count = 10000 }
                    };

                    RetrieveDuplicatesResponse rspRetrievedDuplicateRules = (RetrieveDuplicatesResponse)orgService.Execute(rqtRetrieveDuplicateRules);

                    if (rspRetrievedDuplicateRules != null && rspRetrievedDuplicateRules.DuplicateCollection != null && rspRetrievedDuplicateRules.DuplicateCollection.Entities != null && rspRetrievedDuplicateRules.DuplicateCollection.Entities.Count > 0)
                    {
                        checkDuplicateRcrdFound = true;
                        entcolDuplicateRcrds.Entities.AddRange(rspRetrievedDuplicateRules.DuplicateCollection.Entities);

                        if (!checkDuplicateRcrdFound)
                        {
                            intPageNo++;
                        }
                    }
                }
                while (!checkDuplicateRcrdFound);
            }

            return entcolDuplicateRcrds;
        }

        public bool CheckIsStringValidXML(string strCheckXML, out XmlDocument xmlDoc)
        {
            bool xmlCheckor = false;
            xmlDoc = null;

            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(strCheckXML);
                xmlCheckor = true;
            }
            catch
            {
                xmlCheckor = false;
            }
            return xmlCheckor;
        }

        public Entity RetrieveAnonymousCustomer(IOrganizationService orgService)
        {
            Entity entAnonymousCust = null;

            string strQueryForAnonymousCust = cRMFetchXML_S.GetQueryForAnonymousCustomer();

            if (!string.IsNullOrWhiteSpace(strQueryForAnonymousCust))
            {
                EntityCollection entcolAnonymousCust = null;
                entcolAnonymousCust = orgService.RetrieveMultiple(new FetchExpression(strQueryForAnonymousCust));

                if (entcolAnonymousCust != null && entcolAnonymousCust.Entities != null && entcolAnonymousCust.Entities.Count > 0)
                {
                    entAnonymousCust = entcolAnonymousCust.Entities[0];
                }
            }

            return entAnonymousCust;
        }

        public Entity RetrieveRelatedCustomerCategory(IOrganizationService orgService, string strCategoryId)
        {
            Entity entCustCategory = null;

            string strQueryForCustCategory = cRMFetchXML_S.GetQueryForCustomerCategory(strCategoryId);

            if (!string.IsNullOrWhiteSpace(strQueryForCustCategory))
            {
                EntityCollection entcolCustomerCatgeories = null;
                entcolCustomerCatgeories = orgService.RetrieveMultiple(new FetchExpression(strQueryForCustCategory));

                if (entcolCustomerCatgeories != null && entcolCustomerCatgeories.Entities != null && entcolCustomerCatgeories.Entities.Count > 0)
                {
                    entCustCategory = entcolCustomerCatgeories.Entities[0];
                }
            }

            return entCustCategory;
        }

        public void AssignTargetRecord(IOrganizationService organizationService, EntityReference entrefUserorTeam, EntityReference entrefTargetRecord)
        {
            if (entrefTargetRecord != null && entrefTargetRecord.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefTargetRecord.LogicalName)
                && entrefUserorTeam != null && entrefUserorTeam.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefUserorTeam.LogicalName))
            {
                // Create the Request Object and Set the Request Object's Properties
                AssignRequest rqtAssign = new AssignRequest
                {
                    Assignee = entrefUserorTeam,
                    Target = entrefTargetRecord
                };

                // Execute the Request
                AssignResponse rspAssign = (AssignResponse)organizationService.Execute(rqtAssign);
            }
        }

        public void ShareTargetRcrdWithReadWriteAppendPrivileges(IOrganizationService organizationService, EntityReference entrefUserorTeam, EntityReference entrefTargetRecord)
        {
            if (entrefTargetRecord != null && entrefTargetRecord.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefTargetRecord.LogicalName)
                && entrefUserorTeam != null && entrefUserorTeam.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefUserorTeam.LogicalName))
            {
                // Create the Request Object and Set the Request Object's Properties
                GrantAccessRequest rqtShareAccess = new GrantAccessRequest
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess,
                        Principal = entrefUserorTeam
                    },
                    Target = entrefTargetRecord
                };

                // Execute the Request
                GrantAccessResponse rspShareAccess = (GrantAccessResponse)organizationService.Execute(rqtShareAccess);
            }
        }
    }
}