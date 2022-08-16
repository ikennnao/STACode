var COFReqFieldsForWeb = "tc_salutation, mobilephone, emailaddress1, tc_countryofresidence, tc_customersegment, tc_nationality";
var COFReqFieldsForPortalMobileAppKiosk = "mobilephone, emailaddress1";
var COFReqFieldsForEmailLiveChatSM = "emailaddress1";
var COFRecFieldsForEmailLiveChatSM = "mobilephone";
var COFReqFieldsForPhoneCallWhatsApp = "mobilephone";
var COFRecFieldsForPhoneCallWhatsApp = "emailaddress1";
var COFRecFieldsCommon = "tc_salutation, tc_countryofresidence, tc_customersegment, tc_nationality";
var formType = null, alertStrings = new Array(), alertOptions = new Array();
var userLCID = GetContextUserLCID();

function form_OnLoad(executionContext) {
    var custTypeCodeVal = GetAttributeValue(executionContext, "customertypecode");

    switch (custTypeCodeVal) {
        case 0: //Anonymous
            DisableAllAttrCtrls(executionContext);
            break;
        case 1: //Customer
        default:
            formType = GetFormType(executionContext);
            var businessTypeVal = GetAttributeValue(executionContext, "cc_businesstype");
            if (businessTypeVal == null) {
                SetAttributeValue(executionContext, "cc_businesstype", 948120001); //Tourist Care
                SetAttrSubmitMode(executionContext, "cc_businesstype", "always");
            }

            SetAttrRequiredLevel(executionContext, "tc_channelorigin", "required");
            custCatgeory_OnChange(executionContext, false);
            channelOfOrigin_OnLoad(executionContext);
            hideShowAdminTabAndSections(executionContext);
            if (formType == 1) { //Create
                SetAttrDisability(executionContext, "tc_channelorigin", false);
                SetSectionVisibility(executionContext, "tab_summary", "sec_custratings", false);
            }
            setTimeout(function () { contactNo_Validation(executionContext, "mobilephone"); }, 2000);
            setTimeout(function () { contactNo_Validation(executionContext, "telephone2"); }, 2000);
            setTimeout(function () { dtPassportExpiry_OnChange(executionContext, false); }, 2000);
            ChangeLookupLanguages(executionContext);
            break;
    }
}

function form_OnSave(executionContext) {
    formType = GetFormType(executionContext);
    if (formType == 1) {
        SetSectionVisibility(executionContext, "tab_summary", "sec_custratings", true);
    }
    loadCustomerRating(executionContext);
}

function loadCustomerRating(executionContext) {
    var wrControl = GetControl(executionContext, "WebResource_ContactRatingsForm_html");
    if (wrControl != null) {
        var wrCtrlSrc = wrControl.getSrc();
        wrControl.setSrc(wrCtrlSrc);
    }
}

function custCatgeory_OnChange(executionContext, onChangeCheck) {
    //
    SetAttrRequiredLevel(executionContext, "tc_customercategory", "required");
    var custCategoryIdVal = getCustCatgeyIdType(executionContext);
    switch (custCategoryIdVal) {
        case "1": //B2B
            SetSectionVisibility(executionContext, "tab_summary", "sec_custpreferences", true);
            SetSectionVisibility(executionContext, "tab_additionalinfo", "sec_additionalcustdetails", false);
            SetTabVisibility(executionContext, "documents_sharepoint", false);
            SetAttrVisibility(executionContext, "tc_parentaccount", true);
            SetAttrRequiredLevel(executionContext, "tc_parentaccount", "required");
            break;
        case "2": //B2C or null
        default:
            SetAttrRequiredLevel(executionContext, "tc_parentaccount", "none");
            SetAttrVisibility(executionContext, "tc_parentaccount", false);
            SetSectionVisibility(executionContext, "tab_summary", "sec_custpreferences", true);
            SetSectionVisibility(executionContext, "tab_additionalinfo", "sec_additionalcustdetails", true);
            SetTabVisibility(executionContext, "documents_sharepoint", true);
            break;
    }
    isPriorityCare_OnChange(executionContext);
    channelOfOrigin_OnChange(executionContext);

    if (onChangeCheck) {
        SetAttributeValue(executionContext, "tc_customersegment", null);
    }
}

