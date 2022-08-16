using System;
using TC.WrapperService.Models;
using static TC.WrapperService.Utility.CRMCommonMethods;

namespace TC.WrapperService.Resources
{
    public class CRMFetchXML_s
    {
        internal string GetQueryForAnonymousCustomer()
        {
            string strXMLForAnonymousCust = string.Empty;

            strXMLForAnonymousCust = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='contact'>
                                        <attribute name='fullname' />                                        
                                        <attribute name='contactid' />
                                        <order attribute='fullname' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='statecode' operator='eq' value='0' />
                                          <condition attribute='customertypecode' operator='eq' value='0' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            return strXMLForAnonymousCust;
        }

        internal string GetQueryForTargetCase(RetrieveCase caseDetails)
        {
            string strXMLForTargetCase = string.Empty;

            strXMLForTargetCase = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                          <entity name='incident'>
                                            <attribute name='createdon' />
                                            <attribute name='createdby' />
                                            <attribute name='incidentid' />
                                            <attribute name='customerid' />
                                            <attribute name='tc_channelorigin' />
                                            <attribute name='statuscode' />
                                            <attribute name='tc_subcategory' />
                                            <attribute name='tc_category' />
                                            <attribute name='tc_casetype' />
                                            <attribute name='description' />
                                            <attribute name='tc_caserefno' />
                                            <attribute name='tc_externalrefno' />
                                            <attribute name='tc_apprecordurl' />
                                            <attribute name='tc_resolutioncomments' />
                                            <attribute name='prioritycode' />
                                            <attribute name='statecode' />
                                            <order attribute='createdon' descending='true' />";

            if (!string.IsNullOrWhiteSpace(caseDetails.CaseRefNo) || !string.IsNullOrWhiteSpace(caseDetails.CustomerEmail) || !string.IsNullOrWhiteSpace(caseDetails.ChannelRefNo) || caseDetails.CaseGuid != Guid.Empty || caseDetails.Stauts != null)
            {
                strXMLForTargetCase += "<filter type='and'>";

                if (!string.IsNullOrWhiteSpace(caseDetails.CaseRefNo))
                {
                    strXMLForTargetCase += "<condition attribute='tc_caserefno' operator='eq' value='" + caseDetails.CaseRefNo + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(caseDetails.ChannelRefNo))
                {
                    strXMLForTargetCase += "<condition attribute='tc_externalrefno' operator='eq' value='" + caseDetails.ChannelRefNo + @"' />";
                }
                if (caseDetails.CaseGuid != Guid.Empty)
                {
                    strXMLForTargetCase += "<condition attribute='incidentid' operator='eq' value='" + caseDetails.CaseGuid + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(caseDetails.CustomerEmail))
                {
                    strXMLForTargetCase += "<condition attribute='emailaddress' operator='eq' value='" + caseDetails.CustomerEmail + @"' />";
                }
                if (caseDetails.Stauts != null && caseDetails.Stauts.Value != int.MinValue)
                {
                    strXMLForTargetCase += "<condition attribute='statecode' operator='eq' value='" + caseDetails.Stauts.Value + @"' />";
                }

                strXMLForTargetCase += "</filter>";
            }
            if (!string.IsNullOrWhiteSpace(caseDetails.ChannelOrigin))
            {
                strXMLForTargetCase += "<link-entity name='tc_channelorigin' from='tc_channeloriginid' to='tc_channelorigin' link-type='inner' alias='cco'><filter type='and'><condition attribute='tc_origincode' operator='eq' value='" + caseDetails.ChannelOrigin + @"' /></filter></link-entity>";
            }
            if (caseDetails.SMHDetails != null && !string.IsNullOrWhiteSpace(caseDetails.SMHDetails.UserId))
            {
                strXMLForTargetCase += "<link-entity name='tc_socialmediachannel' from='tc_socialmediachannelid' to='tc_socialmediahandle' link-type='inner' alias='smh'><filter type='and'><condition attribute='tc_socialuserid' operator='eq' value='" + caseDetails.SMHDetails.UserId + @"' /></filter></link-entity>";
            }


            strXMLForTargetCase += "</entity></fetch>";

            return strXMLForTargetCase;
        }

