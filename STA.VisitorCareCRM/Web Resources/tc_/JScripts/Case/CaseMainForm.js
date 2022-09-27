var formExecuteContext = null;
var roleAgtSupMng = "AgtSupMng", roleAgtSupMngExtAgtExtBM = "AgtSupMngExtAgtExtBM", roleSupMng = "SupMng";
var roleCheckForCCUpdateAccess = "OtherCaseFunctionalities", roleCheckForOSUpdateAccess = "SkipManagersandSupervisors";
var disableBPFCommonFields = "header_process_tc_casetype, header_process_customerid, header_process_tc_category, header_process_tc_subcategory, header_process_prioritycode, header_process_tc_momentofdelight, header_process_tc_momentofdelight_1, header_process_tc_momentofdelight_2";
var disableBPFComplaintFields = "header_process_tc_issubmitted, header_process_tc_pendingwith, header_process_tc_correspondencewith, header_process_resolvebyslastatus";
var fieldsRelatedToOS = "header_process_tc_isreportedtomanagement, header_process_tc_ismanagementrecommendedaction, header_process_tc_recommendedactions, tc_isreportedtomanagement, tc_ismanagementrecommendedaction, tc_recommendedactions";
var IsPriorityCare = false, IsCustomerAnonymous = false, previousCaseTypeName = null;


function form_OnLoad(executionContext) {
    formExecuteContext = executionContext;
    SetAttrRequiredLevel(executionContext, "description", "required");
    showRequestTypeMsgs(executionContext, null);
    loadSetCustomerInfo(executionContext, false);
    caseType_OnChange(executionContext, false);
    caseSubCategory_OnChange(executionContext, false);
    formExecuteContext.getFormContext().data.process.addOnStageChange(function (bpfStageContext) {
        hideShow_Sections_BPFActiveStage();
    });
    //hideShowAdminTabAndSections();    
    ChangeLookupLanguages(executionContext);

}



