var formOnLoadRequiredFields = "tc_motregistered, tc_facilitynamearabic, telephone1, emailaddress1, tc_serviceprovidertype, cc_region";
var formOnLoadRecommendedFields = "accountnumber, tc_licenseexpirydate";
var alertStrings = new Array(), alertOptions = new Array(), fncSuccess = null;
var roleCheckforMTRegistered = "SkipManagersandSupervisors";
var userLCID = GetContextUserLCID();

function form_OnLoad(executionContext) {
    var businessTypeVal = GetAttributeValue(executionContext, "cc_businesstype");
    if (businessTypeVal == null) {
        SetAttributeValue(executionContext, "cc_businesstype", 948120001); //Tourist Care
        SetAttrSubmitMode(executionContext, "cc_businesstype", "always");
    }
    SetAttrsRequiredLevel(executionContext, formOnLoadRequiredFields, ", ", "required");
    SetAttrsRequiredLevel(executionContext, formOnLoadRecommendedFields, ", ", "recommended");
    ttpType_OnChange(executionContext);
    setTimeout(function () { dtLicenseExpiry_OnChange(executionContext, false); }, 2000);
    contactNo_Validation(executionContext, "telephone1");
    disable_MTRegistered(executionContext);
    ChangeLookupLanguages(executionContext);
}

function ttpType_OnChange(executionContext) {
    var ttpypeVal = GetAttributeValue(executionContext, "tc_serviceprovidertype");

    switch (ttpypeVal) {
        case 1: //Accommodation Facility                 
            SetAttrRequiredLevel(executionContext, "tc_accommodationtype", "required");
            SetSectionVisibility(executionContext, "tab_summary", "sec_accommodationfacilitydetails", true);
            SetSectionVisibility(executionContext, "tab_summary", "sec_tourguidedetails", false);
            SetSectionVisibility(executionContext, "tab_summary", "sec_addressdetails", true);
            break;
        case 2: //Tourist Agency
            SetAttrRequiredLevel(executionContext, "tc_accommodationtype", "none");
            SetSectionVisibility(executionContext, "tab_summary", "sec_addressdetails", true);
            SetSectionVisibility(executionContext, "tab_summary", "sec_accommodationfacilitydetails", false);
            SetSectionVisibility(executionContext, "tab_summary", "sec_tourguidedetails", false);
            break;
        case 3: //Tour Guide               
            SetAttrRequiredLevel(executionContext, "tc_accommodationtype", "none");
            SetSectionVisibility(executionContext, "tab_summary", "sec_tourguidedetails", true);
            SetSectionVisibility(executionContext, "tab_summary", "sec_accommodationfacilitydetails", false);
            SetSectionVisibility(executionContext, "tab_summary", "sec_addressdetails", false);
            break;
        default:
            SetAttrRequiredLevel(executionContext, "tc_accommodationtype", "none");
            SetSectionVisibility(executionContext, "tab_summary", "sec_tourguidedetails", false);
            SetSectionVisibility(executionContext, "tab_summary", "sec_accommodationfacilitydetails", false);
            SetSectionVisibility(executionContext, "tab_summary", "sec_addressdetails", false);
            break;
    }
}

function dtLicenseExpiry_OnChange(executionContext, onChangeCheck) {
    var dtLExpVal = GetAttributeValue(executionContext, "tc_licenseexpirydate");

    if (dtLExpVal != null) {
        var dtValidation = CheckDateWithCurrentDt(dtLExpVal);

        if (dtValidation) {
            if (onChangeCheck) {
                switch (userLCID) {
                    case 1025: //Arabic
                        alertStrings = { confirmButtonLabel: ConfirmBtnLabelAR, text: DateMsgEN, title: titleDateMsgAR };
                        break;
                    case 1033: //English
                    default:
                        alertStrings = { confirmButtonLabel: ConfirmBtnLabelEN, text: DateMsgEN, title: titleDateMsgEN };
                        break;
                }

                alertOptions = { height: 120, width: 260 };
                openAlertDialog(alertStrings, alertOptions, null);
                SetAttributeValue(executionContext, attrName, null);
            }
            else {
                parent.$("[data-id='tc_licenseexpirydate.fieldControl-date-time-input']").css({ "-webkit-text-fill-color": "red", "font-weight": "bold" });
            }
        }
        else {
            parent.$("[data-id='tc_licenseexpirydate.fieldControl-date-time-input']").css({ "-webkit-text-fill-color": "#666669", "font-weight": "normal" });
        }
    }
    else {
        parent.$("[data-id='tc_licenseexpirydate.fieldControl-date-time-input']").css({ "-webkit-text-fill-color": "#666669", "font-weight": "normal" });
    }
}

