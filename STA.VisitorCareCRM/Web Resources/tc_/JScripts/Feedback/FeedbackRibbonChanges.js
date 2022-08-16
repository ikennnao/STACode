var roleTrainMngDrctor = "TrainMngDirector";
var roleCheckForFeedbackStatus = "KACreateAmendAccess";
var curentuserID = null, userLCID = GetContextUserLCID();

function btnFeedbackStatus_EnableRule(executionContext) {
    var roleCheckConditions = checkForRoleFunctionalities(roleTrainMngDrctor);
    var statecodeVal = GetAttributeValue(executionContext, "statecode");
    var feedbackStatusVal = GetAttributeValue(executionContext, "tc_feedbackstatus");
    var showFeedbackStatus = false;

    if (roleCheckConditions && statecodeVal == 0 && (feedbackStatusVal == 3 || feedbackStatusVal == null)) { // Status == 'Open' && (Feedback Status == 'Pending' || Feedback Status == null)
        showFeedbackStatus = true;
    }
    return showFeedbackStatus;
}

function btnApprove_CustomAct(targetRecordId, targetEntName) {
    if (targetRecordId != null && targetEntName != null && targetRecordId.length > 0) {
        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { text: txtApproveMsgEN, title: titleApproveMsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { text: txtApproveMsgEN, title: titleApproveMsgEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();

                    var entUpdateFeedbackRcrd = {}, entityFormOptions = {};
                    entUpdateFeedbackRcrd.tc_feedbackstatus = 1; //Approve
                    entUpdateFeedbackRcrd.statecode = 1; //Closed
                    entUpdateFeedbackRcrd.statuscode = 3; //Approved

                    entityFormOptions["entityName"] = targetEntName;
                    entityFormOptions["entityId"] = targetRecordId;

                    ShowProgressIndicator("Approving the Target Feedback Record....");
                    UpdateRecordInCRMUsgWebApi(entUpdateFeedbackRcrd, targetEntName, targetRecordId, false, null, entityFormOptions);
                    CloseProgressIndicator();
                }
            });
    }
}

function btnDecline_CustomAct(targetRecordId, targetEntName) {
    if (targetRecordId != null && targetEntName != null && targetRecordId.length > 0) {
        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { text: txtDeclineMsgAR, title: titleDeclineMsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { text: txtDeclineMsgEN, title: titleDeclineMsgEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();

                    var entUpdateFeedbackRcrd = {}, entityFormOptions = {};
                    entUpdateFeedbackRcrd.tc_feedbackstatus = 2; //Approve
                    entUpdateFeedbackRcrd.statecode = 1; //Closed
                    entUpdateFeedbackRcrd.statuscode = 4; //Declined

                    entityFormOptions["entityName"] = targetEntName;
                    entityFormOptions["entityId"] = targetRecordId;

                    ShowProgressIndicator("Declining the Target Feedback Record....");
                    UpdateRecordInCRMUsgWebApi(entUpdateFeedbackRcrd, targetEntName, targetRecordId, false, null, entityFormOptions);
                    CloseProgressIndicator();
                }
            });
    }
}

function btnReactivate_EnableRule(executionContext) {
    var roleCheckConditions = checkForRoleFunctionalities(roleTrainMngDrctor);
    var statecodeVal = GetAttributeValue(executionContext, "statecode");
    var feedbackStatusVal = GetAttributeValue(executionContext, "tc_feedbackstatus");
    var showReactivate = false;

    if (roleCheckConditions && statecodeVal == 1 && (feedbackStatusVal == 1 || feedbackStatusVal == 2)) { // Status == 'Closed' && (Feedback Status == 'Approved' || Feedback Status == 'Rejected')
        showReactivate = true;
    }
    return showReactivate;
}

function btnReactivate_CustomAct(targetRecordId, targetEntName) {
    if (targetRecordId != null && targetEntName != null && targetRecordId.length > 0) {
        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { text: txtReactivateMsgEN, title: titleReactivateMsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { text: txtReactivateMsgEN, title: titleReactivateMsgEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();

                    var entUpdateFeedbackRcrd = {}, entityFormOptions = {};
                    entUpdateFeedbackRcrd.tc_feedbackstatus = 3; //Pending
                    entUpdateFeedbackRcrd.statecode = 0; //Open
                    entUpdateFeedbackRcrd.statuscode = 2; //Pending

                    entityFormOptions["entityName"] = targetEntName;
                    entityFormOptions["entityId"] = targetRecordId;

                    ShowProgressIndicator("Reactivating the Target Feedback Record....");
                    UpdateRecordInCRMUsgWebApi(entUpdateFeedbackRcrd, targetEntName, targetRecordId, false, null, entityFormOptions);
                    CloseProgressIndicator();
                }
            });
    }
}

function checkForRoleFunctionalities(roleCheckParam) {
    var roleCheckFlag = false, configParamVal = null, currentUserRoles = null;
    currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();

    switch (roleCheckParam) {
        case "TrainMngDirector":
            configParamVal = GetConfigParameterValue(roleCheckForFeedbackStatus, false, null);
            break;
    }

    if (configParamVal != null) {
        var configRoleNames = null;

        if (configParamVal.indexOf(",") > -1) {
            configRoleNames = configParamVal.split(',');
        }
        else {
            configRoleNames = configParamVal;
        }

        if (currentUserRoles != null && currentUserRoles.length > 0) {
            for (var i = 0; i < currentUserRoles.length; i++) {
                if (configRoleNames.includes(currentUserRoles[i].name.toLowerCase())) {
                    roleCheckFlag = true;
                    break;
                }
            }
        }
    }

    return roleCheckFlag;
}