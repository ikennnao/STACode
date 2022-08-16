using STA.TouristCareCRM.Workflows.Helpers;
using System;
using System.Collections.Generic;

namespace STA.TouristCareCRM.Workflows.Resources
{
    public class CRMFetchXMLs
    {
        internal string GetCustomerRelatedCases(Guid guidTargetCustomer, PaginationClass clsPagination)
        {
            string strfetchXMLConfig = string.Empty;

            if (guidTargetCustomer != Guid.Empty)
            {
                strfetchXMLConfig = @"<fetch version='1.0'";

                if (clsPagination != null && clsPagination.RecordCount != int.MinValue && clsPagination.RecordCount > 0 && clsPagination.Page != int.MinValue && clsPagination.Page > 0)
                {
                    if (!string.IsNullOrWhiteSpace(clsPagination.PagingCookie))
                    {
                        clsPagination.PagingCookie = clsPagination.PagingCookie.Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&quot;");
                    }

                    strfetchXMLConfig += @" count='" + clsPagination.RecordCount + @"' page='" + clsPagination.Page + @"' page-cookie='" + clsPagination.PagingCookie + @"'";
                }

                strfetchXMLConfig += @" returntotalrecordcount='true' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='incident'>
                                        <attribute name='incidentid' />
                                        <attribute name='customerid' />
                                        <attribute name='casetypecode' />
                                        <attribute name='tc_caserefno' />
                                        <order attribute='casetypecode' descending='true' />
                                        <filter type='and'>
                                          <condition attribute='customerid' operator='eq' value='" + guidTargetCustomer + @"' />
                                          <condition attribute='casetypecode' operator='not-null' />
                                        </filter>    
                                      </entity>
                                    </fetch>";
            }

            return strfetchXMLConfig;
        }

        internal string GetRelatedCaseAssignmentHistory(Guid guidTargetCase)
        {
            string strfetchXMLConfig = string.Empty;

            if (guidTargetCase != Guid.Empty)
            {
                strfetchXMLConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                          <entity name='tc_caseassignmenthistory'>
                                            <attribute name='tc_releasedate' />
                                            <attribute name='tc_pickupdate' />
                                            <attribute name='tc_assignee' />
                                            <attribute name='tc_assignedby' />
                                            <attribute name='tc_releasedby' />
                                            <attribute name='tc_regarding' />
                                            <attribute name='tc_caseassignmenthistoryid' />
                                            <order attribute='tc_pickupdate' descending='true' />
                                            <filter type='and'>
                                              <condition attribute='tc_regarding' operator='eq' value='" + guidTargetCase + @"' />
                                              <condition attribute='tc_releasedby' operator='null' />
                                              <condition attribute='tc_releasedate' operator='null' />
                                              <condition attribute='statecode' operator='eq' value='0' />
                                            </filter>
                                          </entity>
                                        </fetch>";
            }

            return strfetchXMLConfig;
        }


        internal string GetRelatedSurveyAnswers(Guid guidTargetPostedSurvey)
        {
            string strfetchXMLConfig = string.Empty;

            if (guidTargetPostedSurvey != Guid.Empty)
            {

                strfetchXMLConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cdi_surveyanswer'>
    <attribute name='cdi_question' />
    <attribute name='cdi_surveyquestionid' />
    <attribute name='createdon' />
    <attribute name='cdi_value' />
    <attribute name='cdi_postedsurveyid' />
    <attribute name='cdi_contactid' />
    <attribute name='cdi_comment' />
    <attribute name='cdi_surveyanswerid' />
    <attribute name='cdi_incidentid' />
    <attribute name='cdi_accountid' />
    <order attribute='cdi_question' descending='false' />
    <filter type='and'>
      <condition attribute='cdi_postedsurveyid' operator='eq' uiname='Complaint Survey' uitype='cdi_postedsurvey' value='" + guidTargetPostedSurvey+@"' />
    </filter>
  </entity>
</fetch>";
            }

            return strfetchXMLConfig;
        }