        internal string GetContactData(RetrieveCustomerObj custDetails)
        {
            string strXMLForTargetCust = string.Empty;

            strXMLForTargetCust = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                   <entity name='contact' >
                    <attribute name='firstname' />
                    <attribute name='lastname' />
                    <attribute name='emailaddress1' />
                    <order attribute='createdon' descending='false' />";
            if (!string.IsNullOrWhiteSpace(custDetails.Email))
            {
                strXMLForTargetCust += "<filter type='or'>";

                strXMLForTargetCust += "<condition attribute = 'emailaddress1' operator= 'eq' value = '" + custDetails.Email + @"' /> ";
                strXMLForTargetCust += "<condition attribute = 'emailaddress2' operator= 'eq' value = '" + custDetails.Email + @"' /> ";

                strXMLForTargetCust += "</filter>";
                strXMLForTargetCust += "</entity></fetch>";
            }

            return strXMLForTargetCust;
        }

        internal string GetSocialMediaHandleData(RetrieveSMHObj socialMediaHandleDetails)
        {
            string strXMLForSMH = string.Empty;

            strXMLForSMH = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                   <entity name='tc_socialmediachannel' >
                    <attribute name='tc_name' />
                    <attribute name='tc_channelhandle' />
                    <attribute name='tc_socialuserid' />
                    <attribute name='tc_customer' />
                    <order attribute='createdon' descending='false' />";

            strXMLForSMH += "<filter type='and'>";

            if (!string.IsNullOrWhiteSpace(socialMediaHandleDetails.UserID) || !string.IsNullOrWhiteSpace(socialMediaHandleDetails.UserName) || socialMediaHandleDetails.SocialMediaHandleGuid != Guid.Empty)
            {
                if (!string.IsNullOrWhiteSpace(socialMediaHandleDetails.UserID))
                {
                    strXMLForSMH += "<condition attribute = 'tc_socialuserid' operator= 'eq' value = '" + socialMediaHandleDetails.UserID + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(socialMediaHandleDetails.UserName))
                {
                    strXMLForSMH += "<condition attribute = 'tc_name' operator= 'eq' value = '" + socialMediaHandleDetails.UserName + @"' />";
                }

                if (socialMediaHandleDetails.SocialMediaHandleGuid != Guid.Empty)
                {
                    strXMLForSMH += "<condition attribute = 'tc_socialmediachannelid' operator= 'eq' value = '" + socialMediaHandleDetails.SocialMediaHandleGuid + @"' />";
                }
            }

            if (socialMediaHandleDetails.SocialChannel != null && !string.IsNullOrWhiteSpace(socialMediaHandleDetails.SocialChannel.Id.ToString()))
            {
                strXMLForSMH += "<condition attribute='tc_channelhandle' operator= 'eq' value = '" + socialMediaHandleDetails.SocialChannel.Id + @"' uitype = 'tc_channelorigin' />";
            }

            if (socialMediaHandleDetails.Customer != null && !string.IsNullOrWhiteSpace(socialMediaHandleDetails.Customer.Id.ToString()))
            {
                strXMLForSMH += "<condition attribute='tc_customer' operator= 'eq' value = '" + socialMediaHandleDetails.Customer.Id + @"' uitype = 'contact' />";
            }
            strXMLForSMH += "<condition attribute='statecode' operator='eq' value='0' />";
            strXMLForSMH += "</filter></entity></fetch>";

            return strXMLForSMH;
        }

