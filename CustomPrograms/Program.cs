using CRM.Common.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace CustomPrograms
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Entity entityAccount = new CrmWork().GetRandomRecord();
        }
    }

    internal class CrmWork
    {
        private readonly IOrganizationService service;
        public CrmWork()
        {
            service = CrmConnection.GetCrmServiceClient(CrmConnection.UrlCrm.CrmTest);
        }
        internal Entity GetRandomRecord()
        {
            string entityName = "account";
            Guid entityid = new Guid("");
            ColumnSet columns = new ColumnSet("name");

            Entity entityAccount = service.Retrieve(entityName, entityid, columns);

            return entityAccount;
        }
    }
}
