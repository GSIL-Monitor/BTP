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
    public class CategoryCommissionFacade : BaseFacade<ICategoryCommission>
    {
        /// <summary>
        /// 查询类别佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO> GetCategoryCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO search)
        {
            base.Do();
            return this.Command.GetCategoryCommissionList(search);
        }
        /// <summary>
        /// 保存类别佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            base.Do();
            return this.Command.SaveCategoryCommission(model);
        }


        /// <summary>
        /// 修改类别佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            base.Do();
            return this.Command.UpdateCategoryCommission(model);
        }

        /// <summary>
        /// 删除类别佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            base.Do();
            return this.Command.DelCategoryCommission(model);
        }

        /// <summary>
        /// 根据id获取类别佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO GetCategoryCommission(Guid id)
        {
            base.Do();
            return this.Command.GetCategoryCommission(id);
        }
    }
}
