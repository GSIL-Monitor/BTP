using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{
    [ServiceContract]
    public interface IDiyGroup : ICommand
    {
        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDiyGroupDetail", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> GetDiyGroupDetail(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailSearchDTO search);

        /// <summary>
        /// 处理超时未成团
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DealUnDiyGroupTimeout", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void DealUnDiyGroupTimeout();

        /// <summary>
        /// 处理 未成团退款
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DealUnDiyGroupRefund", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void DealUnDiyGroupRefund();
        /// <summary>
        /// 我的拼团订单列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDiyGroupList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> GetDiyGroupList(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search);

        /// <summary>
        /// 自动确认成团 -- JOB调用
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/VoluntarilyConfirmDiyGroup", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyConfirmDiyGroup();

        /// <summary>
        /// 成团自动退款 -- JOB调用
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/VoluntarilyRefundDiyGroup", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyRefundDiyGroup();

        /// <summary>
        /// 检查拼团状态
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckDiyGroup", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CheckDiyGroupOutputDTO> CheckDiyGroup(CheckDiyGroupInputDTO inputDTO);
    }
}
