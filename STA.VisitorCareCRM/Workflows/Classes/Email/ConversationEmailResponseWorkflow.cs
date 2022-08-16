using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Email
{
    public class ConversationEmailResponseWorkflow : CodeActivity
    {
      

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            try
            {
                ConversationEmailResponsePresenter emailPresenter = new ConversationEmailResponsePresenter();
                objWorkFlowCommon.srvTracing.Trace("Starting PerformCustomerCheck Functionality --- OK");
                emailPresenter.PerformAction(objWorkFlowCommon,objWorkFlowCommon.srvContextUsr);
                objWorkFlowCommon.srvTracing.Trace("End PerformCustomerCheck Functionality --- OK");

               

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}