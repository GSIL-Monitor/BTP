
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/3/19 10:22:06
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

    public class DeliveryAddressAgent : BaseBpAgent<IDeliveryAddress>, IDeliveryAddress
    {
        public void AddDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO deliveryAddressDTO)
        {

            try
            {
                //调用代理方法
                base.Channel.AddDeliveryAddress(deliveryAddressDTO);

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
        public void DeleteDeliveryAddress(System.Guid id)
        {

            try
            {
                //调用代理方法
                base.Channel.DeleteDeliveryAddress(id);

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
        public void UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.DeliveryAddressDTO commodityDTO)
        {

            try
            {
                //调用代理方法
                base.Channel.UpdateDeliveryAddress(commodityDTO);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.DeliveryAddressDTO> GetAllDeliveryAddress(System.Guid userId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.DeliveryAddressDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllDeliveryAddress(userId);

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
        public Jinher.AMP.BTP.Deploy.DeliveryAddressDTO GetDeliveryAddress(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.DeliveryAddressDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDeliveryAddress(id);

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
