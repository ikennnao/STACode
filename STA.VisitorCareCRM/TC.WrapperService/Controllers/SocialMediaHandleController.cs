using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/SocialMediaHandle")]
    public class SocialMediaHandleController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForSocialMediaHandle customErrorMsgsForSMH = new CustomErrorMsgsForSocialMediaHandle();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Social Media Handle Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;
        private string targerEnity = string.Empty;

        // POST: API/SocialMediaHandle/PostRetrieveSocialMediaHandle
        [Route("PostRetrieveSocialMediaHandle")]
        public ResponseClass PostRetrieveSocialMediaHandle(RetrieveSocialMediaHandleRequest rqtGetSMH)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveSocialMediaHandle";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                if (rqtGetSMH.RetrieveSMHobj.Customer.Id != Guid.Empty)
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

                        Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetSMH.AuthenticateDetails);

                        #endregion

                        if (guidLoggedInUser != Guid.Empty)
                        {
                            RetrieveSocialMediaHandleResponse retrieveSMHInfo = RetrieveSocialMediaHandleMtd(rqtGetSMH.RetrieveSMHobj);
                            otptResponse = apiCMs.SetSuccessResponse(retrieveSMHInfo);
                        }
                    }

                    #endregion
                }
                else
                {
                    throw new Exception(customErrorMsgsForSMH.missingCustomer);
                }
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInRetrieveOperation + "Target table: " + SocialMediaHandleEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Customer/PostCreateSocialMediaHandle
        [Route("PostCreateSocialMediaHandle")]
        public ResponseClass PostCreateSocialMediaHandle(CreateSocialMediaHandleRequest rqtCreateSMH)
        {
            exceptionResponseObjects.strAPIServiceName = "CreateSocialMediaHandle";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtCreateSMH.AuthenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        CreateSocialMediaHandleResponse createdSMHInfo = CreateSocialMediaHandleMtd(guidLoggedInUser, rqtCreateSMH.CreateSMH);
                        otptResponse = apiCMs.SetSuccessResponse(createdSMHInfo);
                        otptResponse.apiResult.resultDesDetails = "Social Media Handle was created successfully";
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + SocialMediaHandleEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveSocialMediaHandleResponse RetrieveSocialMediaHandleMtd(RetrieveSMHObj socialMediaHandleDetails)
        {
            RetrieveSocialMediaHandleResponse rspRetrieveSMH = null;
            string strExceptionMsg = string.Empty;

            try
            {
                string strSMHMetaData = crmFetchXML.GetSocialMediaHandleData(socialMediaHandleDetails);

                EntityCollection entcolSocialMediaHandle = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strSMHMetaData));

                if (entcolSocialMediaHandle != null && entcolSocialMediaHandle.Entities != null && entcolSocialMediaHandle.Entities.Count > 0)
                {
                    rspRetrieveSMH = new RetrieveSocialMediaHandleResponse();
                    rspRetrieveSMH.LstSMHRecords = new List<RspRetrieveSMHObj>() { };

                    foreach (Entity entCustomer in entcolSocialMediaHandle.Entities)
                    {
                        RspRetrieveSMHObj rspSMHObj = new RspRetrieveSMHObj();
                        rspSMHObj.SocialMediaHandleGuid = crmCMs.GetAttributeValFromTargetEntity(entCustomer, SocialMediaHandleEntityAttributesNames.SocialMediaHandleID);
                        rspSMHObj.UserName = crmCMs.GetAttributeValFromTargetEntity(entCustomer, SocialMediaHandleEntityAttributesNames.SocialUserName);
                        rspSMHObj.UserID = crmCMs.GetAttributeValFromTargetEntity(entCustomer, SocialMediaHandleEntityAttributesNames.SocialUserID);
                        rspSMHObj.Customer = apiCMs.SetCustomFormatEntRefValue(entCustomer, SocialMediaHandleEntityAttributesNames.Customer);
                        rspSMHObj.SocialChannel = apiCMs.SetCustomFormatEntRefValue(entCustomer, SocialMediaHandleEntityAttributesNames.SocialHandle);
                        rspRetrieveSMH.LstSMHRecords.Add(rspSMHObj);
                    }

                    rspRetrieveSMH.SocialMediaHandleETN = entcolSocialMediaHandle.EntityName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return rspRetrieveSMH;
        }

        private CreateSocialMediaHandleResponse CreateSocialMediaHandleMtd(Guid guidLoggedInUser, CreateSMH SMHDetails)
        {
            CreateSocialMediaHandleResponse rspSocialMediaHandle = new CreateSocialMediaHandleResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Entity entCreateSMH = null;
                Guid guidCRSocialMediaHandle = Guid.Empty;

                if (SMHDetails != null)
                {
                    entCreateSMH = new Entity(SocialMediaHandleEntityAttributesNames.EntityLogicalName);

                    if (!string.IsNullOrWhiteSpace(SMHDetails.UserName) && SMHDetails.UserName != null)
                    {
                        entCreateSMH.Attributes[SocialMediaHandleEntityAttributesNames.SocialUserName] = SMHDetails.UserName;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForSMH.missingUserName;
                    }

                    if (SMHDetails.Customer.Id != Guid.Empty && SMHDetails.Customer != null)
                    {
                        Entity customerEntity = crmAdapter.CRMAdminService().Retrieve(ContactEntityAttributesNames.EntityLogicalName, SMHDetails.Customer.Id, new ColumnSet(ContactEntityAttributesNames.CustomerId));

                        if (customerEntity.Id != Guid.Empty)
                        {
                            entCreateSMH.Attributes[SocialMediaHandleEntityAttributesNames.Customer] = new EntityReference(ContactEntityAttributesNames.EntityLogicalName, SMHDetails.Customer.Id);
                        }
                        else
                        {
                            strExceptionMsg += customErrorMsgsForSMH.inValidCustomerId;
                        }
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForSMH.missingCustomer;
                    }

                    if (SMHDetails.SocialChannel.Id != Guid.Empty && SMHDetails.SocialChannel != null)
                    {
                        Entity socialChannelEntity = crmAdapter.CRMAdminService().Retrieve(ChannelOriginEntityAttributeNames.EntityLogicalName, SMHDetails.SocialChannel.Id, new ColumnSet(ChannelOriginEntityAttributeNames.ChannelOriginId));

                        if (socialChannelEntity.Id != Guid.Empty)
                        {
                            entCreateSMH.Attributes[SocialMediaHandleEntityAttributesNames.SocialHandle] = new EntityReference(ContactEntityAttributesNames.EntityLogicalName, SMHDetails.SocialChannel.Id);
                        }
                        else
                        {
                            strExceptionMsg += customErrorMsgsForSMH.inValidSocialChannel;
                        }
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForSMH.missingSocialChannel;
                    }

                    if (!string.IsNullOrWhiteSpace(SMHDetails.UserID) && SMHDetails.UserID != null)
                    {
                        entCreateSMH.Attributes[SocialMediaHandleEntityAttributesNames.SocialUserID] = SMHDetails.UserID;
                    }

                    if (string.IsNullOrWhiteSpace(strExceptionMsg))
                    {
                        entCreateSMH.Id = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Create(entCreateSMH);

                        if (entCreateSMH.Id != Guid.Empty)
                        {
                            rspSocialMediaHandle.EntityLogicalName = SocialMediaHandleEntityAttributesNames.EntityLogicalName;
                            rspSocialMediaHandle.SocialMediaHandleGuid = entCreateSMH.Id;
                        }
                    }
                    else
                    {
                        throw new Exception(strExceptionMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspSocialMediaHandle;
        }
    }
}