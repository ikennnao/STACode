using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Resources;
using System;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class PostedSurveyPresenter
    {
        CRMFetchXMLs cRMFetchXMLs = new CRMFetchXMLs();
        EntityCollection surveyAnswers = null; Entity caseRef = null;
        Entity surveyQuestion; EntityReference contactRef; EntityReference accountRef; 
        int? Agent_CSAT; int? Information_CSAT; int? Outcome_CSAT; int? _NPS; int? _CES; string Comment;


        public void UpdatePostedSurveywithCustomer(CommonWorkFlowExtensions cmWorkFlowObject, EntityReference incident)
        {
            cmWorkFlowObject.srvTracing.Trace("Start Posted Survey function");
            string[] caseCol = { CaseEntityAttributeNames.Customer }; 
           
            Entity postedSurveyToUpdate = new Entity(PostedSurveyEntityAttributeNames.EntityLogicalName, cmWorkFlowObject.workflowContext.PrimaryEntityId);
            if (incident != null)
            {
                 caseRef = cmWorkFlowObject.srvContextUsr.Retrieve(CaseEntityAttributeNames.EntityLogicalName, incident.Id, new ColumnSet(caseCol));
                if (caseRef != null && caseRef.Attributes.Contains(CaseEntityAttributeNames.Customer))
                {
                    if (caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).LogicalName == ContactEntityAttributeNames.EntityLogicalName)
                    {
                        contactRef = new EntityReference(ContactEntityAttributeNames.EntityLogicalName, caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).Id);
                        postedSurveyToUpdate.Attributes[PostedSurveyEntityAttributeNames.Contact] = contactRef;
                    }
                    else if (caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).LogicalName == AccountEntityAttributeNames.EntityLogicalName)
                    {
                        accountRef = new EntityReference(AccountEntityAttributeNames.EntityLogicalName, caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).Id);
                        postedSurveyToUpdate.Attributes[PostedSurveyEntityAttributeNames.Account] = accountRef;
                    }
                }
            }

            else // Check if the posted survey contains the vist record and retrieve the case id from there
            {
                // Retrieve Posted Survey with the related visit record
                cmWorkFlowObject.srvTracing.Trace("Retreive the Posted survey --- OK");
                Entity _postedsurveyRecord = cmWorkFlowObject.srvContextUsr.Retrieve(PostedSurveyEntityAttributeNames.EntityLogicalName, cmWorkFlowObject.workflowContext.PrimaryEntityId, new ColumnSet(true));
                if (_postedsurveyRecord != null && _postedsurveyRecord.Contains(PostedSurveyEntityAttributeNames.Visit))
                {
                    cmWorkFlowObject.srvTracing.Trace("Posted survey Retrieve --- OK");
                    //get the visit enity entry page field value
                    EntityReference visitpage = _postedsurveyRecord.GetAttributeValue<EntityReference>(PostedSurveyEntityAttributeNames.Visit);
                    if (visitpage != null)
                    {
                        Entity visitPage = cmWorkFlowObject.srvContextUsr.Retrieve(VisitEntityAttributeNames.EntityLogicalName, visitpage.Id, new ColumnSet(true));
                        if (visitPage != null && visitPage.Contains(VisitEntityAttributeNames.EntryPage))
                        {
                            //get the case Id from the entrypage url

                            string caseId = visitPage.GetAttributeValue<string>(VisitEntityAttributeNames.EntryPage).Split('&')[1].Split('=')[1];
                            cmWorkFlowObject.srvTracing.Trace("caseId is " + caseId);
                            if (!string.IsNullOrEmpty(caseId))
                            {
                                //retrieve the case record
                                caseRef = cmWorkFlowObject.srvContextUsr.Retrieve(CaseEntityAttributeNames.EntityLogicalName, new Guid(caseId), new ColumnSet(caseCol));
                                if (caseRef != null)
                                {
                                    incident = caseRef.ToEntityReference();
                                    postedSurveyToUpdate.Attributes[PostedSurveyEntityAttributeNames.Case] = incident;

                                        if (caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).LogicalName == ContactEntityAttributeNames.EntityLogicalName)
                                        {
                                            contactRef = new EntityReference(ContactEntityAttributeNames.EntityLogicalName, caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).Id);
                                            postedSurveyToUpdate.Attributes[PostedSurveyEntityAttributeNames.Contact] = contactRef;
                                        }
                                        else if (caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).LogicalName == AccountEntityAttributeNames.EntityLogicalName)
                                        {
                                            accountRef = new EntityReference(AccountEntityAttributeNames.EntityLogicalName, caseRef.GetAttributeValue<EntityReference>(CaseEntityAttributeNames.Customer).Id);
                                            postedSurveyToUpdate.Attributes[PostedSurveyEntityAttributeNames.Account] = accountRef;
                                        }
                                    
                                }
                            }

                        }
                    }
                }
            }
            cmWorkFlowObject.srvTracing.Trace("Update Posted Survey...");
            cmWorkFlowObject.srvContextUsr.Update(postedSurveyToUpdate);
            UpdateSurveyAnswersCustomerandCase(cmWorkFlowObject, incident, contactRef, accountRef);
            cmWorkFlowObject.srvTracing.Trace("End Posted Survey function");
            
        }


        public void UpdateSurveyAnswersCustomerandCase(CommonWorkFlowExtensions cmWorkFlowObject, EntityReference incident, EntityReference contact, EntityReference account)
        {
            surveyAnswers = RetrieveSurveyAnswers(cmWorkFlowObject.srvContextUsr, cmWorkFlowObject.workflowContext.PrimaryEntityId);
            foreach(Entity surveyAnswer in surveyAnswers.Entities)
            {
                if (incident != null && incident.Id != null)
                    surveyAnswer.Attributes[SurveyAnswersEntityAttributeNames.Case] = new EntityReference(CaseEntityAttributeNames.EntityLogicalName, incident.Id);
                if (contact != null && contact.Id != null)
                surveyAnswer.Attributes[SurveyAnswersEntityAttributeNames.Contact] = new EntityReference(ContactEntityAttributeNames.EntityLogicalName, contact.Id);
                if (account != null && account.Id != null)
                    surveyAnswer.Attributes[SurveyAnswersEntityAttributeNames.Account] = new EntityReference(AccountEntityAttributeNames.EntityLogicalName, account.Id);

                if (surveyAnswer.GetAttributeValue<EntityReference>(SurveyAnswersEntityAttributeNames.SurveyQuestionId) != null)
                {
                    Guid surveyQuestionId = surveyAnswer.GetAttributeValue<EntityReference>(SurveyAnswersEntityAttributeNames.SurveyQuestionId).Id;
                    cmWorkFlowObject.srvTracing.Trace("Get Survey Question --- OK");
                    surveyQuestion = cmWorkFlowObject.srvContextUsr.Retrieve(SurveyQuestionEntityAttributeNames.EntityLogicalName, surveyQuestionId, new ColumnSet(true));
                    cmWorkFlowObject.srvTracing.Trace("End Survey Question --- OK");
                   
                    int questionRatingType = surveyQuestion.GetAttributeValue<OptionSetValue>(SurveyQuestionEntityAttributeNames.RatingsType)!=null? surveyQuestion.GetAttributeValue<OptionSetValue>(SurveyQuestionEntityAttributeNames.RatingsType).Value:0;
                    //Check what type of rating does the question have and assign the appropriate value
                    switch (questionRatingType)
                    {
                        case (int)RatingsType.CSAT_Agent:
                            {
                                Agent_CSAT = Convert.ToInt32(surveyAnswer.GetAttributeValue<string>(SurveyAnswersEntityAttributeNames.Value));
                                cmWorkFlowObject.srvTracing.Trace("Agent_CSAT is "+ Agent_CSAT);
                            }
                            break;

                        case (int)RatingsType.CSAT_Information:
                            {
                                Information_CSAT = Convert.ToInt32(surveyAnswer.GetAttributeValue<string>(SurveyAnswersEntityAttributeNames.Value));
                                cmWorkFlowObject.srvTracing.Trace("Information_CSAT is " + Information_CSAT);
                            }
                            break;
                        case (int)RatingsType.CSAT_Outcome:
                            {
                                Outcome_CSAT = Convert.ToInt32(surveyAnswer.GetAttributeValue<string>(SurveyAnswersEntityAttributeNames.Value));
                                cmWorkFlowObject.srvTracing.Trace("Outcome_CSAT is " + Outcome_CSAT);
                            }
                            break;
                        case (int)RatingsType.NPS:
                            {
                                _NPS = Convert.ToInt32(surveyAnswer.GetAttributeValue<string>(SurveyAnswersEntityAttributeNames.Value));
                                cmWorkFlowObject.srvTracing.Trace("_NPS is " + _NPS);
                            }
                            break;
                        case (int)RatingsType.CES:
                            {
                                _CES = Convert.ToInt32(surveyAnswer.GetAttributeValue<string>(SurveyAnswersEntityAttributeNames.Value));
                                cmWorkFlowObject.srvTracing.Trace("_CES is " + _CES);
                            }
                            break;

                        //default:
                        //    {
                        //        Comment = surveyAnswer.GetAttributeValue<string>(SurveyAnswersEntityAttributeNames.Value);
                        //    }
                        //    break;
                    }
                    
                }
                //update surveyanswer
                cmWorkFlowObject.srvContextUsr.Update(surveyAnswer);
            }
            cmWorkFlowObject.srvTracing.Trace("Starting Customer Update Functionality --- OK");
            //Get the related contact and update the ratings on the customer profile
            if (contact != null && contact.Id != null)
            {
                Entity _contactToUpdate = new Entity(ContactEntityAttributeNames.EntityLogicalName, contact.Id);
                if (Agent_CSAT.HasValue)
                    _contactToUpdate.Attributes[ContactEntityAttributeNames.CurrentCSATAgent] = Agent_CSAT;
                if (Information_CSAT.HasValue)
                    _contactToUpdate.Attributes[ContactEntityAttributeNames.CurrentCSATInformation] = Information_CSAT;
                if (Outcome_CSAT.HasValue)
                    _contactToUpdate.Attributes[ContactEntityAttributeNames.CurrentCSATOutcome] = Outcome_CSAT;
                if (_NPS.HasValue)
                    _contactToUpdate.Attributes[ContactEntityAttributeNames.CurrentNPSScore] = _NPS;
                if (_CES.HasValue)
                    _contactToUpdate.Attributes[ContactEntityAttributeNames.CurrentCESScore] = _CES;

                //update contact
                cmWorkFlowObject.srvContextUsr.Update(_contactToUpdate);
                cmWorkFlowObject.srvTracing.Trace("End Customer Update Functionality --- OK");
            }

            //Create survey ratings record and associate with case and customer records
            Entity surveyRatings = new Entity(SurveyRatingsEntityAttributeNames.EntityLogicalName);
            
            if (incident!=null && incident.Id != null)
            {
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.Case] = new EntityReference(CaseEntityAttributeNames.EntityLogicalName, incident.Id);
            }
           
            if(contact!=null && contact.Id !=null)
            {
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.Contact] = new EntityReference(ContactEntityAttributeNames.EntityLogicalName, contact.Id);
            }
            if (account != null && account.Id != null)
            {
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.Contact] = new EntityReference(ContactEntityAttributeNames.EntityLogicalName, contact.Id);
            }
            EntityReference postedSurveyRef = new EntityReference(PostedSurveyEntityAttributeNames.EntityLogicalName, cmWorkFlowObject.workflowContext.PrimaryEntityId);
            surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.PostedSurvey] = postedSurveyRef;
            if (Agent_CSAT.HasValue)
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.CSAT_Agent] = Agent_CSAT;
            if (Information_CSAT.HasValue)
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.CSAT_Information] = Information_CSAT;
            if (Outcome_CSAT.HasValue)
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.CSAT_Outcome] = Outcome_CSAT;
            if (_NPS.HasValue)
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.NPS] = _NPS;
            if (_CES.HasValue)
                surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.CES] = _CES;
            Entity _postedSurvey = cmWorkFlowObject.srvContextUsr.Retrieve(PostedSurveyEntityAttributeNames.EntityLogicalName, cmWorkFlowObject.workflowContext.PrimaryEntityId, new ColumnSet(new string[] { PostedSurveyEntityAttributeNames.Name }));
            surveyRatings.Attributes[SurveyRatingsEntityAttributeNames.Name] = "Survey Ratings for " + _postedSurvey.GetAttributeValue<string>(PostedSurveyEntityAttributeNames.Name);
            //create the survey ratings record
            cmWorkFlowObject.srvContextUsr.Create(surveyRatings);
        }


        private EntityCollection RetrieveSurveyAnswers(IOrganizationService orgService, Guid guidTargetPostedSurvey)
        {
            string strFetchQryForPostedSurvey = string.Empty;
            strFetchQryForPostedSurvey = cRMFetchXMLs.GetRelatedSurveyAnswers(guidTargetPostedSurvey);

            if (!string.IsNullOrWhiteSpace(strFetchQryForPostedSurvey))
            {
                EntityCollection entcolsurveyanswers = orgService.RetrieveMultiple(new FetchExpression(strFetchQryForPostedSurvey));

                if (entcolsurveyanswers != null && entcolsurveyanswers.Entities != null && entcolsurveyanswers.Entities.Count > 0)
                {
                    surveyAnswers = entcolsurveyanswers;
                }
            }

            return surveyAnswers;
        }




    }
}