using PinkoCalcEngineWorker.Handlers;
using Microsoft.Practices.Unity;

namespace PinkoCalcEngineWorker
{
    /// <summary>
    /// WorkerRole
    /// </summary>
    public class WorkerRole : AzureWebRoleBase
    {
        /// <summary>
        /// Worker Role OnStart()
        /// </summary>
        /// <returns></returns>
        public override bool OnStart()
        {
            var rtn = base.OnStart();

            // Register message type to process
            MessageHandlers.Add(PinkoContainer.Resolve<HandleCalculateExpression>().Register());

            return rtn;
        }
    }
}
