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
    /// App自提点类
    /// </summary>
    [ServiceContract]
    public interface IAppSelfTakeStation : ICommand
    {
        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="model">自提点实体</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveAppSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model);

        /// <summary>
        /// 修改自提点
        /// </summary>
        /// <param name="model">自提点实体</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAppSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateAppSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO model);

        /// <summary>
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteAppSelfTakeStations", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStations(System.Collections.Generic.List<System.Guid> ids);

       /// <summary>
        /// 查询自提点信息
       /// </summary>
       /// <param name="search">查询类</param>
       /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppSelfTakeStationList", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchResultSDTO GetAppSelfTakeStationList(Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSearchSDTO search);

        /// <summary>
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppSelfTakeStationById", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationSDTO GetAppSelfTakeStationById(System.Guid id);

        /// <summary>
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckUserIdExists", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckUserIdExists(System.Guid userId,System.Guid appId);
       
    }
}
