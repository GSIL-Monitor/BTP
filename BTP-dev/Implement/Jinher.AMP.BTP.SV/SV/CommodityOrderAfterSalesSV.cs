
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/10/23 11:03:41
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using System.Diagnostics;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityOrderAfterSalesSV : BaseSv, ICommodityOrderAfterSales
    {

        /// <summary>
        /// 处理待发货的退款处理订单 48小时内未响应 交易状态变为 7 已退款
        ///  </summary>
        public void AutoDaiRefundOrderAfterSales()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoDaiRefundOrderAfterSalesExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AutoDaiRefundOrderAfterSales：耗时：{0}。", timer.ElapsedMilliseconds));


        }
        /// <summary>
        /// 售后提交退款/退货申请订单
        /// </summary>
        /// <param name="submitOrderRefundDTO">DTO</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SubmitOrderRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO submitOrderRefundDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SubmitOrderRefundAfterSalesExt(submitOrderRefundDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.SubmitOrderRefundAfterSales：耗时：{0}。入参：submitOrderRefundDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(submitOrderRefundDTO), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 售后撤销退款/退货申请
        /// </summary>
        /// <param name="cancelOrderRefundDTO">撤销退款/退货申请</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelOrderRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelOrderRefundDTO cancelOrderRefundDTO)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CancelOrderRefundAfterSalesExt(cancelOrderRefundDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.CancelOrderRefundAfterSales：耗时：{0}。入参：cancelOrderRefundDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(cancelOrderRefundDTO), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 售后查询退款/退货申请
        /// </summary>
        /// <param name="commodityorderId">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefundAfterSales(System.Guid commodityorderId, Guid orderItemId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrderRefundAfterSalesExt(commodityorderId, orderItemId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.GetOrderRefundAfterSales：耗时：{0}。入参：commodityorderId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, commodityorderId, JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 售后增加退货物流信息
        /// </summary>
        /// <param name="addOrderRefundExpDTO">物流信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOrderRefundExpAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.AddOrderRefundExpDTO addOrderRefundExpDTO)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AddOrderRefundExpAfterSalesExt(addOrderRefundExpDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AddOrderRefundExpAfterSales：耗时：{0}。入参：addOrderRefundExpDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(addOrderRefundExpDTO), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        public void AutoYiRefundOrderAfterSales()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoYiRefundOrderAfterSalesExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AutoYiRefundOrderAfterSales：耗时：{0}。", timer.ElapsedMilliseconds));

        }
        /// <summary>
        /// 买家7天不发出退货，自动恢复交易成功天数计时
        /// </summary>
        public void AutoRefundAndCommodityOrderAfterSales()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoRefundAndCommodityOrderAfterSalesExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AutoRefundAndCommodityOrderAfterSales：耗时：{0}。", timer.ElapsedMilliseconds));

        }
        /// <summary>
        /// 满7天自动处理售后（排除退款退货申请和卖家拒绝之间的时间）
        /// </summary>
        public void AutoDealOrderAfterSales()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoDealOrderAfterSalesExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AutoDealOrderAfterSales：耗时：{0}。", timer.ElapsedMilliseconds));

        }
        /// <summary>
        ///  售后同意退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CancelTheOrderAfterSalesExt(cancelTheOrderDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.CancelTheOrderAfterSales：耗时：{0}。入参：cancelTheOrderDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(cancelTheOrderDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.RefuseRefundOrderAfterSalesExt(cancelTheOrderDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.RefuseRefundOrderAfterSales：耗时：{0}。入参：cancelTheOrderDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(cancelTheOrderDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 拒绝原因
        /// </summary>
        /// <param name="refuseDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> DealRefuseBusinessAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefuseDTO refuseDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DealRefuseBusinessAfterSalesExt(refuseDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.DealRefuseBusinessAfterSales：耗时：{0}。入参：refuseDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(refuseDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 获取自提点售后待处理和已处理的自提的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <param name="state">0：待处理，1：已处理</param>
        /// <returns>自提点售后待处理和已处理的自提的订单信息</returns>
        public List<Deploy.CustomDTO.OrderListCDTO> GetSelfTakeOrderListAfterSales(Guid userId, int pageIndex, int pageSize, string state)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetSelfTakeOrderListAfterSalesExt(userId, pageIndex, pageSize, state);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.GetSelfTakeOrderListAfterSales：耗时：{0}。入参：userId:{1},pageIndex:{2},pageSize:{3},state:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, userId, pageIndex, pageSize, state, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 待自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>待自提订单数量</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<int> GetSelfTakeManagerAfterSales(Guid userId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetSelfTakeManagerAfterSalesExt(userId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.GetSelfTakeManagerAfterSales：耗时：{0}。入参：userId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, userId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        public void AutoDealOrderConfirmAfterSales()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AutoDealOrderConfirmAfterSalesExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommodityOrderAfterSalesSV.AutoDealOrderConfirmAfterSales：耗时：{0}。", timer.ElapsedMilliseconds));

        }
    }
}
