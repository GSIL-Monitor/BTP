
/***************
功能描述: BTP-OPTProxy
作    者: 
创建时间: 2015/7/30 17:59:55
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

    public class FreightAgent : BaseBpAgent<IFreight>, IFreight
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> GetFreightTemplateListByAppId(System.Guid appId, int pageIndex, int pageSize, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetFreightTemplateListByAppId(appId, pageIndex, pageSize, out rowCount);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> GetFreightTemplateDetailListByTemId(System.Guid freightTemplateId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetFreightTemplateDetailListByTemId(freightTemplateId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO freightDetail)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddFreightDetail(freightDetail);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteFreightDetail(System.Guid freightDetailId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteFreightDetail(freightDetailId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteFreight(System.Guid Id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteFreight(Id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddFreightAndFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDTO freight, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> freightDetailList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddFreightAndFreightDetail(freight, freightDetailList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateFreightAndFreightDetail(Jinher.AMP.BTP.Deploy.FreightTemplateDTO freight, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.FreightTemplateDetailDTO> freightDetailList)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateFreightAndFreightDetail(freight, freightDetailList);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO GetOneFreight(System.Guid freightTemplateId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOneFreight(freightTemplateId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> GetFreightListByAppId(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetFreightListByAppId(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO IsContactCommodity(System.Guid freightTemplateId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.IsContactCommodity(freightTemplateId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveFreightTemplateFull(Jinher.AMP.BTP.Deploy.CustomDTO.FreightTempFullDTO freightDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveFreightTemplateFull(freightDTO);

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


        public Deploy.CustomDTO.ResultDTO SaveRangeFreightTemplate(Deploy.CustomDTO.RangeFreightTemplateInputDTO freightDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveRangeFreightTemplate(freightDTO);

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
        /// 建立运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO JoinCommodity(Deploy.CustomDTO.FreightTemplateAssociationCommodityInputDTO inputDTO)
        {//定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.JoinCommodity(inputDTO);

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
        /// 解除运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO UnjoinCommodity(Deploy.CustomDTO.FreightTemplateAssociationCommodityInputDTO inputDTO)
        {//定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UnjoinCommodity(inputDTO);

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
