function onload(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("jobtitle").setRequiredLevel("required");
    var phonenumber = formContext.getAttribute("telephone1").getValue();
    var country = formContext.getAttribute("cc_country").getValue();
    if (phonenumber != null && country != null) {
        formContext.getControl("telephone1").setDisabled(false);
    }
    else {
        formContext.getControl("telephone1").setDisabled(true);
    }
}

//onChange of the country field, get the country phone code and prefix it on the phone field
function onChange(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext != null) {
        var countryid = formContext.getAttribute("cc_country").getValue() != null ? formContext.getAttribute("cc_country").getValue()[0].id : null;
        if (countryid != null) {
            countryid = countryid.replace("{", "").replace("}", "");
            setPhonePrefix(countryid, formContext);
            formContext.getControl("telephone1").setDisabled(false);
        }
        else {
            formContext.getControl("telephone1").setDisabled(true);
        }
    }
}

function setPhonePrefix(countryid, formContext) {
    //get the country that has been selected
    var globalContext = Xrm.Utility.getGlobalContext();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: `${globalContext.getClientUrl()}/api/data/v9.1/cc_countries(${countryid})?$select=cc_countrycode`,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: true,
        success: function (data, textStatus, xhr) {
            var result = data;
            var _cc_country_code = result["cc_countrycode"]
            // var _new_country_value_formatted = result["_new_country_value@OData.Community.Display.V1.FormattedValue"];
            // var _new_country_value_lookuplogicalname = result["_new_country_value@Microsoft.Dynamics.CRM.lookuplogicalname"];

            formContext.getAttribute("telephone1").setValue(_cc_country_code);
            formContext.getControl("telephone1").setDisabled(false);

            // if(formContext.data.entity.getEntityName()=="contact"){
            //     formContext.getAttribute("mobilephone").setValue(_sta_country_code);
            // }
        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });


}

function onPhoneNumberChange(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getControl("telephone1").clearNotification();
}

function onSave(executionContext) {
    var formContext = executionContext.getFormContext();
    //check if the mobile phone filed only contains the code and not the actual number
    var mobile = formContext.getAttribute("telephone1").getValue();
    if (mobile != null && mobile.length <= 5) {
        var message = "The Mobile Number is incorrect. Please enter a valid number."
        formContext.getControl("telephone1").setNotification("Please Enter a Valid Phone Number");

    }
    else {
        formContext.getControl("telephone1").clearNotification();
    }
}
function ClearCity(executionContext) {
    var formContext = executionContext.getFormContext();
    var city = formContext.getAttribute("cc_city").getValue();
    if (city != null) {
        formContext.getAttribute("cc_city").setValue(null);
    }

}
function setemailandjobtiltlerequerd(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("jobtitle").setRequiredLevel("required");
    formContext.getAttribute("emailaddress1").setRequiredLevel("required");
}