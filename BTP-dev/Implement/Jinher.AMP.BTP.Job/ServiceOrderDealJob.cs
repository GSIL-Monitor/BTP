using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
   public class ServiceOrderDealJob:  IJob
    {
        public void Execute(JobExecutionContext context)
        {
             ConsoleLog.WriteLog("服务订单处理begin");
             try
             {
                 Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facace = new ISV.Facade.CommodityOrderFacade();
                 facace.ServiceOrderStateChangedNotify();
             }
             catch (Exception ex)
             {
                 ConsoleLog.WriteLog("Exception：" + ex, LogLevel.Error);
             }
             ConsoleLog.WriteLog("服务订单处理end");
        }
    }
}
