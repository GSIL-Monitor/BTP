
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017-09-06 15:08:25
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

    public class ExpressOrderTemplateAgent : BaseBpAgent<IExpressOrderTemplate>, IExpressOrderTemplate
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderPrintTemplate> GetExpressOrderTemplate(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderPrintTemplate> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetExpressOrderTemplate(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExpressTemplateDTO> GetExpressOrderTemplateByAppId(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExpressTemplateDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetExpressOrderTemplateByAppId(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO> Save(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.Save(dto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.Remove(dto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(System.Guid appId, System.Collections.Generic.List<System.Guid> templateIdList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveUsed(appId, templateIdList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<System.Guid>> GetUsed(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<System.Guid>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUsed(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveProperty(System.Guid templateId, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressOrderTemplatePropertyDTO> propertyList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveProperty(templateId, propertyList);

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
