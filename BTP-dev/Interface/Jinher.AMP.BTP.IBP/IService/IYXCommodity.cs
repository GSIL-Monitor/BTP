/***************
功能描述: BTPIService
作    者:  LSH
创建时间: 2017/9/21 10:52:36
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
    public interface IYXCommodity : ICommand
    {
        /// <summary>
        /// 新建商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AddYXCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddYXCommodity(JdCommodityDTO input);
        /// <summary>
        /// 导入严选商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ImportYXCommodityData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> ImportYXCommodityData(List<JdCommodityDTO> JdComList, Guid AppId);
        /// <summary>
        /// 自动同步严选商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoSyncYXCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> AutoSyncYXCommodityInfo(Guid AppId, List<Guid> Ids);
    }
}
