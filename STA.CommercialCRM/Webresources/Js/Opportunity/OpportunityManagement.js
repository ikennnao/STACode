//Opportunity management
var GlobalformContext;
function getOptions(executionContext) {
    var formContext = executionContext.getFormContext();
    var obj = formContext.getAttribute("cc_isclientoperatingwithanotherregioncompeti");

    if (obj != null) {
        var optionSetText = obj.getText();
        if (optionSetText != null && optionSetText == "Yes") {
            //   alert(obj.getText())
            if (formContext.getControl('cc_otherregioncompetitor') != null)
                formContext.getControl('cc_otherregioncompetitor').setVisible(true);
        }
        else {
            if (formContext.getControl('cc_otherregioncompetitor') != null)
                formContext.getControl('cc_otherregioncompetitor').setVisible(false);
        }
    }
    else {
        if (formContext.getControl('cc_otherregioncompetitor') != null)
            formContext.getControl('cc_otherregioncompetitor').setVisible(false);
    }
}

function changeCPAColor(executionContext) {
    var formContext = executionContext.getFormContext();
    var objBudget = formContext.getAttribute("cc_budget").getValue();
    var objPax = formContext.getAttribute("cc_pax").getValue();

    if (objBudget != null && objPax != null) {

        var result = parseInt(objBudget) / parseInt(objPax);
        if (result != null && result <= 30)
            parent.$("[data-id='cc_cpa.fieldControl-decimal-number-text-input']").css("-webkit-text-fill-color", "green");
        else if (result != null && result <= 50 && result >= 40)
            parent.$("[data-id='cc_cpa.fieldControl-decimal-number-text-input']").css("-webkit-text-fill-color", "orange");
        else if (result != null && result >= 51)
            parent.$("[data-id='cc_cpa.fieldControl-decimal-number-text-input']").css("-webkit-text-fill-color", "red");
    }


}
function getAccountManager(executionContext) {

    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("parentaccountid").getValue() != null) {
        var AccountId = formContext.getAttribute("parentaccountid").getValue(0)[0].id;
        Xrm.WebApi.retrieveRecord("account", AccountId,
            "?$expand=owninguser($select=systemuserid, fullname), owningteam($select=teamid, name)").
            then(

                function (account) {
                    var owningTeam = account.owningteam;
                    var owningUser = account.owninguser;

                    if (owningUser != null) {
                        var ownerId = owningUser.ownerid;
                        var ownerName = owningUser.fullname;
                        var owner = [{ entityType: "systemuser", id: ownerId, name: ownerName }]
                        formContext.getAttribute('msdyn_accountmanagerid').setValue(owner);
                    }
                    else if (owningTeam != null) {
                        var ownerId = owningTeam.ownerid;
                        var ownerName = owningTeam.name;
                        var owner = [{ entityType: "team", id: ownerId, name: ownerName }]
                        formContext.getAttribute('msdyn_accountmanagerid').setValue(owner);

                    }
                },

                function (e) {
                    console.log(e.message);
                    alert(e.message);
                });

    }
}

