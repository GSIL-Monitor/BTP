using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Cache;

namespace Jinher.AMP.BTP.Job
{
    public class AppSetDealJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时处理正品会APP缓存:AppSetDealJob...begin");
            try
            {
                Jinher.AMP.BTP.ISV.Facade.AppSetFacade facade = new ISV.Facade.AppSetFacade();
                #region 商品列表缓存

                ConsoleLog.WriteLog("定时清理正品会APP缓存开始...");
                var result = facade.RemoveAppInZPHCache();
                if (result != null && result.ResultCode == 0)
                {
                    ConsoleLog.WriteLog("清理正品会APP缓存成功");
                }
                else
                {
                    ConsoleLog.WriteLog("清理正品会APP缓存失败");
                }
                ConsoleLog.WriteLog("定时清理正品会APP缓存完成...");

                #endregion

            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时处理正品会APP缓存:AppSetDealJob end");
        }
    }
}
