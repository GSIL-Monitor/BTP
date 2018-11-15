using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Facade
{
   public class ProductDetailsPictureFacade :  BaseFacade<IProductDetailsPicture>
    {
        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(Guid commodityId)
        {
            base.Do();
            return this.Command.GetProductDetailsPictureByCommodityId(commodityId);
        }
    }
}
