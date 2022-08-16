using System;
using System.Collections.Generic;
using System.Text;

namespace Plugins.Classes
{
    public class GetAppRecordUrl : CodeActivity
    {
        #region "Parameter Definition"
        [RequiredArgument]
        [Input("Record URL")]
        [Default("")]
        public InArgument<String> RecordURL { get; set; }

        [RequiredArgument]
        [Input("Application Unique Name")]
        [Default("")]
        public InArgument<String> AppModuleUniqueName { get; set; }

        [Output("Record URL for App Module")]
        public OutArgument<string> AppRecordUrl { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {

            #region "Load CRM Service from context"
            Common objCommon = new Common(executionContext);
            objCommon.tracingService.Trace("Load CRM Service from context --- OK");
            #endregion

            #region "Read Parameters"
            String recordURL = this.RecordURL.Get(executionContext);
            String appModuleUniqueName = this.AppModuleUniqueName.Get(executionContext);
            #endregion

            msdyncrmWorkflowTools_Class commonClass = new msdyncrmWorkflowTools_Class(objCommon.service, objCommon.tracingService);

            string appRecordUrl = commonClass.GetAppRecordUrl(recordURL, appModuleUniqueName);

            this.AppRecordUrl.Set(executionContext, appRecordUrl);

        }


    }
}
