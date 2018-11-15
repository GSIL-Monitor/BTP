
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/4/22 10:49:49
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

    public class JdCommodityAgent : BaseBpAgent<IJdCommodity>, IJdCommodity
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> GetJdCommodityList(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJdCommodityList(input);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddJdCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddJdCommodity(input);

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
        public Jinher.AMP.BTP.Deploy.JdCommodityDTO GetJdCommodityInfo(System.Guid Id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.JdCommodityDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJdCommodityInfo(Id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelJdCommodityAll(System.Collections.Generic.List<System.Guid> Ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelJdCommodityAll(Ids);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> ExportJdCommodityData(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.ExportJdCommodityData(input);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportJdCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ImportJdCommodityData(JdComList, AppId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.AutoSyncCommodityInfo(AppId, Ids);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.InnerCategoryDTO> GetCategories(System.Guid AppId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.InnerCategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategories(AppId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryList(System.Guid AppId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryList(AppId);

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
