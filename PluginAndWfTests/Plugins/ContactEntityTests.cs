using CRM.Common.Connection;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace PluginAndWf.Plugins.Tests
{
    [TestClass()]
    public class ContactEntityTests
    {
        private readonly IOrganizationService service;

        public ContactEntityTests()
        {
            service = CrmConnection.GetCrmServiceClient(CrmConnection.UrlCrm.CrmTest);
        }


        [TestMethod()]
        public void ContactEntityTest()
        {
            XrmRealContext realContext = new XrmRealContext(service);

            QueryExpression quContact = new QueryExpression()
            {
                EntityName = "Contact",
                ColumnSet = new ColumnSet("lastname", "firstname"),
                TopCount = 1
            };

            Entity entityTarget = service.RetrieveMultiple(quContact).Entities[0];

            ParameterCollection inputParameters = new ParameterCollection
            {
                { "Target", entityTarget }
            };

            XrmFakedPluginExecutionContext fakedPluginContext = realContext.GetDefaultPluginContext();
            fakedPluginContext.MessageName = "Update";
            fakedPluginContext.Mode = (int)ProcessingStepMode.Synchronous;
            fakedPluginContext.Stage = (int)ProcessingStepStage.Preoperation;
            fakedPluginContext.InputParameters = inputParameters;

            realContext.ExecutePluginWith<ContactEntity>(fakedPluginContext);
        }
    }
}