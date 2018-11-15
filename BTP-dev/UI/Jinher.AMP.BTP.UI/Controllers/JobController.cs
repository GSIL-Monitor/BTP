using System;
using System.Diagnostics;
using System.Web.Mvc;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class JobController : BaseController
    {
        /// <summary>
        /// 自动调价设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoSettingCommodityPrice()
        {
            Jinher.AMP.BTP.TPS.Helper.Jobs.AutoSettingCommodityPrice();
            return Content("ok");
        }

        /// <summary>
        /// 京东商品自动审核
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoAuditJdCommodity()
        {
            Jinher.AMP.BTP.TPS.Helper.JdJobHelper.AutoAudit();
            return Content("ok");
        }

        /// <summary>
        /// 同步京东订单物流信息
        /// </summary>
        [HttpGet]
        public ActionResult GetJdOrderExpress()
        {
            new IBP.Facade.OrderExpressRouteFacade().GetOrderExpressForJdJob();
            return Content("ok");
        }

        /// <summary>
        /// 同步急速数据订单物流信息
        /// </summary>
        [HttpGet]
        public ActionResult GetJsOrderExpress()
        {
            new IBP.Facade.OrderExpressRouteFacade().GetOrderExpressForJsJob();
            return Content("ok");
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        [HttpGet]
        public ActionResult QuerenShou()
        {
            var facade = new ISV.Facade.CommodityOrderFacade();
            facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            facade.AutoDealOrder();
            return Content("ok");
        }

        /// <summary>
        /// 第三方电商同步订单物流信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetThirdECOrderExpress()
        {
            ThirdECommerceHelper.GetOrderExpressForJob();
            return Content("ok");
        }

        /// <summary>
        /// 清空App信息的缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RemoveAppCache()
        {
            new ISV.Facade.CacheFacade().RemoveAppCache();
            return Content("ok");
        }

        /// <summary>
        /// 同步今日促销信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAppPromotions()
        {
            var result = new ISV.Facade.PromotionFacade().GetAppPromotions();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 清空商品信息的缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RemoveCommodityCache()
        {
            new ISV.Facade.CommodityFacade().RemoveCache();
            return Content("ok");
        }

        /// <summary>
        /// 众筹每日计算
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CrowdfundingCalc()
        {
            var facade = new ISV.Facade.CrowdfundingFacade();
            var CalcDate = DateTime.Today.AddDays(-1);
            facade.CalcUserCrowdfundingDaily(CalcDate);
            facade.CalcCfDividend();
            facade.CalcCfStatistics();
            return Content("ok");
        }

        /// <summary>
        /// 更新易捷员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmployeeUpdate()
        {
            new ISV.Facade.YJEmployeeFacade().UpdataYJEmployeeInfo();
            return Content("ok");
        }

        /// <summary>
        /// 处理回退积分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoRefundScore()
        {
            new ISV.Facade.ErrorCommodityOrderFacade().AutoRefundScore();
            return Content("ok");
        }

        /// <summary>
        /// 处理过期红包
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HandleInValidRedEnvelope()
        {
            var facace = new ISV.Facade.ShareRedEnvelopeFacade();
            //处理众销过期红包
            facace.HandleInValidRedEnvelope();
            //处理众筹过期红包
            facace.HandleCfInValidRedEnvelope();
            return Content("ok");
        }

        /// <summary>
        /// 处理热门商品
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddHotCommodity()
        {
            var facace = new ISV.Facade.PromotionFacade();
            facace.AddHotCommodity();
            return Content("ok");
        }

        /// <summary>
        /// 订单售后业务处理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrderDealAfterSales()
        {
            var facade = new ISV.Facade.CommodityOrderAfterSalesFacade();
            //满7天自动处理售后（排除退款退货申请和卖家拒绝之间的时间，排除退款退货申请和卖家同意并未超时未收到货之间的时间）
            facade.AutoDealOrderAfterSales();
            //处理的退款处理订单 5天内未响应 交易状态变为 7 已退款
            facade.AutoDaiRefundOrderAfterSales();
            //买家7天不发出退货，自动恢复交易成功天数计时，满7天自动处理售后
            facade.AutoRefundAndCommodityOrderAfterSales();
            //处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
            facade.AutoYiRefundOrderAfterSales();
            //买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
            facade.AutoDealOrderConfirmAfterSales();
            return Content("ok");
        }

        /// <summary>
        /// 订单业务处理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrderDeal()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            //统计订单信息 获取一年内订单总额、订单总量、最后一笔订单的支付时间
            facace.RenewOrderStatistics();
            //自动确认支付
            facace.AutoDealOrder();
            //处理三天未付款订单
            facace.ThreeDayNoPayOrder();
            //处理48小时商家未响应的退款订单
            facace.AutoDaiRefundOrder();
            //处理5天内商家未响应的退货订单
            facace.AutoYiRefundOrder();
            //售中买家7天未发货超时处理
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
            return Content("ok");
        }

        /// <summary>
        /// 统计订单信息 获取一年内订单总额、订单总量、最后一笔订单的支付时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RenewOrderStatistics()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.RenewOrderStatistics();
            return Content("ok");
        }

        /// <summary>
        /// 自动确认支付
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoDealOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoDealOrder();
            return Content("ok");
        }

        /// <summary>
        /// 处理三天未付款订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ThreeDayNoPayOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.ThreeDayNoPayOrder();
            return Content("ok");
        }

        /// <summary>
        /// 处理48小时商家未响应的退款订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoDaiRefundOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoDaiRefundOrder();
            return Content("ok");
        }

        /// <summary>
        /// 处理5天内商家未响应的退货订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoYiRefundOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoYiRefundOrder();
            return Content("ok");
        }

        /// <summary>
        /// 售中买家7天未发货超时处理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoRefundAndCommodityOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoRefundAndCommodityOrder();
            return Content("ok");
        }

        /// <summary>
        /// 售中买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoDealOrderConfirm()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoDealOrderConfirm();
            return Content("ok");
        }

        /// <summary>
        /// 处理售中仅退款的申请订单 5天内未响应 交易状态变为 7 已退款
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoOnlyRefundOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoOnlyRefundOrder();
            return Content("ok");
        }

        /// <summary>
        /// 统计分润异常订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CalcOrderException()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.CalcOrderException();
            return Content("ok");
        }

        /// <summary>
        /// 批量增加售后完成送积分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoAddOrderScore()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.AutoAddOrderScore();
            return Content("ok");
        }

        /// <summary>
        /// 重新校验已完成订单的钱款去向
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckFinishOrder()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            facace.CheckFinishOrder();
            return Content("ok");
        }

        /// <summary>
        /// 促销发广场
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PromotionPushIUS()
        {
            new ISV.Facade.PromotionFacade().PromotionPushIUS();
            return Content("ok");
        }

        /// <summary>
        /// 发送众筹红包
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendCfRedEnvelope()
        {
            new ISV.Facade.ShareRedEnvelopeFacade().SendCfRedEnvelope();
            return Content("ok");
        }

        /// <summary>
        /// 发送红包
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendRedEnvelope()
        {
            new ISV.Facade.ShareRedEnvelopeFacade().SendRedEnvelope();
            return Content("ok");
        }

        /// <summary>
        /// 众销佣金结算
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SettleCommossion()
        {
            new ISV.Facade.ShareRedEnvelopeFacade().SettleCommossion();
            return Content("ok");
        }

        /// <summary>
        /// 更新评价表用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateUserInfoForReview()
        {
            new ISV.Facade.PromotionFacade().UpdateUserInfo();
            return Content("ok");
        }

        /// <summary>
        /// 全量同步严选库存和价格
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SyncYXStockAndPrice()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            YXCommodityHelper.AutoSyncAllStockNum();
            LogHelper.Info(string.Format("YXCommodityHelper.AutoSyncAllStockNum：耗时：{0}。", timer.ElapsedMilliseconds));
            timer.Restart();
            YXCommodityHelper.AutoUpdateYXComInfo();
            timer.Stop();
            LogHelper.Info(string.Format("YXCommodityHelper.AutoUpdateYXComInfo：耗时：{0}。", timer.ElapsedMilliseconds));
            return Content("ok");
        }

        /// <summary>
        /// 商品改低价格消息发送
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PushModifyPriceMessage()
        {
            new ISV.Facade.CommodityFacade().AutoPushCommodityModifyPrice();
            return Content("ok");
        }

        /// <summary>
        /// 处理拼团订单业务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DealDiyGroup()
        {
            var diygFacade = new ISV.Facade.DiyGroupFacade();
            //超时未成团
            diygFacade.DealUnDiyGroupTimeout();
            //未成团退款
            diygFacade.DealUnDiyGroupRefund();
            //自动成团
            diygFacade.VoluntarilyConfirmDiyGroup();
            //自动退款
            diygFacade.VoluntarilyRefundDiyGroup();
            return Content("ok");
        }

        /// <summary>
        /// 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DownloadEInvoiceInfo()
        {
            var facace = new ISV.Facade.CommodityOrderFacade();
            //中石化电子发票 补发错误发票请求以及下载电子发票接口调用
            facace.DownloadEInvoiceInfo();
            return Content("ok");
        }

        /// <summary>
        /// 同步京东订单退款状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SyncJdRefundStatus()
        {
            JdJobHelper.SyncRefundStatus();
            return Content("ok");
        }

        /// <summary>
        /// 预售商品定时上架
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PresellCommodityOnSale()
        {
            PromotionHelper.Shelve();
            return Content("ok");
        }

        /// <summary>
        /// 通过消息池同步京东价格、上下架、库存以及金采支付京东补单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SyncJdInfo()
        {
            JdJobHelper.AutoUpdatePriceByMessage();
            JdJobHelper.AutoUpdateJdSkuStateByMessage();
            JdJobHelper.AutoUpdateJdStockByMessage();
            JdOrderHelper.SynchroJdForJC();
            return Content("ok");
        }

        /// <summary>
        /// 京东拒收退款信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult JdAutoRefund()
        {
            JdJobHelper.AutoRefund();
            JdJobHelper.AutoRefundAgree();
            return Content("ok");
        }

        /// <summary>
        /// 处理活动过期未支付订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoExpirePayOrder()
        {
            new ISV.Facade.CommodityOrderFacade().AutoExpirePayOrder();
            return Content("ok");
        }

        /// <summary>
        /// 重新订阅快递鸟物流信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SubscribeOrderExpress()
        {
            new ISV.Facade.OrderExpressRouteFacade().SubscribeOrderExpressForJob();
            return Content("ok");
        }

        /// <summary>
        /// 自提订单补生成二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RepairePickUpCode()
        {
            new ISV.Facade.CommodityOrderFacade().RepairePickUpCode();
            return Content("ok");
        }

        /// <summary>
        /// 推送促销信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PromotionPush()
        {
            new ISV.Facade.PromotionFacade().PromotionPush();
            return Content("ok");
        }

        /// <summary>
        /// 服务订单状态变化发出通知
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ServiceOrderStateChangedNotify()
        {
            new ISV.Facade.CommodityOrderFacade().ServiceOrderStateChangedNotify();
            return Content("ok");
        }

        /// <summary>
        /// 易捷币拆分到订单项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateOrderItemYjbPrice()
        {
            new ISV.Facade.CommodityOrderFacade().UpdateOrderItemYjbPrice();
            return Content("ok");
        }

        /// <summary>
        /// 导入自营商家未设置结算价时的结算订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ImportNotSettleOrder()
        {
            SettleAccountHelper.ImportNotSettleOrder();
            return Content("ok");
        }

        /// <summary>
        /// 补发订单数据到盈科大数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SendOrderInfoToYKBDMq()
        {
            new ISV.Facade.CommodityOrderFacade().SendOrderInfoToYKBDMq();
            return Content("ok");
        }

        /// <summary>
        /// 处理京东已确认收货而btp未交易完成的订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AdjustJdOrderState()
        {
            JdJobHelper.AutoModifyOrderState();
            return Content("ok");
        }

        /// <summary>
        /// 苏宁同步订单状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_ChangeOrderStatus()
        {
            new SNOrderItemFacade().ChangeOrderStatusForJob();
            return Content("ok");
        }

        /// <summary>
        /// 苏宁同步物流状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_ChangeLogistStatus()
        {
            new SNExpressTraceFacade().ChangeLogistStatusForJob();
            return Content("ok");
        }

        /// <summary>
        /// 苏宁同步商品价格
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_AutoUpdatePriceByMessage()
        {
            Jinher.AMP.BTP.TPS.Helper.SNJobHelper.AutoUpdatePriceByMessage();
            return Content("ok");
        }
        /// <summary>
        /// 苏宁同步商品上下架，信息变更
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_AutoUpdateSNByMessage()
        {
            Jinher.AMP.BTP.TPS.Helper.SNJobHelper.AutoUpdateSNByMessage();
            return Content("ok");
        }
        /// <summary>
        /// 全量更新苏宁商品税率
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_AutoUpdateSNTax()
        {
            Jinher.AMP.BTP.TPS.Helper.SNJobHelper.AutoUpdateSNTax();
            return Content("ok");
        }

        /// <summary>
        /// 苏宁同步商品上下架
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_AutoUpdateSNSkuStateByMessage()
        {
            Jinher.AMP.BTP.TPS.Helper.SNJobHelper.AutoUpdateSNSkuStateByMessage();
            return Content("ok");
        }

        /// <summary>
        /// 苏宁同步商品变更
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_AutoUpdateSNCommodityByMessage()
        {
            Jinher.AMP.BTP.TPS.Helper.SNJobHelper.AutoUpdateSNCommodityByMessage();
            return Content("ok");
        }

        /// <summary>
        /// 苏宁商品自动审核
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suning_AutoAuditSNCommodity()
        {
            Jinher.AMP.BTP.TPS.Helper.SNJobHelper.AutoAuditSNCommodity();
            return Content("ok");
        }

        /// <summary>
        /// 修复订单抵用易捷币数量和抵现劵异常订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RepairOrderItemYjbPrice()
        {
            return Content(Jobs.RepairOrderItemYjbPrice(1));
        }

        /// <summary>
        /// 方正电商订单提交检查补救及获取商品sn码
        /// </summary>
        [HttpGet]
        public ActionResult FangZhengCheck()
        {
            FangZhengSV.FangZheng_Order_ForJobs();
            return Content("ok");
        }

        /// <summary>
        /// 严选物流绑单回调补偿机制
        /// </summary>
        /// <returns></returns>
        public ActionResult YXAutoDeliverOrder()
        {
            YXJob.AutoDeliverOrder();
            return Content("ok");
        }
        /// <summary>
        /// 给盈科同步指定商城数据
        /// </summary>
        /// <returns></returns>
        public ActionResult MallAppsForJob()
        {
            Guid EsAppid = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
            new MallApplyFacade().GetMallAppsForJob(EsAppid);
            return Content("ok");
        }
    }
}
