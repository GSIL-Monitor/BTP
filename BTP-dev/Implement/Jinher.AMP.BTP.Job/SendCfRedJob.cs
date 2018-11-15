using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;

namespace Jinher.AMP.BTP.Job
{

    public class SendCfRedJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("发送众筹红包消息:SendCfRedJob...begin");
            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade facace = new ISV.Facade.ShareRedEnvelopeFacade();
                //自动确认支付
                facace.SendCfRedEnvelope();

            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("发送众筹红包消息:SendCfRedJob end");
        }
    }
}
