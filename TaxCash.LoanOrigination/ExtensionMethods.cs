using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TaxCash.LoanOrigination.Connected_Services.NLSWS;
using TaxCash.LoanOrigination.Exceptions;
using TaxCash.LoanOrigination.Properties;

namespace TaxCash.LoanOrigination
{
    
    public static class ExtensionMethods
    {
        public static string ToFlag(this bool b)
        {
            return b ? "1" : "0";
        }

        public static async Task<TResponse> ToAsyncTask<TArg1, TResponse>(this Func<TArg1, AsyncCallback, object, IAsyncResult> beginFunc, Func<IAsyncResult, TResponse> endFunc, TArg1 arg1)
        {
            return await Task<TResponse>.Factory.FromAsync(beginFunc, endFunc, arg1, null);
        }

        public static NlsResponseCode GetNlsResponseCode(this string response)
        {

            if (string.IsNullOrEmpty(response) || !response.StartsWith("<"))
            {
                return NlsResponseCode.Success;
            }
            string errorCode = string.Empty;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);
                if ("ERRORCODE".Equals(xmlDoc.DocumentElement.Name))
                {
                    errorCode = xmlDoc.DocumentElement.InnerText;
                }
            }
            catch (Exception)
            {
            }
            if (int.TryParse(errorCode, out int errorCodeInt))
            {
                return (NlsResponseCode)errorCodeInt;
            }
            return NlsResponseCode.UnknownResponseCode;
        }

        public static void ProcessNlsResponseCode(this string response)
        {
            NlsResponseCode responseCode = GetNlsResponseCode(response);
            switch (responseCode)
            {
                case NlsResponseCode.Success: return;
                case NlsResponseCode.SessionExpired: throw new TaxCashSessionExpiredException(Resources.SessionExpired);
                case NlsResponseCode.InvalidCredentials: throw new TaxCashPermissionDeniedException(Resources.InvalidCredentials);
                case NlsResponseCode.AccountLocked: throw new TaxCashPermissionDeniedException(Resources.AccountLocked);
                case NlsResponseCode.AlreadyRegistered: throw new TaxCashBadRequestException(Resources.AccountAlreadyRegistered);
                case NlsResponseCode.InvalidHintAnswer: throw new TaxCashBadRequestException(Resources.InvalidSecurityAnswer);
                case NlsResponseCode.InvalidHintQuestion: throw new TaxCashBadRequestException(Resources.InvalidSecurityQuestion);
                case NlsResponseCode.InvalidNewPassword: throw new TaxCashBadRequestException(Resources.InvalidNewPassword);
                case NlsResponseCode.InvalidOldPassword: throw new TaxCashPermissionDeniedException(Resources.InvalidCredentials);
                case NlsResponseCode.InvalidToken: throw new TaxCashInfrastructureException(Resources.InvalidToken);
                case NlsResponseCode.InvalidUserName: throw new TaxCashBadRequestException(Resources.InvalidUserName);
                case NlsResponseCode.InvalidXml: throw new TaxCashInfrastructureException(Resources.InvalidXml);
                case NlsResponseCode.InvalidXmlTag: throw new TaxCashInfrastructureException(Resources.InvalidXmlTag);
                case NlsResponseCode.NothingToVerify: throw new TaxCashBadRequestException(Resources.NothingToVerify);
                case NlsResponseCode.PasswordAnswer2Requred: throw new TaxCashBadRequestException(Resources.SecurityAnswerRequired);
                case NlsResponseCode.PasswordAnwwer1Required: throw new TaxCashBadRequestException(Resources.SecurityAnswerRequired);
                case NlsResponseCode.PasswordTooShort: throw new TaxCashBadRequestException(Resources.PasswordTooShort);
                case NlsResponseCode.UnableToVerifyContact: throw new TaxCashBadRequestException(Resources.VerifyContactFailed);
                case NlsResponseCode.UserNameNotUnique: throw new TaxCashBadRequestException(Resources.AccountAlreadyRegistered);
                case NlsResponseCode.UserNameTooShort: throw new TaxCashBadRequestException(Resources.EmailTooShort);
                case NlsResponseCode.WebAccessDisabled: throw new TaxCashConfigurationException(Resources.WebServiceAccessDisabled);
                case NlsResponseCode.UnableToLoadNLSProcReqdll: throw new TaxCashInfrastructureException(Resources.UnableToLoadInternalDLL);
                default: throw new TaxCashInfrastructureException(Resources.InternalError + response ?? string.Empty);
            }
        }

        public static string GetNlsResponseError(this NlsResponseCode response)
        {
            switch (response)
            {
                case NlsResponseCode.Success: return "SUCCESS";
                case NlsResponseCode.UnableToLoadNLSProcReqdll: return "Unable to load NLSProcReq.dll";
                case NlsResponseCode.InvalidXml: return "Invalid XML";
                case NlsResponseCode.InvalidXmlTag: return "Invalid XML tag";
                case NlsResponseCode.SessionExpired: return "Session expired";
                case NlsResponseCode.ConnectionError: return "Unable to connect to database";
                case NlsResponseCode.DatabaseError: return "Database error encountered";
                case NlsResponseCode.InvalidToken: return "Invalid token";
                case NlsResponseCode.InternalError: return "Error";
                case NlsResponseCode.InvalidUserName: return "Invalid User Name";
                case NlsResponseCode.InvalidOldPassword: return "Invalid Old Password";
                case NlsResponseCode.InvalidNewPassword: return "Invalid New Password";
                case NlsResponseCode.InvalidCredentials: return "Invalid username or password";
                case NlsResponseCode.AccountLocked: return "Account is locked. Attempted more than 5x in a day.";
                case NlsResponseCode.WebAccessDisabled: return "Web access disabled";
                case NlsResponseCode.GeneralError: return "General Error";
                case NlsResponseCode.NothingToVerify: return "VERIFYCONTACT_NOTHING_TO_VERIFY";
                case NlsResponseCode.UnableToVerifyContact: return "Unable to Verify Contact";
                case NlsResponseCode.AlreadyRegistered: return "You are already registered.";
                case NlsResponseCode.UserNameTooShort: return "User Name Too Short";
                case NlsResponseCode.PasswordTooShort: return "Password Too Short";
                case NlsResponseCode.UserNameNotUnique: return "Your email has already been registered.";
                case NlsResponseCode.PasswordAnwwer1Required: return "Password Answer #1 Required";
                case NlsResponseCode.PasswordAnswer2Requred: return "Password Answer #2 Required";
                case NlsResponseCode.EmailNotSet: return "Email Not Set";
                case NlsResponseCode.InvalidHintQuestion: return "Invalid Hint Question";
                case NlsResponseCode.InvalidHintAnswer: return "Invalid Hint Answer";
                default: return "Unknown error code";
            }
        }

        public static string SerializeToXml<TObject>(this TObject obj)
            where TObject : class
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(typeof(TObject));
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
        }

        public static TObject DeserializeFromXml<TObject>(this string serializedObj)
            where TObject : class
        {
            using (var stringReader = new System.IO.StringReader(serializedObj))
            {
                var serializer = new XmlSerializer(typeof(TObject));
                return serializer.Deserialize(stringReader) as TObject;
            }
        }
    }
}
