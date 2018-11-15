
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/2/15 11:37:50
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class DistributorFacade : BaseFacade<IDistributor>
    {

        /// <summary>
        /// 保存分销商关系
        /// </summary>
        /// <param name="distributor">分销用户关系</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Guid> SaveDistributorRelation(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributor)
        {
            base.Do();
            return this.Command.SaveDistributorRelation(distributor);
        }
        /// <summary>
        /// 查询分销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO GetDistributorProfits(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search)
        {
            base.Do();
            return this.Command.GetDistributorProfits(search);
        }
        /// <summary>
        /// 更新分销商的用户信息。
        ///  </summary>
        /// <param name="uinfo">用户信息</param>
        /// <returns>操作结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDistributorUserInfo(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO uinfo)
        {
            base.Do();
            return this.Command.UpdateDistributorUserInfo(uinfo);
        }
        /// <summary>
        /// 判断应用是否有三级分销功能，用户是否为分销商
        /// </summary>
        /// <param name="cuinfo"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckDistributorUserInfo(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO cuinfo)
        {
            base.Do();
            return this.Command.CheckDistributorUserInfo(cuinfo);
        }
        /// <summary>
        /// 获取分销商信息
        /// </summary>
        /// <param name="distributorUserRelationDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO GetDistributorInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributorUserRelationDTO)
        {
            base.Do();
            return this.Command.GetDistributorInfo(distributorUserRelationDTO);
        }
        /// <summary>
        /// 获取分销商佣金入账信息
        /// </summary>
        /// <param name="distributeMoneySearch"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO GetDistributorMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.DistributeMoneySearch distributeMoneySearch)
        {
            base.Do();
            return this.Command.GetDistributorMoneyInfo(distributeMoneySearch);
        }
        /// <summary>
        /// 分销信息校验
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserDistributionCheckResultDTO UserDistributionCheck(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            return this.Command.UserDistributionCheck(search);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributionIdentitySetFullDTO GetApplySet(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            return this.Command.GetApplySet(search);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributApplyFullDTO GetApply(Jinher.AMP.BTP.Deploy.CustomDTO.DistributionSearchDTO search)
        {
            base.Do();
            return this.Command.GetApply(search);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveApply(Jinher.AMP.BTP.Deploy.CustomDTO.DistributApplyFullDTO dto)
        {
            base.Do();
            return this.Command.SaveApply(dto);
        }


        /// <summary>
        /// 同步正式环境历史数据使用 勿调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveMicroshop()
        {
            base.Do();
            return this.Command.SaveMicroshop();
        }
    }
}