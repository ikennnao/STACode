using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;
using System.Linq;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class RestrictModClosurePresenter
    {
        private readonly CommonMethods commonMethods = new CommonMethods();
        private readonly CRMFetchXML_s cRMFetch = new CRMFetchXML_s();
        private readonly CustomMessages customMsgs = new CustomMessages();
        private bool hasRole = false;

        public void ExecuteModClosureTask(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCaseApproval = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPreImageCaseApproval = commonMethods.RetrievePreImageEntityFromContext(objCommonForAllPlugins.pluginContext);
            string regardingObjectLogicalName = string.Empty;
            if (entTargetCaseApproval != null)
            {
                EntityReference entrefRegardingObject = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCaseApproval, entPreImageCaseApproval, CaseApprovalEntityAttributeNames.RegardingObject);
                EntityReference entrefRelatedCaseObject = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCaseApproval, entPreImageCaseApproval, CaseApprovalEntityAttributeNames.RelatedCase);
                OptionSetValue optsetRequestType = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCaseApproval, entPreImageCaseApproval, CaseApprovalEntityAttributeNames.RequestType);

                    if (entrefRegardingObject != null && !string.IsNullOrWhiteSpace(entrefRegardingObject.LogicalName)) { 
                        regardingObjectLogicalName = entrefRegardingObject.LogicalName;
                    } 

                    else if(entrefRelatedCaseObject != null && !string.IsNullOrEmpty(entrefRelatedCaseObject.LogicalName))
                    {
                        regardingObjectLogicalName = entrefRelatedCaseObject.LogicalName;
                    }


                    switch (regardingObjectLogicalName)
                    {
                        case CaseEntityAttributeNames.EntityLogicalName:
                            if (optsetRequestType != null && optsetRequestType.Value != int.MinValue && optsetRequestType.Value == (int)RequestType.MomentofDelight)
                            {
                                //Get the role Id from the configparameter - VC Quality Assurance
                                string query = cRMFetch.GetRequiredConfigParamRecord("MoDRoleName");
                                string roleName = string.Empty;
                                if (!string.IsNullOrWhiteSpace(query))
                                {
                                    EntityCollection ModRole = objCommonForAllPlugins.srvContextUsr.RetrieveMultiple(new FetchExpression(query));
                                    if (ModRole != null && ModRole.Entities != null && ModRole.Entities.Count > 0)
                                    {
                                        roleName = ModRole.Entities[0].GetAttributeValue<string>(ConfigParamsEntityAttributeNames.ConfigParamValue);
                                    }

                                    //Get the initiating users security roles and check if the user has the VC Quality Assurance role
                                    EntityCollection userRoles = commonMethods.GetUserRoles(objCommonForAllPlugins.srvContextUsr, objCommonForAllPlugins.pluginContext.InitiatingUserId);
                                    if (userRoles != null && userRoles.Entities != null && userRoles.Entities.Count > 0)
                                    {
                                       hasRole = userRoles.Entities.Any(y => y.Attributes.Contains(RoleEntityAttributeNames.Name) && ((string)y.Attributes[RoleEntityAttributeNames.Name]).ToString().ToLower().Trim() == roleName.ToLower().Trim());

                                        if (!hasRole)
                                        {
                                            throw new Exception($"{objCommonForAllPlugins.pluginContext.MessageName}  Operation terminated.You do not have permission to close this record. Please contact your System Admin for further assistance");
                                        }


                                    }
                                }



                            }

                            break;
                    }
                

            }

        }

    }
}