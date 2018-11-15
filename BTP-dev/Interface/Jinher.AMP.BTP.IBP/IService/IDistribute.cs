using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using CommodityDistributionDTO = Jinher.AMP.BTP.Deploy.CommodityDistributionDTO;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface IDistribute : ICommand
    {
        /// <summary>
        /// 分销商数量和层级
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ManageNc", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ManageNumDTO ManageNc(ManageVM manaDTO);
        /// <summary>
        /// 分销商列表信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ManageInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ManageDTO ManageInfo(ManageVM manaDTO);

        /// <summary>
        /// 获取分销商申请设置
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributRuleFull", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO GetDistributRuleFull(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search);

        /// <summary>
        /// 获取用户申请资料信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributRuleFullDTOByAppId_Mobile", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO GetDistributRuleFullDTOByAppId_Mobile(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search);

        /// <summary>
        /// 添加或者编辑分销商设置
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ModifyDistributRuleFull", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ModifyDistributRuleFull(Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO distributRuleFullDto);

        /// <summary>
        /// 查询 分销商申请 的身份设置
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorApplyIdentityVals", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributionIdentityDTO> GetDistributorApplyIdentityVals(Guid applyId);

        /// <summary>
        /// 变更审核状态
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="distributeApplyStateEnum"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDistributionApplyState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateDistributionApplyState(Guid appId, Guid userId, DistributeApplyStateEnum distributeApplyStateEnum);

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DistributionApplyDTO GetDistributionApply(Guid ruleId, Guid userId);

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DistributionApplyDTO GetDistributionApply(Guid applyId);

        /// <summary>
        /// 查询一条 分销商申请资料设置 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionRule", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DistributionRuleDTO GetDistributionRule(Guid ruleId);

        /// <summary>
        /// 查询 分销商申请资料设置 的身份设置
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionIdentitySets", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributionIdentitySetDTO> GetDistributionIdentitySets(Guid ruleId);

        /// <summary>
        /// 查询某个app的 分销商申请 列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionApplyList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributionApplyResultDTO> GetDistributionApplyList(Guid appId, int pageSize, int pageIndex,
            out int rowCount);

        /// <summary>
        /// 查询某个app符合条件的 分销商申请 列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="userName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionApplyListByWhere", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributionApplyResultDTO> GetDistributionApplyListByWhere(Guid appId, int pageSize, int pageIndex,
            out int rowCount, string userName, int state);

        /// <summary>
        /// 备注 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RemarkDistributionApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RemarkDistributionApply(Guid id, string remarks);

        /// <summary>
        /// 获取分销商申请的审批历史记录
        /// </summary>
        /// <param name="applyId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetApplyAuditList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributionApplyAuditListDTO> GetApplyAuditList(Guid applyId);

        /// <summary>
        /// 备注 分销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RemarkDistributor", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RemarkDistributor(Guid id, string remarks);

        /// <summary>
        /// 审核 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPass"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AuditingDistributionApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<AuditDistributionResultDTO> AuditingDistributionApply(Guid id, bool isPass, string refuseReason);

        /// <summary>
        /// 获取一个分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorBy", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DistributorDTO GetDistributorBy(Guid appId, Guid userId);


        /// <summary>
        /// 更新微小店QrCode
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateMicroshopQrCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateMicroshopQrCode(UpdateQrCodeRequestDTO dto);

        /// <summary>
        /// 新增分销商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddDistributor", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddDistributor(DistributorDTO dto);

        /// <summary>
        /// 添加申请成为分销商资料
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userId"></param>
        /// <param name="ruleId"></param>
        /// <param name="distributApplyFullDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddDistributionIdentityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddDistributionIdentityInfo(string userCode, string userId, string ruleId, DistributApplyFullDTO distributApplyFullDto);

        /// <summary>
        /// 获取审核资料状态
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributeState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO GetDistributeState(Guid appId, Guid userId);

        /// <summary>
        /// 获取分销商的所有身份设置
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorsIdentitys", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributorsHasIdentityResultDTO> GetDistributorsIdentitys(List<Guid> distributorList);

        /// <summary>
        /// 查询 分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributors", BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DistributorDTO> GetDistributors(Guid appId, int pageSize, int pageIndex, out int rowCount);

        /// <summary>
        /// 获取分销商们的备注信息
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributorsRemarks", BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Dictionary<Guid, string> GetDistributorsRemarks(List<Guid> distributorList);


        /// <summary>
        /// 获取分销商微小店信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDistributionMicroShop", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DistributionMicroShopDTO GetDistributionMicroShop(Guid appId, Guid userId);

        /// <summary>
        /// 修改微小店信息
        /// </summary>
        /// <param name="microshopDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDistributionMicroShop", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateDistributionMicroShop(MicroshopDTO microshopDto);

        /// <summary>
        /// 微小店 下架商品
        /// </summary>
        /// <param name="microshopComDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveMicroshopCom", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveMicroshopCom(MicroshopComDTO microshopComDto);

        /// <summary>
        /// 微小店 上架商品
        /// </summary>
        /// <param name="microshopComId"></param>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateMicroshopCom", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateMicroshopCom(Guid microshopComId, Guid microshopId);

        /// <summary>
        /// 根据Id获取微小店
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMicroshop", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        MicroshopDTO GetMicroshop(Guid microshopId);

        /// <summary>
        /// 获取微小店
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMicroshop", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        MicroshopDTO GetMicroshop(Guid appId, Guid userId);

        /// <summary>
        /// 获取某应用的所有加入微小店的商品Id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppAllMicroshopCommoditys", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityDistributionDTO> GetAppAllMicroshopCommoditys(Guid appId);

        /// <summary>
        /// 获取某微小店的所有下架商品Id
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMicroshopOfflineCommodityIds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Guid> GetMicroshopOfflineCommodityIds(Guid microshopId);
    }

}
