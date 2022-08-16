function form_OnLoad(executionContext) {
    populateRecipientInfo(executionContext);
}

function populateRecipientInfo(executionContext) {
    var objRegardingId = null, objRegardingName = null, objRegardingEntType = null, partylistTo = new Array(), qryForSMS = null, resultsForSMS = null;

    objRegardingEntType = GetLookupEntityType(executionContext, "regardingobjectid");
    objRegardingId = GetLookupValId(executionContext, "regardingobjectid");
    objRegardingName = GetLookupValName(executionContext, "regardingobjectid");

    if (objRegardingId != null && objRegardingEntType != null) {
        SetAttrDisability(executionContext, "regardingobjectid", true);

        switch (objRegardingEntType) {
            case "incident":
                qryForSMS = "incidents(" + objRegardingId + ")?$select=_customerid_value,tc_contactnumber";
                resultsForSMS = RetrieveMultipleRecords(qryForSMS, false, null);

                if (resultsForSMS != null) {
                    if (resultsForSMS._customerid_value != null) {
                        partylistTo[0] = new Object();
                        partylistTo[0].id = resultsForSMS._customerid_value;
                        partylistTo[0].name = resultsForSMS["_customerid_value@OData.Community.Display.V1.FormattedValue"];
                        partylistTo[0].entityType = resultsForSMS["_customerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                        SetAttributeValue(executionContext, "to", partylistTo);
                    }
                    if (resultsForSMS.tc_contactnumber != null) {
                        SetAttributeValue(executionContext, "tc_recipientno", resultsForSMS.tc_contactnumber);
                        recipientNo_Validation(executionContext, "tc_recipientno");
                    }
                }

                break;
            case "contact":
                qryForSMS = "contacts(" + objRegardingId + ")?$select=mobilephone,telephone2";
                resultsForSMS = RetrieveMultipleRecords(qryForSMS, false, null);

                if (resultsForSMS != null) {
                    partylistTo[0] = new Object();
                    partylistTo[0].id = objRegardingId;
                    partylistTo[0].name = objRegardingName;
                    partylistTo[0].entityType = objRegardingEntType;
                    SetAttributeValue(executionContext, "to", partylistTo);

                    if (resultsForSMS.mobilephone != null) {
                        SetAttributeValue(executionContext, "tc_recipientno", resultsForSMS.mobilephone);
                        recipientNo_Validation(executionContext, "tc_recipientno");
                    }
                    else if (resultsForSMS.telephone2 != null) {
                        SetAttributeValue(executionContext, "tc_recipientno", resultsForSMS.telephone2);
                        recipientNo_Validation(executionContext, "tc_recipientno");
                    }
                }
                break;
            case "account":
                qryForSMS = "accounts(" + objRegardingId + ")?$select=telephone1";
                resultsForSMS = RetrieveMultipleRecords(qryForSMS, false, null);

                if (resultsForSMS != null) {
                    partylistTo[0] = new Object();
                    partylistTo[0].id = objRegardingId;
                    partylistTo[0].name = objRegardingName;
                    partylistTo[0].entityType = objRegardingEntType;
                    SetAttributeValue(executionContext, "to", partylistTo);

                    if (resultsForSMS.telephone1 != null) {
                        SetAttributeValue(executionContext, "tc_recipientno", resultsForSMS.telephone1);
                        recipientNo_Validation(executionContext, "tc_recipientno");
                    }
                }
                break;
        }
    }
}

function recipientNo_Validation(executionContext, attrName) {
    var recipientNoVal = GetAttributeValue(executionContext, attrName);

    if (recipientNoVal != null) {
        var aryContactOutput = CheckConditionsForContactNo(recipientNoVal);

        if (aryContactOutput != null) {
            if (aryContactOutput.validationStatus != undefined && !aryContactOutput.validationStatus) {
                if (aryContactOutput.validationMsg != null && aryContactOutput.validationMsg != undefined) {
                    SetAttrCtrlNotification(executionContext, attrName, aryContactOutput.validationMsg);
                }
            }
            else {
                if (aryContactOutput.contactNoEndVal != null && recipientNoVal.length != aryContactOutput.contactNoEndVal.length) {
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

function populateSMSMsg(executionContext) {
    if (GetAttributeValue(executionContext, "description") != null) {
        SetAttributeValue(executionContext, "description", null);
    }
    if (GetAttrDisability(executionContext, "description") != false) {
        SetAttrDisability(executionContext, "description", false);
    }

    var objTemplateId = GetLookupValId(executionContext, "tc_template");

    if (objTemplateId != null) {
        if (GetAttributeValue(executionContext, "subject") == null) {
            var objTemplateName = GetLookupValName(executionContext, "tc_template");
            SetAttributeValue(executionContext, "subject", objTemplateName);
        }

        var languageVal = GetAttributeValue(executionContext, "tc_language");
        var qryForSMSBody = null, resultForSMSBody = null;

        switch (languageVal) {
            case 1025: //Arabic
                qryForSMSBody = "tc_smstemplates(" + objTemplateId + ")?$select=tc_messagearabic";
                break;
            case 1033: //English
            default:
                qryForSMSBody = "tc_smstemplates(" + objTemplateId + ")?$select=tc_messageenglish";
                break;
        }

        if (qryForSMSBody != null) {
            resultForSMSBody = RetrieveMultipleRecords(qryForSMSBody, false, null);

            if (resultForSMSBody != null) {
                var smsmsgBodyVal = null;

                switch (languageVal) {
                    case 1025: //Arabic
                        if (resultForSMSBody.tc_messagearabic != null) {
                            smsmsgBodyVal = resultForSMSBody.tc_messagearabic;
                        }
                        break;
                    case 1033: //English
                    default:
                        if (resultForSMSBody.tc_messageenglish != null) {
                            smsmsgBodyVal = resultForSMSBody.tc_messageenglish;
                        }
                        break;
                }

                if (smsmsgBodyVal != null) {
                    SetAttributeValue(executionContext, "description", smsmsgBodyVal);
                    SetAttrDisability(executionContext, "description", true);
                }
            }
        }
    }
}