using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class SendSMSRequest
    {
        public rqtObjSendSMS objSendSMS { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class rqtObjSendSMS
    {
        public string AppId { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
        public long Recipient { get; set; }
    }

    public class SendSMSResponse
    {
        public bool IsSMSSuccess { get; set; }
        public string ResponseMsg { get; set; }
        public string ResponseCode { get; set; }
        public object ResponseData { get; set; }
    }

    public class SMSResponseDataObj
    {
        public string MessageID { get; set; }
    }
}