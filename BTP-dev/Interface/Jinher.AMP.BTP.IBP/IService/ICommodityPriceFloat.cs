/***************
功能描述: BTPIService
作    者: LSH
创建时间: 2018/6/12 18:58:05
***************/
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 自动调价设置
    /// </summary>
    [ServiceContract]
    public interface ICommodityPriceFloat : ICommand
    {
        /// <summary>
        /// 获取自动调价设置数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDataList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommodityPriceFloatList<CommodityPriceFloatListDto>> GetDataList(Guid appId);

        /// <summary>
        /// 添加自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/Add", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Add(CommodityPriceFloatDTO dto);

        /// <summary>
        /// 修改自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/Update", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Update(CommodityPriceFloatDTO dto);

        /// <summary>
        /// 删除自动调价设置
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/Delete", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Delete(Guid id);

        /// <summary>
        /// 获取商城下所有的app信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetApps", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Guid> GetApps(Guid esAppId);
    }
}
