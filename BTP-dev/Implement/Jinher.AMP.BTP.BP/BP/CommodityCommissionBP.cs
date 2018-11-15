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
    public partial class CommodityCommissionBP : BaseBP, ICommodityCommission
    {
        /// <summary>
        /// 查询商品佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> GetCommodityCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO search)
        {
            base.Do();
            return this.GetCommodityCommissionListExt(search);
        }
        /// <summary>
        /// 保存商品佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            base.Do();
            return this.SaveCommodityCommissionExt(model);
        }


        /// <summary>
        /// 修改商品佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            base.Do();
            return this.UpdateCommodityCommissionExt(model);
        }

        /// <summary>
        /// 删除商品佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            base.Do();
            return this.DelCommodityCommissionExt(model);
        }

        /// <summary>
        /// 根据id获取商品佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO GetCommodityCommission(Guid id)
        {
            base.Do();
            return this.GetCommodityCommissionExt(id);
        }
    }
}
