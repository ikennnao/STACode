var CRMOrgURL = Xrm.Utility.getGlobalContext().getClientUrl();
var ContextUserDetails = Xrm.Utility.getGlobalContext().userSettings;
var resultSetForPickCases = null, resultSetForReleaseCases = null;
var curentuserID = null, currentUserRoles = null, userLCID = null, checkForSMRole = false;
var roleCheckForAgent = "PickandReleaseCaseFunctionality", roleCheckForSupervisorAndManager = "SkipManagersandSupervisors";

var PickOneCaseMsgEN = " case, would you like to pick it ?";
var PickOneCaseMsgAR = " حالة ، هل ترغب في اختيارها؟ ";
var PickMultiCaseMsgEN = " cases, would you like to pick them?";
var PickMultiCaseMsgAR = "الحالات ، هل ترغب في اختيارهم؟ ";
var ReleaseOneCaseMsgEN = " case, would you like to release it ?";
var ReleaseOneCaseMsgAR = " الحالة ، هل ترغب في تحريرها؟ ";
var ReleaseMultiCaseMsgEN = " cases, would you like to release them?";
var ReleaseMultiCaseMsgAR = "الحالات ، هل ترغب في تحريرهم؟ ";
var PickSelectCasemMsgEN = "You have selected ";
var PickSelectCasemMsgAR = " لقد قمت باختيار";
var confirmButtonLabelEN = "Confirm";
var confirmButtonLabelAR = "موافق";
var titlePickCasemsgEN = "Pick the Selected Cases";
var titlePickCasemsgAR = "اختيار الحالات المحددة";
var titleReleaseCasemsgEN = "Release the Selected Cases";
var titleReleaseCasemsgAR = "حرر الحالات المحددة";
var ProcessingPickCaseMsgEN = "Pick Case Processing....";
var ProcessingPickCaseMsgAR = "معالجة الحالة";
var ProcessingReleaseCaseMsgEN = "Release Case Processing....";
var ProcessingReleaseCaseMsgAR = "معالجة تحرير الحالة";
var AssignOneCaseMsgEN = " case, would you like to assign it to the selected user?";
var AssignOneCaseMsgAR = " الحالة ، هل ترغب في تخصيصها للمستخدم المحدد؟ ";
var AssignMultiCaseMsgEN = " cases, would you like to assign them to the selected user?";
var AssignMultiCaseMsgAR = "الحالات ، هل ترغب في تخصيصها للمستخدم المحدد؟ ";
var titleAssignCasemsgEN = "Assign Cases to Selected User";
var titleAssignCasemsgAR = "تعيين الحالات للمستخدم المحدد";
var AssignconfirmButtonLabelEN = "Assign";
var AssignconfirmButtonLabelAR = "تعيين";
var ProcessingAssignCaseMsgEN = "Assign Case Processing....";
var ProcessingAssignCaseMsgAR = "معالجة تعيين الحالة";
var AssigncancelButtonLabelEN = "Cancel";
var AssigncancelButtonLabelAR = "ألغاء";

function btnHPG_Pick_EnableRule(selectedRecordIds) {
    var showPick = false;

    if (selectedRecordIds != null && selectedRecordIds != undefined && selectedRecordIds.length > 0) {
        if (currentUserRoles == null) {
            currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();
        }

        var configParamVal = GetConfigParameterValue(roleCheckForAgent, false, null);

        if (configParamVal != null) {
            var roleNames = null;

            if (configParamVal.indexOf(',') > -1) {
                roleNames = configParamVal.split(',');
            }
            else {
                roleNames = configParamVal;
            }

            if (roleNames != null) {
                for (var i = 0; i < currentUserRoles.length; i++) {
                    if (roleNames.includes(currentUserRoles[i].name.toLowerCase())) {
                        var fetchXMLExpQryPC = FetchXML_PickCases(selectedRecordIds);

                        if (fetchXMLExpQryPC != null) {
                            resultSetForPickCases = FetchXML_GetRecords(fetchXMLExpQryPC, "incidents", false, null);
                        }

                        if (resultSetForPickCases != null && resultSetForPickCases.length > 0) {
                            showPick = true;
                            break;
                        }
                    }
                }
            }
        }
    }
    return showPick;
}

