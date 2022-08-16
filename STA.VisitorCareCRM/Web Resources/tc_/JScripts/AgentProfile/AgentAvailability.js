var Xrm = parent.window.Xrm; var previousState;

$(document).ready(function () {
    $('#myModal').modal('show');
    getUserAvailability();
    $("#availability").trigger("focusout");
});

$("#availability").on("focusout", function ($this) {
    var option = $("#availability").val();
    if (option == 2) {
        $("#availability").addClass("is-invalid");
        $("#availability").removeClass("is-valid");
    }
    else {
        $("#availability").removeClass("is-invalid");
        $("#availability").addClass("is-valid");
    }

    $("#savechanges").unbind().click(function (e) {
        //if(confirm("change status?"))
        saveUserAvailability();
    })
});

$("select").on('focus', function () {
    // Store the current value on focus and on change
    previousState = this.selectedOptions[0].innerText
});

function saveUserAvailability() {
    var entityName = "tc_agentprofiles"; var recordId;
    var userId = Xrm.Page.context.userSettings.userId;
    userId = userId.replace("{", "").replace("}", "");
    var value = parseInt($("#availability").val());
    var entity = {
        tc_agentavailability: value
    };
    var filter = `?$select=tc_agentprofileid,tc_name&$filter=_tc_agentname_value eq ${userId}`

    var record = _RetrieveMultipleRecords(entityName, filter);
    if (record != null && record.value.length > 0) {

        recordId = record.value[0]["tc_agentprofileid"];
    }
    var imperUserId = GetConfigParameterValue("AdminUserGuid", false);

    UpdateRecordInCRMUsgWebApi(entity, entityName, recordId, false, imperUserId, null);
    showGlobalNotification();
    logAgentProfileStatusChange();
}

function showGlobalNotification() {
    var message = null; 
    var currUserLcid = Xrm.Page.context.getUserLcid();
    if (currUserLcid == 1025) {
        //mainmsg
        message = "<h5>تم تغيير الحالة</h5>";
    }
    else {
        message = "<h5>Status has been Changed</h5>";
    }
    $(".toast-body").html(message);
    $('.toast').toast('show')
}

function getUserAvailability() {
    var entityName = "tc_agentprofiles";
    var recId = Xrm.Page.context.userSettings.userId;
    recId = recId.replace("{", "").replace("}", "");
    var filter = `?$select=tc_agentavailability,_tc_agentname_value,tc_agentprofileid,tc_name&$filter=_tc_agentname_value eq ${recId} and  statecode eq 0`

    var record = _RetrieveMultipleRecords(entityName, filter);
    if (record != null && record.value.length > 0) {
        //show the UI 
        $(".container").css("display", "block");
        $("#availability").val(record.value[0]["tc_agentavailability"]);
    }
    else {
        $(".notification").css("display", "block");
    }    
}

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