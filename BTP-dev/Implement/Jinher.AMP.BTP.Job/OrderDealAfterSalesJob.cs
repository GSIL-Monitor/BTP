using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Job
{
    public class OrderDealAfterSalesJob:IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时售后处理订单:OrderDealAfterSalesJob...begin");
            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderAfterSalesFacade facace = new ISV.Facade.CommodityOrderAfterSalesFacade();

                //满7天自动处理售后（排除退款退货申请和卖家拒绝之间的时间，排除退款退货申请和卖家同意并未超时未收到货之间的时间）
                facace.AutoDealOrderAfterSales(); 

                //处理的退款处理订单 5天内未响应 交易状态变为 7 已退款
                facace.AutoDaiRefundOrderAfterSales();

                //买家7天不发出退货，自动恢复交易成功天数计时，满7天自动处理售后
                facace.AutoRefundAndCommodityOrderAfterSales();

                //处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
                facace.AutoYiRefundOrderAfterSales();

                //买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
                facace.AutoDealOrderConfirmAfterSales();

            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时售后处理订单:OrderDealAfterSalesJob end");
        }
    }
}
