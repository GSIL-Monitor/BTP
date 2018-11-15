using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class UpdateOrderItemYjbPriceJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            try
            {
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facace = new ISV.Facade.CommodityOrderFacade();
                ConsoleLog.WriteLog("易捷币抵现订单，按照商品进行拆分:UpdateOrderItemYjbPriceJob...begin");
                facace.UpdateOrderItemYjbPrice();
                ConsoleLog.WriteLog("易捷币抵现订单，按照商品进行拆分:UpdateOrderItemYjbPriceJob end");

                //ConsoleLog.WriteLog("处理单品退款，OrderItem表State状态不正确的问题:UpdateOrderItemState...begin");
                //facace.UpdateOrderItemState();
                //ConsoleLog.WriteLog("处理单品退款，OrderItem表State状态不正确的问题:UpdateOrderItemState end");
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
        }
    }
}
