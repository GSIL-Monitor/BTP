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

    public class CategoryCommissionAgent : BaseBpAgent<ICategoryCommission>, ICategoryCommission
    {
        /// <summary>
        /// 查询类别佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO> GetCategoryCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO search)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryCommissionList(search);

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
        /// 保存类别佣金
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.SaveCategoryCommission(model);

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
        /// 修改类别佣金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultDTO UpdateCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCategoryCommission(model);
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
        /// 删除类别佣金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultDTO DelCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DelCategoryCommission(model);
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
        /// 获取类别佣金实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO GetCategoryCommission(Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCategoryCommission(id);
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
