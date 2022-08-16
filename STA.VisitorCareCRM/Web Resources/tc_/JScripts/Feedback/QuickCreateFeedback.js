function form_OnLoad(executionContext) {
    var minRating = null, maxRating = null;

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
}