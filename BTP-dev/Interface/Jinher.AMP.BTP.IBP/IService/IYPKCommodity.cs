
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/8/29 18:56:38
***************/
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using System.Collections.Generic;


namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IYPKCommodity : ICommand
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AddYPKCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddYPKCommodity(JdCommodityDTO input);
        /// <summary>
        /// 导入易派客商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ImportYPKCommodityData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> ImportYPKCommodityData(List<JdCommodityDTO> JdComList, Guid AppId);
        /// <summary>
        /// 自动同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoSyncYPKCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> AutoSyncYPKCommodityInfo(Guid AppId, List<Guid> Ids);
        /// <summary>
        /// 全量同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param> 
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoYPKSyncCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> AutoYPKSyncCommodity(Guid AppId);
    }
}
