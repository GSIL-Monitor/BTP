
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/1/25 11:40:18
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

    public class CommodityChangeAgent : BaseBpAgent<ICommodityChange>, ICommodityChange
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>> GetCommodityChangeList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityChangeList(Search, pageIndex, pageSize);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> GetCommodityChangeExport(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityChangeExport(Search);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyList(System.Guid EsAppid)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApplyList(EsAppid);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierList(System.Guid EsAppid)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSupplierList(EsAppid);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetAppIdsBySupplier(System.Guid EsAppId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppIdsBySupplier(EsAppId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.UserNameDTO> GetUserList()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.UserNameDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserList();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityChange(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> CommodityChange)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveCommodityChange(CommodityChange);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.totalNum> GetTotalList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.totalNum> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetTotalList(Search);

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
        /// 根据商品id获取活动类型
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public int JudgeActivityType(Guid commodityId)
        {
            //定义返回值
            int str;

            try
            {
                //调用代理方法
                str = base.Channel.JudgeActivityType(commodityId);

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
            return str;
        }
    }
}
