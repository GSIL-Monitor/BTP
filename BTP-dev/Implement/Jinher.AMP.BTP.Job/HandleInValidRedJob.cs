using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;

namespace Jinher.AMP.BTP.Job
{

    public class HandleInValidRedJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时处理过期红包:HandleInValidRedJob...begin");
            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade facace = new ISV.Facade.ShareRedEnvelopeFacade();
                //处理众销过期红包
                facace.HandleInValidRedEnvelope();
                //处理众筹过期红包
                facace.HandleCfInValidRedEnvelope();

            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时处理过期红包:HandleInValidRedJob end");
        }
    }
}
