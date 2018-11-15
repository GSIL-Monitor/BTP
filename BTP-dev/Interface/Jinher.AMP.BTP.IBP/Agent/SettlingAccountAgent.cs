
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2015/7/27 14:02:35
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

    public class SettlingAccountAgent : BaseBpAgent<ISettlingAccount>, ISettlingAccount
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> GetNowSettlingAccount(Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountSearchDTO search, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetNowSettlingAccount(search, out rowCount);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSettlingAccount(Jinher.AMP.BTP.Deploy.SettlingAccountDTO settlingAccountDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSettlingAccount(settlingAccountDTO);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> GetHistorySettlingAccount(Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountHistorySearchDTO search, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetHistorySettlingAccount(search, out rowCount);

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
        /// 删除厂家结算记录
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSettlingAccountById(List<Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteSettlingAccountById(ids);

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