function getCustCatgeyIdType(executionContext) {
    var custCategoryidVal = null;
    var custCategoryGuid = GetLookupValId(executionContext, "tc_customercategory");

    if (custCategoryGuid != null) {
        var selectQuery = null, qryResults = null;

        var selectQuery = "tc_customercategories?$select=tc_categoryid&$filter=tc_customercategoryid eq " + custCategoryGuid;
        qryResults = RetrieveMultipleRecords(selectQuery, false, null);

        if (qryResults != null) {
            custCategoryidVal = qryResults.value[0]["tc_categoryid"];
        }
    }
    return custCategoryidVal;
}

function isPriorityCare_OnChange(executionContext) {
    var custCategoryIdVal = getCustCatgeyIdType(executionContext);
    var isHighProfileVal = GetAttributeValue(executionContext, "tc_prioritycare");

    switch (custCategoryIdVal) {
        case "1": //B2B            
            switch (isHighProfileVal) {
                case true:
                    SetAttrRequiredLevel(executionContext, "tc_designation", "none");
                    SetAttrVisibility(executionContext, "header_tc_designation", false);
                    SetAttrVisibility(executionContext, "jobtitle", true);
                    SetAttrRequiredLevel(executionContext, "jobtitle", "required");
                    break;
                case false:
                    SetAttrsRequiredLevel(executionContext, "jobtitle, tc_designation", ", ", "none");
                    SetAttrVisibility(executionContext, "jobtitle", true);
                    SetAttrVisibility(executionContext, "header_tc_designation", false);
                    break;
                case null:
                    SetAttrsRequiredLevel(executionContext, "jobtitle, tc_designation", ", ", "none");
                    SetAttrsVisibility(executionContext, "jobtitle, header_tc_designation", ", ", false);
                    break;
            }
            break;
        case "2": //B2C
            switch (isHighProfileVal) {
                case true:
                    SetAttrRequiredLevel(executionContext, "jobtitle", "none");
                    SetAttrVisibility(executionContext, "jobtitle", false);
                    SetAttrVisibility(executionContext, "header_tc_designation", true);
                    SetAttrRequiredLevel(executionContext, "tc_designation", "required");
                    break;
                case false:
                    SetAttrsRequiredLevel(executionContext, "jobtitle, tc_designation", ", ", "none");
                    SetAttrsVisibility(executionContext, "jobtitle, header_tc_designation", ", ", false);
                    break;
                case null:
                    SetAttrsRequiredLevel(executionContext, "jobtitle, tc_designation", ", ", "none");
                    SetAttrsVisibility(executionContext, "jobtitle, header_tc_designation", ", ", false);
                    break;
            }
            break;
        default: //null
            SetAttrsRequiredLevel(executionContext, "jobtitle, tc_designation", ", ", "none");
            SetAttrsVisibility(executionContext, "jobtitle, header_tc_designation", ", ", false);
            break;
    }
}

