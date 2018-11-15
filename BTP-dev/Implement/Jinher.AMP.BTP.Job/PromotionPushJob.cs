using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class PromotionPushJob : IJob
    {

        public void Execute(JAP.Job.Engine.JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时执行促销推送:PromotionPushJob...begin");

            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.PromotionFacade facade = new ISV.Facade.PromotionFacade();
                facade.PromotionPush();
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteLog(ex.Message, LogLevel.Error);
                ConsoleLog.WriteLog(ex.StackTrace, LogLevel.Error);
            }

            ConsoleLog.WriteLog("定时执行促销推送:PromotionPushJob...end");
        }
    }
}
