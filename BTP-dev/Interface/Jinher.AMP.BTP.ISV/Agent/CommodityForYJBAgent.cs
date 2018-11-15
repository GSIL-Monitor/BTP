
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/9/7 9:39:08
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

    public class CommodityForYJBAgent : BaseBpAgent<ICommodityForYJB>, ICommodityForYJB
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListOutPut>> GetCommodities(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchInput input)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListOutPut>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodities(input);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO GetAllCommodities(System.Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodities(appId, commodityCategory, commodityName, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> GetAppIdCommodity(string name, System.Guid appId, decimal price, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppIdCommodity(name, appId, price, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> GetCommodityById(System.Guid appid, System.Collections.Generic.List<System.Guid> ids, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityById(appid, ids, pageIndex, pageSize);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO> GetCommodityByIds(System.Guid appid, System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByIds(appid, ids);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityPrice(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityPrice(CkPriceInfo);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RecoverCommodityPrice(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RecoverCommodityPrice(CkPriceInfo);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyInfoList(System.Guid EsappId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApplyInfoList(EsappId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierInfoList(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSupplierInfoList(appId);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyList()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApplyList();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierList()
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSupplierList();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyByIds(System.Guid esAppId, System.Collections.Generic.List<System.Guid> appIds)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApplyByIds(esAppId, appIds);

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
        public System.Collections.Generic.List<System.Guid> GetYXappIds()
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetYXappIds();

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> GetOrderItemList(string StarTime, string EndTime)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderItemList(StarTime, EndTime);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> GetOrderInfoByAppId(System.Guid AppId, string StarTime, string EndTime)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderInfoByAppId(AppId, StarTime, EndTime);

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
