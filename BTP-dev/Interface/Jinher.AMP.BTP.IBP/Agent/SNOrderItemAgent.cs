using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Agent
{
    public class SNOrderItemAgent : BaseBpAgent<ISNOrderItem>, ISNOrderItem
    {
        public bool AddSNOrderItem(List<Deploy.SNOrderItemDTO> snOrderItem)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.AddSNOrderItem(snOrderItem);

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

        public bool UpdSNOrderItem(Deploy.SNOrderItemDTO snOrderItem)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdSNOrderItem(snOrderItem);

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

        public bool ChangeOrderStatusForJob()
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.ChangeOrderStatusForJob();

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

        public bool OrderConfirmReceived(Guid OrderId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.OrderConfirmReceived(OrderId);

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