function ClearStatusReason(executionContext) {
    var formContext = executionContext.getFormContext();
    var StatusReason = formContext.getAttribute("cc_statusreason").getValue();
    var Status = formContext.getAttribute("cc_status").getValue();

    if (StatusReason != null && Status == null) {
        formContext.getAttribute("cc_statusreason").setValue(null);
    }




}
function showrelatedfieldeforProductmeet(executionContext) {
    var formContext = executionContext.getFormContext();
    var Productneedsmet = formContext.getAttribute("cc_productneedsmet").getValue();
    if (Productneedsmet == 0) {
        formContext.getControl("cc_productgaps").setVisible(true);
        formContext.getControl("cc_doestheproductexist").setVisible(true);
        formContext.getControl("cc_productsource").setVisible(true);
    }
    else {
        formContext.getControl("cc_productgaps").setVisible(false);
        formContext.getControl("cc_doestheproductexist").setVisible(false);
        formContext.getControl("cc_productsource").setVisible(false);
        formContext.getAttribute("cc_productgaps").setValue("");
        formContext.getAttribute("cc_doestheproductexist").setValue(false);
        formContext.getAttribute("cc_productsource").setValue("");
    }

}
function showrelatedfieldCoinvestmentopportunities(executionContext) {
    var formContext = executionContext.getFormContext();
    var coinvestmentopportunities = formContext.getAttribute("cc_coinvestmentopportunities").getValue();
    formContext.getAttribute("cc_defineplan").setRequiredLevel("none");
    if (coinvestmentopportunities == 0) {
        formContext.getControl("cc_defineplan").setVisible(true);
        formContext.getAttribute("cc_defineplan").setRequiredLevel("required");

    }
    else {
        formContext.getControl("cc_defineplan").setVisible(false);
        formContext.getAttribute("cc_defineplan").setRequiredLevel("none");
        formContext.getAttribute("cc_defineplan").setValue("");

    }
}
function showreltedtabsforCommricalandMarkting(executionContext) {
    var formContext = executionContext.getFormContext();
    var oppType = GlobalformContext.getAttribute("cc_opportunitytype").getValue();
    GlobalformContext.getControl("header_process_cc_opportunitytype_1").setVisible(false);
    if (oppType == 948120000) {
        formContext.ui.process.setVisible(true);
        formContext.ui.tabs.get("Engage").setVisible(true);
        formContext.ui.tabs.get("Evaluate").setVisible(false);
        formContext.ui.tabs.get("Identify").sections.get("NonSTALedOpportunity").setVisible(false);
        formContext.ui.tabs.get("Qualify").setVisible(true)
        formContext.ui.tabs.get("Approvals").setVisible(true);
        formContext.ui.tabs.get("Monitor").setVisible(true);
        formContext.ui.tabs.get("Contract").setVisible(true);
        //SetDirectoryApprovalDisabled(GlobalformContext);
        formContext.ui.tabs.get("Approvals").sections.get("ManagerApproval").setVisible(true);
        formContext.ui.tabs.get("Qualify").sections.get("Commercial").setVisible(true);
        formContext.ui.tabs.get("Qualify").sections.get("Marketing").setVisible(false);
        formContext.getControl("header_process_cc_criteriametforcommercialpartnershipclassi").setVisible(true);
        formContext.getControl("header_process_cc_targetsegmentmarket").setVisible(false);
        formContext.getControl("header_process_cc_isclientopportunityalignedwithstascope").setVisible(false);
        formContext.getControl("header_process_cc_timeframeofopportunity").setVisible(false);
        formContext.getControl("header_process_cc_internalalignmentvp").setVisible(false);
        formContext.getControl("header_process_cc_internalmarketingstrategist").setVisible(false);
        //  formContext.data.refresh(true);


    }
    else if (oppType == 2) {
        formContext.ui.process.setVisible(true);
        formContext.ui.tabs.get("Evaluate").setVisible(true);
        formContext.ui.tabs.get("Engage").setVisible(false);
        formContext.ui.tabs.get("Identify").sections.get("NonSTALedOpportunity").setVisible(false);
        formContext.ui.tabs.get("Qualify").setVisible(true)
        formContext.ui.tabs.get("Approvals").setVisible(true);
        formContext.ui.tabs.get("Monitor").setVisible(true);
        formContext.ui.tabs.get("Contract").setVisible(true);
        formContext.ui.tabs.get("Approvals").sections.get("ManagerApproval").setVisible(false);
        formContext.ui.tabs.get("Qualify").sections.get("Marketing").setVisible(true);
        formContext.ui.tabs.get("Qualify").sections.get("Commercial").setVisible(false);
        formContext.getControl("header_process_cc_targetsegmentmarket").setVisible(true);
        formContext.getControl("header_process_cc_timeframeofopportunity").setVisible(true);
        formContext.getControl("header_process_cc_internalalignmentvp").setVisible(true);
        formContext.getControl("header_process_cc_internalmarketingstrategist").setVisible(true);
        //  formContext.data.refresh(true);

    }
    else {
        formContext.ui.process.setVisible(false);
        formContext.ui.tabs.get("Identify").sections.get("NonSTALedOpportunity").setVisible(true);
        formContext.ui.tabs.get("Qualify").setVisible(false)
        formContext.ui.tabs.get("Engage").setVisible(false);
        formContext.ui.tabs.get("Evaluate").setVisible(false);
        formContext.ui.tabs.get("Approvals").setVisible(false);
        formContext.ui.tabs.get("Monitor").setVisible(false);
        formContext.ui.tabs.get("Contract").setVisible(false);
        //  formContext.data.refresh(true);
    }


}
function onLoadBPF(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var formType = GlobalformContext.ui.getFormType();
    GlobalformContext.getAttribute("parentaccountid").setRequiredLevel("required");
    if (formType != 1 && GlobalformContext.data.process != null && GlobalformContext.data.process != undefined) {
        GlobalformContext.data.process.addOnStageChange(onStageChange);
        GlobalformContext.data.process.addOnStageSelected(onStageSelected);
    }
}