function loadSetCustomerInfo(executionContext, onChangeCheck) {
    var objCustomerVal = GetAttributeValue(executionContext, "customerid");

    if (objCustomerVal == null) {
        SetAttributeValue(executionContext, "tc_customername", null);
        SetAttrSubmitMode(executionContext, "tc_customername", "always");
        SetAttributeValue(executionContext, "emailaddress", null);
        SetAttrSubmitMode(executionContext, "emailaddress", "always");
        SetAttributeValue(executionContext, "tc_contactnumber", null);
        SetAttrSubmitMode(executionContext, "tc_contactnumber", "always");
    }
    else {
        var custEntityType = GetLookupEntityType(executionContext, "customerid");
        var caseCustName = GetAttributeValue(executionContext, "tc_customername");
        var caseEmail = GetAttributeValue(executionContext, "emailaddress");
        var caseContNo = GetAttributeValue(executionContext, "tc_contactnumber");
        var caseCustCategryId = GetLookupValId(executionContext, "tc_customercategory");

        if (custEntityType != null) {
            var qryselectCustCatgry = null, qryResultsCustCatgry = null, custCategoryValue = null, custIDVal = null, custNameVal = null;
            var customerIdVal = GetLookupValId(executionContext, "customerid");

            switch (custEntityType) {
                case "account":
                    SetSectionVisibility(executionContext, "tab_custsummary", "sec_custflagdetails", false); //Hide Customer Flag Details Section
                    SetSectionVisibility(executionContext, "tab_custsummary", "sec_customervisitdetails", false); //Hide Customer Visits Section

                    //Retrieve the Facility Details and set them on Target Case
                    var qryselectAccount = "accounts?$select=name,emailaddress1,telephone1&$filter=accountid eq " + customerIdVal;
                    var qryResultsAccount = RetrieveMultipleRecords(qryselectAccount, false, null);

                    if (qryResultsAccount != null && qryResultsAccount.value != null && qryResultsAccount.value.length > 0) {
                        var accName = null, accntMobPhn = null, accntEmail = null;

                        accName = qryResultsAccount.value[0]["name"];
                        accntMobPhn = qryResultsAccount.value[0]["telephone1"];
                        accntEmail = qryResultsAccount.value[0]["emailaddress1"];

                        if (caseCustName == null || (caseCustName != null && accName != null && (caseCustName.toLowerCase() != accName.toLowerCase()))) {
                            SetAttributeValue(executionContext, "tc_customername", accName);
                            SetAttrSubmitMode(executionContext, "tc_customername", "always");
                        }
                        if (caseEmail == null || (caseEmail != null && accntEmail != null && (caseEmail.toLowerCase() != accntEmail.toLowerCase()))) {
                            SetAttributeValue(executionContext, "emailaddress", accntEmail);
                            SetAttrSubmitMode(executionContext, "emailaddress", "always");
                        }
                        if (caseContNo == null || (caseContNo != null && accntMobPhn != null && (caseContNo.toLowerCase() != accntMobPhn.toLowerCase()))) {
                            SetAttributeValue(executionContext, "tc_contactnumber", accntMobPhn);
                            AttrFireOnChange(executionContext, "tc_contactnumber");
                        }

                        SetAttrsDisability(executionContext, "tc_customername, emailaddress, tc_contactnumber", ", ", true);
                        SetSectionVisibility(executionContext, "tab_custsummary", "sec_custdetails_2", true);
                    }

                    // Query to Retrieve B2B Category
                    qryselectCustCatgry = "tc_customercategories?$select=tc_customercategoryid,tc_name&$filter=tc_categoryid eq '1'";
                    break;
                case "contact":
                    SetSectionVisibility(executionContext, "tab_custsummary", "sec_custflagdetails", true);  //Show Customer Flag Details Section
                    SetSectionVisibility(executionContext, "tab_custsummary", "sec_customervisitdetails", true); //Show Customer Visits Section

                    //Check whether the target contact is Anonymous Customer or not
                    var qryselectCustomer = "contacts?$select=firstname,lastname,emailaddress1,mobilephone,emailaddress2,telephone2,customertypecode,tc_prioritycare,_tc_customercategory_value&$filter=contactid eq " + customerIdVal;
                    var qryResultsCustomer = RetrieveMultipleRecords(qryselectCustomer, false, null);

                    if (qryResultsCustomer != null && qryResultsCustomer.value != null && qryResultsCustomer.value.length > 0) {
                        var custFirstName = null, custLastName = null, custRelationshipType = null, custPrimaryCntNo = null, custPrimaryEmail = null, custSecondaryCntNo = null, custSecondaryEmail = null, custCategoryID = null, custCategoryName = null;
                        var custFinalName = null, custFinalEmail = null, custFinalCntNo = null;

                        custFirstName = qryResultsCustomer.value[0]["firstname"];
                        custLastName = qryResultsCustomer.value[0]["lastname"];
                        custRelationshipType = qryResultsCustomer.value[0]["customertypecode"];
                        custPrimaryCntNo = qryResultsCustomer.value[0]["mobilephone"];
                        custPrimaryEmail = qryResultsCustomer.value[0]["emailaddress1"];
                        custSecondaryEmail = qryResultsCustomer.value[0]["emailaddress2"];
                        custSecondaryCntNo = qryResultsCustomer.value[0]["telephone2"];
                        IsPriorityCare = qryResultsCustomer.value[0]["tc_prioritycare"];

                        switch (custRelationshipType) {
                            case 0: //Anonymous Customer
                                IsCustomerAnonymous = true;
                                custCategoryID = qryResultsCustomer.value[0]["_tc_customercategory_value"];
                                custCategoryName = qryResultsCustomer.value[0]["_tc_customercategory_value@OData.Community.Display.V1.FormattedValue"];
                                custCategoryValue = new Array();
                                custCategoryValue[0] = new Object();
                                custCategoryValue[0].id = custCategoryID; // GUID of the lookup id
                                custCategoryValue[0].name = custCategoryName; // Name of the lookup
                                custCategoryValue[0].entityType = "tc_customercategory"; //Entity Type of the lookup entity
                                qryselectCustCatgry = null;
                                SetSectionVisibility(executionContext, "tab_custsummary", "sec_custdetails_2", false);
                                SetAttrsDisability(executionContext, "tc_customername, emailaddress, tc_contactnumber", ", ", false);
                                break;
                            case 1: //Real-Time Customer
                                IsCustomerAnonymous = false;

                                if (custFirstName != null) {
                                    if (custLastName != null) {
                                        custFinalName = custFirstName + " " + custLastName;
                                    }
                                    else {
                                        custFinalName = custFirstName;
                                    }
                                }
                                if (caseCustName == null || (caseCustName != null && custFinalName != null && caseCustName.toLowerCase() != custFinalName.toLowerCase())) {
                                    SetAttributeValue(executionContext, "tc_customername", custFinalName);
                                    SetAttributeValue(executionContext, "tc_firstname", custFirstName);
                                    SetAttributeValue(executionContext, "tc_lastname", custLastName);
                                    SetAttrSubmitMode(executionContext, "tc_customername", "always");
                                }

                                if (caseEmail == null) {
                                    if (custPrimaryEmail != null) {
                                        custFinalEmail = custPrimaryEmail;
                                    }
                                    else if (custSecondaryEmail != null) {
                                        custFinalEmail = custSecondaryEmail;
                                    }
                                }
                                else {
                                    if (custPrimaryEmail != null) {
                                        if (custSecondaryEmail != null) {
                                            if (caseEmail.toLowerCase() != custPrimaryEmail.toLowerCase() && caseEmail.toLowerCase() != custSecondaryEmail.toLowerCase()) {
                                                custFinalEmail = custPrimaryEmail;
                                            }
                                        }
                                        else if (caseEmail.toLowerCase() != custPrimaryEmail.toLowerCase()) {
                                            custFinalEmail = custPrimaryEmail;
                                        }
                                    }
                                    else if (custSecondaryEmail != null && caseEmail.toLowerCase() != custSecondaryEmail.toLowerCase()) {
                                        custFinalEmail = custSecondaryEmail;
                                    }
                                }
                                if (custFinalEmail != null) {
                                    SetAttributeValue(executionContext, "emailaddress", custFinalEmail);
                                    SetAttrSubmitMode(executionContext, "emailaddress", "always");
                                }

                                if (caseContNo == null) {
                                    if (custPrimaryCntNo != null) {
                                        custFinalCntNo = custPrimaryCntNo;
                                    }
                                    else if (custSecondaryCntNo != null) {
                                        custFinalCntNo = custSecondaryCntNo;
                                    }
                                }
                                else {
                                    if (custPrimaryCntNo != null) {
                                        if (custSecondaryCntNo != null) {
                                            if (caseContNo.toLowerCase() != custPrimaryCntNo.toLowerCase() && caseContNo.toLowerCase() != custSecondaryCntNo.toLowerCase()) {
                                                custFinalCntNo = custPrimaryCntNo;
                                            }
                                        }
                                        else if (caseContNo.toLowerCase() != custPrimaryCntNo.toLowerCase()) {
                                            custFinalCntNo = custPrimaryCntNo;
                                        }
                                    }
                                    else if (custSecondaryCntNo != null && caseContNo.toLowerCase() != custSecondaryCntNo.toLowerCase()) {
                                        custFinalCntNo = custSecondaryCntNo;
                                    }
                                }
                                if (custFinalCntNo != null) {
                                    SetAttributeValue(executionContext, "tc_contactnumber", custFinalCntNo);
                                    AttrFireOnChange(executionContext, "tc_contactnumber");
                                }

                                qryselectCustCatgry = "tc_customercategories?$select=tc_customercategoryid,tc_name&$filter=tc_categoryid eq '2'or tc_categoryid eq '3'";
                                SetAttrsDisability(executionContext, "tc_customername, emailaddress, tc_contactnumber", ", ", true);
                                SetSectionVisibility(executionContext, "tab_custsummary", "sec_custdetails_2", true);
                                break;
                        }
                    }
                    break;
                case null:
                    SetSectionVisibility(executionContext, "tab_custsummary", "sec_custflagdetails", false); //Hide Customer Flag Details Section
                    break;
            }

            if (qryselectCustCatgry != null) {
                qryResultsCustCatgry = RetrieveMultipleRecords(qryselectCustCatgry, false, null);

                if (qryResultsCustCatgry != null && qryResultsCustCatgry.value != null && qryResultsCustCatgry.value.length > 0) {
                    custCategoryValue = new Array();
                    custIDVal = qryResultsCustCatgry.value[0]["tc_customercategoryid"];
                    custNameVal = qryResultsCustCatgry.value[0]["tc_name"];
                    custCategoryValue[0] = new Object();
                    custCategoryValue[0].id = custIDVal; // GUID of the lookup id
                    custCategoryValue[0].name = custNameVal; // Name of the lookup
                    custCategoryValue[0].entityType = "tc_customercategory"; //Entity Type of the lookup entity
                }
            }

            if (custCategoryValue != null && custCategoryValue.length > 0 && custCategoryValue[0].id != null) {
                if (caseCustCategryId == null || (caseCustCategryId != null && caseCustCategryId.toLowerCase() != custCategoryValue[0].id.toLowerCase())) {
                    SetAttributeValue(executionContext, "tc_customercategory", custCategoryValue);
                }
            }
        }
    }
}

