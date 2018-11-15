
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/5/18 14:39:04
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

    public class DiyGroupAgent : BaseBpAgent<IDiyGroup>, IDiyGroup
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> GetDiyGroupDetail(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDiyGroupDetail(search);

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
        public void DealUnDiyGroupTimeout()
        {

            try
            {
                //调用代理方法
                base.Channel.DealUnDiyGroupTimeout();

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
        public void DealUnDiyGroupRefund()
        {

            try
            {
                //调用代理方法
                base.Channel.DealUnDiyGroupRefund();

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
        /// <summary>
        /// 我的拼团订单列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> GetDiyGroupList(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDiyGroupList(search);

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
        /// 成团自动退款 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyRefundDiyGroup()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.VoluntarilyRefundDiyGroup();

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
        /// 自动确认成团 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyConfirmDiyGroup()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.VoluntarilyConfirmDiyGroup();

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


        public Deploy.CustomDTO.ResultDTO<Deploy.CustomDTO.CheckDiyGroupOutputDTO> CheckDiyGroup(Deploy.CustomDTO.CheckDiyGroupInputDTO inputDTO)
        {
            Deploy.CustomDTO.ResultDTO<Deploy.CustomDTO.CheckDiyGroupOutputDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckDiyGroup(inputDTO);

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
