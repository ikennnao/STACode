using Microsoft.Crm.Sdk.Messages;
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
using static TC.WrapperService.Utility.Crypto;


namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/KnowledgeArticle")]
    public class KnowledgeArticleController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "KnowledgeArticle Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/CaseType/PostRetrieveCaseType
        [Route("PostRetrieveKnowledgeArticle")]
        public ResponseClass PostRetrieveKnowledgeArticle(RetrieveKnowledgeRequest rqtGetKA)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveKnowledgeArticle";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();
            //string a = "DfqeoEfsgoDUuJmELEY6GP75oqzDMxFxrhekrAuHl29fJffXJvi56UNWfGYk6wox";
            //string dkey = Crypto.TripleDESDecrypt(a);
            //string encrypt = Crypto.TripleDESEncrypt("apiadminuser|J!J@R#I$H%K^F&R*2021|20210817");

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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetKA.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        rqtGetKA.retrieveKnowledgeArticle.LanguageCode =  GetLanguageLocale(guidLoggedInUser, rqtGetKA.retrieveKnowledgeArticle.LanguageCode).Id.ToString();
                        RetrieveKnowledgeResponse retrieveKAInfo = RetrieveFAQs(guidLoggedInUser, rqtGetKA.retrieveKnowledgeArticle);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveKAInfo);
                    }

                   
                }

               

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Knowledge Article";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private RetrieveKnowledgeResponse RetrieveFAQs(Guid guidLoggedInUser, RetrieveKnowledgeArticleObj retrieveKA)
        {
            RetrieveKnowledgeResponse rspRetrieveKA = new RetrieveKnowledgeResponse();

            try
            {
                if (retrieveKA != null)
                {
                    //if (!retrieveKA.IsInternalArticle == int.MinValue)
                    //{
                    //    retrieveKA.IsInternalArticle = 0;
                    //}

                    string strKARcrdData = crmFetchXML.GetKnowledgeArticles(retrieveKA);

                    if (!string.IsNullOrWhiteSpace(strKARcrdData))
                    {
                        EntityCollection entcolKA = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strKARcrdData));
                        
                        if (entcolKA != null && entcolKA.Entities != null && entcolKA.Entities.Count > 0)
                        {
                            rspRetrieveKA.kaDataObj = new List<RetrieveKnowledgeArticle>();

                            foreach (Entity item in entcolKA.Entities)
                            {
                                RetrieveKnowledgeArticle gbCaseTypeData = new RetrieveKnowledgeArticle();
                                gbCaseTypeData.KnowledgeArticleGuid = crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.KnowledgeArticleId);
                                gbCaseTypeData.Title = crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.Title);
                                if (retrieveKA.IsContentRequired)
                                {
                                    gbCaseTypeData.Content = crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.Content);
                                }
                                gbCaseTypeData.AppRecordUrl = crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.AppRecordUrl);
                                gbCaseTypeData.KeyWords = crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.Keywords);
                                gbCaseTypeData.Order = crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.Order)!=null ? crmCMs.GetAttributeValFromTargetEntity(item, KnowledgeArticleEntityAttributeNames.Order):-1;
                                rspRetrieveKA.kaDataObj.Add(gbCaseTypeData);
                            }

                            rspRetrieveKA.knowledgeArticleETN = entcolKA.EntityName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspRetrieveKA;
        }

        private Entity GetLanguageLocale(Guid guidLoggedInUser,string langCode)
        {
            string[] columns = { LanguageLocaleEntityAttributeNames.Code, LanguageLocaleEntityAttributeNames.Language, LanguageLocaleEntityAttributeNames.LocaleId, LanguageLocaleEntityAttributeNames.Name };
            QueryExpression qe = new QueryExpression(LanguageLocaleEntityAttributeNames.EntityLogicalName);
            ConditionExpression conditionExpression = new ConditionExpression(LanguageLocaleEntityAttributeNames.LocaleId, ConditionOperator.Equal, langCode);
            ColumnSet columnSet = new ColumnSet(columns);
            FilterExpression filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(conditionExpression);
            qe.Criteria.AddFilter(filter);
            qe.ColumnSet = columnSet;
            EntityCollection entLang  = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(qe);
            if (entLang != null && entLang.Entities != null && entLang.Entities.Count > 0)
            {
                return entLang.Entities[0];
            }
            else
            {

                return null;
            }
        }

    }
}