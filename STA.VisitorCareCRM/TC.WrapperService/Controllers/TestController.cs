using System;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Utility;
using static TC.WrapperService.Models.TestModel;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/Test")]
    public class TestController : ApiController
    {
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "TestAPI Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // GET api/<controller>/5
        [Route("GetTestAPI")]
        public string GetTestAPI(int id)
        {
            return "value";
        }

        // GET api/<controller>/5
        [Route("GetTestAPI1")]
        public TestModelResponse GetTestAPI1(int id)
        {
            TestModelResponse testModelResponse = new TestModelResponse();

            try
            {
                #region Validate Session using STA Authenticate API

                HttpRequestHeaders reqHeaders = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeaders);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeaders);

                #endregion

                #region

                if (id != int.MinValue && id > 0)
                {
                    testModelResponse.TestAPIOutput = "Success Call for Test API";
                }
                else
                {
                    testModelResponse.TestAPIOutput = "Failure Call for Test API";
                }

                #endregion                
            }
            catch (Exception ex)
            {
                testModelResponse.TestAPIOutput = ex.Message;
            }

            return testModelResponse;
        }

        // GET api/<controller>/5
        [Route("GetTestAPI2")]
        public ResponseClass GetTestAPI2(int id)
        {
            exceptionResponseObjects.strAPIServiceName = "GetTestAPI2";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                TestModelResponse testModelResponse = new TestModelResponse();

                #region

                if (id != int.MinValue && id > 0)
                {
                    testModelResponse.TestAPIOutput = "Success Call for Test API";
                    otptResponse = apiCMs.SetSuccessResponse(testModelResponse);
                }
                else
                {
                    exceptionResponseObjects.resultDetails.resultDes = "Failure Call for Test API";
                    otptResponse = apiCMs.SetSuccessResponse(exceptionResponseObjects);
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = "Failure Call for Test API";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }
    }
}