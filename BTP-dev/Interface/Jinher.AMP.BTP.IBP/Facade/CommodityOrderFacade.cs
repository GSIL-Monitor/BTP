
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/3/13 17:26:04
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
    public class CommodityOrderFacade : BaseFacade<ICommodityOrder>
    {

        /// <summary>
        /// 获得订单详细信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderVM GetCommodityOrder(System.Guid id, System.Guid appId)
        {
            base.Do();
            return this.Command.GetCommodityOrder(id, appId);
        }
        /// <summary>
        /// 获得商家所有订单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO GetAllCommodityOrderByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            base.Do();
            return this.Command.GetAllCommodityOrderByAppId(search);
        }
        /// <summary>
        /// 获得商家所有订单（获取电商馆下所有订单）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO> GetAllCommodityOrderByEsAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search)
        {
            base.Do();
            return this.Command.GetAllCommodityOrderByEsAppId(search);
        }
        /// <summary>
        /// 修改订单实收总价
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="price">实收总价</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderPrice(System.Guid orderId, decimal price, System.Guid userId)
        {
            base.Do();
            return this.Command.UpdateOrderPrice(orderId, price, userId);
        }
        /// <summary>
        /// 根据订单号获取商家对应的支付信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.PaymentsDTO GetPaymentByOrderId(System.Guid orderId, string paymentName)
        {
            base.Do();
            return this.Command.GetPaymentByOrderId(orderId, paymentName);
        }
        /// <summary>
        /// 订单确认支付(支付宝回调)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmPayment(System.Guid orderId, int payment)
        {
            base.Do();
            return this.Command.ConfirmPayment(orderId, payment);
        }
        /// <summary>
        /// 订单导出
        /// </summary>
        /// <param name="orderIds">订单Ids</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ImportOrder(System.Collections.Generic.List<System.Guid> orderIds)
        {
            base.Do();
            return this.Command.ImportOrder(orderIds);
        }
        /// <summary>
        ///  查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderRefund(System.Guid commodityorderId)
        {
            base.Do();
            return this.Command.GetOrderRefund(commodityorderId);
        }
        /// <summary>
        ///  查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SubmitOrderRefundDTO GetOrderItemRefund(System.Guid commodityorderId, System.Guid orderItemId)
        {
            base.Do();
            return this.Command.GetOrderItemRefund(commodityorderId, orderItemId);
        }
        /// <summary>
        /// 发货填写发货物流信息
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
    public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ShipUpdataOrder(System.Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            base.Do();
            return this.Command.ShipUpdataOrder(commodityOrderId, shipExpCo, expOrderNo);
        }
        /// <summary>
        /// 根据订单Id获取所有商品Id列表
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetCommodityIdsByOrderId(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetCommodityIdsByOrderId(orderId);
        }
        /// <summary>
        /// 支付宝直接到账退款
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="ReceiverAccount"></param>
        /// <param name="Receiver"></param>
        /// <param name="RefundMoney"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AlipayZhiTui(System.Guid commodityorderId, string ReceiverAccount, string Receiver, decimal RefundMoney)
        {
            base.Do();
            return this.Command.AlipayZhiTui(commodityorderId, ReceiverAccount, Receiver, RefundMoney);
        }
        /// <summary>
        /// 查询应用交易金额信息
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QryOrderTradeMoneyDTO> QryAppOrderTradeInfo(string appName)
        {
            base.Do();
            return this.Command.QryAppOrderTradeInfo(appName);
        }
        /// <summary>
        /// 导出订单数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> ExportResult(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do();
            return this.Command.ExportResult(param);
        }
        /// <summary>
        /// 导出订单数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExportResultDTO> ExportResult1(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do();
            return this.Command.ExportResult1(param);
        }
        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetEsOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do();
            return this.Command.GetEsOrderIds(param);
        }
        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do();
            return this.Command.GetOrderIds(param);
        }

        /// <summary>
        /// 获取查询的所有阳光餐饮的订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Guid> GetTotalOrderIds(Jinher.AMP.BTP.Deploy.CustomDTO.ExportParamDTO param)
        {
            base.Do();
            return this.Command.GetTotalOrderIds(param);
        }


        /// <summary>
        /// 修改卖家备注信息
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="SellersRemark"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSellersRemark(System.Guid commodityOrderId, string SellersRemark)
        {
            base.Do();
            return this.Command.UpdateSellersRemark(commodityOrderId, SellersRemark);
        }
        /// <summary>
        /// 修改物流信息（只修改信息，不修改订单状态）
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultShipDTO ChgOrderShip(System.Guid commodityOrderId, string shipExpCo, string expOrderNo)
        {
            base.Do();
            return this.Command.ChgOrderShip(commodityOrderId, shipExpCo, expOrderNo);
        }
        /// <summary>
        /// 根据对账订单Id列表取电商订单对账信息
        /// </summary>
        /// <param name="mainOrderIds">对账订单Id列表</param>
        /// <returns>电商订单对账信息</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderCheckAccount> GetMainOrdersPay(string mainOrderIds)
        {
            base.Do();
            return this.Command.GetMainOrdersPay(mainOrderIds);
        }
        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrder(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.RefuseRefundOrder(cancelTheOrderDTO);
        }
        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSeller(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.RefuseRefundOrderSeller(cancelTheOrderDTO);
        }
        /// <summary>
        /// 售中卖家延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTime(System.Guid commodityorderId)
        {
            base.Do();
            return this.Command.DelayConfirmTime(commodityorderId);
        }
        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> GetRefundInfo(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO)
        {
            base.Do();
            return this.Command.GetRefundInfo(refundInfoDTO);
        }
        /// <summary>
        /// 查询分销订单
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO GetDistributeOrderList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search)
        {
            base.Do();
            return this.Command.GetDistributeOrderList(search);
        }
        /// <summary>
        /// 获取订单相关信息（订单，售后， 退款，分润设置，钱款去向，订单项）
        /// </summary>
        /// <param name="orderId">商品订单ID或订单编号</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.OrderFullInfo GetFullOrderInfoById(string orderId)
        {
            base.Do();
            return this.Command.GetFullOrderInfoById(orderId);
        }
        /// <summary>
        /// 退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO">退款model，orderId为必填参数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderCallBack(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO)
        {
            base.Do();
            return this.Command.CancelTheOrderCallBack(cancelTheOrderDTO);
        }
        /// <summary>
        /// 获取订单来源
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetOrderSource(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            base.Do();
            return this.Command.GetOrderSource(search);
        }
        /// <summary>
        /// 获取app的钱款去向
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetCommodityOrderMoneyTo(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search)
        {
            base.Do();
            return this.Command.GetCommodityOrderMoneyTo(search);
        }
        /// <summary>
        ///   修改订单
        ///  </summary>
        /// <param name="ucopDto">参数实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrder(Jinher.AMP.BTP.Deploy.CustomDTO.UpdateCommodityOrderParamDTO ucopDto)
        {
            base.Do();
            return this.Command.CancelTheOrder(ucopDto);
        }
        /// <summary>
        ///   批量修改订单状态为出库中
        ///  </summary>
        /// <param name="commodityOrderIds"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO BatchUpdateCommodityOrder(string commodityOrderIds)
        {
            base.Do();
            return this.Command.BatchUpdateCommodityOrder(commodityOrderIds);
        }
        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefund(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto)
        {
            base.Do();
            return this.Command.DirectPayRefund(orderRefundDto);
        }
        /// <summary>
        /// 获取新增待处理订单数量
        /// </summary>
        /// <param name="appId">店铺id</param>
        /// <param name="lastPayTime">最后支付时间</param>
        /// <returns></returns>
        public int GetNewCyUntreatedCount(System.Guid appId, System.DateTime lastPayTime)
        {
            base.Do();
            return this.Command.GetNewCyUntreatedCount(appId, lastPayTime);
        }
        /// <summary>
        /// 分享订单获取相关的的数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.Share.ShareOrderDTO GetShareOrderInfoByOrderId(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetShareOrderInfoByOrderId(orderId);
        }
        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityOrderDTO GetCommodityOrderInfo(System.Guid id)
        {
            base.Do();
            return this.Command.GetCommodityOrderInfo(id);
        }
        /// <summary>
        /// 获得主订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.MainOrderDTO GetMainOrderInfo(System.Guid suborderId)
        {
            base.Do();
            return this.Command.GetMainOrderInfo(suborderId);
        }
        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityOrder(Jinher.AMP.BTP.Deploy.CommodityOrderDTO model)
        {
            base.Do();
            return this.Command.UpdateCommodityOrder(model);
        }
        /// <summary>
        /// 根据ExpOrderNo获取订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityOrderDTO GetCommodityOrderbyExpOrderNo(string ExpOrderNo)
        {
            base.Do();
            return this.Command.GetCommodityOrderbyExpOrderNo(ExpOrderNo);
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
            base.Do();
            return this.Command.ExportOrderForJD(appId, startTime, endTime);
        }

        /// <summary>
        /// 退款运费计算
        /// </summary>
        /// <param name="FreightTo"></param>
        /// <param name="tem"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public decimal CalOneFreight(string FreightTo, Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO tem, Guid templateId)
        {
            base.Do();
            return this.Command.CalOneFreight(FreightTo, tem, templateId);
        }

        /// <summary>
        /// 发送订单支付实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        public void SendPayInfoToYKBDMq(Guid orderId)
        {
            base.Do();
            this.Command.SendPayInfoToYKBDMq(orderId);
        }

        /// <summary>
        /// 发送订单售中退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        public void SendRefundInfoToYKBDMq(Guid orderId)
        {
            base.Do();
            this.Command.SendRefundInfoToYKBDMq(orderId);
        }

        /// <summary>
        /// 发送订单售后退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        public void SendASRefundInfoToYKBDMq(Guid orderId)
        {
            base.Do();
            this.Command.SendASRefundInfoToYKBDMq(orderId);
        }
    }
}
