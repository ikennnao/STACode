var roleCheckForSupervisorAndManager = "SkipManagersandSupervisors", roleCheckForQualityAssurance = "UpdateMomentofDelightRqts";

function form_OnLoad(executionContext) {
    enableApprvlFieldsForLoggedInUser(executionContext);
    ChangeLookupLanguages(executionContext);
}

function enableApprvlFieldsForLoggedInUser(executionContext) {
    var checkEnableApprvlFields = false, configParamVal = null, configRoleNames = null;
    var loggedInUserRoles = RetrieveLoggedInD365UserSecurityRoles();
    var requestType = GetAttributeValue(executionContext, "tc_requesttype");

    if (requestType != null) {
        switch (requestType) {
            case 1: //Pause SLA            
            case 3: //Cancel Case
                configParamVal = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);
                SetAttrVisibility(executionContext, "tc_relatedcase", false);
                SetAttrVisibility(executionContext, "regardingobjectid", true);
                break;
            case 4: //Change Case Priority                            
                configParamVal = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);
                SetAttrVisibility(executionContext, "tc_relatedcase", false);
                SetAttrsVisibility(executionContext, "regardingobjectid, tc_oldcasecategory, tc_oldcasesubcategory, tc_oldpriority, tc_newpriority", ", ", true);
                break;
            case 2: //Change Case Category
                SetAttrVisibility(executionContext, "tc_relatedcase", false);
                SetAttrsVisibility(executionContext, "regardingobjectid, tc_oldcasecategory, tc_oldcasesubcategory, tc_newcasecategory, tc_newcasesubcateogry", ", ", true);
                SetAttrRequiredLevel(executionContext, "tc_requestreason", "required");
                configParamVal = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);
                break;
            case 5: //Moment of Delight
                SetAttrVisibility(executionContext, "regardingobjectid", false);
                SetAttrVisibility(executionContext, "tc_relatedcase", true);
                SetAttrRequiredLevel(executionContext, "tc_requestreason", "none");
                configParamVal = GetConfigParameterValue(roleCheckForQualityAssurance, false, null);
                break;
            default:
                SetAttrVisibility(executionContext, "tc_relatedcase", false);
                SetAttrRequiredLevel(executionContext, "tc_requestreason", "required");
                break;
        }
    }

    if (configParamVal != null) {
        if (configParamVal.indexOf(",") > -1) {
            configRoleNames = configParamVal.split(',');
        }
        else {
            configRoleNames = configParamVal;
        }

        if (loggedInUserRoles != null && loggedInUserRoles.length > 0) {
            for (var i = 0; i < loggedInUserRoles.length; i++) {
                if (configRoleNames.includes(loggedInUserRoles[i].name.toLowerCase())) {
                    checkEnableApprvlFields = true;
                    break;
                }
            }
        }
    }

    if (checkEnableApprvlFields) {
        SetAttrsDisability(executionContext, "tc_requestapprovalstatus, tc_approverscomments", ", ", false);
    }
    else {
        SetAttrsDisability(executionContext, "tc_requestapprovalstatus, tc_approverscomments", ", ", true);
    }
}

function setApprovedByUser(executionContext) {
    var UserID = new Array();
    var approvee = {
        id: ContextUserDetails.userId,
        entityType: "systemuser",
        name: ContextUserDetails.userName
    }
    UserID.push(approvee);
    SetAttributeValue(executionContext, "tc_approvedby", UserID);
    SetAttrSubmitMode(executionContext, "tc_approvedby", "always");
}

function ChangeLookupLanguages(executionContext) {
    //tc_requestreason
    getArabicLookups(executionContext, "tc_casereasons", "tc_requestreason", "tc_arabicname");
    //tc_oldcasecategory
    getArabicLookups(executionContext, "tc_casecategories", "tc_oldcasecategory", "tc_arabicname");
    //tc_oldcasesubcategory
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_oldcasesubcategory", "tc_arabicname");
    //tc_newcasecategory
    getArabicLookups(executionContext, "tc_casecategories", "tc_newcasecategory", "tc_arabicname");
    //tc_newcasesubcateogry
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_newcasesubcateogry", "tc_arabicname");
}