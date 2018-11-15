
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/5/14 16:43:21
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class DiyGroupAgent : BaseBpAgent<IDiyGroup>, IDiyGroup
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO> GetDiyGroups(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupManageDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDiyGroups(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ConfirmDiyGroup(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ConfirmDiyGroup(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Refund(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.Refund(search);

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
        /// 获取 未完成的拼团列表
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.UnfinishedDiyGroupOutputDTO>> UnfinishedDiyGrouplist(Deploy.CustomDTO.UnfinishedDiyGroupInputDTO inputDTO)
        {
            var result = new ResultDTO<List<UnfinishedDiyGroupOutputDTO>>()
            {
                Data = new List<UnfinishedDiyGroupOutputDTO>()
            };

            try
            {
                result = base.Channel.UnfinishedDiyGrouplist(inputDTO);
            }
            catch
            {
                throw;
            }
            finally
            {
                ChannelClose();
            }

            return result;
        }
    }
}
