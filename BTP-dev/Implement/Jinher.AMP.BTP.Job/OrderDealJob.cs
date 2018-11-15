using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Job.Engine;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.Job
{
    public class OrderDealJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            ConsoleLog.WriteLog("定时处理订单:OrderDealJob...begin");
            try
            {
                //匿名账号
                //AuthorizeHelper.InitAuthorizeInfo();
                Jinher.AMP.BTP.ISV.Facade.CommodityOrderFacade facace = new ISV.Facade.CommodityOrderFacade();
                //统计订单信息 获取一年内订单总额、订单总量、最后一笔订单的支付时间
                facace.RenewOrderStatistics();
                //自动确认支付
                facace.AutoDealOrder();
                //处理三天未付款订单
                facace.ThreeDayNoPayOrder();
                //处理48小时 商家未相应的退款订单
                facace.AutoDaiRefundOrder();
                //处理5天内商家未响应的退货订单
                facace.AutoYiRefundOrder();
                // 售中买家7天未发货超时处理
                facace.AutoRefundAndCommodityOrder();
                //售中买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
                facace.AutoDealOrderConfirm();
                //处理售中仅退款的申请订单 5天内未响应 交易状态变为 7 已退款
                facace.AutoOnlyRefundOrder();
                //统计分润异常订单
                facace.CalcOrderException();
                //批量增加售后完成送积分
                facace.AutoAddOrderScore();
                //重新校验已完成订单的钱款去向
                facace.CheckFinishOrder();
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLog("Exception：" + e.Message, LogLevel.Error);
                ConsoleLog.WriteLog("Exception：" + e.StackTrace, LogLevel.Error);
            }
            ConsoleLog.WriteLog("定时处理订单:OrderDealJob end");
        }
    }
}
