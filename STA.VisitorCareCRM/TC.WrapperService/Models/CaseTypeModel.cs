using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveCaseTypeRequest
    {
        public RetrieveCaseType retrieveCaseType { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCaseType
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Guid CaseTypeGuid { get; set; }
    }

    public class RetrieveCaseTypeResponse
    {
        public string caseTypeETN { get; set; }
        public List<RetrieveCaseType> caseTypeDataObj { get; set; }
    }
}