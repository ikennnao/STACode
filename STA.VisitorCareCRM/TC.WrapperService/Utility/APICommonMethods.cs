using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using TC.WrapperService.Interfaces;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Utility
{
    public class APICommonMethods
    {
        private readonly ILogging logger = Instance;
        private CRMCommonMethods crmCMs = new CRMCommonMethods();

        public class ExceptionResponseObjects
        {
            public ResultClass resultDetails { get; set; }
            public string strException { get; set; }
            public string strAPIPageName { get; set; }
            public string strAPIServiceName { get; set; }
        }

        public class ResponseClass
        {
            public ResultClass apiResult { get; set; }
            public dynamic apidataObj { get; set; }
        }

        public class ResultClass
        {
            public string resultCode { get; set; }
            public string resultDes { get; set; }
            public string resultDesDetails { get; set; }
        }

        public class AuthenticateRequest
        {
            public string UserName { get; set; }
            public string ChannelId { get; set; }
        }

        public class CustomOptionsetValue
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        public class CustomOptionsetValueCollection
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        public class CustomEntityReference
        {
            public string Name { get; set; }
            public string LogicalName { get; set; }
            public Guid Id { get; set; }
        }

        public bool ValidateWithAPIToken(HttpRequestHeaders rqtHeaders)
        {
            bool isSessionValid = false;
            string strExceptionMsg = string.Empty;

            try
            {
                string strAPIToken = string.Empty, strRqtToken = string.Empty;

                strRqtToken = GetRequestHeaderValues(rqtHeaders, "APIToken");
                strAPIToken = ConfigurationManager.AppSettings["APIToken"];

                if (!string.IsNullOrWhiteSpace(strRqtToken) && !string.IsNullOrWhiteSpace(strAPIToken))
                {
                    strRqtToken = Crypto.DESDecrypt(strRqtToken);
                    strAPIToken = Crypto.DESDecrypt(strAPIToken);

                    if (strAPIToken.ToLower() == strRqtToken.ToLower())
                    {
                        isSessionValid = true;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(strRqtToken))
                    {
                        strExceptionMsg = "'API Token' parameter in Request Header is Null (or) Empty";
                    }
                    if (string.IsNullOrWhiteSpace(strAPIToken))
                    {
                        strExceptionMsg += "'API Token' parameter in Service Web.config file is Null (or) Empty";
                    }

                    throw new Exception(strExceptionMsg);
                }
            }
            catch (Exception exe)
            {
                throw new Exception(exe.Message);
            }

            return isSessionValid;
        }

        public bool ValidateWithAPIAuthKey(HttpRequestHeaders rqtHeaders)
        {
            bool isAPIAuthKeyValidated = false;
            string strExceptionMsg = string.Empty;
            string strDecryptedAPIAuthKey = string.Empty;

            try
            {
                string strAPIAuthKey = GetRequestHeaderValues(rqtHeaders, "APIAuthKey");

                if (!string.IsNullOrWhiteSpace(strAPIAuthKey))
                {
                    strDecryptedAPIAuthKey = Crypto.TripleDESDecrypt(strAPIAuthKey);

                    if (!string.IsNullOrWhiteSpace(strDecryptedAPIAuthKey))
                    {
                        string[] strAryAPIRqtAuthKeys = null;
                        strAryAPIRqtAuthKeys = strDecryptedAPIAuthKey.Split('|');

                        if (strAryAPIRqtAuthKeys != null && strAryAPIRqtAuthKeys.Length == 3)
                        {
                            string[] strAryWebConfigVals = GetWebConfigAPIAuthKeyValues();

                            if (strAryWebConfigVals != null && strAryWebConfigVals.Length == 3)
                            {
                                if (!string.IsNullOrWhiteSpace(strAryAPIRqtAuthKeys.ElementAt(2)) && !string.IsNullOrWhiteSpace(strAryWebConfigVals.ElementAt(2))
                                    && strAryAPIRqtAuthKeys.ElementAt(2).ToLower() == strAryWebConfigVals.ElementAt(2).ToLower())
                                {
                                    if (!string.IsNullOrWhiteSpace(strAryAPIRqtAuthKeys.ElementAt(1)) && !string.IsNullOrWhiteSpace(strAryWebConfigVals.ElementAt(1))
                                        && strAryAPIRqtAuthKeys.ElementAt(1).ToLower() == strAryWebConfigVals.ElementAt(1).ToLower())
                                    {
                                        if (!string.IsNullOrWhiteSpace(strAryAPIRqtAuthKeys.ElementAt(0)) && !string.IsNullOrWhiteSpace(strAryWebConfigVals.ElementAt(0))
                                            && strAryAPIRqtAuthKeys.ElementAt(0).ToLower() == strAryWebConfigVals.ElementAt(0).ToLower())
                                        {
                                            isAPIAuthKeyValidated = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!isAPIAuthKeyValidated)
                {
                    strExceptionMsg = "Unauthorized access: Invalid Credentails.";

                    throw new Exception(strExceptionMsg);
                }
            }
            catch (Exception exe)
            {
                if (string.IsNullOrWhiteSpace(strExceptionMsg) && !string.IsNullOrWhiteSpace(exe.Message))
                {
                    strExceptionMsg = "Unauthorized access: Invalid Credentails. " + exe.Message;
                }

                throw new Exception(strExceptionMsg);
            }

            return isAPIAuthKeyValidated;
        }

        public string GetRequestHeaderValues(HttpRequestHeaders rqtHeaders, string strHeaderkey)
        {
            string strHeaderValue = string.Empty;

            try
            {
                IEnumerable<string> strValues = new List<string>();
                HttpHeaders headers = rqtHeaders;

                if (rqtHeaders != null && !string.IsNullOrWhiteSpace(strHeaderkey))
                {
                    rqtHeaders.TryGetValues(strHeaderkey, out strValues);

                    if (strValues != null)
                    {
                        strHeaderValue = strValues.FirstOrDefault();
                    }
                }
            }
            catch (Exception e2)
            {
                throw e2;
            }

            return strHeaderValue;
        }

        public string[] GetWebConfigAPIAuthKeyValues()
        {
            //strAPIAuthKey = strAPIAuthKey.Replace("Basic ", "");
            //byte[] dataBasicAuth = Convert.FromBase64String(strAPIAuthKey);
            //string strBase64Decoded = Encoding.UTF8.GetString(dataBasicAuth);

            string[] strAryAuthValues = new string[3];
            string strAPIUsername = string.Empty, strAPIPassword = string.Empty;

            strAPIUsername = ConfigurationManager.ConnectionStrings["APIUserName"] != null ? ConfigurationManager.ConnectionStrings["APIUserName"].ConnectionString : "DfqeoEfsgoDnbNof0tp1TA==";
            strAPIPassword = ConfigurationManager.ConnectionStrings["APIPassword"] != null ? ConfigurationManager.ConnectionStrings["APIPassword"].ConnectionString : "GnkYfglb6gma7YJSE1nL1AATZJY0JHm4";

            if (!string.IsNullOrWhiteSpace(strAPIUsername))
            {
                strAryAuthValues[0] = Crypto.TripleDESDecrypt(strAPIUsername);
            }
            if (!string.IsNullOrWhiteSpace(strAPIPassword))
            {
                strAryAuthValues[1] = Crypto.TripleDESDecrypt(strAPIPassword);
            }

            DateTime dtAPIWebConfigVal = DateTime.MinValue;
            dtAPIWebConfigVal = DateTime.Now.ToUniversalTime();

            if (dtAPIWebConfigVal != DateTime.MinValue)
            {
                strAryAuthValues[2] = dtAPIWebConfigVal.ToString("yyyyMMdd");
            }

            return strAryAuthValues;
        }

        public ResponseClass SetSuccessResponse(dynamic outputResponse)
        {
            ResponseClass outputResponseClass = new ResponseClass();
            outputResponseClass.apiResult = new ResultClass();

            outputResponseClass.apidataObj = outputResponse;
            outputResponseClass.apiResult.resultCode = "0";
            outputResponseClass.apiResult.resultDes = "SUCCESS";
            //DisposeCRMService();
            return outputResponseClass;
        }

        public ResponseClass SetExceptionResponse(ExceptionResponseObjects exceptionResponseObjects)
        {
            ResponseClass outputResponseClass = new ResponseClass();
            outputResponseClass.apiResult = new ResultClass();

            outputResponseClass.apiResult.resultCode = !string.IsNullOrWhiteSpace(exceptionResponseObjects.resultDetails.resultCode) ? exceptionResponseObjects.resultDetails.resultCode : "1";
            outputResponseClass.apiResult.resultDes = exceptionResponseObjects.resultDetails.resultDes;
            outputResponseClass.apiResult.resultDesDetails = string.Format("Got Error in {0} with Full Exception Message: {1}", exceptionResponseObjects.strAPIPageName, exceptionResponseObjects.resultDetails.resultDesDetails);
            //DisposeCRMService();
            return outputResponseClass;
        }

        public void BuildLogErrorException(Exception exception, string strAPIName)
        {
            string strExceptionMsg = string.Empty;

            if (exception != null)
            {
                if (!string.IsNullOrWhiteSpace(exception.Message))
                {
                    strExceptionMsg = exception.Message;
                }
                if (!string.IsNullOrWhiteSpace(exception.StackTrace))
                {
                    strExceptionMsg += exception.StackTrace;
                }
                if (exception.InnerException != null && !string.IsNullOrWhiteSpace(exception.InnerException.Message))
                {
                    strExceptionMsg += exception.InnerException.Message;
                }
                logger.Error(string.Format("Got Error in {0} with Exception Message : {1} and Full Exception :{2}", strAPIName, strExceptionMsg, exception.ToString()));
            }
            else
            {
                logger.Error(string.Format("Got Error in {0} with Exception Message : {1} and Full Exception :{2}", strAPIName, strExceptionMsg, null));
            }
        }

        public CustomOptionsetValue SetCustomFormatOptionsetValue(Entity entTarget, string strAttributeName)
        {
            CustomOptionsetValue customOptionsetValue = null;

            OptionSetValue optstVal = crmCMs.GetAttributeValFromTargetEntity(entTarget, strAttributeName);

            if (optstVal != null && optstVal.Value != int.MinValue)
            {
                customOptionsetValue = new CustomOptionsetValue();
                customOptionsetValue.Value = optstVal.Value;
                customOptionsetValue.Text = crmCMs.GetAttributeFormattedValFromTargetEntity(entTarget, strAttributeName);
            }

            return customOptionsetValue;
        }

        public CustomOptionsetValueCollection SetCustomFormatOptionsetValueCollection(Entity entTarget, string strAttributeName)
        {
            CustomOptionsetValueCollection customOptionsetValueCollection = null;

            OptionSetValueCollection optstValColl = crmCMs.GetAttributeValFromTargetEntity(entTarget, strAttributeName);

            if (optstValColl != null)
            {
                customOptionsetValueCollection = new CustomOptionsetValueCollection();

                foreach (var optionSetValue in optstValColl)
                {
                    customOptionsetValueCollection.Value += optionSetValue.Value;
                    customOptionsetValueCollection.Text = crmCMs.GetAttributeFormattedValFromTargetEntity(entTarget, strAttributeName);
                }
            }

            return customOptionsetValueCollection;
        }

        public CustomEntityReference SetCustomFormatEntRefValue(Entity entTarget, string strAttributeName)
        {
            CustomEntityReference customEntRefValue = null;

            EntityReference entrefVal = crmCMs.GetAttributeValFromTargetEntity(entTarget, strAttributeName);

            if (entrefVal != null)
            {
                customEntRefValue = new CustomEntityReference();
                customEntRefValue.Id = entrefVal.Id;
                customEntRefValue.Name = entrefVal.Name;
                customEntRefValue.LogicalName = entrefVal.LogicalName;
            }

            return customEntRefValue;
        }

        public int FormatChannelOriginFromStringToInt(string strChannelOrigin)
        {
            int intChannelOrigin = int.MinValue;
            bool isVaildInt = false;

            if (!string.IsNullOrWhiteSpace(strChannelOrigin))
            {
                strChannelOrigin = Regex.Replace(strChannelOrigin, "[^0-9]", "");
                isVaildInt = Int32.TryParse(strChannelOrigin, out intChannelOrigin);
            }

            return intChannelOrigin;
        }

        public string FormatDateTimeAttrValToUTCDateTime(Entity entTargetRcrd, string strAttrName)
        {
            string strDTUTCFormat = string.Empty;

            if (entTargetRcrd != null && entTargetRcrd.Attributes.Contains(strAttrName))
            {
                DateTime dtFieldVal = DateTime.MinValue;
                dtFieldVal = entTargetRcrd.GetAttributeValue<DateTime>(strAttrName);

                if (dtFieldVal != null && dtFieldVal != DateTime.MinValue)
                {
                    strDTUTCFormat = dtFieldVal.ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");
                }
            }

            return strDTUTCFormat;
        }
    }
}