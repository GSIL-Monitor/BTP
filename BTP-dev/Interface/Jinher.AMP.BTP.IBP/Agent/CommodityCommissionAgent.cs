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

    public class CommodityCommissionAgent : BaseBpAgent<ICommodityCommission>, ICommodityCommission
    {
        /// <summary>
        /// 查询商品佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> GetCommodityCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO search)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCommissionList(search);

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
        /// 保存商品佣金
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            //定义返回值
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.SaveCommodityCommission(model);

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
        /// 修改商品佣金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityCommission(model);
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
        /// 删除商品佣金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultDTO DelCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.DelCommodityCommission(model);
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
        /// 获取商品佣金实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO GetCommodityCommission(Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCommission(id);
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
