
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/10/23 11:01:23
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.BE.BELogic;
using Jinher.AMP.BTP.Common;
using System.Data;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityOrderAfterSalesBP : BaseBP, ICommodityOrderAfterSales
    {

        /// <summary>
        /// 售后同意退款/退货申请(同意退款申请，同意退款/退货申请，确认收到退货)
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO == null || cancelTheOrderDTO.OrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            LogHelper.Debug(string.Format("进入退款方法"));
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            if (cancelTheOrderDTO.OrderItemId != Guid.Empty)
            {
                return CancelTheOrderItemAfterSales(cancelTheOrderDTO);
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == cancelTheOrderDTO.OrderId && (c.State == 5 && r.State == 0 || c.State == 10 && r.State == 10 || c.State == 10 && r.State == 11)
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == cancelTheOrderDTO.OrderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的售后申请退款的订单" };
                }

                //原订单状态
                int oldState = commodityOrderService.State;
                //原申请状态
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;
                //现订单状态
                int newState = cancelTheOrderDTO.State;

                commodityOrderService.State = cancelTheOrderDTO.State;

                //时间
                //DateTime now = DateTime.Now;              
                decimal yjbprice = 0;
                var yjbresult = YJBSV.GetOrderYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                if (yjbresult.Data != null)
                {
                    yjbprice = yjbresult.Data.InsteadCashAmount;
                }
                var cashmoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                //退款金额
                decimal orRefundMoney = 0;
                if (orderRefundAfterSales.OrderRefundMoneyAndCoupun == null)
                {//老的退款数据
                    orRefundMoney = orderRefundAfterSales.RefundMoney > cashmoney ? cashmoney : orderRefundAfterSales.RefundMoney;
                }
                else
                {
                    orRefundMoney = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);
                }
                
                var tradeType = PaySource.GetTradeType(commodityOrder.Payment);
                LogHelper.Debug(string.Format("原订单状态:{0},原申请状态:{1},现订单状态{2},退款金额{3},退款状态{4}", oldState, oldOrderRefundAfterSalesState, newState, orRefundMoney, tradeType));
                //同意退款申请，以及确认收到退货
                if (newState == 7)
                {
                    LogHelper.Debug(string.Format("原订单状态{0},退款状态:{1}", oldState, newState));
                    if (oldState != 5 && oldState != 10)
                    {
                        LogHelper.Error(string.Format("(警告)code:100001该订单号{0}状态{1}无法变为已退款", commodityOrderService.Id, oldState));
                        return new ResultDTO { ResultCode = 1, Message = "当前订单状态错误,请重试" };
                    }
                    ////抵用券金额
                    //decimal coupon_price = 0;
                    //var user_yjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                    //if (user_yjcoupon.Data != null)
                    //{
                    //    coupon_price = user_yjcoupon.Data.UseAmount.Value;
                    //}
                    //orRefundMoney = orderRefundAfterSales.RefundMoney - coupon_price;

                    if (orRefundMoney > 0 && (tradeType == TradeTypeEnum.SecTrans || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
                    {
                        Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                        if (tradeType == TradeTypeEnum.SecTrans && commodityOrder.GoldPrice > 0)
                        {
                            //解冻金币
                            var unFreezeResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayUnFreeze(buildConfirmPayUnFreezeDTO(contextSession, commodityOrder, applicationDTO));
                            if (unFreezeResult == null)
                            {
                                return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                            }
                            if (unFreezeResult.Code != 0)
                            {
                                return new ResultDTO { ResultCode = 1, Message = string.Format("退款失败,{0}", unFreezeResult.Message) };
                            }
                            LogHelper.Debug(string.Format("金币信息Code:{0},Message:{1}", unFreezeResult.Code, unFreezeResult.Message));
                        }

                        
                        var cancelPayResult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(OrderSV.BuildCancelPayDTOAfterSales(commodityOrder, orRefundMoney, contextSession, applicationDTO, commodityOrderService), tradeType);
                        LogHelper.Debug(string.Format("cancelPayResult的信息:{0}---{1}", cancelPayResult.Code, cancelPayResult.Message));
                        if (cancelPayResult == null)
                        {
                            return new ResultDTO { ResultCode = 1, Message = "售后退款失败" };
                        }
                        if (cancelPayResult.Code != 0)
                        {
                            return new ResultDTO { ResultCode = 1, Message = string.Format("售后退款失败:{0}", cancelPayResult.Message) };
                        }
                        if (tradeType == TradeTypeEnum.Direct)
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                commodityOrderService.State = 12;
                                orderRefundAfterSales.State = 12;
                            }
                            else
                            {
                                return new ResultDTO { ResultCode = 1, Message = string.Format("售后退款失败:{0}", cancelPayResult.Message) };
                            }
                        }
                        else
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                commodityOrderService.State = 7;
                                orderRefundAfterSales.State = 1;
                            }
                            else
                            {
                                commodityOrderService.State = 12;
                                orderRefundAfterSales.State = 12;
                            }
                        }

                    }
                    else
                    {
                        commodityOrderService.State = 7;
                        orderRefundAfterSales.State = 1;
                    }

                    // 回退积分
                    SignSV.CommodityOrderAfterSalesRefundScore(contextSession, commodityOrder, orderRefundAfterSales);

                    //给易捷卡退款
                    //if (orderRefundAfterSales)
                    //{
                    //    Jinher.AMP.BTP.TPS.Helper.YJBHelper.RetreatYjc(commodityOrder.UserId, orderRefundAfterSales ? 0 : Convert.ToDecimal(commodityOrder.YJCardPrice));
                    //}
                    // 回退易捷币
                    //Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales);

                    #region 回退易捷币和易捷抵用券
                    //抵用券金额
                    decimal couponprice = 0;
                    decimal couponmoney = 0;//抵用券使用总金额
                    //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
                    bool isexistsplit = false;//是否有拆单，如果有的话，整单退款的时候，退的是抵用券的使用金额
                    var issplit = MainOrder.ObjectSet().Where(x => x.SubOrderId == commodityOrder.Id).FirstOrDefault();
                    if (issplit != null)
                    {
                        isexistsplit = true;
                    }
                    var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                    if (useryjcoupon.Data != null && useryjcoupon.Data.Count > 0)
                    {
                        useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.Price).ToList();
                        decimal refundmoney = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);

                        foreach (var item in useryjcoupon.Data)
                        {
                            couponmoney += item.UseAmount ?? 0;
                        }
                        if (refundmoney == couponmoney + orRefundMoney)
                        {//全额退
                            for (int i = 0; i < useryjcoupon.Data.Count; i++)
                            {
                                if (useryjcoupon.Data[i] != null)
                                {
                                    if (isexistsplit)
                                        couponprice = useryjcoupon.Data[i].UsePrice;
                                    else
                                        couponprice = useryjcoupon.Data[i].Price;
                                    if (i == 0)
                                    {

                                    }
                                    else
                                    {//易捷币不能循环退
                                        orderRefundAfterSales.RefundYJBMoney = 0;
                                    }
                                    decimal coupon = couponprice;
                                    Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales, coupon, useryjcoupon.Data[i].Id);
                                }
                            }
                        }
                        else
                        {//部分退款
                            for (int i = 0; i < useryjcoupon.Data.Count; i++)
                            {
                                if (useryjcoupon.Data[i] != null)
                                {
                                    if (isexistsplit)
                                        couponprice = useryjcoupon.Data[i].UsePrice;
                                    else
                                        couponprice = useryjcoupon.Data[i].Price;
                                    //totalcouprice += useryjcoupon.Data[i].Price;
                                    if (i == 0)
                                    {

                                    }
                                    else
                                    {//易捷币不能循环退
                                        orderRefundAfterSales.RefundYJBMoney = 0;
                                    }
                                    decimal coupon = 0;
                                    if (refundmoney - orRefundMoney > 0)
                                    {
                                        if (refundmoney - orRefundMoney - couponprice >= 0)
                                        {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                         //if (refundmoney - cashmoney - couponprice - orderRefundAfterSales.RefundYJBMoney < 0)
                                         //{//返还部分易捷币
                                         //    orderRefundAfterSales.RefundYJBMoney = orderRefundAfterSales.RefundYJBMoney - (refundmoney + cashmoney + couponprice);
                                         //}
                                            coupon = couponprice;
                                        }
                                        else
                                        {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                            coupon = refundmoney - orRefundMoney;
                                            orderRefundAfterSales.RefundYJBMoney = 0;
                                        }
                                    }
                                    Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales, coupon, useryjcoupon.Data[i].Id);
                                    refundmoney -= coupon;
                                }
                            }
                        }
                    }
                    else
                    {
                        Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales, 0, Guid.Empty);
                    }
                    #endregion

                    // 更新结算项
                    Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.OrderRefund(contextSession, commodityOrder, orderRefundAfterSales);

                    if (oldState == 5)
                    {
                        orderRefundAfterSales.RefuseTime = DateTime.Now;
                    }
                }
                //同意退款/退货申请
                else if (newState == 10)
                {
                    LogHelper.Debug(string.Format("原订单状态{0},退款状态:{1}", oldState, newState));
                    if (oldState != 5 || orderRefundAfterSales.RefundType != 1)
                    {
                        LogHelper.Error(string.Format("(警告)code:100001该订单号{0}状态{1}无法变为已发货退款中商家同意退款,商家未收到货", commodityOrder.Id, oldState));
                        return new ResultDTO { ResultCode = 1, Message = "当前订单状态错误,请重试" };
                    }

                    commodityOrderService.State = 10;
                    orderRefundAfterSales.State = 10;
                    orderRefundAfterSales.RefuseTime = DateTime.Now;
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单状态无法进行售后同意退款/退货申请操作" };
                }

                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                //更新订单的修改时间                           
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                int result = contextSession.SaveChanges();

                //更新退货物流跟新信息
                UpdateRefundExpressTrace(cancelTheOrderDTO.OrderId, newState, false, commodityOrder.AppId);

                if (result > 0)
                {
                    LogHelper.Debug("result存在一条记录");
                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.IsPush = false;
                        LogHelper.Debug(string.Format("原订单状态{0},退款状态:{1}", oldState, newState));
                        if (newState == 7 && oldState == 5 && commodityOrder.Payment != 1)
                        {
                            journal.Name = "售后退款同意退款申请";
                        }
                        else if (newState == 7 && oldState == 10 && commodityOrder.Payment != 1)
                        {
                            journal.Name = "售后退款同意退款/退货申请";
                        }
                        else if (newState == 10 && oldState == 5)
                        {
                            journal.Name = "售后退款达成退款/退货协议";
                        }
                        else
                        {
                            journal.IsPush = true;
                        }
                        journal.Code = commodityOrderService.Code;
                        journal.SubTime = DateTime.Now;
                        journal.SubId = cancelTheOrderDTO.UserId;
                        journal.Details = "售后订单状态由" + oldState + "变为" + commodityOrderService.State;
                        journal.CommodityOrderId = cancelTheOrderDTO.OrderId;
                        journal.StateFrom = oldState;
                        journal.StateTo = commodityOrderService.State;

                        journal.OrderType = commodityOrder.OrderType;

                        //保存日志
                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("售后同意退款/退货申请记日志异常。", ex);
                    }

                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    LogHelper.Debug(string.Format("发送消息，异步执行"));
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrderService.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrderService.State;
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });

                    //易捷北京自营商品申请开电子发票 (红票)
                    Guid esAppId = new Guid(CustomConfig.InvoiceAppId);
                    //易捷北京的自营或者门店自营
                    MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == esAppId && t.AppId == commodityOrder.AppId && (t.Type == 0 || t.Type == 2 || t.Type == 3));
                    if (mallApply != null)
                    {
                        var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                        if (invoice != null && invoice.Category == 2 && commodityOrder.RealPrice == orderRefundAfterSales.RefundMoney)
                        {
                            TPS.Invoic.InvoicManage invoicManage = new TPS.Invoic.InvoicManage();
                            invoicManage.CreateInvoic(contextSession, commodityOrder, 1);
                            contextSession.SaveChanges();
                        }
                    }
                }
                else
                {
                    LogHelper.Error(string.Format("售后同意退款/退货申请保存失败。cancelTheOrderDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)));
                    return new ResultDTO { ResultCode = 1, Message = "售后同意退款/退货申请保存失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后同意退款/退货申请。cancelTheOrderDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary> 
        /// 售后同意退款/退货申请(同意退款申请，同意退款/退货申请，确认收到退货) 单商品退款/退货
        /// </summary>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderItemAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO == null || cancelTheOrderDTO.OrderId == Guid.Empty || cancelTheOrderDTO.OrderItemId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //现订单状态
                int newState = cancelTheOrderDTO.State;
                var orderItem = OrderItem.FindByID(cancelTheOrderDTO.OrderItemId);
                if (orderItem.State != 1 && orderItem.State != 3)
                {
                    if (orderItem.State == 0)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "客户已撤销退款申请" };
                    }
                    return new ResultDTO { ResultCode = 1, Message = "当前订单项状态错误,请重试" };
                }
                var commodityOrderService = CommodityOrderService.FindByID(cancelTheOrderDTO.OrderId);
                var commodityOrder = CommodityOrder.FindByID(cancelTheOrderDTO.OrderId);
                var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == cancelTheOrderDTO.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();

                //退款金额
                decimal orRefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney - orderItem.ScorePrice;
                if (orderItem.YjbPrice != null)
                {
                    orRefundMoney = orRefundMoney - (decimal)orderItem.YjbPrice;
                }
                orRefundMoney = orRefundMoney - orderItem.ScorePrice;
                //抵用券金额
                decimal coupon_price = 0;
                var user_yjcoupon = YJBSV.GetUserYJCouponItemByOrderId(orderItem.Id);
                if (user_yjcoupon.Data != null)
                {
                    foreach (var item in user_yjcoupon.Data)
                    {
                        coupon_price += item.UseAmount;
                    }

                }
                decimal yjbprice = 0;
                var yjbresult = YJBSV.GetOrderItemYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                if (yjbresult.Data != null)
                {
                    yjbprice = yjbresult.Data.Items != null ? yjbresult.Data.Items[0].InsteadCashAmount : 0;
                }
                var CurrPic1 = orderItem.RealPrice * orderItem.Number;
                if (CurrPic1 == 0)
                {
                    CurrPic1 = (orderItem.DiscountPrice * orderItem.Number);
                }
                var cashmoney = CurrPic1.Value - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - coupon_price - yjbprice;
                if (orderRefundAfterSales.OrderRefundMoneyAndCoupun == null)
                {//老的退款数据
                    orRefundMoney = orderRefundAfterSales.RefundMoney > cashmoney ? cashmoney : orderRefundAfterSales.RefundMoney;
                }
                else
                {
                    orRefundMoney = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);
                }

                var tradeType = PaySource.GetTradeType(commodityOrder.Payment);
                //同意退款申请，以及确认收到退货
                if (newState == 21)
                {
                    if (orderRefundAfterSales.RefundType == 1 && orderItem.State != 3)
                    {
                        return UpdateOrderItemStateTo3(cancelTheOrderDTO, commodityOrder);
                    }
                    if (orRefundMoney > 0 && (tradeType == TradeTypeEnum.SecTrans || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
                    {
                        if (cancelTheOrderDTO.SendBackFreightMoney >= orderRefundAfterSales.RefundMoney) return new ResultDTO() { ResultCode = 1, Message = "残品寄回运费金额须小于退款金额" };
                        orderRefundAfterSales.RefundMoney -= cancelTheOrderDTO.SendBackFreightMoney;
                        orRefundMoney -= cancelTheOrderDTO.SendBackFreightMoney;

                        Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                        if (tradeType == TradeTypeEnum.SecTrans && commodityOrder.GoldPrice > 0)
                        {
                            //解冻金币
                            var par = buildConfirmPayUnFreezeDTO(contextSession, commodityOrder, applicationDTO);
                            var unFreezeResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayUnFreeze(par);
                            if (unFreezeResult == null)
                            {
                                return new ResultDTO { ResultCode = 1, Message = "退款失败" };
                            }
                            if (unFreezeResult.Code != 0)
                            {
                                //LogHelper.Debug(string.Format("同意退款-解冻金币 失败，" + "Reqest:" + JsonHelper.JsonSerializer(par) + " Response: " + JsonHelper.JsonSerializer(unFreezeResult)));
                                return new ResultDTO { ResultCode = 1, Message = string.Format("退款失败,{0}", unFreezeResult.Message) };
                            }
                            LogHelper.Debug(string.Format("金币信息Code:{0},Message:{1}", unFreezeResult.Code, unFreezeResult.Message));
                        }

                        var cancelPayResult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(OrderSV.BuildCancelPayDTOAfterSales(commodityOrder, orRefundMoney, contextSession, applicationDTO, commodityOrderService), tradeType);
                        LogHelper.Debug(string.Format("cancelPayResult的信息:{0}---{1}", cancelPayResult.Code, cancelPayResult.Message));
                        if (cancelPayResult.Code != 0)
                        {
                            return new ResultDTO { ResultCode = 1, Message = string.Format("售后退款失败:{0}", cancelPayResult.Message) };
                        }
                        if (tradeType == TradeTypeEnum.Direct)
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                orderItem.State = 2; ;
                                orderRefundAfterSales.State = 1;
                            }
                            else
                            {
                                return new ResultDTO { ResultCode = 1, Message = string.Format("售后退款失败:{0}", cancelPayResult.Message) };
                            }
                        }
                        else
                        {
                            if (cancelPayResult.Message == "success")
                            {
                                orderItem.State = 2; ;
                                orderRefundAfterSales.State = 1;
                            }
                            else
                            {
                                orderRefundAfterSales.State = 12;
                            }
                        }

                    }
                    else
                    {
                        orderItem.State = 2;
                        orderRefundAfterSales.State = 1;
                    }
                    //退款总金额
                    var totRefund = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney + orderRefundAfterSales.RefundYJBMoney;
                    LogHelper.Info($"售后退款运费refundFreightPrice:totRefund{totRefund} = orderRefundAfterSales.RefundMoney{orderRefundAfterSales.RefundMoney} + orderRefundAfterSales.RefundScoreMoney{orderRefundAfterSales.RefundScoreMoney} + orderRefundAfterSales.RefundYJBMoney{orderRefundAfterSales.RefundYJBMoney}");

                    //之前退款积分为订单的 现在修改为订单商品项的
                    orderRefundAfterSales.RefundScoreMoney = orderItem.ScorePrice;
                    // 回退积分
                    SignSV.CommodityOrderAfterSalesRefundScore(contextSession, commodityOrder, orderRefundAfterSales);

                    //之前退款易捷币为订单的 现在修改为订单商品项的
                    if (orderItem.YjbPrice != null)
                    {
                        orderRefundAfterSales.RefundYJBMoney = (decimal)orderItem.YjbPrice;
                    }

                    // 回退易捷币
                    //Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales);

                    #region 回退易捷币和易捷抵用券

                    //orderRefundAfterSales.RefundYJBMoney = yjbprice;//单品易捷币退款金额为0，在这里处理一下
                    //if (orderRefundAfterSales.RefundMoney - cashmoney - coupon_price - orderRefundAfterSales.RefundYJBMoney <= 0)
                    //{//返还部分易捷币
                    //    orderRefundAfterSales.RefundYJBMoney = orderRefundAfterSales.RefundMoney - cashmoney - coupon_price;
                    //}
                    //抵用券金额(可以叠加使用)
                    decimal couponprice = 0;
                    //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
                    var useryjcoupon = YJBSV.GetUserYJCouponItemByOrderId(orderItem.Id);
                    if (useryjcoupon.Data != null)
                    {
                        useryjcoupon.Data = useryjcoupon.Data.OrderBy(x => x.UseAmount).ToList();
                        decimal refundmoney = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);
                        for (int i = 0; i < useryjcoupon.Data.Count; i++)
                        {
                            if (useryjcoupon.Data[i] != null)
                            {
                                couponprice = useryjcoupon.Data[i].UseAmount;
                                //totalcouprice += useryjcoupon.Data[i].UseAmount;
                                decimal coupon = 0;
                                if (i == 0)
                                {
                                    
                                }
                                else
                                {//易捷币不能循环退
                                    orderRefundAfterSales.RefundYJBMoney = 0;
                                }
                                if (refundmoney - orRefundMoney > 0)
                                {
                                    if (refundmoney - orRefundMoney - couponprice >= 0)
                                    {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券使用金额

                                        coupon = couponprice;
                                    }
                                    else
                                    {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                        coupon = refundmoney - orRefundMoney;
                                        orderRefundAfterSales.RefundYJBMoney = 0;
                                    }
                                }
                                Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderItemAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales, coupon, orderItem.CommodityId, orderItem.Id, useryjcoupon.Data[i].UserYJCouponId);
                                refundmoney -= coupon;
                            }
                        }
                    }
                    
                    #endregion


                    // 更新结算项
                    Jinher.AMP.BTP.TPS.Helper.SettleAccountHelper.OrderRefund(contextSession, commodityOrder, orderRefundAfterSales);

                    //记录商品退款详情到退款表
                    var CurrPic = (orderItem.RealPrice * orderItem.Number);
                    if (CurrPic == 0)
                    {
                        CurrPic = (orderItem.DiscountPrice * orderItem.Number);
                    }
                    //商品单项退款运费
                    LogHelper.Info($"售后退款运费refundFreightPrice:totRefund{totRefund} - (decimal)CurrPic{(decimal)CurrPic} + orderItem.Duty{orderItem.Duty}");
                    decimal refundFreightPrice = totRefund - (decimal)CurrPic + orderItem.Duty;
                    if (orderItem.CouponPrice != null && orderItem.CouponPrice > 0)
                    {
                        LogHelper.Info($"售后退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} += (decimal)orderItem.CouponPrice{(decimal)orderItem.CouponPrice}");
                        refundFreightPrice += (decimal)orderItem.CouponPrice;
                    }
                    if (orderItem.ChangeRealPrice != null && orderItem.ChangeRealPrice > 0)
                    {
                        LogHelper.Info($"售后退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice} += (decimal)orderItem.ChangeRealPrice{(decimal)orderItem.ChangeRealPrice}");
                        refundFreightPrice += (decimal)orderItem.ChangeRealPrice;
                    }

                    // 易捷抵用劵
                    //if (orderItem.YJCouponPrice.HasValue && orderItem.YJCouponPrice > 0)
                    //{
                    //    refundFreightPrice += orderItem.YJCouponPrice.Value;
                    //}
                    //修改退款运费金额负数问题 lhx-8-20
                    if (refundFreightPrice < 0)
                    {
                        LogHelper.Info($"售后退款运费refundFreightPrice:refundFreightPrice{refundFreightPrice}");
                        decimal tempPrice = orderItem.FreightPrice ?? 0;
                        if (totRefund > tempPrice)
                        {
                            LogHelper.Info($"售后退款运费refundFreightPrice: refundFreightPrice{refundFreightPrice} = tempPrice{tempPrice}");
                            refundFreightPrice = tempPrice;
                        }
                        else
                        {
                            LogHelper.Info($"售后退款运费refundFreightPrice: refundFreightPrice{refundFreightPrice} = totRefund{totRefund}");
                            refundFreightPrice = totRefund;
                        }
                    }
                    orderRefundAfterSales.RefundFreightPrice = refundFreightPrice;

                    //orderRefundAfterSales.RefundMoney = totRefund;
                    orderRefundAfterSales.RefundMoney = orRefundMoney;
                }
                //同意退款/退货申请
                else if (newState == 10)
                {
                    orderItem.State = 3;
                    orderRefundAfterSales.State = 10;
                    orderRefundAfterSales.RefuseTime = DateTime.Now;
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单状态无法进行售后同意退款/退货申请操作" };
                }
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orderRefundAfterSales);

                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orderItem);

                var count = OrderItem.ObjectSet().Count(t => t.CommodityOrderId == cancelTheOrderDTO.OrderId && t.Id != cancelTheOrderDTO.OrderItemId && t.State != 2);
                if (count == 0)
                {
                    commodityOrder.State = 7;
                    commodityOrder.ModifiedOn = DateTime.Now;
                    contextSession.SaveObject(commodityOrder);

                    commodityOrderService.State = 7;
                    commodityOrderService.ModifiedOn = DateTime.Now;
                    contextSession.SaveObject(commodityOrderService);
                }

                int result = contextSession.SaveChanges();

                //更新退货物流跟新信息
                UpdateRefundExpressTrace(cancelTheOrderDTO.OrderId, newState, true, commodityOrder.AppId, cancelTheOrderDTO.OrderItemId);

                if (result > 0)
                {
                }
                else
                {
                    LogHelper.Error(string.Format("售后同意退款/退货申请保存失败。cancelTheOrderDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)));
                    return new ResultDTO { ResultCode = 1, Message = "售后同意退款/退货申请保存失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后同意退款/退货申请。cancelTheOrderDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 更新退货物流跟踪信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="state"></param>
        /// <param name="isItem">是否单商品</param>
        /// <param name="appId">appId</param>
        private void UpdateRefundExpressTrace(Guid orderId, int state, bool isItem, Guid appId, Guid? orderItemId = null)
        {
            try
            {
                RefundExpressTraceBP bp = new RefundExpressTraceBP();
                Jinher.AMP.BTP.Deploy.RefundExpressTraceDTO retd = new Deploy.RefundExpressTraceDTO();
                retd.OrderId = orderId;
                retd.OrderItemId = orderItemId;
                //同意退款申请，以及确认收到退货
                if ((!isItem && state == 7) || (isItem && state == 21))
                {
                    bp.UpdateRefundExpressTraceExt(retd, appId);
                }
                //同意退款/退货申请
                else if ((!isItem && state == 10) || (isItem && state == 10))
                {
                    retd.AgreeRefundTime = DateTime.Now;
                    bp.AddRefundExpressTraceExt(retd);
                }
                LogHelper.Debug(string.Format("CommodityOrderAfterSalesBPExt.UpdateRefundExpressTrace,Message:更新退货物流跟踪信息操作完成;入参orderId:{0},state:{1},appid:{2}", orderId, state, appId));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderAfterSalesBPExt.UpdateRefundExpressTrace,更新退货物流跟踪信息操作异常,入参orderId:{0},state:{1},appid:{2}Message:{3}", orderId, state, appId, ex.ToString()));
            }
        }

        /// <summary>
        /// 更新订单状态（目标状态：已发货退款中商家同意退款，商家未收到货=10） 单商品退款/退货
        /// </summary>
        /// <param name="ucopDto">参数</param>
        /// <param name="commodityOrder">订单信息</param>
        /// <returns></returns>
        private ResultDTO UpdateOrderItemStateTo3(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO ucopDto, CommodityOrder commodityOrder)
        {
            ResultDTO result = new ResultDTO();
            DateTime now = DateTime.Now;

            var orderItem = OrderItem.FindByID(ucopDto.OrderItemId);
            var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == ucopDto.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
            if (orderRefundAfterSales == null)
            {
                LogHelper.Error(string.Format("CommodityOrderAfterSalesBP.UpdateOrderItemStateTo10未找到退款信息,ucopDto.orderItemId:{0}", ucopDto.OrderItemId));
                return new ResultDTO { ResultCode = 1, Message = "退款信息有误" };
            }
            if (ucopDto.PickUpFreightMoney > 0 && ucopDto.PickUpFreightMoney >= orderRefundAfterSales.RefundMoney) return new ResultDTO() { ResultCode = 1, Message = "上门取件服务费金额须小于退款金额" };
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            //对原状态为出库中退款中的特殊处理
            orderRefundAfterSales.RefuseTime = DateTime.Now;
            orderRefundAfterSales.State = 10;
            orderRefundAfterSales.ModifiedOn = now;
            orderRefundAfterSales.PickUpFreightMoney = ucopDto.PickUpFreightMoney;
            orderRefundAfterSales.RefundMoney = orderRefundAfterSales.RefundMoney - ucopDto.PickUpFreightMoney;
            orderRefundAfterSales.AuditUserId = this.ContextDTO.LoginUserID;
            if (orderRefundAfterSales.AuditUserId.HasValue && orderRefundAfterSales.AuditUserId.Value != Guid.Empty)
            {
                var user = TPS.CBCSV.Instance.GetUserBasicInfoNew(orderRefundAfterSales.AuditUserId.Value);
                if (user != null) orderRefundAfterSales.AuditUserName = user.Name;
            }
            orderRefundAfterSales.EntityState = EntityState.Modified;
            contextSession.SaveObject(orderRefundAfterSales);

            orderItem.State = 3;
            orderItem.ModifiedOn = DateTime.Now;
            orderItem.EntityState = EntityState.Modified;
            contextSession.SaveObject(orderItem);

            contextSession.SaveChanges();

            if (orderRefundAfterSales.PickwareType.HasValue && orderRefundAfterSales.JDEclpOrderRefundAfterSalesId.HasValue
                && orderRefundAfterSales.JDEclpOrderRefundAfterSalesId.Value != Guid.Empty) //进销存订单京东售后
                new IBP.Facade.JdEclpOrderFacade().CreateJDEclpRefundAfterSales(ucopDto.OrderId, ucopDto.OrderItemId, string.Empty);

            return result;
        }

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO.OrderItemId != Guid.Empty)
            {
                return RefuseRefundOrderItemAfterSales(cancelTheOrderDTO);
            }
            if (cancelTheOrderDTO == null || cancelTheOrderDTO.OrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == cancelTheOrderDTO.OrderId && c.State == 5 && r.State == 0
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                //更新订单的修改时间
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == cancelTheOrderDTO.OrderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的售后申请退款的订单" };
                }

                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                commodityOrderService.State = 3;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.State = 2;
                orderRefundAfterSales.RefuseTime = DateTime.Now;
                orderRefundAfterSales.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                int result = contextSession.SaveChanges();

                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrderService.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrderService.State;
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.RefuseReason = orderRefundAfterSales.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                }

                try
                {
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    if (orderRefundAfterSales.RefundType == 0)
                    {
                        journal.Name = "售后退款拒绝退款申请";
                    }
                    else if (orderRefundAfterSales.RefundType == 1)
                    {
                        journal.Name = "售后退款拒绝退款/退货申请";
                    }
                    journal.Code = commodityOrder.Code;
                    journal.SubTime = DateTime.Now;
                    journal.SubId = commodityOrder.UserId;
                    journal.Details = "售后订单状态由" + oldState + "变为" + commodityOrderService.State;
                    journal.CommodityOrderId = commodityOrder.Id;
                    journal.StateFrom = oldState;
                    journal.StateTo = commodityOrderService.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;

                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("售后拒绝退款/退货申请记日志异常。", ex);
                }

                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后拒绝退款/退货申请。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderItemAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrderService commodityOrderService = CommodityOrderService.FindByID(cancelTheOrderDTO.OrderId);
                OrderRefundAfterSales orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == cancelTheOrderDTO.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                //更新订单的修改时间
                var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == cancelTheOrderDTO.OrderId);
                OrderItem orderItem = OrderItem.FindByID(cancelTheOrderDTO.OrderItemId);

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null || orderItem == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的售后申请退款的订单" };
                }

                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                commodityOrderService.State = 3;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrderService);

                orderRefundAfterSales.State = 2;
                orderRefundAfterSales.RefuseTime = DateTime.Now;
                orderRefundAfterSales.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orderRefundAfterSales);

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);

                //拒绝退款申请
                orderItem.State = 4;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                int result = contextSession.SaveChanges();
                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrderService.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrderService.State;
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.RefuseReason = orderRefundAfterSales.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                }

                try
                {
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    if (orderRefundAfterSales.RefundType == 0)
                    {
                        journal.Name = "售后退款拒绝退款申请";
                    }
                    else if (orderRefundAfterSales.RefundType == 1)
                    {
                        journal.Name = "售后退款拒绝退款/退货申请";
                    }
                    journal.Code = commodityOrder.Code;
                    journal.SubTime = DateTime.Now;
                    journal.SubId = commodityOrder.UserId;
                    journal.Details = "售后订单状态由" + oldState + "变为" + commodityOrderService.State;
                    journal.CommodityOrderId = commodityOrder.Id;
                    journal.CommodityOrderItemId = cancelTheOrderDTO.OrderItemId;
                    journal.StateFrom = oldState;
                    journal.StateTo = commodityOrderService.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;

                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("售后拒绝退款/退货申请记日志异常。", ex);
                }

                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后拒绝退款/退货申请。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
        }

        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSalesExt(System.Guid commodityorderId)
        {
            try
            {
                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == commodityorderId
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                var orderRefundAfterSales = afterSalesList.OrderByDescending(t => t.orderRefundAfterSales.SubTime).Select(t => t.orderRefundAfterSales).FirstOrDefault();

                if (orderRefundAfterSales != null)
                {
                    SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                    {
                        Id = orderRefundAfterSales.Id,
                        commodityorderId = orderRefundAfterSales.OrderId,
                        RefundReason = orderRefundAfterSales.RefundReason,
                        RefundMoney = orderRefundAfterSales.RefundMoney,
                        RefundDesc = orderRefundAfterSales.RefundDesc,
                        OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                        State = orderRefundAfterSales.State,
                        RefundExpCo = orderRefundAfterSales.RefundExpCo,
                        RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                        RefundType = orderRefundAfterSales.RefundType,
                        RefuseReason = orderRefundAfterSales.RefuseReason,
                        RefuseTime = orderRefundAfterSales.RefuseTime,
                        RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                        IsDelayConfirmTimeAfterSales = commodityOrderService.IsDelayConfirmTimeAfterSales,
                        SubTime = orderRefundAfterSales.SubTime,
                        NotReceiveTime = orderRefundAfterSales.NotReceiveTime,
                        RefundScoreMoney = orderRefundAfterSales.RefundScoreMoney,
                        SalerRemark = orderRefundAfterSales.SalerRemark,
                        RefundYJBMoney = orderRefundAfterSales.RefundYJBMoney,
                        RefundCouponPirce = orderRefundAfterSales.RefundCouponPirce
                    };

                    CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == result.commodityorderId);
                    OrderItem orderItem = OrderItem.ObjectSet().FirstOrDefault(p => p.Code == commodityOrder.Code);
                    if (orderItem != null)
                    {
                        result.Name = orderItem.Name;
                        result.Num = orderItem.Number;
                        result.CommodityAttributes = orderItem.CommodityAttributes;
                        result.Pic = orderItem.PicturesPath;
                        result.Price = orderItem.CurrentPrice;
                        result.Specifications = orderItem.Specifications ?? 0;
                    }
                    result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(commodityOrder.AppId);
                    return result;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查看售后退款申请服务异常。commodityorderId：{0}", commodityorderId), ex);
                return new SubmitOrderRefundDTO();
            }
        }

        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderItemRefundAfterSalesExt(System.Guid commodityorderId, Guid orderItemId)
        {
            try
            {
                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == commodityorderId
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                var orderRefundAfterSales = afterSalesList.OrderByDescending(t => t.orderRefundAfterSales.SubTime).Select(t => t.orderRefundAfterSales).FirstOrDefault();
                if (orderItemId != Guid.Empty)
                {
                    orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).Where(t => t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                }

                if (orderRefundAfterSales != null)
                {
                    SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                    {
                        Id = orderRefundAfterSales.Id,
                        commodityorderId = orderRefundAfterSales.OrderId,
                        RefundReason = orderRefundAfterSales.RefundReason,
                        RefundMoney = orderRefundAfterSales.RefundMoney,
                        RefundDesc = orderRefundAfterSales.RefundDesc,
                        OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                        State = orderRefundAfterSales.State,
                        RefundExpCo = orderRefundAfterSales.RefundExpCo,
                        RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                        RefundType = orderRefundAfterSales.RefundType,
                        RefuseReason = orderRefundAfterSales.RefuseReason,
                        RefuseTime = orderRefundAfterSales.RefuseTime,
                        RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                        IsDelayConfirmTimeAfterSales = commodityOrderService.IsDelayConfirmTimeAfterSales,
                        SubTime = orderRefundAfterSales.SubTime,
                        NotReceiveTime = orderRefundAfterSales.NotReceiveTime,
                        RefundScoreMoney = orderRefundAfterSales.RefundScoreMoney,
                        SalerRemark = orderRefundAfterSales.SalerRemark,
                        RefundYJBMoney = orderRefundAfterSales.RefundYJBMoney,
                        PickwareType = orderRefundAfterSales.PickwareType,
                        Address = new AddressInfo
                        {
                            customerContactName = orderRefundAfterSales.CustomerContactName,
                            customerTel = orderRefundAfterSales.CustomerTel,
                            pickwareAddress = orderRefundAfterSales.PickwareAddress
                        }
                    };
                    var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.OrderRefundAfterSalesId == result.Id).FirstOrDefault();
                    if (jdOrderRefundAfterSales != null)
                    {
                        result.JdOrderRefundInfo = new JdOrderRefundDto
                        {
                            ServiceId = jdOrderRefundAfterSales.AfsServiceId,
                            Cancel = jdOrderRefundAfterSales.Cancel,
                            CustomerContactName = jdOrderRefundAfterSales.CustomerContactName,
                            CustomerTel = jdOrderRefundAfterSales.CustomerTel,
                            PickwareAddress = jdOrderRefundAfterSales.PickwareAddress,
                            PickwareType = jdOrderRefundAfterSales.PickwareType
                        };
                    }
                    OrderItem orderItem = OrderItem.FindByID(orderItemId);
                    if (orderItem != null)
                    {
                        result.Name = orderItem.Name;
                        result.Num = orderItem.Number;
                        result.CommodityAttributes = orderItem.CommodityAttributes;
                        result.Pic = orderItem.PicturesPath;
                        result.Price = orderItem.CurrentPrice;
                        result.Specifications = orderItem.Specifications ?? 0;
                    }
                    var order = CommodityOrder.FindByID(commodityorderId);
                    result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                    return result;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查看售后退款申请服务异常。commodityorderId：{0}", commodityorderId), ex);
                return new SubmitOrderRefundDTO();
            }
        }

        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> GetRefundInfoAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            var result = new List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO>();
            if (refundInfoDTO == null)
            {
                return result;
            }
            var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == refundInfoDTO.CommodityOrderId);
            if (refundInfoDTO.CommodityOrderItemId != Guid.Empty)
            {
                orderRefundAfterSales = orderRefundAfterSales.Where(t => t.OrderItemId == refundInfoDTO.CommodityOrderItemId);
            }
            orderRefundAfterSales = orderRefundAfterSales.OrderByDescending(t => t.SubTime).Skip((refundInfoDTO.PageIndex - 1) * refundInfoDTO.PageSize).Take(refundInfoDTO.PageSize);
            if (orderRefundAfterSales != null && orderRefundAfterSales.Count() > 0)
            {
                foreach (var item in orderRefundAfterSales)
                {
                    var model = new Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO();

                    model.Id = item.Id;
                    model.RefundType = item.RefundType;
                    model.RefundReason = item.RefundReason;
                    model.RefundMoney = item.RefundMoney;
                    model.RefundDesc = item.RefundDesc;
                    model.OrderId = item.OrderId;
                    model.State = item.State;
                    model.ReceiverAccount = item.ReceiverAccount;
                    model.Receiver = item.Receiver;
                    model.RefundExpCo = item.RefundExpCo;
                    model.RefundExpOrderNo = item.RefundExpOrderNo;
                    model.OrderRefundImgs = item.OrderRefundImgs;
                    model.SubTime = item.SubTime;
                    model.ModifiedOn = item.ModifiedOn;
                    model.DataType = item.DataType;
                    model.OrderItemId = item.OrderItemId;
                    model.RefuseTime = item.RefuseTime;
                    model.IsFullRefund = item.IsFullRefund;
                    model.RefuseReason = item.RefuseReason;
                    model.NotReceiveTime = item.NotReceiveTime;
                    model.RefundExpOrderTime = item.RefundExpOrderTime;
                    model.RefundScoreMoney = item.RefundScoreMoney;

                    var jdOrderRefundAfterSales = JdOrderRefundAfterSales.ObjectSet().Where(_ => _.OrderRefundAfterSalesId == model.Id).FirstOrDefault();
                    if (jdOrderRefundAfterSales != null)
                    {
                        model.JdOrderRefundInfo = new JdOrderRefundDto
                        {
                            ServiceId = jdOrderRefundAfterSales.AfsServiceId,
                            Cancel = jdOrderRefundAfterSales.Cancel,
                            CustomerContactName = jdOrderRefundAfterSales.CustomerContactName,
                            CustomerTel = jdOrderRefundAfterSales.CustomerTel,
                            PickwareAddress = jdOrderRefundAfterSales.PickwareAddress,
                            PickwareType = jdOrderRefundAfterSales.PickwareType
                        };
                    }

                    result.Add(model);
                }
            }

            return result;
        }

        /// <summary>
        /// 售后延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTimeAfterSalesExt(Guid commodityorderId)
        {
            if (commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == commodityorderId && (c.State == 10 && r.State == 11)
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).FirstOrDefault();
                if (afterSalesList == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "没有相关订单" };
                }

                CommodityOrderService commodityOrderService = afterSalesList.commodityOrderService;
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.orderRefundAfterSales;
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == commodityorderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                commodityOrderService.IsDelayConfirmTimeAfterSales = true;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                //更新订单的修改时间                           
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrderService.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrderService.State;
                            //messageModel.RefundType = orderRefundAfterSales.RefundType;
                            //messageModel.RefundMoney = orderRefundAfterSales.RefundMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            //messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;                            
                            //messageModel.RefuseReason = orderRefundAfterSales.RefuseReason;
                            messageModel.IsSellerDelayTime = true;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "延长收货时间失败" };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("延长收货时间服务异常。commodityorderId：{0}", commodityorderId.ToString()), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(commodityorderId);
            }
        }

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSellerAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO == null || cancelTheOrderDTO.OrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (cancelTheOrderDTO.OrderItemId != Guid.Empty)
            {
                return RefuseRefundOrderItemSellerAfterSales(cancelTheOrderDTO);
            }
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == cancelTheOrderDTO.OrderId && c.State == 10 && r.State == 11
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                //更新订单的修改时间
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == cancelTheOrderDTO.OrderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的售后订单" };
                }

                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                commodityOrderService.State = 3;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.State = 4;
                orderRefundAfterSales.RefuseTime = DateTime.Now;
                orderRefundAfterSales.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                int result = contextSession.SaveChanges();

                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrderService.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrderService.State;
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.RefuseReason = orderRefundAfterSales.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                }
                try
                {
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    journal.Name = "售后退款拒绝收货";
                    journal.Code = commodityOrder.Code;
                    journal.SubTime = DateTime.Now;
                    journal.SubId = commodityOrder.UserId;
                    journal.Details = "售后订单状态由" + oldState + "变为" + commodityOrderService.State;
                    journal.CommodityOrderId = commodityOrder.Id;
                    journal.StateFrom = oldState;
                    journal.StateTo = commodityOrderService.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;

                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("售后拒绝收货记日志异常。", ex);
                }

                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后拒绝收货。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }

        }

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderItemSellerAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityOrderService commodityOrderService = CommodityOrderService.FindByID(cancelTheOrderDTO.OrderId);
                OrderRefundAfterSales orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == cancelTheOrderDTO.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                //更新订单的修改时间
                var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == cancelTheOrderDTO.OrderId);
                OrderItem orderItem = OrderItem.FindByID(cancelTheOrderDTO.OrderItemId);

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null || orderItem == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的售后订单" };
                }

                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                commodityOrderService.State = 3;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrderService);

                orderRefundAfterSales.State = 4;
                orderRefundAfterSales.RefuseTime = DateTime.Now;
                orderRefundAfterSales.RefuseReason = cancelTheOrderDTO.RefuseReason;
                orderRefundAfterSales.RefundExpCo = "";
                orderRefundAfterSales.RefundExpOrderNo = "";
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(orderRefundAfterSales);

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityOrder);

                orderItem.State = 5;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                int result = contextSession.SaveChanges();
                if (result > 0)
                {
                    BTPMessageSV addmassage = new BTPMessageSV();
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            AfterSalesMessages messageModel = new AfterSalesMessages();
                            messageModel.IsAuto = false;
                            messageModel.Id = commodityOrderService.Id.ToString();
                            messageModel.UserIds = commodityOrder.UserId.ToString();
                            messageModel.AppId = commodityOrder.AppId;
                            messageModel.Code = commodityOrder.Code;
                            messageModel.State = commodityOrderService.State;
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.RefuseReason = orderRefundAfterSales.RefuseReason;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                }
                try
                {
                    Journal journal = new Journal();
                    journal.Id = Guid.NewGuid();
                    journal.Name = "售后退款拒绝收货";
                    journal.Code = commodityOrder.Code;
                    journal.SubTime = DateTime.Now;
                    journal.SubId = commodityOrder.UserId;
                    journal.Details = "售后订单状态由" + oldState + "变为" + commodityOrderService.State;
                    journal.CommodityOrderId = commodityOrder.Id;
                    journal.CommodityOrderItemId = cancelTheOrderDTO.OrderItemId;
                    journal.StateFrom = oldState;
                    journal.StateTo = commodityOrderService.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;

                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("售后拒绝收货记日志异常。", ex);
                }

                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后拒绝收货。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }

        }
        #region 金币参数组建

        /// <summary>
        /// 售后结束付款
        /// </summary>
        private ConfirmPayDTO buildConfirmPayUnFreezeDTO(ContextSession contextSession, CommodityOrder commodityOrder, Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO, string password = null)
        {


            ConfirmPayDTO comConfirmPayDto = new ConfirmPayDTO
            {
                BizId = commodityOrder.Id,
                Password = password,
                Sign = CustomConfig.PaySing,
                UserId = commodityOrder.UserId,
                AppId = commodityOrder.AppId,
                PayeeId = applicationDTO.OwnerId
            };

            return comConfirmPayDto;
        }
        #endregion

        /// <summary>
        /// 售后退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO">退款model，orderId为必填参数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSalesCallBackExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            if (cancelTheOrderDTO == null || cancelTheOrderDTO.OrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(cancelTheOrderDTO.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == cancelTheOrderDTO.OrderId && (c.State == 12 && r.State == 12 || c.State == 7 && r.State == 1)
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();


                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                CommodityOrder commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == cancelTheOrderDTO.OrderId);

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的售后申请退款的订单" };
                }
                if (commodityOrderService.State == 7 && orderRefundAfterSales.State == 1)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }

                int oldSate = commodityOrderService.State;
                commodityOrderService.State = 7;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.State = 1;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                //更新订单的修改时间
                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;
                int result = contextSession.SaveChanges();
                try
                {
                    //订单日志
                    Journal journal = Journal.CreateJournal();
                    journal.Id = Guid.NewGuid();
                    journal.Name = "售后退款金币回调";
                    journal.Code = commodityOrderService.Code;
                    journal.SubTime = DateTime.Now;
                    journal.SubId = Guid.Empty;
                    journal.Details = "售后订单状态由" + oldSate + "变为" + commodityOrderService.State;
                    journal.CommodityOrderId = cancelTheOrderDTO.OrderId;
                    journal.StateFrom = oldSate;
                    journal.StateTo = commodityOrderService.State;
                    journal.IsPush = false;
                    journal.OrderType = commodityOrder.OrderType;

                    //保存日志
                    journal.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(journal);
                    contextSession.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("售后退款金币回调CommodityOrderAfterSalesBP.CancelTheOrderAfterSalesCallBackExt,记日志异常。cancelTheOrderDTO:{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后退款金币回调CommodityOrderAfterSalesBP.CancelTheOrderAfterSalesCallBackExt。cancelTheOrderDTO：{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelTheOrderDTO.OrderId);
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefundAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            if (orderRefundDto == null || orderRefundDto.OrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(orderRefundDto.OrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                DateTime now = DateTime.Now;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commityList = (from c in CommodityOrder.ObjectSet()
                                   join cAfter in CommodityOrderService.ObjectSet()
                                   on c.Id equals cAfter.Id
                                   join r in OrderRefundAfterSales.ObjectSet()
                                   on c.Id equals r.OrderId
                                   where c.Id == orderRefundDto.OrderId && c.State == 3 && cAfter.State == 5 && r.State == 0
                                   select new
                                   {
                                       CommodityOrder = c,
                                       CommodityOrderAfterSales = cAfter,
                                       OrderRefundAfterSales = r
                                   }).FirstOrDefault();
                if (commityList == null || commityList.CommodityOrder == null || commityList.CommodityOrderAfterSales == null || commityList.OrderRefundAfterSales == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "找不到相应的订单" };
                }

                var commodityOrder = commityList.CommodityOrder;
                var commodityOrderAfterSales = commityList.CommodityOrderAfterSales;
                var orderRefundAfterSales = commityList.OrderRefundAfterSales;

                List<int> directArrivalPayments = new PaySourceBP().GetDirectArrivalPaymentExt();
                if (!directArrivalPayments.Contains(commodityOrder.Payment))
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单支付方式不是直接到账" };
                }

                var orderitemlist = OrderItem.ObjectSet().Where(n => n.CommodityOrderId == commodityOrder.Id).ToList();
                UserLimited.ObjectSet().Context.ExecuteStoreCommand("delete from UserLimited where CommodityOrderId='" + commodityOrder.Id + "'");

                List<HotCommodity> hotCommodities = new List<HotCommodity>();
                if (orderitemlist.Any())
                {
                    var ids = orderitemlist.Select(c => c.CommodityId).ToList();
                    hotCommodities =
                        HotCommodity.ObjectSet().Where(c => ids.Contains(c.CommodityId)).Distinct().ToList();

                }

                foreach (OrderItem items in orderitemlist)
                {
                    Guid comId = items.CommodityId;
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == comId).First();
                    com.EntityState = System.Data.EntityState.Modified;
                    com.Stock += items.Number;
                    contextSession.SaveObject(com);
                    needRefreshCacheCommoditys.Add(com);
                    HotCommodity hotCommodity = hotCommodities.FirstOrDefault(c => c.CommodityId == comId);
                    if (hotCommodity != null)
                    {
                        hotCommodity.EntityState = EntityState.Modified;
                        hotCommodity.Stock = com.Stock;
                        contextSession.SaveObject(hotCommodity);
                    }

                    if (items.Intensity != 10 || items.DiscountPrice != -1)
                    {
                        TodayPromotion to = TodayPromotion.GetCurrentPromotion(comId);
                        if (to != null)
                        {
                            to.SurplusLimitBuyTotal = to.SurplusLimitBuyTotal - items.Number;
                            to.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(to);
                            needRefreshCacheTodayPromotions.Add(to);
                            PromotionItems pti = PromotionItems.ObjectSet().Where(n => n.PromotionId == to.PromotionId && n.CommodityId == comId).FirstOrDefault();
                            pti.SurplusLimitBuyTotal = pti.SurplusLimitBuyTotal - items.Number;
                            pti.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(pti);
                        }
                    }

                }
                commodityOrderAfterSales.State = 7;
                commodityOrderAfterSales.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.State = 1;
                orderRefundAfterSales.ReceiverAccount = orderRefundDto.ReceiverAccount;
                orderRefundAfterSales.Receiver = orderRefundDto.Receiver;
                orderRefundAfterSales.RefundMoney = orderRefundDto.RefundMoney;
                orderRefundAfterSales.SalerRemark = orderRefundDto.SalerRemark;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {

                    if (needRefreshCacheCommoditys.Any())
                    {
                        needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }
                    if (needRefreshCacheTodayPromotions.Any())
                    {
                        needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Modified));
                    }

                    AddMessage addmassage = new AddMessage();
                    string type = "order";
                    //发送消息，异步执行
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        a =>
                        {
                            //向客户端推送交易失败消息
                            string messages = string.Format("您的订单{0}已完成退款，退款金额{1}元，请确认！", commodityOrder.Code, orderRefundDto.RefundMoney);
                            Guid EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessages(orderRefundDto.OrderId.ToString(), commodityOrder.UserId.ToString(), EsAppId,
                                commodityOrder.Code, commodityOrder.State, messages, type);
                            ////正品会发送消息
                            //if (new ZPHSV().CheckIsAppInZPH(commodityOrder.AppId))
                            //{
                            //    addmassage.AddMessages(commodityorderId.ToString(), commodityOrder.UserId.ToString(), CustomConfig.ZPHAppId,
                            //       commodityOrder.Code, commodityOrder.State, messages, type);
                            //}
                        });
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 3, Message = "退款失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("售后直接到账退款服务异常。orderRefundDto：{0}", JsonHelper.JsonSerializer(orderRefundDto)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(orderRefundDto.OrderId);
            }
        }
    }
}