        internal string GetCustomerData(RetrieveCustomerObj customerDetails)
        {
            string strXMLForTargetCust = string.Empty;

            strXMLForTargetCust = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                   <entity name='contact' >
                    <attribute name='firstname' />
                    <attribute name='lastname' />
                    <attribute name='mobilephone' />
                    <attribute name='emailaddress1' />
                    <attribute name='telephone2' />
                    <attribute name='emailaddress2' />
                    <attribute name='tc_customercategory' />
                    <attribute name='tc_channelorigin' />
                    <attribute name='parentcustomerid' />
                    <attribute name='tc_apprecordurl' />
                    <attribute name='contactid' />
                    <attribute name='statecode' />
                    <attribute name='tc_ssid' />
                    <attribute name='tc_ssidinterest' />
                    <attribute name='tc_travelpartner' />
                    <order attribute='createdon' descending='false' />";

            if (!string.IsNullOrWhiteSpace(customerDetails.FirstName) || !string.IsNullOrWhiteSpace(customerDetails.LastName) || !string.IsNullOrWhiteSpace(customerDetails.Email) || !string.IsNullOrWhiteSpace(customerDetails.PhoneNumber) || customerDetails.CustomerGuid != Guid.Empty || !string.IsNullOrWhiteSpace(customerDetails.Status))
            {
                strXMLForTargetCust += "<filter type='and'><filter type='and'>";

                if (!string.IsNullOrWhiteSpace(customerDetails.FirstName))
                {
                    strXMLForTargetCust += "<condition attribute = 'firstname' operator= 'eq' value = '" + customerDetails.FirstName + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(customerDetails.LastName))
                {
                    strXMLForTargetCust += "<condition attribute = 'lastname' operator= 'eq' value = '" + customerDetails.LastName + @"' />";
                }
                strXMLForTargetCust += "</filter><filter type='or'>";
                if (!string.IsNullOrWhiteSpace(customerDetails.Email))
                {
                    strXMLForTargetCust += "<filter type='or'>";

                    strXMLForTargetCust += "<condition attribute = 'emailaddress1' operator= 'eq' value = '" + customerDetails.Email + @"' /> ";
                    strXMLForTargetCust += "<condition attribute = 'emailaddress2' operator= 'eq' value = '" + customerDetails.Email + @"' /> ";

                    strXMLForTargetCust += "</filter>";
                }
                if (!string.IsNullOrWhiteSpace(customerDetails.PhoneNumber))
                {
                    strXMLForTargetCust += "<filter type='or'>";

                    strXMLForTargetCust += "<condition attribute = 'mobilephone' operator= 'eq' value = '" + customerDetails.PhoneNumber + @"' />";
                    strXMLForTargetCust += "<condition attribute = 'telephone2' operator= 'eq' value = '" + customerDetails.PhoneNumber + @"' />";

                    strXMLForTargetCust += "</filter>";
                }
                strXMLForTargetCust += "</filter>";
                if (customerDetails.CustomerGuid != Guid.Empty)
                {
                    strXMLForTargetCust += "<condition attribute = 'contactid' operator= 'eq' value = '" + customerDetails.CustomerGuid + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(customerDetails.Status))
                {
                    strXMLForTargetCust += "<condition attribute = 'statecode' operator= 'eq' value = '" + customerDetails.Status + @"' />";
                }

            }

            if (!string.IsNullOrWhiteSpace(customerDetails.ChannelOrigin))
            {
                strXMLForTargetCust += "<link-entity name='tc_channelorigin' from='tc_channeloriginid' to='tc_channelorigin' link-type='inner' alias='cco'><filter type='and'><condition attribute='tc_origincode' operator='eq' value='" + customerDetails.ChannelOrigin + @"' /></filter></link-entity>";
            }
            if (customerDetails.SMHDetails != null && !string.IsNullOrWhiteSpace(customerDetails.SMHDetails.UserId))
            {
                strXMLForTargetCust += "<link-entity name='tc_socialmediachannel' from='tc_customer' to='contactid' link-type='inner' alias='smh'><filter type='and'><condition attribute='tc_socialuserid' operator='eq' value='" + customerDetails.SMHDetails.UserId + @"' /></filter></link-entity>";
            }

            strXMLForTargetCust += "</filter></entity></fetch>";

            return strXMLForTargetCust;
        }

