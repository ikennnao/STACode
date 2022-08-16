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
    [RoutePrefix("API/CaseSubCategory")]
    public class CaseSubCategoryController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForCaseSubCategory customErrorMsgsForCaseSubCategory = new CustomErrorMsgsForCaseSubCategory();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "SubCategory Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/Category/PostRetrieveSubCategory
        [Route("PostRetrieveSubCategory")]
        public ResponseClass PostRetrieveSubCategory(RetrieveSubCategoryRequest rqtGetSubCategory)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveSubCategory";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validate

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetSubCategory.authenticateDetails);

                    #endregion

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveSubCategoryResponse retrieveSubCategoryInfo = RetrieveSubCategory(guidLoggedInUser, rqtGetSubCategory.retrieveSubCategory);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveSubCategoryInfo);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveSubCategoryResponse RetrieveSubCategory(Guid guidLoggedInUser, rqtObjRetrieveSubCategory rqtRetrieveSubCategory)
        {
            RetrieveSubCategoryResponse rspSubCategory = new RetrieveSubCategoryResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                if (rqtRetrieveSubCategory.CategoryGuid != Guid.Empty)
                {
                    string strSubCategoryMetaData = crmFetchXML.GetSubCategory(rqtRetrieveSubCategory.CategoryGuid);

                    if (!string.IsNullOrWhiteSpace(strSubCategoryMetaData))
                    {
                        EntityCollection entcolSubCategory = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strSubCategoryMetaData));

                        if (entcolSubCategory != null && entcolSubCategory.Entities != null && entcolSubCategory.Entities.Count > 0)
                        {
                            rspSubCategory.subcategoryDataObj = new List<rspObjRetrieveSubCategory>();

                            foreach (Entity entSubCategory in entcolSubCategory.Entities)
                            {
                                rspObjRetrieveSubCategory gbSubCategory = new rspObjRetrieveSubCategory();
                                gbSubCategory.SubCategoryGuid = crmCMs.GetAttributeValFromTargetEntity(entSubCategory, CaseSubCategoryEntityAttributeNames.CaseSubcategoryId);
                                gbSubCategory.Name = crmCMs.GetAttributeValFromTargetEntity(entSubCategory, CaseSubCategoryEntityAttributeNames.CaseSubcategoryName);
                                gbSubCategory.ArabicName = crmCMs.GetAttributeValFromTargetEntity(entSubCategory, CaseSubCategoryEntityAttributeNames.CaseSubCategoryArabicName);
                                gbSubCategory.Category = apiCMs.SetCustomFormatEntRefValue(entSubCategory, CaseSubCategoryEntityAttributeNames.CaseCategory);
                                rspSubCategory.subcategoryDataObj.Add(gbSubCategory);
                            }

                            rspSubCategory.subcategoryETN = entcolSubCategory.EntityName;
                        }
                    }
                }
                else
                {
                    throw new Exception(customErrorMsgsForCaseSubCategory.missingCaseCategory);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspSubCategory;
        }

    }
}