using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using TC.WrapperService.Interfaces;

namespace TC.WrapperService.Utility
{
    public class CRMAdapter : IAdapter
    {
        private OrganizationServiceProxy _crmService = null;
        private ILogging logger;
        private string connectionString;       

        public class SortingAttribute
        {
            public string AttributeName { get; set; }
            public string IsDescending { get; set; }
        }

        public class FilteringAttribute
        {
            public string AttributeName { get; set; }
            public string AttributeValue { get; set; }
            public string ConditionOperator { get; set; }
        }

        public class PaginationClass
        {
            public int Page { get; set; }
            public string PagingCookie { get; set; }
            public int RecordCount { get; set; }
        }

        private OrganizationServiceProxy CRMService
        {
            get
            {
                if (_crmService == null)
                {
                    try
                    {
                        IOrganizationService crmOrgService = null;
                        string connectionString = ConfigurationManager.ConnectionStrings["Xrm"].ConnectionString;
                        string password = connectionString.Split(';')[4].Replace("Password=", "").Trim();
                        string decryptedPassword = Crypto.TripleDESDecrypt(password);
                        connectionString = connectionString.Replace(password, decryptedPassword);

                        CrmServiceClient conn = new CrmServiceClient(connectionString);

                        ////todo test this timeout start
                        //int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["OrganizationServiceProxyTimeout"]);
                        //conn.OrganizationServiceProxy.Timeout = new TimeSpan(0, timeout, 0);
                        ////todo test this timeout End

                        crmOrgService = conn.OrganizationWebProxyClient != null ? conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

                        if (crmOrgService == null)
                        {
                            throw new Exception(conn.LastCrmError);
                        }

                        _crmService = (OrganizationServiceProxy)crmOrgService;
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Exception("Error while creating CRM Connection");
                        HandleCRMException(ex);
                        throw;
                    }
                }
                return _crmService;
            }
        }

        public OrganizationServiceProxy ImpersonatedCRMService(Guid serviceCallerId)
        {
            if (_crmService == null)
            {
                _crmService = CRMService;
            }
            if (serviceCallerId != Guid.Empty && (_crmService.CallerId == Guid.Empty || _crmService.CallerId != serviceCallerId))
            {
                _crmService.CallerId = serviceCallerId;
            }
            return _crmService;
        }

        public OrganizationServiceProxy CRMAdminService()
        {
            if (_crmService == null)
            {
                _crmService = CRMService;
            }
            if (_crmService.CallerId != Guid.Empty)
            {
                _crmService.CallerId = Guid.Empty;
            }
            return _crmService;
        }

        public void DisposeCRMService()
        {
            if (_crmService != null)
            {
                _crmService.Dispose();
            }
        }        

        public ILogging Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = Logging.Instance;
                }
                return logger;
            }
            set
            {
                logger = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = value;
            }
        }

        public bool IsAdapterActive
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public CRMAdapter(ILogging _logger)
        {
            this.Logger = _logger;
        }

        private void HandleCRMException(Exception ex)
        {
            this.Logger.Exception(ex.ToString());
            //if (ex is FaultException<OrganizationServiceFault> || ex is TimeoutException || ex is SecurityTokenValidationException
            //    || ex is ExpiredSecurityTokenException || ex is SecurityAccessDeniedException || ex is MessageSecurityException || ex is SecurityNegotiationException)
            //{
            //string subject = System.Configuration.ConfigurationManager.AppSettings["Email_Subject_CRMError"];
            //SendEmailAsync(Logger, subject, ex.ToString());
            //}
        }        
    }
}