        internal string GetCaseTypes(RetrieveCaseType retrieveCaseType)
        {
            string strXMLForCaseType = string.Empty;

            strXMLForCaseType = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='tc_casetype'>
                                    <attribute name='tc_casetypeid'/>
                                    <attribute name='tc_name'/>
                                    <attribute name='tc_arabicname'/>
                                    <order descending='false' attribute='tc_name'/>
                                    <filter type='and'>";

            if (retrieveCaseType != null)
            {
                if (!string.IsNullOrWhiteSpace(retrieveCaseType.Name))
                {
                    strXMLForCaseType += "<condition attribute='tc_name' operator='eq' value='" + retrieveCaseType.Name + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(retrieveCaseType.ArabicName))
                {
                    strXMLForCaseType += "<condition attribute='tc_arabicname' operator='eq' value='" + retrieveCaseType.ArabicName + @"' />";
                }
                if (retrieveCaseType.CaseTypeGuid != Guid.Empty)
                {
                    strXMLForCaseType += "<condition attribute='tc_casetypeid' operator='eq' value='" + retrieveCaseType.CaseTypeGuid + @"' />";
                }
            }

            strXMLForCaseType += "<condition attribute='statecode' operator='eq' value='0' /></filter></entity></fetch>";

            return strXMLForCaseType;
        }

        internal string GetKnowledgeArticles(RetrieveKnowledgeArticleObj retrieveKA)
        {
            string strXMLForRetrieveKA = string.Empty;
            const string DefaultLangCode = "{56940B3E-300F-4070-A559-5A6A4D11A8A3}";

            strXMLForRetrieveKA = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='knowledgearticle'>
                                    <attribute name='knowledgearticleid'/>
                                    <attribute name='title'/>
                                    <attribute name='modifiedon'/>
                                    <attribute name='content'/>
                                    <attribute name='tc_apprecordurl'/>
                                    <attribute name='statecode'/>
                                    <attribute name='keywords'/>
                                    <attribute name='tc_order'/>
                                    <order descending='false' attribute='tc_order'/>";

            if (retrieveKA != null)
            {
                if (!string.IsNullOrWhiteSpace(retrieveKA.Keywords) || !string.IsNullOrWhiteSpace(retrieveKA.Status) || retrieveKA.KnowledgeArticleGuid != Guid.Empty || !string.IsNullOrWhiteSpace(retrieveKA.CategoryCode))
                {
                    strXMLForRetrieveKA += "<filter type='and'>";

                    if (!string.IsNullOrWhiteSpace(retrieveKA.Keywords))
                    {
                        strXMLForRetrieveKA += "<condition attribute='keywords' operator='like' value='%" + retrieveKA.Keywords + @"%' />";
                    }
                    if (!string.IsNullOrWhiteSpace(retrieveKA.IsInternalArticle) && retrieveKA.IsInternalArticle.ToLower() != "both")
                    {
                        strXMLForRetrieveKA += "<condition attribute='isinternal' operator='eq' value='%" + retrieveKA.IsInternalArticle + @"%' />";
                    }
                    if (!string.IsNullOrWhiteSpace(retrieveKA.Status))
                    {
                        strXMLForRetrieveKA += "<condition attribute='statecode' operator='eq' value='" + Convert.ToInt32(retrieveKA.Status) + @"'/>";
                    }

                    else // Default should be Published FAQs
                    {
                        strXMLForRetrieveKA += "<condition attribute='statecode' operator='eq' value='" + (int)ArticleStatus.Published + @"'/>";
                    }
                    if (retrieveKA.KnowledgeArticleGuid != Guid.Empty)
                    {
                        strXMLForRetrieveKA += "<condition attribute='knowledgearticleid' operator='eq' value='" + retrieveKA.KnowledgeArticleGuid + @"' />";
                    }

                    if (!string.IsNullOrWhiteSpace(retrieveKA.CategoryCode))
                    {
                        int option = Convert.ToInt32(retrieveKA.CategoryCode);
                        strXMLForRetrieveKA += "<condition attribute='tc_faqcategories' operator='contain-values'> <value>" + option + @"</value></condition>";
                    }

                    if (!string.IsNullOrWhiteSpace(retrieveKA.LanguageCode))
                    {
                        strXMLForRetrieveKA += "<condition attribute='languagelocaleid' operator='eq' value='" + retrieveKA.LanguageCode + @"' />";
                    }
                    else //default will be english
                    {
                        strXMLForRetrieveKA += "<condition attribute='languagelocaleid' operator='eq' value='" + DefaultLangCode + @"' />";
                    }

                    strXMLForRetrieveKA += "</filter>";
                }
            }

            strXMLForRetrieveKA += "</entity></fetch>";

            return strXMLForRetrieveKA;
        }

