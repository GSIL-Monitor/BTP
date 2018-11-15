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
    public class CommodityModifyPriceJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时发消商品改低价格的消息:CommodityModifyPriceJob...begin");
            try
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new ISV.Facade.CommodityFacade();
                #region 商品列表缓存

                ConsoleLog.WriteLog("定时发消商品改低价格的消息开始...");
                facade.AutoPushCommodityModifyPrice();
                ConsoleLog.WriteLog("定时发消商品改低价格的消息完成...");

                #endregion

            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时发消商品改低价格的消息:CommodityModifyPriceJob end");
        }
    }
}
