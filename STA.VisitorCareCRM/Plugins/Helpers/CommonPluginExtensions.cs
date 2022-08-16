using Microsoft.Xrm.Sdk;
using System;

namespace STA.TouristCareCRM.Plugins.Helpers
{
    public class CommonPluginExtensions
    {
        public IServiceProvider pluginSrvProvider;
        public ITracingService srvTracing;
        public IPluginExecutionContext pluginContext;
        public IOrganizationServiceFactory serviceFactory;
        public IOrganizationService srvContextUsr;
        public IOrganizationService srvInitiatingUsr;

        public CommonPluginExtensions(IServiceProvider srvProvider)
        {
            pluginSrvProvider = srvProvider;
            // Obtain the tracing service
            srvTracing = (ITracingService)srvProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider.
            pluginContext = (IPluginExecutionContext)srvProvider.GetService(typeof(IPluginExecutionContext));
            // Obtain the organization service reference.
            serviceFactory = (IOrganizationServiceFactory)srvProvider.GetService(typeof(IOrganizationServiceFactory));
            // Obtain the organization service reference.
            srvContextUsr = serviceFactory.CreateOrganizationService(pluginContext.UserId);
            srvInitiatingUsr = serviceFactory.CreateOrganizationService(pluginContext.InitiatingUserId);
        }
    }
}