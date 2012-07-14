using PinkoAzureService;
using PinkoServices;

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

            return rtn;
        }
    }
}
