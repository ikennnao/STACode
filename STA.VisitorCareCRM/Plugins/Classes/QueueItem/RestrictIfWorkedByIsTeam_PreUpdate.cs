using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Presenters;
using System;

namespace STA.TouristCareCRM.Plugins.Classes.QueueItem
{
    public class RestrictIfWorkedByIsTeam_PreUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context from the service provider.
            IPluginExecutionContext exeContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the tracing service
            ITracingService srvTracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            if (exeContext.Depth > 1)
            {
                return;
            }

            if (exeContext.PrimaryEntityName == QueueItemEntityAttributeNames.EntityLogicalName)
            {
                try
                {
                    QueueItemPresenter presenterQueueItem = new QueueItemPresenter();

                    if (exeContext.Stage == (int)PluginStages.PreOperation && exeContext.MessageName == PluginHelperStrigs.UpdateMsgName)
                    {
                        presenterQueueItem.CheckWorkedByType_FromTargetQueueItem(serviceFactory, exeContext, srvTracing);

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