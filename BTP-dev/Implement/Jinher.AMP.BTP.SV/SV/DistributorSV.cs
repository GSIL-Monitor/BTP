
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/2/15 11:37:51
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class DistributorSV : BaseSv, IDistributor
    {

        /// <summary>
        /// 保存分销商关系
        /// </summary>
        /// <param name="distributor">分销用户关系</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Guid> SaveDistributorRelation(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributor)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveDistributorRelationExt(distributor);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.SaveDistributorRelation：耗时：{0}。入参：distributor:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(distributor), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 查询分销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO GetDistributorProfits(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorProfitsExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.GetDistributorProfits：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 更新分销商的用户信息。
        ///  </summary>
        /// <param name="uinfo">用户信息</param>
        /// <returns>操作结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDistributorUserInfo(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO uinfo)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateDistributorUserInfoExt(uinfo);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.UpdateDistributorUserInfo：耗时：{0}。入参：uinfo:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(uinfo), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 判断应用是否有三级分销功能，用户是否为分销商
        /// </summary>
        /// <param name="cuinfo"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckDistributorUserInfo(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO cuinfo)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CheckDistributorUserInfoExt(cuinfo);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.CheckDistributorUserInfo：耗时：{0}。入参：cuinfo:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(cuinfo), JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 获取分销商信息
        /// </summary>
        /// <param name="distributorUserRelationDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO GetDistributorInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributorUserRelationDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorInfoExt(distributorUserRelationDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.GetDistributorInfo：耗时：{0}。入参：distributorUserRelationDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(distributorUserRelationDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取分销商佣金入账信息
        /// </summary>
        /// <param name="distributeMoneySearch"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO GetDistributorMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributeMoneySearch distributeMoneySearch)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetDistributorMoneyInfoExt(distributeMoneySearch);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.GetDistributorMoneyInfo：耗时：{0}。入参：distributeMoneySearch:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(distributeMoneySearch), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 分销信息校验
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public UserDistributionCheckResultDTO UserDistributionCheck(DistributionSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UserDistributionCheckExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.UserDistributionCheck：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        public DistributionIdentitySetFullDTO GetApplySet(DistributionSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetApplySetExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.GetApplySet：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        public DistributApplyFullDTO GetApply(DistributionSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetApplyExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.UserDistributionCheck：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        public ResultDTO SaveApply(DistributApplyFullDTO dto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveApplyExt(dto);
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.SaveApply：耗时：{0}。入参：dto:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(dto), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 同步正式环境历史数据使用 勿调用
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveMicroshop()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveMicroshopExt();
            timer.Stop();
            LogHelper.Debug(string.Format("DistributorSV.SaveMicroshop：耗时：{0}。出参：{1}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(result)));
            return result;
        }
    }
}
