using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Case
{
    public class UpdateCasesInfoInTargetCustomer : CodeActivity
    {
        #region "Parameter Definition"

        [RequiredArgument]
        [Input("Target Customer")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> TargetCustomer { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objCommon = new CommonWorkFlowExtensions(exeContext);
            objCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            #region "Read Parameters"

            EntityReference entrefTargetCustomer = this.TargetCustomer.Get(exeContext);

            #endregion

            try
            {
                UpdateCustomerInfo customerInfoPresenter = new UpdateCustomerInfo();
                Entity entUpdateCustomerInfo = new Entity(entrefTargetCustomer.LogicalName, entrefTargetCustomer.Id);

                switch (objCommon.workflowContext.MessageName)
                {
                    case PluginHelperStrigs.CreateMsgName:
                        entUpdateCustomerInfo = customerInfoPresenter.SetRelatedRecentlyInteractedAgent(objCommon, entUpdateCustomerInfo);
                        entUpdateCustomerInfo = customerInfoPresenter.SetRelatedCasesCount(objCommon, entrefTargetCustomer, entUpdateCustomerInfo);
                        break;
                    case PluginHelperStrigs.UpdateMsgName:
                        entUpdateCustomerInfo = customerInfoPresenter.SetRelatedCasesCount(objCommon, entrefTargetCustomer, entUpdateCustomerInfo);
                        break;
                    case PluginHelperStrigs.DeleteMsgName:
                        entUpdateCustomerInfo = customerInfoPresenter.SetRelatedCasesCount(objCommon, entrefTargetCustomer, entUpdateCustomerInfo);
                        break;
                    case PluginHelperStrigs.ExecuteWorkflowMsgName:
                        entUpdateCustomerInfo = customerInfoPresenter.SetRelatedCasesCount(objCommon, entrefTargetCustomer, entUpdateCustomerInfo);
                        break;
                }

                if (entUpdateCustomerInfo != null && entUpdateCustomerInfo.Attributes != null && entUpdateCustomerInfo.Attributes.Count > 0)
                {
                    objCommon.srvContextUsr.Update(entUpdateCustomerInfo);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}