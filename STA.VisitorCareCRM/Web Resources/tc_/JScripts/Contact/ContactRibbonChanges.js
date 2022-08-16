var roleCheckForNewCase = "NewCaseButtonRole", currentUserRoles = null;

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

function GetFormName(executionContext) {
    var targetFormName = null;

    var formContext = CheckForExecContext(executionContext);
    if (formContext != null) {
        if (formContext.ui != null && formContext.ui.formSelector != null && formContext.ui.formSelector.getCurrentItem() != null && formContext.ui.formSelector.getCurrentItem().getLabel() != null) {
            targetFormName = formContext.ui.formSelector.getCurrentItem().getLabel().toLowerCase();
        }
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return targetFormName;
}

function btnNewCase_EnableRule(executionContext) {
    var enableNewCase = false, configRoleNames = null;
    var formtype = GetFormName(executionContext);

    if (formtype == "tc - customer 360" || "رعاية الزوار - العميل 360") {
        if (currentUserRoles == null) {
            currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();
        }

        var configParamVal = GetConfigParameterValue(roleCheckForNewCase, false, null);
        if (configParamVal != null) {
            if (configParamVal.indexOf(",") > -1) {
                configRoleNames = configParamVal.split(',');
            }
            else {
                configRoleNames = configParamVal;
            }

            if (configRoleNames != null && configRoleNames.length > 0 && currentUserRoles != null && currentUserRoles.length > 0) {
                for (var i = 0; i < currentUserRoles.length; i++) {
                    if (configRoleNames.includes(currentUserRoles[i].name.toLowerCase())) {
                        enableNewCase = true;
                        break;
                    }
                }
            }
        }
    }
    return enableNewCase;
}

function OpenNewCase(executionContext) {
    var entityFormOptions = {}, formParameters = {};
    entityFormOptions["entityName"] = "incident";
    entityFormOptions["useQuickCreateForm"] = false;
    entityFormOptions["windowPosition"] = 1;

    formParameters["customerid"] = executionContext.data.entity.getId();
    formParameters["customeridname"] = executionContext.data.entity.getPrimaryAttributeValue();
    formParameters["customeridtype"] = executionContext.data.entity.getEntityName();

    // Open the Quick Create form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
        },
        function (error) {
            alert(error);
        });
}