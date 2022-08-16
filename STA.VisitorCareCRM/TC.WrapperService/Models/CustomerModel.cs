using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.CRMCommonMethods;
using TC.WrapperService.Resources;

namespace TC.WrapperService.Models
{
    public class CreateCustomerRequest
    {
        public CreateCustomer createCustomer { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class CreateCustomerResponse
    {
        public string FullName { get; set; }
        public string AppRecordUrl { get; set; }
        public Guid CustomerGuid { get; set; }
        public string EntityLogicalName { get; set; }
        public string ExceptionDetails { get; set; }
    }

    public class CreateCustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryContactNo { get; set; }
        public RqtObjSocialHandleDetails SMHDetails { get; set; }
        public string ChannelOrigin { get; set; }
        public int? LeadType { get; set; }
        public string SaudiTourismContent { get; set; }
        public string CapabilityBuildingProgram { get; set; }
        public CustomEntityReference Country { get; set; }
        public int BusinessType { get; set; }
        public CustomEntityReference CustomerCategory { get; set; }
        public List<CustomEntityReference> InternationalMarkets { get; set; }
        public CustomEntityReference CompanyName { get; set; }
    }

    public class RetrieveCustomerRequest
    {
        public RetrieveCustomerObj retrieveCustomer { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class RetrieveCustomerObj
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ChannelOrigin { get; set; }
        public RqtObjSocialHandleDetails SMHDetails { get; set; }
        public Guid CustomerGuid { get; set; }
        public string Status { get; set; }

    }

    public class RetrieveCustomerResponse
    {
        public string customerETN { get; set; }
        public List<RspRetrieveCustomerObj> lstCustomerRecords { get; set; }
        public List<RspRetrieveContactObj> _firstContactRecord { get; set; }
    }

    public class RspRetrieveCustomerObj
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryContactNo { get; set; }
        public string SecondaryEmail { get; set; }
        public string SecondaryContactNo { get; set; }
        public CustomEntityReference ChannelOrigin { get; set; }
        public string AppRecordUrl { get; set; }
        public CustomEntityReference CustomerCategroy { get; set; }
        public CustomOptionsetValue Status { get; set; }
        public Guid CustomerGuid { get; set; }
        public string SSID { get; set; }
        public string TravelPartner { get; set; }
        public CustomOptionsetValueCollection SSIDInterest { get; set; }


    }

    public class RspRetrieveContactObj
    {
        public Guid CustomerGuid { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public UpdateCustomer updateCustomer { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }


    public class UpdateCustomer
    {
        public int? LeadType { get; set; }
        public Guid CustomerGuid { get; set; }
        public string SSID { get; set; }
        public int[] SSIDInterest { get; set; }
        public string TravelPartner { get; set; }

    }

    public class UpdateCustomerResponse
    {
        public Guid CustomerGuid { get; set; }
        public string exceptionDetails { get; set; }
    }
}