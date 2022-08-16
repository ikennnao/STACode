using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class RetrieveCustomerCategoryRequest
    {
        public RetrieveCustomerCategoryObj retrieveCustomerCategory { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCustomerCategoryObj
    {
        public string CategoryId { get; set; }
        public string Status { get; set; }
    }

    public class RetrieveCustomerCategory
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Guid CustomerCategoryGuid { get; set; }
    }

    public class RetrieveCustomerCategoryResponse
    {
        public string customerCategoryETN { get; set; }
        public List<RetrieveCustomerCategory> customerCategoryDataObj { get; set; }
    }
}