        internal string GetCountries()
        {
            string strXMLForCountries = string.Empty;

            strXMLForCountries = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
  <entity name='cc_country'>
    <attribute name='cc_name' />
	<attribute name='cc_isocode' />
    <attribute name='cc_arabicname' />
    <attribute name='cc_countryid' />
    <attribute name='cc_region' />
    <order attribute='cc_name' descending='false' />
    <filter type='and'>
      <condition attribute='statecode' operator='eq' value='0' />
    </filter>
  </entity>
</fetch>";

            return strXMLForCountries;
        }

        internal string GetCountriesFromRegion(Guid regionId)
        {
            string strXMLForCountriesRegion = string.Empty;

            strXMLForCountriesRegion = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
  <entity name='cc_country'>
    <attribute name='cc_name' />
	<attribute name='cc_isocode' />
    <attribute name='cc_arabicname' />
    <attribute name='cc_countryid' />
    <attribute name='cc_region' />
    <order attribute='cc_name' descending='false' />
    <filter type='and'>";
            if (regionId != null && regionId != Guid.Empty)
            {
                strXMLForCountriesRegion += " <condition attribute = 'cc_region' operator= 'eq' value = '" + regionId + @"' /> ";
            }
            strXMLForCountriesRegion += "<condition attribute='statecode' operator='eq' value='0' /></filter></entity></fetch>";

            return strXMLForCountriesRegion;
        }
        

        internal string GetCities(Guid countryId)
        {
            string strXMLForCities = string.Empty;

            strXMLForCities = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
  <entity name='cc_city'>
    <attribute name='cc_name' />
	<attribute name='cc_cityid' />
    <attribute name='cc_country' />
    <order attribute='cc_name' descending='false' />
    <filter type='and'>
      <condition attribute='tc_businesstype' operator='ne' value='948120001' />
      <condition attribute='statecode' operator='eq' value='0' />";

            strXMLForCities += "<condition attribute ='cc_country' operator= 'eq' value = '" + countryId + @"'/></filter></entity></fetch>";

            return strXMLForCities;
        }

        internal string GetRegions()
        {
            string strXMLForRegions = string.Empty;

            strXMLForRegions = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cc_region'>
    <attribute name='cc_regionid' />
    <attribute name='cc_name' />
    <attribute name='createdon' />
    <order attribute='cc_name' descending='false' />
    <filter type='and'>
      <condition attribute='tc_businesstype' operator='ne' value='948120001' />
      <condition attribute='statecode' operator='eq' value='0' />
    </filter>
  </entity>
</fetch>";

            return strXMLForRegions;
        }

        internal string GetSubRegions(Guid regionId)
        {
            string strXMLForSubRegions = string.Empty;

            strXMLForSubRegions = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
  <entity name='cc_subregion'>
    <attribute name='cc_subregionid' />
    <attribute name='cc_name' />
   <attribute name='cc_region' />
  <order attribute='cc_name' descending='false' />
    <filter type='and'>";
            if (regionId != null && regionId != Guid.Empty)
            {
                strXMLForSubRegions += "<condition attribute ='cc_region' operator= 'eq' value = '" + regionId + @"'/>";
            }
            strXMLForSubRegions += "<condition attribute='statecode' operator='eq' value='0' /></filter></entity></fetch>";

            return strXMLForSubRegions;
        }

