namespace STA.TouristCareCRM.Plugins.Resources
{
    #region Account Entity Attributes Names

    public sealed class AccountEntityAttributeNames
    {
        public const string EntityLogicalName = "account";
        public const string AccountId = "accountid";
        public const string AccountName = "name";
        public const string EmailAddress = "emailaddress1";
        public const string ContactNumber = "telephone1";
    }

    #endregion

    #region Contact Entity Attributes Names

    public sealed class ContactEntityAttributeNames
    {
        public const string EntityLogicalName = "contact";
        public const string AccountId = "contactid";
        public const string PrimaryEmail = "emailaddress1";
        public const string PrimaryContactNo = "mobilephone";
        public const string SecondaryEmail = "emailaddress2";
        public const string SecondaryContactNo = "telephone2";
        public const string FirstName = "firstname";
        public const string LastName = "lastname";
        public const string RelationshipType = "customertypecode";

        public const string CustomerCategory = "tc_customercategory";
        public const string RecentlyInteractedAgent = "tc_recentlyinteractedagent";
        public const string TotalCasesCount = "tc_totalcasescount";
        public const string EnquiryCasesCount = "tc_enquirycasescount";
        public const string ComplaintCasesCount = "tc_complaintcasescount";
        public const string SuggestionCasesCount = "tc_suggestioncasescount";
        public const string OutofScopeCasesCount = "tc_outofscopecasescount";
        public const string EmergenciesCasesCount = "tc_emergenciescasescount";
        public const string CurrentCESScore = "tc_currentcesscore";
        public const string CurrentCSATAgent = "tc_currentcsatagent";
        public const string CurrentCSATInformation = "tc_currentcsatinformation";
        public const string CurrentCSATOutcome = "tc_currentcsatoutcome";
        public const string CurrentNPSScore = "tc_currentnpsscore";
        public const string CreatedBy = "createdby";
    }

    #endregion

    #region Customer Category Entity Attributes Names

    public sealed class CustomerCategoryEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_customercategory";
        public const string CustomerCategoryId = "tc_customercategoryid";
        public const string CategroryId = "tc_categoryid";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Case Entity Attributes Names

    public sealed class CaseEntityAttributeNames
    {
        public const string EntityLogicalName = "incident";
        public const string CaseId = "incidentid";
        public const string CaseTcktNo = "ticketnumber";
        public const string Customer = "customerid";
        public const string EmailAddress = "emailaddress";
        public const string PriorityCode = "prioritycode";
        public const string CaseTitle = "title";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string Owner = "ownerid";
        public const string OOBCaseTypeCode = "casetypecode";
        public const string SLAStatus = "resolvebyslastatus";

        public const string CaseRefNo = "tc_caserefno";
        public const string CustomerName = "tc_customername";
        public const string ContactNumber = "tc_contactnumber";
        public const string CustomerCategory = "tc_customercategory";
        public const string CaseType = "tc_casetype";
        public const string Category = "tc_category";
        public const string Subcatgeory = "tc_subcategory";
        public const string ChannelOrigin = "tc_channelorigin";
        public const string CaseNotificationLanguage = "tc_caselanguage";
        public const string IsSubmitted = "tc_issubmitted";
        public const string SubmitToExternalTeam = "tc_submittoexternalteam";
        public const string SubmittedBy = "tc_submittedby";
        public const string SubmittedOn = "tc_submittedon";
        public const string CaseProcessConfig = "tc_caseprocessconfig";
        public const string ResolutionPartner = "tc_pendingwith";
        public const string ResolutionPartnerQueue = "tc_pendingwithqueue";
        public const string SLAConfig = "tc_slaconfig";
        public const string NotificationConfig = "tc_notificationconfig";
        public const string WarnAfter = "tc_warnafter";
        public const string EscalateAfter = "tc_escalateafter";
        public const string SLADuration = "tc_sladuration";
        public const string CaseWorkedBy = "tc_caseworkedby";
        public const string CaseWorkedOn = "tc_caseworkedon";
        public const string ServiceRelatedTo = "tc_servicerelatedto";
        public const string SendSuggestionTo = "tc_sendsuggestionto";
        public const string RelatedConversation = "tc_relatedconversation";
        public const string ConversationIncomingMsg = "tc_conversationincomingmessage";
        public const string RecognizeMomentofDelight = "tc_momentofdelight";
        public const string CSATAgent = "tc_agentcsat";
        public const string CESScore = "tc_cesscore";
        public const string CSATInformation = "tc_csatinformation";
        public const string CSATOutcome = "tc_csatoutcome";
        public const string NPSScore = "tc_npsscore";
    }

    #endregion

    #region Case Approval Entity Attributes Names

