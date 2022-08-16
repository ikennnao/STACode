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
    [RoutePrefix("API/CaseType")]
    public class CaseTypeController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "CaseType Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CaseType/PostRetrieveCaseType
        [Route("PostRetrieveCaseType")]
        public ResponseClass PostRetrieveCaseType(RetrieveCaseTypeRequest rqtGetCaseType)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveCaseType";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCaseType.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCaseTypeResponse retrieveCaseTypeInfo = RetrieveCaseType(guidLoggedInUser, rqtGetCaseType.retrieveCaseType);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCaseTypeInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Case Type";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveCaseTypeResponse RetrieveCaseType(Guid guidLoggedInUser, RetrieveCaseType retrieveCaseType)
        {
            RetrieveCaseTypeResponse rspCaseType = new RetrieveCaseTypeResponse();

            try
            {
                string strCaseTypeMetaData = crmFetchXML.GetCaseTypes(retrieveCaseType);

                if (!string.IsNullOrWhiteSpace(strCaseTypeMetaData))
                {
                    EntityCollection entcolCaseType = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strCaseTypeMetaData));

                    if (entcolCaseType != null && entcolCaseType.Entities != null && entcolCaseType.Entities.Count > 0)
                    {
                        rspCaseType.caseTypeDataObj = new List<RetrieveCaseType>();

                        foreach (Entity item in entcolCaseType.Entities)
                        {
                            RetrieveCaseType gbCaseTypeData = new RetrieveCaseType();
                            gbCaseTypeData.CaseTypeGuid = crmCMs.GetAttributeValFromTargetEntity(item, CaseTypeEntityAttributeNames.CaseTypeId);
                            gbCaseTypeData.Name = crmCMs.GetAttributeValFromTargetEntity(item, CaseTypeEntityAttributeNames.CaseTypeName);
                            gbCaseTypeData.ArabicName = crmCMs.GetAttributeValFromTargetEntity(item, CaseTypeEntityAttributeNames.CaseTypeArabicName);
                            rspCaseType.caseTypeDataObj.Add(gbCaseTypeData);
                        }

                        rspCaseType.caseTypeETN = entcolCaseType.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCaseType;
        }
    }
}