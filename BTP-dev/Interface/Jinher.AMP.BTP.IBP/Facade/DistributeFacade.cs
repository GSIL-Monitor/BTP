
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/1/31 18:19:17
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.IService;
using CommodityDistributionDTO = Jinher.AMP.BTP.Deploy.CommodityDistributionDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class DistributeFacade : BaseFacade<IDistribute>
    {

        /// <summary>
        /// 分销商数量和层级
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ManageNumDTO ManageNc(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            base.Do();
            return this.Command.ManageNc(manaDTO);
        }

        /// <summary>
        /// 分销商列表信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ManageDTO ManageInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            base.Do();

            return this.Command.ManageInfo(manaDTO);
        }

        /// <summary>
        /// 获取分销商申请设置
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO GetDistributRuleFull(
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            return this.Command.GetDistributRuleFull(search);
        }

        /// <summary>
        /// 获取用户申请资料信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO GetDistributRuleFullDTOByAppId_Mobile(
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            return this.Command.GetDistributRuleFullDTOByAppId_Mobile(search);
        }

        /// <summary>
        /// 添加或者编辑分销商设置
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ModifyDistributRuleFull(
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO distributRuleFullDto)
        {
            base.Do();
            return this.Command.ModifyDistributRuleFull(distributRuleFullDto);

        }

        /// <summary>
        /// 查询 分销商申请 的身份设置
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public List<DistributionIdentityDTO> GetDistributorApplyIdentityVals(Guid applyId)
        {
            base.Do();
            return this.Command.GetDistributorApplyIdentityVals(applyId);

        }

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public DistributionApplyDTO GetDistributionApply(Guid applyId)
        {
            base.Do();
            return this.Command.GetDistributionApply(applyId);
        }

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributionApplyDTO GetDistributionApply(Guid ruleId, Guid userId)
        {
            base.Do();
            return this.Command.GetDistributionApply(ruleId, userId);
        }

        /// <summary>
        /// 查询一条 分销商申请资料设置 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public DistributionRuleDTO GetDistributionRule(Guid ruleId)
        {
            base.Do();
            return this.Command.GetDistributionRule(ruleId);
        }

        /// <summary>
        /// 查询 分销商申请资料设置 的身份设置
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public List<DistributionIdentitySetDTO> GetDistributionIdentitySets(Guid ruleId)
        {
            base.Do();
            return this.Command.GetDistributionIdentitySets(ruleId);
        }

        /// <summary>
        /// 查询某个app的 分销商申请 列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<DistributionApplyResultDTO> GetDistributionApplyList(Guid appId, int pageSize, int pageIndex,
            out int rowCount)
        {
            base.Do();
            return this.Command.GetDistributionApplyList(appId, pageSize, pageIndex, out rowCount);
        }

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
        public List<DistributionApplyResultDTO> GetDistributionApplyListByWhere(Guid appId, int pageSize, int pageIndex,
            out int rowCount, string userName, int state)
        {
            base.Do();
            return this.Command.GetDistributionApplyListByWhere(appId, pageSize, pageIndex, out rowCount, userName,
                state);
        }
        
        /// <summary>
        /// 备注 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public ResultDTO RemarkDistributionApply(Guid id, string remarks)
        {
            base.Do();
            return this.Command.RemarkDistributionApply(id, remarks);
        }
        
        /// <summary>
        /// 获取分销商申请的审批历史记录
        /// </summary>
        /// <param name="applyId"></param>
        public List<DistributionApplyAuditListDTO> GetApplyAuditList(Guid applyId)
        {
            base.Do();
            return this.Command.GetApplyAuditList(applyId);
        }
        
        /// <summary>
        /// 备注 分销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public ResultDTO RemarkDistributor(Guid id, string remarks)
        {
            base.Do();
            return this.Command.RemarkDistributor(id, remarks);
        }

        /// <summary> 
        /// 审核 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPass"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public ResultDTO<AuditDistributionResultDTO> AuditingDistributionApply(Guid id, bool isPass, string refuseReason)
        {
            base.Do();
            return this.Command.AuditingDistributionApply(id, isPass, refuseReason);
        }

        /// <summary>
        /// 获取一个分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributorDTO GetDistributorBy(Guid appId, Guid userId)
        {
            base.Do();
            return this.Command.GetDistributorBy(appId, userId);
        }
        
        /// <summary>
        /// 更新微小店QrCode
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopQrCode(UpdateQrCodeRequestDTO dto)
        {
            base.Do();
            return this.Command.UpdateMicroshopQrCode(dto);
        }

        /// <summary>
        /// 新增分销商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributor(DistributorDTO dto)
        {
            base.Do();
            return this.Command.AddDistributor(dto);
        }

        /// <summary>
        /// 变更审核状态
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="distributeApplyStateEnum"></param>
        /// <returns></returns>
        public ResultDTO UpdateDistributionApplyState(Guid appId, Guid userId, DistributeApplyStateEnum distributeApplyStateEnum)
        {
            base.Do();
            return this.Command.UpdateDistributionApplyState(appId, userId, distributeApplyStateEnum);
        }

        /// <summary>
        /// 添加申请成为分销商资料
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddDistributionIdentityInfo(string userCode, string userId,
            string ruleId, DistributApplyFullDTO distributApplyFullDto)
        {
            base.Do();
            return this.Command.AddDistributionIdentityInfo(userCode, userId, ruleId, distributApplyFullDto);
        }


        /// <summary>
        /// 获取审核资料状态
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO GetDistributeState(Guid appId, Guid userId)
        {
            base.Do();
            return this.Command.GetDistributeState(appId, userId);
        }

        /// <summary>
        /// 获取分销商的所有身份设置
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public List<DistributorsHasIdentityResultDTO> GetDistributorsIdentitys(List<Guid> distributorList)
        {
            base.Do();
            return this.Command.GetDistributorsIdentitys(distributorList);
        }

        /// <summary>
        /// 查询 分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<DistributorDTO> GetDistributors(Guid appId, int pageSize,int pageIndex,out int rowCount)
        {
            base.Do();
            return this.Command.GetDistributors(appId, pageSize, pageIndex, out rowCount);
        }


        /// <summary>
        /// 获取分销商们的备注信息
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public Dictionary<Guid,string> GetDistributorsRemarks(List<Guid> distributorList)
        {
            base.Do();
            return this.Command.GetDistributorsRemarks(distributorList);
        }


        /// <summary>
        /// 获取分销商微小店信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributionMicroShopDTO GetDistributionMicroShop(Guid appId, Guid userId)
        {
            base.Do();
            return this.Command.GetDistributionMicroShop(appId, userId);
        }

        /// <summary>
        /// 修改微小店信息
        /// </summary>
        /// <param name="microshopDto"></param>
        /// <returns></returns>
        public ResultDTO UpdateDistributionMicroShop(MicroshopDTO microshopDto)
        {
            base.Do();
            return this.Command.UpdateDistributionMicroShop(microshopDto);
        }

        /// <summary>
        /// 微小店 下架商品
        /// </summary>
        /// <param name="microshopComDto"></param>
        /// <returns></returns>
        public ResultDTO SaveMicroshopCom(MicroshopComDTO microshopComDto)
        {
            base.Do();
            return this.Command.SaveMicroshopCom(microshopComDto);
        }

        /// <summary>
        /// 微小店 上架商品
        /// </summary>
        /// <param name="microshopComId"></param>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopCom(Guid microshopComId, Guid microshopId)
        {
            base.Do();
            return this.Command.UpdateMicroshopCom(microshopComId, microshopId);
        }
        
        /// <summary>
        /// 根据Id获取微小店
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshop(Guid microshopId)
        {
            base.Do();
            return this.Command.GetMicroshop(microshopId);
        }

        /// <summary>
        /// 获取微小店
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshop(Guid appId, Guid userId)
        {
            base.Do();
            return this.Command.GetMicroshop(appId, userId);
        }

        /// <summary>
        /// 获取某应用的所有加入微小店的商品Id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<CommodityDistributionDTO> GetAppAllMicroshopCommoditys(Guid appId)
        {
            base.Do();
            return this.Command.GetAppAllMicroshopCommoditys(appId);
        }

        /// <summary>
        /// 获取某微小店的所有下架商品Id
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public List<Guid> GetMicroshopOfflineCommodityIds(Guid microshopId)
        {
            base.Do();
            return this.Command.GetMicroshopOfflineCommodityIds(microshopId);
        }
    }
}