var GlobalFormContext; var globalContext;
function onload(executionContext) {
    var formContext = executionContext.getFormContext();

    var phonenumber = formContext.getAttribute("telephone1").getValue();
    var country = formContext.getAttribute("cc_country").getValue();
    if (phonenumber != null && country != null) {
        formContext.getControl("telephone1").setDisabled(false);
    }
    else {
        formContext.getControl("telephone1").setDisabled(true);
    }

    setRequiredFields(executionContext);
    globalContext = executionContext;
    //set Global context
    GlobalFormContext = executionContext.getFormContext();
    showPendingApprovalNotification(executionContext)

}

function setRequiredFields(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("cc_typeofaccount").setRequiredLevel("required");
    formContext.getAttribute("cc_marketsegment").setRequiredLevel("required");
    formContext.getAttribute("cc_businesspotential").setRequiredLevel("required");
    formContext.getAttribute("cc_region").setRequiredLevel("required");
    formContext.getAttribute("cc_subregion").setRequiredLevel("required");
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

    ClearCity(executionContext);
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
    var formContext = executionContext.getFormContext();
    var city = formContext.getAttribute("cc_city").getValue();
    if (city != null) {
        formContext.getAttribute("cc_city").setValue(null);
    }

}

function populateMenuCommand(commandProperties) {
    var entityName = Xrm.Page.data.entity.getEntityName();
    var isUCI = Xrm.Internal.isUci();
    var page = "Form";
    var commandId = 'cc.account.ChangeAcctItem.Command';
    if (isUCI) {
        commandId = entityName + "|NoRelationship|" + page + "|" + commandId;
    }


    var menuXml = '<Menu Id="cc.CustomApproval.Menu">';
    menuXml += '<MenuSection Id="cc.CustomApprovalSection" Sequence="10">';
    menuXml += '<Controls Id="cc.CustomApprovalControls">';
    menuXml += '<Button Alt="Change to Strategic Account" Command="' + commandId + '" Id="cc.Button.StrategicButton" class="hide" LabelText="Change to Strategic Account" Sequence="20" TemplateAlias="o1" ToolTipTitle="Twitter" ToolTipDescription="" />';
    menuXml += '<Button Alt="Change to Key Account" Command="' + commandId + '" Id="cc.Button.KeyButton" class="hide"  LabelText="Change to Key Account" Sequence="10" TemplateAlias="o1" ToolTipTitle="Key Account" ToolTipDescription="" />';
    menuXml += '<Button Alt="Change to Active" Command="' + commandId + '" Id="cc.Button.ActiveButton" class="hide"  LabelText="Change to Active Account" Sequence="20" TemplateAlias="o1" ToolTipTitle="Twitter" ToolTipDescription="" />';
    menuXml += '</Controls>';
    menuXml += '</MenuSection>';
    menuXml += '</Menu>';

    commandProperties["PopulationXML"] = menuXml;

}

function onMenuItemClick(commandProperties) {
    var levelofAccount;
    if (commandProperties.SourceControlId === "cc.Button.KeyButton") //Key Account
    {
        levelofAccount = 4;
    }

    else if (commandProperties.SourceControlId === "cc.Button.StrategicButton") //Strategic
    {
        levelofAccount = 1;
    }
    else if (commandProperties.SourceControlId === "cc.Button.ActiveButton") // Active
    {
        levelofAccount = 2;
        GlobalFormContext.getAttribute("cc_levelofaccount").setValue(levelofAccount);
        GlobalFormContext.getAttribute("cc_levelofaccount").setSubmitMode("always");
        GlobalFormContext.ui.clearFormNotification("approvalnotification");
        GlobalFormContext.data.save({ saveMode: 1 });
        return;
    }

    //set the upgrade account level
    GlobalFormContext.getAttribute("cc_upgradeaccountto").setValue(levelofAccount);
    setApprovalRequestUser(globalContext);
    showPendingApprovalNotification(globalContext);
    GlobalFormContext.data.save({ saveMode: 1 });
}

function setApprovalRequestUser(executionContext) {
    formContext = executionContext.getFormContext();
    var context = Xrm.Utility.getGlobalContext();
    var UserID = new Array();
    var approvee = {
        id: context.userSettings.userId,
        entityType: "systemuser",
        name: context.userSettings.userName
    }

    UserID.push(approvee);
    formContext.getAttribute("cc_approvalrequestedby").setValue(UserID);
}

function SetApprovadByandApprovedOn(executionContext) {
    formContext = executionContext.getFormContext();
    var context = Xrm.Utility.getGlobalContext();
    var ManagerID = new Array();
    var manager = {
        id: context.userSettings.userId,
        entityType: "systemuser",
        name: context.userSettings.userName
    }

    ManagerID.push(manager);
    var currentDate = new Date();
    formContext.getAttribute("cc_approvedby").setValue(ManagerID);
    formContext.getAttribute("cc_approvedon").setValue(currentDate);
}

function showPendingApprovalNotification(executionContext) {
    var formContext = executionContext.getFormContext();
    var message = `An Approval Request to Change the Level of Account to ${formContext.getAttribute("cc_upgradeaccountto").getText()} has been sent to the Appropriate Manager. You will be Notified on the Approval Status.`
    var level = "INFO"
    var uniqueId = "approvalnotification"
    var promoteTo = formContext.getAttribute("cc_upgradeaccountto").getValue();
    var currentLevel = formContext.getAttribute("cc_levelofaccount").getValue();
    if (promoteTo != null && promoteTo != currentLevel) {
        formContext.ui.setFormNotification(message, level, uniqueId);


    }
    else {
        formContext.ui.clearFormNotification(uniqueId);
    }
}

