using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Plugins.Presenters
{
  
    public class WorkflowToolsClass
    {
        private IOrganizationService service;
        private ITracingService tracing;

        //Shared HttpClient
        private static HttpClient httpClient;

        /// <summary>
        /// Class Constructor: Inits Singletion objects
        /// </summary>
        static WorkflowToolsClass()
        {
            //Setup a commong HttpClient as a best practice to avoid leaving open connections
            //more details https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 30); //30 second timeout as recommend by Microsoft Support to prevent TimeOut on Sandbox
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

        }

        public WorkflowToolsClass(IOrganizationService _service, ITracingService _tracing)
        {
            service = _service;
            tracing = _tracing;
        }

        public WorkflowToolsClass(IOrganizationService _service)
        {
            service = _service;
            tracing = null;
        }

      
        public string GetAppModuleId(string appModuleUniqueName)
        {
            var query = new QueryExpression
            {
                EntityName = "appmodule",
                ColumnSet = new ColumnSet("appmoduleid", "uniquename"),
                Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression ("uniquename", ConditionOperator.Equal, appModuleUniqueName)
                            }
                        }
            };

            var appmodules = service.RetrieveMultiple(query).Entities;
            return appmodules.First()["appmoduleid"].ToString();
        }

        public string GetAppRecordUrl(string recordUrl, string appModuleUniqueName)
        {
            string appModuleId = GetAppModuleId(appModuleUniqueName);

            return recordUrl + "&appid=" + appModuleId;
        }

     
    }
}
