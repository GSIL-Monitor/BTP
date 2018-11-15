using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class OrderRepaireJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时修复订单:OrderRepaireJob...begin");
            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facace = new ISV.Facade.CommodityOrderFacade();
                facace.RepairePickUpCode();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时修复订单:OrderRepaireJob end");
        }
    }
}
