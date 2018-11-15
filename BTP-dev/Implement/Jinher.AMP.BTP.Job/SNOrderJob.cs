using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.IBP.Facade;

namespace Jinher.AMP.BTP.Job
{
    public class SNOrderJob : IJob
    {

        public void Execute(JobExecutionContext context)
        {
            try
            {
                ConsoleLog.WriteLog("苏宁订单定时任务[Start]........" + DateTime.Now);
                new SNOrderItemFacade().ChangeOrderStatusForJob();
                new SNExpressTraceFacade().ChangeLogistStatusForJob();
              
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("苏宁订单Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("苏宁订单Exception：" + e.StackTrace, LogLevel.Error);
            }
            finally
            {
                ConsoleLog.WriteLog("苏宁订单定时任务[End]........" + DateTime.Now);
            }            
        }
    }
}
