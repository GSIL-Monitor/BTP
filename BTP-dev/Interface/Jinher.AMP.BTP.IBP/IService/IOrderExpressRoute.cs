using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO.Commodity;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface IOrderExpressRoute : ICommand
    {
        /// <summary>
        /// 接收快递鸟推送的物流路由信息。
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ReceiveKdniaoExpressRoute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ReceiveKdniaoExpressRoute(List<OrderExpressRouteExtendDTO> oerList);

        /// <summary>
        /// 按快递单号获取快递路由信息。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressRouteByExpNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<OrderExpressRouteExtendDTO> GetExpressRouteByExpNo(OrderExpressRouteDTO express);

        /// <summary>
        /// 根据快递单号获取快递信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressRouteByExpOrderNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        OrderExpressRouteDTO GetExpressRouteByExpOrderNo(string expOrderNo);


        /// <summary>
        /// 修改快递信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateExpressRoute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateExpressRoute(OrderExpressRouteDTO model);

        /// <summary>
        ///  获取物流跟踪信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderExpressForJdJob", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void GetOrderExpressForJdJob();

        /// <summary>
        ///  获取物流跟踪信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderExpressForJsJob", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void GetOrderExpressForJsJob();
        /// <summary>
        /// 获取用户最新的订单物流信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserNewOrderExpress", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ComOrderExpressNew> GetUserNewOrderExpress(Guid AppId, Guid Userid);

    }
}