function btnHPG_Pick_CustomAct(selectedRecordIds, selectedCntrl) {
    var pickedCasesCount = 0;

    if (resultSetForPickCases == null) {
        var fetchXMLExpQryPC = FetchXML_PickCases(selectedRecordIds);

        if (fetchXMLExpQryPC != null) {
            resultSetForPickCases = FetchXML_GetRecords(fetchXMLExpQryPC, "incidents", false, null);
        }
    }

    if (resultSetForPickCases != null && resultSetForPickCases.length > 0) {
        var msgSubTitle = "";
        userLCID = GetContextUserLCID();

        if (resultSetForPickCases.length == 1) {
            switch (userLCID) {
                case 1025: //Arabic
                    msgSubTitle = PickSelectCasemMsgAR + selectedRecordIds.length + PickOneCaseMsgAR;
                    break;
                case 1033: //English
                default:
                    msgSubTitle = PickSelectCasemMsgEN + selectedRecordIds.length + PickOneCaseMsgEN;
                    break;
            }
        }
        else if (resultSetForPickCases.length > 1) {
            switch (userLCID) {
                case 1025: //Arabic
                    msgSubTitle = PickSelectCasemMsgAR + selectedRecordIds.length + PickMultiCaseMsgAR;
                    break;
                case 1033: //English
                default:
                    msgSubTitle = PickSelectCasemMsgEN + selectedRecordIds.length + PickMultiCaseMsgEN;
                    break;
            }
        }

        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { title: titlePickCasemsgAR, text: msgSubTitle, confirmButtonLabel: confirmButtonLabelAR };
                break;
            case 1033: //English
            default:
                confirmStrings = {
                    title: titlePickCasemsgEN, text: msgSubTitle, confirmButtonLabel: confirmButtonLabelEN
                };
                break;
        }

        var confirmOptions = { height: 200, width: 350 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    switch (userLCID) {
                        case 1025: //Arabic
                            ShowProgressIndicator(ProcessingPickCaseMsgAR);
                            break;
                        case 1033: //English
                        default:
                            ShowProgressIndicator(ProcessingPickCaseMsgEN);
                            break;
                    }

                    if (curentuserID == null) {
                        curentuserID = GetContextUserId();
                    }
                    for (var r = 0; r < resultSetForPickCases.length; r++) {
                        var targetRecordId = null;
                        targetRecordId = resultSetForPickCases[r].incidentid.replace("{", "").replace("}", "").toLowerCase();

                        if (targetRecordId != null) {
                            var usrAssignee = {}, usrAssignedBy = {}, actionQry = null, actionInputData = {};
                            usrAssignee.systemuserid = curentuserID;
                            usrAssignedBy.systemuserid = curentuserID;

                            actionInputData.Assignee = usrAssignee;
                            actionInputData.AssingedBy = usrAssignedBy;
                            actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_PickCaseFunctionality";

                            var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, actionInputData, false, null);

                            if (actionOutput != null && actionOutput != undefined && actionOutput.IsCasePicked) {
                                pickedCasesCount++;
                            }
                        }
                    }
                    CloseProgressIndicator();

                    if (pickedCasesCount > 0) {
                        selectedCntrl.refresh();
                    }
                }
                else {
                }
            });
    }
}

