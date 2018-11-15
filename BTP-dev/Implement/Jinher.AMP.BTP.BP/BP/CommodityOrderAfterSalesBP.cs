
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/10/23 11:01:20
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using System.Diagnostics;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityOrderAfterSalesBP : BaseBP, ICommodityOrderAfterSales
    {

        /// <summary>
        /// 售后同意退款/退货申请
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CancelTheOrderAfterSalesExt(cancelTheOrderDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesBP.CancelTheOrderAfterSales：耗时：{0},入参:{1},出参:{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(cancelTheOrderDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            LogHelper.Info(string.Format(" 售后拒绝退款/退货申请 CommodityOrderAfterSalesBP.RefuseRefundOrderAfterSales, cancelTheOrderDTO:{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), "BTP_Order");
            base.Do();
            return this.RefuseRefundOrderAfterSalesExt(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSales(System.Guid commodityorderId)
        {
            base.Do(false);
            return this.GetOrderRefundAfterSalesExt(commodityorderId);
        }
        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderItemRefundAfterSales(Guid commodityorderId, Guid orderItemId)
        {
            base.Do(false);
            return this.GetOrderItemRefundAfterSalesExt(commodityorderId, orderItemId);
        }
        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> GetRefundInfoAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            base.Do();
            return this.GetRefundInfoAfterSalesExt(refundInfoDTO);
        }
        /// <summary>
        /// 售后延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTimeAfterSales(Guid commodityorderId)
        {
            LogHelper.Info(string.Format("售后售后延长收货时间 CommodityOrderAfterSalesBP.DelayConfirmTimeAfterSales, commodityorderId:{0}", commodityorderId), "BTP_Order");
            base.Do();
            return this.DelayConfirmTimeAfterSalesExt(commodityorderId);
        }
        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSellerAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            LogHelper.Info(string.Format("售后拒绝收货 CommodityOrderAfterSalesBP.RefuseRefundOrderSellerAfterSales, cancelTheOrderDTO:{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), "BTP_Order");
            base.Do();
            return this.RefuseRefundOrderSellerAfterSalesExt(cancelTheOrderDTO);
        }

        /// <summary>
        /// 售后退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO">退款model，orderId为必填参数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSalesCallBack(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            LogHelper.Info(string.Format("售后退款金币回调 CommodityOrderAfterSalesBP.CancelTheOrderAfterSalesCallBack, cancelTheOrderDTO:{0}", JsonHelper.JsonSerializer(cancelTheOrderDTO)), "BTP_Order");
            base.Do(false);
            return this.CancelTheOrderAfterSalesCallBackExt(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            base.Do(false);
            return this.DirectPayRefundAfterSalesExt(orderRefundDto);
        }
    }
}
