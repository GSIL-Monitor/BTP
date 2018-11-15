
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/24 13:52:39
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
    /// <summary>
    /// 收藏接口
    /// </summary>
    [ServiceContract]
    public interface ISetCollection : ICommand
    {
        /// <summary>
        /// 添加商品收藏
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/SaveCommodityCollection
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="channelId">渠道Id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommodityCollection", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityCollection(System.Guid commodityId, System.Guid userId, System.Guid channelId);

        /// <summary>
        /// 店铺收藏
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/SaveAppCollection
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="userId">用户ID</param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveAppCollection", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppCollection(System.Guid appId, System.Guid userId, System.Guid channelId);

        /// <summary>
        /// 根据用户ID查询收藏商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionComs
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCollectionComs", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCollectionComs(SetCollectionSearchDTO search);


        /// <summary>
        /// 根据用户ID查询收藏商品数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionComsCount
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCollectionComsCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetCollectionComsCount(SetCollectionSearchDTO search);


        /// <summary>
        /// 根据用户ID查询收藏商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionApps
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCollectionApps", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> GetCollectionApps(SetCollectionSearchDTO search);


        /// <summary>
        /// 根据用户ID查询收藏店铺数量
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/GetCollectionAppsCount
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCollectionAppsCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetCollectionAppsCount(SetCollectionSearchDTO search);

        /// <summary>
        /// 删除正品会收藏
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.SetCollectionSV.svc/DeleteCollections
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCollections", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCollections(SetCollectionSearchDTO search);

        /// <summary>
        /// 校验是否收藏店铺
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckAppCollected", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckAppCollected(Guid channelId,Guid userId, Guid appId);
    }
}
