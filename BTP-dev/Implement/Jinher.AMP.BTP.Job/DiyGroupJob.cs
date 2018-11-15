using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.ISV.Facade;

namespace Jinher.AMP.BTP.Job
{
    class DiyGroupJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时处理拼团开始");
            try
            {
                DiyGroupFacade diygFacade = new DiyGroupFacade();
                diygFacade.DealUnDiyGroupTimeout();
                diygFacade.DealUnDiyGroupRefund();

                //自动成团
                diygFacade.VoluntarilyConfirmDiyGroup();
                //自动退款
                diygFacade.VoluntarilyRefundDiyGroup();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("定时处理拼团异常：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("定时处理拼团异常：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时处理拼团结束");
        }
    }
}
