
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/6/15 13:57:20
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
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
    public partial class BrandSV : BaseSv, IBrand
    {

        /// <summary>
        /// 根据一级分类ID获取品牌墙信息（热门品牌）
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.BrandwallDTO>> getBrandByCateID(System.Guid CategoryID)
        {
            base.Do(false);
            return this.getBrandByCateIDExt(CategoryID);

        }
        /// <summary>
        /// 获取指定品牌下的商品
        /// </summary>
        /// <param name="BrandID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CommodityDTO>> getBrandCommodity(System.Guid BrandID)
        {
            base.Do();
            return this.getBrandCommodityExt(BrandID);

        }
    }
}