using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Resources;
using System;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class UpdateReleaseInfoOnCAH
    {
        CRMFetchXMLs cRMFetchXMLs = new CRMFetchXMLs();

        public void UpdatetheReleaseDetails_ActiveChildCAH(CommonWorkFlowExtensions cmWorkFlowObject, EntityReference entrefReleasedBy)
        {
            Entity entCaseAssignmentHistory = null;

            if (entrefReleasedBy != null && entrefReleasedBy.Id != Guid.Empty)
            {
                entCaseAssignmentHistory = RetrieveActiveCAH_ForTargetCase(cmWorkFlowObject.srvContextUsr, cmWorkFlowObject.workflowContext.PrimaryEntityId, entCaseAssignmentHistory);

                if (entCaseAssignmentHistory != null && entCaseAssignmentHistory.Id != Guid.Empty)
                {
                    entCaseAssignmentHistory.Attributes[CaseAssignmentHistoryEntityAttributeNames.ReleasedBy] = new EntityReference(entrefReleasedBy.LogicalName, entrefReleasedBy.Id);
                    entCaseAssignmentHistory.Attributes[CaseAssignmentHistoryEntityAttributeNames.ReleaseDateTime] = DateTime.UtcNow;
                    cmWorkFlowObject.srvContextUsr.Update(entCaseAssignmentHistory);

                    SetStateRequest rqtSetState = new SetStateRequest
                    {
                        State = new OptionSetValue((int)EntityStateCode.InActive),
                        Status = new OptionSetValue((int)EntityStatusCode.InActive),
                        EntityMoniker = entCaseAssignmentHistory.ToEntityReference()
                    };

                    SetStateResponse rspSetState = (SetStateResponse)cmWorkFlowObject.srvContextUsr.Execute(rqtSetState);
                }
            }
        }

        private Entity RetrieveActiveCAH_ForTargetCase(IOrganizationService orgService, Guid guidTargetCase, Entity entRqurdCAH)
        {
            string strFetchQryForCAH = string.Empty;
            strFetchQryForCAH = cRMFetchXMLs.GetRelatedCaseAssignmentHistory(guidTargetCase);

            if (!string.IsNullOrWhiteSpace(strFetchQryForCAH))
            {
                EntityCollection entcolCAHs = orgService.RetrieveMultiple(new FetchExpression(strFetchQryForCAH));

                if (entcolCAHs != null && entcolCAHs.Entities != null && entcolCAHs.Entities.Count > 0)
                {
                    entRqurdCAH = entcolCAHs.Entities[0];
                }
            }

            return entRqurdCAH;
        }
    }
}