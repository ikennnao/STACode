function form_OnLoad(executionContext) {
    caseType_OnChange(executionContext, false);
}

function caseType_OnChange(executionContext, onChangeCheck) {
    var objCaseType = GetAttributeValue(executionContext, "tc_casetype");

    if (onChangeCheck) {
        if (GetAttributeValue(executionContext, "tc_casecategory") != null) {
            SetAttributeValue(executionContext, "tc_casecategory", null);
        }
        SetAttrDisability(executionContext, "tc_casecategory", false);

        if (GetAttributeValue(executionContext, "tc_casetype") == null) {
            SetAttrDisability(executionContext, "tc_casecategory", true);
        }
    }
    else {
        if (objCaseType != null) {
            SetAttrDisability(executionContext, "tc_casecategory", false);
        }
        else {
            if (GetAttributeValue(executionContext, "tc_casecategory") != null) {
                SetAttributeValue(executionContext, "tc_casecategory", null);
            }
            SetAttrDisability(executionContext, "tc_casecategory", true);
        }
    }

    caseCategory_OnChange(executionContext, onChangeCheck);
}

function caseCategory_OnChange(executionContext, onChangeCheck) {
    var objCategory = GetAttributeValue(executionContext, "tc_casecategory");

    if (onChangeCheck) {
        if (GetAttributeValue(executionContext, "tc_casesubcategory") != null) {
            SetAttributeValue(executionContext, "tc_casesubcategory", null);
        }
        SetAttrDisability(executionContext, "tc_casesubcategory", false);

        if (GetAttributeValue(executionContext, "tc_casecategory") == null) {
            SetAttrDisability(executionContext, "tc_casesubcategory", true);
        }
    }
    else {
        if (objCategory != null) {
            SetAttrDisability(executionContext, "tc_casesubcategory", false);
        }
        else {
            if (GetAttributeValue(executionContext, "tc_casesubcategory") != null) {
                SetAttributeValue(executionContext, "tc_casesubcategory", null);
            }
            SetAttrDisability(executionContext, "tc_casesubcategory", true);
        }
    }
}