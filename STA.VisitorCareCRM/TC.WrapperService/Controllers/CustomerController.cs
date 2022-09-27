using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Http.Headers;
using System.Web.Http;
using TC.WrapperService.Interfaces;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/Customer")]
    public class CustomerController : ApiController
    {
        private readonly ILogging logger = Instance;
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForCustomer customErrorMsgsForCustomer = new CustomErrorMsgsForCustomer();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Customer Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;

        // POST: API/Customer/CreateCustomer // This method is used just for the PENSO API
        [Route("CreateContact")]
        public ResponseClass CreateContact(CreateCustomerRequest rqtCreateCustomer)
        {
            exceptionResponseObjects.strAPIServiceName = "CreateContact";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtCreateCustomer.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        CreateCustomerResponse createdCustomerInfo = CreateCommercialCustomerMtd(guidLoggedInUser, rqtCreateCustomer.createCustomer);
                        if (createdCustomerInfo.CustomerGuid != null)
                        {
                            otptResponse = apiCMs.SetSuccessResponse(createdCustomerInfo);
                            otptResponse.apiResult.resultDesDetails = "Contact was created successfully";
                        }

                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Customer/PostCreateCustomer
        [Route("PostCreateCustomer")]
        public ResponseClass PostCreateCustomer(CreateCustomerRequest rqtCreateCustomer)
        {
            exceptionResponseObjects.strAPIServiceName = "CreateCustomer";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtCreateCustomer.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        CreateCustomerResponse createdCustomerInfo = CreateCustomerMtd(guidLoggedInUser, rqtCreateCustomer.createCustomer);
                        otptResponse = apiCMs.SetSuccessResponse(createdCustomerInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Customer/PostRetrieveCustomer
        [Route("PostRetrieveCustomer")]
        public ResponseClass PostRetrieveCustomer(RetrieveCustomerRequest rqtGetCustomer)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveCustomer";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCustomer.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCustomerResponse retrieveCustomerInfo = RetrieveCustomerMtd(rqtGetCustomer.retrieveCustomer);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCustomerInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInRetrieveOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        [Route("PostRetrieveContact")]
        public ResponseClass PostRetrieveContact(RetrieveCustomerRequest rqtGetCustomer)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveContact";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCustomer.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCustomerResponse retrieveCustomerInfo = RetrieveCommercialContactMtd(rqtGetCustomer.retrieveCustomer);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCustomerInfo);
                        otptResponse.apiResult.resultDesDetails = "Contact Exists";
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Customer/PostUpdateCustomer
        [Route("PostUpdateCustomer")]
        public ResponseClass PostUpdateCustomer(UpdateCustomerRequest rqtUpdateCustomer)
        {
            exceptionResponseObjects.strAPIServiceName = "PostUpdateCustomer";
            exceptionResponseObjects.strAPIPageName = strAPIPageName;
            exceptionResponseObjects.resultDetails = new ResultClass();

            try
            {
                #region Validate API Token & API Auth Key using STA Authenticate API

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);

                #endregion

                #region If, Request Header is Validated

                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtUpdateCustomer.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        UpdateCustomerResponse updateCustomerInfo = UpdateCustomerMtd(guidLoggedInUser, rqtUpdateCustomer.updateCustomer);
                        otptResponse = apiCMs.SetSuccessResponse(updateCustomerInfo);
                        otptResponse.apiResult.resultDesDetails = "Contact was updated successfully";
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInUpdateOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Customer/PostUpdateCustomer  This method is used just for the PENSO API
        //[Route("PostUpdateContact")]
        //public ResponseClass PostUpdateContact(UpdateCutomerRequest rqtUpdateCustomer)
        //{
        //    exceptionResponseObjects.strAPIServiceName = "UpdateContact";
        //    exceptionResponseObjects.strAPIPageName = strAPIPageName;
        //    exceptionResponseObjects.resultDetails = new ResultClass();

        //    try
        //    {
        //        #region Validate Session using CBD Authenticate API

        //        HttpRequestHeaders reqHeader = this.Request.Headers;
        //        isSessionValid = apiCMs.ValidateSession(reqHeader);

        //        #endregion

        //        #region If, Session is Valid

        //        if (isSessionValid)
        //        {
        //            #region Post Validate Session Check Conditions

        //            Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtUpdateCustomer.authenticateDetails);

        //            #endregion                    

        //            if (guidLoggedInUser != Guid.Empty)
        //            {
        //                UpdateCustomerResponse createdCustomerInfo = UpdateCustomerMtd(guidLoggedInUser, rqtUpdateCustomer.updateCustomer);
        //                otptResponse = apiCMs.SetSuccessResponse(createdCustomerInfo);
        //                otptResponse.apiResult.resultDesDetails = "Contact was updated successfully";
        //            }
        //        }

        //        #endregion                
        //    }
        //    catch (Exception ex)
        //    {
        //        exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + ContactEntityAttributesNames.EntityLogicalName;
        //        exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
        //        otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
        //        apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
        //    }

        //    return otptResponse;
        //}

        private CreateCustomerResponse CreateCustomerMtd(Guid guidLoggedInUser, CreateCustomer customerDetails)
        {
            CreateCustomerResponse rspCustomer = new CreateCustomerResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Entity entCreateCustomer = null, entRequiredChannelOrigin = null;
                int intChannelOrigin = int.MinValue;

                Guid guidCRCustomer = Guid.Empty;

                if (customerDetails != null)
                {
                    if (!string.IsNullOrWhiteSpace(customerDetails.ChannelOrigin) && !string.IsNullOrWhiteSpace(customerDetails.FirstName) && !string.IsNullOrWhiteSpace(customerDetails.LastName))
                    {
                        entCreateCustomer = new Entity(ContactEntityAttributesNames.EntityLogicalName);
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.FirstName] = customerDetails.FirstName;
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.LastName] = customerDetails.LastName;
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.PrimaryContactNo] = customerDetails.PrimaryContactNo;
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.PrimaryEmail] = customerDetails.PrimaryEmail;
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.SSID] = customerDetails.SSID;
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.TravelPartner] = customerDetails.TravelPartner;
                        OptionSetValueCollection SSIDInterests = new OptionSetValueCollection();
                        foreach(var item in customerDetails.SSIDInterest)
                        {
                            SSIDInterests.Add(new OptionSetValue(item));
                        }
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.SSIDInterest] = SSIDInterests;
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.Gender] = new OptionSetValue(customerDetails.Gender);
                        DateTime birthday;
                        if (DateTime.TryParseExact(customerDetails.Birthday, new string[] { "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
                            entCreateCustomer.Attributes[ContactEntityAttributesNames.Birthday] = DateTime.Parse(customerDetails.Birthday);
                        else
                            strExceptionMsg += customErrorMsgsForCustomer.BirthdayFormat;
                        if (customerDetails.BusinessType != int.MinValue)
                        {
                            entCreateCustomer.Attributes[ContactEntityAttributesNames.BusinessType] = new OptionSetValue(customerDetails.BusinessType);
                        }

                        #region Set the Customer Category

                        if (customerDetails.CustomerCategory != null && customerDetails.CustomerCategory.Id != Guid.Empty)
                        {
                            entCreateCustomer.Attributes[ContactEntityAttributesNames.CustomerCategory] = new EntityReference(CustomerCategoryEntityAttributeNames.EntityLogicalName, customerDetails.CustomerCategory.Id);
                        }
                        else
                        {
                            Entity entCustomerCategory = crmCMs.RetrieveCustomerCategoryFromCategoryId(crmAdapter.CRMAdminService(), "2"); //B2C - Customer Category

                            if (entCustomerCategory != null)
                            {
                                entCreateCustomer.Attributes[ContactEntityAttributesNames.CustomerCategory] = entCustomerCategory.ToEntityReference();
                            }
                        }

                        #endregion

                        #region Set the Channel Origin

                        intChannelOrigin = apiCMs.FormatChannelOriginFromStringToInt(customerDetails.ChannelOrigin);
                        entRequiredChannelOrigin = crmCMs.RetrieveChannelOriginFromOriginCode(crmAdapter.CRMAdminService(), customerDetails.ChannelOrigin);

                        if (entRequiredChannelOrigin != null && entRequiredChannelOrigin.Id != Guid.Empty)
                        {
                            entCreateCustomer.Attributes[ContactEntityAttributesNames.ChannelOrigin] = new EntityReference(entRequiredChannelOrigin.LogicalName, entRequiredChannelOrigin.Id);
                        }

                        #endregion                     

                        switch (intChannelOrigin)
                        {
                            case (int)ChannelOrigin.Email:
                            case (int)ChannelOrigin.Facebook:
                            case (int)ChannelOrigin.Instagram:
                            case (int)ChannelOrigin.Twitter:
                            case (int)ChannelOrigin.Youtube:
                            case (int)ChannelOrigin.LiveChat:
                            case (int)ChannelOrigin.TripAdvisor:
                                if (string.IsNullOrWhiteSpace(customerDetails.PrimaryEmail))
                                {
                                    strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryEmail;
                                }
                                break;
                            case (int)ChannelOrigin.WhatsApp:
                            case (int)ChannelOrigin.PhoneCall:
                                if (string.IsNullOrWhiteSpace(customerDetails.PrimaryContactNo))
                                {
                                    strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryContactNo;
                                }
                                break;
                            case (int)ChannelOrigin.ChatBot:
                                if (string.IsNullOrWhiteSpace(customerDetails.PrimaryEmail))
                                {
                                    strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryEmail;
                                }
                                break;
                            case (int)ChannelOrigin.Kiosk:
                            case (int)ChannelOrigin.BayenaPortal:
                            case (int)ChannelOrigin.MobileApp:
                            case (int)ChannelOrigin.HelpVisitSaudi:

                            default:
                                if (string.IsNullOrWhiteSpace(customerDetails.PrimaryContactNo))
                                {
                                    //strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryContactNo;
                                }
                                if (string.IsNullOrWhiteSpace(customerDetails.PrimaryEmail))
                                {
                                    strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryEmail;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(customerDetails.ChannelOrigin))
                        {
                            strExceptionMsg += customErrorMsgsForCustomer.missingChannelOrigin;
                        }
                        if (string.IsNullOrWhiteSpace(customerDetails.FirstName))
                        {
                            strExceptionMsg += customErrorMsgsForCustomer.missingFirstName;
                        }
                        if (string.IsNullOrWhiteSpace(customerDetails.LastName))
                        {
                            strExceptionMsg += customErrorMsgsForCustomer.missingLastName;
                        }
                        if (customerDetails.CustomerCategory == null || customerDetails.CustomerCategory.Id == Guid.Empty)
                        {
                            strExceptionMsg += customErrorMsgsForCustomer.missingCustomerCategory;
                        }
                    }
                }
                else
                {
                    strExceptionMsg += customErrorMsgsForCustomer.missingCustomerDetails;
                }

                if (string.IsNullOrWhiteSpace(strExceptionMsg))
                {
                    entCreateCustomer.Id = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Create(entCreateCustomer);

                    if (entCreateCustomer.Id != Guid.Empty)
                    {
                        if (customerDetails.SMHDetails != null)
                        {
                            Entity entSocialHandle = crmCMs.RetrieveSMCFromNetworkId(crmAdapter.CRMAdminService(), customerDetails.SMHDetails);

                            if (entSocialHandle == null || entSocialHandle.Id == Guid.Empty)
                            {
                                entSocialHandle = crmCMs.CreateSMCFromNetworkId(crmAdapter.ImpersonatedCRMService(guidLoggedInUser), customerDetails.SMHDetails, entRequiredChannelOrigin.ToEntityReference(), entCreateCustomer.ToEntityReference());
                            }
                            else
                            {
                                crmCMs.UpdateSMCFromNetworkId(crmAdapter.ImpersonatedCRMService(guidLoggedInUser), entSocialHandle, entCreateCustomer.ToEntityReference());
                            }
                        }

                        rspCustomer.EntityLogicalName = ContactEntityAttributesNames.EntityLogicalName;
                        rspCustomer.CustomerGuid = entCreateCustomer.Id;

                        Entity entRetrieveCustomer = null;
                        entRetrieveCustomer = crmAdapter.CRMAdminService().Retrieve(entCreateCustomer.LogicalName, entCreateCustomer.Id, new ColumnSet(ContactEntityAttributesNames.FullName, ContactEntityAttributesNames.AppRecordUrl));

                        if (entRetrieveCustomer != null)
                        {
                            rspCustomer.FullName = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCustomer, ContactEntityAttributesNames.FullName);
                            rspCustomer.AppRecordUrl = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCustomer, ContactEntityAttributesNames.AppRecordUrl);
                        }
                    }
                }
                else
                {
                    throw new Exception(strExceptionMsg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCustomer;
        }

        private CreateCustomerResponse CreateCommercialCustomerMtd(Guid userGuid, CreateCustomer custDetails)
        {
            CreateCustomerResponse rspCustomer = new CreateCustomerResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Entity entCreateCustomer = null;

                Guid guidCRCustomer = Guid.Empty;

                if (custDetails != null)
                {
                    entCreateCustomer = new Entity(ContactEntityAttributesNames.EntityLogicalName);
                    if (!string.IsNullOrWhiteSpace(custDetails.PrimaryEmail))
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.PrimaryEmail] = custDetails.PrimaryEmail;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryEmail;
                    }

                    if (!string.IsNullOrWhiteSpace(custDetails.FirstName))
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.FirstName] = custDetails.FirstName;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCustomer.missingFirstName;
                    }

                    if (!string.IsNullOrWhiteSpace(custDetails.LastName))
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.LastName] = custDetails.LastName;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCustomer.missingLastName;
                    }

                    if (!string.IsNullOrWhiteSpace(custDetails.PrimaryContactNo))
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.PrimaryContactNo] = custDetails.PrimaryContactNo;
                    }
                    //else
                    //{
                    //    strExceptionMsg += customErrorMsgsForCustomer.missingPrimaryContactNo;
                    //}

                    if (custDetails.Country != null && custDetails.Country.Id != null)
                    {

                        entCreateCustomer.Attributes[ContactEntityAttributesNames.Country] = new EntityReference(CountryEntityAttributeNames.EntityLogicalName, custDetails.Country.Id);
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCustomer.missingCustomerCountry;
                    }

                    if (custDetails.LeadType != null)
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.LeadType] = new OptionSetValue((int)custDetails.LeadType);
                    }

                    if (!string.IsNullOrEmpty(custDetails.CapabilityBuildingProgram))
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.CapabilityBuildingProgram] = custDetails.CapabilityBuildingProgram == "Yes";
                    }

                    if (!string.IsNullOrEmpty(custDetails.SaudiTourismContent))
                    {
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.SaudiTourismContent] = custDetails.SaudiTourismContent == "Yes";
                    }

                    if (custDetails.CompanyName != null && custDetails.CompanyName.Id != null)
                    {

                        entCreateCustomer.Attributes[ContactEntityAttributesNames.Account] = new EntityReference(AccountEntityAttributesNames.EntityLogicalName, custDetails.CompanyName.Id);
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCustomer.missingCompanyName;
                    }

                    if (string.IsNullOrWhiteSpace(strExceptionMsg))
                    {
                        //set the customer Type 
                        entCreateCustomer.Attributes[ContactEntityAttributesNames.BusinessType] = new OptionSetValue(Convert.ToInt32(BusinessType.Commercial));
                        //set owner of the record to the commercial team
                        Entity team = crmCMs.GetOwningTeam(ConfigurationManager.AppSettings["OwningTeam"], crmAdapter.ImpersonatedCRMService(userGuid));
                        if (team != null && team.Id != null)
                        {
                            entCreateCustomer.Attributes[ContactEntityAttributesNames.Owner] = new EntityReference(TeamEntityAttributeNames.EntityLogicalName, team.Id);
                        }
                        entCreateCustomer.Id = crmAdapter.ImpersonatedCRMService(userGuid).Create(entCreateCustomer);

                        if (entCreateCustomer.Id != Guid.Empty)
                        {

                            rspCustomer.CustomerGuid = entCreateCustomer.Id;

                            Entity entRetrieveCustomer = null;
                            entRetrieveCustomer = crmAdapter.CRMAdminService().Retrieve(entCreateCustomer.LogicalName, entCreateCustomer.Id, new ColumnSet(ContactEntityAttributesNames.FullName));

                            if (entRetrieveCustomer != null)
                            {
                                //assocaite the International markets to the created contact
                                var regionReferences = new EntityReferenceCollection();
                                if (custDetails.InternationalMarkets != null && custDetails.InternationalMarkets.Count > 0)
                                {
                                    custDetails.InternationalMarkets.ForEach(x =>
                                    {
                                        regionReferences.Add(new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, x.Id));
                                    });
                                    // The relationship to use
                                    var relationship = new Relationship("cc_contact_cc_region");

                                    // Use the Associate method
                                    crmAdapter.ImpersonatedCRMService(userGuid).Associate(entRetrieveCustomer.LogicalName, entRetrieveCustomer.Id, relationship, regionReferences);
                                }
                                rspCustomer.FullName = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCustomer, ContactEntityAttributesNames.FullName);
                                // rspCustomer.AppRecordUrl = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCustomer, ContactEntityAttributesNames.AppRecordUrl);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(strExceptionMsg);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

            return rspCustomer;
        }

        private RetrieveCustomerResponse RetrieveCustomerMtd(RetrieveCustomerObj customerDetails)
        {
            RetrieveCustomerResponse rspRetrieveCustomer = null;
            string strExceptionMsg = string.Empty;

            try
            {
                string strCustomerMetaData = crmFetchXML.GetCustomerData(customerDetails);

                EntityCollection entcolCustomers = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strCustomerMetaData));

                if (entcolCustomers != null && entcolCustomers.Entities != null && entcolCustomers.Entities.Count > 0)
                {
                    rspRetrieveCustomer = new RetrieveCustomerResponse();
                    rspRetrieveCustomer.lstCustomerRecords = new List<RspRetrieveCustomerObj>() { };

                    foreach (Entity entCustomer in entcolCustomers.Entities)
                    {
                        RspRetrieveCustomerObj rspCustomerObj = new RspRetrieveCustomerObj();
                        rspCustomerObj.CustomerGuid = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.CustomerId);
                        rspCustomerObj.FirstName = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.FirstName);
                        rspCustomerObj.LastName = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.LastName);
                        rspCustomerObj.PrimaryContactNo = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.PrimaryContactNo);
                        rspCustomerObj.PrimaryEmail = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.PrimaryEmail);
                        rspCustomerObj.SecondaryContactNo = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.SecondaryContactNo);
                        rspCustomerObj.SecondaryEmail = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.SecondaryEmail);
                        rspCustomerObj.CustomerCategroy = apiCMs.SetCustomFormatEntRefValue(entCustomer, ContactEntityAttributesNames.CustomerCategory);
                        rspCustomerObj.ChannelOrigin = apiCMs.SetCustomFormatEntRefValue(entCustomer, ContactEntityAttributesNames.ChannelOrigin);
                        rspCustomerObj.AppRecordUrl = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.AppRecordUrl);
                        rspCustomerObj.Status = apiCMs.SetCustomFormatOptionsetValue(entCustomer, ContactEntityAttributesNames.Status);
                        rspCustomerObj.SSID = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.SSID);
                        rspCustomerObj.SSIDInterest = apiCMs.SetCustomFormatOptionsetValueCollection(entCustomer, ContactEntityAttributesNames.SSIDInterest);
                        rspCustomerObj.TravelPartner = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.TravelPartner);
                        rspCustomerObj.Gender = apiCMs.SetCustomFormatOptionsetValue(entCustomer, ContactEntityAttributesNames.Gender);
                        rspCustomerObj.Birthday = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.Birthday);
                        rspRetrieveCustomer.lstCustomerRecords.Add(rspCustomerObj);
                    }

                    rspRetrieveCustomer.customerETN = entcolCustomers.EntityName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return rspRetrieveCustomer;
        }

        private RetrieveCustomerResponse RetrieveCommercialContactMtd(RetrieveCustomerObj customerDetails)
        {
            RetrieveCustomerResponse rspRetrieveCustomer = null;
            string strExceptionMsg = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(customerDetails.Email))
                {
                    string strCustomerMetaData = crmFetchXML.GetContactData(customerDetails);

                    EntityCollection entcolCustomers = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strCustomerMetaData));

                    if (entcolCustomers != null && entcolCustomers.Entities != null && entcolCustomers.Entities.Count > 0)
                    {
                        rspRetrieveCustomer = new RetrieveCustomerResponse
                        {
                            _firstContactRecord = new List<RspRetrieveContactObj>() { }
                        };


                        foreach (Entity entCustomer in entcolCustomers.Entities)
                        {
                            RspRetrieveContactObj rspCustomerObj = new RspRetrieveContactObj();
                            rspCustomerObj.CustomerGuid = crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.CustomerId);
                            //string strInternationalMarketsMetaData = crmFetchXML.GetManyToManyContactRegion(crmCMs.GetAttributeValFromTargetEntity(entCustomer, ContactEntityAttributesNames.CustomerId));

                            //EntityCollection entcolInternationalMarkets = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strInternationalMarketsMetaData));
                            //if (entcolInternationalMarkets != null && entcolInternationalMarkets.Entities != null && entcolInternationalMarkets.Entities.Count > 0)
                            //{
                            //    rspCustomerObj.InternationalMarkets = new List<CustomEntityReference>();
                            //    foreach (Entity InternationalMarket in entcolInternationalMarkets.Entities)
                            //    {
                            //        CustomEntityReference item = new CustomEntityReference
                            //        {
                            //            Id = InternationalMarket.Id,
                            //            Name = InternationalMarket.Attributes[RegionsEntityAttributeNames.RegionName].ToString(),
                            //            LogicalName = InternationalMarket.LogicalName

                            //        };
                            //        rspCustomerObj.InternationalMarkets.Add(item);
                            //    }


                            //}
                            rspRetrieveCustomer._firstContactRecord.Add(rspCustomerObj);


                        }

                        rspRetrieveCustomer.customerETN = entcolCustomers.EntityName;
                    }
                    else
                    {
                        strExceptionMsg = "Customer was not found";
                    }
                }
                else
                {

                    strExceptionMsg = "Email Parameter is null or Empty";
                }

                if (string.IsNullOrEmpty(strExceptionMsg))
                {
                    return rspRetrieveCustomer;
                }
                else
                {
                    throw new Exception(strExceptionMsg);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }


        }

        private UpdateCustomerResponse UpdateCustomerMtd(Guid guidLoggedInUser, UpdateCustomer updateCustomerDetails)
        {
            UpdateCustomerResponse rspCustomer = new UpdateCustomerResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Entity entUpdateCustomer = null;
                var entRequiredChannelOrigin = crmCMs.RetrieveChannelOriginFromOriginCode(crmAdapter.CRMAdminService(), updateCustomerDetails.ChannelOrigin);
                if (updateCustomerDetails != null)
                {
                    entUpdateCustomer = new Entity(ContactEntityAttributesNames.EntityLogicalName);

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.CustomerGuid.ToString()))
                    {
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.CustomerId] = updateCustomerDetails.CustomerGuid;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCustomer.missingCustomerGuid;
                    }

                    if (updateCustomerDetails.LeadType != null)
                    {
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.LeadType] = new OptionSetValue((int)updateCustomerDetails.LeadType);
                    }
                    if (updateCustomerDetails.SSIDInterest != null)
                    {
                        OptionSetValueCollection collectionOptionSetValues = new OptionSetValueCollection();
                        foreach (var optionSetValue in updateCustomerDetails.SSIDInterest)
                        {
                            collectionOptionSetValues.Add(new OptionSetValue(Convert.ToInt32(optionSetValue)));
                        }
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.SSIDInterest] = collectionOptionSetValues;
                    }

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.SSID))
                    {
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.SSID] = updateCustomerDetails.SSID;
                    }

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.TravelPartner))
                    {
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.TravelPartner] = updateCustomerDetails.TravelPartner;
                    }
                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.FirstName))
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.FirstName] = updateCustomerDetails.FirstName;

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.LastName))
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.LastName] = updateCustomerDetails.LastName;

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.PrimaryEmail))
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.PrimaryEmail] = updateCustomerDetails.PrimaryEmail;

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.PrimaryContactNo))
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.PrimaryContactNo] = updateCustomerDetails.PrimaryContactNo;

                    if (updateCustomerDetails.Gender != int.MinValue)
                        entUpdateCustomer.Attributes[ContactEntityAttributesNames.Gender] = new OptionSetValue(updateCustomerDetails.Gender);

                    if (!string.IsNullOrWhiteSpace(updateCustomerDetails.Birthday))
                    {
                        DateTime birthday;
                        if(DateTime.TryParseExact(updateCustomerDetails.Birthday,new string [] { "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None,out birthday))
                             entUpdateCustomer.Attributes[ContactEntityAttributesNames.Birthday] = DateTime.Parse(updateCustomerDetails.Birthday);
                        else
                            strExceptionMsg += customErrorMsgsForCustomer.BirthdayFormat;
                    }

                    if (updateCustomerDetails.SMHDetails != null)
                    {
                        Entity entSocialHandle = crmCMs.RetrieveSMCFromNetworkId(crmAdapter.CRMAdminService(), updateCustomerDetails.SMHDetails);

                        if (entSocialHandle == null || entSocialHandle.Id == Guid.Empty)
                        {
                            entSocialHandle = crmCMs.CreateSMCFromNetworkId(crmAdapter.ImpersonatedCRMService(guidLoggedInUser), updateCustomerDetails.SMHDetails, entRequiredChannelOrigin.ToEntityReference(), entUpdateCustomer.ToEntityReference());
                        }
                        else
                        {
                            crmCMs.UpdateSMCFromNetworkId(crmAdapter.ImpersonatedCRMService(guidLoggedInUser), entSocialHandle, entUpdateCustomer.ToEntityReference());
                        }
                    }
                    if (string.IsNullOrWhiteSpace(strExceptionMsg))
                    {
                        //update customer
                        if (updateCustomerDetails.CustomerGuid != Guid.Empty)
                        {
                            entUpdateCustomer.Id = updateCustomerDetails.CustomerGuid;
                            crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Update(entUpdateCustomer);
                            rspCustomer.CustomerGuid = entUpdateCustomer.Id;
                        }
                    }

                    else
                    {
                        throw new Exception(strExceptionMsg);
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspCustomer;

        }

        //private UpdateCustomerResponse UpdateCustomerMtd(Guid userGuid, UpdateCustomer customerDetails)
        //{
        //    UpdateCustomerResponse rspCustomer = new UpdateCustomerResponse();
        //    string strExceptionMsg = string.Empty;

        //    try
        //    {
        //        Entity entUpdateCustomer = null;

        //        Guid guidCRCustomer = Guid.Empty;

        //        if (customerDetails != null && customerDetails.CustomerGuid != null)
        //        {
        //            entUpdateCustomer = new Entity(ContactEntityAttributesNames.EntityLogicalName);

        //            if ((Convert.ToInt32(customerDetails.LeadType) == 1 || Convert.ToInt32(customerDetails.LeadType) == 2))
        //            {
        //                entUpdateCustomer.Attributes[ContactEntityAttributesNames.LeadType] = new OptionSetValue(Convert.ToInt32(customerDetails.LeadType));
        //            }

        //            if (!string.IsNullOrEmpty(customerDetails.CapabilityBuildingProgram))
        //            {
        //                entUpdateCustomer.Attributes[ContactEntityAttributesNames.CapabilityBuildingProgram] = customerDetails.CapabilityBuildingProgram == "Yes" ? true : false;
        //            }

        //            if (!string.IsNullOrEmpty(customerDetails.SaudiTourismContent))
        //            {
        //                entUpdateCustomer.Attributes[ContactEntityAttributesNames.SaudiTourismContent] = customerDetails.SaudiTourismContent == "Yes" ? true : false;
        //            }

        //            else
        //            {

        //            }

        //            if (string.IsNullOrWhiteSpace(strExceptionMsg))
        //            {
        //                //update customer
        //                if (customerDetails.CustomerGuid != Guid.Empty)
        //                {
        //                    entUpdateCustomer.Id  = customerDetails.CustomerGuid;
        //                    crmAdapter.ImpersonatedCRMService(userGuid).Update(entUpdateCustomer);

        //                    if (entUpdateCustomer.Id != Guid.Empty)
        //                    {

        //                        Entity entRetrieveCustomer = null;
        //                        entRetrieveCustomer = crmAdapter.CRMAdminService().Retrieve(entUpdateCustomer.LogicalName, entUpdateCustomer.Id, new ColumnSet(ContactEntityAttributesNames.FullName));

        //                        if (entRetrieveCustomer != null)
        //                        {
        //                            // The relationship to use
        //                            var relationship = new Relationship("cc_contact_cc_region");
        //                            //assocaite the International markets to the existing contact
        //                            var regionReferences = new EntityReferenceCollection(); var regionReferencesToRemove = new EntityReferenceCollection();
        //                            if (customerDetails.InternationalMarkets != null && customerDetails.InternationalMarkets.Count > 0)
        //                            {
        //                                //Dissasociate the contact internation markets
        //                                string strInternationalMarketsMetaData = crmFetchXML.GetManyToManyContactRegion(crmCMs.GetAttributeValFromTargetEntity(entRetrieveCustomer, ContactEntityAttributesNames.CustomerId));

        //                                EntityCollection entcolInternationalMarkets = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strInternationalMarketsMetaData));
        //                                if (entcolInternationalMarkets != null && entcolInternationalMarkets.Entities != null && entcolInternationalMarkets.Entities.Count > 0)
        //                                {
        //                                    foreach (Entity InternationalMarket in entcolInternationalMarkets.Entities)
        //                                    {
        //                                        regionReferencesToRemove.Add(new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, InternationalMarket.Id));
        //                                    }
        //                                    //Disassocite records
        //                                    crmAdapter.ImpersonatedCRMService(userGuid).Disassociate(entRetrieveCustomer.LogicalName, entRetrieveCustomer.Id, relationship, regionReferencesToRemove);
        //                                }
        //                                //Associate records with the International marke
        //                                customerDetails.InternationalMarkets.ForEach(x =>
        //                                {
        //                                    regionReferences.Add(new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, x.Id));
        //                                });
        //                                // Use the Associate method
        //                                crmAdapter.ImpersonatedCRMService(userGuid).Associate(entRetrieveCustomer.LogicalName, entRetrieveCustomer.Id, relationship, regionReferences);
        //                                rspCustomer.CustomerGuid = entUpdateCustomer.Id;
        //                            }



        //                        }
        //                    }
        //                }

        //                else
        //                {
        //                    throw new Exception(strExceptionMsg);

        //                }
        //            }


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //    return rspCustomer;
        //}

        // POST: API/Customer/PostRetrieveCustomer
    }
}