
/***************
功能描述: BTPIService
作    者: 
创建时间: 2016/5/14 16:18:23
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 拼团
    /// </summary>
    [ServiceContract]
    public interface IDiyGroup : ICommand
    {
        /// <summary>
        /// 获取拼团信息（必传参数AppId、PageIndex、PageSize、State，可选参数ComNameSub）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDiyGroups", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<DiyGroupManageDTO> GetDiyGroups(DiyGroupSearchDTO search);
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ConfirmDiyGroup", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ConfirmDiyGroup(DiyGroupSearchDTO search);
        /// <summary>
        /// 确认成团(必传参数DiyGoupId)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ConfirmDiyGroup", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Refund(DiyGroupSearchDTO search);

        /// <summary>
        /// 获取 未完成的拼团列表
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "GET", UriTemplate = "/UnfinishedDiyGrouplist", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<UnfinishedDiyGroupOutputDTO>> UnfinishedDiyGrouplist(UnfinishedDiyGroupInputDTO inputDTO);
    }
}
