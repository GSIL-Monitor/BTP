
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/17 14:51:05
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
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class PromotionAgent : BaseBpAgent<IPromotion>, IPromotion
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddPromotion(Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM discountsDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddPromotion(discountsDTO);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelPromotion(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DelPromotion(id);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM discountsDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdatePromotion(discountsDTO);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM> GetAllPromotion(System.Guid sellerID, int pageSize, int pageIndex, string startTime, string endTime, string sintensity, string eintensity, string commodityName, string state, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllPromotion(sellerID, pageSize, pageIndex, startTime, endTime, sintensity, eintensity, commodityName, state, out rowCount);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemsVM> GetPromotionItemsByPromotionID(System.Guid promotionID, Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO search, int pageSize, int pageIndex, out int rownum)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemsVM> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPromotionItemsByPromotionID(promotionID, search, pageSize, pageIndex, out rownum);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM GetPromotionByPromotionID(System.Guid promotionID)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DiscountsVM result;

            try
            {
                //调用代理方法
                result = base.Channel.GetPromotionByPromotionID(promotionID);

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
        public System.Collections.Generic.List<System.Guid> GetCommodityID(System.Guid appID, System.DateTime startTime, System.DateTime endTime)
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityID(appID, startTime, endTime);

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
        public System.Collections.Generic.List<string> GetCommodityCodeByPromotion(System.Guid appid, System.DateTime startTime, System.DateTime endTime)
        {
            //定义返回值
            System.Collections.Generic.List<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityCodeByPromotion(appid, startTime, endTime);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PromotionItemsDTO> GetAllPromotionItems(System.Guid promotionId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.PromotionItemsDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllPromotionItems(promotionId);

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
        public void DeletePromotionItems(System.Guid promotionId)
        {

            try
            {
                //调用代理方法
                base.Channel.DeletePromotionItems(promotionId);

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
        public bool IsAddPromotion(System.DateTime starTime, System.DateTime endTime, System.Guid appId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsAddPromotion(starTime, endTime, appId);

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
        public System.Collections.Generic.List<System.Guid> IsCommodityCan(System.DateTime starTime, System.DateTime endTime, System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.IsCommodityCan(starTime, endTime, appId);

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

        public System.Collections.Generic.List<CommodityDTO> GetCommodityByPromotionID(Guid promotionID)
        {
            //定义返回值
            System.Collections.Generic.List<CommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByPromotionID(promotionID);

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

        public int GetAllCommodityNum(System.DateTime starTime, System.DateTime endTime, Guid appId, Guid? promotionid)
        {
            //定义返回值
            int result = 0;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityNum(starTime, endTime, appId, promotionid);

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

        public List<string> GetCodes(List<string> commCodes, Guid appId)
        {
            //定义返回值
            List<string> result = new List<string>();

            try
            {
                //调用代理方法
                result = base.Channel.GetCodes(commCodes, appId);

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

        public List<Guid> GetCommodityIds(Guid promotionId)
        {

            //定义返回值
            List<Guid> result = new List<Guid>();

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityIds(promotionId);

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

        public int GetCommodityNum(Guid promotionId)
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityNum(promotionId);

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