var StageNames = {
    Identify: 'Identify',
    Qualify: 'Qualify',
    Engage: 'Engage',
    Evaluate: 'Evaluate',
    Approvals: 'Approvals',
    Contract: 'Contract',
    Monitor: 'Monitor'

};

var TabsAndSections = {
    TabIdentify: 'Identify',
    TabQualify: 'Qualify',
    TabEngage: 'Engage',
    TabEvaluate: 'Evaluate',
    TabApprovals: 'Approvals',
    TabContract: 'Contract',
    TabMonitor: 'Monitor',
    TabQualify_SectionCommercial: 'Commercial',
    TabQualify_SectionMarketing: 'Marketing',
    TabApprovals_SectionManagerApproval: 'ManagerApproval',
    TabApprovals_SectionRegionalDirector: 'RegionalDirector',
    TabApprovals_SectionMarketingSpecialistApproval: 'MarketingSpecialistApproval',
    TabApprovals_SectionVPApproval: 'VPApproval'
};
function onLoadBPF(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    GlobalformContext.getAttribute("parentaccountid").setRequiredLevel("required");
    var formType = GlobalformContext.ui.getFormType();
    if (formType != 1 && GlobalformContext.data.process != null && GlobalformContext.data.process != undefined) {
        GlobalformContext.data.process.addOnStageChange(onStageChange);
        GlobalformContext.data.process.addOnStageSelected(onStageSelected);
    }
}
function hideAllTabsAndSections() {
    //--General Tab and Sections in it
    GlobalformContext.ui.tabs.get(TabsAndSections.TabGeneral).setVisible(false);
    GlobalformContext.ui.tabs.get(TabsAndSections.TabGeneral).sections.get(TabsAndSections.TabGeneral_SectionGeneral).setVisible(false);
    GlobalformContext.ui.tabs.get(TabsAndSections.TabGeneral).sections.get(TabsAndSections.TabGeneral_SectionSummary).setVisible(false);

    //--Info Tab and Sections in it
    GlobalformContext.ui.tabs.get(TabsAndSections.TabInfo).setVisible(false);
    GlobalformContext.ui.tabs.get(TabsAndSections.TabInfo).sections.get(TabsAndSections.TabInfo_SectionAlerts).setVisible(false);
    GlobalformContext.ui.tabs.get(TabsAndSections.TabInfo).sections.get(TabsAndSections.TabInfo_SectionImports).setVisible(false);
}

