using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class RetrieveSocialMediaHandleRequest
    {
        public RetrieveSMHObj RetrieveSMHobj { get; set; }
        public AuthenticateRequest AuthenticateDetails { get; set; }
    }

    public class RetrieveSMHObj
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public Guid SocialMediaHandleGuid { get; set; }
        public CustomEntityReference Customer { get; set; }
        public CustomEntityReference SocialChannel { get; set; }
    }

    public class RetrieveSocialMediaHandleResponse
    {
        public string SocialMediaHandleETN { get; set; }
        public List<RspRetrieveSMHObj> LstSMHRecords { get; set; }
    }

    public class RspRetrieveSMHObj
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public Guid SocialMediaHandleGuid { get; set; }
        public CustomEntityReference Customer { get; set; }
        public CustomEntityReference SocialChannel { get; set; }
    }

    public class CreateSocialMediaHandleRequest
    {
        public CreateSMH CreateSMH { get; set; }
        public AuthenticateRequest AuthenticateDetails { get; set; }
    }

    public class CreateSocialMediaHandleResponse
    {
        public string UserName { get; set; }
        public Guid SocialMediaHandleGuid { get; set; }
        public string EntityLogicalName { get; set; }
        public string ExceptionDetails { get; set; }
    }

    public class CreateSMH
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public CustomEntityReference Customer { get; set; }
        public CustomEntityReference SocialChannel { get; set; }
    }
}