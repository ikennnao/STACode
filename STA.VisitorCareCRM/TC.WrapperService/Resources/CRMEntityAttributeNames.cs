namespace TC.WrapperService.Resources
{
    #region Case Entity Attributes Names

    public sealed class CaseEntityAttributeNames
    {
        public const string EntityLogicalName = "incident";
        public const string CaseId = "incidentid";
        public const string Customer = "customerid";
        public const string TcktNo = "ticketnumber";
        public const string CaseTitle = "title";
        public const string Description = "description";
        public const string CaseOrigin = "caseorigincode";
        public const string PriorityCode = "prioritycode";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string Owner = "ownerid";
        public const string ResolutionComments = "tc_resolutioncomments";
        public const string CaseType = "tc_casetype";
        public const string Category = "tc_category";
        public const string SubCatgeory = "tc_subcategory";
        public const string IsSubmitted = "tc_issubmitted";
        public const string ChannelOrigin = "tc_channelorigin";
        public const string SubmittedBy = "tc_submittedby";
        public const string SubmittedOn = "tc_submittedon";
        public const string CaseProcessConfig = "tc_caseprocessconfig";
        public const string PendingWith = "tc_pendingwith";
        public const string SLAConfig = "tc_slaconfig";
        public const string WarnAfter = "tc_warnafter";
        public const string EscalateAfter = "tc_escalateafter";
        public const string SLAStatus = "resolvebyslastatus";
        public const string CaseWorkedBy = "tc_caseworkedby";
        public const string CaseWorkedOn = "tc_caseworkedon";
        public const string CaseRefNo = "tc_caserefno";
        public const string AppRecordUrl = "tc_apprecordurl";
        public const string SocialMediaHandle = "tc_socialmediahandle";
        public const string ChannelRefNo = "tc_externalrefno";
        public const string SMPostDateTime = "tc_socialmediapostdatetime";
        public const string SMPostText = "tc_socialmediaposttext";
    }

    #endregion

    #region CaseType Entity Attributes Names

    public sealed class CaseTypeEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_casetype";
        public const string CaseTypeId = "tc_casetypeid";
        public const string CaseTypeName = "tc_name";
        public const string CaseTypeArabicName = "tc_arabicname";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Countries Entity Attributes Names

    public sealed class CountryEntityAttributeNames
    {
        public const string EntityLogicalName = "cc_country";
        public const string CountryId = "cc_countryid";
        public const string CountryName = "cc_name";
        public const string CountryArabicName = "cc_arabicname";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string ISO = "cc_isocode";
        public const string RegionId = "cc_region";
    }

    #endregion

    #region Cities Entity Attributes Names

    public sealed class CitiesEntityAttributeNames
    {
        public const string EntityLogicalName = "cc_city";
        public const string CityName = "cc_name";
        public const string CityGuid = "cc_cityid";
        public const string Country = "cc_country";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Region Entity Attributes Names

    public sealed class RegionsEntityAttributeNames
    {
        public const string EntityLogicalName = "cc_region";
        public const string RegionId = "cc_regionid";
        public const string RegionName = "cc_name";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region SubRegion Entity Attributes Names

    public sealed class SubRegionsEntityAttributeNames
    {
        public const string EntityLogicalName = "cc_subregion";
        public const string SubRegionId = "cc_subregionid";
        public const string SubRegionName = "cc_name";
        public const string RegionId = "cc_regionid";
        public const string Region = "cc_region";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Category Entity Attributes Names

    public sealed class CaseCategoryEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_casecategory";
        public const string CaseCategoryId = "tc_casecategoryid";
        public const string CaseCategoryName = "tc_name";
        public const string CaseCategoryArabicName = "tc_arabicname";
        public const string CaseType = "tc_casetype";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Case Subcategory Entity Attributes Names

    public sealed class CaseSubCategoryEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_casesubcategory";
        public const string CaseSubcategoryId = "tc_casesubcategoryid";
        public const string CaseSubcategoryName = "tc_name";
        public const string CaseSubCategoryArabicName = "tc_arabicname";
        public const string CaseCategory = "tc_casecategory";
        public const string DefaultPriority = "tc_defaultpriority";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Case Process Configuration Entity Attributes Names

    public sealed class CaseProcessConfigEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_caseprocessconfiguration";
        public const string CaseType = "tc_casetype";
        public const string Category = "tc_category";
        public const string Subcatgeory = "tc_subcategory";
        public const string CasePriority = "tc_casepriority";
        public const string AssignTo = "tc_assignto";
        public const string User = "tc_user";
        public const string Team = "tc_team";
        public const string Queue = "tc_queue";
        public const string SLAConfig = "tc_slaconfiguration";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region SLA Configuration Entity Attributes Names

    public sealed class SLAConfigEntityAttributeNames
    {
        public const string EntityLogicalName = "incident";
        public const string InitiateFrom = "tc_initiatefrom";
        public const string WarnAfter = "tc_warnafter";
        public const string EscalateAfter = "tc_escalateafter";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region SystemUser Entity Attributes Names

    public sealed class SystemUserEntityAttributeNames
    {
        public const string EntityLogicalName = "systemuser";
        public const string SystemUserId = "systemuserid";
        public const string DomainName = "domainname";
        public const string PrimaryEmail = "internalemailaddress";
        public const string IsDisabled = "isdisabled";
    }

    #endregion

    #region Team Entity Attributes Names

    public sealed class TeamEntityAttributeNames
    {
        public const string EntityLogicalName = "team";
        public const string TeamId = "teamid";
        public const string DefaultQueue = "queueid";
        public const string TeamName = "name";
    }

    #endregion

    #region Role Entity Attributes Names

    public sealed class RoleEntityAttributeNames
    {
        public const string EntityLogicalName = "role";
        public const string RoleId = "roleid";
        public const string RoleName = "name";
    }

    #endregion

    #region SystemUserRole Entity Attributes Names

    public sealed class SystemUserRoleEntityAttributeNames
    {
        public const string EntityLogicalName = "systemuserrole";
        public const string SystemUserRoleId = "systemuserroleid";
        public const string RoleId = "roleid";
        public const string SystemUserId = "systemuserid";
    }

    #endregion

    #region Queue Item Entity Attributes Names

    public sealed class QueueItemEntityAttributeNames
    {
        public const string EntityLogicalName = "queueitem";
        public const string QueueItemId = "queueitemid";
        public const string ObjectId = "objectid";
        public const string WorkedBy = "workerid";
        public const string WorkedOn = "workeridmodifiedon";
        public const string ObjectTypeCode = "objecttypecode";
        public const string CreatedOn = "createdon";
        public const string ModifiedOn = "modifiedon";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Contact Entity Attributes Names
    public sealed class ContactEntityAttributesNames
    {
        public const string EntityLogicalName = "contact";
        public const string CustomerId = "contactid";
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string FullName = "fullname";
        public const string PrimaryEmail = "emailaddress1";
        public const string PrimaryContactNo = "mobilephone";
        public const string SecondaryEmail = "emailaddress2";
        public const string SecondaryContactNo = "telephone2";
        public const string ChannelOrigin = "tc_channelorigin";
        public const string CustomerCategory = "tc_customercategory";
        public const string AppRecordUrl = "tc_apprecordurl";
        public const string Status = "statecode";
        public const string CapabilityBuildingProgram = "cc_capabilitybuildingprogram";
        public const string SaudiTourismContent = "cc_sauditourismcontent";
        public const string LeadType = "cc_leadtype";
        public const string Country = "cc_country";
        public const string Region = "cc_region";
        public const string Account = "parentcustomerid";
        public const string BusinessType = "cc_businesstype";
        public const string Owner = "ownerid";
        public const string OwningTeam = "owningteam";
        public const string OwningUser = "owninguser";
        public const string SSID = "tc_ssid";
        public const string SSIDInterest = "tc_ssidinterest";
        public const string TravelPartner = "tc_travelpartner";
    }

    #endregion

    #region Account Entity Attributes Names
    public sealed class AccountEntityAttributesNames
    {
        public const string EntityLogicalName = "account";
        public const string CompanyId = "accountid";
        public const string CompanyName = "name";
        public const string BusinessType = "cc_businesstype";
        public const string Status = "statecode";
        public const string Region = "cc_region";
        public const string SubRegion = "cc_subregion";
        public const string City = "cc_city";
        public const string Country = "cc_country";
        public const string CompanyPhone = "telephone1";
        public const string CompanyAddress = "address1_line1";
        public const string CompanyWebSite = "websiteurl";
        public const string CompanyEmail = "emailaddress1";
        public const string Owner = "ownerid"; 
        public const string Type = "cc_businesstype";
        public const string CompanyType = "cc_typeofaccount";
        public const string OasisTypes = "cc_oasistype";
        public const string InternationalMaarket = "cc_oasisinternationalmarket";
        public const string AccountNumber = "accountnumber";
        public const string CompanyRegistrationNumber = "tc_companyregistrationnumber";
        public const string MTCRNumber = "tc_mtcrnumber";
        public const string MTCRMID = "tc_mtcrmid";
        public const string PreferredContactMethod = "preferredcontactmethodcode";
        public const string ParentAccount = "parentaccountid";
        public const string CompanyNameAR = "tc_facilitynamearabic";
    }

    #endregion

    #region Social Media Channel Entity Attributes Names
    public sealed class SocialMediaHandleEntityAttributesNames
    {
        public const string EntityLogicalName = "tc_socialmediachannel";
        public const string SocialMediaHandleID = "tc_socialmediachannelid";
        public const string SocialUserName = "tc_name";
        public const string SocialHandle = "tc_channelhandle";
        public const string SocialUserID = "tc_socialuserid";
        public const string Customer = "tc_customer";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }
    #endregion

    #region Conversation Entity Attributes Names
    public sealed class ConversationEntityAttributesNames
    {
        public const string EntityLogicalName = "tc_conversation";
        public const string ConversationID = "tc_conversationid";
        public const string Subject = "tc_name";
        public const string FirstName = "tc_firstname";
        public const string ExistingCase = "tc_existingcase";
        public const string LastName = "tc_lastname";
        public const string EmailAddress = "tc_emailaddress";
        public const string IncomingMessage = "tc_incomingmessage";
        public const string ChannelOrigin = "tc_channelorigin";
        public const string MobileNumber = "tc_contactnumber";
        public const string Customer = "tc_customer";
        public const string CaseType = "tc_casetype";
    }
    #endregion

    #region  CustomerCategory Entity Attributes Names

    public sealed class CustomerCategoryEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_customercategory";
        public const string CustomerCategoryId = "tc_customercategoryid";
        public const string CustomerCategoryName = "tc_name";
        public const string CustomerCategoryArabicName = "tc_arabicname";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region  ChannelOrigin Entity Attributes Names

    public sealed class ChannelOriginEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_channelorigin";
        public const string ChannelOriginId = "tc_channeloriginid";
        public const string ChannelOriginName = "tc_name";
        public const string ChannelOriginArabicName = "tc_arabicname";
        public const string ChannelOriginType = "tc_channeltype";
        public const string ChannelOriginCode = "tc_origincode";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion    

    #region Knowledge Article Entity Attributes Names

    public sealed class KnowledgeArticleEntityAttributeNames
    {
        public const string EntityLogicalName = "knowledgearticle";
        public const string KnowledgeArticleId = "knowledgearticleid";
        public const string Content = "content";
        public const string Keywords = "keywords";
        public const string Description = "description";
        public const string Title = "title";
        public const string Status = "statecode";
        public const string AppRecordUrl = "tc_apprecordurl";
        public const string FAQCategory = "tc_faqcategories";
        public const string Order = "tc_order";
    }

    #endregion

    #region Language Locale Entity Attributes Names

    public sealed class LanguageLocaleEntityAttributeNames
    {
        public const string EntityLogicalName = "languagelocale";
        public const string LanguageLocalId = "languagelocaleid";
        public const string Code = "code";
        public const string Language = "language";
        public const string LocaleId = "localeid";
        public const string Name = "name";
        public const string Status = "statecode";
 }

    #endregion
}