var roleAgtSupMng = "AgtSupMng", roleAgtSupMngExtAgtExtBM = "AgtSupMngExtAgtExtBM", roleSupMng = "SupMng";
var roleCheckForPickReleaseFunctionality = "PickandReleaseCaseFunctionality", roleCheckForOtherFunctionalities = "OtherCaseFunctionalities", roleCheckForSupervisorAndManager = "SkipManagersandSupervisors";
var CRMAppURL = Xrm.Utility.getGlobalContext().getCurrentAppUrl();
var curentuserID = null, userLCID = GetContextUserLCID();

function btnSaveandSubmit_EnableRule(executionContext) {
    var roleCheckConditions = checkForCaseFunctionalities(roleAgtSupMng);
    var isSubmttedVal = GetAttributeValue(executionContext, "tc_issubmitted");
    var objCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
    var stageName = GetBPFActiveStageName(executionContext);
    var caseSLAstatus = GetAttributeValue(executionContext, "resolvebyslastatus");
    var showSaveandSubmit = false;
    if (isSubmttedVal == false && (objCaseTypeName == "complaint" || objCaseTypeName == "شكوى") && (stageName == "initiate" || stageName == "أنشئ") && caseSLAstatus != 5 && roleCheckConditions) {
        showSaveandSubmit = true;
    }
    return showSaveandSubmit;
}

function btnSaveandSubmittoMe_CustomAct(executionContext, targetRecordId) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: SaveandSubmittoMeMsgAR, title: submitCasetitleMsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: SaveandSubmittoMeMsgEN, title: submitCasetitleMsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttributeValue(executionContext, "tc_issubmitted", true);
                RefreshFormData(executionContext, true);

                if (targetRecordId != null) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();
                    switch (userLCID) {
                        case 1025: //Arabic
                            ShowProgressIndicator(ProcessingCaseSubmissionMsgAR);
                            break;
                        case 1033: //English
                        default:
                            ShowProgressIndicator(ProcessingCaseSubmissionMsgEN);
                            break;
                    }
                    setTimeout(function () {
                        CloseProgressIndicator();
                        var entityFormOptions = {};
                        entityFormOptions["entityName"] = "incident";
                        entityFormOptions["entityId"] = targetRecordId;

                        // Reload the Target Record.
                        Xrm.Navigation.openForm(entityFormOptions);
                    }, 10000);
                }
                else {
                    SetAttrsDisability(executionContext, "customerid, tc_casetype, tc_category, tc_subcategory, prioritycode", ", ", true);
                }
            }
        });
}

function btnSaveandSubmittoTeam_CustomAct(executionContext, targetRecordId) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: SaveandSubmittoMeMsgAR, title: submitCasetitleMsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: SaveandSubmittoMeMsgEN, title: submitCasetitleMsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttributeValue(executionContext, "tc_issubmitted", true);
                SetAttributeValue(executionContext, "tc_submittoexternalteam", true);
                RefreshFormData(executionContext, true);

                if (targetRecordId != null) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();
                    switch (userLCID) {
                        case 1025: //Arabic
                            ShowProgressIndicator(ProcessingCaseSubmissionToTeamMsgAR);
                            break;
                        case 1033: //English
                        default:
                            ShowProgressIndicator(ProcessingCaseSubmissionToTeamMsgEN);
                            break;
                    }
                    setTimeout(function () {
                        CloseProgressIndicator();
                        if (CRMAppURL == undefined || CRMAppURL == null) {
                            CRMAppURL = Xrm.Utility.getGlobalContext().getCurrentAppUrl();
                        }

                        var redirectUrl = CRMAppURL + "&pagetype=entitylist&etn=incident";
                        window.top.location.href = redirectUrl;
                    }, 10000);
                }
                else {
                    SetAttrsDisability(executionContext, "customerid, tc_casetype, tc_category, tc_subcategory, prioritycode", ", ", true);
                }
            }
        });
}

function btnCaseApprovalRqt_EnableRule(executionContext) {
    var showCaseRequest = false, roleCheckConditions = null, checkForRespectiveCaseActiveCAseApps = null;
    roleCheckConditions = checkForCaseFunctionalities(roleAgtSupMng);
    checkForRespectiveCaseActiveCAseApps = checkForRespectiveCaseActiveCAs(executionContext);

    if (checkForRespectiveCaseActiveCAseApps != null && !checkForRespectiveCaseActiveCAseApps && roleCheckConditions != null && roleCheckConditions) {
        showCaseRequest = true;
    }
    return showCaseRequest;
}

