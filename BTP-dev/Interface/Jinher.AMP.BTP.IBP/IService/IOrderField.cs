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
    /// <summary>
    /// 订单设置
    /// </summary>
    [ServiceContract]
    public interface IOrderField : ICommand
    {
        /// <summary>
        /// 获取订单设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderSet", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.OrderFieldDTO GetOrderSet(Guid AppId);


        
    }
}
