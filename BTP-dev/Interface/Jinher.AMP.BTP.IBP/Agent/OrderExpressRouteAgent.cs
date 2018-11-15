﻿
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/5/10 17:19:34
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

    public class OrderExpressRouteAgent : BaseBpAgent<IOrderExpressRoute>, IOrderExpressRoute
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReceiveKdniaoExpressRoute(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> oerList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ReceiveKdniaoExpressRoute(oerList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> GetExpressRouteByExpNo(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO express)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.OrderExpressRouteExtendDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetExpressRouteByExpNo(express);

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
        public Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO GetExpressRouteByExpOrderNo(string expOrderNo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetExpressRouteByExpOrderNo(expOrderNo);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateExpressRoute(Jinher.AMP.BTP.Deploy.OrderExpressRouteDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateExpressRoute(model);

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

        public void GetOrderExpressForJdJob()
        {

            try
            {
                //调用代理方法
                base.Channel.GetOrderExpressForJdJob();

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


        public void GetOrderExpressForJsJob()
        {

            try
            {
                //调用代理方法
                base.Channel.GetOrderExpressForJsJob();

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


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> GetUserNewOrderExpress(System.Guid AppId, System.Guid Userid)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.ComOrderExpressNew> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserNewOrderExpress(AppId, Userid);

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