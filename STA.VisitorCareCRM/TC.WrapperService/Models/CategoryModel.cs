using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class RetrieveCategoryRequest
    {
        public rqtObjRetrieveCategory retrieveCategory { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class rqtObjRetrieveCategory
    {
        public Guid CaseTypeId { get; set; }
        public Guid CustomerCatgeoryId { get; set; }
    }

    public class RetrieveCategoryResponse
    {
        public string categoryETN { get; set; }
        public List<rspObjRetrieveCategory> categoryDataObj { get; set; }
    }

    public class rspObjRetrieveCategory
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Guid CategoryGuid { get; set; }
        public CustomEntityReference CaseType { get; set; }
    }
}