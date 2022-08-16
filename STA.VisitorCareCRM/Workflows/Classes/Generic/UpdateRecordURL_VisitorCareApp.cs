using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Generic
{
    public class UpdateRecordURL_VisitorCareApp : CodeActivity
    {
        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"            

            #endregion

            try
            {
                GenerateRecordAppUrlPresenter recordAppUrInfoPresenter = new GenerateRecordAppUrlPresenter();

                switch (objWorkFlowCommon.workflowContext.MessageName)
                {
                    case PluginHelperStrigs.CreateMsgName:
                    case PluginHelperStrigs.ExecuteWorkflowMsgName:
                        recordAppUrInfoPresenter.UpdatetheAppRecordUrl(objWorkFlowCommon);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}