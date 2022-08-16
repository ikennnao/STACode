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
    public class SendEmailWorkflow : CodeActivity
    {
      

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            try
            {
                SendEmailPresenter emailPresenter = new SendEmailPresenter();
                objWorkFlowCommon.srvTracing.Trace("Starting PerformCustomerCheck Functionality --- OK");
                emailPresenter.SendMail(objWorkFlowCommon,objWorkFlowCommon.srvContextUsr);
                objWorkFlowCommon.srvTracing.Trace("End PerformCustomerCheck Functionality --- OK");

               

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}