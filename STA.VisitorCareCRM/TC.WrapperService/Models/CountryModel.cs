using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveCountriesRequest
    {
       public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCountries
    {
        public string CountryName { get; set; }
        public string ISO { get; set; }
        public Guid CountryGuid { get; set; }
        public Guid RegionGuid { get; set; }
    }

    public class RetrieveCountriesResponse
    {
        public string countryTypeETN { get; set; }
        public List<RetrieveCountries> countriesDataObj { get; set; }
    }
}