function btnPauseSLA_EnableRule(executionContext) {
    var showPauseSLABtn = false;
    var objCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
    var caseSLAStatusValue = GetAttributeValue(executionContext, "resolvebyslastatus");
    if ((objCaseTypeName == "complaint" || objCaseTypeName == "شكوى") && caseSLAStatusValue != null && caseSLAStatusValue != 5 && caseSLAStatusValue != 4) { //SLA Status != 'Pause SLA'
        showPauseSLABtn = true;
    }
    return showPauseSLABtn;
}

function btnPauseSLA_CustomAct(executionContext) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: btnPauseSLAmsgAR, title: titlePauseSLAmsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: btnPauseSLAmsgEN, title: titlePauseSLAmsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttrsDisability(executionContext, "tc_category, tc_subcategory, prioritycode", ", ", true);

                var checkSupManFlag = checkForCaseFunctionalities(roleSupMng);

                if (checkSupManFlag) {
                    SetTabVisibility(executionContext, "tab_approvalrqtreasons", true);
                    SetSectionVisibility(executionContext, "tab_approvalrqtreasons", "sec_cancelcaserqt", false);
                    SetSectionVisibility(executionContext, "tab_approvalrqtreasons", "sec_pauseslarqt", true);
                    SetAttrRequiredLevel(executionContext, "tc_pauseslareason", "required");
                    SetFieldFocus(executionContext, "tc_pauseslareason");
                }
                else {
                    SetAttrsRequiredLevel(executionContext, "tc_cancellationreason, tc_cancelcaserequestercomments, tc_pauseslareason, tc_pauseslarequestercomments", ", ", "none");
                    SetTabVisibility(executionContext, "tab_approvalrqtreasons", false);

                    var requestTitle = "", requestType = null;
                    switch (userLCID) {
                        case 1025: //Arabic
                            requestTitle = PauseSLArequestTypetitleMsgAR;
                            break;
                        case 1033: //English
                        default:
                            requestTitle = PauseSLArequestTypetitleMsgEN;
                            break;
                    }
                    requestType = 1;

                    var entityFormOptions = {}, formParameters = {};
                    entityFormOptions["entityName"] = "tc_caseapproval";
                    entityFormOptions["useQuickCreateForm"] = true;
                    entityFormOptions["windowPosition"] = 2;

                    //Set default values for the Case Approval form    
                    formParameters["subject"] = requestTitle;
                    formParameters["tc_requesttype"] = requestType;

                    //set the regarding case parameters
                    formParameters["regardingobjectid"] = executionContext.data.entity.getId();
                    formParameters["regardingobjectidname"] = executionContext.data.entity.getPrimaryAttributeValue();
                    formParameters["regardingobjectidtype"] = executionContext.data.entity.getEntityName();

                    // Open the Quick Create form.
                    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                        function (success) {
                            if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                                showRequestTypeMsgs(executionContext, 1); //Pause SLA
                            }
                        },
                        function (error) {
                            alert(error);
                        });
                }
            }
        });
}

function btnChangeCategory_EnableRule(executionContext) {
    var showChangeCategBtn = false;
    var bpfActiveStageName = GetBPFActiveStageName(executionContext);
    if (bpfActiveStageName != "initiate" || bpfActiveStageName != "أنشئ") {
        showChangeCategBtn = true;
    }
    return showChangeCategBtn;
}

