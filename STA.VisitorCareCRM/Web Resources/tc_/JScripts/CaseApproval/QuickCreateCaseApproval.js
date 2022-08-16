var formExecuteContext = null, alertStrings = new Array(), alertOptions = new Array();
var userLCID = GetContextUserLCID();
function form_OnLoad(executionContext) {
    formExecuteContext = executionContext;
    var requestType = GetAttributeValue(executionContext, "tc_requesttype");

    switch (requestType) {
        case 1: //Pause SLA
        case 3: //Cancel Case
        default:
            SetSectionVisibility(executionContext, "tab_approvaldetails", "sec_changecategoryorpriority", false);
            SetAttrRequiredLevel(executionContext, "tc_relatedcase", "none");
            SetAttrVisibility(executionContext, "tc_relatedcase", false);
            SetAttrsVisibility(executionContext, "tc_requestreason, regardingobjectid", ", ", true);
            SetAttrsRequiredLevel(executionContext, "tc_requestreason, regardingobjectid", ", ", "required");
            break;
        case 2: //Change Categorization
            SetSectionVisibility(executionContext, "tab_approvaldetails", "sec_changecategoryorpriority", true);
            SetAttrsRequiredLevel(executionContext, "tc_newpriority, tc_relatedcase", ", ", "none");
            SetAttrsVisibility(executionContext, "tc_newpriority, tc_relatedcase", ", ", false);
            SetAttrsVisibility(executionContext, "tc_requestreason, tc_newcasecategory, tc_newcasesubcateogry, regardingobjectid", ", ", true);
            SetAttrsRequiredLevel(executionContext, "tc_requestreason, tc_newcasecategory, tc_newcasesubcateogry, regardingobjectid", ", ", "required");
            break;
        case 4: //Change Priority
            SetSectionVisibility(executionContext, "tab_approvaldetails", "sec_changecategoryorpriority", true);
            SetAttrsRequiredLevel(executionContext, "tc_newcasecategory, tc_newcasesubcateogry, tc_relatedcase", ", ", "none");
            SetAttrsVisibility(executionContext, "tc_newcasecategory, tc_newcasesubcateogry, tc_relatedcase", ", ", false);
            SetAttrsVisibility(executionContext, "tc_requestreason, tc_newpriority, regardingobjectid", ", ", true);
            SetAttrsRequiredLevel(executionContext, "tc_requestreason, tc_newpriority, regardingobjectid", ", ", "required");
            break;
        case 5: //Moment of Delight
            SetSectionVisibility(executionContext, "tab_approvaldetails", "sec_changecategoryorpriority", false);
            SetAttrsRequiredLevel(executionContext, "tc_requestreason, regardingobjectid", ", ", "none");
            SetAttrsVisibility(executionContext, "tc_requestreaso, regardingobjectid", ", ", false);
            SetAttrVisibility(executionContext, "tc_relatedcase", true);
            SetAttrRequiredLevel(executionContext, "tc_relatedcase", "required");
            break;
    }
    SetAttrSubmitMode(executionContext, "tc_requesttype", "always");
    SetApprovalRequestedUser(executionContext);
    addCustFilterToRqtReasons(executionContext);
    addCustFilterToNewCatgry(executionContext);
    ChangeLookupLanguages(executionContext);
}

function SetApprovalRequestedUser(executionContext) {
    var requestedByUser = null;

    if (GetContextUserId() != null) {
        requestedByUser = new Array();
        requestedByUser[0] = new Object();
        requestedByUser[0].id = GetContextUserId();
        requestedByUser[0].name = GetContextUserName();
        requestedByUser[0].entityType = "systemuser";
    }

    if (requestedByUser != null) {
        SetAttributeValue(executionContext, "tc_requestedby", requestedByUser);
        SetAttrSubmitMode(executionContext, "tc_requestedby", "always");
    }
}

function addCustFilterToRqtReasons(executionContext) {
    // add the event handler for PreSearch Event
    var ctrlRqtReasons = GetControl(executionContext, "tc_requestreason");
    if (ctrlRqtReasons != null) {
        ctrlRqtReasons.addPreSearch(addRqtReasonFilter);
    }
}

