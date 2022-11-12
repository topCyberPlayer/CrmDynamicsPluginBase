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
        [TestMethod()]
        public void ContactEntityTest()
        {
            XrmFakedContext xrmContext = new XrmFakedContext();
            xrmContext.UsePipelineSimulation = true;

            Entity entityTarget = new Entity("contact")
            {
                Id = new System.Guid(),
            };

            ParameterCollection inputParameters = new ParameterCollection
            {
                { "Target", entityTarget }
            };

            //Устнавливаем параметры для context
            XrmFakedPluginExecutionContext fakedPluginContext = xrmContext.GetDefaultPluginContext();
            fakedPluginContext.MessageName = "Update";
            fakedPluginContext.Mode = (int)ProcessingStepMode.Synchronous;
            fakedPluginContext.Stage = (int)ProcessingStepStage.Preoperation;
            fakedPluginContext.InputParameters = inputParameters;

            xrmContext.ExecutePluginWith<ContactEntityCustom>(fakedPluginContext);
        }
    }
}