using Microsoft.Xrm.Sdk.Client;
using System;
using System.Net;
using System.ServiceModel.Description;

namespace CRM.Common.Connection
{
    public class CrmConnection
    {
        /// <summary>
        /// Connect as current user.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static OrganizationServiceProxy GetCrmServiceClient(UrlCrm url)
        {
            Uri uri = new Uri(SelectUrlCrm(url));

            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

            OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(uri, null, clientCredentials, null);

            return serviceProxy;
        }

        /// <summary>
        /// Connect as different user.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="yourUserName"></param>
        /// <param name="yourPassword"></param>
        /// <returns></returns>
        public static OrganizationServiceProxy GetCrmServiceClient(UrlCrm url, string yourUserName, string yourPassword)
        {
            Uri uri = new Uri(SelectUrlCrm(url));

            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = yourUserName;
            clientCredentials.UserName.Password = yourPassword;

            OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(uri, null, clientCredentials, null);

            return serviceProxy;
        }


        private static string SelectUrlCrm(UrlCrm urlCrm)
        {
            const string end = "/XRMServices/2011/Organization.svc";

            switch (urlCrm)
            {
                case UrlCrm.CrmDev:
                    return "https://crm.dev.com/organization1" + end;

                case UrlCrm.CrmTest:
                    return "https://crm.test.com/organization1" + end;

                case UrlCrm.CrmProd:
                    return "https://crm.prod.com/organization1" + end;

                default:
                    return null;
            }
        }

        public enum UrlCrm
        {
            CrmDev,
            CrmTest,
            CrmProd
        }
    }
}