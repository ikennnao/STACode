using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Workflows.Resources;
using System.Collections.Generic;
using System.Linq;

namespace STA.TouristCareCRM.Workflows.Helpers
{
    public class CommonMethods
    {
        private CRMFetchXMLs cRMFetchXMLs = new CRMFetchXMLs();

        public Entity RetrieveTargetEntityFromContext(IWorkflowContext exeContext)
        {
            Entity entTarget = null;
            if (exeContext.InputParameters.Contains(PluginHelperStrigs.Target) && exeContext.InputParameters[PluginHelperStrigs.Target] is Entity)
            {
                entTarget = (Entity)exeContext.InputParameters[PluginHelperStrigs.Target];
            }
            return entTarget;
        }

        public Entity RetrievePreImageEntityFromContext(IWorkflowContext exeContext)
        {
            Entity entPreImage = null;
            if (exeContext.PreEntityImages.Contains(PluginHelperStrigs.PreImageName) && exeContext.PreEntityImages[PluginHelperStrigs.PreImageName] is Entity)
            {
                entPreImage = exeContext.PreEntityImages[PluginHelperStrigs.PreImageName];
            }
            return entPreImage;
        }

        public Entity RetrievePostImageEntityFromContext(IWorkflowContext exeContext)
        {
            Entity entPostImage = null;
            if (exeContext.PostEntityImages.Contains(PluginHelperStrigs.PostImageName) && exeContext.PostEntityImages[PluginHelperStrigs.PostImageName] is Entity)
            {
                entPostImage = exeContext.PostEntityImages[PluginHelperStrigs.PostImageName];
            }
            return entPostImage;
        }

        public EntityReference RetrieveEntityMonikerEntRefFromContext(IWorkflowContext exeContext)
        {
            EntityReference entrefTarget = null;
            if (exeContext.InputParameters.Contains(PluginHelperStrigs.EntityMoniker) && exeContext.InputParameters[PluginHelperStrigs.EntityMoniker] is Entity)
            {
                entrefTarget = (EntityReference)exeContext.InputParameters[PluginHelperStrigs.EntityMoniker];
            }
            return entrefTarget;
        }

        public dynamic GetAttributeValFromTargetEntity(Entity entTarget, string strAttributeName)
        {
            dynamic otptAttributeVale = null;
            if (!string.IsNullOrWhiteSpace(strAttributeName) && entTarget != null && entTarget.Attributes.Contains(strAttributeName))
            {
                otptAttributeVale = entTarget.Attributes[strAttributeName];
            }
            return otptAttributeVale;
        }

        public dynamic GetAttributeValFromTargetOrImageEntities(Entity entTarget, Entity entImage, string strAttributeName)
        {
            dynamic otptAttributeVale = null;
            if (!string.IsNullOrWhiteSpace(strAttributeName))
            {
                if (entTarget != null && entTarget.Attributes.Contains(strAttributeName))
                {
                    otptAttributeVale = entTarget.Attributes[strAttributeName];
                }
                else if (entImage != null && entImage.Attributes.Contains(strAttributeName))
                {
                    otptAttributeVale = entImage.Attributes[strAttributeName];
                }
            }
            return otptAttributeVale;
        }

        public dynamic RetrieveAliasAttributeValue(Entity entTarget, string strAttributeName, string strAlias)
        {
            dynamic otptAttributeVale = null;

            if (!string.IsNullOrWhiteSpace(strAttributeName) && !string.IsNullOrWhiteSpace(strAlias) && entTarget.Attributes.Contains(strAlias + strAttributeName))
            {
                AliasedValue otptAliasedValue = null;
                otptAliasedValue = (AliasedValue)entTarget.Attributes[strAlias + strAttributeName];

                if (otptAliasedValue != null)
                {
                    otptAttributeVale = otptAliasedValue.Value;
                }
            }
            return otptAttributeVale;
        }

        public dynamic GetAttributeFormattedValFromTargetOrImageEntities(Entity entTarget, Entity entImage, string strAttributeName)
        {
            dynamic otptAttrOptionsetVale = null;
            if (!string.IsNullOrWhiteSpace(strAttributeName))
            {
                if (entTarget != null && entTarget.FormattedValues.Contains(strAttributeName))
                {
                    otptAttrOptionsetVale = entTarget.FormattedValues[strAttributeName];
                }
                else if (entImage != null && entImage.FormattedValues.Contains(strAttributeName))
                {
                    otptAttrOptionsetVale = entImage.FormattedValues[strAttributeName];
                }
            }
            return otptAttrOptionsetVale;
        }

