
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/5/10 14:16:21
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

    [ServiceContract]
    public interface IRefundExpressTrace : ICommand
    {
        /// <summary>
        /// 更新退货物流跟踪数据（物流公司及物流单号）
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateRefundExpress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateRefundExpress(RefundExpressTraceDTO retd);


        /// <summary>
        /// 获取退货物流跟踪列表数据
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetRefundExpressTrace", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<RefundExpressTraceDTO>> GetRefundExpressTrace();
    }
}
