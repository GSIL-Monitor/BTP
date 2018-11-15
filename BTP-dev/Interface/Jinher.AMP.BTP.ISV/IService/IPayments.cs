
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/4/3 11:48:54
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
    /// 支付方式
    /// </summary>
    [ServiceContract]
    public interface IPayments : ICommand
    {
        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPayments", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PaymentsSDTO> GetPayments(Guid appId);

        /// <summary>
        /// 获取支付方式 --- 厂家直销
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSetPayments", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PaymentsSDTO> GetSetPayments();

        /// <summary>
        /// 获取商家收款ID
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPayeeId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Guid GetPayeeId(Guid appId);

        /// <summary>
        /// 获取支付宝信息
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAlipayInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AlipayDTO GetAlipayInfo(Guid appId);

        /// <summary>
        /// 是不是所有店铺app都支持“货到付款”。
        /// </summary>
        /// <param name="appIds">店铺appId</param>
        /// <returns>是不是所有店铺app都支持“货到付款”</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsAllAppSupportCOD", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<bool> IsAllAppSupportCOD(List<Guid> appIds);
    }
}
