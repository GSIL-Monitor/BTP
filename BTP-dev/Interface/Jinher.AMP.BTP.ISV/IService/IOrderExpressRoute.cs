using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO.Commodity;

namespace Jinher.AMP.BTP.ISV.IService
{
    [ServiceContract]
    public interface IOrderExpressRoute : ICommand
    {
        /// <summary>
        /// 按快递单号获取快递路由信息。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressRouteByExpNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<OrderExpressRouteExtendDTO> GetExpressRouteByExpNo(OrderExpressRouteDTO express);


        /// <summary>
        ///  使用job重新订阅快递鸟物流信息（对订阅失败的）。
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/SubscribeOrderExpressForJob", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SubscribeOrderExpressForJob();
        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserNewOrderExpress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ComOrderExpressNew> GetUserNewOrderExpress(Guid AppId, Guid Userid);


        /// <summary>
        /// 获取用户最新的所有订单的物流信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserAllNewOrderExpress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<ComOrderExpressNew>> GetUserAllNewOrderExpress(Guid AppId, Guid UserId);
    }
}