function setTabsAndSectionsVisibilityByStage(selectedStage) {
    //hideAllTabsAndSections();
    var oppType = GlobalformContext.getAttribute("cc_opportunitytype").getValue();
    var selectedStageName = selectedStage.getName();
    if (selectedStageName == StageNames.Identify) {
        GlobalformContext.ui.tabs.get("Identify").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(4);
        if (oppType == 948120000) {

            GlobalformContext.getAttribute("cc_criteriametforcommercialpartnershipclassi").setRequiredLevel("none");


        }
        else if (oppType == 2) {

            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_marketingobjective").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_isclientopportunityalignedwithstascope").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_timeframeofopportunity").setRequiredLevel("none");




        }
        else {

            GlobalformContext.getAttribute("cc_criteriametforcommercialpartnershipclassi").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_marketingobjective").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_isclientopportunityalignedwithstascope").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_timeframeofopportunity").setRequiredLevel("none");

        }

    }
    else if (selectedStageName == StageNames.Qualify) {
        GlobalformContext.ui.tabs.get("Qualify").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(0);
        GlobalformContext.getControl("header_process_cc_opportunitytype_1").setVisible(false);
        if (oppType == 948120000) {
            GlobalformContext.ui.tabs.get("Qualify").sections.get(TabsAndSections.TabQualify_SectionCommercial).setVisible(true);
            GlobalformContext.getControl("header_process_cc_criteriametforcommercialpartnershipclassi").setVisible(true);
            GlobalformContext.getAttribute("cc_criteriametforcommercialpartnershipclassi").setRequiredLevel("required");
            GlobalformContext.getControl("header_process_cc_targetsegmentmarket").setVisible(false);
            GlobalformContext.getControl("header_process_cc_isclientopportunityalignedwithstascope").setVisible(false);
            GlobalformContext.getControl("header_process_cc_timeframeofopportunity").setVisible(false);
            GlobalformContext.getControl("header_process_cc_opportunitytype_1").setVisible(false);
            GlobalformContext.getAttribute("cc_presentationandstastrategypresentationcom").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_coinvestmentopportunities").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_doesstarequireamouorpartnershipcontract").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_keydecisionmakersidentified").setRequiredLevel("none");

        }
        else if (oppType == 2) {
            GlobalformContext.getControl("header_process_cc_criteriametforcommercialpartnershipclassi").setVisible(false);
            GlobalformContext.getControl("header_process_cc_opportunitytype_1").setVisible(false);
            GlobalformContext.ui.tabs.get("Qualify").sections.get(TabsAndSections.TabQualify_SectionMarketing).setVisible(true);
            GlobalformContext.getControl("header_process_cc_targetsegmentmarket").setVisible(true);
            GlobalformContext.getControl("header_process_cc_isclientopportunityalignedwithstascope").setVisible(true);
            GlobalformContext.getControl("header_process_cc_timeframeofopportunity").setVisible(true);
            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_marketingobjective").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_isclientopportunityalignedwithstascope").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_timeframeofopportunity").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_identifyreach").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_targetaudience").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_demographics").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_kpistarget").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_budget").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_pax").setRequiredLevel("none");


        }
        else {

            GlobalformContext.getAttribute("cc_criteriametforcommercialpartnershipclassi").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_marketingobjective").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_isclientopportunityalignedwithstascope").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_timeframeofopportunity").setRequiredLevel("none");
        }
    }
    else if (selectedStageName == StageNames.Engage) {
        GlobalformContext.ui.tabs.get("Engage").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(9);
        GlobalformContext.getAttribute("cc_approvalstatusmanager").setRequiredLevel("none");
        GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("none");
        if (oppType == 948120000) {
            GlobalformContext.getAttribute("cc_presentationandstastrategypresentationcom").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_coinvestmentopportunities").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_doesstarequireamouorpartnershipcontract").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_keydecisionmakersidentified").setRequiredLevel("required");

        }
        else {
            GlobalformContext.getAttribute("cc_presentationandstastrategypresentationcom").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_targetsegmentmarket").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_coinvestmentopportunities").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_doesstarequireamouorpartnershipcontract").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_keydecisionmakersidentified").setRequiredLevel("none");
        }

    }
    else if (selectedStageName == StageNames.Evaluate) {
        GlobalformContext.ui.tabs.get("Evaluate").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(15);
        GlobalformContext.getAttribute("cc_internalalignmentvp").setRequiredLevel("none");
        GlobalformContext.getAttribute("cc_internalmarketingstrategist").setRequiredLevel("none");
        GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("none");
        if (oppType == 2) {
            GlobalformContext.getAttribute("cc_identifyreach").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_targetaudience").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_demographics").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_kpistarget").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_budget").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_pax").setRequiredLevel("required");
        }
        else {
            GlobalformContext.getAttribute("cc_identifyreach").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_targetaudience").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_demographics").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_kpistarget").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_budget").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_pax").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_internalalignmentvp").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_internalmarketingstrategist").setRequiredLevel("required");
        }

    }
    else if (selectedStageName == StageNames.Approvals) {
        GlobalformContext.ui.tabs.get("Approvals").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(10);

        if (oppType == 948120000) {
            GlobalformContext.ui.tabs.get("Approvals").sections.get("ManagerApproval").setVisible(true);
            GlobalformContext.getAttribute("cc_approvalstatusmanager").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("required");
            GlobalformContext.getControl('header_process_cc_approvalstatusmanager').setDisabled(true);
            GlobalformContext.getControl('header_process_cc_approvalstatusdirector').setDisabled(true);
            GlobalformContext.getControl("header_process_cc_internalalignmentvp").setVisible(false);
            GlobalformContext.getControl("header_process_cc_internalmarketingstrategist").setVisible(false);
            // var ManagerApproval = GlobalformContext.getAttribute("cc_approvalstatusmanager").getValue();
            //var DirectorApproval = GlobalformContext.getAttribute("cc_approvalstatusdirector").getValue();
            //  if (ManagerApproval == 948120000 && DirectorApproval == null) {
            //     GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("required");

            // }
            // else if (DirectorApproval == 948120000) {

            // }
            // else {
            //    GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("none");

            // }
        }
        else if (oppType == 2) {
            GlobalformContext.getControl('header_process_cc_approvalstatusdirector').setDisabled(true);
            GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_approvalstatusmanager").setRequiredLevel("none");
            GlobalformContext.getControl("header_process_cc_approvalstatusmanager").setVisible(false);
            GlobalformContext.getControl("header_process_cc_internalalignmentvp").setVisible(true);
            GlobalformContext.getControl("header_process_cc_internalmarketingstrategist").setVisible(true);
            GlobalformContext.getAttribute("cc_internalalignmentvp").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_internalmarketingstrategist").setRequiredLevel("required");

        }
        else {
            setApporvalsrequiredfiledstonone(GlobalformContext);
        }
        CheckUserRoleforApprovalStage();

    }
    else if (selectedStageName == StageNames.Contract) {
        GlobalformContext.ui.tabs.get("Contract").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(11);
        GlobalformContext.data.refresh(true);
        GlobalformContext.getAttribute("cc_ontarget").setRequiredLevel("none");
        GlobalformContext.getAttribute("cc_deliverables").setRequiredLevel("none");
    }
    else if (selectedStageName == StageNames.Monitor) {
        GlobalformContext.ui.tabs.get("Monitor").setFocus();
        GlobalformContext.getAttribute("cc_stage").setValue(12);
        GlobalformContext.data.refresh(true);
        if (oppType == 948120000 || oppType == 2) {
            GlobalformContext.getAttribute("cc_ontarget").setRequiredLevel("required");
            GlobalformContext.getAttribute("cc_deliverables").setRequiredLevel("required");
        }



    }
}