function btnChangeCategory_CustomAct(executionContext) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: ChangeCategorizationmsgAR, title: titleChangeCategorizationmsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: ChangeCategorizationmsgEN, title: titleChangeCategorizationmsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttrsRequiredLevel(executionContext, "tc_cancellationreason, tc_cancelcaserequestercomments, tc_pauseslareason, tc_pauseslarequestercomments", ", ", "none");
                SetTabVisibility(executionContext, "tab_approvalrqtreasons", false);
                SetAttrDisability(executionContext, "prioritycode", true);

                var checkSupManFlag = checkForCaseFunctionalities(roleSupMng);

                if (checkSupManFlag) {
                    SetAttrsDisability(executionContext, "tc_category, tc_subcategory", ", ", false);
                    SetFieldFocus(executionContext, "tc_category");
                }
                else {
                    //tc_category
                    var requestTitle = "", requestType = null;
                    switch (userLCID) {
                        case 1025: //Arabic
                            requestTitle = ChangeCategorizationrequestTypetitleMsgAR;
                            break;
                        case 1033: //English
                        default:
                            requestTitle = ChangeCategorizationrequestTypetitleMsgEN;
                            break;
                    }
                    requestType = 2;

                    var entityFormOptions = {}, formParameters = {};
                    entityFormOptions["entityName"] = "tc_caseapproval";
                    entityFormOptions["useQuickCreateForm"] = true;
                    entityFormOptions["windowPosition"] = 2;

                    formParameters["tc_customercategory"] = GetLookupValId(executionContext, "tc_customercategory");
                    formParameters["tc_casetype"] = GetLookupValId(executionContext, "tc_casetype");

                    //set the old case category parameters
                    formParameters["tc_oldcasecategory"] = GetLookupValId(executionContext, "tc_category");
                    formParameters["tc_oldcasecategoryname"] = GetLookupValName(executionContext, "tc_category");
                    formParameters["tc_oldcasecategorytype"] = GetLookupEntityType(executionContext, "tc_category");

                    //set the old case sub category parameters
                    formParameters["tc_oldcasesubcategory"] = GetLookupValId(executionContext, "tc_subcategory");
                    formParameters["tc_oldcasesubcategoryname"] = GetLookupValName(executionContext, "tc_subcategory");
                    formParameters["tc_oldcasesubcategorytype"] = GetLookupEntityType(executionContext, "tc_subcategory");

                    //set the old priority parameters
                    formParameters["tc_oldpriority"] = GetAttributeValue(executionContext, "prioritycode");

                    //Set default values for the Case Approval form    
                    formParameters["subject"] = requestTitle;
                    formParameters["tc_requesttype"] = requestType;

                    //set the regarding case parameters
                    formParameters["regardingobjectid"] = executionContext.data.entity.getId();
                    formParameters["regardingobjectidname"] = executionContext.data.entity.getPrimaryAttributeValue();
                    formParameters["regardingobjectidtype"] = executionContext.data.entity.getEntityName();

                    // Open the Quick Create form.
                    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                        function (success) {
                            if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                                showRequestTypeMsgs(executionContext, 2); //Change Case Category
                            }
                        },
                        function (error) {
                            alert(error);
                        });
                }
            }
        });
}

function btnCancelCase_EnableRule(executionContext) {
    var showCancelCaseBtn = false, checkForRespectiveCaseActiveCAseApps = null;
    checkForRespectiveCaseActiveCAseApps = checkForRespectiveCaseActiveCAs(executionContext);
    if (checkForRespectiveCaseActiveCAseApps != null && !checkForRespectiveCaseActiveCAseApps) {
        showCancelCaseBtn = true;
    }
    return showCancelCaseBtn;
}

function btnCancelCase_CustomAct(executionContext) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: btnCancelCasemsgAR, title: titleCancelCasemsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: btnCancelCasemsgEN, title: titleCancelCasemsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttrsDisability(executionContext, "tc_category, tc_subcategory, prioritycode", ", ", true);

                var checkSupManFlag = checkForCaseFunctionalities(roleSupMng);

                if (checkSupManFlag) {
                    SetTabVisibility(executionContext, "tab_approvalrqtreasons", true);
                    SetSectionVisibility(executionContext, "tab_approvalrqtreasons", "sec_pauseslarqt", false);
                    SetSectionVisibility(executionContext, "tab_approvalrqtreasons", "sec_cancelcaserqt", true);
                    SetAttrRequiredLevel(executionContext, "tc_cancellationreason", "required");
                    SetFieldFocus(executionContext, "tc_cancellationreason");
                }
                else {
                    SetAttrsRequiredLevel(executionContext, "tc_cancellationreason, tc_cancelcaserequestercomments, tc_pauseslareason, tc_pauseslarequestercomments", ", ", "none");
                    SetTabVisibility(executionContext, "tab_approvalrqtreasons", false);

                    var requestTitle = "", requestType = null;
                    switch (userLCID) {
                        case 1025: //Arabic
                            requestTitle = CancelCaserequestTypetitleMsgAR;
                            break;
                        case 1033: //English
                        default:
                            requestTitle = CancelCaserequestTypetitleMsgEN;
                            break;
                    }
                    requestType = 3;

                    var entityFormOptions = {}, formParameters = {};
                    entityFormOptions["entityName"] = "tc_caseapproval";
                    entityFormOptions["useQuickCreateForm"] = true;
                    entityFormOptions["windowPosition"] = 2;

                    //Set default values for the Case Approval form    
                    formParameters["subject"] = requestTitle;
                    formParameters["tc_requesttype"] = requestType;

                    //set the regarding case parameters
                    formParameters["regardingobjectid"] = executionContext.data.entity.getId();
                    formParameters["regardingobjectidname"] = executionContext.data.entity.getPrimaryAttributeValue();
                    formParameters["regardingobjectidtype"] = executionContext.data.entity.getEntityName();

                    // Open the Quick Create form.
                    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                        function (success) {
                            if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                                showRequestTypeMsgs(executionContext, 3); //Cancel case
                            }
                        },
                        function (error) {
                            alert(error);
                        });
                }
            }
        });
}

