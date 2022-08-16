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
    [RoutePrefix("API/CustomerCategory")]
    public class CustomerCategoryController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "CustomerCategory Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CustomerCategory/PostRetrieveCustomerCategory
        [Route("PostRetrieveCustomerCategory")]
        public ResponseClass PostRetrieveCustomerCategory(RetrieveCustomerCategoryRequest rqtGetCustCategory)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveCustomerCategory";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCustCategory.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCustomerCategoryResponse retrieveCustomerCategoryInfo = RetrieveCustomerCategory(rqtGetCustCategory.retrieveCustomerCategory);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCustomerCategoryInfo);
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

        private RetrieveCustomerCategoryResponse RetrieveCustomerCategory(RetrieveCustomerCategoryObj retrieveCustomerCategoryObj)
        {
            RetrieveCustomerCategoryResponse rspCustomerCategory = new RetrieveCustomerCategoryResponse();

            try
            {
                if (retrieveCustomerCategoryObj == null)
                {
                    retrieveCustomerCategoryObj = new RetrieveCustomerCategoryObj();
                    retrieveCustomerCategoryObj.Status = "0"; //Active
                }

                string strCustomerCategoryMetaData = crmFetchXML.GetCustomerCategory(retrieveCustomerCategoryObj);

                if (!string.IsNullOrWhiteSpace(strCustomerCategoryMetaData))
                {
                    EntityCollection entcolCustomerCategory = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strCustomerCategoryMetaData));

                    if (entcolCustomerCategory != null && entcolCustomerCategory.Entities != null && entcolCustomerCategory.Entities.Count > 0)
                    {
                        rspCustomerCategory.customerCategoryDataObj = new List<RetrieveCustomerCategory>();

                        foreach (Entity item in entcolCustomerCategory.Entities)
                        {
                            RetrieveCustomerCategory gbCustomerCategoryData = new RetrieveCustomerCategory();
                            gbCustomerCategoryData.CustomerCategoryGuid = crmCMs.GetAttributeValFromTargetEntity(item, CustomerCategoryEntityAttributeNames.CustomerCategoryId);
                            gbCustomerCategoryData.Name = crmCMs.GetAttributeValFromTargetEntity(item, CustomerCategoryEntityAttributeNames.CustomerCategoryName);
                            gbCustomerCategoryData.ArabicName = crmCMs.GetAttributeValFromTargetEntity(item, CustomerCategoryEntityAttributeNames.CustomerCategoryArabicName);
                            rspCustomerCategory.customerCategoryDataObj.Add(gbCustomerCategoryData);
                        }

                        rspCustomerCategory.customerCategoryETN = entcolCustomerCategory.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCustomerCategory;
        }
    }
}