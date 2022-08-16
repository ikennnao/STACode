using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class PopulateCaseInfoPresenter
    {
        private CommonMethods commonMethods = new CommonMethods();

        public void SetCaseRequiredInfo_OnPreOperation(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPreImageCase = commonMethods.RetrievePreImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCase != null)
            {
                EntityReference entrefCustomer = null, entrefCaseType = null, entrefSubCategory = null;
                OptionSetValue optsetProCode = null, optsetServiceRelatedTo = null, optsetOOBCaseType = null;
                string strCaseCustName = string.Empty, strCaseEmail = string.Empty, strCaseContactNo = string.Empty;

                entrefCustomer = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.Customer);
                entrefCaseType = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.CaseType);
                entrefSubCategory = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.Subcatgeory);
                optsetProCode = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.PriorityCode);
                optsetServiceRelatedTo = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.ServiceRelatedTo);
                optsetOOBCaseType = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetCase, entPreImageCase, CaseEntityAttributeNames.OOBCaseTypeCode);

                #region Populate Customer Name, Email Address and Contact Number Info. from Target Case Customer field

                if (entrefCustomer != null && entrefCustomer.Id != Guid.Empty && !string.IsNullOrWhiteSpace(entrefCustomer.LogicalName))
                {
                    Entity entCustomer = null;
                    ColumnSet colsetCustomer = null;

                    switch (entrefCustomer.LogicalName)
                    {
                        case ContactEntityAttributeNames.EntityLogicalName:
                            colsetCustomer = new ColumnSet(ContactEntityAttributeNames.FirstName, ContactEntityAttributeNames.LastName, ContactEntityAttributeNames.PrimaryEmail, ContactEntityAttributeNames.PrimaryContactNo, ContactEntityAttributeNames.SecondaryEmail, ContactEntityAttributeNames.SecondaryContactNo, ContactEntityAttributeNames.CustomerCategory, ContactEntityAttributeNames.RelationshipType);
                            break;
                        case AccountEntityAttributeNames.EntityLogicalName:
                            colsetCustomer = new ColumnSet(AccountEntityAttributeNames.AccountName, AccountEntityAttributeNames.EmailAddress, AccountEntityAttributeNames.ContactNumber);
                            break;
                    }

                    entCustomer = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefCustomer.LogicalName, entrefCustomer.Id, colsetCustomer);

                    if (entCustomer != null && entCustomer.Attributes.Count > 0)
                    {
                        OptionSetValue optsetRelationshipType = null;

                        if (entCustomer.LogicalName == ContactEntityAttributeNames.EntityLogicalName)
                        {
                            optsetRelationshipType = commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.RelationshipType);
                        }

                        if (optsetRelationshipType == null || (optsetRelationshipType != null && optsetRelationshipType.Value == (int)CustomerTypeCode.Customer))
                        {
                            #region Set Customer Name in Target Case 'Customer Name'

                            string strCustomerName = string.Empty;

                            switch (entrefCustomer.LogicalName)
                            {
                                case ContactEntityAttributeNames.EntityLogicalName:
                                    strCustomerName = commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.FirstName) + " " + commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.LastName);
                                    break;
                                case AccountEntityAttributeNames.EntityLogicalName:
                                    strCustomerName = commonMethods.GetAttributeValFromTargetEntity(entCustomer, AccountEntityAttributeNames.AccountName);
                                    break;
                            }

                            if (string.IsNullOrWhiteSpace(strCustomerName))
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.CustomerName] = strCustomerName;
                            }

                            #endregion

                            #region Set Customer Email in Target Case Email Address

                            if (string.IsNullOrWhiteSpace(strCustomerName))
                            {
                                switch (entrefCustomer.LogicalName)
                                {
                                    case ContactEntityAttributeNames.EntityLogicalName:
                                        entTargetCase.Attributes[CaseEntityAttributeNames.EmailAddress] = commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.PrimaryEmail) != null ? commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.PrimaryEmail) : commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.SecondaryEmail);
                                        break;
                                    case AccountEntityAttributeNames.EntityLogicalName:
                                        entTargetCase.Attributes[CaseEntityAttributeNames.EmailAddress] = commonMethods.GetAttributeValFromTargetEntity(entCustomer, AccountEntityAttributeNames.EmailAddress);
                                        break;
                                }
                            }

                            #endregion

                            #region Set Customer Contact No. in Target Case Contact No.

                            switch (entrefCustomer.LogicalName)
                            {
                                case ContactEntityAttributeNames.EntityLogicalName:
                                    entTargetCase.Attributes[CaseEntityAttributeNames.ContactNumber] = commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.PrimaryContactNo) != null ? commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.PrimaryContactNo) : commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.SecondaryContactNo);
                                    break;
                                case AccountEntityAttributeNames.EntityLogicalName:
                                    entTargetCase.Attributes[CaseEntityAttributeNames.ContactNumber] = commonMethods.GetAttributeValFromTargetEntity(entCustomer, AccountEntityAttributeNames.ContactNumber);
                                    break;
                            }

                            #endregion

                            #region Set Customer Category in Target Case Customer

                            bool isRealTimeCustomer = false;
                            EntityReference entrefCustCategory = null;

                            switch (entrefCustomer.LogicalName)
                            {
                                case ContactEntityAttributeNames.EntityLogicalName:
                                    entrefCustCategory = commonMethods.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributeNames.CustomerCategory);
                                    break;
                                case AccountEntityAttributeNames.EntityLogicalName:
                                    Entity entCustomerCategory = commonMethods.RetrieveRelatedCustomerCategory(objCommonForAllPlugins.srvContextUsr, "1"); //B2B - Tourist Trade Provider
                                    if (entCustomerCategory != null && entCustomerCategory.Id != Guid.Empty)
                                    {
                                        entrefCustCategory = entCustomerCategory.ToEntityReference();
                                        isRealTimeCustomer = true;
                                    }
                                    break;
                            }

                            if (entrefCustCategory != null && entrefCustCategory.Id != Guid.Empty && !isRealTimeCustomer)
                            {
                                Entity entCustomerCategoryDetails = objCommonForAllPlugins.srvContextUsr.Retrieve(CustomerCategoryEntityAttributeNames.EntityLogicalName, entrefCustCategory.Id,
                                    new ColumnSet(CustomerCategoryEntityAttributeNames.CategroryId));

                                if (entCustomerCategoryDetails != null && entCustomerCategoryDetails.Attributes.Count > 0)
                                {
                                    string strCustCategoryId = commonMethods.GetAttributeValFromTargetEntity(entCustomerCategoryDetails, CustomerCategoryEntityAttributeNames.CategroryId);

                                    if (strCustCategoryId != "0")
                                    {
                                        isRealTimeCustomer = true;
                                    }
                                }
                            }

                            if (isRealTimeCustomer)
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.CustomerCategory] = entrefCustCategory;
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                #region Populate Case Type Code (i.e., OOB Optionset Field) from Custom Case Type Lookup field

                if (entrefCaseType != null && entrefCaseType.Id != Guid.Empty)
                {
                    if (string.IsNullOrWhiteSpace(entrefCaseType.Name))
                    {
                        Entity entCaseType = null;
                        entCaseType = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseTypeEntityAttributeNames.EntityLogicalName, entrefCaseType.Id, new ColumnSet(CaseTypeEntityAttributeNames.CaseTypeName));

                        if (entCaseType != null && entCaseType.Attributes.Count > 0)
                        {
                            entrefCaseType.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseType, CaseTypeEntityAttributeNames.CaseTypeName);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(entrefCaseType.Name))
                    {
                        int intCaseTypeCode = int.MinValue;

                        switch (entrefCaseType.Name.ToLower())
                        {
                            case "enquiry":
                                intCaseTypeCode = (int)CaseTypeCode.Enquiry;
                                break;
                            case "complaint":
                                intCaseTypeCode = (int)CaseTypeCode.Complaint;
                                break;
                            case "suggestion":
                                intCaseTypeCode = (int)CaseTypeCode.Suggestion;
                                break;
                            case "out of scope":
                                intCaseTypeCode = (int)CaseTypeCode.OutofScope;
                                break;
                            case "emergency":
                                intCaseTypeCode = (int)CaseTypeCode.Emergency;
                                break;
                        }

                        if (intCaseTypeCode != int.MinValue)
                        {
                            if (optsetOOBCaseType == null || optsetOOBCaseType.Value == int.MinValue || (optsetOOBCaseType != null && optsetOOBCaseType.Value != intCaseTypeCode))
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.OOBCaseTypeCode] = new OptionSetValue(intCaseTypeCode);
                            }
                        }
                    }
                }

                #endregion

                #region Populate Default Priority & Service Related To from related Case SubCategory

                if (entrefSubCategory != null && entrefSubCategory.Id != Guid.Empty)
                {
                    Entity entCaseSubcategory = null;

                    if (optsetProCode == null || optsetProCode.Value == int.MinValue || optsetServiceRelatedTo == null || optsetServiceRelatedTo.Value == int.MinValue)
                    {
                        entCaseSubcategory = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefSubCategory.LogicalName, entrefSubCategory.Id, new ColumnSet(CaseSubcategoryEntityAttributeNames.DefaultPriority, CaseSubcategoryEntityAttributeNames.ServiceRelatedTo, CaseSubcategoryEntityAttributeNames.CaseSolvedBy));
                    }

                    if (entCaseSubcategory != null && entCaseSubcategory.Attributes.Count > 0)
                    {
                        #region Set Default Priority as Case Priority, if Priority on Target Case is 'Null'

                        if (optsetProCode == null || optsetProCode.Value == int.MinValue)
                        {
                            OptionSetValue optstDefaultPriority = commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.DefaultPriority);

                            if (optstDefaultPriority != null && optstDefaultPriority.Value != int.MinValue)
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.PriorityCode] = new OptionSetValue(optstDefaultPriority.Value);
                            }
                        }

                        #endregion

                        #region Set Service Related To on Target Case, if Service Related To on Target Case is 'Null'

                        OptionSetValue optsetServiceRelatedToRetrieved = commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.ServiceRelatedTo);

                        if (optsetServiceRelatedToRetrieved != null && optsetServiceRelatedToRetrieved.Value != int.MinValue)
                        {
                            if (optsetServiceRelatedTo == null || optsetServiceRelatedTo.Value == int.MinValue || optsetServiceRelatedToRetrieved.Value != optsetServiceRelatedTo.Value)
                            {
                                entTargetCase.Attributes[CaseEntityAttributeNames.ServiceRelatedTo] = new OptionSetValue(optsetServiceRelatedToRetrieved.Value);
                            }
                        }
                        else
                        {
                            entTargetCase.Attributes[CaseEntityAttributeNames.ServiceRelatedTo] = null;
                        }

                        #endregion

                        #region Set Send Suggestion To on Target Case, if 'Send Suggestion To' on Target Case is 'Null'

                        string strSendSuggestionTo = commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.CaseSolvedBy);
                        entTargetCase.Attributes[CaseEntityAttributeNames.SendSuggestionTo] = strSendSuggestionTo;

                        #endregion
                    }
                }

                #endregion                
            }
        }

        public void SetCaseServiceRelatedTo_OnPreOperation(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCase = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCase != null)
            {
                EntityReference entrefSubCategory = null;
                OptionSetValue optsetServiceRelatedTo = null;

                entrefSubCategory = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.Subcatgeory);
                optsetServiceRelatedTo = commonMethods.GetAttributeValFromTargetEntity(entTargetCase, CaseEntityAttributeNames.ServiceRelatedTo);

                #region If Sub Category on Target Case is changed then, Populate 'Service Related To' from related Case SubCategory

                if (entrefSubCategory != null && entrefSubCategory.Id != Guid.Empty)
                {
                    Entity entCaseSubcategory = null;
                    entCaseSubcategory = objCommonForAllPlugins.srvContextUsr.Retrieve(entrefSubCategory.LogicalName, entrefSubCategory.Id, new ColumnSet(CaseSubcategoryEntityAttributeNames.ServiceRelatedTo));

                    if (entCaseSubcategory != null && entCaseSubcategory.Attributes.Count > 0)
                    {
                        #region Set Service Related To on Target Case, if Service Related To on Target Case is 'Null'

                        OptionSetValue optstServiceRelatedTo = commonMethods.GetAttributeValFromTargetEntity(entCaseSubcategory, CaseSubcategoryEntityAttributeNames.ServiceRelatedTo);

                        if (optstServiceRelatedTo != null && optstServiceRelatedTo.Value != int.MinValue)
                        {
                            entTargetCase.Attributes[CaseEntityAttributeNames.ServiceRelatedTo] = new OptionSetValue(optstServiceRelatedTo.Value);
                        }

                        #endregion
                    }
                }

                #endregion
            }
        }
    }
}