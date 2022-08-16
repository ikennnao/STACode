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
    [RoutePrefix("API/SubRegions")]
    public class SubRegionsController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Sub Regions Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CaseType/PostRetrieveCaseType
        [Route("GetSubRegions")]
        public ResponseClass GetSubregions(Guid regionId)
        {
            exceptionResponseObjects.strAPIServiceName = "GetSubRegionsRegions";
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
                    RetrieveSubRegionsRequest rqtGetSubRegions = new RetrieveSubRegionsRequest();
                    rqtGetSubRegions.authenticateDetails = _authenticateDetails;
                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetSubRegions.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {

                        RetrieveSubRegionsResponse retrieveRegionsInfo = RetrieveSubRegions(guidLoggedInUser, regionId);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveRegionsInfo);

                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: SubRegions";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveSubRegionsResponse RetrieveSubRegions(Guid guidLoggedInUser, Guid regionId)
        {
            RetrieveSubRegionsResponse rspSubRegions = new RetrieveSubRegionsResponse();

            try
            {
                string strSubRegionsMetaData = crmFetchXML.GetSubRegions(regionId);

                if (!string.IsNullOrWhiteSpace(strSubRegionsMetaData))
                {
                    EntityCollection entcol = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strSubRegionsMetaData));

                    if (entcol != null && entcol.Entities != null && entcol.Entities.Count > 0)
                    {
                        rspSubRegions.SubregionDataObj = new List<RetrieveSubRegions>();

                        foreach (Entity item in entcol.Entities)
                        {
                            RetrieveSubRegions _subregion = new RetrieveSubRegions();
                            _subregion.SubRegionGuid = crmCMs.GetAttributeValFromTargetEntity(item, SubRegionsEntityAttributeNames.SubRegionId);
                            _subregion.SubRegionName = crmCMs.GetAttributeValFromTargetEntity(item, SubRegionsEntityAttributeNames.SubRegionName);
                            _subregion.RegionGuid = apiCMs.SetCustomFormatEntRefValue(item, SubRegionsEntityAttributeNames.Region).Id;
                            _subregion.RegionName = apiCMs.SetCustomFormatEntRefValue(item, SubRegionsEntityAttributeNames.Region).Name;
                            rspSubRegions.SubregionDataObj.Add(_subregion);
                        }

                        rspSubRegions.SubRegionTypeETN = entcol.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspSubRegions;
        }
    }
}