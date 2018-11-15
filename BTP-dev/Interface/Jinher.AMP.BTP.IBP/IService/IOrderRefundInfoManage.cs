
/***************
功能描述: BTPIService
作    者: 
创建时间: 2017/1/22 10:45:06
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{
     /// <summary>
    /// 部分退单商品退款信息
    /// </summary>
    [ServiceContract]
    public interface IOrderRefundInfoManage : ICommand
    {
        /// <summary>
        /// 添加部分退单商品信息
        /// </summary>
        /// <param name="cdto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddRefundComdtyInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO AddRefundComdtyInfo(Deploy.CustomDTO.BOrderRefundInfoCDTO cdto);

        /// <summary>
        /// 获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefundInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.FOrderRefundInfoCDTO GetOrderRefundInfo(Guid orderId);

        /// <summary>
        /// 根据订单商品ID获取订单退款详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderRefundInfoByItemId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.OrderRefundDTO GetOrderRefundInfoByItemId(Guid orderItemId);
       
    }
}
