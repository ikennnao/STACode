using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class CheckToRestrictDeactivateStep
    {
        private CommonMethods commonMethods = new CommonMethods();

        public void CheckConditionToDeactivate_OnPreOperation(IOrganizationServiceFactory serviceFactory, IPluginExecutionContext exeContext, ITracingService srvTracing, string strUnsecureConfig)
        {
            if (!string.IsNullOrWhiteSpace(strUnsecureConfig))
            {
                // Obtain the organization service reference.
                IOrganizationService srvContextUsr = serviceFactory.CreateOrganizationService(exeContext.UserId);
                IOrganizationService srvInitiatingUsr = serviceFactory.CreateOrganizationService(exeContext.InitiatingUserId);

                string strConfigParamVal = commonMethods.RetrieveConfigParaValFromConfigParams(srvContextUsr, strUnsecureConfig);
                List<string> lststrConfigParamVals = commonMethods.FormatConfigParaValtoListofStrings(strConfigParamVal);

                if (lststrConfigParamVals != null && lststrConfigParamVals.Count > 0)
                {
                    EntityCollection entcolInitiatingUsrRoles = commonMethods.GetUserRoles(srvContextUsr, exeContext.InitiatingUserId);

                    if (entcolInitiatingUsrRoles != null && entcolInitiatingUsrRoles.Entities != null && entcolInitiatingUsrRoles.Entities.Count > 0)
                    {
                        bool CheckRestrictDeactive = false;

                        foreach (string strParamVal in lststrConfigParamVals)
                        {
                            CheckRestrictDeactive = entcolInitiatingUsrRoles.Entities.Any(y => y.Attributes.Contains(RoleEntityAttributeNames.Name) && ((string)y.Attributes[RoleEntityAttributeNames.Name]).ToLower().Trim() == strParamVal.ToLower().Trim());

                            if (CheckRestrictDeactive)
                            {
                                throw new Exception("Insufficient Permissions to 'Activate/Deactivate' the record. Please contact System Admin for further assistance.");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("'Unsecure Config.' is required for this plugin to execute. Please contact System Admin for further assistance.");
            }
        }
    }
}