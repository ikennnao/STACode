using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class SendSMSPresenter
    {
        private string strConfigSMSAppId = "SendSMSAppId";
        private string strConfigSMSSenderId = "SendSMSSenderId";
        private string strConfigWrapperSrvUrl = "WrapperServiceUrl";
        private string strConfigSendSMSAPIUrl = "SendSMSAPIUrl";
        private string strConfigWrapperSrvAuthKey = "WrapperServiceAuthKey";
        private string strConfigWrapperSrvAuthVal = "WrapperServiceAuthValue";

        private JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer();
        private CommonMethods commonMethods = new CommonMethods();

        public bool CheckAndFormatSendSMSParameters(CommonWorkFlowExtensions cmWorkFlowObject, string strRecipientNo, string strSMSMessage, bool IsAPICallSuccess = false)
        {
            Entity entTargetSMS = commonMethods.RetrieveTargetEntityFromContext(cmWorkFlowObject.workflowContext);

            if (entTargetSMS == null || entTargetSMS.Id == Guid.Empty)
            {
                entTargetSMS = new Entity(cmWorkFlowObject.workflowContext.PrimaryEntityName, cmWorkFlowObject.workflowContext.PrimaryEntityId);
            }

            if (!string.IsNullOrWhiteSpace(strRecipientNo) && !string.IsNullOrWhiteSpace(strSMSMessage))
            {
                string strWrapperSrvUrl = string.Empty, strSendSMSApiUrl = string.Empty, strWrapperSrvHeaderKey = string.Empty, strWrapperSrvHeaderVal = string.Empty;
                string strSMSAPPId = string.Empty, strSMSSenderId = string.Empty, strUserName = string.Empty;

                strRecipientNo = Regex.Replace(strRecipientNo, "[^0-9]+", "");
                int zeroIndex = strRecipientNo.IndexOf('0');
                if(zeroIndex==3)
                strRecipientNo = strRecipientNo.Remove(zeroIndex, 1);
                

                #region Retrieve the User Name from CRM for the Initiating User Id

                //strUserName = Convert.ToString(cmWorkFlowObject.workflowContext.InitiatingUserId);
                strUserName = "crmadmin";

                #endregion

                #region Retrieve the required Config Parameter Records from CRM

                List<string> lststrSendSMSKeys = new List<string>() { strConfigSMSAppId, strConfigSMSSenderId, strConfigWrapperSrvUrl, strConfigSendSMSAPIUrl, strConfigWrapperSrvAuthKey, strConfigWrapperSrvAuthVal };
                Dictionary<string, string> dictConfigParams = commonMethods.RetrieveMultipleConfigParaValFromConfigParams(cmWorkFlowObject.srvContextUsr, lststrSendSMSKeys);

                #endregion

                if (dictConfigParams != null && dictConfigParams.Count > 0)
                {
                    #region Extract the Config Param Values from Config Params Records Dictionary object

                    strSMSAPPId = dictConfigParams[strConfigSMSAppId.ToLower()];
                    strSMSSenderId = dictConfigParams[strConfigSMSSenderId.ToLower()];
                    strWrapperSrvUrl = dictConfigParams[strConfigWrapperSrvUrl.ToLower()];
                    strSendSMSApiUrl = dictConfigParams[strConfigSendSMSAPIUrl.ToLower()];
                    strWrapperSrvHeaderKey = dictConfigParams[strConfigWrapperSrvAuthKey.ToLower()];
                    strWrapperSrvHeaderVal = dictConfigParams[strConfigWrapperSrvAuthVal.ToLower()];

                    #endregion

                    RqtObjUriDetails rqtObjUriDetails = null;
                    RqtCRMAuthDetails rqtCRMAuthDetails = null;
                    RqtObjSendSMS rqtObjSendSMS = null;

                    #region Set the parameters in 'UriDetails' class

                    if (!string.IsNullOrWhiteSpace(strWrapperSrvUrl) && !string.IsNullOrWhiteSpace(strSendSMSApiUrl) && !string.IsNullOrWhiteSpace(strWrapperSrvHeaderKey) && !string.IsNullOrWhiteSpace(strWrapperSrvHeaderVal))
                    {
                        rqtObjUriDetails = new RqtObjUriDetails();
                        rqtObjUriDetails.SrvUrl = strWrapperSrvUrl;
                        rqtObjUriDetails.ApiUrl = strSendSMSApiUrl;
                        rqtObjUriDetails.AuthKey = strWrapperSrvHeaderKey;
                        rqtObjUriDetails.AuthValue = strWrapperSrvHeaderVal;
                        rqtObjUriDetails.ContentType = "application/json";
                        rqtObjUriDetails.Accept = "*/*";
                        rqtObjUriDetails.Method = "POST";
                    }

                    #endregion

                    #region Set the parameters in 'RqtCRMAuthDetails' class

                    if (!string.IsNullOrWhiteSpace(strUserName))
                    {
                        rqtCRMAuthDetails = new RqtCRMAuthDetails();
                        rqtCRMAuthDetails.UserName = strUserName;
                    }

                    #endregion

                    #region Set the parameters in 'RqtObjSendSMS' class

                    if (!string.IsNullOrWhiteSpace(strSMSAPPId) && !string.IsNullOrWhiteSpace(strSMSSenderId))
                    {
                        rqtObjSendSMS = new RqtObjSendSMS();
                        rqtObjSendSMS.AppId = strSMSAPPId;
                        rqtObjSendSMS.SenderId = strSMSSenderId;
                        rqtObjSendSMS.Message = strSMSMessage;
                        rqtObjSendSMS.Recipient = Convert.ToInt64(strRecipientNo);
                    }

                    #endregion

                    string strWrapperAPIResponse = RqtSerializeAndSendSMSToWrapperAPI(rqtObjUriDetails, rqtCRMAuthDetails, rqtObjSendSMS);

                    SendSMSResponse rspSendSMSAPI = null;
                    rspSendSMSAPI = DeSerializeAndFormatAPIResponse(strWrapperAPIResponse, rspSendSMSAPI);

                    if (rspSendSMSAPI != null)
                    {
                        IsAPICallSuccess = UpdateSMSAPIRspInTargetSMSActivity(cmWorkFlowObject.srvContextUsr, entTargetSMS, rspSendSMSAPI, IsAPICallSuccess);
                    }
                }
            }

            return IsAPICallSuccess;
        }

        private string RqtSerializeAndSendSMSToWrapperAPI(RqtObjUriDetails rqtObjUri, RqtCRMAuthDetails rqtCRMAuthDetails, RqtObjSendSMS rqtObjSMS)
        {
            string strResponseText = string.Empty;

            if (rqtObjUri != null && rqtObjSMS != null)
            {
                string strSerializedRqtObj = string.Empty;

                SendSMSRequest sendSMSRequest = new SendSMSRequest();
                sendSMSRequest.objSendSMS = rqtObjSMS;
                sendSMSRequest.authenticateDetails = rqtCRMAuthDetails;

                strSerializedRqtObj = JavaScriptSerializer.Serialize(sendSMSRequest);

                if (!string.IsNullOrWhiteSpace(strSerializedRqtObj))
                {
                    byte[] rqtobjByteArray = Encoding.UTF8.GetBytes(strSerializedRqtObj);

                    // Create a request using a URL that can receive a post.   
                    HttpWebRequest rqtAuthSession = (HttpWebRequest)WebRequest.Create(rqtObjUri.SrvUrl + rqtObjUri.ApiUrl);
                    rqtAuthSession.Headers.Add(rqtObjUri.AuthKey, rqtObjUri.AuthValue);
                    rqtAuthSession.ContentType = rqtObjUri.ContentType;
                    rqtAuthSession.Accept = rqtObjUri.Accept;
                    rqtAuthSession.Method = rqtObjUri.Method;
                    rqtAuthSession.ContentLength = rqtobjByteArray.Length;

                    // Create a request stream which holds request data
                    Stream rqtStream = rqtAuthSession.GetRequestStream();
                    //Write the memory stream data into stream object before send it.            
                    rqtStream.Write(rqtobjByteArray, 0, rqtobjByteArray.Length);
                    // Close the Stream object.
                    rqtStream.Close();

                    HttpWebResponse rspAuthSession = (HttpWebResponse)rqtAuthSession.GetResponse();

                    using (rqtStream = rspAuthSession.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader rspStreamReader = new StreamReader(rqtStream);
                        // Read the content.
                        strResponseText = rspStreamReader.ReadToEnd();
                    }

                    // Close the response.
                    rspAuthSession.Close();
                }
            }

            return strResponseText;
        }

        private SendSMSResponse DeSerializeAndFormatAPIResponse(string strAPIResponse, SendSMSResponse rspSendSMSAPI)
        {
            rspSendSMSAPI = new SendSMSResponse();

            if (!string.IsNullOrWhiteSpace(strAPIResponse))
            {
                ResponseClass deserializedRsp = null;
                deserializedRsp = JavaScriptSerializer.Deserialize<ResponseClass>(strAPIResponse);

                if (deserializedRsp != null)
                {
                    if (deserializedRsp.apiResult != null && deserializedRsp.apiResult.resultCode == "0")
                    {
                        if (deserializedRsp.apidataObj != null)
                        {
                            Dictionary<string, object> dictApiRspDataObj = deserializedRsp.apidataObj;

                            if (dictApiRspDataObj != null && dictApiRspDataObj.Count > 0)
                            {
                                rspSendSMSAPI.IsSMSSuccess = dictApiRspDataObj.ContainsKey("IsSMSSuccess") ? Convert.ToBoolean(dictApiRspDataObj["IsSMSSuccess"]) : false;
                                rspSendSMSAPI.ResponseMsg = dictApiRspDataObj.ContainsKey("ResponseMsg") ? Convert.ToString(dictApiRspDataObj["ResponseMsg"]) : string.Empty;
                                rspSendSMSAPI.ResponseCode = dictApiRspDataObj.ContainsKey("ResponseCode") ? Convert.ToString(dictApiRspDataObj["ResponseCode"]) : string.Empty;

                                dynamic dySMSAPIRspDataObj = null;
                                dySMSAPIRspDataObj = dictApiRspDataObj["ResponseData"];

                                Dictionary<string, object> dictSMSApiRspData = null;

                                if (dySMSAPIRspDataObj != null || dySMSAPIRspDataObj != "")
                                {
                                    dictSMSApiRspData = dySMSAPIRspDataObj;
                                }

                                if (dictSMSApiRspData != null && dictSMSApiRspData.Count > 0)
                                {
                                    string strApiRspData = string.Empty;

                                    foreach (KeyValuePair<string, object> smsApiRspData in dictSMSApiRspData)
                                    {
                                        strApiRspData += smsApiRspData.Key + ": " + smsApiRspData.Value + Environment.NewLine;
                                    }

                                    rspSendSMSAPI.ResponseData = !string.IsNullOrWhiteSpace(strApiRspData) ? strApiRspData : string.Empty;

                                    rspSendSMSAPI.APIMessageID = dictSMSApiRspData.ContainsKey("MessageID") ? Convert.ToString(dictSMSApiRspData["MessageID"]) : string.Empty;
                                }
                            }
                        }
                        else
                        {
                            rspSendSMSAPI.IsSMSSuccess = false;
                            rspSendSMSAPI.ResponseMsg = "API Data Object in Response Class of Wrapper Service 'Null' or 'Empty'.";
                            rspSendSMSAPI.ResponseCode = deserializedRsp.apiResult.resultCode;
                            rspSendSMSAPI.ResponseData = deserializedRsp.apiResult.resultDes + Environment.NewLine + deserializedRsp.apiResult.resultDesDetails;
                        }
                    }
                    else
                    {
                        rspSendSMSAPI.IsSMSSuccess = false;

                        if (deserializedRsp.apiResult == null)
                        {
                            rspSendSMSAPI.ResponseMsg = "API Result in Response Class of Wrapper Service 'Null' or 'Empty'.";
                        }
                        else if (deserializedRsp.apiResult != null)
                        {
                            rspSendSMSAPI.ResponseCode = deserializedRsp.apiResult.resultCode;
                            rspSendSMSAPI.ResponseMsg = deserializedRsp.apiResult.resultDes;
                            rspSendSMSAPI.ResponseData = deserializedRsp.apiResult.resultDesDetails;
                        }
                    }
                }
                else
                {
                    rspSendSMSAPI.IsSMSSuccess = false;
                    rspSendSMSAPI.ResponseMsg = "Deserialized Response Class of Wrapper Service 'Null' or 'Empty'.";
                }
            }
            else
            {
                rspSendSMSAPI.IsSMSSuccess = false;
                rspSendSMSAPI.ResponseMsg = "API Response String Object of Wrapper Service 'Null' or 'Empty'.";
            }

            return rspSendSMSAPI;
        }

        private bool UpdateSMSAPIRspInTargetSMSActivity(IOrganizationService orgService, Entity entTargetSMSActivity, SendSMSResponse rspSendSMSAPI, bool IsAPICallSuccess)
        {
            if (entTargetSMSActivity != null && entTargetSMSActivity.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entTargetSMSActivity.LogicalName) && rspSendSMSAPI != null)
            {
                Entity entUpdateSMSActivity = new Entity(entTargetSMSActivity.LogicalName, entTargetSMSActivity.Id);
                entUpdateSMSActivity.Attributes[SMSActivityEntityAttributeNames.IsSMSAPISuccess] = rspSendSMSAPI.IsSMSSuccess;
                if (rspSendSMSAPI.IsSMSSuccess)
                {
                    entUpdateSMSActivity.Attributes[SMSActivityEntityAttributeNames.DateSent] = DateTime.UtcNow;
                }
                entUpdateSMSActivity.Attributes[SMSActivityEntityAttributeNames.APIResponseMessage] = rspSendSMSAPI.ResponseMsg;
                entUpdateSMSActivity.Attributes[SMSActivityEntityAttributeNames.APIResponseCode] = rspSendSMSAPI.ResponseCode;
                entUpdateSMSActivity.Attributes[SMSActivityEntityAttributeNames.APIResponseMessageID] = rspSendSMSAPI.APIMessageID;
                entUpdateSMSActivity.Attributes[SMSActivityEntityAttributeNames.APIResponseData] = rspSendSMSAPI.ResponseData;
                orgService.Update(entUpdateSMSActivity);
                IsAPICallSuccess = rspSendSMSAPI.IsSMSSuccess;
            }
            else
            {
                IsAPICallSuccess = false;
            }
            return IsAPICallSuccess;
        }
    }

    public class RqtObjUriDetails
    {
        public string SrvUrl { get; set; }
        public string ApiUrl { get; set; }
        public string AuthKey { get; set; }
        public string AuthValue { get; set; }
        public string ContentType { get; set; }
        public string Accept { get; set; }
        public string Method { get; set; }
    }

    public class SendSMSRequest
    {
        public RqtObjSendSMS objSendSMS { get; set; }
        public RqtCRMAuthDetails authenticateDetails { get; set; }
    }

    public class RqtObjSendSMS
    {
        public string AppId { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
        public long Recipient { get; set; }
    }

    public class RqtCRMAuthDetails
    {
        public string UserName { get; set; }
        public string ChannelId { get; set; }
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

    public class SendSMSResponse
    {
        public bool IsSMSSuccess { get; set; }
        public string ResponseMsg { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseData { get; set; }
        public string APIMessageID { get; set; }
    }

    public class ResponseDataObj
    {
        public string MessageID { get; set; }
        public string CorrelationID { get; set; }
        public int NumberOfUnits { get; set; }
        public int Cost { get; set; }
        public int Balance { get; set; }
        public string Recipient { get; set; }
        public string TimeCreated { get; set; }
        public string CurrencyCode { get; set; }
    }
}