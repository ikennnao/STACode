using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class PopulateKAInfoPresenter
    {
        private CommonMethods commonMethods = new CommonMethods();

        public void SetKeywordsInfoFromKA_OnPreOperation(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetKA = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPreImageKA = commonMethods.RetrievePreImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetKA != null)
            {
                EntityReference entrefCaseCategory = null, entrefCaseSubCategory = null;
                string strKAKeywords = string.Empty, strNewKeywordsForKA = string.Empty;

                strKAKeywords = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetKA, entPreImageKA, KnowledgeArticleEntityAttributeNames.Keywords);
                entrefCaseCategory = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetKA, entPreImageKA, KnowledgeArticleEntityAttributeNames.CaseCategory);
                entrefCaseSubCategory = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetKA, entPreImageKA, KnowledgeArticleEntityAttributeNames.CaseSubcategory);

                strNewKeywordsForKA = GetNewKeywordsInfo(objCommonForAllPlugins, entrefCaseCategory, entrefCaseSubCategory, strKAKeywords);

                if (!string.IsNullOrWhiteSpace(strNewKeywordsForKA))
                {
                    entTargetKA.Attributes[KnowledgeArticleEntityAttributeNames.Keywords] = strNewKeywordsForKA;
                }
            }
        }

        public void SetKeywordsInfoFromKAC_OnPostOperation(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetKAC = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPreImageKAC = commonMethods.RetrievePreImageEntityFromContext(objCommonForAllPlugins.pluginContext);
            Entity entPostImageKAC = commonMethods.RetrievePostImageEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetKAC != null)
            {
                EntityReference entrefCaseCategory = null, entrefCaseSubCategory = null, entrefKnowledgeArticle = null;
                OptionSetValue otptStatus = null;

                entrefKnowledgeArticle = commonMethods.GetAttributeValFromTargetEntity(entTargetKAC, KACategorizationEntityAttributeNames.KnowledgeArticle);
                entrefCaseCategory = commonMethods.GetAttributeValFromTargetEntity(entTargetKAC, KACategorizationEntityAttributeNames.CaseCategory);
                entrefCaseSubCategory = commonMethods.GetAttributeValFromTargetEntity(entTargetKAC, KACategorizationEntityAttributeNames.CaseSubcategory);
                otptStatus = commonMethods.GetAttributeValFromTargetEntity(entTargetKAC, KACategorizationEntityAttributeNames.Status);

                if (entrefKnowledgeArticle != null || entrefCaseCategory != null || entrefCaseSubCategory != null || otptStatus != null)
                {
                    EntityReference entrefPostImageCaseCategory = null, entrefPostImageCaseSubCategory = null, entrefPostImageKnowledgeArticle = null;
                    OptionSetValue otptPostImageStatus = null;

                    entrefPostImageKnowledgeArticle = commonMethods.GetAttributeValFromTargetEntity(entPostImageKAC, KACategorizationEntityAttributeNames.KnowledgeArticle);
                    entrefPostImageCaseCategory = commonMethods.GetAttributeValFromTargetEntity(entPostImageKAC, KACategorizationEntityAttributeNames.CaseCategory);
                    entrefPostImageCaseSubCategory = commonMethods.GetAttributeValFromTargetEntity(entPostImageKAC, KACategorizationEntityAttributeNames.CaseSubcategory);
                    otptPostImageStatus = commonMethods.GetAttributeValFromTargetEntity(entPostImageKAC, KACategorizationEntityAttributeNames.Status);

                    int intStatus = otptPostImageStatus != null && otptPostImageStatus.Value != int.MinValue ? otptPostImageStatus.Value : int.MinValue;

                    string strTargetKAKeywords = string.Empty, strNewKeywordsForTargetKA = string.Empty;
                    Entity entRetrievedKA = objCommonForAllPlugins.srvContextUsr.Retrieve(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPostImageKnowledgeArticle.Id, new ColumnSet(KnowledgeArticleEntityAttributeNames.Keywords));

                    if (entRetrievedKA != null)
                    {
                        strTargetKAKeywords = commonMethods.GetAttributeValFromTargetEntity(entRetrievedKA, KnowledgeArticleEntityAttributeNames.Keywords);
                    }

                    #region If Target KAC is 'Active'

                    if (intStatus == (int)EntityStateCode.Active)
                    {
                        strNewKeywordsForTargetKA = GetNewKeywordsInfo(objCommonForAllPlugins, entrefPostImageCaseCategory, entrefPostImageCaseSubCategory, strTargetKAKeywords);

                        if (!string.IsNullOrWhiteSpace(strNewKeywordsForTargetKA))
                        {
                            Entity entUpdateKeywordsInKA = new Entity(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPostImageKnowledgeArticle.Id);
                            entUpdateKeywordsInKA.Attributes[KnowledgeArticleEntityAttributeNames.Keywords] = strNewKeywordsForTargetKA;
                            objCommonForAllPlugins.srvContextUsr.Update(entUpdateKeywordsInKA);
                        }

                        #region If KA field is updated with a new value, then remove the Category & Sub Category Names in the Pre Image KA Record

                        if (entPreImageKAC != null && entPreImageKAC.Attributes.Count > 0)
                        {
                            EntityReference entrefPreImageKA = null, entrefPreImageCaseCategory = null, entrefPreImageCaseSubCategory = null;
                            bool isRelatedKAChanged = false, isCaseCategoryChanged = false, isCaseSubCategoryChanged = false;

                            entrefPreImageKA = commonMethods.GetAttributeValFromTargetEntity(entPreImageKAC, KACategorizationEntityAttributeNames.KnowledgeArticle);
                            entrefPreImageCaseCategory = commonMethods.GetAttributeValFromTargetEntity(entPreImageKAC, KACategorizationEntityAttributeNames.CaseCategory);
                            entrefPreImageCaseSubCategory = commonMethods.GetAttributeValFromTargetEntity(entPreImageKAC, KACategorizationEntityAttributeNames.CaseSubcategory);

                            if (entrefPreImageKA != null && entrefPreImageKA.Id != Guid.Empty && entrefPreImageKA.Id != entrefPostImageKnowledgeArticle.Id)
                            {
                                isRelatedKAChanged = true;
                            }
                            if (entrefPreImageCaseCategory != null && entrefPreImageCaseCategory.Id != Guid.Empty && entrefPreImageCaseCategory.Id != entrefPostImageCaseCategory.Id)
                            {
                                isCaseCategoryChanged = true;
                            }
                            if (entrefPreImageCaseSubCategory != null && entrefPreImageCaseSubCategory.Id != Guid.Empty && entrefPreImageCaseSubCategory.Id != entrefPostImageCaseSubCategory.Id)
                            {
                                isCaseSubCategoryChanged = true;
                            }

                            if (isRelatedKAChanged)
                            {
                                string strOldKAKeywords = string.Empty, strNewKeywordsForOldKA = string.Empty;
                                Entity entRetrievedOldKA = objCommonForAllPlugins.srvContextUsr.Retrieve(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPreImageKA.Id, new ColumnSet(KnowledgeArticleEntityAttributeNames.Keywords));

                                if (entRetrievedOldKA != null)
                                {
                                    strOldKAKeywords = commonMethods.GetAttributeValFromTargetEntity(entRetrievedOldKA, KnowledgeArticleEntityAttributeNames.Keywords);
                                }

                                if (!string.IsNullOrWhiteSpace(strOldKAKeywords))
                                {
                                    strNewKeywordsForOldKA = RemoveOldKeywordsInfoInRelatedKA(objCommonForAllPlugins, entrefPostImageCaseCategory, entrefPostImageCaseSubCategory, strOldKAKeywords, true, true);

                                    Entity entUpdateKeywordsInKA = new Entity(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPreImageKA.Id);
                                    entUpdateKeywordsInKA.Attributes[KnowledgeArticleEntityAttributeNames.Keywords] = strNewKeywordsForOldKA;
                                    objCommonForAllPlugins.srvContextUsr.Update(entUpdateKeywordsInKA);
                                }
                            }

                            if (isCaseCategoryChanged || isCaseSubCategoryChanged)
                            {
                                string strOldKAKeywords = string.Empty, strNewKeywordsForOldKA = string.Empty;
                                Entity entRetrievedOldKA = objCommonForAllPlugins.srvContextUsr.Retrieve(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPostImageKnowledgeArticle.Id, new ColumnSet(KnowledgeArticleEntityAttributeNames.Keywords));

                                if (entRetrievedOldKA != null)
                                {
                                    strOldKAKeywords = commonMethods.GetAttributeValFromTargetEntity(entRetrievedOldKA, KnowledgeArticleEntityAttributeNames.Keywords);
                                }

                                if (!string.IsNullOrWhiteSpace(strOldKAKeywords))
                                {
                                    strNewKeywordsForOldKA = RemoveOldKeywordsInfoInRelatedKA(objCommonForAllPlugins, entrefPreImageCaseCategory, entrefPreImageCaseSubCategory, strOldKAKeywords, isCaseCategoryChanged, isCaseSubCategoryChanged);

                                    Entity entUpdateKeywordsInKA = new Entity(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPostImageKnowledgeArticle.Id);
                                    entUpdateKeywordsInKA.Attributes[KnowledgeArticleEntityAttributeNames.Keywords] = strNewKeywordsForOldKA;
                                    objCommonForAllPlugins.srvContextUsr.Update(entUpdateKeywordsInKA);
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region If Target KAC is 'InActive'

                    else if (intStatus == (int)EntityStateCode.InActive)
                    {
                        string strOldKAKeywords = string.Empty, strNewKeywordsForOldKA = string.Empty;
                        Entity entRetrievedOldKA = objCommonForAllPlugins.srvContextUsr.Retrieve(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPostImageKnowledgeArticle.Id, new ColumnSet(KnowledgeArticleEntityAttributeNames.Keywords));

                        if (entRetrievedOldKA != null)
                        {
                            strOldKAKeywords = commonMethods.GetAttributeValFromTargetEntity(entRetrievedOldKA, KnowledgeArticleEntityAttributeNames.Keywords);
                        }

                        if (!string.IsNullOrWhiteSpace(strOldKAKeywords))
                        {
                            strNewKeywordsForOldKA = RemoveOldKeywordsInfoInRelatedKA(objCommonForAllPlugins, entrefPostImageCaseCategory, entrefPostImageCaseSubCategory, strOldKAKeywords, true, true);

                            Entity entUpdateKeywordsInKA = new Entity(KnowledgeArticleEntityAttributeNames.EntityLogicalName, entrefPostImageKnowledgeArticle.Id);
                            entUpdateKeywordsInKA.Attributes[KnowledgeArticleEntityAttributeNames.Keywords] = strNewKeywordsForOldKA;
                            objCommonForAllPlugins.srvContextUsr.Update(entUpdateKeywordsInKA);
                        }
                    }

                    #endregion
                }
            }
        }

        public string GetNewKeywordsInfo(CommonPluginExtensions objCommonForAllPlugins, EntityReference entrefCaseCategory, EntityReference entrefCaseSubCategory, string strKeywords)
        {
            string strNewKeywords = string.Empty;

            #region Extract the Formatted Case Category Name

            if (entrefCaseCategory != null)
            {
                #region Get the Case Category Name

                if (string.IsNullOrWhiteSpace(entrefCaseCategory.Name) && entrefCaseCategory.Id != Guid.Empty)
                {
                    Entity entCaseCategory = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseCategoryEntityAttributeNames.EntityLogicalName, entrefCaseCategory.Id, new ColumnSet(CaseCategoryEntityAttributeNames.CaseCategoryName));

                    if (entCaseCategory != null && entCaseCategory.Attributes != null && entCaseCategory.Attributes.Count > 0)
                    {
                        entrefCaseCategory.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseCategory, CaseCategoryEntityAttributeNames.CaseCategoryName);
                    }
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(entrefCaseCategory.Name))
                {
                    if (!string.IsNullOrWhiteSpace(strKeywords))
                    {
                        if (!strKeywords.Contains(entrefCaseCategory.Name))
                        {
                            strNewKeywords += entrefCaseCategory.Name + ",";
                        }
                    }
                    else
                    {
                        strNewKeywords += entrefCaseCategory.Name + ",";
                    }
                }
            }

            #endregion

            #region Extract the Formatted Case Sub Category Name

            if (entrefCaseSubCategory != null)
            {
                #region Get the Case SubCategory Name

                if (string.IsNullOrWhiteSpace(entrefCaseSubCategory.Name) && entrefCaseSubCategory.Id != Guid.Empty)
                {
                    Entity entCaseSubCategory = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseSubcategoryEntityAttributeNames.EntityLogicalName, entrefCaseSubCategory.Id, new ColumnSet(CaseSubcategoryEntityAttributeNames.CaseSubcategoryName));

                    if (entCaseSubCategory != null && entCaseSubCategory.Attributes != null && entCaseSubCategory.Attributes.Count > 0)
                    {
                        entrefCaseSubCategory.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseSubCategory, CaseSubcategoryEntityAttributeNames.CaseSubcategoryName);
                    }
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(entrefCaseSubCategory.Name))
                {
                    if (!string.IsNullOrWhiteSpace(strKeywords))
                    {
                        if (!strKeywords.Contains(entrefCaseSubCategory.Name))
                        {
                            strNewKeywords += entrefCaseSubCategory.Name;
                        }
                    }
                    else
                    {
                        strNewKeywords += entrefCaseSubCategory.Name;
                    }
                }
            }

            #endregion

            #region Extract the Formatted Keywords Value

            if (!string.IsNullOrWhiteSpace(strKeywords))
            {
                if (strKeywords.StartsWith(","))
                {
                    strKeywords = strKeywords.Substring(1);
                }
                if (strKeywords.EndsWith(","))
                {
                    strKeywords = strKeywords.Substring(0, strKeywords.Length - 1);
                }
            }

            #endregion

            if (!string.IsNullOrWhiteSpace(strKeywords))
            {
                if (!string.IsNullOrWhiteSpace(strNewKeywords))
                {
                    strNewKeywords = (strKeywords.StartsWith(",") || strNewKeywords.EndsWith(",")) ? strNewKeywords + strKeywords : strNewKeywords + "," + strKeywords;
                }
                else
                {
                    strNewKeywords = strKeywords;
                }
            }

            #region Append the Case Category Name, Case Subcategory Name in Keywords as Prefix

            if (!string.IsNullOrWhiteSpace(strNewKeywords))
            {
                if (strNewKeywords.StartsWith(","))
                {
                    strNewKeywords = strNewKeywords.Substring(1);
                }
                if (strNewKeywords.EndsWith(","))
                {
                    strNewKeywords = strNewKeywords.Substring(0, strNewKeywords.Length - 1);
                }
            }

            #endregion

            return strNewKeywords;
        }

        public string RemoveOldKeywordsInfoInRelatedKA(CommonPluginExtensions objCommonForAllPlugins, EntityReference entrefCaseCategory, EntityReference entrefCaseSubCategory, string strKeywords, bool isCaseCategoryChanged, bool isCaseSubCategoryChanged)
        {
            string strNewKeywords = string.Empty;

            #region Extract the Formatted Case Category Name

            if (entrefCaseCategory != null && isCaseCategoryChanged)
            {
                #region Get the Case Category Name

                if (string.IsNullOrWhiteSpace(entrefCaseCategory.Name) && entrefCaseCategory.Id != Guid.Empty)
                {
                    Entity entCaseCategory = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseCategoryEntityAttributeNames.EntityLogicalName, entrefCaseCategory.Id, new ColumnSet(CaseCategoryEntityAttributeNames.CaseCategoryName));

                    if (entCaseCategory != null && entCaseCategory.Attributes != null && entCaseCategory.Attributes.Count > 0)
                    {
                        entrefCaseCategory.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseCategory, CaseCategoryEntityAttributeNames.CaseCategoryName);
                    }
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(entrefCaseCategory.Name))
                {
                    if (strKeywords.Contains(entrefCaseCategory.Name + ","))
                    {
                        strKeywords = strKeywords.Replace(entrefCaseCategory.Name + ",", string.Empty);
                    }
                    else if (strKeywords.Contains(entrefCaseCategory.Name))
                    {
                        strKeywords = strKeywords.Replace(entrefCaseCategory.Name, string.Empty);
                    }
                }
            }

            #endregion

            #region Extract the Formatted Case Sub Category Name

            if (entrefCaseSubCategory != null && isCaseSubCategoryChanged)
            {
                #region Get the Case SubCategory Name

                if (string.IsNullOrWhiteSpace(entrefCaseSubCategory.Name) && entrefCaseSubCategory.Id != Guid.Empty)
                {
                    Entity entCaseSubCategory = objCommonForAllPlugins.srvContextUsr.Retrieve(CaseSubcategoryEntityAttributeNames.EntityLogicalName, entrefCaseSubCategory.Id, new ColumnSet(CaseSubcategoryEntityAttributeNames.CaseSubcategoryName));

                    if (entCaseSubCategory != null && entCaseSubCategory.Attributes != null && entCaseSubCategory.Attributes.Count > 0)
                    {
                        entrefCaseSubCategory.Name = commonMethods.GetAttributeValFromTargetEntity(entCaseSubCategory, CaseSubcategoryEntityAttributeNames.CaseSubcategoryName);
                    }
                }

                #endregion

                if (!string.IsNullOrWhiteSpace(entrefCaseSubCategory.Name))
                {
                    if (strKeywords.Contains(entrefCaseSubCategory.Name + ","))
                    {
                        strKeywords = strKeywords.Replace(entrefCaseSubCategory.Name + ",", string.Empty);
                    }
                    else if (strKeywords.Contains(entrefCaseSubCategory.Name))
                    {
                        strKeywords = strKeywords.Replace(entrefCaseSubCategory.Name, string.Empty);
                    }
                }
            }

            #endregion

            #region Extract the Formatted Keywords Value

            if (!string.IsNullOrWhiteSpace(strKeywords))
            {
                if (strKeywords.StartsWith(","))
                {
                    strKeywords = strKeywords.Substring(1);
                }
                if (strKeywords.EndsWith(","))
                {
                    strKeywords = strKeywords.Substring(0, strKeywords.Length - 1);
                }
            }

            #endregion

            strNewKeywords = strKeywords;

            return strNewKeywords;
        }
    }
}