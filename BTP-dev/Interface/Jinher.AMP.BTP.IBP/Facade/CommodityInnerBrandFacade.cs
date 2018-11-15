
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/23 16:12:15
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class CommodityInnerBrandFacade : BaseFacade<ICommodityInnerBrand>
    {

        /// <summary>
        /// 添加商品品牌
        /// </summary>
        /// <param name="brandWallDto">商品品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddComInnerBrand(Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO innerBrandDto)
        {
            base.Do();
            return this.Command.AddComInnerBrand(innerBrandDto);
        }
        /// <summary>
        /// 根据商品Id查询商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO GetComInnerBrand(System.Guid commodityId)
        {
            base.Do();
            return this.Command.GetComInnerBrand(commodityId);
        }
        /// <summary>
        /// 删除商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelComInnerBrand(System.Guid commodityId)
        {
            base.Do();
            return this.Command.DelComInnerBrand(commodityId);
        }
    }
}