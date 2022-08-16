using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Presenters;
using System;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Classes.Generic
{
    public class ShareRecords : CodeActivity
    {
        [RequiredArgument]
        [Input("Team")]
        [ReferenceTarget("team")]
        public InArgument<EntityReference> Team { get; set; }


        [RequiredArgument]
        [Input("Case")]
        [ReferenceTarget("incident")]
        public InArgument<EntityReference> _Case { get; set; }

        protected override void Execute(CodeActivityContext exeContext)
        {
            #region "Load CRM Service from context"

            CommonWorkFlowExtensions objWorkFlowCommon = new CommonWorkFlowExtensions(exeContext);
            objWorkFlowCommon.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            try
            {
                EntityReference incident = _Case.Get(exeContext);
                EntityReference team = Team.Get(exeContext);

                ShareTargetRcrdWithReadWriteAppendPrivileges(objWorkFlowCommon.srvContextUsr, team, incident);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }


        public void ShareTargetRcrdWithReadWriteAppendPrivileges(IOrganizationService organizationService, EntityReference entrefUserorTeam, EntityReference entrefTargetRecord)
        {
            if (entrefTargetRecord != null && entrefTargetRecord.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefTargetRecord.LogicalName)
                && entrefUserorTeam != null && entrefUserorTeam.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefUserorTeam.LogicalName))
            {
                // Create the Request Object and Set the Request Object's Properties
                GrantAccessRequest rqtShareAccess = new GrantAccessRequest
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.ReadAccess | AccessRights.WriteAccess | AccessRights.AppendAccess,
                        Principal = entrefUserorTeam
                    },
                    Target = entrefTargetRecord
                };

                // Execute the Request
                GrantAccessResponse rspShareAccess = (GrantAccessResponse)organizationService.Execute(rqtShareAccess);
            }
        }
    }
}