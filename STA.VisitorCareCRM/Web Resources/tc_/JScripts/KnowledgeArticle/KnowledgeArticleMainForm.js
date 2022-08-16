var KACreateAmendKey = "KACreateAmendAccess", KAApprovalKey = "KAApprovalRole";
var formExecuteContext = null, updateContentVal_OnLoad = null, statusVal_OnLoad = null, isNotASaveEvent = true;
var fieldsOnBPF = "header_process_readyforreview, header_process_review, header_process_updatecontent, header_process_isinternal, header_process_expirationdate";
var fieldsEnableForAuthorStage = "header_process_readyforreview";
var fieldsEnableForReviewStage = "header_process_review";
var fieldsEnableForUpdateContentStage = "header_process_updatecontent";
var fieldsEnableForPublishStage = "header_process_isinternal, header_process_expirationdate";
var fieldsDisableForAuthorStage = "header_process_review, header_process_updatecontent, header_process_isinternal, header_process_expirationdate";
var fieldsDisableForReviewStage = "header_process_readyforreview, header_process_updatecontent, header_process_isinternal, header_process_expirationdate";
var fieldsDisableForUpdateContentStage = "header_process_readyforreview, header_process_review, header_process_isinternal, header_process_expirationdate";
var fieldsDisableForPublishStage = "header_process_readyforreview, header_process_review, header_process_updatecontent";
var fieldsOnKAForm = "languagelocaleid, title, tc_casecategory, tc_casesubcategory, tc_region, description, keywords, content";

function form_onLoad(executionContext) {
    formExecuteContext = executionContext;
    updateContentVal_OnLoad = GetAttributeValue(executionContext, "updatecontent");
    statusVal_OnLoad = GetAttributeValue(executionContext, "statecode");
    switch (statusVal_OnLoad) {
        case 3: //Published
            DisableAllAttrCtrls(executionContext);
            break;
        default: //Not Published
            var checkKAAccess = CheckUserRoleforKACreateAmendAccess(executionContext);

            if (!checkKAAccess) {
                DisableAllAttrCtrls(executionContext);
            }
            else {
                var formType = GetFormType(executionContext);
                switch (formType) {
                    case 1: //Create
                        SetAttrsDisability(executionContext, fieldsOnBPF, ", ", true);
                        SetAttrDisability(executionContext, "tc_faqcategories", false);
                        SetAttrDisability(executionContext, "tc_articletype", false);
                        language(executionContext);
                        break;
                    case 2: //Update
                        enableDisable_Fields_BPFActiveStage();
                        SetAttrDisability(executionContext, "tc_articletype", true);
                        break;
                }

                formExecuteContext.getFormContext().data.process.addOnStageChange(function (bpfStageContext) {
                    enableDisable_Fields_BPFActiveStage();
                });
            }
            break;
    }
    ChangeLookupLanguages(executionContext);
    ArticleType_onChange(executionContext);
}

