using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Job.Engine;

namespace Jinher.AMP.BTP.Job
{

    public class CrowdfundingCalcJob : IJob
    {

        public DateTime CalcDate { get; set; }

        public void Execute(JobExecutionContext context)
        {

            ConsoleLog.WriteLog("众筹每日计算:CrowdfundingCalcJob...begin");
            try
            {
                CalcDate = DateTime.Today.AddDays(-1);

                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.CrowdfundingFacade facace = new ISV.Facade.CrowdfundingFacade();
                //计算每日股东、每日众筹
                var calcFlag = facace.CalcUserCrowdfundingDaily(CalcDate);
                if (!calcFlag)
                {
                    ConsoleLog.WriteLog(string.Format("众筹每日统计、众筹股东每日统计，更新CrowdfundingDaily、UserCrowdfundingDaily表错误，统计日期为{0}", CalcDate));
                    return;
                }
                calcFlag = facace.CalcCfDividend();
                if (!calcFlag)
                {
                    ConsoleLog.WriteLog("众筹每日计算众筹分红，更新CfOrderDividendDetail、CfDividend表错误");
                    return;
                }
                calcFlag = facace.CalcCfStatistics();
                if (!calcFlag)
                {
                    ConsoleLog.WriteLog("众筹汇总统计，更新CrowdfundingStatistics表");
                    return;
                }


            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("众筹每日计算:CrowdfundingCalcJob end");
        }
    }
}
