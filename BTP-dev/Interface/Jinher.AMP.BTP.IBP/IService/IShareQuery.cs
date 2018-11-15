
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/4/18 11:21:55
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
    /// <summary>
    /// 商品/订单分享查询接口
    /// </summary>
    [ServiceContract]
    public interface IShareQuery : ICommand
    {
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityDTO GetCommodity(Guid commodityId);

        /// <summary>
        /// 获取订单中的商品列表
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderCommoditys", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        OrderForShareDTO GetOrderCommoditys(Guid orderId);
    }
}
