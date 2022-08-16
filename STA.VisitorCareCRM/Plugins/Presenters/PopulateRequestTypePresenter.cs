using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class PopulateRequestTypePresenter
    {
        private CommonMethods commonMethods = new CommonMethods();

        public void SetRqtTypeInRegardingCase(CommonPluginExtensions objCommonForAllPlugins)
        {
            Entity entTargetCaseApproval = commonMethods.RetrieveTargetEntityFromContext(objCommonForAllPlugins.pluginContext);

            if (entTargetCaseApproval != null)
            {
                EntityReference entrefRegardingObject = commonMethods.GetAttributeValFromTargetEntity(entTargetCaseApproval, CaseApprovalEntityAttributeNames.RegardingObject);
                OptionSetValue optsetRequestType = commonMethods.GetAttributeValFromTargetEntity(entTargetCaseApproval, CaseApprovalEntityAttributeNames.RequestType);

                if (entrefRegardingObject != null && !string.IsNullOrWhiteSpace(entrefRegardingObject.LogicalName))
                {
                    switch (entrefRegardingObject.LogicalName)
                    {
                        case CaseEntityAttributeNames.EntityLogicalName:
                            if (optsetRequestType != null && optsetRequestType.Value != int.MinValue)
                            {
                                Entity entUpdateRgrdgCase = new Entity(entrefRegardingObject.LogicalName, entrefRegardingObject.Id);

                                if (optsetRequestType.Value == (int)RequestType.MomentofDelight)
                                {
                                    entUpdateRgrdgCase.Attributes[CaseEntityAttributeNames.RecognizeMomentofDelight] = true;
                                }

                                objCommonForAllPlugins.srvInitiatingUsr.Update(entUpdateRgrdgCase);
                            }
                            break;
                    }
                }
            }
        }
    }
}