        public Dictionary<string, string> RetrieveMultipleConfigParaValFromConfigParams(IOrganizationService orgService, List<string> lststrConfigParamKeys)
        {
            Dictionary<string, string> dicConfigParamKeysVals = null;

            string strRetrieveConfigParamRecord = cRMFetchXMLs.GetListOfConfigParamRecords(lststrConfigParamKeys);

            if (!string.IsNullOrWhiteSpace(strRetrieveConfigParamRecord))
            {
                EntityCollection entcolConfigParams = null;
                entcolConfigParams = orgService.RetrieveMultiple(new FetchExpression(strRetrieveConfigParamRecord));

                if (entcolConfigParams != null && entcolConfigParams.Entities != null && entcolConfigParams.Entities.Count > 0)
                {
                    dicConfigParamKeysVals = new Dictionary<string, string>();

                    foreach (Entity entConfigParam in entcolConfigParams.Entities)
                    {
                        string strConfigParamKey = GetAttributeValFromTargetEntity(entConfigParam, ConfigParamsEntityAttributeNames.ConfigParamKey);
                        string strConfigParamVal = GetAttributeValFromTargetEntity(entConfigParam, ConfigParamsEntityAttributeNames.ConfigParamValue);

                        dicConfigParamKeysVals.Add(strConfigParamKey.ToLower(), strConfigParamVal);
                    }
                }
            }

            return dicConfigParamKeysVals;
        }

        public string RetrieveConfigParaValFromConfigParams(IOrganizationService orgService, string strConfigParamKey)
        {
            string strConfigParamVal = string.Empty;

            string strRetrieveConfigParamRecord = cRMFetchXMLs.GetRequiredConfigParamRecord(strConfigParamKey);

            if (!string.IsNullOrWhiteSpace(strConfigParamKey))
            {
                EntityCollection entcolConfigParams = null;
                entcolConfigParams = orgService.RetrieveMultiple(new FetchExpression(strRetrieveConfigParamRecord));

                if (entcolConfigParams != null && entcolConfigParams.Entities != null && entcolConfigParams.Entities.Count == 1)
                {
                    strConfigParamVal = GetAttributeValFromTargetEntity(entcolConfigParams.Entities[0], ConfigParamsEntityAttributeNames.ConfigParamValue);
                }
            }

            return strConfigParamVal;
        }

        public Entity GetEmailTemplateDetails(IOrganizationService orgService, string strTemplateName, int intTemplateTypeCode, int intTemplateLanguageCode)
        {
            Entity entTemplateDetails = null;

            if (!string.IsNullOrWhiteSpace(strTemplateName) && intTemplateLanguageCode != int.MinValue && intTemplateLanguageCode != int.MinValue)
            {
                string strFetchForTemplateDetails = cRMFetchXMLs.RetrieveTemplateRcrdDetails(strTemplateName, intTemplateTypeCode, intTemplateLanguageCode);

                if (!string.IsNullOrWhiteSpace(strFetchForTemplateDetails))
                {
                    EntityCollection entcolTemplates = orgService.RetrieveMultiple(new FetchExpression(strFetchForTemplateDetails));

                    if (entcolTemplates != null && entcolTemplates.Entities != null && entcolTemplates.Entities.Count > 0)
                    {
                        entTemplateDetails = entcolTemplates.Entities.FirstOrDefault();
                    }
                }
            }

            return entTemplateDetails;
        }

        public Entity RetrieveAnonymousCustomer(IOrganizationService orgService)
        {
            Entity entAnonymousCust = null;

            string strQueryForAnonymousCust = cRMFetchXMLs.GetQueryForAnonymousCustomer();

            if (!string.IsNullOrWhiteSpace(strQueryForAnonymousCust))
            {
                EntityCollection entcolAnonymousCust = null;
                entcolAnonymousCust = orgService.RetrieveMultiple(new FetchExpression(strQueryForAnonymousCust));

                if (entcolAnonymousCust != null && entcolAnonymousCust.Entities != null && entcolAnonymousCust.Entities.Count > 0)
                {
                    entAnonymousCust = entcolAnonymousCust.Entities[0];
                }
            }

            return entAnonymousCust;
        }
    }
}