
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/4/12 13:40:02
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

    [ServiceContract]
    public interface IBTPUser : ICommand
    {
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDTO">用户信息DTO</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateUser", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateUser(UserSDTO userDTO);


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUser", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        UserSDTO GetUser(Guid userId,Guid appId);

        /// <summary>
        /// 待自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>待自提订单数量</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSelfTakeManager", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<int> GetSelfTakeManager(Guid userId);

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetVipInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VipPromotionDTO> GetVipInfo(Guid userId, Guid appId);
    }
}
