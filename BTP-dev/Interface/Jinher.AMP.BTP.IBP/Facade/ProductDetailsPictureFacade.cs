
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/3/26 13:49:38
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
    public class ProductDetailsPictureFacade : BaseFacade<IProductDetailsPicture>
    {

        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(System.Guid commodityId)
        {
            base.Do();
            return this.Command.GetProductDetailsPictureByCommodityId(commodityId);
        }
        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeletePicture(System.Guid commodityId)
        {
            base.Do();
            return this.Command.DeletePicture(commodityId);
        }
    }
}
