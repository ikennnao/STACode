using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/FAQCategories")]
    public class FAQCategoriesController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForCaseCategory customErrorMsgsForCaseCategory = new CustomErrorMsgsForCaseCategory();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "FAQ Category Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/FAQCategories/PostRetrieveFAQCategories
        [Route("PostRetrieveFAQCategories")]
        public ResponseClass PostRetrieveFAQCategories(RetrieveFAQCategoriesRequest rqtGetFAQCategories)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveFAQCategories";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetFAQCategories.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveFAQCategoriesResponse retrieveCategoryInfo = RetrieveFAQCategories(guidLoggedInUser);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCategoryInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: FAQ Category";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveFAQCategoriesResponse RetrieveFAQCategories(Guid guidLoggedInUser)
        {
            RetrieveFAQCategoriesResponse rspCategory = new RetrieveFAQCategoriesResponse();
            rspCategory.categoryDataObj = new List<rspObjRetrieveFAQCategories>();
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = "knowledgearticle",
                    LogicalName = "tc_faqcategories",
                    RetrieveAsIfPublished = true
                };

                var attributeResponse = (RetrieveAttributeResponse)crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Execute(attributeRequest);
                var attributeMetadata = (EnumAttributeMetadata)attributeResponse.AttributeMetadata;

                rspCategory.categoryDataObj = (from o in attributeMetadata.OptionSet.Options
                                  select new rspObjRetrieveFAQCategories { Value = Convert.ToInt32(o.Value), Name = o.Label.LocalizedLabels[0].Label, ArabicName = o.Label.LocalizedLabels[1].Label }).ToList();

                
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCategory;
        }
    }
}