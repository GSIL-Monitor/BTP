using System;
using System.Collections.Generic;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.YJB.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Linq;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 易捷币相关信息
    /// </summary>
    public class YJBHelper
    {
        /// <summary>
        /// 查询商品的易捷币抵用信息
        /// </summary>
        public static OrderInsteadCashDTO GetCommodityCashPercentWithoutUser(Guid? esAppId, List<OrderInsteadCashInputCommodityDTO> commodities)
        {
            if (!esAppId.HasValue || esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return new OrderInsteadCashDTO { Enabled = false };
            }
            return YJBSV.GetCommodityCashPercentWithoutUser(commodities);
        }

        /// <summary>
        /// 查询商品的易捷币抵用信息
        /// </summary>
        public static CanInsteadCashDTO GetCommodityCashPercent(Guid? esAppId, OrderInsteadCashInputDTO input)
        {
            var emptyResult = new CanInsteadCashDTO { YJBInfo = new OrderInsteadCashDTO { Enabled = false }, YJCouponInfo = null };
            if (!esAppId.HasValue)
            {
                return emptyResult;
            }
            if (esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return emptyResult;
            }
            return YJBSV.GetCommodityCashPercent(input);
        }

        /// <summary>
        ///  创建订单时，扣除用户易捷币和易捷抵现劵
        /// </summary>
        public static ResultDTO CreateOrder(Guid esAppId, CreateOrderInputDTO input)
        {
            if (esAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return ResultDTO.Successed;
            return YJBSV.CreateOrderJournal(input);
        }

        /// <summary>
        ///  支付成功时时，解冻用户易捷币和易捷抵现劵
        /// </summary>
        public static ResultDTO PayOrder(CommodityOrder order)
        {
            LogHelper.Info("YJBHelper.PayOrder, OrderId: " + order.Id);
            if (order.EsAppId.HasValue && order.EsAppId.Value == YJBConsts.YJAppId)
            {
                return YJBSV.PayOrderJournal(order);
            }
            return ResultDTO.Successed;
        }

        /// <summary>
        ///  创建订单失败时，回退用户易捷币
        /// </summary>
        public static ResultDTO CreateOrderFail(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            if (commodityOrder.EsAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return ResultDTO.Successed;
            var result = YJBSV.CancelOrderJournal(commodityOrder.Id);
            if (!result.IsSuccess)
            {
                LogHelper.Error("创建订单失败时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
                LogOrderErrorInfo(contextSession, commodityOrder, "创建订单失败时，回退用户易捷币");
            }
            return result;
        }

        /// <summary>
        ///  取消订单时，回退用户易捷币
        /// </summary>
        public static ResultDTO CancelOrder(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            if (commodityOrder.EsAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId)
            {
                return ResultDTO.Successed;
            }

            decimal yjbMoney = OrderPayDetail.ObjectSet()
                .Where(t => t.OrderId == commodityOrder.Id && (t.ObjectType == 10 || t.ObjectType == 15) && t.Amount > 0)
                .Select(t => t.Amount).FirstOrDefault();
            if (yjbMoney <= 0)
            {
                return ResultDTO.Successed;
            } 

            var result = YJBSV.RefundAllOrderJournal(commodityOrder.Id);
            if (!result.IsSuccess)
            {
                if (result.Code != "OrderNotFound")
                {
                    LogHelper.Error("取消订单时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
                    LogOrderErrorInfo(contextSession, commodityOrder, "取消订单时，回退用户易捷币失败");
                }
            }
            return result;
        }
        /// <summary>
        /// 退款时把金额退回到易捷卡
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Refund"></param>
        /// <returns></returns>
        public static ResultDTO UpdateCardCash(Guid UserId, Decimal Refund)
        {
            var result = YJBSV.UpdateCardCash(UserId, Refund);
            if (!result.IsSuccess)
            {
                LogHelper.Error("退款时，回退用户易捷卡失败，用户ID：" + UserId + "，错误内容：" + result.Message);
            }
            return result;
        }
        /// <summary>
        /// 退回易捷卡
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Refund"></param>
        /// <returns></returns>
        public static ResultDTO RetreatYjc(Guid UserId, Decimal Refund, Guid orderId, Guid orderItemId)
        {
            var result = YJBSV.RetreatYjc(UserId, Refund, orderId, orderItemId);
            if (!result.IsSuccess)
            {
                LogHelper.Error("退款时，回退用户易捷卡失败，用户ID：" + UserId + "，错误内容：" + result.Message);
            }
            return result;
        }
        /// <summary>
        /// 根据卡绑定人获取易捷卡面值
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static decimal GetYJCardAcount(Guid UserId)
        {
            var result = YJBSV.GetYJCardAcount(UserId);
            if (result == 0)
            {
                LogHelper.Error("退款时，回退用户易捷卡失败，用户ID：" + UserId);
            }
            return result;
        }

        /// <summary>
        /// 售中退款时，回退用户易捷币和易捷抵用券
        /// </summary>
        public static ResultDTO OrderRefund(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefund orderRefund, decimal commodityPrice, Guid useryjcouponid)
        {
            if (commodityOrder.EsAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return ResultDTO.Successed;

            //var result = YJBSV.RefundAllOrderJournal(commodityOrder.Id);
            //if (!result.IsSuccess)
            //{
            //    LogHelper.Error("售中退款时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
            //    LogOrderErrorInfo(contextSession, commodityOrder, "售中退款时，回退用回退用户易捷币失败户易捷币");
            //}
            //return result;
            #region 记录抵用券退款明细表
            var orderitemorder = (from i in OrderItem.ObjectSet()
                                  where commodityOrder.Id == i.CommodityOrderId
                                  select i).ToList();
            var user = CBCSV.GetUserNameAndCode(commodityOrder.UserId);
            foreach (var item in orderitemorder)
            {
                CouponRefundDetail couponRefundDetailDTO = new CouponRefundDetail();
                couponRefundDetailDTO.Id = Guid.NewGuid();
                couponRefundDetailDTO.SubTime = DateTime.Now;
                couponRefundDetailDTO.ModifiedOn = DateTime.Now;
                couponRefundDetailDTO.RefundTime = DateTime.Now;
                couponRefundDetailDTO.ReceiveAccount = user != null ? user.Item2 : "";
                couponRefundDetailDTO.ReceiveName = user != null ? user.Item1 : "";
                couponRefundDetailDTO.CommodityCouponMoney = commodityPrice;
                couponRefundDetailDTO.FreightCouponMoney = 0;
                couponRefundDetailDTO.CommodityRefundMoney = orderRefund.RefundMoney;
                couponRefundDetailDTO.FreightRefundMoney = orderRefund.RefundFreightPrice;
                couponRefundDetailDTO.RefundTotalMoney = commodityPrice + orderRefund.RefundMoney + orderRefund.RefundFreightPrice;
                couponRefundDetailDTO.ShopName = commodityOrder.AppName;
                couponRefundDetailDTO.OrderNo = commodityOrder.Code;
                couponRefundDetailDTO.CommoidtyName = item.Name;
                couponRefundDetailDTO.ReceivePhone = commodityOrder.ReceiptPhone;
                couponRefundDetailDTO.ConsigneeName = commodityOrder.ReceiptUserName;
                couponRefundDetailDTO.Remark = "";
                couponRefundDetailDTO.EntityState = System.Data.EntityState.Added;

                contextSession.SaveObject(couponRefundDetailDTO);
            }
            contextSession.SaveChanges();
            #endregion
            var mobile = user != null ? user.Item2 : "";
            var result = YJBSV.RefundOrderJournal(commodityOrder.Id, orderRefund.RefundYJBMoney, commodityPrice, mobile, commodityOrder.UserId, useryjcouponid);
            if (!result.IsSuccess)
            {
                LogHelper.Error("售中退款时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
                LogOrderErrorInfo(contextSession, commodityOrder, "售中退款时，回退用回退用户易捷币失败户易捷币");
            }
            return result;
        }

        /// <summary>
        /// 售中退款时，回退用户易捷币和易捷抵用券 单品
        /// </summary>
        public static ResultDTO OrderItemRefund(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefund orderRefund, Guid comodityId, Guid orderItemId, decimal commodityPrice, Guid useryjcouponid)
        {
            if (commodityOrder.EsAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && commodityOrder.EsAppId != new Guid("1375ad99-de3b-4e93-80d5-5b96e1588967")) return ResultDTO.Successed;

            #region 记录抵用券退款明细表
            var user = CBCSV.GetUserNameAndCode(commodityOrder.UserId);
            if (commodityPrice > 0)
            {
                var orderitemorder = (from i in OrderItem.ObjectSet()
                                      where i.Id == orderItemId
                                      select i).FirstOrDefault();
                CouponRefundDetail couponRefundDetailDTO = new CouponRefundDetail();
                couponRefundDetailDTO.Id = Guid.NewGuid();
                couponRefundDetailDTO.SubTime = DateTime.Now;
                couponRefundDetailDTO.ModifiedOn = DateTime.Now;
                couponRefundDetailDTO.RefundTime = DateTime.Now;
                couponRefundDetailDTO.ReceiveAccount = user != null ? user.Item2 : "";
                couponRefundDetailDTO.ReceiveName = user != null ? user.Item1 : "";
                couponRefundDetailDTO.CommodityCouponMoney = commodityPrice;
                couponRefundDetailDTO.FreightCouponMoney = 0;
                couponRefundDetailDTO.CommodityRefundMoney = orderRefund.RefundMoney;
                couponRefundDetailDTO.FreightRefundMoney = orderRefund.RefundFreightPrice;
                couponRefundDetailDTO.RefundTotalMoney = commodityPrice + orderRefund.RefundMoney + orderRefund.RefundFreightPrice;
                couponRefundDetailDTO.ShopName = commodityOrder.AppName;
                couponRefundDetailDTO.OrderNo = commodityOrder.Code;
                couponRefundDetailDTO.CommoidtyName = orderitemorder.Name;
                couponRefundDetailDTO.ReceivePhone = commodityOrder.ReceiptPhone;
                couponRefundDetailDTO.ConsigneeName = commodityOrder.ReceiptUserName;
                couponRefundDetailDTO.Remark = "";
                couponRefundDetailDTO.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(couponRefundDetailDTO);
                contextSession.SaveChanges();
            }
            #endregion
            var mobile = user != null ? user.Item2 : "";
            var result = YJBSV.RefundOrderItemJournal(commodityOrder.Id, comodityId, orderRefund.RefundYJBMoney, orderItemId, commodityPrice, mobile, commodityOrder.UserId,useryjcouponid);
            if (!result.IsSuccess)
            {
                LogHelper.Error("售中退款时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
                LogOrderErrorInfo(contextSession, commodityOrder, "售中退款时，回退用回退用户易捷币失败户易捷币");
            }
            return result;
        }

        /// <summary>
        /// 售后退款时，回退用户易捷币和易捷抵用券
        /// </summary>
        public static ResultDTO OrderAfterSalesRefund(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefundAfterSales orderRefundAfterSales, decimal commodityPrice, Guid useryjcouponid)
        {
            if (commodityOrder.EsAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return ResultDTO.Successed;

            #region 记录抵用券退款明细表
            var user = CBCSV.GetUserNameAndCode(commodityOrder.UserId);
            if (commodityPrice > 0)
            {
                var orderitemorder = (from i in OrderItem.ObjectSet()
                                      where commodityOrder.Id == i.CommodityOrderId
                                      select i).ToList();
                foreach (var item in orderitemorder)
                {
                    CouponRefundDetail couponRefundDetailDTO = new CouponRefundDetail();
                    couponRefundDetailDTO.Id = Guid.NewGuid();
                    couponRefundDetailDTO.SubTime = DateTime.Now;
                    couponRefundDetailDTO.ModifiedOn = DateTime.Now;
                    couponRefundDetailDTO.RefundTime = DateTime.Now;
                    couponRefundDetailDTO.ReceiveAccount = user != null ? user.Item2 : "";
                    couponRefundDetailDTO.ReceiveName = user != null ? user.Item1 : "";
                    couponRefundDetailDTO.CommodityCouponMoney = commodityPrice;
                    couponRefundDetailDTO.FreightCouponMoney = 0;
                    couponRefundDetailDTO.CommodityRefundMoney = orderRefundAfterSales.RefundMoney;
                    couponRefundDetailDTO.FreightRefundMoney = orderRefundAfterSales.RefundFreightPrice;
                    couponRefundDetailDTO.RefundTotalMoney = commodityPrice + orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundFreightPrice;
                    couponRefundDetailDTO.ShopName = commodityOrder.AppName;
                    couponRefundDetailDTO.OrderNo = commodityOrder.Code;
                    couponRefundDetailDTO.CommoidtyName = item.Name;
                    couponRefundDetailDTO.ReceivePhone = commodityOrder.ReceiptPhone;
                    couponRefundDetailDTO.ConsigneeName = commodityOrder.ReceiptUserName;
                    couponRefundDetailDTO.Remark = "";
                    couponRefundDetailDTO.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(couponRefundDetailDTO);
                }
                contextSession.SaveChanges();
            }
            #endregion

            var mobile = user != null ? user.Item2 : "";
            var result = YJBSV.RefundOrderJournal(commodityOrder.Id, orderRefundAfterSales.RefundYJBMoney, commodityPrice, mobile, commodityOrder.UserId, useryjcouponid);
            if (!result.IsSuccess)
            {
                LogHelper.Error("售后退款时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
                LogOrderErrorInfo(contextSession, commodityOrder, "售后退款时，回退用户易捷币失败");
            }
            return result;
        }

        /// <summary>
        /// 售后退款时，回退用户易捷币和易捷抵用券 单品
        /// </summary>
        public static ResultDTO OrderItemAfterSalesRefund(ContextSession contextSession, CommodityOrder commodityOrder, OrderRefundAfterSales orderRefundAfterSales, decimal commodityPrice, Guid comodityId, Guid orderItemId, Guid useryjcouponid)
        {
            if (commodityOrder.EsAppId != Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId) return ResultDTO.Successed;

            #region 记录抵用券退款明细表
            var user = CBCSV.GetUserNameAndCode(commodityOrder.UserId);
            if (commodityPrice > 0)
            {
                var orderitemorder = (from i in OrderItem.ObjectSet()
                                      where i.Id == orderItemId
                                      select i).FirstOrDefault();
                CouponRefundDetail couponRefundDetailDTO = new CouponRefundDetail();
                couponRefundDetailDTO.Id = Guid.NewGuid();
                couponRefundDetailDTO.SubTime = DateTime.Now;
                couponRefundDetailDTO.ModifiedOn = DateTime.Now;
                couponRefundDetailDTO.RefundTime = DateTime.Now;
                couponRefundDetailDTO.ReceiveAccount = user != null ? user.Item2 : "";
                couponRefundDetailDTO.ReceiveName = user != null ? user.Item1 : "";
                couponRefundDetailDTO.CommodityCouponMoney = commodityPrice;
                couponRefundDetailDTO.FreightCouponMoney = 0;
                couponRefundDetailDTO.CommodityRefundMoney = orderRefundAfterSales.RefundMoney;
                couponRefundDetailDTO.FreightRefundMoney = orderRefundAfterSales.RefundFreightPrice;
                couponRefundDetailDTO.RefundTotalMoney = commodityPrice + orderRefundAfterSales.RefundMoney + orderRefundAfterSales.RefundFreightPrice;
                couponRefundDetailDTO.ShopName = commodityOrder.AppName;
                couponRefundDetailDTO.OrderNo = commodityOrder.Code;
                couponRefundDetailDTO.CommoidtyName = orderitemorder.Name;
                couponRefundDetailDTO.ReceivePhone = commodityOrder.ReceiptPhone;
                couponRefundDetailDTO.ConsigneeName = commodityOrder.ReceiptUserName;
                couponRefundDetailDTO.Remark = "";
                couponRefundDetailDTO.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(couponRefundDetailDTO);
                contextSession.SaveChanges();
            }
            #endregion
            var mobile = user != null ? user.Item2 : "";
            var result = YJBSV.RefundOrderItemJournal(commodityOrder.Id, comodityId, orderRefundAfterSales.RefundYJBMoney, orderItemId, commodityPrice, mobile, commodityOrder.UserId, useryjcouponid);
            if (!result.IsSuccess)
            {
                LogHelper.Error("售后退款时，回退用户易捷币失败，订单ID：" + commodityOrder.Id + "，错误内容：" + result.Message);
                LogOrderErrorInfo(contextSession, commodityOrder, "售后退款时，回退用户易捷币失败");
            }
            return result;
        }


        /// <summary>
        /// 记录易捷币错误信息
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="description"></param>
        private static void LogOrderErrorInfo(ContextSession contextSession, CommodityOrder commodityOrder, string description)
        {
            ErrorCommodityOrder errorOrder = new ErrorCommodityOrder();
            errorOrder.Id = Guid.NewGuid();
            errorOrder.ErrorOrderId = commodityOrder.Id;
            errorOrder.ResourceType = 7;
            errorOrder.Description = description;
            errorOrder.Source = commodityOrder.State;
            errorOrder.State = 0;
            errorOrder.AppId = commodityOrder.EsAppId.Value;
            errorOrder.UserId = commodityOrder.UserId;
            errorOrder.OrderCode = commodityOrder.Code;
            errorOrder.SubTime = DateTime.Now;
            errorOrder.ModifiedOn = DateTime.Now;
            errorOrder.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(errorOrder);
        }

        /// <summary>
        /// 获取商品的油卡兑换券额度
        /// </summary>
        /// <param name="esAppId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<CommodityYouKaDTO> GetCommodityYouKaPercent(Guid esAppId, List<Guid> commodityIdList)
        {
            //易捷APP才显示
            if (esAppId != YJBConsts.YJAppId || commodityIdList == null || commodityIdList.Count == 0)
            {
                return new List<CommodityYouKaDTO>();
            }
            return YJBSV.GetCommodityYouKaPercent(new GetYouKaPersentDTO { EsAppId = esAppId, CommodityIdList = commodityIdList });
        }
    }
}
