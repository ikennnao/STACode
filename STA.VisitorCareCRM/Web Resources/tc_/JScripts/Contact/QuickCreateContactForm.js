//Quick Create form
var COFReqFieldsForWeb = "tc_salutation, mobilephone, emailaddress1, tc_countryofresidence, tc_customersegment, tc_nationality";
var COFReqFieldsForPortalMobileAppKiosk = "mobilephone, emailaddress1";
var COFReqFieldsForEmailLiveChatSM = "emailaddress1";
var COFRecFieldsForEmailLiveChatSM = "mobilephone";
var COFReqFieldsForPhoneCallWhatsApp = "mobilephone";
var COFRecFieldsForPhoneCallWhatsApp = "emailaddress1";
var COFRecFieldsCommon = "tc_salutation, tc_countryofresidence, tc_customersegment, tc_nationality";
var alertStrings = new Array(), alertOptions = new Array();
var userLCID = GetContextUserLCID();

function form_OnLoad(executionContext) {
    var businessTypeVal = GetAttributeValue(executionContext, "cc_businesstype");
    if (businessTypeVal == null) {
        SetAttributeValue(executionContext, "cc_businesstype", 948120001); //Tourist Care
        SetAttrSubmitMode(executionContext, "cc_businesstype", "always");
    }
    SetAttrRequiredLevel(executionContext, "tc_channelorigin", "required");
    checkParentAccountID(executionContext);
    custCatgeory_OnChange(executionContext);
    ChangeLookupLanguages(executionContext);
}

function checkParentAccountID(executionContext) {
    if (GetAttributeValue(executionContext, "tc_parentaccount") != null && GetAttributeValue(executionContext, "tc_customercategory") == null) {
        getCustomerCategoryId(executionContext);
    }
}

function custCatgeory_OnChange(executionContext) {
    SetAttrRequiredLevel(executionContext, "tc_customercategory", "required");

    var custCategoryIdVal = getCustCatgeyIdType(executionContext);
    switch (custCategoryIdVal) {
        case "1": //B2B
            SetAttrVisibility(executionContext, "tc_parentaccount", true);
            SetAttrRequiredLevel(executionContext, "tc_parentaccount", "required");
            break;
        case "2": //B2C or null
        default:
            SetAttrRequiredLevel(executionContext, "tc_parentaccount", "none");
            SetAttrVisibility(executionContext, "tc_parentaccount", false);
            break;
    }
    channelOfOrigin_OnChange(executionContext);
}

function getCustomerCategoryId(executionContext) {
    var selectQuery = null, qryResults = null;
    var selectQuery = "tc_customercategories?$select=tc_customercategoryid,tc_name&$filter=tc_categoryid eq '1'";
    qryResults = RetrieveMultipleRecords(selectQuery, false, null);
    if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
        var custCategoryVal = new Array();
        custCategoryVal[0] = new Object();
        custCategoryVal[0].id = qryResults.value[0]["tc_customercategoryid"];
        custCategoryVal[0].name = qryResults.value[0]["tc_name"];
        custCategoryVal[0].entityType = "tc_customercategory";
        SetAttributeValue(executionContext, "tc_customercategory", custCategoryVal);
        getsetCustSegmentId(executionContext, custCategoryVal[0].id);
    }
}

function getsetCustSegmentId(executionContext, custCategoryidVal) {
    var selectQuery = null, qryResults = null;
    var selectQuery = "tc_customersegments?$select=tc_customersegmentid,tc_name&$filter=_tc_customercategory_value eq " + custCategoryidVal;
    qryResults = RetrieveMultipleRecords(selectQuery, false, null);
    if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
        var custSegmentVal = new Array();
        custSegmentVal[0] = new Object();
        custSegmentVal[0].id = qryResults.value[0]["tc_customersegmentid"];
        custSegmentVal[0].name = qryResults.value[0]["tc_name"];
        custSegmentVal[0].entityType = "tc_customersegment";
        SetAttributeValue(executionContext, "tc_customersegment", custSegmentVal);
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

    if (caseOriginCodeVal != null) {
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
            default:
                SetAttrsRequiredLevel(executionContext, COFReqFieldsForWeb, ", ", "required");
                break;
            case "coc-010": //Kiosk
            case "coc-011": //Bayena Portal
            case "coc-013": //Mobile App
                SetAttrsRequiredLevel(executionContext, COFReqFieldsForPortalMobileAppKiosk, ", ", "required");
                SetAttrsRequiredLevel(executionContext, COFRecFieldsCommon, ", ", "recommended");
                break;
        }
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
                parent.$("[data-id='" + attrName + ".fieldControl-phone-text-input-aria-descriptor']").css({ "-webkit-text-fill-color": "black", "font-weight": "bold" });
                ClearAttrCtrlNotification(executionContext, attrName);
            }
        }
    }
    else {
        parent.$("[data-id='" + attrName + ".fieldControl-phone-text-input-aria-descriptor']").css({ "-webkit-text-fill-color": "#666669", "font-weight": "normal" });
        ClearAttrCtrlNotification(executionContext, attrName);
    }
}

function dtPassportExpiry_OnChange(executionContext) {
    var dtLExpVal = GetAttributeValue(executionContext, "tc_passportexpirydate");

    if (dtLExpVal != null) {
        checkdtPassportExpiry(executionContext, "tc_passportexpirydate", dtLExpVal);
    }
}

function checkdtPassportExpiry(executionContext, attrName, attrDtValue) {
    var dtValidation = CheckDateWithCurrentDt(attrDtValue);

    if (dtValidation) {
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

function ChangeLookupLanguages(executionContext) {
    //tc_channelorigin
    getArabicLookups(executionContext, "tc_channelorigins", "tc_channelorigin", "tc_arabicname");
    //tc_customercategory
    getArabicLookups(executionContext, "tc_customercategories", "tc_customercategory", "tc_arabicname");
    //tc_customersegment
    getArabicLookups(executionContext, "tc_customersegments", "tc_customersegment", "tc_arabicname");
    //tc_serviceprovider
    getArabicLookups(executionContext, "accounts", "tc_parentaccount", "tc_facilitynamearabic");
    //Nationality
    getArabicLookups(executionContext, "cc_countries", "tc_nationality", "cc_arabicname");
    //tc_preferredcommunicationlanguage
    getArabicLookups(executionContext, "tc_preferredlanguages", "tc_preferredcommunicationlanguage", "tc_arabicname");
}