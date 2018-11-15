using System;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.Job
{
    public class JdSyncAllPriceJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("全量同步京东价格--------------Begin");
            try
            {
                JdJobHelper.AutoUpdateJdPrice();
                JdJobHelper.AutoUpdateJdStock();
                JdJobHelper.AutoUpdateJdSkuState();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("全量同步京东价格--------------End");
        }
    }
}
