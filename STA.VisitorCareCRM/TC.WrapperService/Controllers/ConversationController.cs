using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Interfaces;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/Conversation")]
    public class ConversationController : ApiController
    {
        private readonly ILogging logger = Instance;
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForConversation custErrorMsgsForConvst = new CustomErrorMsgsForConversation();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Conversation Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/Conversation/PostCreateConversation
        //Create New Conversation 
        [Route("PostCreateConversation")]
        public ResponseClass PostCreateConversation(CreateConversationRequest rqtCreateConversation)
        {
            exceptionResponseObjects.strAPIServiceName = "PostCreateConversation";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtCreateConversation.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        ConversationResponse createdConversationInfo = CreateConversation(guidLoggedInUser, rqtCreateConversation.createConversation, (int)APIOperationType.Create);
                        otptResponse = apiCMs.SetSuccessResponse(createdConversationInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + ConversationEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private ConversationResponse CreateConversation(Guid guidLoggedInUser, CreateConversation conversationDetails, int OperationType)
        {
            ConversationResponse rspConversation = new ConversationResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                if (OperationType != int.MinValue)
                {
                    Entity entConversation = new Entity(ConversationEntityAttributesNames.EntityLogicalName);
                    int intChannelOrigin = int.MinValue;

                    switch (OperationType)
                    {
                        case 1: //Create
                            if (!string.IsNullOrWhiteSpace(conversationDetails.IncomingMessage) && !string.IsNullOrWhiteSpace(conversationDetails.ChannelOrigin))
                            {
                                Entity entRequiredChannelOrigin = crmCMs.RetrieveChannelOriginFromOriginCode(crmAdapter.CRMAdminService(), conversationDetails.ChannelOrigin);

                                if (entRequiredChannelOrigin != null)
                                {
                                    intChannelOrigin = apiCMs.FormatChannelOriginFromStringToInt(conversationDetails.ChannelOrigin);

                                    if (!string.IsNullOrWhiteSpace(conversationDetails.CaseRefNo))
                                    {
                                        Entity entRelatedCase = crmCMs.RetrieveCaseFromCaseRefNo(crmAdapter.CRMAdminService(), conversationDetails.CaseRefNo);

                                        if (entRelatedCase != null)
                                        {
                                            entConversation.Attributes[ConversationEntityAttributesNames.ExistingCase] = new EntityReference(entRelatedCase.LogicalName, entRelatedCase.Id);
                                        }
                                    }

                                    entConversation.Attributes[ConversationEntityAttributesNames.ChannelOrigin] = entRequiredChannelOrigin.ToEntityReference();
                                    entConversation.Attributes[ConversationEntityAttributesNames.Subject] = conversationDetails.Subject;
                                    entConversation.Attributes[ConversationEntityAttributesNames.FirstName] = conversationDetails.FirstName;
                                    entConversation.Attributes[ConversationEntityAttributesNames.LastName] = conversationDetails.LastName;
                                    entConversation.Attributes[ConversationEntityAttributesNames.EmailAddress] = conversationDetails.EmailAddress;
                                    entConversation.Attributes[ConversationEntityAttributesNames.MobileNumber] = conversationDetails.MobileNumber;
                                    entConversation.Attributes[ConversationEntityAttributesNames.IncomingMessage] = conversationDetails.IncomingMessage;
                                    if (conversationDetails.CaseType != null && conversationDetails.CaseType.Id != Guid.Empty)
                                    {
                                        entConversation.Attributes[ConversationEntityAttributesNames.CaseType] = new EntityReference(CaseTypeEntityAttributeNames.EntityLogicalName, conversationDetails.CaseType.Id);
                                    }
                                    if (conversationDetails.Customer != null && conversationDetails.Customer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(conversationDetails.Customer.LogicalName))
                                    {
                                        entConversation.Attributes[ConversationEntityAttributesNames.Customer] = new EntityReference(conversationDetails.Customer.LogicalName, conversationDetails.Customer.Id);
                                    }
                                }

                                switch (intChannelOrigin)
                                {
                                    case (int)ChannelOrigin.WhatsApp:
                                        if (string.IsNullOrWhiteSpace(conversationDetails.MobileNumber))
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingMobileNumber;
                                        }
                                        if (string.IsNullOrWhiteSpace(conversationDetails.CaseRefNo))
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingCaseRefNo;
                                        }
                                        break;
                                    case (int)ChannelOrigin.LiveChat:
                                    case (int)ChannelOrigin.VideoCall:
                                    case (int)ChannelOrigin.ChatBot:
                                    case (int)ChannelOrigin.Facebook:
                                    case (int)ChannelOrigin.Instagram:
                                    case (int)ChannelOrigin.Twitter:
                                    case (int)ChannelOrigin.Youtube:
                                        if (string.IsNullOrWhiteSpace(conversationDetails.EmailAddress))
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingEmailAddress;
                                        }
                                        if (string.IsNullOrWhiteSpace(conversationDetails.CaseRefNo))
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingCaseRefNo;
                                        }
                                        break;
                                    case (int)ChannelOrigin.STAWeb:
                                        {
                                            if (string.IsNullOrWhiteSpace(conversationDetails.EmailAddress))
                                            {
                                                strExceptionMsg += custErrorMsgsForConvst.missingEmailAddress;
                                            }
                                            if (string.IsNullOrWhiteSpace(conversationDetails.MobileNumber))
                                            {
                                                strExceptionMsg += custErrorMsgsForConvst.missingMobileNumber;
                                            }
                                        }
                                        break;
                                    case (int)ChannelOrigin.HelpVisitSaudi:
                                    default:
                                        if (string.IsNullOrWhiteSpace(conversationDetails.EmailAddress))
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingEmailAddress;
                                        }
                                        if (string.IsNullOrWhiteSpace(conversationDetails.MobileNumber))
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingMobileNumber;
                                        }
                                        if (conversationDetails.CaseType == null || conversationDetails.CaseType.Id == Guid.Empty)
                                        {
                                            strExceptionMsg += custErrorMsgsForConvst.missingCaseType;
                                        }
                                        break;
                                    case int.MinValue:
                                        strExceptionMsg += custErrorMsgsForConvst.inValidChannelOriginCode;
                                        break;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(conversationDetails.IncomingMessage))
                                {
                                    strExceptionMsg += custErrorMsgsForConvst.missingIncomingMsg;
                                }
                                if (string.IsNullOrWhiteSpace(conversationDetails.ChannelOrigin))
                                {
                                    strExceptionMsg += custErrorMsgsForConvst.missingChannelOrigin;
                                }
                            }

                            if (string.IsNullOrWhiteSpace(strExceptionMsg))
                            {
                                entConversation.Id = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Create(entConversation);
                            }
                            else if (!string.IsNullOrWhiteSpace(strExceptionMsg))
                            {
                                throw new Exception(strExceptionMsg);
                            }
                            break;
                        case 2: //Update                            
                            break;
                    }

                    if (entConversation.Id != Guid.Empty)
                    {
                        rspConversation.ConversationGuid = entConversation.Id;
                        rspConversation.EntityLogicalName = ConversationEntityAttributesNames.EntityLogicalName;

                        Entity entRetrieveConversation = null;
                        entRetrieveConversation = crmAdapter.CRMAdminService().Retrieve(entConversation.LogicalName, entConversation.Id, new ColumnSet(ConversationEntityAttributesNames.Subject, ConversationEntityAttributesNames.ConversationID));

                        if (entRetrieveConversation != null)
                        {
                            string strConversationSubject = crmCMs.GetAttributeValFromTargetEntity(entRetrieveConversation, ConversationEntityAttributesNames.Subject);
                            rspConversation.Subject = strConversationSubject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspConversation;
        }
    }
}