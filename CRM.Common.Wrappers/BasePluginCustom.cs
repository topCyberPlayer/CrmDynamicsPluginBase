using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static CRM.Common.Wrappers.BasePlugin;

namespace CRM.Common.Wrappers
{
    public class BasePluginCustom : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            LocalPluginContext context = new LocalPluginContext();
            //Найти плагины удовлетворящие условиям context
            foreach (PluginEvent item in RegisteredEvents())
            {
                if (item.Stage == context.Stage)
                {
                    if (item.Mode == context.Mode)
                    {
                        if (item.MessageName == context.MessageName)
                        {
                            if (item.EntityName == context.EntityName)
                            {

                            }
                        }
                    }
                }
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
        }

        protected class LocalPluginContext
        {
            internal IPluginExecutionContext _Context { get; private set; }

            internal eStage Stage { get { return (eStage)this._Context.Stage; } }
            internal eMode Mode { get { return (eMode)this._Context.Mode; } }
            internal string MessageName { get { return this._Context.MessageName; } }

            internal string EntityName { get { return this._Context.PrimaryEntityName; } }
        }

        protected Collection<PluginEvent> RegisteredEvents()
        {
            Collection<PluginEvent> registeredEvents = new Collection<PluginEvent>();

            return registeredEvents;
        }
    }
}