        internal string GetOriginChannel(string code)
        {
            string strfetchXMLConfig = string.Empty;

            if (!string.IsNullOrEmpty(code))
            {

                strfetchXMLConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='tc_channelorigin'>
    <attribute name='tc_name' />
    <attribute name='createdon' />
    <attribute name='tc_origincode' />
    <attribute name='tc_channeltype' />
    <attribute name='tc_arabicname' />
    <attribute name='tc_channeloriginid' />
    <order attribute='tc_name' descending='false' />
    <filter type='and'>
      <condition attribute='tc_origincode' operator='eq' value='"+code+@"' />
    </filter>
  </entity>
</fetch>";
            }

            return strfetchXMLConfig;
        }



        internal string GetListOfConfigParamRecords(List<string> lststrConfigParamKeys)
        {
            string strfetchXMLConfigParams = string.Empty;

            strfetchXMLConfigParams = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='cc_configurationparameters'>
                                        <attribute name='cc_configurationparametersid' />
                                        <attribute name='cc_key' />
                                        <attribute name='cc_value' />
                                        <order attribute='cc_key' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />";

            if (lststrConfigParamKeys != null && lststrConfigParamKeys.Count > 0)
            {
                strfetchXMLConfigParams += "<filter type = 'or'> ";

                foreach (string strConfigParamKey in lststrConfigParamKeys)
                {
                    if (!string.IsNullOrWhiteSpace(strConfigParamKey))
                    {
                        strfetchXMLConfigParams += "<condition attribute='cc_key' operator='eq' value='" + strConfigParamKey + @"' />";
                    }
                }

                strfetchXMLConfigParams += "</filter>";
            }

            strfetchXMLConfigParams += "</filter></entity></fetch>";

            return strfetchXMLConfigParams;
        }

        internal string GetRequiredConfigParamRecord(string strConfigParamKey)
        {
            string strfetchXMLConfig = string.Empty;

            if (!string.IsNullOrWhiteSpace(strConfigParamKey))
            {
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
            }

            return strfetchXMLConfig;
        }

        internal string RetrieveNotificationConfigRcrd(Guid guidNotificationConfigId)
        {
            string strfetchXMLConfig = string.Empty;

            if (guidNotificationConfigId != Guid.Empty)
            {
                strfetchXMLConfig = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='tc_notificationconfiguration'>
                                        <attribute name='tc_notificationconfigurationid' />                                        
                                        <attribute name='tc_emailtemplatecreate' />
                                        <attribute name='tc_smstemplatecreate' />
                                        <attribute name='tc_emailtemplateresolve' />
                                        <attribute name='tc_smstemplateresolve' />
                                        <order attribute='createdon' descending='true' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='tc_notificationconfigurationid' operator='eq' value='" + guidNotificationConfigId + @"' />
                                        </filter>
                                      </entity>
                                    </fetch>";
            }

            return strfetchXMLConfig;
        }

        internal string RetrieveTemplateRcrdDetails(string strTemplateName, int intTemplateTypeCode, int intTemplateLanguageCode)
        {
            string strfetchXMLTemplate = string.Empty;

            strfetchXMLTemplate = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='template'>
                                        <attribute name='title' />
                                        <attribute name='templateid' />
                                        <order attribute='createdon' descending='true' />
                                        <filter type='and'>
                                          <condition attribute='languagecode' operator='eq' value='" + intTemplateLanguageCode + @"' />
                                          <condition attribute='templatetypecode' operator='eq' value='" + intTemplateTypeCode + @"' />
                                          <condition attribute='title' operator='eq' value='" + strTemplateName + @"' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            return strfetchXMLTemplate;
        }

        internal string GetQueryForAnonymousCustomer()
        {
            string strXMLForAnonymousCust = string.Empty;

            strXMLForAnonymousCust = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='contact'>                                        
                                        <attribute name='contactid' />
                                        <attribute name='tc_customercategory' />
                                        <order attribute='createdon' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='customertypecode' operator='eq' value='0' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            return strXMLForAnonymousCust;
        }
    }
}