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
            QueryExpression qeAccount = new QueryExpression()
            {
                EntityName = "account",
                ColumnSet = new ColumnSet("name"),
                TopCount = 1
            };

            Entity entityAccount = service.RetrieveMultiple(qeAccount)?.Entities[0];

            return entityAccount;
        }
    }
}