function btnHPG_Release_EnableRule(selectedRecordIds) {
    var showRelease = false, configParamValForPRCF = null, roleNamesForPRCF = null, configParamValForSMRoles = null, roleNamesForSM = null;

    if (selectedRecordIds != null && selectedRecordIds != undefined && selectedRecordIds.length > 0) {
        if (currentUserRoles == null) {
            currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();
        }

        configParamValForPRCF = GetConfigParameterValue(roleCheckForAgent, false, null);
        configParamValForSMRoles = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);

        if (configParamValForPRCF != null) {
            if (configParamValForPRCF.indexOf(',') > -1) {
                roleNamesForPRCF = configParamValForPRCF.split(',');
            }
            else {
                roleNamesForPRCF = configParamValForPRCF;
            }
        }
        if (configParamValForSMRoles != null) {
            roleNamesForSM = null;

            if (configParamValForSMRoles.indexOf(",") > -1) {
                roleNamesForSM = configParamValForSMRoles.split(',');
            }
            else {
                roleNamesForSM = configParamValForSMRoles;
            }
        }

        if (roleNamesForPRCF != null) {
            var fetchXMLExpQryRC = null;

            for (var k = 0; k < currentUserRoles.length; k++) {
                if (roleNamesForPRCF.includes(currentUserRoles[k].name.toLowerCase())) {
                    if (roleNamesForSM != null && roleNamesForSM.length > 0 && roleNamesForSM.includes(currentUserRoles[k].name.toLowerCase())) {
                        checkForSMRole = true;
                        fetchXMLExpQryRC = FetchXML_ReleaseCases(selectedRecordIds, checkForSMRole); //Fetch XML for Contact Center Supervisor/Manager
                    }
                    else {
                        fetchXMLExpQryRC = FetchXML_ReleaseCases(selectedRecordIds, checkForSMRole); //Fetch XML for Contact Center Agent
                    }

                    if (fetchXMLExpQryRC != null) {
                        resultSetForReleaseCases = FetchXML_GetRecords(fetchXMLExpQryRC, "incidents", false, null);
                    }

                    if (resultSetForReleaseCases != null && resultSetForReleaseCases.length > 0) {
                        showRelease = true;
                        break;
                    }
                }
            }
        }
    }

    return showRelease;
}

function btnHPG_Release_CustomAct(selectedRecordIds, selectedCntrl) {
    var releasedCasesCount = 0;

    if (resultSetForReleaseCases == null) {
        var fetchXMLExpQryRC = FetchXML_ReleaseCases(selectedRecordIds, checkForSMRole);

        if (fetchXMLExpQryRC != null) {
            resultSetForReleaseCases = FetchXML_GetRecords(fetchXMLExpQryRC, "incidents", false, null);
        }
    }

    if (resultSetForReleaseCases != null && resultSetForReleaseCases.length > 0) {
        var msgSubTitle = "";
        userLCID = GetContextUserLCID();

        if (resultSetForReleaseCases.length == 1) {
            switch (userLCID) {
                case 1025: //Arabic
                    msgSubTitle = PickSelectCasemMsgAR + selectedRecordIds.length + ReleaseOneCaseMsgAR;
                    break;
                case 1033: //English
                default:
                    msgSubTitle = PickSelectCasemMsgEN + selectedRecordIds.length + ReleaseOneCaseMsgEN;
                    break;
            }
        }
        else if (resultSetForReleaseCases.length > 1) {
            switch (userLCID) {
                case 1025: //Arabic
                    msgSubTitle = PickSelectCasemMsgAR + selectedRecordIds.length + ReleaseMultiCaseMsgAR;
                    break;
                case 1033: //English
                default:
                    msgSubTitle = PickSelectCasemMsgEN + selectedRecordIds.length + ReleaseMultiCaseMsgEN;
                    break;
            }
        }

        var confirmStrings = null;
        switch (userLCID) {
            case 1025: //Arabic
                confirmStrings = { title: titleReleaseCasemsgAR, text: msgSubTitle, confirmButtonLabel: confirmButtonLabelAR };
                break;
            case 1033: //English
            default:
                confirmStrings = { title: titleReleaseCasemsgEN, text: msgSubTitle, confirmButtonLabel: confirmButtonLabelEN };
                break;
        }

        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed) {
                    switch (userLCID) {
                        case 1025: //Arabic
                            ShowProgressIndicator(ProcessingReleaseCaseMsgAR);
                            break;
                        case 1033: //English
                        default:
                            ShowProgressIndicator(ProcessingReleaseCaseMsgEN);
                            break;
                    }

                    if (curentuserID == null) {
                        curentuserID = GetContextUserId();
                    }
                    for (var f = 0; f < resultSetForReleaseCases.length; f++) {
                        var targetRecordId = null;
                        targetRecordId = resultSetForReleaseCases[f].incidentid.replace("{", "").replace("}", "").toLowerCase();

                        if (targetRecordId != null) {
                            var usrReleasedBy = {}, actionQry = null, actionInputData = {};
                            usrReleasedBy.systemuserid = curentuserID;
                            actionInputData.ReleasedBy = usrReleasedBy;
                            actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_ReleaseCaseFunctionality";
                            var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, actionInputData, false, null);

                            if (actionOutput != null && actionOutput != undefined && actionOutput.IsCaseReleased) {
                                releasedCasesCount++;
                            }
                        }
                    }
                    CloseProgressIndicator();

                    if (releasedCasesCount > 0) {
                        selectedCntrl.refresh();
                    }
                }
                else {
                }
            });
    }
}

