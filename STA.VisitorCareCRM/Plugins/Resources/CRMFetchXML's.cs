using static STA.TouristCareCRM.Plugins.Presenters.CaseProcessConfigPresenter;

namespace STA.TouristCareCRM.Plugins.Resources
{
    public class CRMFetchXML_s
    {
        internal string GetCaseProcessConfigWithSLAConfig(CaseProcessConfigParams caseProcessConfigParams)
        {
            string strXMLForCaseConfig = string.Empty;

            strXMLForCaseConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='tc_caseprocessconfiguration'>
                                        <attribute name='tc_caseprocessconfigurationid' />
                                        <attribute name='tc_name' />
                                        <attribute name='tc_resolutionpartner' />                                        
                                        <attribute name='tc_notificationconfiguration' />
                                        <attribute name='tc_slaconfiguration' />
                                        <attribute name='tc_casetype' />
                                        <attribute name='tc_casecategory' />
                                        <attribute name='tc_allcasecategories' />
                                        <attribute name='tc_casesubcategory' />
                                        <attribute name='tc_allcasesubcategories' />
                                        <attribute name='tc_casepriority' />
                                        <attribute name='tc_allcasepriorities' />
                                        <order attribute='createdon' descending='true' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='tc_casetype' operator='eq' value='" + caseProcessConfigParams.CaseTypeId + @"' />
                                            <filter type='or'>
                                             <condition attribute='tc_casecategory' operator='eq' value='" + caseProcessConfigParams.CategoryId + @"' />
                                             <condition attribute='tc_allcasecategories' operator='eq' value='1' />
                                            </filter>
                                            <filter type='or'>
                                             <condition attribute='tc_casesubcategory' operator='eq' value='" + caseProcessConfigParams.SubCategoryId + @"' />
                                             <condition attribute='tc_allcasesubcategories' operator='eq' value='1' />
                                            </filter>
                                            <filter type='or'>
                                             <condition attribute='tc_casepriority' operator='eq' value='" + caseProcessConfigParams.CasePriority + @"' />
                                             <condition attribute='tc_allcasepriorities' operator='eq' value='1' />
                                            </filter>
                                          </filter>
                                          <link-entity name='tc_slaconfiguration' from='tc_slaconfigurationid' to='tc_slaconfiguration' link-type='outer' alias='ac'>
                                          <attribute name='tc_warnafter' />
                                          <attribute name='tc_initiatefrom' />
                                          <attribute name='tc_escalateafter' />                                             
                                          </link-entity>
                                        </entity>
                                      </fetch>";

            return strXMLForCaseConfig;
        }

        internal string GetRequiredConfigParamRecord(string strConfigParamKey)
        {
            string strfetchXMLConfig = string.Empty;

            strfetchXMLConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='cc_configurationparameters'>
                                        <attribute name='cc_configurationparametersid' />
                                        <attribute name='cc_key' />
                                        <attribute name='cc_value' />
                                        <attribute name='createdon' />
                                        <order attribute='cc_key' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='cc_key' operator='eq' value='" + strConfigParamKey + @"' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            return strfetchXMLConfig;
        }

        internal string GetInactiveDuplicateDetectionRules(string strEntityTypeName)
        {
            string strfetchXMLConfig = string.Empty;

            strfetchXMLConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='duplicaterulecondition'>
                                        <all-attributes/>
                                        <filter/>
                                        <order attribute='regardingobjectid' />
                                        <link-entity name='duplicaterule' from='duplicateruleid' to='regardingobjectid' link-type='inner' alias='dr'>
                                            <attribute name='statecode' />
                                            <attribute name='name' />
                                            <attribute name='matchingentityname' />
                                            <attribute name='excludeinactiverecords' />
                                            <filter type='and'>
                                                <condition attribute='statecode' operator='eq' value='0' />
                                                <condition attribute='baseentityname' operator='eq' value='" + strEntityTypeName + @"' />
                                            </filter>
                                        </link-entity>
                                    </entity>
                                </fetch>";

            return strfetchXMLConfig;
        }

        internal string GetContactInfoDetailsFromPrimaryInfo(string strEmail, string strContactNo)
        {
            string strXMLForContactInfoFromPrimaryInfo = string.Empty;

            if (!string.IsNullOrWhiteSpace(strEmail) || !string.IsNullOrWhiteSpace(strContactNo))
            {
                strXMLForContactInfoFromPrimaryInfo = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='contact'>
                                        <attribute name='contactid' />
                                        <order attribute='createdon' descending='false' />
                                        <filter type='and'>
                                            <condition attribute='statecode' operator='eq' value='0' />";

                if (!string.IsNullOrWhiteSpace(strEmail) && !string.IsNullOrWhiteSpace(strContactNo))
                {
                    strXMLForContactInfoFromPrimaryInfo += @"<condition attribute='emailaddress1' operator='eq' value='" + strEmail + @"' />
                                                             <condition attribute='mobilephone' operator='eq' value='" + strContactNo + @"' />";
                }
                else if (!string.IsNullOrWhiteSpace(strEmail))
                {
                    strXMLForContactInfoFromPrimaryInfo += @"<condition attribute='emailaddress1' operator='eq' value='" + strEmail + @"' />";
                }
                else if (!string.IsNullOrWhiteSpace(strContactNo))
                {
                    strXMLForContactInfoFromPrimaryInfo += @"<condition attribute='mobilephone' operator='eq' value='" + strContactNo + @"' />";
                }

                strXMLForContactInfoFromPrimaryInfo += @"</filter></entity></fetch>";
            }

            return strXMLForContactInfoFromPrimaryInfo;
        }

