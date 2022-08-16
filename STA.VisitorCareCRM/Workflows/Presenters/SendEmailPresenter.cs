using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using Microsoft.Xrm.Sdk.Query;
using System;
using Microsoft.Crm.Sdk.Messages;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class SendEmailPresenter
    {

        private CommonMethods commonMethods = new CommonMethods();

        public void SendMail(CommonWorkFlowExtensions objWorkFlowCommon, IOrganizationService organizationService)
        {
            IOrganizationService orgService = organizationService;
            // Retreive the email record
            var email = objWorkFlowCommon.srvContextUsr.Retrieve(EmailEntityAttributeNames.EntityLogicalName, objWorkFlowCommon.workflowContext.PrimaryEntityId, new ColumnSet(true));
            //List<Entity> fromEntities = new List<Entity>();
            //List<Entity> toEntities = new List<Entity>();
            //Entity fromactivityParty = new Entity();
            //fromactivityParty.LogicalName = "activityparty";
            //fromactivityParty.Attributes["partyid"] = new EntityReference("queue", fromQueue);
            //fromEntities.Add(fromactivityParty);

            //Entity toactivityParty = new Entity();
            //toactivityParty.LogicalName = "activityparty";
            //toactivityParty.Attributes["partyid"] = new EntityReference("queue", toQueue);
            //toEntities.Add(toactivityParty);

            //email.Attributes["from"] = fromEntities.ToArray();
            //email.Attributes["to"] = toEntities.ToArray();
            //email.Attributes["regardingobjectid"] = new EntityReference(SurveySchedulerEntityAttributeNames.EntityLogicalName, scheduler.Id);
            SendEmailRequest req = new SendEmailRequest();
            req.EmailId = email.Id;
            req.TrackingToken = "";
            req.IssueSend = true;
            SendEmailResponse res = (SendEmailResponse)orgService.Execute(req);
            
        }


    }
}