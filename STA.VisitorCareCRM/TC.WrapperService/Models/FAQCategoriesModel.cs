using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class RetrieveFAQCategoriesRequest
    {
        public AuthenticateRequest authenticateDetails { get; set; }
    }

  

    public class RetrieveFAQCategoriesResponse
    {
      
        public List<rspObjRetrieveFAQCategories> categoryDataObj { get; set; }
    }

    public class rspObjRetrieveFAQCategories
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public int Value { get; set; }
        
    }
}