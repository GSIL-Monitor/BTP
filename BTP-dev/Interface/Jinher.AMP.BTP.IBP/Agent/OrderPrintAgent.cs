
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/2/16 16:21:21
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

    public class OrderPrintAgent : BaseBpAgent<IOrderPrint>, IOrderPrint
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PrintOrderDTO> GetPrintOrder(System.Collections.Generic.List<System.Guid> orderIds)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PrintOrderDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPrintOrder(orderIds);

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
        /// 打印快递单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SavePrintOrders(orders);

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
        /// 打印发货单更新保存数据
        /// </summary>
        /// <param name="orders"></param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePrintInvoiceOrders(Jinher.AMP.BTP.Deploy.CustomDTO.UpdatePrintDTO orders)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SavePrintInvoiceOrders(orders);

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