function channelOfOrigin_OnChange(executionContext) {
    var objCaseOriginId = null, caseOriginCodeVal = null;
    objCaseOriginId = GetLookupValId(executionContext, "tc_channelorigin");

    if (objCaseOriginId != null) {
        var qryForCaseOriginCode = null, resultsForCaseOriginCode = null;
        qryForCaseOriginCode = "tc_channelorigins?$select=tc_origincode&$filter=statecode eq 0 and tc_channeloriginid eq  " + objCaseOriginId;
        resultsForCaseOriginCode = RetrieveMultipleRecords(qryForCaseOriginCode, false, null);

        if (resultsForCaseOriginCode != null && resultsForCaseOriginCode.value != null && resultsForCaseOriginCode.value.length > 0) {
            caseOriginCodeVal = resultsForCaseOriginCode.value[0]["tc_origincode"];
            if (caseOriginCodeVal != null && caseOriginCodeVal != undefined) {
                caseOriginCodeVal = caseOriginCodeVal.toLowerCase();
            }
        }
    }

    switch (caseOriginCodeVal) {
        case "coc-001": //Phone Call
        case "coc-008": // WhatsApp 
            SetAttrRequiredLevel(executionContext, COFReqFieldsForPhoneCallWhatsApp, "required");
            SetAttrRequiredLevel(executionContext, COFRecFieldsForPhoneCallWhatsApp, "recommended");
            SetAttrsRequiredLevel(executionContext, COFRecFieldsCommon, ", ", "recommended");
            break;
        case "coc-002": //Email
        case "coc-004": //Facebook 
        case "coc-005": //Twitter 
        case "coc-006": //Instagram 
        case "coc-007": //Youtube 
        case "coc-009": //LiveChat
        case "coc-012": //Trip Advisor
            SetAttrRequiredLevel(executionContext, COFReqFieldsForEmailLiveChatSM, "required");
            SetAttrRequiredLevel(executionContext, COFRecFieldsForEmailLiveChatSM, "recommended");
            SetAttrsRequiredLevel(executionContext, COFRecFieldsCommon, ", ", "recommended");
            break;
        case "coc-003": //Web
        case null:
        default:
            SetAttrsRequiredLevel(executionContext, COFReqFieldsForWeb, ", ", "required");
            break;
        case "coc-010": //Kiosk
        case "coc-011": //Bayena Portal
        case "coc-013": //Mobile APP
            SetAttrsRequiredLevel(executionContext, COFReqFieldsForPortalMobileAppKiosk, ", ", "required");
            SetAttrsRequiredLevel(executionContext, COFRecFieldsCommon, ", ", "recommended");
            break;
    }
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

function dtPassportExpiry_OnChange(executionContext, onChangeCheck) {
    var dtValidation = null, dtLExpVal = null
    dtLExpVal = GetAttributeValue(executionContext, "tc_passportexpirydate");

    if (dtLExpVal != null) {
        dtValidation = CheckDateWithCurrentDt(dtLExpVal);

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
                SetAttributeValue(executionContext, "tc_passportexpirydate", null);
            }
            else {
                parent.$("[data-id='tc_passportexpirydate.fieldControl-date-time-input']").css({ "-webkit-text-fill-color": "red", "font-weight": "bold" });
            }
        }
        else {
            parent.$("[data-id='tc_passportexpirydate.fieldControl-date-time-input']").css({ "-webkit-text-fill-color": "#666669", "font-weight": "normal" });
        }
    }
    else {
        parent.$("[data-id='tc_passportexpirydate.fieldControl-date-time-input']").css({ "-webkit-text-fill-color": "#666669", "font-weight": "normal" });
    }
}

function openAlertDialog(alertStrngs, alertOptns) {
    Xrm.Navigation.openAlertDialog(alertStrngs, alertOptns).then(
        function (success) {
        },
        function (error) {
            alert(error.message);
        }
    );
}

function hideShowAdminTabAndSections(executionContext) {
    if (IsTargetUserSystemAdmin) {
        SetSectionVisibility(executionContext, "tab_additionalinfo", "sec_admindetails", true);
    }
    else {
        SetSectionVisibility(executionContext, "tab_additionalinfo", "sec_admindetails", false);
    }
}

function channelOfOrigin_OnLoad(executionContext) {
    var objChannelOriginVal = GetAttributeValue(executionContext, "tc_channelorigin");
    if (objChannelOriginVal != null && objChannelOriginVal != undefined) {
        SetAttrDisability(executionContext, "tc_channelorigin", true);
    }
}

function ChangeLookupLanguages(executionContext) {
    //tc_serviceprovider
    getArabicLookups(executionContext, "accounts", "tc_parentaccount", "tc_facilitynamearabic");
    //tc_channelorigin
    getArabicLookups(executionContext, "tc_channelorigins", "tc_channelorigin", "tc_arabicname");
    //tc_customercategory
    getArabicLookups(executionContext, "tc_customercategories", "tc_customercategory", "tc_arabicname");
    //tc_customersegment
    getArabicLookups(executionContext, "tc_customersegments", "tc_customersegment", "tc_arabicname");
    //Nationality
    getArabicLookups(executionContext, "cc_countries", "tc_nationality", "cc_arabicname");
    //CountryofResidence
    getArabicLookups(executionContext, "cc_countries", "tc_countryofresidence", "cc_arabicname");
    //tc_city
    getArabicLookups(executionContext, "cc_cities", "tc_cityofresidence", "cc_arabicname");
    //tc_preferredcommunicationlanguage
    getArabicLookups(executionContext, "tc_preferredlanguages", "tc_preferredcommunicationlanguage", "tc_arabicname");
}