using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface ISNExpressTrace : ICommand
    {
        /// <summary>
        /// 获得苏宁物流详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressTrace", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Deploy.SNExpressTraceDTO> GetExpressTrace(string orderId, string orderItemId);
        /// <summary>
        /// 苏宁物流信息同步
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeLogistStatusForJob", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool ChangeLogistStatusForJob();

    }
}