function btnResume_EnableRule(executionContext) {
    var showResume = false;
    var objCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
    var caseSLAstatus = GetAttributeValue(executionContext, "resolvebyslastatus");
    if (caseSLAstatus == 5 && (objCaseTypeName == "complaint" || objCaseTypeName == "شكوى")) { //CaseSLAStatus == "SLA Paused"
        showResume = true;
    }
    return showResume;
}

function btnResume_CustomAct(executionContext) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: btnResumeSLAmsgAR, title: titleResumeSLAmsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: btnResumeSLAmsgEN, title: titleResumeSLAmsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                var caseSLAstatus = GetAttributeValue(executionContext, "resolvebyslastatus");
                var objCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
                if (caseSLAstatus == 5 && (objCaseTypeName == "complaint" || objCaseTypeName == "شكوى")) {
                    SetAttributeValue(executionContext, "resolvebyslastatus", 1);
                    executionContext.data.refresh(true);
                }
            }
        });
}

function btnChangeCasePriority_EnableRule(executionContext) {
    var showChangeCategBtn = false, checkForRespectiveCaseActiveCAseApps = null;
    checkForRespectiveCaseActiveCAseApps = checkForRespectiveCaseActiveCAs(executionContext);

    if (checkForRespectiveCaseActiveCAseApps != null && !checkForRespectiveCaseActiveCAseApps) {
        showChangeCategBtn = true;
    }
    return showChangeCategBtn;
}

function btnChangeCasePriority_CustomAct(executionContext) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: btnChangePrioritymsgAR, title: titleChangePrioritymsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: btnChangePrioritymsgEN, title: titleChangePrioritymsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttrsRequiredLevel(executionContext, "tc_cancellationreason, tc_cancelcaserequestercomments, tc_pauseslareason, tc_pauseslarequestercomments", ", ", "none");
                SetTabVisibility(executionContext, "tab_approvalrqtreasons", false);
                SetAttrsDisability(executionContext, "tc_category, tc_subcategory", ", ", true);

                var checkSupManFlag = checkForCaseFunctionalities(roleSupMng);

                if (checkSupManFlag) {
                    SetAttrDisability(executionContext, "prioritycode", false);
                    SetFieldFocus(executionContext, "prioritycode");
                }
                else {
                    var requestTitle = "", requestType = null;
                    switch (userLCID) {
                        case 1025: //Arabic
                            requestTitle = ChangePriorityrequestTypetitleMsgAR;
                            break;
                        case 1033: //English
                        default:
                            requestTitle = ChangePriorityrequestTypetitleMsgEN;
                            break;
                    }
                    requestType = 4;
                    var entityFormOptions = {}, formParameters = {};
                    entityFormOptions["entityName"] = "tc_caseapproval";
                    entityFormOptions["useQuickCreateForm"] = true;
                    entityFormOptions["windowPosition"] = 2;

                    formParameters["tc_customercategory"] = GetLookupValId(executionContext, "tc_customercategory");
                    formParameters["tc_casetype"] = GetLookupValId(executionContext, "tc_casetype");

                    //set the old case category parameters
                    formParameters["tc_oldcasecategory"] = GetLookupValId(executionContext, "tc_category");
                    formParameters["tc_oldcasecategoryname"] = GetLookupValName(executionContext, "tc_category");
                    formParameters["tc_oldcasecategorytype"] = GetLookupEntityType(executionContext, "tc_category");

                    //set the old case sub category parameters
                    formParameters["tc_oldcasesubcategory"] = GetLookupValId(executionContext, "tc_subcategory");
                    formParameters["tc_oldcasesubcategoryname"] = GetLookupValName(executionContext, "tc_subcategory");
                    formParameters["tc_oldcasesubcategorytype"] = GetLookupEntityType(executionContext, "tc_subcategory");

                    //set the old priority parameters
                    formParameters["tc_oldpriority"] = GetAttributeValue(executionContext, "prioritycode");

                    //Set default values for the Case Approval form    
                    formParameters["subject"] = requestTitle;
                    formParameters["tc_requesttype"] = requestType;

                    //set the regarding case parameters
                    formParameters["regardingobjectid"] = executionContext.data.entity.getId();
                    formParameters["regardingobjectidname"] = executionContext.data.entity.getPrimaryAttributeValue();
                    formParameters["regardingobjectidtype"] = executionContext.data.entity.getEntityName();

                    // Open the Quick Create form.
                    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                        function (success) {
                            if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                                showRequestTypeMsgs(executionContext, 4); //Change Case Priority
                            }
                        },
                        function (error) {
                            alert(error);
                        });
                }
            }
        });
}

