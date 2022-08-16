using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.Case
{
    public class PopulateCaseInfo : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region "Load CRM Service from context"

            CommonPluginExtensions objCommonForAllPlugins = new CommonPluginExtensions(serviceProvider);
            objCommonForAllPlugins.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion            

            if (CheckCasePluginsDepth(objCommonForAllPlugins.pluginContext))
            {
                return;
            }

            if (objCommonForAllPlugins.pluginContext.PrimaryEntityName == CaseEntityAttributeNames.EntityLogicalName)
            {
                try
                {
                    PopulateCaseInfoPresenter caseInfoPresenter = new PopulateCaseInfoPresenter();

                    if (objCommonForAllPlugins.pluginContext.Stage == (int)PluginStages.PreOperation)
                    {
                        switch (objCommonForAllPlugins.pluginContext.MessageName)
                        {
                            case PluginHelperStrigs.CreateMsgName:
                            case PluginHelperStrigs.UpdateMsgName:
                                caseInfoPresenter.SetCaseRequiredInfo_OnPreOperation(objCommonForAllPlugins);
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

        public bool CheckCasePluginsDepth(IPluginExecutionContext exeContext)
        {
            bool checkDepth = false;

            if (exeContext.Depth > 1 && exeContext.ParentContext != null && exeContext.ParentContext.ParentContext != null && exeContext.ParentContext.ParentContext.PrimaryEntityName == CaseEntityAttributeNames.EntityLogicalName)
            {
                checkDepth = true;
            }
            return checkDepth;
        }
    }
}