function enableDisable_Fields_BPFActiveStage() {
    var bpfActiveStatusName = GetBPFStatusName(formExecuteContext);
    var bpfActiveStageName = GetBPFActiveStageName(formExecuteContext);
    var reviewVal = GetAttributeValue(formExecuteContext, "review");

    if (bpfActiveStageName != null && bpfActiveStatusName == "active") {
        switch (bpfActiveStageName) {
            case "author":
            case "المؤلف":
                SetAttrsDisability(formExecuteContext, fieldsDisableForAuthorStage, ", ", true);
                SetAttrsDisability(formExecuteContext, fieldsEnableForAuthorStage, ", ", false);
                SetAttrsDisability(formExecuteContext, fieldsOnKAForm, ", ", false);
                break;
            case "review":
            case "مراجعة":
                if (reviewVal == 0) { //Approved
                    DisableAllAttrCtrls(formExecuteContext);
                }
                else {
                    SetAttrsDisability(formExecuteContext, fieldsDisableForReviewStage, ", ", true);
                }
                SetAttrsDisability(formExecuteContext, fieldsEnableForReviewStage, ", ", false);
                CheckUserRoleforApprovalStage(formExecuteContext);
                break;
            case "update content":
            case "تحديث المحتوى":
                SetAttrsDisability(formExecuteContext, fieldsDisableForUpdateContentStage, ", ", true);
                SetAttrsDisability(formExecuteContext, fieldsEnableForUpdateContentStage, ", ", false);
                SetAttrsDisability(formExecuteContext, fieldsOnKAForm, ", ", false);
                break;
            case "publish":
            case "نشر":
                if (reviewVal == 0) { //Approved
                    DisableAllAttrCtrls(formExecuteContext);
                }
                else {
                    SetAttrsDisability(formExecuteContext, fieldsDisableForPublishStage, ", ", true);
                }
                SetAttrsDisability(formExecuteContext, fieldsEnableForPublishStage, ", ", false);
                break;
        }
    }
    else {
        SetAttrsDisability(formExecuteContext, fieldsOnBPF, ", ", true);
    }
    RefreshFormRibbon(formExecuteContext, false);
    if (isNotASaveEvent) {
        RefreshFormData(formExecuteContext, false);
    }
}

function CheckUserRoleforApprovalStage(executionContext) {
    var checkKAAppRoles = false;
    var currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();

    var ConfigRoleName = GetConfigParameterValue(KAApprovalKey, false);
    if (ConfigRoleName != null) {
        var configrolearry = ConfigRoleName;
        var Roles = configrolearry.split(',');
        for (var i = 0; i < currentUserRoles.length; i++) {
            if (Roles.includes(currentUserRoles[i].name.toLowerCase())) {
                checkKAAppRoles = true;
            }
        }
    }

    if (checkKAAppRoles) {
        SetAttrDisability(executionContext, "header_process_review", false);
    }
    else {
        SetAttrDisability(executionContext, "header_process_review", true);
    }
}

function status_onChange(executionContext) {
    var statusVal_OnChange = GetAttributeValue(executionContext, "statecode");

    if (statusVal_OnLoad != 3 && statusVal_OnChange == 3) { // Published
        ShowProgressIndicator("Publishing the Knowledge Article....");

        setTimeout(function () {
            CloseProgressIndicator();
            var targetRecordId = GetTargetRecordId(executionContext);
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "knowledgearticle";
            entityFormOptions["entityId"] = targetRecordId;

            // Reload the Target Record.
            Xrm.Navigation.openForm(entityFormOptions);
        }, 10000);
    }
}

function form_onSave(executionContext) {
    isNotASaveEvent = false;
    formExecuteContext = executionContext;
    enableDisable_Fields_BPFActiveStage();
    var updateContentVal_OnSave = GetAttributeValue(executionContext, "updatecontent");

    if ((updateContentVal_OnLoad == null || updateContentVal_OnLoad == false) && updateContentVal_OnSave != null && updateContentVal_OnSave) {
        ShowProgressIndicator("Processing Knowledge Article for Review....");

        setTimeout(function () {
            CloseProgressIndicator();
            var targetRecordId = GetTargetRecordId(executionContext);
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "knowledgearticle";
            entityFormOptions["entityId"] = targetRecordId;

            // Reload the Target Record.
            Xrm.Navigation.openForm(entityFormOptions);
        }, 15000);
    }
}

function CheckUserRoleforKACreateAmendAccess() {
    var checkKACARoles = false;
    var currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();

    var ConfigRoleName = GetConfigParameterValue(KACreateAmendKey, false);
    if (ConfigRoleName != null) {
        var configrolearry = ConfigRoleName;
        var Roles = configrolearry.split(',');
        for (var i = 0; i < currentUserRoles.length; i++) {
            if (Roles.includes(currentUserRoles[i].name.toLowerCase())) {
                checkKACARoles = true;
                break;
            }
        }
    }

    return checkKACARoles;
}

