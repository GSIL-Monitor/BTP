
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/12 20:29:52
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class CategoryAgent : BaseBpAgent<ICategory>, ICategory
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategory(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategory(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategory2(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategory2(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategoryByDrawer(System.Guid appId, out int levelCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryByDrawer(appId, out levelCount);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityList(commodityListInfer);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> CheckIsShowSearchMenu(Jinher.AMP.BTP.Deploy.CustomDTO.CategorySearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<bool> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckIsShowSearchMenu(search);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCache2DTO> GetCacheCateGories(System.Guid appid)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCache2DTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCacheCateGories(appid);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommodityCategory(System.Guid belongTo, System.Collections.Generic.List<System.Guid> appList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCommodityCategory(belongTo, appList);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityLisByBeLongTo(Jinher.AMP.ZPH.Deploy.CustomDTO.ComdtySearch4SelCDTO comdtySearch4SelCdto, out int comdtyCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityLisByBeLongTo(comdtySearch4SelCdto, out comdtyCount);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryL1(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryL1(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityListV2(commodityListInfer);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityFilterList(commodityListInfer);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterListSecond(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListInferSearchDTO commodityListInfer)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityFilterListSecond(commodityListInfer);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetBrandAndAdvertise(System.Guid CategoryID)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBrandAndAdvertise(CategoryID);

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
        /// <summary>
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        /// 
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto> GetZshCategories()
        {
            //定义返回值
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetZshCategories();

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
