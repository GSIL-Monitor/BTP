
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/3/26 13:49:43
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class ProductDetailsPictureAgent : BaseBpAgent<IProductDetailsPicture>, IProductDetailsPicture
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(System.Guid commodityId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ProductDetailsPictureDTO> result;

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeletePicture(Guid commodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeletePicture(commodityId);

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