function caseType_OnChange(executionContext, onChangeCheck) {
    var bpfActiveStageName = GetBPFActiveStageName(executionContext);

    if (bpfActiveStageName != null && (bpfActiveStageName == "initiate" || bpfActiveStageName == "أنشئ")) {
        if (onChangeCheck) {
            if (GetAttributeValue(executionContext, "tc_category") != null) {
                SetAttributeValue(executionContext, "tc_category", null);
            }
            SetAttrDisability(executionContext, "tc_category", false);

            if (GetAttributeValue(executionContext, "tc_casetype") == null) {
                SetAttrDisability(executionContext, "tc_category", true);
            }
        }
        else {
            previousCaseTypeName = GetLookupValName(executionContext, "tc_casetype");
            var objCaseType = GetAttributeValue(executionContext, "tc_casetype");

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
    }

    hideShowCaseInfo_CType(executionContext);
    caseCategory_OnChange(executionContext, onChangeCheck);
    hideShow_Sections_BPFActiveStage();
    // var caseTypeId = GetLookupValId(executionContext, "tc_casetype");
    // if(caseTypeId!=null){
    // ExecuteFilterAllCities(executionContext,caseTypeId);
    // }
}

function ExecuteFilterAllCities(executionContext, caseTypeId) {

    if (caseTypeId != null) {
        //check if we need to exclude all cities options
        var qryExcludeAllCities = "tc_casetypes(" + caseTypeId + ")?$select=tc_filterallcities";
        var qryResultExcludeAllCities = RetrieveMultipleRecords(qryExcludeAllCities, false, null);
        var ctrlCity = GetControl(executionContext, "tc_city");
        if (qryResultExcludeAllCities != null && qryResultExcludeAllCities != undefined) {
            var isExcludeAllCities = qryResultExcludeAllCities.tc_filterallcities;
                
            if (isExcludeAllCities) {
                //add preseach filter to cities
                ctrlCity.addPreSearch(excludeAllCityFilter);
            }


        }
    }
    else { return; }
}


function ExecuteFilterInternationalCities(executionContext, casesubcategoryId) {
    var ctrlCity = GetControl(executionContext, "tc_city");
    if (casesubcategoryId != null) {
        //check if we need to exclude all cities options
        var qryExcludeInternationalCities = "tc_casesubcategories(" + casesubcategoryId + ")?$select=tc_filterinternationalcity";
        var qryResultExcludeInternationalCities = RetrieveMultipleRecords(qryExcludeInternationalCities, false, null);
        if (qryResultExcludeInternationalCities != null && qryResultExcludeInternationalCities != undefined) {
            var isExcludeInternationalCities = qryResultExcludeInternationalCities.tc_filterinternationalcity;
               
            if (isExcludeInternationalCities) {
                //add preseach filter to cities
                ctrlCity.addPreSearch(excludeInternationalCityFilter);
            }


        }
    }
    else { return; }
}



/***************
// using the City Code ALLCTY to filter out the All city 
// Instead of the city name as the name field can be modified. Also this will help 
//apply a uniqueID rather than using the name
***************/
function excludeAllCityFilter() {
    //create a filter xml
    var customFilter = "<filter type='and'>" +
        "<condition attribute='tc_citycode' operator='ne' value='ALLCTY' />" +
        "</filter>";
    //add filter
    formExecuteContext.getFormContext().getControl("tc_city").addCustomFilter(customFilter, "cc_city");
}


/***************
// using the City Code INTCTY to filter out the International city 
// Instead of the city name as the name field can be modified. Also this will help 
//apply a uniqueID rather than using the name
***************/
function excludeInternationalCityFilter() {
    //create a filter xml
    var customFilter = "<filter type='and'>" +
        "<condition attribute='tc_citycode' operator='ne' value='INTCTY' />" +
        "<condition attribute='tc_citycode' operator='ne' value='ALLCTY' />" +
        "</filter>";
    //add filter
    formExecuteContext.getFormContext().getControl("tc_city").addCustomFilter(customFilter, "cc_city");
}

function caseCategory_OnChange(executionContext, onChangeCheck) {
    var objCategory = GetAttributeValue(executionContext, "tc_category");

    if (onChangeCheck) {
        if (GetAttributeValue(executionContext, "tc_subcategory") != null) {
            SetAttributeValue(executionContext, "tc_subcategory", null);
        }
        SetAttrDisability(executionContext, "tc_subcategory", false);

        if (GetAttributeValue(executionContext, "tc_category") == null) {
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
            }
            SetAttrDisability(executionContext, "tc_subcategory", true);
        }
    }
}

