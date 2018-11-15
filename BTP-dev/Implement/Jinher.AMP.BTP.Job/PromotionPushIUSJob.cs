using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;

namespace Jinher.AMP.BTP.Job
{
    public class PromotionPushIUSJob : IJob
    {

        public void Execute(JAP.Job.Engine.JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时执行促销发广场:PromotionPushIUSJob...begin");

            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new ISV.Facade.PromotionFacade();
                facade.PromotionPushIUS();
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteLog(ex.Message, LogLevel.Error);
                ConsoleLog.WriteLog(ex.StackTrace, LogLevel.Error);
            }

            ConsoleLog.WriteLog("定时执行促销发广场:PromotionPushIUSJob...end");
        }
    }
    
}