function onStageChange(excontext) {
    var nextPrev = excontext.getEventArgs().getDirection();//-- Next / Previous
    var activeStage = GlobalformContext.data.process.getActiveStage();

    //--On Next click on BPF
    if (activeStage != null && nextPrev == 'Next') {
        setTabsAndSectionsVisibilityByStage(activeStage);

    }
    //--On Previous click on BPF
    else if (activeStage != null && nextPrev == 'Previous') {
        setTabsAndSectionsVisibilityByStage(activeStage);
    }

}

function onStageSelected() {
    var selectedStage = GlobalformContext.data.process.getSelectedStage();
    if (selectedStage != null) {
        setTabsAndSectionsVisibilityByStage(selectedStage);
    }
}

function setApporvalsrequiredfiledstonone(GformContext) {

    GformContext.getAttribute("cc_approvalstatusmanager").setRequiredLevel("none");
    GformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("none");
    GformContext.getAttribute("cc_managerapprovalname").setRequiredLevel("none");
    GformContext.getAttribute("cc_managerapprovaldate").setRequiredLevel("none");
    GformContext.getAttribute("cc_managerapprovalreason").setRequiredLevel("none");
    GformContext.getAttribute("cc_directorapprovalname").setRequiredLevel("none");
    GformContext.getAttribute("cc_directorapprovaldate").setRequiredLevel("none");
    GformContext.getAttribute("cc_directorapprovalreason").setRequiredLevel("none");

}

