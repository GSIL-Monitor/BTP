using System;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.TPS.Helper;
using System.Diagnostics;

namespace Jinher.AMP.BTP.Job
{
    public class YXSyncAllPriceJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("全量同步严选价格.库存--------------Begin");
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                YXCommodityHelper.AutoSyncAllStockNum();
                LogHelper.Info(string.Format("YXCommodityHelper.AutoSyncAllStockNum：耗时：{0}。", timer.ElapsedMilliseconds));
                timer.Restart();
                YXCommodityHelper.AutoUpdateYXComInfo();
                timer.Stop();
                LogHelper.Info(string.Format("YXCommodityHelper.AutoUpdateYXComInfo：耗时：{0}。", timer.ElapsedMilliseconds));
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("全量同步严选价格.库存--------------End");
        }
    }
}
