using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Case
{
    public class TriggerEmailsToCustomer : CodeActivity
    {
        #region "Parameter Definition"        

        [RequiredArgument]
        [Input("Notification Config")]
        [ReferenceTarget("tc_notificationconfiguration")]
        public InArgument<EntityReference> NotificationConfig { get; set; }

        [RequiredArgument]
        [Input("Email Address")]
        public InArgument<string> EmailAddress { get; set; }

        [RequiredArgument]
        [Input("Customer AttrName")]
        public InArgument<string> CustomerAttrName { get; set; }

        [Input("Email Language AttrName")]
        public InArgument<string> EmailLanguagueAttrName { get; set; }

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

                EmailNotificationInputParams objNotificationParams = new EmailNotificationInputParams();
                objNotificationParams.entrefNotificationConfig = this.NotificationConfig.Get(exeContext);
                objNotificationParams.strEmailAddress = this.EmailAddress.Get(exeContext);
                objNotificationParams.strCustomerAttrName = this.CustomerAttrName.Get(exeContext);
                objNotificationParams.strEmailLanguageAttrName = this.EmailLanguagueAttrName.Get(exeContext);

                if (!string.IsNullOrWhiteSpace(objCommon.workflowContext.PrimaryEntityName) && objCommon.workflowContext.PrimaryEntityId != Guid.Empty)
                {
                    objNotificationParams.entTargetRcrdDetails = new Entity(objCommon.workflowContext.PrimaryEntityName, objCommon.workflowContext.PrimaryEntityId);
                }

                #endregion                

                EmailNotificationsPresenter emailNotificationsPresenter = new EmailNotificationsPresenter();
                emailNotificationsPresenter.CreateAndSendEmailsFromTemplates(objCommon, objNotificationParams);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}