using System;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.Job
{
    public class JdSyncPriceJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("同步京东价格--------------Begin");
            try
            {
                JdJobHelper.AutoUpdatePriceByMessage();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("同步京东价格--------------End");


            ConsoleLog.WriteLog("同步京东上下架--------------Begin");
            try
            {
                JdJobHelper.AutoUpdateJdSkuStateByMessage();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("同步京东上下架--------------End");


            ConsoleLog.WriteLog("同步京东库存--------------Begin");
            try
            {
                JdJobHelper.AutoUpdateJdStockByMessage();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("同步京东库存--------------End");


            ConsoleLog.WriteLog("京东商品拒收自动退款--------------Begin");
            try
            {
                JdJobHelper.AutoRefund();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("京东商品拒收自动退款--------------End");

            try
            {
                JdOrderHelper.SynchroJdForJC();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
        }
    }
}