function btnPick_EnableRule(executionContext) {
    var showPick = false, roleNamesForPRCF = null;
    roleNamesForPRCF = checkForCaseFunctionalities(roleAgtSupMngExtAgtExtBM);

    if (roleNamesForPRCF) {
        var caseWorkedBy = GetLookupValId(executionContext, "tc_caseworkedby");
        if (caseWorkedBy == null) {
            showPick = true;
        }
    }

    return showPick;
}

function btnPick_CustomAct(targetRecordId) {
    if (targetRecordId != null && targetRecordId.length > 0) {
        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { text: btnPickCasemsgAR, title: titlePickCasemsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { text: btnPickCasemsgEN, title: titlePickCasemsgEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();
                    curentuserID = GetContextUserId();

                    var usrAssignee = {}, usrAssignedBy = {}, actionQry = null, actionInputData = {};
                    usrAssignee.systemuserid = curentuserID;
                    usrAssignedBy.systemuserid = curentuserID;

                    actionInputData.Assignee = usrAssignee;
                    actionInputData.AssingedBy = usrAssignedBy;
                    actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_PickCaseFunctionality";

                    ShowProgressIndicator("Processing Pick Case....");
                    var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, actionInputData, false, null);
                    CloseProgressIndicator();

                    if (actionOutput != null && actionOutput != undefined && actionOutput.IsCasePicked) {
                        var entityFormOptions = {};
                        entityFormOptions["entityName"] = "incident";
                        entityFormOptions["entityId"] = targetRecordId;

                        // Reload the Target Record.
                        Xrm.Navigation.openForm(entityFormOptions);
                    }
                }
            });
    }
}

function btnRelease_EnableRule(executionContext) {
    var showRelease = false, roleNamesForPRCF = null, roleNamesForSSM = null;
    roleNamesForPRCF = checkForCaseFunctionalities(roleAgtSupMngExtAgtExtBM);
    roleNamesForSSM = checkForCaseFunctionalities(roleSupMng);

    if (roleNamesForPRCF) {
        var caseWorkedBy = GetLookupValId(executionContext, "tc_caseworkedby");

        if (roleNamesForSSM) {
            if (caseWorkedBy != null) {
                showRelease = true;
            }
        }
        else {

            if (curentuserID == null) {
                curentuserID = GetContextUserId();
            }
            if (caseWorkedBy != null && curentuserID != null && curentuserID == caseWorkedBy.toLowerCase()) {
                showRelease = true;
            }
        }
    }
    return showRelease;
}

