using CRM.Common.Wrappers;
using Microsoft.Xrm.Sdk;

namespace PluginAndWf.Plugins
{
    public class ContactEntity : BasePlugin
    {
        public ContactEntity() : base()
        {
            //Устанавливаем параметры при которых должен вызываться плагин (item)
            RegisteredEvents.Add(new PluginEvent()
            { EntityName = "Contact", Stage = eStage.PreOperation, MessageName = eMessageName.Update, Mode = eMode.Synchronous, PluginAction = UpdateField });
        }

        private void UpdateField(LocalPluginContext localContext)
        {
            Entity entityContact = (Entity)localContext._Context.InputParameters["Target"];
        }
    }
}
