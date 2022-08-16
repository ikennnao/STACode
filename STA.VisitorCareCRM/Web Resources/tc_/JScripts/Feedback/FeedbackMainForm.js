function form_OnLoad(executionContext) {
    SetAttrRequiredLevel(executionContext, "regardingobjectid", "required");

    var formType = null, minRating = null, maxRating = null;

    minRating = GetAttributeValue(executionContext, "minrating");
    maxRating = GetAttributeValue(executionContext, "maxrating");

    if (minRating == null) {
        SetAttributeValue(executionContext, "minrating", 1);
        SetAttrSubmitMode(executionContext, "minrating", "always");
    }
    if (maxRating == null) {
        SetAttributeValue(executionContext, "maxrating", 5);
        SetAttrSubmitMode(executionContext, "maxrating", "always");
    }

    formType = GetFormType(executionContext);
    if (formType != 1) {
        SetAttrsDisability(executionContext, "regardingobjectid, rating", ", ", true);
    }
    else {
        SetAttrsDisability(executionContext, "regardingobjectid, rating", ", ", false);
    }
}