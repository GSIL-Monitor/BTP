
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/15 13:57:18
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
    public class BrandFacade : BaseFacade<IBrand>
    {

        /// <summary>
        /// 根据一级分类ID获取品牌墙信息（热门品牌）
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.BrandwallDTO>> getBrandByCateID(System.Guid CategoryID)
        {
            base.Do();
            return this.Command.getBrandByCateID(CategoryID);
        }
        /// <summary>
        /// 获取指定品牌下的商品
        /// </summary>
        /// <param name="BrandID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CommodityDTO>> getBrandCommodity(System.Guid BrandID)
        {
            base.Do();
            return this.Command.getBrandCommodity(BrandID);
        }
    }
}