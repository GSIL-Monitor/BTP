using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 应用扩展（店铺扩展）
    /// </summary>
    [ServiceContract]
    public interface IAppExtension : ICommand
    {
        /// <summary>
        /// 更新应用扩展（店铺扩展）
        /// </summary>
        /// <param name="appExtDTO">应用扩展信息实体</param>
        /// <returns>操作结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAppExtension", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateAppExtension(Jinher.AMP.BTP.Deploy.AppExtensionDTO appExtDTO);


        /// <summary>
        /// 获取特定app在电商中的扩展信息。
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppExtensionByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Jinher.AMP.BTP.Deploy.AppExtensionDTO> GetAppExtensionByAppId(Guid appId);

        /// <summary>
        /// 获取app的积分抵现设置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetScoreSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AppScoreSettingDTO GetScoreSetting(Guid appId);
        /// <summary>
        /// 设置渠道佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetDefaultChannelAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaultChannelAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension);
        /// <summary>
        /// 获取渠道默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDefaulChannelAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulChannelAccount(Guid appId);

        /// <summary>
        /// 获取应用统计信息
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppStatistics", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DaMiWang.AppStatisticsDTO GetAppStatistics(Guid appId);
    }
}