function caseSubCategory_OnChange(executionContext, onChangeCheck) {
    var objSubCategoryId = GetLookupValId(executionContext, "tc_subcategory");

    if (objSubCategoryId != null) {
        var qrySubCatgryDfConfigs = "tc_casesubcategories(" + objSubCategoryId + ")?$select=tc_defaultpriority,tc_istouristattractionmandatory,tc_istouristtradeprovidermandatory";
        var qryResultSubCatgry = RetrieveMultipleRecords(qrySubCatgryDfConfigs, false, null);

        if (qryResultSubCatgry != null && qryResultSubCatgry != undefined) {
            var isTouristProviderMandatory = null, isTouristAttractionMandatory = null, defaultPriority = null;

            isTouristProviderMandatory = qryResultSubCatgry.tc_istouristtradeprovidermandatory;
            isTouristAttractionMandatory = qryResultSubCatgry.tc_istouristattractionmandatory;
            defaultPriority = qryResultSubCatgry.tc_defaultpriority;

            if (onChangeCheck) {
                if (IsPriorityCare) {
                    SetAttributeValue(executionContext, "prioritycode", 1); //High
                }
                else {
                    if (defaultPriority != null) {
                        SetAttributeValue(executionContext, "prioritycode", defaultPriority);
                    }
                }
            }
            if (isTouristProviderMandatory) {
                SetAttrRequiredLevel(executionContext, "tc_serviceprovider", "required");
            }
            else {
                SetAttrRequiredLevel(executionContext, "tc_serviceprovider", "none");
            }
            if (isTouristAttractionMandatory) {
                SetAttrRequiredLevel(executionContext, "tc_touristattraction", "required");
            }
            else {
                SetAttrRequiredLevel(executionContext, "tc_touristattraction", "none");
            }
        }
        else {
            SetAttrRequiredLevel(executionContext, "tc_serviceprovider", "none");
            SetAttrRequiredLevel(executionContext, "tc_touristattraction", "none");
        }

        ExecuteFilterInternationalCities(executionContext, objSubCategoryId)
    }
}

function form_OnSave(executionContext) {
    formExecuteContext = executionContext;

    var formType = GetFormType(executionContext);
    if (formType == 1) { //If formType is 'Create'
        var objCreatedBy = GetAttributeValue(executionContext, "createdby");
        var dtCaseWorkedOn = new Date();

        if (objCreatedBy == null) {
            objCreatedBy = new Array();
            objCreatedBy[0] = new Object();
            objCreatedBy[0].id = GetContextUserId();
            objCreatedBy[0].name = GetContextUserName();
            objCreatedBy[0].entityType = "systemuser";
        }
        SetAttributeValue(executionContext, "tc_caseworkedby", objCreatedBy);
        SetAttributeValue(executionContext, "tc_caseworkedon", dtCaseWorkedOn);
        if (GetAttributeValue(executionContext, "tc_channelorigin") == null) {
            SetAttrRequiredLevel(executionContext, "tc_channelorigin", "required");
            executionContext.getFormContext().getControl("tc_channelorigin").setFocus()
            SetAttrCtrlNotification(executionContext, "tc_channelorigin", "Enter a Channel of Origin");
            executionContext.getEventArgs().preventDefault();
        }
        else {
            ClearAttrCtrlNotification(executionContext, "tc_channelorigin");
        }
    }

    hideShowCaseInfo_CType(executionContext);
    hideShow_Sections_BPFActiveStage();

    if (GetAttrDisability(executionContext, "prioritycode") != true) {
        SetAttrDisability(executionContext, "prioritycode", true);
    }


}

function hideShowCaseInfo_CType(executionContext) {
    var formType = GetFormType(executionContext);
    var caseTypeName = GetLookupValName(executionContext, "tc_casetype");

    if (formType == 1) {
        SetAttrsDisability(executionContext, "tc_casetype, tc_channelorigin", ", ", false);
    }
    else {
        if ((previousCaseTypeName == "out of scope" || previousCaseTypeName == "أخرى") && caseTypeName == null) {
            SetAttrDisability(executionContext, "tc_casetype", false);
        }
        else {
            SetAttrDisability(executionContext, "tc_casetype", true);
        }
        SetAttrDisability(executionContext, "tc_channelorigin", true);
        SetTabVisibility(executionContext, "tab_interactiondetails", true);
        SetTabVisibility(executionContext, "tab_approvalrqtsdetails", true);
    }

    if (caseTypeName != null) {
        SetBPFVisibility(formExecuteContext, true);
        SetAttrsVisibility(executionContext, "header_process_tc_casetype, tc_sendsuggestionto", ", ", false);
        SetAttrsDisability(executionContext, disableBPFCommonFields, ", ", true);

        switch (caseTypeName) {
            case "complaint": //Complaint
            case "شكوى":
                if (formType != 1) {
                    SetTabVisibility(executionContext, "tab_enhancedsladetails", true);
                    SetSectionVisibility(executionContext, "tab_casedetails", "sec_sladetails", true);
                }
                SetAttrsDisability(executionContext, disableBPFComplaintFields, ", ", true);
                if (GetCtrlAttrValue(executionContext, "header_process_tc_isknowledgebasearticleavailable") == null) {
                    SetCtrlAttrValue(executionContext, "header_process_tc_isknowledgebasearticleavailable", 0);
                    SetCtrlAttrSubmitMode(executionContext, "header_process_tc_isknowledgebasearticleavailable", "always");
                }
                SetAttrsVisibility(executionContext, "header_process_tc_isknowledgebasearticleavailable, header_process_tc_correspondencewith", ", ", false);
                SetAttrVisibility(executionContext, "header_process_tc_issubmitted", true);
                break;
            case "emergency": //Emergency
            case "حالة طوارئ": //Emergency
            case "enquiry": //Enquiry
            case "استفسار": //Enquiry
                SetAttrsVisibility(executionContext, "header_process_tc_issubmitted", ", ", false);
                SetAttrVisibility(executionContext, "header_process_tc_isknowledgebasearticleavailable", true);
                SetSectionVisibility(executionContext, "tab_casedetails", "sec_sladetails", false);
                SetTabVisibility(executionContext, "tab_enhancedsladetails", false);
                break;
            case "out of scope": //Out of Scope
            case "أخرى":
                if (GetCtrlAttrValue(executionContext, "header_process_tc_isknowledgebasearticleavailable") == null) {
                    SetCtrlAttrValue(executionContext, "header_process_tc_isknowledgebasearticleavailable", 0);
                    SetCtrlAttrSubmitMode(executionContext, "header_process_tc_isknowledgebasearticleavailable", "always");
                }
                SetAttrsVisibility(executionContext, "header_process_tc_issubmitted, header_process_tc_isknowledgebasearticleavailable", ", ", false);
                SetSectionVisibility(executionContext, "tab_casedetails", "sec_sladetails", false);
                SetTabVisibility(executionContext, "tab_enhancedsladetails", false);
                break;
            case "suggestion": //Suggestion
            case "اقتراحات":
                SetBPFVisibility(formExecuteContext, false);
                if (GetCtrlAttrValue(executionContext, "header_process_tc_isknowledgebasearticleavailable") == null) {
                    SetCtrlAttrValue(executionContext, "header_process_tc_isknowledgebasearticleavailable", 0);
                    SetCtrlAttrSubmitMode(executionContext, "header_process_tc_isknowledgebasearticleavailable", "always");
                }
                SetAttrsVisibility(executionContext, "header_process_tc_issubmitted, header_process_tc_isknowledgebasearticleavailable", ", ", false);
                SetAttrVisibility(executionContext, "tc_sendsuggestionto", true);
                SetSectionVisibility(executionContext, "tab_casedetails", "sec_sladetails", false);
                SetTabVisibility(executionContext, "tab_enhancedsladetails", false);
                break;
        }
    }
    else {
        SetBPFVisibility(formExecuteContext, false);
        SetSectionVisibility(executionContext, "tab_casedetails", "sec_sladetails", false);
        SetTabVisibility(executionContext, "tab_enhancedsladetails", false);
        SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", false);
        SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", false);
    }
    RefreshFormRibbon(executionContext, false);
}

