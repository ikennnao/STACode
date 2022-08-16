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
    [RoutePrefix("API/Countries")]
    public class CountriesController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Countries Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CaseType/PostRetrieveCaseType
        [Route("GetCountries")]
        public ResponseClass GetCountries()
        {
            exceptionResponseObjects.strAPIServiceName = "GetCountries";
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
                    RetrieveCountriesRequest rqtGetCountries = new RetrieveCountriesRequest();
                    rqtGetCountries.authenticateDetails = _authenticateDetails;
                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCountries.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCountriesResponse retrieveCountriesInfo = RetrieveCountries(guidLoggedInUser);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCountriesInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Countris";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveCountriesResponse RetrieveCountries(Guid guidLoggedInUser)
        {
            RetrieveCountriesResponse rspCountries = new RetrieveCountriesResponse();

            try
            {
                string strCountriesMetaData = crmFetchXML.GetCountries();

                if (!string.IsNullOrWhiteSpace(strCountriesMetaData))
                {
                    EntityCollection entcolCountries = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strCountriesMetaData));

                    if (entcolCountries != null && entcolCountries.Entities != null && entcolCountries.Entities.Count > 0)
                    {
                        rspCountries.countriesDataObj = new List<RetrieveCountries>();

                        foreach (Entity item in entcolCountries.Entities)
                        {
                            RetrieveCountries _country = new RetrieveCountries();
                            _country.CountryGuid = crmCMs.GetAttributeValFromTargetEntity(item, CountryEntityAttributeNames.CountryId);
                            _country.CountryName = crmCMs.GetAttributeValFromTargetEntity(item, CountryEntityAttributeNames.CountryName);
                            _country.ISO = crmCMs.GetAttributeValFromTargetEntity(item, CountryEntityAttributeNames.ISO);
                            rspCountries.countriesDataObj.Add(_country);
                        }

                        rspCountries.countryTypeETN = entcolCountries.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCountries;
        }
        [Route("GetCountriesFromRegion")]
        public ResponseClass GetCountriesFromRegion(Guid regionId)
        {
            exceptionResponseObjects.strAPIServiceName = "GetCountriesFromRegion";
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
                    RetrieveCountriesRequest rqtGetCountries = new RetrieveCountriesRequest();
                    rqtGetCountries.authenticateDetails = _authenticateDetails;
                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCountries.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCountriesResponse retrieveCountriesInfo = RetrieveCountriesFromRegion(guidLoggedInUser, regionId);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCountriesInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Countris";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveCountriesResponse RetrieveCountriesFromRegion(Guid guidLoggedInUser, Guid regionId)
        {
            RetrieveCountriesResponse rspCountries = new RetrieveCountriesResponse();

            try
            {
                string strCountriesMetaData = crmFetchXML.GetCountriesFromRegion(regionId);

                if (!string.IsNullOrWhiteSpace(strCountriesMetaData))
                {
                    EntityCollection entcolCountries = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strCountriesMetaData));

                    if (entcolCountries != null && entcolCountries.Entities != null && entcolCountries.Entities.Count > 0)
                    {
                        rspCountries.countriesDataObj = new List<RetrieveCountries>();

                        foreach (Entity item in entcolCountries.Entities)
                        {
                            RetrieveCountries _country = new RetrieveCountries();
                            _country.CountryGuid = crmCMs.GetAttributeValFromTargetEntity(item, CountryEntityAttributeNames.CountryId);
                            _country.CountryName = crmCMs.GetAttributeValFromTargetEntity(item, CountryEntityAttributeNames.CountryName);
                            _country.ISO = crmCMs.GetAttributeValFromTargetEntity(item, CountryEntityAttributeNames.ISO);
                            _country.RegionGuid = apiCMs.SetCustomFormatEntRefValue(item, CountryEntityAttributeNames.RegionId).Id;
                            rspCountries.countriesDataObj.Add(_country);
                        }

                        rspCountries.countryTypeETN = entcolCountries.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCountries;
        }
    }
}