function btnHPG_Assign_EnableRule() {
    var showAssign = false;
    if (currentUserRoles == null) {
        currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();
    }

    var configParamVal = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);

    if (configParamVal != null) {
        var roleNames = null;

        if (configParamVal.indexOf(',') > -1) {
            roleNames = configParamVal.split(',');
        }
        else {
            roleNames = configParamVal;
        }

        if (roleNames != null) {
            for (var k = 0; k < currentUserRoles.length; k++) {
                if (roleNames.includes(currentUserRoles[k].name.toLowerCase())) {
                    showAssign = true;
                    break;
                }
            }
        }
    }

    return showAssign;
}

function btnHPG_Assign_CustomAct(selectedRecordIds, selectedCntrl) {
    var assignedCasesCount = 0, selectedUser = null;

    if (selectedRecordIds != null && selectedRecordIds.length > 0) {
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
                    var msgSubTitle = "";
                    userLCID = GetContextUserLCID();

                    if (selectedRecordIds.length == 1) {
                        switch (userLCID) {
                            case 1025: //Arabic
                                msgSubTitle = PickSelectCasemMsgAR + selectedRecordIds.length + AssignOneCaseMsgAR;
                                break;
                            case 1033: //English
                            default:
                                msgSubTitle = PickSelectCasemMsgEN + selectedRecordIds.length + AssignOneCaseMsgEN;
                                break;
                        }
                    }
                    else if (selectedRecordIds.length > 1) {
                        switch (userLCID) {
                            case 1025: //Arabic
                                msgSubTitle = PickSelectCasemMsgAR + selectedRecordIds.length + AssignMultiCaseMsgAR;
                                break;
                            case 1033: //English
                            default:
                                msgSubTitle = PickSelectCasemMsgEN + selectedRecordIds.length + AssignMultiCaseMsgEN;
                                break;
                        }
                    }

                    var confirmStrings = null;
                    switch (userLCID) {
                        case 1025: //Arabic
                            confirmStrings = { title: titleAssignCasemsgAR, text: msgSubTitle, confirmButtonLabel: AssignconfirmButtonLabelAR, cancelButtonLabel: AssigncancelButtonLabelAR };
                            break;
                        case 1033: //English
                        default:
                            confirmStrings = { title: titleAssignCasemsgEN, text: msgSubTitle, confirmButtonLabel: AssignconfirmButtonLabelEN, cancelButtonLabel: AssigncancelButtonLabelEN };
                            break;
                    }

                    var confirmOptions = { height: 200, width: 450 };

                    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
                        function (success) {
                            if (success.confirmed && selectedUser != null) {
                                switch (userLCID) {
                                    case 1025: //Arabic
                                        ShowProgressIndicator(ProcessingAssignCaseMsgAR);
                                        break;
                                    case 1033: //English
                                    default:
                                        ShowProgressIndicator(ProcessingAssignCaseMsgEN);
                                        break;
                                }

                                selectedUser = results[0];
                                if (curentuserID == null) {
                                    curentuserID = GetContextUserId();
                                }

                                for (var y = 0; y < selectedRecordIds.length; y++) {
                                    var targetRecordId = selectedRecordIds[y].replace("{", "").replace("}", "").toLowerCase();
                                    var usrAssignee = {}, usrAssignedBy = {}, actionQry = null, actionInputData = {};
                                    usrAssignee.systemuserid = selectedUser.id.replace("{", "").replace("}", "").toLowerCase(); // GUID of the lookup id;
                                    usrAssignedBy.systemuserid = curentuserID;

                                    actionInputData.Assignee = usrAssignee;
                                    actionInputData.AssignedBy = usrAssignedBy;
                                    actionQry = "incidents(" + targetRecordId + ")/Microsoft.Dynamics.CRM.tc_AssignCaseFunctionality";

                                    ShowProgressIndicator("Assign Case Processing....");
                                    var actionOutput = CallCustomActionInCRMUsgWebApi(actionQry, actionInputData, false, null);
                                    CloseProgressIndicator();

                                    if (actionOutput != null && actionOutput != undefined && actionOutput.IsCaseAssigned) {
                                        assignedCasesCount++;
                                    }
                                }
                                CloseProgressIndicator();

                                if (assignedCasesCount > 0) {
                                    selectedCntrl.refresh();
                                }
                            }
                        });
                }
            });
    }
}

