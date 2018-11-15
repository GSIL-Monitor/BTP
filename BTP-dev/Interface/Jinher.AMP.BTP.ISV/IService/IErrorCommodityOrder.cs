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
    /// <summary>
    /// 错误订单处理接口
    /// </summary>
    [ServiceContract]
    public interface IErrorCommodityOrder : ICommand
    {
        /// <summary>
        /// Job自动处理取消订单时回退积分
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrderCancelSrore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrderCancelSrore();

        /// <summary>
        ///  Job自动处理售中退款时回退积分
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrderRefundScore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrderRefundScore();

        /// <summary>
        ///  Job自动处理售后退款时回退积分
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoDealOrderAfterSalesRefundScore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoDealOrderAfterSalesRefundScore();

        /// <summary>
        /// Job自动处理回退积分
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoRefundScore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoRefundScore();
    }
}
