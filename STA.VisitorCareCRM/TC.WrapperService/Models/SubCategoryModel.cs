using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveSubCategoryRequest
    {
        public rqtObjRetrieveSubCategory retrieveSubCategory { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class rqtObjRetrieveSubCategory
    {
        public Guid CategoryGuid { get; set; }
    }

    public class RetrieveSubCategoryResponse
    {
        public string subcategoryETN { get; set; }
        public List<rspObjRetrieveSubCategory> subcategoryDataObj { get; set; }
    }

    public class rspObjRetrieveSubCategory
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public CustomEntityReference Category { get; set; }
        public Guid SubCategoryGuid { get; set; }
    }
}