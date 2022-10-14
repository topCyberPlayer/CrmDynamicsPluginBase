using CRM.Common.Wrappers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginAndWf.Plugins
{
    public class ContactEntity : BasePlugin
    {
        public ContactEntity() : base()
        {
            RegisteredEvents.Add(new PluginEvent()
            { EntityName = "Contact", Stage = eStage.PreOperation, MessageName = eMessageName.Update, Mode = eMode.Synchronous, PluginAction = UpdateField });
        }

        private void UpdateField(LocalPluginContext localContext)
        {
            Entity entityContact = (Entity)localContext._Context.InputParameters["Target"];
        }
    }
}
