using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.SurveyScheduler
{
    public class SurveyScheduler : CodeActivity
    {
        [Input("Frequency")]
        [AttributeTarget(SurveySchedulerEntityAttributeNames.EntityLogicalName, SurveySchedulerEntityAttributeNames.Frequency)]
        public InArgument<OptionSetValue> Frequency { get; set; }
        [Output("Next Run Date")]
        public OutArgument<DateTime> NextRunDate { get; set; }

        [Output("Last Run Date")]
        public OutArgument<DateTime> LastRunDate { get; set; }


        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"            

            int frequency = Frequency.Get(exeContext).Value;
            #endregion

            try
            {
                SurveySchedulerPresenter scheduler = new SurveySchedulerPresenter();
                objWorkFlowCommon.srvTracing.Trace("Starting PerformCustomerCheck Functionality --- OK");
                var _nextrunddate = scheduler.CalculateNextRunDate(objWorkFlowCommon, frequency);
                objWorkFlowCommon.srvTracing.Trace("End PerformCustomerCheck Functionality --- OK");

                //set Outputs
                LastRunDate.Set(exeContext, DateTime.Now);
                NextRunDate.Set(exeContext, _nextrunddate);

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}