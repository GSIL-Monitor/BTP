using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.SV
{
    public partial class ProductDetailsPictureSV : BaseSv,IProductDetailsPicture
    {
        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityIdExt(Guid commodityId)
        {
            return new List<ProductDetailsPictureDTO>();
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
