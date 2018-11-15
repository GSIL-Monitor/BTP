using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Job
{
    public class JdOrderJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {



            /**********************************同步京东售后状态**********************************/
            ConsoleLog.WriteLog("同步京东售后状态Job.......................begin");
            // 同步京东售后状态
            try
            {
                Jinher.AMP.BTP.TPS.Helper.JdJobHelper.SyncRefundStatus();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("同步京东售后状态Job.......................end");


            /**********************************处理预售上架**********************************/
            ConsoleLog.WriteLog("处理预售上架Job.......................begin");
            // 预售商品自动上架
            try
            {
                Jinher.AMP.BTP.TPS.Helper.PromotionHelper.Shelve();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("处理预售上架Job.......................end");
        }
    }
}
