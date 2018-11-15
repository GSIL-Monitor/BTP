
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/18 17:35:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface ICommodityOrder : ICommand
    {
        /// <summary>
        /// 获得订单详细信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityOrderVM GetCommodityOrder(Guid id, System.Guid appId);

        /// <summary>
        /// 获得商家所有订单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityOrderByAppId", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO GetAllCommodityOrderByAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search);

        /// <summary>
        /// 获得商家所有订单（获取电商馆下所有订单）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityOrderByEsAppId", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderListDTO> GetAllCommodityOrderByEsAppId(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSearchDTO search);

        /// <summary>
        /// 修改订单实收总价
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="price">实收总价</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateOrderPrice(Guid orderId, decimal price, Guid userId);

        /// <summary>
        /// 根据订单号获取商家对应的支付信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPaymentByOrderId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        PaymentsDTO GetPaymentByOrderId(Guid orderId, string paymentName);


        /// <summary>
        /// 订单确认支付(支付宝回调)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ConfirmPayment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ConfirmPayment(Guid orderId, int payment);

        /// <summary>
        /// 订单导出
        /// </summary>
        /// <param name="orderIds">订单Ids</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ImportOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ImportOrder(List<Guid> orderIds);

        /// <summary>
        ///  查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SubmitOrderRefundDTO GetOrderRefund(Guid commodityorderId);

        /// <summary>
        ///  查看退款申请
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItemRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SubmitOrderRefundDTO GetOrderItemRefund(Guid commodityorderId, Guid orderItemId);

        /// <summary>
        /// 发货填写发货物流信息
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
        [OperationContract]
        ResultDTO ShipUpdataOrder(Guid commodityOrderId, string shipExpCo, string expOrderNo);

        /// <summary>
        /// 根据订单Id获取所有商品Id列表
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        List<Guid> GetCommodityIdsByOrderId(Guid orderId);

        /// <summary>
        /// 支付宝直接到账退款
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="ReceiverAccount"></param>
        /// <param name="Receiver"></param>
        /// <param name="RefundMoney"></param>
        /// <returns></returns>
        [OperationContract]
        ResultDTO AlipayZhiTui(Guid commodityorderId, string ReceiverAccount, string Receiver, decimal RefundMoney);

        /// <summary>
        /// 查询应用交易金额信息
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        [OperationContract]
        List<QryOrderTradeMoneyDTO> QryAppOrderTradeInfo(string appName);

        /// <summary>
        /// 导出订单数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExportResultDTO> ExportResult(ExportParamDTO param);

        /// <summary>
        /// 导出订单数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [OperationContract]
        List<ExportResultDTO> ExportResult1(ExportParamDTO param);

        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<Guid> GetEsOrderIds(ExportParamDTO param);

        /// <summary>
        /// 获取查询的所有订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<Guid> GetOrderIds(ExportParamDTO param);

        /// <summary>
        /// 获取查询的所有阳光餐饮的订单号id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<Guid> GetTotalOrderIds(ExportParamDTO param);

        /// <summary>
        /// 修改卖家备注信息
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="SellersRemark"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSellersRemark", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateSellersRemark(Guid commodityOrderId, string SellersRemark);

        /// <summary>
        /// 修改物流信息（只修改信息，不修改订单状态）
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <param name="shipExpCo"></param>
        /// <param name="expOrderNo"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChgOrderShip", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultShipDTO ChgOrderShip(Guid commodityOrderId, string shipExpCo, string expOrderNo);

        /// <summary>
        /// 根据对账订单Id列表取电商订单对账信息
        /// </summary>
        /// <param name="mainOrderIds">对账订单Id列表</param>
        /// <returns>电商订单对账信息</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMainOrdersPay", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityOrderCheckAccount> GetMainOrdersPay(string mainOrderIds);

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RefuseRefundOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrder(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RefuseRefundOrderSeller", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RefuseRefundOrderSeller(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 售中卖家延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelayConfirmTime", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTime(Guid commodityorderId);

        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetRefundInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO> GetRefundInfo(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO);

        /// <summary>
        /// 查询分销订单
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributeOrderList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionResultDTO GetDistributeOrderList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderDistributionSearchDTO search);

        /// <summary>
        /// 获取订单相关信息（订单，售后， 退款，分润设置，钱款去向，订单项）
        /// </summary>
        /// <param name="orderId">商品订单ID或订单编号</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFullOrderInfoById", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        OrderFullInfo GetFullOrderInfoById(string orderId);

        /// <summary>
        /// 退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO">退款model，orderId为必填参数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelTheOrderCallBack", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderCallBack(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 获取订单来源
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderSource", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetOrderSource(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search);
        
        /// <summary>
        /// 获取app的钱款去向
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderMoneyTo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToDTO GetCommodityOrderMoneyTo(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderMoneyToSearch search);


        /// <summary>
        ///  修改订单
        /// </summary>
        ///<param name="ucopDto">参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelTheOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelTheOrder(UpdateCommodityOrderParamDTO ucopDto);

        /// <summary>
        ///  批量修改订单状态为出库中
        /// </summary>
        ///<param name="commodityOrderIds"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/BatchUpdateCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO BatchUpdateCommodityOrder(string commodityOrderIds);

        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DirectPayRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefund(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto);

        /// <summary>
        /// 获取新增待处理订单数量
        /// </summary>
        /// <param name="appId">店铺id</param>
        /// <param name="lastPayTime">最后支付时间</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetNewCyUntreatedCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetNewCyUntreatedCount(Guid appId, DateTime lastPayTime);


        /// <summary>
        /// 分享订单获取相关的的数据
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShareOrderInfoByOrderId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.Share.ShareOrderDTO GetShareOrderInfoByOrderId(Guid orderId);

        /// <summary>
        /// 获得订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityOrderDTO GetCommodityOrderInfo(Guid id);



        /// <summary>
        /// 获得主订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMainOrderInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        MainOrderDTO GetMainOrderInfo(Guid suborderId);


        /// <summary>
        /// 修改订单信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodityOrder(Jinher.AMP.BTP.Deploy.CommodityOrderDTO model);


        /// <summary>
        /// 根据ExpOrderNo获取订单信息
        /// </summary>
        /// <param name="id">商品订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderbyExpOrderNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityOrderDTO GetCommodityOrderbyExpOrderNo(string ExpOrderNo);

        /// <summary>
        /// 进销存系统对接临时方案-按京东eclp系统标准导出订单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportOrderForJD", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ExportOrderForJDDTO> ExportOrderForJD(Guid appId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 计算退款运费
        /// </summary>
        /// <param name="FreightTo"></param>
        /// <param name="tem"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportOrderForJD", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        decimal CalOneFreight(string FreightTo, Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO tem, Guid templateId);

        /// <summary>
        /// 发送订单支付实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendPayInfoToYKBDMq", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendPayInfoToYKBDMq(Guid orderId);

        /// <summary>
        /// 发送订单售中退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendRefundInfoToYKBDMq", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendRefundInfoToYKBDMq(Guid orderId);

        /// <summary>
        /// 发送订单售后退款实时数据到盈科大数据系统mq
        /// </summary>
        /// <param name="orderId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendASRefundInfoToYKBDMq", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendASRefundInfoToYKBDMq(Guid orderId);
    }
}
