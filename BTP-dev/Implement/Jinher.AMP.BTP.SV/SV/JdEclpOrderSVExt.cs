using System;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.SV
{
    /// <summary>
    /// 进销存-京东订单
    /// </summary>
    public partial class JdEclpOrderSV : BaseSv, IJdEclpOrder
    {
        /// <summary>
        /// 拒收后自动申请退款
        /// </summary>
        /// <param name="orderId"></param>
        private void ApplyRefund(Guid orderId)
        {
            var order = new ISV.Facade.CommodityOrderFacade().GetOrderItems(orderId, Guid.Empty, Guid.Empty);
            if (order == null)
            {
                LogHelper.Error("进销存-拒收后自动申请退款，未获取到订单信息,入参:" + orderId);
                return;
            }
            var refundMoney = order.Price + (order.YJBPrice ?? 0) + order.ScorePrice + order.YJCouponPrice;
            var result = new CommodityOrderSV().SubmitOrderRefundExt(new SubmitOrderRefundDTO
            {
                commodityorderId = orderId,
                Id = orderId,
                RefundDesc = "用户已拒收",
                RefundExpOrderNo = string.Empty,
                RefundMoney = refundMoney,
                RefundCouponPirce = order.YJCouponPrice,
                State = 2,
                RefundReason = "用户已拒收",
                RefundType = 0,
                OrderRefundImgs = string.Empty
            });
            if (result.ResultCode != 0)
            {
                LogHelper.Error("进销存-拒收后自动申请退款失败:" + result.Message + ",入参:" + orderId);
            }
        }

        /// <summary>
        /// 妥投后自动确认收货
        /// </summary>
        /// <param name="orderId"></param>
        private void ConfirmOrder(Guid orderId)
        {
            var order = CommodityOrder.ObjectSet().FirstOrDefault(p => p.Id == orderId);
            if (order == null)
            {
                LogHelper.Error("进销存-妥投后自动确认收货:未找到订单,入参:" + orderId);
                return;
            }
            var result = new CommodityOrderSV().UpdateCommodityOrderExt(3, orderId, order.SubId, order.AppId, order.Payment, string.Empty, string.Empty);
            if (result.ResultCode != 0)
            {
                LogHelper.Error("进销存-妥投后自动确认收货失败:" + result.Message + ",入参:" + orderId);
            }
        }

        /// <summary>
        /// 进销存-同步京东订单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ResultDTO SynchronizeJDOrderStateExt(JDEclpOrderJournalDTO arg)
        {
            var json = string.Empty;
            var result = new ResultDTO();
            try
            {
                json = JsonHelper.JsonSerializer(arg);
                LogHelper.Debug("进销存-同步京东订单状态,入参:" + json);
                var order = JDEclpOrder.ObjectSet()
                    .Where(p => p.OrderId == arg.OrderId && p.EclpOrderNo == arg.EclpOrderNo)
                    .FirstOrDefault();
                if (order == null) return new ResultDTO { isSuccess = true, Message = "订单不存在" };
                if (JDEclpOrderJournal.ObjectSet().Any(p => p.Code == arg.Code))
                    return new ResultDTO { isSuccess = true, Message = "已同步过" };
                switch ((JDEclpOrderStateEnum)arg.StateTo)
                {
                    case JDEclpOrderStateEnum.EclpOrderState10019://发货
                        order.ShipTime = arg.SubTime;
                        new IBP.Facade.JdEclpOrderFacade().GetExpOrderNo(order.OrderId);
                        break;
                    case JDEclpOrderStateEnum.EclpOrderState10034://妥投
                        if (!order.ReceiveTime.HasValue)
                        {
                            ConfirmOrder(arg.OrderId);
                            order.ReceiveTime = arg.SubTime;
                        }
                        break;
                    case JDEclpOrderStateEnum.EclpOrderState10028://取消订单
                    case JDEclpOrderStateEnum.EclpOrderState10035://拒收
                    case JDEclpOrderStateEnum.EclpOrderState10038://逆向完成
                        if (!order.RejectTime.HasValue)
                        {
                            ApplyRefund(arg.OrderId);
                            order.RejectTime = arg.SubTime;
                        }
                        break;
                }
                var journal = new JDEclpOrderJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    EclpOrderNo = arg.EclpOrderNo,
                    SubTime = arg.SubTime,
                    Name = arg.Name,
                    Details = arg.Name,
                    StateFrom = order.EclpOrderState,
                    StateTo = arg.StateTo,
                    Code = arg.Code,
                    Json = arg.Json,
                    EntityState = System.Data.EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                if (arg.StateTo > order.EclpOrderState)
                {
                    order.EclpOrderState = arg.StateTo;
                    order.EclpOrderStateName = arg.Name;
                }
                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                {
                    result.isSuccess = true;
                    result.Message = "同步成功";
                }
                else
                {
                    LogHelper.Error("进销存-同步京东订单状态:数据保存失败,入参:" + json);
                    result.Message = "数据保存失败";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("进销存-同步京东订单状态异常,入参:" + json, ex);
                result.Message = "同步异常";
            }
            return result;
        }

        /// <summary>
        /// 进销存-同步京东服务单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ResultDTO SynchronizeJDServiceStateExt(SynchronizeJDServiceStateDTO arg)
        {
            var json = string.Empty;
            var result = new ResultDTO();
            try
            {
                json = JsonHelper.JsonSerializer(arg);
                LogHelper.Debug("进销存-同步京东服务单状态,入参:" + json);
                var service = JDEclpOrderRefundAfterSales.ObjectSet()
                    .Where(p => p.Id == arg.JDEclpOrderRefundAfterSalesId)
                    .FirstOrDefault();
                if (service == null) return new ResultDTO { isSuccess = true, Message = "京东服务单不存在" };
                var refund = OrderRefundAfterSales.ObjectSet().Where(p => p.Id == service.OrderRefundAfterSalesId).FirstOrDefault();
                if (refund == null) return new ResultDTO { isSuccess = true, Message = "售后退款单不存在" };
                if (JDEclpOrderRefundAfterSalesJournal.ObjectSet().Any(p => p.Code == arg.Code))
                    return new ResultDTO { isSuccess = true, Message = "已同步过" };
                switch ((JDEclpServicesStateEnum)arg.StateTo)
                {
                    case JDEclpServicesStateEnum.EclpServiceState20080://正常入库
                    case JDEclpServicesStateEnum.EclpServiceState20100://异常入库
                        var goodNum = arg.ServiceItemList.Count(p => p.WareType == 1 && p.GoodsStatus == 1);//可退商品数量
                        if (goodNum != arg.ServiceItemList.Count)
                        {
                            if (goodNum == 0) refund.RefundMoney = 0;
                            refund.RefundMoney = Math.Round(refund.RefundMoney * goodNum / arg.ServiceItemList.Count, 2);
                        }
                        break;
                }
                var journal = new JDEclpOrderRefundAfterSalesJournal
                {
                    Id = Guid.NewGuid(),
                    OrderId = service.OrderId,
                    OrderCode = service.OrderCode,
                    OrderItemId = service.OrderItemId,
                    OrderRefundAfterSalesId = service.OrderRefundAfterSalesId,
                    EclpOrderNo = service.EclpOrderNo,
                    EclpServicesNo = arg.EclpServicesNo,
                    SubTime = arg.SubTime,
                    Name = new EnumHelper().GetDescription((JDEclpServicesStateEnum)arg.StateTo),
                    Details = arg.Name,
                    StateFrom = service.EclpServicesState,
                    StateTo = arg.StateTo,
                    JDEclpOrderRefundAfterSalesId = service.Id,
                    WarehouseNo = arg.WarehouseNo,
                    WarehouseName = arg.WarehouseName,
                    EntityState = System.Data.EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(journal);
                if (arg.ServiceItemList != null && arg.ServiceItemList.Count > 0)
                {
                    arg.ServiceItemList.ForEach(p =>
                    {
                        var item = new JDEclpOrderRefundAfterSalesItem
                        {
                            Id = Guid.NewGuid(),
                            JDEclpOrderRefundAfterSalesId = service.Id,
                            IsvGoodsNo = p.IsvGoodsNo,
                            SpareCode = p.SpareCode ?? string.Empty,
                            PartReceiveType = p.PartReceiveType,
                            GoodsStatus = p.GoodsStatus,
                            WareType = p.WareType,
                            ApproveNotes = p.ApproveNotes,
                            EntityState = System.Data.EntityState.Added
                        };
                        ContextFactory.CurrentThreadContext.SaveObject(item);
                    });
                }
                service.EclpServicesState = arg.StateTo;
                service.EclpServicesStateName = new EnumHelper().GetDescription((JDEclpServicesStateEnum)arg.StateTo);
                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                {
                    result.isSuccess = true;
                    result.Message = "同步成功";
                }
                else
                {
                    LogHelper.Error("进销存-同步京东服务单状态:数据保存失败,入参:" + json);
                    result.Message = "数据保存失败";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("进销存-同步京东服务单状态异常,入参:" + json, ex);
                result.Message = "同步异常";
            }
            return result;
        }
    }
}
