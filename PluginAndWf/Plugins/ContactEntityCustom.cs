using CRM.Common.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginAndWf.Plugins
{
    public class ContactEntityCustom : BasePluginCustom
    {
        public ContactEntityCustom() : base()
        {
            RegisteredEvents().Add(new PluginEvent() 
            { EntityName = "Contact", Stage = BasePlugin.eStage.PreOperation, Mode = BasePlugin.eMode.Synchronous, MessageName = "Create", });
        }

        private void UpdateFieldCustom(LocalPluginContext localContext)
        {
            string entityName = localContext.EntityName;
        }
    }
}
