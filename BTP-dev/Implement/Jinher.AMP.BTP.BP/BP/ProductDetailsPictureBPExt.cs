
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/26 13:49:41
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ProductDetailsPictureBP : BaseBP, IProductDetailsPicture
    {

        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityIdExt(System.Guid commodityId)
        {
            var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodityId);

            var result = from p in productDetailsPictures
                         select new ProductDetailsPictureDTO
                         {
                             Code = p.Code,
                             CommodityId = p.CommodityId,
                             Id = p.Id,
                             Name = p.Name,
                             PicturesPath = p.PicturesPath,
                             Sort = p.Sort
                         };

            return result.ToList();
        }

        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public ResultDTO DeletePictures(System.Guid commodityId)
        {
            try
            {
                List<ProductDetailsPicture> productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodityId).ToList();
                ProductDetailsPicture pdp = new ProductDetailsPicture();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (ProductDetailsPicture item in productDetailsPictures)
                {
                    item.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(item);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("删除消息服务异常。commodityId：{0}", commodityId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }


    }
}