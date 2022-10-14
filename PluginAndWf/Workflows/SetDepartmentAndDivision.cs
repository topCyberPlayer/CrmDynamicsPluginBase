using CRM.Common.Wrappers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginAndWf.Workflows
{
    public class SetDepartmentAndDivision : BaseWorkflow
    {
        [RequiredArgument]
        [Input("Team")]
        [ReferenceTarget("Team")]
        public InArgument<EntityReference> TeamInput { get; set; }
        protected override void ExecuteInternal(LocalWorkflowContext context)
        {
            EntityReference argTeam = TeamInput.Get(context._CodeActivityContext);
        }
    }
}