        internal string GetManyToManyContactRegion(Guid contactId)
        {
            string strXMLForManyToManyCR = string.Empty;
            strXMLForManyToManyCR = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='cc_region' >
    <attribute name='cc_name' />
    <link-entity name='cc_contact_cc_region' from='cc_regionid' to='cc_regionid' intersect='true' >
      <attribute name='cc_regionid' alias='market' />
      <filter>
        <condition attribute='contactid' operator='eq' value='" + contactId + @"' />
      </filter>
    </link-entity>
  </entity>
</fetch>";
            return strXMLForManyToManyCR;
        }

        internal string GetAccounts()
        {
            string strXMLForAccounts;

            strXMLForAccounts = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
  <entity name='account'>
    <attribute name='name' />
    <attribute name='accountid' />
    <order attribute='name' descending='false' />
    <filter type='and'>
      <condition attribute='cc_businesstype' operator='ne' value='948120001' />
      <condition attribute='statecode' operator='eq' value='0' />
    </filter>
  </entity>
</fetch>";

            return strXMLForAccounts;
        }

        internal string GetCompanies(RetrieveCompanyObj companyDetails)
        {
            string strXMLForCompanies;

            strXMLForCompanies = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
    <entity name='account'>
    <attribute name='name' />
    <attribute name='cc_region' />
    <attribute name='cc_subregion' />
    <attribute name='cc_city' />
    <attribute name='cc_country' />
    <attribute name='telephone1' />
    <attribute name='emailaddress1' />
    <attribute name='tc_companyregistrationnumber' />
    <attribute name='tc_mtcrmid' />
    <attribute name='tc_mtcrnumber' />
    <attribute name='preferredcontactmethodcode' />
    <attribute name='tc_facilitynamearabic' />
    <attribute name='parentaccountid' />
    <attribute name='accountid' />
    <order attribute='name' descending='false' />";

            if (!string.IsNullOrWhiteSpace(companyDetails.CompanyName) || !string.IsNullOrWhiteSpace(companyDetails.CompanyEmail) || !string.IsNullOrWhiteSpace(companyDetails.CompanyPhone) || companyDetails.CompanyGuid != Guid.Empty || !string.IsNullOrWhiteSpace(companyDetails.CompanyRegistrationNumber) || !string.IsNullOrWhiteSpace(companyDetails.MTCRMID) || !string.IsNullOrWhiteSpace(companyDetails.MTCRNumber))
            {
                strXMLForCompanies += "<filter type='and'>";

                if (!string.IsNullOrWhiteSpace(companyDetails.CompanyName))
                {
                    strXMLForCompanies += "<condition attribute = 'name' operator= 'eq' value = '" + companyDetails.CompanyName + @"' />";
                }

                if (!string.IsNullOrWhiteSpace(companyDetails.CompanyPhone))
                {
                    strXMLForCompanies += "<condition attribute = 'telephone1' operator= 'eq' value = '" + companyDetails.CompanyPhone + @"' />";
                }

                if (!string.IsNullOrWhiteSpace(companyDetails.CompanyEmail))
                {
                    strXMLForCompanies += "<filter type='or'>";
                    strXMLForCompanies += "<condition attribute = 'emailaddress1' operator= 'eq' value = '" + companyDetails.CompanyEmail + @"' /> ";
                    strXMLForCompanies += "<condition attribute = 'emailaddress2' operator= 'eq' value = '" + companyDetails.CompanyEmail + @"' /> ";
                    strXMLForCompanies += "</filter>";
                }

                if (!string.IsNullOrWhiteSpace(companyDetails.CompanyRegistrationNumber))
                {
                    strXMLForCompanies += "<condition attribute = 'tc_companyregistrationnumber' operator= 'eq' value = '" + companyDetails.CompanyRegistrationNumber + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(companyDetails.MTCRMID))
                {
                    strXMLForCompanies += "<condition attribute = 'tc_mtcrmid' operator= 'eq' value = '" + companyDetails.MTCRMID + @"' />";
                }
                if (!string.IsNullOrWhiteSpace(companyDetails.MTCRNumber))
                {
                    strXMLForCompanies += "<condition attribute = 'tc_mtcrnumber' operator= 'eq' value = '" + companyDetails.MTCRNumber + @"' />";
                }

                if (companyDetails.CompanyGuid != Guid.Empty)
                {
                    strXMLForCompanies += "<condition attribute = 'accountid' operator= 'eq' value = '" + companyDetails.CompanyGuid + @"' />";
                }

                strXMLForCompanies += "<condition attribute='statecode' operator='eq' value='0' />";
                strXMLForCompanies += "</filter></entity></fetch>";
            }

            return strXMLForCompanies;
        }