//-------------- Get User Rolles ------------------- 
function RetrieveLoggedInD365UserSecurityRoles() {
    var resultSet = "", loggedInUserId = null;
    loggedInUserId = GetContextUserId();

    if (loggedInUserId != null) {
        var fetchXMLToRetrieveRoleNames = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "<entity name='role'>" +
            "<attribute name='roleid' />" +
            "<attribute name='name' />" +
            "<order attribute='name' descending='false' />" +
            "<link-entity name='systemuserroles' from='roleid' to='roleid' intersect='true'>" +
            "<link-entity name='systemuser' from='systemuserid' to='systemuserid' intersect='true'>" +
            "<filter>" +
            "<condition attribute='systemuserid' operator='eq' value='" + loggedInUserId + "' />" +
            "</filter>" +
            "</link-entity>" +
            "</link-entity>" +
            "</entity>" +
            "</fetch>";

        if (fetchXMLToRetrieveRoleNames != null) {
            resultSet = FetchXML_GetRecords(fetchXMLToRetrieveRoleNames, "roles", false, null);
        }
    }

    // returns the resultset that has the Teams where current logged in user is partof
    return resultSet;
}

function FetchXML_PickCases(selectedRecordIds) {
    var fetchXmlForPickCases = null;

    if (selectedRecordIds != null && selectedRecordIds != undefined && selectedRecordIds.length > 0) {
        var fetchXMLCondition = "";

        for (var j = 0; j < selectedRecordIds.length; j++) {
            fetchXMLCondition += "<condition attribute='incidentid' operator='eq' value='" + selectedRecordIds[j] + "'/>";
        }

        fetchXmlForPickCases = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "<entity name='incident'>" +
            "<attribute name='incidentid' />" +
            "<attribute name='tc_caseworkedby' />" +
            "<order attribute='createdon' descending='false' />" +
            "<filter type='and'>" +
            "<condition attribute='tc_caseworkedby' operator='null' />" +
            "<condition attribute='statecode' operator='eq' value='0' />" +
            "<filter type='or'>" + fetchXMLCondition +
            "</filter>" +
            "</filter>" +
            "</entity>" +
            "</fetch>";
    }

    return fetchXmlForPickCases;
}

function FetchXML_ReleaseCases(selectedRecordIds, checkForSMRole) {
    var fetchXmlForReleaseCases = null;
    var currentUserID = GetContextUserId();

    if (selectedRecordIds != null && selectedRecordIds != undefined && selectedRecordIds.length > 0 && currentUserID != null) {
        var fetchXMLCondition = "";

        for (var h = 0; h < selectedRecordIds.length; h++) {
            fetchXMLCondition += "<condition attribute='incidentid' operator='eq' value='" + selectedRecordIds[h] + "'/>";
        }

        fetchXmlForReleaseCases = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "<entity name='incident'>" +
            "<attribute name='incidentid' />" +
            "<attribute name='tc_caseworkedby' />" +
            "<order attribute='createdon' descending='false' />" +
            "<filter type='and'>";
        if (checkForSMRole) {
            fetchXmlForReleaseCases += "<condition attribute='tc_caseworkedby' operator='not-null' />";
        }
        else {
            fetchXmlForReleaseCases += "<condition attribute='tc_caseworkedby' operator='eq' value='" + currentUserID + "' />";
        }
        fetchXmlForReleaseCases += "<condition attribute='statecode' operator='eq' value='0' />" +
            "<filter type='or'>" + fetchXMLCondition +
            "</filter>" +
            "</filter>" +
            "</entity>" +
            "</fetch>";
    }

    return fetchXmlForReleaseCases;
}

