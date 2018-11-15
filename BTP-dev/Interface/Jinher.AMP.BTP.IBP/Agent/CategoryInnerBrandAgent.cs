
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/22 10:15:41
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

    public class CategoryInnerBrandAgent : BaseBpAgent<ICategoryInnerBrand>, ICategoryInnerBrand
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Add(Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.Add(model);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO>> GetBrandWallList(System.Guid CategoryId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetBrandWallList(CategoryId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCateBrandsByCategoryId(System.Guid CategoryId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCateBrandsByCategoryId(CategoryId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddList(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO> models)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddList(models);

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