function btnRelease_CustomAct(targetRecordId) {
    if (targetRecordId != null && targetRecordId.length > 0) {
        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { text: btnReleaseCasemsgAR, title: titleReleaseCasemsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { text: btnReleaseCasemsgEN, title: titleReleaseCasemsgEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();
                    if (curentuserID == null) {
                        curentuserID = GetContextUserId();
                    }

                    var usrReleasedBy = {}, actionQry = null, actionInputData = {};

                    usrReleasedBy.systemuserid = curentuserID;
                    actionInputData.ReleasedBy = usrReleasedBy;
                    actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_ReleaseCaseFunctionality";

                    ShowProgressIndicator("Processing Release Case....");
                    var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, actionInputData, false, null);
                    CloseProgressIndicator();


                    if (actionOutput != null && actionOutput != undefined && actionOutput.IsCaseReleased) {
                        if (CRMAppURL == undefined || CRMAppURL == null) {
                            CRMAppURL = Xrm.Utility.getGlobalContext().getCurrentAppUrl();
                        }

                        var redirectUrl = CRMAppURL + "&pagetype=entitylist&etn=incident";
                        window.top.location.href = redirectUrl;
                    }
                }
            });
    }
}

function btnAssign_EnableRule() {
    var showAssign = false, roleNamesForSSM = null;
    roleNamesForSSM = checkForCaseFunctionalities(roleSupMng);

    if (roleNamesForSSM) {
        showAssign = true;
    }
    return showAssign;
}

function btnAssign_CustomAct(targetRecordId) {
    var selectedUser = null;

    if (targetRecordId != null && targetRecordId != undefined) {
        var lookupOptions =
        {
            defaultEntityType: "systemuser",
            entityTypes: ["systemuser"],
            allowMultiSelect: false,
            filters: [{ filterXml: "<filter type='and'><condition attribute='isdisabled' value='0' operator='eq'/ /></filter>", entityLogicalName: "systemuser" }]
        };

        // Get account records based on the lookup Options
        Xrm.Utility.lookupObjects(lookupOptions).then(
            function (results) {
                if (results != null && results.length > 0) {
                    selectedUser = results[0];
                    var confirmStrings = null;
                    switch (userLCID) {
                        case 1025: //Arabic
                            confirmStrings = { title: titleAssignmsgAR, text: btnAssignmsgAR, confirmButtonLabel: AssignconfirmButtonLabelAR, cancelButtonLabel: AssigncancelButtonLabelAR };
                            break;
                        case 1033: //English
                        default:
                            confirmStrings = { title: titleAssignmsgEN, text: btnAssignmsgEN, confirmButtonLabel: AssignconfirmButtonLabelEN, cancelButtonLabel: AssigncancelButtonLabelEN };
                            break;
                    }

                    var confirmOptions = { height: 200, width: 450 };

                    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                        function (success) {
                            if (success.confirmed && selectedUser != null) {
                                targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();

                                if (curentuserID == null) {
                                    curentuserID = GetContextUserId();
                                }

                                var usrAssignee = {}, usrAssignedBy = {}, actionQry = null, actionInputData = {};
                                usrAssignee.systemuserid = selectedUser.id.replace("{", "").replace("}", "").toLowerCase(); // GUID of the lookup id;
                                usrAssignedBy.systemuserid = curentuserID;

                                actionInputData.Assignee = usrAssignee;
                                actionInputData.AssignedBy = usrAssignedBy;
                                actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_AssignCaseFunctionality";

                                ShowProgressIndicator("Processing Assign Case....");
                                var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, actionInputData, false, null);
                                CloseProgressIndicator();

                                if (actionOutput != null && actionOutput != undefined && actionOutput.IsCaseAssigned) {
                                    var entityFormOptions = {};
                                    entityFormOptions["entityName"] = "incident";
                                    entityFormOptions["entityId"] = targetRecordId;

                                    // Reload the Target Record.
                                    Xrm.Navigation.openForm(entityFormOptions);
                                }
                            }
                        });
                }
            }
        );
    }
}

function btnMomentofDelight_EnableRule(executionContext) {
    var showMomentofDelightbtn = false, checkForRelatedCaseActiveMoMApps = null, stageName = null, roleCheckConditions = null;
    roleCheckConditions = checkForCaseFunctionalities(roleAgtSupMng);
    checkForRelatedCaseActiveMoMApps = checkForRespectiveCaseActiveCAs(executionContext, 5);
    stageName = GetBPFActiveStageName(executionContext);

    if ((stageName == "close" || stageName == "إغلاق") && checkForRelatedCaseActiveMoMApps != null && !checkForRelatedCaseActiveMoMApps && roleCheckConditions != null && roleCheckConditions) {
        showMomentofDelightbtn = true;
    }

    return showMomentofDelightbtn;
}

