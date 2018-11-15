using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 应用扩展（店铺扩展,需要有历史记录的扩展）
    /// </summary>
    [ServiceContract]
    public interface IScoreSetting : ICommand
    {
        /// <summary>
        /// 获取特定app在电商中的当前生效的扩展信息。
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetScoreSettingByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ScoreSettingDTO> GetScoreSettingByAppId(Guid appId);

        /// <summary>
        /// 获取特定app在电商中的当前生效的积分扩展信息。
        /// </summary>
        /// <param name="userId">当前用户id</param>
        /// <param name="appId">应用id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserScoreInApp", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<UserScoreDTO> GetUserScoreInApp(Param2DTO paramDto);

        /// <summary>
        /// 校验下单积分
        /// </summary>
        /// <param name="paramDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/OrderScoreCheck", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<OrderScoreCheckResultDTO> OrderScoreCheck(OrderScoreCheckDTO paramDto);
    }
}
