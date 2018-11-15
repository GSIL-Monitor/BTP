using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;

namespace Jinher.AMP.BTP.Job
{
    
    public class SettleSaleCommissionJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {

            ConsoleLog.WriteLog("众销佣金结算:SendShareRedJob...begin");
            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.ShareRedEnvelopeFacade facace = new ISV.Facade.ShareRedEnvelopeFacade();
                //众销佣金结算
                facace.SettleCommossion();

            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("众销佣金结算:SendShareRedJob end");
        }
    }
}
