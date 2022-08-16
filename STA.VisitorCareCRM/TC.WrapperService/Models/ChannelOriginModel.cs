using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class RetrieveChannelOriginRequest
    {
        public RqtObjChannelOrigin rqtobjChannelOrigin { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RqtObjChannelOrigin
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Guid ChannelOriginGuid { get; set; }
        public int ChannelType { get; set; }
        public string ChannelOriginCode { get; set; }
    }

    public class RetrieveChannelOriginResponse
    {
        public string channelOriginETN { get; set; }
        public List<RspObjChannelOrigin> channelOriginDataObj { get; set; }
    }

    public class RspObjChannelOrigin
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Guid ChannelOriginGuid { get; set; }
        public CustomOptionsetValue ChannelType { get; set; }
        public string ChannelOriginCode { get; set; }
    }
}