using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{    
     /// <summary>
    /// 物流详细信息
    /// </summary>
    [ServiceContract]
    public interface IExpressTrace : ICommand
    {
        /// <summary>
        /// 查询物流详细信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressTraceList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ExpressTraceDTO> GetExpressTraceList(ExpressTraceDTO search);
        /// <summary>
        /// 保存物流详细信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveExpressTraceList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveExpressTraceList(List<ExpressTraceDTO> list);


        /// <summary>
        /// 根据主键id删除物流详细信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelExpressTrace", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelExpressTrace(Guid ExpRouteId);

      


    }
}
