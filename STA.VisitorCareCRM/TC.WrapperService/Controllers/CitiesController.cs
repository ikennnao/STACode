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
    [RoutePrefix("API/Cities")]
    public class CitiesController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Cities Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CaseType/PostRetrieveCaseType
        [Route("GetCities")]
        public ResponseClass GetCities(Guid CountryId)
        {
            exceptionResponseObjects.strAPIServiceName = "GetCities";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & APi Auth Key using STA Authenticate API

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
                    RetrieveCitiesRequest rqtGetCities = new RetrieveCitiesRequest();
                    rqtGetCities.authenticateDetails = _authenticateDetails;
                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCities.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        if (CountryId != Guid.Empty)
                        {
                            RetrieveCitiesResponse retrieveCountriesInfo = RetrieveCities(guidLoggedInUser, CountryId);
                            otptResponse = apiCMs.SetSuccessResponse(retrieveCountriesInfo);
                        }
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Cities";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveCitiesResponse RetrieveCities(Guid guidLoggedInUser, Guid countryId)
        {
            RetrieveCitiesResponse rspCities = new RetrieveCitiesResponse();

            try
            {
                string strCitiesMetaData = crmFetchXML.GetCities(countryId);

                if (!string.IsNullOrWhiteSpace(strCitiesMetaData))
                {
                    EntityCollection entcol = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strCitiesMetaData));

                    if (entcol != null && entcol.Entities != null && entcol.Entities.Count > 0)
                    {
                        rspCities.citiesDataObj = new List<RetrieveCities>();

                        foreach (Entity item in entcol.Entities)
                        {
                            RetrieveCities _cities = new RetrieveCities();
                            _cities.CityGuid = crmCMs.GetAttributeValFromTargetEntity(item, CitiesEntityAttributeNames.CityGuid);
                            _cities.CityName = crmCMs.GetAttributeValFromTargetEntity(item, CitiesEntityAttributeNames.CityName);
                            _cities.CountryName = apiCMs.SetCustomFormatEntRefValue(item, CitiesEntityAttributeNames.Country).Name;
                            _cities.CountryGuid = apiCMs.SetCustomFormatEntRefValue(item, CitiesEntityAttributeNames.Country).Id;
                            rspCities.citiesDataObj.Add(_cities);
                        }

                        rspCities.citiesTypeETN = entcol.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCities;
        }
    }
}