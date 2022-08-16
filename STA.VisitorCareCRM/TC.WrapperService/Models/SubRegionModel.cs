using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveSubRegionsRequest
    {
       public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveSubRegions
    {
        public string SubRegionName { get; set; }
        public string RegionName { get; set; }
        public Guid SubRegionGuid { get; set; }
        public Guid RegionGuid { get; set; }
    }

    public class RetrieveSubRegionsResponse
    {
        public string SubRegionTypeETN { get; set; }
        public List<RetrieveSubRegions> SubregionDataObj { get; set; }
    }
}