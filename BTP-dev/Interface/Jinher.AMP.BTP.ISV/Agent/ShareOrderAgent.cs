using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;
namespace Jinher.AMP.BTP.ISV.Agent
{
    public class ShareOrderAgent : BaseBpAgent<IShareOrder>, IShareOrder
    {
        /// <summary>
        /// 获取众销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO GetShareOrderMoneySumInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShareOrderMoneySumInfo(search);

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
        /// 获取众销入账信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO GetShareOrderMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShareOrderMoneyInfo(search);

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
