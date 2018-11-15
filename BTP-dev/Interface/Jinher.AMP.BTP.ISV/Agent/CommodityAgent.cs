
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/3/26 10:14:52
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class CommodityAgent : BaseBpAgent<ICommodity>, ICommodity
    {

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetOrByCommodity
      (System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize, int fieldSort, int order, string areaCode)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrByCommodity(categoryId, appId, pageIndex, pageSize, fieldSort, order, areaCode);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByWhere
           (Jinher.AMP.ZPH.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, int pageIndex, int pageSize, string areaCode)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByWhere(commoditySearch, pageIndex, pageSize, areaCode);

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity
            (System.Guid appId, int pageIndex, int pageSize, string areaCode)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodity(appId, pageIndex, pageSize, areaCode);

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity2
    (System.Guid appId, int pageIndex, int pageSize, string areaCode, int isChkTime, DateTime beginTime, DateTime endTime)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodity2(appId, pageIndex, pageSize, areaCode, isChkTime, beginTime, endTime);

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

        public ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity3
(CommodityListInputDTO input)
        {
            //定义返回值
            ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodity3(input);

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetWantCommodity
            (string want, System.Guid appId, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetWantCommodity(want, appId, pageIndex, pageSize);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByCategory
            (System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByCategory(categoryId, appId, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetails(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo = "")
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetails(commodityId, appId, userId, freightTo);

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


        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetailsNew(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo = "")
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetailsNew(commodityId, appId, userId, freightTo);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO CommoditySearch(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            //定义返回值

            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CommoditySearch(appId, commodityCategory, commodityName, pageIndex, pageSize);

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

        public CommoditySearchForAppsResultDTO CommoditySearchFromApps(List<Guid> appIds, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            //定义返回值

            Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchForAppsResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CommoditySearchFromApps(appIds, commodityCategory, commodityName, pageIndex, pageSize);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.GetCommoditySearchResultDTO GetCommoditySearch(Guid appId, string commodityName, int pageIndex, int pageSize)
        {
            //定义返回值

            Jinher.AMP.BTP.Deploy.CustomDTO.GetCommoditySearchResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommoditySearch(appId, commodityName, pageIndex, pageSize);

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

        public CommodityInfoListDTO GetCommodityInfo(Guid commodityId)
        {
            //定义返回值

            CommodityInfoListDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityInfo(commodityId);

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

        public ResultDTO CreateUserPrizeRecord(UserPrizeRecordDTO userPrizeRecordDTO)
        {
            //定义返回值

            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CreateUserPrizeRecord(userPrizeRecordDTO);

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

        public PrizeRecordDTO GetUserPrizeRecord(Guid promotionId, Guid commodityId, Guid userId)
        {
            //定义返回值

            PrizeRecordDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetUserPrizeRecord(promotionId, commodityId, userId);

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

        public List<CommodityListCDTO> GetCommodityByIds(List<Guid> commodityIds, bool isDefaultOrder = false)
        {
            //定义返回值

            List<CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByIds(commodityIds, isDefaultOrder);

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodity(Guid UserID, List<Guid> CommodityIdsList)
        {
            //定义返回值

            List<CheckCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodity(UserID, CommodityIdsList);

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityNew(Guid UserID, List<CommodityIdAndStockId> CommodityIdsList)
        {
            //定义返回值

            List<CheckCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodityNew(UserID, CommodityIdsList);

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

        public List<SquareHotCommodityDTO> GetHotCommoditis()
        {
            //定义返回值

            List<SquareHotCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetHotCommoditis();

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO CalFreight(string FreightTo, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> TemplateCounts)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.CalFreight(FreightTo, TemplateCounts);

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
        public FreightDetailDTO GetFreightDetails(Guid CommodityId)
        {
            FreightDetailDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetFreightDetails(CommodityId);

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
        public CommodityZPHResultDTO GetCommoditysZPH(CommoditySearchZPHDTO search)
        {
            CommodityZPHResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommoditysZPH(search);

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
        /// ZPH商品服务项商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public CommodityZPHResultDTO GetCommoditysforZPH(CommoditySearchZPHDTO search)
        {
            CommodityZPHResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommoditysforZPH(search);

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
        /// ZPH关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        public ResultDTO JoinComdtyServiceSetting(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId)
        {
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.JoinComdtyServiceSetting(ComIds, AppId, ServiceSettingId);
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
        /// ZPH取消关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        public ResultDTO CancelComdtyServiceSetting(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId)
        {
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.CancelComdtyServiceSetting(ComIds, AppId, ServiceSettingId);
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
        public List<CommodityListCDTO> GetCommodityByZPHActId(CommoditySearchZPHDTO search)
        {
            List<CommodityListCDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByZPHActId(search);

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

        public CommodityListResultDTO GetCommodityList(CommodityListSearchDTO search)
        {
            CommodityListResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityList(search);

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

        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPH(Guid commodityId, Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId, string areaCode = "000000")
        {
            ResultDTO<CommoditySDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetailsZPH(commodityId, appId, userId, freightTo, outPromotionId, actId, areaCode);
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

        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNew(Guid commodityId, Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId, string areaCode = "000000")
        {
            ResultDTO<CommoditySDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetailsZPHNew(commodityId, appId, userId, freightTo, outPromotionId, actId, areaCode);
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

        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSku(Guid commodityId, Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId, string areaCode = "000000")
        {
            ResultDTO<CommoditySDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetailsZPHNewSku(commodityId, appId, userId, freightTo, outPromotionId, actId, areaCode);
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
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSkuII(Guid commodityId, Guid appId, Guid userId, string freightTo, Guid? jcActivityId, System.Guid actId, string areaCode = "000000")
        {
            ResultDTO<CommoditySDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetailsZPHNewSkuII(commodityId, appId, userId, freightTo, jcActivityId, actId, areaCode);
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
        public ResultDTO<CommoditySDTO> GetCommodityDetailsByActId(Guid actId, Guid appId, Guid userId, string freightTo, string areaCode)
        {
            ResultDTO<CommoditySDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityDetailsByActId(actId, appId, userId, freightTo, areaCode);
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

        public ResultDTO SaveMyPresellComdtyZPH(Guid outPromotionId, Guid userId, string verifyCode, Guid esAppId, Guid commodityId, Guid commodityStockId)
        {
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.SaveMyPresellComdtyZPH(outPromotionId, userId, verifyCode, esAppId, commodityId, commodityStockId);
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

        public ResultDTO<byte[]> GetVerifyCodeZPH()
        {
            ResultDTO<byte[]> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetVerifyCodeZPH();
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
        /// 添加和取消到货提醒
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <param name="Iscancel"></param>
        /// <returns></returns>
        public ResultDTO<NotificationsDTO> SaveStockNotificationsZPH(Guid commodityId, Guid userId, Guid esAppId, int noticeType, bool Iscancel)
        {
            ResultDTO<NotificationsDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.SaveStockNotificationsZPH(commodityId, userId, esAppId, noticeType, Iscancel);
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

        public ResultDTO SendNotificationsZPH(Guid commodityId, Guid outPromotionId, Guid esAppId)
        {
            ResultDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.SendNotificationsZPH(commodityId, outPromotionId, esAppId);
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
        [Obsolete("已废弃，请调用CheckCommodity", false)]
        public List<CheckCommodityDTO> CheckCommodityWithPreSell(Guid userId, List<CommodityIdAndStockId> commodityIdsList)
        {



            List<CheckCommodityDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodityWithPreSell(userId, commodityIdsList);
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

        public void RemoveCache()
        {
            try
            {
                //调用代理方法
                base.Channel.RemoveCache();
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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerID(CommoditySearchDTO commoditySearch)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityBySellerID(commoditySearch);

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


        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityForCoupon(CommoditySearchDTO commoditySearch, out int count)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityForCoupon(commoditySearch, out count);

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
        /// 多app运费计算
        /// </summary>
        /// <param name="freightTo">运送到</param>
        /// <param name="isSelfTake">是否自提</param>
        /// <param name="templateCounts">模板数据集合</param>
        /// <returns>运费计算结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalFreightMultiApps(string freightTo, int isSelfTake, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> templateCounts, Dictionary<Guid, decimal> coupons, Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbInfo, List<Guid> yjCouponIds)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CalFreightMultiApps(freightTo, isSelfTake, templateCounts, coupons, yjbInfo, yjCouponIds);

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

        public List<CommodityListCDTO> GetCommodityByIdsWithPreSell(List<Guid> commodityIds, bool isDefaultOrder)
        {
            //定义返回值
            List<CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByIdsWithPreSell(commodityIds, isDefaultOrder);

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

        public List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongTo(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder)
        {
            //定义返回值
            List<CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByIdsWithPreSellInBeLongTo(beLongTo, commodityIds, isDefaultOrder);

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

        public List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongToWithType(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder, int mallAppType)
        {
            //定义返回值
            List<CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByIdsWithPreSellInBeLongToWithType(beLongTo, commodityIds, isDefaultOrder, mallAppType);

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
        /// 查询AppID列表下的所有上架的商品
        /// </summary>
        /// <param name="appListSearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityByAppIdList(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchByAppIdListDTO appListSearch)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityByAppIdList(appListSearch);

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
        /// 根据商品ID列表获取商品是否支持自提的信息
        /// </summary>
        /// <param name="commodityIdList">商品ID列表</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO> GetCommodityIsEnableSelfTakeList(List<Guid> commodityIdList)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityIsEnableSelfTakeList(commodityIdList);

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

        public ResultDTO<ComAttStockDTO> CommodityAttrStocks(CommoditySearchDTO search)
        {
            //定义返回值
            ResultDTO<ComAttStockDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CommodityAttrStocks(search);

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
        /// 添加商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public ResultDTO SaveServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveServiceCommodity(serviceCommodityAndCategoryDTO);

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
        /// 修改商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateServiceCommodity(serviceCommodityAndCategoryDTO);

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
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommodity(Guid id)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteServiceCommodity(id);

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
        /// 删除多个商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommoditys(List<Guid> ids)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteServiceCommoditys(ids);

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

        public ResultDTO<CateringCommodityDTO> GetCateringCommodity(CommodityListSearchDTO search)
        {
            //定义返回值
            ResultDTO<CateringCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCateringCommodity(search);

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
        /// 判断商品是不是定制应用所发布的商品。
        /// </summary>
        /// <returns></returns>
        public ResultDTO<bool> IsFittedAppCommodity(Guid commodityId)
        {
            //定义返回值
            ResultDTO<bool> result;

            try
            {
                //调用代理方法
                result = base.Channel.IsFittedAppCommodity(commodityId);

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
        /// 获取发布商品的店铺的appId
        /// </summary>
        /// <returns></returns>
        public ResultDTO<Guid> GetCommodityAppId(Guid commodityId)
        {
            //定义返回值
            ResultDTO<Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityAppId(commodityId);

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
        /// 获取商品简略信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO<CommodityThumb> GetCommodityThumb(Guid commodityId)
        {
            //定义返回值
            ResultDTO<CommodityThumb> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityThumb(commodityId);

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
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttribute(System.Guid commodityId, Guid userId)
        {
            //定义返回值
            ResultDTO<CommoditySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityAttribute(commodityId, userId);

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
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttributeNew(System.Guid commodityId, Guid userId)
        {
            //定义返回值
            ResultDTO<CommoditySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityAttributeNew(commodityId, userId);

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


        public List<CommodityListCDTO> GetCommodityByIdsNew(CommoditySearchDTO search)
        {
            //定义返回值

            List<CommodityListCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByIdsNew(search);

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
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3(CheckCommodityParam ccp)
        {
            //定义返回值
            List<CheckCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodityV3(ccp);

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
        /// 校验商品信息  金采团购活动
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3II(CheckCommodityParam ccp)
        {
            //定义返回值
            List<CheckCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodityV3II(ccp);

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
        /// 校验购物车商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckShopCommodityDTO> CheckCommodityV4(CheckShopCommodityParam ccp)
        {
            //定义返回值
            List<CheckShopCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodityV4(ccp);

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
        /// 获取商品列表，提供跨店铺优惠券
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityListForCoupon(CommodityListSearchDTO search)
        {
            ComdtyListResultCDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityListForCoupon(search);
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
            //返回结果
            return result;
        }

        public ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search)
        {
            ComdtyListResultCDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityListV2(search);

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
        /// xiexg优惠券获取商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityListV2_New(CommodityListSearchDTO search)
        {
            ComdtyListResultCDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityListV2_New(search);

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

        public ComdtyListResultCDTO GetCommodityList3(CommodityListSearchDTO search)
        {
            ComdtyListResultCDTO result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityList3(search);

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
        /// 商品改低价格时，Job发送消息(每10分钟处理一次)
        /// </summary>
        public void AutoPushCommodityModifyPrice()
        {


            try
            {
                //调用代理方法
                base.Channel.AutoPushCommodityModifyPrice();

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

        public List<CheckCommodityDTO> CheckCommodityV5(CheckCommodityParam ccp)
        {
            //定义返回值
            List<CheckCommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckCommodityV5(ccp);

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

        public ResultDTO<CreateOrderDutyResultDTO> GetComListDuty(List<ComScoreCheckDTO> search)
        {
            //定义返回值
            ResultDTO<CreateOrderDutyResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetComListDuty(search);

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


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityByName(pdto);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDStock(Jinher.AMP.BTP.Deploy.CommodityDTO arg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SynchronizeJDStock(arg);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalRefundFreight(Guid orderId, Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CalRefundFreight(orderId, orderItemId);

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

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityWithSales(CommoditySearchDTO commoditySearch)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllCommodityWithSales(commoditySearch);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityYouka(Guid commodityId, decimal youka)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCommodityYouka(commodityId, youka);
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
        /// 获取商品税收编码列表(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<CommodityTaxRateZphDto> GetSingleCommodityCode()
        {
            //定义返回值
            ThirdResponse<CommodityTaxRateZphDto> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSingleCommodityCode();

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
        /// 同步商品库存(openApi)
        /// </summary>
        public ThirdResponse ModifyCommodityStock(string Code, string skuId, int Stock)
        {
            //定义返回值
            ThirdResponse result;

            try
            {
                //调用代理方法
                result = base.Channel.ModifyCommodityStock(Code, skuId, Stock);

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
        /// 添加商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse AddCommodity(List<Commoditydto> objlist)
        {
            //定义返回值
            ThirdResponse result;

            try
            {
                //调用代理方法
                result = base.Channel.AddCommodity(objlist);

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
        /// 修改商品名称(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityName(string skuId, string CommodityName)
        {
            //定义返回值
            ThirdResponse result;

            try
            {
                //调用代理方法
                result = base.Channel.ModifyCommodityName(skuId,CommodityName);

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
        /// 修改商品价格(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityPrice(string skuId, decimal Price)
        {
            //定义返回值
            ThirdResponse result;

            try
            {
                //调用代理方法
                result = base.Channel.ModifyCommodityPrice(skuId, Price);

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
        /// 修改商品上下架(openApi)
        /// </summary>
        /// <param name="SupId"></param>
        /// <param name="State">0上架，1下架</param>
        /// <returns></returns>
        public ThirdResponse Upperandlower(string SupId,int State)
        {
            //定义返回值
            ThirdResponse result;

            try
            {
                //调用代理方法
                result = base.Channel.Upperandlower(SupId,State);

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


        #region NetCore刷新缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity1()
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.NetCoreAutoAuditJdCommodity1();

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
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity(JdBTPRefreshCache dict)
        {

            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.NetCoreAutoAuditJdCommodity1();

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
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdPromotions(JdBTPRefreshCache dict)
        {

            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.NetCoreAutoAuditJdCommodity1();

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
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCountInfo(JdBTPRefreshCache dict)
        {

            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.NetCoreAutoAuditJdCommodity1();

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
        #endregion


        /// <summary>
        /// 根据appid和订单id查找该订单下所有的商品信息(评价用)
        /// </summary>
        /// <param name="Code">订单编号</param>
        /// <param name="OrderId">订单号id</param>
        /// <param name="Commodityid">商品id（单个评价用）</param>
        /// <returns></returns>
        public ResultDTO<List<CommodityDTO>> GetOrderIdComInfo(string Code, Guid OrderId, Guid Commodityid)
        {
            //定义返回值
            ResultDTO<List<CommodityDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderIdComInfo(Code, OrderId, Commodityid);

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
