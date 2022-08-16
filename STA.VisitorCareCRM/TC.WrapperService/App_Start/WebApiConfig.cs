using System;
using System.Diagnostics;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;

namespace TC.WrapperService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableSystemDiagnosticsTracing();
            string strAllowedOrigins = "https://www.twilio.com, https://flex.twilio.com, https://saudiexpert.sta.gov.sa";
            
            File.AppendAllText("C:\\Users\\Public\\Logs\\Log.txt", strAllowedOrigins + " " + DateTime.Now.ToString() + Environment.NewLine);
            Trace.WriteLine(strAllowedOrigins);

            // Web API configuration and services
            var cors = new EnableCorsAttribute(strAllowedOrigins, "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}