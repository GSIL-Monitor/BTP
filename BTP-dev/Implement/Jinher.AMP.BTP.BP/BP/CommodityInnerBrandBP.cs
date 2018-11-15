
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/23 16:12:18
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityInnerBrandBP : BaseBP, ICommodityInnerBrand
    {

        /// <summary>
        /// 添加商品品牌
        /// </summary>
        /// <param name="brandWallDto">商品品牌实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddComInnerBrand(Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO innerBrandDto)
        {
            base.Do(false);
            return this.AddComInnerBrandExt(innerBrandDto);
        }
        /// <summary>
        /// 根据商品Id查询商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO GetComInnerBrand(System.Guid commodityId)
        {
            base.Do(false);
            return this.GetComInnerBrandExt(commodityId);
        }
        /// <summary>
        /// 删除商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelComInnerBrand(System.Guid commodityId)
        {
            base.Do(false);
            return this.DelComInnerBrandExt(commodityId);
        }
    }
}