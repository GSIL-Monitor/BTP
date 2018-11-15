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
    /// <summary>
    ///  售后订单业务处理
    /// </summary>
    [ServiceContract]
    public interface ICommodityOrderAfterSales : ICommand
    {
        /// <summary>
        /// 售后同意退款/退货申请
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelTheOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelTheOrderAfterSales(CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 售后拒绝退款/退货申请
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/RefuseRefundOrderAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RefuseRefundOrderAfterSales(CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefundAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SubmitOrderRefundDTO GetOrderRefundAfterSales(Guid commodityorderId);

        /// <summary>
        /// 售后查看详情页面使用
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItemRefundAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SubmitOrderRefundDTO GetOrderItemRefundAfterSales(Guid commodityorderId, Guid orderItemId);

        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="refundInfoDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetRefundInfoAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundAfterSalesDTO> GetRefundInfoAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.RefundInfoDTO refundInfoDTO);

        /// <summary>
        /// 售后延长收货时间
        /// </summary>
        /// <param name="commodityorderId">订单号</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelayConfirmTimeAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelayConfirmTimeAfterSales(Guid commodityorderId);

        /// <summary>
        /// 拒绝收货
        /// </summary>
        /// <param name="cancelTheOrderDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RefuseRefundOrderSellerAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RefuseRefundOrderSellerAfterSales(CancelTheOrderDTO cancelTheOrderDTO);



        /// <summary>
        /// 售后退款金币回调
        /// </summary>
        /// <param name="cancelTheOrderDTO">退款model，orderId为必填参数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelTheOrderAfterSalesCallBack", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CancelTheOrderAfterSalesCallBack(Jinher.AMP.BTP.Deploy.CustomDTO.CancelTheOrderDTO cancelTheOrderDTO);

        /// <summary>
        /// 售中直接到账退款
        /// </summary>
        /// <param name="orderRefundDto">退款信息</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DirectPayRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DirectPayRefundAfterSales(Jinher.AMP.BTP.Deploy.CustomDTO.OrderRefundDTO orderRefundDto);
    }
}

