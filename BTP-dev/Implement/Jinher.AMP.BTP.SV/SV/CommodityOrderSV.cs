
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/4/3 14:32:48
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Diagnostics;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityOrderSV : BaseSv, ICommodityOrder
    {

        /// <summary>
        /// 生成订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SaveCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="orderSDTO">订单实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SaveCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveCommodityOrderExt(orderSDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SaveCommodityOrder：耗时：{0}。入参：orderSDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(orderSDTO), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 生成订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SavePrizeCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="orderSDTO">订单实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SavePrizeCommodityOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderSDTO orderSDTO)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SavePrizeCommodityOrderExt(orderSDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SavePrizeCommodityOrder：耗时：{0}。入参：orderSDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(orderSDTO), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="state">订单状态:未付款=0，未发货=1，已发货=2，确认收货=3，删除=4</param>
        /// <param name="orderId">订单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <param name="payment">付款方式:金币=0，到付=1，支付宝=2</param>
        /// <returns></returns>
        [Obsolete("已过时，请调用UpdateCommodityOrderNew", false)]
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrder(int state, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateCommodityOrderExt(state, orderId, userId, appId, payment, goldpwd, remessage);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.UpdateCommodityOrder：耗时：{0}。入参：state:{1},orderId:{2},userId:{3},appId:{4},payment:{5},goldpwd:{6},remessage:{7},\r\n出参：{8}", timer.ElapsedMilliseconds, state, orderId, userId, appId, payment, goldpwd, remessage, JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 查询订单详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetOrderItems
        /// </para>
        /// </summary>
        /// <param name="commodityorderId">订单ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetOrderItems(System.Guid commodityorderId, System.Guid userId, System.Guid appId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderItemsExt(commodityorderId, userId, appId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderItems：耗时：{0}。入参：commodityorderId:{1},userId:{2},appId:{3},\r\n出参：{4}", timer.ElapsedMilliseconds, commodityorderId, userId, appId, JsonHelper.JsonSerializer(result)));
            return result;

        }


        /// <summary>
        /// 查询分享订单详情页面  
        /// Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetShareOrderItems
        /// </summary>
        /// <param name="commodityorderId">订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderShareDTO GetShareOrderItems(System.Guid commodityorderId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetShareOrderItemsExt(commodityorderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetShareOrderItemsExt：耗时：{0}。入参：commodityorderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, commodityorderId, JsonHelper.JsonSerializer(result)));
            return result;

        }


        /// <summary>
        /// 根据交易状态获取订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetCommodityOrderByState
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="state">订单状态</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByState(System.Guid userId, System.Guid appId, int state, int pageIndex, int pageSize)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityOrderByStateExt(userId, appId, state, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetCommodityOrderByState：耗时：{0}。入参：userId:{1},appId:{2},state:{3},pageIndex:{4},pageSize:{5},\r\n出参：{6}", timer.ElapsedMilliseconds, userId, appId, state, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 获取用户所有订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state">订单状态0：未付款|1:未发货|2:已发货|3:交易成功|-1：失败</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetCommodityOrderByUserID(
            System.Guid userId, int pageIndex, int pageSize, int? state)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityOrderByUserIDExt(userId, pageIndex, pageSize, state);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetCommodityOrderByUserID：耗时：{0}。入参：userId:{1},pageIndex:{2},pageSize:{3},state:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, userId, pageIndex, pageSize, state, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 金币确认收货
        /// </summary>
        /// <param name="commodityOrderId">订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmOrder(System.Guid commodityOrderId, string password)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.ConfirmOrderExt(commodityOrderId, password);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.ConfirmOrder：耗时：{0}。入参：commodityOrderId:{1},password:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, commodityOrderId, password, JsonHelper.JsonSerializer(result)));
            return result;

        }


        /// <summary>
        /// 确认按最新价支付
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NewResultDTO ConfirmPayPrice(System.Guid commodityOrderId, System.Guid userId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.ConfirmPayPriceExt(commodityOrderId, userId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.ConfirmPayPrice：耗时：{0}。入参：commodityOrderId:{1},userId:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, commodityOrderId, userId, JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 定时处理订单
        /// </summary>
        public void AutoDealOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoDealOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoDealOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderResultDTO SubmitOrder(Jinher.AMP.BTP.Deploy.CustomDTO.OrderQueueDTO orderSDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SubmitOrderExt(orderSDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SubmitOrder：耗时：{0}。入参：orderSDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(orderSDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        public List<LotteryOrderInfoDTO> GetLotteryOrders(Guid lotteryId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetLotteryOrdersExt(lotteryId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetLotteryOrders：耗时：{0}。入参：lotteryId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, lotteryId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public ResultDTO GetOrderStateByCode(string orderCode)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderStateByCodeExt(orderCode);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderStateByCode：耗时：{0}。入参：orderCode:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, orderCode, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="submitOrderRefundDTO"></param>
        /// <returns></returns>
        public ResultDTO SubmitOrderRefund(SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SubmitOrderRefundExt(submitOrderRefundDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SubmitOrderRefund：耗时：{0}。入参：submitOrderRefundDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(submitOrderRefundDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        ///    查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderRefund(Guid commodityorderId, Guid orderItemId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderRefundExt(commodityorderId, orderItemId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderRefund：耗时：{0}。入参：commodityorderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, commodityorderId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ResultDTO CancelOrderRefund(Guid commodityorderId, int state)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CancelOrderRefundExt(commodityorderId, state);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.CancelOrderRefund：耗时：{0}。入参：commodityorderId:{1},state:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, commodityorderId, state, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 撤销退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ResultDTO CancelOrderItemRefund(Guid commodityorderId, int state, Guid orderItemId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CancelOrderItemRefundExt(commodityorderId, state, orderItemId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.CancelOrderRefund：耗时：{0}。入参：commodityorderId:{1},state:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, commodityorderId, state, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 退款物流信息提交
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="RefundExpCo">退货物流公司</param>
        /// <param name="RefundExpOrderNo">退货单号</param>
        /// <returns></returns>
        public ResultDTO AddOrderRefundExp(Guid commodityorderId, string RefundExpCo, string RefundExpOrderNo, Guid orderItemId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AddOrderRefundExpExt(commodityorderId, RefundExpCo, RefundExpOrderNo, orderItemId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AddOrderRefundExp：耗时：{0}。入参：commodityorderId:{1},RefundExpCo:{2},RefundExpOrderNo:{3},\r\n出参：{4}", timer.ElapsedMilliseconds, commodityorderId, RefundExpCo, RefundExpOrderNo, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 延长收货时间 
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public ResultDTO DelayConfirmTime(Guid commodityorderId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DelayConfirmTimeExt(commodityorderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.DelayConfirmTime：耗时：{0}。入参：commodityorderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, commodityorderId, JsonHelper.JsonSerializer(result)));
            return result;
        }
        public ResultDTO PayUpdateCommodityOrder(Guid orderId, Guid userId, Guid appId, int payment, ulong gold, decimal money, decimal couponCount)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.PayUpdateCommodityOrderExt(orderId, userId, appId, payment, gold, money, couponCount);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.PayUpdateCommodityOrder：耗时：{0}。入参：orderId:{1},userId:{2},appId:{3},payment:{4},gold:{5},money:{6},couponCount:{7},\r\n出参：{8}", timer.ElapsedMilliseconds, orderId, userId, appId, payment, gold, money, couponCount, JsonHelper.JsonSerializer(result)));
            return result;

        }

        public ResultDTO PayUpdateCommodityOrderForJc(Guid orderId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.PayUpdateCommodityOrderForJcExt(orderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.PayUpdateCommodityOrder：耗时：{0}。入参：orderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, orderId, JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="IsDel"></param>
        /// <returns></returns>
        public ResultDTO DelOrder(Guid commodityorderId, int IsDel)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DelOrderExt(commodityorderId, IsDel);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.DelOrder：耗时：{0}。入参：commodityorderId:{1},IsDel:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, commodityorderId, IsDel, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 提交订单后3天未付款，则交易状态变为“交易失败”，实收款显示为 0（超时交易关闭state=6）
        /// </summary>
        public void ThreeDayNoPayOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.ThreeDayNoPayOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.ThreeDayNoPayOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 处理待发货的退款处理订单 48小时内未响应 交易状态变为 7 已退款
        /// </summary>
        public void AutoDaiRefundOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoDaiRefundOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoDaiRefundOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        public void AutoYiRefundOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoYiRefundOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoYiRefundOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.SetOrderResultDTO SaveSetCommodityOrder(OrderSDTO orderSDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveSetCommodityOrderExt(orderSDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SaveSetCommodityOrder：耗时：{0}。入参：orderSDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(orderSDTO), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// 商家批量删除订单
        /// </summary>
        /// <returns></returns>
        public ResultDTO DeleteOrders(List<Guid> list)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DeleteOrdersExt(list);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.DeleteOrders：耗时：{0}。入参：list:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(list), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 根据订单主表ID获取子订单信息
        /// </summary>
        /// <param name="MainOrderId"></param>
        /// <returns></returns>
        public List<MainOrdersDTO> GetMianOrderList(Guid MainOrderId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetMianOrderListExt(MainOrderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetMianOrderList：耗时：{0}。入参：MainOrderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, MainOrderId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 删除订单集合表
        /// </summary>
        /// <param name="SubOrderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteMainOrder(Guid SubOrderId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DeleteMainOrderExt(SubOrderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.DeleteMainOrder：耗时：{0}。入参：SubOrderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, SubOrderId, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 处理超时未支付订单
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        public ResultDTO ExpirePayOrder(Guid orderId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.ExpirePayOrderExt(orderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.ExpirePayOrder：耗时：{0}。入参：orderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, orderId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 批量处理超时未支付订单
        /// </summary>
        public void AutoExpirePayOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoExpirePayOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoExpirePayOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 订单实时传递给盈科，补发数据
        /// </summary>
        public void SendOrderInfoToYKBDMq()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.SendOrderInfoToYKBDMqExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SendOrderInfoToYKBDMq：耗时：{0}。", timer.ElapsedMilliseconds));

        }
        /// <summary>
        /// 保存商品订单 --- 厂家直销，一次购买多个App
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public SetOrderResultDTO SaveSetCommodityOrderNew(List<OrderSDTO> orderList)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveSetCommodityOrderNewExt(orderList);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.SaveSetCommodityOrderNew：耗时：{0}。入参：orderList:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(orderList), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 统计分润异常订单
        /// </summary>
        public void CalcOrderException()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.CalcOrderExceptionExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.CalcOrderException：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 获取订货商品清单信息
        /// </summary>
        /// <param name="managerId">管理员ID</param>
        /// <param name="pickUpCode">推广码</param>
        /// <returns>订单编号和商品明细</returns>
        public ResultDTO<CommodityOrderSDTO> GetOrderItemsByPickUpCode(Guid managerId, string pickUpCode)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderItemsByPickUpCodeExt(managerId, pickUpCode);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderItemsByPickUpCode：耗时：{0}。入参：managerId:{1},pickUpCode:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, managerId, pickUpCode, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 获取提货点管理员所管理的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <param name="state">订单状态</param>
        /// <returns>提货点管理员所管理的订单信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetOrderListByManagerId(Guid userId, int pageIndex, int pageSize)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderListByManagerIdExt(userId, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderListByManagerId：耗时：{0}。入参：userId:{1},pageIndex:{2},pageSize:{3},\r\n出参：{4}", timer.ElapsedMilliseconds, userId, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 补生成二维码方法
        /// </summary>
        public void RepairePickUpCode()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.RepairePickUpCodeExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.RepairePickUpCode：耗时：{0}。", timer.ElapsedMilliseconds));

        }
        /// <summary>
        /// 批量增加售后完成送积分
        /// </summary>
        /// <returns></returns>
        public bool AutoAddOrderScore()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AutoAddOrderScoreExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AutoAddOrderScore：耗时：{0}。出参：{0}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 售中买家7天未发货超时处理
        /// </summary>
        public void AutoRefundAndCommodityOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoRefundAndCommodityOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoRefundAndCommodityOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 售中买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirm()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoDealOrderConfirmExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoDealOrderConfirm：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        ///处理售中仅退款的申请订单 5天内未响应 交易状态变为 7 已退款
        /// </summary>
        public void AutoOnlyRefundOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoOnlyRefundOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.AutoOnlyRefundOrder：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 中石化电子发票 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        public void DownloadEInvoiceInfo()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.DownloadEInvoiceInfoExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.DownloadEInvoiceInfo：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 中石化电子发票 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        public void PrCreateInvoic()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.PrCreateInvoicExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.PrCreateInvoic：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 重新校验已完成订单的钱款去向
        /// </summary>
        public void CheckFinishOrder()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.CheckFinishOrderExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.CheckFinishOrder：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        /// <summary>
        /// 获取订单列表（包含保险订单）
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        public List<OrderListCDTO> GetCommodityOrderByUserIDNew(OrderQueryParamDTO oqpDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityOrderByUserIDNewExt(oqpDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetCommodityOrderByUserIDNew：耗时：{0}。入参：oqpDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(oqpDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 获取订单列表（不包含保险订单）
        /// </summary>
        /// <param name="oqpDTO"></param>
        /// <returns></returns>
        public List<OrderListCDTO> GetCustomCommodityOrderByUserIDNew(OrderQueryParamDTO oqpDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCustomCommodityOrderByUserIDNewExt(oqpDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetCustomCommodityOrderByUserIDNew：耗时：{0}。入参：oqpDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(oqpDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 查询用户订单数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        public UserOrderCountDTO GetOrderCount(Guid userId, Guid esAppId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderCountExt(userId, esAppId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderCount：耗时：{0}。入参：userId:{1},esAppId:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, userId, esAppId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 服务订单状态变化发出通知.
        /// </summary>
        public void ServiceOrderStateChangedNotify()
        {
            base.Do(false);
            this.ServiceOrderStateChangedNotifyExt();
        }
        public ResultDTO<string> OrderSign(SignUrlDTO signUrlDTO)
        {
            base.Do(false);
            return this.OrderSignExt(signUrlDTO);
        }


        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrderNew
        /// </para>
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityOrderNew(UpdateCommodityOrderParamDTO ucopDto)
        {
            base.Do(false);
            return this.UpdateCommodityOrderNewExt(ucopDto);
        }

        /// <summary>
        /// 生成在线支付的Url地址
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetPayUrl(Jinher.AMP.BTP.Deploy.CustomDTO.PayOrderToFspDTO payOrderToFspDto)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetPayUrlExt(payOrderToFspDto);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetPayUrl：耗时：{0}。入参：payOrderToFspDto:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(payOrderToFspDto), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取订单详情校验数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<OrderCheckDTO> GetOrderCheckInfo(OrderQueryParamDTO search)
        {
            base.Do(false);
            var result = this.GetOrderCheckInfoExt(search);
            return result;
        }

        public ResultDTO UpdateOrderServiceTime(OrderQueryParamDTO search)
        {
            base.Do(false);
            var result = this.UpdateOrderServiceTimeExt(search);
            return result;
        }


        //job更新京东订单
        public ResultDTO UpdateJobCommodityOrder(DateTime time, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage)
        {
            base.Do(false);
            return this.UpdateJobCommodityOrderExt(time, orderId, userId, appId, payment, goldpwd, remessage);

        }

        /// <summary>
        /// 判断订单是否为拆分订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool CheckIsMainOrder(Guid orderId)
        {
            base.Do();
            return this.CheckIsMainOrderExt(orderId);
        }


        /// <summary>
        /// 跟据id获取订单内容
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public List<CommodityOrderDTO> GetCommodityOrder(List<Guid> ids)
        {
            base.Do(false);
            return this.GetCommodityOrderExt(ids);
        }

        public void ProcessJdOrder()
        {
            base.Do(false);
            this.ProcessJdOrderExt();
        }

        /// <summary>
        /// 更新订单统计表 记录用户近一年的订单总数、金额总数以及最后的交易时间等字段
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RenewOrderStatistics()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.RenewOrderStatisticsExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.RenewOrderStatistics：耗时：{0}。r\n出参：{1}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(result)));
            return result;
            base.Do(false);
        }

        /// <summary>
        /// 获取符合条件的用户数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<OrderStatisticsSDTO>> GetOrderStatistics(Jinher.AMP.Coupon.Deploy.CustomDTO.SearchOrderStatisticsExtDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderStatisticsExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetOrderStatistics：耗时：{0}。入参：：{2}r\n出参：{1}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(result), JsonHelper.JsonSerializer(search)));
            return result;
        }

        /// <summary>
        /// 易捷币抵现订单，按照商品进行拆分
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemYjbPrice()
        {
            base.Do(false);
            return this.UpdateOrderItemYjbPriceExt();
        }


        /// <summary>
        /// 处理单品退款，OrderItem表State状态不正确的问题
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemState()
        {
            base.Do(false);
            return this.UpdateOrderItemStateExt();
        }


        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetCommodityOrderSdtoByOrderItemId(Guid orderItemId)
        {
            base.Do(false);
            return this.GetCommodityOrderSdtoByOrderItemIdExt(orderItemId);
        }

        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        public string GetOrderItemAttrId(Guid orderItemId)
        {
            base.Do(false);
            return this.GetOrderItemAttrIdExt(orderItemId);
        }

        /// <summary>
        /// 发货提醒，发消息给商家
        /// </summary>
        /// <param name="commodityOrderId">订单ID</param>
        /// <returns></returns>
        public bool ShipmentRemind(Guid commodityOrderId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.ShipmentRemindExt(commodityOrderId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.ShipmentRemind：耗时：{0}。入参：commodityOrderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, commodityOrderId, JsonHelper.JsonSerializer(result)));
            return result;
        }
        
        /// <summary>
        /// 返回用户的退换货列表
        /// </summary>
        /// <param name="oqpDTO"></param>
        /// <returns></returns>
        public List<RefundOrderListDTO> GetRefundList(OrderQueryParamDTO oqpDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetRefundListExt(oqpDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderSV.GetRefundList：耗时：{0}。入参：userId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(oqpDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 补发分享佣金接口
        /// </summary>
        /// <param name="orderId">订单Id</param>
        public bool PushShareMoney(Guid orderId)
        {
            base.Do(false);
            return this.PushShareMoneyExt(orderId);
        }
    }
}
