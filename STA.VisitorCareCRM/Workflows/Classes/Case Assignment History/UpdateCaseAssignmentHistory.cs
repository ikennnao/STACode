using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Case_Assignment_History
{
    public class UpdateCaseAssignmentHistory : CodeActivity
    {
        [RequiredArgument]
        [Input("Released By")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> CaseReleasedBy { get; set; }

        [RequiredArgument]
        [Output("Is Case Released")]
        [ReferenceTarget("")]
        public OutArgument<bool> IsCaseReleased { get; set; }

        [RequiredArgument]
        [Output("Output Message")]
        [ReferenceTarget("")]
        public OutArgument<string> OutputMessage { get; set; }

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"

            EntityReference entrefCaseReleasedBy = this.CaseReleasedBy.Get(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Case Released By: " + entrefCaseReleasedBy.Name);

            #endregion

            try
            {
                UpdateReleaseInfoOnCAH cahInfoPresenter = new UpdateReleaseInfoOnCAH();

                switch (objWorkFlowCommon.workflowContext.MessageName)
                {
                    case PluginHelperStrigs.ReleaseCaseFunctionalityMsgName:
                        cahInfoPresenter.UpdatetheReleaseDetails_ActiveChildCAH(objWorkFlowCommon, entrefCaseReleasedBy);
                        break;
                    case PluginHelperStrigs.AssignCaseFunctionalityMsgName:
                        cahInfoPresenter.UpdatetheReleaseDetails_ActiveChildCAH(objWorkFlowCommon, entrefCaseReleasedBy);
                        break;
                }

                this.IsCaseReleased.Set(exeContext, true);
                this.OutputMessage.Set(exeContext, "Success");
            }
            catch (Exception ex)
            {
                this.IsCaseReleased.Set(exeContext, false);
                this.OutputMessage.Set(exeContext, "Error: " + ex.Message);
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}