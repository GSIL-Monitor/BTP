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
    public class OTMSJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("处理追踪物流过程:OTMSJob...begin");
            try
            {
                BTP.IBP.Facade.OrderExpressRouteFacade oerFacade = new BTP.IBP.Facade.OrderExpressRouteFacade();
                oerFacade.GetOrderExpressForJdJob();
                oerFacade.GetOrderExpressForJsJob();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("OTMSJobException：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("OTMSJobException：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("处理追踪物流过程:OTMSJob...end");
        }
    }
}
