using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;

namespace CRM.Common.Wrappers
{
    public abstract class BasePlugin : IPlugin
    {
        internal BasePlugin()
        {

        }

        /// <summary>
        /// Initializes a new instance of the CRM.Common.Wrappers.BasePlugin class.
        /// </summary>
        internal BasePlugin(string unsecureConfig, string secureConfig)
        {
            this.UnsecureConfig = unsecureConfig;
            this.SecureConfig = secureConfig;
        }

        protected class LocalPluginContext : IDisposable
        {
            internal IServiceProvider _ServiceProvider { get; private set; }
            /// <summary>The IPluginExecutionContext provides access to the context for the event that executed the plugin</summary>
            internal IPluginExecutionContext _Context { get; private set; }
            /// <summary>The ITracingService enables writing to the tracing log</summary>
            internal ITracingService _TracingService { get; private set; }
            /// <summary>The IOrganizationServiceFactory interface provides access to a service variable that implements the IOrganizationService interface</summary>
            internal IOrganizationServiceFactory _ServiceFactory { get; private set; }
            /// <summary>The IOrganizationService interface which provides the methods you will use to interact with the service to create the task</summary>
            internal IOrganizationService _Service { get; private set; }
            /// <summary>You can write LINQ queries against Microsoft Dynamics 365 data</summary>
            internal OrganizationServiceContext _CrmContext { get; private set; }

            internal eStage Stage { get { return (eStage)this._Context.Stage; } }
            internal eMode Mode { get { return (eMode)this._Context.Mode; } }
            internal int Depth { get { return this._Context.Depth; } }
            internal string MessageName { get { return this._Context.MessageName; } }
            internal string EntityName { get { return this._Context.PrimaryEntityName; } }

            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                    throw new ArgumentNullException("serviceProvider");

                this._ServiceProvider = serviceProvider;

                // Obtain the tracing service from the service provider.
                this._TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the execution context service from the service provider.
                this._Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the Organization Service factory service from the service provider
                this._ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                this._Service = this._ServiceFactory.CreateOrganizationService(this._Context.UserId);

                // Generate the CrmContext to use with LINQ etc
                this._CrmContext = new OrganizationServiceContext(this._Service);
            }

            /// <summary>
            /// Returns image by name for the pipeline execution
            /// </summary>
            internal M GetPreImage<M>(string namePreImage) where M : Entity
            {
                if (this._Context.PreEntityImages.Contains(namePreImage))
                    return GetEntityAsType<M>(this._Context.PreEntityImages[namePreImage]);
                return default;
            }
            /// <summary>
            /// Returns image by name for the pipeline execution
            /// </summary>
            internal M GetPostImage<M>(string namePostImage) where M : Entity
            {
                if (this._Context.PostEntityImages.Contains(namePostImage))
                    return GetEntityAsType<M>(this._Context.PostEntityImages[namePostImage]);
                return default;
            }
            /// <summary>
            /// Returns the 'Target' of the message if available
            /// This is an 'Entity' instead of the specified type in order to retain the same instance of the 'Entity' object. This allows for updates to the target in a 'Pre' stage that
            /// will get persisted during the transaction.
            /// </summary>
            internal M GetTarget<M>() where M: Entity
            {
                if (this._Context.InputParameters.Contains("Target"))
                {
                    M targetParameter = this._Context.InputParameters["Target"] as M;
                    return targetParameter.ToEntity<M>();
                }

                return default;
            }
            /// <summary>
            /// Returns the 'Target' of the message as an EntityReference if available
            /// </summary>
            internal EntityReference GetTargetEntityReference()
            {
                if (this._Context.InputParameters.Contains("Target"))
                        return this._Context.InputParameters ["Target"] as EntityReference;
                    return null;
            }   
            
            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this._TracingService == null) return;

                if (this._Context == null)
                    this._TracingService.Trace(message);
                else
                {
                    this._TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this._Context.CorrelationId,
                        this._Context.InitiatingUserId);
                }
            }
            
            public void Dispose()
            {
                if (this._CrmContext != null)
                    this._CrmContext.Dispose();
            }
            
            private M GetEntityAsType<M>(Entity entity) where M : Entity
            {
                if (typeof(M) == entity.GetType())
                    return entity as M;
                else
                    return entity.ToEntity<M>();
            }
        }

        protected class PluginEvent
        {
            /// <summary>
            /// Execution pipeline stage that the plugin should be registered against.
            /// </summary>
            public eStage Stage { get; set; }

            public eMode Mode { get; set; }

            /// <summary>
            /// Logical name of the entity that the plugin should be registered against. Leave 'null' to register against all entities.
            /// </summary>
            public string EntityName { get; set; }
            /// <summary>
            /// Name of the message that the plugin should be triggered off of.
            /// </summary>
            public string MessageName { get; set; }
            /// <summary>
            /// Method that should be executed when the conditions of the Plugin Event have been met.
            /// </summary>
            public Action<LocalPluginContext> PluginAction { get; set; }
        }

        private Collection<PluginEvent> registeredEvents;

        /// <summary>
        /// Gets the List of events that the plug-in should fire for. Each List
        /// </summary>
        protected Collection<PluginEvent> RegisteredEvents
        {
            get
            {
                if (this.registeredEvents == null)
                    this.registeredEvents = new Collection<PluginEvent>();
                return this.registeredEvents;
            }
        }

        /// <summary>
        /// Un secure configuration specified during the registration of the plugin step
        /// </summary>
        public string UnsecureConfig { get; private set; }

        /// <summary>
        /// Secure configuration specified during the registration of the plugin step
        /// </summary>
        public string SecureConfig { get; private set; }

        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            using(LocalPluginContext localContext = new LocalPluginContext(serviceProvider))
            {
                localContext._TracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

                try
                {
                    // Iterate over all of the expected registered events to ensure that the plugin
                    // has been invoked by an expected event
                    var entityActions =
                        (from a in RegisteredEvents
                         where (
                            (int)a.Stage == (int)localContext.Stage &&
                            (int)a.Mode == (int)localContext.Mode &&
                             (string.IsNullOrWhiteSpace(a.MessageName) ? true : a.MessageName.ToLowerInvariant() == localContext.MessageName.ToLowerInvariant()) &&
                             (string.IsNullOrWhiteSpace(a.EntityName) ? true : a.EntityName.ToLowerInvariant() == localContext.EntityName.ToLowerInvariant())
                         )
                         select a.PluginAction);

                    if (entityActions.Any())
                    {
                        foreach (var entityAction in entityActions)
                        {
                            localContext._TracingService.Trace(string.Format(
                                CultureInfo.InvariantCulture,
                                "{0} is firing for Entity: {1}, Message: {2}, Method: {3}",
                                this.GetType().ToString(),
                                localContext.EntityName,
                                localContext.MessageName,
                                entityAction.Method.Name));

                            entityAction.Invoke(localContext);
                        }
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    localContext._TracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", ex.ToString()));
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    localContext._TracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", ex.ToString()));
                    throw;
                }
                finally
                {
                    localContext._TracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.GetType().ToString()));
                }
            }
        }

        public enum eStage
        {
            PreValidation = 10,
            PreOperation = 20,
            PostOperation = 40
        }

        public enum eMode
        {
            Synchronous = 0,
            Asynchronous = 1
        }

        public class eMessageName
        {
            public const string Create = "Create";
            public const string Update = "Update";
        }
    }
}
