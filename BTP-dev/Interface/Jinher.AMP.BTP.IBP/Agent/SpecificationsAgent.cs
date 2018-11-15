
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

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class SpecificationsAgent : BaseBpAgent<ISpecifications>, ISpecifications
    {   

        public List<Jinher.AMP.BTP.Deploy.SpecificationsDTO> GetSpecificationsList(Jinher.AMP.BTP.Deploy.SpecificationsDTO search)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.SpecificationsDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetSpecificationsList(search);

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


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSpecifications(Jinher.AMP.BTP.Deploy.SpecificationsDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSpecifications(model);

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


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Del(Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.Del(id);

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
