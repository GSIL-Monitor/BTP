using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class SendOrderInfoToYKBDMqJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("补发订单数据到盈科大数据:SendOrderInfoToYKBDMqJob...begin");
            try
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facace = new ISV.Facade.CommodityOrderFacade();
                facace.SendOrderInfoToYKBDMq();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("补发订单数据到盈科大数据:SendOrderInfoToYKBDMqJob end");
        }
    }
}
