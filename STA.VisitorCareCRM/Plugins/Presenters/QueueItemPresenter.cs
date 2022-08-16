using Microsoft.Xrm.Sdk;
using STA.TouristCareCRM.Plugins.Helpers;
using STA.TouristCareCRM.Plugins.Resources;
using System;

namespace STA.TouristCareCRM.Plugins.Presenters
{
    public class QueueItemPresenter
    {
        CommonMethods commonMethods = new CommonMethods();

        public void CheckWorkedByType_FromTargetQueueItem(IOrganizationServiceFactory serviceFactory, IPluginExecutionContext exeContext, ITracingService srvTracing)
        {
            Entity entTargetQueueItem = commonMethods.RetrieveTargetEntityFromContext(exeContext);

            if (entTargetQueueItem != null)
            {
                // Obtain the organization service reference.
                IOrganizationService srvContextUsr = serviceFactory.CreateOrganizationService(exeContext.UserId);
                IOrganizationService srvInitiatingUsr = serviceFactory.CreateOrganizationService(exeContext.InitiatingUserId);

                Entity entPreImageQueueItem = null;
                EntityReference entrefWorkedBy = null;
                OptionSetValue optsetRegardingType = null;
                int intObjectType = int.MinValue;

                if (exeContext.MessageName == PluginHelperStrigs.UpdateMsgName)
                {
                    entPreImageQueueItem = commonMethods.RetrievePostImageEntityFromContext(exeContext);
                }

                entrefWorkedBy = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPreImageQueueItem, QueueItemEntityAttributeNames.WorkedBy);
                optsetRegardingType = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPreImageQueueItem, QueueItemEntityAttributeNames.ObjectTypeCode);

                if (optsetRegardingType != null && optsetRegardingType.Value != int.MinValue)
                {
                    intObjectType = optsetRegardingType.Value;
                }

