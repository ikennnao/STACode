var formExecuteContext = null, IsPriorityCare = false, IsCustomerAnonymous = false;
var fieldsForIsECNull = "tc_existingcase, tc_casetype, tc_category, tc_subcategory, tc_priority";
var requiredFieldsForIsECNo = "tc_casetype, tc_category, tc_subcategory";

function form_OnLoad(executionContext) {
    formExecuteContext = executionContext;
    HideShowFormSections(executionContext);
    getCustomerDetails(executionContext);
    isCaseExisting_OnChange(executionContext, false);
    ChangeLookupLanguages(executionContext);
}

function HideShowFormSections(executionContext) {
    var isCOVal = GetLookupValName(executionContext, "tc_channelorigin");
    if (isCOVal != null) {
        isCOVal = isCOVal.toLowerCase();
    }

    switch (isCOVal) {
        case "email": //Email
            SetSectionVisibility(executionContext, "tab_general", "sec_generaldetails", false);
            SetSectionVisibility(executionContext, "tab_general", "sec_emaildetails", true);
            break;
        default: //Null or Anything
            SetSectionVisibility(executionContext, "tab_general", "sec_emaildetails", false);
            SetSectionVisibility(executionContext, "tab_general", "sec_generaldetails", true);
            break;
    }
}

function getCustomerDetails(executionContext) {
    var custEntityType = GetLookupEntityType(executionContext, "tc_customer");
    if (custEntityType == "contact") {
        var customerIdVal = GetLookupValId(executionContext, "tc_customer");
        var qryselectCustomer = "contacts?$select=customertypecode,tc_prioritycare&$filter=contactid eq " + customerIdVal + " and  statecode eq 0";
        var qryResultsCustomer = RetrieveMultipleRecords(qryselectCustomer, false, null);

        if (qryResultsCustomer != null && qryResultsCustomer.value != null && qryResultsCustomer.value.length > 0) {
            var custRelationshipType = null;

            custRelationshipType = qryResultsCustomer.customertypecode;
            IsPriorityCare = qryResultsCustomer.tc_prioritycare;

            switch (custRelationshipType) {
                case 0: //Anonymous Customer
                    IsCustomerAnonymous = true;
                    break;
                case 1: //Real-Time Customer
                default:
                    IsCustomerAnonymous = false;
                    break;
            }
        }
    }
}

function isCaseExisting_OnChange(executionContext, onChangeCheck) {
    var isCEVal = GetAttributeValue(executionContext, "tc_iscaseexisting");
    switch (isCEVal) {
        case 0: //No
            SetAttrDisability(executionContext, "tc_existingcase", true);
            SetAttrRequiredLevel(executionContext, "tc_existingcase", "none");
            if (onChangeCheck) {
                SetAttributeValue(executionContext, "tc_existingcase", null);
                SetAttrSubmitMode(executionContext, "tc_existingcase", "always");
            }
            SetAttrDisability(executionContext, "tc_casetype", false);
            SetAttrsRequiredLevel(executionContext, requiredFieldsForIsECNo, ", ", "required");
            SetFieldFocus(executionContext, "tc_casetype");
            break;
        case 1: //Yes
            SetAttrDisability(executionContext, "tc_casetype", true);
            SetAttrsRequiredLevel(executionContext, requiredFieldsForIsECNo, ", ", "none");
            if (onChangeCheck) {
                SetAttributeValue(executionContext, "tc_casetype", null);
                SetAttrSubmitMode(executionContext, "tc_casetype", "always");
            }
            caseType_OnChange(executionContext, onChangeCheck);
            SetAttrDisability(executionContext, "tc_existingcase", false);
            SetAttrRequiredLevel(executionContext, "tc_existingcase", "required");
            SetFieldFocus(executionContext, "tc_existingcase");
            break;
        default: //Null            
            SetAttrsDisability(executionContext, fieldsForIsECNull, ", ", true);
            SetAttrsRequiredLevel(executionContext, fieldsForIsECNull, ", ", "none");
            if (onChangeCheck) {
                var attrsArray = SplitStringData(fieldsForIsECNull, ", ");
                if (attrsArray != null && attrsArray.length > 0) {
                    for (var i = 0; i < attrsArray.length; i++) {
                        var attrVal = GetAttributeValue(executionContext, attrsArray[i]);
                        if (attrVal != null) {
                            SetAttributeValue(executionContext, attrsArray[i], null);
                            SetAttrSubmitMode(executionContext, attrsArray[i], "always");
                        }
                    }
                }
            }
            break;
    }
    RefreshFormRibbon(executionContext, false);
}

