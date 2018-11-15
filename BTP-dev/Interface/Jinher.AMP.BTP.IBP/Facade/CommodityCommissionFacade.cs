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
    public class CommodityCommissionFacade : BaseFacade<ICommodityCommission>
    {
        /// <summary>
        /// 查询商品佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> GetCommodityCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO search)
        {
            base.Do();
            return this.Command.GetCommodityCommissionList(search);
        }
        /// <summary>
        /// 保存商品佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            base.Do();
            return this.Command.SaveCommodityCommission(model);
        }


        /// <summary>
        /// 修改商品佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            base.Do();
            return this.Command.UpdateCommodityCommission(model);
        }

        /// <summary>
        /// 删除商品佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            base.Do();
            return this.Command.DelCommodityCommission(model);
        }

        /// <summary>
        /// 根据id获取商品佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO GetCommodityCommission(Guid id)
        {
            base.Do();
            return this.Command.GetCommodityCommission(id);
        }
    }
}
