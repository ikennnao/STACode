using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.Generic
{
    public class RestrictDuplicateRecords : IPlugin
    {
        private string strUnsecureConfig { get; set; }

        public RestrictDuplicateRecords(string unsecureConfig, string secureConfig)
        {
            strUnsecureConfig = unsecureConfig;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext exeContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the tracing service
            ITracingService srvTracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            try
            {
                CheckToRestrictDuplicateRecord entityInfoPresenter = new CheckToRestrictDuplicateRecord();

                if (exeContext.Stage == (int)PluginStages.PreOperation && (exeContext.MessageName == PluginHelperStrigs.CreateMsgName || exeContext.MessageName == PluginHelperStrigs.UpdateMsgName))
                {
                    entityInfoPresenter.CheckConditionToDuplicate_OnPreOperation(exeContext, serviceFactory, srvTracing, strUnsecureConfig);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}