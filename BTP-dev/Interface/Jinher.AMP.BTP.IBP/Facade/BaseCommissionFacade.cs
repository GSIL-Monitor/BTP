/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/5/29 11:37:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class BaseCommissionFacade : BaseFacade<IBaseCommission>
    {
        /// <summary>
        /// 查询佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> GetBaseCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO search)
        {
            base.Do();
            return this.Command.GetBaseCommissionList(search);
        }
        /// <summary>
        /// 保存佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            base.Do();
            return this.Command.SaveBaseCommission(model);
        }


        /// <summary>
        /// 修改佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            base.Do();
            return this.Command.UpdateBaseCommission(model);
        }

        /// <summary>
        /// 删除佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            base.Do();
            return this.Command.DelBaseCommission(model);
        }


        /// <summary>
        /// 根据id获取基础佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO GetBaseCommission(Guid id, Guid mallApplyId)
        {
            base.Do();
            return this.Command.GetBaseCommission(id, mallApplyId);
        }
    }
}