function addCustFilterToCaseCategory(executionContext) {
    // add the event handler for PreSearch Event
    var ctrlCaseCategory = GetControl(executionContext, "tc_category");
    if (ctrlCaseCategory != null && !IsCustomerAnonymous) {
        ctrlCaseCategory.addPreSearch(addFilter);
    }
}

function addFilter() {
    var custCategoryId = GetLookupValId(formExecuteContext, "tc_customercategory");

    if (custCategoryId != null) {

        //create a filter xml
        var customFilter = "<filter type='and'>" +
            "<condition attribute='tc_customercategory' operator='eq' value='" + custCategoryId + "'/>" +
            "</filter>";

        //add filter
        formExecuteContext.getFormContext().getControl("tc_category").addCustomFilter(customFilter, "tc_casecategory");
    }
}

function hideShowCaseInfo_COrigin(executionContext) {
    var objCaseOriginId = null, originChannelType = null;
    objCaseOriginId = GetLookupValId(executionContext, "tc_channelorigin");

    if (objCaseOriginId != null) {
        var qryForChannelOrigins = null, resultsForChannelOrigins = null;
        qryForChannelOrigins = "tc_channelorigins(" + objCaseOriginId + ")?$select=tc_channeltype";
        resultsForChannelOrigins = RetrieveMultipleRecords(qryForChannelOrigins, false, null);

        if (resultsForChannelOrigins != null && resultsForChannelOrigins.tc_channeltype != null) {
            originChannelType = resultsForChannelOrigins.tc_channeltype;
        }
    }
    switch (originChannelType) {
        case 2: //Social Media
            SetAttrsVisibility(executionContext, "header_process_tc_isresolutioncommunicatedonsocialchannel, header_process_tc_isresolutioncommunicatedonsocialchannel_1, header_process_tc_isresolutioncommunicatedonsocialchannel_2, tc_isresolutioncommunicatedonsocialchannel, tc_isresolutioncommunicatedonsocialchannel1, tc_isresolutioncommunicatedonsocialchannel2", ", ", true);
            SetAttrsDisability(executionContext, "header_process_tc_isresolutioncommunicatedonsocialchannel, header_process_tc_isresolutioncommunicatedonsocialchannel_1, header_process_tc_isresolutioncommunicatedonsocialchannel_2, tc_isresolutioncommunicatedonsocialchannel, tc_isresolutioncommunicatedonsocialchannel1, tc_isresolutioncommunicatedonsocialchannel2", ", ", false);

            var objSMHVal = null, smhPstDaTmVal = null, smhPstTxt = null
            objSMHVal = GetAttributeValue(executionContext, "tc_socialmediahandle");
            smhPstDaTmVal = GetAttributeValue(executionContext, "tc_socialmediapostdatetime");
            smhPstTxt = GetAttributeValue(executionContext, "tc_socialmediaposttext");

            objSMHVal != null ? SetAttrDisability(executionContext, "tc_socialmediahandle", true) : SetAttrDisability(executionContext, "tc_socialmediahandle", false);
            smhPstDaTmVal != null ? SetAttrDisability(executionContext, "tc_socialmediapostdatetime", true) : SetAttrDisability(executionContext, "tc_socialmediapostdatetime", false);
            smhPstTxt != null ? SetAttrDisability(executionContext, "tc_socialmediaposttext", true) : SetAttrDisability(executionContext, "tc_socialmediaposttext", false);
            break;
        default:
            SetAttrsDisability(executionContext, "tc_socialmediahandle, tc_socialmediapostdatetime, tc_socialmediaposttext, header_process_tc_isresolutioncommunicatedonsocialchannel, header_process_tc_isresolutioncommunicatedonsocialchannel_1, header_process_tc_isresolutioncommunicatedonsocialchannel_2, tc_isresolutioncommunicatedonsocialchannel, tc_isresolutioncommunicatedonsocialchannel1, tc_isresolutioncommunicatedonsocialchannel2", ", ", true);
            SetAttrsVisibility(executionContext, "header_process_tc_isresolutioncommunicatedonsocialchannel, header_process_tc_isresolutioncommunicatedonsocialchannel_1, header_process_tc_isresolutioncommunicatedonsocialchannel_2, tc_isresolutioncommunicatedonsocialchannel, tc_isresolutioncommunicatedonsocialchannel1, tc_isresolutioncommunicatedonsocialchannel2", ", ", false);
            break;
    }
    if (GetAttributeValue(executionContext, "tc_channelorigin") != null) {
        ClearAttrCtrlNotification(executionContext, "tc_channelorigin");
    }
    else {

    }
}

