function form_OnLoad(executionContext) {
    ChangeLookupLanguages(executionContext);
}

function city_OnChange(executionContext) {
    var objCityId = GetLookupValId(executionContext, "tc_city");

    if (objCityId != null) {
        var qryRetrieveRegion = "cc_cities?$select=cc_cityid,_tc_region_value&$filter=cc_cityid eq " + objCityId;
        var qryResults = RetrieveMultipleRecords(qryRetrieveRegion, false, null);

        if (qryResults != null && qryResults.value != null && qryResults.value.length > 0) {
            var regionLookupVal = new Array();
            regionLookupVal[0] = new Object();
            regionLookupVal[0].id = qryResults.value[0]["_tc_region_value"];
            regionLookupVal[0].name = qryResults.value[0]["_tc_region_value@OData.Community.Display.V1.FormattedValue"];
            regionLookupVal[0].entityType = qryResults.value[0]["_tc_region_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
            SetAttributeValue(executionContext, "tc_region", regionLookupVal);
            SetAttrSubmitMode(executionContext, "tc_region", "always");
            AttrFireOnChange(executionContext, "tc_region");
        }
    }
    else {
        if (GetAttributeValue(executionContext, "tc_region") != null) {
            SetAttributeValue(executionContext, "tc_region", null);
            SetAttrSubmitMode(executionContext, "tc_region", "always");
            AttrFireOnChange(executionContext, "tc_region");
        }
    }
}

function ChangeLookupLanguages(executionContext) {
    //tc_city
    getArabicLookups(executionContext, "cc_cities", "tc_city", "cc_arabicname");
    //tc_region
    getArabicLookups(executionContext, "cc_regions", "tc_region", "tc_arabicname");
}