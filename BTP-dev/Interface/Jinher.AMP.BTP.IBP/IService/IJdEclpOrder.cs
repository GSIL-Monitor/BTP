using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 进销存
    /// </summary>
    [ServiceContract]
    public interface IJdEclpOrder : ICommand
    {
        /// <summary>
        /// 创建京东订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="eclpOrderNo">京东订单编号,京东接口失败补录数据用</param>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void CreateOrder(Guid orderId, string eclpOrderNo);

        /// <summary>
        /// 发送支付信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SendPayInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendPayInfoToHaiXin(Guid orderId);

        /// <summary>
        /// 发送售中整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendRefundInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendRefundInfoToHaiXin(Guid orderId);

        /// <summary>
        /// 发送售中单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendSingleRefundInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendSingleRefundInfoToHaiXin(Guid orderId, Guid orderItemId);

        /// <summary>
        /// 发送售后整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendASRefundInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendASRefundInfoToHaiXin(Guid orderId);

        /// <summary>
        /// 发送售后单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/SendASSingleRefundInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendASSingleRefundInfoToHaiXin(Guid orderId, Guid orderItemId);

        /// <summary>
        /// 获取京东订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        JDEclpOrderDTO GetOrderInfo(Guid orderId);

        /// <summary>
        /// 创建进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <param name="servicesNo">京东服务单编号,京东接口失败补录数据用</param>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateJDEclpRefundAfterSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void CreateJDEclpRefundAfterSales(Guid orderId, Guid orderItemId, string servicesNo);

        /// <summary>
        /// 是否进销存京东订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ISEclpOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool ISEclpOrder(Guid orderId);

        /// <summary>
        /// 获取进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJdEclpOrderRefundAfterSale", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        JDEclpOrderRefundAfterSalesDTO GetJdEclpOrderRefundAfterSale(Guid orderId, Guid orderItemId);

        /// <summary>
        /// 进销存-京东商品库存同步日志
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJDStockJourneyList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<JDStockJournalDTO>> GetJDStockJourneyList(JourneyDTO searcharg);

        /// <summary>
        /// 进销存-京东订单日志
        /// </summary>
        /// <param name="searcharg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJDEclpOrderJournalList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<JDEclpJourneyExtendDTO>> GetJDEclpOrderJournalList(JourneyDTO searcharg);

        /// <summary>
        /// 进销存-京东服务单日志
        /// </summary>
        /// <param name="searcharg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJDEclpOrderRefundAfterSalesJournalList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<JDEclpOrderRefundAfterSalesJournalDTO>> GetJDEclpOrderRefundAfterSalesJournalList(JourneyDTO searcharg);

        /// <summary>
        /// 进销存-获取订单物流单号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpOrderNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<string> GetExpOrderNo(Guid orderId);


        /// <summary>
        /// 重新发送支付信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RetranPayInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void RetranPayInfoToHaiXin(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 重新发送售中整单退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/RetranRefundInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void RetranRefundInfoToHaiXin(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 重新发送售后单品退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/RetranASSingleRefundInfoToHaiXin", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void RetranASSingleRefundInfoToHaiXin(DateTime startTime, DateTime endTime);
    }
}
