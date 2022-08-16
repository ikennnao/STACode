var fieldsToDisable = "tc_name, tc_customer, tc_socialhandle, tc_socialuserid";

function form_OnLoad(executionContext) {
    if (GetFormType(executionContext) != 1) {
        SetAttrsDisability(executionContext, fieldsToDisable, ", ", true);
    }
    else if (GetFormType(executionContext) == 1) {
        SetAttrsDisability(executionContext, fieldsToDisable, ", ", false);
    }
}