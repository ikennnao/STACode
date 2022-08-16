using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Conversation
{
    public class CheckConversation : CodeActivity
    {
        [RequiredArgument]
        [Input("Contact")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> Contact { get; set; }

    
        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"            

            EntityReference contact = Contact.Get(exeContext);
            #endregion

            try
            {
                CheckConversationPresenter checkconversation = new CheckConversationPresenter();
                objWorkFlowCommon.srvTracing.Trace("Starting PerformCustomerCheck Functionality --- OK");
                checkconversation.PerformCustomerCheck(objWorkFlowCommon, contact);
                objWorkFlowCommon.srvTracing.Trace("End PerformCustomerCheck Functionality --- OK");

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}