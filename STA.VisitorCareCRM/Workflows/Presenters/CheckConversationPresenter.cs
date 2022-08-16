using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using Microsoft.Xrm.Sdk.Query;
using System;
using Microsoft.Crm.Sdk.Messages;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class CheckConversationPresenter
    {
       
        private CommonMethods commonMethods = new CommonMethods();

        public void PerformCustomerCheck(CommonWorkFlowExtensions objWorkFlowCommon, EntityReference contact)
        {
            //check if conversation has a contact record assocaited with it
            if(contact!=null)
            {
                //check if the contact was created by the system user
                string[] cols = { ContactEntityAttributeNames.CreatedBy };
                Entity _contact = objWorkFlowCommon.srvContextUsr.Retrieve(ContactEntityAttributeNames.EntityLogicalName, contact.Id, new ColumnSet(cols));
                objWorkFlowCommon.srvTracing.Trace("Conatact Retreived. Contact GUID " + _contact.Id );
                if (_contact!=null && _contact.Contains(ContactEntityAttributeNames.CreatedBy))
                {
                    //Retrieve the created by user and check if the user name is system
                    EntityReference _createdby = _contact.GetAttributeValue<EntityReference>(ContactEntityAttributeNames.CreatedBy);
                    objWorkFlowCommon.srvTracing.Trace("Created By " + _createdby.Name);
                    if (_createdby != null && _createdby.Name.StartsWith("SYSTEM"))
                    {
                        Entity anonymous = commonMethods.RetrieveAnonymousCustomer(objWorkFlowCommon.srvContextUsr);
                        //retrieve the target entity record
                        Entity _conversation = objWorkFlowCommon.srvContextUsr.Retrieve(ConversationEntityAttributeNames.EntityLogicalName, objWorkFlowCommon.workflowContext.PrimaryEntityId, new ColumnSet(true));
                        if (anonymous != null)
                        {
                            //update the conversation record with the anonymous customer 
                            _conversation.Attributes[ConversationEntityAttributeNames.Customer] = new EntityReference(ContactEntityAttributeNames.EntityLogicalName, anonymous.Id);
                            _conversation.Attributes[ConversationEntityAttributeNames.CustomerCategory] = commonMethods.GetAttributeValFromTargetEntity(anonymous, ContactEntityAttributeNames.CustomerCategory);
                            objWorkFlowCommon.srvTracing.Trace("Update Conversation Record ---Started ");
                            objWorkFlowCommon.srvContextUsr.Update(_conversation);
                            objWorkFlowCommon.srvTracing.Trace("Update Conversation Record ---Finished ");
                            // deactivate the contact 
                            // Create the Request Object
                            SetStateRequest rqtSetState = new SetStateRequest()
                            {
                                // Set the Request Object's Properties
                                State = new OptionSetValue((int)EntityStateCode.InActive),
                                Status = new OptionSetValue((int)EntityStatusCode.InActive),


                                // Point the Request to the case whose state is being changed
                                EntityMoniker = _contact.ToEntityReference()
                            };

                            // Execute the Request
                            objWorkFlowCommon.srvTracing.Trace("Contact Record Deactivation---- Started ");
                            SetStateResponse rspSetState = (SetStateResponse)(objWorkFlowCommon.srvContextUsr.Execute(rqtSetState));
                            objWorkFlowCommon.srvTracing.Trace("Contact Record Deactivation---- Finished ");
                            
                        }

                    }
                }
            }
        }

        
    }
}