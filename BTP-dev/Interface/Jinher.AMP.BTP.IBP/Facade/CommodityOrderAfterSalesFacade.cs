
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/10/23 11:01:17
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class CommodityOrderAfterSalesFacade : BaseFacade<ICommodityOrderAfterSales>
    {

        /// <summary>
        /// 售后同意退款/退货申请
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.CancelTheOrderAfterSales(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.RefuseRefundOrderAfterSales(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSales(System.Guid commodityorderId)
        {
            base.Do();
            return this.Command.GetOrderRefundAfterSales(commodityorderId);
        }

        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderItemRefundAfterSales(System.Guid commodityorderId, Guid orderItemId)
        {
            base.Do();
            return this.Command.GetOrderItemRefundAfterSales(commodityorderId, orderItemId);
        }

        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> GetRefundInfoAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            base.Do();
            return this.Command.GetRefundInfoAfterSales(refundInfoDTO);
        }

        /// <summary>
        /// 售后延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTimeAfterSales(Guid commodityorderId)
        {
            base.Do();
            return this.Command.DelayConfirmTimeAfterSales(commodityorderId);
        }

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSellerAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.RefuseRefundOrderSellerAfterSales(cancelTheOrderDTO);
        }

        /// <summary>
        /// 售后退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO">退款model，orderId为必填参数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSalesCallBack(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.CancelTheOrderAfterSalesCallBack(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            base.Do();
            return this.Command.DirectPayRefundAfterSales(orderRefundDto);
        }
    }
}