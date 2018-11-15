
/***************
功能描述: 售后订单服务
作    者: 
创建时间: 2015/10/22 11:25:47
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
    /// 售后订单业务处理
    /// </summary>
    [ServiceContract]
    public interface ICommodityOrderAfterSales : ICommand
    {

        /// <summary>
        ///  满7天自动处理售后（排除退款退货申请和卖家拒绝之间的时间，排除退款退货申请和卖家同意并未超时未收到货之间的时间）
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrderAfterSales();

        /// <summary>
        ///处理的退款处理订单 5天内未响应 交易状态变为 7 已退款
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDaiRefundOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDaiRefundOrderAfterSales();

        /// <summary>
        /// 售后提交退款/退货申请订单
        /// </summary>
        /// <param name="submitOrderRefundDTO">DTO</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SubmitOrderRefundAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SubmitOrderRefundAfterSales(SubmitOrderRefundDTO submitOrderRefundDTO);

        /// <summary>
        /// 售后撤销退款/退货申请
        /// </summary>
        /// <param name="cancelOrderRefundDTO">撤销退款/退货申请</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelOrderRefundAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelOrderRefundAfterSales(CancelOrderRefundDTO cancelOrderRefundDTO);

        /// <summary>
        /// 售后查询退款/退货申请
        /// </summary>
        /// <param name="commodityorderId">商品订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefundAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SubmitOrderRefundDTO GetOrderRefundAfterSales(Guid commodityorderId, Guid orderItemId);


        /// <summary>
        /// 售后增加退货物流信息
        /// </summary>
        /// <param name="addOrderRefundExpDTO">物流信息</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddOrderRefundExpAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddOrderRefundExpAfterSales(AddOrderRefundExpDTO addOrderRefundExpDTO);

        /// <summary>
        /// 处理5天内商家未响应，自动达成同意退款/退货申请协议订 交易状态变为 10 
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoYiRefundOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoYiRefundOrderAfterSales();

        /// <summary>
        /// 买家7天不发出退货，自动恢复交易成功天数计时
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoRefundAndCommodityOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoRefundAndCommodityOrderAfterSales();

        /// <summary>
        ///  售后同意退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelTheOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelTheOrderAfterSales(CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RefuseRefundOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RefuseRefundOrderAfterSales(CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 拒绝原因
        /// </summary>
        /// <param name="refuseDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DealRefuseBusinessAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<int> DealRefuseBusinessAfterSales(RefuseDTO refuseDTO);

        /// <summary>
        /// 获取自提点售后待处理和已处理的自提的订单信息
        /// </summary>
        /// <param name="userId">提货点管理员</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">页面数量</param>
        /// <param name="state">0：待处理，1：已处理</param>
        /// <returns>自提点售后待处理和已处理的自提的订单信息</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSelfTakeOrderListAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderListCDTO> GetSelfTakeOrderListAfterSales(Guid userId, int pageIndex, int pageSize, string state);

        /// <summary>
        /// 售后自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>售后自提订单数量</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSelfTakeManagerAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<int> GetSelfTakeManagerAfterSales(Guid userId);

        /// <summary>
        /// 买家发货（物流信息）后9天（若有延长，则为12天），卖家自动确认收货。
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrderConfirmAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrderConfirmAfterSales();
    }
}