        internal string GetContactInfoDetailsFromSecondaryInfo(string strEmail, string strContactNo)
        {
            string strXMLForContactInfoFromSecondaryInfo = string.Empty;

            if (!string.IsNullOrWhiteSpace(strEmail) || !string.IsNullOrWhiteSpace(strContactNo))
            {
                strXMLForContactInfoFromSecondaryInfo = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='contact'>
                                        <attribute name='contactid' />
                                        <order attribute='createdon' descending='false' />
                                        <filter type='and'>
                                            <condition attribute='statecode' operator='eq' value='0' />";

                if (!string.IsNullOrWhiteSpace(strEmail) && !string.IsNullOrWhiteSpace(strContactNo))
                {
                    strXMLForContactInfoFromSecondaryInfo += @"<condition attribute='emailaddress2' operator='eq' value='" + strEmail + @"' />
                                                             <condition attribute='telephone2' operator='eq' value='" + strContactNo + @"' />";
                }
                else if (!string.IsNullOrWhiteSpace(strEmail))
                {
                    strXMLForContactInfoFromSecondaryInfo += @"<condition attribute='emailaddress2' operator='eq' value='" + strEmail + @"' />";
                }
                else if (!string.IsNullOrWhiteSpace(strContactNo))
                {
                    strXMLForContactInfoFromSecondaryInfo += @"<condition attribute='telephone2' operator='eq' value='" + strContactNo + @"' />";
                }

                strXMLForContactInfoFromSecondaryInfo += @"</filter></entity></fetch>";
            }

            return strXMLForContactInfoFromSecondaryInfo;
        }

        internal string GetAccountInfoDetailsFromPrimaryInfo(string strEmail, string strContactNo)
        {
            string strXMLForAccountInfoFromPrimaryInfo = string.Empty;

            if (!string.IsNullOrWhiteSpace(strEmail) || !string.IsNullOrWhiteSpace(strContactNo))
            {
                strXMLForAccountInfoFromPrimaryInfo = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='account'>
                                        <attribute name='accountid' />
                                        <order attribute='createdon' descending='false' />
                                        <filter type='and'>
                                            <condition attribute='statecode' operator='eq' value='0' />";

                if (!string.IsNullOrWhiteSpace(strEmail) && !string.IsNullOrWhiteSpace(strContactNo))
                {
                    strXMLForAccountInfoFromPrimaryInfo += @"<condition attribute='emailaddress1' operator='eq' value='" + strEmail + @"' />
                                                             <condition attribute='telephone1' operator='eq' value='" + strContactNo + @"' />";
                }
                else if (!string.IsNullOrWhiteSpace(strEmail))
                {
                    strXMLForAccountInfoFromPrimaryInfo += @"<condition attribute='emailaddress1' operator='eq' value='" + strEmail + @"' />";
                }
                else if (!string.IsNullOrWhiteSpace(strContactNo))
                {
                    strXMLForAccountInfoFromPrimaryInfo += @"<condition attribute='telephone1' operator='eq' value='" + strContactNo + @"' />";
                }

                strXMLForAccountInfoFromPrimaryInfo += @"</filter></entity></fetch>";
            }

            return strXMLForAccountInfoFromPrimaryInfo;
        }

        internal string GetQueryForAnonymousCustomer()
        {
            string strXMLForAnonymousCust = string.Empty;

            strXMLForAnonymousCust = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='contact'>                                        
                                        <attribute name='contactid' />
                                        <order attribute='createdon' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='customertypecode' operator='eq' value='0' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            return strXMLForAnonymousCust;
        }

        internal string GetQueryForCustomerCategory(string strCategoryId)
        {
            string strXMLForCustomerCategory = string.Empty;

            if (!string.IsNullOrWhiteSpace(strCategoryId))
            {
                strXMLForCustomerCategory = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='tc_customercategory'>                                        
                                        <attribute name='tc_customercategoryid' />
                                        <attribute name='tc_categoryid' />
                                        <order attribute='createdon' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='tc_categoryid' operator='eq' value='" + strCategoryId + @"' />
                                        </filter>
                                      </entity>
                                    </fetch>";
            }

            return strXMLForCustomerCategory;
        }
    }
}