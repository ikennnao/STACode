using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using STA.TouristCareCRM.Plugins.Presenters;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.KACategorization
{
    public class PopulateKeywordsInKA : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region "Load CRM Service from context"

            CommonPluginExtensions objCommonForAllPlugins = new CommonPluginExtensions(serviceProvider);
            objCommonForAllPlugins.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            if (objCommonForAllPlugins.pluginContext.PrimaryEntityName == KACategorizationEntityAttributeNames.EntityLogicalName)
            {
                try
                {
                    PopulateKAInfoPresenter kaInfoPresenter = new PopulateKAInfoPresenter();

                    if (objCommonForAllPlugins.pluginContext.Stage == (int)PluginStages.PostOperation)
                    {
                        switch (objCommonForAllPlugins.pluginContext.MessageName)
                        {
                            case PluginHelperStrigs.CreateMsgName:
                            case PluginHelperStrigs.UpdateMsgName:
                                kaInfoPresenter.SetKeywordsInfoFromKAC_OnPostOperation(objCommonForAllPlugins);
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