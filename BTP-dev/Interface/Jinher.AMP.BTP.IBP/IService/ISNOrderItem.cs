using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface ISNOrderItem : ICommand
    {
        /// <summary>
        /// 添加苏宁关系记录表
        /// </summary>
        /// <param name="allPaymentId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddSNOrderItem", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool AddSNOrderItem(List<Jinher.AMP.BTP.Deploy.SNOrderItemDTO> snOrderItem);
        /// <summary>
        /// 修改苏宁关系记录表
        /// </summary>
        /// <param name="allPaymentId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdSNOrderItem", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool UpdSNOrderItem(Jinher.AMP.BTP.Deploy.SNOrderItemDTO snOrderItem);
        /// <summary>
        /// 修改订单关系表状态
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeOrderStatusForJob", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool ChangeOrderStatusForJob();
        /// <summary>
        /// 厂送商品确认收货
        /// </summary>
        /// <param name="OrderId">订单ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/OrderConfirmReceived", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool OrderConfirmReceived(Guid OrderId);


    }
}
