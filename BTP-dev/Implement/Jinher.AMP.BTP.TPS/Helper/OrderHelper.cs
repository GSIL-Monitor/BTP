using System;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Common;
using System.Threading;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 订单帮助类
    /// </summary>
    public static class OrderHelper
    {
        /// <summary>
        /// 同意取消订单（未发货）
        /// </summary>
        /// <param name="orderId"></param>
        public static ResultDTO ApproveCancelOrder(Guid orderId)
        {
            return ApproveCancelOrder(orderId, Guid.Empty);
        }
        /// <summary>
        /// 同意取消订单（未发货）
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public static ResultDTO ApproveCancelOrder(Guid orderId, Guid orderItemId)
        {
            UpdateCommodityOrderParamDTO dto = new UpdateCommodityOrderParamDTO();
            dto.orderId = orderId;
            dto.orderItemId = orderItemId;
            dto.targetState = 7;
            var cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            cf.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            return cf.CancelTheOrder(dto);
        }

        /// <summary>
        /// 同意退款申请
        /// </summary>
        /// <param name="orderId"></param>
        public static ResultDTO ApproveOrderRefund(Guid orderId)
        {
            return ApproveOrderRefund(orderId, Guid.Empty);
        }

        /// <summary>
        /// 同意退款申请（达成协议前后2次同意都调用此方法）
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public static ResultDTO ApproveOrderRefund(Guid orderId, Guid orderItemId)
        {
            var param = new UpdateCommodityOrderParamDTO
            {
                orderId = orderId,
                orderItemId = orderItemId,
                targetState = 21
            };
            var cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            cf.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            return cf.CancelTheOrder(param);
        }

        /// <summary>
        /// 同意退款申请-售后
        /// </summary>
        /// <param name="orderId"></param>
        public static ResultDTO ApproveOrderRefundAfterSales(Guid orderId)
        {
            return ApproveOrderRefundAfterSales(orderId, Guid.Empty);
        }
        /// <summary>
        /// 同意退款申请-售后（达成协议前后2次同意都调用此方法）
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public static ResultDTO ApproveOrderRefundAfterSales(Guid orderId, Guid orderItemId)
        {
            var param = new CancelTheOrderDTO
            {
                OrderId = orderId,
                OrderItemId = orderItemId,
                State = 21
            };
            var cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
            cf.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            return cf.CancelTheOrderAfterSales(param);
        }

        /// <summary>
        /// 拒绝退款申请
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="refuseReason"></param>
        public static ResultDTO RejectOrderRefund(Guid orderId, string refuseReason)
        {
            return RejectOrderRefund(orderId, Guid.Empty, refuseReason);
        }
        /// <summary>
        /// 拒绝退款申请（达成协议前后2次拒绝都调用此方法）
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="refuseReason"></param>
        public static ResultDTO RejectOrderRefund(Guid orderId, Guid orderItemId, string refuseReason)
        {
            var param = new CancelTheOrderDTO()
            {
                OrderId = orderId,
                OrderItemId = orderItemId,
                State = 2,
                RefuseReason = refuseReason
            };
            var cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderFacade();
            cf.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            return cf.RefuseRefundOrder(param);
        }

        /// <summary>
        /// 拒绝退款申请-售后
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="refuseReason"></param>
        public static ResultDTO RejectOrderRefundAfterSales(Guid orderId, string refuseReason)
        {
            return RejectOrderRefundAfterSales(orderId, Guid.Empty, refuseReason);
        }
        /// <summary>
        /// 拒绝退款申请-售后（达成协议前后2次拒绝都调用此方法）
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <param name="refuseReason"></param>
        public static ResultDTO RejectOrderRefundAfterSales(Guid orderId, Guid orderItemId, string refuseReason)
        {
            var param = new CancelTheOrderDTO()
            {
                OrderId = orderId,
                OrderItemId = orderItemId,
                State = 2,
                RefuseReason = refuseReason
            };
            var cf = new Jinher.AMP.BTP.IBP.Facade.CommodityOrderAfterSalesFacade();
            cf.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
            return cf.RefuseRefundOrderAfterSales(param);
        }
    }
}
