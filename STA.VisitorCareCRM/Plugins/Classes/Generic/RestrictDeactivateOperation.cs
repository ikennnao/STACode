using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.Generic
{
    public class RestrictDeactivateOperation : IPlugin
    {
        private string strUnsecureConfig { get; set; }

        public RestrictDeactivateOperation(string unsecureConfig, string secureConfig)
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
                CheckToRestrictDeactivateStep entityInfoPresenter = new CheckToRestrictDeactivateStep();

                if (exeContext.Stage == (int)PluginStages.PreOperation && exeContext.MessageName == PluginHelperStrigs.SetStateDynamicEntityMsgName)
                {
                    entityInfoPresenter.CheckConditionToDeactivate_OnPreOperation(serviceFactory, exeContext, srvTracing, strUnsecureConfig);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}