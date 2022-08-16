using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;
using TC.WrapperService.Models;
using TC.WrapperService.Resources;
using TC.WrapperService.Utility;
using static TC.WrapperService.Utility.APICommonMethods;
using static TC.WrapperService.Utility.Logging;

namespace TC.WrapperService.Controllers
{
    [RoutePrefix("API/Accounts")]
    public class AccountsController : ApiController
    {
        private CRMAdapter crmAdapter = new CRMAdapter(Instance);
        private APICommonMethods apiCMs = new APICommonMethods();
        private CRMCommonMethods crmCMs = new CRMCommonMethods();
        private CRMFetchXML_s crmFetchXML = new CRMFetchXML_s();
        private CustomErrorMessages customErrorMsgs = new CustomErrorMessages();
        private CustomErrorMsgsForCompanies customErrorMsgsForCompanies = new CustomErrorMsgsForCompanies();
        private ResponseClass otptResponse = new ResponseClass();
        private ExceptionResponseObjects exceptionResponseObjects = new ExceptionResponseObjects();
        private string strAPIPageName = "Account Controller";
        private bool isAPITokenValidated = false, isAPIAuthKeyValidated = false;
        private string targerEnity = string.Empty;
        // POST: API/Accounts/GetCompanyNames
        [Route("GetCompanyNames")]
        public ResponseClass GetCompanyNames()
        {
            exceptionResponseObjects.strAPIServiceName = "GetCompanyNames";
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

                    AuthenticateRequest _authenticateDetails = new AuthenticateRequest();
                    _authenticateDetails.UserName = ConfigurationManager.AppSettings["svc_username"];
                    RetrieveCompaniesRequest rqtGetCompanies = new RetrieveCompaniesRequest();
                    rqtGetCompanies.authenticateDetails = _authenticateDetails;
                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCompanies.authenticateDetails);

                    #endregion

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCompaniesResponse retrieveRegionsInfo = RetrieveAccounts(guidLoggedInUser);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveRegionsInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target Table: Accounts";
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Accounts/CreateCompany
        [Route("CreateCompany")]
        public ResponseClass CreateCompany(CreateCompanyRequest rqtCreateCompany)
        {
            exceptionResponseObjects.strAPIServiceName = "CreateCompany";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtCreateCompany.authenticateDetails);

                    #endregion

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        CreateCompanyResponse createdCompanyInfo = CreateCompanyMtd(guidLoggedInUser, rqtCreateCompany.createCompany);
                        otptResponse = apiCMs.SetSuccessResponse(createdCompanyInfo);
                        otptResponse.apiResult.resultDesDetails = "Account was created successfully";
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + "Target table: " + AccountEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private CreateCompanyResponse CreateCompanyMtd(Guid userGuid, CreateCompany companyDetails)
        {
            CreateCompanyResponse rspCompany = new CreateCompanyResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Entity entCreateCompany = null;

                Guid guidCRCompany = Guid.Empty;


                if (companyDetails != null)
                {
                    entCreateCompany = new Entity(AccountEntityAttributesNames.EntityLogicalName);
                    if (!string.IsNullOrWhiteSpace(companyDetails.CompanyName))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyName] = companyDetails.CompanyName;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingCompanyName;
                    }