function showRequestTypeMsgs(executionContext, rqtTypeVal) {
    var msgNotification = "", msgUniqueId = "approvalnotification", requestType = null;
    var userLCID = GetContextUserLCID();
    ClearFormNotification(executionContext, msgUniqueId);

    if (rqtTypeVal == null || rqtTypeVal == undefined) {
        var targetCaseId = GetTargetRecordId(executionContext);
        if (targetCaseId != null) {
            var qryForRqtTypes = null, qryResultsRqtTypes = null;
            qryForRqtTypes = "tc_caseapprovals?$select=tc_requesttype&$filter=_regardingobjectid_value eq " + targetCaseId + " and  (statecode eq 0 or  statecode eq 3)";
            qryResultsRqtTypes = RetrieveMultipleRecords(qryForRqtTypes, false, null);

            if (qryResultsRqtTypes != null && qryResultsRqtTypes.value != null && qryResultsRqtTypes.value.length > 0) {
                requestType = qryResultsRqtTypes.value[0]["tc_requesttype"];
            }
        }
    }
    else {
        requestType = rqtTypeVal;
    }

    if (requestType != null) {
        switch (requestType) {
            case 1://Pause SLA
                switch (userLCID) {
                    case 1025: //Arabic
                        msgNotification = PauseSLArequestTypemsgNotificationAR;
                        break;
                    case 1033: //English
                    default:
                        msgNotification = PauseSLArequestTypemsgNotificationEN;
                        break;
                }

                break;
            case 2://Change Categorization
                switch (userLCID) {
                    case 1025: //Arabic
                        msgNotification = ChangeCategorizationArequestTypemsgNotificationAR;
                        break;
                    case 1033: //English
                    default:
                        msgNotification = ChangeCategorizationArequestTypemsgNotificationEN;
                        break;
                }

                break;
            case 3://Cancel Case
                switch (userLCID) {
                    case 1025: //Arabic
                        msgNotification = CancelCaserequestTypemsgNotificationAR;
                        break;
                    case 1033: //English
                    default:
                        msgNotification = CancelCaserequestTypemsgNotificationEN;
                        break;
                }

                break;
            case 4://Change Priority
                switch (userLCID) {
                    case 1025: //Arabic
                        msgNotification = ChangePriorityrequestTypemsgNotificationAR;
                        break;
                    case 1033: //English
                    default:
                        msgNotification = ChangePriorityrequestTypemsgNotificationEN;
                        break;
                }

                break;
            case 5://Moment of Delight
                switch (userLCID) {
                    case 1025: //Arabic
                        msgNotification = MomentofDelightrequestTypemsgNotificationAR;
                        break;
                    case 1033: //English
                    default:
                        msgNotification = MomentofDelightrequestTypemsgNotificationEN;
                        break;
                }

                break;
        }
        SetFormNotification(executionContext, msgNotification, "INFO", msgUniqueId);
    }

    RefreshFormRibbon(executionContext, false);
}

function hideShow_Sections_BPFActiveStage() {
    var bpfActiveStageName = GetBPFActiveStageName(formExecuteContext);
    var caseTypeName = GetLookupValName(formExecuteContext, "tc_casetype");
    var pendingwithVal = GetAttributeValue(formExecuteContext, "tc_pendingwith");
    var categoryName = GetLookupValName(formExecuteContext, "tc_category");
    var roleCheckForUpdateAccess = null;
    var formType = GetFormType(formExecuteContext);

    if (bpfActiveStageName != null) {
        switch (bpfActiveStageName) {
            case "initiate":
            case "أنشئ":
                if (caseTypeName == "out of scope" || caseTypeName == "أخرى") {
                    SetAttrDisability(formExecuteContext, "tc_casetype", false);

                    if (categoryName == "(os) not relevant case" || categoryName == "حالة غير ذات صلة") {
                        if (GetCtrlAttrValue(formExecuteContext, "header_process_tc_isreportedtomanagement") == null) {
                            SetCtrlAttrValue(formExecuteContext, "header_process_tc_isreportedtomanagement", 0);
                            SetCtrlAttrSubmitMode(formExecuteContext, "header_process_tc_isreportedtomanagement", "always");
                        }
                        SetAttrsVisibility(formExecuteContext, fieldsRelatedToOS, ", ", false);
                    }
                    else if (categoryName == "(os) abuse" || categoryName == "إساءة") {
                        SetAttrsVisibility(formExecuteContext, fieldsRelatedToOS, ", ", true);
                    }
                }
                if (formType == 2) { //Update
                    SetAttrsDisability(formExecuteContext, "tc_category, tc_subcategory", ", ", false);
                }
                SetAttrDisability(formExecuteContext, "customerid", false);
                SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", false);
                SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", false);
                break;
            case "review":
            case "مراجعة":
                SetAttrsDisability(formExecuteContext, "customerid, tc_category, tc_subcategory", ", ", true);
                if (caseTypeName == "complaint" || caseTypeName == "شكوى") {
                    if (pendingwithVal == 1) {
                        SetAttrsVisibility(formExecuteContext, "header_process_tc_correspondencewith, tc_correspondencewith", ", ", true);
                    }
                    else {
                        SetAttrsVisibility(formExecuteContext, "header_process_tc_correspondencewith, tc_correspondencewith", ", ", false);
                    }
                    SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", false);
                    SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", true);
                    SetSectionVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", "sec_cc_closuredetails", false);
                    SetSectionVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", "sec_cc_reviewdetails", true);
                    resolutionResponseReceived_OnChange(formExecuteContext);
                }
                else {
                    SetAttrsRequiredLevel(formExecuteContext, "header_process_tc_correspondencewith, tc_correspondencewith", ", ", "none");
                    SetAttrsVisibility(formExecuteContext, "header_process_tc_correspondencewith, tc_correspondencewith", ", ", false);
                }
                break;
            case "close":
            case "إغلاق":
                SetAttrsDisability(formExecuteContext, "customerid, tc_category, tc_subcategory", ", ", true);
                if (caseTypeName != null && caseTypeName != undefined) {
                    switch (caseTypeName) {
                        case "complaint": //Complaint
                        case "شكوى": //Complaint                            
                            SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", false);
                            SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", true);
                            SetSectionVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", "sec_cc_closuredetails", true);
                            SetSectionVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", "sec_cc_reviewdetails", true);
                            if (pendingwithVal == 1) {
                                SetAttrsVisibility(formExecuteContext, "header_process_tc_correspondencewith, tc_correspondencewith", ", ", true);
                            }
                            else {
                                SetAttrsVisibility(formExecuteContext, "header_process_tc_correspondencewith, tc_correspondencewith", ", ", false);
                            }
                            resolutionResponseReceived_OnChange(formExecuteContext);
                            communicationSentToCust_OnChange(formExecuteContext);
                            if (formType == 2) { //Update
                                roleCheckForUpdateAccess = checkForCaseFunctionalities(roleAgtSupMng);
                                if (!roleCheckForUpdateAccess) {
                                    DisableAllAttrCtrls(formExecuteContext);
                                }
                            }
                            break;
                        case "emergency": //Emergency
                        case "حالة طوارئ": //Emergency
                        case "enquiry": //Enquiry
                        case "استفسار": //Enquiry
                        case "suggestion": //Suggestion
                        case "اقتراحات": //Suggestion    
                            SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", false);
                            SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", true);
                            SetSectionVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", "sec_os_closuredetails", false);
                            SetSectionVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", "sec_em_en_sg_closuredetails", true);
                            communicationSentToCust_OnChange(formExecuteContext);
                            break;
                        case "out of scope": //Out of Scope
                        case "أخرى": //Out of Scope
                            SetAttrDisability(formExecuteContext, "tc_casetype", true);
                            SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", false);
                            SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", true);
                            SetSectionVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", "sec_em_en_sg_closuredetails", false);
                            SetSectionVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", "sec_os_closuredetails", true);

                            if (categoryName == "(os) not relevant case" || categoryName == "حالة غير ذات صلة") {
                                if (GetCtrlAttrValue(formExecuteContext, "header_process_tc_isreportedtomanagement") == null) {
                                    SetCtrlAttrValue(formExecuteContext, "header_process_tc_isreportedtomanagement", 0);
                                    SetCtrlAttrSubmitMode(formExecuteContext, "header_process_tc_isreportedtomanagement", "always");
                                }
                                SetAttrsVisibility(formExecuteContext, fieldsRelatedToOS, ", ", false);
                            }
                            else if (categoryName == "(os) abuse" || categoryName == "إساءة") {
                                SetAttrsVisibility(formExecuteContext, fieldsRelatedToOS, ", ", true);
                            }
                            validOSCase_OnChange(formExecuteContext);
                            if (formType == 2) { //Update
                                roleCheckForUpdateAccess = checkForCaseFunctionalities(roleSupMng);
                                if (!roleCheckForUpdateAccess) {
                                    DisableAllAttrCtrls(formExecuteContext);
                                }
                            }
                            break;
                    }
                }
                break;
        }
    }
    else {
        SetAttrsDisability(formExecuteContext, "customerid, tc_category, tc_subcategory, header_process_customerid, header_process_tc_category, header_process_tc_subcategory", ", ", true);
        SetTabVisibility(formExecuteContext, "tab_cc_reviewclosuredetails", false);
        SetTabVisibility(formExecuteContext, "tab_em_en_sg_closuredetails", false);
    }

    RefreshFormRibbon(formExecuteContext, false);
    hideShowCaseInfo_COrigin(formExecuteContext);
}

