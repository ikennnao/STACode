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
    [RoutePrefix("API/ChannelOrigin")]
    public class ChannelOriginController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "ChannelOrigin Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CustomerCategory/PostRetrieveCustomerCategory
        [Route("PostRetrieveChannelOrigin")]
        public ResponseClass PostRetrieveChannelOrigin(RetrieveChannelOriginRequest rqtGetChannelOrigin)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveChannelOrigin";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetChannelOrigin.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveChannelOriginResponse retrieveChannelOriginInfo = RetrieveChannelOrigins(rqtGetChannelOrigin.rqtobjChannelOrigin);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveChannelOriginInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInRetrieveOperation + "Target Table: Customer Category";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveChannelOriginResponse RetrieveChannelOrigins(RqtObjChannelOrigin rqtObjChannelOrigin)
        {
            RetrieveChannelOriginResponse rspChannelOrigins = new RetrieveChannelOriginResponse();

            try
            {
                string strChannelOriginData = crmFetchXML.GetChannelOrigin(rqtObjChannelOrigin);

                if (!string.IsNullOrWhiteSpace(strChannelOriginData))
                {
                    EntityCollection entcolChannelOrigin = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strChannelOriginData));

                    if (entcolChannelOrigin != null && entcolChannelOrigin.Entities != null && entcolChannelOrigin.Entities.Count > 0)
                    {
                        rspChannelOrigins.channelOriginDataObj = new List<RspObjChannelOrigin>();

                        foreach (Entity entChannelOrigin in entcolChannelOrigin.Entities)
                        {
                            RspObjChannelOrigin gbChannelOriginData = new RspObjChannelOrigin();
                            gbChannelOriginData.ChannelOriginGuid = crmCMs.GetAttributeValFromTargetEntity(entChannelOrigin, ChannelOriginEntityAttributeNames.ChannelOriginId);
                            gbChannelOriginData.Name = crmCMs.GetAttributeValFromTargetEntity(entChannelOrigin, ChannelOriginEntityAttributeNames.ChannelOriginName);
                            gbChannelOriginData.ArabicName = crmCMs.GetAttributeValFromTargetEntity(entChannelOrigin, ChannelOriginEntityAttributeNames.ChannelOriginArabicName);
                            gbChannelOriginData.ChannelType = apiCMs.SetCustomFormatOptionsetValue(entChannelOrigin, ChannelOriginEntityAttributeNames.ChannelOriginType);
                            gbChannelOriginData.ChannelOriginCode = crmCMs.GetAttributeValFromTargetEntity(entChannelOrigin, ChannelOriginEntityAttributeNames.ChannelOriginCode);
                            rspChannelOrigins.channelOriginDataObj.Add(gbChannelOriginData);
                        }

                        rspChannelOrigins.channelOriginETN = entcolChannelOrigin.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspChannelOrigins;
        }
    }
}