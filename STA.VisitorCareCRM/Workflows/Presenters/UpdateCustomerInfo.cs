using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Helpers;
using STA.TouristCareCRM.Workflows.Resources;
using System.Collections.Generic;
using System.Linq;

namespace STA.TouristCareCRM.Workflows.Presenters
{
    public class UpdateCustomerInfo
    {
        CommonMethods commonMethods = new CommonMethods();
        CRMFetchXMLs cRMFetchXMLs = new CRMFetchXMLs();

        public Entity SetRelatedRecentlyInteractedAgent(CommonWorkFlowExtensions cmObject, Entity entUpdateCustomerInfo)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(cmObject.workflowContext);

            if (entTargetCase != null && entTargetCase.LogicalName == CaseEntityAttributeNames.EntityLogicalName && entTargetCase.Attributes != null && entTargetCase.Attributes.Count > 0)
            {
                EntityReference entrefCaseCreatedBy = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.CreatedBy);

                if (entrefCaseCreatedBy != null && entrefCaseCreatedBy.LogicalName == SystemUserEntityAttributeNames.EntityLogicalName)
                {
                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.RecentlyInteractedAgent] = entrefCaseCreatedBy;
                }
            }

            return entUpdateCustomerInfo;
        }

        public Entity SetRelatedCasesCount(CommonWorkFlowExtensions cmObject, EntityReference entrefTargetCustomer, Entity entUpdateCustomerInfo)
        {
            if (entrefTargetCustomer != null)
            {
                EntityCollection entcolCustomerCases = GetRelatedCustomerCases(cmObject.srvContextUsr, entrefTargetCustomer);

                if (entcolCustomerCases != null && entcolCustomerCases.Entities != null)
                {
                    if (entcolCustomerCases.Entities.Count > 0)
                    {
                        Entity entRetrieveCustomerInfo = cmObject.srvContextUsr.Retrieve(entrefTargetCustomer.LogicalName, entrefTargetCustomer.Id,
                            new ColumnSet(ContactEntityAttributeNames.TotalCasesCount, ContactEntityAttributeNames.EnquiryCasesCount, ContactEntityAttributeNames.ComplaintCasesCount,
                            ContactEntityAttributeNames.OutofScopeCasesCount, ContactEntityAttributeNames.EmergenciesCasesCount));

                        int TotalCasesCount = int.MinValue, EQCasesCount = int.MinValue, CCCasesCount = int.MinValue, SGCasesCount = int.MinValue, OSCasesCount = int.MinValue, EMCasesCount = int.MinValue;

                        if (entRetrieveCustomerInfo != null && entRetrieveCustomerInfo.Attributes != null && entRetrieveCustomerInfo.Attributes.Count > 0)
                        {
                            TotalCasesCount = commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.TotalCasesCount) != null ? commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.TotalCasesCount) : int.MinValue;
                            EQCasesCount = commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.EnquiryCasesCount) != null ? commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.EnquiryCasesCount) : int.MinValue;
                            CCCasesCount = commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.ComplaintCasesCount) != null ? commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.ComplaintCasesCount) : int.MinValue;
                            SGCasesCount = commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.SuggestionCasesCount) != null ? commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.SuggestionCasesCount) : int.MinValue;
                            OSCasesCount = commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.OutofScopeCasesCount) != null ? commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.OutofScopeCasesCount) : int.MinValue;
                            EMCasesCount = commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.EmergenciesCasesCount) != null ? commonMethods.GetAttributeValFromTargetEntity(entRetrieveCustomerInfo, ContactEntityAttributeNames.EmergenciesCasesCount) : int.MinValue;
                        }

                        if (TotalCasesCount != entcolCustomerCases.Entities.Count)
                        {
                            entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.TotalCasesCount] = entcolCustomerCases.Entities.Count;
                        }

                        var entcolGroupByCaseType = entcolCustomerCases.Entities.GroupBy(x => ((OptionSetValue)x.Attributes[CaseEntityAttributeNames.OOBCaseTypeCode]).Value).ToList();

                        if (entcolGroupByCaseType.Count > 0)
                        {
                            bool EQCheck = false, CCCheck = false, SGCheck = false, OSCheck = false, EMCheck = false;

                            for (int i = 0; i < entcolGroupByCaseType.Count; i++)
                            {
                                List<Entity> lstentCases = new List<Entity>() { };
                                lstentCases = entcolGroupByCaseType[i].ToList();

                                OptionSetValue optsetOOBCaseTypeName = commonMethods.GetAttributeValFromTargetEntity(lstentCases[0], CaseEntityAttributeNames.OOBCaseTypeCode);

                                if (optsetOOBCaseTypeName != null && optsetOOBCaseTypeName.Value != int.MinValue)
                                {
                                    switch (optsetOOBCaseTypeName.Value)
                                    {
                                        case (int)CaseTypeCode.Enquiry:
                                            if (EQCasesCount != lstentCases.Count)
                                            {
                                                if (lstentCases.Count > 0)
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EnquiryCasesCount] = lstentCases.Count;
                                                }
                                                else
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EnquiryCasesCount] = 0;
                                                }
                                            }
                                            EQCheck = true;
                                            break;
                                        case (int)CaseTypeCode.Complaint:
                                            if (CCCasesCount != lstentCases.Count)
                                            {
                                                if (lstentCases.Count > 0)
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.ComplaintCasesCount] = lstentCases.Count;
                                                }
                                                else
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.ComplaintCasesCount] = 0;
                                                }
                                            }
                                            CCCheck = true;
                                            break;
                                        case (int)CaseTypeCode.Suggestion:
                                            if (SGCasesCount != lstentCases.Count)
                                            {
                                                if (lstentCases.Count > 0)
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.SuggestionCasesCount] = lstentCases.Count;
                                                }
                                                else
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.SuggestionCasesCount] = 0;
                                                }
                                            }
                                            SGCheck = true;
                                            break;
                                        case (int)CaseTypeCode.OutofScope:
                                            if (OSCasesCount != lstentCases.Count)
                                            {
                                                if (lstentCases.Count > 0)
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.OutofScopeCasesCount] = lstentCases.Count;
                                                }
                                                else
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.OutofScopeCasesCount] = 0;
                                                }
                                            }
                                            OSCheck = true;
                                            break;
                                        case (int)CaseTypeCode.Emergency:
                                            if (EMCasesCount != lstentCases.Count)
                                            {
                                                if (lstentCases.Count > 0)
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EmergenciesCasesCount] = lstentCases.Count;
                                                }
                                                else
                                                {
                                                    entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EmergenciesCasesCount] = 0;
                                                }
                                            }
                                            EMCheck = true;
                                            break;
                                    }
                                }
                            }

                            #region Check If: Any of the Case Type is not available in groupby list, then set the respective case type as '0'               

                            if (!EQCheck)
                            {
                                entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EnquiryCasesCount] = 0;
                            }
                            if (!CCCheck)
                            {
                                entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.ComplaintCasesCount] = 0;
                            }
                            if (!SGCheck)
                            {
                                entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.SuggestionCasesCount] = 0;
                            }
                            if (!OSCheck)
                            {
                                entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.OutofScopeCasesCount] = 0;
                            }
                            if (!EMCheck)
                            {
                                entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EmergenciesCasesCount] = 0;
                            }

                            #endregion
                        }
                    }
                    else if (entcolCustomerCases.Entities.Count == 0)
                    {
                        entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.TotalCasesCount] = 0;
                        entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EnquiryCasesCount] = 0;
                        entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.ComplaintCasesCount] = 0;
                        entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.SuggestionCasesCount] = 0;
                        entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.OutofScopeCasesCount] = 0;
                        entUpdateCustomerInfo.Attributes[ContactEntityAttributeNames.EmergenciesCasesCount] = 0;
                    }
                }
            }

            return entUpdateCustomerInfo;
        }

        private EntityCollection GetRelatedCustomerCases(IOrganizationService orgService, EntityReference entrefTargetCustomer)
        {
            string strFetchforRetrieveCases = string.Empty;
            EntityCollection entcolTotalCustCases = new EntityCollection(), entcolPartialCustCases = null;

            PaginationClass clsPagination = new PaginationClass();
            clsPagination.Page = 1;
            clsPagination.PagingCookie = string.Empty;
            clsPagination.RecordCount = 5000;

            do
            {
                strFetchforRetrieveCases = cRMFetchXMLs.GetCustomerRelatedCases(entrefTargetCustomer.Id, clsPagination);

                if (!string.IsNullOrWhiteSpace(strFetchforRetrieveCases))
                {
                    entcolPartialCustCases = new EntityCollection();
                    entcolPartialCustCases = orgService.RetrieveMultiple(new FetchExpression(strFetchforRetrieveCases));
                }

                if (entcolPartialCustCases != null && entcolPartialCustCases.Entities != null)
                {
                    entcolTotalCustCases.Entities.AddRange(entcolPartialCustCases.Entities);
                }
                if (entcolPartialCustCases.MoreRecords)
                {
                    clsPagination.Page++;
                    clsPagination.PagingCookie = entcolPartialCustCases.PagingCookie;
                }

            } while (entcolPartialCustCases.MoreRecords);

            return entcolTotalCustCases;
        }
    }
}