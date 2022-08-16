using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace STA.TouristCareCRM.Workflows.Helpers
{
    public class CommonWorkFlowExtensions
    {
        public ITracingService srvTracing;
        public IWorkflowContext workflowContext;
        public IOrganizationServiceFactory serviceFactory;
        public IOrganizationService srvContextUsr;        
        public IOrganizationService srvInitiatingUsr;
        public CodeActivityContext codeActivityContext;

        public CommonWorkFlowExtensions(CodeActivityContext executionContext)
        {
            codeActivityContext = executionContext;
            srvTracing = executionContext.GetExtension<ITracingService>();
            workflowContext = executionContext.GetExtension<IWorkflowContext>();
            serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            srvContextUsr = serviceFactory.CreateOrganizationService(workflowContext.UserId);
            srvInitiatingUsr = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);
        }        
    }

    public class PaginationClass
    {
        public int Page { get; set; }
        public string PagingCookie { get; set; }
        public int RecordCount { get; set; }
    }
}