function SetDirectoryApprovalDisabled(GlobalformContext) {

    var oppType = GlobalformContext.getAttribute("cc_opportunitytype").getValue();
    var ManagerApproval = GlobalformContext.getAttribute("cc_approvalstatusmanager").getValue();
    var DirectorApproval = GlobalformContext.getAttribute("cc_approvalstatusmanager").getValue();
    GlobalformContext.getControl('cc_managerapprovalname').setDisabled(true);
    GlobalformContext.getControl('cc_managerapprovaldate').setDisabled(true);
    if (oppType == 948120000 && ManagerApproval != 948120000) {
        GlobalformContext.getControl('cc_approvalstatusdirector').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovalname').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovaldate').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovalreason').setDisabled(true);

    }
    else if (ManagerApproval == 948120000 && DirectorApproval == 948120000) {
        GlobalformContext.getControl('cc_approvalstatusmanager').setDisabled(true);
        GlobalformContext.getControl('cc_managerapprovalname').setDisabled(true);
        GlobalformContext.getControl('cc_managerapprovaldate').setDisabled(true);
        GlobalformContext.getControl('cc_managerapprovalreason').setDisabled(true);
        GlobalformContext.getControl('cc_approvalstatusdirector').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovalname').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovaldate').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovalreason').setDisabled(true);
    }
    else if (ManagerApproval == 948120000 && DirectorApproval == null) {
        GlobalformContext.getControl('cc_approvalstatusdirector').setDisabled(false);
        GlobalformContext.getControl('cc_directorapprovalname').setDisabled(false);
        GlobalformContext.getControl('cc_directorapprovaldate').setDisabled(false);
        GlobalformContext.getControl('cc_directorapprovalreason').setDisabled(false);
    }
    else if (DirectorApproval == 948120000) {
        GlobalformContext.getControl('cc_approvalstatusdirector').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovalname').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovaldate').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovalreason').setDisabled(true);
    }


}

function SetOpportunityTypeDisabledOnSave(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    if (GlobalformContext.getAttribute("cc_opportunitytype").getValue() != null && GlobalformContext.getAttribute("parentaccountid").getValue() != null) {
        // GlobalformContext.getControl('cc_opportunitytype').setDisabled(true);
        // GlobalformContext.getControl('header_process_cc_opportunitytype').setDisabled(true);
    }

}

function SetApprovadByandApporvalDateforManger(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var context = Xrm.Utility.getGlobalContext();
    var ManagerID = new Array();
    ManagerID[0] = new Object();
    ManagerID[0].id = context.userSettings.userId;
    ManagerID[0].entityType = "systemuser";
    ManagerID[0].name = context.userSettings.userName;
    var ManagerApproval = GlobalformContext.getAttribute("cc_approvalstatusmanager").getValue();
    var currentDate = new Date();
    if (ManagerID != null && ManagerApproval != null) {
        GlobalformContext.getAttribute("cc_managerapprovalname").setValue(ManagerID);
        GlobalformContext.getAttribute("cc_managerapprovaldate").setValue(currentDate);
        GlobalformContext.getControl('cc_managerapprovalname').setDisabled(true);
        GlobalformContext.getControl('cc_managerapprovaldate').setDisabled(true);
    }


}

