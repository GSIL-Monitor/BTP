
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/20 19:31:22
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

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 订单接口
    /// </summary>
    [ServiceContract]
    public interface ICommodityOrder : ICommand
    {
       

        /// <summary>
        /// 生成订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SaveCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="orderSDTO">订单实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        OrderResultDTO SaveCommodityOrder(OrderSDTO orderSDTO);   //订单DTO包含订单商品

        /// <summary>
        /// 生成奖品订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SavePrizeCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="orderSDTO">订单实体</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SavePrizeCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        OrderResultDTO SavePrizeCommodityOrder(OrderSDTO orderSDTO);   //订单DTO包含订单商品

        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="state">订单状态:未付款=0，未发货=1，已发货=2，确认收货=3，删除=4</param>
        /// <param name="orderId">订单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <param name="payment">付款方式:金币=0，到付=1，支付宝=2</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("已过时，请调用UpdateCommodityOrderNew", false)]
        ResultDTO UpdateCommodityOrder(int state, Guid orderId, Guid userId, Guid appId, int payment, string goldpwd, string remessage);


        /// <summary>
        /// 查询订单详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetOrderItems
        /// </para>
        /// </summary>
        /// <param name="commodityorderId">订单ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityOrderSDTO GetOrderItems(Guid commodityorderId, Guid userId, Guid appId);

        /// <summary>
        /// 查询分享订单详情页面  
        /// Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetShareOrderItems
        /// </summary>
        /// <param name="commodityorderId">订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShareOrderItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderShareDTO GetShareOrderItems(System.Guid commodityorderId);


        /// <summary>
        /// 根据交易状态获取订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetCommodityOrderByState
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <param name="state">订单状态</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderByState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderListCDTO> GetCommodityOrderByState(Guid userId, Guid appId, int state, int pageIndex, int pageSize);

        /// <summary>
        /// 获取用户所有订单
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetCommodityOrderByUserID
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state">订单状态0：未付款|1:未发货|2:已发货|3:交易成功|-1：失败</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderByUserID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderListCDTO> GetCommodityOrderByUserID(Guid userId, int pageIndex, int pageSize, int? state);


        /// <summary>
        /// 金币确认收货
        /// </summary>
        /// <param name="commodityOrderId">订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ConfirmOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ConfirmOrder(Guid commodityOrderId, string password);

        /// <summary>
        /// 确认按最新价支付
        /// </summary>
        /// <param name="commodityOrderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ConfirmPayPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NewResultDTO ConfirmPayPrice(Guid commodityOrderId, Guid userId);

        /// <summary>
        /// 定时处理订单
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrder();

        /// <summary>
        /// 使用队列将订单提交到数据库
        /// </summary>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SubmitOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        OrderResultDTO SubmitOrder(OrderQueueDTO orderSDTO);   //订单DTO包含订单商品

        /// <summary>
        /// 根据好运来活动Id获取所有订单
        /// </summary>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetLotteryOrders", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<LotteryOrderInfoDTO> GetLotteryOrders(Guid lotteryId);

        /// <summary>
        /// 根据订单号获取订单状态
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetOrderStateByCode
        /// </summary>
        /// <param name="orderSDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderStateByCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO GetOrderStateByCode(string orderCode);

        /// <summary>
        /// 申请退款
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/SubmitOrderRefund
        /// </summary>
        /// <param name="submitOrderRefundDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SubmitOrderRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SubmitOrderRefund(SubmitOrderRefundDTO submitOrderRefundDTO);


        /// <summary>
        /// 查看退款申请
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/GetOrderRefund
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SubmitOrderRefundDTO GetOrderRefund(Guid commodityorderId, Guid orderItemId);

        /// <summary>
        /// 撤销退款申请
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/CancelOrderRefund
        /// </summary>
        /// <param name="submitOrderRefundDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelOrderRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelOrderRefund(Guid commodityorderId, int state);

        /// <summary>
        /// 撤销退款申请
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/CancelOrderRefund
        /// </summary>
        /// <param name="submitOrderRefundDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelOrderItemRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelOrderItemRefund(Guid commodityorderId, int state, Guid orderItemId);

        /// <summary>
        /// 退款物流信息提交
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/AddOrderRefundExp
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="RefundExpCo">退货物流公司</param>
        /// <param name="RefundExpOrderNo">退货单号</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddOrderRefundExp", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddOrderRefundExp(Guid commodityorderId, string RefundExpCo, string RefundExpOrderNo, Guid orderItemId);

        /// <summary>
        /// 延长收货时间
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/DelayConfirmTime
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelayConfirmTime", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelayConfirmTime(Guid commodityorderId);

        /// <summary>
        /// 提交订单后3天未付款，则交易状态变为“交易失败”，实收款显示为 0（超时交易关闭state=6）
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ThreeDayNoPayOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void ThreeDayNoPayOrder();

        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/PayUpdateCommodityOrder
        /// </para>
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <param name="payment">付款方式:金币=0，到付=1，支付宝=2</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/PayUpdateCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO PayUpdateCommodityOrder(Guid orderId, Guid userId, Guid appId, int payment, ulong gold, decimal money, decimal couponCount);

        /// <summary>
        /// 订单状态修改 金采团购活动使用
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/PayUpdateCommodityOrderForJc", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO PayUpdateCommodityOrderForJc(Guid orderId);

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="commodityorderId"></param>
        /// <param name="IsDel"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelOrder(Guid commodityorderId, int IsDel);

        /// <summary>
        /// 处理待发货的退款处理订单 48小时内未响应 交易状态变为 7 已退款
        /// </summary>
        //[WebInvoke(Method = "POST", UriTemplate = "/AutoDaiRefundOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDaiRefundOrder();

        /// <summary>
        /// 处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        //[WebInvoke(Method = "POST", UriTemplate = "/AutoYiRefundOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoYiRefundOrder();





        /// <summary>
        /// 保存商品订单 --- 厂家直销，一次购买多个App
        /// </summary>
        /// <param name="orderSDTO">orderSDTO</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSetCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SetOrderResultDTO SaveSetCommodityOrder(OrderSDTO orderSDTO);

        /// <summary>
        /// 商家批量删除订单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteOrders", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteOrders(List<Guid> list);

        /// <summary>
        /// 根据订单主表ID获取子订单信息
        /// </summary>
        /// <param name="MainOrderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMianOrderList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<MainOrdersDTO> GetMianOrderList(Guid MainOrderId);

        /// <summary>
        /// 删除订单集合表
        /// </summary>
        /// <param name="SubOrderId"></param>
        /// <returns></returns>
        //[WebInvoke(Method = "POST", UriTemplate = "/DeleteMainOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteMainOrder(Guid SubOrderId);

        /// <summary>
        /// 处理超时未支付订单
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ExpirePayOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ExpirePayOrder(Guid orderId);

        /// <summary>
        /// 批量处理超时未支付订单
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoExpirePayOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoExpirePayOrder();

        /// <summary>
        /// 订单实时传递给盈科，补发数据
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SendOrderInfoToYKBDMq", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendOrderInfoToYKBDMq();

        /// <summary>
        /// 保存商品订单 --- 厂家直销，一次购买多个App
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSetCommodityOrderNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SetOrderResultDTO SaveSetCommodityOrderNew(List<OrderSDTO> orderList);

        /// <summary>
        /// 统计分润异常订单
        /// </summary>
        /// <param name="orderList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalcOrderException", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void CalcOrderException();

        /// <summary>
        /// 获取提货点管理员所管理的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns>提货点管理员所管理的订单信息</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderListByManagerId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderListCDTO> GetOrderListByManagerId(Guid userId, int pageIndex, int pageSize);



        /// <summary>
        /// 获取订货商品清单信息
        /// </summary>
        /// <param name="managerId">管理员ID</param>
        /// <param name="pickUpCode">推广码</param>
        /// <returns>订单编号和商品明细</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItemsByPickUpCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommodityOrderSDTO> GetOrderItemsByPickUpCode(Guid managerId, string pickUpCode);

        /// <summary>
        /// 补生成二维码方法
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/RepairePickUpCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void RepairePickUpCode();
        /// <summary>
        /// 批量增加售后完成送积分
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoAddOrderScore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool AutoAddOrderScore();

        /// <summary>
        /// 售中买家7天未发货超时处理
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoRefundAndCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoRefundAndCommodityOrder();


        /// <summary>
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrderConfirm", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrderConfirm();

        /// <summary>
        ///处理售中仅退款的申请订单 5天内未响应 交易状态变为 7 已退款
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoOnlyRefundOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoOnlyRefundOrder();

        /// <summary>
        ///重新校验已完成订单的钱款去向
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckFinishOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void CheckFinishOrder();

        /// <summary>
        ///中石化电子发票 补发错误发票请求以及下载电子发票接口调用
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DownloadEInvoiceInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void DownloadEInvoiceInfo();

        /// <summary>
        ///中石化电子发票 部分退款商品 退完全款后 继续开正常发票
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/PrCreateInvoic", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void PrCreateInvoic();

        /// <summary>
        /// 获取订单列表（包含保险订单）
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderByUserIDNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderListCDTO> GetCommodityOrderByUserIDNew(OrderQueryParamDTO oqpDTO);

        /// <summary>
        /// 获取订单列表（不包含保险订单）
        /// </summary>
        /// <param name="oqpDTO">订单列表查询参数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCustomCommodityOrderByUserIDNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderListCDTO> GetCustomCommodityOrderByUserIDNew(OrderQueryParamDTO oqpDTO);

        /// <summary>
        /// 获取所有订单数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        UserOrderCountDTO GetOrderCount(Guid userId, Guid esAppId);

        /// <summary>
        /// 服务订单状态变化发出通知.
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ServiceOrderStateChangedNotify", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void ServiceOrderStateChangedNotify();



        /// <summary>
        /// 订单状态修改
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/UpdateCommodityOrderNew
        /// </para>
        /// </summary>
        /// <param name="state">订单状态:未付款=0，未发货=1，已发货=2，确认收货=3，删除=4</param>
        /// <param name="orderId">订单Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <param name="payment">付款方式:金币=0，到付=1，支付宝=2</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityOrderNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodityOrderNew(UpdateCommodityOrderParamDTO ucopDto);

        /// <summary>
        /// 订单验签名
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/OrderSign", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<string> OrderSign(SignUrlDTO signUrlDTO);

        /// <summary>
        /// 生成在线支付的Url地址
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPayUrl", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetPayUrl(Jinher.AMP.BTP.Deploy.CustomDTO.PayOrderToFspDTO payOrderToFspDto);

        /// <summary>
        /// 生成在线支付的Url地址
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderCheckInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<OrderCheckDTO> GetOrderCheckInfo(OrderQueryParamDTO search);

        /// <summary>
        /// 更新订单售后时间
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderServiceTime", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderServiceTime(OrderQueryParamDTO search);

        /// <summary>
        /// 判断订单是否为拆分订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckIsMainOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool CheckIsMainOrder(Guid orderId);


        /// <summary>
        /// 跟据id获取订单内容
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityOrderDTO> GetCommodityOrder(List<Guid> ids);


        //job更新京东订单
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateJobCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateJobCommodityOrder(DateTime time, System.Guid orderId, System.Guid userId, System.Guid appId, int payment, string goldpwd, string remessage);

        /// <summary>
        /// 更新订单统计表 记录用户近一年的订单总数、金额总数以及最后的交易时间等字段
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RenewOrderStatistics", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RenewOrderStatistics();

        /// <summary>
        /// 获取符合条件的用户数据
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderStatistics", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<OrderStatisticsSDTO>> GetOrderStatistics(Jinher.AMP.Coupon.Deploy.CustomDTO.SearchOrderStatisticsExtDTO search);

        /// <summary>
        /// 易捷币抵现订单，按照商品进行拆分
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderItemYjbPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemYjbPrice();

        /// <summary>
        /// 处理单品退款，OrderItem表State状态不正确的问题
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderItemState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderItemState();

        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityOrderSdtoByOrderItemId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderSDTO GetCommodityOrderSdtoByOrderItemId(Guid orderItemId);

        /// <summary>
        /// 根据订单项Id获取订单部分信息 封装给sns使用
        /// </summary>
        /// <param name="orderItemId">订单项id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItemAttrId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetOrderItemAttrId(Guid orderItemId);

        /// <summary>
        /// 发货提醒，发消息给商家
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ShipmentRemind", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool ShipmentRemind(Guid commodityOrderId);

        /// <summary>
        /// 返回用户的退换货列表
        /// </summary>
        /// <param name="oqpDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetRefundList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<RefundOrderListDTO> GetRefundList(OrderQueryParamDTO oqpDTO);

        /// <summary>
        /// 补发分享佣金接口
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/PushShareMoney", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool PushShareMoney(Guid orderId);
    }
}
