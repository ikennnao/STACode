var roleCheckForNewCase = "NewCasebuttonRole", currentUserRoles = null;
var ContextUserDetails = Xrm.Utility.getGlobalContext().userSettings;
var userLCID = null;

function GetContextUserLCID() {
    if (ContextUserDetails != null && ContextUserDetails != undefined) {
        userLCID = ContextUserDetails.languageId;
    }
    else {
        alert("Required parameters are not defined. Please contact System Admin for further assistance.");
    }
    return userLCID;
}

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

    if (formtype == "tc - trade provider 360" || "رعاية الزوار - العميل 360") {
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

function OpenNewCase(executionContext, sourceId) {
    var entityFormOptions = {}, formParameters = {};
    entityFormOptions["entityName"] = "incident";
    entityFormOptions["useQuickCreateForm"] = false;
    entityFormOptions["windowPosition"] = 1;
    //var recordId = executionContext.data.entity.getId();

    if (sourceId == 1) {
        formParameters["customerid"] = executionContext.data.entity.getId();
        formParameters["customeridname"] = executionContext.data.entity.getPrimaryAttributeValue();
        formParameters["customeridtype"] = executionContext.data.entity.getEntityName();
    }
    else if (sourceId == 2) {
        formParameters["tc_serviceprovider"] = executionContext.data.entity.getId();
        formParameters["tc_serviceprovidername"] = executionContext.data.entity.getPrimaryAttributeValue();
        formParameters["tc_serviceprovidertype"] = executionContext.data.entity.getEntityName();
    }
    // Open the Quick Create form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
            if (success != null && success.savedEntityReference != null && success.savedEntityReference.length != undefined) {
                // SetAttributeValue(executionContext, "tc_requesttype", 2);
                // RefreshFormRibbon(executionContext, false);
            }
        },
        function (error) {
            alert(error);
        });
}

function PopulateCaseTypeCommand(commandProperties) {
    var entityName = Xrm.Page.data.entity.getEntityName();
    var isUCI = Xrm.Internal.isUci(), page = "Form", commandId = 'tc.account.Form.CreateCase.Command';
    var buttonfortouristprovidertxt = null, buttonfortouristprovidertooltip = null, buttonAgainsttouristprovidertxt = null, buttonAgainsttouristprovidertooltip = null;
    if (isUCI) {
        commandId = entityName + "|NoRelationship|" + page + "|" + commandId;
    }

    userLCID = GetContextUserLCID();
    switch (userLCID) {
        case 1025: //Arabic
            buttonfortouristprovidertxt = casefortouristproviderAR;
            buttonfortouristprovidertooltip = tooltipcasefortouristproviderAR;
            buttonAgainsttouristprovidertxt = caseAgainsttouristproviderAR;
            buttonAgainsttouristprovidertooltip = tooltipcaseAgainsttouristproviderAR;
            break;
        case 1033: //English
        default:
            buttonfortouristprovidertxt = casefortouristproviderEN;
            buttonfortouristprovidertooltip = tooltipcasefortouristproviderEN;
            buttonAgainsttouristprovidertxt = caseAgainsttouristproviderEN;
            buttonAgainsttouristprovidertooltip = tooltipcaseAgainsttouristproviderEN;
            break;
    }

    var menuXml = '<Menu Id="cc.CustomApproval.Menu">';
    menuXml += '<MenuSection Id="cc.CustomApprovalSection" Sequence="10">';
    menuXml += '<Controls Id="cc.CustomApprovalControls">';
    menuXml += '<Button Alt="' + buttonfortouristprovidertxt + '" Command="' + commandId + '" Id="tc.Button.ForTourProviderButton" class="hide" LabelText="' + buttonfortouristprovidertxt + '" Sequence="20" TemplateAlias="o1" ToolTipTitle="' + buttonfortouristprovidertooltip + '" ToolTipDescription="" />';
    menuXml += '<Button Alt="' + buttonAgainsttouristprovidertxt + '" Command="' + commandId + '" Id="tc.Button.AgainstTourProviderButton" class="hide"  LabelText="' + buttonAgainsttouristprovidertxt + '" Sequence="10" TemplateAlias="o1" ToolTipTitle="' + buttonAgainsttouristprovidertooltip + '" ToolTipDescription="" />';
    menuXml += '</Controls>';
    menuXml += '</MenuSection>';
    menuXml += '</Menu>';
    commandProperties["PopulationXML"] = menuXml;
}

function onTCAccountMenuItemClick(commandProperties, executionContext) {
    var sourceId;
    if (commandProperties.SourceControlId === "tc.Button.ForTourProviderButton") //for tc
    {
        sourceId = 1;
    }
    else if (commandProperties.SourceControlId === "tc.Button.AgainstTourProviderButton") //against tc
    {
        sourceId = 2;
    }
    OpenNewCase(executionContext, sourceId);
}

function btnMngAcctLvl_EnableRule(executionContext) {
    var ShowLevelofAccountbtn = true;
    var formtype = GetFormName(executionContext);
    if (formtype == "tc - trade provider 360") {
        ShowLevelofAccountbtn = false;
    }
    return ShowLevelofAccountbtn;
}