function ChangeLookupLanguages(executionContext) {
    //tc_casecategory
    getArabicLookups(executionContext, "tc_casecategories", "tc_casecategory", "tc_arabicname");
    //tc_casesubcategory
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_casesubcategory", "tc_arabicname");
    //tc_region
    getArabicLookups(executionContext, "cc_regions", "tc_region", "tc_arabicname");
}

function checkKAViewRcrdOnLoad(executionContext) {
    var formType = GetFormType(executionContext);

    if (formType != 1) {
        var kaRcrdId = GetTargetRecordId(executionContext);
        var loggedInUserId = GetContextUserId();

        if (kaRcrdId != null && loggedInUserId != null) {
            var qryForKAView = null, resultsForKAView = null, IsViewRcrdUpdated = false;
            qryForKAView = "knowledgearticleviews?$select=knowledgearticleview,knowledgearticleviewsid,viewdate&$filter=_knowledgearticleid_value eq " + kaRcrdId + " and _createdby_value eq " + loggedInUserId + " and location eq 3&$orderby=viewdate desc";
            resultsForKAView = RetrieveMultipleRecords(qryForKAView, false, null);

            if (resultsForKAView != null && resultsForKAView.value != null && resultsForKAView.value.length > 0) {
                var kaViewRcrdId = null, viewCount = null, viewDate = null;

                viewDate = resultsForKAView.value[0]["viewdate"];

                if (viewDate != null) {
                    viewDate = new Date(viewDate);

                    var currentDate = null, formattedViewDate = null;
                    currentDate = formatDate(new Date());
                    formattedViewDate = formatDate(viewDate);

                    if (formattedViewDate == currentDate) {
                        kaViewRcrdId = resultsForKAView.value[0]["knowledgearticleviewsid"];
                        viewCount = resultsForKAView.value[0]["knowledgearticleview"];

                        updateKAVRcrdOnLoad(kaViewRcrdId, viewCount, loggedInUserId);
                        IsViewRcrdUpdated = true;
                    }
                }
            }

            if (!IsViewRcrdUpdated) {
                createKAVRcrdOnLoad(kaRcrdId, loggedInUserId);
            }
        }
    }
}

//--Create KA View record for the current user using Web API
function createKAVRcrdOnLoad(kaViewRcrdId) {
    if (kaViewRcrdId != null) {
        var entCreateKAView = {};
        entCreateKAView["knowledgearticleid@odata.bind"] = "/knowledgearticles(" + kaViewRcrdId + ")";
        entCreateKAView.viewdate = new Date();
        entCreateKAView.knowledgearticleview = 1;
        entCreateKAView.location = 3; //KA Form        
        CreateRecordInCRMUsgWebApi(entCreateKAView, "knowledgearticleviews", false, null);
    }
}

//--Update KA View record for the current user using Web API
function updateKAVRcrdOnLoad(kaViewRcrdId, viewCount) {
    if (kaViewRcrdId != null) {
        var entUpdateKAView = {};
        entUpdateKAView.knowledgearticleview = viewCount != null ? (viewCount + 1) : 1;
        UpdateRecordInCRMUsgWebApi(entUpdateKAView, "knowledgearticleviews", kaViewRcrdId, false, null, null, null);
    }
}

function formatDate(dtValue) { //Formats the Input Date Value in 'YYYY-MM-DD'
    var formattedDateVal = null, formattedDate = null, formattedMonth = null, formattedYear = null;
    formattedDate = dtValue.getDate() <= 9 ? "0" + dtValue.getDate() : dtValue.getDate();
    formattedMonth = (dtValue.getMonth() + 1) <= 9 ? "0" + (dtValue.getMonth() + 1) : (dtValue.getMonth() + 1);
    formattedYear = dtValue.getFullYear();
    formattedDateVal = formattedYear + "-" + formattedMonth + "-" + formattedDate;
    return formattedDateVal;
}