function disableNewlyCreatedRecord(executionContext) {
    var formContext = executionContext.getFormContext();
    //check if the record is a new record or not
    if (formContext.ui.getFormType() == 2) {
        formContext.getControl("cc_country").setDisabled(true);
        formContext.getControl("cc_numberofpassengers").setDisabled(true);
    }
}

function disableDublicateRecords(executionContext) {
    var formContext = executionContext.getFormContext();
    var querytype; var recordId;
    var value = formContext.getAttribute("cc_country").getValue();
    var accountId = formContext.getAttribute("cc_account").getValue() != null ? formContext.getAttribute("cc_account").getValue()[0].id : null;
    var leadId = formContext.getAttribute("cc_lead").getValue() != null ? formContext.getAttribute("cc_lead").getValue()[0].id : null;
    if (accountId != null) {
        recordId = accountId.replace("{", "").replace("}", "");
        querytype = "_cc_account_value"
    }
    if (leadId != null) {
        recordId = leadId.replace("{", "").replace("}", "");
        querytype = "_cc_lead_value"
    }

    var globalContext = Xrm.Utility.getGlobalContext();
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: `${globalContext.getClientUrl()}/api/data/v9.1/cc_passengers?$select=statecode,cc_country&$filter=cc_country eq ${value} and  ${querytype} eq ${recordId}&$orderby=createdon desc`,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: true,
        success: function (data, textStatus, xhr) {
            var results = data;
            for (var i = 0; i < results.value.length; i++) {
                //disable all the other countries which have been created earlier
                // var recordStatus = results.value[i]["statecode"];
                updatePassengers(results.value[i]["cc_passengerid"])

            }
        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });
}


function updatePassengers(recordId) {
    var passenger = {};
    passenger.statecode = 1
    WebAPIUpdate(recordId, passenger, "cc_passengers",
        function (data) {
            //  writeMessage("The account record changes were saved");
            //  deleteAccount(AccountId);
        },
        errorHandler
    );
}

function errorHandler(error) {
    writeMessage(error.message);
}

function WebAPIUpdate(recId, entity, recordType, successcallback, errorcallback) {
    var globalContext = Xrm.Utility.getGlobalContext();
    $.ajax({
        type: "PATCH",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: `${globalContext.getClientUrl()}/api/data/v9.1/${recordType}(${recId})`,
        data: JSON.stringify(entity),
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("If-Match", "*");
        },
        async: true,
        success: function (data, textStatus, xhr) {
            successcallback(data);
        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
            errorcallback;
        }
    });
}

function getTypeofAccount(executionContext) {
    var FormContextt = executionContext.getFormContext();

    FormContextt.getAttribute('cc_typeofaccountvalues').setValue('');
    var text = '';
    var attvalue = FormContextt.getAttribute('cc_typeofaccount').getValue();
    if (attvalue != null) {
        var attribute = FormContextt.getAttribute('cc_typeofaccount');
        var selectedValue = attribute.getValue().toString();
        var options = attribute.getSelectedOption();
        for (var i = 0; i < options.length; i++) {
            var option = options[i];
            if (i > 0) {
                text += ',';
            }
            text += option.text;
            if (selectedValue === option.value.toString()) {
                text += '(Selected),';
            }
        }
        FormContextt.getAttribute('cc_typeofaccountvalues').setValue(text);
    }
}

function getBusinessPotential(executionContext) {
    var FormContextt = executionContext.getFormContext();
    FormContextt.getAttribute('cc_businesspotentialvalues').setValue('');
    var text = '';
    var attvalue = FormContextt.getAttribute('cc_businesspotential').getValue();
    if (attvalue != null) {
        var attribute = FormContextt.getAttribute('cc_businesspotential');
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
        FormContextt.getAttribute('cc_businesspotentialvalues').setValue(text);
    }
}
function getMarketSegment(executionContext) {
    var FormContextt = executionContext.getFormContext();
    FormContextt.getAttribute('cc_marketsegmentvalues').setValue('');
    var text = '';
    var attvalue = FormContextt.getAttribute('cc_marketsegment').getValue();
    if (attvalue != null) {
        var attribute = FormContextt.getAttribute('cc_marketsegment');
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
        FormContextt.getAttribute('cc_marketsegmentvalues').setValue(text);
    }
}

HideManageroflevel = function GetpeendingApprovalRequest(AccountId) {
    var peendingApproval = false;
    var globalContext = Xrm.Utility.getGlobalContext();
    var id = GlobalFormContext.data.entity.getId();
    if (id != "") {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: globalContext.getClientUrl() + "/api/data/v9.1/cc_approvalrequests?$select=cc_approvalstatus,subject&$expand=regardingobjectid_account_cc_approvalrequest($select=accountid)&$filter=_regardingobjectid_value eq '" + AccountId + "' and cc_approvalstatus eq 3",
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
                XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            },
            async: false,
            success: function (data, textStatus, xhr) {
                var results = data;
                if (results.value.length > 0) {
                    peendingApproval = true;
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
            }
        });
    }
    return peendingApproval;
}
