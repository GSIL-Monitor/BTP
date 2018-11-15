
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/2 18:07:23
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using System.Diagnostics;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityOrderBP : BaseBP, ICommodityOrder
    {

        /// <summary>
        /// 获得订单详细信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderVM GetCommodityOrder(System.Guid id, System.Guid appId)
        {
            base.Do(false);
            return this.GetCommodityOrderExt(id, appId);
        }

        /// <summary>
        /// 获得商家所有订单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO GetAllCommodityOrderByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            base.Do(false);
            return this.GetAllCommodityOrderByAppIdExt(search);
        }

        /// <summary>
        /// 获得商家所有订单（获取电商馆下所有订单）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO> GetAllCommodityOrderByEsAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            base.Do(false);
            return this.GetAllCommodityOrderByEsAppIdExt(search);
        }

        /// <summary>
        /// 修改订单实收总价
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="price">实收总价</param>
        /// <returns></returns>
        public ResultDTO UpdateOrderPrice(Guid orderId, decimal price, Guid userId)
        {
            LogHelper.Info(string.Format("售中修改订单实收总价 CommodityOrderBP.UpdateOrderPrice, orderId:{0} , price:{1} ,userId:{2}  ", orderId, price, userId), "BTP_Order");
            base.Do(false);
            return this.UpdateOrderPriceExt(orderId, price, userId);
        }

        public PaymentsDTO GetPaymentByOrderId(Guid orderId, string paymentName)
        {
            base.Do(false);
            return this.GetPaymentByOrderIdExt(orderId, paymentName);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmPayment(Guid orderId, int payment)
        {
            LogHelper.Info(string.Format("确认订单支付(支付宝回调) CommodityOrderBP.ConfirmPayment, orderId:{0} , payment:{1}  ", orderId, payment), "BTP_Order");
            base.Do(false);
            return this.ConfirmPaymentExt(orderId, payment);
        }

        /// <summary>
        /// 订单导出
        /// </summary>
        /// <param name="orderIds">订单Ids</param>
        /// <returns></returns>
        public ResultDTO ImportOrder(List<Guid> orderIds)
        {
            base.Do(false);
            return this.ImportOrderExt(orderIds);
        }

        /// <summary>
        ///    查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderItemRefund(Guid commodityorderId, Guid orderItemId)
        {
            base.Do(false);
            return this.GetOrderItemRefundExt(commodityorderId, orderItemId);
        }

        /// <summary>
        ///    查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public SubmitOrderRefundDTO GetOrderRefund(Guid commodityorderId)
        {
            base.Do(false);
            return this.GetOrderRefundExt(commodityorderId);
        }

        public ResultDTO ShipUpdataOrder(Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            LogHelper.Info(string.Format("订单发货 CommodityOrderBP.ShipUpdataOrder, commodityOrderId:{0} , shipExpCo:{1} ,expOrderNo:{2} ", commodityOrderId, shipExpCo, expOrderNo), "BTP_Order");
            base.Do(false);
            return this.ShipUpdataOrderExt(commodityOrderId, shipExpCo, expOrderNo);
        }

        public List<Guid> GetCommodityIdsByOrderId(Guid orderId)
        {
            base.Do(false);
            return this.GetCommodityIdsByOrderIdExt(orderId);
        }

        /// <summary>
        /// 支付宝直接到账退款
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="ReceiverAccount"></param>
        /// <param name="Receiver"></param>
        /// <param name="RefundMoney"></param>
        /// <returns></returns>
        public ResultDTO AlipayZhiTui(Guid commodityorderId, string ReceiverAccount, string Receiver, decimal RefundMoney)
        {
            LogHelper.Info(string.Format("支付宝直接到账退款 CommodityOrderBP.AlipayZhiTui, commodityOrderId:{0} , ReceiverAccount:{1} ,Receiver:{2} ,RefundMoney:{3}", commodityorderId, ReceiverAccount, Receiver, RefundMoney), "BTP_Order");
            base.Do(false);
            return this.AlipayZhiTuiExt(commodityorderId, ReceiverAccount, Receiver, RefundMoney);
        }

        public List<QryOrderTradeMoneyDTO> QryAppOrderTradeInfo(string appName)
        {
            base.Do(false);
            return this.QryAppOrderTradeInfoExt(appName);
        }

        /// <summary>
        /// 导出订单数据
        /// </summary>ExportResult
        /// <param name="param"></param>

        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> ExportResult(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {

            base.Do(false);
            return this.ExportResultExt(param);
        }

        /// <summary>
        /// 导出订单数据
        /// </summary>ExportResult
        /// <param name="param"></param>

        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> ExportResult1(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {

            base.Do(false);
            return this.ExportResult1Ext(param);
        }

        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetEsOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do(false);
            return this.GetEsOrderIdsExt(param);
        }

        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do(false);
            return this.GetOrderIdsExt(param);
        }



        /// <summary>
        /// 获取查询的所有阳光餐饮的订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Guid> GetTotalOrderIds(ExportParamDTO param)
        {
            base.Do(false);
            return this.GetTotalOrderIdsExt(param);
        }


        /// <summary>
        /// 修改卖家备注信息
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="SellersRemark"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSellersRemark(System.Guid commodityOrderId, string SellersRemark)
        {
            base.Do(false);
            return this.UpdateSellersRemarkExt(commodityOrderId, SellersRemark);
        }

        /// <summary>
        /// 修改物流信息（只修改信息，不修改订单状态）
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
        public ResultShipDTO ChgOrderShip(Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            LogHelper.Info(string.Format("修改物流信息 CommodityOrderBP.ChgOrderShip, commodityOrderId:{0} , shipExpCo:{1} ,expOrderNo:{2}", commodityOrderId, shipExpCo, expOrderNo), "BTP_Order");
            base.Do(false);
            return this.ChgOrderShipExt(commodityOrderId, shipExpCo, expOrderNo);
        }
        /// <summary>
        /// 根据对账订单Id列表取电商订单对账信息
        /// </summary>
        /// <param name="mainOrderIds">对账订单Id列表</param>
        /// <returns>电商订单对账信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderCheckAccount> GetMainOrdersPay(string mainOrderIds)
        {
            base.Do(false);
            return this.GetMainOrdersPayExt(mainOrderIds);
        }
        /// <summary>
        /// 售中拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrder(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            LogHelper.Info(string.Format("拒绝退款/退货申请 CommodityOrderBP.RefuseRefundOrder, cancelTheOrderDTO:{0} ", JsonHelper.JsonSerializer(cancelTheOrderDTO)), "BTP_Order");
            base.Do(false);
            return this.RefuseRefundOrderExt(cancelTheOrderDTO);
        }
        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSeller(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            LogHelper.Info(string.Format("拒绝收货 CommodityOrderBP.RefuseRefundOrderSeller, cancelTheOrderDTO:{0} ", JsonHelper.JsonSerializer(cancelTheOrderDTO)), "BTP_Order");
            base.Do(false);
            return this.RefuseRefundOrderSellerExt(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售中卖家延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTime(Guid commodityorderId)
        {
            LogHelper.Info(string.Format("拒绝收货 CommodityOrderBP.DelayConfirmTime, commodityorderId:{0} ", commodityorderId), "BTP_Order");
            base.Do(false);
            return this.DelayConfirmTimeExt(commodityorderId);
        }
        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> GetRefundInfo(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            base.Do(false);
            return this.GetRefundInfoExt(refundInfoDTO);
        }
        /// <summary>
        /// 查询分销订单
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO GetDistributeOrderList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search)
        {
            base.Do(false);
            return this.GetDistributeOrderListExt(search);
        }

        /// <summary>
        /// 获取订单相关信息（订单，售后， 退款，分润设置，钱款去向，订单项）
        /// </summary>
        /// <param name="orderId">商品订单ID或订单编号</param>
        /// <returns></returns>
        public OrderFullInfo GetFullOrderInfoById(string orderId)
        {
            base.Do(false);
            return this.GetFullOrderInfoByIdExt(orderId);
        }
        /// <summary>
        /// 退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public ResultDTO CancelTheOrderCallBack(CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do(false);
            return this.CancelTheOrderCallBackExt(cancelTheOrderDTO);
        }

        /// <summary>
        /// 获取订单来源
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetOrderSource(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            base.Do(false);
            return this.GetOrderSourceExt(search);
        }

        /// <summary>
        /// 获取app的钱款去向
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetCommodityOrderMoneyTo(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            base.Do(false);
            return this.GetCommodityOrderMoneyToExt(search);
        }


        /// <summary>
        ///  修改订单
        /// </summary>
        ///<param name="ucopDto">参数实体</param>
        /// <returns></returns>
        public ResultDTO CancelTheOrder(UpdateCommodityOrderParamDTO ucopDto)
        {
            base.Do(false);
            return this.CancelTheOrderExt(ucopDto);
        }
        /// <summary>
        ///  批量修改订单状态为出库中
        /// </summary>
        ///<param name="commodityOrderIds"></param>
        /// <returns></returns>
        public ResultDTO BatchUpdateCommodityOrder(string commodityOrderIds)
        {
            base.Do(false);
            return this.BatchUpdateCommodityOrderExt(commodityOrderIds);
        }
        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefund(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            base.Do(false);
            return this.DirectPayRefundExt(orderRefundDto);
        }
        /// <summary>
        /// 获取新增待处理订单数量
        /// </summary>
        /// <param name="appId">店铺id</param>
        /// <param name="lastPayTime">最后支付时间</param>
        /// <returns></returns>
        public int GetNewCyUntreatedCount(Guid appId, DateTime lastPayTime)
        {
            base.Do(false);
            return this.GetNewCyUntreatedCountExt(appId, lastPayTime);
        }

        /// <summary>
        /// 分享订单获取相关的的数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.Share.ShareOrderDTO GetShareOrderInfoByOrderId(Guid orderId)
        {
            base.Do(false);
            return this.GetShareOrderInfoByOrderIdExt(orderId);
        }
        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityOrderDTO GetCommodityOrderInfo(System.Guid id)
        {
            base.Do(false);
            return this.GetCommodityOrderInfoExt(id);
        }


        /// <summary>
        /// 获得主订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.MainOrderDTO GetMainOrderInfo(Guid suborderId)
        {
            base.Do(false);
            return this.GetMainOrderInfoExt(suborderId);
        }


        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityOrder(Jinher.AMP.BTP.Deploy.CommodityOrderDTO model)
        {
            base.Do(false);
            return this.UpdateCommodityOrderExt(model);
        }


        /// <summary>
        /// 根据ExpOrderNo获取订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public CommodityOrderDTO GetCommodityOrderbyExpOrderNo(string ExpOrderNo)
        {
            base.Do(false);
            return this.GetCommodityOrderbyExpOrderNoExt(ExpOrderNo);

        }

        /// <summary>
        /// 进销存系统对接临时方案-按京东eclp系统标准导出订单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportOrderForJDDTO> ExportOrderForJD(System.Guid appId, System.DateTime startTime, System.DateTime endTime)
        {
            base.Do(false);
            return this.ExportOrderForJDExt(appId, startTime, endTime);
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="FreightTo"></param>
        /// <param name="tem"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public decimal CalOneFreight(string FreightTo, Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO tem, Guid templateId)
        {
            this.Do();
            return this.CalOneFreightExt(FreightTo, tem, templateId);
        }

        /// <summary>
        /// 发送订单支付实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendPayInfoToYKBDMq(Guid orderId)
        {
            base.Do(false);
            this.SendPayInfoToYKBDMqExt(orderId);
        }
        
        /// <summary>
        /// 发送订单售中退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendRefundInfoToYKBDMq(Guid orderId)
        {
            base.Do(false);
            this.SendRefundInfoToYKBDMqExt(orderId);
        }
        
        /// <summary>
        /// 发送订单售后退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendASRefundInfoToYKBDMq(Guid orderId)
        {
            base.Do(false);
            this.SendASRefundInfoToYKBDMqExt(orderId);
        }        
    }
}
