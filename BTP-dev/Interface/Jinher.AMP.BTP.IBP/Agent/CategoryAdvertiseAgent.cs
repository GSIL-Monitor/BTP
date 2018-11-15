
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/29 17:00:33
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

    public class CategoryAdvertiseAgent : BaseBpAgent<ICategoryAdvertise>, ICategoryAdvertise
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateCategoryAdvertise(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CreateCategoryAdvertise(entity);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>> CateGoryAdvertiseList(string advertiseName, int state, System.Guid CategoryId, int pageIndex, int pageSize, out int rowCount)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.CateGoryAdvertiseList(advertiseName, state, CategoryId, pageIndex, pageSize, out rowCount);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCategoryAdvertise(System.Guid advertiseId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCategoryAdvertise(advertiseId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditCategoryAdvertise(Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO entity)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.EditCategoryAdvertise(entity);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> GetCategoryAdvertise(System.Guid advertiseId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CategoryAdvertiseDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryAdvertise(advertiseId);

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
