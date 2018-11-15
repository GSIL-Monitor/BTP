
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/3/26 10:14:50
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Diagnostics;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommoditySV : BaseSv, ICommodity
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetOrByCommodityExt(categoryId, appId, pageIndex, pageSize, fieldSort, order, areaCode);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetOrByCommodity：耗时：{0}。入参：categoryId:{1},appId:{2},pageIndex:{3},pageSize:{4},fieldSort:{5},order:{6},areaCode:{7},\r\n出参：{8}", timer.ElapsedMilliseconds, categoryId, appId, pageIndex, pageSize, fieldSort, order, areaCode, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByWhere(Jinher.AMP.ZPH.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, int pageIndex, int pageSize, string areaCode)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByWhereExt(commoditySearch, pageIndex, pageSize, areaCode);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByWhere：耗时：{0}。入参：appId:{1},pageIndex:{2},pageSize:{3},areaCode:{4}", timer.ElapsedMilliseconds, pageIndex, pageSize, areaCode, JsonHelper.JsonSerializer(result)));
            return result;

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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityExt(appId, pageIndex, pageSize, areaCode);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodity：耗时：{0}。入参：appId:{1},pageIndex:{2},pageSize:{3},areaCode:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, appId, pageIndex, pageSize, areaCode, JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 获取商品列表 带促销查询  
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <param name="isChkTime">是否查询促销</param>
        /// <param name="beginTime">促销开始时间</param>
        /// <param name="endTime">促销结束时间</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity2
            (System.Guid appId, int pageIndex, int pageSize, string areaCode, int isChkTime, DateTime beginTime, DateTime endTime)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodity2Ext(appId, pageIndex, pageSize, areaCode, isChkTime, beginTime, endTime);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodity2：耗时：{0}。入参：appId:{1},pageIndex:{2},pageSize:{3},areaCode:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, appId, pageIndex, pageSize, areaCode, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 获取商品列表 带促销查询  
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <param name="isChkTime">是否查询促销</param>
        /// <param name="beginTime">促销开始时间</param>
        /// <param name="endTime">促销结束时间</param>
        /// <returns></returns>
        public ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity3
            (CommodityListInputDTO input)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodity3Ext(input);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodity3：耗时：{0}。入参：appId:{1}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(input)));
            return result;
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetWantCommodityExt(want, appId, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetWantCommodity：耗时：{0}。入参：want:{1},appId:{2},pageIndex:{3},pageSize:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, want, appId, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;

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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByCategoryExt(categoryId, appId, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByCategory：耗时：{0}。入参：categoryId:{1},appId:{2},pageIndex:{3},pageSize:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, categoryId, appId, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;

        }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetails(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo = "")
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityDetailsExt(commodityId, appId, userId, freightTo);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityDetails：耗时：{0}。入参：commodityId:{1},appId:{2},userId:{3},freightTo:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, commodityId, appId, userId, freightTo, JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetailsNew(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo = "")
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityDetailsNewExt(commodityId, appId, userId, freightTo);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityDetailsNew：耗时：{0}。入参：commodityId:{1},appId:{2},userId:{3},freightTo:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, commodityId, appId, userId, freightTo, JsonHelper.JsonSerializer(result)));
            return result;

        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO CommoditySearch(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CommoditySearchExt(appId, commodityCategory, commodityName, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CommoditySearch：耗时：{0}。入参：appId:{1},commodityCategory:{2},commodityName:{3},pageIndex:{4},pageSize:{5},\r\n出参：{6}", timer.ElapsedMilliseconds, appId, commodityCategory, commodityName, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public CommoditySearchForAppsResultDTO CommoditySearchFromApps(List<Guid> appIds, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CommoditySearchFromAppsExt(appIds, commodityCategory, commodityName, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CommoditySearchFromApps：耗时：{0}。入参：appIds:{1},commodityCategory:{2},commodityName:{3},pageIndex:{4},pageSize:{5},\r\n出参：{6}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(appIds), commodityCategory, commodityName, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.GetCommoditySearchResultDTO GetCommoditySearch(Guid appId, string commodityName, int pageIndex, int pageSize)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommoditySearchExt(appId, commodityName, pageIndex, pageSize);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommoditySearch：耗时：{0}。入参：appId:{1},commodityName:{2},pageIndex:{3},pageSize:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, appId, commodityName, pageIndex, pageSize, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public CommodityInfoListDTO GetCommodityInfo(Guid commodityId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityInfoExt(commodityId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityInfo：耗时：{0}。入参：commodityId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, commodityId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public ResultDTO CreateUserPrizeRecord(UserPrizeRecordDTO userPrizeRecordDTO)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CreateUserPrizeRecordExt(userPrizeRecordDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CreateUserPrizeRecord：耗时：{0}。入参：userPrizeRecordDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(userPrizeRecordDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }
        public PrizeRecordDTO GetUserPrizeRecord(Guid promotionId, Guid commodityId, Guid userId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetUserPrizeRecordExt(promotionId, commodityId, userId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetUserPrizeRecord：耗时：{0}。入参：promotionId:{1},commodityId:{2},userId:{3},\r\n出参：{4}", timer.ElapsedMilliseconds, promotionId, commodityId, userId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public List<CommodityListCDTO> GetCommodityByIds(List<Guid> commodityIds, bool isDefaultOrder)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByIdsExt(commodityIds, isDefaultOrder);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByIds：耗时：{0}。入参：commodityIds:{1},isDefaultOrder:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(commodityIds), isDefaultOrder, JsonHelper.JsonSerializer(result)));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CheckCommodityExt(UserID, CommodityIdsList);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CheckCommodity：耗时：{0}。入参：UserID:{1},CommodityIdsList:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, UserID, JsonHelper.JsonSerializer(CommodityIdsList), JsonHelper.JsonSerializer(result)));
            return result;
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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CheckCommodityNewExt(UserID, CommodityIdsList);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNew：耗时：{0}。入参：UserID:{1},CommodityIdsList:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, UserID, JsonHelper.JsonSerializer(CommodityIdsList), JsonHelper.JsonSerializer(result)));
            return result;
        }

        public List<SquareHotCommodityDTO> GetHotCommoditis()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetHotCommoditisExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetHotCommoditis：耗时：{0}。出参：{1}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 运费计算
        /// </summary>
        /// <param name="FreightTo"></param>
        /// <param name="TemplateCounts"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO CalFreight(string FreightTo, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> TemplateCounts)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CalFreightExt(FreightTo, TemplateCounts);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CalFreight：耗时：{0}。入参：FreightTo:{1},TemplateCounts:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, FreightTo, JsonHelper.JsonSerializer(TemplateCounts), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 运费详细列表
        /// </summary>
        /// <param name="CommodityId">商品编号</param>
        /// <returns></returns>
        public FreightDetailDTO GetFreightDetails(Guid CommodityId)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetFreightDetailsExt(CommodityId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetFreightDetails：耗时：{0}。入参：CommodityId:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, CommodityId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public CommodityZPHResultDTO GetCommoditysZPH(CommoditySearchZPHDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommoditysZPHExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommoditysZPH：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// ZPH商品服务项商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public CommodityZPHResultDTO GetCommoditysforZPH(CommoditySearchZPHDTO search)
        {
            base.Do(false);
            return this.GetCommoditysforZPHExt(search);
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
            base.Do(false);
            return this.JoinComdtyServiceSettingExt(ComIds, AppId, ServiceSettingId);
        }
        /// <summary>
        ///ZPH取消关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        public ResultDTO CancelComdtyServiceSetting(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId)
        {
            base.Do(false);
            return this.CancelComdtyServiceSettingExt(ComIds, AppId, ServiceSettingId);
        }
        /// <summary>
        /// 根据正品会活动id获取对应分页信息的商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByZPHActId(CommoditySearchZPHDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByZPHActIdExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByZPHActId：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public CommodityListResultDTO GetCommodityList(CommodityListSearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityListExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityList：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId">活动Id（ios兼容）</param>
        /// <param name="areaCode">当前地区(ios兼容)</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPH(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId = new Guid(), string areaCode = ProvinceCityHelper.CountryCode)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            ResultDTO<CommoditySDTO> result;
            if (actId != Guid.Empty)
            {
                result = this.GetCommodityDetailsByActIdExt(actId, appId, userId, freightTo, areaCode);
            }
            else
            {
                result = this.GetCommodityDetailsZPHExt(commodityId, appId, userId, freightTo, outPromotionId);
            }
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityDetailsZPH：耗时：{0}。入参：commodityId:{1},appId:{2},userId:{3},freightTo:{4},outPromotionId:{5},actId:{7},areaCode:{8},\r\n出参：{6}", timer.ElapsedMilliseconds, commodityId, appId, userId, freightTo, outPromotionId, JsonHelper.JsonSerializer(result), actId, areaCode));
            return result;
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId">活动Id（ios兼容）</param>
        /// <param name="areaCode">当前地区(ios兼容)</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNew(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId = new Guid(), string areaCode = ProvinceCityHelper.CountryCode)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            ResultDTO<CommoditySDTO> result;
            if (actId != Guid.Empty)
            {
                result = this.GetCommodityDetailsByActIdExt(actId, appId, userId, freightTo, areaCode);
            }
            else
            {
                result = this.GetCommodityDetailsZPHNewExt(commodityId, appId, userId, freightTo, outPromotionId);
            }
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityDetailsZPH：耗时：{0}。入参：commodityId:{1},appId:{2},userId:{3},freightTo:{4},outPromotionId:{5},actId:{7},areaCode:{8},\r\n出参：{6}", timer.ElapsedMilliseconds, commodityId, appId, userId, freightTo, outPromotionId, JsonHelper.JsonSerializer(result), actId, areaCode));
            return result;
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId">活动Id（ios兼容）</param>
        /// <param name="areaCode">当前地区(ios兼容)</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSku(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId = new Guid(), string areaCode = ProvinceCityHelper.CountryCode)
        {
            base.Do(false);
            using (StopwatchLogHelper.BeginScope("CommoditySV.GetCommodityDetailsZPH"))
            {
                //LogHelper.Info("GetCommodityDetailsZPHNewSku-actId：" + actId);  00000000-0000-0000-0000-000000000000
                ResultDTO<CommoditySDTO> result;
                if (actId != Guid.Empty)
                {
                    result = this.GetCommodityDetailsByActIdExt(actId, appId, userId, freightTo, areaCode);
                }
                else
                {
                    result = this.GetCommodityDetailsZPHNewExt(commodityId, appId, userId, freightTo, outPromotionId);                    
                }                

                return result;
            }
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="jcActivityId">金采团购活动Id</param>
        /// <param name="actId">活动Id（ios兼容）</param>
        /// <param name="areaCode">当前地区(ios兼容)</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSkuII(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? jcActivityId, System.Guid actId = new Guid(), string areaCode = ProvinceCityHelper.CountryCode)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityDetailsZPHNewIIExt(commodityId, appId, userId, freightTo, jcActivityId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityDetailsZPHNewSkuII：耗时：{0}。入参：commodityId:{1},appId:{2},userId:{3},freightTo:{4},jcActivityId:{5},actId:{7},areaCode:{8},\r\n出参：{6}", timer.ElapsedMilliseconds, commodityId, appId, userId, freightTo, jcActivityId, JsonHelper.JsonSerializer(result), actId, areaCode));
            return result;
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityDetailsByActIdExt(actId, appId, userId, freightTo, areaCode);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityDetailsByActId：耗时：{0}。入参：actId:{1},appId:{2},userId:{3},freightTo:{4},areaCode:{5},\r\n出参：{6}", timer.ElapsedMilliseconds, actId, appId, userId, freightTo, areaCode, JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 活动预约
        /// </summary>
        /// <param name="outPromotionId">外部活动Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="esAppId">预约app</param>
        /// <returns></returns>
        public ResultDTO SaveMyPresellComdtyZPH(Guid outPromotionId, Guid userId, string verifyCode, Guid esAppId, Guid commodityId, Guid commodityStockId)
        {
            LogHelper.Info(string.Format("SaveMyPresellComdtyZPH：入参：commodityId:{0},commodityStockId:{1}", commodityId, commodityStockId));
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveMyPresellComdtyZPHExt(outPromotionId, userId, verifyCode, esAppId, commodityId, commodityStockId);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.SaveMyPresellComdtyZPH：耗时：{0}。入参：outPromotionId:{1},userId:{2},verifyCode:{3},commodityId:{4},commodityStockId:{5},\r\n出参：{4}", timer.ElapsedMilliseconds, outPromotionId, userId, verifyCode, commodityId, commodityStockId, JsonHelper.JsonSerializer(result)));
            return result;
        }

        public ResultDTO<byte[]> GetVerifyCodeZPH()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetVerifyCodeZPHExt();
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetVerifyCodeZPH：耗时：{0}。出参：{1}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(result)));
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SaveStockNotificationsZPHExt(commodityId, userId, esAppId, noticeType, Iscancel);
            timer.Stop();
            return result;
        }

        public ResultDTO SendNotificationsZPH(Guid commodityId, Guid outPromotionId, Guid esAppId)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.SendNotificationsZPHExt(commodityId, outPromotionId, esAppId);
            timer.Stop();
            return result;


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
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CheckCommodityWithPreSellExt(userId, commodityIdsList);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CheckCommodityWithPreSell：耗时：{0}。入参：userId:{1},commodityIdsList:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, userId, JsonHelper.JsonSerializer(commodityIdsList), JsonHelper.JsonSerializer(result)));
            return result;

        }

        /// <summary>
        /// 清空商品信息的缓存        
        /// </summary> 
        public void RemoveCache()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.RemoveCacheExt();
            timer.Stop();
            LogHelper.Info(string.Format("CommoditySV.RemoveCache：耗时：{0}。", timer.ElapsedMilliseconds));

        }

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerID(CommoditySearchDTO commoditySearch)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetAllCommodityBySellerIDExt(commoditySearch);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetAllCommodityBySellerID：耗时：{0}。入参：commoditySearch:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(commoditySearch), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 查询某个APP下的商品 
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityForCoupon(CommoditySearchDTO commoditySearch, out int count)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetAllCommodityForCouponExt(commoditySearch, out count);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetAllCommodityForCoupon：耗时：{0}。入参：commoditySearch:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(commoditySearch), JsonHelper.JsonSerializer(result)));
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
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CalFreightMultiAppsExt(freightTo, isSelfTake, templateCounts, coupons, yjbInfo, yjCouponIds);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CalFreightMultiApps：耗时：{0}。入参：freightTo:{1},isSelfTake:{2},templateCounts:{3},\r\n出参：{4}", timer.ElapsedMilliseconds, freightTo, isSelfTake, JsonHelper.JsonSerializer(templateCounts), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 查询AppID列表下的所有上架的商品
        /// </summary>
        /// <param name="appListSearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityByAppIdList(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchByAppIdListDTO appListSearch)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetAllCommodityByAppIdListExt(appListSearch);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetAllCommodityByAppIdList：耗时：{0}。入参：appListSearch:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(appListSearch), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 根据商品ID列表获取商品是否支持自提的信息
        /// </summary>
        /// <param name="commodityIdList">商品ID列表</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO> GetCommodityIsEnableSelfTakeList(List<Guid> commodityIdList)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityIsEnableSelfTakeListExt(commodityIdList);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityIsEnableSelfTakeList：耗时：{0}。入参：commodityIdList:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(commodityIdList), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsWithPreSell(List<Guid> commodityIds, bool isDefaultOrder)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByIdsWithPreSellExt(commodityIds, isDefaultOrder);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByIdsWithPreSell：耗时：{0}。入参：commodityIds:{1},isDefaultOrder:{2},\r\n出参：{3}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(commodityIds), isDefaultOrder, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongTo(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByIdsWithPreSellInBeLongToExt(beLongTo, commodityIds, isDefaultOrder);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByIdsWithPreSellInBeLongTo：耗时：{0}。入参：beLongTo:{1},commodityIds:{2},isDefaultOrder:{3},\r\n出参：{4}", timer.ElapsedMilliseconds, beLongTo, JsonHelper.JsonSerializer(commodityIds), isDefaultOrder, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongToWithType(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder, int mallAppType)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByIdsWithPreSellInBeLongToWithTypeExt(beLongTo, commodityIds, isDefaultOrder, mallAppType);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByIdsWithPreSellInBeLongToWithType：耗时：{0}。入参：beLongTo:{1},commodityIds:{2},isDefaultOrder:{3},mallAppType:{4},\r\n出参：{5}", timer.ElapsedMilliseconds, beLongTo, JsonHelper.JsonSerializer(commodityIds), isDefaultOrder, mallAppType, JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取商品属性
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<ComAttStockDTO> CommodityAttrStocks(CommoditySearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CommodityAttrStocksExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.CommodityAttrStocks：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public ResultDTO SaveServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = SaveServiceCommodityExt(serviceCommodityAndCategoryDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.SaveServiceCommodity：耗时：{0}。入参：submitOrderRefundDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(serviceCommodityAndCategoryDTO), JsonHelper.JsonSerializer(result)));
            return result;


        }

        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {

            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = UpdateServiceCommodityExt(serviceCommodityAndCategoryDTO);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.UpdateServiceCommodity：耗时：{0}。入参：submitOrderRefundDTO:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(serviceCommodityAndCategoryDTO), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommodity(Guid id)
        {
            base.Do();
            return this.DeleteServiceCommodityExt(id);
        }
        /// <summary>
        /// 删除多个商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommoditys(List<Guid> ids)
        {
            base.Do();
            return this.DeleteServiceCommoditysExt(ids);
        }
        /// <summary>
        /// 获取点餐列表数据       
        /// </summary>
        /// <param name="search">查询条件model：必传参数AppId</param>
        /// <returns></returns>
        public ResultDTO<CateringCommodityDTO> GetCateringCommodity(CommodityListSearchDTO search)
        {
            base.Do(false);
            return this.GetCateringCommodityExt(search);
        }

        /// <summary>
        /// 判断商品是不是定制应用所发布的商品。
        /// </summary>
        /// <returns></returns>
        public ResultDTO<bool> IsFittedAppCommodity(Guid commodityId)
        {
            base.Do(false);
            return this.IsFittedAppCommodityExt(commodityId);
        }

        /// <summary>
        /// 获取发布商品的店铺的appId
        /// </summary>
        /// <returns></returns>
        public ResultDTO<Guid> GetCommodityAppId(Guid commodityId)
        {
            base.Do(false);
            return this.GetCommodityAppIdExt(commodityId);
        }

        /// <summary>
        /// 获取商品简略信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO<CommodityThumb> GetCommodityThumb(Guid commodityId)
        {
            base.Do(false);
            return this.GetCommodityThumbExt(commodityId);
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttribute(System.Guid commodityId, Guid userId)
        {
            base.Do(false);
            return this.GetCommodityAttributeExt(commodityId, userId);
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttributeNew(System.Guid commodityId, Guid userId)
        {
            base.Do(false);
            return this.GetCommodityAttributeNewExt(commodityId, userId);
        }

        /// <summary>
        /// 多应用：根据商品Ids获取商品信息列表
        /// </summary>
        /// <param name="search">查询条件，有效参数commodityIdList,IsDefaultOrder,AreaCode</param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityByIdsNew(CommoditySearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityByIdsNewExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityByIdsNew：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3(CheckCommodityParam ccp)
        {
            base.Do(false);
            return this.CheckCommodityV3Ext(ccp);
        }

        /// <summary>
        /// 校验商品信息  金采团购活动
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3II(CheckCommodityParam ccp)
        {
            base.Do(false);
            return this.CheckCommodityV3IIExt(ccp);
        }

        /// <summary>
        /// 校验购物车商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckShopCommodityDTO> CheckCommodityV4(CheckShopCommodityParam ccp)
        {
            base.Do(false);
            return this.CheckCommodityV4Ext(ccp);
        }

        public ComdtyListResultCDTO GetCommodityListForCoupon(CommodityListSearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityListForCouponExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityListForCoupon：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        public ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityListV2Ext(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityListV2：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// xiexg最新优惠券
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityListV2_New(CommodityListSearchDTO search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetCommodityListV2_NewExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityListV2_New：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 店铺商品列表6-28
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ComdtyListResultCDTO GetCommodityList3(CommodityListSearchDTO search)
        {
            base.Do(false);
            return this.GetCommodityList3Ext(search);
        }
        /// <summary>
        /// 商品改低价格时，Job发送消息(每10分钟处理一次)
        /// </summary>
        public void AutoPushCommodityModifyPrice()
        {
            base.Do(false);
            this.AutoPushCommodityModifyPriceExt();
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV5(CheckCommodityParam ccp)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CheckCommodityV5Ext(ccp);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.GetCommodityListV5：耗时：{0}。入参：ccp:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(ccp), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 获取商品列表关税合计
        /// </summary> 
        /// <param name="search">商品列表</param>
        /// <returns></returns>
        public ResultDTO<CreateOrderDutyResultDTO> GetComListDuty(List<ComScoreCheckDTO> search)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetComListDutyExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("CommoditySV.SumComListDuty：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }

        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto)
        {
            base.Do(false);
            return this.GetCommodityByNameExt(pdto);
        }

        /// <summary>
        /// 同步京东库存
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDStock(Jinher.AMP.BTP.Deploy.CommodityDTO arg)
        {
            base.Do(true);
            return this.SynchronizeJDStockExt(arg);
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
            return this.CalRefundFreightExt(orderId, orderItemId);
        }


        /// <summary>
        /// 查询某个APP下的商品 按照销量进行相关排序
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityWithSales(CommoditySearchDTO commoditySearch)
        {
            base.Do(false);
            return this.GetAllCommodityWithSalesExt(commoditySearch);
        }

        //public ResultDTO Update

        public ResultDTO UpdateCommodityYouka(Guid commodityId, decimal youka)
        {
            base.Do(false);
            return this.UpdateCommodityYoukaExt(commodityId, youka);
        }

        /// <summary>
        /// 获取商品税收编码列表(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<CommodityTaxRateZphDto> GetSingleCommodityCode()
        {
            base.Do(false);
            return this.GetSingleCommodityCodeExt();
        }

        /// <summary>
        /// 同步商品库存(openApi)
        /// </summary>
        public ThirdResponse ModifyCommodityStock(string Code, string skuId, int Stock)
        {
            base.Do(false);
            return this.ModifyCommodityStockExt(Code, skuId, Stock);
        }


        /// <summary>
        /// 添加商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse AddCommodity(List<Commoditydto> objlist)
        {
            base.Do(false);
            return this.AddCommodityExt(objlist);
        }

        /// <summary>
        /// 修改商品名称(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityName(string skuId, string CommodityName)
        {
            base.Do(false);
            return this.ModifyCommodityNameExt(skuId, CommodityName);
        }

        /// <summary>
        /// 修改商品价格(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityPrice(string skuId, decimal Price)
        {
            base.Do(false);
            return this.ModifyCommodityPriceExt(skuId, Price);
        }

        /// <summary>
        /// 修改商品上下架(openApi)
        /// </summary>
        /// <param name="SupId"></param>
        /// <param name="State">0上架，1下架</param>
        /// <returns></returns>
        public ThirdResponse Upperandlower(string SupId, int State)
        {
            base.Do(false);
            return this.UpperandlowerExt(SupId, State);
        }


        #region NetCore刷新缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity1()
        {
            return this.NetCoreAutoAuditJdCommodity1Ext();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity(JdBTPRefreshCache dict)
        {
            return this.NetCoreAutoAuditJdCommodityExt(dict);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdPromotions(JdBTPRefreshCache dict)
        {
            return this.NetCoreAutoAuditJdPromotionsExt(dict);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCountInfo(JdBTPRefreshCache dict)
        {
            return this.NetCoreAutoAuditJdCountInfoExt(dict);
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
            base.Do(false);
            return this.GetOrderIdComInfoExt(Code, OrderId, Commodityid);
        }

    }
}
