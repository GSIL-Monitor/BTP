using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 支付方式接口
    /// </summary>
    [ServiceContract]
    public interface IPaySource : ICommand
    {
        /// <summary>
        /// 获取所有支付方式和描述信息。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPaySources", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PaySourceDTO> GetAllPaySources();

       

        /// <summary>
        /// 获取担保交易类支付方式。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSecuriedTransactionPayment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<int> GetSecuriedTransactionPayment();

        /// <summary>
        /// 获取担保交易类支付方式,排除金币。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSecTransWithoutGoldPayment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<int> GetSecTransWithoutGoldPayment();


        /// <summary>
        /// 获取直接到账类支付方式。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDirectArrivalPayment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<int> GetDirectArrivalPayment();

    }
}
