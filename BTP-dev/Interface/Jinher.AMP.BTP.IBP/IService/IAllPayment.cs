
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/4/8 16:20:46
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

    [ServiceContract]
    public interface IAllPayment : ICommand
    {
        /// <summary>
        /// 由ID得到支付名称
        /// </summary>
        /// <param name="allPaymentId">支付ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetNameById", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetNameById(Guid allPaymentId);
    }
}
