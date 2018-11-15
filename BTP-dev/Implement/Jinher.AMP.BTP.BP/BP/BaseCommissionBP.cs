/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:06
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class BaseCommissionBP : BaseBP, IBaseCommission
    {
        /// <summary>
        /// 查询佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> GetBaseCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO search)
        {
            base.Do();
            return this.GetBaseCommissionListExt(search);
        }
        /// <summary>
        /// 保存佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            base.Do();
            return this.SaveBaseCommissionExt(model);
        }


        /// <summary>
        /// 修改佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            base.Do();
            return this.UpdateBaseCommissionExt(model);
        }

        /// <summary>
        /// 删除佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            base.Do();
            return this.DelBaseCommissionExt(model);
        }

        /// <summary>
        /// 根据id获取基础佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO GetBaseCommission(Guid id, Guid mallApplyId)
        {
            base.Do();
            return this.GetBaseCommissionExt(id,mallApplyId);
        }
    }
}
