using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveRegionsRequest
    {
       public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveRegions
    {
        public string RegionName { get; set; }
        public Guid RegionGuid { get; set; }
    }

    public class RetrieveRegionsResponse
    {
        public string RegionTypeETN { get; set; }
        public List<RetrieveRegions> regionDataObj { get; set; }
    }
}