//--Create Case View History record for the current user using Web API - Calling this function in checkCaseViewHistoryRecordOnLoad()
function createKAVHRcrdOnLoad(executionContext) {
    var targetRcrdId = GetTargetRecordId(executionContext);
    var loggedInUserId = GetContextUserId();

    if (targetRcrdId != null && loggedInUserId != null) {
        var formType = GetFormType(executionContext);
        var imperUserId = GetConfigParameterValue("AdminUserGuid", false)

        if (formType != 1) {
            var entCreateViewHistory = {};
            entCreateViewHistory.tc_name = "KA View History Record";
            entCreateViewHistory.tc_viewedon = new Date();
            entCreateViewHistory["tc_regardingknowledgearticle@odata.bind"] = "/knowledgearticles(" + targetRcrdId + ")";
            entCreateViewHistory["tc_viewedby@odata.bind"] = "/systemusers(" + loggedInUserId + ")";
            CreateRecordInCRMUsgWebApi(entCreateViewHistory, "tc_viewhistories", false, imperUserId);
        }
    }
}

function ArticleType_onChange(executionContext) {
    var articletype = GetAttributeValue(formExecuteContext, "tc_articletype");
    switch (articletype) {
        case 2: //FAQ
            SetAttrRequiredLevel(executionContext, "tc_faqcategories", "required");
            SetAttrVisibility(executionContext, "tc_faqcategories", true);
            SetAttrVisibility(executionContext, "tc_order", true);
            SetAttrVisibility(executionContext, "tc_region", false);
            SetTabVisibility(executionContext, "tab_kacategorizations", false);
            break;
        default:
            SetAttrVisibility(executionContext, "tc_faqcategories", false);
            SetAttrVisibility(executionContext, "tc_order", false);
            SetAttrVisibility(executionContext, "tc_region", true);
            SetTabVisibility(executionContext, "tab_kacategorizations", true);
            break;
    }
}

function language(executionContext) {
    var userLCID = GetContextUserLCID();
    var qryselectlang = null, langValue = null, langIDVal = null, langNameVal = null;
    switch (userLCID) {
        case 1025: //Arabic
            var qryselectlang = "languagelocale?$select=code,language,languagelocaleid,name,localeid&$filter=localeid eq 1025";
            var qryResultslang = RetrieveMultipleRecords(qryselectlang, false, null);
            if (qryResultslang != null && qryResultslang.value != null && qryResultslang.value.length > 0) {
                langValue = new Array();
                langIDVal = qryResultslang.value[0]["languagelocaleid"];
                langNameVal = qryResultslang.value[0]["name"];
                langValue[0] = new Object();
                langValue[0].id = langIDVal; // GUID of the lookup id
                langValue[0].name = langNameVal; // Name of the lookup
                langValue[0].entityType = "languagelocale"; //Entity Type of the lookup entity
                SetAttributeValue(executionContext, "languagelocaleid", langValue);
            }
            break;
        case 1033: //English
        default:
            var qryselectlang = "languagelocale?$select=code,language,languagelocaleid,name,localeid&$filter=localeid eq 1033";
            var qryResultslang = RetrieveMultipleRecords(qryselectlang, false, null);
            if (qryResultslang != null && qryResultslang.value != null && qryResultslang.value.length > 0) {
                langValue = new Array();
                langIDVal = qryResultslang.value[0]["languagelocaleid"];
                langNameVal = qryResultslang.value[0]["name"];
                langValue[0] = new Object();
                langValue[0].id = langIDVal; // GUID of the lookup id
                langValue[0].name = langNameVal; // Name of the lookup
                langValue[0].entityType = "languagelocale"; //Entity Type of the lookup entity
                SetAttributeValue(executionContext, "languagelocaleid", langValue);
                break;
            }
    }

}