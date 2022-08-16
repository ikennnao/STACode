function form_OnLoad(executionContext) {
    var partylistTo = null, objRegardingEntType = null, isRegardingValid = false;

    objRegardingEntType = GetLookupEntityType(executionContext, "regardingobjectid");
    switch (objRegardingEntType) {
        case "incident":
        case "contact":
        case "account":
            SetAttrsDisability(executionContext, "regardingobjectid, to", ", ", true);
            isRegardingValid = true;
            break;
        default:
            break;
    }

    partylistTo = GetAttributeValue(executionContext, "to");
    if (partylistTo != null && partylistTo.length == 1 && partylistTo[0].entityType != null) {
        switch (partylistTo[0].entityType) {
            case "account":
            case "contact":
                if (isRegardingValid) {
                    var querySelect = "queues?$select=name,queueid&$filter=contains(name, 'Visitor%20Care%20Team')";
                    var qryResults = RetrieveMultipleRecords(querySelect, false, null);
                    var queueid = null, queuename = null;
                    if (qryResults != null && qryResults.value != null && qryResults.value.length == 1) {
                        queueid = qryResults.value[0]["queueid"];
                        queuename = qryResults.value[0]["name"];
                    }

                    if (queueid != null) {
                        //Set default values for the Email form
                        var fromValue = new Array();
                        fromValue[0] = new Object();
                        fromValue[0].id = queueid; // GUID of the lookup id
                        fromValue[0].name = queuename; // Name of the lookup
                        fromValue[0].entityType = "queue";
                        SetAttributeValue(executionContext, "from", fromValue);
                        SetAttrDisability(executionContext, "from", true);
                    }
                }

                if (partylistTo[0].entityType == "contact" && partylistTo[0].id != null && objRegardingEntType == "incident") {
                    var objToId = partylistTo[0].id.replace("{", "").replace("}", "");;
                    var qryForCustType = null, resultsForCustType = null;
                    qryForCustType = "contacts(" + objToId + ")?$select=customertypecode";
                    resultsForCustType = RetrieveMultipleRecords(qryForCustType, false, null);

                    if (resultsForCustType != null && resultsForCustType.customertypecode == 0) {
                        var objRegardingId = null, qryForCustEmail = null, resultsForCustEmail = null;

                        objRegardingId = GetLookupValId(executionContext, "regardingobjectid");
                        if (objRegardingId != null) {
                            qryForCustEmail = "incidents(" + objRegardingId + ")?$select=emailaddress";
                            resultsForCustEmail = RetrieveMultipleRecords(qryForCustEmail, false, null);

                            if (resultsForCustEmail != null && resultsForCustEmail.emailaddress != null) {
                                partylistTo[0].addressused = resultsForCustEmail.emailaddress;
                                SetAttributeValue(executionContext, "to", partylistTo);
                                SetAttrSubmitMode(executionContext, "to", "always");
                            }
                        }
                    }
                }
                break;
        }
    }
}