                if (entrefWorkedBy != null && !string.IsNullOrWhiteSpace(entrefWorkedBy.LogicalName) && entrefWorkedBy.LogicalName == TeamEntityAttributeNames.EntityLogicalName && intObjectType == (int)ObjectTypeCode.Case)
                {
                    throw new Exception("Cannot update the 'Worked By' with any Team. Please update it with one of the Enabled User in the system." + Environment.NewLine +
                        "For further assistance, please contact System Administrator.");
                }
            }
        }

        public void SetWorkedBy_AddToQueueItem(IOrganizationServiceFactory serviceFactory, IPluginExecutionContext exeContext, ITracingService srvTracing)
        {
            Entity entTargetQueueItem = commonMethods.RetrieveTargetEntityFromContext(exeContext);

            if (entTargetQueueItem != null)
            {
                // Obtain the organization service reference.
                IOrganizationService srvContextUsr = serviceFactory.CreateOrganizationService(exeContext.UserId);
                IOrganizationService srvInitiatingUsr = serviceFactory.CreateOrganizationService(exeContext.InitiatingUserId);
                
                EntityReference entrefWorkedBy = null, entrefRegardingCase = null;
                DateTime dtWorkedOn = DateTime.MinValue, dtModifiedOn = DateTime.MinValue;
                OptionSetValue optsetRegardingType = null;
                int intObjectType = int.MinValue;                

                entrefWorkedBy = commonMethods.GetAttributeValFromTargetEntity(entTargetQueueItem, QueueItemEntityAttributeNames.WorkedBy);
                dtWorkedOn = commonMethods.GetAttributeValFromTargetEntity(entTargetQueueItem, QueueItemEntityAttributeNames.WorkedOn);
                entrefRegardingCase = commonMethods.GetAttributeValFromTargetEntity(entTargetQueueItem, QueueItemEntityAttributeNames.ObjectId);
                optsetRegardingType = commonMethods.GetAttributeValFromTargetEntity(entTargetQueueItem, QueueItemEntityAttributeNames.ObjectTypeCode);
                dtModifiedOn = commonMethods.GetAttributeValFromTargetEntity(entTargetQueueItem, QueueItemEntityAttributeNames.ModifiedOn);

                if (optsetRegardingType != null && optsetRegardingType.Value != int.MinValue)
                {
                    intObjectType = optsetRegardingType.Value;
                }

                if (entrefWorkedBy != null && entrefWorkedBy.Id != Guid.Empty && intObjectType == (int)ObjectTypeCode.Case && entrefRegardingCase != null && entrefRegardingCase.Id != Guid.Empty)
                {
                    Entity entUpdateCase = new Entity(CaseEntityAttributeNames.EntityLogicalName, entrefRegardingCase.Id);
                    entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedBy] = entrefWorkedBy;
                    if (dtWorkedOn != DateTime.MinValue)
                    {
                        entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedOn] = dtWorkedOn.ToUniversalTime();
                    }
                    else if (dtModifiedOn != DateTime.MinValue)
                    {
                        entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedOn] = dtModifiedOn.ToUniversalTime();
                    }
                    else
                    {
                        entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedOn] = DateTime.Now;
                    }

                    srvInitiatingUsr.Update(entUpdateCase);
                }
            }
        }

        public void SetCaseWorkedBy_FromTargetQueueItem(IOrganizationServiceFactory serviceFactory, IPluginExecutionContext exeContext, ITracingService srvTracing)
        {
            Entity entTargetQueueItem = commonMethods.RetrieveTargetEntityFromContext(exeContext);

            if (entTargetQueueItem != null)
            {
                // Obtain the organization service reference.
                IOrganizationService srvContextUsr = serviceFactory.CreateOrganizationService(exeContext.UserId);
                IOrganizationService srvInitiatingUsr = serviceFactory.CreateOrganizationService(exeContext.InitiatingUserId);

                Entity entPostImageQueueItem = null;
                EntityReference entrefWorkedBy = null, entrefRegardingCase = null;
                DateTime dtWorkedOn = DateTime.MinValue, dtModifiedOn = DateTime.MinValue;
                OptionSetValue optsetRegardingType = null;
                int intObjectType = int.MinValue;

                if (exeContext.MessageName == PluginHelperStrigs.UpdateMsgName)
                {
                    entPostImageQueueItem = commonMethods.RetrievePostImageEntityFromContext(exeContext);
                }

                entrefWorkedBy = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPostImageQueueItem, QueueItemEntityAttributeNames.WorkedBy);
                dtWorkedOn = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPostImageQueueItem, QueueItemEntityAttributeNames.WorkedOn);
                entrefRegardingCase = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPostImageQueueItem, QueueItemEntityAttributeNames.ObjectId);
                optsetRegardingType = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPostImageQueueItem, QueueItemEntityAttributeNames.ObjectTypeCode);
                dtModifiedOn = commonMethods.GetAttributeValFromTargetOrImageEntities(entTargetQueueItem, entPostImageQueueItem, QueueItemEntityAttributeNames.ModifiedOn);

                if (optsetRegardingType != null && optsetRegardingType.Value != int.MinValue)
                {
                    intObjectType = optsetRegardingType.Value;
                }

                if (entrefWorkedBy != null && entrefWorkedBy.Id != Guid.Empty && intObjectType == (int)ObjectTypeCode.Case && entrefRegardingCase != null && entrefRegardingCase.Id != Guid.Empty)
                {
                    Entity entUpdateCase = new Entity(CaseEntityAttributeNames.EntityLogicalName, entrefRegardingCase.Id);
                    entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedBy] = entrefWorkedBy;
                    if (dtWorkedOn != DateTime.MinValue)
                    {
                        entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedOn] = dtWorkedOn.ToUniversalTime();
                    }
                    else if (dtModifiedOn != DateTime.MinValue)
                    {
                        entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedOn] = dtModifiedOn.ToUniversalTime();
                    }
                    else
                    {
                        entUpdateCase.Attributes[CaseEntityAttributeNames.CaseWorkedOn] = DateTime.Now;
                    }

                    srvInitiatingUsr.Update(entUpdateCase);
                }
            }
        }
    }
}