function btnMomentofDelight_CustomAct(executionContext) {
    var confirmStrings = null;
    switch (userLCID) {
        case 1025: //Arabic
            confirmStrings = { text: btnMomentofDelightmsgAR, title: titleMomentofDelightmsgAR };
            break;
        case 1033: //English
        default:
            confirmStrings = { text: btnMomentofDelightmsgEN, title: titleMomentofDelightmsgEN };
            break;
    }

    var confirmOptions = { height: 200, width: 450 };

    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                SetAttrsRequiredLevel(executionContext, "tc_cancellationreason, tc_cancelcaserequestercomments, tc_pauseslareason, tc_pauseslarequestercomments", ", ", "none");
                SetTabVisibility(executionContext, "tab_approvalrqtreasons", false);
                SetAttrsDisability(executionContext, "tc_category, tc_subcategory, prioritycode", ", ", true);

                var requestTitle = "", requestType = null;
                switch (userLCID) {
                    case 1025: //Arabic
                        requestTitle = MomentofDelightrequestTypetitleMsgAR;
                        break;
                    case 1033: //English
                    default:
                        requestTitle = MomentofDelightrequestTypetitleMsgEN;
                        break;
                }
                requestType = 5;

                var entityFormOptions = {}, formParameters = {};
                entityFormOptions["entityName"] = "tc_caseapproval";
                entityFormOptions["useQuickCreateForm"] = true;
                entityFormOptions["windowPosition"] = 2;

                //Set default values for the Case Approval form    
                formParameters["subject"] = requestTitle;
                formParameters["tc_requesttype"] = requestType;
                //set the regarding case parameters
                formParameters["tc_relatedcase"] = executionContext.data.entity.getId();
                formParameters["tc_relatedcasename"] = executionContext.data.entity.getPrimaryAttributeValue();
                formParameters["tc_relatedcasetype"] = executionContext.data.entity.getEntityName();

                // Open the Quick Create form.
                Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                    function (success) {
                        if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                            RefreshFormRibbon(executionContext, false);
                        }
                    },
                    function (error) {
                        alert(error);
                    });
            }
        });
}

function btnSendSuggestion_EnableRule(executionContext) {
    var showSendSuggestionbtn = false;
    var objCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
    if (objCaseTypeName == "suggestion" || objCaseTypeName == "اقتراحات") {
        showSendSuggestionbtn = true;
    }
    return showSendSuggestionbtn;
}

function btnSendSuggestion_CustomAct(executionContext) {
    var querySelect = "queues?$select=name,queueid&$filter=contains(name, 'Visitor%20Care%20Team')";
    var qryResults = RetrieveMultipleRecords(querySelect, false, null);
    var queueid = null, queuename = null;
    if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
        queueid = qryResults.value[0]["queueid"];
        queuename = qryResults.value[0]["name"];
    }

    if (queueid != null) {
        var entityFormOptions = {}, formParameters = {};
        entityFormOptions["entityName"] = "email";
        entityFormOptions["useQuickCreateForm"] = false;
        entityFormOptions["windowPosition"] = 1;

        //Set default values for the Email form
        var fromValue = new Array();
        fromValue[0] = new Object();
        fromValue[0].id = queueid; // GUID of the lookup id
        fromValue[0].name = queuename; // Name of the lookup
        fromValue[0].entityType = "queue";
        formParameters["from"] = fromValue;

        //set the regarding case parameters
        formParameters["regardingobjectid"] = executionContext.data.entity.getId();
        formParameters["regardingobjectidname"] = executionContext.data.entity.getPrimaryAttributeValue();
        formParameters["regardingobjectidtype"] = executionContext.data.entity.getEntityName();
        // Open the Quick Create form.
        Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
            function (success) {
                if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                    RefreshFormRibbon(executionContext, false);
                }
            },
            function (error) {
                alert(error);
            });
    }
}

