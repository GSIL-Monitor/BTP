
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/10/23 11:03:38
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class CommodityOrderAfterSalesFacade : BaseFacade<ICommodityOrderAfterSales>
    {

        /// <summary>
        /// 处理的退款处理订单 5天内未响应 交易状态变为 7 已退款
        ///  </summary>
        public void AutoDaiRefundOrderAfterSales()
        {
            base.Do();
            this.Command.AutoDaiRefundOrderAfterSales();
        }
        /// <summary>
        /// 售后提交退款/退货申请订单
        /// </summary>
        /// <param name="submitOrderRefundDTO">DTO</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SubmitOrderRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            base.Do();
            return this.Command.SubmitOrderRefundAfterSales(submitOrderRefundDTO);
        }
        /// <summary>
        /// 售后撤销退款/退货申请
        /// </summary>
        /// <param name="cancelOrderRefundDTO">撤销退款/退货申请</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelOrderRefundDTO cancelOrderRefundDTO)
        {
            base.Do();
            return this.Command.CancelOrderRefundAfterSales(cancelOrderRefundDTO);
        }
        /// <summary>
        /// 售后查询退款/退货申请
        /// </summary>
        /// <param name="commodityorderId">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSales(System.Guid commodityorderId, Guid orderItemId)
        {
            base.Do();
            return this.Command.GetOrderRefundAfterSales(commodityorderId, orderItemId);
        }
        /// <summary>
        /// 售后增加退货物流信息
        /// </summary>
        /// <param name="addOrderRefundExpDTO">物流信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOrderRefundExpAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.AddOrderRefundExpDTO addOrderRefundExpDTO)
        {
            base.Do();
            return this.Command.AddOrderRefundExpAfterSales(addOrderRefundExpDTO);
        }
        /// <summary>
        /// 处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        public void AutoYiRefundOrderAfterSales()
        {
            base.Do();
            this.Command.AutoYiRefundOrderAfterSales();
        }
        /// <summary>
        /// 买家7天不发出退货，自动恢复交易成功天数计时，满7天自动处理售后
        /// </summary>
        public void AutoRefundAndCommodityOrderAfterSales()
        {
            base.Do();
            this.Command.AutoRefundAndCommodityOrderAfterSales();
        }

        /// <summary>
        /// 买家7天不发出退货，自动恢复交易成功天数计时，满7天自动处理售后
        /// </summary>
        public void AutoDealOrderAfterSales()
        {
            base.Do();
            this.Command.AutoDealOrderAfterSales();
        }

        /// <summary>
        ///  售后同意退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.CancelTheOrderAfterSales(cancelTheOrderDTO);
        }

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.RefuseRefundOrderAfterSales(cancelTheOrderDTO);
        }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        /// <param name="refuseDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> DealRefuseBusinessAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefuseDTO refuseDTO)
        {
            base.Do();
            return this.Command.DealRefuseBusinessAfterSales(refuseDTO);
        }

        /// <summary>
        /// 获取自提点售后待处理和已处理的自提的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <param name="state">订单状态</param>
        /// <returns>自提点售后待处理和已处理的自提的订单信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetSelfTakeOrderListAfterSales(Guid userId, int pageIndex, int pageSize, string state)
        {
            base.Do();
            return this.Command.GetSelfTakeOrderListAfterSales(userId, pageIndex, pageSize, state);
        }

        /// <summary>
        /// 售后自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>售后自提订单数量</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> GetSelfTakeManagerAfterSales(Guid userId)
        {
            base.Do();
            return this.Command.GetSelfTakeManagerAfterSales(userId);
        }

        /// <summary>
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirmAfterSales()
        {
            base.Do();
            this.Command.AutoDealOrderConfirmAfterSales();
        }
    }
}