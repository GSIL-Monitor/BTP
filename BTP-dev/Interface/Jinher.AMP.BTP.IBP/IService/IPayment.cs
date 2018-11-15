
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/4/8 11:08:14
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

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IPayment : ICommand
    {
        /// <summary>
        /// 获取所有支付方式
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPayment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PaymentsForEditDTO> GetAllPayment(Guid appId);

        /// <summary>
        /// 修改用户支付方式
        /// </summary>
        /// <param name="paymentsDTO">支付方式实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdatePayment", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdatePayment(PaymentsVM paymentsVM);

        /// <summary>
        /// 是否可以取消积分 (平台启用了分销并且设置了值，或启用了众销且设置了值，就不能取消。)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsEnableCancelScore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool IsEnableCancelScore(Guid appId);
    }
}
