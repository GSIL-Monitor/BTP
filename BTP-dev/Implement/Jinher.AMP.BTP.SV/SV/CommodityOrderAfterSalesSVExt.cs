
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/10/23 11:03:44
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.BE.BELogic;
using System.Collections.Generic;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.FSP.ISV.Facade;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using System.Data;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using System.Text;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.FSP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityOrderAfterSalesSV : BaseSv, ICommodityOrderAfterSales
    {

        /// <summary>
        /// 处理申请的仅退款退款处理订单 10天内未响应 交易状态变为 7 已退款
        ///  </summary>
        public void AutoDaiRefundOrderAfterSalesExt()
        {
            LogHelper.Info(string.Format("处理10天内未响应的申请的仅退款的订单Job服务开始"));

            //处理订单状态为已退款
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();

                //所有阳关餐饮的app.
                Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
                List<Guid> ygcyAppids = storefacade.GetAppIdList("1");

                while (true)
                {
                    DateTime now = DateTime.Now;
                    //售后查询10天内 商家未处理的申请退款的订单
                    DateTime lastday = now.AddDays(-10);
                    //DateTime lastday = now;
                    var jdOrderRefundAfterSalesQuery = JdOrderRefundAfterSales.ObjectSet();
                    var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                          join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                          join com in CommodityOrder.ObjectSet() on r.OrderId equals com.Id
                                          where c.SelfTakeFlag == 0 && c.State == 5 && r.State == 0 && r.RefundType == 0 && r.SubTime < lastday
                                          // 过滤京东订单
                                          && !jdOrderRefundAfterSalesQuery.Any(j => j.OrderRefundAfterSalesId == r.Id)
                                          //过滤掉“阳关餐饮”的订单，该类订单不需要自动退款。
                                            && !ygcyAppids.Contains(com.AppId)
                                          select new
                                          {
                                              Id = com.Id,
                                              commodityOrderService = c,
                                              orderRefundAfterSales = r,
                                              commodityOrder = com
                                          }).Take(pageSize).ToList();

                    if (!afterSalesList.Any())
                        break;
                    //售后旧订单状态
                    int oldState = 0;
                    //售后旧退款流水状态
                    int oldOrderRefundAfterSalesState = -1;

                    LogHelper.Info(string.Format("处理5天内 商家未处理的申请退款订单服务处理订单数:{0}", afterSalesList.Count));

                    foreach (var com in afterSalesList)
                    {
                        var commodityOrder = com.commodityOrder;
                        ContextFactory.ReleaseContextSession();
                        //当前要处理的售后订单
                        var commodityOrderService = com.commodityOrderService;
                        //当前要处理的售后退款申请
                        var orderRefundAfterSales = com.orderRefundAfterSales;
                        //售后旧订单状态
                        oldState = commodityOrderService.State;
                        oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                        UpdateOrderStateTo7(commodityOrder, commodityOrderService, orderRefundAfterSales, contextSession);

                        contextSession.SaveChanges();

                        try
                        {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统处理售后5天内商家未响应，自动达成同意退款申请协议订单";
                            journal.Code = commodityOrder.Code;
                            journal.SubId = commodityOrder.UserId;
                            journal.SubTime = DateTime.Now;
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
                            LogHelper.Error("系统处理5天内商家未响应，自动达成同意退款申请协议订单记日志异常。", ex);
                            continue;
                        }
                        //添加消息
                        BTPMessageSV addmassage = new BTPMessageSV();
                        AfterSalesMessages messageModel = new AfterSalesMessages();
                        messageModel.IsAuto = true;
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
                    }
                    if (afterSalesList.Count < pageSize)
                    {
                        break;
                    }
                }
                LogHelper.Info("系统处理5天内商家未响应，自动达成同意退款申请协议Job处理成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理5天内商家未响应，自动达成同意退款申请协议订单Job服务异常。", ex);
            }
        }

        /// <summary>
        /// 售后提交退款/退货申请订单
        /// </summary>
        /// <param name="submitOrderRefundDTO">DTO</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SubmitOrderRefundAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            if (submitOrderRefundDTO.OrderItemId != Guid.Empty)
            {
                return SubmitOrderItemRefundAfterSales(submitOrderRefundDTO);
            }
            //退积分金额
            decimal spendScoreMoney = 0;
            // 退易捷币
            decimal spendYJBMoney = 0;
            decimal spendAllYJBMoney = 0;
            if (submitOrderRefundDTO == null || submitOrderRefundDTO.commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (submitOrderRefundDTO.RefundType != 0 && submitOrderRefundDTO.RefundType != 1)
            {
                return new ResultDTO { ResultCode = 1, Message = "退款类型参数不正确" };
            }
            if (submitOrderRefundDTO.RefundMoney < 0)
            {
                return new ResultDTO { ResultCode = 1, Message = "退款金额不能小于0" };
            }
            if (!OrderSV.LockOrder(submitOrderRefundDTO.commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var afterSalesList = (from c in CommodityOrder.ObjectSet()
                                      join r in CommodityOrderService.ObjectSet() on c.Id equals r.Id
                                      where c.Id == submitOrderRefundDTO.commodityorderId
                                      select new
                                      {
                                          CommodityOrder = c,
                                          CommodityOrderService = r
                                      }).FirstOrDefault();

                if (afterSalesList == null || afterSalesList.CommodityOrder == null || afterSalesList.CommodityOrderService == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "售后订单不存在" };
                }

                var commodityOrderService = afterSalesList.CommodityOrderService;
                var commodityOrder = afterSalesList.CommodityOrder;

                //订单状态（必填）：确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12,售后交易成功=15
                if (commodityOrderService.State != 3)
                {
                    return new ResultDTO { ResultCode = 3, Message = "订单状态无法申请售后退款" };
                }

                if (ThirdECommerceHelper.IsWangYiYanXuan(commodityOrder.AppId))
                {
                    return new ResultDTO { ResultCode = 1, Message = "该订单不支持整单退款~" };
                }

                //查询抵用券
                decimal couponprice = 0;
                decimal couponuseprice = 0;
                var useryjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
                if (useryjcoupon.Data != null)
                {
                    foreach (var item in useryjcoupon.Data)
                    {
                        if (item != null)
                        {
                            couponprice += item.Price;
                            couponuseprice += (item.UseAmount ?? 0);
                        }
                    }
                }

                //退款金额
                if (submitOrderRefundDTO.RefundMoney > commodityOrder.RealPrice)
                {
                    bool isOverPrice = true;
                    spendScoreMoney = OrderPayDetail.ObjectSet().Where(t => t.OrderId == commodityOrder.Id && t.ObjectType == 2 && t.Amount > 0).Select(t => t.Amount).FirstOrDefault();
                    if (spendScoreMoney > 0)
                    {
                        if (submitOrderRefundDTO.RefundMoney <= commodityOrder.RealPrice + spendScoreMoney)
                        {
                            isOverPrice = false;
                        }
                    }

                    //查询抵用券
                    if (submitOrderRefundDTO.RefundMoney >= commodityOrder.RealPrice + couponprice)
                    {
                        if (couponprice > 0)
                        {
                            isOverPrice = false;
                        }
                    }

                    decimal realprice = 0;
                    if (commodityOrder.State == 1 || commodityOrder.State == 8)
                    {//待发货，退运费
                        realprice = commodityOrder.RealPrice.Value;
                    }
                    else
                    {//其他状态不退运费
                        realprice = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                    }
                    // 查询易捷币
                    if (submitOrderRefundDTO.RefundMoney > realprice + spendScoreMoney + couponprice)
                    {
                        var yjbInfo = YJBSV.GetOrderYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                        if (!yjbInfo.IsSuccess)
                        {
                            return new ResultDTO { ResultCode = 1, Message = yjbInfo.Message };
                        }
                        spendAllYJBMoney = yjbInfo.Data.InsteadCashAmount;
                        if (spendAllYJBMoney > 0)
                        {
                            if (submitOrderRefundDTO.RefundMoney <= realprice + spendScoreMoney + spendAllYJBMoney + couponprice)
                            {
                                spendYJBMoney = submitOrderRefundDTO.RefundMoney - realprice - spendScoreMoney - couponprice;
                                isOverPrice = false;
                            }
                        }
                    }
                    else
                    {
                        isOverPrice = false;
                    }

                    if (isOverPrice)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款金额不能大于订单金额" };
                    }
                }

                int oldcommodityOrderServiceState = commodityOrderService.State;

                OrderRefundAfterSales orderRefundAfterSales = new OrderRefundAfterSales();
                orderRefundAfterSales.Id = Guid.NewGuid();
                orderRefundAfterSales.RefundType = submitOrderRefundDTO.RefundType;
                orderRefundAfterSales.RefundReason = submitOrderRefundDTO.RefundReason;
                //if (submitOrderRefundDTO.RefundMoney > commodityOrder.RealPrice + couponprice + spendYJBMoney)
                //{
                //    commodityOrder.RealPrice = commodityOrder.RealPrice.HasValue ? commodityOrder.RealPrice.Value : 0;
                //    orderRefundAfterSales.RefundMoney = commodityOrder.RealPrice.Value + spendYJBMoney - commodityOrder.Freight;
                //    orderRefundAfterSales.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - spendYJBMoney;
                //    // 退易捷币金额
                //    orderRefundAfterSales.RefundYJBMoney = spendYJBMoney;

                //    commodityOrder.YJCouponPrice = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - commodityOrder.Freight;
                //    orderRefundAfterSales.OrderRefundMoneyAndCoupun = commodityOrder.RealPrice.Value + spendYJBMoney + couponprice;
                //}
                //else
                //{
                //    orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                //    if (submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value>0)
                //    {
                //        orderRefundAfterSales.RefundMoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                //        commodityOrder.YJCouponPrice = submitOrderRefundDTO.RefundMoney - commodityOrder.RealPrice.Value - commodityOrder.Freight;
                //    }
                //    else
                //    {
                //        orderRefundAfterSales.RefundMoney = submitOrderRefundDTO.RefundMoney;
                //        commodityOrder.YJCouponPrice = 0;
                //    }
                //    orderRefundAfterSales.RefundScoreMoney = 0;
                //    orderRefundAfterSales.RefundYJBMoney = 0;
                //}

                if (submitOrderRefundDTO.RefundMoney >= (commodityOrder.RealPrice - commodityOrder.Freight) + couponuseprice)
                {//全额退 
                    LogHelper.Info("SubmitOrderRefundAfterSales全额退OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                    LogHelper.Info("SubmitOrderRefundAfterSales全额退RefundMoney:" + (commodityOrder.RealPrice.Value - commodityOrder.Freight));
                    LogHelper.Info("SubmitOrderRefundAfterSales全额退RefundYJCouponMoney:" + couponprice);
                    LogHelper.Info("SubmitOrderRefundAfterSales全额退RefundYJBMoney:" + spendYJBMoney);
                    orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                    orderRefundAfterSales.RefundMoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                    //commodityOrder.YJCouponPrice = couponprice;
                    orderRefundAfterSales.RefundYJCouponMoney = couponprice;
                    //orderRefundAfterSales.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - orderRefundAfterSales.RefundMoney - spendYJBMoney - couponprice;
                    orderRefundAfterSales.RefundScoreMoney = 0;
                    // 退易捷币金额
                    orderRefundAfterSales.RefundYJBMoney = spendYJBMoney;

                }
                else
                {//部分退
                    if (submitOrderRefundDTO.RefundMoney <= (commodityOrder.RealPrice - commodityOrder.Freight))
                    {//只退现金
                        LogHelper.Info("SubmitOrderRefundAfterSales部分退，只退现金RefundMoney:" + submitOrderRefundDTO.RefundMoney);
                        LogHelper.Info("SubmitOrderRefundAfterSales部分退，只退现金OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                        orderRefundAfterSales.RefundMoney = submitOrderRefundDTO.RefundMoney;
                        orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                    }
                    else if (submitOrderRefundDTO.RefundMoney < (commodityOrder.RealPrice - commodityOrder.Freight) + couponuseprice)
                    {//退现金+部分抵用券
                        LogHelper.Info("SubmitOrderRefundAfterSales退现金+部分抵用券RefundMoney:" + (commodityOrder.RealPrice.Value - commodityOrder.Freight));
                        LogHelper.Info("SubmitOrderRefundAfterSales退现金+部分抵用券RefundMoney:" + submitOrderRefundDTO.RefundMoney);
                        LogHelper.Info("SubmitOrderRefundAfterSales退现金+部分抵用券RefundYJCouponMoney:" + (orderRefundAfterSales.OrderRefundMoneyAndCoupun - orderRefundAfterSales.RefundMoney));
                        orderRefundAfterSales.RefundMoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
                        orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                        //commodityOrder.YJCouponPrice = orderRefundAfterSales.OrderRefundMoneyAndCoupun - orderRefundAfterSales.RefundMoney;
                        orderRefundAfterSales.RefundYJCouponMoney = orderRefundAfterSales.OrderRefundMoneyAndCoupun - orderRefundAfterSales.RefundMoney;
                    }
                    orderRefundAfterSales.RefundScoreMoney = 0;
                    orderRefundAfterSales.RefundYJBMoney = 0;
                }

                orderRefundAfterSales.RefundDesc = submitOrderRefundDTO.RefundDesc;
                orderRefundAfterSales.OrderId = submitOrderRefundDTO.commodityorderId;
                //退款中
                orderRefundAfterSales.State = 0;
                orderRefundAfterSales.OrderRefundImgs = submitOrderRefundDTO.OrderRefundImgs;
                orderRefundAfterSales.DataType = "0";
                if (commodityOrder.RealPrice + spendScoreMoney + spendAllYJBMoney == submitOrderRefundDTO.RefundMoney)
                {
                    orderRefundAfterSales.IsFullRefund = 1;
                }
                else
                {
                    orderRefundAfterSales.IsFullRefund = 0;
                }
                orderRefundAfterSales.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(orderRefundAfterSales);

                commodityOrderService.State = 5;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                var eventResult = OrderEventHelper.OnOrderRefundAfterSales(commodityOrder, orderRefundAfterSales);
                if (!eventResult.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                }

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {
                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        if (orderRefundAfterSales.RefundType == 0)
                        {
                            journal.Name = "售后退款提交退款申请订单";
                        }
                        else
                        {
                            journal.Name = "售后退款提交退款/退货申请订单";
                        }
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "售后订单状态由3" + "变为" + commodityOrderService.State;
                        journal.CommodityOrderId = commodityOrder.Id;
                        journal.StateFrom = 3;
                        journal.StateTo = commodityOrderService.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("售后提交退款/退货申请订单记日志异常。", ex);
                    }

                    //添加消息
                    BTPMessageSV addmassage = new BTPMessageSV();
                    AfterSalesMessages messageModel = new AfterSalesMessages();
                    messageModel.IsAuto = true;
                    messageModel.Id = commodityOrderService.Id.ToString();
                    messageModel.UserIds = commodityOrder.UserId.ToString();
                    messageModel.AppId = commodityOrder.AppId;
                    messageModel.Code = commodityOrder.Code;
                    messageModel.State = commodityOrderService.State;
                    messageModel.RefundType = orderRefundAfterSales.RefundType;
                    messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                    messageModel.PayType = commodityOrder.Payment;
                    messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                    messageModel.oldOrderRefundAfterSalesState = 0;
                    messageModel.SelfTakeFlag = commodityOrderService.SelfTakeFlag;

                    if (messageModel.SelfTakeFlag == 1)
                    {
                        var UserIds = (from a in AppOrderPickUp.ObjectSet()
                                       join b in AppStsManager.ObjectSet() on a.SelfTakeStationId equals b.SelfTakeStationId
                                       where a.Id == commodityOrderService.Id && !b.IsDel
                                       select b.UserId).ToList();
                        messageModel.SelfTakeManagerIds = UserIds;

                    }
                    messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                    addmassage.AddMessagesAfterSales(messageModel);

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    LogHelper.Error(string.Format("发送申请售后退款消息失败。orderSDTO：{0}", JsonHelper.JsonSerializer(submitOrderRefundDTO)));
                    return new ResultDTO { ResultCode = 1, Message = "发送申请售后退款消息失败" };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("申请售后退款服务异常。submitOrderRefundDTO：{0}", JsonHelper.JsonSerializer(submitOrderRefundDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(submitOrderRefundDTO.commodityorderId);
            }
        }

        /// <summary>
        /// 售后提交退款/退货申请订单 单商品退款
        /// </summary>
        /// <param name="submitOrderRefundDTO">DTO</param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SubmitOrderItemRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            LogHelper.Debug("开始进入单商品售后申请退款方法SubmitOrderItemRefundAfterSales，参数为submitOrderRefundDTO：" + JsonHelper.JsSerializer(submitOrderRefundDTO));
            //退积分金额
            decimal spendScoreMoney = 0;
            // 退易捷币
            decimal spendYJBMoney = 0;
            decimal spendAllYJBMoney = 0;
            if (submitOrderRefundDTO == null || submitOrderRefundDTO.commodityorderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (submitOrderRefundDTO.RefundType != 0 && submitOrderRefundDTO.RefundType != 1)
            {
                return new ResultDTO { ResultCode = 1, Message = "退款类型参数不正确" };
            }
            if (submitOrderRefundDTO.RefundMoney != -999 && submitOrderRefundDTO.RefundMoney < 0)
            {
                return new ResultDTO { ResultCode = 1, Message = "退款金额不能小于0" };
            }
            if (!OrderSV.LockOrder(submitOrderRefundDTO.commodityorderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var afterSalesList = (from c in CommodityOrder.ObjectSet()
                                      join r in CommodityOrderService.ObjectSet() on c.Id equals r.Id
                                      where c.Id == submitOrderRefundDTO.commodityorderId
                                      select new
                                      {
                                          CommodityOrder = c,
                                          CommodityOrderService = r
                                      }).FirstOrDefault();

                if (afterSalesList == null || afterSalesList.CommodityOrder == null || afterSalesList.CommodityOrderService == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "售后订单不存在" };
                }

                var commodityOrderService = afterSalesList.CommodityOrderService;
                var commodityOrder = afterSalesList.CommodityOrder;

                //订单状态（必填）：确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12,售后交易成功=15
                if (commodityOrderService.State != 3)
                {
                    return new ResultDTO { ResultCode = 3, Message = "订单状态无法申请售后退款" };
                }

                var orderItem = OrderItem.FindByID(submitOrderRefundDTO.OrderItemId);

                //退款金额
                //最大退款金额                   
                decimal maxMoney = ((decimal)orderItem.RealPrice * orderItem.Number) + orderItem.Duty;
                var m = (decimal)orderItem.RealPrice * orderItem.Number;
                if (m == 0)
                {
                    maxMoney = (decimal)(orderItem.DiscountPrice * orderItem.Number);
                }

                if (orderItem.FreightPrice != null)
                {
                    maxMoney = maxMoney + (decimal)orderItem.FreightPrice;
                }
                if (orderItem.CouponPrice != null)
                {
                    maxMoney = maxMoney - (decimal)orderItem.CouponPrice;
                }
                if (orderItem.ChangeFreightPrice != null)
                {
                    maxMoney = maxMoney - (decimal)orderItem.ChangeFreightPrice;
                }
                if (orderItem.ChangeRealPrice != null)
                {
                    maxMoney = maxMoney - (decimal)orderItem.ChangeRealPrice;
                }
                if (submitOrderRefundDTO.RefundMoney > maxMoney)
                {
                    return new ResultDTO { ResultCode = 1, Message = "退款金额不能大于订单金额" };
                }
                // -999 表示全部退款
                if (submitOrderRefundDTO.RefundMoney == -999)
                {
                    submitOrderRefundDTO.RefundMoney = maxMoney;
                }
                OrderRefundAfterSales orderRefundAfterSales = new OrderRefundAfterSales();
                orderRefundAfterSales.Id = Guid.NewGuid();
                orderRefundAfterSales.RefundType = submitOrderRefundDTO.RefundType;
                orderRefundAfterSales.RefundReason = submitOrderRefundDTO.RefundReason;

                decimal couponprice = 0;
                var useryjcoupon = YJBSV.GetUserYJCouponItemByOrderId(orderItem.Id);
                if (useryjcoupon.Data != null)
                {
                    foreach (var item in useryjcoupon.Data)
                    {
                        if (item != null)
                        {
                            couponprice += item.UseAmount;
                        }
                    }
                }

                //if (submitOrderRefundDTO.RefundMoney >= orderItem.RealPrice * orderItem.Number)
                //{//单品全额退
                //    orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                //    orderRefundAfterSales.RefundMoney = (decimal)(orderItem.RealPrice * orderItem.Number) - couponprice;
                //    orderRefundAfterSales.RefundScoreMoney = submitOrderRefundDTO.RefundMoney - (decimal)(orderItem.RealPrice * orderItem.Number) - spendYJBMoney;
                //    // 退易捷币金额
                //    orderRefundAfterSales.RefundYJBMoney = spendYJBMoney;
                //}
                //else
                //{//单品部分退
                //    if(submitOrderRefundDTO.RefundMoney - (decimal)(orderItem.RealPrice * orderItem.Number)>0)
                //    {
                //        orderRefundAfterSales.RefundMoney = (decimal)(orderItem.RealPrice * orderItem.Number);
                //        orderItem.YJCouponPrice = submitOrderRefundDTO.RefundMoney - (decimal)(orderItem.RealPrice * orderItem.Number);
                //    }
                //    else
                //    {
                //        orderRefundAfterSales.RefundMoney = submitOrderRefundDTO.RefundMoney;
                //        orderItem.YJCouponPrice = 0;
                //    }
                //    orderRefundAfterSales.OrderRefundMoneyAndCoupun = (submitOrderRefundDTO.RefundMoney - (orderItem.YJCouponPrice??0))>0?submitOrderRefundDTO.RefundMoney - (orderItem.YJCouponPrice ?? 0): submitOrderRefundDTO.RefundMoney;
                //    orderRefundAfterSales.RefundScoreMoney = 0;
                //    orderRefundAfterSales.RefundYJBMoney = 0;
                //}

                //计算实付金额
                decimal yjbprice = 0;
                var yjbresult = YJBSV.GetOrderItemYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
                if (yjbresult.Data != null)
                {
                    yjbprice = yjbresult.Data.Items != null ? yjbresult.Data.Items[0].InsteadCashAmount : 0;
                }
                var CurrPic1 = orderItem.RealPrice * orderItem.Number;
                if (CurrPic1 == 0)
                {
                    CurrPic1 = (orderItem.DiscountPrice.Value * orderItem.Number);
                }
                var cashmoney = CurrPic1 - (orderItem.CouponPrice ?? 0) - (orderItem.ChangeRealPrice ?? 0) - orderItem.Duty - couponprice - yjbprice;

              
                if (submitOrderRefundDTO.RefundMoney >= (cashmoney ?? 0) + couponprice)
                {//全额退
                    LogHelper.Info("SubmitOrderItemRefundAfterSales全额退OrderRefundMoneyAndCoupun:"+ submitOrderRefundDTO.RefundMoney);
                    LogHelper.Info("SubmitOrderItemRefundAfterSales全额退RefundMoney:" + (cashmoney ?? 0));
                    LogHelper.Info("SubmitOrderItemRefundAfterSales全额退RefundYJCouponMoney:" + couponprice);
                    LogHelper.Info("SubmitOrderItemRefundAfterSales全额退RefundYJBMoney:" + spendYJBMoney);

                    orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                    orderRefundAfterSales.RefundMoney = (cashmoney ?? 0);
                    //commodityOrder.YJCouponPrice = couponprice;
                    orderRefundAfterSales.RefundYJCouponMoney = couponprice;
                    // 退易捷币金额
                    orderRefundAfterSales.RefundYJBMoney = spendYJBMoney;
                    orderRefundAfterSales.RefundScoreMoney = 0;
                }
                else
                {//部分退
                    if (submitOrderRefundDTO.RefundMoney <= (cashmoney ?? 0))
                    {//只退现金
                        LogHelper.Info("SubmitOrderItemRefundAfterSales部分退,只退现金RefundMoney:" + submitOrderRefundDTO.RefundMoney);

                        orderRefundAfterSales.RefundMoney = submitOrderRefundDTO.RefundMoney;
                        orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                    }
                    else if (submitOrderRefundDTO.RefundMoney < (cashmoney ?? 0) + couponprice)
                    {//退现金+部分抵用券
                        LogHelper.Info("SubmitOrderItemRefundAfterSales部分退,退现金+部分抵用券RefundMoney:" + (cashmoney ?? 0));
                        LogHelper.Info("SubmitOrderItemRefundAfterSales部分退,退现金+部分抵用券OrderRefundMoneyAndCoupun:" + submitOrderRefundDTO.RefundMoney);
                        LogHelper.Info("SubmitOrderItemRefundAfterSales部分退,退现金+部分抵用券RefundYJCouponMoney:" + (orderRefundAfterSales.OrderRefundMoneyAndCoupun - orderRefundAfterSales.RefundMoney));

                        orderRefundAfterSales.RefundMoney = (cashmoney ?? 0);
                        orderRefundAfterSales.OrderRefundMoneyAndCoupun = submitOrderRefundDTO.RefundMoney;
                        //commodityOrder.YJCouponPrice = orderRefundAfterSales.OrderRefundMoneyAndCoupun - orderRefundAfterSales.RefundMoney;
                        orderRefundAfterSales.RefundYJCouponMoney = orderRefundAfterSales.OrderRefundMoneyAndCoupun - orderRefundAfterSales.RefundMoney;
                    }
                    orderRefundAfterSales.RefundScoreMoney = 0;
                }
                

                orderRefundAfterSales.RefundDesc = submitOrderRefundDTO.RefundDesc;
                orderRefundAfterSales.OrderId = submitOrderRefundDTO.commodityorderId;
                //退款中
                orderRefundAfterSales.State = 0;
                orderRefundAfterSales.OrderRefundImgs = submitOrderRefundDTO.OrderRefundImgs;
                orderRefundAfterSales.DataType = "2";

                decimal rePrice = 0;
                decimal couponPrice = orderItem.CouponPrice == null ? 0 : (decimal)orderItem.CouponPrice;
                decimal freightPrice = orderItem.FreightPrice == null ? 0 : (decimal)orderItem.FreightPrice;
                decimal changeFreightPrice = orderItem.ChangeFreightPrice == null ? 0 : (decimal)orderItem.ChangeFreightPrice;
                decimal changeRealPrice = orderItem.ChangeRealPrice == null ? 0 : (decimal)orderItem.ChangeRealPrice;
                if (commodityOrder.State == 1)
                {
                    rePrice = (((decimal)orderItem.RealPrice * orderItem.Number) - couponPrice + freightPrice - changeFreightPrice - changeRealPrice + orderItem.Duty);
                }
                else if (commodityOrder.State == 2 || commodityOrder.State == 2)
                {
                    rePrice = (((decimal)orderItem.RealPrice * orderItem.Number) - couponPrice - changeRealPrice - orderItem.Duty);
                    //最大退款金额
                    var maxRePrice = (orderItem.RealPrice * orderItem.Number) - couponPrice + freightPrice - changeFreightPrice - changeRealPrice - orderItem.Duty;
                    if (maxRePrice < rePrice)
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款金额不能大于最大退款金额" };
                    }
                }

                if (rePrice == submitOrderRefundDTO.RefundMoney)
                {
                    orderRefundAfterSales.IsFullRefund = 1;
                }
                else
                {
                    orderRefundAfterSales.IsFullRefund = 0;
                }

                #region 进销存京东订单
                if (submitOrderRefundDTO.IsJdEclpOrder)
                {
                    orderRefundAfterSales.JDEclpOrderRefundAfterSalesId = Guid.NewGuid();
                    orderRefundAfterSales.PickwareType = submitOrderRefundDTO.PickwareType;
                    if (submitOrderRefundDTO.Address != null)
                    {
                        orderRefundAfterSales.PickwareAddress = submitOrderRefundDTO.Address.ProviceCityStr + submitOrderRefundDTO.Address.pickwareAddress;
                        orderRefundAfterSales.CustomerContactName = submitOrderRefundDTO.Address.customerContactName;
                        orderRefundAfterSales.CustomerTel = submitOrderRefundDTO.Address.customerTel;
                    }
                }
                #endregion

                orderRefundAfterSales.OrderItemId = submitOrderRefundDTO.OrderItemId;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(orderRefundAfterSales);

                //保存订单项退款状态
                orderItem.State = 1;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;
                contextSession.SaveObject(orderItem);

                var orderItemCount = OrderItem.ObjectSet().Count(t =>
                                t.Id != orderItem.Id && t.CommodityOrderId == submitOrderRefundDTO.commodityorderId &&
                                (t.State == 0 || t.State == 4 || t.State == 5));
                if (orderItemCount == 0)
                {
                    commodityOrderService.State = 5;
                    commodityOrderService.ModifiedOn = DateTime.Now;
                    commodityOrderService.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(commodityOrderService);

                    commodityOrder.ModifiedOn = DateTime.Now;
                    commodityOrder.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(commodityOrder);
                }

                //****** 判断是否为京东退款  增加字段IsJDOrder
                // if (submitOrderRefundDTO.IsJdEclpOrder == false && submitOrderRefundDTO.Address != null)
                if (submitOrderRefundDTO.IsJDOrder)
                {
                    var result = JdOrderHelper.SubmitRefund(contextSession, commodityOrder, orderItem, orderRefundAfterSales, submitOrderRefundDTO.Address);
                    if (!result.isSuccess)
                    {
                        return result;
                    }
                }

                #region //*******判断是否为苏宁退款
                if (submitOrderRefundDTO.IsSNOrder)
                {
                    var result = SNOrderAfterSalesHelper.SNSubmitRefund(contextSession, commodityOrder, orderItem, orderRefundAfterSales, submitOrderRefundDTO);
                    if (!result.isSuccess)
                    {
                        return result;
                    }
                }
                #endregion


                var eventResult = OrderEventHelper.OnOrderItemRefundAfterSales(commodityOrder, orderItem, orderRefundAfterSales);
                if (!eventResult.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                }

                int reslult = contextSession.SaveChanges();
                if (reslult > 0)
                {

                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        if (orderRefundAfterSales.RefundType == 0)
                        {
                            journal.Name = "售后退款提交退款申请订单";
                        }
                        else
                        {
                            journal.Name = "售后退款提交退款/退货申请订单";
                        }
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.CommodityOrderId = commodityOrder.Id;
                        if (orderItemCount == 0)
                        {
                            journal.Details = "售后订单状态由3" + "变为" + commodityOrderService.State;
                            journal.StateFrom = 3;
                            journal.StateTo = commodityOrderService.State;
                        }
                        else
                        {
                            journal.Details = "订单单商品退款，订单id：" + commodityOrder.Id + "订单商品项id：" + orderItem.Id;
                            journal.StateFrom = 3;
                            journal.StateTo = commodityOrderService.State; ;
                        }
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("售后提交退款/退货申请订单记日志异常。", ex);
                    }
                    if (orderItemCount == 0)
                    {
                        //添加消息
                        BTPMessageSV addmassage = new BTPMessageSV();
                        AfterSalesMessages messageModel = new AfterSalesMessages();
                        messageModel.IsAuto = true;
                        messageModel.Id = commodityOrderService.Id.ToString();
                        messageModel.UserIds = commodityOrder.UserId.ToString();
                        messageModel.AppId = commodityOrder.AppId;
                        messageModel.Code = commodityOrder.Code;
                        messageModel.State = commodityOrderService.State;
                        messageModel.RefundType = orderRefundAfterSales.RefundType;
                        messageModel.RefundMoney = orderRefundAfterSales.RefundMoney +
                                                   orderRefundAfterSales.RefundScoreMoney;
                        messageModel.PayType = commodityOrder.Payment;
                        messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                        messageModel.oldOrderRefundAfterSalesState = 0;
                        messageModel.SelfTakeFlag = commodityOrderService.SelfTakeFlag;

                        if (messageModel.SelfTakeFlag == 1)
                        {
                            var UserIds = (from a in AppOrderPickUp.ObjectSet()
                                           join b in AppStsManager.ObjectSet() on a.SelfTakeStationId equals b.SelfTakeStationId
                                           where a.Id == commodityOrderService.Id && !b.IsDel
                                           select b.UserId).ToList();
                            messageModel.SelfTakeManagerIds = UserIds;

                        }
                        messageModel.EsAppId = commodityOrder.EsAppId.HasValue
                            ? commodityOrder.EsAppId.Value
                            : commodityOrder.AppId;
                        addmassage.AddMessagesAfterSales(messageModel);

                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                }
                else
                {
                    LogHelper.Error(string.Format("发送申请售后退款消息失败。orderSDTO：{0}", JsonHelper.JsonSerializer(submitOrderRefundDTO)));
                    return new ResultDTO { ResultCode = 1, Message = "发送申请售后退款消息失败" };
                }
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("申请售后退款服务异常。submitOrderRefundDTO：{0}", JsonHelper.JsonSerializer(submitOrderRefundDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(submitOrderRefundDTO.commodityorderId);
            }
        }


        /// <summary>
        /// 售后撤销退款/退货申请
        /// </summary>
        /// <param name="cancelOrderRefundDTO">撤销退款/退货申请</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderRefundAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelOrderRefundDTO cancelOrderRefundDTO)
        {
            if (cancelOrderRefundDTO.OrderItemId != Guid.Empty)
            {
                //订单单商品撤销
                return CancelOrderItemRefundAfterSales(cancelOrderRefundDTO);
            }
            if (cancelOrderRefundDTO == null || cancelOrderRefundDTO.CommodityOrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(cancelOrderRefundDTO.CommodityOrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      orderby r.SubTime descending
                                      where c.Id == cancelOrderRefundDTO.CommodityOrderId && (c.State == 5 && r.State == 0 || c.State == 10 && r.State == 10)
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == cancelOrderRefundDTO.CommodityOrderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "此类申请订单不存在" };
                }
                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                commodityOrderService.State = 3;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.State = 3;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                var eventResult = OrderEventHelper.OnCancelOrderRefundAfterSales(commodityOrder, orderRefundAfterSales);
                if (!eventResult.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                }

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
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售后退款撤销退款/退货申请";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
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
                        LogHelper.Error("售后撤销退款/退货申请记日志异常。", ex);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    LogHelper.Error(string.Format("发送已撤销售后退款申请消息失败。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelOrderRefundDTO)));
                    return new ResultDTO { ResultCode = 1, Message = "发送已撤销售后退款申请消息失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("撤销售后退款申请。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelOrderRefundDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelOrderRefundDTO.CommodityOrderId);
            }

        }

        /// <summary>
        /// 售后撤销退款/退货申请
        /// </summary>
        /// <param name="cancelOrderRefundDTO">撤销退款/退货申请</param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderItemRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelOrderRefundDTO cancelOrderRefundDTO)
        {
            LogHelper.Debug("开始进入售后撤销退款/退货申请方法CancelOrderItemRefundAfterSales，参数为cancelOrderRefundDTO：" + JsonHelper.JsSerializer(cancelOrderRefundDTO));
            if (cancelOrderRefundDTO == null || cancelOrderRefundDTO.CommodityOrderId == Guid.Empty || cancelOrderRefundDTO.OrderItemId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(cancelOrderRefundDTO.CommodityOrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      orderby r.SubTime descending
                                      where c.Id == cancelOrderRefundDTO.CommodityOrderId && (c.State == 5 && r.State == 0 || c.State == 10 && r.State == 10)
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == cancelOrderRefundDTO.CommodityOrderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "此类申请订单不存在" };
                }
                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                commodityOrderService.State = 3;
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

                orderRefundAfterSales.State = 3;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                commodityOrder.ModifiedOn = DateTime.Now;
                commodityOrder.EntityState = System.Data.EntityState.Modified;

                var orderItem = OrderItem.FindByID(cancelOrderRefundDTO.OrderItemId);
                orderItem.State = 0;
                orderItem.ModifiedOn = DateTime.Now;
                orderItem.EntityState = EntityState.Modified;

                // 京东退款
                var jdCancelResult = JdOrderHelper.CancelRefund(contextSession, orderRefundAfterSales);
                if (!jdCancelResult.isSuccess)
                {
                    return jdCancelResult;
                }

                var eventResult = OrderEventHelper.OnCancelOrderItemRefundAfterSales(commodityOrder, orderItem, orderRefundAfterSales);
                if (!eventResult.isSuccess)
                {
                    return new ResultDTO { ResultCode = 1, Message = eventResult.Message };
                }

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
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });
                    try
                    {
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售后退款撤销退款/退货申请";
                        journal.Code = commodityOrder.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
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
                        LogHelper.Error("售后撤销退款/退货申请记日志异常。", ex);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    LogHelper.Error(string.Format("发送已撤销售后退款申请消息失败。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelOrderRefundDTO)));
                    return new ResultDTO { ResultCode = 1, Message = "发送已撤销售后退款申请消息失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("撤销售后退款申请。cancelOrderRefundDTO：{0}", JsonHelper.JsonSerializer(cancelOrderRefundDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(cancelOrderRefundDTO.CommodityOrderId);
            }

        }

        /// <summary>
        /// 售后查询退款/退货申请
        /// </summary>
        /// <param name="commodityorderId">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSalesExt(System.Guid commodityorderId, Guid orderItemId)
        {
            try
            {
                var order = CommodityOrder.FindByID(commodityorderId);
                if (orderItemId != Guid.Empty)
                {
                    var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == commodityorderId && t.OrderItemId == orderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                    if (orderRefundAfterSales != null)
                    {
                        SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                        {
                            Id = orderRefundAfterSales.Id,
                            commodityorderId = orderRefundAfterSales.OrderId,
                            RefundReason = orderRefundAfterSales.RefundReason,
                            RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney,
                            RefundDesc = orderRefundAfterSales.RefundDesc,
                            OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                            State = orderRefundAfterSales.State,
                            RefundExpCo = orderRefundAfterSales.RefundExpCo,
                            RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                            RefundType = orderRefundAfterSales.RefundType,
                            RefuseReason = orderRefundAfterSales.RefuseReason,
                            RefuseTime = orderRefundAfterSales.RefuseTime,
                            RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                            SubTime = orderRefundAfterSales.SubTime,
                            PickwareType = orderRefundAfterSales.PickwareType,
                            Address = new AddressInfo
                            {
                                pickwareAddress = orderRefundAfterSales.PickwareAddress,
                                customerContactName = orderRefundAfterSales.CustomerContactName,
                                customerTel = orderRefundAfterSales.CustomerTel
                            }
                        };
                        var orderItem = OrderItem.FindByID(orderItemId);
                        {
                            result.Pic = orderItem.PicturesPath;
                            result.Name = orderItem.Name;
                            result.CommodityAttributes = orderItem.CommodityAttributes;
                            result.Num = orderItem.Number;
                        }
                        result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                        return result;
                    }
                    else
                    {
                        orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == commodityorderId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                        if (orderRefundAfterSales != null)
                        {
                            SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                            {
                                Id = orderRefundAfterSales.Id,
                                commodityorderId = orderRefundAfterSales.OrderId,
                                RefundReason = orderRefundAfterSales.RefundReason,
                                RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney,
                                RefundDesc = orderRefundAfterSales.RefundDesc,
                                OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                                State = orderRefundAfterSales.State,
                                RefundExpCo = orderRefundAfterSales.RefundExpCo,
                                RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                                RefundType = orderRefundAfterSales.RefundType,
                                RefuseReason = orderRefundAfterSales.RefuseReason,
                                RefuseTime = orderRefundAfterSales.RefuseTime,
                                RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                                SubTime = orderRefundAfterSales.SubTime,
                                PickwareType = orderRefundAfterSales.PickwareType,
                                Address = new AddressInfo
                                {
                                    pickwareAddress = orderRefundAfterSales.PickwareAddress,
                                    customerContactName = orderRefundAfterSales.CustomerContactName,
                                    customerTel = orderRefundAfterSales.CustomerTel
                                }
                            };
                            result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                            return result;
                        }
                    }
                    return null;
                }
                else
                {
                    var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderId == commodityorderId).OrderByDescending(t => t.SubTime).FirstOrDefault();
                    if (orderRefundAfterSales != null)
                    {
                        SubmitOrderRefundDTO result = new SubmitOrderRefundDTO()
                        {
                            Id = orderRefundAfterSales.Id,
                            commodityorderId = orderRefundAfterSales.OrderId,
                            RefundReason = orderRefundAfterSales.RefundReason,
                            RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney,
                            RefundDesc = orderRefundAfterSales.RefundDesc,
                            OrderRefundImgs = orderRefundAfterSales.OrderRefundImgs,
                            State = orderRefundAfterSales.State,
                            RefundExpCo = orderRefundAfterSales.RefundExpCo,
                            RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo,
                            RefundType = orderRefundAfterSales.RefundType,
                            RefuseReason = orderRefundAfterSales.RefuseReason,
                            RefuseTime = orderRefundAfterSales.RefuseTime,
                            RefundExpOrderTime = orderRefundAfterSales.RefundExpOrderTime,
                            SubTime = orderRefundAfterSales.SubTime,
                            PickwareType = orderRefundAfterSales.PickwareType,
                            Address = new AddressInfo
                            {
                                pickwareAddress = orderRefundAfterSales.PickwareAddress,
                                customerContactName = orderRefundAfterSales.CustomerContactName,
                                customerTel = orderRefundAfterSales.CustomerTel
                            }
                        };
                        result.IsThirdECommerce = ThirdECommerceHelper.IsWangYiYanXuan(order.AppId);
                        return result;
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查看退款申请服务异常。commodityorderId：{0}", commodityorderId), ex);
                return new SubmitOrderRefundDTO();
            }
        }

        /// <summary>
        /// 售后增加退货物流信息
        /// </summary>
        /// <param name="addOrderRefundExpDTO">物流信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOrderRefundExpAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.AddOrderRefundExpDTO addOrderRefundExpDTO)
        {
            if (addOrderRefundExpDTO.OrderItemId != Guid.Empty)
            {
                var orderItem = OrderItem.FindByID(addOrderRefundExpDTO.OrderItemId);
                //判断不是整单退款
                if (orderItem.State != 0)
                {
                    return AddOrderItemRefundExpAfterSales(addOrderRefundExpDTO);
                }
            }
            if (addOrderRefundExpDTO == null || addOrderRefundExpDTO.CommodityOrderId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (!OrderSV.LockOrder(addOrderRefundExpDTO.CommodityOrderId))
            {
                return new ResultDTO { ResultCode = 110, Message = "操作失败" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                      join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                      where c.Id == addOrderRefundExpDTO.CommodityOrderId && (c.State == 10 && r.State == 10)
                                      select new
                                      {
                                          commodityOrderService = c,
                                          orderRefundAfterSales = r
                                      }).ToList();

                CommodityOrderService commodityOrderService = afterSalesList.Select(t => t.commodityOrderService).FirstOrDefault();
                OrderRefundAfterSales orderRefundAfterSales = afterSalesList.Select(t => t.orderRefundAfterSales).FirstOrDefault();
                var commodityOrder = CommodityOrder.ObjectSet().Where(n => n.Id == addOrderRefundExpDTO.CommodityOrderId).FirstOrDefault();

                if (commodityOrderService == null || orderRefundAfterSales == null || commodityOrder == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "此类申请订单不存在" };
                }
                if (commodityOrderService.SelfTakeFlag == 1)
                {
                    return new ResultDTO { ResultCode = 4, Message = "自提订单不能提交物流信息" };
                }
                int oldState = commodityOrderService.State;
                int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                orderRefundAfterSales.RefundExpCo = string.IsNullOrWhiteSpace(addOrderRefundExpDTO.RefundExpCo) ? "" : addOrderRefundExpDTO.RefundExpCo.Trim();
                orderRefundAfterSales.RefundExpOrderNo = string.IsNullOrWhiteSpace(addOrderRefundExpDTO.RefundExpOrderNo) ? "" : addOrderRefundExpDTO.RefundExpOrderNo.Trim();
                orderRefundAfterSales.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo.Replace("+", "");
                orderRefundAfterSales.State = 11;
                orderRefundAfterSales.RefundExpOrderTime = DateTime.Now;
                orderRefundAfterSales.ModifiedOn = DateTime.Now;
                orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                //更新售后订单的修改时间
                commodityOrderService.ModifiedOn = DateTime.Now;
                commodityOrderService.EntityState = System.Data.EntityState.Modified;

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
                            messageModel.RefundType = orderRefundAfterSales.RefundType;
                            messageModel.RefundMoney = orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundScoreMoney;
                            messageModel.PayType = commodityOrder.Payment;
                            messageModel.orderRefundAfterSalesState = orderRefundAfterSales.State;
                            messageModel.oldOrderRefundAfterSalesState = oldOrderRefundAfterSalesState;
                            messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                            addmassage.AddMessagesAfterSales(messageModel);
                        });

                    //TODO dzc 向第三方“快递鸟”发送物流订阅请求。 
                    OrderExpressRoute oer = new OrderExpressRoute()
                    {
                        ShipExpCo = orderRefundAfterSales.RefundExpCo,
                        ExpOrderNo = orderRefundAfterSales.RefundExpOrderNo
                    };
                    OrderExpressRouteSV oerSv = new OrderExpressRouteSV();
                    oerSv.SubscribeOneOrderExpressExt(oer);

                    //添加退货物流跟踪信息
                    RefundExpressTraceDTO retd = new RefundExpressTraceDTO();
                    retd.OrderId = addOrderRefundExpDTO.CommodityOrderId;
                    retd.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                    retd.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                    retd.UploadExpOrderTime = DateTime.Now;
                    UpdateRefundExpress(retd);

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "退款物流信息提交失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("退款物流信息提交服务异常。addOrderRefundExpDTO：{0}", JsonHelper.JsonSerializer(addOrderRefundExpDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(addOrderRefundExpDTO.CommodityOrderId);
            }
        }

        /// <summary>
        /// 售后增加退货物流信息 单商品
        /// </summary>
        /// <param name="addOrderRefundExpDTO">物流信息</param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOrderItemRefundExpAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.AddOrderRefundExpDTO addOrderRefundExpDTO)
        {
            if (addOrderRefundExpDTO == null || addOrderRefundExpDTO.OrderItemId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var orderRefundAfterSales = OrderRefundAfterSales.ObjectSet().Where(t => t.OrderItemId == addOrderRefundExpDTO.OrderItemId).OrderByDescending(t => t.SubTime).FirstOrDefault();

                if (orderRefundAfterSales != null)
                {
                    orderRefundAfterSales.RefundExpCo = string.IsNullOrWhiteSpace(addOrderRefundExpDTO.RefundExpCo) ? "" : addOrderRefundExpDTO.RefundExpCo.Trim();
                    orderRefundAfterSales.RefundExpOrderNo = string.IsNullOrWhiteSpace(addOrderRefundExpDTO.RefundExpOrderNo) ? "" : addOrderRefundExpDTO.RefundExpOrderNo.Trim();
                    orderRefundAfterSales.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo.Replace("+", "");
                    orderRefundAfterSales.State = 11;
                    orderRefundAfterSales.RefundExpOrderTime = DateTime.Now;
                    orderRefundAfterSales.ModifiedOn = DateTime.Now;
                    orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(orderRefundAfterSales);

                    var orderItem = OrderItem.FindByID(addOrderRefundExpDTO.OrderItemId);
                    orderItem.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                    orderItem.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                    orderItem.ModifiedOn = DateTime.Now;
                    orderItem.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(orderItem);

                    int reslult = contextSession.SaveChanges();
                    if (reslult > 0)
                    {
                        //TODO dzc 向第三方“快递鸟”发送物流订阅请求。 
                        OrderExpressRoute oer = new OrderExpressRoute()
                        {
                            ShipExpCo = orderRefundAfterSales.RefundExpCo,
                            ExpOrderNo = orderRefundAfterSales.RefundExpOrderNo
                        };
                        OrderExpressRouteSV oerSv = new OrderExpressRouteSV();
                        oerSv.SubscribeOneOrderExpressExt(oer);

                        //添加退货物流跟踪信息
                        RefundExpressTraceDTO retd = new RefundExpressTraceDTO();
                        retd.OrderId = addOrderRefundExpDTO.CommodityOrderId;
                        retd.RefundExpCo = orderRefundAfterSales.RefundExpCo;
                        retd.RefundExpOrderNo = orderRefundAfterSales.RefundExpOrderNo;
                        retd.UploadExpOrderTime = DateTime.Now;
                        UpdateRefundExpress(retd);

                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    else
                    {
                        return new ResultDTO { ResultCode = 1, Message = "退款物流信息提交失败" };
                    }
                }
                return new ResultDTO { ResultCode = 1, Message = "退款物流信息提交失败" };

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("退款物流信息提交服务异常。addOrderRefundExpDTO：{0}", JsonHelper.JsonSerializer(addOrderRefundExpDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                OrderSV.UnLockOrder(addOrderRefundExpDTO.CommodityOrderId);
            }
        }


        /// <summary>
        /// 添加退货物流跟踪信息
        /// </summary>
        /// <param name="retd"></param>
        private void UpdateRefundExpress(RefundExpressTraceDTO retd)
        {
            RefundExpressTraceSV sv = new RefundExpressTraceSV();
            sv.UpdateRefundExpressExt(retd);
        }

        /// <summary>
        /// 处理10天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        public void AutoYiRefundOrderAfterSalesExt()
        {
            LogHelper.Info(string.Format("10天内商家未响应，自动达成同意退款/退货申请协议订单服务开始"));
            //处理订单状态为商家同意退款 但是没有收到买家发过来的货
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();

                //所有阳关餐饮的app.
                Jinher.AMP.Store.ISV.Facade.StoreFacade storefacade = new Jinher.AMP.Store.ISV.Facade.StoreFacade();
                List<Guid> ygcyAppids = storefacade.GetAppIdList("1");


                while (true)
                {
                    DateTime now = DateTime.Now;
                    //查询10天内商家未响应，自动达成同意退款/退货申请协议订单
                    DateTime lastday = now.AddDays(-10);
                    //DateTime lastday = now;
                    var jdOrderRefundAfterSalesQuery = JdOrderRefundAfterSales.ObjectSet();
                    var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                          join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                          join com in CommodityOrder.ObjectSet() on r.OrderId equals com.Id
                                          where c.SelfTakeFlag == 0 && c.State == 5 && r.State == 0 && r.RefundType == 1 && r.SubTime < lastday && !directArrivalPayments.Contains(com.Payment)
                                              // 过滤京东订单
                                          && !jdOrderRefundAfterSalesQuery.Any(j => j.OrderRefundAfterSalesId == r.Id)
                                            && !ygcyAppids.Contains(com.AppId)
                                          select new
                                          {
                                              commodityOrderService = c,
                                              orderRefundAfterSales = r,
                                              commodityOrder = com
                                          }).Take(pageSize).ToList();

                    if (!afterSalesList.Any())
                        break;

                    ////要处理的售后订单
                    //var ordersServiceList = afterSalesList.Select(t => t.commodityOrderService).ToList();
                    ////要处理的订单ID列表
                    //List<Guid> orderIds = ordersServiceList.Select(t => t.Id).ToList();
                    ////申请退款列表
                    //var orderRefundAfterSalesList = afterSalesList.Select(t => t.orderRefundAfterSales).ToList();
                    ////售前订单
                    //var commodityOrderList = CommodityOrder.ObjectSet().Where(n => orderIds.Contains(n.Id) && (n.Payment != 1003 || n.Payment != 1004)).ToList();


                    LogHelper.Info(string.Format("处理10天内商家未响应，自动达成同意退款/退货申请协议订单Job服务处理订单数:{0}", afterSalesList.Count));

                    //foreach (CommodityOrder commodityOrder in commodityOrderList)
                    foreach (var com in afterSalesList)
                    {

                        var commodityOrder = com.commodityOrder;
                        LogHelper.Info(string.Format("处理10天内商家未响应，自动达成同意退款/退货申请协议订单Job，订单Id:{0}", commodityOrder.Id));
                        //售后旧订单状态
                        int oldState = 0;
                        //当前要处理的售后订单
                        var commodityOrderService = com.commodityOrderService;
                        //当前要处理的售后退款申请
                        var orderRefundAfterSales = com.orderRefundAfterSales;
                        //售后旧订单状态
                        oldState = commodityOrderService.State;
                        int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;

                        orderRefundAfterSales.State = 10;
                        orderRefundAfterSales.RefuseTime = DateTime.Now;
                        orderRefundAfterSales.ModifiedOn = DateTime.Now;
                        orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                        commodityOrderService.State = 10;
                        commodityOrderService.ModifiedOn = DateTime.Now;
                        commodityOrderService.EntityState = System.Data.EntityState.Modified;

                        //更新订单的修改时间                           
                        commodityOrder.ModifiedOn = DateTime.Now;
                        commodityOrder.EntityState = System.Data.EntityState.Modified;

                        contextSession.SaveChanges();

                        try
                        {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统处理售后10天内商家未响应，自动达成同意退款/退货申请协议订单";
                            journal.Code = commodityOrder.Code;
                            journal.SubId = commodityOrder.UserId;
                            journal.SubTime = DateTime.Now;
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
                            LogHelper.Error(string.Format("系统处理10天内商家未响应，自动达成同意退款/退货申请协议订单保存Job异常。commodityOrderId：{0}", commodityOrderService.Id), ex);
                            continue;
                        }

                        //添加消息
                        BTPMessageSV addmassage = new BTPMessageSV();
                        AfterSalesMessages messageModel = new AfterSalesMessages();
                        messageModel.IsAuto = true;
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
                    }
                    if (afterSalesList.Count < pageSize)
                    {
                        break;
                    }
                }
                LogHelper.Info("系统处理10天内商家未响应，自动达成同意退款/退货申请协议订单Job服务处理成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理10天内商家未响应，自动达成同意退款/退货申请协议订单Job服务异常。", ex);
            }
        }

        /// <summary>
        ///  买家7天不发出退货，满7天自动处理售后
        /// </summary>
        public void AutoRefundAndCommodityOrderAfterSalesExt()
        {
            LogHelper.Info(string.Format("买家7天不发出退货，满7天自动处理售后服务开始"));
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();


                while (true)
                {
                    DateTime now = DateTime.Now;
                    DateTime lastday = now.AddDays(-7);
                    //DateTime lastday = now;
                    var jdOrderRefundAfterSalesQuery = JdOrderRefundAfterSales.ObjectSet();
                    var afterSalesList = (from c in CommodityOrderService.ObjectSet()
                                          join r in OrderRefundAfterSales.ObjectSet() on c.Id equals r.OrderId
                                          join com in CommodityOrder.ObjectSet() on r.OrderId equals com.Id
                                          where c.SelfTakeFlag == 0 && (c.State == 10 && r.State == 10) && r.RefuseTime < lastday && r.RefundType == 1 && !directArrivalPayments.Contains(com.Payment)
                                              // 过滤京东订单
                                          && !jdOrderRefundAfterSalesQuery.Any(j => j.OrderRefundAfterSalesId == r.Id)
                                          select new
                                          {
                                              commodityOrderService = c,
                                              orderRefundAfterSales = r,
                                              commodityOrder = com
                                          }).Take(pageSize).ToList();
                    if (!afterSalesList.Any())
                    {
                        break;
                    }
                    foreach (var com in afterSalesList)
                    {
                        var commodityOrderService = com.commodityOrderService;
                        //当前要处理的售后退款申请
                        var orderRefundAfterSales = com.orderRefundAfterSales;
                        //当前要处理的售前订单
                        var commodityOrder = com.commodityOrder;
                        int oldState = commodityOrderService.State;
                        int oldOrderRefundAfterSalesState = orderRefundAfterSales.State;
                        if (commodityOrder.Payment != 1 && orderRefundAfterSales.RefundMoney > 0)
                        {
                            //更新售后表
                            commodityOrderService.State = 3;
                            orderRefundAfterSales.State = 13;
                            orderRefundAfterSales.NotReceiveTime = DateTime.Now;

                        }
                        else if (commodityOrder.Payment != 1 && orderRefundAfterSales.RefundMoney == 0)
                        {
                            commodityOrderService.State = 3;
                            orderRefundAfterSales.State = 13;
                            orderRefundAfterSales.NotReceiveTime = DateTime.Now;
                        }
                        else
                        {
                            LogHelper.Error(string.Format("此订单不能进行售后退款。commodityOrderId：{0}", commodityOrderService.Id));
                            continue;
                        }

                        commodityOrderService.ModifiedOn = DateTime.Now;
                        commodityOrderService.EntityState = System.Data.EntityState.Modified;

                        orderRefundAfterSales.ModifiedOn = DateTime.Now;
                        orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

                        //更新订单的修改时间                           
                        commodityOrder.ModifiedOn = DateTime.Now;
                        commodityOrder.EntityState = System.Data.EntityState.Modified;

                        contextSession.SaveChanges();

                        try
                        {
                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统处理售后买家7天不发出退货，满7天自动处理售后订单";
                            journal.Code = commodityOrder.Code;
                            journal.SubId = commodityOrder.UserId;
                            journal.SubTime = DateTime.Now;
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
                            LogHelper.Error(string.Format("系统处理买家7天不发出退货，满7天自动处理售后保存日志异常。"), ex);
                            continue;
                        }
                        //添加消息
                        BTPMessageSV addmassage = new BTPMessageSV();
                        AfterSalesMessages messageModel = new AfterSalesMessages();
                        messageModel.IsAuto = true;
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
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("系统处理买家7天不发出退货，满7天自动处理售后服务异常。", ex);
            }
        }

        /// <summary>
        ///  售后同意退款/退货申请(同意退款申请，同意退款/退货申请，确认收到退货)
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
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
                DateTime now = DateTime.Now;

                //退款金额
                decimal orRefundMoney = orderRefundAfterSales.RefundMoney;

                //同意退款申请，以及确认收到退货
                if (newState == 7)
                {

                    if (oldState != 5 && oldState != 10)
                    {
                        LogHelper.Error(string.Format("(警告)code:100001该订单号{0}状态{1}无法变为已退款", commodityOrderService.Id, oldState));
                        return new ResultDTO { ResultCode = 1, Message = "当前订单状态错误,请重试" };
                    }
                    UpdateOrderStateTo7(commodityOrder, commodityOrderService, orderRefundAfterSales, contextSession);

                    if (oldState == 5)
                    {
                        orderRefundAfterSales.RefuseTime = DateTime.Now;
                    }
                }
                //同意退款/退货申请
                else if (newState == 10)
                {
                    if (oldState != 5)
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

                if (result > 0)
                {
                    try
                    {
                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.IsPush = false;
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
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
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
        /// 拒绝(拒绝原因) 暂时不用
        /// </summary>
        /// <param name="refuseDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> DealRefuseBusinessAfterSalesExt(Jinher.AMP.BTP.Deploy.CustomDTO.RefuseDTO refuseDTO)
        {
            return new ResultDTO<int> { ResultCode = 0, Message = "Success" };
        }

        #region shenkt增加

        /// <summary>
        /// 满7天自动处理售后（排除退款退货申请和卖家拒绝之间的时间，排除退款退货申请和卖家同意并未超时未收到货之间的时间）
        /// 先解冻，再支付
        /// </summary>
        public void AutoDealOrderAfterSalesExt()
        {
            LogHelper.Info(string.Format("售后自动确认支付服务开始"));
            //处理订单状态为售后确认收货
            try
            {
                DateTime now = DateTime.Now;
                //售后7天自动处理售后
                double allAfterSalesTime = 7 * 24;
                //获取
                var orders = CommodityOrderService.ObjectSet().Where(p => p.State == 3).ToList();
                //var orderIds = orders.Select(_ => _.Id.ToString());
                //var jdOrderIds = JdOrderItem.ObjectSet().Where(_ => _.State == 3 && orderIds.Contains(_.CommodityOrderId)).Select(_ => _.CommodityOrderId).ToList();
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();

                if (orders.Any())
                {
                    ContextDTO contextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;

                    foreach (CommodityOrderService order in orders)
                    {
                        //// 暂时过滤京东拒收订单
                        //if (jdOrderIds.Contains(order.Id.ToString()))
                        //{
                        //    continue;
                        //}

                        ContextFactory.ReleaseContextSession();
                        var commodityOrder = CommodityOrder.ObjectSet().FirstOrDefault(n => n.Id == order.Id);
                        double afterSalesTimeOut = 0;
                        //获取退款信息
                        var refundInformation = OrderRefundAfterSales.ObjectSet().Where(n => n.OrderId == order.Id).OrderByDescending(p => p.ModifiedOn);
                        //存在售后退款
                        if (refundInformation.Any())
                        {
                            double allRequestTime = 0;
                            //取最后一次发起的售后信息为当前用户售后处理结果
                            var firstRecode = refundInformation.FirstOrDefault();
                            if (firstRecode.State == 2 || firstRecode.State == 4 || firstRecode.State == 13)
                            {
                                foreach (OrderRefundAfterSales orderRefund in refundInformation)
                                {
                                    if (orderRefund.RefuseTime.HasValue)
                                    {
                                        //获取申请拒绝时间和申请未收到货时间之和
                                        allRequestTime += orderRefund.RefuseTime.Value.Subtract(orderRefund.SubTime).TotalHours;
                                    }
                                    if (orderRefund.NotReceiveTime.HasValue)
                                    {
                                        //申请未收到货时间之和
                                        allRequestTime += orderRefund.NotReceiveTime.Value.Subtract(orderRefund.SubTime).TotalHours;
                                    }
                                }
                            }
                            //当前时间-请求拒绝间隔时间和申请未收到货间隔时间=售后期时间
                            afterSalesTimeOut = now.Subtract(order.SubTime).TotalHours - allRequestTime;
                        }
                        else
                            afterSalesTimeOut = now.Subtract(order.SubTime).TotalHours;
                        //售后时间大于7天
                        if (afterSalesTimeOut >= allAfterSalesTime)
                        {
                            if (!directArrivalPayments.Contains(commodityOrder.Payment))
                            {
                                ConfirmPayDTO comConfirmPayDto = null;
                                //金币支付
                                try
                                {
                                    Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayUnFreezeResult = new Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO();
                                    Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO goldPayResult = new Jinher.AMP.FSP.Deploy.CustomDTO.ReturnInfoDTO();

                                    List<object> saveList = new List<object>();

                                    Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId, contextDTO);

                                    if (commodityOrder.RealPrice > 0)
                                    {
                                        //goldPayresult = goldPayFacade.ConfirmPay(buildConfirmPayDTO(contextSession, order, now, out saveList, applicationDTO, isSaveObject: false));
                                        //冻结账户资金
                                        //goldPayresult = goldPayFacade.ConfirmPayFreeze(buildConfirmPayDTO(contextSession, order, now, out saveList, applicationDTO, isSaveObject: false));

                                        //解冻账户资金
                                        goldPayUnFreezeResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayUnFreeze(buildConfirmPayUnFreezeDTO(contextSession, commodityOrder, applicationDTO));

                                        if (goldPayUnFreezeResult.Code != 0)
                                        {
                                            LogHelper.Error(string.Format("售后自动支付，调用ConfirmPayUnFreeze金币账户解冻接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayUnFreezeResult.Code, goldPayUnFreezeResult.Message));
                                            continue;//金币支付失败
                                        }
                                        else
                                        {
                                            LogHelper.Info(string.Format("售后自动支付，调用ConfirmPayUnFreeze金币账户解冻接口,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayUnFreezeResult.Code, goldPayUnFreezeResult.Message));
                                        }

                                        //支付账户付款
                                        var confirmDto = OrderSV.BuildConfirmPayDTOAfterSales(contextSession, commodityOrder, out saveList, applicationDTO, isSaveObject: false);
                                        LogHelper.Info(string.Format("订单确认收货AutoDealOrderAfterSalesExt：订单id={0} , 支付DTO ={1}", commodityOrder.Id, JsonHelper.JsonSerializer(confirmDto)), "BTP_Order");
                                        if (confirmDto == null)
                                        {
                                            LogHelper.Error(string.Format("售后自动支付，调用ConfirmPay确认支付金币接口支付失败，未设置结算价，OrderId:{0},OrderCode:{1}。", order.Id, order.Code));
                                        }
                                        else
                                        {
                                            goldPayResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPay(confirmDto);

                                            if (goldPayResult.Code != 0)
                                            {
                                                LogHelper.Error(string.Format("售后自动支付，调用ConfirmPay确认支付金币接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayResult.Code, goldPayResult.Message));
                                                continue;//金币支付失败
                                            }
                                            else
                                            {
                                                if (goldPayResult.Code == -8)
                                                {
                                                    LogHelper.Error(string.Format("售后自动支付，调用ConfirmPay支付金币接口支付失败,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayResult.Code, goldPayResult.Message));
                                                }

                                                LogHelper.Info(string.Format("售后自动支付，调用ConfirmPay确认支付金币接口,OrderId:{0},OrderCode:{1}。code：{2}，Message：{3}", order.Id, order.Code, goldPayResult.Code, goldPayResult.Message));

                                                if (saveList != null && saveList.Any())
                                                {
                                                    foreach (var o in saveList)
                                                    {
                                                        contextSession.SaveObject(o);
                                                    }
                                                }

                                                // 生成结算单
                                                LogHelper.Info(string.Format("售后自动支付，生成结算单，OrderId: {0}", commodityOrder.Id));
                                                var sa = SettleAccountHelper.CreateSettleAccount(contextSession, commodityOrder);
                                                if (sa != null)
                                                {
                                                    sa.IsPaySuccess = goldPayResult.Code == 0;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Error(string.Format("售后自动确认支付服务异常。commdoityOrderId:{0}", commodityOrder.Id), ex);
                                    continue;//金币支付失败
                                }
                            }

                            //众销与分销以积分形式发放
                            if (CustomConfig.IsShareAsScore)
                            {
                                SignSV.DistributeSaleGiveScore(contextSession, commodityOrder, order);
                                SignSV.ShareSaleGiveScore(contextSession, commodityOrder, order);
                                SignSV.ChannelShareSaleGiveScore(contextSession, commodityOrder, order);
                            }
                            //更新订单状态
                            order.State = 15;
                            order.EndTime = DateTime.Now;
                            order.ModifiedOn = DateTime.Now;
                            order.EntityState = System.Data.EntityState.Modified;

                            // 跟新结算表
                            SettleAccountHelper.AfterSalesEndOrder(contextSession, order);

                            //订单日志
                            Journal journal = new Journal();
                            journal.Id = Guid.NewGuid();
                            journal.Name = "系统售后确认收货后7天自动售后付款";
                            journal.Code = order.Code;
                            journal.SubId = commodityOrder.UserId;
                            journal.SubTime = DateTime.Now;
                            journal.Details = "售后订单状态由3" + "变为15";
                            journal.CommodityOrderId = order.Id;
                            journal.StateFrom = 3;
                            journal.StateTo = 15;
                            journal.IsPush = false;
                            journal.OrderType = commodityOrder.OrderType;

                            journal.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(journal);
                            contextSession.SaveChanges();
                        }

                    }

                }

            }
            catch (Exception ex)
            {

                LogHelper.Error("自动确认支付服务异常。", ex);
            }

        }

        /// <summary>
        /// 售后结束解冻付款参数
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

        /// <summary>
        /// 金额转换成金币
        /// </summary>
        /// <param name="price">总价</param>
        /// <returns>转化后金额</returns>
        private static long toGold(decimal price)
        {
            return (long)(100 * price) * 10;
        }

        /// <summary>
        /// 获取自提点售后待处理和已处理的自提的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <param name="state">0：待处理，1：已处理</param>
        /// <returns>自提点售后待处理和已处理的自提的订单信息</returns>
        public List<OrderListCDTO> GetSelfTakeOrderListAfterSalesExt(Guid userId, int pageIndex, int pageSize, string state)
        {
            List<OrderListCDTO> resultlist = new List<OrderListCDTO>();
            try
            {
                if (userId == Guid.Empty)
                    return resultlist;

                //根据管理员ID获取管理员信息
                var selfTakeStationIds = (from p in AppStsManager.ObjectSet()
                                          join r in AppSelfTakeStation.ObjectSet() on p.SelfTakeStationId equals r.Id
                                          where p.UserId == userId && p.IsDel == false && r.IsDel == false
                                          select p.SelfTakeStationId
                                   ).Distinct();
                //判断管理员信息是否存在
                if (!selfTakeStationIds.Any())
                {
                    LogHelper.Info(string.Format("抱歉，您暂时没有权限查看此信息。userId：{0}，pageIndex：{1}，pageSize：{2}，state：{3}", userId, pageIndex, pageSize, state));
                    return resultlist;
                }
                pageSize = pageSize == 0 ? 10 : pageSize;

                var orderQuery = (from r in selfTakeStationIds
                                  join t in AppOrderPickUp.ObjectSet() on r equals t.SelfTakeStationId
                                  join order in CommodityOrder.ObjectSet() on t.Id equals order.Id
                                  join os in CommodityOrderService.ObjectSet() on t.Id equals os.Id
                                  join refund in OrderRefundAfterSales.ObjectSet() on t.Id equals refund.OrderId
                                  where refund.State != 3 && order.IsDel != 2 && order.IsDel != 3
                                  select
                                      new
                                          {
                                              CommodityOrderId = order.Id,
                                              Price = (decimal)order.RealPrice,
                                              AppId = order.AppId,
                                              UserId = order.UserId,
                                              State = order.State,
                                              Freight = order.Freight,
                                              IsModifiedPrice = order.IsModifiedPrice,
                                              OriginPrice = order.Price + order.Freight,
                                              PayType = order.Payment,
                                              SelfTakeFlag = order.SelfTakeFlag,
                                              StateAfterSales = refund.State,
                                              refund.SubTime
                                          });
                if (state == "0")
                {
                    orderQuery = orderQuery.Where(c => c.StateAfterSales == 0);
                }
                else
                {
                    orderQuery = orderQuery.Where(c => c.StateAfterSales != 0);
                }

                var commodityorderList = orderQuery.Distinct().OrderByDescending(c => c.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (!commodityorderList.Any())
                    return resultlist;

                Dictionary<Guid, string> listApp = null;
                //查询订单列表的所有自提订单商品，并以订单id分组
                var commodityOrderIds = commodityorderList.Select(n => n.CommodityOrderId);
                var commoditySDTOList = (from o in OrderItem.ObjectSet()
                                         where commodityOrderIds.Contains(o.CommodityOrderId)
                                         select new OrderListItemCDTO
                                         {
                                             Id = o.Id,
                                             OrderId = o.CommodityOrderId,
                                             Pic = o.PicturesPath,
                                             Name = o.Name,
                                             Price = o.CurrentPrice,
                                             CommodityNumber = o.Number,
                                             Size = o.CommodityAttributes,
                                             HasReview = o.AlreadyReview,
                                             Intensity = (decimal)o.Intensity,
                                             DiscountPrice = (decimal)(o.DiscountPrice != null ? o.DiscountPrice : -1),
                                             CommodityId = o.CommodityId
                                         });
                Dictionary<Guid, IEnumerable<OrderListItemCDTO>> csdtoList = commoditySDTOList
                    .GroupBy(c => c.OrderId, (key, group) => new { OrderId = key, CommodityList = group })
                    .ToDictionary(c => c.OrderId, c => c.CommodityList);
                //获取APP名称通过接口或Redis获取
                var listAppIds = (from co in commodityorderList select co.AppId).Distinct().ToList();
                listApp = APPSV.GetAppNameListByIds(listAppIds);

                foreach (var commodityOrder in commodityorderList)
                {
                    OrderListCDTO listItem = new OrderListCDTO
                    {
                        CommodityOrderId = commodityOrder.CommodityOrderId,
                        Price = commodityOrder.Price,
                        AppId = commodityOrder.AppId,
                        UserId = commodityOrder.UserId,
                        State = commodityOrder.State,
                        Freight = commodityOrder.Freight,
                        IsModifiedPrice = commodityOrder.IsModifiedPrice,
                        OriginPrice = commodityOrder.OriginPrice,
                        PayType = commodityOrder.PayType,
                        SelfTakeFlag = commodityOrder.SelfTakeFlag,
                        StateAfterSales = commodityOrder.StateAfterSales
                    };

                    if (csdtoList.ContainsKey(listItem.CommodityOrderId))
                    {
                        var commodityDTOList = csdtoList[listItem.CommodityOrderId];
                        listItem.ShoppingCartItemSDTO = commodityDTOList.ToList();
                    }
                    if (listApp != null && listApp.Count > 0 && listApp.ContainsKey(listItem.AppId))
                    {
                        var appNameDto = listApp[listItem.AppId];
                        if (!String.IsNullOrEmpty(appNameDto))
                        {
                            //App名称赋值
                            listItem.AppName = appNameDto;
                        }
                    }
                    resultlist.Add(listItem);
                }

                return resultlist;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderAfterSalesSV.GetSelfTakeOrderListAfterSalesExt获取自提点管理员下所有退款/退货申请订单。userId：{0}，pageIndex：{1}，pageSize：{2}，state：{3}", userId, pageIndex, pageSize, state), ex);
                return resultlist;
            }
        }

        /// <summary>
        /// 售后自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>售后自提订单数量</returns>
        public ResultDTO<int> GetSelfTakeManagerAfterSalesExt(Guid userId)
        {
            try
            {
                // 返回 是否管理员，待自提订单数量
                if (userId == Guid.Empty)
                    return new ResultDTO<int> { Data = 0, ResultCode = 1, Message = "管理员用户ID非法." };
                //根据自提订单管理员ID获取管理员信息
                var selfTakeStationIds = (from p in AppStsManager.ObjectSet()
                                          join r in AppSelfTakeStation.ObjectSet() on p.SelfTakeStationId equals r.Id
                                          where p.UserId == userId && p.IsDel == false && r.IsDel == false
                                          select p.SelfTakeStationId
                                   ).Distinct();
                if (!selfTakeStationIds.Any())
                {
                    LogHelper.Info(string.Format("该用户不是自提点管理员或没有与自提点绑定，userId：{0}", userId));
                    return new ResultDTO<int> { Data = 0, ResultCode = -1, Message = "抱歉，您暂时没有权限查看此信息" };
                }

                //获取商品订单数量信息   --未处理完的订单不能删除，AppOrderPickUp订单中有数据代表为自提订单，本查询不需要查询订单表
                var commodityorderListCount = (from r in selfTakeStationIds
                                               join t in AppOrderPickUp.ObjectSet() on r equals t.SelfTakeStationId
                                               join refund in OrderRefundAfterSales.ObjectSet() on t.Id equals refund.OrderId
                                               where refund.State == 0
                                               select t.Id).Count();
                //待处理订单数量赋值
                if (commodityorderListCount != 0)
                {
                    return new ResultDTO<int> { Data = commodityorderListCount, ResultCode = 0, Message = "sucess" };
                }
                return new ResultDTO<int> { Data = 0, ResultCode = -2, Message = "订单数量为0" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderAfterSalesSV.GetSelfTakeManagerAfterSalesExt获取待自提订单数量异常。userId：{0}", userId), ex);
                return new ResultDTO<int> { Data = 0, ResultCode = -3, Message = "Exception" };
            }
        }

        /// <summary>
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirmAfterSalesExt()
        {
            LogHelper.Info(string.Format("售后发货后9天确认收货开始...."));
            //处理订单状态为确认收货
            try
            {
                List<int> directArrivalPayments = new PaySourceSV().GetDirectArrivalPaymentExt();
                List<int> secTranPayments = new PaySourceSV().GetSecuriedTransactionPaymentExt();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                DateTime now = DateTime.Now;
                //查询超过12天未确认收货的订单
                DateTime lastday = now.AddDays(-9);
                DateTime threelastday = now.AddDays(-12);
                //获取卖家已发货商品信息订单 
                var jdOrderRefundAfterSalesQuery = JdOrderRefundAfterSales.ObjectSet();
                var ordersQuery = (from p in OrderRefundAfterSales.ObjectSet()
                                   join q in CommodityOrderService.ObjectSet() on p.OrderId equals q.Id
                                   join c in CommodityOrder.ObjectSet() on p.OrderId equals c.Id
                                   where p.State == 11 && q.SelfTakeFlag == 0 && p.RefundExpOrderTime < (q.IsDelayConfirmTimeAfterSales ? threelastday : lastday)
                                       // 过滤京东订单
                                    && !jdOrderRefundAfterSalesQuery.Any(j => j.OrderRefundAfterSalesId == p.Id)
                                   select new
                                   {
                                       orderRefundAfterSales = p,
                                       commodityOrderService = q,
                                       commodityOrder = c
                                   }
                              );

                if (!CustomConfig.IsSystemDirectRefund)
                {
                    ordersQuery = ordersQuery.Where(c => !directArrivalPayments.Contains(c.commodityOrder.Payment));
                }
                var orders = ordersQuery.Distinct().ToList();

                LogHelper.Info(string.Format("自动确认支付服务处理订单数:{0}", orders.Count));

                if (orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        CommodityOrder commodityOrder = order.commodityOrder;

                        int oldState = order.commodityOrderService.State;
                        //金币支付，
                        //1
                        var refundResult = UpdateOrderStateTo7(order.commodityOrder, order.commodityOrderService, order.orderRefundAfterSales, contextSession);
                        if (refundResult.ResultCode != 0)
                            continue;

                        //订单日志
                        Journal journal = new Journal();
                        journal.Id = Guid.NewGuid();
                        journal.Name = "售后系统9天后卖家自动确认收货";
                        journal.Code = order.commodityOrderService.Code;
                        journal.SubId = commodityOrder.UserId;
                        journal.SubTime = DateTime.Now;
                        journal.Details = "售后订单状态由" + oldState + "变为" + order.commodityOrderService.State;
                        journal.CommodityOrderId = order.commodityOrderService.Id;
                        journal.EntityState = System.Data.EntityState.Added;
                        journal.StateFrom = oldState;
                        journal.StateTo = order.commodityOrderService.State;
                        journal.IsPush = false;
                        journal.OrderType = commodityOrder.OrderType;

                        journal.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(journal);
                        contextSession.SaveChanges();

                        //消息
                        BTPMessageSV addmassage = new BTPMessageSV();

                        AfterSalesMessages messageModel = new AfterSalesMessages();
                        messageModel.IsAuto = false;
                        messageModel.Id = commodityOrder.Id.ToString();
                        messageModel.UserIds = commodityOrder.UserId.ToString();
                        messageModel.AppId = commodityOrder.AppId;
                        messageModel.Code = commodityOrder.Code;
                        messageModel.State = commodityOrder.State;
                        messageModel.RefundType = order.orderRefundAfterSales.RefundType;
                        messageModel.RefundMoney = order.orderRefundAfterSales.RefundMoney + order.orderRefundAfterSales.RefundScoreMoney;
                        messageModel.PayType = commodityOrder.Payment;
                        messageModel.orderRefundAfterSalesState = order.orderRefundAfterSales.State;
                        messageModel.oldOrderRefundAfterSalesState = 11;
                        messageModel.EsAppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId;
                        addmassage.AddMessagesAfterSales(messageModel);

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("售后9天卖家自动确认支付服务异常。", ex);
            }
        }

        #endregion

        #region
        /// <summary>
        /// 退款的时候 处理众筹相关的业务
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="orRefundMoney"></param>
        /// <param name="now"></param>
        public long CancelCrowdfundingExt(ContextSession contextSession, CommodityOrder commodityOrder, decimal orRefundMoney, DateTime now, bool isSaveObject = true)
        {
            long gold = 0;
            //众筹股东表
            var UserCrowdfundingQuery = UserCrowdfunding.ObjectSet().FirstOrDefault(q => q.UserId == commodityOrder.UserId && q.AppId == commodityOrder.AppId);
            //众筹成功
            if (commodityOrder.IsCrowdfunding == 2)
            {
                //不是众筹
                if (UserCrowdfundingQuery == null)
                    return gold;
                //众筹成功，且当前订单
                UserCrowdfundingQuery.OrdersMoney -= orRefundMoney;
                //全额退款，减去订单
                if (commodityOrder.IsModifiedPrice)
                {
                    //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                    if (orRefundMoney == commodityOrder.RealPrice)
                    {
                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                }
                else
                {
                    //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                    if (orRefundMoney >= commodityOrder.Price)
                    {

                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                }
                UserCrowdfundingQuery.EntityState = EntityState.Modified;
                contextSession.SaveObject(UserCrowdfundingQuery);
                return gold;
            }

            //众筹基本表
            var CrowdfundingQuery = Crowdfunding.ObjectSet().FirstOrDefault(q => q.AppId == commodityOrder.AppId && q.StartTime < now);
            if (CrowdfundingQuery != null)
            {
                //众筹计数表 
                var CrowdfundingCountQuery = CrowdfundingCount.ObjectSet().FirstOrDefault(q => q.CrowdfundingId == CrowdfundingQuery.Id);
                //众筹股东表

                decimal corRealPrice = 0;
                decimal fenRealPrice = 0;


                decimal realPrice = commodityOrder.IsModifiedPrice
                                        ? commodityOrder.RealPrice.Value
                                        : commodityOrder.Price;

                if (commodityOrder.IsModifiedPrice)
                {
                    corRealPrice = orRefundMoney;
                    //如果退款金额等于订单金额 相当于全额退款 订单数量减1
                    if (orRefundMoney == commodityOrder.RealPrice)
                    {
                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                    else
                    {
                        //订单金额-退款 剩余部分 计算分红
                        fenRealPrice = commodityOrder.RealPrice.Value - orRefundMoney;
                    }
                }
                else
                {
                    //如果退款的金额大于 不计算运费的金额的时候 按不算运费的金额计算 相当于全额退款 订单数量减1
                    if (orRefundMoney >= commodityOrder.Price)
                    {
                        corRealPrice = commodityOrder.Price;
                        UserCrowdfundingQuery.OrderCount = UserCrowdfundingQuery.OrderCount - 1;
                    }
                    else
                    {
                        //订单金额-退款 剩余部分 计算分红
                        fenRealPrice = commodityOrder.Price - orRefundMoney;
                        corRealPrice = orRefundMoney;
                    }
                }
                //解决众筹临界点订单退款问题，如果退款后众筹有效金额仍未改变，则不改变股东表信息
                decimal notCfMoney = realPrice - corRealPrice - commodityOrder.CrowdfundingPrice;
                if (notCfMoney < 0)
                {
                    var notCfMoneyless = -notCfMoney;
                    decimal afterMoney = UserCrowdfundingQuery.Money - notCfMoneyless;
                    long afterShareCnt = (long)(afterMoney / CrowdfundingQuery.PerShareMoney);
                    //用户减少的股数
                    long result = UserCrowdfundingQuery.CurrentShareCount - afterShareCnt;
                    if (result > 0)
                    {
                        // 如果众筹已经成功 变成进行中
                        if (CrowdfundingQuery.State == 1)
                        {
                            CrowdfundingQuery.State = 0;
                            CrowdfundingQuery.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(CrowdfundingQuery);
                        }
                        CrowdfundingCountQuery.CurrentShareCount = CrowdfundingCountQuery.CurrentShareCount - result;
                        CrowdfundingCountQuery.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(CrowdfundingCountQuery);
                    }
                    else
                    {
                        result = 0;
                    }

                    UserCrowdfundingQuery.Money = afterMoney;
                    UserCrowdfundingQuery.CurrentShareCount -= result;
                    UserCrowdfundingQuery.EntityState = System.Data.EntityState.Modified;



                    if (commodityOrder.CrowdfundingPrice - orRefundMoney < 0)
                    {
                        commodityOrder.CrowdfundingPrice = 0;
                    }
                    else
                    {
                        commodityOrder.CrowdfundingPrice = commodityOrder.CrowdfundingPrice - orRefundMoney;
                    }
                }
                UserCrowdfundingQuery.OrdersMoney -= corRealPrice;
                contextSession.SaveObject(UserCrowdfundingQuery);


                if (fenRealPrice > 0)
                {
                    //众筹股东每日统计
                    var yestorday = DateTime.Today.AddDays(-1);
                    var UserCrowdfundingDailyQuery = UserCrowdfundingDaily.ObjectSet().Where(q => q.AppId == commodityOrder.AppId && q.SettlementDate == yestorday).FirstOrDefault();

                    if (UserCrowdfundingDailyQuery != null)
                    {
                        gold = (long)(UserCrowdfundingDailyQuery.ShareCount * CrowdfundingQuery.DividendPercent * fenRealPrice * 1000);
                        if (gold > 0)
                        {
                            CfOrderDividend cfOrderDividend = CfOrderDividend.CreateCfOrderDividend();
                            cfOrderDividend.Gold = gold;
                            cfOrderDividend.AppId = commodityOrder.AppId;
                            cfOrderDividend.State = 0;
                            cfOrderDividend.CurrentShareCount = UserCrowdfundingDailyQuery.ShareCount;
                            cfOrderDividend.CommodityOrderId = commodityOrder.Id;
                            contextSession.SaveObject(cfOrderDividend);
                        }

                    }
                }
            }
            return gold;
        }

        /// <summary>
        /// 计算订单分润异常，如果没有异常返回null
        /// </summary>
        /// <param name="commodityOrder">订单实体</param>
        /// <param name="appName">应用名称</param>
        /// <param name="exTime">统计时间</param>
        /// <param name="shareMoney">订单需要分成金额</param>
        /// <param name="clearingPrice">应用主分成金额</param>
        /// <param name="orderService">订单售后表</param>
        /// <param name="isNeedCalcShare">是否需要计算订单分成（如果传如true则本方法会重新计算shareMoney，否则使用传入shareMoney字段）</param>
        /// <returns></returns>
        private CommodityOrderException buildOrderException(CommodityOrder commodityOrder, string appName, DateTime exTime, decimal shareMoney, out decimal clearingPrice, CommodityOrderService orderService, bool isNeedCalcShare = false)
        {
            CommodityOrderException result = null;
            clearingPrice = 0.0m;
            decimal orderRealPrice = commodityOrder.RealPrice.Value;
            try
            {
                var orderTime = commodityOrder.PaymentTime.HasValue ? commodityOrder.PaymentTime.Value : commodityOrder.SubTime;
                bool hasException = false;
                int exceptionType = 0;
                string message = "";

                var orderItems = OrderItem.ObjectSet().Where(c => c.CommodityOrderId == commodityOrder.Id).ToList();
                var comIds = orderItems.Select(c => c.CommodityId).ToList();
                var dict = SettlingAccount.ObjectSet()
                                          .Where(c => comIds.Contains(c.CommodityId) && c.EffectiveTime <= orderTime)
                                          .GroupBy(c => c.CommodityId)
                                          .ToDictionary(c => c.Key, c => c.OrderByDescending(m => m.EffectiveTime).First());
                //校验商家结算价是否存在
                foreach (var orderItem in orderItems)
                {
                    if (!dict.ContainsKey(orderItem.CommodityId))
                    {
                        hasException = true;
                        exceptionType = 0;
                        message += "订单商品\"" + orderItem.Name + "\"未设置结算价\r\n";
                        continue;
                    }
                    clearingPrice += Math.Round(dict[orderItem.CommodityId].ManufacturerClearingPrice * orderItem.Number, 2);
                }
                //厂家结算价=设置的厂家结算金额+运费-优惠券-部分退款金额
                //订单总价-分销-商家=金和
                if (!hasException)
                {

                    //运费金额给商家，订单改价除外
                    if (!commodityOrder.IsModifiedPrice)
                        clearingPrice += commodityOrder.Freight;
                    //优惠券减免金额商家承担
                    var couponAmount = 0.0m;
                    var coupons = OrderPayDetail.ObjectSet()
                                                .Where(c => c.OrderId == commodityOrder.Id && c.ObjectType == 1 && c.UseType == 0)
                                                .Select(c => c.Amount)
                                                .ToList();
                    if (coupons.Any())
                        couponAmount = coupons.Sum();
                    clearingPrice -= couponAmount;
                    if (clearingPrice < 0)
                        clearingPrice = 0;

                    bool isRefund = false;
                    //退款处理
                    if (new List<int> { 5, 7, 10, 12 }.Contains(orderService.State))
                    {
                        var orderRefund = OrderRefundAfterSales.ObjectSet().FirstOrDefault(c => c.OrderId == commodityOrder.Id && c.State != 2 && c.State != 3 && c.State != 4 && c.State != 13);
                        if (orderRefund != null)
                        {
                            clearingPrice -= orderRefund.RefundMoney;
                            orderRealPrice -= orderRefund.RefundMoney;
                            if (clearingPrice < 0)
                                clearingPrice = 0;
                            isRefund = true;
                        }
                    }

                    if (clearingPrice <= orderRealPrice)
                    {
                        //退款不计算分成
                        if (isRefund)
                            return null;

                        //订单参与分成金额
                        var shareRealPrice = commodityOrder.IsModifiedPrice ? commodityOrder.RealPrice : commodityOrder.Price;

                        #region 计算分成金额
                        if (isNeedCalcShare)
                        {
                            shareMoney = 0.0m;
                            //众销
                            if (commodityOrder.SrcType == 33 || commodityOrder.SrcType == 34)
                            {
                                decimal commission = ((long)(shareRealPrice * CustomConfig.SaleShare.Commission * 100)) / 100.0m;
                                shareMoney += commission;
                            }
                            if (commodityOrder.SrcAppId.HasValue && commodityOrder.SrcAppId != Guid.Empty)
                            {
                                decimal ownerShare = ((long)(shareRealPrice * CustomConfig.ShareOwner.OwnerPercent * 100)) / 100.0m;
                                shareMoney += ownerShare;
                            }
                            else
                            {
                                decimal spreadShare = ((long)(shareRealPrice * CustomConfig.SpreaderAccount.SpreaderPercent * 100)) /
                                                      100.0m;
                                shareMoney += spreadShare;
                            }
                            //TODO 众筹已取消，这里就不计算了
                        }
                        #endregion
                        //比较 分成金额+结算价是否小于等于订单总价，如果不是，结算价异常
                        if (clearingPrice + shareMoney > orderRealPrice)
                        {
                            hasException = true;
                            exceptionType = 2;
                            message += string.Format("订单分成金额不足，结算价为:￥{0}，订单总价为：￥{1}，需要分成金额为：￥{2}\r\n", clearingPrice, orderRealPrice, shareMoney);
                        }
                    }
                    //订单金额不够给厂家结算
                    else
                    {
                        hasException = true;
                        exceptionType = 1;
                        message += string.Format("结算价大于订单金额，结算价为:￥{0}，订单总价为：￥{1}\r\n", clearingPrice, orderRealPrice);
                    }
                }

                if (hasException)
                {
                    result = CommodityOrderException.CreateCommodityOrderException();
                    result.OrderId = commodityOrder.Id;
                    result.OrderCode = commodityOrder.Code;
                    result.OrderRealPrice = commodityOrder.RealPrice;
                    result.ClearingPrice = exceptionType == 0 ? null : (decimal?)clearingPrice; ;
                    result.ExceptionType = exceptionType;
                    result.ExceptionReason = message;
                    result.AppId = commodityOrder.AppId;
                    result.AppName = appName;
                    result.ExceptionTime = exTime;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommodityOrderSV.buildOrderException异常，订单id：{0}", commodityOrder.Id), ex);
                result = null;
            }
            return result;
        }

        #endregion


        private ResultDTO UpdateOrderStateTo7(CommodityOrder commodityOrder, CommodityOrderService commodityOrderService, OrderRefundAfterSales orderRefundAfterSales, ContextSession contextSession)
        {
            ResultDTO result = new ResultDTO();
            var tradeType = PaySource.GetTradeType(commodityOrder.Payment);
            if (tradeType == TradeTypeEnum.Hdfk)
            {
                LogHelper.Error(string.Format("此订单不能进行售后退款。commodityOrderId：{0}", commodityOrderService.Id));
                result.ResultCode = 1;
                result.Message = "此订单不能进行售后退款";
                return result;
            }
            if (orderRefundAfterSales.RefundMoney <= 0 || (tradeType == TradeTypeEnum.Direct && !CustomConfig.IsSystemDirectRefund))
            {
                commodityOrderService.State = 7;
                orderRefundAfterSales.State = 1;
            }
            decimal yjbprice = 0;
            var yjbresult = YJBSV.GetOrderYJBInfo(commodityOrder.EsAppId.Value, commodityOrder.Id);
            if (yjbresult.Data != null)
            {
                yjbprice = yjbresult.Data.InsteadCashAmount;
            }
            var cashmoney = commodityOrder.RealPrice.Value - commodityOrder.Freight;
            //退款金额
            decimal refundmoney = 0;
            if (orderRefundAfterSales.OrderRefundMoneyAndCoupun == null)
            {//老的退款数据
                refundmoney = orderRefundAfterSales.RefundMoney > cashmoney ? cashmoney : orderRefundAfterSales.RefundMoney;
            }
            else
            {
                refundmoney = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0) > cashmoney ? cashmoney : (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);
            }
            //decimal coupon_price = 0;
            //var user_yjcoupon = YJBSV.GetUserYJCouponByOrderId(commodityOrder.Id);
            //if (user_yjcoupon.Data != null)
            //{
            //    coupon_price = user_yjcoupon.Data.UseAmount.Value;
            //}
            //refundmoney = orderRefundAfterSales.RefundMoney - coupon_price;//退金币时候去掉抵用券金额
            if (refundmoney > 0 && (tradeType == TradeTypeEnum.SecTrans || tradeType == TradeTypeEnum.Direct && CustomConfig.IsSystemDirectRefund))
            {
                //解冻金币
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO applicationDTO = APPSV.Instance.GetAppOwnerInfo(commodityOrder.AppId);
                if (tradeType == TradeTypeEnum.SecTrans && commodityOrder.GoldPrice > 0)
                {
                    var unFreezeResult = Jinher.AMP.BTP.TPS.FSPSV.Instance.ConfirmPayUnFreeze(buildConfirmPayUnFreezeDTO(null, commodityOrder, applicationDTO));
                    if (unFreezeResult == null)
                    {
                        result.ResultCode = 1;
                        result.Message = "解冻金币失败";
                        return result;
                    }
                    if (unFreezeResult.Code != 0)
                    {
                        result.ResultCode = 1;
                        result.Message = string.Format("解冻金币失败  {0}", unFreezeResult.Message);
                        return result;
                    }
                }
                var cancelPayResult = Jinher.AMP.BTP.TPS.FSPSV.CancelPay(OrderSV.BuildCancelPayDTOAfterSales(commodityOrder, refundmoney, contextSession, applicationDTO, commodityOrderService), tradeType);
                if (cancelPayResult == null)
                {
                    result.ResultCode = 1;
                    result.Message = "退款失败";
                    return result;
                }
                if (cancelPayResult.Code != 0)
                {
                    result.ResultCode = 1;
                    result.Message = string.Format("退款失败  {0}", cancelPayResult.Message);
                    return result;
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
                        result.ResultCode = 1;
                        result.Message = string.Format("退款失败  {0}", cancelPayResult.Message);
                        return result;
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
                LogHelper.Info(string.Format("售后退款返回值：{0},Message:{1},order.commodityOrderId:{2}", cancelPayResult.Code, cancelPayResult.Message, commodityOrderService.Id));
            }

            // 回退积分
            SignSV.CommodityOrderAfterSalesRefundScore(contextSession, commodityOrder, orderRefundAfterSales);

            // 回退易捷币
            //Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales);

            #region 回退易捷币和易捷抵用券
            decimal couponprice = 0;
            decimal couponmoney = 0;//抵用券使用总金额
            //decimal totalcouprice = 0;//保存抵用券之和(当2个或者2个以上的抵用券相加的时候)
            //decimal pretotalcouprice = 0;//保存上一个抵用券相加的价格(当2个或者2个以上的抵用券相加的时候)
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
                var refundmoney1 = (orderRefundAfterSales.OrderRefundMoneyAndCoupun ?? 0);
                foreach (var item in useryjcoupon.Data)
                {
                    couponmoney += item.UseAmount ?? 0;
                }
                if (refundmoney == couponmoney + refundmoney)
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
                                //pretotalcouprice = couponprice;
                            }
                            else
                            {//易捷币不能循环退
                                orderRefundAfterSales.RefundYJBMoney = 0;
                            }
                            decimal coupon = 0;
                            if (refundmoney1 - refundmoney > 0)
                            {
                                if (refundmoney1 - refundmoney - couponprice >= 0)
                                {//退款金额大于等于（实际支付金额+抵用券金额），直接返回抵用券面值
                                 //if (refundmoney - cashmoney - couponprice - orderRefundAfterSales.RefundYJBMoney < 0)
                                 //{//返还部分易捷币
                                 //    orderRefundAfterSales.RefundYJBMoney = orderRefundAfterSales.RefundYJBMoney - (refundmoney + cashmoney + couponprice);
                                 //}
                                    coupon = couponprice;
                                }
                                else
                                {//否则，表示不是全额退款，返还部分抵用券，易捷币返还0
                                    coupon = refundmoney1 - refundmoney;
                                    orderRefundAfterSales.RefundYJBMoney = 0;
                                }
                            }
                            Jinher.AMP.BTP.TPS.Helper.YJBHelper.OrderAfterSalesRefund(contextSession, commodityOrder, orderRefundAfterSales, coupon, useryjcoupon.Data[i].Id);
                            refundmoney1 -= coupon;
                            //pretotalcouprice = totalcouprice;
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
            SettleAccountHelper.OrderRefund(contextSession, commodityOrder, orderRefundAfterSales);

            commodityOrderService.ModifiedOn = DateTime.Now;
            commodityOrderService.EntityState = System.Data.EntityState.Modified;

            orderRefundAfterSales.ModifiedOn = DateTime.Now;
            orderRefundAfterSales.EntityState = System.Data.EntityState.Modified;

            //更新订单的修改时间                           
            commodityOrder.ModifiedOn = DateTime.Now;
            commodityOrder.EntityState = System.Data.EntityState.Modified;

            return result;
        }
    }
}