function caseType_OnChange(executionContext, onChangeCheck) {
    var objCaseType = GetAttributeValue(executionContext, "tc_casetype");

    if (onChangeCheck) {
        if (GetAttributeValue(executionContext, "tc_category") != null) {
            SetAttributeValue(executionContext, "tc_category", null);
        }
        SetAttrDisability(executionContext, "tc_category", false);

        if (objCaseType == null) {
            SetAttrDisability(executionContext, "tc_category", true);
        }
    }
    else {
        if (objCaseType != null) {
            SetAttrDisability(executionContext, "tc_category", false);
        }
        else {
            if (GetAttributeValue(executionContext, "tc_category") != null) {
                SetAttributeValue(executionContext, "tc_category", null);
            }
            SetAttrDisability(executionContext, "tc_category", true);
        }
    }

    // add the event handler for PreSearch Event
    var ctrlCaseCategory = GetControl(executionContext, "tc_category");
    if (ctrlCaseCategory != null && !IsCustomerAnonymous) {
        ctrlCaseCategory.addPreSearch(addFilter);
    }

    caseCategory_OnChange(executionContext, "tc_category", onChangeCheck);
}

function addFilter() {
    //check if the city is not empty
    var caseTypeId = GetLookupValId(formExecuteContext, "tc_casetype");

    if (caseTypeId != null) {

        //create a filter xml
        var customFilter = "<filter type='and'>" +
            "<condition attribute='tc_casetype' operator='eq' value='" + caseTypeId + "'/>" +
            "</filter>";

        //add filter
        formExecuteContext.getFormContext().getControl("tc_category").addCustomFilter(customFilter, "tc_casecategory");
    }
}

function caseCategory_OnChange(executionContext, onChangeCheck) {
    var objCategory = GetAttributeValue(executionContext, "tc_category");

    if (onChangeCheck) {
        if (GetAttributeValue(executionContext, "tc_subcategory") != null) {
            SetAttributeValue(executionContext, "tc_subcategory", null);
            AttrFireOnChange(executionContext, "tc_subcategory");
        }
        SetAttrDisability(executionContext, "tc_subcategory", false);

        if (objCategory == null) {
            SetAttrDisability(executionContext, "tc_subcategory", true);
        }
    }
    else {
        if (objCategory != null) {
            SetAttrDisability(executionContext, "tc_subcategory", false);
        }
        else {
            if (GetAttributeValue(executionContext, "tc_subcategory") != null) {
                SetAttributeValue(executionContext, "tc_subcategory", null);
                AttrFireOnChange(executionContext, "tc_subcategory");
            }
            SetAttrDisability(executionContext, "tc_subcategory", true);
        }
    }
}

function caseSubCategory_OnChange(executionContext) {
    var priorityCodeVal = null;

    if (IsPriorityCare) {
        priorityCodeVal = 1; //High
    }
    else {
        var objSubCategoryId = GetLookupValId(executionContext, "tc_subcategory");
        if (objSubCategoryId != null) {
            var qrySubCatgryDfConfigs = "tc_casesubcategories(" + objSubCategoryId + ")?$select=tc_defaultpriority";
            var qryResultSubCatgry = RetrieveMultipleRecords(qrySubCatgryDfConfigs, false, null);

            if (qryResultSubCatgry != null && qryResultSubCatgry != undefined) {
                priorityCodeVal = qryResultSubCatgry.tc_defaultpriority;
            }
        }
        else {
            priorityCodeVal = null
        }
    }

    SetAttributeValue(executionContext, "tc_priority", priorityCodeVal);
    SetAttrSubmitMode(executionContext, "tc_priority", "always");
}

function ChangeLookupLanguages(executionContext) {
    //tc_casetype
    getArabicLookups(executionContext, "tc_casetypes", "tc_casetype", "tc_arabicname");
    //tc_category
    getArabicLookups(executionContext, "tc_casecategories", "tc_category", "tc_arabicname");
    //tc_subcategory
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_subcategory", "tc_arabicname");
    //tc_customercategory
    getArabicLookups(executionContext, "tc_customercategories", "tc_customercategory", "tc_arabicname");
    //tc_channelorigin
    getArabicLookups(executionContext, "tc_channelorigins", "tc_channelorigin", "tc_arabicname");
}