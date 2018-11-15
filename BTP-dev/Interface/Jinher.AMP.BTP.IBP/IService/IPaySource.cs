using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.IBP.IService
{ 
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
        /// 获取所有支付方式对应的描述信息。
        /// </summary>
        /// <param name="payment">支付方式编号</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPaymentName", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetPaymentName(int payment);
    }
}
