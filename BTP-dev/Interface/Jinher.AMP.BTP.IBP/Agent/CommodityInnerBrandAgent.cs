
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/23 16:12:22
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

    public class CommodityInnerBrandAgent : BaseBpAgent<ICommodityInnerBrand>, ICommodityInnerBrand
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddComInnerBrand(Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO innerBrandDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddComInnerBrand(innerBrandDto);

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
        public Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO GetComInnerBrand(System.Guid commodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CommodityInnerBrandDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetComInnerBrand(commodityId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelComInnerBrand(System.Guid commodityId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelComInnerBrand(commodityId);

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
