using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;
using STA.TouristCareCRM.Plugins.Resources;
using Microsoft.Xrm.Sdk.Query;

namespace STA.TouristCareCRM.Workflows.Classes.Generic
{
    public class UpdateLanguageLocale : CodeActivity
    {
        [RequiredArgument]
        [Input("Status")]
        public InArgument<int> Status { get; set; }

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            try
            {
                int _statecode = Status.Get(exeContext);
                

                UpdateLanguageLocaleRecords(objWorkFlowCommon.srvContextUsr, _statecode);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }


        public void UpdateLanguageLocaleRecords(IOrganizationService organizationService, int status)
        {
            EntityCollection languageLocals = GetLanguageLocale(organizationService);
            if (languageLocals != null && languageLocals.Entities.Count > 0)
            {
                //update the languages to the given status
                SetStateRequest rqtSetState = new SetStateRequest();
                foreach (var lang in languageLocals.Entities)
                {
                    if (status == 1)
                    {
                        rqtSetState.State = new OptionSetValue((int)EntityStateCode.InActive);
                        rqtSetState.Status = new OptionSetValue((int)EntityStatusCode.InActive);
                    }

                    if (status == 0)
                    {
                        rqtSetState.State = new OptionSetValue((int)EntityStateCode.Active);
                        rqtSetState.Status = new OptionSetValue((int)EntityStatusCode.Active);
                    }
                    // Point the Request to the case whose state is being changed
                    rqtSetState.EntityMoniker = lang.ToEntityReference();
                    SetStateResponse rspSetState = (SetStateResponse)(organizationService.Execute(rqtSetState));
                }
            }
            }
        


        private EntityCollection GetLanguageLocale(IOrganizationService organizationService)
        {
            string[] columns = { LanguageLocaleEntityAttributeNames.Code, LanguageLocaleEntityAttributeNames.Language, LanguageLocaleEntityAttributeNames.LocaleId, LanguageLocaleEntityAttributeNames.Name };
            QueryExpression qe = new QueryExpression(LanguageLocaleEntityAttributeNames.EntityLogicalName);
            ConditionExpression conditionExpression1 = new ConditionExpression(LanguageLocaleEntityAttributeNames.LocaleId, ConditionOperator.NotEqual, "1033");
            ConditionExpression conditionExpression2 = new ConditionExpression(LanguageLocaleEntityAttributeNames.LocaleId, ConditionOperator.NotEqual, "1025");
            ColumnSet columnSet = new ColumnSet(columns);
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(conditionExpression1);
            filter.AddCondition(conditionExpression2);
            qe.Criteria.AddFilter(filter);
            qe.ColumnSet = columnSet;
            EntityCollection entLang = organizationService.RetrieveMultiple(qe);
            if (entLang != null && entLang.Entities != null && entLang.Entities.Count > 0)
            {
                return entLang;
            }
            else
            {

                return null;
            }
        }
    }
}