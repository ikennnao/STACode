using System;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Models;
using TC.WrapperService.Utility;
using UnifonicNextGen.Standard.Controllers;
using UnifonicNextGen.Standard.Models;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/SMS")]
    public class SMSController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForSMSActivity customErrorMsgsForSMS = new CustomErrorMsgsForSMSActivity();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private ResponseClass otptResponse = new ResponseClass();
        private string strAPIPageName = "SMS Controller";
        private bool isSessionValid = false;


        // POST: API/SMS/PostSendSMS
        [Route("PostSendSMS")]
        public ResponseClass PostSendSMS(SendSMSRequest rqtSendSMSDetails)
        {
            exceptionResponseObjects.strAPIServiceName = "PostSendSMS";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate Session using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isSessionValid = apiCMs.ValidateWithAPIToken(reqHeader);

                #endregion

                #region If, Session is Valid

                if (isSessionValid)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtSendSMSDetails.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        SendSMSResponse rspSendSMSInfo = SendSMSToUnifonicNextGen(rqtSendSMSDetails.objSendSMS);
                        otptResponse = apiCMs.SetSuccessResponse(rspSendSMSInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInSendSMSOperation + "Target Table: SMS Activity";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message + "Customer No :" + rqtSendSMSDetails.objSendSMS.Recipient;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private SendSMSResponse SendSMSToUnifonicNextGen(rqtObjSendSMS rqtObjSendSMSDetails)
        {
            SendSMSResponse rspSendSMS = null;
            string strExceptionMsg = string.Empty;

            try
            {
                if (rqtObjSendSMSDetails != null && !string.IsNullOrWhiteSpace(rqtObjSendSMSDetails.AppId) && !string.IsNullOrWhiteSpace(rqtObjSendSMSDetails.Message)
                    && !string.IsNullOrWhiteSpace(rqtObjSendSMSDetails.SenderId))
                {
                    if (rqtObjSendSMSDetails.Recipient != Int64.MinValue && rqtObjSendSMSDetails.Recipient != Int64.MaxValue)
                    {
                        RestController restController = new RestController();
                        SendResponse rspSendSMSApi = restController.CreateSendMessage(rqtObjSendSMSDetails.AppId, rqtObjSendSMSDetails.SenderId, rqtObjSendSMSDetails.Message, rqtObjSendSMSDetails.Recipient);

                        if (rspSendSMSApi != null)
                        {
                            rspSendSMS = new SendSMSResponse();

                            if (!string.IsNullOrWhiteSpace(rspSendSMSApi.Success))
                            {
                                rspSendSMS.IsSMSSuccess = rspSendSMSApi.Success.ToLower().Trim() == "true" ? true : false;
                            }
                            rspSendSMS.ResponseCode = rspSendSMSApi.ErrorCode;
                            rspSendSMS.ResponseMsg = rspSendSMSApi.Message;
                            rspSendSMS.ResponseData = rspSendSMSApi.Data;
                        }
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForSMS.inValidRecipientVal;
                        throw new Exception(strExceptionMsg);
                    }
                }
                else
                {
                    if (rqtObjSendSMSDetails == null)
                    {
                        strExceptionMsg += customErrorMsgsForSMS.missingSendSMSDetails;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(rqtObjSendSMSDetails.AppId))
                        {
                            strExceptionMsg += customErrorMsgsForSMS.missingAppId;
                        }
                        if (string.IsNullOrWhiteSpace(rqtObjSendSMSDetails.Message))
                        {
                            strExceptionMsg += customErrorMsgsForSMS.missingMessage;
                        }
                        if (string.IsNullOrWhiteSpace(rqtObjSendSMSDetails.SenderId))
                        {
                            strExceptionMsg += customErrorMsgsForSMS.missingSenderId;
                        }
                    }
                    throw new Exception(strExceptionMsg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspSendSMS;
        }
    }
}