
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/3 15:37:13
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

    public class ComAttibuteAgent : BaseBpAgent<IComAttibute>, IComAttibute
    {
        public void AddComAttibute(System.Collections.Generic.List<System.Guid> secondAttributeIds, System.Guid commodityId, System.Guid attributeId)
        {

            try
            {
                //调用代理方法
                base.Channel.AddComAttibute(secondAttributeIds, commodityId, attributeId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> GetColorOrSizeByAppId(System.Guid appId, System.Guid commodityId, System.Guid attributeId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ColorAndSizeAttributeVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetColorOrSizeByAppId(appId, commodityId, attributeId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SecondAttributeDTO> GetSecondAttribute(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SecondAttributeDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSecondAttribute(appId);

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
