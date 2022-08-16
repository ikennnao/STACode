function form_Onload(executionContext) {    
    taskType_OnChange(executionContext);
}

function taskType_OnChange(executionContext) {
    var taskSubject = "";
    var taskTypeVal = GetAttributeValue(executionContext, "tc_tasktype");
    var objRegardingName = GetLookupValName(executionContext, "regardingobjectid");

    if (taskTypeVal == 1) {
        SetAttrVisibility(executionContext, "tc_followupdate", true);
        SetAttrRequiredLevel(executionContext, "tc_followupdate", "required");
        taskSubject = "Followup Task for " + objRegardingName;
        SetAttributeValue(executionContext, "subject", taskSubject);
    }
    else {
        SetAttrRequiredLevel(executionContext, "tc_followupdate", "none");
        SetAttrVisibility(executionContext, "tc_followupdate", false);
        taskSubject = "Task for " + objRegardingName;
        SetAttributeValue(executionContext, "subject", taskSubject);
    }
}