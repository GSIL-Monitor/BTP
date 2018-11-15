
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/15 13:42:08
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

    public class CommodityCategoryAgent : BaseBpAgent<ICommodityCategory>, ICommodityCategory
    {
        public void AddCommodityCategory(Jinher.AMP.BTP.Deploy.CommodityCategoryDTO commodityCategoryDTO)
        {

            try
            {
                //调用代理方法
                base.Channel.AddCommodityCategory(commodityCategoryDTO);

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
            }
        }
        public void DeleteCommodityCategory(System.Guid commodityId)
        {

            try
            {
                //调用代理方法
                base.Channel.DeleteCommodityCategory(commodityId);

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
            }
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetCommodityCategoryByCategory(System.Guid categoryId, int pageSize, int pageIndex)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCategoryByCategory(categoryId, pageSize, pageIndex);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetAllCommodityCategory()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityCategory();

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetAllCommodityCategoryByAppId(Guid appId, Guid commodityId, int pageSize, int pageIndex)
        { 
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityCategory();

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