    public sealed class CaseApprovalEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_caseapproval";
        public const string CaseApprovalId = "tc_caseapprovalid";
        public const string RequestType = "tc_requesttype";
        public const string RegardingObject = "regardingobjectid";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string Owner = "ownerid";
    }

    #endregion

    #region Case Type Entity Attributes Names

    public sealed class CaseTypeEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_casetype";
        public const string CaseTypeId = "tc_casetypeid";
        public const string CaseTypeName = "tc_name";
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
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Case Subcategory Entity Attributes Names

    public sealed class CaseSubcategoryEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_casesubcategory";
        public const string CaseSubcategoryId = "tc_casesubcategoryid";
        public const string CaseSubcategoryName = "tc_name";
        public const string CaseCategory = "tc_casecategory";
        public const string DefaultPriority = "tc_defaultpriority";
        public const string ServiceRelatedTo = "tc_servicerelatedto";
        public const string CaseSolvedBy = "tc_casesolvedby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Case Process Configuration Entity Attributes Names

    public sealed class CaseProcessConfigEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_caseprocessconfiguration";
        public const string CaseType = "tc_casetype";
        public const string Category = "tc_casecategory";
        public const string Subcatgeory = "tc_casesubcategory";
        public const string CasePriority = "tc_casepriority";
        public const string AllCategories = "tc_allcasecategories";
        public const string AllSubcatgeories = "tc_allcasesubcategories";
        public const string AllCasePriorities = "tc_allcasepriorities";
        public const string NotificationConfig = "tc_notificationconfiguration";
        public const string SLAConfig = "tc_slaconfiguration";
        public const string ResolutionPartner = "tc_resolutionpartner";

        public const string OperationType_BCS = "tc_operationtype_bcs";
        public const string AssignTo_BCS = "tc_assignto_bcs";
        public const string UserAssign_BCS = "tc_user_abcs";
        public const string TeamAssign_BCS = "tc_team_abcs";
        public const string ShareTo_BCS = "tc_shareto_bcs";
        public const string UserShare_BCS = "tc_user_sbcs";
        public const string TeamShare_BCS = "tc_team_sbcs";

        public const string OperationType_ACS = "tc_operationtype_acs";
        public const string AssignTo_ACS = "tc_assignto_acs";
        public const string UserAssign_ACS = "tc_user_aacs";
        public const string TeamAssign_ACS = "tc_team_aacs";
        public const string ShareTo_ACS = "tc_shareto_acs";
        public const string UserShare_ACS = "tc_user_sacs";
        public const string TeamShare_ACS = "tc_team_sacs";

        public const string AssignTo = "tc_assignto";
        public const string User = "tc_user";
        public const string Team = "tc_team";
        public const string Queue = "tc_queue";

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
        public const string UserName = "domainname";
        public const string DefaultQueue = "queueid";
    }

    #endregion

    #region Team Entity Attributes Names

    public sealed class TeamEntityAttributeNames
    {
        public const string EntityLogicalName = "team";
        public const string TeamId = "teamid";
        public const string DefaultQueue = "queueid";
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

    #region Configuration Parameters Entity Attributes Names

    public sealed class ConfigParamsEntityAttributeNames
    {
        public const string EntityLogicalName = "cc_configurationparameters";
        public const string ConfigParamsId = "cc_configurationparametersid";
        public const string ConfigParamKey = "cc_key";
        public const string ConfigParamValue = "cc_value";
        public const string CreatedOn = "createdon";
        public const string ModifiedOn = "modifiedon";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
    }

    #endregion

    #region Role Entity Attributes Names

    public sealed class RoleEntityAttributeNames
    {
        public const string EntityLogicalName = "role";
        public const string RoleId = "roleid";
        public const string Name = "name";
    }

    #endregion

    #region System User Role Entity Attributes Names

    public sealed class SystemUserRoleEntityAttributeNames
    {
        public const string EntityLogicalName = "systemuserroles";
        public const string SystemUserRoleId = "systemuserrolesid";
        public const string RoleId = "roleid";
        public const string SystemUserId = "systemuserid";
    }

    #endregion

    #region Duplicate Rule Entity Attributes Names

    public sealed class DuplicateRuleEntityAttributeNames
    {
        public const string EntityLogicalName = "duplicaterule";
        public const string DuplicateRuleId = "duplicateruleid";
        public const string Name = "name";
        public const string IsCaseSensitive = "iscasesensitive";
        public const string ExcludeInactiveRecords = "excludeinactiverecords";
        public const string BaseETC = "baseentitytypecode";
        public const string BaseETN = "baseentityname";
        public const string MatchingETC = "matchingentitytypecode";
        public const string MatchingETN = "matchingentityname";
        public const string Statecode = "statecode";
    }

    #endregion

    #region Duplicate Rule Condition Entity Attributes Names

    public sealed class DuplicateRuleConditionEntityAttributeNames
    {
        public const string EntityLogicalName = "duplicaterulecondition";
        public const string SystemUserRoleId = "duplicateruleconditionid";
        public const string RegardingObjectId = "regardingobjectid";
        public const string BaseAttributeName = "baseattributename";
        public const string MatchingAttributeName = "matchingattributename";
    }

    #endregion

    #region Conversation Entity Attributes Names

    public sealed class ConversationEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_conversation";
        public const string ConversationId = "tc_conversationid";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string Owner = "ownerid";

        public const string Customer = "tc_customer";
        public const string EmailAddress = "tc_emailaddress";
        public const string PriorityCode = "tc_priority";
        public const string ExistingCase = "tc_existingcase";
        public const string CaseType = "tc_casetype";
        public const string Category = "tc_category";
        public const string Subcatgeory = "tc_subcategory";
        public const string IsCaseExisting = "tc_iscaseexisting";
        public const string ContactNumber = "tc_contactnumber";
        public const string OriginChannel = "tc_channelorigin";
        public const string CustomerCategory = "tc_customercategory";
        public const string DoesAnyCaseAssociated = "tc_doesanycaseassociated";
        public const string FirstName = "tc_firstname";
        public const string LastName = "tc_lastname";
        public const string Subject = "tc_name";
        public const string IncomingMessage = "tc_incomingmessage";
        public const string RegardingEmail = "tc_regardingemail";
        public const string RelatedCase = "tc_existingcase";
    }

    #endregion

    #region Case Assignment History Entity Attributes Names

    public sealed class CaseAssignmentHistoryEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_caseassignmenthistory";
        public const string CaseAssignmentHistoryId = "tc_caseassignmenthistoryid";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";

        public const string Name = "tc_name";
        public const string RegardingCase = "tc_regarding";
        public const string Assignee = "tc_assignee";
        public const string AssignedBy = "tc_assignedby";
        public const string ReleasedBy = "tc_releasedby";
        public const string PickUpDateTime = "tc_pickupdate";
        public const string ReleaseDateTime = "tc_releasedate";
    }

    #endregion

    #region SMS Activity Entity Attributes Names

    public sealed class SMSActivityEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_sms";
        public const string SMSActivityId = "activityid";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string SMSMessage = "description";
        public const string APIResponseData = "activityadditionalparams";
        public const string DateSent = "senton";
        public const string DueDate = "scheduledend";
        public const string Subject = "subject";
        public const string RegardingObject = "regardingobjectid";
        public const string From = "from";
        public const string To = "to";
        public const string Owner = "ownerid";

        public const string RecipientNo = "tc_recipientno";
        public const string IsSMSAPISuccess = "tc_issmsapicallsuccess";
        public const string APIResponseCode = "tc_apiresponsecode";
        public const string APIResponseMessage = "tc_apiresponsemessage";
        public const string APIResponseMessageID = "tc_apiresponsemessageid";
    }

    #endregion

    #region Notification Configuration Entity Attributes Names

    public sealed class NotificationConfigEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_notificationconfiguration";
        public const string NotificationConfigId = "tc_notificationconfigurationid";
        public const string CreatedOn = "createdon";
        public const string CreatedBy = "createdby";
        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";

        public const string Name = "tc_name";
        public const string EntityType = "tc_entitytype";
        public const string SendFrom = "tc_sendfrom";
        public const string FromUser = "tc_fromuser";
        public const string FromTeam = "tc_fromteam";

        public const string EmailTemplateCreate = "tc_emailtemplatecreate";
        public const string EmailTemplateResolve = "tc_emailtemplateresolve";

        public const string SMSTemplateCreate = "tc_smstemplatecreate";
        public const string SMSTemplateResolve = "tc_smstemplateresolve";
    }

    #endregion

    #region Activity Party Entity Attributes Names

    public sealed class ActivityPartyEntityAttributeNames
    {
        public const string EntityLogicalName = "activityparty";
        public const string PartyId = "partyid";
        public const string AddressUsed = "addressused";
    }

    #endregion

    #region Email Entity Attributes Names

    public sealed class EmailEntityAttributeNames
    {
        public const string EntityLogicalName = "email";
        public const string EmailId = "emailid";
        public const string From = "from";
        public const string To = "to";
        public const string DueDate = "scheduledend";
        public const string Subject = "subject";
        public const string Description = "description";
        public const string DirectionCode = "directioncode";
        public const string RegardingObject = "regardingobjectid";
        public const string Owner = "ownerid";
    }

    #endregion

    #region Knowledge Article Entity Attributes Names

    public sealed class KnowledgeArticleEntityAttributeNames
    {
        public const string EntityLogicalName = "knowledgearticle";
        public const string KnowledgeArticleId = "knowledgearticleid";
        public const string Keywords = "keywords";
        public const string Description = "description";
        public const string CaseCategory = "tc_casecategory";
        public const string CaseSubcategory = "tc_casesubcategory";
        public const string Region = "tc_region";
        public const string Title = "title";
    }

    #endregion

    #region Knowledge Article Categorization Entity Attributes Names

    public sealed class KACategorizationEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_knowledgearticlecategorization";
        public const string KACategorizationId = "tc_knowledgearticlecategorizationid";
        public const string KnowledgeArticle = "tc_knowledgearticle";
        public const string CaseCategory = "tc_casecategory";
        public const string CaseSubcategory = "tc_casesubcategory";
        public const string Status = "statecode";
        public const string StatusReason = "statuscode";
    }

    #endregion

    #region Survey Answers Entity Attributes Names

    public sealed class SurveyAnswersEntityAttributeNames
    {
        public const string EntityLogicalName = "cdi_surveyanswer";
        public const string SurveyAnswerId = "cdi_surveyanswerid";
        public const string SurveyQuestionId = "cdi_surveyquestionid";
        public const string Value = "cdi_value";
        public const string Account = "cdi_accountid";
        public const string Contact = "cdi_contactid";
        public const string Case = "cdi_incidentid";
        public const string Question = "cdi_question";
        public const string PostedSurvey = "cdi_postedsurveyid";

    }

    #endregion

    #region Survey Question Entity Attributes Names

    public sealed class SurveyQuestionEntityAttributeNames
    {
        public const string EntityLogicalName = "cdi_surveyquestion";
        public const string SurveyQuestionId = "cdi_surveyquestionid";
        public const string Name = "cdi_name";
        public const string RatingsType = "tc_ratingstype";
        public const string Question = "cdi_question";
    }

    #endregion

    #region Posted Survey Entity Attributes Names

    public sealed class PostedSurveyEntityAttributeNames
    {
        public const string EntityLogicalName = "cdi_postedsurvey";
        public const string Name = "cdi_name";
        public const string Account = "cdi_accountid";
        public const string Contact = "cdi_contactid";
        public const string Case = "cdi_incidentid";
        public const string Question = "cdi_question";
        public const string PostedSurveyId = "cdi_postedsurveyid";
        public const string Visit = "cdi_visitid";
    }

    #endregion    

    #region Survey Ratings Entity Attributes Names

    public sealed class SurveyRatingsEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_surveyratings";
        public const string Name = "tc_name";
        public const string Account = "tc_account";
        public const string Contact = "tc_contact";
        public const string Case = "tc_incident";
        public const string CSAT_Agent = "tc_csatagent";
        public const string CSAT_Information = "tc_csatinformation";
        public const string CSAT_Outcome = "tc_csatoutcome";
        public const string NPS = "tc_nps";
        public const string CES = "tc_ces";
        public const string SurveryRatingsId = "tc_surveyratingsid";
        public const string PostedSurvey = "tc_postedsurvey";
        public const string Comment = "tc_comment";
    }

    #endregion

    #region Survey Scheduler Entity Attributes Names

    public sealed class SurveySchedulerEntityAttributeNames
    {
        public const string EntityLogicalName = "tc_surveyscheduler";
        public const string Name = "tc_name";
        public const string Frequency = "tc_frequency";
        public const string LastRunDate = "tc_lastrundate";
        public const string NextRunDate = "tc_nextrundate";
        public const string StartDate = "tc_startdate";
        public const string Workflow = "tc_workflow";
        public const string SurverySchedulerId = "tc_surveyschedulerid";
        public const string TemplateName = "tc_templatename";
        public const string SendSurveyFrom = "tc_fromqueue";
        public const string SendSurveyTo = "tc_toqueue";

    }

    #endregion

    #region Visit Page Attributes Names

    public sealed class VisitEntityAttributeNames
    {
        public const string EntityLogicalName = "cdi_visit";
        public const string Name = "cdi_name";
        public const string EntryPage = "cdi_entrypage";
        public const string VisitId = "cdi_visitid";
     }

    #endregion

    #region Channel of Origin

    public sealed class ChannelofOriginAttributeNames
    {
        public const string EntityLogicalName = "tc_channelorigin";
        public const string Name = "tc_name";
        public const string ChannelCode = "tc_origincode";
        public const string ChannelType = "tc_channeltype";
        public const string ChannelOriginId = "tc_channeloriginid";
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