
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/26 13:49:40
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
    public partial class ProductDetailsPictureBP : BaseBP, IProductDetailsPicture
    {

        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(System.Guid commodityId)
        {
            base.Do();
            return this.GetProductDetailsPictureByCommodityIdExt(commodityId);
        }
        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeletePicture(System.Guid commodityId)
        {
            base.Do();
            return this.DeletePictures(commodityId);
        }
    }
}
