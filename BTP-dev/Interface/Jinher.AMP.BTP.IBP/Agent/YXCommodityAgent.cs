
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/5/19 12:55:52
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

    public class YXCommodityAgent : BaseBpAgent<IYXCommodity>, IYXCommodity
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYXCommodity(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddYXCommodity(input);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYXCommodityData(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.ImportYXCommodityData(JdComList, AppId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYXCommodityInfo(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.AutoSyncYXCommodityInfo(AppId, Ids);

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