function addRqtReasonFilter() {
    //Check if Request Type is not empty
    var rqtTypeVal = GetAttributeValue(formExecuteContext, "tc_requesttype");
    if (rqtTypeVal != null) {
        if (rqtTypeVal == 2 || rqtTypeVal == 4) { //RqtType = 2 --> Change Case Categorization || RqtType = 4 --> Change Case Priority
            rqtTypeVal = 2; //Change Case Categorization
        }

        //Create a Filter XML
        var customFilter = "<filter type='or'>" +
            "<condition attribute='tc_reasontype' operator='eq' value='" + rqtTypeVal + "'/>" +
            "<condition attribute='tc_reasontype' operator='null' />" +
            "</filter>";

        //add filter
        AddCustomFilterToAttr(formExecuteContext, "tc_requestreason", customFilter, "tc_casereason");
    }
}

function addCustFilterToNewCatgry(executionContext) {
    // add the event handler for PreSearch Event
    var ctrlCaseCategory = GetControl(executionContext, "tc_newcasecategory");
    if (ctrlCaseCategory != null) {
        ctrlCaseCategory.addPreSearch(addNewCatgryFilter);
    }
}

function addNewCatgryFilter() {
    var caseTypeId = GetAttributeValue(formExecuteContext, "tc_casetype");
    var custCategoryId = GetAttributeValue(formExecuteContext, "tc_customercategory");

    if (caseTypeId != null && custCategoryId != null) {
        //create a filter xml
        var customFilter = "<filter type='and'>" +
            "<condition attribute='tc_casetype' operator='eq' value='" + caseTypeId + "'/>" +
            "<condition attribute='tc_customercategory' operator='eq' value='" + custCategoryId + "'/>" +
            "</filter>";

        //add filter
        AddCustomFilterToAttr(formExecuteContext, "tc_newcasecategory", customFilter, "tc_casecategory");
    }
}

function newPriority_OnChange(executionContext) {
    var newPriorityVal = null, oldPriorityVal = null;

    newPriorityVal = GetAttributeValue(executionContext, "tc_newpriority");
    oldPriorityVal = GetAttributeValue(executionContext, "tc_oldpriority");

    if (oldPriorityVal == newPriorityVal) {
        alertStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { confirmButtonLabel: confirmButtonLabelAR, text: PriorityMsgAR, title: titlePrioritynMsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { confirmButtonLabel: confirmButtonLabelEN, text: PriorityMsgEN, title: titlePrioritynMsgEN };
                break;
        }
        alertOptions = { height: 120, width: 260 };
        openAlertDialog(alertStrings, alertOptions);
        SetAttributeValue(executionContext, "tc_newpriority", null);
    }
}

function newSubCategory_OnChange(executionContext) {
    var newSubCategoryId = null, oldSubCategoryId = null;

    newSubCategoryId = GetLookupValId(executionContext, "tc_newcasesubcateogry");
    oldSubCategoryId = GetLookupValId(executionContext, "tc_oldcasesubcategory");

    if (oldSubCategoryId == newSubCategoryId) {
        alertStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { confirmButtonLabel: confirmButtonLabelAR, text: CatgeorizationMsgAR, title: titleCatgeorizationMsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { confirmButtonLabel: confirmButtonLabelEN, text: CatgeorizationMsgEN, title: titleCatgeorizationMsgEN };
                break;
        }
        alertOptions = { height: 120, width: 260 };
        openAlertDialog(alertStrings, alertOptions);
        SetAttributeValue(executionContext, "tc_newcasesubcateogry", null);
        SetAttributeValue(executionContext, "tc_newcasecategory", null);
    }
}

function openAlertDialog(alertStrngs, alertOptns) {
    Xrm.Navigation.openAlertDialog(alertStrngs, alertOptns).then(
        function (success) {
        },
        function (error) {
            alert(error.message);
        }
    );
}

function ChangeLookupLanguages(executionContext) {
    //tc_requestreeason
    getArabicLookups(executionContext, "tc_casereasons", "tc_requestreason", "tc_arabicname");
    //tc_oldcasecategory
    getArabicLookups(executionContext, "tc_casecategories", "tc_oldcasecategory", "tc_arabicname");
    //tc_oldcasesubcategory
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_oldcasesubcategory", "tc_arabicname");
    //tc_newcasecategory
    getArabicLookups(executionContext, "tc_casecategories", "tc_newcasecategory", "tc_arabicname");
    //tc_newcasesubcateogry
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_newcasesubcateogry", "tc_arabicname");
}