var GlobalformContext;

function onload(executionContext) {


    var formContext = executionContext.getFormContext();
    GlobalformContext = executionContext.getFormContext();
    formContext.data.process.addOnStageChange(SetBusinessRequired);


    var ContactCountry = formContext.getAttribute("cc_contactcountry").getValue();
    var AccountCountry = formContext.getAttribute("cc_accountcountry").getValue();
    var Region = formContext.getAttribute("cc_region").getValue() != null ? formContext.getAttribute("cc_region").getValue()[0].id : null;
    var SubRegion = formContext.getAttribute("cc_subregion").getValue() != null ? formContext.getAttribute("cc_subregion").getValue()[0].id : null;

    if (Region == null) {
        formContext.getControl("cc_subregion").setDisabled(true);
        formContext.getControl("cc_accountcountry").setDisabled(true);
        formContext.getControl("cc_accountcity").setDisabled(true);

    }

    if (ContactCountry == null)
        formContext.getControl("cc_phone").setDisabled(true);

    if (AccountCountry == null)
        formContext.getControl("telephone1").setDisabled(true);

    SetBusinessRequired(formContext);

    // address ==> region
    //account (customer) ==> sub region

    if (formContext.getControl("header_process_cc_region") != null && formContext.getControl("header_process_cc_region") != undefined) {

        formContext.getControl("header_process_cc_subregion").addPreSearch(function () {
            addSubRegionLookupFilter();
        });


        formContext.getControl("header_process_cc_accountcountry").addPreSearch(function () {
            addCountryLookupFilter();
        });
    }

    if (formContext.getControl("header_process_cc_accountcountry") != null && formContext.getControl("header_process_cc_accountcountry") != undefined) {

        formContext.getControl("header_process_cc_accountcity").addPreSearch(function () {
            addCityLookupFilter();
        });
    }
    CheckleadRole();

}

function addSubRegionLookupFilter() {
    var functionName = "addSubRegionLookupFilter";
    try {

        var region = Xrm.Page.getControl("header_process_cc_region").getAttribute().getValue()[0].id;
        // region = region.replace("&", "&amp;");
        region = region.replace("{", "").replace("}", "");

        if (region != null && region != undefined) {
            fetchXml = "<filter><condition attribute='cc_region' operator='eq' value='" + region + "'/></filter>";
            Xrm.Page.getControl("header_process_cc_subregion").addCustomFilter(fetchXml);
        }

    } catch (e) {
        throw new Error(e.message);
    }


}


function addCityLookupFilter() {
    var functionName = "addCityLookupFilter";
    try {

        var country = Xrm.Page.getControl("header_process_cc_accountcountry").getAttribute().getValue()[0].id;
        // region = region.replace("&", "&amp;");
        country = country.replace("{", "").replace("}", "");

        if (country != null && country != undefined) {
            fetchXml = "<filter><condition attribute='cc_country' operator='eq' value='" + country + "'/></filter>";
            Xrm.Page.getControl("header_process_cc_accountcity").addCustomFilter(fetchXml);
        }

    } catch (e) {
        throw new Error(e.message);
    }

}


function addCountryLookupFilter() {
    var functionName = "addCountryLookupFilter";
    try {

        var region = Xrm.Page.getControl("header_process_cc_region").getAttribute().getValue()[0].id;
        // region = region.replace("&", "&amp;");
        region = region.replace("{", "").replace("}", "");

        if (region != null && region != undefined) {
            fetchXml = "<filter><condition attribute='cc_region' operator='eq' value='" + region + "'/></filter>";
            Xrm.Page.getControl("header_process_cc_accountcountry").addCustomFilter(fetchXml);
        }

    } catch (e) {
        throw new Error(e.message);
    }

}
function SetBusinessRequired(formContext) {


    var stageID = Xrm.Page.data.process.getActiveStage().getName() != null ? Xrm.Page.data.process.getActiveStage().getName() : null;
    if (stageID == "Review" || stageID == "Qualify" && stageID != null) {

        //  var context = formContext.getFormContext();
        Xrm.Page.getAttribute("cc_typeofaccount").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_marketsegment").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_businesspotential").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_operatingksa").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_jobtitle").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_role").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_strengthofrelationship").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_contactcountry").setRequiredLevel("required");
        Xrm.Page.getAttribute("cc_contactcity").setRequiredLevel("required");
        Xrm.Page.getAttribute("telephone1").setRequiredLevel("required");
        Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("required");

    }

}

function RegionChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var Region = formContext.getAttribute("cc_region").getValue() != null ? formContext.getAttribute("cc_region").getValue()[0].id : null;
    if (Region != null) {

        formContext.getControl("cc_subregion").setDisabled(false);
        formContext.getControl("cc_accountcountry").setDisabled(false);
        formContext.getControl("cc_accountcity").setDisabled(false);
    }
    else {
        formContext.getControl("cc_subregion").setDisabled(true);
        formContext.getControl("cc_accountcountry").setDisabled(true);
        formContext.getControl("cc_accountcity").setDisabled(true);
    }

}


//onChange of the country field, get the country phone code and prefix it on the phone field
function ChangeCountry(executionContext, IsAccount) {

    var formContext = executionContext.getFormContext();
    if (formContext != null) {
        var _accountcountryid = formContext.getAttribute("cc_accountcountry").getValue() != null ? formContext.getAttribute("cc_accountcountry").getValue()[0].id : null;
        var _contactcountryid = formContext.getAttribute("cc_contactcountry").getValue() != null ? formContext.getAttribute("cc_contactcountry").getValue()[0].id : null;

        if (_accountcountryid != null && IsAccount == true) {
            _accountcountryid = _accountcountryid.replace("{", "").replace("}", "");
            setPhonePrefix(_accountcountryid, formContext, true, false);
            formContext.getControl("cc_phone").setDisabled(false);
        }

        if (_contactcountryid != null && IsAccount == false) {
            _contactcountryid = _contactcountryid.replace("{", "").replace("}", "");
            setPhonePrefix(_contactcountryid, formContext, false, true);
            formContext.getControl("telephone1").setDisabled(false);

        }

    }
}

function setPhonePrefix(countryid, formContext, isAccount, isContact) {
    //get the country that has been selected
    var globalContext = Xrm.Utility.getGlobalContext();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: globalContext.getClientUrl() + "/api/data/v9.1/cc_countries(" + countryid + ")?$select=cc_countrycode",
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: true,
        success: function (data, textStatus, xhr) {
            var result = data;
            var _cc_country_code = result["cc_countrycode"];
            var telValue = formContext.getAttribute("telephone1").getValue() != null ? formContext.getAttribute("telephone1").getValue() : null;
            if (isContact == true && telValue == null) {
                formContext.getAttribute("telephone1").setValue(_cc_country_code);
            }
            else if (isAccount == true) {
                formContext.getAttribute("cc_phone").setValue(_cc_country_code);
            }

        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });


}

function onPhoneNumberChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var telValue = formContext.getAttribute("telephone1").getValue() != null ? formContext.getAttribute("telephone1").getValue() : null;
    var phoneValue = formContext.getAttribute("cc_phone").getValue() != null ? formContext.getAttribute("cc_phone").getValue() : null;
    if (telValue != null && telValue.length > 5)
        formContext.getControl("telephone1").clearNotification();
    if (phoneValue != null && phoneValue.length > 5)
        formContext.getControl("cc_phone").clearNotification();
}



function onSave(executionContext) {

    var formContext = executionContext.getFormContext();
    //check if the mobile phone filed only contains the code and not the actual number

    var mobile = formContext.getAttribute("telephone1").getValue() != null ? formContext.getAttribute("telephone1").getValue() : null;
    var phone = formContext.getAttribute("cc_phone").getValue() != null ? formContext.getAttribute("cc_phone").getValue() : null;
    if (mobile != null && mobile.length <= 5) {
        var message = "The Mobile Number is incorrect. Please enter a valid number."
        formContext.getControl("telephone1").setNotification("Please Enter a Valid Phone Number");
    }
    else
        formContext.getControl("telephone1").clearNotification();


    if (phone != null && phone.length <= 5) {
        var message = "The Mobile Number is incorrect. Please enter a valid number."
        formContext.getControl("cc_phone").setNotification("Please Enter a Valid Phone Number");
    }
    else
        formContext.getControl("cc_phone").clearNotification();

    SetBusinessRequired(formContext);
}
function ClearCity(executionContext) {

    var formContext = executionContext.getFormContext();
    var city = formContext.getAttribute("cc_city").getValue();
    if (city != null) {
        formContext.getAttribute("cc_city").setValue(null);
    }

}

