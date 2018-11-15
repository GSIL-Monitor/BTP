using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class OrderExpressRouteJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("使用job重新订阅快递鸟物流信息（对订阅失败的）:OrderExpressRouteJob...begin");
            try
            {
                Jinher.AMP.BTP.ISV.Facade.OrderExpressRouteFacade oerFacade = new ISV.Facade.OrderExpressRouteFacade();
                oerFacade.SubscribeOrderExpressForJob();
            }
            catch (Exception e)
            {
                string msg = string.Format("使用job重新订阅快递鸟物流信息（对订阅失败的）异常，异常信息：{0}", e);
                ConsoleLog.WriteLog(msg, LogLevel.Error);
            }
            ConsoleLog.WriteLog("使用job重新订阅快递鸟物流信息（对订阅失败的）:OrderExpressRouteJob end");
        }
    }
}