function btnResolveCase_EnableRule(executionContext) {
    var showResolveCasebtn = false, checkForRespectiveCaseActiveCAseApps = null, stageName = null, roleCheckConditions = null;
    checkForRespectiveCaseActiveCAseApps = checkForRespectiveCaseActiveCAs(executionContext);
    roleCheckConditions = checkForCaseFunctionalities(roleAgtSupMng);
    stageName = GetBPFActiveStageName(executionContext);

    if (checkForRespectiveCaseActiveCAseApps != null && !checkForRespectiveCaseActiveCAseApps && (stageName == "close" || stageName == "إغلاق") && roleCheckConditions != null && roleCheckConditions) {
        var objCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
        var roleCheckToResolve = null;

        switch (objCaseTypeName) {
            case "complaint": //Complaint
            case "شكوى": //Complaint
                roleCheckToResolve = checkForCaseFunctionalities(roleAgtSupMng);

                if (roleCheckToResolve) {
                    showResolveCasebtn = true;
                }
                break;
            case "out of scope": //Out of Scope
            case "أخرى": //Out of Scope
                roleCheckToResolve = checkForCaseFunctionalities(roleSupMng);

                if (roleCheckToResolve) {
                    showResolveCasebtn = true;
                }
                break;
            case "emergency": //Emergency
            case "حالة طوارئ": //Emergency
            case "enquiry": //Enquiry
            case "استفسار": //Enquiry
            case "suggestion": //Suggestion
            case "اقتراحات": //Suggestion 
                showResolveCasebtn = true;
                break;
        }
    }

    return showResolveCasebtn;
}

function btnResolveCase_CustomAct(targetRecordId) {
    if (targetRecordId != null && targetRecordId.length > 0) {
        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { text: btnResolveCasemsgAR, title: titleResolveCasemsgAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { text: btnResolveCasemsgEN, title: titleResolveCasemsgEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    targetRecordId = targetRecordId[0].replace("{", "").replace("}", "").toLowerCase();
                    var actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_ResolveCaseFunctionality";
                    switch (userLCID) {
                        case 1025: //Arabic
                            ShowProgressIndicator(ProcessingCaseResolveMsgAR);
                            break;
                        case 1033: //English
                        default:
                            ShowProgressIndicator(ProcessingCaseResolveMsgEN);
                            break;
                    }
                    var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, null, false, null);
                    CloseProgressIndicator();

                    if (actionOutput != null && actionOutput != undefined && actionOutput.IsCaseResolved) {
                        var entityFormOptions = {};
                        entityFormOptions["entityName"] = "incident";
                        entityFormOptions["entityId"] = targetRecordId;

                        // Reload the Target Record.
                        Xrm.Navigation.openForm(entityFormOptions);
                    }
                }
            });
    }
}

function checkForCaseFunctionalities(roleCheckParam) {
    var roleCheckFlag = false, configParamVal = null, currentUserRoles = null;

    currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();

    switch (roleCheckParam) {
        case "AgtSupMngExtAgtExtBM":
            configParamVal = GetConfigParameterValue(roleCheckForPickReleaseFunctionality, false, null);
            break;
        case "AgtSupMng":
            configParamVal = GetConfigParameterValue(roleCheckForOtherFunctionalities, false, null);
            break;
        case "SupMng":
            configParamVal = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);
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

function checkForRespectiveCaseActiveCAs(executionContext, rqtType) {
    var checkForAnyActiveCAs = false;
    var loggedUserId = GetContextUserId();
    var targetCaseId = GetTargetRecordId(executionContext);

    if (targetCaseId != null) {
        var qryForRqtTypes = null, qryResultsRqtTypes = null;
        switch (rqtType) {
            case 1: //Pause SLA
            case 2: //Change Categories
            case 3: //Cancel Case
            case 4: //Change Priority
            default:
                qryForRqtTypes = "tc_caseapprovals?$select=tc_requesttype&$filter=_regardingobjectid_value eq " + targetCaseId + " and  (statecode eq 0 or statecode eq 3)";
                break;
            case 5: // Moment of Delight
                qryForRqtTypes = "tc_caseapprovals?$select=tc_requesttype&$filter=tc_requesttype eq 5 and _tc_relatedcase_value eq " + targetCaseId + " and (statecode eq 0 or statecode eq 3)";
                if (loggedUserId != null) {
                    qryForRqtTypes += " and _tc_requestedby_value eq " + loggedUserId;
                }
                break;
        }

        qryResultsRqtTypes = RetrieveMultipleRecords(qryForRqtTypes, false, null);

        if (qryResultsRqtTypes != null && qryResultsRqtTypes.value != null && qryResultsRqtTypes.value.length > 0) {
            checkForAnyActiveCAs = true;
        }
    }

    return checkForAnyActiveCAs;
}