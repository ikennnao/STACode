using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class CheckToRestrictDuplicateRecord
    {
        private CommonMethods commonMethods = new CommonMethods();
        private CustomMessages customMsgs = new CustomMessages();
        private CRMFetchXML_s cRMFetchXML_S = new CRMFetchXML_s();

        public void CheckConditionToDuplicate_OnPreOperation(IPluginExecutionContext exeContext, IOrganizationServiceFactory orgSrvFactory, ITracingService srvTracing, string strUnsecureConfig)
        {
            if (!string.IsNullOrWhiteSpace(strUnsecureConfig))
            {
                Entity entTargetRcrd = null, entPreImageTargetRcrd = null;

                entTargetRcrd = commonMethods.RetrieveTargetEntityFromContext(exeContext);

                if (exeContext.MessageName == PluginHelperStrigs.UpdateMsgName)
                {
                    entPreImageTargetRcrd = commonMethods.RetrievePreImageEntityFromContext(exeContext);
                }

                if (entTargetRcrd != null && !string.IsNullOrWhiteSpace(entTargetRcrd.LogicalName))
                {
                    // Obtain the organization service reference.
                    IOrganizationService srvContextUsr = orgSrvFactory.CreateOrganizationService(exeContext.UserId);
                    IOrganizationService srvInitiatingUsr = orgSrvFactory.CreateOrganizationService(exeContext.InitiatingUserId);

                    EntityCollection entcolDuplicateDetectionRules = RetrieveDuplicateDetectionRules(srvContextUsr, strUnsecureConfig);

                    if (entcolDuplicateDetectionRules != null && entcolDuplicateDetectionRules.Entities != null && entcolDuplicateDetectionRules.Entities.Count > 0)
                    {
                        QueryExpression qryExpForDuplicateDetection = RetrieveDDRQueryConditionsForMatch(entcolDuplicateDetectionRules, entTargetRcrd, entPreImageTargetRcrd);

                        if (qryExpForDuplicateDetection != null)
                        {
                            EntityCollection entcolDupRcrdsForMatchCond = FormatQryWithPagingInfoAndCheckDupRcrds(exeContext, srvContextUsr, qryExpForDuplicateDetection);

                            if (entcolDupRcrdsForMatchCond != null && entcolDupRcrdsForMatchCond.Entities != null && entcolDupRcrdsForMatchCond.Entities.Count > 0)
                            {
                                int intDupEntitiesCount = entcolDupRcrdsForMatchCond.Entities.Count;

                                if (exeContext.MessageName == PluginHelperStrigs.CreateMsgName)
                                {
                                    intDupEntitiesCount = entcolDupRcrdsForMatchCond.Entities.Count;
                                }
                                else if (exeContext.MessageName == PluginHelperStrigs.UpdateMsgName)
                                {
                                    intDupEntitiesCount = entcolDupRcrdsForMatchCond.Entities.Count - 1;
                                }

                                throw new Exception(exeContext.MessageName + " Operation terminated: '" + intDupEntitiesCount + " Duplicate Records detected'." + customMsgs.msgSystemAdminForFA);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception(customMsgs.msgMissingUnSecureConfig + customMsgs.msgSystemAdminForFA);
            }
        }

        private EntityCollection RetrieveDuplicateDetectionRules(IOrganizationService orgService, string strEntityTypeName)
        {
            EntityCollection entcolInActiveDDRs = null;

            string strRetrieveConfigParamRecord = cRMFetchXML_S.GetInactiveDuplicateDetectionRules(strEntityTypeName);

            if (!string.IsNullOrWhiteSpace(strRetrieveConfigParamRecord))
            {
                entcolInActiveDDRs = orgService.RetrieveMultiple(new FetchExpression(strRetrieveConfigParamRecord));
            }

            return entcolInActiveDDRs;
        }

        private QueryExpression RetrieveDDRQueryConditionsForMatch(EntityCollection entcolDDRs, Entity entTargetRcrd, Entity entPreImageTargetRcrd)
        {
            QueryExpression qryExpForDuplicateRcrds = null;
            var varEntColDRCs = entcolDDRs.Entities.GroupBy(x => x.Attributes[DuplicateRuleConditionEntityAttributeNames.RegardingObjectId]).ToList();

            if (varEntColDRCs.Count > 0)
            {
                qryExpForDuplicateRcrds = new QueryExpression(entTargetRcrd.LogicalName);

                FilterExpression filterExpMain = new FilterExpression(LogicalOperator.Or);

                for (int i = 0; i < varEntColDRCs.Count; i++)
                {
                    List<Entity> lstEntRuleConds = new List<Entity>() { };
                    lstEntRuleConds = varEntColDRCs[i].ToList();
                    FilterExpression filterExpRuleCond = null;
                    bool mandateInputsAvailable = true;

                    if (lstEntRuleConds.Count > 0)
                    {
                        string strRuleMatchRcrdETN = commonMethods.RetrieveAliasAttributeValue(lstEntRuleConds[0], DuplicateRuleEntityAttributeNames.MatchingETN, "dr.");

                        if (!string.IsNullOrWhiteSpace(strRuleMatchRcrdETN))
                        {
                            if (strRuleMatchRcrdETN == entTargetRcrd.LogicalName)
                            {
                                filterExpRuleCond = new FilterExpression(LogicalOperator.And);

                                foreach (Entity entRuleCond in lstEntRuleConds)
                                {
                                    string strTargetRcrdAttrName = commonMethods.GetAttributeValFromTargetEntity(entRuleCond, DuplicateRuleConditionEntityAttributeNames.BaseAttributeName);
                                    dynamic strTargetRcrdVal = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetRcrd, entPreImageTargetRcrd, strTargetRcrdAttrName);

                                    if (!string.IsNullOrWhiteSpace(strTargetRcrdAttrName) && strTargetRcrdVal != null)
                                    {
                                        ConditionExpression condExpForFilter = new ConditionExpression();
                                        condExpForFilter.AttributeName = strTargetRcrdAttrName;
                                        condExpForFilter.Operator = ConditionOperator.Equal;
                                        condExpForFilter.Values.Add(strTargetRcrdVal);

                                        filterExpRuleCond.AddCondition(condExpForFilter);
                                    }
                                    else
                                    {
                                        mandateInputsAvailable = false;
                                    }
                                }

                                bool excludeInactiveRcrds = commonMethods.RetrieveAliasAttributeValue(lstEntRuleConds[0], DuplicateRuleEntityAttributeNames.ExcludeInactiveRecords, "dr.");

                                if (excludeInactiveRcrds)
                                {
                                    ConditionExpression condExpForStatecodeFilter = new ConditionExpression();
                                    condExpForStatecodeFilter.AttributeName = "statecode";
                                    condExpForStatecodeFilter.Operator = ConditionOperator.Equal;
                                    condExpForStatecodeFilter.Values.Add(0);
                                    filterExpRuleCond.AddCondition(condExpForStatecodeFilter);
                                }
                            }
                            else if (strRuleMatchRcrdETN != entTargetRcrd.LogicalName)
                            {
                                List<KeyValuePair<string, Entity>> lstKeyValForMisMatchDRCs = new List<KeyValuePair<string, Entity>>() { };
                            }
                        }
                    }

                    if (filterExpRuleCond != null && filterExpRuleCond.Conditions != null && filterExpRuleCond.Conditions.Count > 0 && mandateInputsAvailable)
                    {
                        filterExpMain.AddFilter(filterExpRuleCond);
                    }
                }

                qryExpForDuplicateRcrds.Criteria.AddFilter(filterExpMain);
            }

            return qryExpForDuplicateRcrds;
        }

        private EntityCollection FormatQryWithPagingInfoAndCheckDupRcrds(IPluginExecutionContext exeContext, IOrganizationService orgService, QueryExpression qryExpForDupRcrdsWithPagInfo)
        {
            EntityCollection entcolTotalRcrds = new EntityCollection(), entcolInitiaiRcrds = null;
            int checkDupCount = int.MinValue;

            switch (exeContext.MessageName)
            {
                case PluginHelperStrigs.CreateMsgName:
                    checkDupCount = 0;
                    break;
                case PluginHelperStrigs.UpdateMsgName:
                    checkDupCount = 1;
                    break;
            }

            // Query using the paging cookie.
            // Define the paging attributes.

            // The number of records per page to retrieve. And Initialize the page number.
            int queryCount = 5000, pageNumber = 1;

            if (qryExpForDupRcrdsWithPagInfo != null)
            {
                // Assign the pageinfo properties to the query expression.
                qryExpForDupRcrdsWithPagInfo.PageInfo = new PagingInfo();
                qryExpForDupRcrdsWithPagInfo.PageInfo.Count = queryCount;
                qryExpForDupRcrdsWithPagInfo.PageInfo.PageNumber = pageNumber;
                qryExpForDupRcrdsWithPagInfo.PageInfo.ReturnTotalRecordCount = true;

                // The current paging cookie. When retrieving the first page, 
                // pagingCookie should be null.
                qryExpForDupRcrdsWithPagInfo.PageInfo.PagingCookie = null;

                do
                {
                    entcolInitiaiRcrds = new EntityCollection();
                    entcolInitiaiRcrds = orgService.RetrieveMultiple(qryExpForDupRcrdsWithPagInfo);

                    if (entcolInitiaiRcrds != null && entcolInitiaiRcrds.Entities != null)
                    {
                        if (entcolInitiaiRcrds.Entities.Count > checkDupCount)
                        {
                            entcolTotalRcrds.Entities.AddRange(entcolInitiaiRcrds.Entities);
                        }
                        if (entcolInitiaiRcrds.MoreRecords)
                        {
                            qryExpForDupRcrdsWithPagInfo.PageInfo.PageNumber++;
                            qryExpForDupRcrdsWithPagInfo.PageInfo.PagingCookie = entcolInitiaiRcrds.PagingCookie;
                        }
                    }
                }
                while (entcolInitiaiRcrds.MoreRecords);
            }

            return entcolTotalRcrds;
        }

        private QueryExpression RetrieveDDRQueryConditionsForMisMatch(EntityCollection entcolDDRs, Entity entTargetRcrd, Entity entPreImageTargetRcrd)
        {
            QueryExpression qryExpForDuplicateRcrds = null;
            var varEntColDRCs = entcolDDRs.Entities.GroupBy(x => x.Attributes[DuplicateRuleConditionEntityAttributeNames.RegardingObjectId]).ToList();

            if (varEntColDRCs.Count > 0)
            {
                qryExpForDuplicateRcrds = new QueryExpression(entTargetRcrd.LogicalName);

                FilterExpression filterExpMain = new FilterExpression(LogicalOperator.Or);

                for (int i = 0; i < varEntColDRCs.Count; i++)
                {
                    List<Entity> lstEntRuleConds = new List<Entity>() { };
                    lstEntRuleConds = varEntColDRCs[i].ToList();
                    FilterExpression filterExpRuleCond = null;

                    if (lstEntRuleConds.Count > 0)
                    {
                        bool excludeInactiveRcrds = commonMethods.RetrieveAliasAttributeValue(lstEntRuleConds[0], DuplicateRuleEntityAttributeNames.ExcludeInactiveRecords, "dr");

                        filterExpRuleCond = new FilterExpression(LogicalOperator.And);

                        foreach (Entity entRuleCond in lstEntRuleConds)
                        {
                            string strTargetRcrdAttrName = commonMethods.GetAttributeValFromTargetEntity(entRuleCond, DuplicateRuleConditionEntityAttributeNames.BaseAttributeName);

                            if (!string.IsNullOrWhiteSpace(strTargetRcrdAttrName) && entTargetRcrd.Attributes.Contains(strTargetRcrdAttrName))
                            {
                                ConditionExpression condExpForFilter = new ConditionExpression();
                                condExpForFilter.AttributeName = strTargetRcrdAttrName;
                                condExpForFilter.Operator = ConditionOperator.Equal;
                                condExpForFilter.Values.Add(commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetRcrd, entPreImageTargetRcrd, strTargetRcrdAttrName));

                                filterExpRuleCond.AddCondition(condExpForFilter);
                            }
                        }

                        if (excludeInactiveRcrds)
                        {
                            ConditionExpression condExpForStatecodeFilter = new ConditionExpression();
                            condExpForStatecodeFilter.AttributeName = "statecode";
                            condExpForStatecodeFilter.Operator = ConditionOperator.Equal;
                            condExpForStatecodeFilter.Values.Add(0);
                            filterExpRuleCond.AddCondition(condExpForStatecodeFilter);
                        }
                    }

                    if (filterExpRuleCond != null)
                    {
                        filterExpMain.AddFilter(filterExpRuleCond);
                    }
                }

                qryExpForDuplicateRcrds.Criteria.AddFilter(filterExpMain);
            }

            return qryExpForDuplicateRcrds;
        }
    }
}