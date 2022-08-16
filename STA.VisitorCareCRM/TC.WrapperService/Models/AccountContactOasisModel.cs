using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{
    public class CreateAccountContactOasisResponse
    {
        public string AccountId { get; set; }
        public string ContactId { get; set; }
        public string AccountNumber { get; set; }
    }
    public class CreateAccountContactOasisRequest
    {
        public CreateAccountContactOasis createAccountContactOasis { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }
    public enum OasisType 
    {
         Domestic = 948120000,
        International = 948120001
    }
    public class CreateAccountContactOasis
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalEmailAddress { get; set; }
        public string ProfileImage { get; set; }
        public string OasisType { get; set; }
        public string InternationalMarket { get; set; }
        public string CompanyEmailAddress { get; set; }
        public CustomEntityReference Country { get; set; }
        public string MobileNumber { get; set; }
        public string CompanyName { get; set; }
        public int CompanyType { get; set; } 
        public string Website { get; set; }
        public string Address { get; set; }
    }
}