function city_OnChange(executionContext) {
    var objCityId = GetLookupValId(executionContext, "tc_city");

    if (objCityId != null) {
        var qryRetrieveRegion = "cc_cities?$select=cc_cityid,_tc_region_value&$filter=cc_cityid eq " + objCityId;
        var qryResults = RetrieveMultipleRecords(qryRetrieveRegion, false, null);

        if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
            var regionValue = new Array();
            regionValue[0] = new Object();
            regionValue[0].id = qryResults.value[0]["_tc_region_value"];
            regionValue[0].name = qryResults.value[0]["_tc_region_value@OData.Community.Display.V1.FormattedValue"];
            regionValue[0].entityType = qryResults.value[0]["_tc_region_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
            SetAttributeValue(executionContext, "tc_region", regionValue);
            SetAttrSubmitMode(executionContext, "tc_region", "always");
        }
    }
    else {
        if (GetAttributeValue(executionContext, "tc_region") != null) {
            SetAttributeValue(executionContext, "tc_region", null);
            SetAttrSubmitMode(executionContext, "tc_region", "always");
        }
    }
}

function resolutionResponseReceived_OnChange(executionContext) {
    var rrrVal = GetAttributeValue(executionContext, "tc_isreviewerresponsereceived");

    if (rrrVal != null && rrrVal != undefined) {
        if (rrrVal) {
            SetAttrsRequiredLevel(executionContext, "tc_reviewerstatus, tc_reviewercomments", ", ", "required");
        }
        else {
            SetAttrsRequiredLevel(executionContext, "tc_reviewerstatus, tc_reviewercomments", ", ", "none");
        }
    }
    else {
        SetAttrsRequiredLevel(executionContext, "tc_reviewerstatus, tc_reviewercomments", ", ", "none");
    }
}

function communicationSentToCust_OnChange(executionContext) {
    var cscVal = GetAttributeValue(executionContext, "tc_communicationsenttocustomer");
    var caseTypeName = GetLookupValName(formExecuteContext, "tc_casetype");

    if (cscVal != null && cscVal != undefined) {
        if (cscVal) {
            SetAttrsRequiredLevel(executionContext, "tc_resolutionreason, tc_resolutioncomments", ", ", "required");
            if (caseTypeName == "complaint" || caseTypeName == "شكوى") {
                SetAttrVisibility(formExecuteContext, "tc_customersatisfiedwithresolution", true);
                SetAttrRequiredLevel(executionContext, "tc_customersatisfiedwithresolution", "required");
            }
            else {
                SetAttrRequiredLevel(executionContext, "tc_customersatisfiedwithresolution", "none");
                SetAttrVisibility(formExecuteContext, "tc_customersatisfiedwithresolution", false);
            }
        }
        else {
            SetAttrVisibility(formExecuteContext, "tc_customersatisfiedwithresolution", false);
            SetAttrsRequiredLevel(executionContext, "tc_customersatisfiedwithresolution, tc_resolutionreason, tc_resolutioncomments", ", ", "none");
        }
    }
    else {
        SetAttrVisibility(formExecuteContext, "tc_customersatisfiedwithresolution", false);
        SetAttrsRequiredLevel(executionContext, "tc_customersatisfiedwithresolution, tc_resolutionreason, tc_resolutioncomments", ", ", "none");
    }
}

