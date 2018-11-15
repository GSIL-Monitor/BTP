
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/1/31 18:19:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy; 
using System.ServiceModel.Activation;
using System.Diagnostics;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using NPOI.OpenXmlFormats.Shared;
using CommodityDistributionDTO = Jinher.AMP.BTP.Deploy.CommodityDistributionDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class DistributeBP : BaseBP, IDistribute
    {

        /// <summary>
        /// 分销商数量和层级
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ManageNumDTO ManageNc(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            base.Do();
            return this.ManageNcExt(manaDTO);
        }

        /// <summary>
        /// 分销商列表信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ManageDTO ManageInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.ManageInfoExt(manaDTO);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.ManageInfo：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取分销商申请设置
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO GetDistributRuleFull(
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributRuleFullExt(search);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributRuleFull：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取用户申请资料信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO GetDistributRuleFullDTOByAppId_Mobile(
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributRuleFullDTOByAppId_MobileExt(search);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributRuleFullDTOByAppId_Mobile：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取分销商申请设置
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ModifyDistributRuleFull(
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO distributRuleFullDto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.ModifyDistributRuleFullExt(distributRuleFullDto);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.ModifyDistributRuleFull：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 查询 分销商申请 的身份设置
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public List<DistributionIdentityDTO> GetDistributorApplyIdentityVals(Guid applyId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorApplyIdentityValsExt(applyId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(
                string.Format("DistributeBP.GetDistributorApplyIdentityVals：耗时：{0}。", timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public DistributionApplyDTO GetDistributionApply(Guid applyId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionApplyExt(applyId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributionApply：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionApplyExt(ruleId, userId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributionApply：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 查询一条 分销商申请资料设置 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public DistributionRuleDTO GetDistributionRule(Guid ruleId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionRuleExt(ruleId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributionRule：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 查询 分销商申请资料设置 的身份设置
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public List<DistributionIdentitySetDTO> GetDistributionIdentitySets(Guid ruleId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionIdentitySetsExt(ruleId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributionIdentitySets：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionApplyListExt(appId, pageSize, pageIndex, out rowCount);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributionApplyList：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionApplyListByWhereExt(appId, pageSize, pageIndex, out rowCount, userName,
                state);
            //Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("GetDistributionApplyListByWhereExt(userName:{0},state:{1})，最终返回：{2}条记录。", userName, state, rowCount));

            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(
                string.Format("DistributeBP.GetDistributionApplyListByWhere：耗时：{0}。", timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 添加申请成为分销商资料
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userId"></param>
        /// <param name="ruleId"></param>
        /// <param name="distributApplyFullDto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributionIdentityInfo(string userCode, string userId, string ruleId,
            DistributApplyFullDTO distributApplyFullDto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AddDistributionIdentityInfoExt(userCode, userId, ruleId, distributApplyFullDto);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.AddDistributionIdentityInfo：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 添加申请成为分销商资料
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultDTO GetDistributeState(Guid appId, Guid userId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributeStateExt(appId, userId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributeState：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateDistributionApplyStateExt(appId, userId, distributeApplyStateEnum);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.UpdateDistributionApplyState：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.RemarkDistributionApplyExt(id, remarks);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.RemarkDistributionApply：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }
         
        /// <summary>
        /// 获取分销商申请的审批历史记录
        /// </summary>
        /// <param name="applyId"></param>
        public List<DistributionApplyAuditListDTO> GetApplyAuditList(Guid applyId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetApplyAuditListExt(applyId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetApplyAuditList：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.RemarkDistributorExt(id, remarks);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.RemarkDistributor：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AuditingDistributionApplyExt(id, isPass, refuseReason);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.AuditingDistributionApply：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }


        /// <summary>
        /// 获取一个分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributorDTO GetDistributorBy(Guid appId, Guid userId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorByExt(appId, userId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributorBy：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 更新微小店QrCode
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopQrCode(UpdateQrCodeRequestDTO dto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateMicroshopQrCodeExt(dto);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.UpdateMicroshopQrCode：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 新增分销商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributor(DistributorDTO dto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.AddDistributorExt(dto);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.AddDistributor：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }
        
        /// <summary>
        /// 获取分销商的所有身份设置
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public List<DistributorsHasIdentityResultDTO> GetDistributorsIdentitys(List<Guid> distributorList)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorsIdentitysExt(distributorList);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributorsIdentitys：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorsExt(appId, pageSize, pageIndex, out rowCount);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributors：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }
        /// <summary>
        /// 获取分销商们的备注信息
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public Dictionary<Guid,string> GetDistributorsRemarks(List<Guid> distributorList)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorsRemarksExt(distributorList);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributorsRemarks：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取分销商微小店信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributionMicroShopDTO GetDistributionMicroShop(Guid appId, Guid userId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributionMicroShopExt(appId, userId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetDistributionMicroShop：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 修改微小店信息
        /// </summary>
        /// <param name="microshopDto"></param>
        /// <returns></returns>
        public ResultDTO UpdateDistributionMicroShop(MicroshopDTO microshopDto)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateDistributionMicroShopExt(microshopDto);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.UpdateDistributionMicroShop：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 微小店 下架商品
        /// </summary>
        /// <param name="microshopComDto"></param>
        /// <returns></returns>
        public ResultDTO SaveMicroshopCom(MicroshopComDTO microshopComDto)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveMicroshopComExt(microshopComDto);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.SaveMicroshopCom：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }


        /// <summary>
        /// 微小店 上架商品
        /// </summary>
        /// <param name="microshopComId"></param>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopCom(Guid microshopComId, Guid microshopId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateMicroshopComExt(microshopComId, microshopId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.UpdateMicroshopCom：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 根据Id获取微小店
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshop(Guid microshopId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetMicroshopExt(microshopId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetMicroshop：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取微小店
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshop(Guid appId, Guid userId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetMicroshopExt(appId, userId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetMicroshop：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取某应用的所有加入微小店的商品Id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<CommodityDistributionDTO> GetAppAllMicroshopCommoditys(Guid appId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetAppAllMicroshopCommoditysExt(appId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetAppAllMicroshopCommoditys：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }

        /// <summary>
        /// 获取某微小店的所有下架商品Id
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public List<Guid> GetMicroshopOfflineCommodityIds(Guid microshopId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetMicroshopOfflineCommodityIdsExt(microshopId);
            timer.Stop();
            Jinher.JAP.Common.Loging.LogHelper.Debug(string.Format("DistributeBP.GetMicroshopOfflineCommodityIds：耗时：{0}。",
                timer.ElapsedMilliseconds));
            return result;
        }
    }
}
