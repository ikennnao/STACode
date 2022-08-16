using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.KnowledgeArticle
{
    public class PopulateKeywordsInKA : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region "Load CRM Service from context"

            CommonPluginExtensions objCommonForAllPlugins = new CommonPluginExtensions(serviceProvider);
            objCommonForAllPlugins.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            if (objCommonForAllPlugins.pluginContext.PrimaryEntityName == KnowledgeArticleEntityAttributeNames.EntityLogicalName)
            {
                try
                {
                    PopulateKAInfoPresenter kaInfoPresenter = new PopulateKAInfoPresenter();

                    if (objCommonForAllPlugins.pluginContext.Stage == (int)PluginStages.PreOperation)
                    {
                        switch (objCommonForAllPlugins.pluginContext.MessageName)
                        {
                            case PluginHelperStrigs.CreateMsgName:
                            case PluginHelperStrigs.UpdateMsgName:
                                kaInfoPresenter.SetKeywordsInfoFromKA_OnPreOperation(objCommonForAllPlugins);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
    }
}