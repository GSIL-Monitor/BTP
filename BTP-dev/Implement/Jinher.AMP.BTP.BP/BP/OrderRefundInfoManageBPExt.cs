
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/1/22 11:05:06
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class OrderRefundInfoManageBP : BaseBP, IOrderRefundInfoManage
    {

        /// <summary>
        /// 添加部分退单商品信息
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddRefundComdtyInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.BOrderRefundInfoCDTO cdto)
        {
            Deploy.CustomDTO.ResultDTO ret = new Deploy.CustomDTO.ResultDTO() { isSuccess = false, ResultCode = 1, Message = "添加失败！" };

            if (cdto == null)
            {
                ret.Message = "参数不能为空！";
                return ret;
            }
            BE.OrderRefundInfo refundInfo = new OrderRefundInfo();
            refundInfo.Id = cdto.id;
            refundInfo.OrderId = cdto.orderId;
            refundInfo.ItemId = cdto.itemId;
            refundInfo.Refund = cdto.refund;
            refundInfo.isDelivery = cdto.isDelivery;
            refundInfo.OrderRefundId = cdto.orderRefundId;
            refundInfo.SubId = cdto.subId;
            refundInfo.SubTime = cdto.subTime;
            refundInfo.ModifiedOn = cdto.modifiedOn;
            refundInfo.EntityState = System.Data.EntityState.Added;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(refundInfo);
                int change = contextSession.SaveChanges();
                ret.isSuccess = change > 0;
                if (ret.isSuccess)
                    ret.Message = "添加成功！";

                return ret;
            }
            catch (Exception ex)
            {
                ret.Message = ex.Message;
            }
            return ret;
        }
        
        /// <summary>
        /// 获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FOrderRefundInfoCDTO GetOrderRefundInfoExt(System.Guid orderId)
        {
            Deploy.CustomDTO.FOrderRefundInfoCDTO refundInfo = (from info in BE.OrderRefundInfo.ObjectSet()
                                                        where info.OrderId == orderId
                                                        orderby info.SubTime descending
                                                        select new
                                                            Deploy.CustomDTO.FOrderRefundInfoCDTO
                                                        {
                                                            id= info.Id,
                                                            orderId = info.OrderId,
                                                            itemId = info.ItemId,
                                                            refund = info.Refund,
                                                            isDelivery = info.isDelivery,
                                                            orderRefundId = info.OrderRefundId
                                                        }).FirstOrDefault();

            return refundInfo == null ? new Deploy.CustomDTO.FOrderRefundInfoCDTO() : refundInfo;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO GetOrderRefundInfoByItemIdExt(System.Guid orderItemId)
        {
            var a = OrderRefund.ObjectSet();
            var refundInfo = (from refund in OrderRefund.ObjectSet()
                              join orderItem in OrderItem.ObjectSet() on refund.OrderItemId equals orderItem.Id
                              where refund.OrderItemId == orderItemId
                              orderby refund.SubTime descending
                              select new Deploy.CustomDTO.OrderRefundDTO
                              {
                                  Id = refund.Id,
                                  RefundType = refund.RefundType,
                                  RefundReason = refund.RefundReason,
                                  RefundMoney = refund.RefundMoney,
                                  RefundDesc = refund.RefundDesc,
                                  OrderId = refund.OrderId,
                                  State = refund.State,
                                  OrderRefundImgs = refund.OrderRefundImgs,
                                  DataType = refund.DataType,
                                  OrderItemId = refund.OrderItemId,
                                  RefuseTime = refund.RefuseTime,
                                  RefuseReason = refund.RefuseReason,
                                  SubTime = refund.SubTime,
                                  ModifiedOn = refund.ModifiedOn,
                                  Num= orderItem.Number
                              }).FirstOrDefault();

            return refundInfo == null ? new Deploy.CustomDTO.OrderRefundDTO() : refundInfo;
        }
    }
}