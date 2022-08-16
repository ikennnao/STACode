using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveCitiesRequest
    {
       public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCities
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public Guid CityGuid { get; set; }
        public Guid CountryGuid { get; set; }
    }

    public class RetrieveCitiesResponse
    {
        public string citiesTypeETN { get; set; }
        public List<RetrieveCities> citiesDataObj { get; set; }
    }
}