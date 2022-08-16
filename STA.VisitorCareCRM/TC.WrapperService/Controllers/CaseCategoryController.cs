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
    [RoutePrefix("API/CaseCategory")]
    public class CaseCategoryController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForCaseCategory customErrorMsgsForCaseCategory = new CustomErrorMsgsForCaseCategory();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Category Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/Category/PostRetrieveCategory
        [Route("PostRetrieveCategory")]
        public ResponseClass PostRetrieveCategory(RetrieveCategoryRequest rqtGetCategory)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveCategory";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCategory.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCategoryResponse retrieveCategoryInfo = RetrieveCategory(guidLoggedInUser, rqtGetCategory.retrieveCategory);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCategoryInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Case Category";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveCategoryResponse RetrieveCategory(Guid guidLoggedInUser, rqtObjRetrieveCategory rqtObjCategory)
        {
            RetrieveCategoryResponse rspCategory = new RetrieveCategoryResponse();

            try
            {
                if (rqtObjCategory != null && rqtObjCategory.CaseTypeId != Guid.Empty)
                {
                    string strCategoryMetaData = crmFetchXML.GetCategory(rqtObjCategory);

                    if (!string.IsNullOrWhiteSpace(strCategoryMetaData))
                    {
                        EntityCollection entcolCategory = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strCategoryMetaData));

                        if (entcolCategory != null && entcolCategory.Entities != null && entcolCategory.Entities.Count > 0)
                        {
                            rspCategory.categoryDataObj = new List<rspObjRetrieveCategory>();

                            foreach (Entity entCategory in entcolCategory.Entities)
                            {
                                rspObjRetrieveCategory gbCategory = new rspObjRetrieveCategory();
                                gbCategory.CategoryGuid = crmCMs.GetAttributeValFromTargetEntity(entCategory, CaseCategoryEntityAttributeNames.CaseCategoryId);
                                gbCategory.Name = crmCMs.GetAttributeValFromTargetEntity(entCategory, CaseCategoryEntityAttributeNames.CaseCategoryName);
                                gbCategory.ArabicName = crmCMs.GetAttributeValFromTargetEntity(entCategory, CaseCategoryEntityAttributeNames.CaseCategoryArabicName);
                                gbCategory.CaseType = apiCMs.SetCustomFormatEntRefValue(entCategory, CaseCategoryEntityAttributeNames.CaseType);
                                rspCategory.categoryDataObj.Add(gbCategory);
                            }

                            rspCategory.categoryETN = entcolCategory.EntityName;
                        }
                    }
                }
                else
                {
                    throw new Exception(customErrorMsgsForCaseCategory.missingCaseType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCategory;
        }
    }
}