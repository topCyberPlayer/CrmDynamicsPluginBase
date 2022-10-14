using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Globalization;

namespace CRM.Common.Wrappers
{
    public abstract class BaseWorkflow : CodeActivity
    {
        protected class LocalWorkflowContext : IDisposable
        {
            public CodeActivityContext _CodeActivityContext { get; private set; }
            public IWorkflowContext _WorkflowContext { get; private set; }
            public IOrganizationServiceFactory _ServiceFactory { get; private set; }
            public OrganizationServiceContext _CrmContext { get; private set; }
            public IOrganizationService _OrganizationService { get; private set; }
            public ITracingService _TracingService { get; private set; }
            
            internal LocalWorkflowContext(CodeActivityContext codeActivityContext)
            {
                if (codeActivityContext == null)
                    throw new ArgumentNullException("codeActivityContext");

                // Set the code activity context
                this._CodeActivityContext = codeActivityContext;

                // Obtain the workflow context from the service provider.
                this._WorkflowContext = codeActivityContext.GetExtension<IWorkflowContext>();

                // Obtain the tracing service from the service provider.
                this._TracingService = codeActivityContext.GetExtension<ITracingService>();

                // Obtain the Organization Service factory service from the service provider
                this._ServiceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();

                // Use the factory to generate the Organization Service.
                this._OrganizationService = this._ServiceFactory.CreateOrganizationService(this._WorkflowContext.UserId);

                // Generate the CrmContext to use with LINQ etc
                this._CrmContext = new OrganizationServiceContext(this._OrganizationService);
            }
            
            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this._TracingService == null) return;

                if (this._WorkflowContext == null)
                    this._TracingService.Trace(message);
                else
                {
                    this._TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this._WorkflowContext.CorrelationId,
                        this._WorkflowContext.InitiatingUserId);
                }
            }
            public void Dispose()
            {
                if (this._CrmContext != null)
                    this._CrmContext.Dispose();
            }
        }

        protected override void Execute(CodeActivityContext context)
        {
            using (LocalWorkflowContext localContext = new LocalWorkflowContext(context))
            {
                localContext.Trace("Calling ExecuteInternal");
                try
                {
                    this.ExecuteInternal(localContext);
                }
                catch (Exception ex)
                {
                    localContext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", ex.ToString()));
                    throw;
                }
                finally
                {
                    localContext.Trace("ExecuteInternal finished");
                }
            }
        }

        protected abstract void ExecuteInternal(LocalWorkflowContext context);
    }
}
