using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.ZPH.Deploy.Enum;
using Jinher.AMP.ZPH.Deploy.MobileCDTO;
using Jinher.AMP.ZPH.ISV.Facade;
using Jinher.JAP.Cache;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using AppSetAppDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.AppSetAppDTO;
using AppSetSearchDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.AppSetSearchDTO;
using CategorySDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.CategorySDTO;
using CommodityListCDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityListCDTO;
using QryCommodityDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.QryCommodityDTO;
using QueryActCommodityParam = Jinher.AMP.ZPH.Deploy.CustomDTO.QueryActCommodityParam;
using SkuActivityCDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.SkuActivityCDTO;
using JCActivityItemsListCDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.JCActivityItemsListCDTO;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class ZPHSV : OutSideServiceBase<ZPHSVFacade>
    {

        private static string GetCustomConfig(Guid appId, string key)
        {
            var config = Instance.GetMaskPicVI(appId);
            if (config != null && config.appSetting != null && config.appSetting.Any(c => c.key == key))
            {
                return config.appSetting.First(c => c.key == key).value;
            }
            return null;
        }

        /// <summary>
        ///  获取店铺720云景地址
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public static string GetCloudViewUrl(Guid appId)
        {
            const string key = "720cloudview";
            return GetCustomConfig(appId, key);
        }
        /// <summary>
        ///  获取确认订单提示
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public static string GetCreateOrderTip(Guid appId)
        {
            const string key = "createOrderTip";
            return GetCustomConfig(appId, key);
        }
        /// <summary>
        /// 获取付款成功配置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static OrderPayedConfig GetOrderPayedConfig(Guid appId)
        {
            OrderPayedConfig result = new OrderPayedConfig();

            const string picKey = "orderPayedPic";
            const string descKey = "orderShareDesc";

            var config = Instance.GetMaskPicVI(appId);
            if (config != null && config.appSetting != null)
            {
                if (config.appSetting.Any(c => c.key == picKey))
                {
                    result.OrderPayedPic = config.appSetting.First(c => c.key == picKey).value;
                }
                if (config.appSetting.Any(c => c.key == descKey))
                {
                    result.OrderShareDesc = config.appSetting.First(c => c.key == descKey).value;
                }
            }
            return result;
        }

        /// <summary>
        /// 判断是不是所有app都是代运营app.
        /// </summary>
        /// <param name="appIds">以,分隔的appid串</param>
        /// <returns></returns>
        public static bool CheckAllAppInZPH(string appIds)
        {
            if (string.IsNullOrWhiteSpace(appIds))
            {
                return false;
            }
            string[] strs = appIds.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length == 0)
            {
                return false;
            }
            List<Guid> listAppIds = strs.ToList().ConvertAll<Guid>(str => new Guid(str));
            bool isAllInZPH = Instance.CheckAllAppInZPH(listAppIds);
            return isAllInZPH;
        }
        /// <summary>
        /// 获取所有"正品O2O电商馆"，（Id、名称）
        /// </summary>
        /// <returns></returns>
        public static List<AppNameDTO> GetEsNetApps()
        {
            List<AppNameDTO> result = new List<AppNameDTO>();
            var list = Instance.GetEsNetList();
            if (list != null && list.Any())
            {
                foreach (var esNetContentCdto in list)
                {
                    result.Add(new AppNameDTO { Id = esNetContentCdto.ESAppId, AppName = esNetContentCdto.ESName });
                }
            }
            return result;
        }

        /// <summary>
        ///  获取店铺直播列表
        /// </summary>
        /// <returns></returns>       
        public static ReturnInfo<List<LiveActivityListCDTO>> GetLiveActivityList(Jinher.AMP.ZPH.Deploy.CustomDTO.QueryLiveActivityParam param)
        {
            return Instance.GetLiveActivityList(param);
        }

        /// <summary>
        /// 是否专题活动
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static bool IsSSActivity(Guid ssActId)
        {
            return Instance.IsSSActivity(ssActId);
        }
        /// <summary>
        ///添加和取消到货提醒
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>        
        public static ReturnInfo<Jinher.AMP.ZPH.Deploy.MyNotificationsDTO> SaveNotifications(NoticeCDTO dto)
        {
            return Instance.SaveNotifications(dto);            
        }
        /// <summary>
        ///触发消息提醒
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>       
        public static void SendStockNotifications(List<Guid> ComIds)
        {
            NoticeCDTO dto = new NoticeCDTO()
            {
                ComIds = ComIds
            };
            Instance.SendStockNotifications(dto);
        }
        /// <summary>
        ///获取提醒状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>       
        public static Jinher.AMP.ZPH.Deploy.MyNotificationsDTO GetNotificationsType(Guid comId, Guid userId)
        {
            NoticeCDTO dto = new NoticeCDTO()
            {
                bizId = comId,
                userId=userId,
                subId=userId
            };
            return Instance.GetNotificationsType(dto);
        }

    }
    public class ZPHSVFacade : OutSideFacadeBase
    {

        /// <summary>
        /// 校验app是否在正品会中
        /// </summary>
        /// <param name="appid"></param>
        [BTPAopLogMethod]
        public bool CheckIsAppInZPH(Guid appid)
        {
            //白晓东商品详情页出不来临时注释掉
            //var result = GlobalCacheWrapper.GetData(RedisKeyConst.AppInZPH, appid.ToString(), CacheTypeEnum.redisSS, "BTPCache");
            //if (result != null && result.ToString() == "1")
            //{
            //    return true;
            //}
            //else if (result != null && result.ToString() == "0")
            //{
            //    return false;
            //}
            bool resultCheck = false;
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.AppSetFacade appSetSV = new Jinher.AMP.ZPH.ISV.Facade.AppSetFacade();
                appSetSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                resultCheck = appSetSV.CheckAppInZPH(appid);
                if (!resultCheck)
                {
                    GlobalCacheWrapper.Add(RedisKeyConst.AppInZPH, appid.ToString(), "0", CacheTypeEnum.redisSS, "BTPCache");
                    return false;
                }
                else
                {
                    GlobalCacheWrapper.Add(RedisKeyConst.AppInZPH, appid.ToString(), "1", CacheTypeEnum.redisSS, "BTPCache");
                    return true;
                }
            }
            catch (Exception e)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("商品列表查询错误：", e);
            }
            finally
            {
                LogHelper.Debug(string.Format("校验app是否在正品会中, appId:{0} ,resultCheck：{1}", appid, resultCheck));
            }

            return false;
        }
        /// <summary>
        /// 获取活动sku属性集合
        /// </summary>
        /// <param name="activityId"></param>
        [BTPAopLogMethod]
        public List<SkuActivityCDTO> GetSkuActivityList(Guid activityId)
        {
            List<SkuActivityCDTO> result = new List<SkuActivityCDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SkuActivityFacade skuActivityFacade = new Jinher.AMP.ZPH.ISV.Facade.SkuActivityFacade();
                skuActivityFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = skuActivityFacade.GetSkuActivityList(activityId);
            }
            catch (Exception e)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("获取活动sku属性集合：", e);
            }
            finally
            {
                LogHelper.Debug(string.Format("获取活动sku属性集合, activityId:{0}", activityId));
            }
            return result;
        }


        /// <summary>
        /// 获取活动sku属性集合
        /// </summary>
        /// <param name="activityIds">活动id列表</param>
        [BTPAopLogMethod]
        public List<SkuActivityCDTO> GetSkuActivityListBatch(List<Guid> activityIds)
        {
            List<SkuActivityCDTO> result = new List<SkuActivityCDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SkuActivityFacade skuActivityFacade = new Jinher.AMP.ZPH.ISV.Facade.SkuActivityFacade();
                skuActivityFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = skuActivityFacade.GetSkuActivityListBatch(activityIds);
            }
            catch (Exception e)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("GetSkuActivityListBatch获取活动sku属性集合：", e);
            }
            finally
            {
                LogHelper.Debug(string.Format("GetSkuActivityListBatch获取活动sku属性集合, activityIds:{0}", activityIds));
            }
            return result;
        }
        ///gong
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="qryCommodityDTO"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<CommodityListCDTO> GetCommodityList(QryCommodityDTO qryCommodityDTO)
        {
            List<CommodityListCDTO> result = new List<CommodityListCDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.AppSetFacade appSetSV = new AMP.ZPH.ISV.Facade.AppSetFacade();
                appSetSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                result = appSetSV.GetCommodityList(qryCommodityDTO);
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("ZPHSV.GetCommodityList服务异常:获取应用信息异常。 qryCommodityDTO：{0}", JsonHelper.JsonSerializer(qryCommodityDTO)), ex);
            }
            return result;
        }
        /// <summary>
        /// 获取商品分类列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<CategorySDTO> GetCategory(Guid appId)
        {
            List<CategorySDTO> cateDTO = new List<CategorySDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.AppSetFacade appSetSV = new AMP.ZPH.ISV.Facade.AppSetFacade();
                appSetSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                cateDTO = appSetSV.GetCategory(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetCategory服务异常:获取应用信息异常。 appId：{0}", appId), ex);
            }
            return cateDTO;
        }
        /// <summary>
        /// 按照关键字获取商品列表
        /// </summary>
        /// <param name="want"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<CommodityListCDTO> GetWantCommodity(string want, int pageIndex, int pageSize)
        {
            List<CommodityListCDTO> comDTO = new List<CommodityListCDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.AppSetFacade appSetSV = new AMP.ZPH.ISV.Facade.AppSetFacade();
                appSetSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                comDTO = appSetSV.GetWantCommodity(want, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetWantCommodity服务异常:获取应用信息异常。 want：{0}", want), ex);
            }
            return comDTO;
        }

        /// <summary>
        /// 根据分类Id获取该分类下的app列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<AppSetAppDTO> GetCategoryAppList(AppSetSearchDTO search)
        {
            List<AppSetAppDTO> appDTO = new List<AppSetAppDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.AppSetFacade appSetSV = new AMP.ZPH.ISV.Facade.AppSetFacade();
                appSetSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                appDTO = appSetSV.GetCategoryAppList(search);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetCategoryAppList服务异常:获取应用信息异常。 search：{0}", JsonHelper.JsonSerializer(search)), ex);
            }
            return appDTO;
        }
        /// <summary>
        /// 获取活动商品
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Guid> GetCommodityByActId(QueryActCommodityParam param)
        {
            List<Guid> guid = new List<Guid>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.GeneralActivityFacade genFacade = new AMP.ZPH.ISV.Facade.GeneralActivityFacade();
                genFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                guid = genFacade.GetCommodityByActId(param);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetCommodityByActId服务异常:获取应用信息异常。 param：{0}", JsonHelper.JsonSerializer(param)), ex);
            }
            return guid;
        }
        /// <summary>
        /// 我的预预售数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public int GetMyPresellComdtyNum(Guid userId, Guid? esAppId)
        {
            int num = 0;
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                num = presellFacade.GetMyPresellComdtyNum2(userId, esAppId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetMyPresellComdtyNum服务异常:获取应用信息异常。 userId：{0}，esAppId：{1}", userId, esAppId), ex);
            }
            return num;
        }
        /// <summary>
        /// 获取预售信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public PresellComdtyInfoCDTO GetAndCheckPresellInfoById(CheckPresellInfoCDTO dto)
        {
            PresellComdtyInfoCDTO preDTO = new PresellComdtyInfoCDTO();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                preDTO = presellFacade.GetAndCheckPresellInfoById(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetAndCheckPresellInfoById服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return preDTO;
        }

        /// <summary>
        /// 获取预售信息
        /// </summary>
        ///<param name="promotionIds">活动id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<PresellComdtyInfoCDTO> GetPresellInfoByIds(List<Guid> promotionIds)
        {
            List<PresellComdtyInfoCDTO> preDTO = new List<PresellComdtyInfoCDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                preDTO = presellFacade.GetPresellInfoByIds(promotionIds);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetPresellInfoByIds服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(promotionIds)), ex);
            }
            return preDTO;
        }

        /// <summary>
        /// 保存我的预售
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo SaveMyPresellComdty(MyPresellComdtyCDTO dto)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = presellFacade.SaveMyPresellComdty(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.SaveMyPresellComdty服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 保存我的预售
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo SaveMyPresellComdty2(MyPresellComdty2CDTO dto)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = presellFacade.SaveMyPresellComdty2(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.SaveMyPresellComdty2服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<byte[]> GetVerifyCode()
        {
            ReturnInfo<byte[]> returnInfo = new ReturnInfo<byte[]>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = presellFacade.GetVerifyCode();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetVerifyCode服务异常:获取应用信息异常。 Exception：{0}", ex));
            }
            return returnInfo;
        }
        /// <summary>
        /// 核查用户是否可以预约
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo CheckMyPresellComdty(MyPresellComdtyCDTO dto)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.PresellFacade presellFacade = new AMP.ZPH.ISV.Facade.PresellFacade();
                presellFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = presellFacade.CheckMyPresellComdty(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.CheckMyPresellComdty服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 总代列表
        /// </summary>
        /// <param name="prarm"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<List<ProxyContentCDTO>> GetAllProxyList4BTP(QueryProxyPrarm prarm)
        {
            ReturnInfo<List<ProxyContentCDTO>> returnInfo = new ReturnInfo<List<ProxyContentCDTO>>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.ProxyFacade proxyFacade = new AMP.ZPH.ISV.Facade.ProxyFacade();
                proxyFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = proxyFacade.GetAllProxyList4BTP(prarm);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetAllProxyList4BTP服务异常:获取应用信息异常。 prarm：{0}", JsonHelper.JsonSerializer(prarm)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 消息提醒
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo SendNotifications(NoticeCDTO dto)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SeckillFacade seckillFacade = new AMP.ZPH.ISV.Facade.SeckillFacade();
                seckillFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = seckillFacade.SendNotifications(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.SendNotifications服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        ///添加和取消到货提醒
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<Jinher.AMP.ZPH.Deploy.MyNotificationsDTO> SaveNotifications(NoticeCDTO dto)
        {
            ReturnInfo<Jinher.AMP.ZPH.Deploy.MyNotificationsDTO> returnInfo = new ReturnInfo<ZPH.Deploy.MyNotificationsDTO>();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SeckillFacade seckillFacade = new AMP.ZPH.ISV.Facade.SeckillFacade();
                seckillFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = seckillFacade.SaveStockNotifications(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.SaveStockNotifications服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        ///获取提醒状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.ZPH.Deploy.MyNotificationsDTO GetNotificationsType(NoticeCDTO dto)
        {
            Jinher.AMP.ZPH.Deploy.MyNotificationsDTO returnInfo = new ZPH.Deploy.MyNotificationsDTO();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SeckillFacade seckillFacade = new AMP.ZPH.ISV.Facade.SeckillFacade();
                seckillFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = seckillFacade.GetMyNotifications(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetMyNotifications服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        ///触发消息提醒
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public void SendStockNotifications(NoticeCDTO dto)
        {
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SeckillFacade seckillFacade = new AMP.ZPH.ISV.Facade.SeckillFacade();
                seckillFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                LogHelper.Info(string.Format("发送到货提醒商品id:{0}", JsonHelper.JsonSerializer(dto.ComIds)));
                seckillFacade.SendStockNotifications(dto);               
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.SendStockNotifications服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
        }
        /// <summary>
        /// 获取秒杀信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public SeckillComdtyInfoCDTO GetSeckillInfoById(SeckillInfoCDTO dto)
        {
            SeckillComdtyInfoCDTO seckillDTO = new SeckillComdtyInfoCDTO();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.SeckillFacade seckillFacade = new AMP.ZPH.ISV.Facade.SeckillFacade();
                seckillFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                seckillDTO = seckillFacade.GetSeckillInfoById(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetSeckillInfoById服务异常:获取应用信息异常。 dto：{0}", JsonHelper.JsonSerializer(dto)), ex);
            }
            return seckillDTO;
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="subId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo AddCommodity(System.Guid subId, Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityCDTO dto)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.CommodityFacade commodityFacade = new Jinher.AMP.ZPH.ISV.Facade.CommodityFacade();
                commodityFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = commodityFacade.AddCommodity(subId, dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加商品。subId：{0}，dto：{1}", subId, JsonHelper.JsonSerializer(dto)), ex);
            }
            return returnInfo;
        }

        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo UpdateCommodity(Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityCDTO dto)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            Jinher.AMP.ZPH.ISV.Facade.CommodityFacade commodityFacade = new Jinher.AMP.ZPH.ISV.Facade.CommodityFacade();
            commodityFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
            try
            {
                returnInfo = commodityFacade.UpdateCommodity(dto);
                if (!returnInfo.isSuccess)
                {
                    GlobalCacheWrapper.Add("UpdateCommodity_InZph", dto.Id.ToString(), "0", CacheTypeEnum.redisSS, "BTPCache");
                }
                else
                {
                    GlobalCacheWrapper.Add("UpdateCommodity_InZph", dto.Id.ToString(), "1", CacheTypeEnum.redisSS, "BTPCache");
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("同步修改商品信息操作。dto：{0}", JsonHelper.JsonSerializer(dto)), e);
            }
            return returnInfo;
        }

        /// <summary>
        /// 获取appID下的app列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<List<PavilionAppListCDTO>> GetPavilionApp(QueryPavilionAppParam param)
        {
            ReturnInfo<List<PavilionAppListCDTO>> returnInfo = new ReturnInfo<List<PavilionAppListCDTO>>();
            try
            {
                AMP.ZPH.ISV.Facade.AppPavilionFacade appPavilionFacade = new AMP.ZPH.ISV.Facade.AppPavilionFacade();
                appPavilionFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = appPavilionFacade.GetPavilionApp(param);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetPavilionApp。param：{0}", JsonHelper.JsonSerializer(param)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 获取电商馆信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public AppPavilionInfoIICDTO GetAppPavilionInfo(QueryAppPavilionParam param)
        {
            AppPavilionInfoIICDTO returnInfo = new AppPavilionInfoIICDTO();
            try
            {
                AMP.ZPH.ISV.Facade.AppPavilionFacade appPavilionFacade = new AMP.ZPH.ISV.Facade.AppPavilionFacade();
                appPavilionFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = appPavilionFacade.GetAppPavilionInfoII(param);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GetAppPavilionInfo;param：{0}", JsonHelper.JsonSerializer(param)), ex);
            }
            return returnInfo;
        }

        /// <summary>
        /// 判断是不是所有app都是代运营app.
        /// </summary>
        /// <param name="appIds">app列表</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool CheckAllAppInZPH(List<Guid> appIds)
        {
            try
            {
                AMP.ZPH.ISV.Facade.AppSetFacade asFacade = new AMP.ZPH.ISV.Facade.AppSetFacade();
                asFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                bool b = asFacade.CheckAppsInZPH(appIds);
                return b;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.CheckAllAppInZPH判断是不是所有app都是代运营app异常，异常信息：{0}", ex);
            }
            return false;
        }

        /// <summary>
        /// 判断是否是电商馆
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool IsAppPavilion(System.Guid appId)
        {
            bool returnInfo = false;
            try
            {
                AMP.ZPH.ISV.Facade.AppPavilionFacade appPavilionFacade = new AMP.ZPH.ISV.Facade.AppPavilionFacade();
                appPavilionFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = appPavilionFacade.IsAppPavilion(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("IsAppPavilion。param：{0}", JsonHelper.JsonSerializer(appId)), ex);
            }
            return returnInfo;
        }

        /// <summary>
        /// 活动遮照图片地址
        /// </summary>
        /// <param name="esAppId">所登录的appid</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public MMaskPicIICDTO GetMaskPicII(Guid esAppId)
        {
            Jinher.AMP.ZPH.Deploy.MobileCDTO.MMaskPicIICDTO result = null;
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.ConfigManageFacade cnfigManageSV = new AMP.ZPH.ISV.Facade.ConfigManageFacade();
                cnfigManageSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.ZPH.Deploy.MobileCDTO.QueryMaskPicParam query = new QueryMaskPicParam();
                query.appId = esAppId;
                query.maskPicLocal = MaskPicLocaltion.All;

                result = cnfigManageSV.GetMaskPicIII(query);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("ZPHSV.GetMaskPicII服务异常:获取应用信息异常。 esAppId：{0}", esAppId), ex);
            }
            return result;
        }

        /// <summary>
        /// 活动遮照图片地址
        /// </summary>
        /// <param name="esAppId">所登录的appid</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public MMaskPicIVCDTO GetMaskPicV(Guid esAppId)
        {
            MMaskPicIVCDTO result = null;
            try
            {
                ConfigManageFacade cnfigManageSv = new ConfigManageFacade
                {
                    ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo()
                };
                QueryMaskPicParam query = new QueryMaskPicParam
                {
                    appId = esAppId,
                    maskPicLocal = MaskPicLocaltion.All
                };
                result = cnfigManageSv.GetMaskPicV(query);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("ZPHSV.GetMaskPicV服务异常:获取应用信息异常。 esAppId：{0}", esAppId), ex);
            }
            return result;
        }

        /// <summary>
        /// 店铺设置
        /// </summary>
        /// <param name="esAppId">所登录的appid</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public MMaskPicVCDTO GetMaskPicVI(Guid esAppId)
        {
            Jinher.AMP.ZPH.Deploy.MobileCDTO.MMaskPicVCDTO result = null;
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.ConfigManageFacade cnfigManageSV = new AMP.ZPH.ISV.Facade.ConfigManageFacade();
                cnfigManageSV.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.ZPH.Deploy.MobileCDTO.QueryMaskPicParam query = new QueryMaskPicParam();
                query.appId = esAppId;
                query.maskPicLocal = MaskPicLocaltion.All;

                result = cnfigManageSV.GetMaskPicVI(query);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("ZPHSV.GetMaskPicVI服务异常:获取应用信息异常。 esAppId：{0}", esAppId), ex);
            }
            return result;
        }

        /// <summary>
        /// 核查代理能否创建体验柜
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo ChecksCanBeCreated(ProxyCabinetParam param)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.ProxyFacade proxyFac = new ZPH.ISV.Facade.ProxyFacade();
                proxyFac.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = proxyFac.ChecksCanBeCreated(param);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.ChecksCanBeCreated服务异常:获取应用信息异常。 param：{0}", JsonHelper.JsonSerializer(param)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 更新已建体验柜数量
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo UpdateProxyCabinetNum(ProxyCabinetNumParam param)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                Jinher.AMP.ZPH.ISV.Facade.ProxyFacade proxyFac = new ZPH.ISV.Facade.ProxyFacade();
                proxyFac.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                returnInfo = proxyFac.UpdateProxyCabinetNum(param);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.UpdateProxyCabinetNum服务异常:获取应用信息异常。 param：{0}", JsonHelper.JsonSerializer(param)), ex);
            }
            return returnInfo;
        }
        /// <summary>
        /// 秒杀配置
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public SeckillConfigCDTO GetSeckillConfig(Guid appId)
        {
            SeckillConfigCDTO seckillConfigCDTO = new SeckillConfigCDTO();
            try
            {
                Jinher.AMP.ZPH.IBP.Facade.SeckillManageFacade seckillFacade = new ZPH.IBP.Facade.SeckillManageFacade();
                seckillFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                seckillConfigCDTO = seckillFacade.GetSeckillConfig(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetSeckillConfig服务异常:获取应用信息异常。 appId：{0}", JsonHelper.JsonSerializer(appId)), ex);
            }
            return seckillConfigCDTO;
        }

        /// <summary>
        /// 获取渠道码
        /// </summary>
        /// <param name="esAppId"></param>
        /// <param name="shareId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Guid GetPromoCodeByShareId(Guid esAppId, string shareId)
        {
            Guid result = Guid.Empty;
            try
            {
                QueryPromoAppIICDTO query = new QueryPromoAppIICDTO();
                query.appId = esAppId;
                query.shareId = shareId;

                Jinher.AMP.ZPH.ISV.Facade.PromoBusinessFacade facade = new ZPH.ISV.Facade.PromoBusinessFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                var data = facade.GetPromoCodeByShareId(query);
                LogHelper.Debug(string.Format("ZPHSV.GetPromoCodeByShareId返回结果为{0}", JsonHelper.JsonSerializer(data)));
                if (data == null || data.promoAppId == Guid.Empty || data.promoAppId == esAppId)
                {
                    result = Guid.Empty;
                }
                else
                {
                    result = data.promoAppId;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetPromoCodeByShareId服务异常:获取应用信息异常。 esAppId：{0},shareId：{1}", esAppId, shareId), ex);
            }

            return result;
        }

        /// <summary>
        /// 判断商品详情是否显示销量
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool CheckIsShowSalesVolume(Guid appId)
        {
            bool result = false;
            try
            {
                AMP.ZPH.ISV.Facade.ConfigManageFacade configManageFacade = new ConfigManageFacade();
                configManageFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.ZPH.Deploy.MobileCDTO.QueryMaskPicParam query = new QueryMaskPicParam();
                query.appId = appId;
                result = configManageFacade.GetMaskPicIV(query).showSalesvolume;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.CheckIsShowSalesvolume判断商品详情是否显示销量异常，异常信息：{0}", ex);
            }
            return result;
        }
        /// <summary>
        /// 获取App自提配置方式 1快递，2自提，3两者
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public int GetAppSelfTakeWay(Guid appId)
        {
            int result = 0;
            try
            {
                AMP.ZPH.ISV.Facade.ConfigManageFacade configManageFacade = new ConfigManageFacade();
                configManageFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                Jinher.AMP.ZPH.Deploy.MobileCDTO.QueryMaskPicParam query = new QueryMaskPicParam();
                query.appId = appId;
                string tmp = configManageFacade.GetMaskPicIV(query).Dispatching;
                if (tmp == "1")
                {
                    result = 1;
                }
                else if (tmp == "2")
                {
                    result = 2;
                }
                else if (tmp == "1,2" || tmp == "1，2")
                {
                    result = 3;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetAppSelfTakeWay获取App自提配置方式异常。appId：{0}", appId), ex);
            }
            return result;
        }
        /// <summary>
        /// 根据AppId获取可用发件人信息列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public System.Collections.Generic.List<Jinher.AMP.ZPH.Deploy.CustomDTO.OrderSenderDTO> GetSenderByAppId(System.Guid appId)
        {
            List<Jinher.AMP.ZPH.Deploy.CustomDTO.OrderSenderDTO> senders = new List<Jinher.AMP.ZPH.Deploy.CustomDTO.OrderSenderDTO>();
            try
            {
                AMP.ZPH.ISV.Facade.OrderSenderFacade orderSenderBP = new OrderSenderFacade();
                orderSenderBP.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                senders = orderSenderBP.GetSenderByAppId(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ZPHSV.GetSenderByAppId服务异常:根据AppId获取可用发件人信息列表异常。 appId：{0}", appId), ex);
            }
            return senders;
        }
        /// <summary>
        /// 获取所有"正品O2O电商馆"
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<ZPH.Deploy.CustomDTO.ESNetContentCDTO> GetEsNetList()
        {
            List<ZPH.Deploy.CustomDTO.ESNetContentCDTO> list = new List<ZPH.Deploy.CustomDTO.ESNetContentCDTO>();
            try
            {
                AMP.ZPH.ISV.Facade.ESNetFacade facade = new ESNetFacade();
                facade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                list = facade.GetESNetList(new ZPH.Deploy.MobileCDTO.QueryESNetParam
                    {
                        orderBy = ZPH.Deploy.Enum.OrderType.ASC,
                        pageSize = int.MaxValue
                    });
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetESNetList服务异常:获取所有正品O2O电商馆列表异常。", ex);
            }
            return list;
        }

        /// <summary>
        /// 根据商品ID获取商品参与的优惠套装
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<ZPH.Deploy.CustomDTO.SetMealActivityCDTO> GetSetMealActivitysByCommodityId(Guid commodityId, Guid appId, bool isDetailPage)
        {
            List<ZPH.Deploy.CustomDTO.SetMealActivityCDTO> list = new List<ZPH.Deploy.CustomDTO.SetMealActivityCDTO>();
            try
            {
                AMP.ZPH.ISV.Facade.SetMealFacade facade = new SetMealFacade();
                var lists = facade.GetSetMealActivitysByCommodityId(commodityId, appId, isDetailPage);
                if (lists != null && lists.Data != null)
                {
                    list = lists.Data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetSetMealActivitysByCommodityId服务异常:根据商品ID获取商品参与的优惠套装异常。", ex);
            }
            return list;
        }

        /// <summary>
        /// 根据ID获取优惠套装详情
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ZPH.Deploy.CustomDTO.SetMealActivityCDTO GetSetMealActivitysById(Guid setMealId)
        {
            ZPH.Deploy.CustomDTO.SetMealActivityCDTO list = new ZPH.Deploy.CustomDTO.SetMealActivityCDTO();
            try
            {
                AMP.ZPH.ISV.Facade.SetMealFacade facade = new SetMealFacade();
                list = facade.GetSetMealActivitysById(setMealId).Data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetSetMealActivitysById服务异常:根据商品ID获取商品参与的优惠套装异常。", ex);
            }
            return list;
        }

        /// <summary>
        /// 根据APPID获取拼团设置
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ZPH.Deploy.CustomDTO.DiyGroupConfigCDTO GetDiyGroupConfig(Guid appId)
        {
            ZPH.Deploy.CustomDTO.DiyGroupConfigCDTO list = new ZPH.Deploy.CustomDTO.DiyGroupConfigCDTO();
            try
            {
                AMP.ZPH.ISV.Facade.ActivityFacade facade = new ActivityFacade();
                list = facade.GetDiyGroupConfig(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetDiyGroupConfig服务异常:根据APPID获取拼团设置异常。", ex);
            }
            return list;
        }

        /// <summary>
        ///  根据金采团购活动Id获取商品相关信息
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<List<JCActivityItemsListCDTO>> GetItemsListByActivityId(Guid actId)
        {
            ReturnInfo<List<JCActivityItemsListCDTO>> list = new ReturnInfo<List<JCActivityItemsListCDTO>>();
            try
            {
                QueryJCActItemsParam queryJcActItemsParam = new QueryJCActItemsParam
                {
                    actId = actId,
                    pageIndex = 1,
                    pageSize = Int32.MaxValue,
                    orderBy = OrderType.ASC
                };

                AMP.ZPH.ISV.Facade.FJCPayFacade facade = new FJCPayFacade();
                list = facade.GetItemsListByActivityId(queryJcActItemsParam);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetItemsListByActivityId服务异常:根据活动Id获取商品相关信息异常。", ex);
            }
            return list;
        }


       

        /// <summary>
        ///  根据金采团购活动Id获取客户对应的发票信息
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<JCInvoiceListCDTO> GetJCInvoiceByActivityId(Guid actId)
        {
            ReturnInfo<JCInvoiceListCDTO> list = new ReturnInfo<JCInvoiceListCDTO>();
            try
            {
                AMP.ZPH.ISV.Facade.FJCPayFacade facade = new FJCPayFacade();
                list = facade.GetJCInvoiceByActivityId(actId);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetJCInvoiceByActivityId服务异常:根据活动Id获取客户对应的发票信息。", ex);
            }
            return list;
        }

        /// <summary>
        ///  根据活动信息获取活动Id
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<JCActivityListCDTO> GetActivityIdByInfo(SearchJCActivityCDTO searchJcActivityCdto)
        {
            ReturnInfo<JCActivityListCDTO> list = new ReturnInfo<JCActivityListCDTO>();
            try
            {
                AMP.ZPH.ISV.Facade.FJCPayFacade facade = new FJCPayFacade();
                list = facade.GetActivityIdByInfo(searchJcActivityCdto);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetJCInvoiceByActivityId服务异常:根据活动信息获取活动Id。", ex);
            }
            return list;
        }

        /// <summary>
        ///  获取商品的对应的金采团活动
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<JcActivityIdsDTO> GetJcActivityIdsByCommodity(GetJcActivityIdsParamDTO dto)
        {
            List<JcActivityIdsDTO> list = new List<JcActivityIdsDTO>();
            try
            {
                AMP.ZPH.ISV.Facade.FJCPayFacade facade = new FJCPayFacade();
                var result = facade.GetJcActivityIdsByCommodity(dto);
                LogHelper.Debug(string.Format("GetJcActivityIdsByCommodity结果：{0}",JsonConvert.SerializeObject(result)));
                if (result.Code == 0 && result.Data != null)
                {
                    list = result.Data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetJcActivityIdsByCommodity服务异常:", ex);
            }
            return list;
        }

        /// <summary>
        ///  根据金采团购活动Id等获取订单应该给予的油卡兑换券金额
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public decimal GetjcActivityYouKa(Guid actId, List<ComScoreCheckDTO> comScoreCheckDtos)
        {
            LogHelper.Debug("进入GetjcActivityYouKa方法：comScoreCheckDtos：" + JsonHelper.JsSerializer(comScoreCheckDtos));

            decimal tolMoney = 0;
            ReturnInfo<List<JCActivityItemsListCDTO>> list = new ReturnInfo<List<JCActivityItemsListCDTO>>();
            try
            {
                QueryJCActItemsParam queryJcActItemsParam = new QueryJCActItemsParam
                {
                    actId = actId,
                    pageIndex = 1,
                    pageSize = Int32.MaxValue,
                    orderBy = OrderType.ASC
                };

                AMP.ZPH.ISV.Facade.FJCPayFacade facade = new FJCPayFacade();
                list = facade.GetItemsListByActivityId(queryJcActItemsParam);

                LogHelper.Debug("进入GetjcActivityYouKa方法：金采活动列表如下：" + JsonHelper.JsSerializer(list));

                if (list.Data != null)
                {
                    foreach (var comScoreCheckDto in comScoreCheckDtos)
                    {
                        var arr = comScoreCheckDto.ColorAndSize.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 0)
                        {
                            //无属性
                            var jcsActivityItem = list.Data.FirstOrDefault(t => t.ComdtyId == comScoreCheckDto.CommodityId);
                            if (jcsActivityItem != null)
                            {
                                tolMoney += Math.Round((jcsActivityItem.GroupPrice * (jcsActivityItem.GiftGardScale / 100) * comScoreCheckDto.Num), 2);
                            }
                        }
                        else
                        {
                            if (arr.Length == 1)
                            {
                                //单属性
                                var comStocks = CommodityStock.ObjectSet().Where(c => c.CommodityId == comScoreCheckDto.CommodityId).Select(c => new Deploy.CommodityStockDTO { Id = c.Id, ComAttribute = c.ComAttribute }).ToList();
                                if (comStocks.Any())
                                {
                                    foreach (var commodityStockDto in comStocks)
                                    {
                                        var comAttributeDtos = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStockDto.ComAttribute);
                                        if (comAttributeDtos.Any(c => c.SecondAttribute == arr[0]))
                                        {
                                            //有属性商品
                                            var jcsActivityItem = list.Data.FirstOrDefault(t => t.ComdtyId == comScoreCheckDto.CommodityId && t.ComdtyStockId == commodityStockDto.Id);
                                            if (jcsActivityItem != null)
                                            {
                                                tolMoney += Math.Round((jcsActivityItem.GroupPrice * (jcsActivityItem.GiftGardScale / 100) * comScoreCheckDto.Num), 2);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //多属性
                                var comStocks = CommodityStock.ObjectSet().Where(c => c.CommodityId == comScoreCheckDto.CommodityId).Select(c => new Deploy.CommodityStockDTO { Id = c.Id, ComAttribute = c.ComAttribute }).ToList();
                                if (comStocks.Any())
                                {
                                    foreach (var commodityStockDto in comStocks)
                                    {
                                        var comAttributeDtos = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStockDto.ComAttribute);
                                        if (comAttributeDtos.Any(c => c.SecondAttribute == arr[0]) && comAttributeDtos.Any(c => c.SecondAttribute == arr[1]))
                                        {
                                            //有属性商品
                                            var jcsActivityItem = list.Data.FirstOrDefault(t => t.ComdtyId == comScoreCheckDto.CommodityId && t.ComdtyStockId == commodityStockDto.Id);
                                            if (jcsActivityItem != null)
                                            {
                                                tolMoney += Math.Round((jcsActivityItem.GroupPrice * (jcsActivityItem.GiftGardScale / 100) * comScoreCheckDto.Num), 2);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetjcActivityYouKa服务异常:根据活动Id获取商品相关信息异常。", ex);
            }
            return tolMoney;
        }

        /// <summary>
        /// 检验用户是否可以参与指定金采活动
        /// </summary>
        /// <param name="jcActivityId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool IsActivityAuthToUser(Guid jcActivityId, Guid userId)
        {
            try
            {
                AMP.ZPH.ISV.Facade.FJCPayFacade facade = new FJCPayFacade();
                var dto = facade.IsActivityAuthToUser(jcActivityId, userId);
                return dto.Data;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetJCInvoiceByActivityId服务异常:根据活动信息获取活动Id。", ex);
            }
            return false;
        }

        /// <summary>
        /// 根据电商馆appId获取下面的所有入驻的appId
        /// </summary>
        /// <param name="EsAppIds"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<SearchAppPavilion> GetAppIdlist(List<Guid> EsAppIds)
        {
            AMP.ZPH.ISV.Facade.AppPavilionFacade facade = new AppPavilionFacade();
            List<SearchAppPavilion> result = new List<SearchAppPavilion>();
            result = facade.GetAppIdlist(EsAppIds);
            return result;
        }
        /// <summary>
        ///  获取店铺直播列表
        /// </summary>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfo<List<LiveActivityListCDTO>> GetLiveActivityList(Jinher.AMP.ZPH.Deploy.CustomDTO.QueryLiveActivityParam param)
        {
            ReturnInfo<List<LiveActivityListCDTO>> LiveActivityList = new ReturnInfo<List<LiveActivityListCDTO>>();
            try
            {
                AMP.ZPH.ISV.Facade.LiveActivityFacade facade = new LiveActivityFacade();
                LiveActivityList = facade.GetLiveActivityList(param);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.GetLiveActivityList获取直播列表服务异常:", ex);
            }
            return LiveActivityList;
        }

        /// <summary>
        /// 是否专题活动
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool IsSSActivity(Guid ssActId)
        {
            try
            {
                var result = new ZPH.ISV.Facade.SpecialSubjectFacade().IsSSActivity(ssActId);
                if (result.isSuccess) return result.Data;
                LogHelper.Error("ZPHSV.IsSSActivity是否专题活动失败:" + result.Message);
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZPHSV.IsSSActivity是否专题活动异常:", ex);
            }
            return false;
        }
    }
}
