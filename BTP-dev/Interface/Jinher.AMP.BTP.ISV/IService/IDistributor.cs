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
    public interface IDistributor : ICommand
    {
        /// <summary>
        /// 保存分销商关系
        /// </summary>
        /// <param name="distributor">分销用户关系</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveDistributorRelation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Guid> SaveDistributorRelation(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributor);

        /// <summary>
        /// 查询分销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorProfits", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO GetDistributorProfits(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search);



        /// <summary>
        ///更新分销商的用户信息。
        /// </summary>
        /// <param name="uinfo">用户信息</param>
        /// <returns>操作结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDistributorUserInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateDistributorUserInfo(UserSDTO uinfo);
        /// <summary>
        /// 判断应用是否有三级分销功能，用户是否为分销商
        /// </summary>
        /// <param name="cuinfo"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckDistributorUserInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CheckDistributorUserInfo(UserSDTO cuinfo);
        
        /// <summary>
        /// 获取分销商信息
        /// </summary>
        /// <param name="distributorUserRelationDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO GetDistributorInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributorUserRelationDTO);

        /// <summary>
        /// 获取分销商佣金入账信息
        /// </summary>
        /// <param name="distributeMoneySearch"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorMoneyInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO GetDistributorMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributeMoneySearch distributeMoneySearch);

        /// <summary>
        /// 分销信息校验
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UserDistributionCheck", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.UserDistributionCheckResultDTO UserDistributionCheck(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search);

        /// <summary>
        /// 获取分销商申请设置
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetApplySet", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributionIdentitySetFullDTO GetApplySet(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search);
        /// <summary>
        /// 获取用户分销商申请
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributApplyFullDTO GetApply(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search);
        /// <summary>
        /// 保存用户分销商申请
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveApply(Jinher.AMP.BTP.Deploy.CustomDTO.DistributApplyFullDTO dto);

        /// <summary>
        /// 同步正式环境历史数据使用 勿调用
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveMicroshop", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveMicroshop();
    }
}
