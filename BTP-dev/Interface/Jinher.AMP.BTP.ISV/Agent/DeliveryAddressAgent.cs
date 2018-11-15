
/***************
功能描述: BTP-OPTProxy
作    者: 
创建时间: 2015/8/25 16:59:47
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

namespace Jinher.AMP.BTP.ISV.Agent
{
    
    public class DeliveryAddressAgent : BaseBpAgent<IDeliveryAddress>, IDeliveryAddress
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDeliveryAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
                      //定义返回值
           Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. SaveDeliveryAddress( addressDTO);
                               
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
         public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> GetDeliveryAddressList(System.Guid userId,System.Guid appId,int IsDefault)
        {
                      //定义返回值
           System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. GetDeliveryAddressList( userId, appId, IsDefault);
                               
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
         public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> GetDeliveryAddress(System.Guid userId,System.Guid appId)
        {
                      //定义返回值
           System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO> result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. GetDeliveryAddress( userId, appId);
                               
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
         public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteDeliveryAddress(System.Guid addressId,System.Guid appId)
        {
                      //定义返回值
           Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. DeleteDeliveryAddress( addressId, appId);
                               
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
         public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDeliveryAddressIsDefault(System.Guid addressId)
         {
             //定义返回值
             Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

             try
             {
                 //调用代理方法
                 result = base.Channel.UpdateDeliveryAddressIsDefault(addressId);

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
         public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDeliveryAddress(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO,System.Guid appId)
        {
                      //定义返回值
           Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. UpdateDeliveryAddress( addressDTO, appId);
                               
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
         public Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO GetDeliveryAddressByAddressId(System.Guid addressId,System.Guid appId)
        {
                      //定义返回值
           Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. GetDeliveryAddressByAddressId( addressId, appId);
                               
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
         public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDeliveryAddressNew(Jinher.AMP.BTP.Deploy.CustomDTO.AddressSDTO addressDTO)
        {
                      //定义返回值
           Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result ;
                    
            try
            {
                //调用代理方法
                        result = base.Channel. SaveDeliveryAddressNew( addressDTO);
                               
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
