var KACreateAmendKey = "KACreateAmendAccess";

function btnPublish_EnableRule(executionContext) {
    var enablePublishBtn = false;
    var bpfActiveStageName = GetBPFActiveStageName(executionContext);
    var reviewVal = GetAttributeValue(executionContext, "review");
    if ((bpfActiveStageName == "publish" || bpfActiveStageName == "نشر") && reviewVal == 0) { //Review == 'Approved'
        enablePublishBtn = true;
    }
    return enablePublishBtn;
}

function CheckUserRoleforKACreateAmendAccess() {
    var checkKACARoles = false;
    var currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();

    var ConfigRoleName = GetConfigParameterValue(KACreateAmendKey, false);
    if (ConfigRoleName != null) {
        var configrolearry = ConfigRoleName;
        var Roles = configrolearry.split(',');
        for (var i = 0; i < currentUserRoles.length; i++) {
            if (Roles.includes(currentUserRoles[i].name.toLowerCase())) {
                checkKACARoles = true;
                break;
            }
        }
    }

    return checkKACARoles;
}