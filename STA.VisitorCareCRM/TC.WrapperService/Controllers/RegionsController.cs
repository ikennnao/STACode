using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/Regions")]
    public class RegionsController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Regions Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CaseType/PostRetrieveCaseType
        [Route("GetInternationalMarkets")]
        public ResponseClass GetInternationalMarkets()
        {
            exceptionResponseObjects.strAPIServiceName = "GetRegions";
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

                    AuthenticateRequest _authenticateDetails = new AuthenticateRequest();
                    _authenticateDetails.UserName = ConfigurationManager.AppSettings["svc_username"];
                    RetrieveRegionsRequest rqtGetRegions = new RetrieveRegionsRequest();
                    rqtGetRegions.authenticateDetails = _authenticateDetails;
                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetRegions.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveRegionsResponse retrieveRegionsInfo = RetrieveRegions(guidLoggedInUser);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveRegionsInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Regions";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveRegionsResponse RetrieveRegions(Guid guidLoggedInUser)
        {
            RetrieveRegionsResponse rspRegions = new RetrieveRegionsResponse();

            try
            {
                string strRegionsMetaData = crmFetchXML.GetRegions();

                if (!string.IsNullOrWhiteSpace(strRegionsMetaData))
                {
                    EntityCollection entcol = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strRegionsMetaData));

                    if (entcol != null && entcol.Entities != null && entcol.Entities.Count > 0)
                    {
                        rspRegions.regionDataObj = new List<RetrieveRegions>();

                        foreach (Entity item in entcol.Entities)
                        {
                            RetrieveRegions _region = new RetrieveRegions();
                            _region.RegionGuid = crmCMs.GetAttributeValFromTargetEntity(item, RegionsEntityAttributeNames.RegionId);
                            _region.RegionName = crmCMs.GetAttributeValFromTargetEntity(item, RegionsEntityAttributeNames.RegionName);
                            rspRegions.regionDataObj.Add(_region);
                        }

                        rspRegions.RegionTypeETN = entcol.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspRegions;
        }
    }
}