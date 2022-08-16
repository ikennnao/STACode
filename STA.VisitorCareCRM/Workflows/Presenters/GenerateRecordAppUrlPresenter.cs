using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using System;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class GenerateRecordAppUrlPresenter
    {
        private string strVisitorCareAppUrlKey = "VisitorCareAppUrl";
        private CommonMethods commonMethods = new CommonMethods();

        public void UpdatetheAppRecordUrl(CommonWorkFlowExtensions objWorkFlowCommon)
        {
            string strTargetEntityName = string.Empty, strTargetRecordId = string.Empty;

            strTargetEntityName = objWorkFlowCommon.workflowContext.PrimaryEntityName;
            strTargetRecordId = Convert.ToString(objWorkFlowCommon.workflowContext.PrimaryEntityId);

            string strVisitorCareAppUrlVal = commonMethods.RetrieveConfigParaValFromConfigParams(objWorkFlowCommon.srvContextUsr, strVisitorCareAppUrlKey);

            if (!string.IsNullOrWhiteSpace(strVisitorCareAppUrlVal) && !string.IsNullOrWhiteSpace(strTargetEntityName) && !string.IsNullOrWhiteSpace(strTargetRecordId))
            {
                string strPageTypeETN = "&pagetype=entityrecord&etn=", strUrlId = "&id=";

                string strAppRecordUrl = strVisitorCareAppUrlVal + strPageTypeETN + strTargetEntityName.ToLower() + strUrlId + strTargetRecordId.ToLower();

                if (!string.IsNullOrWhiteSpace(strAppRecordUrl))
                {
                    Entity entUpdateTargetRcrd = new Entity(strTargetEntityName, new Guid(strTargetRecordId));
                    entUpdateTargetRcrd.Attributes["tc_apprecordurl"] = strAppRecordUrl;
                    if (strTargetEntityName == CaseEntityAttributeNames.EntityLogicalName)
                    {
                        entUpdateTargetRcrd.Attributes["tc_recordguid"] = strTargetRecordId;
                    }
                    objWorkFlowCommon.srvContextUsr.Update(entUpdateTargetRcrd);
                }
            }
        }
    }
}