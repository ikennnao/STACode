var CRMOrgURL = Xrm.Utility.getGlobalContext().getClientUrl();
var ContextUserDetails = Xrm.Utility.getGlobalContext().userSettings;
var resultSetForRevertToDraft = null, KACreateAmendKey = "KACreateAmendAccess";

function hideRevertToDraftBtn(selectedRecordIds) {
    var RevertToDraftBtn = true;

    if (selectedRecordIds != null && selectedRecordIds != undefined && selectedRecordIds.length > 0) {
        var fetchXMLExpQryPC = FetchXML_RevertToDraft(selectedRecordIds);

        if (fetchXMLExpQryPC != null) {
            resultSetForRevertToDraft = FetchXML_GetRecords(fetchXMLExpQryPC, "knowledgearticles", false, null);
        }

        if (resultSetForRevertToDraft != null && resultSetForRevertToDraft.length > 0) {
            RevertToDraftBtn = false;
        }
    }
    return RevertToDraftBtn;
}

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

function FetchXML_RevertToDraft(selectedRecordIds) {
    var fetchXmlForRevertToDraft = null;

    if (selectedRecordIds != null && selectedRecordIds != undefined && selectedRecordIds.length > 0) {
        var fetchXMLCondition = "";

        for (var j = 0; j < selectedRecordIds.length; j++) {
            fetchXMLCondition += "<condition attribute='knowledgearticleid' operator='eq' value='" + selectedRecordIds[j] + "'/>";
        }

        fetchXmlForRevertToDraft = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "<entity name='knowledgearticle'>" +
            "<attribute name='knowledgearticleid' />" +
            "<attribute name='statuscode' />" +
            "<order attribute='createdon' descending='false' />" +
            "<filter type='and'>" +
            "<condition attribute='statuscode' operator='eq' value='2' />" +
            "<filter type='or'>" + fetchXMLCondition +
            "</filter>" +
            "</filter>" +
            "</entity>" +
            "</fetch>";
    }
    return fetchXmlForRevertToDraft;
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
            }
        }
    }

    return checkKACARoles;
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