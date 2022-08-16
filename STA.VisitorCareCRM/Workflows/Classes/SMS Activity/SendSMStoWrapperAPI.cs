using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.SMS_Activity
{
    public class SendSMStoWrapperAPI : CodeActivity
    {
        [RequiredArgument]
        [Input("SMS Message")]
        [ReferenceTarget("")]
        public InArgument<string> SMSMessage { get; set; }

        [RequiredArgument]
        [Input("Recipient No.")]
        [ReferenceTarget("")]
        public InArgument<string> RecipientNo { get; set; }

        [RequiredArgument]
        [Output("Is SMS API Call Success")]
        [ReferenceTarget("")]
        public OutArgument<bool> IsSMSAPICallSuccess { get; set; }

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"            

            string strSMSMessage = this.SMSMessage.Get(exeContext);
            string strRecipientNo = this.RecipientNo.Get(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Recipient No.: " + strRecipientNo);
            objWorkFlowCommon.srvTracing.Trace("SMS Message: " + strSMSMessage);

            #endregion

            try
            {
                SendSMSPresenter sendSMSPresenter = new SendSMSPresenter();

                switch (objWorkFlowCommon.workflowContext.MessageName)
                {
                    case PluginHelperStrigs.CreateMsgName:
                    case PluginHelperStrigs.ExecuteWorkflowMsgName:
                        bool IsAPICallSuccess = sendSMSPresenter.CheckAndFormatSendSMSParameters(objWorkFlowCommon, strRecipientNo, strSMSMessage);
                        this.IsSMSAPICallSuccess.Set(exeContext, IsAPICallSuccess);
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