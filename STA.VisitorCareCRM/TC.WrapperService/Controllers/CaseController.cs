using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/Case")]
    public class CaseController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s cRMFetchXML_S = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForCase customErrorMsgsForCase = new CustomErrorMsgsForCase();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Case Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/Case/PostRetrieveCase
        // Start adding Comments
        [Route("PostRetrieveCase")]
        public ResponseClass PostRetrieveCase(RetrieveCaseRequest rqtRetrieveCase)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveCase";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeaders = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeaders);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeaders);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtRetrieveCase.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCaseResponse retrievedCaseInfo = CaseRetrieveMtd(rqtRetrieveCase.retrieveCase);
                        otptResponse = apiCMs.SetSuccessResponse(retrievedCaseInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Case";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }


        // POST: API/Case/PostCreateCase
        // Start adding Comments
        [Route("PostCreateCase")]
        public ResponseClass PostCreateCase(CreateCaseRequest rqtCreateCase)
        {
            exceptionResponseObjects.strAPIServiceName = "PostCreateCase";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeaders = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeaders);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeaders);

                #endregion

                #region If, Request Header is Validate

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtCreateCase.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        CreateCaseResponse createdCaseInfo = CaseCreateMtd(guidLoggedInUser, rqtCreateCase.createCase);
                        otptResponse = apiCMs.SetSuccessResponse(createdCaseInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Case";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveCaseResponse CaseRetrieveMtd(RetrieveCase caseDetails)
        {
            RetrieveCaseResponse rspRetrieveCase = null;
            string strExceptionMsg = string.Empty;

            try
            {
                if (caseDetails != null)
                {
                    string strTargetCaseData = cRMFetchXML_S.GetQueryForTargetCase(caseDetails);

                    if (!string.IsNullOrWhiteSpace(strTargetCaseData))
                    {
                        EntityCollection entcolCases = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strTargetCaseData));

                        if (entcolCases != null && entcolCases.Entities != null && entcolCases.Entities.Count > 0)
                        {
                            rspRetrieveCase = new RetrieveCaseResponse();
                            rspRetrieveCase.lstCaseRecords = new List<CaseResponseObj>() { };

                            foreach (Entity entTargetCase in entcolCases.Entities)
                            {
                                CaseResponseObj rspCaseObj = new CaseResponseObj();
                                rspCaseObj.CaseRefNo = crmCMs.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.CaseRefNo);
                                rspCaseObj.ChannelRefNo = crmCMs.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.ChannelRefNo);
                                rspCaseObj.Customer = apiCMs.SetCustomFormatEntRefValue(entTargetCase, CaseEntityAttributeNames.Customer);
                                rspCaseObj.CaseType = apiCMs.SetCustomFormatEntRefValue(entTargetCase, CaseEntityAttributeNames.CaseType);
                                rspCaseObj.Category = apiCMs.SetCustomFormatEntRefValue(entTargetCase, CaseEntityAttributeNames.Category);
                                rspCaseObj.SubCategory = apiCMs.SetCustomFormatEntRefValue(entTargetCase, CaseEntityAttributeNames.SubCatgeory);
                                rspCaseObj.CaseOriginChannel = apiCMs.SetCustomFormatEntRefValue(entTargetCase, CaseEntityAttributeNames.ChannelOrigin);
                                rspCaseObj.CasePriority = apiCMs.SetCustomFormatOptionsetValue(entTargetCase, CaseEntityAttributeNames.PriorityCode);
                                rspCaseObj.CaseDescription = crmCMs.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.Description);
                                rspCaseObj.CaseGuid = crmCMs.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.CaseId);
                                rspCaseObj.AppRecordUrl = crmCMs.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.AppRecordUrl);
                                rspCaseObj.Status = apiCMs.SetCustomFormatOptionsetValue(entTargetCase, CaseEntityAttributeNames.StateCode);
                                rspCaseObj.CreatedOn = apiCMs.FormatDateTimeAttrValToUTCDateTime(entTargetCase, CaseEntityAttributeNames.CreatedOn);
                                rspCaseObj.CreatedBy = apiCMs.SetCustomFormatEntRefValue(entTargetCase, CaseEntityAttributeNames.CreatedBy);
                                rspCaseObj.ResolutionComments = crmCMs.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.ResolutionComments);

                                rspRetrieveCase.lstCaseRecords.Add(rspCaseObj);
                            }

                            rspRetrieveCase.caseETN = entcolCases.EntityName;
                        }
                    }
                }
                else
                {
                    if (caseDetails == null)
                    {
                        strExceptionMsg += customErrorMsgsForCase.missingCaseDetails;
                    }
                    throw new Exception(strExceptionMsg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return rspRetrieveCase;
        }

        private CreateCaseResponse CaseCreateMtd(Guid guidLoggedInUser, CreateCaseObj caseDetails)
        {
            CreateCaseResponse rspCaseDetails = new CreateCaseResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Guid guidCUCase = Guid.Empty;
                Entity entCreateCase = null, entRequiredChannelOrigin = null, entSocialHandle = null;
                EntityReference entrefRelatedCustomer = null;

                if (caseDetails != null)
                {
                    entCreateCase = new Entity(CaseEntityAttributeNames.EntityLogicalName);
                    entCreateCase.Attributes[CaseEntityAttributeNames.Description] = caseDetails.CaseDescription;
                    entCreateCase.Attributes[CaseEntityAttributeNames.ChannelRefNo] = caseDetails.ChannelRefNo;
                    entCreateCase.Attributes[CaseEntityAttributeNames.SMPostText] = caseDetails.SMPostText;
                    entCreateCase.Attributes[CaseEntityAttributeNames.SMPostDateTime] = caseDetails.SMPostDateTime;

                    if (caseDetails.CaseType != null && caseDetails.CaseType.Id != Guid.Empty && caseDetails.Category != null && caseDetails.Category.Id != Guid.Empty
                                && caseDetails.SubCategory != null && caseDetails.SubCategory.Id != Guid.Empty && !string.IsNullOrWhiteSpace(caseDetails.ChannelOrigin))
                    {
                        #region Set the Related Customer

                        if (caseDetails.Customer != null && caseDetails.Customer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(caseDetails.Customer.LogicalName))
                        {
                            entrefRelatedCustomer = new EntityReference(caseDetails.Customer.LogicalName, caseDetails.Customer.Id);
                            entCreateCase.Attributes[CaseEntityAttributeNames.Customer] = new EntityReference(caseDetails.Customer.LogicalName, caseDetails.Customer.Id);
                        }
                        else
                        {
                            entCreateCase.Attributes[CaseEntityAttributeNames.Customer] = crmCMs.RetrieveAnonymousCustomer(crmAdapter.CRMAdminService());
                        }

                        #endregion

                        #region Set the Case Type

                        entCreateCase.Attributes[CaseEntityAttributeNames.CaseType] = new EntityReference(CaseTypeEntityAttributeNames.EntityLogicalName, caseDetails.CaseType.Id);

                        #endregion

                        #region Set the Case Category

                        entCreateCase.Attributes[CaseEntityAttributeNames.Category] = new EntityReference(CaseCategoryEntityAttributeNames.EntityLogicalName, caseDetails.Category.Id);

                        #endregion

                        #region Set the Case Sub Category

                        entCreateCase.Attributes[CaseEntityAttributeNames.SubCatgeory] = new EntityReference(CaseSubCategoryEntityAttributeNames.EntityLogicalName, caseDetails.SubCategory.Id);

                        #endregion

                        #region Set the Case Origin Channel

                        entRequiredChannelOrigin = crmCMs.RetrieveChannelOriginFromOriginCode(crmAdapter.CRMAdminService(), caseDetails.ChannelOrigin);

                        if (entRequiredChannelOrigin != null && entRequiredChannelOrigin.Id != Guid.Empty)
                        {
                            entCreateCase.Attributes[CaseEntityAttributeNames.ChannelOrigin] = new EntityReference(entRequiredChannelOrigin.LogicalName, entRequiredChannelOrigin.Id);
                        }

                        #endregion                     
                    }
                    else
                    {
                        if (caseDetails.CaseType == null || caseDetails.CaseType.Id == Guid.Empty)
                        {
                            strExceptionMsg += customErrorMsgsForCase.missingCaseType;
                        }
                        if (caseDetails.Category == null || caseDetails.Category.Id == Guid.Empty)
                        {
                            strExceptionMsg += customErrorMsgsForCase.missingCategory;
                        }
                        if (caseDetails.SubCategory == null || caseDetails.SubCategory.Id == Guid.Empty)
                        {
                            strExceptionMsg += customErrorMsgsForCase.missingSubCategory;
                        }
                        if (string.IsNullOrWhiteSpace(caseDetails.ChannelOrigin))
                        {
                            strExceptionMsg += customErrorMsgsForCase.missingCaseOrigin;
                        }
                    }

                    #region If related Social Media Handle details are avaialble in the request body, associate it on the target case record

                    if (caseDetails.SMHDetails != null)
                    {
                        entSocialHandle = crmCMs.RetrieveSMCFromNetworkId(crmAdapter.CRMAdminService(), caseDetails.SMHDetails);

                        if (entSocialHandle == null || entSocialHandle.Id == Guid.Empty)
                        {
                            entSocialHandle = crmCMs.CreateSMCFromNetworkId(crmAdapter.ImpersonatedCRMService(guidLoggedInUser), caseDetails.SMHDetails, entRequiredChannelOrigin.ToEntityReference(), entrefRelatedCustomer);
                        }

                        if (entSocialHandle != null && entSocialHandle.Id != Guid.Empty)
                        {
                            #region Set the Related Social Media Handle

                            entCreateCase.Attributes[CaseEntityAttributeNames.SocialMediaHandle] = entSocialHandle.ToEntityReference();

                            #endregion
                        }
                    }

                    #endregion
                }
                else if (caseDetails == null)
                {
                    strExceptionMsg += customErrorMsgsForCase.missingCaseDetails;
                }

                if (string.IsNullOrWhiteSpace(strExceptionMsg))
                {
                    guidCUCase = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Create(entCreateCase);

                    if (guidCUCase != Guid.Empty)
                    {
                        rspCaseDetails.CaseGuid = guidCUCase;

                        Entity entRetrieveCase = null;
                        entRetrieveCase = crmAdapter.CRMAdminService().Retrieve(CaseEntityAttributeNames.EntityLogicalName, guidCUCase, new ColumnSet(CaseEntityAttributeNames.CaseRefNo, CaseEntityAttributeNames.AppRecordUrl));

                        if (entRetrieveCase != null)
                        {
                            rspCaseDetails.CaseRefNo = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCase, CaseEntityAttributeNames.CaseRefNo);
                            rspCaseDetails.AppRecordUrl = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCase, CaseEntityAttributeNames.AppRecordUrl);
                        }
                    }
                }
                else
                {
                    throw new Exception(strExceptionMsg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return rspCaseDetails;
        }
    }
}