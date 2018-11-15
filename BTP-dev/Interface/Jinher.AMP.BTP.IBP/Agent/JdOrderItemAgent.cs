/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/9/21 15:02:29
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{   
    public class JdOrderItemAgent : BaseBpAgent<IJdOrderItem>, IJdOrderItem
    {
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemList(Jinher.AMP.BTP.Deploy.JdOrderItemDTO search)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJdOrderItemList(search);

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


        public ResultDTO SaveJdOrderItem(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveJdOrderItem(model);

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


        public ResultDTO UpdateJdOrderItem(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateJdOrderItem(model);

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


        public ResultDTO DeleteJdOrderItem(List<string> jdorders)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DeleteJdOrderItem(jdorders);

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

       
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetList(List<string> jdporders)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetList(jdporders);

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

        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderIdList(List<string> jdorders)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetJdOrderIdList(jdorders);

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


        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemLists(List<Guid> TempIds)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetJdOrderItemLists(TempIds);

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
