
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/3/26 10:14:48
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class CommodityFacade : BaseFacade<ICommodity>
    {
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        /// <param name="appId">appid</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="fieldSort">排序字段，枚举有对应的值</param>
        /// <param name="order">0为降序，1为升序</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetOrByCommodity
            (System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize, int fieldSort, int order, string areaCode)
        {
            base.Do();
            return this.Command.GetOrByCommodity(categoryId, appId, pageIndex, pageSize, fieldSort, order, areaCode);
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByWhere
            (Jinher.AMP.ZPH.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, int pageIndex, int pageSize, string areaCode)
        {
            base.Do();
            return this.Command.GetCommodityByWhere(commoditySearch, pageIndex, pageSize, areaCode);
        }



        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity
            (System.Guid appId, int pageIndex, int pageSize, string areaCode)
        {
            base.Do();
            return this.Command.GetCommodity(appId, pageIndex, pageSize, areaCode);
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <param name="isChkTime"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity2
            (System.Guid appId, int pageIndex, int pageSize, string areaCode, int isChkTime, DateTime beginTime, DateTime endTime)
        {
            base.Do();
            return this.Command.GetCommodity2(appId, pageIndex, pageSize, areaCode, isChkTime, beginTime, endTime);
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <param name="isChkTime"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity3
            (CommodityListInputDTO input)
        {
            base.Do();
            return this.Command.GetCommodity3(input);
        }

        /// <summary>
        /// 根据搜索条件获取商品
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="want">搜索关键字</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetWantCommodity
            (string want, System.Guid appId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetWantCommodity(want, appId, pageIndex, pageSize);
        }
        /// <summary>
        /// 根据分类获取商品
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByCategory
            (System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetCommodityByCategory(categoryId, appId, pageIndex, pageSize);
        }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetails(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo = "")
        {
            base.Do();
            return this.Command.GetCommodityDetails(commodityId, appId, userId, freightTo);
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetailsNew(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo = "")
        {
            base.Do();
            return this.Command.GetCommodityDetailsNew(commodityId, appId, userId, freightTo);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO CommoditySearch(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.CommoditySearch(appId, commodityCategory, commodityName, pageIndex, pageSize);
        }

        public CommoditySearchForAppsResultDTO CommoditySearchFromApps(List<Guid> appIds, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.CommoditySearchFromApps(appIds, commodityCategory, commodityName, pageIndex, pageSize);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.GetCommoditySearchResultDTO GetCommoditySearch(Guid appId, string commodityName, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetCommoditySearch(appId, commodityName, pageIndex, pageSize);
        }

        public CommodityInfoListDTO GetCommodityInfo(Guid commodityId)
        {
            base.Do();
            return this.Command.GetCommodityInfo(commodityId);
        }

        public ResultDTO CreateUserPrizeRecord(UserPrizeRecordDTO userPrizeRecordDTO)
        {
            base.Do();
            return this.Command.CreateUserPrizeRecord(userPrizeRecordDTO);
        }

        public PrizeRecordDTO GetUserPrizeRecord(Guid promotionId, Guid commodityId, Guid userId)
        {
            base.Do();
            return this.Command.GetUserPrizeRecord(promotionId, commodityId, userId);
        }

        public List<CommodityListCDTO> GetCommodityByIds(List<Guid> commodityIds, bool isDefaultOrder = false)
        {
            base.Do();
            return this.Command.GetCommodityByIds(commodityIds, isDefaultOrder);
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CommodityIdsList">商品list</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodity(Guid UserID, List<Guid> CommodityIdsList)
        {
            base.Do();
            return this.Command.CheckCommodity(UserID, CommodityIdsList);
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CommodityIdsList">商品list</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityNew(Guid UserID, List<CommodityIdAndStockId> CommodityIdsList)
        {
            base.Do();
            return this.Command.CheckCommodityNew(UserID, CommodityIdsList);
        }

        public List<SquareHotCommodityDTO> GetHotCommoditis()
        {
            base.Do();
            return this.Command.GetHotCommoditis();
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO CalFreight(string FreightTo, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> TemplateCounts)
        {
            base.Do();
            return this.Command.CalFreight(FreightTo, TemplateCounts);
        }
        public FreightDetailDTO GetFreightDetails(Guid CommodityId)
        {
            base.Do();
            return this.Command.GetFreightDetails(CommodityId);
        }
        public CommodityZPHResultDTO GetCommoditysZPH(CommoditySearchZPHDTO search)
        {
            base.Do();
            return this.Command.GetCommoditysZPH(search);
        }
        /// <summary>
        /// ZPH商品服务项商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public CommodityZPHResultDTO GetCommoditysforZPH(CommoditySearchZPHDTO search)
        {
            base.Do();
            return this.Command.GetCommoditysforZPH(search);
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
            base.Do();
            return Command.JoinComdtyServiceSetting(ComIds, AppId, ServiceSettingId);
        }
        /// <summary>
        ///  ZPH取消关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        public ResultDTO CancelComdtyServiceSetting(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId)
        {
            base.Do();
            return Command.CancelComdtyServiceSetting(ComIds, AppId, ServiceSettingId);
        }
        public List<CommodityListCDTO> GetCommodityByZPHActId(CommoditySearchZPHDTO search)
        {
            base.Do();
            return this.Command.GetCommodityByZPHActId(search);
        }
        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [Obsolete("请参见新版本", false)]
        public CommodityListResultDTO GetCommodityList(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityList(search);
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPH(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId = new Guid(), string areaCode = "000000")
        {
            base.Do();
            return this.Command.GetCommodityDetailsZPH(commodityId, appId, userId, freightTo, outPromotionId, actId, areaCode);
        }
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNew(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId = new Guid(), string areaCode = "000000")
        {
            base.Do();
            return this.Command.GetCommodityDetailsZPHNew(commodityId, appId, userId, freightTo, outPromotionId, actId, areaCode);
        }

        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSku(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId = new Guid(), string areaCode = "000000")
        {
            base.Do();
            return this.Command.GetCommodityDetailsZPHNewSku(commodityId, appId, userId, freightTo, outPromotionId, actId, areaCode);
        }
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSkuII(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? jcActivityId, System.Guid actId = new Guid(), string areaCode = "000000")
        {
            base.Do();
            return this.Command.GetCommodityDetailsZPHNewSkuII(commodityId, appId, userId, freightTo, jcActivityId, actId, areaCode);
        }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="actId">正品会活动Id</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsByActId(System.Guid actId, System.Guid appId, Guid userId, string freightTo, string areaCode)
        {
            base.Do();
            return this.Command.GetCommodityDetailsByActId(actId, appId, userId, freightTo, areaCode);
        }

        /// <summary>
        /// 活动预约
        /// </summary>
        /// <param name="outPromotionId">外部活动Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="esAppId">预约app</param>
        /// <param name="commodityId"></param>
        /// <param name="commodityStockId"></param>
        /// <returns></returns>
        public ResultDTO SaveMyPresellComdtyZPH(Guid outPromotionId, Guid userId, string verifyCode, Guid esAppId, Guid commodityId, Guid commodityStockId)
        {
            base.Do();
            return Command.SaveMyPresellComdtyZPH(outPromotionId, userId, verifyCode, esAppId, commodityId, commodityStockId);
        }
        public ResultDTO<byte[]> GetVerifyCodeZPH()
        {
            base.Do();
            return Command.GetVerifyCodeZPH();
        }

        //public ResultDTO<byte[]> GetVerifyCodeZPH()
        //{
        //    base.Do();
        //    return Command.GetVerifyCodeZPH();
        //}
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
            base.Do();
            return Command.SaveStockNotificationsZPH(commodityId, userId, esAppId, noticeType, Iscancel);
        }

        public ResultDTO SendNotificationsZPH(Guid commodityId, Guid outPromotionId, Guid esAppId)
        {
            base.Do();
            return Command.SendNotificationsZPH(commodityId, outPromotionId, esAppId);
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="commodityIdsList">商品list</param>
        /// <returns></returns>
        [Obsolete("已废弃，请调用CheckCommodity", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityWithPreSell(Guid userId, List<CommodityIdAndStockId> commodityIdsList)
        {
            base.Do();
            return this.Command.CheckCommodityWithPreSell(userId, commodityIdsList);
        }

        /// <summary>
        /// 清空商品信息的缓存        
        /// </summary> 
        public void RemoveCache()
        {
            base.Do();
            this.Command.RemoveCache();
        }

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerID(CommoditySearchDTO commoditySearch)
        {
            base.Do();
            return this.Command.GetAllCommodityBySellerID(commoditySearch);
        }

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <param name="count"></param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityForCoupon(CommoditySearchDTO commoditySearch, out int count)
        {
            base.Do();
            return this.Command.GetAllCommodityForCoupon(commoditySearch, out count);
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
            base.Do();
            return this.Command.CalFreightMultiApps(freightTo, isSelfTake, templateCounts, coupons, yjbInfo, yjCouponIds);
        }

        /// <summary>
        /// 查询AppID列表下的所有上架的商品
        /// </summary>
        /// <param name="appListSearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityByAppIdList(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchByAppIdListDTO appListSearch)
        {
            base.Do();
            return this.Command.GetAllCommodityByAppIdList(appListSearch);
        }

        /// <summary>
        /// 根据商品ID列表获取商品是否支持自提的信息
        /// </summary>
        /// <param name="commodityIdList">商品ID列表</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO> GetCommodityIsEnableSelfTakeList(List<Guid> commodityIdList)
        {
            base.Do();
            return this.Command.GetCommodityIsEnableSelfTakeList(commodityIdList);
        }
        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsWithPreSell(List<Guid> commodityIds, bool isDefaultOrder)
        {
            base.Do();
            return this.Command.GetCommodityByIdsWithPreSell(commodityIds, isDefaultOrder);

        }

        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongTo(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder)
        {
            base.Do();
            return this.Command.GetCommodityByIdsWithPreSellInBeLongTo(beLongTo, commodityIds, isDefaultOrder);

        }
        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongToWithType(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder, int mallAppType)
        {
            base.Do();
            return this.Command.GetCommodityByIdsWithPreSellInBeLongToWithType(beLongTo, commodityIds, isDefaultOrder, mallAppType);

        }
        /// <summary>
        /// 获取商品属性库存
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<ComAttStockDTO> CommodityAttrStocks(CommoditySearchDTO search)
        {
            base.Do();
            return this.Command.CommodityAttrStocks(search);
        }
        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public ResultDTO SaveServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            base.Do();
            return this.Command.SaveServiceCommodity(serviceCommodityAndCategoryDTO);
        }

        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            base.Do();
            return this.Command.UpdateServiceCommodity(serviceCommodityAndCategoryDTO);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommodity(Guid id)
        {
            base.Do();
            return this.Command.DeleteServiceCommodity(id);
        }
        /// <summary>
        /// 删除多个商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommoditys(List<Guid> ids)
        {
            base.Do();
            return this.Command.DeleteServiceCommoditys(ids);
        }
        /// <summary>
        /// 获取点餐列表数据       
        /// </summary>
        /// <param name="search">查询条件model：必传参数AppId</param>
        /// <returns></returns>
        public ResultDTO<CateringCommodityDTO> GetCateringCommodity(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCateringCommodity(search);
        }

        /// <summary>
        /// 判断商品是不是定制应用所发布的商品。
        /// </summary>
        /// <returns></returns>
        public ResultDTO<bool> IsFittedAppCommodity(Guid commodityId)
        {
            base.Do();
            return this.Command.IsFittedAppCommodity(commodityId);
        }

        /// <summary>
        /// 获取发布商品的店铺的appId
        /// </summary>
        /// <returns></returns>
        public ResultDTO<Guid> GetCommodityAppId(Guid commodityId)
        {
            base.Do();
            return this.Command.GetCommodityAppId(commodityId);
        }

        /// <summary>
        /// 获取商品简略信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO<CommodityThumb> GetCommodityThumb(Guid commodityId)
        {
            base.Do();
            return this.Command.GetCommodityThumb(commodityId);
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttribute(System.Guid commodityId, Guid userId)
        {
            base.Do();
            return this.Command.GetCommodityAttribute(commodityId, userId);
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttributeNew(System.Guid commodityId, Guid userId)
        {
            base.Do();
            return this.Command.GetCommodityAttributeNew(commodityId, userId);
        }

        /// <summary>
        /// 多应用：根据商品Ids获取商品信息列表
        /// </summary>
        /// <param name="search">查询条件，有效参数CommodityIds,IsDefaultOrder,AreaCode</param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsNew(CommoditySearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityByIdsNew(search);
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3(CheckCommodityParam ccp)
        {
            base.Do();
            return this.Command.CheckCommodityV3(ccp);
        }
        /// <summary>
        /// 校验商品信息  金采团购活动
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3II(CheckCommodityParam ccp)
        {
            base.Do();
            return this.Command.CheckCommodityV3II(ccp);
        }
        /// <summary>
        /// 校验购物车商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckShopCommodityDTO> CheckCommodityV4(CheckShopCommodityParam ccp)
        {
            base.Do();
            return this.Command.CheckCommodityV4(ccp);
        }

        /// <summary>
        /// 获取商品列表，提供跨店铺优惠券
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityListForCoupon(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityListForCoupon(search);
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityListV2(search);
        }

        /// <summary>
        /// xiexg优惠券获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityListV2_New(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityListV2_New(search);
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityList3(CommodityListSearchDTO search)
        {
            base.Do();
            return this.Command.GetCommodityList3(search);
        }
        /// <summary>
        /// 商品改低价格时，Job发送消息(每10分钟处理一次)
        /// </summary>
        public void AutoPushCommodityModifyPrice()
        {
            base.Do();
            this.Command.AutoPushCommodityModifyPrice();
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV5(CheckCommodityParam ccp)
        {
            base.Do();
            return this.Command.CheckCommodityV5(ccp);
        }
        /// <summary>
        /// 获取商品列表关税合计
        /// </summary> 
        /// <param name="search">商品列表</param>
        /// <returns></returns>
        public ResultDTO<CreateOrderDutyResultDTO> GetComListDuty(List<ComScoreCheckDTO> search)
        {
            base.Do();
            return this.Command.GetComListDuty(search);
        }

        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto)
        {
            base.Do();
            return this.Command.GetCommodityByName(pdto);
        }

        /// <summary>
        /// 同步京东库存
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDStock(Jinher.AMP.BTP.Deploy.CommodityDTO arg)
        {
            base.Do();
            return this.Command.SynchronizeJDStock(arg);
        }

        /// <summary>
        /// 订单或订单项拒收或取件运费计算
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalRefundFreight(Guid orderId, Guid orderItemId)
        {
            base.Do();
            return this.Command.CalRefundFreight(orderId, orderItemId);
        }

        /// <summary>
        /// 查询某个APP下的商品 按照销量进行相关排序
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityWithSales(CommoditySearchDTO commoditySearch)
        {
            base.Do();
            return this.Command.GetAllCommodityWithSales(commoditySearch);
        }

        /// <summary>
        /// 更新商品表的赠送油卡量
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityYouka(Guid commodityId, decimal youka)
        {
            base.Do();
            return this.Command.UpdateCommodityYouka(commodityId, youka);
        }
		

        /// <summary>
        /// 获取商品税收编码列表(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<CommodityTaxRateZphDto> GetSingleCommodityCode()
        {
            base.Do();
            return this.Command.GetSingleCommodityCode();
        }


        /// <summary>
        /// 同步商品库存(openApi)
        /// </summary>
        public ThirdResponse ModifyCommodityStock(string Code, string skuId, int Stock)
        {
            base.Do();
            return this.Command.ModifyCommodityStock(Code,skuId,Stock);
        }

        /// <summary>
        /// 添加商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse AddCommodity(List<Commoditydto> objlist)
        {
            base.Do();
            return this.Command.AddCommodity(objlist);
        }


        /// <summary>
        /// 修改商品名称(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityName(string skuId, string CommodityName)
        {
            base.Do();
            return this.Command.ModifyCommodityName(skuId,CommodityName);
        }

        /// <summary>
        /// 修改商品价格(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityPrice(string skuId, decimal Price)
        {
            base.Do();
            return this.Command.ModifyCommodityPrice(skuId, Price);
        }


        /// <summary>
        /// 修改商品上下架(openApi)
        /// </summary>
        /// <param name="SupId"></param>
        /// <param name="State">0上架，1下架</param>
        /// <returns></returns>
        public ThirdResponse Upperandlower(string SupId, int State)
        {
            base.Do();
            return this.Command.Upperandlower(SupId,State);
        }


        #region NetCore刷新缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity1()
        {
            return this.Command.NetCoreAutoAuditJdCommodity1();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity(JdBTPRefreshCache dict)
        {
            return this.Command.NetCoreAutoAuditJdCommodity(dict);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdPromotions(JdBTPRefreshCache dict)
        {
            return this.Command.NetCoreAutoAuditJdPromotions(dict);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCountInfo(JdBTPRefreshCache dict)
        {
            return this.Command.NetCoreAutoAuditJdCountInfo(dict);

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
            base.Do();
            return this.Command.GetOrderIdComInfo(Code, OrderId, Commodityid);
        }
    }
}