//***************FetchXML to get the result set***********************//    
function FetchXML_GetRecords(originalFetch, entityname, isAsync, imperUserId) {
    var records = null;

    if (isAsync == null || isAsync == undefined) {
        isAsync = true;
    }
    var encodedFetch = encodeURI(originalFetch);
    var qryFetch = entityname + "?fetchXml=" + encodedFetch;
    var req = new XMLHttpRequest();
    req.open("GET", CRMOrgURL + "/api/data/v9.0/" + qryFetch, isAsync);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");

    if (imperUserId != null && imperUserId != "") {
        req.setRequestHeader("MSCRMCallerID", imperUserId);
    }

    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var results = JSON.parse(this.response);
                if (results != null) {
                    records = results.value;
                }
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();
    return records;
}

function GetContextUserId() {
    if (ContextUserDetails != null && ContextUserDetails != undefined) {
        return ContextUserDetails.userId.replace("{", "").replace("}", "").toLowerCase();
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetContextUserLCID() {
    var currrentUserLCID = null;

    if (ContextUserDetails != null && ContextUserDetails != undefined) {
        currrentUserLCID = ContextUserDetails.languageId;
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return currrentUserLCID;
}

function ShowProgressIndicator(prgMsg) {
    Xrm.Utility.showProgressIndicator(prgMsg);
}

function CloseProgressIndicator() {
    Xrm.Utility.closeProgressIndicator();
}

//***************Method to retrieve the Configuration Patameter Record***************//
function GetConfigParameterValue(configParamKey, isAsync, imperUserId) {
    var configParamVal = null;

    if (isAsync == null || isAsync == undefined) {
        isAsync = true;
    }

    if (configParamKey != null && configParamKey != undefined) {
        var req = new XMLHttpRequest();
        req.open("GET", CRMOrgURL + "/api/data/v9.0/cc_configurationparameterses?$select=cc_value&$filter=cc_key eq '" + configParamKey + "'", isAsync);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");

        if (imperUserId != null && imperUserId != "" && imperUserId != undefined) {
            req.setRequestHeader("MSCRMCallerID", imperUserId);
        }

        req.onreadystatechange = function () {
            if (this.readyState === 4 /* complete */) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var qryResults = JSON.parse(this.response);

                    if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
                        if (qryResults.value[0] != null && qryResults.value[0]["cc_value"] != null) {
                            configParamVal = qryResults.value[0]["cc_value"].toLowerCase();
                        }
                    }
                }
                else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
    }
    return configParamVal;
}

//***************Method to Call a Custom Action in CRM using Web API***************//
function CallCustomActionInCRMUsgWebApi(actionQuery, actionData, isAsync, imperUserId) {
    var resultOutput = null;

    if (isAsync == null || isAsync == undefined) {
        isAsync = true;
    }

    if (actionQuery != null && actionQuery != undefined && actionData != null && actionData != undefined) {
        var req = new XMLHttpRequest();
        req.open("POST", CRMOrgURL + "/api/data/v9.0/" + actionQuery, isAsync);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");

        if (imperUserId != null && imperUserId != "") {
            req.setRequestHeader("MSCRMCallerID", imperUserId);
        }

        req.onreadystatechange = function () {
            if (this.readyState === 4 /* complete */) {
                req.onreadystatechange = null;
                if (this.status >= 200 && this.status <= 300) {
                    if (this.response != null && this.response != undefined && this.response != "") {
                        resultOutput = JSON.parse(this.response);
                    }
                    else if (this.responseText != null && this.responseText != undefined && this.responseText != "") {
                        resultOutput = JSON.parse(this.responseText);
                    }
                }
                else {
                    var alertStrings = new Array(), alertOptions = new Array(), msgError = null;

                    if (this.statusText != null) {
                        msgError += this.statusText;
                    }
                    if (this.response != null) {
                        msgError += JSON.parse(this.response).error;
                    }

                    if (msgError != null) {
                        alertStrings = { confirmButtonLabel: "OK", text: "Error: " + msgError + " Please contact System Admin for further assistance.", title: "Call Custom Action" };
                        alertOptions = { height: 120, width: 260 };

                        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                            function (success) {
                            },
                            function (error) {
                                alert(error.message);
                            }
                        );
                    }
                }
            }
        };
        req.send(JSON.stringify(actionData));
    }
    return resultOutput;
}