function validOSCase_OnChange(executionContext) {
    var vosCaseVal = GetAttributeValue(executionContext, "tc_isvalidoutofscopecase");

    if (vosCaseVal != null && vosCaseVal != undefined) {
        if (vosCaseVal) {
            SetAttrsRequiredLevel(executionContext, "tc_resolutionreason, tc_resolutioncomments", ", ", "required");
        }
        else {
            SetAttrsRequiredLevel(executionContext, "tc_isreportedtomanagement, tc_resolutionreason, tc_resolutioncomments", ", ", "none");
        }
    }
    else {
        SetAttrsRequiredLevel(executionContext, "tc_isreportedtomanagement, tc_resolutionreason, tc_resolutioncomments", ", ", "none");
    }
}

function reportMgmtCase_OnChange(executionContext) {
    var reportMgmtCaseVal = GetAttributeValue(executionContext, "tc_isreportedtomanagement");
    var categoryName = GetLookupValName(formExecuteContext, "tc_category");

    if (categoryName == "(os) abuse" || categoryName == "إساءة") {
        if (reportMgmtCaseVal != 1) { //No (or) Null
            var attrNotification = null;
            switch (userLCID) {
                case 1025: //Arabic
                    attrNotification = osabusemsgNotificationAR;
                    break;
                case 1033: //English
                default:
                    attrNotification = osabusemsgNotificationEN;
                    break;
            }
            SetAttrCtrlNotification(executionContext, "tc_isreportedtomanagement", attrNotification);
        }
        else if (reportMgmtCaseVal == 1) { //Yes
            ClearAttrCtrlNotification(executionContext, "tc_isreportedtomanagement");
        }
    }
    else {
        ClearAttrCtrlNotification(executionContext, "tc_isreportedtomanagement");
    }
}

function mgmtRecommendedAction_OnChange(executionContext) {
    var mgmtRAVal = GetAttributeValue(executionContext, "tc_ismanagementrecommendedaction");

    if (mgmtRAVal == 1) { //Yes
        SetAttrRequiredLevel(executionContext, "tc_recommendedactions", "required");
    }
    else { //No (or) Null
        SetAttrRequiredLevel(executionContext, "tc_recommendedactions", "none");
    }
}

function formRibbonRefresh_OnChange(executionContext) {
    RefreshFormRibbon(executionContext, false);
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

function hideShowAdminTabAndSections() {
    if (IsTargetUserSystemAdmin) {
        SetTabVisibility(formExecuteContext, "tab_administrationdetails", true);
    }
    else {
        SetTabVisibility(formExecuteContext, "tab_administrationdetails", false);
    }
}

function ChangeLookupLanguages(executionContext) {
    //tc_casetype
    getArabicLookups(executionContext, "tc_casetypes", "tc_casetype", "tc_arabicname");
    //tc_category
    getArabicLookups(executionContext, "tc_casecategories", "tc_category", "tc_arabicname");
    //tc_subcategory
    getArabicLookups(executionContext, "tc_casesubcategories", "tc_subcategory", "tc_arabicname");
    //tc_channelorigin
    getArabicLookups(executionContext, "tc_channelorigins", "tc_channelorigin", "tc_arabicname");
    //tc_touristattraction
    getArabicLookups(executionContext, "tc_touristattractions", "tc_touristattraction", "tc_arabicname");
    //tc_serviceprovider
    getArabicLookups(executionContext, "accounts", "tc_serviceprovider", "tc_facilitynamearabic");
    //tc_city
    getArabicLookups(executionContext, "cc_cities", "tc_city", "cc_arabicname");
    //tc_region
    getArabicLookups(executionContext, "cc_regions", "tc_region", "tc_arabicname");
    //tc_customercategory
    getArabicLookups(executionContext, "tc_customercategories", "tc_customercategory", "tc_arabicname");
    //tc_reviewerstatus
    getArabicLookups(executionContext, "tc_casereasons", "tc_reviewerstatus", "tc_arabicname");
    //tc_reviewerstatus
    getArabicLookups(executionContext, "tc_casereasons", "tc_reviewerstatus", "tc_arabicname");
    //tc_resolutionreason
    getArabicLookups(executionContext, "tc_casereasons", "tc_resolutionreason", "tc_arabicname");
    //tc_cancellationreason
    getArabicLookups(executionContext, "tc_casereasons", "tc_cancellationreason", "tc_arabicname");
    //tc_pauseslareason
    getArabicLookups(executionContext, "tc_casereasons", "tc_pauseslareason", "tc_arabicname");
    //header_process_tc_category
    getArabicLookups(executionContext, "tc_casecategories", "header_process_tc_category", "tc_arabicname");
    //header_process_tc_subcategory
    getArabicLookups(executionContext, "tc_casesubcategories", "header_process_tc_subcategory", "tc_arabicname");
}

function checkForCaseFunctionalities(roleCheckParam) {
    var roleCheckFlag = false, configParamVal = null, currentUserRoles = null;

    currentUserRoles = RetrieveLoggedInD365UserSecurityRoles();

    switch (roleCheckParam) {
        case "AgtSupMngExtAgtExtBM":
            configParamVal = GetConfigParameterValue(roleCheckForPickReleaseFunctionality, false, null);
            break;
        case "AgtSupMng":
            configParamVal = GetConfigParameterValue(roleCheckForOtherFunctionalities, false, null);
            break;
        case "SupMng":
            configParamVal = GetConfigParameterValue(roleCheckForSupervisorAndManager, false, null);
            break;
        case "SysAdmSysCus":
            configParamVal = GetConfigParameterValue(roleCheckForSystemAdminSytemCustomizer, false, null);
            break;
    }

    if (configParamVal != null) {
        var configRoleNames = null;

        if (configParamVal.indexOf(",") > -1) {
            configRoleNames = configParamVal.split(',');
        }
        else {
            configRoleNames = configParamVal;
        }

        if (currentUserRoles != null && currentUserRoles.length > 0) {
            for (var i = 0; i < currentUserRoles.length; i++) {
                if (configRoleNames.includes(currentUserRoles[i].name.toLowerCase())) {
                    roleCheckFlag = true;
                    break;
                }
            }
        }
    }

    return roleCheckFlag;
}