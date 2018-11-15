/***************
功能描述: 
作    者: 
创建时间: 2016/5/29 11:37:11
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
using Jinher.AMP.BTP.Deploy.CustomDTO.MallApply;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class BaseCommissionAgent : BaseBpAgent<IBaseCommission>, IBaseCommission
    {
        /// <summary>
        /// 查询基础佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> GetBaseCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO search)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetBaseCommissionList(search);

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
        /// 保存基础佣金
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.SaveBaseCommission(model);

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
        /// 修改基础佣金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultDTO UpdateBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateBaseCommission(model);
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
        /// 删除佣金信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultDTO DelBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DelBaseCommission(model);
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
        /// 获取基础佣金实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO GetBaseCommission(Guid id, Guid mallApplyId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetBaseCommission(id, mallApplyId);
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
