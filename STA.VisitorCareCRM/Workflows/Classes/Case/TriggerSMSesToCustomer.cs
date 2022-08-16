using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;
using static STA.TouristCareCRM.Workflows.Presenters.SMSNotificationsPresenter;

namespace STA.TouristCareCRM.Workflows.Classes.Case
{
    public class TriggerSMSesToCustomer : CodeActivity
    {
        #region "Parameter Definition"        

        [RequiredArgument]
        [Input("Notification Config")]
        [ReferenceTarget("tc_notificationconfiguration")]
        public InArgument<EntityReference> NotificationConfig { get; set; }

        [RequiredArgument]
        [Input("Contact Number")]
        public InArgument<string> ContactNo { get; set; }

        [Input("Customer AttrName")]
        public InArgument<string> CustomerAttrName { get; set; }

        [Input("SMS Language AttrName")]
        public InArgument<string> SMSLanguagueAttrName { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objCommon = new CommonWorkFlowExtensions(exeContext);
            objCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion            

            try
            {
                #region "Read Parameters"

                SMSNotificationInputParams objNotificationParams = new SMSNotificationInputParams();
                objNotificationParams.entrefNotificationConfig = this.NotificationConfig.Get(exeContext);
                objNotificationParams.strContactNo = this.ContactNo.Get(exeContext);
                objNotificationParams.strCustomerAttrName = this.CustomerAttrName.Get(exeContext);
                objNotificationParams.strSMSLanguageAttrName = this.SMSLanguagueAttrName.Get(exeContext);

                if (!string.IsNullOrWhiteSpace(objCommon.workflowContext.PrimaryEntityName) && objCommon.workflowContext.PrimaryEntityId != Guid.Empty)
                {
                    objNotificationParams.entTargetRcrdDetails = new Entity(objCommon.workflowContext.PrimaryEntityName, objCommon.workflowContext.PrimaryEntityId);
                }

                #endregion                

                SMSNotificationsPresenter smsNotificationsPresenter = new SMSNotificationsPresenter();
                smsNotificationsPresenter.CreateAndSendSMSesFromTemplates(objCommon, objNotificationParams);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}