        internal string GetCategory(rqtObjRetrieveCategory objRetrieveCatgryDetails)
        {
            string strXMLForCategory = string.Empty;

            strXMLForCategory = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='tc_casecategory'>
                                    <attribute name='tc_casecategoryid'/>
                                    <attribute name='tc_name'/>
                                    <attribute name='createdon'/>
                                    <attribute name='tc_customercategory'/>
                                    <attribute name='tc_casetype'/>
                                    <attribute name='tc_arabicname'/>
                                    <order descending='false' attribute='tc_name'/>
                                    <filter type='and'>
                                    <condition attribute='tc_casetype' operator='eq' value='" + objRetrieveCatgryDetails.CaseTypeId + @"' />";

            if (objRetrieveCatgryDetails.CustomerCatgeoryId != Guid.Empty)
            {
                strXMLForCategory += "<condition attribute = 'tc_customercategory' operator= 'eq' value = '" + objRetrieveCatgryDetails.CustomerCatgeoryId + @"' />";
            }

            strXMLForCategory += "<condition attribute='statecode' operator='eq' value='0' /></filter></entity></fetch>";

            return strXMLForCategory;
        }

        internal string GetSubCategory(Guid CategoryID)
        {
            string strXMLForSubCategory = string.Empty;

            strXMLForSubCategory = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='tc_casesubcategory'>
                                    <attribute name='tc_casesubcategoryid'/>
                                    <attribute name='tc_name'/>
                                    <attribute name='tc_arabicname'/>
                                    <attribute name='tc_casecategory'/>
                                    <order descending='false' attribute='tc_name'/>
                                    <filter type='and'>
                                    <condition attribute='tc_casecategory' operator='eq' value='" + CategoryID + @"' />
                                    <condition attribute='statecode' operator='eq' value='0' />
                                    </filter>
                                    </entity>
                                   </fetch>";

            return strXMLForSubCategory;
        }

        internal string GetCustomerCategory(RetrieveCustomerCategoryObj retrieveCustomerCategoryObj)
        {
            string strXMLForCustomerCategories = string.Empty;

            strXMLForCustomerCategories = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='tc_customercategory'>
                                    <attribute name='tc_customercategoryid'/>
                                    <attribute name='tc_name'/>
                                    <attribute name='tc_arabicname'/>
                                    <order descending='false' attribute='tc_name'/>";

            if (retrieveCustomerCategoryObj != null)
            {
                if (!string.IsNullOrWhiteSpace(retrieveCustomerCategoryObj.Status) || !string.IsNullOrWhiteSpace(retrieveCustomerCategoryObj.CategoryId))
                {
                    strXMLForCustomerCategories += "<filter type='and'>";

                    if (!string.IsNullOrWhiteSpace(retrieveCustomerCategoryObj.Status) && retrieveCustomerCategoryObj.Status.ToLower() != "both")
                    {
                        strXMLForCustomerCategories += "<condition attribute='statecode' operator='eq' value='" + retrieveCustomerCategoryObj.Status + @"' />";
                    }
                    if (!string.IsNullOrWhiteSpace(retrieveCustomerCategoryObj.CategoryId))
                    {
                        strXMLForCustomerCategories += "<condition attribute='tc_categoryid' operator='eq' value='" + retrieveCustomerCategoryObj.CategoryId + @"' />";
                    }

                    strXMLForCustomerCategories += "</filter>";
                }
            }

            strXMLForCustomerCategories += "</entity></fetch>";

            return strXMLForCustomerCategories;
        }

