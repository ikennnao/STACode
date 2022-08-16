using System;
using System.Collections.Generic;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.CRMCommonMethods;

namespace TC.WrapperService.Models
{
    public class CreateCaseRequest
    {
        public CreateCaseObj createCase { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }

    public class CreateCaseResponse
    {
        public string CaseRefNo { get; set; }
        public string AppRecordUrl { get; set; }
        public Guid CaseGuid { get; set; }
    }

    public class CreateCaseObj
    {
        public RqtObjSocialHandleDetails SMHDetails { get; set; }
        public string ChannelRefNo { get; set; }
        public CustomEntityReference Customer { get; set; }
        public CustomEntityReference CaseType { get; set; }
        public CustomEntityReference Category { get; set; }
        public CustomEntityReference SubCategory { get; set; }
        public string CaseDescription { get; set; }
        public string ChannelOrigin { get; set; }
        public string SMPostText { get; set; }
        public DateTime SMPostDateTime { get; set; }
    }

    public class RetrieveCaseRequest
    {
        public RetrieveCase retrieveCase { get; set; }
        public AuthenticateRequest authenticateDetails { get; set; }
    }
   
    public class RetrieveCaseResponse
    {
        public string caseETN { get; set; }
        public List<CaseResponseObj> lstCaseRecords { get; set; }
    }

    public class RetrieveCase
    {
        public string CaseRefNo { get; set; }
        public string ChannelRefNo { get; set; }
        public RqtObjSocialHandleDetails SMHDetails { get; set; }
        public string ChannelOrigin { get; set; }
        public Guid CaseGuid { get; set; }
        public string CustomerEmail { get; set; }
        public CustomOptionsetValue Stauts { get; set; }
    }
 

    public class CaseResponseObj
    {
        public string CaseRefNo { get; set; }
        public string ChannelRefNo { get; set; }
        public CustomEntityReference Customer { get; set; }
        public CustomEntityReference CaseType { get; set; }
        public CustomEntityReference Category { get; set; }
        public CustomEntityReference SubCategory { get; set; }
        public string CaseDescription { get; set; }
        public CustomEntityReference CaseOriginChannel { get; set; }
        public CustomOptionsetValue CasePriority { get; set; }
        public CustomOptionsetValue Status { get; set; }
        public Guid CaseGuid { get; set; }
        public string AppRecordUrl { get; set; }
        public string CreatedOn { get; set; }
        public string ResolutionComments{ get; set; }
        public CustomEntityReference CreatedBy { get; set; }
    }
}