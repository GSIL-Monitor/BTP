using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy;


namespace Jinher.AMP.BTP.ISV.Agent
{
    public class ProductDetailsPictureAgent : BaseBpAgent<IProductDetailsPicture>, IProductDetailsPicture
    {
        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public List<ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(Guid commodityId)
        {
            //定义返回值
            List<ProductDetailsPictureDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetProductDetailsPictureByCommodityId(commodityId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
