using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.CaseApproval
{
    public class RestrictModClosure : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region "Load CRM Service from context"

            CommonPluginExtensions objCommonForAllPlugins = new CommonPluginExtensions(serviceProvider);
            objCommonForAllPlugins.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion            

            if (objCommonForAllPlugins.pluginContext.PrimaryEntityName == CaseApprovalEntityAttributeNames.EntityLogicalName)
            {
                try
                {
                    RestrictModClosurePresenter restrictPresenter = new RestrictModClosurePresenter();

                    if (objCommonForAllPlugins.pluginContext.Stage == (int)PluginStages.PreOperation)
                    {
                        switch (objCommonForAllPlugins.pluginContext.MessageName)
                        {
                            case PluginHelperStrigs.UpdateMsgName:
                                restrictPresenter.ExecuteModClosureTask(objCommonForAllPlugins);
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