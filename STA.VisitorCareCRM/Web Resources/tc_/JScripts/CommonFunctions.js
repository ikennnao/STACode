var CRMOrgURL = Xrm.Utility.getGlobalContext().getClientUrl();
var CRMAppURL = Xrm.Utility.getGlobalContext().getCurrentAppUrl();
var ContextUserDetails = Xrm.Utility.getGlobalContext().userSettings;
var IsTargetUserSystemAdmin = IsLoggedInUserSystemAdmin();

function CheckForExecContext(executionContext) {
    var rqdContext = null;
    if (executionContext != null && executionContext != undefined) {
        if (executionContext.getFormContext != null && executionContext.getFormContext != undefined) {
            rqdContext = executionContext.getFormContext();
        }
        else {
            rqdContext = executionContext;
        }
    }
    return rqdContext;
}

function IsLoggedInUserSystemAdmin() {
    var UserDetails = RetrieveLoggedInD365UserSecurityRoles();
    var IsSystemAdmin = false;
    if (UserDetails != null && UserDetails.length > 0) {
        var objUserRoles = UserDetails;
        for (var i = 0; i < objUserRoles.length; i++) {
            if (objUserRoles[i] != null && objUserRoles[i].name != null && objUserRoles[i].name.toLowerCase() == "system administrator") {
                IsSystemAdmin = true;
            }
        }
    }
    return IsSystemAdmin;
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

//-------------- Get LoggedInUser Associated Teams ------------------- 
function RetrieveLoggedInD365UserTeams() {
    var resultSet = "", loggedInUserId = null;
    loggedInUserId = GetContextUserId();

    if (loggedInUserId != null) {
        var fetchXMLToRetrieveTeamNames = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "<entity name='team'>" +
            "<attribute name='teamid' />" +
            "<attribute name='name' />" +
            "<order attribute='name' descending='false' />" +
            "<link-entity name='teammembership' from='teamid' to='teamid' intersect='true'>" +
            "<link-entity name='systemuser' from='systemuserid' to='systemuserid' intersect='true'>" +
            "<filter>" +
            "<condition attribute='systemuserid' operator='eq' value='" + loggedInUserId + "' />" +
            "</filter>" +
            "</link-entity>" +
            "</link-entity>" +
            "</entity>" +
            "</fetch>";

        if (fetchXMLToRetrieveTeamNames != null) {
            resultSet = FetchXML_GetRecords(fetchXMLToRetrieveTeamNames, "teams", false, null);
        }
    }

    // returns the resultset that has the Teams where current logged in user is partof
    return resultSet;
}

function AddCustomFilterToAttr(executionContext, attrName, custFilter, entityName) {
    if (executionContext != null && executionContext != undefined && attrName != null && attrName != undefined
        && custFilter != null && custFilter != undefined && entityName != null && entityName != undefined) {
        var attrCtrl = GetControl(executionContext, attrName);
        if (attrCtrl != null) {
            attrCtrl.addCustomFilter(custFilter, entityName);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function RefreshFormRibbon(executionContext, refreshAll) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        return formContext.ui.refreshRibbon(refreshAll);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function RefreshFormData(executionContext, refresh) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        return formContext.data.refresh(refresh);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SplitStringData(strgValue, splitChar) {
    var strngArray = new Array();
    if (strgValue != null && strgValue.length > 0) {
        if (splitChar != null && splitChar != undefined) {
            strngArray = strgValue.split(splitChar);
        }
        else {
            strngArray = strgValue.split(", ");
        }
    }
    return strngArray;
}

function GetFormEventArgs(executionContext) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        return formContext.getEventArgs();
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetBPFEventArgs(bpfStageContext) {
    if (bpfStageContext != null && bpfStageContext != undefined) {
        return bpfStageContext.getEventArgs();
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetAttribute(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (attrName != null && attrName != undefined) {
            return formContext.getAttribute(attrName);
        }
        else {
            return null;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetAttrDisability(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        var attrCtrl = null;
        if (attrName != null && attrName != undefined) {
            attrCtrl = formContext.getControl(attrName);
            if (attrCtrl != null) {
                return attrCtrl.getDisabled();
            }
            else {
                return attrCtrl;
            }
        }
        else {
            return attrCtrl;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetAttributeValue(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = null;
        attr = formContext.getAttribute(attrName);
        if (attr != null) {
            return attr.getValue();
        }
        else {
            return null;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetControl(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (attrName != null && attrName != undefined) {
            return formContext.getControl(attrName);
        }
        else {
            return null;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetAllCtrlsOnForm(executionContext) {
    var formContext = CheckForExecContext(executionContext);
    var formAllCtrlNames = new Array();

    if (formContext != null) {
        var ctrlsLength = formContext.getControl().length;

        formContext.getControl().forEach(function (control, controlIndex) {
            var ctrlName = control.name;

            if (ctrlName != null) {
                if (controlIndex == 0) {
                    formAllAttrNames = ctrlAttrName + ", ";
                }
                else if (controlIndex != ctrlsLength - 1) {
                    formAllCtrlNames += ctrlAttrName + ", ";
                }
                else if (controlIndex == ctrlsLength - 1) {
                    formAllCtrlNames += ctrlAttrName;
                }
            }
        });
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }

    return formAllCtrlNames;
}

function GetAllAttrCtrlsOnForm(executionContext) {
    var formContext = CheckForExecContext(executionContext);
    var formAllAttrNames = null;

    if (formContext != null) {
        var ctrlsLength = formContext.getControl().length;
        formContext.getControl().forEach(function (control, controlIndex) {
            if (control.getControlType() != "subgrid" && control.getControlType() != "iframe" && control.getControlType() != "webresource" &&
                control.getControlType() != "notes" && control.getControlType() != "quickform" && control.getControlType() != "kbsearch" &&
                control.getControlType() != "timercontrol" && control.getControlType() != "timelinewall") {
                var ctrlAttrName = control.name;

                if (ctrlAttrName != null && ctrlAttrName != "header_process") {
                    if (controlIndex == 0) {
                        formAllAttrNames = ctrlAttrName + ", ";
                    }
                    else if (controlIndex != ctrlsLength - 1) {
                        formAllAttrNames += ctrlAttrName + ", ";
                    }
                    else if (controlIndex == ctrlsLength - 1) {
                        formAllAttrNames += ctrlAttrName;
                    }
                }
            }
        });
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }

    return formAllAttrNames;
}

function GetAttrLabel(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        var attrCtrl = null;
        if (attrName != null && attrName != undefined) {
            attrCtrl = formContext.getControl(attrName);
            if (attrCtrl != null) {
                return attrCtrl.getLabel().toLowerCase();
            }
            else {
                return attrCtrl;
            }
        }
        else {
            return attrCtrl;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetCtrlAttrValue(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = null;
        attr = formContext.getControl(attrName);
        if (attr != null) {
            return attr.getAttribute().getValue();
        }
        else {
            return null;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetCtrlAttribute(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = null;
        attr = formContext.getControl(attrName);
        if (attr != null) {
            return attr.getAttribute();
        }
        else {
            return null;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetLookupValId(executionContext, attrName) {
    var lookupGuid = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attrCtrl = null;
        attrCtrl = formContext.getAttribute(attrName);
        if (attrCtrl != null) {
            var attrValue = null;
            if (attrCtrl.getValue() != null && attrCtrl.getValue()[0] != null && attrCtrl.getValue()[0].id != null) {
                if (attrCtrl.getValue()[0].id.indexOf("{") > -1 && attrCtrl.getValue()[0].id.indexOf("}") > -1) {
                    attrValue = attrCtrl.getValue()[0].id.replace('{', '').replace('}', '');
                }
                else {
                    attrValue = attrCtrl.getValue()[0].id;
                }
            }
            lookupGuid = attrValue;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }

    return lookupGuid;
}

function GetLookupValName(executionContext, attrName) {
    var lookupName = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attrCtrl = null;
        attrCtrl = formContext.getAttribute(attrName);
        if (attrCtrl != null) {
            var attrValue = null;
            if (attrCtrl.getValue() != null && attrCtrl.getValue()[0] != null && attrCtrl.getValue()[0].name != null) {
                attrValue = attrCtrl.getValue()[0].name.toLowerCase();
            }
            lookupName = attrValue;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return lookupName;
}

function GetLookupEntityType(executionContext, attrName) {
    var entityTypeName = null;
    var formContext = CheckForExecContext(executionContext);

    if (formContext != null && attrName != null && attrName != undefined) {
        var attrCtrl = null;
        attrCtrl = formContext.getAttribute(attrName);
        if (attrCtrl != null) {
            var attrEntityType = null;
            if (attrCtrl.getValue() != null && attrCtrl.getValue()[0] != null && attrCtrl.getValue()[0].entityType != null) {
                attrEntityType = attrCtrl.getValue()[0].entityType.toLowerCase();
            }
            entityTypeName = attrEntityType;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }

    return entityTypeName;
}

function GetFormType(executionContext) {
    var targetFormType = null;

    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.ui != null && formContext.ui.getFormType() != null) {
            targetFormType = formContext.ui.getFormType();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }

    return targetFormType;
}

function GetFormName(executionContext) {
    var targetFormName = null;

    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.ui != null && formContext.ui.formSelector != null && formContext.ui.formSelector.getCurrentItem() != null && formContext.ui.formSelector.getCurrentItem().getLabel() != null) {
            targetFormName = formContext.ui.formSelector.getCurrentItem().getLabel().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return targetFormName;
}

function GetBPFStatusName(executionContext) {
    var txtBPFStatusName = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.data != null && formContext.data.process != null && formContext.data.process.getStatus() != null) {
            txtBPFStatusName = formContext.data.process.getStatus().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return txtBPFStatusName;
}

function GetBPFActiveStageName(executionContext) {
    var txtBPFStageName = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.data != null && formContext.data.process != null
            && formContext.data.process.getActiveStage() != null
            && formContext.data.process.getActiveStage().getName() != null) {
            txtBPFStageName = formContext.data.process.getActiveStage().getName().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return txtBPFStageName;
}

function GetBPFActiveStageId(executionContext) {
    var txtBPFStageId = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.data != null && formContext.data.process != null
            && formContext.data.process.getActiveStage() != null
            && formContext.data.process.getActiveStage().getId() != null) {
            txtBPFStageId = formContext.data.process.getActiveStage().getId().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return txtBPFStageId;
}

function GetBPFAllStages(executionContext) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.data != null && formContext.data.process != null
            && formContext.data.process.getActiveProcess() != null
            && formContext.data.process.getActiveProcess().getStages() != null
            && formContext.data.process.getActiveProcess().getStages().getAll() != null) {
            return formContext.data.process.getActiveProcess().getStages().getAll();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function GetBPFStageDirection(bpfStageContext) {
    var txtBPFDirection = null;
    var eventArgs = GetBPFEventArgs(bpfStageContext);
    if (eventArgs != null) {
        if (eventArgs.getDirection() != null) {
            txtBPFDirection = eventArgs.getDirection().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return txtBPFDirection;
}

function GetOptionSetValText(executionContext, attrName) {
    var optionsetText = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var optsetAttr = GetAttribute(formContext, attrName);
        if (optsetAttr != null) {
            optionsetText = optsetAttr.getText().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return optionsetText;
}

function GetTargetRecordId(executionContext) {
    var targetRecordId = null;
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && formContext.data.entity.getId() != null && formContext.data.entity.getId() != "") {
        targetRecordId = formContext.data.entity.getId();
        if (targetRecordId.indexOf("{") > -1 && targetRecordId.indexOf("}") > -1) {
            targetRecordId = targetRecordId.replace("{", "").replace("}", "");
        }
    }
    return targetRecordId;
}

function GetContextUserId() {
    var currrentUserID = null;

    if (ContextUserDetails != null && ContextUserDetails != undefined) {
        currrentUserID = ContextUserDetails.userId;

        if (currrentUserID.indexOf("{") > -1 && currrentUserID.indexOf("}") > -1) {
            currrentUserID = currrentUserID.replace("{", "").replace("}", "");
        }

        currrentUserID = currrentUserID.toLowerCase();
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return currrentUserID;
}

function GetContextUserName() {
    if (ContextUserDetails != null && ContextUserDetails != undefined) {
        return ContextUserDetails.userName.toLowerCase();
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

function getArabicLookups(executionContext, webapiEntityName, attrName, arabicAttrName) {
    var currUserLcid = GetContextUserLCID();

    if (currUserLcid == "1025" && attrName != null && attrName != undefined && attrName != "") { // If Context Language is Arabic
        var objLookupId = GetLookupValId(executionContext, attrName);
        var objLookupEntityName = GetLookupEntityType(executionContext, attrName);

        if (objLookupId != null && objLookupEntityName != null) {
            if (webapiEntityName != null && webapiEntityName != undefined && webapiEntityName != "" && arabicAttrName != null && arabicAttrName != undefined && arabicAttrName != "") {
                var arabicName = null;
                var qryExp = webapiEntityName + "(" + objLookupId + ")?$select=" + arabicAttrName;
                var qryResults = RetrieveMultipleRecords(qryExp, false, null);

                if (qryResults != null) {
                    switch (arabicAttrName) {
                        case "tc_arabicname":
                            arabicName = qryResults.tc_arabicname;
                            break;
                        case "tc_facilitynamearabic":
                            arabicName = qryResults.tc_facilitynamearabic;
                            break;
                        case "cc_arabicname":
                            arabicName = qryResults.cc_arabicname;
                            break;
                    }
                }

                if (arabicName != null && arabicName != "" && arabicName != undefined) {
                    var lookupProps = new Array();
                    lookupProps[0] = new Object();
                    lookupProps[0].id = objLookupId;
                    lookupProps[0].name = arabicName;
                    lookupProps[0].entityType = objLookupEntityName;

                    SetAttributeValue(executionContext, attrName, lookupProps);
                }
            }
        }
    }
}

function SetAttributeValue(executionContext, attrName, attrValue) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = GetAttribute(formContext, attrName);
        if (attr != null) {
            attr.setValue(attrValue);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetCtrlAttrValue(executionContext, attrName, attrValue) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = null;
        attr = formContext.getControl(attrName);
        if (attr != null) {
            return attr.getAttribute().setValue(attrValue);
        }
        else {
            return null;
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetCtrlAttrSubmitMode(executionContext, attrName, attrValue) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = GetCtrlAttribute(formContext, attrName);
        if (attr != null) {
            attr.setSubmitMode(attrValue);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrSubmitMode(executionContext, attrName, attrValue) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attr = GetAttribute(formContext, attrName);
        if (attr != null) {
            attr.setSubmitMode(attrValue);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrRequiredLevel(executionContext, attrName, rqdLevel) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined && rqdLevel != null && rqdLevel != undefined) {
        var attr = GetAttribute(formContext, attrName);
        if (attr != null) {
            attr.setRequiredLevel(rqdLevel);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrsRequiredLevel(executionContext, attrNames, splitChar, rqdLevel) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrNames != null && attrNames != undefined
        && rqdLevel != null && rqdLevel != undefined) {
        var attrsArray = SplitStringData(attrNames, splitChar);

        if (attrsArray != null && attrsArray.length > 0) {
            for (var i = 0; i < attrsArray.length; i++) {
                var attr = GetAttribute(formContext, attrsArray[i]);
                if (attr != null) {
                    attr.setRequiredLevel(rqdLevel);
                }
            }
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrsDisability(executionContext, attrNames, splitChar, disabilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrNames != null && attrNames != undefined
        && disabilityVal != null && disabilityVal != undefined) {
        var attrsArray = SplitStringData(attrNames, splitChar);

        if (attrsArray != null && attrsArray.length > 0) {
            for (var i = 0; i < attrsArray.length; i++) {
                var attrCtrl = GetControl(formContext, attrsArray[i]);
                if (attrCtrl != null) {
                    attrCtrl.setDisabled(disabilityVal);
                }
            }
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrsVisibility(executionContext, attrNames, splitChar, visibilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrNames != null && attrNames != undefined
        && visibilityVal != null && visibilityVal != undefined) {
        var attrsArray = SplitStringData(attrNames, splitChar);

        if (attrsArray != null && attrsArray.length > 0) {
            for (var i = 0; i < attrsArray.length; i++) {
                var attrCtrl = GetControl(formContext, attrsArray[i]);
                if (attrCtrl != null) {
                    attrCtrl.setVisible(visibilityVal);
                }
            }
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrDisability(executionContext, attrName, disabilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined
        && disabilityVal != null && disabilityVal != undefined) {
        var attrCtrl = GetControl(formContext, attrName);
        if (attrCtrl != null) {
            attrCtrl.setDisabled(disabilityVal);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrVisibility(executionContext, attrName, visibilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        var attrCtrl = GetControl(formContext, attrName);
        if (attrCtrl != null) {
            attrCtrl.setVisible(visibilityVal);
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetTabVisibility(executionContext, tabName, visibilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && tabName != null && tabName != undefined && visibilityVal != null && visibilityVal != undefined) {
        var tabCtrl = null;
        tabCtrl = formContext.ui.tabs.get(tabName);
        if (tabCtrl != null) {
            tabCtrl.setVisible(visibilityVal);
        }
        else {
            alert("Specified Tab '" + tabName + "' is not available. Please contact System Admin for further assistance.");
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetSectionVisibility(executionContext, tabName, sectName, visibilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && tabName != null && tabName != undefined &&
        sectName != null && sectName != undefined && visibilityVal != null && visibilityVal != undefined) {
        var tabCtrl = null;
        tabCtrl = formContext.ui.tabs.get(tabName);
        if (tabCtrl != null) {
            var sectCtrl = null;
            sectCtrl = tabCtrl.sections.get(sectName);
            if (sectCtrl != null) {
                sectCtrl.setVisible(visibilityVal);
            }
            else {
                alert("Specified Section '" + sectName + "' is not available. Please contact System Admin for further assistance.");
            }
        }
        else {
            alert("Specified Tab '" + tabName + "' is not available. Please contact System Admin for further assistance.");
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetBPFVisibility(executionContext, visibilityVal) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && visibilityVal != null && visibilityVal != undefined) {
        formContext.ui.process.setVisible(visibilityVal);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetBPFActiveStage(executionContext, bpfStageId) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && bpfStageId != null && bpfStageId != undefined) {
        formContext.data.process.setActiveStage(bpfStageId);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetFieldFocus(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null) {
        var attrCtrl = GetControl(formContext, attrName);
        if (attrCtrl != null) {
            attrCtrl.setFocus();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetFormNotification(executionContext, msgNotification, msgLevel, msgUniqueId) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && msgNotification != null && msgNotification != undefined
        && msgLevel != null && msgLevel != undefined && msgUniqueId != null && msgUniqueId != undefined) {
        formContext.ui.setFormNotification(msgNotification, msgLevel, msgUniqueId);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function ClearFormNotification(executionContext, msgUniqueId) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && msgUniqueId != null && msgUniqueId != undefined) {
        formContext.ui.clearFormNotification(msgUniqueId);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function SetAttrCtrlNotification(executionContext, attrName, msgNotification) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined
        && msgNotification != null && msgNotification != undefined) {
        formContext.getControl(attrName).setNotification(msgNotification);
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function ClearAttrCtrlNotification(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null && attrName != null && attrName != undefined) {
        formContext.getControl(attrName).clearNotification();
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function AttrFireOnChange(executionContext, attrName) {
    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        var attr = GetAttribute(formContext, attrName);
        if (attr != null && attr != undefined) {
            attr.fireOnChange();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
}

function ShowProgressIndicator(prgMsg) {
    Xrm.Utility.showProgressIndicator(prgMsg);
}

function CloseProgressIndicator() {
    Xrm.Utility.closeProgressIndicator();
}

function DisableAllAttrCtrls(executionContext) {
    var formAttrNames = GetAllAttrCtrlsOnForm(executionContext);

    if (formAttrNames != null) {
        SetAttrsDisability(executionContext, formAttrNames, ", ", true);
    }
}

//***************Method to retrieve the Anonymous Customer Record***************//
function RetrieveAnonymousCustomer() {
    var custFirstName = "Anonymous", custIDVal = null;
    var qrySelect = "contacts?$select=contactid&$filter=firstname eq '" + custFirstName + "'";
    var qryResults = RetrieveMultipleRecords(qrySelect, false, null);

    if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
        custIDVal = qryResults.value[0]["contactid"];

        if (custIDVal != null && custIDVal.indexOf("{") > -1 && custIDVal.indexOf("}") > -1) {
            custIDVal = custIDVal.replace("{", "").replace("}", "");
        }
    }
    return custIDVal;
}

//***************Method to retrieve the multiple records***************//
function RetrieveMultipleRecords(selectQuery, isAsync, imperUserId) {
    var qryResults = null;

    if (isAsync == null || isAsync == undefined) {
        isAsync = true;
    }

    if (selectQuery != null && selectQuery != undefined) {
        var req = new XMLHttpRequest();
        req.open("GET", CRMOrgURL + "/api/data/v9.0/" + selectQuery, isAsync);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");

        if (imperUserId != null && imperUserId != "") {
            req.setRequestHeader("MSCRMCallerID", imperUserId);
        }

        req.onreadystatechange = function () {
            if (this.readyState === 4 /* complete */) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    qryResults = JSON.parse(this.response);
                }
                else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
    }
    return qryResults;
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

//***************Method to Create Record in CRM using Web Api***************//
function CreateRecordInCRMUsgWebApi(entityObject, entityName, isAsync, imperUserId) {
    var entCreatedId = null;

    if (isAsync == null || isAsync == undefined) {
        isAsync = true;
    }

    if (entityObject != null && entityObject != undefined && entityName != null && entityName != undefined) {
        var req = new XMLHttpRequest();
        req.open("POST", CRMOrgURL + "/api/data/v9.0/" + entityName, isAsync);
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
                if (this.status === 204) {
                    var Uri = this.getResponseHeader("OData-EntityId");
                    var regExp = /\(([^)]+)\)/;
                    var matches = regExp.exec(Uri);
                    entCreatedId = matches[1];
                    if (entCreatedId != null && entCreatedId.indexOf("{") > -1 && entCreatedId.indexOf("}") > -1) {
                        entCreatedId = entCreatedId.replace("{", "").substring(1, 37); //get only GUID
                    }
                }
                else {
                    //Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(entityObject));
    }

    return entCreatedId;
}

function UpdateRecordInCRMUsgWebApi(entityObject, entityWebApiName, recordID, isAsync, imperUserId, entityFormOptions) {
    var entUpdatedId = null;

    if (isAsync == null || isAsync == undefined) {
        isAsync = true;
    }

    if (entityObject != null && entityObject != undefined && entityWebApiName != null && entityWebApiName != undefined) {
        var req = new XMLHttpRequest();
        req.open("PATCH", CRMOrgURL + "/api/data/v9.0/" + entityWebApiName + "(" + recordID + ")", isAsync);
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
                if (this.status === 204) {
                    var Uri = this.getResponseHeader("OData-EntityId");
                    var regExp = /\(([^)]+)\)/;
                    var matches = regExp.exec(Uri);
                    entUpdatedId = matches[1];
                    if (entUpdatedId != null && entUpdatedId.indexOf("{") > -1 && entUpdatedId.indexOf("}") > -1) {
                        entUpdatedId = entUpdatedId.replace("{", "").substring(1, 37); //get only GUID
                    }

                    if (entityFormOptions != null && entityFormOptions != undefined) {
                        // Reload the Target Record.
                        Xrm.Navigation.openForm(entityFormOptions);
                    }
                }
                else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(entityObject));
    }

    return entUpdatedId;
}

function CheckDateWithCurrentDt(attrDtValue) {
    var attrDate = null, attrMonth = null, attrYear = null, dtValidation = false;
    var currentDate = null, currentMonth = null, currentYear = null;

    attrDate = attrDtValue.getDate();
    attrMonth = attrDtValue.getMonth();
    attrYear = attrDtValue.getFullYear();

    currentDate = new Date().getDate();
    currentMonth = new Date().getMonth();
    currentYear = new Date().getFullYear();

    if (attrYear < currentYear) {
        dtValidation = true;
    }
    else if (attrYear <= currentYear && attrMonth < currentMonth) {
        dtValidation = true;
    }
    else if (attrYear <= currentYear && attrMonth <= currentMonth && attrDate < currentDate) {
        dtValidation = true;
    }

    return dtValidation;
}

function CheckConditionsForContactNo(contactNoVal) {
    var aryContactValid = null;

    if (contactNoVal != null && contactNoVal.length > 0) {
        aryContactValid = new Object();

        if (contactNoVal.length >= 6) {
            var checkForNumericChars = /[0-9]/g, contactNoWithOnlyNumericChars = "";

            for (var i = 0; i < contactNoVal.length; i++) {
                var charN = contactNoVal[i];
                contactNoWithOnlyNumericChars += charN.match(checkForNumericChars) != null ? charN.match(checkForNumericChars) : "";
            }

            if (contactNoWithOnlyNumericChars != null) {
                if (contactNoWithOnlyNumericChars.length >= 6) {
                    if (contactNoWithOnlyNumericChars.indexOf("+") != 0) {
                        aryContactValid.contactNoEndVal = "+" + contactNoWithOnlyNumericChars;
                    }
                    else {
                        aryContactValid.contactNoEndVal = contactNoWithOnlyNumericChars;
                    }

                    aryContactValid.validationStatus = true;
                    aryContactValid.validationMsg = null;
                }
                else {
                    aryContactValid.contactNoEndVal = null;
                    aryContactValid.validationStatus = false;
                    aryContactValid.validationMsg = "Contact No. should have minimum of 6 digits with Country Code (Example: +966123456789)";
                }
            }
            else {
                aryContactValid.contactNoEndVal = null;
                aryContactValid.validationStatus = false;
                aryContactValid.validationMsg = "Contact No. is 'Null' (or) 'Empty'.";
            }
        }
        else {
            aryContactValid.contactNoEndVal = null;
            aryContactValid.validationStatus = false;
            aryContactValid.validationMsg = "Contact No. should have minimum of 6 digits with Country Code (Example: +966123456789)";
        }
    }
    return aryContactValid;
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

    if (actionQuery != null && actionQuery != undefined) {
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

        if (actionData != null && actionData != undefined && actionData != "") {
            req.send(JSON.stringify(actionData));
        }
        else {
            req.send();
        }
    }

    return resultOutput;
}

//***************Method to retrieve a Record Using WebApi***************//
function RetrieveRecord(entityname, recordId, filter) {
    var configParamVal = null;
    Xrm.WebApi.online.retrieveRecord(entityname, recordId, filter).then(
        function success(result) {
            if (result != null) {
                configParamVal = result;
            }
        },
        function (error) {
            Xrm.Utility.alertDialog(error.message);
        }
    );

    return configParamVal;
}

//***************Method to retrieve Multiple records Using Jquery***************//
function _RetrieveMultipleRecords(entityname, filter) {
    var configParamVal = null;
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: `${Xrm.Page.context.getClientUrl()}/api/data/v9.0/${entityname}${filter}`,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: false,
        success: function (data, textStatus, xhr) {
            configParamVal = data;
        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });
    return configParamVal;
}