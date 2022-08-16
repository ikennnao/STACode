using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using Microsoft.Xrm.Sdk.Query;
using System;
using Microsoft.Crm.Sdk.Messages;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using STA.TouristCareCRM.Workflows.Resources;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class ConversationEmailResponsePresenter
    {

        private CommonMethods commonMethods = new CommonMethods();
        CRMFetchXMLs cRMFetchXMLs = new CRMFetchXMLs(); Entity _channelOrigin = null;

        public void PerformAction(CommonWorkFlowExtensions objWorkFlowCommon, IOrganizationService organizationService)
        {
            IOrganizationService orgService = organizationService;
            // Retreive the email record
            var email = objWorkFlowCommon.srvContextUsr.Retrieve(EmailEntityAttributeNames.EntityLogicalName, objWorkFlowCommon.workflowContext.PrimaryEntityId, new ColumnSet(true));
            //check if the email record is regarding a case
            var regardingObject = email.GetAttributeValue<EntityReference>(EmailEntityAttributeNames.RegardingObject);
            if (regardingObject != null && regardingObject.LogicalName == CaseEntityAttributeNames.EntityLogicalName)
            {
                //create a new conversation record and associate the email to the conversation
                Entity conversation = new Entity(ConversationEntityAttributeNames.EntityLogicalName);
                
                EntityCollection fromCollection = email.GetAttributeValue<EntityCollection>(EmailEntityAttributeNames.From);

                if (fromCollection != null && fromCollection.Entities.Count > 0)
                {
                    Entity sender = fromCollection[0];
                    string sendersEmail = sender.GetAttributeValue<string>("addressused");
                    conversation.Attributes[ConversationEntityAttributeNames.EmailAddress] = sendersEmail;
                }

                conversation.Attributes[ConversationEntityAttributeNames.RegardingEmail] = new EntityReference(EmailEntityAttributeNames.EntityLogicalName, email.Id);
               _channelOrigin = RetrieveChannelOrigin(objWorkFlowCommon.srvContextUsr, "COC-002");
                if (_channelOrigin != null)
                {
                    conversation.Attributes[ConversationEntityAttributeNames.OriginChannel] = new EntityReference(ChannelofOriginAttributeNames.EntityLogicalName, _channelOrigin.Id);
                }
                //get the regarding case Ticket number
                string[] col = { CaseEntityAttributeNames.CaseRefNo};

                string ticketnum = objWorkFlowCommon.srvContextUsr.Retrieve(CaseEntityAttributeNames.EntityLogicalName, regardingObject.Id, new ColumnSet(col)).GetAttributeValue<string>(CaseEntityAttributeNames.CaseRefNo);
                conversation.Attributes[ConversationEntityAttributeNames.Subject] = email.GetAttributeValue<string>(EmailEntityAttributeNames.Subject) + " [ "+ticketnum+" ]";
                objWorkFlowCommon.srvContextUsr.Create(conversation);

            }


        }

        private Entity RetrieveChannelOrigin(IOrganizationService orgService, string channelCode)
        {
            string strFetchQryForChannelCode = string.Empty;
            strFetchQryForChannelCode = cRMFetchXMLs.GetOriginChannel(channelCode);

            if (!string.IsNullOrWhiteSpace(strFetchQryForChannelCode))
            {
                EntityCollection entcolsurveyanswers = orgService.RetrieveMultiple(new FetchExpression(strFetchQryForChannelCode));

                if (entcolsurveyanswers != null && entcolsurveyanswers.Entities != null && entcolsurveyanswers.Entities.Count > 0)
                {
                    _channelOrigin = entcolsurveyanswers[0];
                }
            }

            return _channelOrigin;
        }
    }
}