function openAlertDialog(alertStrngs, alertOptns, fncSuccess) {
    Xrm.Navigation.openAlertDialog(alertStrngs, alertOptns).then(
        function (success) {
            if (fncSuccess != null) {
                fncSuccess();
            }
        },
        function (error) {
            alert(error.message);
        }
    );
}

function contactNo_Validation(executionContext, attrName) {
    var contactNoVal = GetAttributeValue(executionContext, attrName);

    if (contactNoVal != null) {
        var aryContactOutput = CheckConditionsForContactNo(contactNoVal);

        if (aryContactOutput != null) {
            if (aryContactOutput.validationStatus != undefined && !aryContactOutput.validationStatus) {
                if (aryContactOutput.validationMsg != null && aryContactOutput.validationMsg != undefined) {
                    SetAttrCtrlNotification(executionContext, attrName, aryContactOutput.validationMsg);
                }
            }
            else {
                if (aryContactOutput.contactNoEndVal != null && contactNoVal.length != aryContactOutput.contactNoEndVal.length) {
                    SetAttributeValue(executionContext, attrName, aryContactOutput.contactNoEndVal);
                    SetAttrSubmitMode(executionContext, attrName, "always");
                }
                parent.$("[data-id='" + attrName + ".fieldControl-phone-text-input']").css({ "-webkit-text-fill-color": "black", "font-weight": "bold" });
                ClearAttrCtrlNotification(executionContext, attrName);
            }
        }
    }
    else {
        parent.$("[data-id='" + attrName + ".fieldControl-phone-text-input']").css({ "-webkit-text-fill-color": "#666669", "font-weight": "normal" });
        ClearAttrCtrlNotification(executionContext, attrName);
    }
}

function disable_MTRegistered(executionContext) {
    var disableMTRegistred = true;
    var loggedInUserRoles = RetrieveLoggedInD365UserSecurityRoles();
    var configParamVal = GetConfigParameterValue(roleCheckforMTRegistered, false, null);

    if (configParamVal != null) {
        var roleNames = null;

        if (configParamVal.indexOf(',') > -1) {
            roleNames = configParamVal.split(',');
        }
        else {
            roleNames = configParamVal;
        }

        if (roleNames != null && loggedInUserRoles != null && loggedInUserRoles.length > 0) {
            for (var i = 0; i < loggedInUserRoles.length; i++) {
                if (roleNames.includes(loggedInUserRoles[i].name.toLowerCase())) {
                    disableMTRegistred = false;
                    break;
                }
            }
        }
    }

    SetAttrDisability(executionContext, "tc_motregistered", disableMTRegistred);
}

function city_OnChange(executionContext) {
    var objCityId = GetLookupValId(executionContext, "cc_city");

    if (objCityId != null) {
        var qryRetrieveRegion = "cc_cities?$select=cc_cityid,_tc_region_value&$filter=cc_cityid eq " + objCityId;
        var qryResults = RetrieveMultipleRecords(qryRetrieveRegion, false, null);

        if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
            var regionLookupVal = new Array();
            regionLookupVal[0] = new Object();
            regionLookupVal[0].id = qryResults.value[0]["_tc_region_value"];
            regionLookupVal[0].name = qryResults.value[0]["_tc_region_value@OData.Community.Display.V1.FormattedValue"];
            regionLookupVal[0].entityType = qryResults.value[0]["_tc_region_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
            SetAttributeValue(executionContext, "cc_region", regionLookupVal);
            SetAttrSubmitMode(executionContext, "cc_region", "always");
            AttrFireOnChange(executionContext, "cc_region");
        }
    }
    else {
        if (GetAttributeValue(executionContext, "cc_region") != null) {
            SetAttributeValue(executionContext, "cc_region", null);
            SetAttrSubmitMode(executionContext, "cc_region", "always");
            AttrFireOnChange(executionContext, "cc_region");
        }
    }
}

function ChangeLookupLanguages(executionContext) {
    //cc_city
    getArabicLookups(executionContext, "cc_cities", "cc_city", "cc_arabicname");
    //cc_region
    getArabicLookups(executionContext, "cc_regions", "cc_region", "tc_arabicname");
    //parentaccountid
    getArabicLookups(executionContext, "accounts", "parentaccountid", "tc_facilitynamearabic");
}