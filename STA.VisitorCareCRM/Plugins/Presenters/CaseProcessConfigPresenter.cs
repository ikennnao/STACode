using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class CaseProcessConfigPresenter
    {
        private CommonMethods commonMethods = new CommonMethods();
        private CRMFetchXML_s cRMFetchXML_S = new CRMFetchXML_s();

        public void SetCaseProcessConfig_SLAConfig_AdminInfo(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPreImageCase = commonMethods.RetrievePreImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCase != null)
            {
                EntityReference entrefCaseType = null, entrefCategory = null, entrefSubCategory = null;
                OptionSetValue optsetProCode = null, optsetSLAStatus = null;
                int intPriorityCode = int.MinValue, intSLAStatus = int.MinValue; bool isCaseSubmitted = false;
                DateTime dtCaseCreatedOn = DateTime.MinValue, dtCaseSubmittedOn = DateTime.MinValue;

                entrefCaseType = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.CaseType);
                entrefCategory = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.Category);
                entrefSubCategory = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.Subcatgeory);
                optsetProCode = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.PriorityCode);
                optsetSLAStatus = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.SLAStatus);
                isCaseSubmitted = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.IsSubmitted) == null ? false : commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.IsSubmitted);
                dtCaseCreatedOn = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.CreatedOn) == null ? DateTime.MinValue : commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.CreatedOn);
                dtCaseSubmittedOn = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.SubmittedOn) == null ? DateTime.MinValue : commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.SubmittedOn);

                if (optsetProCode != null && optsetProCode.Value != int.MinValue)
                {
                    intPriorityCode = optsetProCode.Value;
                }

                if (optsetSLAStatus != null && optsetSLAStatus.Value != int.MinValue)
                {
                    intSLAStatus = optsetSLAStatus.Value;
                }

                #region If Target Case contains Is Submmited

                if (isCaseSubmitted)
                {
                    entTargetCase.Attributes[CaseEntityAttributeNames.SubmittedBy] = new EntityReference(SystemUserEntityAttributeNames.EntityLogicalName, objCommonForAllPlugins.pluginContext.InitiatingUserId);
                    dtCaseSubmittedOn = DateTime.Now.ToUniversalTime();
                    entTargetCase.Attributes[CaseEntityAttributeNames.SubmittedOn] = dtCaseSubmittedOn;
                }

                #endregion               

                if (entrefCaseType != null && entrefCategory != null && entrefSubCategory != null && intPriorityCode != int.MinValue)
                {
                    #region Set Case Process Config Info. and SLA Config.

                    CaseProcessConfigParams caseProcessConfigParams = new CaseProcessConfigParams();
                    caseProcessConfigParams.CaseTypeId = entrefCaseType.Id;

                    if (entrefCategory != null && entrefCategory.Id != Guid.Empty)
                    {
                        caseProcessConfigParams.CategoryId = entrefCategory.Id;
                    }
                    if (entrefSubCategory != null && entrefSubCategory.Id != Guid.Empty)
                    {
                        caseProcessConfigParams.SubCategoryId = entrefSubCategory.Id;
                    }
                    if (intPriorityCode != int.MinValue)
                    {
                        caseProcessConfigParams.CasePriority = intPriorityCode;
                    }

                    Entity entCaseProcessConfig = GetMostMatchCaseProcessConfig(objCommonForAllPlugins, caseProcessConfigParams);

                    if (entCaseProcessConfig != null)
                    {
                        entTargetCase.Attributes[CaseEntityAttributeNames.CaseProcessConfig] = new EntityReference(CaseProcessConfigEntityAttributeNames.EntityLogicalName, entCaseProcessConfig.Id);

                        if (entCaseProcessConfig.Attributes.Count > 0)
                        {
                            #region Populate SLA Config, Notification Config & Resolution Partner fields on Target Case from the Case Process Config Info.

                            EntityReference entrefNotificationConfig = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.NotificationConfig);

                            if (entrefNotificationConfig != null && entrefNotificationConfig.Id != Guid.Empty)
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.NotificationConfig] = new EntityReference(NotificationConfigEntityAttributeNames.EntityLogicalName, entrefNotificationConfig.Id);
                            }

                            EntityReference entrefSLAConfig = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.SLAConfig);

                            if (entrefSLAConfig != null && entrefSLAConfig.Id != Guid.Empty)
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.SLAConfig] = new EntityReference(SLAConfigEntityAttributeNames.EntityLogicalName, entrefSLAConfig.Id);
                            }

                            OptionSetValue optsetPendingWith = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.ResolutionPartner);

                            if (optsetPendingWith != null && optsetPendingWith.Value != int.MinValue)
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.ResolutionPartner] = new OptionSetValue(optsetPendingWith.Value);
                            }

                            #endregion

                            #region Populate Warn After, Escalate After & SLA Status fields on Target Case from the SLA Config Info.

                            int slaInitiateFrom = int.MinValue;
                            OptionSetValue optsetInitiateFrom = commonMethods.RetrieveAliasAttributeValue(entCaseProcessConfig, SLAConfigEntityAttributeNames.InitiateFrom, "ac.");

                            if (optsetInitiateFrom != null && optsetInitiateFrom.Value != int.MinValue)
                            {
                                slaInitiateFrom = optsetInitiateFrom.Value;
                            }

                            if (slaInitiateFrom != int.MinValue)
                            {
                                string strSLADurationinHours = string.Empty;
                                double slaWarnAfter = 0, slaEscalateAfter = 0;
                                DateTime dtWarnAfter = DateTime.MinValue, dtEscalateAfter = DateTime.MinValue;
                                slaWarnAfter = commonMethods.RetrieveAliasAttributeValue(entCaseProcessConfig, SLAConfigEntityAttributeNames.WarnAfter, "ac.");
                                slaEscalateAfter = commonMethods.RetrieveAliasAttributeValue(entCaseProcessConfig, SLAConfigEntityAttributeNames.EscalateAfter, "ac.");

                                switch (slaInitiateFrom)
                                {
                                    case (int)SLAInitiateFrom.CreatedOn: //Created On                                        
                                        if (dtCaseCreatedOn != DateTime.MinValue)
                                        {
                                            dtWarnAfter = dtCaseCreatedOn.ToUniversalTime().AddMinutes(slaWarnAfter);
                                            dtEscalateAfter = dtCaseCreatedOn.ToUniversalTime().AddMinutes(slaEscalateAfter);
                                            entTargetCase.Attributes[CaseEntityAttributeNames.WarnAfter] = dtWarnAfter;
                                            entTargetCase.Attributes[CaseEntityAttributeNames.EscalateAfter] = dtEscalateAfter;
                                            if (intSLAStatus != (int)CaseSLAStatus.InProgress && dtWarnAfter > dtCaseSubmittedOn)
                                            {
                                                entTargetCase.Attributes[CaseEntityAttributeNames.SLAStatus] = new OptionSetValue((int)CaseSLAStatus.InProgress);
                                            }
                                            entTargetCase.Attributes[CaseEntityAttributeNames.SLADuration] = dtEscalateAfter.ToLocalTime().ToString("dd-MMMM-yyyy");
                                        }
                                        break;
                                    case (int)SLAInitiateFrom.SubmittedOn: // Submitted On                                        
                                        if (isCaseSubmitted)
                                        {
                                            if (dtCaseSubmittedOn != DateTime.MinValue)
                                            {
                                                dtWarnAfter = dtCaseSubmittedOn.ToUniversalTime().AddMinutes(slaWarnAfter);
                                                dtEscalateAfter = dtCaseSubmittedOn.ToUniversalTime().AddMinutes(slaEscalateAfter);
                                                entTargetCase.Attributes[CaseEntityAttributeNames.WarnAfter] = dtWarnAfter;
                                                entTargetCase.Attributes[CaseEntityAttributeNames.EscalateAfter] = dtEscalateAfter;
                                                if (intSLAStatus != (int)CaseSLAStatus.InProgress && dtWarnAfter > dtCaseSubmittedOn)
                                                {
                                                    entTargetCase.Attributes[CaseEntityAttributeNames.SLAStatus] = new OptionSetValue((int)CaseSLAStatus.InProgress);
                                                }
                                                entTargetCase.Attributes[CaseEntityAttributeNames.SLADuration] = dtEscalateAfter.ToLocalTime().ToString("dd-MMMM-yyyy");
                                            }
                                        }
                                        else
                                        {
                                            entTargetCase.Attributes[CaseEntityAttributeNames.WarnAfter] = null;
                                            entTargetCase.Attributes[CaseEntityAttributeNames.EscalateAfter] = null;
                                            entTargetCase.Attributes[CaseEntityAttributeNames.SLAStatus] = null;
                                            entTargetCase.Attributes[CaseEntityAttributeNames.SLADuration] = string.Empty;
                                        }
                                        break;
                                }
                            }

                            #endregion
                        }
                    }

                    #endregion                    
                }
            }
        }

        public Entity RouteTargetCase_CaseProcessConfig(CommonPluginExtensions objCommonForAllPlugins, Entity entUpdateCase)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPostImageCase = commonMethods.RetrievePostImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCase != null && entTargetCase.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entTargetCase.LogicalName))
            {
                string strCaseRefNo = string.Empty;
                EntityReference entrefCategory = null, entrefSubCategory = null, entrefCaseProcessConfig = null;
                string isCaseSubmitted = string.Empty, submitToExternalTeam = string.Empty;

                entrefCategory = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.Category);
                entrefSubCategory = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.Subcatgeory);
                entrefCaseProcessConfig = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.CaseProcessConfig);
                strCaseRefNo = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.CaseRefNo);
                isCaseSubmitted = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.IsSubmitted) != null ? Convert.ToString(commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.IsSubmitted)) : null;
                submitToExternalTeam = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.SubmitToExternalTeam) != null ? Convert.ToString(commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.SubmitToExternalTeam)) : null;

                #region Set Case Title

                if (entTargetCase.Attributes.Contains(CaseEntityAttributeNames.Category) || entTargetCase.Attributes.Contains(CaseEntityAttributeNames.Subcatgeory))
                {
                    if (string.IsNullOrWhiteSpace(entrefCategory.Name) || string.IsNullOrWhiteSpace(entrefSubCategory.Name))
                    {
                        Entity entCaseSubcategory = null;
                        entCaseSubcategory = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefSubCategory.LogicalName, entrefSubCategory.Id, new ColumnSet(CaseSubcategoryEntityAttributeNames.CaseSubcategoryName, CaseSubcategoryEntityAttributeNames.CaseCategory));

                        if (entCaseSubcategory != null && entCaseSubcategory.Attributes.Count > 0)
                        {
                            entrefSubCategory.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.CaseSubcategoryName);
                            entrefCategory.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.CaseCategory) != null ? ((EntityReference)commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.CaseCategory)).Name : string.Empty;
                        }
                    }
                    entUpdateCase.Attributes[CaseEntityAttributeNames.CaseTitle] = entrefCategory.Name + " - " + entrefSubCategory.Name + " - " + strCaseRefNo;
                }

                #endregion

                #region Assign/Share/Both Target Case to Respective User/Team/Both, only when the Target Case is Submitted

                if (entTargetCase.Attributes.Contains(CaseEntityAttributeNames.IsSubmitted))
                {
                    if (entrefCaseProcessConfig != null && entrefCaseProcessConfig.Id != Guid.Empty)
                    {
                        Entity entCaseProcessConfig = null;

                        if (!string.IsNullOrEmpty(isCaseSubmitted))
                        {
                            #region If Target Case is 'Submitted' And 

                            if (isCaseSubmitted.ToLower() == "true")
                            {
                                #region If Target Case is 'Submitted' and 'Submitted To External Team'

                                if (!string.IsNullOrEmpty(submitToExternalTeam) && submitToExternalTeam.ToLower() == "true")
                                {
                                    entCaseProcessConfig = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseProcessConfigEntityAttributeNames.EntityLogicalName, entrefCaseProcessConfig.Id,
                                    new ColumnSet(CaseProcessConfigEntityAttributeNames.OperationType_ACS, CaseProcessConfigEntityAttributeNames.AssignTo_ACS, CaseProcessConfigEntityAttributeNames.UserAssign_ACS, CaseProcessConfigEntityAttributeNames.TeamAssign_ACS,
                                    CaseProcessConfigEntityAttributeNames.ShareTo_ACS, CaseProcessConfigEntityAttributeNames.UserShare_ACS, CaseProcessConfigEntityAttributeNames.TeamShare_ACS));

                                    OptionSetValue optsetOperationType = null, optsetAssignTo = null, optsetShareTo = null;
                                    int intOperationType = int.MinValue, intAssignTo = int.MinValue, intShareTo = int.MinValue;

                                    optsetOperationType = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.OperationType_ACS);
                                    optsetAssignTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.AssignTo_ACS);
                                    optsetShareTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.ShareTo_ACS);

                                    if (optsetOperationType != null && optsetOperationType.Value != int.MinValue)
                                    {
                                        intOperationType = optsetOperationType.Value;
                                        EntityReference entrefTargetCase = null, entrefAssignCaseTo = null, entrefShareCaseTo = null;
                                        entrefTargetCase = new EntityReference(entTargetCase.LogicalName, entTargetCase.Id);

                                        if (optsetAssignTo != null && optsetAssignTo.Value != int.MinValue)
                                        {
                                            intAssignTo = optsetAssignTo.Value;

                                            switch (intAssignTo)
                                            {
                                                case (int)AssignToOrShareTo.User:
                                                    entrefAssignCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.UserAssign_ACS);
                                                    break;
                                                case (int)AssignToOrShareTo.Team:
                                                    entrefAssignCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.TeamAssign_ACS);
                                                    break;
                                            }
                                        }

                                        if (optsetShareTo != null && optsetShareTo.Value != int.MinValue)
                                        {
                                            intShareTo = optsetShareTo.Value;

                                            switch (intShareTo)
                                            {
                                                case (int)AssignToOrShareTo.User:
                                                    entrefShareCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.UserShare_ACS);
                                                    break;
                                                case (int)AssignToOrShareTo.Team:
                                                    entrefShareCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.TeamShare_ACS);
                                                    break;
                                            }
                                        }

                                        switch (intOperationType)
                                        {
                                            case (int)OperationType.Assign:
                                                commonMethods.AssignTargetRecord(objCommonForAllPlugins.srvContextUsr, entrefAssignCaseTo, entrefTargetCase);
                                                if (entrefAssignCaseTo != null && entrefAssignCaseTo.Id != Guid.Empty)
                                                {
                                                    entUpdateCase.Attributes[CaseEntityAttributeNames.ResolutionPartnerQueue] = RetrieveDefaultQueueToSetResolutionPartnerQueue(objCommonForAllPlugins, entrefAssignCaseTo);
                                                }
                                                break;
                                            case (int)OperationType.Share:
                                                commonMethods.ShareTargetRcrdWithReadWriteAppendPrivileges(objCommonForAllPlugins.srvContextUsr, entrefShareCaseTo, entrefTargetCase);
                                                if (entrefShareCaseTo != null && entrefShareCaseTo.Id != Guid.Empty)
                                                {
                                                    entUpdateCase.Attributes[CaseEntityAttributeNames.ResolutionPartnerQueue] = RetrieveDefaultQueueToSetResolutionPartnerQueue(objCommonForAllPlugins, entrefShareCaseTo);
                                                }
                                                break;
                                            case (int)OperationType.BothAssignAndShare:
                                                commonMethods.AssignTargetRecord(objCommonForAllPlugins.srvContextUsr, entrefAssignCaseTo, entrefTargetCase);
                                                if (entrefAssignCaseTo != null && entrefAssignCaseTo.Id != Guid.Empty)
                                                {
                                                    entUpdateCase.Attributes[CaseEntityAttributeNames.ResolutionPartnerQueue] = RetrieveDefaultQueueToSetResolutionPartnerQueue(objCommonForAllPlugins, entrefAssignCaseTo);
                                                }

                                                commonMethods.ShareTargetRcrdWithReadWriteAppendPrivileges(objCommonForAllPlugins.srvContextUsr, entrefShareCaseTo, entrefTargetCase);
                                                if (entrefShareCaseTo != null && entrefShareCaseTo.Id != Guid.Empty)
                                                {
                                                    if (!entUpdateCase.Attributes.Contains(CaseEntityAttributeNames.ResolutionPartnerQueue)
                                                    || (entUpdateCase.Attributes.Contains(CaseEntityAttributeNames.ResolutionPartnerQueue) && entUpdateCase.Attributes[CaseEntityAttributeNames.ResolutionPartnerQueue] == null))
                                                    {
                                                        entUpdateCase.Attributes[CaseEntityAttributeNames.ResolutionPartnerQueue] = RetrieveDefaultQueueToSetResolutionPartnerQueue(objCommonForAllPlugins, entrefShareCaseTo);
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }

                                #endregion
                            }

                            #endregion

                            #region If Target Case is 'Not Submitted'

                            else if (isCaseSubmitted.ToLower() == "false")
                            {
                                entCaseProcessConfig = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseProcessConfigEntityAttributeNames.EntityLogicalName, entrefCaseProcessConfig.Id,
                                new ColumnSet(CaseProcessConfigEntityAttributeNames.OperationType_BCS, CaseProcessConfigEntityAttributeNames.AssignTo_BCS, CaseProcessConfigEntityAttributeNames.UserAssign_BCS, CaseProcessConfigEntityAttributeNames.TeamAssign_BCS,
                                CaseProcessConfigEntityAttributeNames.ShareTo_BCS, CaseProcessConfigEntityAttributeNames.UserShare_BCS, CaseProcessConfigEntityAttributeNames.TeamShare_BCS));

                                OptionSetValue optsetOperationType = null, optsetAssignTo = null, optsetShareTo = null;
                                int intOperationType = int.MinValue, intAssignTo = int.MinValue, intShareTo = int.MinValue;

                                optsetOperationType = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.OperationType_BCS);
                                optsetAssignTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.AssignTo_BCS);
                                optsetShareTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.ShareTo_BCS);

                                if (optsetOperationType != null && optsetOperationType.Value != int.MinValue)
                                {
                                    intOperationType = optsetOperationType.Value;
                                    EntityReference entrefTargetCase = null, entrefAssignCaseTo = null, entrefShareCaseTo = null;
                                    entrefTargetCase = new EntityReference(entTargetCase.LogicalName, entTargetCase.Id);

                                    if (optsetAssignTo != null && optsetAssignTo.Value != int.MinValue)
                                    {
                                        intAssignTo = optsetAssignTo.Value;

                                        switch (intAssignTo)
                                        {
                                            case (int)AssignToOrShareTo.User:
                                                entrefAssignCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.UserAssign_BCS);
                                                break;
                                            case (int)AssignToOrShareTo.Team:
                                                entrefAssignCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.TeamAssign_BCS);
                                                break;
                                        }
                                    }

                                    if (optsetShareTo != null && optsetShareTo.Value != int.MinValue)
                                    {
                                        intShareTo = optsetShareTo.Value;

                                        switch (intShareTo)
                                        {
                                            case (int)AssignToOrShareTo.User:
                                                entrefShareCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.UserShare_BCS);
                                                break;
                                            case (int)AssignToOrShareTo.Team:
                                                entrefShareCaseTo = commonMethods.GetAttributeValFromTargetEntity(entCaseProcessConfig, CaseProcessConfigEntityAttributeNames.TeamShare_BCS);
                                                break;
                                        }
                                    }

                                    switch (intOperationType)
                                    {
                                        case (int)OperationType.Assign:
                                            commonMethods.AssignTargetRecord(objCommonForAllPlugins.srvContextUsr, entrefAssignCaseTo, entrefTargetCase);
                                            break;
                                        case (int)OperationType.Share:
                                            commonMethods.ShareTargetRcrdWithReadWriteAppendPrivileges(objCommonForAllPlugins.srvContextUsr, entrefShareCaseTo, entrefTargetCase);
                                            break;
                                        case (int)OperationType.BothAssignAndShare:
                                            commonMethods.AssignTargetRecord(objCommonForAllPlugins.srvContextUsr, entrefAssignCaseTo, entrefTargetCase);
                                            commonMethods.ShareTargetRcrdWithReadWriteAppendPrivileges(objCommonForAllPlugins.srvContextUsr, entrefShareCaseTo, entrefTargetCase);
                                            break;
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }

                #endregion
            }

            return entUpdateCase;
        }

        public void CreateAssignmentHistory_ForTargetCase(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPostImageCase = commonMethods.RetrievePostImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCase != null && entTargetCase.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entTargetCase.LogicalName))
            {
                string strCaseRefNo = string.Empty;
                EntityReference entrefCaseWorkedBy = null;

                entrefCaseWorkedBy = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.CaseWorkedBy);
                strCaseRefNo = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.CaseRefNo);

                #region Create Assignment History, if Target Case contains 'Case Worked By' information

                if (entrefCaseWorkedBy != null && entrefCaseWorkedBy.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefCaseWorkedBy.LogicalName))
                {
                    Entity entCreateCAH = new Entity(CaseAssignmentHistoryEntityAttributeNames.EntityLogicalName);
                    entCreateCAH.Attributes[CaseAssignmentHistoryEntityAttributeNames.AssignedBy] = new EntityReference(entrefCaseWorkedBy.LogicalName, entrefCaseWorkedBy.Id);
                    entCreateCAH.Attributes[CaseAssignmentHistoryEntityAttributeNames.Assignee] = new EntityReference(entrefCaseWorkedBy.LogicalName, entrefCaseWorkedBy.Id);
                    entCreateCAH.Attributes[CaseAssignmentHistoryEntityAttributeNames.RegardingCase] = new EntityReference(entTargetCase.LogicalName, entTargetCase.Id);
                    entCreateCAH.Attributes[CaseAssignmentHistoryEntityAttributeNames.PickUpDateTime] = DateTime.Now;
                    entCreateCAH.Attributes[CaseAssignmentHistoryEntityAttributeNames.Name] = "Assignment History for " + strCaseRefNo;
                    objCommonForAllPlugins.srvInitiatingUsr.Create(entCreateCAH);
                }

                #endregion                
            }
        }

        public void CallReleaseCaseAction_ForTargetCase(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPostImageCase = commonMethods.RetrievePostImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCase != null && entTargetCase.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entTargetCase.LogicalName))
            {
                bool submmitedToExternalTeam = false;
                EntityReference entrefCaseWorkedBy = null;

                submmitedToExternalTeam = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.SubmitToExternalTeam) == null ? false : commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.SubmitToExternalTeam);
                entrefCaseWorkedBy = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPostImageCase, CaseEntityAttributeNames.CaseWorkedBy);

                #region Create Assignment History, if Target Case contains 'Case Worked By' information

                if (submmitedToExternalTeam)
                {
                    if (entrefCaseWorkedBy != null && entrefCaseWorkedBy.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefCaseWorkedBy.LogicalName))
                    {
                        OrganizationRequest rqtCallAction = new OrganizationRequest("tc_ReleaseCaseFunctionality");
                        rqtCallAction["ReleasedBy"] = new EntityReference(entrefCaseWorkedBy.LogicalName, entrefCaseWorkedBy.Id);
                        rqtCallAction["Target"] = new EntityReference(entTargetCase.LogicalName, entTargetCase.Id);

                        OrganizationResponse rspCallAction = objCommonForAllPlugins.srvInitiatingUsr.Execute(rqtCallAction);
                    }
                }

                #endregion
            }
        }

        private Entity GetMostMatchCaseProcessConfig(CommonPluginExtensions objCommonForAllPlugins, CaseProcessConfigParams caseProcessConfigParams)
        {
            Entity entCaseProcessConfig = null;

            if (caseProcessConfigParams != null)
            {
                string strCaseConfigFetch = cRMFetchXML_S.GetCaseProcessConfigWithSLAConfig(caseProcessConfigParams);

                if (!string.IsNullOrWhiteSpace(strCaseConfigFetch))
                {
                    EntityCollection entcolCaseProcessConfigs = objCommonForAllPlugins.srvContextUsr.RetrieveMultiple(new FetchExpression(strCaseConfigFetch));

                    if (entcolCaseProcessConfigs != null && entcolCaseProcessConfigs.Entities != null && entcolCaseProcessConfigs.Entities.Count > 0)
                    {
                        Dictionary<int, Entity> dictCasePConfig = new Dictionary<int, Entity>() { };
                        int intMaxMatchingVal = int.MinValue, intMaxNormalVal = int.MinValue, intMaxDefaultVal = int.MinValue;

                        foreach (Entity entMatchRcrd in entcolCaseProcessConfigs.Entities)
                        {
                            EntityReference entrefCaseCategory = null, entrefCaseSubCategory = null;
                            OptionSetValue optsetProCode = null;
                            bool boolAllCategories = false, boolAllSubCategories = false, boolAllPriorities = false;
                            int intCasePriority = int.MinValue, intNormalCriteria = 0, intDefaultCriteria = 0;

                            entrefCaseCategory = commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.Category);
                            entrefCaseSubCategory = commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.Subcatgeory);
                            optsetProCode = commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.CasePriority);
                            boolAllCategories = commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.AllCategories) != null ? commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.AllCategories) : false;
                            boolAllSubCategories = commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.AllSubcatgeories) != null ? commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.AllSubcatgeories) : false;
                            boolAllPriorities = commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.AllCasePriorities) != null ? commonMethods.GetAttributeValFromTargetEntity(entMatchRcrd, CaseProcessConfigEntityAttributeNames.AllCasePriorities) : false;

                            if (optsetProCode != null && optsetProCode.Value != int.MinValue)
                            {
                                intCasePriority = optsetProCode.Value;
                            }

                            if (entrefCaseCategory != null && entrefCaseCategory.Id != Guid.Empty && caseProcessConfigParams.CategoryId != Guid.Empty && entrefCaseCategory.Id == caseProcessConfigParams.CategoryId)
                            {
                                intNormalCriteria++;
                            }
                            else if (boolAllCategories)
                            {
                                intDefaultCriteria++;
                            }

                            if (entrefCaseSubCategory != null && entrefCaseSubCategory.Id != Guid.Empty && caseProcessConfigParams.SubCategoryId != Guid.Empty && entrefCaseSubCategory.Id == caseProcessConfigParams.SubCategoryId)
                            {
                                intNormalCriteria++;
                            }
                            else if (boolAllSubCategories)
                            {
                                intDefaultCriteria++;
                            }

                            if (intCasePriority != int.MinValue && caseProcessConfigParams.CasePriority != int.MinValue && intCasePriority == caseProcessConfigParams.CasePriority)
                            {
                                intNormalCriteria++;
                            }
                            else if (boolAllPriorities)
                            {
                                intDefaultCriteria++;
                            }

                            if (intNormalCriteria != int.MinValue && intDefaultCriteria != int.MinValue)
                            {
                                if (intNormalCriteria >= 1)
                                {
                                    if (intDefaultCriteria >= 1 || intNormalCriteria == 3)
                                    {
                                        if (dictCasePConfig.Count <= 0)
                                        {
                                            dictCasePConfig.Add(intNormalCriteria, entMatchRcrd);
                                            intMaxNormalVal = intNormalCriteria;
                                            intMaxDefaultVal = intDefaultCriteria;
                                        }
                                        else if (dictCasePConfig.Count > 0)
                                        {
                                            if (intNormalCriteria > intMaxNormalVal)
                                            {
                                                dictCasePConfig.Remove(intMaxNormalVal);
                                                dictCasePConfig.Add(intNormalCriteria, entMatchRcrd);
                                                intMaxNormalVal = intNormalCriteria;
                                                intMaxDefaultVal = intDefaultCriteria;
                                            }
                                        }
                                    }
                                }
                                else if (intDefaultCriteria >= 1)
                                {
                                    if (dictCasePConfig.Count <= 0)
                                    {
                                        intMaxNormalVal = intNormalCriteria;
                                        intMaxDefaultVal = intDefaultCriteria;
                                        dictCasePConfig.Add(intNormalCriteria, entMatchRcrd);
                                    }
                                    else if (dictCasePConfig.Count > 0)
                                    {
                                        if (intDefaultCriteria > intMaxDefaultVal && intMaxNormalVal < 1)
                                        {
                                            dictCasePConfig.Remove(intMaxDefaultVal);
                                            dictCasePConfig.Add(intNormalCriteria, entMatchRcrd);
                                            intMaxDefaultVal = intDefaultCriteria;
                                        }
                                    }
                                }
                            }
                        }

                        if (dictCasePConfig.Count > 0)
                        {
                            intMaxMatchingVal = dictCasePConfig.Keys.Max();
                            entCaseProcessConfig = dictCasePConfig.FirstOrDefault(x => x.Key == intMaxMatchingVal).Value;
                        }
                    }
                }
            }

            return entCaseProcessConfig;
        }

        private EntityReference RetrieveDefaultQueueToSetResolutionPartnerQueue(CommonPluginExtensions objCommonForAllPlugins, EntityReference entrefUserorTeam)
        {
            EntityReference entrefDefaultQueue = null;

            if (entrefUserorTeam != null && entrefUserorTeam.Id != Guid.Empty)
            {
                Entity entUserorTeam = null;
                entUserorTeam = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefUserorTeam.LogicalName, entrefUserorTeam.Id, new ColumnSet(SystemUserEntityAttributeNames.DefaultQueue));

                if (entUserorTeam != null && entUserorTeam.Id != Guid.Empty && entUserorTeam.Attributes != null && entUserorTeam.Attributes.Count > 0)
                {
                    entrefDefaultQueue = commonMethods.GetAttributeValFromTargetEntity(entUserorTeam, SystemUserEntityAttributeNames.DefaultQueue);
                }
            }

            return entrefDefaultQueue;
        }

        public class CaseProcessConfigParams
        {
            public Guid CaseTypeId { get; set; }
            public Guid CategoryId { get; set; }
            public Guid SubCategoryId { get; set; }
            public int CasePriority { get; set; }
        }
    }
}