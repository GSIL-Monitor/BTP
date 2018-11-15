
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/25 17:24:12
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

    public class BrandAgent : BaseBpAgent<IBrand>, IBrand
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddBrand(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddBrand(brandWallDto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrand(Jinher.AMP.BTP.Deploy.BrandwallDTO brandWallDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateBrand(brandWallDto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandList(string brandName, int status, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBrandList(brandName, status, appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> GetBrandPageList(string brandName, int status, int pageSize, int pageIndex, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.BrandwallDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBrandPageList(brandName, status, pageSize, pageIndex, appId);

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
        public bool CheckBrand(string brandName, out int rowCount, System.Guid appId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckBrand(brandName, out rowCount, appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateBrandStatus(System.Guid id, int status, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateBrandStatus(id, status, appId);

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
        public Jinher.AMP.BTP.Deploy.BrandwallDTO GetBrand(System.Guid id, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.BrandwallDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBrand(id, appId);

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
