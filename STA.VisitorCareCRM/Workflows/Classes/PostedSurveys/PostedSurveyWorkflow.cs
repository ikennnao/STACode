using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.PostedSurveys
{
    public class PostedSurveyWorkflow : CodeActivity
    {
        [RequiredArgument]
        [Input("Case")]
        [ReferenceTarget("incident")]
        public InArgument<EntityReference> Case { get; set; }

    
        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"            

            EntityReference incident = Case.Get(exeContext);
            #endregion

            try
            {
                PostedSurveyPresenter postedSurvey = new PostedSurveyPresenter();
                objWorkFlowCommon.srvTracing.Trace("Starting Update Posted Survey Functionality --- OK");
                postedSurvey.UpdatePostedSurveywithCustomer(objWorkFlowCommon, incident);
                objWorkFlowCommon.srvTracing.Trace("End Update Posted Survey Functionality --- OK");

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}