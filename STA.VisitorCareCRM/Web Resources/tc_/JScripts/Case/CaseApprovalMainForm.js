function form_OnLoad(executionContext) {
    var requestType = GetAttributeValue(executionContext, "tc_requesttype");
    if (requestType == 2) { //change category
        SetAttrsVisibility(executionContext, "tc_oldcasecategory, tc_oldcasesubcategory, tc_newcasecategory, tc_newcasesubcateogry", true);
    }
}

function setApprovedByUser(executionContext) {
    var UserID = new Array();
    var approvee = {
        id: ContextUserDetails.userId,
        entityType: "systemuser",
        name: ContextUserDetails.userName
    }
    UserID.push(approvee);
    SetAttributeValue(executionContext, "tc_approvedby", UserID);
    SetAttrSubmitMode(executionContext, "tc_approvedby", "always");
}