function ConvertToCase_EnableRule(executionContext) {
    var doesAnyCaseAssociatedVal = GetAttributeValue(executionContext, "tc_doesanycaseassociated");
    var caseTypeVal = GetAttributeValue(executionContext, "tc_casetype");
    var categoryVal = GetAttributeValue(executionContext, "tc_category");
    var subCategoryVal = GetAttributeValue(executionContext, "tc_subcategory");
    var existingCaseVal = GetAttributeValue(executionContext, "tc_existingcase");

    if ((doesAnyCaseAssociatedVal == null || doesAnyCaseAssociatedVal == false) &&
        ((caseTypeVal != null && categoryVal != null && subCategoryVal != null) || (existingCaseVal != null))) {
        return true;
    }
    else {
        return false;
    }
}

function ConvertToCase_CustomRule(executionContext) {
    var isCaseExistingVal = GetAttributeValue(executionContext, "tc_iscaseexisting");
    var isAnyCaseAssociated = false, entCreatedCaseId = null, existingCaseId = null;

    switch (isCaseExistingVal) {
        case 0: //No
            var emailaddressVal = GetAttributeValue(executionContext, "tc_emailaddress");
            var contactNoVal = GetAttributeValue(executionContext, "tc_contactnumber");
            var customerId = GetLookupValId(executionContext, "tc_customer");
            var customerEntityType = GetLookupEntityType(executionContext, "tc_customer");
            var caseTypeId = GetLookupValId(executionContext, "tc_casetype");
            var categoryId = GetLookupValId(executionContext, "tc_category");
            var subCategoryId = GetLookupValId(executionContext, "tc_subcategory");
            var priorityVal = GetAttributeValue(executionContext, "tc_priority");
            var channelOriginId = GetLookupValId(executionContext, "tc_channelorigin");
            var targetConversationId = GetTargetRecordId(executionContext);

            if (caseTypeId != null && categoryId != null && subCategoryId != null && channelOriginId != null) {
                var entCreateCase = {};
                entCreateCase["tc_channelorigin@odata.bind"] = "/tc_channelorigins(" + channelOriginId + ")";
                entCreateCase["tc_casetype@odata.bind"] = "/tc_casetypes(" + caseTypeId + ")";

                if (customerId != null && customerEntityType != null) {
                    switch (customerEntityType) {
                        case "account":
                            entCreateCase["customerid_account@odata.bind"] = "/accounts(" + customerId + ")";
                            break;
                        case "contact":
                            entCreateCase["customerid_contact@odata.bind"] = "/contacts(" + customerId + ")";
                            break;
                    }
                }
                else {
                    customerId = RetrieveAnonymousCustomer();
                    if (customerId != null) {
                        entCreateCase["customerid_contact@odata.bind"] = "/contacts(" + customerId + ")";
                    }
                }

                entCreateCase["tc_category@odata.bind"] = "/tc_casecategories(" + categoryId + ")";
                entCreateCase["tc_subcategory@odata.bind"] = "/tc_casesubcategories(" + subCategoryId + ")";
                entCreateCase.emailaddress = emailaddressVal;
                entCreateCase.tc_contactnumber = contactNoVal;
                entCreateCase.prioritycode = priorityVal;
                entCreateCase["tc_relatedconversation@odata.bind"] = "/tc_conversations(" + targetConversationId + ")";

                entCreatedCaseId = CreateRecordInCRMUsgWebApi(entCreateCase, "incidents", false, null);
                if (entCreatedCaseId != null) {
                    isAnyCaseAssociated = true;
                }
            }
            break;
        case 1: //Yes
            existingCaseId = GetLookupValId(executionContext, "tc_existingcase");

            if (existingCaseId != null) {
                entCreatedCaseId = existingCaseId;
                isAnyCaseAssociated = true;
            }
            break;
    }

    if (isAnyCaseAssociated && entCreatedCaseId != null) {
        if (existingCaseId == null) {
            var relatedCaseObj = new Array();
            relatedCaseObj[0] = new Object();
            relatedCaseObj[0].id = entCreatedCaseId;
            relatedCaseObj[0].entityType = "incident";
            SetAttributeValue(executionContext, "tc_existingcase", relatedCaseObj);
            SetAttrSubmitMode(executionContext, "tc_existingcase", "always");
        }
        SetAttributeValue(executionContext, "tc_doesanycaseassociated", true);
        SetAttrSubmitMode(executionContext, "tc_doesanycaseassociated", "always");
        SetAttributeValue(executionContext, "statecode", 1);
        SetAttrSubmitMode(executionContext, "statecode", "always");
        SetAttributeValue(executionContext, "statuscode", 2);
        SetAttrSubmitMode(executionContext, "statuscode", "always");
        executionContext.data.save().then(
            function (success) {
                var entityFormOptions = {};
                entityFormOptions["entityName"] = "incident";
                entityFormOptions["entityId"] = entCreatedCaseId;
                entityFormOptions["useQuickCreateForm"] = false;
                entityFormOptions["windowPosition"] = 2;

                // Open the Created Case Record.
                Xrm.Navigation.openForm(entityFormOptions).then(
                    function (success) {
                    },
                    function (error) {
                        Xrm.Utility.alertDialog(error);
                    });
            },
            function (error) {
                Xrm.Utility.alertDialog(error);
            }
        );
    }
}