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
    /// 众销红包
    /// </summary>
    [ServiceContract]
    public interface IShareRedEnvelope : ICommand
    {

        /// <summary>
        /// 佣金结算
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/SettleCommossion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SettleCommossion();

        /// <summary>
        /// 发送红包
        /// </summary>

        [WebInvoke(Method = "POST", UriTemplate = "/SendRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendRedEnvelope();

        /// <summary>
        /// 发送红包
        /// </summary>
        //[WebInvoke(Method = "POST", UriTemplate = "/HandleInValidRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void HandleInValidRedEnvelope();

        /// <summary>
        /// 处理过期红包
        /// </summary>
        [OperationContract]
        void HandleCfInValidRedEnvelope();
        /// <summary>
        /// 获取红包
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO GetRedEnvelope(Guid redEnvelopeId);

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="userRedEnvelopeId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DrawRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawRedEnvelope(Guid userRedEnvelopeId);

        /// <summary>
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMyRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyRedEnvelope(Guid userId, int type, int pageIndex, int pageSize);

        /// <summary>
        /// 获取我的组织红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMyOrgRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyOrgRedEnvelope(Guid userId, int type, int pageIndex, int pageSize);

        [WebInvoke(Method = "POST", UriTemplate = "/UseRuleDescription", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UseRuleDescription(Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO ruleDescriptionDTO);

        [WebInvoke(Method = "POST", UriTemplate = "/GetRuleDescription", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO GetRuleDescription(Guid appId);

        /// <summary>
        /// 发送众筹红包
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/SendCfRedEnvelope", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void SendCfRedEnvelope();

        /// <summary>
        /// 获取众销明细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShareList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ShareListResult GetShareList(int pageSize, int pageIndex);
    }
}