function onChangeQualificationInfo(executionContext) {

    var formContext = executionContext.getFormContext();
    var QualifyInfo = formContext.getControl("cc_enoughinformationprovidedtoqualifylead").getValue();
    if (QualifyInfo == "No")
        parent.$("[data-lp-id='MscrmControls.Containers.ProcessStageControl|MscrmControls.Containers.ProcessStageControl|lead|finishButtonContainer']").disabled = "disabled";
    parent.$("[data-lp-id='MscrmControls.Containers.ProcessStageControl|MscrmControls.Containers.ProcessStageControl|lead|finishButtonContainer']").disabled = false;

}

function getLeadApprovalUser() {
    var globalContext = Xrm.Utility.getGlobalContext();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: globalContext.getClientUrl() + "/api/data/v9.1/cc_configurationparameterses?$select=cc_value&$filter=cc_key eq 'LeadApproval'",
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: true,
        success: function (data, textStatus, xhr) {
            var results = data;
            var cc_value = results.value[0].cc_value.spilt(',');
            var roles = Xrm.Utility.getGlobalContext().userSettings.roles;
            cc_value.forEach(function (item) {

            });

            roles.forEach(function (item) {
                if (item.name.toLowerCase() == cc_value.toLowerCase()) {
                    hasRole = true;
                    alert(hasRole);
                }
            });
            return hasRole();
        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });

}


function getTypeofAccount() {

    GlobalformContext.getAttribute('cc_typeofaccountvalues').setValue('');
    var text = '';
    var attvalue = GlobalformContext.getAttribute('cc_typeofaccount').getValue();
    if (attvalue != null) {
        var attribute = GlobalformContext.getAttribute('cc_typeofaccount');
        var selectedValue = attribute.getValue().toString();
        var options = attribute.getSelectedOption();
        for (var i = 0; i < options.length; i++) {
            var option = options[i];
            if (i > 0) {
                text += ',';
            }
            text += option.text;
            if (selectedValue === option.value.toString()) {
                text += ',';
            }
        }
        GlobalformContext.getAttribute('cc_typeofaccountvalues').setValue(text);
    }
}
function getBusinessPotential() {

    GlobalformContext.getAttribute('cc_businesspotentialvalues').setValue('');
    var text = '';
    var attvalue = GlobalformContext.getAttribute('cc_typeofaccount').getValue();
    if (attvalue != null) {
        var attribute = GlobalformContext.getAttribute('cc_businesspotential');
        var selectedValue = attribute.getValue().toString();
        var options = attribute.getSelectedOption();
        for (var i = 0; i < options.length; i++) {
            var option = options[i];
            if (i > 0) {
                text += ',';
            }
            text += option.text;
            if (selectedValue === option.value.toString()) {
                text += ',';
            }
        }
        GlobalformContext.getAttribute('cc_businesspotentialvalues').setValue(text);
    }
}
function CheckleadRole() {
    var currentUserRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
    var hasrole = false;
    var ConfigRoleName = GetRoleName();
    if (ConfigRoleName != null) {
        var configrolearry = ConfigRoleName;
        var Roles = configrolearry.split(',');
        for (var i = 0; i < Object.keys(currentUserRoles._collection).length; i++) {
            //var userRoleName = currentUserRoles[i].name;
            if (Roles.includes(currentUserRoles.getByIndex(i).name.toLowerCase())) {
                hasrole = true;
            }
        }
    }
    return hasrole;
}

//Get Rolename based on RoleId
function GetRoleName() {
    var roleName = null;
    var globalContext = Xrm.Utility.getGlobalContext();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: globalContext.getClientUrl() + "/api/data/v9.1/cc_configurationparameterses?$select=cc_value&$filter=cc_key eq 'LeadApproval'",
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: false,
        success: function (data, textStatus, xhr) {
            var results = data;
            roleName = results.value[0]["cc_value"].toLowerCase();

        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });

    return roleName;
}