        internal string GetChannelOrigin(RqtObjChannelOrigin rqtObjChannelOrigin)
        {
            string strXMLForChannelOrigins = string.Empty;

            strXMLForChannelOrigins = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='tc_channelorigin'>
                                    <attribute name='tc_channeloriginid'/>
                                    <attribute name='tc_name'/>
                                    <attribute name='tc_arabicname'/>
                                    <attribute name='tc_origincode'/>
                                    <attribute name='tc_channeltype'/>
                                    <order descending='false' attribute='tc_name'/>
                                    <filter type='and'>
                                    <condition attribute='statecode' operator='eq' value='0' />";

            if (!string.IsNullOrWhiteSpace(rqtObjChannelOrigin.Name))
            {
                strXMLForChannelOrigins += "<condition attribute='tc_name' operator='eq' value='" + rqtObjChannelOrigin.Name + @"' />";
            }
            if (!string.IsNullOrWhiteSpace(rqtObjChannelOrigin.ArabicName))
            {
                strXMLForChannelOrigins += "<condition attribute='tc_arabicname' operator='eq' value='" + rqtObjChannelOrigin.ArabicName + @"' />";
            }
            if (!string.IsNullOrWhiteSpace(rqtObjChannelOrigin.ChannelOriginCode))
            {
                strXMLForChannelOrigins += "<condition attribute='tc_origincode' operator='eq' value='" + rqtObjChannelOrigin.ChannelOriginCode + @"' />";
            }
            if (rqtObjChannelOrigin.ChannelType != int.MinValue && rqtObjChannelOrigin.ChannelType != 0)
            {
                strXMLForChannelOrigins += "<condition attribute='tc_channeltype' operator='eq' value='" + rqtObjChannelOrigin.ChannelType + @"' />";
            }

            strXMLForChannelOrigins += "</filter></entity></fetch>";

            return strXMLForChannelOrigins;
        }

        internal string GetCaseFromCaseRefNo(string strCaseRefNo)
        {
            string strXMLForCase = string.Empty;

            if (!string.IsNullOrWhiteSpace(strCaseRefNo))
            {
                strXMLForCase = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='incident'>
                                    <attribute name='incidentid'/>
                                    <attribute name='title'/>                                    
                                    <order descending='false' attribute='createdon'/>
                                    <filter type='and'>
                                    <condition attribute='tc_caserefno' operator='eq' value='" + strCaseRefNo + @"' />
                                    </filter>
                                    </entity>
                                   </fetch>";
            }

            return strXMLForCase;
        }

        internal string GetSocialMediaHandle(RqtObjSocialHandleDetails objSocialHandleDetails)
        {
            string strXMLForSocialMediaHandles = string.Empty;

            if (!string.IsNullOrWhiteSpace(objSocialHandleDetails.UserId))
            {
                strXMLForSocialMediaHandles = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                    <entity name='tc_socialmediachannel'>
                                    <attribute name='tc_socialmediachannelid'/>
                                    <attribute name='tc_name'/>
                                    <attribute name='tc_channelhandle'/>
                                    <attribute name='tc_customer'/>
                                    <attribute name='tc_socialuserid'/>
                                    <order descending='false' attribute='tc_name'/>
                                        <filter type='and'>
                                            <condition attribute='tc_socialuserid' operator='eq' value='" + objSocialHandleDetails.UserId + @"' />
                                        </filter>";

                if (!string.IsNullOrWhiteSpace(objSocialHandleDetails.HandleType))
                {
                    strXMLForSocialMediaHandles += "<link-entity name='tc_channelorigin' from='tc_channeloriginid' to='tc_channelhandle' link-type='inner' alias='co'><filter type='and'><condition attribute='tc_origincode' operator='eq' value='" + objSocialHandleDetails.HandleType + @"' /></filter></link-entity>";
                }
                strXMLForSocialMediaHandles += "</entity></fetch>";
            }

            return strXMLForSocialMediaHandles;
        }
    }
}