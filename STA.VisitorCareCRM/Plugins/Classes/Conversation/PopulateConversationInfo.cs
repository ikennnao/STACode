using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.Conversation
{
    public class PopulateConversationInfo : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region "Load CRM Service from context"

            CommonPluginExtensions objCommonForAllPlugins = new CommonPluginExtensions(serviceProvider);
            objCommonForAllPlugins.srvTracing.Trace("Load CRM Service from context --- OK");

            #endregion

            if (objCommonForAllPlugins.pluginContext.PrimaryEntityName == ConversationEntityAttributeNames.EntityLogicalName)
            {
                try
                {
                    PopulateConversationInfoPresenter conversationInfoPresenter = new PopulateConversationInfoPresenter();

                    switch (objCommonForAllPlugins.pluginContext.MessageName)
                    {
                        case PluginHelperStrigs.CreateMsgName:
                            switch (objCommonForAllPlugins.pluginContext.Stage)
                            {
                                case (int)PluginStages.PreOperation:
                                    conversationInfoPresenter.SetConversationRequiredInfo_OnPreOperation(objCommonForAllPlugins);
                                    break;
                                case (int)PluginStages.PostOperation:
                                    conversationInfoPresenter.SetConversationInActive_OnPostOperation(objCommonForAllPlugins);
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
    }
}