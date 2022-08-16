using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;

namespace TC.WrapperService.Models
{

    public class RetrieveCompaniesRequest
    {
       public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCompanies
    {
        public string CompanyName { get; set; }
        public Guid CompanyGuid { get; set; }
    }

    public class RetrieveCompaniesResponse
    {
        public string accountTypeETN { get; set; }
        public List<RetrieveCompanies> accountDataObj { get; set; }
    }


    public class CreateCompanyResponse
    {
        public string CompanyName { get; set; }
        public Guid CompanyGuid { get; set; }
        public string exceptionDetails { get; set; }
    }

    public class CreateCompanyRequest
    {
        public CreateCompany createCompany { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class CreateCompany
    {
        public string CompanyName { get; set; }
        public CustomEntityReference Region { get; set; }
        public CustomEntityReference SubRegion { get; set; }
        public CustomEntityReference City { get; set; }
        public CustomEntityReference Country { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyWebSite { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string MTCRNumber { get; set; }
        public string MTCRMID { get; set; }
        public string CompanyNameAR { get; set; }
        public int? PreferredContactMethod { get; set; }
        public CustomEntityReference ParentAccount { get; set; }

    }

    public class UpdateCompanyRequest
    {
        public UpdateCompany updateCompany { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class UpdateCompany
    {
        public string CompanyName { get; set; }
        public Guid CompanyGuid { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string MTCRNumber { get; set; }
        public string MTCRMID { get; set; }
        public string CompanyNameAR { get; set; }
        public int? PreferredContactMethod { get; set; }
        public CustomEntityReference ParentAccount { get; set; }

        //public CustomEntityReference Region { get; set; }
        //public CustomEntityReference SubRegion { get; set; }
        //public CustomEntityReference City { get; set; }
        //public CustomEntityReference Country { get; set; }
    }

    public class UpdateCompanyResponse
    {
        public Guid CompanyGuid { get; set; }
        public string exceptionDetails { get; set; }
    }

    public class RetrieveCompanyRequest
    {
        public RetrieveCompanyObj retrieveCompany { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCompanyObj
    {
        public string CompanyName { get; set; }
        public Guid CompanyGuid { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string MTCRNumber { get; set; }
        public string MTCRMID { get; set; }
        public string CompanyNameAR { get; set; }
        public int? PreferredContactMethod { get; set; }
        public CustomEntityReference ParentAccount { get; set; }

    }

    public class RetrieveCompanyResponse
    {
        public string companyETN { get; set; }
        public List<RspRetrieveCompanyObj> LstCompanyRecords { get; set; }
    }

    public class RspRetrieveCompanyObj
    {
        public string CompanyName { get; set; }
        public Guid CompanyGuid { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string MTCRNumber { get; set; }
        public string MTCRMID { get; set; }
        public string CompanyNameAR { get; set; }
        public CustomEntityReference Region { get; set; }
        public CustomEntityReference SubRegion { get; set; }
        public CustomEntityReference City { get; set; }
        public CustomEntityReference Country { get; set; }
        public int? PreferredContactMethod { get; set; }

        public CustomEntityReference ParentAccount { get; set; }
        

    }
}