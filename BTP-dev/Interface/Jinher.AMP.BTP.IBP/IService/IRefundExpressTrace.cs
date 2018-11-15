
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/5/10 13:44:10
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
    public interface IRefundExpressTrace : ICommand
    {
       
        /// <summary>
        /// 更新退货物流跟踪数据(物流相关时间)
        /// </summary>
        /// <param name="retd"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateRefundExpressTrace", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateRefundExpressTrace(RefundExpressTraceDTO retd,Guid appId);


        /// <summary>
        /// 新增退货物流跟踪数据(商家确认退款时间)
        /// </summary>
        /// <param name="retd"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddRefundExpressTrace", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddRefundExpressTrace(RefundExpressTraceDTO retd);

    }
}
