using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.JAP.BF.SV.Base;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.SV
{
     [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
   public partial class ProductDetailsPictureSV: BaseSv,IProductDetailsPicture
    {
        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(Guid commodityId)
        {
            base.Do(false);
            return this.GetProductDetailsPictureByCommodityIdExt(commodityId);
        }
    }
}
