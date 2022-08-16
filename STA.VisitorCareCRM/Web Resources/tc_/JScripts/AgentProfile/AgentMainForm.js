function logAgentProfileStatusChange() {
    var entityName = "tc_agentprofilelogs"; var recordId;
    var userId = Xrm.Page.context.userSettings.userId;
    userId = userId.replace("{", "").replace("}", "");
    var value = $("#availability option:selected").text();

    var filter = `?$select=tc_agentprofileid,tc_name&$filter=_tc_agentname_value eq ${userId}`

    var record = _RetrieveMultipleRecords("tc_agentprofiles", filter);
    if (record != null && record.value.length > 0) {

        recordId = record.value[0]["tc_agentprofileid"];
    }
    var imperUserId = GetConfigParameterValue("AdminUserGuid", false);
    var entity = {};
    entity["tc_AgentProfile@odata.bind"] = "/tc_agentprofiles(" + recordId + ")";
    entity["tc_ChangedBy@odata.bind"] = "/systemusers(" + userId + ")";
    entity.tc_name = `Availability Status Change Log-${new Date()}`;
    entity.tc_changeddate = new Date().toISOString();
    entity.tc_newvalue = value;
    entity.tc_oldvalue = previousState;
    entity.tc_operationtype = 1;
    var id = CreateRecordInCRMUsgWebApi(entity, entityName, false, imperUserId)
}

function CheckFormIsDirty(executionContext, nameOfFieldToCheck) {
    attributes = Xrm.Page.data.entity.attributes.get();
    var entityName = "tc_agentprofilelogs";
    var userId = Xrm.Page.context.userSettings.userId;
    userId = userId.replace("{", "").replace("}", "");

    var formContext = executionContext.getFormContext();
    if (attributes != null) {
        for (var i in attributes) {
            if (attributes[i].getIsDirty() && attributes[i].getName() == nameOfFieldToCheck) {

            }
        }
    }
}