function SetApprovadByandApporvalDateforDirector(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var context = Xrm.Utility.getGlobalContext();
    var DirectorID = new Array();
    DirectorID[0] = new Object();
    DirectorID[0].id = context.userSettings.userId;
    DirectorID[0].entityType = "systemuser";
    DirectorID[0].name = context.userSettings.userName;
    var DirectorApproval = GlobalformContext.getAttribute("cc_approvalstatusdirector").getValue();
    var currentDate = new Date();
    if (DirectorID != null && DirectorApproval != null) {
        GlobalformContext.getAttribute("cc_directorapprovalname").setValue(DirectorID);
        GlobalformContext.getAttribute("cc_directorapprovaldate").setValue(currentDate);
        GlobalformContext.getControl('cc_directorapprovalname').setDisabled(true);
        GlobalformContext.getControl('cc_directorapprovaldate').setDisabled(true);
    }


}

function setAccountDisabledonload(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var Account = GlobalformContext.getAttribute("parentaccountid").getValue();
    if (Account != null) {
        GlobalformContext.getControl('parentaccountid').setDisabled(true);
    }

}

function DisabledFormFieldswhentheystatusClosed(executionContext) {
    GlobalformContext = executionContext.getFormContext();
    var Status = GlobalformContext.getAttribute("cc_status").getValue();
    if (Status != null) {
        var CStatus = GlobalformContext.getAttribute("cc_status").getValue()[0].name;
        if (CStatus == "Closed") {
            GlobalformContext.ui.controls.forEach(function (control, i) {
                if (control && control.getDisabled && !control.getDisabled()) {
                    control.setDisabled(true);
                }
            });
        }

    }
}

function CheckUserRoleforApprovalStage() {

    var currentUserRoles = GlobalformContext.context.getUserRoles();
    for (var i = 0; i < currentUserRoles.length; i++) {
        var userRoleId = currentUserRoles[i];
        var userRoleName = GetRoleName(userRoleId);
        if (userRoleName == "Chief" || userRoleName == "Directors" || userRoleName == "VP Domestic Markets" || userRoleName == "VP International") {
            GlobalformContext.getAttribute("cc_approvalstatusmanager").setRequiredLevel("none");
            GlobalformContext.getAttribute("cc_approvalstatusdirector").setRequiredLevel("none");
            GlobalformContext.getControl('header_process_cc_approvalstatusmanager').setDisabled(false);
            GlobalformContext.getControl('header_process_cc_approvalstatusdirector').setDisabled(false);
            return true;
        }
    }
    return false;
}

//Get Rolename based on RoleId
function GetRoleName(roleId) {
    var roleName = null;
    var globalContext = Xrm.Utility.getGlobalContext();
    //var serverUrl = Xrm.Page.context.getServerUrl();
    //var serverUrl = location.protocol + "//" + location.host + "/" + Xrm.Page.context.getOrgUniqueName();
    //var odataSelect = serverUrl + "/XRMServices/2011/OrganizationData.svc" + "/" + "RoleSet?$filter=RoleId eq guid'" + roleId + "'";
    //var roleName = null;
    //$.ajax(
    //    {
    //        type: "GET",
    //        async: false,
    //        contentType: "application/json; charset=utf-8",
    //        datatype: "json",
    //        url: odataSelect,
    //        beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
    //        success: function (data, textStatus, XmlHttpRequest) {
    //            roleName = data.d.results[0].Name;
    //        },
    //        error: function (XmlHttpRequest, textStatus, errorThrown) { alert('OData Select Failed: ' + textStatus + errorThrown + odataSelect); }
    //    }
    //);
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: globalContext.getClientUrl() + "/api/data/v9.1/roles?$select=name&$filter=roleid eq '" + roleId + "'",
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("OData-MaxVersion", "4.0");
            XMLHttpRequest.setRequestHeader("OData-Version", "4.0");
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
            XMLHttpRequest.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        },
        async: false,
        success: function (data, textStatus, xhr) {
            var results = data;
            roleName = results.value[0].name;

        },
        error: function (xhr, textStatus, errorThrown) {
            Xrm.Utility.alertDialog(textStatus + " " + errorThrown);
        }
    });

    return roleName;
}


