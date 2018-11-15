
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/7/11 17:24:53
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using System.Diagnostics;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class PromotionSV : BaseSv, IPromotion
    {

        /// <summary>
        /// 获取最新促销
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.PromotionSV.svc/GetNewPromotion
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.PromotionHotSDTO GetNewPromotion(System.Guid appId)
        {
            base.Do(false);
            return this.GetNewPromotionExt(appId);

        }
        /// <summary>
        /// 易捷点滴接口
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.PromotionSV.svc/GetYJDianDi
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetYJDianDi(System.Guid appId)
        {
            base.Do(false);
            return this.GetYJDianDiExt(appId);

        }
        /// <summary>
        /// 获取最新促销商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.PromotionSV.svc/GetPromotionItems
        /// </para>
        /// </summary>
        /// <param name="promotionId">促销ID</param>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetPromotionItems(System.Guid promotionId, System.Guid appId, int pageIndex, int pageSize)
        {
            base.Do(false);
            return this.GetPromotionItemsExt(promotionId, appId, pageIndex, pageSize);

        }
        /// <summary>
        /// 获取所有商品折扣信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemShortCDTO> GetAllPromotionItems()
        {
            base.Do(false);
            return this.GetAllPromotionItemsExt();

        }
        /// <summary>
        /// 获取当日商品促销信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.Dictionary<System.Guid, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.TodayPromotionDTO>> GetAppPromotions()
        {
            base.Do(false);
            return this.GetAppPromotionsExt();

        }
        public void AddHotCommodity()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.AddHotCommodityExt();
            timer.Stop();
            LogHelper.Info(string.Format("PromotionSV.AddHotCommodity：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        /// <summary>
        /// 获取所有门店信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreCacheDTO> GetAllStores()
        {
            base.Do(false);
            return this.GetAllStoresExt();

        }
        /// <summary>
        /// 获取所有商品属性
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttributeCacheDTO> GetAllCommAttributes()
        {
            base.Do(false);
            return this.GetAllCommAttributesExt();

        }
        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCacheDTO> GetAllCateGories()
        {
            base.Do(false);
            return this.GetAllCateGoriesExt();

        }
        /// <summary>
        /// 推送促销信息
        /// </summary>
        /// <returns></returns>
        public void PromotionPush()
        {
            base.Do(false);
            this.PromotionPushExt();

        }
        /// <summary>
        /// 修改评价表用户信息
        /// </summary>
        public void UpdateUserInfo()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.UpdateUserInfoExt();
            timer.Stop();
            LogHelper.Info(string.Format("PromotionSV.UpdateUserInfo：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        public void PromotionPushIUS()
        {
            base.Do(false);
            this.PromotionPushIUSExt();

        }
        /// <summary>
        /// 查询商品即将开始的秒杀活动
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.PromotionItemShortCDTO GetSecKillPromotion(System.Guid commodityId)
        {
            base.Do(false);
            return this.GetSecKillPromotionExt(commodityId);

        }
        /// <summary>
        /// 判断是否可以购买 商品活动进行中，或者没有即将开始的秒杀活动可以购买
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public bool CheckSecKillBuy(System.Guid commodityId)
        {
            base.Do(false);
            return this.CheckSecKillBuyExt(commodityId);

        }
        /// <summary>
        /// 添加折扣
        /// </summary>
        /// <param name="discountsDTO">折扣属性</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddOutsidePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO)
        {
            base.Do(false);
            return this.AddOutsidePromotionExt(discountsDTO);

        }
        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="outsideId">外部活动id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelOutsidePromotion(System.Guid outsideId)
        {
            base.Do(false);
            return this.DelOutsidePromotionExt(outsideId);

        }
        /// <summary>
        /// 修改折扣(活动开始后不允许修改)
        /// </summary>
        /// <param name="discountsDTO">折扣属性</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOutsidePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO)
        {
            base.Do(false);
            return this.UpdateOutsidePromotionExt(discountsDTO);

        }
        /// <summary>
        /// 设置外部活动订单不支付过期时间
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetExpireSeconds(System.Guid appId, long seconds)
        {
            base.Do(false);
            return this.SetExpireSecondsExt(appId, seconds);

        }
        /// <summary>
        /// 数据库中商品活动信息与Redis中保存商品活动信息同步
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CommodityDataAndRedisDataSynchronization()
        {
            base.Do(false);
            return this.CommodityDataAndRedisDataSynchronizationExt();

        }
        /// <summary>
        /// 根据活动ID数据库中商品活动信息与Redis中保存商品活动信息同步
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO PromotionRedis(System.Guid guid)
        {
            base.Do(false);
            return this.PromotionRedisExt(guid);

        }
        /// <summary>
        /// 获取当日商品促销信息
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.TodayPromotionDTO> GeTodayPromotions(System.Collections.Generic.List<System.Guid?> outsideId)
        {
            base.Do(false);
            return this.GeTodayPromotionsExt(outsideId);

        }
        /// <summary>
        /// 获取当日商品促销购买数量
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.PromotionSurplusLimitBuyTotalDto> GetSurplusLimitBuyTotal(System.Collections.Generic.List<System.Guid> outsideId)
        {
            base.Do(false);
            return this.GetSurplusLimitBuyTotalExt(outsideId);

        }
        /// <summary>
        /// 获取商城类目
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryList(System.Guid AppId)
        {
            base.Do(false);
            return this.GetCategoryListExt(AppId);

        }

         /// <summary>
        /// 获取应用的一级商品分类
        /// <para>
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryL1(Guid appId)
        {
            base.Do(false); //有内部调用，暂时用false
            return this.GetCategoryL1Ext(appId);
        }
    }
}
