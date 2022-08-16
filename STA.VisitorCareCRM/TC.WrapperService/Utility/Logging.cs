using log4net;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using TC.WrapperService.Interfaces;

namespace TC.WrapperService.Utility
{
    public class Logging : ILogging
    {
        #region Vars

        private readonly ILog logger = LogManager.GetLogger(typeof(Logging));
        private static volatile Logging instance;
        private static object syncRoot = new Object();

        #endregion

        private Logging() { }

        public static Logging Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Logging();
                        }
                    }
                }

                return instance;
            }
        }

        public void Error(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            logger.Error(string.Format("{0} {1} {2} {3}", memberName, filePath, lineNumber, message));
        }

        public void Exception(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            logger.Fatal(string.Format("{0} {1} {2} {3}", memberName, filePath, lineNumber, message));
        }

        public void Information(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            logger.Info(string.Format("{0} {1} {2} {3}", memberName, filePath, lineNumber, message));
        }

        public void Verbose(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            logger.Debug(string.Format("{0} {1} {2} {3}", memberName, filePath, lineNumber, message));
        }

        public void Warning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            logger.Warn(string.Format("{0} {1} {2} {3}", memberName, filePath, lineNumber, message));
        }

        public static void WriteLog(string message, string filePath)
        {
            try
            {
                WriteLogInternal(string.Format("{0}:{1}", DateTime.Now, message), filePath);
            }
            catch (Exception ex)
            { }
        }

        public static void WriteLogInternal(string message, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine(message);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public class CustomErrorMessages
        {
            public string missingAuthenticateDetails = "Authenticate Details in Request is 'Null'" + Environment.NewLine;
            public string missingUserName = "'UserName' in Request Authenticate Details is 'Null or Empty or Whitespace'" + Environment.NewLine;
            public string missingLoggedInUserId = "'LoggedUserId' from Request Authenticate Details is either 'Guid.Empty' or 'Not a Valid Guid'" + Environment.NewLine;
            public string errorInRetrieveOperation = "Error in Retrieve Operation. Please contact System Administrator." + Environment.NewLine;
            public string errorInCreateOperation = "Error in Create Operation. Please contact System Administrator." + Environment.NewLine;
            public string errorInUpdateOperation = "Error in Update Operation. Please contact System Administrator." + Environment.NewLine;
            public string errorInSendSMSOperation = "Error in Send SMS Operation. Please contact System Administrator." + Environment.NewLine;
            public string missingOperationTypeVale = "Missing the 'Operation Type' value declaration in respective request. Please contact System Administrator." + Environment.NewLine;
        }

        public class CustomErrorMsgsForCase
        {
            public string missingCaseDetails = "'Case Details' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSHDetails = "'Social Handle Details' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCaseType = "'Case Type' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCategory = "'Case Category' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSubCategory = "'Case Subcategory' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCaseOrigin = "'Channel Origin' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSMPostId = "'Social Media Post Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
        }

        public class CustomErrorMsgsForCustomer
        {
            public string missingCustomerDetails = "'Customer Details' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSHDetails = "'Social Handle Details' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingChannelOrigin = "'Channel Origin' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingFirstName = "'First Name' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingLastName = "'Last Name' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingPrimaryEmail = "'Primary Email' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingPrimaryContactNo = "'Primary Contact No.' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCustomerCategory = "'Customer Category' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCustomerCountry = "'Country of Residence' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCompanyName = "'Company Name' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCustomerGuid = "'Customer Guid' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
        }


        public class CustomErrorMsgsForCompanies
        {
            public string missingRegion = "'Region' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCity = "'City' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSubRegion = "'Sub Region' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCountry = "'Country' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCompanyName = "'Company Name' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCompanyId = "'Company Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
        }


        public class CustomErrorMsgsForCaseCategory
        {
            public string missingCaseType = "'Case Type ID' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
        }

        public class CustomErrorMsgsForCaseSubCategory
        {
            public string missingCaseCategory = "'Category ID' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
        }

        public class CustomErrorMsgsForConversation
        {
            public string missingSubject = "'Subject' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingIncomingMsg = "'Incoming Message' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCaseRefNo = "'CaseRefNo' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingChannelOrigin = "'Channel Origin' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingEmailAddress = "'Email Address' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingMobileNumber = "'Mobile Number' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCaseType = "'Case Type' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSMPostId = "'Social Media Post Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string inValidChannelOriginCode = "'Channel Origin Code' parameter value in Request Body is invalid/non-existence value" + Environment.NewLine;
        }

        public class CustomErrorMsgsForSMSActivity
        {
            public string missingSendSMSDetails = "'Send SMS' object in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingAppId = "'App Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSenderId = "'Sender Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingMessage = "'Message' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingRecipient = "'Recipient' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string inValidRecipientVal = "'Recipient' parameter in Request Body is Invalid value" + Environment.NewLine;
        }

        public class CustomErrorMsgsForSocialMediaHandle
        {
            public string missingUserName = "'User Name' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingCustomer = "'Customer Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSocialChannel = "'Social Channel' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string missingSocialMediaHandleId = "'Social Media Handle Id' parameter in Request Body is Null (or) Empty" + Environment.NewLine;
            public string inValidCustomerId = "'Customer' parameter in Request Body is Invalid/Non-Existence value" + Environment.NewLine;
            public string inValidSocialChannel = "'SocialChannel' parameter in Request Body is Invalid/Non-Existence value" + Environment.NewLine;
        }
    }
}