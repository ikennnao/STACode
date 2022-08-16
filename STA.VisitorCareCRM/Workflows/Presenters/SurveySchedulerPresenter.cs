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
    public class SurveySchedulerPresenter
    {
       
        private CommonMethods commonMethods = new CommonMethods();
        DateTime nextrundate = DateTime.Now;
        public DateTime CalculateNextRunDate(CommonWorkFlowExtensions objWorkFlowCommon, int freq)
        {
            Entity scheduler = objWorkFlowCommon.srvContextUsr.Retrieve(SurveySchedulerEntityAttributeNames.EntityLogicalName, objWorkFlowCommon.workflowContext.PrimaryEntityId, new ColumnSet(true));
            //check if frequency is set
            if(scheduler!=null)
            {
               switch (freq)
                {
                    case 1: // Monthly
                        {
                            //set next run date 30 days from now
                            nextrundate = nextrundate.AddMonths(1);

                        }
                        break;
                    case 2:// Quaterly 
                        {
                            nextrundate = nextrundate.AddMonths(4);
                        }
                        break;
                    case 3: //Yearly
                        {
                            nextrundate = nextrundate.AddMonths(12);
                        }
                        break;
                    case 4: // one day
                        {
                            nextrundate = nextrundate.AddDays(1);
                        }
                        break;
                    default:
                        {
                            nextrundate = nextrundate.AddMonths(4);
                        }
                        break;


                }

                //Send Email to the Visitor Care Team
                SendMail(scheduler, objWorkFlowCommon.srvContextUsr);

                
            }
            return nextrundate;
            
        }

        private void SendMail(Entity scheduler, IOrganizationService organizationService)
        {
            
            IOrganizationService orgService = organizationService;
            string templateName = scheduler.GetAttributeValue<string>(SurveySchedulerEntityAttributeNames.TemplateName);
            Guid fromQueue = scheduler.GetAttributeValue<EntityReference>(SurveySchedulerEntityAttributeNames.SendSurveyFrom).Id;
            Guid toQueue = scheduler.GetAttributeValue<EntityReference>(SurveySchedulerEntityAttributeNames.SendSurveyTo).Id;
            Entity entity = GetGlobalTemplate(templateName,organizationService);
            Entity email = new Entity();
            email.LogicalName = "email";

            // get the Subject content from template
            email.Attributes["subject"] = GetDataFromXml(entity.Attributes["subject"].ToString(), "match");
            // get the description from template 
            email.Attributes["description"] = GetDataFromXml(entity.Attributes["body"].ToString(), "match") ;

            List<Entity> fromEntities = new List<Entity>();
            List<Entity> toEntities = new List<Entity>();
            Entity fromactivityParty = new Entity();
            fromactivityParty.LogicalName = "activityparty";
            fromactivityParty.Attributes["partyid"] = new EntityReference("queue", fromQueue);
            fromEntities.Add(fromactivityParty);

            Entity toactivityParty = new Entity();
            toactivityParty.LogicalName = "activityparty";
            toactivityParty.Attributes["partyid"] = new EntityReference("queue", toQueue);
            toEntities.Add(toactivityParty);

            email.Attributes["from"] = fromEntities.ToArray();
            email.Attributes["to"] = toEntities.ToArray();
            //email.Attributes["regardingobjectid"] = new EntityReference(SurveySchedulerEntityAttributeNames.EntityLogicalName, scheduler.Id);

            Guid emailCreated = orgService.Create(email);
            SendEmailRequest req = new SendEmailRequest();
            req.EmailId = emailCreated;
            req.TrackingToken = "";
            req.IssueSend = true;
            SendEmailResponse res = (SendEmailResponse)orgService.Execute(req);
        }

        private static string GetDataFromXml(string value, string attributeName)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            XDocument document = XDocument.Parse(value);
            // get the Element with the attribute name specified
            XElement element = document.Descendants().Where(ele => ele.Attributes().Any(attr => attr.Name == attributeName)).FirstOrDefault();
            return element == null ? string.Empty : element.Value;
        }

        public static Entity GetGlobalTemplate(string title, IOrganizationService orgService)
        {
            Entity emailTemplate = null;
            try
            {

                QueryExpression query = new QueryExpression();

                // Setup the query for the template entity
                query.EntityName = "template";

                // Return all columns
                query.ColumnSet.AllColumns = true;
                query.Criteria = new FilterExpression();
                query.Criteria.FilterOperator = LogicalOperator.And;

                // Create the title condition
                ConditionExpression condition1 = new ConditionExpression();
                condition1.AttributeName = "title";
                condition1.Operator = ConditionOperator.Equal;
                condition1.Values.Add(title);

                query.Criteria.Conditions.Add(condition1);

                // Execute the query and return the result
                EntityCollection entityColl = orgService.RetrieveMultiple(query);

                if (entityColl.Entities.Count > 0)
                {
                    emailTemplate = entityColl.Entities[0];
                }
            }
            catch
            {
                throw;
            }
            return emailTemplate;
        }


    }
}