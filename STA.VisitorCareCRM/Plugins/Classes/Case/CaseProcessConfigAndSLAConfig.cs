using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.Case
{
    public class CaseProcessConfigAndSLAConfig : IPlugin
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
                    CaseProcessConfigPresenter caseProcessConfigPresenter = new CaseProcessConfigPresenter();
                    Entity entUpdateCaseAccordingly = new Entity(objCommonForAllPlugins.pluginContext.PrimaryEntityName, objCommonForAllPlugins.pluginContext.PrimaryEntityId);

                    switch (objCommonForAllPlugins.pluginContext.MessageName)
                    {
                        case PluginHelperStrigs.CreateMsgName:
                            switch (objCommonForAllPlugins.pluginContext.Stage)
                            {
                                case (int)PluginStages.PreOperation:
                                    caseProcessConfigPresenter.SetCaseProcessConfig_SLAConfig_AdminInfo(objCommonForAllPlugins);
                                    break;
                                case (int)PluginStages.PostOperation:
                                    entUpdateCaseAccordingly = caseProcessConfigPresenter.RouteTargetCase_CaseProcessConfig(objCommonForAllPlugins, entUpdateCaseAccordingly);

                                    if (entUpdateCaseAccordingly != null && entUpdateCaseAccordingly.Attributes != null && entUpdateCaseAccordingly.Attributes.Count > 0)
                                    {
                                        objCommonForAllPlugins.srvInitiatingUsr.Update(entUpdateCaseAccordingly);
                                    }

                                    caseProcessConfigPresenter.CreateAssignmentHistory_ForTargetCase(objCommonForAllPlugins);
                                    break;
                            }
                            break;
                        case PluginHelperStrigs.UpdateMsgName:
                            switch (objCommonForAllPlugins.pluginContext.Stage)
                            {
                                case (int)PluginStages.PreOperation:
                                    caseProcessConfigPresenter.SetCaseProcessConfig_SLAConfig_AdminInfo(objCommonForAllPlugins);
                                    break;
                                case (int)PluginStages.PostOperation:
                                    entUpdateCaseAccordingly = caseProcessConfigPresenter.RouteTargetCase_CaseProcessConfig(objCommonForAllPlugins, entUpdateCaseAccordingly);

                                    if (entUpdateCaseAccordingly != null && entUpdateCaseAccordingly.Attributes != null && entUpdateCaseAccordingly.Attributes.Count > 0)
                                    {
                                        objCommonForAllPlugins.srvInitiatingUsr.Update(entUpdateCaseAccordingly);
                                    }

                                    caseProcessConfigPresenter.CallReleaseCaseAction_ForTargetCase(objCommonForAllPlugins);
                                    break;
                            }
                            break;
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