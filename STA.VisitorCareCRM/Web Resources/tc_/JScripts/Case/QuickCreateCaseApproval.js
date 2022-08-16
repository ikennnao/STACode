var formExecuteContext = null;

function form_OnLoad(executionContext) {    
    formExecuteContext = executionContext;
    var requestType = GetAttributeValue(executionContext, "tc_requesttype");
    if (requestType == 2) { //change category
        SetSectionVisibility(executionContext, "tab_ApprovalDetails", "sec_ChangeCaseCategory", true);
        SetAttrRequiredLevel(executionContext, "tc_newcasecategory", "required");
        SetAttrRequiredLevel(executionContext, "tc_newcasesubcateogry", "required");
    }
    else {
        SetSectionVisibility(executionContext, "tab_ApprovalDetails", "sec_ChangeCaseCategory", false);
    }
    SetAttrSubmitMode(executionContext, "tc_requesttype", "always");
    setApprovalRequestUser(executionContext);
    addCustFilterToRqtReasons(executionContext);
    addCustFilterToNewCatgry(executionContext);
}

function setApprovalRequestUser(executionContext) {
    var UserID = new Array();
    var approvee = {
        id: ContextUserDetails.userId,
        entityType: "systemuser",
        name: ContextUserDetails.userName
    }
    UserID.push(approvee);
    SetAttributeValue(executionContext, "tc_requestedby", UserID);
    SetAttrSubmitMode(executionContext, "tc_requestedby", "always");
}

function addCustFilterToRqtReasons(executionContext) {
    // add the event handler for PreSearch Event
    var ctrlRqtReasons = GetControl(executionContext, "tc_requestreason");
    if (ctrlRqtReasons != null) {
        ctrlRqtReasons.addPreSearch(addRqtReasonFilter);
    }
}

function addRqtReasonFilter() {
    //check if the city is not empty
    var rqtTypeVal = GetAttributeValue(formExecuteContext, "tc_requesttype");
    if (rqtTypeVal != null) {
        //create a filter xml
        var customFilter = "<filter type='and'>" +
            "<condition attribute='tc_reasontype' operator='eq' value='" + rqtTypeVal + "'/>" +
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