                    if (companyDetails.Region != null)
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.Region] = new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, companyDetails.Region.Id);
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingRegion;
                    }

                    if (companyDetails.SubRegion != null && companyDetails.SubRegion.Id != null)
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.SubRegion] = new EntityReference(SubRegionsEntityAttributeNames.EntityLogicalName, companyDetails.SubRegion.Id);
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingSubRegion;
                    }

                    if (companyDetails.City != null && companyDetails.City.Id != null)
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.City] = new EntityReference(CitiesEntityAttributeNames.EntityLogicalName, companyDetails.City.Id);
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingCity;
                    }

                    if (companyDetails.Country != null && companyDetails.Country.Id != null)
                    {

                        entCreateCompany.Attributes[AccountEntityAttributesNames.Country] = new EntityReference(CountryEntityAttributeNames.EntityLogicalName, companyDetails.Country.Id);
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingCountry;
                    }

                    if (!string.IsNullOrEmpty(companyDetails.CompanyAddress))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyAddress] = companyDetails.CompanyAddress;
                    }

                    if (!string.IsNullOrEmpty(companyDetails.CompanyEmail))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyEmail] = companyDetails.CompanyEmail;
                    }

                    if (!string.IsNullOrEmpty(companyDetails.CompanyPhone))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyPhone] = companyDetails.CompanyPhone;
                    }

                    if (!string.IsNullOrEmpty(companyDetails.CompanyWebSite))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyWebSite] = companyDetails.CompanyWebSite;
                    }

                    if (!string.IsNullOrEmpty(companyDetails.CompanyRegistrationNumber))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyRegistrationNumber] = companyDetails.CompanyRegistrationNumber;
                    }
                    if (!string.IsNullOrEmpty(companyDetails.MTCRNumber))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.MTCRNumber] = companyDetails.MTCRNumber;
                    }
                    if (!string.IsNullOrEmpty(companyDetails.MTCRMID))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.MTCRMID] = companyDetails.MTCRMID;
                    }
                    if (!string.IsNullOrEmpty(companyDetails.CompanyNameAR))
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.CompanyNameAR] = companyDetails.CompanyNameAR;
                    }
                    if (companyDetails.PreferredContactMethod != null)
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.PreferredContactMethod] = new OptionSetValue((int)companyDetails.PreferredContactMethod);
                    }
                    if (companyDetails.ParentAccount != null)
                    {
                        entCreateCompany.Attributes[AccountEntityAttributesNames.ParentAccount] = new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, companyDetails.ParentAccount.Id);
                    }




                    if (string.IsNullOrWhiteSpace(strExceptionMsg))
                    {
                        //set the customer Type 
                        entCreateCompany.Attributes[AccountEntityAttributesNames.BusinessType] = new OptionSetValue(Convert.ToInt32(BusinessType.Commercial));
                        //set owner of the record to the commercial team
                        Entity team = crmCMs.GetOwningTeam(ConfigurationManager.AppSettings["OwningTeam"], crmAdapter.ImpersonatedCRMService(userGuid));
                        if (team != null && team.Id != null)
                        {
                            entCreateCompany.Attributes[AccountEntityAttributesNames.Owner] = new EntityReference(TeamEntityAttributeNames.EntityLogicalName, team.Id);
                        }
                        entCreateCompany.Id = crmAdapter.ImpersonatedCRMService(userGuid).Create(entCreateCompany);

                        if (entCreateCompany.Id != Guid.Empty)
                        {

                            rspCompany.CompanyGuid = entCreateCompany.Id;

                            Entity entRetrieveCustomer = null;
                            entRetrieveCustomer = crmAdapter.CRMAdminService().Retrieve(entCreateCompany.LogicalName, entCreateCompany.Id, new ColumnSet(AccountEntityAttributesNames.CompanyName));

                            if (entRetrieveCustomer != null)
                            {
                                rspCompany.CompanyName = crmCMs.GetAttributeValFromTargetEntity(entRetrieveCustomer, AccountEntityAttributesNames.CompanyName);
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

            return rspCompany;
        }

        private RetrieveCompaniesResponse RetrieveAccounts(Guid guidLoggedInUser)
        {
            RetrieveCompaniesResponse rspAccounts = new RetrieveCompaniesResponse();

            try
            {
                string strRegionsMetaData = crmFetchXML.GetAccounts();

                if (!string.IsNullOrWhiteSpace(strRegionsMetaData))
                {
                    EntityCollection entcolAccounts = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).RetrieveMultiple(new FetchExpression(strRegionsMetaData));

                    if (entcolAccounts != null && entcolAccounts.Entities != null && entcolAccounts.Entities.Count > 0)
                    {
                        rspAccounts.accountDataObj = new List<RetrieveCompanies>();

                        foreach (Entity item in entcolAccounts.Entities)
                        {
                            RetrieveCompanies _accounts = new RetrieveCompanies();
                            _accounts.CompanyGuid = crmCMs.GetAttributeValFromTargetEntity(item, AccountEntityAttributesNames.CompanyId);
                            _accounts.CompanyName = crmCMs.GetAttributeValFromTargetEntity(item, AccountEntityAttributesNames.CompanyName);
                            rspAccounts.accountDataObj.Add(_accounts);
                        }

                        rspAccounts.accountTypeETN = entcolAccounts.EntityName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rspAccounts;
        }

        [Route("AccountContactOasisCreation")]
        public ResponseClass AccountContactOasisCreation(CreateAccountContactOasisRequest createAccountContactOasisRequest)
        {
            try
            {
                exceptionResponseObjects.strAPIServiceName = "AccountContactOasisCreation";
                exceptionResponseObjects.strAPIPageName = strAPIPageName;
                exceptionResponseObjects.resultDetails = new ResultClass();

                HttpRequestHeaders reqHeader = this.Request.Headers;
                isAPITokenValidated = apiCMs.ValidateWithAPIToken(reqHeader);
                isAPIAuthKeyValidated = apiCMs.ValidateWithAPIAuthKey(reqHeader);
                if (isAPITokenValidated && isAPIAuthKeyValidated)
                {
                    #region Post Validate Session Check Conditions

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), createAccountContactOasisRequest.authenticateDetails);

                    #endregion

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        CreateAccountContactOasisResponse createdAccountInfo = CreateAccountAndContact(guidLoggedInUser, createAccountContactOasisRequest.createAccountContactOasis);
                        otptResponse = apiCMs.SetSuccessResponse(createdAccountInfo);
                        otptResponse.apiResult.resultDesDetails = "Account and contact was created successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                if(targerEnity== ContactEntityAttributesNames.EntityLogicalName)
                {
                    exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + " Target Entity: " + ContactEntityAttributesNames.EntityLogicalName;
                }
                else if(targerEnity == AccountEntityAttributesNames.EntityLogicalName)
                {
                    exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInCreateOperation + " Target Entity: " + ContactEntityAttributesNames.EntityLogicalName;
                }

                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }
            return otptResponse;
        }

        // POST: API/Company/PostUpdateCompany
        [Route("PostUpdateCompany")]
        public ResponseClass PostUpdateCompany(UpdateCompanyRequest rqtUpdateCompany)
        {
            exceptionResponseObjects.strAPIServiceName = "PostUpdateCompany";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtUpdateCompany.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        UpdateCompanyResponse updateCompanyInfo = UpdateCompanyMtd(guidLoggedInUser, rqtUpdateCompany.updateCompany);
                        otptResponse = apiCMs.SetSuccessResponse(updateCompanyInfo);
                        otptResponse.apiResult.resultDesDetails = "Company was updated successfully";
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInUpdateOperation + "Target table: " + AccountEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        // POST: API/Customer/PostRetrieveCompanies
        [Route("PostRetrieveCompanies")]
        public ResponseClass PostRetrieveCompanies(RetrieveCompanyRequest rqtGetCompanies)
        {
            exceptionResponseObjects.strAPIServiceName = "PostRetrieveCompanies";
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

                    Guid guidLoggedInUser = crmCMs.PostValidateSessionConditions(crmAdapter.CRMAdminService(), rqtGetCompanies.authenticateDetails);

                    #endregion                    

                    if (guidLoggedInUser != Guid.Empty)
                    {
                        RetrieveCompanyResponse retrieveCompanyInfo = RetrieveCompaniesMtd(rqtGetCompanies.retrieveCompany);
                        otptResponse = apiCMs.SetSuccessResponse(retrieveCompanyInfo);
                    }
                }

                #endregion                
            }
            catch (Exception ex)
            {
                exceptionResponseObjects.resultDetails.resultDes = customErrorMsgs.errorInRetrieveOperation + "Target table: " + AccountEntityAttributesNames.EntityLogicalName;
                exceptionResponseObjects.resultDetails.resultDesDetails = ex.Message;
                otptResponse = apiCMs.SetExceptionResponse(exceptionResponseObjects);
                apiCMs.BuildLogErrorException(ex, exceptionResponseObjects.strAPIServiceName);
            }

            return otptResponse;
        }

        private CreateAccountContactOasisResponse CreateAccountAndContact(Guid guidLoggedInUser, CreateAccountContactOasis createAccountContactOasis)
        {
            try
            {
                CreateAccountContactOasisResponse createAccountContactOasisResponse = new CreateAccountContactOasisResponse();
                string strExceptionMsg = string.Empty;
                if (createAccountContactOasis != null)
                {
                    Entity entCreateContact = null;
                    Entity entCreateAccount = null;
                    entCreateContact = new Entity(ContactEntityAttributesNames.EntityLogicalName);
                    entCreateAccount = new Entity(AccountEntityAttributesNames.EntityLogicalName);
                    if (!string.IsNullOrEmpty(createAccountContactOasis.FirstName))
                    {
                        entCreateContact.Attributes[ContactEntityAttributesNames.FirstName] = createAccountContactOasis.FirstName;

                    }
                    if (!string.IsNullOrEmpty(createAccountContactOasis.LastName))
                    {
                        entCreateContact.Attributes[ContactEntityAttributesNames.LastName] = createAccountContactOasis.LastName;

                    }

                    if (!string.IsNullOrEmpty(createAccountContactOasis.PersonalEmailAddress))
                    {
                        entCreateContact.Attributes[ContactEntityAttributesNames.PrimaryEmail] = createAccountContactOasis.PersonalEmailAddress;
                    }
                    //create account
                    if (!string.IsNullOrEmpty(createAccountContactOasis.CompanyEmailAddress))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.CompanyEmail] = createAccountContactOasis.CompanyEmailAddress;

                    }
                    if (createAccountContactOasis.Country != null && createAccountContactOasis.Country.Id != null)
                    {

                        entCreateAccount.Attributes[AccountEntityAttributesNames.Country] = new EntityReference(CountryEntityAttributeNames.EntityLogicalName, createAccountContactOasis.Country.Id);
                    }

                    if (!string.IsNullOrEmpty(createAccountContactOasis.CompanyName))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.CompanyName] = createAccountContactOasis.CompanyName;

                    }

                    if (!string.IsNullOrEmpty(createAccountContactOasis.Website))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.CompanyWebSite] = createAccountContactOasis.Website;

                    }

                    if (!string.IsNullOrEmpty(createAccountContactOasis.Address))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.CompanyAddress] = createAccountContactOasis.Address;

                    }
                    entCreateAccount.Attributes[AccountEntityAttributesNames.CompanyType] = new OptionSetValueCollection(new List<OptionSetValue> { new OptionSetValue(createAccountContactOasis.CompanyType)}) ;
                    if (createAccountContactOasis.OasisType.Contains("Domestic"))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.OasisTypes] = new OptionSetValue(Convert.ToInt32(OasisType.Domestic));
                    }
                    else if (createAccountContactOasis.OasisType.Contains("International"))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.OasisTypes] = new OptionSetValue(Convert.ToInt32(OasisType.International));
                    }

                    entCreateAccount.Attributes[AccountEntityAttributesNames.CompanyPhone] = createAccountContactOasis.MobileNumber;

                    if (!string.IsNullOrEmpty(createAccountContactOasis.InternationalMarket))
                    {
                        entCreateAccount.Attributes[AccountEntityAttributesNames.InternationalMaarket] = createAccountContactOasis.InternationalMarket;

                    }
                    if (string.IsNullOrEmpty(strExceptionMsg))
                    {
                       
                        var contactId = CreateOasisContact(guidLoggedInUser, entCreateContact, createAccountContactOasis.ProfileImage);
                        createAccountContactOasisResponse.ContactId = contactId;
                        var accountId = CreateOasisAccount(guidLoggedInUser, entCreateAccount);
                        Entity accountData = crmAdapter.CRMAdminService().Retrieve(entCreateAccount.LogicalName, entCreateAccount.Id, new ColumnSet(AccountEntityAttributesNames.AccountNumber));
                        if(accountData !=null)
                        {
                            createAccountContactOasisResponse.AccountNumber= crmCMs.GetAttributeValFromTargetEntity(accountData, AccountEntityAttributesNames.AccountNumber);
                        }
                        createAccountContactOasisResponse.AccountId = accountId;
                    }
                    else
                    {
                        throw new Exception(strExceptionMsg);
                    }
                }
                return createAccountContactOasisResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private string CreateOasisContact(Guid guidLoggedInUser, Entity entCreateContact,string ProfileImage)
        {
            try
            {
                entCreateContact.Id = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Create(entCreateContact);
                if (entCreateContact.Id != null)
                {
                    Entity update = new Entity("contact", entCreateContact.Id); 
                    update["entityimage"] = Convert.FromBase64String(ImageValidation(ProfileImage));
                    crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Update(update);
                }
                return entCreateContact.Id.ToString();
            }
            catch (Exception ex)
            {
                targerEnity = ContactEntityAttributesNames.EntityLogicalName;
                throw new Exception(ex.Message);
            }
        }
        private string ImageValidation(string ProfileImage)
        {
            if(ProfileImage.Contains("data:image"))
            {
                ProfileImage = ProfileImage.Split(',')[1];
                return ProfileImage;
            }
            return ProfileImage;
        }
        private string CreateOasisAccount(Guid guidLoggedInUser, Entity entCreateAccount)
        {
            try
            {
                entCreateAccount.Id = crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Create(entCreateAccount);
                return entCreateAccount.Id.ToString();
            }
            catch (Exception ex)
            {
                targerEnity = AccountEntityAttributesNames.EntityLogicalName;
                throw new Exception(ex.Message);
            }
        }
        private UpdateCompanyResponse UpdateCompanyMtd(Guid guidLoggedInUser, UpdateCompany updateCompanyDetails)
        {
            UpdateCompanyResponse rspCompany = new UpdateCompanyResponse();
            string strExceptionMsg = string.Empty;

            try
            {
                Entity entUpdateCompany = null;

                if (updateCompanyDetails != null)
                {
                    entUpdateCompany = new Entity(AccountEntityAttributesNames.EntityLogicalName);

                    if (!string.IsNullOrWhiteSpace(updateCompanyDetails.CompanyGuid.ToString()))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.CompanyId] = updateCompanyDetails.CompanyGuid;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingCompanyId;
                    }

                    if (!string.IsNullOrWhiteSpace(updateCompanyDetails.CompanyName))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.CompanyName] = updateCompanyDetails.CompanyName;
                    }
                    else
                    {
                        strExceptionMsg += customErrorMsgsForCompanies.missingCompanyName;
                    }

                    if (!string.IsNullOrEmpty(updateCompanyDetails.CompanyEmail))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.CompanyEmail] = updateCompanyDetails.CompanyEmail;
                    }

                    if (!string.IsNullOrEmpty(updateCompanyDetails.CompanyPhone))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.CompanyPhone] = updateCompanyDetails.CompanyPhone;
                    }

                    if (!string.IsNullOrEmpty(updateCompanyDetails.CompanyRegistrationNumber))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.CompanyRegistrationNumber] = updateCompanyDetails.CompanyRegistrationNumber;
                    }
                    if (!string.IsNullOrEmpty(updateCompanyDetails.MTCRNumber))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.MTCRNumber] = updateCompanyDetails.MTCRNumber;
                    }
                    if (!string.IsNullOrEmpty(updateCompanyDetails.MTCRMID))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.MTCRMID] = updateCompanyDetails.MTCRMID;
                    }
                    if (!string.IsNullOrEmpty(updateCompanyDetails.CompanyNameAR))
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.CompanyNameAR] = updateCompanyDetails.CompanyNameAR;
                    }
                    if (updateCompanyDetails.PreferredContactMethod != null)
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.PreferredContactMethod] = new OptionSetValue((int)updateCompanyDetails.PreferredContactMethod);
                    }
                    if (updateCompanyDetails.ParentAccount != null)
                    {
                        entUpdateCompany.Attributes[AccountEntityAttributesNames.ParentAccount] = new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, updateCompanyDetails.ParentAccount.Id);
                    }


                    //if (updateCompanyDetails.Region != null)
                    //{
                    //    entUpdateCompany.Attributes[AccountEntityAttributesNames.Region] = new EntityReference(RegionsEntityAttributeNames.EntityLogicalName, updateCompanyDetails.Region.Id);
                    //}
                    //else
                    //{
                    //    strExceptionMsg += customErrorMsgsForCompanies.missingRegion;
                    //}

                    //if (updateCompanyDetails.SubRegion != null && updateCompanyDetails.SubRegion.Id != null)
                    //{
                    //    entUpdateCompany.Attributes[AccountEntityAttributesNames.SubRegion] = new EntityReference(SubRegionsEntityAttributeNames.EntityLogicalName, updateCompanyDetails.SubRegion.Id);
                    //}
                    //else
                    //{
                    //    strExceptionMsg += customErrorMsgsForCompanies.missingSubRegion;
                    //}

                    //if (updateCompanyDetails.City != null && updateCompanyDetails.City.Id != null)
                    //{
                    //    entUpdateCompany.Attributes[AccountEntityAttributesNames.City] = new EntityReference(CitiesEntityAttributeNames.EntityLogicalName, updateCompanyDetails.City.Id);
                    //}
                    //else
                    //{
                    //    strExceptionMsg += customErrorMsgsForCompanies.missingCity;
                    //}

                    //if (updateCompanyDetails.Country != null && updateCompanyDetails.Country.Id != null)
                    //{

                    //    entUpdateCompany.Attributes[AccountEntityAttributesNames.Country] = new EntityReference(CountryEntityAttributeNames.EntityLogicalName, updateCompanyDetails.Country.Id);
                    //}
                    //else
                    //{
                    //    strExceptionMsg += customErrorMsgsForCompanies.missingCountry;
                    //}

                    if (string.IsNullOrWhiteSpace(strExceptionMsg))
                    {
                        //Update Company
                        if (updateCompanyDetails.CompanyGuid != Guid.Empty)
                        {
                            entUpdateCompany.Id = updateCompanyDetails.CompanyGuid;
                            crmAdapter.ImpersonatedCRMService(guidLoggedInUser).Update(entUpdateCompany);
                            rspCompany.CompanyGuid = entUpdateCompany.Id;
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

            return rspCompany;

        }

        private RetrieveCompanyResponse RetrieveCompaniesMtd(RetrieveCompanyObj companyDetails)
        {
            RetrieveCompanyResponse rspRetrieveCompany = null;
            string strExceptionMsg = string.Empty;

            try
            {
                string strCompanyMetaData = crmFetchXML.GetCompanies(companyDetails);

                EntityCollection entcolCompanies = crmAdapter.CRMAdminService().RetrieveMultiple(new FetchExpression(strCompanyMetaData));

                if (entcolCompanies != null && entcolCompanies.Entities != null && entcolCompanies.Entities.Count > 0)
                {
                    rspRetrieveCompany = new RetrieveCompanyResponse();
                    rspRetrieveCompany.LstCompanyRecords = new List<RspRetrieveCompanyObj>() { };

                    foreach (Entity entCompany in entcolCompanies.Entities)
                    {
                        RspRetrieveCompanyObj rspCompanyObj = new RspRetrieveCompanyObj();
                        rspCompanyObj.CompanyGuid = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.CompanyId);
                        rspCompanyObj.CompanyName = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.CompanyName);
                        rspCompanyObj.CompanyEmail = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.CompanyEmail);
                        rspCompanyObj.CompanyPhone = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.CompanyPhone);
                        rspCompanyObj.CompanyRegistrationNumber = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.CompanyRegistrationNumber);
                        rspCompanyObj.MTCRNumber = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.MTCRNumber);
                        rspCompanyObj.MTCRMID = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.MTCRMID);
                        rspCompanyObj.CompanyNameAR = crmCMs.GetAttributeValFromTargetEntity(entCompany, AccountEntityAttributesNames.CompanyNameAR);
                        //rspCompanyObj.PreferredContactMethod = apiCMs.SetCustomFormatOptionsetValue(entCompany, AccountEntityAttributesNames.PreferredContactMethod);
                        rspCompanyObj.ParentAccount = apiCMs.SetCustomFormatEntRefValue(entCompany, AccountEntityAttributesNames.ParentAccount);
                        rspCompanyObj.Region = apiCMs.SetCustomFormatEntRefValue(entCompany, AccountEntityAttributesNames.Region);
                        rspCompanyObj.SubRegion = apiCMs.SetCustomFormatEntRefValue(entCompany, AccountEntityAttributesNames.SubRegion);
                        rspCompanyObj.City = apiCMs.SetCustomFormatEntRefValue(entCompany, AccountEntityAttributesNames.City);
                        rspCompanyObj.Country = apiCMs.SetCustomFormatEntRefValue(entCompany, AccountEntityAttributesNames.Country);
                        rspRetrieveCompany.LstCompanyRecords.Add(rspCompanyObj);
                    }

                    rspRetrieveCompany.companyETN = entcolCompanies.EntityName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return rspRetrieveCompany;
        }
    }
}