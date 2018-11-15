using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Common.Search;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.SV;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.AMP.ZPH.Deploy.MobileCDTO;
using Jinher.AMP.ZPH.ISV.Facade;
using Jinher.JAP.BF.SV.Base;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using QueryActCommodityParam = Jinher.AMP.ZPH.Deploy.CustomDTO.QueryActCommodityParam;
using Jinher.JAP.Cache;
using CommodityListCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO;
using CommoditySearchDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO;
using CommodityStockDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CommodityStockDTO;
using Jinher.AMP.BTP.TPS;
using OrderType = Jinher.AMP.ZPH.Deploy.Enum.OrderType;
using System.Net;
using System.IO;
using System.Text;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Newtonsoft.Json.Linq;
using Jinher.AMP.BTP.TPS.Cache;
using System.Web.Caching;
using System.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using System.Text.RegularExpressions;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;
using System.Threading.Tasks;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 商品接口类
    /// </summary>
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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetOrByCommodityExt
            (System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize, int fieldSort, int order, string areaCode)
        {
            try
            {
                pageSize = pageSize == 0 ? 10 : pageSize;

                var ocommodityList = (from c in Commodity.ObjectSet()
                                      join data1 in CommodityCategory.ObjectSet() on c.Id equals data1.CommodityId
                                      where data1.CategoryId == categoryId && c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select c);

                if (categoryId == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    ocommodityList = from data1 in Commodity.ObjectSet()
                                     join data in CommodityCategory.ObjectSet() on data1.Id equals data.CommodityId
                                         into data2
                                     from ur in data2.DefaultIfEmpty()
                                     where ur.CategoryId == null && data1.IsDel == false && data1.State == 0 && data1.AppId == appId && data1.CommodityType == 0
                                     select data1;
                }

                if (categoryId == new Guid("11111111-1111-1111-1111-111111111111"))
                {
                    ocommodityList = (from c in Commodity.ObjectSet()
                                      where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select c);
                }

                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(areaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new List<CommodityListCDTO>();
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        ocommodityList = (from c in ocommodityList
                                          where c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode)
                                          select c);
                    }
                    else
                    {
                        ocommodityList = (from c in ocommodityList
                                          where c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode)
                                          select c);
                    }
                }

                DateTime now = DateTime.Now;
                //价格排序 需要按照 折扣价格排序
                if (fieldSort == 1)
                {
                    var todaycom = (from c in TodayPromotion.ObjectSet()
                                    where (c.StartTime <= DateTime.Now || c.PresellStartTime <= DateTime.Now) && c.EndTime >= DateTime.Now && c.PromotionType != 3
                                    select c);
                    if (order == 1)
                    {

                        ocommodityList = (from c in ocommodityList
                                          join data in todaycom
                                          on c.Id equals data.CommodityId
                                          into tempT
                                          from tb3 in tempT.DefaultIfEmpty()
                                          where c.AppId == appId && c.IsDel == false && c.State == 0
                                          orderby (tb3.Intensity != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                          select c);
                    }
                    else
                    {
                        ocommodityList = (from c in ocommodityList
                                          join data in todaycom
                                          on c.Id equals data.CommodityId
                                          into tempT
                                          from tb3 in tempT.DefaultIfEmpty()
                                          where c.AppId == appId && c.IsDel == false && c.State == 0
                                          orderby (tb3.Intensity != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                          select c);
                    }
                }
                #region 判断排序条件
                if (fieldSort == 0)
                {
                    ocommodityList = ocommodityList.OrderByDescending(n => n.Salesvolume);
                }

                if (fieldSort == 2)
                {
                    ocommodityList = ocommodityList.OrderByDescending(n => n.SubTime);
                }
                #endregion
                var commodityList = (from c in ocommodityList
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();


                List<Guid> commodityIds = commodityList.Select(c => c.Id).ToList();

                //读今日折扣表
                try
                {
                    var promotionDic = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        bool isdi = false;
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);

                        foreach (var com in promotionDic)
                        {
                            if (com.CommodityId == commodity.Id)
                            {
                                commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                                commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                                commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;
                                commodity.PromotionType = com.PromotionType;
                                if (com.DiscountPrice > -1)
                                {
                                    commodity.DiscountPrice = Convert.ToDecimal(com.DiscountPrice);
                                    commodity.Intensity = 10;
                                    isdi = true;
                                    break;
                                }
                                else
                                {
                                    commodity.DiscountPrice = -1;
                                    commodity.Intensity = com.Intensity;
                                    isdi = true;
                                    break;
                                }
                            }
                        }
                        if (!isdi)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                    }
                }
                catch (Exception e)
                {

                    Jinher.JAP.Common.Loging.LogHelper.Error("商品列表查询错误", e);
                }
                return commodityList;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误。categoryId{0}，appId{1}，pageIndex{2}，pageSize{3}，fieldSort{4}，order{5}", categoryId, appId, pageIndex, pageSize, fieldSort, order), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByWhereExt(Jinher.AMP.ZPH.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, int pageIndex, int pageSize, string areaCode)
        {
            try
            {
                //所选地区非法，直接返回空
                if (!string.IsNullOrEmpty(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    if (string.IsNullOrEmpty(areaName))
                    {
                        return new List<CommodityListCDTO>();
                    }
                }
                pageSize = pageSize == 0 ? 10 : pageSize;

                var commodityQuery = Commodity.ObjectSet().Where(c => c.AppId == commoditySearch.appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0);
                #region 增加商品查询条件---分类、毛利率区间，价格区间

                CommodityListInputDTO intDot = new CommodityListInputDTO()
                {
                    PageIndex = commoditySearch.pageIndex,
                    PageSize = commoditySearch.pageSize,
                    EndTime = commoditySearch.endTime,
                    CommodityName = commoditySearch.commodityName,

                    MaxInterestRate = string.IsNullOrWhiteSpace(commoditySearch.maxInterestRate) ? "" : commoditySearch.maxInterestRate,
                    MinInterestRate = string.IsNullOrWhiteSpace(commoditySearch.minInterestRate) ? "" : commoditySearch.minInterestRate,
                    Categorys = string.IsNullOrWhiteSpace(commoditySearch.categorys) ? "" : commoditySearch.categorys,
                    MaxPrice = string.IsNullOrWhiteSpace(commoditySearch.maxPrice) ? "" : commoditySearch.maxPrice,
                    MinPrice = string.IsNullOrWhiteSpace(commoditySearch.minPrice) ? "" : commoditySearch.minPrice
                };

                commodityQuery = AddCommoditySelectWhere(intDot, commodityQuery);


                #endregion


                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(areaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new List<CommodityListCDTO>();
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        string provinceCode = province.AreaCode ?? "";
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode));
                    }
                    else
                    {
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                    }
                }

                var commodityList = (from c in commodityQuery
                                     orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                DateTime now = DateTime.Now;

                //读今日折扣表
                var comIds = commodityList.Select(c => c.Id).Distinct().ToList();
                var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
                foreach (var commodity in commodityList)
                {
                    commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                    var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                    if (todayPromotion != null)
                    {
                        commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                        commodity.PromotionType = todayPromotion.PromotionType;
                        if (todayPromotion.DiscountPrice > -1)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                            commodity.Intensity = 10;
                            continue;

                        }

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = todayPromotion.Intensity;
                    }
                    else
                    {

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = 10;
                        commodity.LimitBuyEach = -1;
                        commodity.LimitBuyTotal = -1;
                        commodity.SurplusLimitBuyTotal = -1;
                        commodity.PromotionType = 9999;
                    }
                }

                return commodityList;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误GetCommodityByWhereExt，appId{0}，pageIndex{1}，pageSize{2}", commoditySearch.appId, pageIndex, pageSize), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
        }



        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityExt
            (System.Guid appId, int pageIndex, int pageSize, string areaCode)
        {
            try
            {
                //所选地区非法，直接返回空
                if (!string.IsNullOrEmpty(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    if (string.IsNullOrEmpty(areaName))
                    {
                        return new List<CommodityListCDTO>();
                    }
                }
                pageSize = pageSize == 0 ? 10 : pageSize;

                var commodityQuery = Commodity.ObjectSet().Where(c => c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0);

                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(areaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new List<CommodityListCDTO>();
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        string provinceCode = province.AreaCode ?? "";
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode));
                    }
                    else
                    {
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                    }
                }

                var commodityList = (from c in commodityQuery
                                     orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                DateTime now = DateTime.Now;

                //读今日折扣表
                var comIds = commodityList.Select(c => c.Id).Distinct().ToList();
                var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
                foreach (var commodity in commodityList)
                {
                    commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                    var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                    if (todayPromotion != null)
                    {
                        commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                        commodity.PromotionType = todayPromotion.PromotionType;
                        if (todayPromotion.DiscountPrice > -1)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                            commodity.Intensity = 10;
                            continue;

                        }

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = todayPromotion.Intensity;
                    }
                    else
                    {

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = 10;
                        commodity.LimitBuyEach = -1;
                        commodity.LimitBuyTotal = -1;
                        commodity.SurplusLimitBuyTotal = -1;
                        commodity.PromotionType = 9999;
                    }
                }

                return commodityList;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误，appId{0}，pageIndex{1}，pageSize{2}", appId, pageIndex, pageSize), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodity2Ext
            (System.Guid appId, int pageIndex, int pageSize, string areaCode, int isChkTime, DateTime beginTime, DateTime endTime)
        {
            try
            {
                //所选地区非法，直接返回空
                if (!string.IsNullOrEmpty(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    if (string.IsNullOrEmpty(areaName))
                    {
                        return new List<CommodityListCDTO>();
                    }
                }
                pageSize = pageSize == 0 ? 10 : pageSize;

                var commodityQuery = (from scc in CommodityCategory.ObjectSet()
                                      join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                                      join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                                      where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      orderby scc.MaxSort
                                      select c).Distinct().ToList();

                var commodityQuery1 = (from scc in CommodityCategory.ObjectSet()
                                       join sc in Category.ObjectSet() on scc.CategoryId equals Guid.Empty
                                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                                       where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                       orderby scc.MaxSort
                                       select c).Distinct().ToList();

                foreach (var commodity in commodityQuery1)
                {
                    if (commodityQuery.All(t => t.Id != commodity.Id))
                    {
                        commodityQuery.Add(commodity);
                    }
                }
                commodityQuery.OrderBy(t => t.SubTime);
                //var commodityQuery = Commodity.ObjectSet().Where(c => c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0);

                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(areaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new List<CommodityListCDTO>();
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        string provinceCode = province.AreaCode ?? "";
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode)).ToList();
                    }
                    else
                    {
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode)).ToList();
                    }
                }
                //选中促销中商品 按照促销时间进行过滤
                if (isChkTime == 0)
                {
                    var commodityIds = from item in PromotionItems.ObjectSet()
                                       join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                                       where
                                           !pro.IsDel && pro.PromotionType == 0 && pro.AppId == appId &&
                                           pro.EndTime >= beginTime &&
                                           (pro.StartTime <= endTime || pro.PresellStartTime <= endTime) && pro.PromotionType != 3
                                       select item.CommodityId;
                    commodityQuery = commodityQuery.Where(p => commodityIds.Contains(p.Id)).ToList();
                }

                var commodityList = (from c in commodityQuery
                                     orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                DateTime now = DateTime.Now;

                //读今日折扣表

                var comIds = commodityList.Select(c => c.Id).Distinct().ToList();
                var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
                foreach (var commodity in commodityList)
                {
                    commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                    var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id);

                    if (todayPromotion != null)
                    {
                        commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                        commodity.PromotionType = todayPromotion.PromotionType;
                        if (todayPromotion.DiscountPrice > -1)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                            commodity.Intensity = 10;
                            continue;

                        }

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = todayPromotion.Intensity;
                    }
                    else
                    {

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = 10;
                        commodity.LimitBuyEach = -1;
                        commodity.LimitBuyTotal = -1;
                        commodity.SurplusLimitBuyTotal = -1;
                        commodity.PromotionType = 9999;
                    }
                }

                return commodityList;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误，appId{0}，pageIndex{1}，pageSize{2}", appId, pageIndex, pageSize), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
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
        public ListResult<CommodityListCDTO> GetCommodity3Ext(CommodityListInputDTO input)
        {
            var pageSize = input.PageSize == 0 ? 10 : input.PageSize;

            try
            {
                LogHelper.Debug("CommoditySV.GetCommodity3------------------------Begin");
                //所选地区非法，直接返回空
                if (!string.IsNullOrEmpty(input.AreaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(input.AreaCode);
                    if (string.IsNullOrEmpty(areaName))
                    {
                        return new ListResult<CommodityListCDTO>() { Count = 0, List = new List<CommodityListCDTO>() };
                    }
                }

                LogHelper.Debug("CommoditySV.GetCommodity3------------------------Query");
                var categoryQuery = Category.ObjectSet();
                var commodityQuery = (from scc in CommodityCategory.ObjectSet()
                                          //join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                                      join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                                      //from sc in categoryQuery
                                      where /*(scc.CategoryId == sc.Id || scc.CategoryId == Guid.Empty) &&*/ c.AppId == input.AppId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      orderby scc.MaxSort
                                      select c).Distinct();


                #region 增加商品查询条件---分类、毛利率区间，价格区间
                commodityQuery = AddCommoditySelectWhere(input, commodityQuery);

                #endregion

                //var commodityQuery = (from scc in CommodityCategory.ObjectSet()
                //                      join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                //                      join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                //                      where c.AppId == input.AppId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                //                      orderby scc.MaxSort
                //                      select c).Distinct().ToList();

                //var commodityQuery1 = (from scc in CommodityCategory.ObjectSet()
                //                       join sc in Category.ObjectSet() on scc.CategoryId equals Guid.Empty
                //                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                //                       where c.AppId == input.AppId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                //                       orderby scc.MaxSort
                //                       select c).Distinct().ToList();

                //foreach (var commodity in commodityQuery1)
                //{
                //    if (commodityQuery.All(t => t.Id != commodity.Id))
                //    {
                //        commodityQuery.Add(commodity);
                //    }
                //}
                //commodityQuery.OrderBy(t => t.SubTime);

                //var commodityQuery = Commodity.ObjectSet().Where(c => c.AppId == input.AppId && c.IsDel == false && c.State == 0 && c.CommodityType == 0);

                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(input.AreaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(input.AreaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(input.AreaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(input.AreaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new ListResult<CommodityListCDTO>() { Count = 0, List = new List<CommodityListCDTO>() };
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        string provinceCode = province.AreaCode ?? "";
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode));
                    }
                    else
                    {
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                    }
                }
                //选中促销中商品 按照促销时间进行过滤
                if (input.IsChkTime == 0)
                {
                    var commodityIds = from item in PromotionItems.ObjectSet()
                                       join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                                       where
                                           !pro.IsDel && pro.PromotionType == 0 && pro.AppId == input.AppId &&
                                           pro.EndTime >= input.BeginTime &&
                                           (pro.StartTime <= input.EndTime || pro.PresellStartTime <= input.EndTime) && pro.PromotionType != 3
                                       select item.CommodityId;
                    commodityQuery = commodityQuery.Where(p => commodityIds.Contains(p.Id));
                }

                if (!string.IsNullOrWhiteSpace(input.CommodityName))
                {
                    commodityQuery = commodityQuery.Where(_ => _.Name.Contains(input.CommodityName));
                }

                LogHelper.Debug("CommoditySV.GetCommodity3------------------------QueryCount");
                var count = commodityQuery.Count();

                LogHelper.Debug("CommoditySV.GetCommodity3------------------------ToList");
                var commodityList = (from c in commodityQuery
                                     orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).Skip((input.PageIndex - 1) * pageSize).Take(pageSize).ToList();

                DateTime now = DateTime.Now;

                //读今日折扣表
                var comIds = commodityList.Select(c => c.Id).Distinct().ToList();
                var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
                foreach (var commodity in commodityList)
                {
                    commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                    var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id);

                    if (todayPromotion != null)
                    {
                        commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                        commodity.PromotionType = todayPromotion.PromotionType;
                        if (todayPromotion.DiscountPrice > -1)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                            commodity.Intensity = 10;
                            continue;

                        }

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = todayPromotion.Intensity;
                    }
                    else
                    {

                        commodity.DiscountPrice = -1;
                        commodity.Intensity = 10;
                        commodity.LimitBuyEach = -1;
                        commodity.LimitBuyTotal = -1;
                        commodity.SurplusLimitBuyTotal = -1;
                        commodity.PromotionType = 9999;
                    }
                }
                LogHelper.Debug("CommoditySV.GetCommodity3------------------------End");
                return new ListResult<CommodityListCDTO>() { Count = count, List = commodityList };
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误，input.AppId{0}，input.PageIndex{1}，pageSize{2}", input.AppId, input.PageIndex, pageSize), ex);
            }
            return new ListResult<CommodityListCDTO>() { Count = 0, List = new List<CommodityListCDTO>() };
        }

        /// <summary>
        /// 根据搜索条件获取商品
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="want">搜索关键字</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetWantCommodityExt
            (string want, System.Guid appId, int pageIndex, int pageSize)
        {
            try
            {
                pageSize = pageSize == 0 ? 10 : pageSize;

                IQueryable<Commodity> query = from data in Commodity.ObjectSet()
                                              where data.IsDel == false && data.State == 0 && data.CommodityType == 0
                                              select data;
                if (!string.IsNullOrEmpty(want))
                {
                    query = query.Where(data => data.Name.Contains(want) || want.Contains(data.Name));
                }
                if (appId != Guid.Empty)
                {
                    query = query.Where(data => data.AppId == appId);
                }

                var commodityList = query.OrderBy(n => n.SortValue).ThenByDescending(n => n.SubTime).ThenBy(n => n.State).ThenByDescending(n => n.Salesvolume)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(data => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = data.Id,
                        Pic = data.PicturesPath,
                        Price = data.Price,
                        State = data.State,
                        Stock = data.Stock,
                        Name = data.Name,
                        MarketPrice = data.MarketPrice,
                        IsEnableSelfTake = data.IsEnableSelfTake,
                        ComAttribute = data.ComAttribute,
                        ComAttrType = (data.ComAttribute == "[]" || data.ComAttribute == null) ? 1 : 3

                    }).ToList();

                DateTime now = DateTime.Now;

                List<Guid> commodityIds = commodityList.Select(c => c.Id).ToList();

                //读今日折扣表
                try
                {
                    var promotionDic = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        bool isdi = false;
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        foreach (var com in promotionDic)
                        {
                            if (com.PromotionType != 3)
                            {
                                if (com.CommodityId == commodity.Id)
                                {
                                    commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                                    commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                                    commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;
                                    commodity.PromotionType = com.PromotionType;
                                    if (com.DiscountPrice > -1)
                                    {
                                        commodity.DiscountPrice = Convert.ToDecimal(com.DiscountPrice);
                                        commodity.Intensity = 10;
                                        isdi = true;
                                        break;
                                    }
                                    else
                                    {
                                        commodity.DiscountPrice = -1;
                                        commodity.Intensity = com.Intensity;
                                        isdi = true;
                                        break;
                                    }
                                }
                            }

                        }
                        if (!isdi)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                        //if (promotionDic.ContainsKey(commodity.Id))
                        //{
                        //    commodity.Intensity = promotionDic[commodity.Id];
                        //}
                        //else
                        //{
                        //    commodity.Intensity = 10;
                        //}
                    }

                    //商品分类。
                    var ccQuery = (from cc in CommodityCategory.ObjectSet()
                                   where cc.AppId == appId && commodityIds.Contains(cc.CommodityId)
                                   select new { cc.CommodityId, cc.CategoryId }).ToList();
                    if (ccQuery.Any())
                    {
                        foreach (var commodity in commodityList)
                        {
                            var ccf = ccQuery.FirstOrDefault(cc => cc.CommodityId == commodity.Id);
                            if (ccf != null)
                            {
                                commodity.CategoryId = ccf.CategoryId;
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                    Jinher.JAP.Common.Loging.LogHelper.Error("商品列表查询错误", e);
                }



                return commodityList;

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。want{0}，appId{1}，pageIndex{2}，pageSize{3}", want, appId, pageIndex, pageSize), ex);

                return null;
            }
        }

        /// <summary>
        /// 根据分类获取商品
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByCategoryExt
            (System.Guid categoryId, System.Guid appId, int pageIndex, int pageSize)
        {
            try
            {
                pageSize = pageSize == 0 ? 10 : pageSize;
                var query = from data in CommodityCategory.ObjectSet()
                            join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                            where data.CategoryId == categoryId && data1.IsDel == false && data1.State == 0 && data1.CommodityType == 0
                            select data1;
                if (categoryId == Guid.Empty)
                {
                    query = from data1 in Commodity.ObjectSet()
                            join data in CommodityCategory.ObjectSet() on data1.Id equals data.CommodityId
                                into data2
                            from ur in data2.DefaultIfEmpty()
                            where ur.CategoryId == null && data1.IsDel == false && data1.State == 0 && data1.AppId == appId && data1.CommodityType == 0
                            select data1;
                }

                var commodityList = query.OrderBy(n => n.SortValue).ThenByDescending(n => n.SubTime).ThenBy(n => n.State).ThenByDescending(n => n.Salesvolume)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(data => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = data.Id,
                        Pic = data.PicturesPath,
                        Price = data.Price,
                        State = data.State,
                        Stock = data.Stock,
                        Name = data.Name,
                        MarketPrice = data.MarketPrice,
                        IsEnableSelfTake = data.IsEnableSelfTake,
                        ComAttribute = data.ComAttribute,
                        ComAttrType = (data.ComAttribute == "[]" || data.ComAttribute == null) ? 1 : 3
                    }).ToList();
                DateTime now = DateTime.Now;

                List<Guid> commodityIds = commodityList.Select(c => c.Id).ToList();

                //读今日折扣表
                try
                {
                    var promotionDic = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        bool isdi = false;
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        foreach (var com in promotionDic)
                        {
                            if (com.PromotionType != 3)
                            {
                                if (com.CommodityId == commodity.Id)
                                {
                                    commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                                    commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                                    commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;
                                    commodity.PromotionType = com.PromotionType;
                                    if (com.DiscountPrice > -1)
                                    {
                                        commodity.DiscountPrice = Convert.ToDecimal(com.DiscountPrice);
                                        commodity.Intensity = 10;
                                        isdi = true;
                                        break;
                                    }
                                    else
                                    {
                                        commodity.DiscountPrice = -1;
                                        commodity.Intensity = com.Intensity;
                                        isdi = true;
                                        break;
                                    }
                                }
                            }

                        }
                        if (!isdi)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                    }
                }
                catch (Exception e)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Error("商品列表查询错误", e);
                }


                return commodityList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据分类获取商品异常。categoryId{0}，appId{1}，pageIndex{2}，pageSize{3}", categoryId, appId, pageIndex, pageSize), ex);

                return null;
            }
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetailsExt(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo)
        {
            try
            {
                CommoditySDTO commoditySDTO = new CommoditySDTO();
                CommodityDTO com = new CommodityDTO();
                ReviewSV re = new ReviewSV();
                ComPromotionStatusEnum pPromotion = ComPromotionStatusEnum.NoPromotion;
                if (freightTo == "xxx")
                {
                    var pp = from pro in Promotion.ObjectSet()
                             join item in PromotionItems.ObjectSet() on pro.Id equals item.PromotionId
                             where pro.StartTime <= DateTime.Now && pro.EndTime >= DateTime.Now &&
                             item.CommodityId == commodityId
                             orderby pro.EndTime descending
                             select pro;
                    if (pp.Any())
                        pPromotion = (ComPromotionStatusEnum)pp.FirstOrDefault().PromotionType;

                    commoditySDTO.PromotionTypeNew = pPromotion;

                    return commoditySDTO;

                }
                //从缓存中取数据
                com = Commodity.GetDTOFromCache(appId, commodityId);

                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }

                //商品基本信息
                if (com != null)
                {
                    commoditySDTO.Id = com.Id;
                    commoditySDTO.Name = com.Name;
                    commoditySDTO.Pic = com.PicturesPath;
                    commoditySDTO.Price = com.Price;
                    commoditySDTO.MarketPrice = com.MarketPrice;
                    commoditySDTO.ReviewNum = com.TotalReview;
                    commoditySDTO.Stock = com.Stock;
                    commoditySDTO.Total = com.Salesvolume;
                    commoditySDTO.CollectNum = com.TotalCollection;
                    commoditySDTO.State = com.State;
                    commoditySDTO.AppId = com.AppId;
                    commoditySDTO.IsEnableSelfTake = com.IsEnableSelfTake;
                    commoditySDTO.CommodityType = com.CommodityType;

                    if (com.IsDel)
                    {
                        commoditySDTO.State = 3;
                    }
                    commoditySDTO.Description = com.Description;
                    if (!string.IsNullOrEmpty(com.ComAttribute))
                    {
                        commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
                    }
                    string firstName = "";
                    string twoName = "";
                    var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                    if (queryStock != null)
                    {

                        List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                        foreach (var item in queryStock)
                        {
                            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                            tempStock.Price = item.Price;
                            tempStock.Stock = item.Stock;
                            tempStock.Id = item.Id;
                            tempStock.MarketPrice = item.MarketPrice;
                            tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                            commodityStocks.Add(tempStock);
                            firstName = tempStock.ComAttribute[0].Attribute;
                            twoName = tempStock.ComAttribute[1].Attribute;
                        }
                        commoditySDTO.CommodityStocks = commodityStocks;
                    }

                    //两个属性的时候
                    if (commoditySDTO.CommodityStocks != null && commoditySDTO.CommodityStocks.Count > 0)
                    {
                        if (commoditySDTO.ComAttibutes != null && commoditySDTO.ComAttibutes.Count > 0)
                        {
                            foreach (var attritem in commoditySDTO.ComAttibutes)
                            {
                                if (attritem.Attribute.ToLower() == firstName.ToLower())
                                {
                                    attritem.Attribute = "尺寸";
                                }
                                else if (attritem.Attribute.ToLower() == twoName.ToLower())
                                {
                                    attritem.Attribute = "颜色";
                                }
                            }
                            commoditySDTO.ComAttibutes = commoditySDTO.ComAttibutes.OrderByDescending(r => r.Attribute).ToList();
                        }
                    } //一个属性的时候
                    else if (commoditySDTO.ComAttibutes != null && commoditySDTO.ComAttibutes.Count > 0)
                    {
                        foreach (var attritem in commoditySDTO.ComAttibutes)
                        {
                            if (!string.IsNullOrEmpty(attritem.Attribute) && attritem.Attribute != "尺寸" && attritem.Attribute != "颜色")
                            {
                                attritem.Attribute = "尺寸";
                            }
                        }
                    }
                    Stopwatch timer1 = new Stopwatch();
                    timer1.Start();
                    //运费                   
                    //将目的地由汉字转成编码
                    freightTo = ProvinceCityHelper.GetProvinceCodeByName(freightTo);
                    //TODO yjz 运费缓存暂未实现
                    if (com.FreightTemplateId.HasValue && com.FreightTemplateId != Guid.Empty)
                    {
                        TemplateCountDTO templateCountList = new TemplateCountDTO { Count = 1, CommodityId = com.Id };
                        commoditySDTO.Freight = CalOneFreight(freightTo, templateCountList, com.FreightTemplateId.Value);
                        commoditySDTO.FreightTo = freightTo;
                        commoditySDTO.IsSetMulti = IsSetMulti(com.FreightTemplateId);

                        timer1.Stop();
                        LogHelper.Info("商品详情运费DB:" + timer1.ElapsedMilliseconds);
                        timer1.Reset();
                    }
                }

                //图片
                List<CommodityPictureCDTO> productDetailPicList = new List<CommodityPictureCDTO>();
                var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodityId).OrderBy(n => n.Sort);
                var pictures = from p in productDetailsPictures
                               select new CommodityPictureCDTO
                               {
                                   Sort = p.Sort,
                                   PicturesPath = p.PicturesPath,
                               };
                productDetailPicList = pictures.ToList();
                commoditySDTO.Pictures = productDetailPicList;


                DateTime now = DateTime.Now;

                var promotion = TodayPromotion.GetCurrentPromotionWithPresell(commodityId);
                if (promotion == null)
                {
                    commoditySDTO.Intensity = 10;
                    commoditySDTO.DiscountPrice = -1;
                }
                else
                {
                    if (promotion.DiscountPrice > -1)
                    {
                        commoditySDTO.Intensity = 10;
                        commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                    }
                    else
                    {
                        commoditySDTO.Intensity = promotion.Intensity;
                        commoditySDTO.DiscountPrice = -1;
                    }
                    commoditySDTO.LimitBuyEach = promotion.LimitBuyEach;
                    commoditySDTO.LimitBuyTotal = promotion.LimitBuyTotal;
                    commoditySDTO.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;
                }

                int count = 0;

                //是否收藏过
                count = Collection.ObjectSet().Where(n => n.CommodityId == commodityId && n.UserId == userId).Count();
                commoditySDTO.IsCollect = count > 0 ? true : false;
                commoditySDTO.CurrentTime = now;
                return commoditySDTO;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品详情异常。commodityId{0}，appId{1}，userId{2}，freightTo{3}", commodityId, appId, userId, freightTo), ex);
                return null;
            }
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetCommodityDetailsNewExt(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo)
        {
            try
            {
                CommoditySDTO commoditySDTO = new CommoditySDTO();
                CommodityDTO com = new CommodityDTO();
                ReviewSV re = new ReviewSV();
                //从缓存中取数据
                com = Commodity.GetDTOFromCache(appId, commodityId);

                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }
                //商品基本信息
                if (com == null)
                    return commoditySDTO;
                commoditySDTO.Id = com.Id;
                commoditySDTO.Name = com.Name;
                commoditySDTO.Pic = com.PicturesPath;
                commoditySDTO.Price = com.Price;
                commoditySDTO.MarketPrice = com.MarketPrice;
                commoditySDTO.ReviewNum = com.TotalReview;
                commoditySDTO.Stock = com.Stock;
                commoditySDTO.Total = com.Salesvolume;
                commoditySDTO.CollectNum = com.TotalCollection;
                commoditySDTO.State = com.State;
                commoditySDTO.AppId = com.AppId;
                commoditySDTO.IsEnableSelfTake = com.IsEnableSelfTake;
                commoditySDTO.CommodityType = com.CommodityType;
                if (com.IsDel)
                {
                    commoditySDTO.State = 3;
                }
                commoditySDTO.Description = com.Description;
                if (!string.IsNullOrEmpty(com.ComAttribute))
                {
                    commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
                }
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                if (queryStock != null)
                {

                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                    foreach (var item in queryStock)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        tempStock.Price = item.Price;
                        tempStock.MarketPrice = item.MarketPrice;
                        tempStock.Stock = item.Stock;
                        tempStock.Id = item.Id;
                        tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityStocks.Add(tempStock);
                    }
                    commoditySDTO.CommodityStocks = commodityStocks;
                }

                //相关商品
                commoditySDTO.RelationCommoditys = getRelationCommodity(commodityId);
                //图片
                List<CommodityPictureCDTO> productDetailPicList = new List<CommodityPictureCDTO>();
                var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodityId).OrderBy(n => n.Sort);
                var pictures = from p in productDetailsPictures
                               select new CommodityPictureCDTO
                               {
                                   Sort = p.Sort,
                                   PicturesPath = p.PicturesPath,
                               };
                productDetailPicList = pictures.ToList();
                commoditySDTO.Pictures = productDetailPicList;


                DateTime now = DateTime.Now;
                var promotion = TodayPromotion.GetCurrentPromotionWithPresell(commodityId);

                if (promotion == null)
                {
                    commoditySDTO.Intensity = 10;
                    commoditySDTO.DiscountPrice = -1;
                }
                else
                {
                    if (promotion.DiscountPrice > -1)
                    {
                        commoditySDTO.Intensity = 10;
                        commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                    }
                    else
                    {
                        commoditySDTO.Intensity = promotion.Intensity;
                        commoditySDTO.DiscountPrice = -1;
                    }
                    commoditySDTO.LimitBuyEach = promotion.LimitBuyEach;
                    commoditySDTO.LimitBuyTotal = promotion.LimitBuyTotal;
                    commoditySDTO.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;
                }


                //运费
                List<TemplateCountDTO> templateCountList = new List<TemplateCountDTO>();
                decimal realPrice = commoditySDTO.DiscountPrice > 0 ? commoditySDTO.DiscountPrice.Value : commoditySDTO.Price * commoditySDTO.Intensity.Value / 10;
                templateCountList.Add(new TemplateCountDTO { Count = 1, CommodityId = com.Id, Price = decimal.Round(realPrice, 2, MidpointRounding.AwayFromZero) });

                var frDTO = CalFreightMultiAppsByTextExt(freightTo, com.IsEnableSelfTake, templateCountList, null, null, null);
                commoditySDTO.Freight = frDTO.ResultCode == 0 ? frDTO.Freight : 0;
                commoditySDTO.FreightTo = freightTo;
                commoditySDTO.IsSetMulti = IsSetMulti(com.FreightTemplateId);

                int count = 0;

                //是否收藏过
                count = Collection.ObjectSet().Where(n => n.CommodityId == commodityId && n.UserId == userId).Count();
                commoditySDTO.IsCollect = count > 0 ? true : false;
                commoditySDTO.CurrentTime = now;
                return commoditySDTO;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品详情异常。commodityId{0}，appId{1}，userId{2}，freightTo{3}", commodityId, appId, userId, freightTo), ex);

                return null;
            }

        }

        public CommoditySearchResultDTO CommoditySearchExt(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            try
            {
                CommoditySearchResultDTO result = new CommoditySearchResultDTO();
                pageSize = pageSize == 0 ? 10 : pageSize;
                pageIndex = pageIndex < 1 ? 1 : pageIndex;


                IQueryable<Commodity> query = Commodity.ObjectSet().Where(data => data.AppId == appId && data.IsDel == false && data.State == 0 && data.CommodityType == 0);
                if (!string.IsNullOrEmpty(commodityCategory))
                {

                    query = from data in query
                            where data.CategoryName.Contains(commodityCategory)
                            select data;
                }
                if (!string.IsNullOrEmpty(commodityName))
                {
                    query = from data in query
                            where (data.Name.Contains(commodityName) || commodityName.Contains(data.Name))
                            select data;
                }

                result.TotalCount = query.Count();

                var commodityList = query.OrderByDescending(n => n.Salesvolume).ThenByDescending(n => n.SubTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(data => new CommodityListDTO
                    {
                        CommodityCategory = data.CategoryName,
                        CommodityId = data.Id,
                        CommodityName = data.Name,
                        CommodityPicture = data.PicturesPath,
                        IsEnableSelfTake = data.IsEnableSelfTake
                    }).ToList();

                result.CommodityList = commodityList;

                return result;

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。appId{0}，commodityCategory{1}，commodityName{2}，pageIndex{3}，pageSize{4}", appId, commodityCategory, commodityName, pageIndex, pageSize), ex);

                return null;
            }
        }

        public CommoditySearchForAppsResultDTO CommoditySearchFromAppsExt(List<Guid> appIds, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {

            CommoditySearchForAppsResultDTO result = new CommoditySearchForAppsResultDTO();
            try
            {
                pageSize = pageSize == 0 ? 10 : pageSize;
                pageIndex = pageIndex < 1 ? 1 : pageIndex;


                IQueryable<Commodity> query = Commodity.ObjectSet().Where(data => appIds.Contains(data.AppId) && data.IsDel == false && data.State == 0 && data.CommodityType == 0);
                if (!string.IsNullOrEmpty(commodityCategory))
                {

                    query = from data in query
                            where data.CategoryName.Contains(commodityCategory)
                            select data;
                }
                if (!string.IsNullOrEmpty(commodityName))
                {
                    query = from data in query
                            where (data.Name.Contains(commodityName) || commodityName.Contains(data.Name))
                            select data;
                }

                result.TotalCount = query.Count();

                var commodityList = query.OrderByDescending(n => n.Salesvolume).ThenByDescending(n => n.SubTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(data => new CommodityListCategoryDTO
                    {
                        Id = data.Id,
                        Pic = data.PicturesPath,
                        Price = data.Price,
                        MarketPrice = data.MarketPrice,
                        CommodityCategory = data.CategoryName,
                        Name = data.Name,
                        AppId = data.AppId,
                        IsEnableSelfTake = data.IsEnableSelfTake,
                        ComAttrType = (data.ComAttribute == "[]" || data.ComAttribute == null) ? 1 : 3

                    }).ToList();

                DateTime now = DateTime.Now;

                result.CommodityList = commodityList;

                List<Guid> commodityIds = commodityList.Select(c => c.Id).ToList();

                //读今日缓存表
                try
                {
                    var promotionDic = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        var todayPromotion = promotionDic.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                        if (todayPromotion != null)
                        {
                            commodity.Intensity = todayPromotion.Intensity;
                            commodity.PromotionType = todayPromotion.PromotionType;
                        }
                        else
                        {
                            commodity.Intensity = 10;
                            commodity.PromotionType = 9999;
                        }
                    }
                }
                catch (Exception e)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Error("根据搜索条件获取商品异常", e);
                }
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。appIds{0}，commodityCategory{1}，commodityName{2}，pageIndex{3}，pageSize{4}", appIds, commodityCategory, commodityName, pageIndex, pageSize), ex);


            }
            return result;
        }

        public GetCommoditySearchResultDTO GetCommoditySearchExt(Guid appId, string commodityName, int pageIndex, int pageSize)
        {
            try
            {
                GetCommoditySearchResultDTO result = new GetCommoditySearchResultDTO();
                pageSize = pageSize == 0 ? 10 : pageSize;
                pageIndex = pageIndex < 1 ? 1 : pageIndex;


                IQueryable<Commodity> query = Commodity.ObjectSet().Where(data => data.AppId == appId && data.IsDel == false && data.State == 0 && data.IsEnableSelfTake == 0 && data.CommodityType == 0);

                if (!string.IsNullOrEmpty(commodityName))
                {
                    query = from data in query
                            where (data.Name.Contains(commodityName) || commodityName.Contains(data.Name))
                            select data;
                }

                result.TotalCount = query.Count();

                var commodityList = query.OrderByDescending(n => n.SubTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(data => new CommoditySearchListDTO
                    {
                        CommodityId = data.Id,
                        CommodityName = data.Name,
                        Price = data.Price,
                        MarketPrice = data.MarketPrice,
                        SubTime = data.SubTime,
                        Stock = data.Stock

                    }).ToList();

                result.CommodityList = commodityList;

                return result;

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。appId{0}，commodityName{1}，pageIndex{2}，pageSize{3}", appId, commodityName, pageIndex, pageSize), ex);

                return null;
            }
        }

        public CommodityInfoListDTO GetCommodityInfoExt(Guid commodityId)
        {
            try
            {
                CommodityInfoListDTO commoditySDTO = new CommodityInfoListDTO();

                //获取商品实体对象
                Commodity com = Commodity.ObjectSet().Where(n => n.Id == commodityId).FirstOrDefault();

                //商品基本信息
                if (com != null)
                {
                    commoditySDTO.CommodityId = com.Id;
                    commoditySDTO.CommodityName = com.Name;
                    commoditySDTO.Pic = com.PicturesPath;
                    commoditySDTO.Price = com.Price;
                    commoditySDTO.MarketPrice = com.MarketPrice;
                    commoditySDTO.IsEnableSelfTake = com.IsEnableSelfTake;
                }

                return commoditySDTO;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品信息异常。commodityId：{0}", commodityId), ex);

                return null;
            }
        }

        public ResultDTO CreateUserPrizeRecordExt(UserPrizeRecordDTO userPrizeRecordDTO)
        {
            if (userPrizeRecordDTO == null)
            {
                return new ResultDTO() { ResultCode = 1, Message = "参数不能为空" };
            }
            ResultDTO returnDTO = new ResultDTO();

            ContextSession session = ContextFactory.CurrentThreadContext;
            int resultCount = (from e in GenUserPrizeRecord.ObjectSet()
                               where e.PromotionId == userPrizeRecordDTO.PromotionId && e.CommodityId == userPrizeRecordDTO.CommodityId && e.UserId == userPrizeRecordDTO.UserId
                               select e.Id).Count();
            if (resultCount > 0)
            {
                return new ResultDTO() { ResultCode = 1, Message = "已经生成用户中奖记录" };
            }
            GenUserPrizeRecord prizeRecord = GenUserPrizeRecord.CreateGenUserPrizeRecord();
            prizeRecord.IsBuyed = false;
            prizeRecord.Price = userPrizeRecordDTO.Price;
            prizeRecord.PromotionId = userPrizeRecordDTO.PromotionId;
            prizeRecord.UserId = userPrizeRecordDTO.UserId;
            prizeRecord.ValTime = userPrizeRecordDTO.ValTime;
            if (prizeRecord.ValTime <= DateTime.MinValue)
            {
                prizeRecord.ValTime = Jinher.JAP.Common.TypeDefine.Constant.DbMinValue;
            }
            prizeRecord.CommodityId = userPrizeRecordDTO.CommodityId;
            prizeRecord.EntityState = System.Data.EntityState.Added;
            session.SaveObject(prizeRecord);
            try
            {
                session.SaveChanges();
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("生成用户中奖记录异常。userPrizeRecordDTO:{0}", JsonHelper.JsonSerializer(userPrizeRecordDTO)), ex);

                return new ResultDTO() { ResultCode = 1, Message = "生成用户中奖记录异常" };
            }

            return new ResultDTO() { ResultCode = 0, Message = "Success" }; ;
        }

        public PrizeRecordDTO GetUserPrizeRecordExt(Guid promotionId, Guid commodityId, Guid userId)
        {

            PrizeRecordDTO resultDTO = (from e in GenUserPrizeRecord.ObjectSet()
                                        where e.PromotionId == promotionId && e.CommodityId == commodityId && e.UserId == userId
                                        select new PrizeRecordDTO()
                                        {
                                            Price = e.Price,
                                            IsBuyed = e.IsBuyed
                                        }).FirstOrDefault();
            if (resultDTO != null)
            {
                if (resultDTO.IsBuyed)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "已经购买过";
                }
                else
                {
                    resultDTO.ResultCode = 0;
                }
            }
            else
            {
                resultDTO = new PrizeRecordDTO();
                resultDTO.ResultCode = 1;
                resultDTO.Message = "没有中奖记录";
            }

            return resultDTO;
        }

        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByIdsExt(List<Guid> commodityIds, bool isDefaultOrder = false)
        {
            CommoditySearchDTO search = new CommoditySearchDTO();
            search.commodityIdList = commodityIds;
            search.IsDefaultOrder = isDefaultOrder;
            return GetCommodityByIdsNewExt(search);
        }
        /// <summary>
        /// 多应用：根据商品Ids获取商品信息列表
        /// </summary>
        /// <param name="search">查询条件，有效参数commodityIdList,IsDefaultOrder,AreaCode</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByIdsNewExt(CommoditySearchDTO search)
        {
            if (search == null || search.commodityIdList == null || !search.commodityIdList.Any())
                return new List<CommodityListCDTO>();
            try
            {
                var now = DateTime.Now;
                var commodityQuery = Commodity.ObjectSet().Where(c => c.IsDel == false && c.State < 2 && search.commodityIdList.Contains(c.Id) && c.CommodityType == 0);
                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(search.AreaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(search.AreaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.AreaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.AreaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new List<CommodityListCDTO>();
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        string provinceCode = province.AreaCode ?? "";
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode));
                    }
                    else
                    {
                        commodityQuery = commodityQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                    }
                }

                var commodityList = (from c in commodityQuery
                                     orderby c.State, c.Salesvolume descending, c.SubTime descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         AppId = c.AppId,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         DiscountPrice = -1,
                                         Intensity = 10,
                                         LimitBuyEach = -1,
                                         LimitBuyTotal = -1,
                                         SurplusLimitBuyTotal = -1,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).ToList();
                if (!commodityList.Any())
                    return new List<CommodityListCDTO>();

                if (search.IsDefaultOrder)
                {
                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> tmplist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
                    for (int i = 0; i < search.commodityIdList.Count; i++)
                    {
                        var tmp = commodityList.FirstOrDefault(c => c.Id == search.commodityIdList[i]);
                        if (tmp != null && tmp.State == 0)
                            tmplist.Add(tmp);
                    }
                    commodityList = tmplist;
                }


                List<Guid> appIds = commodityList.Select(c => c.AppId).Distinct().ToList();
                Dictionary<Guid, string> appList = APPSV.GetAppNameListByIds(appIds);
                if (appList.Any())
                {
                    foreach (var commoditySdto in commodityList)
                    {
                        if (appList.ContainsKey(commoditySdto.AppId))
                        {
                            var app = appList[commoditySdto.AppId];
                            if (!String.IsNullOrEmpty(app))
                                commoditySdto.AppName = app;
                        }
                    }
                }

                #region 众筹
                if (CustomConfig.CrowdfundingFlag)
                {
                    var cfAppIds = Crowdfunding.ObjectSet().Where(c => appIds.Contains(c.AppId) && c.StartTime < now && c.State == 0).Select(m => m.AppId).ToList();
                    if (cfAppIds.Any())
                    {
                        foreach (var commodityListCdto in commodityList)
                        {
                            if (cfAppIds.Any(c => c == commodityListCdto.AppId))
                                commodityListCdto.IsActiveCrowdfunding = true;
                        }
                    }
                }
                #endregion

                var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(search.commodityIdList);


                foreach (var commodity in commodityList)
                {
                    bool isdi = false;

                    var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                    if (todayPromotion != null)
                    {
                        commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                        commodity.PromotionType = todayPromotion.PromotionType;
                        if (todayPromotion.DiscountPrice > -1)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                        }
                        else
                        {
                            commodity.Intensity = todayPromotion.Intensity;
                        }
                    }
                    else
                    {
                        commodity.PromotionType = 9999;
                    }
                }

                return commodityList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.GetCommodityByIdsNewExt异常。search{0}", search), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CommodityIdsList">商品list</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityExt(Guid UserID, List<Guid> CommodityIdsList)
        {
            //商品列表
            var commodityList = (from c in Commodity.ObjectSet()
                                 where CommodityIdsList.Contains(c.Id) && c.CommodityType == 0
                                 select new CheckCommodityDTO
                                 {
                                     Id = c.Id,
                                     Price = c.Price,
                                     State = c.IsDel ? 3 : c.State,
                                     Stock = c.Stock,
                                     Intensity = 10,
                                     DiscountPrice = -1,
                                     OPrice = c.Price,
                                     LimitBuyEach = -1,
                                     LimitBuyTotal = -1,
                                     SurplusLimitBuyTotal = 0,
                                     IsEnableSelfTake = c.IsEnableSelfTake
                                 }).ToList();
            DateTime now = DateTime.Now;
            //商品在每日促销表里集合 
            var comIdList = commodityList.Where(c => c.State == 0).Select(m => m.Id).ToList();
            var promotionDic = TodayPromotion.ObjectSet().Where(a => CommodityIdsList.Contains(a.CommodityId)
                   && a.EndTime > now && a.StartTime < now).
                   Select(a => new
                   {
                       ComId = a.CommodityId,
                       Intensity = a.Intensity,
                       DiscountPrice = a.DiscountPrice,
                       LimitBuyTotal = a.LimitBuyTotal,
                       SurplusLimitBuyTotal = a.SurplusLimitBuyTotal,
                       LimitBuyEach = a.LimitBuyEach,
                       PromotionId = a.PromotionId
                   }).Distinct();

            if (promotionDic != null && promotionDic.Count() > 0)
            {
                foreach (var commodity in commodityList)
                {
                    foreach (var com in promotionDic)
                    {
                        int limie = -1;
                        int limitotal = -1;
                        if (com.ComId == commodity.Id)
                        {

                            commodity.SurplusLimitBuyTotal = Convert.ToInt32(com.SurplusLimitBuyTotal);
                            if (com.LimitBuyEach != null && com.LimitBuyEach != -1)
                            {
                                int sumli = 0;
                                var ul = UserLimited.ObjectSet().Where(n => n.UserId == UserID && n.PromotionId == com.PromotionId && n.CommodityId == commodity.Id).Select(s => s.Count).ToList();
                                if (ul != null && ul.Count > 0)
                                {
                                    limie = Convert.ToInt32(com.LimitBuyEach - ul.Sum());
                                }
                                else
                                {
                                    limie = Convert.ToInt32(com.LimitBuyEach);
                                }
                                commodity.LimitBuyEach = Convert.ToInt32(com.LimitBuyEach);
                            }

                            if (com.LimitBuyTotal != null && com.LimitBuyTotal != -1)
                            {
                                limitotal = Convert.ToInt32(com.LimitBuyTotal) - Convert.ToInt32(com.SurplusLimitBuyTotal);
                                commodity.LimitBuyTotal = Convert.ToInt32(com.LimitBuyTotal);
                            }
                            #region 判断 限购总数 每人限购 库存之间的关系
                            //如果只有每人限购 没有促销商品总量
                            if (limie != -1 && limitotal == -1)
                            {
                                if (commodity.Stock > limie)
                                {
                                    commodity.Stock = limie;
                                }
                            }
                            else
                            {
                                //如果只有促销商品总量 没有每人限购 
                                if (limitotal != -1 && limie == -1)
                                {
                                    if (commodity.Stock > limitotal)
                                    {
                                        commodity.Stock = limitotal;
                                    }
                                }
                                else
                                {
                                    //如果都设置了 每人限购 促销商品总量
                                    if (limitotal != -1 && limie != -1)
                                    {
                                        if (limitotal > limie)
                                        {
                                            commodity.Stock = limie;
                                        }
                                        else
                                        {
                                            commodity.Stock = limitotal;
                                        }
                                    }
                                }
                            }
                            #endregion
                            if (com.DiscountPrice > -1)
                            {
                                commodity.Price = Convert.ToDecimal(com.DiscountPrice);
                                commodity.DiscountPrice = Convert.ToDecimal(com.DiscountPrice);
                                continue;
                            }
                            else
                            {
                                commodity.Price = Math.Round((commodity.Price * com.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                                commodity.Intensity = com.Intensity;
                                continue;
                            }
                        }
                    }
                }
            }

            return commodityList;
        }
        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CommodityIdsList">商品list</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityNewExt(Guid UserID, List<CommodityIdAndStockId> CommodityIdsList)
        {
            CheckCommodityParam ccp = new CheckCommodityParam();
            ccp.CommodityIdsList = CommodityIdsList;
            ccp.DiygId = Guid.Empty;
            ccp.PromotionType = -1;
            ccp.UserID = UserID;
            var ccList = CheckCommodityV3Ext(ccp);
            return ccList;
        }

        public List<SquareHotCommodityDTO> GetHotCommoditisExt()
        {
            var commoditys = HotCommodity.ObjectSet()
                        .OrderByDescending(n => n.Salesvolume)
                        .ThenByDescending(n => n.TotalCollection)
                        .Select(n => new SquareHotCommodityDTO
                        {
                            Name = n.Name,
                            Id = n.Id,
                            Picture = n.PicturesPath,

                        }).Take(2).ToList();

            if (commoditys == null || commoditys.Count == 0)
            {
                commoditys = Commodity.ObjectSet()
                 .Where(n => n.IsDel == false && n.State == 0 && n.Stock > 0 && n.CommodityType == 0)
                 .OrderByDescending(n => n.Salesvolume)
                 .ThenByDescending(n => n.TotalCollection)
                 .Select(n => new SquareHotCommodityDTO
                 {
                     Name = n.Name,
                     Id = n.Id,
                     Picture = n.PicturesPath,
                 })
                 .Take(2).ToList();
            }

            return commoditys;
        }
        /// <summary>
        /// 运费计算
        /// </summary>
        /// <param name="FreightTo"></param>
        /// <param name="TemplateCounts"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO CalFreightExt(string FreightTo, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> TemplateCounts)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO fResult = new Deploy.CustomDTO.FreightResultDTO();
            try
            {
                if (!(TemplateCounts != null && TemplateCounts.Count > 0))
                {
                    fResult.Message = "模板数据集合为空或不存在";
                    fResult.ResultCode = 3;
                    fResult.Freight = 0;
                    return fResult;
                }
                //由于颜色尺寸问题，对相同商品合并分组
                var query = from templateCount in TemplateCounts
                            group templateCount by templateCount.CommodityId into g
                            select new
                            {
                                g.Key,
                                Num = g.Sum(templateCount => templateCount.Count)
                            };
                List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> newTemplateCounts = new List<TemplateCountDTO>();
                foreach (var item in query)
                {
                    newTemplateCounts.Add(new TemplateCountDTO() { CommodityId = item.Key, Count = item.Num });
                }
                decimal fDecimal = 0;


                //将目的地由汉字转成编码
                FreightTo = ProvinceCityHelper.GetProvinceCodeByName(FreightTo);

                foreach (Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO tem in newTemplateCounts)
                {
                    //对多个商品取最贵的运费
                    //声明临时变量
                    decimal dec = 0;
                    Commodity com = Commodity.ObjectSet().Where(n => n.Id == tem.CommodityId).FirstOrDefault();
                    if (com != null && com.FreightTemplateId.HasValue)
                    {
                        dec = CalOneFreight(FreightTo, tem, com.FreightTemplateId.Value);
                    }
                    //如果当前商品运费大于等于fDecimal,则改变fDecimal的值
                    if (dec >= fDecimal)
                    {
                        fDecimal = dec;
                    }
                }
                fResult.Message = "Success";
                fResult.ResultCode = 0;
                fResult.Freight = fDecimal;
                return fResult;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("运费计算错误。FreightTo{0}，TemplateCounts{1}", FreightTo, TemplateCounts), ex);
                fResult.Message = "运费计算错误";
                fResult.ResultCode = 1;
                fResult.Freight = 0;
                return fResult;
            }
        }

        private decimal CalOneFreight(string FreightTo, Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO tem, Guid templateId)
        {
            if (string.IsNullOrWhiteSpace(FreightTo))
            {
                return 0;
            }

            decimal dec = 0;
            FreightTemplate ft = FreightTemplate.ObjectSet().Where(s => s.Id == templateId).FirstOrDefault();
            if (ft != null)
            {
                //不包邮
                if (ft.IsFreeExp == false)
                {
                    List<FreightTemplateDetail> ftdList = FreightTemplateDetail.ObjectSet().Where(s => s.FreightTemplateId == ft.Id).ToList();
                    bool IsHaveFreightTo = false;

                    #region 读取运费详情数据
                    if (ftdList != null && ftdList.Count > 0)
                    {
                        foreach (FreightTemplateDetail detail in ftdList)
                        {
                            if (!string.IsNullOrEmpty(detail.DestinationCodes))
                            {
                                string[] freighttoList = detail.DestinationCodes.Replace("，", ",").Replace(";", ",").Replace("；", ",").Split(',');
                                bool isContain = false;
                                //改为包含关系
                                if (freighttoList != null && freighttoList.Length > 0)
                                {
                                    foreach (string fTo in freighttoList)
                                    {
                                        if (fTo.Contains(FreightTo))
                                        {
                                            isContain = true;
                                            break;
                                        }
                                    }
                                }
                                if (isContain)
                                {
                                    IsHaveFreightTo = true;
                                    if (detail.FirstCount >= tem.Count)
                                    {
                                        dec = dec + detail.FirstCountPrice;
                                    }
                                    else
                                    {
                                        dec = dec + detail.FirstCountPrice;
                                        decimal cou = (tem.Count - detail.FirstCount) / detail.NextCount;
                                        dec = dec + cou * detail.NextCountPrice;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    #region 读取默认数据
                    if (IsHaveFreightTo == false)
                    {
                        if (ft.FirstCount >= tem.Count)
                        {
                            dec = dec + ft.FirstCountPrice;
                        }
                        else
                        {
                            dec = dec + ft.FirstCountPrice;
                            decimal cou = (tem.Count - ft.FirstCount) / ft.NextCount;
                            dec = dec + cou * ft.NextCountPrice;
                        }
                    }
                    #endregion
                }
            }
            return dec;
        }
        /// <summary>
        /// 运费详细列表
        /// </summary>
        /// <param name="CommodityId">商品编号</param>
        /// <returns></returns>
        public FreightDetailDTO GetFreightDetailsExt(Guid CommodityId)
        {
            FreightDetailDTO fdDTO = new FreightDetailDTO();
            try
            {
                Commodity com = Commodity.ObjectSet().Where(s => s.Id == CommodityId).FirstOrDefault();
                if (com != null)
                {
                    FreightTemplate ft = FreightTemplate.ObjectSet().Where(s => s.Id == com.FreightTemplateId).FirstOrDefault();
                    if (ft != null)
                    {
                        List<FreightDetail> fdList = new List<FreightDetail>();
                        //不包邮
                        if (ft.IsFreeExp == false)
                        {
                            List<FreightTemplateDetail> ftdList = FreightTemplateDetail.ObjectSet().Where(s => s.FreightTemplateId == ft.Id).ToList();
                            if (ftdList != null && ftdList.Count > 0)
                            {
                                string[] freightToArray;
                                foreach (FreightTemplateDetail ftd in ftdList)
                                {
                                    if (!string.IsNullOrEmpty(ftd.FreightTo))
                                    {
                                        freightToArray = ftd.FreightTo.Split(',');
                                        if (freightToArray != null && freightToArray.Length > 0)
                                        {
                                            foreach (string freightTo in freightToArray)
                                            {
                                                fdList.Add(new FreightDetail() { FreightTo = freightTo, Freitht = ftd.FirstCountPrice });
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                fdList.Add(new FreightDetail() { FreightTo = ft.FreightTo, Freitht = ft.FirstCountPrice });
                            }
                        }
                        //包邮
                        else
                        {
                            fdList.Add(new FreightDetail() { FreightTo = ft.FreightTo, Freitht = 0 });
                        }
                        fdDTO.Message = "Success";
                        fdDTO.ResultCode = 0;
                        fdDTO.FreightList = fdList;
                    }
                    else
                    {
                        fdDTO.Message = "没有找到运费模板";
                        fdDTO.ResultCode = 3;
                    }
                }
                else
                {
                    fdDTO.Message = "没有找到商品";
                    fdDTO.ResultCode = 2;
                }
                return fdDTO;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("运费详细列表错误。CommodityId：{0}", CommodityId), ex);
                fdDTO.Message = "运费详细列表错误";
                fdDTO.ResultCode = 1;
                return fdDTO;
            }
        }

        /// <summary>
        /// 商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public CommodityZPHResultDTO GetCommoditysZPHExt(CommoditySearchZPHDTO search)
        {
            LogHelper.Info("GetCommoditysZPHExt:search   AppName=  " + search.AppName + ",appId=" + search.AppId + ",CommodityName=" + search.CommodityName + ",CommodityCode=" + search.CommodityCode + ",CommodityId=" + search.CommodityId);
            List<Guid> appIdList = new List<Guid>();
            Dictionary<Guid, string> appNamedict = new Dictionary<Guid, string>();
            CommodityZPHResultDTO result = new CommodityZPHResultDTO();
            if (search == null || search.PageIndex <= 0 || search.PageSize <= 0)
                return result;
            if ((!search.AppId.HasValue || search.AppId == Guid.Empty) && string.IsNullOrEmpty(search.AppName) &&
                (!search.CommodityId.HasValue || search.CommodityId == Guid.Empty) && string.IsNullOrEmpty(search.CommodityName))
                return result;

            try
            {
                //appId不为空，则取当前app下的商品
                if (search.AppId.HasValue && search.AppId != Guid.Empty)
                {
                    appIdList.Add(search.AppId.Value);
                }
                else if (!string.IsNullOrEmpty(search.AppName))
                {
                    Jinher.AMP.BTP.Common.Search.AppSearch appSearch = SearchHelper.GetTemplateSearchResult(search.AppName, search.PageIndex, search.PageSize);
                    if (appSearch != null && appSearch.Paragraph != null && appSearch.Paragraph.Count > 0)
                    {
                        foreach (Jinher.AMP.BTP.Common.Search.ParagraphDetail a in appSearch.Paragraph)
                        {
                            var appId = Guid.Parse(a.Content.id);
                            appIdList.Add(appId);
                            if (!appNamedict.ContainsKey(appId))
                                appNamedict.Add(appId, a.Content.name);
                        }
                    }
                }
                var query = Commodity.ObjectSet().Where(c => c.IsDel == false && c.State == 0 && c.CommodityType == 0);
                if ((search.AppId.HasValue && search.AppId != Guid.Empty) || !string.IsNullOrEmpty(search.AppName))
                {
                    query = query.Where(c => appIdList.Contains(c.AppId));
                }
                if (search.CommodityId.HasValue && search.CommodityId != Guid.Empty)
                {
                    var commodityId = search.CommodityId.Value;
                    query = query.Where(c => c.Id == commodityId);
                }

                if (!string.IsNullOrEmpty(search.CommodityName))
                {
                    query = query.Where(c => c.Name.Contains(search.CommodityName));
                }
                var start = Math.Max(0, search.PageIndex - 1) * search.PageSize;

                result.TotalCount = query.Count();
                if (result.TotalCount > 0)
                {
                    var list = query.OrderBy(c => c.AppId).ThenBy(c => c.Name).Skip(start).Take(search.PageSize).Select(com => new CommoditySDTO
                    {
                        Id = com.Id,
                        Name = com.Name,
                        Pic = com.PicturesPath,
                        Price = com.Price,
                        MarketPrice = com.MarketPrice,
                        ReviewNum = com.TotalReview,
                        Stock = com.Stock,
                        Total = com.Salesvolume,
                        CollectNum = com.TotalCollection,
                        State = com.State,
                        AppId = com.AppId,
                        IsEnableSelfTake = com.IsEnableSelfTake,
                        CommodityType = com.CommodityType,
                        ComAttrType = (com.ComAttribute == "[]" || com.ComAttribute == null) ? 1 : 3
                    }).ToList();
                    if (list.Count > 0)
                    {
                        List<Guid> needSearchs = new List<Guid>();
                        foreach (var commoditySdto in list)
                        {
                            if (appNamedict.ContainsKey(commoditySdto.AppId))
                            {
                                commoditySdto.AppName = appNamedict[commoditySdto.AppId];
                            }
                            else
                            {
                                if (needSearchs.All(c => c != commoditySdto.AppId))
                                {
                                    needSearchs.Add(commoditySdto.AppId);
                                }


                            }

                        }
                        if (needSearchs.Any())
                        {
                            try
                            {
                                Dictionary<Guid, string> appList = APPSV.GetAppNameListByIds(needSearchs);
                                if (appList.Any())
                                {
                                    foreach (var commoditySdto in list)
                                    {
                                        if (appList.ContainsKey(commoditySdto.AppId))
                                        {
                                            var app = appList[commoditySdto.AppId];
                                            if (!String.IsNullOrEmpty(app))
                                                commoditySdto.AppName = app;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("CommoditySV.GetCommoditysZPHExt异常：调用APPSV.GetAppNameListByIds异常,  StackTrace:\r\n " + ex);
                            }
                        }
                        result.CommodityList = list;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.GetCommoditysZPHExt异常。search{0}", JsonHelper.JsonSerializer(search)), ex);
                return new CommodityZPHResultDTO();
            }
            return result;
        }
        /// <summary>
        /// ZPH商品服务项商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public CommodityZPHResultDTO GetCommoditysforZPHExt(CommoditySearchZPHDTO search)
        {
            CommodityZPHResultDTO result = new CommodityZPHResultDTO();
            if (search == null || search.PageIndex <= 0 || search.PageSize <= 0)
                return result;
            if ((!search.AppId.HasValue || search.AppId == Guid.Empty) && (search.ServiceSettingId == null || search.CommodityId == Guid.Empty))
                return result;
            try
            {
                var Query = Commodity.ObjectSet().Where(p => p.AppId == search.AppId && p.IsDel == false).AsQueryable();
                string ServiceSettingId = search.ServiceSettingId.ToString();
                if (search.IsJoinServiceSetting == 0)
                {
                    Query = Query.Where(p => p.ServiceSettingId.Contains(ServiceSettingId));
                }
                else if (search.IsJoinServiceSetting == 1)
                {
                    Query = Query.Where(p => !p.ServiceSettingId.Contains(ServiceSettingId));
                }
                if (!string.IsNullOrEmpty(search.CommodityName))
                {
                    Query = Query.Where(p => p.Name.Contains(search.CommodityName) || search.CommodityName.Contains(p.Name));
                }
                result.TotalCount = Query.Count();
                if (result.TotalCount > 0)
                {
                    var list = (from com in Query
                                select new CommoditySDTO
                                {
                                    Id = com.Id,
                                    Name = com.Name,
                                    Pic = com.PicturesPath,
                                    Price = com.Price,
                                    Stock = com.Stock,
                                    AppId = com.AppId

                                }).OrderBy(p => p.Price).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                    result.CommodityList = list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.GetCommoditysforZPHExt异常。search{0}", JsonHelper.JsonSerializer(search)), ex);
                return new CommodityZPHResultDTO();
            }
            return result;
        }
        /// <summary>
        /// ZPH关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        public ResultDTO JoinComdtyServiceSettingExt(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId)
        {
            ResultDTO result = new ResultDTO() { isSuccess = false, ResultCode = 1, Message = "服务项关联失败" };
            if (!ComIds.Any() || AppId == null && AppId == Guid.Empty || ServiceSettingId == null || ServiceSettingId == Guid.Empty)
            {
                result.Message = "参数有误,稍候重试";
                return result;
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var ComdtyList = Commodity.ObjectSet().Where(p => p.AppId == AppId && ComIds.Contains(p.Id) && p.IsDel == false).ToList();
                foreach (var com in ComdtyList)
                {
                    com.ServiceSettingId += "|" + ServiceSettingId.ToString();
                    com.EntityState = EntityState.Modified;
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    result.isSuccess = true;
                    result.ResultCode = 0;
                    result.Message = "关联成功";
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.JoinComdtyServiceSettingExt异常。", ex));
                return new ResultDTO() { isSuccess = false, ResultCode = 1, Message = ex.ToString() };
            }
        }
        /// <summary>
        ///ZPH取消关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        public ResultDTO CancelComdtyServiceSettingExt(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId)
        {
            ResultDTO result = new ResultDTO() { isSuccess = false, ResultCode = 1, Message = "服务项关联取消失败" };
            if (!ComIds.Any() || AppId == null && AppId == Guid.Empty || ServiceSettingId == null || ServiceSettingId == Guid.Empty)
            {
                result.Message = "参数有误,稍候重试";
                return result;
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var ComdtyList = Commodity.ObjectSet().Where(p => p.AppId == AppId && ComIds.Contains(p.Id) && p.IsDel == false).ToList();
                string ServiceSettingInfo = ServiceSettingId.ToString().ToLower();
                foreach (var com in ComdtyList)
                {
                    List<string> str = com.ServiceSettingId.ToLower().Split('|').ToList();
                    int index = str.IndexOf(ServiceSettingInfo);
                    str.RemoveAt(index);
                    string ServiceSettingInfos = null;
                    foreach (var item in str)
                    {
                        ServiceSettingInfos += (item + "|");
                    }
                    if (str.Count == 0)
                    {
                        com.ServiceSettingId = "";
                    }
                    else
                    {
                        ServiceSettingInfos = ServiceSettingInfos.Remove(ServiceSettingInfos.Length - 1, 1);
                        com.ServiceSettingId = ServiceSettingInfos;
                    }
                    com.EntityState = EntityState.Modified;
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    result.isSuccess = true;
                    result.ResultCode = 0;
                    result.Message = "成功取消关联";
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.JoinComdtyServiceSettingExt异常。", ex));
                return new ResultDTO() { isSuccess = false, ResultCode = 1, Message = ex.ToString() };
            }
        }

        /// <summary>
        /// 根据正品会活动id获取对应分页信息的商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByZPHActIdExt(CommoditySearchZPHDTO search)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result = null;
            try
            {
                if (search == null || search.PageIndex <= 0 || search.PageSize <= 0)
                    return result;
                if (search.ActId == null || search.ActId == Guid.Empty)
                    return result;
                List<Guid> commodityIds = new List<Guid>();
                QueryActCommodityParam param = new QueryActCommodityParam
                {
                    actId = search.ActId.Value,
                    pageIndex = search.PageIndex,
                    pageSize = search.PageSize,
                    orderBy = OrderType.ASC
                };
                try
                {
                    commodityIds = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetCommodityByActId(param);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCommodityByZPHActIdExt异常, 获取正品会商品id列表错误。param：{0}", JsonHelper.JsonSerializer(param)), ex);
                    throw;
                }

                result = GetCommodityByIdsWithPreSellExt(commodityIds, true);

                return result;

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.GetCommodityByZPHActIdExt异常。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                return result;
            }
        }

        /// <summary>
        /// 是否有运费模板明细
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        private bool IsSetMulti(Guid? freightTemplateId)
        {
            if (freightTemplateId == null)
                return false;
            var cnt = FreightTemplateDetail.ObjectSet().Count(n => n.FreightTemplateId == freightTemplateId);
            return cnt > 0;
        }
        /// <summary>
        /// 获取商品关联商品
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        private List<Jinher.AMP.BTP.Deploy.CustomDTO.RelationCommodityDTO> getRelationCommodity(Guid commodityId)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.RelationCommodityDTO> result = null;
            var relalist = RelationCommodity.ObjectSet().Where(p => p.CommodityId == commodityId).ToList();
            if (relalist.Count > 0)
            {
                DateTime now = DateTime.Now;
                var commodityList = (from c in Commodity.ObjectSet()
                                     join rc in RelationCommodity.ObjectSet() on c.Id equals rc.RelationCommodityId
                                     where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                           && rc.CommodityId == commodityId
                                     orderby rc.No_Code
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.RelationCommodityDTO
                                     {
                                         RelationCommodityId = c.Id,
                                         CommodityPicturesPath = c.PicturesPath,
                                         Price = c.Price,
                                         MarketPrice = c.MarketPrice,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         AppId = c.AppId,
                                         IsEnableSelfTake = c.IsEnableSelfTake
                                     }).ToList();

                if (commodityList.Any())
                {
                    var appIds = commodityList.Select(c => c.AppId).Distinct().ToList();
                    #region 众筹
                    if (CustomConfig.CrowdfundingFlag)
                    {
                        var cfAppIds = Crowdfunding.ObjectSet().Where(c => appIds.Contains(c.AppId) && c.StartTime < now && c.State == 0).Select(m => m.AppId).ToList();
                        if (cfAppIds.Any())
                        {
                            foreach (var commodityListCdto in commodityList)
                            {
                                if (cfAppIds.Any(c => c == commodityListCdto.AppId))
                                    commodityListCdto.IsActiveCrowdfunding = true;
                            }
                        }
                    }
                    #endregion


                    var commodityIds = commodityList.Select(c => c.RelationCommodityId).ToList();
                    var promotionDic = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        bool isdi = false;

                        foreach (var com in promotionDic)
                        {
                            if (com.CommodityId == commodity.RelationCommodityId)
                            {
                                commodity.LimitBuyEach = com.LimitBuyEach ?? -1;
                                commodity.LimitBuyTotal = com.LimitBuyTotal ?? -1;
                                commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal ?? 0;
                                if (com.DiscountPrice > -1)
                                {
                                    commodity.DiscountPrice = Convert.ToDecimal(com.DiscountPrice);
                                    commodity.Intensity = 10;
                                    isdi = true;
                                    break;
                                }
                                else
                                {
                                    commodity.DiscountPrice = -1;
                                    commodity.Intensity = com.Intensity;
                                    isdi = true;
                                    break;
                                }
                            }
                        }
                        if (!isdi)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                        }
                    }

                }

                result = commodityList;
            }

            return result;
        }

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public CommodityListResultDTO GetCommodityListExt(CommodityListSearchDTO search)
        {
            CommodityListResultDTO result = new CommodityListResultDTO();

            var newResult = GetCommodityListV2Ext(search);

            if (newResult != null)
            {
                result.CommodityList = newResult.comdtyList;
                if (newResult.appInfoList != null && newResult.appInfoList.Count > 0)
                {
                    result.AppName = newResult.appInfoList[0].appName;
                }
            }
            return result;
        }



        private void buildShowPrice(CommodityListCDTO commodity, List<Deploy.CommodityStockDTO> comStocks,
                                    TodayPromotion todayPromotion)
        {
            if (commodity == null)
                return;
            decimal price = commodity.Price;
            decimal? marketPrice = commodity.MarketPrice;
            decimal discountPrice = -1;
            decimal intensity = 10;

            if (todayPromotion != null)
            {
                discountPrice = todayPromotion.DiscountPrice.Value;
                intensity = todayPromotion.Intensity;
            }
            commodity.RealPrice = Commodity.GetShowRealPrice(price, discountPrice, intensity);
            commodity.OriPrice = Commodity.GetShowOriPrice(price, marketPrice, discountPrice, intensity);

        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递的参数为esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo"></param>
        /// <param name="outPromotionId"></param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHExt(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId = null)
        {
            try
            {
                CommodityDTO com = new CommodityDTO();
                ReviewSV re = new ReviewSV();
                //从缓存中取数据
                LogHelper.Debug(string.Format("进入获取商品详情接口GetCommodityDetailsZPH，商品id：{1}，起始时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
                com = Commodity.GetDTOFromCache(appId, commodityId);
                LogHelper.Debug(string.Format("读取缓存结束GetDTOFromCache，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));

                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                        LogHelper.Debug(string.Format("添加缓存结束AddAppCommondityDTOCache，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
                    }
                }
                if (com == null || com.IsDel)
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

                var resultData = getCommodityDetailsZPHOld(com, appId, userId, freightTo, outPromotionId);
                if (resultData != null)
                {
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品详情异常。commodityId：{0}。appId：{1}。userId：{2}。freightTo：{3}。null：{4}", commodityId, appId, userId, freightTo, "null"), ex);
            }
            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 商品详情 金采团购活动商品使用
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递的参数为esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo"></param>
        /// <param name="jcActivityId"></param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewIIExt(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? jcActivityId = null)
        {
            try
            {
                CommodityDTO com = new CommodityDTO();
                ReviewSV re = new ReviewSV();
                //从缓存中取数据
                LogHelper.Debug(string.Format("进入获取商品详情接口GetCommodityDetailsZPH，商品id：{1}，起始时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
                com = Commodity.GetDTOFromCache(appId, commodityId);
                LogHelper.Debug(string.Format("读取缓存结束GetDTOFromCache，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));

                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                        LogHelper.Debug(string.Format("添加缓存结束AddAppCommondityDTOCache，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
                    }
                }
                if (com == null || com.IsDel)
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

                var resultData = getCommodityDetailsZPHII(com, appId, userId, freightTo, jcActivityId);
                if (resultData != null)
                {
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品详情异常。commodityId：{0}。appId：{1}。userId：{2}。freightTo：{3}。null：{4}", commodityId, appId, userId, freightTo, "null"), ex);
            }
            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递的参数为esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo"></param>
        /// <param name="outPromotionId"></param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewExt(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId = null)
        {
            try
            {
                CommodityDTO com = new CommodityDTO();
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                if (commodity != null)
                {
                    com = commodity.ToEntityData();
                }
                if (com == null || com.IsDel)
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };
                using (StopwatchLogHelper.BeginScope("getCommodityDetailsZPH"))
                {
                    var resultData = getCommodityDetailsZPH(com, appId, userId, freightTo, outPromotionId);
                    if (resultData != null)
                    {
                        ValidCostPrice(new List<Guid>() { com.Id }, null);
                        return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品详情异常。commodityId：{0}。appId：{1}。userId：{2}。freightTo：{3}。null：{4}", commodityId, appId, userId, freightTo, "null"), ex);
            }
            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        }


        //public ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewExt(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId = null)
        //{
        //    try
        //    {
        //        CommodityDTO com = new CommodityDTO();
        //        ReviewSV re = new ReviewSV();
        //        //从缓存中取数据
        //        LogHelper.Debug(string.Format("进入获取商品详情接口GetCommodityDetailsZPH，商品id：{1}，起始时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
        //        var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
        //        if (commodity != null)
        //        {
        //            com = commodity.ToEntityData();
        //        }
        //        if (com == null || com.IsDel)
        //            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

        //        var resultData = getCommodityDetailsZPH(com, appId, userId, freightTo, outPromotionId);
        //        if (resultData != null)
        //        {
        //            return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(string.Format("商品详情异常。commodityId：{0}。appId：{1}。userId：{2}。freightTo：{3}。null：{4}", commodityId, appId, userId, freightTo, "null"), ex);
        //    }
        //    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        //}

        private CommodityDTO GetCommodity(Guid appId, Guid commodityId)
        {
            CommodityDTO com = Commodity.GetDTOFromCache(appId, commodityId);
            if (com == null)
            {
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                if (commodity != null)
                {
                    com = commodity.ToEntityData();
                    Commodity.AddAppCommondityDTOCache(com);
                    LogHelper.Debug(string.Format("添加缓存结束AddAppCommondityDTOCache，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), commodityId));
                }
            }

            if (com == null || com.IsDel) return null;
            return com;
        }

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="actId">正品会活动Id</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityDetailsByActIdExt(System.Guid actId, System.Guid appId, Guid userId, string freightTo, string areaCode)
        {
            try
            {
                QueryActCommodityParam param = new QueryActCommodityParam
                {
                    actId = actId,
                    pageIndex = 1,
                    pageSize = 10,
                    orderBy = OrderType.ASC
                };
                var commodityIds = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetCommodityByActId(param);

                if (commodityIds == null || !commodityIds.Any())
                {
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "活动中没有商品" };
                }
                if (commodityIds.Count > 1)
                {
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "活动中包含多个商品" };
                }
                var comId = commodityIds[0];
                var comQuery = Commodity.ObjectSet().Where(c => c.Id == comId && c.State == 0 && c.IsDel == false && c.CommodityType == 0);

                //所选地区非法，直接返回空
                if (!ProvinceCityHelper.IsTheWholeCountry(areaCode))
                {
                    var areaName = ProvinceCityHelper.GetAreaNameByCode(areaCode);
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(areaCode);
                    if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                    {
                        return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "未找到商品" };
                    }
                    if (province.AreaCode == city.AreaCode)
                    {
                        comQuery = comQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                    }
                    else
                    {
                        comQuery = comQuery.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                    }
                }
                var com = comQuery.FirstOrDefault();
                if (com == null)
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "未找到商品" };

                var resultData = getCommodityDetailsZPH(com.ToEntityData(), appId, userId, freightTo);
                if (resultData != null)
                {
                    ValidCostPrice(new List<Guid>() { com.Id }, null);
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.GetCommodityDetailsByActIdExt。。actId：{0}。appId：{1}。userId：{2}。freightTo：{3}", actId, appId, userId, freightTo), ex);
            }
            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        ///  获取ZPH商品详情
        /// </summary>
        /// <param name="com"></param>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="freightTo"></param>
        /// <param name="outPromotionId">outPromotionId相关逻辑已移除</param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO getCommodityDetailsZPHOld(CommodityDTO com, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId = null)
        {
            LogHelper.Debug(string.Format("进入getCommodityDetailsZPH方法，商品id：{1}，开始时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
            if (com == null)
                return null;
            CommoditySDTO commoditySDTO = new CommoditySDTO();
            commoditySDTO.Id = com.Id;
            commoditySDTO.Name = com.Name;
            commoditySDTO.Pic = com.PicturesPath;
            commoditySDTO.Price = com.Price;
            commoditySDTO.MarketPrice = com.MarketPrice;
            commoditySDTO.ReviewNum = com.TotalReview;
            commoditySDTO.Stock = com.Stock;
            commoditySDTO.Total = com.Salesvolume;
            commoditySDTO.CollectNum = com.TotalCollection;
            commoditySDTO.State = com.State;
            commoditySDTO.AppId = com.AppId;
            commoditySDTO.IsEnableSelfTake = com.IsEnableSelfTake;
            commoditySDTO.CommodityType = com.CommodityType;
            commoditySDTO.VideoWebUrl = com.HtmlVideoPath;
            commoditySDTO.VideoUrl = com.MobileVideoPath;
            commoditySDTO.VideoPicUrl = com.VideoPic;
            commoditySDTO.Duty = com.Duty ?? 0;
            commoditySDTO.No_Code = com.No_Code;
            if (com.IsDel)
            {
                commoditySDTO.State = 3;
            }
            commoditySDTO.Description = com.Description;
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
            }
            commoditySDTO.CommodityType = com.CommodityType;
            //分成推广
            if (Jinher.AMP.BTP.TPS.BACBP.CheckAppShare(com.AppId))
            {
                commoditySDTO.IsShare = true;
                commoditySDTO.SharePercent = com.SharePercent;
                if (commoditySDTO.SharePercent == null || commoditySDTO.SharePercent == 0)
                {
                    var share = AppExtension.ObjectSet().Where(t => t.Id == com.AppId).FirstOrDefault();
                    if (share != null)
                    {
                        if (share.IsDividendAll == true)
                        {
                            commoditySDTO.SharePercent = share.SharePercent;
                        }
                    }
                }
            }
            LogHelper.Debug(string.Format("进入BACBP.CheckAppShare方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
            //商品是否参加三级分销(和设置佣金没关系)
            var cdQuery = (from cd in CommodityDistribution.ObjectSet()
                           where cd.Id == com.Id
                           select cd.Id).Any();
            commoditySDTO.IsDistribute = cdQuery;

            // 多属性时，才检查 SKU库存
            if (commoditySDTO.ComAttibutes != null && commoditySDTO.ComAttibutes.Count > 0 && commoditySDTO.ComAttibutes.GroupBy(c => c.Attribute).Count() > 1)
            {
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                if (queryStock != null)
                {

                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                    foreach (var item in queryStock)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        tempStock.Price = item.Price;
                        tempStock.MarketPrice = item.MarketPrice;
                        tempStock.Stock = item.Stock;
                        tempStock.Id = item.Id;
                        tempStock.Duty = item.Duty ?? 0;
                        tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityStocks.Add(tempStock);
                    }
                    commoditySDTO.CommodityStocks = commodityStocks;
                }
            }
            else
            {
                commoditySDTO.CommodityStocks = new List<CommodityAttrStockDTO>();
            }

            //相关商品
            commoditySDTO.RelationCommoditys = getRelationCommodity(com.Id);
            //图片
            List<CommodityPictureCDTO> productDetailPicList = new List<CommodityPictureCDTO>();
            var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == com.Id).OrderBy(n => n.Sort);
            var pictures = from p in productDetailsPictures
                           select new CommodityPictureCDTO
                           {
                               Sort = p.Sort,
                               PicturesPath = p.PicturesPath,
                           };
            productDetailPicList = pictures.ToList();
            commoditySDTO.Pictures = productDetailPicList;


            DateTime now = DateTime.Now;
            List<TodayPromotionDTO> proList;

            if (outPromotionId.HasValue && outPromotionId != Guid.Empty)
            {
                Guid outProId = outPromotionId.Value;
                proList = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable && pro.OutsideId == outProId
                           orderby pro.PromotionType descending
                           select new TodayPromotionDTO()
                           {
                               PromotionId = p.PromotionId,
                               CommodityId = p.CommodityId,
                               Intensity = (decimal)p.Intensity,
                               StartTime = pro.StartTime,
                               EndTime = pro.EndTime,
                               DiscountPrice = (decimal)p.DiscountPrice,
                               LimitBuyEach = p.LimitBuyEach,
                               LimitBuyTotal = p.LimitBuyTotal,
                               SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                               AppId = pro.AppId,
                               ChannelId = pro.ChannelId,
                               OutsideId = pro.OutsideId,
                               PresellStartTime = pro.PresellStartTime,
                               PresellEndTime = pro.PresellEndTime,
                               PromotionType = pro.PromotionType,
                               GroupMinVolume = pro.GroupMinVolume,
                               ExpireSecond = pro.ExpireSecond,
                               Description = pro.Description
                           }).ToList();

            }
            else
            {

                proList = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable &&
                           pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                           orderby pro.PromotionType descending
                           select new TodayPromotionDTO()
                           {
                               PromotionId = p.PromotionId,
                               CommodityId = p.CommodityId,
                               Intensity = (decimal)p.Intensity,
                               StartTime = pro.StartTime,
                               EndTime = pro.EndTime,
                               DiscountPrice = (decimal)p.DiscountPrice,
                               LimitBuyEach = p.LimitBuyEach,
                               LimitBuyTotal = p.LimitBuyTotal,
                               SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                               AppId = pro.AppId,
                               ChannelId = pro.ChannelId,
                               OutsideId = pro.OutsideId,
                               PresellStartTime = pro.PresellStartTime,
                               PresellEndTime = pro.PresellEndTime,
                               PromotionType = pro.PromotionType,
                               GroupMinVolume = pro.GroupMinVolume,
                               ExpireSecond = pro.ExpireSecond,
                               Description = pro.Description
                           }).ToList();
            }

            TodayPromotionDTO promotion = proList.FirstOrDefault(c => c.PromotionType != 3);
            if (promotion != null)
            {
                commoditySDTO.LimitBuyEach = promotion.LimitBuyEach;
                commoditySDTO.LimitBuyTotal = promotion.LimitBuyTotal;
                commoditySDTO.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;
                commoditySDTO.PromotionType = promotion.PromotionType;
                commoditySDTO.PromotionTypeNew = (ComPromotionStatusEnum)promotion.PromotionType;
                commoditySDTO.PromotionStartTime = promotion.StartTime;
                commoditySDTO.PromotionEndTime = promotion.EndTime;
                commoditySDTO.PresellStartTime = promotion.PresellStartTime;
                commoditySDTO.PresellEndTime = promotion.PresellEndTime;
                commoditySDTO.PromotionId = promotion.PromotionId;
                commoditySDTO.OutPromotionId = promotion.OutsideId;
                commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                commoditySDTO.Intensity = promotion.Intensity;

                commoditySDTO.PromotionState = GetPromotionState(promotion);

                //预约
                if (promotion.PromotionType == 2 && promotion.PresellStartTime.HasValue && promotion.PresellStartTime < now && promotion.OutsideId.HasValue)
                {
                    var promotionOutSideId = promotion.OutsideId.Value;
                    var presell = ZPHSV.Instance.GetAndCheckPresellInfoById(new CheckPresellInfoCDTO()
                    {
                        comdtyId = com.Id,
                        id = promotionOutSideId
                    });
                    LogHelper.Debug(string.Format("进入ZPHSV.GetAndCheckPresellInfoById方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                    if (presell != null)
                        commoditySDTO.PreselledNum = presell.preselledNum;

                }
            }

            //拼团信息。
            TodayPromotionDTO tpDiyDto = proList.FirstOrDefault(tp => tp.PromotionType == 3);
            if (tpDiyDto != null)
            {
                string tpJson = JsonHelper.JsonSerializer<TodayPromotionDTO>(tpDiyDto);
                TodayPromotionExtendDTO tpExtend = JsonHelper.JsonDeserialize<TodayPromotionExtendDTO>(tpJson);
                tpExtend.PromotionState = GetPromotionState(tpDiyDto);

                commoditySDTO.DiyGroupPromotion = tpExtend;
            }

            //已参团人数
            var dgQuery = (from dg in DiyGroup.ObjectSet()
                           where dg.CommodityId == com.Id && (dg.State == 0 || dg.State == 1 || dg.State == 2 || dg.State == 3)
                           select dg.JoinNumber).ToList().Sum();
            commoditySDTO.AlreadyJoinCount = dgQuery;



            #region 众筹
            if (CustomConfig.CrowdfundingFlag)
            {
                //众筹状态
                var activeCrowdfundingCnt = Crowdfunding.ObjectSet().Count(c => c.AppId == com.AppId && c.State == 0 && c.StartTime <= now);
                if (activeCrowdfundingCnt > 0)
                    commoditySDTO.IsActiveCrowdfunding = true;
            }
            #endregion



            //是否收藏过
            appId = appId == Guid.Empty ? com.AppId : appId;
            int count = SetCollection.ObjectSet().Count(n => n.ColType == 1 && n.ColKey == com.Id && n.ChannelId == appId && n.UserId == userId);
            commoditySDTO.IsCollect = count > 0;
            commoditySDTO.CurrentTime = DateTime.Now;


            //部分包邮
            var partial = (from f in FreightTemplate.ObjectSet()
                           join fp in FreightPartialFree.ObjectSet() on f.Id equals fp.FreightTemplateId
                           where f.Id == com.FreightTemplateId && f.ExpressType == 2
                           select fp).ToList();


            Stopwatch timer = new Stopwatch();
            timer.Start();

            System.Threading.Tasks.Parallel.Invoke(() =>
            {
                //会员折扣信息
                commoditySDTO.VipPromotion = AVMSV.GetVipIntensity(commoditySDTO.AppId, userId);
                LogHelper.Debug(string.Format("进入AVMSV.GetVipIntensity方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

            }, () =>
            {

                commoditySDTO.HasReviewFunction = BACBP.CheckCommodityReview(appId);
                LogHelper.Debug(string.Format("进入BACBP.CheckCommodityReview方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                if (commoditySDTO.HasReviewFunction)
                {
                    commoditySDTO.Score = SNSSV.GetComFirstScore(com.AppId, com.Id);
                    LogHelper.Debug(string.Format("进入SNSSV.GetComFirstScore方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                }

            }
                    , () =>
                    {
                        var appinfo = APPSV.GetAppNameIcon(commoditySDTO.AppId);
                        LogHelper.Debug(string.Format("进入APPSV.GetAppNameIcon方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                        commoditySDTO.AppName = appinfo.AppName;
                        commoditySDTO.AppIcon = appinfo.AppIcon;
                    }
                    , () =>
                    {
                        commoditySDTO.IsAppSet = ZPHSV.Instance.CheckIsAppInZPH(com.AppId);
                        LogHelper.Debug(string.Format("进入ZPHSV.CheckIsAppInZPH方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        commoditySDTO.EquipmentUrl = LVPSV.GetEquipmentUrl(com.AppId);
                        LogHelper.Debug(string.Format("进入LVPSV.GetEquipmentUrl方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        commoditySDTO.CloudviewUrl = ZPHSV.GetCloudViewUrl(com.AppId);
                        LogHelper.Debug(string.Format("进入ZPHSV.GetCloudViewUrl方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        //运费
                        List<TemplateCountDTO> templateCountList = new List<TemplateCountDTO>();

                        decimal realPrice = commoditySDTO.DiscountPrice > 0 ? commoditySDTO.DiscountPrice.Value : commoditySDTO.Price * commoditySDTO.Intensity.Value / 10;
                        templateCountList.Add(new TemplateCountDTO { Count = 1, CommodityId = com.Id, Price = decimal.Round(realPrice, 2, MidpointRounding.AwayFromZero) });
                        var frDTO = CalFreightMultiAppsByTextExt(freightTo, com.IsEnableSelfTake, templateCountList, null, null, null);
                        commoditySDTO.Freight = frDTO.ResultCode == 0 ? frDTO.Freight : 0;
                        commoditySDTO.FreightTo = freightTo;
                        commoditySDTO.IsSetMulti = IsSetMulti(com.FreightTemplateId);
                        //包邮条件的描述
                        commoditySDTO.FreeFreightStandard = buildFreightPartialFreeDescription(partial, com.PricingMethod);
                    }
               );
            timer.Stop();
            LogHelper.Debug(string.Format("并发执行耗时: {0}", timer.ElapsedMilliseconds));

            return commoditySDTO;
        }

        /// <summary>
        ///  获取ZPH商品详情新（支持单属性SKU，20170731）
        /// </summary>
        /// <param name="com"></param>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="freightTo"></param>
        /// <param name="outPromotionId">outPromotionId相关逻辑已移除</param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO getCommodityDetailsZPH(CommodityDTO com, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId = null)
        {
            LogHelper.Debug(string.Format("进入getCommodityDetailsZPH方法，商品id：{1}，开始时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
            if (com == null)
                return null;
            CommoditySDTO commoditySDTO = new CommoditySDTO();
            commoditySDTO.Id = com.Id;
            commoditySDTO.Name = com.Name;
            commoditySDTO.Pic = com.PicturesPath;
            commoditySDTO.Price = com.Price;
            commoditySDTO.MarketPrice = com.MarketPrice;
            commoditySDTO.ReviewNum = com.TotalReview;
            commoditySDTO.Stock = com.Stock;
            commoditySDTO.Total = com.Salesvolume;
            commoditySDTO.CollectNum = com.TotalCollection;
            commoditySDTO.State = com.State;
            commoditySDTO.AppId = com.AppId;
            commoditySDTO.IsEnableSelfTake = com.IsEnableSelfTake;
            commoditySDTO.CommodityType = com.CommodityType;
            commoditySDTO.VideoWebUrl = com.HtmlVideoPath;
            commoditySDTO.VideoUrl = com.MobileVideoPath;
            commoditySDTO.VideoPicUrl = com.VideoPic;
            commoditySDTO.Duty = com.Duty ?? 0;
            commoditySDTO.No_Code = com.No_Code;
            if (com.IsDel)
            {
                commoditySDTO.State = 3;
            }
            commoditySDTO.Description = com.Description;
            commoditySDTO.TechSpecs = com.TechSpecs;
            commoditySDTO.SaleService = com.SaleService;
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute).GroupBy(p => p.SecondAttribute).Select(p => p.FirstOrDefault()).ToList();
            }
            commoditySDTO.CommodityType = com.CommodityType;
            //分成推广
            if (Jinher.AMP.BTP.TPS.BACBP.CheckAppShare(com.AppId))
            {
                commoditySDTO.IsShare = true;
                commoditySDTO.SharePercent = com.SharePercent;
                if (commoditySDTO.SharePercent == null || commoditySDTO.SharePercent == 0)
                {
                    var share = AppExtension.ObjectSet().Where(t => t.Id == com.AppId).FirstOrDefault();
                    if (share != null)
                    {
                        if (share.IsDividendAll == true)
                        {
                            commoditySDTO.SharePercent = share.SharePercent;
                        }
                    }
                }
            }
            LogHelper.Debug(string.Format("进入BACBP.CheckAppShare方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
            //商品是否参加三级分销(和设置佣金没关系)
            var cdQuery = (from cd in CommodityDistribution.ObjectSet() where cd.Id == com.Id select cd.Id).Any();
            commoditySDTO.IsDistribute = cdQuery;

            // 无属性商品不检查
            if (commoditySDTO.ComAttibutes != null && commoditySDTO.ComAttibutes.Count > 0)
            {
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                if (queryStock != null)
                {
                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                    foreach (var item in queryStock)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        tempStock.Price = item.Price;
                        tempStock.MarketPrice = item.MarketPrice;
                        tempStock.Stock = item.Stock;
                        tempStock.Id = item.Id;
                        tempStock.Duty = item.Duty ?? 0;
                        tempStock.ThumImg = item.ThumImg;
                        if (string.IsNullOrWhiteSpace(item.CarouselImgs))
                        {
                            tempStock.CarouselImgs = new string[0];
                        }
                        else
                        {
                            tempStock.CarouselImgs = item.CarouselImgs.Split(',');
                        }
                        tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityStocks.Add(tempStock);
                    }
                    commoditySDTO.CommodityStocks = commodityStocks.OrderBy(t => t.Price).ToList();
                }
            }
            else
            {
                commoditySDTO.CommodityStocks = new List<CommodityAttrStockDTO>();
            }

            //相关商品
            commoditySDTO.RelationCommoditys = getRelationCommodity(com.Id);
            //图片
            List<CommodityPictureCDTO> productDetailPicList = new List<CommodityPictureCDTO>();
            var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == com.Id).OrderBy(n => n.Sort);
            var pictures = from p in productDetailsPictures

                           select new CommodityPictureCDTO
                           {
                               Sort = p.Sort,
                               PicturesPath = p.PicturesPath,
                           };
            if (ThirdECommerceHelper.IsSuNingYiGou(com.AppId) && pictures.Count() >= 3)//苏宁店铺取后三张图片
            {
                commoditySDTO.Pictures = pictures.Skip(2).ToList();
            }
            else
            {
                commoditySDTO.Pictures = pictures.ToList();
            }
            DateTime now = DateTime.Now;
            List<TodayPromotionDTO> proList;

            if (outPromotionId.HasValue && outPromotionId != Guid.Empty && !ZPHSV.IsSSActivity(outPromotionId.Value))
            {
                Guid outProId = outPromotionId.Value;
                proList = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable && pro.OutsideId == outProId
                           orderby pro.PromotionType descending
                           select new TodayPromotionDTO()
                           {
                               PromotionId = p.PromotionId,
                               CommodityId = p.CommodityId,
                               Intensity = (decimal)p.Intensity,
                               StartTime = pro.StartTime,
                               EndTime = pro.EndTime,
                               DiscountPrice = (decimal)p.DiscountPrice,
                               LimitBuyEach = p.LimitBuyEach,
                               LimitBuyTotal = p.LimitBuyTotal,
                               SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                               AppId = pro.AppId,
                               ChannelId = pro.ChannelId,
                               OutsideId = pro.OutsideId,
                               PresellStartTime = pro.PresellStartTime,
                               PresellEndTime = pro.PresellEndTime,
                               PromotionType = pro.PromotionType,
                               GroupMinVolume = pro.GroupMinVolume,
                               ExpireSecond = pro.ExpireSecond,
                               Description = pro.Description
                           }).ToList();

            }
            else
            {

                proList = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable &&
                           pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                           orderby pro.PromotionType descending
                           select new TodayPromotionDTO()
                           {
                               PromotionId = p.PromotionId,
                               CommodityId = p.CommodityId,
                               Intensity = (decimal)p.Intensity,
                               StartTime = pro.StartTime,
                               EndTime = pro.EndTime,
                               DiscountPrice = (decimal)p.DiscountPrice,
                               LimitBuyEach = p.LimitBuyEach,
                               LimitBuyTotal = p.LimitBuyTotal,
                               SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                               AppId = pro.AppId,
                               ChannelId = pro.ChannelId,
                               OutsideId = pro.OutsideId,
                               PresellStartTime = pro.PresellStartTime,
                               PresellEndTime = pro.PresellEndTime,
                               PromotionType = pro.PromotionType,
                               GroupMinVolume = pro.GroupMinVolume,
                               ExpireSecond = pro.ExpireSecond,
                               Description = pro.Description
                           }).ToList();
            }

            #region 今日商品促销

            TodayPromotionDTO promotion = proList.OrderByDescending(p => p.SubTime).FirstOrDefault(c => c.PromotionType != 3);
            if (promotion != null)
            {
                commoditySDTO.LimitBuyEach = promotion.LimitBuyEach;
                commoditySDTO.LimitBuyTotal = promotion.LimitBuyTotal;
                commoditySDTO.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;
                commoditySDTO.PromotionType = promotion.PromotionType;
                commoditySDTO.PromotionTypeNew = (ComPromotionStatusEnum)promotion.PromotionType;
                commoditySDTO.PromotionStartTime = promotion.StartTime;
                commoditySDTO.PromotionEndTime = promotion.EndTime;
                commoditySDTO.PresellStartTime = promotion.PresellStartTime;
                commoditySDTO.PresellEndTime = promotion.PresellEndTime;
                commoditySDTO.PromotionId = promotion.PromotionId;
                commoditySDTO.OutPromotionId = promotion.OutsideId;
                commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                commoditySDTO.Intensity = promotion.Intensity;
                if (commoditySDTO.Intensity < 10)
                {
                    commoditySDTO.DiscountPrice = -1;
                }

                commoditySDTO.PromotionState = GetPromotionState(promotion);

                //预约或预售
                if ((promotion.PromotionType == 2 || promotion.PromotionType == 5) && promotion.PresellStartTime.HasValue && promotion.PresellStartTime < now && promotion.OutsideId.HasValue)
                {
                    var promotionOutSideId = promotion.OutsideId.Value;
                    var presell = ZPHSV.Instance.GetAndCheckPresellInfoById(new CheckPresellInfoCDTO()
                    {
                        comdtyId = com.Id,
                        id = promotionOutSideId
                    });
                    LogHelper.Debug(string.Format("进入ZPHSV.GetAndCheckPresellInfoById方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                    if (presell != null)
                    {
                        commoditySDTO.PreselledNum = presell.preselledNum;
                        if (promotion.PromotionType == 5)
                        {
                            commoditySDTO.PreselledNum = commoditySDTO.SurplusLimitBuyTotal ?? 0;
                            if (presell.DeliveryDays.HasValue)
                            {
                                commoditySDTO.DeliveryTime = "支付后" + presell.DeliveryDays.Value + "天内";
                            }
                            else if (presell.DeliveryTime.HasValue)
                            {
                                commoditySDTO.DeliveryTime = presell.DeliveryTime.Value.ToString("yyyy-MM-dd");
                            }
                            else
                            {

                            }

                            // 预售商品如果已下架，则更新为上架
                            if (commoditySDTO.State == 1)
                            {
                                var tempCommodity = Commodity.FindByID(commoditySDTO.Id);
                                tempCommodity.RefreshCache(EntityState.Modified);
                                commoditySDTO.State = tempCommodity.State;

                                // 商品上架
                                //tempCommodity.State = 0;
                                //ContextSession contextSession = ContextFactory.CurrentThreadContext;
                                //contextSession.SaveObject(commodity);
                                //contextSession.SaveChange();
                            }
                        }
                    }
                }
            }

            #endregion

            #region 拼团信息

            //拼团信息。
            TodayPromotionDTO tpDiyDto = proList.FirstOrDefault(tp => tp.PromotionType == 3);
            if (tpDiyDto != null)
            {
                string tpJson = JsonHelper.JsonSerializer<TodayPromotionDTO>(tpDiyDto);
                TodayPromotionExtendDTO tpExtend = JsonHelper.JsonDeserialize<TodayPromotionExtendDTO>(tpJson);
                tpExtend.PromotionState = GetPromotionState(tpDiyDto);

                commoditySDTO.DiyGroupPromotion = tpExtend;
            }

            //已参团人数
            var dgQuery = (from dg in DiyGroup.ObjectSet()
                           where dg.CommodityId == com.Id && (dg.State == 0 || dg.State == 1 || dg.State == 2 || dg.State == 3)
                           select dg.JoinNumber).ToList().Sum();
            commoditySDTO.AlreadyJoinCount = dgQuery;

            #endregion


            #region 众筹
            if (CustomConfig.CrowdfundingFlag)
            {
                //众筹状态
                var activeCrowdfundingCnt = Crowdfunding.ObjectSet().Count(c => c.AppId == com.AppId && c.State == 0 && c.StartTime <= now);
                if (activeCrowdfundingCnt > 0)
                    commoditySDTO.IsActiveCrowdfunding = true;
            }
            #endregion


            #region 是否收藏

            //是否收藏过
            appId = appId == Guid.Empty ? com.AppId : appId;
            int count = SetCollection.ObjectSet().Count(n => n.ColType == 1 && n.ColKey == com.Id && n.ChannelId == appId && n.UserId == userId);
            commoditySDTO.IsCollect = count > 0;
            commoditySDTO.CurrentTime = DateTime.Now;

            #endregion


            //部分包邮
            var partial = (from f in FreightTemplate.ObjectSet()
                           join fp in FreightPartialFree.ObjectSet() on f.Id equals fp.FreightTemplateId
                           where f.Id == com.FreightTemplateId && f.ExpressType == 2
                           select fp).ToList();

            var data = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO>();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            System.Threading.Tasks.Parallel.Invoke(
                    () =>
                    {
                        //会员折扣信息
                        commoditySDTO.VipPromotion = AVMSV.GetVipIntensity(commoditySDTO.AppId, userId);
                        LogHelper.Debug(string.Format("进入AVMSV.GetVipIntensity方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {

                        commoditySDTO.HasReviewFunction = BACBP.CheckCommodityReview(appId);
                        LogHelper.Debug(string.Format("进入BACBP.CheckCommodityReview方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                        if (commoditySDTO.HasReviewFunction)
                        {
                            commoditySDTO.Score = SNSSV.GetComFirstScore(com.AppId, com.Id);
                            LogHelper.Debug(string.Format("进入SNSSV.GetComFirstScore方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                        }

                    }
                    , () =>
                    {
                        var appinfo = APPSV.GetAppNameIcon(commoditySDTO.AppId);
                        LogHelper.Debug(string.Format("进入APPSV.GetAppNameIcon方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                        commoditySDTO.AppName = appinfo.AppName;
                        commoditySDTO.AppIcon = appinfo.AppIcon;
                    }
                    , () =>
                    {
                        commoditySDTO.IsAppSet = ZPHSV.Instance.CheckIsAppInZPH(com.AppId);
                        LogHelper.Debug(string.Format("进入ZPHSV.CheckIsAppInZPH方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        commoditySDTO.EquipmentUrl = LVPSV.GetEquipmentUrlNew(com.AppId, ContextDTO.LoginUserID);
                        LogHelper.Debug(string.Format("进入LVPSV.GetEquipmentUrl方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        commoditySDTO.CloudviewUrl = ZPHSV.GetCloudViewUrl(com.AppId);
                        LogHelper.Debug(string.Format("进入ZPHSV.GetCloudViewUrl方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        //运费
                        List<TemplateCountDTO> templateCountList = new List<TemplateCountDTO>();
                        decimal realPrice = commoditySDTO.DiscountPrice > 0 ? commoditySDTO.DiscountPrice.Value : commoditySDTO.Price * commoditySDTO.Intensity.Value / 10;
                        templateCountList.Add(new TemplateCountDTO { Count = 1, CommodityId = com.Id, Price = decimal.Round(realPrice, 2, MidpointRounding.AwayFromZero) });
                        var frDTO = CalFreightMultiAppsByTextExt(freightTo, com.IsEnableSelfTake, templateCountList, null, null, null);
                        commoditySDTO.Freight = frDTO.ResultCode == 0 ? frDTO.Freight : 0;
                        commoditySDTO.FreightTo = freightTo;
                        commoditySDTO.IsSetMulti = IsSetMulti(com.FreightTemplateId);
                        //包邮条件的描述
                        commoditySDTO.FreeFreightStandard = buildFreightPartialFreeDescription(partial, com.PricingMethod);
                    }
                    , () =>
                    {
                        #region 油卡兑换券
                        var ykPersentList = YJBHelper.GetCommodityYouKaPercent(appId, new List<Guid> { com.Id });
                        if (ykPersentList != null && ykPersentList.Count == 1 && ykPersentList[0].CommodityId == com.Id && ykPersentList[0].YouKaPersent > 0)
                        {
                            var giveMoney = Math.Round(ykPersentList[0].YouKaPersent * com.Price / 100, 2);
                            commoditySDTO.YouKaInfo = new CommodityYouKaDTO(giveMoney, ykPersentList[0].YouKaPersent);
                        }
                        else
                        {
                            commoditySDTO.YouKaInfo = new CommodityYouKaDTO(0, 0);
                        }
                        #endregion
                    }
                    , () =>
                    {
                        commoditySDTO.YJBInfo = new YJB.Deploy.CustomDTO.OrderInsteadCashDTO();
                        List<string> lables = new List<string>();
                        Guid yjappid = YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                        // 易捷APP才显示
                        if (appId == yjappid)
                        {
                            // 判断是否易捷发货 若商品所属的APP是自营他配或自营自配，则显示易捷发货
                            MallApply yjApp = MallApply.GetTGQuery(yjappid).Where(_ => _.AppId == commoditySDTO.AppId).FirstOrDefault();
                            if (yjApp == null && commoditySDTO.AppId == yjappid)
                            {
                                yjApp = new MallApply { EsAppId = yjappid, AppId = yjappid, Type = 0 };
                            }
                            if (yjApp != null)
                            {
                                if (yjApp.Type != 1)
                                {
                                    commoditySDTO.SelfSupport = "自营";
                                }
                                else
                                {
                                    lables.Add("商家入驻");
                                    Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                                    model.Title = "商家入驻";
                                    model.Content = "商家入驻";
                                    data.Add(model);
                                }
                                var yjb = YJBHelper.GetCommodityCashPercentWithoutUser(yjApp.EsAppId,
                                        new List<YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO> {
                                            new  YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO {
                                                Id = com.Id, AppId = com.AppId, Number=1, Price = com.Price
                                            }
                                    });
                                commoditySDTO.YJBInfo.Enabled = false;
                                if (yjb.Enabled)
                                {
                                    commoditySDTO.YJBInfo.Enabled = yjb.InsteadCashCount > 0;
                                    commoditySDTO.YJBInfo.InsteadCashCount = yjb.InsteadCashCount;
                                    commoditySDTO.YJBInfo.InsteadCashAmount = yjb.InsteadCashAmount;
                                    commoditySDTO.YJBInfo.CommodityList = yjb.CommodityList;
                                }
                            }
                        }

                        // 七天无理由退货
                        if (com.IsAssurance == true)
                        {
                            lables.Add("七天无理由退");
                            Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                            model.Title = "七天无理由退";
                            model.Content = "七天无理由退";
                            data.Add(model);
                        }
                        if (com.IsReturns == true)
                        {
                            lables.Add("不支持退货");
                            ServiceSettingDTO model = new ServiceSettingDTO();
                            model.Title = "不支持退货";
                            model.Content = "不支持退货";
                            data.Add(model);
                        }
                        if (com.Isnsupport == true)
                        {
                            lables.Add("不支持7天无理由退货");
                            Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                            model.Title = "不支持7天无理由退货";
                            model.Content = "不支持7天无理由退货";
                            data.Add(model);
                        }
                        if (commoditySDTO.IsShare && commoditySDTO.SharePercent > 0)
                        {
                            lables.Add("分享有礼");
                            Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                            model.Title = "分享有礼";
                            model.Content = "将平台商品分享给好友，好友购买商品后，分享者可获得交易额" + Convert.ToInt32(commoditySDTO.SharePercent * 100) + "%的返利，返利以油卡兑换券的形式存入分享者的兑换包。";
                            data.Add(model);
                        }
                        if (!string.IsNullOrEmpty(com.ServiceSettingId))
                        {
                            string[] str = com.ServiceSettingId.Split('|');
                            if (str != null && str.Count() > 0)
                            {
                                List<Guid> ids = new List<Guid>();
                                foreach (var item in str)
                                {
                                    Guid id = Guid.Parse(item);
                                    var entity = ServiceSettings.ObjectSet().FirstOrDefault(p => p.Id == id);
                                    if (entity != null)
                                    {
                                        ServiceSettingDTO model = new ServiceSettingDTO();
                                        model.Title = entity.Title;
                                        model.Content = entity.Content;
                                        data.Add(model);
                                    }
                                }
                            }

                        }
                        commoditySDTO.ServiceSettings = data;
                        // 根据当前用户位置，显示对应的满包邮条件；
                        // 若获取不到当前用户的地理位置，取默认运费的满包邮条件，即显示商品运费金额对应的满包邮条件
                        var freightTemplate = FreightTemplate.ObjectSet().Where(_ => _.Id == com.FreightTemplateId).FirstOrDefault();
                        if (freightTemplate != null)
                        {
                            if (freightTemplate.ExpressType == 1)
                            {
                                lables.Add("全国包邮");
                                commoditySDTO.PostAge = "全国包邮";
                            }
                            else if (freightTemplate.ExpressType == 2)
                            {
                                //var ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                                //var region = RedisHelper.GetHashValue<string>(RedisKeyConst.IpRegion, ip);
                                //if (region == null)
                                //{
                                //    region = IPHelper.GetRegion(ip);
                                //    if (region != null)
                                //    {
                                //        RedisHelper.AddHash(RedisKeyConst.IpRegion, ip, region);
                                //    }
                                //}
                                if (!string.IsNullOrWhiteSpace(freightTo))
                                {
                                    var pcode = ProvinceCityHelper.GetProvinceCodeByName(freightTo);
                                    //LogHelper.Info("ProvinceCityHelper.GetProvinceCodeByName(" + freightTo + ")=>" + pcode);
                                    if (!string.IsNullOrEmpty(pcode))
                                    {
                                        var freightdetails = FreightPartialFree.ObjectSet().Where(_ => _.FreightTemplateId == freightTemplate.Id && _.DestinationCodes.Contains(pcode)).FirstOrDefault();
                                        if (freightdetails != null)
                                        {
                                            if (freightdetails.FreeCount > 0)
                                            {
                                                lables.Add("满" + Math.Floor(freightdetails.FreeCount) + "件包邮");
                                                commoditySDTO.PostAge = "满" + Math.Floor(freightdetails.FreeCount) + "件包邮";
                                            }
                                            else
                                            {
                                                lables.Add("满" + Math.Floor(freightdetails.FreePrice) + "元包邮");
                                                commoditySDTO.PostAge = "满" + Math.Floor(freightdetails.FreePrice) + "元包邮";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        commoditySDTO.Labels = lables.ToArray();

                        commoditySDTO.Coupons = new List<string>().ToArray();
                        // 优惠券
                        //var coupons = CouponSV.Instance.GetUsableCouponsTemplateList(new Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplateUsableRequestDTO
                        //{
                        //    isThirdParty = CacheHelper.MallApply.GetMallTypeListByEsAppId(appId).Any(_ => _.Id == commoditySDTO.AppId && _.Type == 1),
                        //    CurrentPage = 0,
                        //    PageSize = 20,
                        //    UserId = userId,
                        //    AppList = new List<Guid> { commoditySDTO.AppId, appId },//加上appId 显示跨店满减券，入驻自营的，这两个会不一样。其它情况一样的。qgb
                        //    CommodityId = commoditySDTO.Id

                        //}, true );


                        #region//优惠券xiexg
                        ShoppingCartSV catrsv = new ShoppingCartSV();
                        List<Jinher.AMP.Coupon.Deploy.CustomDTO.AppComs> list = new List<AppComs>();
                        ///YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                        var mallApply = MallApply.ObjectSet().Where(t => t.AppId == com.AppId && t.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Select(o => new { o.AppId, o.Type }).Distinct().ToList();
                        if (mallApply != null && mallApply.Count > 0)
                        {
                            LogHelper.Debug(string.Format("谢晓光检测商城id，商城id：{1}，appid: {0}", YJB.Deploy.CustomDTO.YJBConsts.YJAppId, com.AppId));
                            list.Add(new AppComs { AppId = commoditySDTO.AppId, CommodityId = commoditySDTO.Id, type = mallApply.FirstOrDefault().Type });
                        }
                        else
                        {
                            LogHelper.Debug(string.Format("谢晓光检测商城id，没数据，商城id：{1}，appid: {0}", YJB.Deploy.CustomDTO.YJBConsts.YJAppId, com.AppId));
                            list.Add(new AppComs { AppId = commoditySDTO.AppId, CommodityId = commoditySDTO.Id, type = 1 });
                        }
                        List<ShopCartCouponDTO> couponlist = catrsv.GetCouponDto(userId, list, YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                        CouponTemplateCanUseListDTO coupons = new CouponTemplateCanUseListDTO();
                        List<CouponTemplatDetailDTO> templist = new List<CouponTemplatDetailDTO>();
                        if (couponlist != null && couponlist.Count() > 0)
                        {
                            couponlist.ForEach(c =>
                            {
                                var coupon = new Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplatDetailDTO();
                                coupon.Cash = c.Cash;
                                coupon.Id = c.Id;
                                coupon.AppId = c.AppId;
                                coupon.BeginTime = c.BeginTime;
                                coupon.EndTime = c.EndTime;
                                coupon.LimitCondition = c.LimitCondition;
                                coupon.Description = c.Description;
                                coupon.Direction = c.Direction;
                                coupon.LimitUse = c.LimitUse;
                                coupon.RemainCount = c.RemainCount;
                                coupon.ModifiedOn = c.ModifiedOn;
                                coupon.Name = c.Name;
                                coupon.ThrowTime = c.ThrowTime;
                                coupon.SubTime = c.SubTime;
                                coupon.UserId = c.UserId;
                                coupon.IsDraw = c.IsDraw;
                                coupon.UseNum = c.UseNum;
                                templist.Add(coupon);
                            }
                                );
                        }
                        List<CouponTemplatDetailDTO> list2 = new List<CouponTemplatDetailDTO>();
                        foreach (var obj in templist.Distinct().ToList())
                        {
                            if (list2.Where(o => o.Id == obj.Id) == null || list2.Where(o => o.Id == obj.Id).Count() == 0)
                            {
                                list2.Add(obj);
                            }
                        }
                        coupons.RecordCollection = list2;
                        coupons.TotalCount = templist.Distinct().Count();
                        #endregion


                        if (coupons != null && coupons.RecordCollection != null)
                        {
                            List<string> strcoupons = new List<string>();
                            foreach (var item in coupons.RecordCollection)
                            {
                                if (item.LimitCondition == 0)
                                {
                                    strcoupons.Add("减" + item.Cash.ToString("0.##") + "元");//string.Format("{0:0.##}",d) qgb
                                }
                                else
                                {

                                    strcoupons.Add("满" + item.LimitCondition.ToString("0.##") + "元减" + item.Cash.ToString("0.##") + "元");
                                }
                            }
                            commoditySDTO.Coupons = strcoupons.ToArray();
                            commoditySDTO.CouponList = coupons.RecordCollection.ToList();
                            LogHelper.Debug(string.Format("进入Commodity.Lables方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                        }
                    }, () =>
                    {
                        #region 规格设置集合
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                        var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                        if (commoditySpecification.Count() > 0)
                        {
                            Guid commodityId = commoditySDTO.Id;
                            var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                            if (commoditySpecificationlist.Count() > 0)
                            {
                                commoditySpecificationlist.ForEach(p =>
                                {
                                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                    model.Id = p.Id;
                                    model.Name = "规格设置";
                                    model.Attribute = p.Attribute ?? 0;
                                    model.strAttribute = "1*" + p.Attribute + "";
                                    Specificationslist.Add(model);

                                });
                            }

                        }
                        commoditySDTO.Specifications = Specificationslist;
                        #endregion

                    }
                    , () =>
                    {
                        #region 获取活动sku属性集合

                        if (promotion != null && promotion.OutsideId != null)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(promotion.OutsideId)))
                            {
                                LogHelper.Debug(string.Format("进入ZPHSV.Instance.GetSkuActivityList方法，promotion.OutsideId：{0}", promotion.OutsideId));
                                var skuAList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).Where(t => t.IsJoin && t.CommodityId == commoditySDTO.Id);
                                List<Deploy.CustomDTO.SkuActivityCDTO> skuActivityCdtos = skuAList.Select(skuActivityCdto => new Deploy.CustomDTO.SkuActivityCDTO
                                {
                                    id = skuActivityCdto.id,
                                    OutSideActivityId = skuActivityCdto.OutSideActivityId,
                                    OutSideActivityType = skuActivityCdto.OutSideActivityType,
                                    CommodityId = skuActivityCdto.CommodityId,
                                    CommodityStockId = skuActivityCdto.CommodityStockId,
                                    IsJoin = skuActivityCdto.IsJoin,
                                    subId = skuActivityCdto.subId,
                                    subTime = skuActivityCdto.subTime,
                                    modifiedOn = skuActivityCdto.modifiedOn,
                                    JoinPrice = skuActivityCdto.JoinPrice
                                }).ToList();
                                commoditySDTO.SkuActivityCdtos = skuActivityCdtos.OrderBy(t => t.JoinPrice).ToList();
                                if (skuActivityCdtos.Count > 0)
                                {
                                    commoditySDTO.MinSkuPrice = skuActivityCdtos.Min(t => t.JoinPrice);
                                    commoditySDTO.MaxSkuPrice = skuActivityCdtos.Max(t => t.JoinPrice);
                                    if (commoditySDTO.Intensity < 10)
                                    {
                                        commoditySDTO.DiscountPrice = -1;
                                    }
                                    else
                                    {
                                        commoditySDTO.DiscountPrice = commoditySDTO.MaxSkuPrice;
                                    }
                                }
                                else
                                {
                                    //兼容老数据
                                    LogHelper.Debug(string.Format("进入ZPHSV.Instance.GetSkuActivityList方法，老数据的情况 outPromotionId：{0},tpDiyDto：{1}", promotion.OutsideId, JsonHelper.JsSerializer(promotion)));
                                    commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                                }
                            }
                        }
                        else if (tpDiyDto != null && tpDiyDto.OutsideId != null && outPromotionId != null && outPromotionId != Guid.Empty)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(tpDiyDto.OutsideId)))
                            {
                                LogHelper.Debug(string.Format("进入ZPHSV.Instance.GetSkuActivityList方法，tpDiyDto.OutsideId：{0}", tpDiyDto.OutsideId));
                                var skuAList = ZPHSV.Instance.GetSkuActivityList((Guid)tpDiyDto.OutsideId).Where(t => t.IsJoin);
                                List<Deploy.CustomDTO.SkuActivityCDTO> skuActivityCdtos = skuAList.Select(skuActivityCdto => new Deploy.CustomDTO.SkuActivityCDTO
                                {
                                    id = skuActivityCdto.id,
                                    OutSideActivityId = skuActivityCdto.OutSideActivityId,
                                    OutSideActivityType = skuActivityCdto.OutSideActivityType,
                                    CommodityId = skuActivityCdto.CommodityId,
                                    CommodityStockId = skuActivityCdto.CommodityStockId,
                                    IsJoin = skuActivityCdto.IsJoin,
                                    subId = skuActivityCdto.subId,
                                    subTime = skuActivityCdto.subTime,
                                    modifiedOn = skuActivityCdto.modifiedOn,
                                    JoinPrice = skuActivityCdto.JoinPrice
                                }).ToList();
                                commoditySDTO.SkuActivityCdtos = skuActivityCdtos.OrderBy(t => t.JoinPrice).ToList();
                                if (skuActivityCdtos.Count > 0)
                                {
                                    commoditySDTO.MinSkuPrice = skuActivityCdtos.Min(t => t.JoinPrice);
                                    commoditySDTO.MaxSkuPrice = skuActivityCdtos.Max(t => t.JoinPrice);
                                    if (commoditySDTO.CommodityStocks.Any())
                                    {
                                        commoditySDTO.MinPrice = commoditySDTO.CommodityStocks.Min(t => t.Price);
                                        commoditySDTO.MaxPrice = commoditySDTO.CommodityStocks.Max(t => t.Price);
                                    }
                                    commoditySDTO.DiscountPrice = commoditySDTO.MaxSkuPrice;
                                }
                                else
                                {
                                    //兼容老数据
                                    LogHelper.Debug(string.Format("进入ZPHSV.Instance.GetSkuActivityList方法，老数据的情况 outPromotionId：{0},tpDiyDto：{1}", tpDiyDto.OutsideId, JsonHelper.JsSerializer(tpDiyDto)));
                                    commoditySDTO.DiscountPrice = tpDiyDto.DiscountPrice;
                                }
                            }
                        }

                        #endregion
                    }
                    , () =>
                    {
                        #region 根据商品ID获取商品参与的优惠套装

                        var setMealActivitys = ZPHSV.Instance.GetSetMealActivitysByCommodityId(com.Id, appId, true);
                        if (setMealActivitys.Count > 0)
                        {
                            commoditySDTO.MealActivityInfo = "可省" + setMealActivitys[0].PreferentialPrice + "元";
                        }

                        #endregion
                    }
                    , () =>
                    {
                        #region 赠品信息
                        var presents = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == commoditySDTO.Id)
                            .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                                pp => pp.PresentPromotionId, ppc => ppc.Id,
                                (c, p) => new { Commodity = c, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime })
                            .ToList();
                        if (presents.Count > 0)
                        {
                            commoditySDTO.Present = new CommodiyPresentDTO();
                            var firstPromotion = presents.First();
                            commoditySDTO.Present.Limit = firstPromotion.Limit ?? 0;
                            if (commoditySDTO.Present.Limit == 0)
                            {
                                commoditySDTO.Present.Limit = 1;
                            }
                            commoditySDTO.Present.BeginTime = firstPromotion.BeginTime;
                            commoditySDTO.Present.EndTime = firstPromotion.EndTime;
                            commoditySDTO.Present.CommodityStockIds = presents.Where(_ => _.Commodity.CommoditySKUId != Guid.Empty).Select(_ => _.Commodity.CommoditySKUId).ToList();

                            commoditySDTO.Present.IsAll = true;
                            commoditySDTO.Present.Title = "购买" + (commoditySDTO.Present.Limit > 1 ? commoditySDTO.Present.Limit + "件" : "") + "即送超值赠品 （赠完即止）";
                            if (commoditySDTO.CommodityStocks != null && commoditySDTO.CommodityStocks.Count > 0)
                            {
                                bool isAll = true;
                                var titles = new List<string>();
                                foreach (var item in commoditySDTO.CommodityStocks)
                                {
                                    if (commoditySDTO.Present.CommodityStockIds.Any(_ => _ == item.Id))
                                    {
                                        titles.Add(string.Join("，", item.ComAttribute.Select(_ => _.SecondAttribute)));
                                    }
                                    else
                                    {
                                        isAll = false;
                                    }
                                }
                                if (!isAll)
                                {
                                    commoditySDTO.Present.IsAll = false;
                                    commoditySDTO.Present.Title = "购买 “" + string.Join("”、 “", titles) + "” " + (commoditySDTO.Present.Limit > 1 ? commoditySDTO.Present.Limit + "件" : "") + "送超值赠品 （赠完即止）";
                                }
                            }

                            var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == firstPromotion.PromotionId).ToList();
                            var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                            var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                            commoditySDTO.Present.Items = new List<CommodiyPresentItem>();
                            foreach (var item in gifts)
                            {
                                var tempCom = GetCommodity(commoditySDTO.AppId, item.CommodityId);
                                var commodiyPresentItem = new CommodiyPresentItem();
                                commodiyPresentItem.Id = item.CommodityId;
                                commodiyPresentItem.StockId = item.CommoditySKUId;
                                commodiyPresentItem.Name = item.CommodityName;
                                commodiyPresentItem.Pic = tempCom.PicturesPath;
                                commodiyPresentItem.Number = item.Number;

                                if (item.CommoditySKUId != Guid.Empty)
                                {
                                    var giftCommodityStock = giftCommodityStocks.Find(_ => _.Id == item.CommoditySKUId);
                                    if (giftCommodityStock != null && giftCommodityStock.Stock > 0)
                                    {
                                        commodiyPresentItem.SKU = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(giftCommodityStock.ComAttribute);
                                        commodiyPresentItem.Stock = giftCommodityStock.Stock;
                                        commoditySDTO.Present.Items.Add(commodiyPresentItem);
                                    }
                                }
                                else
                                {
                                    if (tempCom.Stock > 0)
                                    {
                                        commodiyPresentItem.Stock = tempCom.Stock;
                                        commoditySDTO.Present.Items.Add(commodiyPresentItem);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    , () =>
                    {
                        #region 获取商品分类Id
                        //判断是否是易捷北京的app
                        var temp = ZPHSV.Instance.GetAppIdlist(new List<Guid>() { YJB.Deploy.CustomDTO.YJBConsts.YJAppId });
                        var ids = temp.Select(t => t.AppId).ToList();

                        if (ids.Contains(commoditySDTO.AppId))
                        {
                            //获取分类id 处理检索出来包含移除分类的商品信息
                            var commodityCategory = CommodityCategory.ObjectSet().FirstOrDefault(t => t.CommodityId == commoditySDTO.Id && t.AppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                            if (commodityCategory != null)
                            {
                                commoditySDTO.CategoryId = commodityCategory.CategoryId;
                                if (commoditySDTO.CategoryId == Guid.Empty)
                                {
                                    commoditySDTO.State = 1;
                                }
                            }
                        }
                        #endregion
                    }
                    , () =>
                    {
                        #region 库存为0时获取到货的提醒状态
                        if (com.Stock == 0)
                        {
                            var MyNotifications = ZPHSV.GetNotificationsType(com.Id, userId);
                            NotificationsDTO NoticeInfo = new NotificationsDTO();
                            if (MyNotifications != null && MyNotifications.Id != Guid.Empty)
                            {

                                NoticeInfo.state = MyNotifications.BizRemindType.Value;
                                NoticeInfo.Content = new EnumHelper().GetDescription(GetNoticeType(MyNotifications));
                                commoditySDTO.Notice = NoticeInfo;
                            }
                            else
                            {
                                NoticeInfo.state = 0;
                                NoticeInfo.Content = new EnumHelper().GetDescription(NoticeTypeEnum.到货提醒);
                                commoditySDTO.Notice = NoticeInfo;
                            }
                        }
                        #endregion
                    }
                    , () =>
                    {
                        #region 获取苏宁易购实时库存
                        //if (ThirdECommerceHelper.IsSuNingYiGou(commoditySDTO.AppId))
                        //{
                        //    var flag = false;
                        //    var hasDefaultAddress = false;
                        //    if (userId != Guid.Empty)
                        //    {
                        //        var address = new DeliveryAddressSV().GetDeliveryAddressListExt(userId, commoditySDTO.AppId, 1);
                        //        if (address.Count > 0)
                        //        {

                        //            hasDefaultAddress = true;
                        //            var addres = address.SingleOrDefault();
                        //            string cityId = "";
                        //            string countryId = "";
                        //            if (",1,2,3,4,".IndexOf(addres.ProvinceCode) > 0)
                        //            {
                        //                cityId = addres.ProvinceCode;
                        //                countryId = addres.CityCode;
                        //            }
                        //            else
                        //            {
                        //                cityId = addres.CityCode;
                        //                countryId = addres.DistrictCode;
                        //            }
                        //            flag = SuningSV.GetSNInventory(cityId, countryId, "1", com.JDCode);
                        //        }
                        //    }
                        //    if (!hasDefaultAddress)
                        //    {
                        //        flag = SuningSV.GetSNMprodstock(CustomConfig.Suning_CityId, com.JDCode);
                        //    }
                        //    if (!flag) commoditySDTO.Stock = 0;
                        //}
                        #endregion
                    }
                    , () =>
                    {
                        #region 获取京东实时库存
                        if (ThirdECommerceHelper.IsJingDongDaKeHu(commoditySDTO.AppId))
                        {
                            var list = JDSV.GetStockById(new List<string> { com.JDCode }, "1_0_0");//取北京的库存
                            if (list != null && list.Count > 0 && !list.Any(p => p.HaveStock)) commoditySDTO.Stock = 0;
                        }
                        #endregion
                    }
                );
            timer.Stop();
            LogHelper.Debug(string.Format("并发执行耗时: {0}", timer.ElapsedMilliseconds));

            #region 易捷北京促销活动时不能使用优惠券、易捷币、加油卡兑换券

            if (appId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && ((commoditySDTO.PromotionId.HasValue) || commoditySDTO.VipPromotion.DiscountPrice > 0 
                || commoditySDTO.Present != null || commoditySDTO.MealActivityInfo != null))
            {
                commoditySDTO.YouKaInfo.GiveMoney = 0;
                commoditySDTO.Coupons = new List<string>().ToArray();
                commoditySDTO.CouponList = new List<CouponTemplatDetailDTO>();
                commoditySDTO.YJBInfo.Enabled = false;
            }
            #endregion

            return commoditySDTO;
        }
        /// <summary>
        /// 获取到货提醒类型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private NoticeTypeEnum GetNoticeType(Jinher.AMP.ZPH.Deploy.MyNotificationsDTO dto)
        {
            var NoticeType = NoticeTypeEnum.到货提醒;
            if (dto.BizRemindType.Value == 3 && dto.IsCancel == false)
            {
                NoticeType = NoticeTypeEnum.接受短信通知;
            }
            else if (dto.BizRemindType.Value == 3 && dto.IsCancel == true)
            {
                NoticeType = NoticeTypeEnum.到货提醒;
            }
            else if (dto.BizRemindType.Value == 4 && dto.IsCancel == false)
            {
                NoticeType = NoticeTypeEnum.已设置到货提醒;
            }
            else if (dto.BizRemindType.Value == 4 && dto.IsCancel == true)
            {
                NoticeType = NoticeTypeEnum.已取消短信到货提醒;
            }
            return NoticeType;
        }
        /// <summary>
        ///  获取ZPH商品详情新（支持单属性SKU，20170731）
        /// </summary>
        /// <param name="com"></param>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="freightTo"></param>
        /// <param name="jcActivityId">金采团购活动id</param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO getCommodityDetailsZPHII(CommodityDTO com, System.Guid appId, Guid userId, string freightTo, Guid? jcActivityId = null)
        {
            LogHelper.Debug(string.Format("进入getCommodityDetailsZPHII方法，商品id：{1}，开始时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
            if (com == null)
                return null;
            CommoditySDTO commoditySDTO = new CommoditySDTO();
            commoditySDTO.Id = com.Id;
            commoditySDTO.Name = com.Name;
            commoditySDTO.Pic = com.PicturesPath;
            commoditySDTO.Price = com.Price;
            commoditySDTO.MarketPrice = com.MarketPrice;
            commoditySDTO.ReviewNum = com.TotalReview;
            commoditySDTO.Stock = com.Stock;
            commoditySDTO.Total = com.Salesvolume;
            commoditySDTO.CollectNum = com.TotalCollection;
            commoditySDTO.State = com.State;
            commoditySDTO.AppId = com.AppId;
            commoditySDTO.IsEnableSelfTake = com.IsEnableSelfTake;
            commoditySDTO.CommodityType = com.CommodityType;
            commoditySDTO.VideoWebUrl = com.HtmlVideoPath;
            commoditySDTO.VideoUrl = com.MobileVideoPath;
            commoditySDTO.VideoPicUrl = com.VideoPic;
            commoditySDTO.Duty = com.Duty ?? 0;
            commoditySDTO.No_Code = com.No_Code;
            if (com.IsDel)
            {
                commoditySDTO.State = 3;
            }
            commoditySDTO.Description = com.Description;
            commoditySDTO.TechSpecs = com.TechSpecs;
            commoditySDTO.SaleService = com.SaleService;
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
            }
            commoditySDTO.CommodityType = com.CommodityType;
            //分成推广
            if (Jinher.AMP.BTP.TPS.BACBP.CheckAppShare(com.AppId))
            {
                commoditySDTO.IsShare = true;
                commoditySDTO.SharePercent = com.SharePercent;
                if (commoditySDTO.SharePercent == null || commoditySDTO.SharePercent == 0)
                {
                    var share = AppExtension.ObjectSet().Where(t => t.Id == com.AppId).FirstOrDefault();
                    if (share != null)
                    {
                        if (share.IsDividendAll == true)
                        {
                            commoditySDTO.SharePercent = share.SharePercent;
                        }
                    }
                }
            }
            LogHelper.Debug(string.Format("进入BACBP.CheckAppShare方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
            //商品是否参加三级分销(和设置佣金没关系)
            var cdQuery = (from cd in CommodityDistribution.ObjectSet()
                           where cd.Id == com.Id
                           select cd.Id).Any();
            commoditySDTO.IsDistribute = cdQuery;

            // 无属性商品不检查
            if (commoditySDTO.ComAttibutes != null && commoditySDTO.ComAttibutes.Count > 0)
            {
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                if (queryStock != null)
                {
                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                    foreach (var item in queryStock)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        tempStock.Price = item.Price;
                        tempStock.MarketPrice = item.MarketPrice;
                        tempStock.Stock = item.Stock;
                        tempStock.Id = item.Id;
                        tempStock.Duty = item.Duty ?? 0;
                        tempStock.ThumImg = item.ThumImg;
                        if (string.IsNullOrWhiteSpace(item.CarouselImgs))
                        {
                            tempStock.CarouselImgs = new string[0];
                        }
                        else
                        {
                            tempStock.CarouselImgs = item.CarouselImgs.Split(',');
                        }
                        tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityStocks.Add(tempStock);
                    }
                    commoditySDTO.CommodityStocks = commodityStocks;
                }
            }
            else
            {
                commoditySDTO.CommodityStocks = new List<CommodityAttrStockDTO>();
            }

            //相关商品
            commoditySDTO.RelationCommoditys = getRelationCommodity(com.Id);
            //图片
            List<CommodityPictureCDTO> productDetailPicList = new List<CommodityPictureCDTO>();
            var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == com.Id).OrderBy(n => n.Sort);
            var pictures = from p in productDetailsPictures
                           select new CommodityPictureCDTO
                           {
                               Sort = p.Sort,
                               PicturesPath = p.PicturesPath,
                           };
            productDetailPicList = pictures.ToList();
            commoditySDTO.Pictures = productDetailPicList;


            DateTime now = DateTime.Now;


            #region 众筹
            if (CustomConfig.CrowdfundingFlag)
            {
                //众筹状态
                var activeCrowdfundingCnt = Crowdfunding.ObjectSet().Count(c => c.AppId == com.AppId && c.State == 0 && c.StartTime <= now);
                if (activeCrowdfundingCnt > 0)
                    commoditySDTO.IsActiveCrowdfunding = true;
            }
            #endregion



            //是否收藏过
            appId = appId == Guid.Empty ? com.AppId : appId;
            int count = SetCollection.ObjectSet().Count(n => n.ColType == 1 && n.ColKey == com.Id && n.ChannelId == appId && n.UserId == userId);
            commoditySDTO.IsCollect = count > 0;
            commoditySDTO.CurrentTime = DateTime.Now;


            //部分包邮
            var partial = (from f in FreightTemplate.ObjectSet()
                           join fp in FreightPartialFree.ObjectSet() on f.Id equals fp.FreightTemplateId
                           where f.Id == com.FreightTemplateId && f.ExpressType == 2
                           select fp).ToList();

            List<Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO> data = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO>();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            System.Threading.Tasks.Parallel.Invoke(
                    () =>
                    {
                        //会员折扣信息
                        commoditySDTO.VipPromotion = AVMSV.GetVipIntensity(commoditySDTO.AppId, userId);
                        LogHelper.Debug(string.Format("进入AVMSV.GetVipIntensity方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {

                        commoditySDTO.HasReviewFunction = BACBP.CheckCommodityReview(appId);
                        LogHelper.Debug(string.Format("进入BACBP.CheckCommodityReview方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                        if (commoditySDTO.HasReviewFunction)
                        {
                            commoditySDTO.Score = SNSSV.GetComFirstScore(com.AppId, com.Id);
                            LogHelper.Debug(string.Format("进入SNSSV.GetComFirstScore方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                        }

                    }
                    , () =>
                    {
                        var appinfo = APPSV.GetAppNameIcon(commoditySDTO.AppId);
                        LogHelper.Debug(string.Format("进入APPSV.GetAppNameIcon方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                        commoditySDTO.AppName = appinfo.AppName;
                        commoditySDTO.AppIcon = appinfo.AppIcon;
                    }
                    , () =>
                    {
                        commoditySDTO.IsAppSet = ZPHSV.Instance.CheckIsAppInZPH(com.AppId);
                        LogHelper.Debug(string.Format("进入ZPHSV.CheckIsAppInZPH方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        commoditySDTO.EquipmentUrl = LVPSV.GetEquipmentUrlNew(com.AppId, ContextDTO.LoginUserID);
                        LogHelper.Debug(string.Format("进入LVPSV.GetEquipmentUrl方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        commoditySDTO.CloudviewUrl = ZPHSV.GetCloudViewUrl(com.AppId);
                        LogHelper.Debug(string.Format("进入ZPHSV.GetCloudViewUrl方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));

                    }
                    , () =>
                    {
                        //运费
                        List<TemplateCountDTO> templateCountList = new List<TemplateCountDTO>();
                        decimal realPrice = commoditySDTO.DiscountPrice > 0 ? commoditySDTO.DiscountPrice.Value : commoditySDTO.Price * commoditySDTO.Intensity.Value / 10;
                        templateCountList.Add(new TemplateCountDTO { Count = 1, CommodityId = com.Id, Price = decimal.Round(realPrice, 2, MidpointRounding.AwayFromZero) });
                        var frDTO = CalFreightMultiAppsByTextExt(freightTo, com.IsEnableSelfTake, templateCountList, null, null, null);
                        commoditySDTO.Freight = frDTO.ResultCode == 0 ? frDTO.Freight : 0;
                        commoditySDTO.FreightTo = freightTo;
                        commoditySDTO.IsSetMulti = IsSetMulti(com.FreightTemplateId);
                        //包邮条件的描述
                        commoditySDTO.FreeFreightStandard = buildFreightPartialFreeDescription(partial, com.PricingMethod);
                    }
                    , () =>
                    {
                        #region 油卡兑换券
                        var ykPersentList = YJBHelper.GetCommodityYouKaPercent(appId, new List<Guid> { com.Id });
                        if (ykPersentList != null && ykPersentList.Count == 1 && ykPersentList[0].CommodityId == com.Id && ykPersentList[0].YouKaPersent > 0)
                        {
                            var giveMoney = Math.Round(ykPersentList[0].YouKaPersent * com.Price / 100, 2);
                            commoditySDTO.YouKaInfo = new CommodityYouKaDTO(giveMoney, ykPersentList[0].YouKaPersent);
                        }
                        else
                        {
                            commoditySDTO.YouKaInfo = new CommodityYouKaDTO(0, 0);
                        }
                        #endregion
                    }
                    , () =>
                    {
                        commoditySDTO.YJBInfo = new YJB.Deploy.CustomDTO.OrderInsteadCashDTO();
                        List<string> lables = new List<string>();
                        Guid yjappid = YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                        // 易捷APP才显示
                        if (appId == yjappid)
                        {
                            // 判断是否易捷发货 若商品所属的APP是自营他配或自营自配，则显示易捷发货
                            MallApply yjApp;
                            int state = (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG;
                            yjApp = MallApply.ObjectSet().Where(_ => (_.EsAppId == yjappid && _.AppId == commoditySDTO.AppId && _.State.Value == state)).FirstOrDefault();
                            if (yjApp != null)
                            {
                                if (yjApp.Type == 0 || yjApp.Type == 2)
                                {
                                    lables.Add("易捷自营");
                                    Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                                    model.Title = "易捷自营";
                                    model.Content = "易捷自营";
                                    data.Add(model);
                                }
                                else
                                {
                                    lables.Add("商家入驻");
                                    Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                                    model.Title = "商家入驻";
                                    model.Content = "商家入驻";
                                    data.Add(model);
                                }

                                var yjb = YJBHelper.GetCommodityCashPercentWithoutUser(yjApp.EsAppId,
                                        new List<YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO> {
                                            new  YJB.Deploy.CustomDTO.OrderInsteadCashInputCommodityDTO {
                                                Id = com.Id, AppId = com.AppId, Number=1, Price = com.Price
                                            }
                                    });
                                commoditySDTO.YJBInfo.Enabled = false;
                                if (yjb.Enabled)
                                {
                                    commoditySDTO.YJBInfo.Enabled = yjb.InsteadCashCount > 0;
                                    commoditySDTO.YJBInfo.InsteadCashCount = yjb.InsteadCashCount;
                                    commoditySDTO.YJBInfo.InsteadCashAmount = yjb.InsteadCashAmount;
                                    commoditySDTO.YJBInfo.CommodityList = yjb.CommodityList;
                                }
                            }
                        }


                        // 七天无理由退货
                        if (com.IsAssurance == true)
                        {
                            lables.Add("七天无理由退");
                            Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                            model.Title = "七天无理由退";
                            model.Content = "七天无理由退";
                            data.Add(model);
                        }
                        if (com.IsReturns == true)
                        {
                            lables.Add("不支持退货");
                            ServiceSettingDTO model = new ServiceSettingDTO();
                            model.Title = "不支持退货";
                            model.Content = "不支持退货";
                            data.Add(model);
                        }

                        if (com.Isnsupport == false)
                        {
                            lables.Add("不支持7天无理由退货");
                            Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                            model.Title = "不支持7天无理由退货";
                            model.Content = "不支持7天无理由退货";
                            data.Add(model);
                        }
                        else
                        {
                            lables.Add("7天无理由退货");
                            Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO();
                            model.Title = "7天无理由退货";
                            model.Content = "7天无理由退货";
                            data.Add(model);
                        }


                        if (!string.IsNullOrEmpty(com.ServiceSettingId))
                        {
                            string[] str = com.ServiceSettingId.Split('|');
                            if (str != null && str.Count() > 0)
                            {
                                List<Guid> ids = new List<Guid>();
                                foreach (var item in str)
                                {
                                    Guid id = Guid.Parse(item);
                                    var entity = ServiceSettings.ObjectSet().FirstOrDefault(p => p.Id == id);
                                    if (entity != null)
                                    {
                                        ServiceSettingDTO model = new ServiceSettingDTO();
                                        model.Title = entity.Title;
                                        model.Content = entity.Content;
                                        data.Add(model);
                                    }
                                }
                            }

                        }
                        commoditySDTO.ServiceSettings = data;
                        // 根据当前用户位置，显示对应的满包邮条件；
                        // 若获取不到当前用户的地理位置，取默认运费的满包邮条件，即显示商品运费金额对应的满包邮条件
                        var freightTemplate = FreightTemplate.ObjectSet().Where(_ => _.Id == com.FreightTemplateId).FirstOrDefault();
                        if (freightTemplate != null)
                        {
                            if (freightTemplate.ExpressType == 1)
                            {
                                lables.Add("全国包邮");
                                commoditySDTO.PostAge = "全国包邮";
                            }
                            else if (freightTemplate.ExpressType == 2)
                            {
                                //var ip = System.Web.HttpContext.Current.Request.UserHostAddress;
                                //var region = RedisHelper.GetHashValue<string>(RedisKeyConst.IpRegion, ip);
                                //if (region == null)
                                //{
                                //    region = IPHelper.GetRegion(ip);
                                //    if (region != null)
                                //    {
                                //        RedisHelper.AddHash(RedisKeyConst.IpRegion, ip, region);
                                //    }
                                //}
                                if (!string.IsNullOrWhiteSpace(freightTo))
                                {
                                    var pcode = ProvinceCityHelper.GetProvinceCodeByName(freightTo);
                                    //LogHelper.Info("ProvinceCityHelper.GetProvinceCodeByName(" + freightTo + ")=>" + pcode);
                                    if (!string.IsNullOrEmpty(pcode))
                                    {
                                        var freightdetails = FreightPartialFree.ObjectSet().Where(_ => _.FreightTemplateId == freightTemplate.Id && _.DestinationCodes.Contains(pcode)).FirstOrDefault();
                                        if (freightdetails != null)
                                        {
                                            if (freightdetails.FreeCount > 0)
                                            {
                                                lables.Add("满" + Math.Floor(freightdetails.FreeCount) + "件包邮");
                                                commoditySDTO.PostAge = "满" + Math.Floor(freightdetails.FreeCount) + "件包邮";
                                            }
                                            else
                                            {
                                                lables.Add("满" + Math.Floor(freightdetails.FreePrice) + "元包邮");
                                                commoditySDTO.PostAge = "满" + Math.Floor(freightdetails.FreePrice) + "元包邮";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        commoditySDTO.Labels = lables.ToArray();

                        commoditySDTO.Coupons = new List<string>().ToArray();
                        // 优惠券
                        var applist = new List<Guid> { commoditySDTO.AppId };
                        var coupons = CouponSV.Instance.GetUsableCouponsTemplateList(
                                new Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplateUsableRequestDTO
                                {
                                    CurrentPage = 0,
                                    PageSize = 20,
                                    UserId = userId,
                                    AppList = applist,
                                    CommodityId = commoditySDTO.Id
                                }, true);
                        if (coupons != null && coupons.RecordCollection != null)
                        {
                            List<string> strcoupons = new List<string>();
                            foreach (var item in coupons.RecordCollection)
                            {
                                if (item.LimitCondition == 0)
                                {
                                    strcoupons.Add("减" + item.Cash + "元");
                                }
                                else
                                {

                                    strcoupons.Add("满" + item.LimitCondition + "元减" + item.Cash + "元");
                                }
                            }
                            commoditySDTO.Coupons = strcoupons.ToArray();
                            commoditySDTO.CouponList = coupons.RecordCollection.ToList();
                            LogHelper.Debug(string.Format("进入Commodity.Lables方法，商品id：{1}，结束时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), com.Id));
                        }
                    }
                    , () =>
                    {
                        #region 获取活动sku属性集合
                        #endregion
                    }
                    , () =>
                    {
                        #region 根据商品ID获取商品参与的优惠套装

                        var setMealActivitys = ZPHSV.Instance.GetSetMealActivitysByCommodityId(com.Id, appId, true);
                        if (setMealActivitys.Count > 0)
                        {
                            commoditySDTO.MealActivityInfo = "最高省" + setMealActivitys[0].PreferentialPrice + "元";
                        }

                        #endregion
                    }
                    , () =>
                    {
                        #region 赠品信息
                        var presents = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == commoditySDTO.Id)
                            .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                                pp => pp.PresentPromotionId, ppc => ppc.Id,
                                (c, p) => new { Commodity = c, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime })
                            .ToList();
                        if (presents.Count > 0)
                        {
                            commoditySDTO.Present = new CommodiyPresentDTO();
                            var firstPromotion = presents.First();
                            commoditySDTO.Present.Limit = firstPromotion.Limit ?? 0;
                            if (commoditySDTO.Present.Limit == 0)
                            {
                                commoditySDTO.Present.Limit = 1;
                            }
                            commoditySDTO.Present.BeginTime = firstPromotion.BeginTime;
                            commoditySDTO.Present.EndTime = firstPromotion.EndTime;
                            commoditySDTO.Present.CommodityStockIds = presents.Where(_ => _.Commodity.CommoditySKUId != Guid.Empty).Select(_ => _.Commodity.CommoditySKUId).ToList();

                            commoditySDTO.Present.IsAll = true;
                            commoditySDTO.Present.Title = "购买" + (commoditySDTO.Present.Limit > 1 ? commoditySDTO.Present.Limit + "件" : "") + "即送超值赠品 （赠完即止）";
                            if (commoditySDTO.CommodityStocks != null && commoditySDTO.CommodityStocks.Count > 0)
                            {
                                bool isAll = true;
                                var titles = new List<string>();
                                foreach (var item in commoditySDTO.CommodityStocks)
                                {
                                    if (commoditySDTO.Present.CommodityStockIds.Any(_ => _ == item.Id))
                                    {
                                        titles.Add(string.Join("，", item.ComAttribute.Select(_ => _.SecondAttribute)));
                                    }
                                    else
                                    {
                                        isAll = false;
                                    }
                                }
                                if (!isAll)
                                {
                                    commoditySDTO.Present.IsAll = false;
                                    commoditySDTO.Present.Title = "购买 “" + string.Join("”、 “", titles) + "” " + (commoditySDTO.Present.Limit > 1 ? commoditySDTO.Present.Limit + "件" : "") + "送超值赠品 （赠完即止）";
                                }
                            }

                            var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == firstPromotion.PromotionId).ToList();
                            var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                            var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                            commoditySDTO.Present.Items = new List<CommodiyPresentItem>();
                            foreach (var item in gifts)
                            {
                                var tempCom = GetCommodity(commoditySDTO.AppId, item.CommodityId);
                                var commodiyPresentItem = new CommodiyPresentItem();
                                commodiyPresentItem.Id = item.CommodityId;
                                commodiyPresentItem.StockId = item.CommoditySKUId;
                                commodiyPresentItem.Name = item.CommodityName;
                                commodiyPresentItem.Pic = tempCom.PicturesPath;
                                commodiyPresentItem.Number = item.Number;

                                if (item.CommoditySKUId != Guid.Empty)
                                {
                                    var giftCommodityStock = giftCommodityStocks.Find(_ => _.Id == item.CommoditySKUId);
                                    if (giftCommodityStock != null && giftCommodityStock.Stock > 0)
                                    {
                                        commodiyPresentItem.SKU = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(giftCommodityStock.ComAttribute);
                                        commodiyPresentItem.Stock = giftCommodityStock.Stock;
                                        commoditySDTO.Present.Items.Add(commodiyPresentItem);
                                    }
                                }
                                else
                                {
                                    if (tempCom.Stock > 0)
                                    {
                                        commodiyPresentItem.Stock = tempCom.Stock;
                                        commoditySDTO.Present.Items.Add(commodiyPresentItem);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    , () =>
                    {
                        #region 获取金采团购活动数据

                        if (jcActivityId != null)
                        {
                            LogHelper.Debug(string.Format("进入ZPHSV.Instance.GetItemsListByActivityId方法，jcActivityId：{0}", jcActivityId));
                            var itemsList = ZPHSV.Instance.GetItemsListByActivityId((Guid)jcActivityId).Data.Where(t => t.ComdtyId == commoditySDTO.Id);
                            List<Deploy.CustomDTO.JCActivityItemsListCDTO> jcActivityItemsListCdtos =
                                itemsList.Select(jcActivityItemsListCDTO => new Deploy.CustomDTO.JCActivityItemsListCDTO
                                {
                                    id = jcActivityItemsListCDTO.id,
                                    JCActivityId = jcActivityItemsListCDTO.JCActivityId,
                                    ComdtyId = jcActivityItemsListCDTO.ComdtyId,
                                    ComdtyStockId = jcActivityItemsListCDTO.ComdtyStockId,
                                    GiftGardScale = jcActivityItemsListCDTO.GiftGardScale,
                                    subId = jcActivityItemsListCDTO.subId,
                                    subTime = jcActivityItemsListCDTO.subTime,
                                    modifiedOn = jcActivityItemsListCDTO.modifiedOn,
                                    GroupPrice = jcActivityItemsListCDTO.GroupPrice
                                }).ToList();

                            commoditySDTO.JCActivityItemsListCdtos = jcActivityItemsListCdtos;
                            if (jcActivityItemsListCdtos.Count > 0)
                            {
                                commoditySDTO.MinJcSkuPrice = jcActivityItemsListCdtos.Min(t => t.GroupPrice);
                                commoditySDTO.MaxJcSkuPrice = jcActivityItemsListCdtos.Max(t => t.GroupPrice);

                                //commoditySDTO.DiscountPrice = commoditySDTO.MaxJcSkuPrice;
                            }
                        }

                        #endregion
                    }
                );
            timer.Stop();
            LogHelper.Debug(string.Format("并发执行耗时: {0}", timer.ElapsedMilliseconds));

            #region 易捷北京促销活动时不能使用优惠券、易捷币、加油卡兑换券

            if (appId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && ((commoditySDTO.PromotionId.HasValue) || commoditySDTO.VipPromotion.DiscountPrice > 0))
            {
                commoditySDTO.YouKaInfo.GiveMoney = 0;
                commoditySDTO.Coupons = new List<string>().ToArray();
                commoditySDTO.CouponList = new List<CouponTemplatDetailDTO>();
                commoditySDTO.YJBInfo.Enabled = false;
            }

            #endregion

            return commoditySDTO;
        }

        /// <summary>
        /// 获取活动状态
        /// </summary>
        /// <param name="promotion">活动信息</param>
        /// <returns>活动状态 0：没有活动或已失效 ,1:预约预售进行中，2：等待抢购：3：活动进行中，4：活动已结束</returns>
        private static int GetPromotionState(TodayPromotionDTO promotion)
        {
            int promotionState = 0;
            DateTime now = DateTime.Now;
            if (promotion.EndTime <= now)
            {
                //已结束
                promotionState = 4;
            }
            else if (promotion.StartTime <= now)
            {
                //进行中
                promotionState = 3;
            }
            else if (!promotion.PresellEndTime.HasValue || promotion.PresellEndTime < now)
            {
                //等待抢购
                promotionState = 2;
            }
            else if (promotion.PresellStartTime <= now)
            {
                //预约预售进行中
                promotionState = 1;
            }
            return promotionState;
        }

        private List<string> buildFreightPartialFreeDescription(List<FreightPartialFree> list, int type)
        {
            List<string> result = new List<string>();

            if (list == null || list.Count < 1)
            {
                return result;
            }

            //List<string> province = new List<string>() { "安徽省", "澳门特别行政区", "北京市", "福建省", "甘肃省", "广东省", "广西壮族自治区", "贵州省", "海南省", "河北省", "河南省", "黑龙江省", "湖北省", "湖南省", "吉林省", "江苏省", "江西省", "辽宁省", "内蒙古自治区", "宁夏回族自治区", "青海省", "山东省", "山西省", "陕西省", "上海市", "四川省", "台湾省", "天津市", "西藏自治区", "香港特别行政区", "新疆维吾尔自治区", "云南省", "浙江省", "重庆市" };

            Dictionary<string, string> provinceArea = ProvinceCityHelper.GetAllProvince().ToDictionary(t => t.AreaCode, y => y.Name);

            //按件计
            if (type == 0)
            {
                foreach (FreightPartialFree item in list)
                {
                    string area = string.Empty;
                    string[] destinationCodesList = item.DestinationCodes.Trim().Replace("，", ",").Replace(";", ",").Replace("；", ",").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    //当设置的地区小于等于5个时全部显示
                    if (destinationCodesList.Count() <= 5)
                    {
                        var proviceNames = (from f in destinationCodesList
                                            join p in provinceArea on f equals p.Key
                                            select p.Value).ToList();

                        area = string.Join("、", proviceNames) + "包邮";
                    }
                    //当全国包邮时
                    else if (provinceArea.Count() - destinationCodesList.Count() == 0)
                    {
                        area = "全国包邮";
                    }
                    //当未选择的地区小于等于5个时，显示全国包邮，xxx、xxx、除外
                    else if (provinceArea.Count() - destinationCodesList.Count() <= 5)
                    {
                        var exceptCodes = provinceArea.Keys.Except(destinationCodesList).ToList();
                        var proviceNames = (from e in exceptCodes
                                            join p in provinceArea on e equals p.Key
                                            select p.Value).ToList();

                        string unSetArea = string.Join("、", proviceNames);
                        area = "全国包邮，" + unSetArea + "除外";
                    }
                    else if (destinationCodesList.Count() > 5)
                    {
                        var showCodes = destinationCodesList.Take(3).ToList();
                        var proviceNames = (from s in showCodes
                                            join p in provinceArea on s equals p.Key
                                            select p.Value).ToList();

                        string threeArea = string.Join("、", proviceNames);
                        area = threeArea + "......等" + destinationCodesList.Count() + "个地区包邮";
                    }

                    //件数
                    if (item.FreeType == 0)
                    {

                        string temp = string.Format("满{0}件，{1}", item.FreeCount.ToString("#0.#####"), area);
                        result.Add(temp);
                    }
                    //金额
                    else if (item.FreeType == 1)
                    {
                        string temp = string.Format("满{0}元，{1}", item.FreePrice.ToString("#0.#####"), area);
                        result.Add(temp);
                    }
                }
            }
            //按重量计
            else if (type == 1)
            {
                foreach (FreightPartialFree item in list)
                {
                    string area = string.Empty;
                    string[] destinationCodesList = item.DestinationCodes.Trim().Replace("，", ",").Replace(";", ",").Replace("；", ",").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    //当设置的地区小于等于5个时全部显示
                    if (destinationCodesList.Count() <= 5)
                    {
                        var proviceNames = (from f in destinationCodesList
                                            join p in provinceArea on f equals p.Key
                                            select p.Value).ToList();

                        area = string.Join("、", proviceNames) + "包邮";
                    }
                    //当全国包邮时
                    else if (provinceArea.Count() - destinationCodesList.Count() == 0)
                    {
                        area = "全国包邮";
                    }
                    //当未选择的地区小于等于5个时，显示全国包邮，xxx、xxx、除外
                    else if (provinceArea.Count() - destinationCodesList.Count() <= 5)
                    {
                        var exceptCodes = provinceArea.Keys.Except(destinationCodesList).ToList();
                        var proviceNames = (from e in exceptCodes
                                            join p in provinceArea on e equals p.Key
                                            select p.Value).ToList();

                        string unSetArea = string.Join("、", proviceNames);
                        area = "全国包邮，" + unSetArea + "除外";
                    }
                    else if (destinationCodesList.Count() > 5)
                    {
                        var showCodes = destinationCodesList.Take(3).ToList();
                        var proviceNames = (from s in showCodes
                                            join p in provinceArea on s equals p.Key
                                            select p.Value).ToList();

                        string threeArea = string.Join("、", proviceNames);
                        area = threeArea + "......等" + destinationCodesList.Count() + "个地区包邮";
                    }

                    //重量
                    if (item.FreeType == 0)
                    {
                        string temp = string.Format("在{0}Kg内，{1}", item.FreeCount.ToString("#0.#####"), area);
                        result.Add(temp);
                    }
                    //金额
                    else if (item.FreeType == 1)
                    {
                        string temp = string.Format("满{0}元，{1}", item.FreePrice.ToString("#0.#####"), area);
                        result.Add(temp);
                    }
                }
            }
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
        public ResultDTO SaveMyPresellComdtyZPHExt(Guid outPromotionId, Guid userId, string verifyCode, Guid esAppId, Guid commodityId, Guid commodityStockId)
        {
            MyPresellComdty2CDTO presellDTO = new MyPresellComdty2CDTO()
            {
                id = Guid.NewGuid(),
                isFlashSale = false,
                presellComdtyId = outPromotionId,
                userId = userId,
                verifyCode = verifyCode,
                appId = esAppId,
                commodityId = commodityId,
                commodityStockId = commodityStockId
            };
            try
            {
                //0，成功，1 已预约过，2 失败 
                var saveResult = Jinher.AMP.BTP.TPS.ZPHSV.Instance.SaveMyPresellComdty2(presellDTO);
                if (saveResult == null)
                    return new ResultDTO() { ResultCode = 2, Message = "预约失败" };

                return new ResultDTO() { ResultCode = saveResult.Code, Message = saveResult.Message };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.SaveMyPresellComdtyZPHExt。outPromotionId：{0}。userId：{1}。verifyCode：{2}", outPromotionId, userId, verifyCode), ex);
                return new ResultDTO() { ResultCode = 1, Message = "预约失败" };
            }

        }
        public ResultDTO<byte[]> GetVerifyCodeZPHExt()
        {
            try
            {
                var aa = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetVerifyCode();
                return new ResultDTO<byte[]>() { ResultCode = 0, Data = aa.Data };
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommoditySV.GetVerifyCodeZPHExt。}", ex);
                return new ResultDTO<byte[]>() { ResultCode = 1, Message = "预约失败" };
            }


        }
        public ResultDTO SendNotificationsZPHExt(Guid commodityId, Guid outPromotionId, Guid esAppId)
        {
            try
            {

                int min = 5;
                SeckillConfigCDTO seckillConfig = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetSeckillConfig(esAppId);
                if (seckillConfig != null)
                {
                    min = seckillConfig.RemindMin;
                }

                var com = Commodity.ObjectSet().FirstOrDefault(c => c.Id == commodityId && !c.IsDel && c.State == 0 && c.CommodityType == 0);
                if (com == null)
                    return new ResultDTO { ResultCode = 1, Message = "商品不存在" };
                var comName = com.Name;

                var pro = Promotion.ObjectSet().FirstOrDefault(c => c.OutsideId == outPromotionId && !c.IsDel && c.IsEnable);
                if (pro == null)
                    return new ResultDTO { ResultCode = 1, Message = "活动不存在" };
                var startTime = pro.StartTime;


                Jinher.AMP.ZPH.Deploy.CustomDTO.NoticeCDTO dto = new NoticeCDTO
                {
                    userId = ContextDTO.LoginUserID,
                    subId = ContextDTO.LoginUserID,
                    bizId = outPromotionId,
                    subTime = DateTime.Now,
                    bizRemindType = Jinher.AMP.ZPH.Deploy.Enum.BizRemindType.Seckill,
                    remindTime = startTime.AddMinutes(-min),
                    notification = "商品：" + comName + "，还有" + min + "分钟就开始秒杀啦！",
                    appId = esAppId

                };
                var sendResult = Jinher.AMP.BTP.TPS.ZPHSV.Instance.SendNotifications(dto);

                if (sendResult == null)
                    return new ResultDTO { ResultCode = 1, Message = "设置失败" };
                if (sendResult.Code == 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "成功" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = sendResult.Message };
                }
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("CommoditySV.SendNotificationsZPHExt。commodityId：{0}。outPromotionId：{1}", commodityId, outPromotionId), ex);
                return new ResultDTO() { ResultCode = 1, Message = "失败" };
            }
        }
        /// <summary>
        /// 添加和取消到货提醒
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <param name="Iscancel"></param>
        /// <returns></returns>
        public ResultDTO<NotificationsDTO> SaveStockNotificationsZPHExt(Guid commodityId, Guid userId, Guid esAppId, int noticeType, bool Iscancel)
        {
            try
            {
                var com = Commodity.ObjectSet().FirstOrDefault(c => c.Id == commodityId && !c.IsDel && c.State == 0);
                if (com == null)
                    return new ResultDTO<NotificationsDTO> { ResultCode = 1, Message = "商品不存在" };
                if (com.Stock > 0)
                    return new ResultDTO<NotificationsDTO> { ResultCode = 1, Message = "商品库存充足" };
                var comName = com.Name;
                var remindTime = DateTime.Now.AddYears(3);
                var RemindType = Jinher.AMP.ZPH.Deploy.Enum.BizRemindType.HaveStock;
                if (noticeType == 4)
                {
                    RemindType = Jinher.AMP.ZPH.Deploy.Enum.BizRemindType.SMS;
                }
                string notificeInfo = "您关注的" + comName + "商品已到货，快去下单吧~";
                if (noticeType == 4)
                {
                    notificeInfo = comName;//短信提醒只保存商品名称
                }
                Jinher.AMP.ZPH.Deploy.CustomDTO.NoticeCDTO dto = new NoticeCDTO
                {
                    userId = userId,
                    subId = userId,
                    bizId = com.Id,
                    subTime = DateTime.Now,
                    Iscancel = false,
                    bizRemindType = RemindType,
                    remindTime = remindTime,
                    notification = notificeInfo,
                    appId = esAppId
                };
                var sendResult = ZPHSV.SaveNotifications(dto);

                if (sendResult == null)
                {
                    NotificationsDTO Notice = new NotificationsDTO()
                    {
                        state = 0,
                        Content = new EnumHelper().GetDescription(NoticeTypeEnum.到货提醒)
                    };
                    return new ResultDTO<NotificationsDTO> { ResultCode = 1, isSuccess = false, Message = "设置失败", Data = Notice };
                }
                if (sendResult.Code == 0)
                {
                    var NoticeType = GetNoticeType(sendResult.Data);
                    NotificationsDTO Notice = new NotificationsDTO()
                    {
                        state = sendResult.Data.BizRemindType.Value,
                        Content = new EnumHelper().GetDescription(NoticeType)
                    };
                    return new ResultDTO<NotificationsDTO> { ResultCode = 0, isSuccess = true, Message = "设置成功", Data = Notice };
                }
                else
                {
                    var NoticeType = GetNoticeType(sendResult.Data);
                    NotificationsDTO Notices = new NotificationsDTO()
                    {
                        state = sendResult.Data.BizRemindType.Value,
                        Content = new EnumHelper().GetDescription(NoticeType)
                    };
                    return new ResultDTO<NotificationsDTO> { ResultCode = 1, isSuccess = false, Message = sendResult.Message, Data = Notices };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.SendNotificationsZPHExt。commodityId：{0}。", commodityId), ex);
                return new ResultDTO<NotificationsDTO>() { ResultCode = 1, isSuccess = false, Message = "失败" };
            }
        }
        /// <summary>
        /// 校验商品信息(包含预约校验)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="commodityIdsList">商品list</param>
        /// <returns></returns>
        [Obsolete("已废弃，请调用CheckCommodity", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityWithPreSellExt(Guid userId, List<CommodityIdAndStockId> commodityIdsList)
        {
            CheckCommodityParam ccp = new CheckCommodityParam();
            ccp.CommodityIdsList = commodityIdsList;
            ccp.DiygId = Guid.Empty;
            ccp.PromotionType = -1;
            ccp.UserID = userId;
            var ccList = CheckCommodityV3Ext(ccp);
            return ccList;
        }
        private class TempCommodity
        {
            public Commodity Com { get; set; }
            public decimal? newPrice { get; set; }
            public Guid? BrandId { get; set; }
            public Guid? CategoryId { get; set; }
        }
        /// <summary>
        /// 清空商品信息的缓存        
        /// </summary> 
        public void RemoveCacheExt()
        {
            try
            {
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache("G_CommodityDetail", "BTPCache", CacheTypeEnum.redisSS);
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommoditySV.RemoveCacheExt。清理商品缓存异常。", ex);
            }
        }

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerIDExt(CommoditySearchDTO commoditySearch)
        {
            if (commoditySearch == null) return null;
            if (commoditySearch.appId == Guid.Empty) return null;
            if (commoditySearch.pageIndex < 1) return null;
            if (commoditySearch.pageSize < 1) return null;

            var commodityDTO = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.CommodityType == 0 && n.AppId.Equals(commoditySearch.appId) && n.State == 0).OrderByDescending(n => n.SubTime).Skip((commoditySearch.pageIndex - 1) * commoditySearch.pageSize).Take(commoditySearch.pageSize);

            #region 增加商品查询条件---分类、毛利率区间，价格区间

            CommodityListInputDTO intDot = new CommodityListInputDTO()
            {
                AppId = commoditySearch.appId,
                PageIndex = commoditySearch.pageIndex,
                PageSize = commoditySearch.pageSize,
                CommodityName = commoditySearch.commodityName,

                MaxInterestRate = string.IsNullOrWhiteSpace(commoditySearch.MaxInterestRate) ? "" : commoditySearch.MaxInterestRate,
                MinInterestRate = string.IsNullOrWhiteSpace(commoditySearch.MinInterestRate) ? "" : commoditySearch.MinInterestRate,
                Categorys = string.IsNullOrWhiteSpace(commoditySearch.CategorysIdList) ? "" : commoditySearch.CategorysIdList,
                MaxPrice = string.IsNullOrWhiteSpace(commoditySearch.MaxPrice) ? "" : commoditySearch.MaxPrice,
                MinPrice = string.IsNullOrWhiteSpace(commoditySearch.MinPrice) ? "" : commoditySearch.MinPrice
            };

            commodityDTO = AddCommoditySelectWhere(intDot, commodityDTO);
            if (!string.IsNullOrWhiteSpace(commoditySearch.commodityName))
            {
                commodityDTO = commodityDTO.Where(p => p.Name.Contains(commoditySearch.commodityName));
            }
            #endregion




            var result = from c in commodityDTO
                         select new CommodityDTO
                         {
                             AppId = c.AppId,
                             Code = c.Code,
                             CategoryName = c.CategoryName,
                             ComAttribute = c.ComAttribute,
                             Description = c.Description,
                             GroundTime = c.GroundTime,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             Name = c.Name,
                             No_Code = c.No_Code,
                             No_Number = c.No_Number,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             State = c.State,
                             MarketPrice = c.MarketPrice,
                             Stock = c.Stock,
                             TotalReview = c.TotalReview,
                             TotalCollection = c.TotalCollection,
                             Salesvolume = c.Salesvolume,
                             IsEnableSelfTake = c.IsEnableSelfTake,
                             CostPrice = c.CostPrice
                         };
            return result.ToList();

        }

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityForCouponExt(CommoditySearchDTO commoditySearch, out int count)
        {
            count = 0;
            if (commoditySearch == null) return null;
            if (commoditySearch.appId == Guid.Empty) return null;
            if (commoditySearch.pageIndex < 1) return null;
            if (commoditySearch.pageSize < 1) return null;

            var temp = Commodity.ObjectSet().Where(n => !n.IsDel && n.CommodityType == 0 && n.AppId.Equals(commoditySearch.appId) && n.State == 0 && n.Stock > 0);
            if (!string.IsNullOrWhiteSpace(commoditySearch.commodityName))
            {
                temp = temp.Where(p => p.Name.Contains(commoditySearch.commodityName));
            }
            count = temp.Count();
            temp = temp.OrderByDescending(t => t.SubTime).Skip((commoditySearch.pageIndex - 1) * commoditySearch.pageSize).Take(commoditySearch.pageSize);

            var result = from c in temp
                         select new CommodityDTO
                         {
                             AppId = c.AppId,
                             Code = c.Code,
                             CategoryName = c.CategoryName,
                             ComAttribute = c.ComAttribute,
                             Description = c.Description,
                             GroundTime = c.GroundTime,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             Name = c.Name,
                             No_Code = c.No_Code,
                             No_Number = c.No_Number,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             State = c.State,
                             MarketPrice = c.MarketPrice,
                             Stock = c.Stock,
                             TotalReview = c.TotalReview,
                             TotalCollection = c.TotalCollection,
                             Salesvolume = c.Salesvolume,
                             IsEnableSelfTake = c.IsEnableSelfTake,
                             CostPrice = c.CostPrice
                         };
            return result.ToList();
        }

        /// <summary>
        /// 多app运费计算
        /// </summary>
        /// <param name="freightTo">运送到</param>
        /// <param name="isSelfTake">是否自提</param>
        /// <param name="templateCounts">模板数据集合</param>
        /// <returns>运费计算结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalFreightMultiAppsExt(string freightTo, int isSelfTake, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> templateCounts, Dictionary<Guid, decimal> coupons, Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbInfo, List<Guid> yjCouponIds)
        {
            if (coupons == null) coupons = new Dictionary<Guid, decimal>();
            Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO fResult = new Deploy.CustomDTO.FreighMultiAppResultDTO();
            decimal totalFDecimal = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(freightTo))
                {
                    fResult.Message = "运送的目的地为空";
                    fResult.ResultCode = 3;
                    fResult.Freight = 0;
                    return fResult;
                }
                if (templateCounts == null || templateCounts.Count == 0)
                {
                    fResult.Message = "模板数据集合为空或不存在";
                    fResult.ResultCode = 3;
                    fResult.Freight = 0;
                    return fResult;
                }
                templateCounts.RemoveAll(delegate (Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO obj)
                {
                    if (obj == null)
                    {
                        return true;
                    }
                    return false;
                });
                if (templateCounts.Count == 0)
                {
                    fResult.Message = "模板数据集合为空或不存在";
                    fResult.ResultCode = 3;
                    fResult.Freight = 0;
                    return fResult;
                }
                if (isSelfTake != 0 && isSelfTake != 1)
                {
                    fResult.Message = "是否自提参数范围错误（0：不自提；1：自提）";
                    fResult.ResultCode = 3;
                    fResult.Freight = 0;
                    return fResult;
                }

                freightTo = freightTo.Trim();

                //所有商品ID
                List<Guid> ids = templateCounts.Select(t => t.CommodityId).Distinct().ToList();

                //各厂家商品
                var appIdsAndCommodityIds = Commodity.ObjectSet().Where(t => ids.Contains(t.Id) && t.CommodityType == 0).GroupBy(t => t.AppId).ToDictionary(x => x.Key, y => y.Select(t => t.Id).ToList());


                List<AppFreight> appFreightlist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.AppFreight>();

                List<Jinher.AMP.YJB.Deploy.CustomDTO.YJCouponWithCommodityInfo> yjCouponInfo = null;
                //if (yjCouponIds != null && yjCouponIds.Count > 0)
                //{
                //    var commodities = new List<YJB.Deploy.CustomDTO.YJCouponWithCommodityInfoInputCommodity>();
                //    foreach (var ck in appIdsAndCommodityIds.Keys)
                //    {
                //        foreach (var c in appIdsAndCommodityIds[ck])
                //        {
                //            var _com = templateCounts.FirstOrDefault(_ => _.CommodityId == c);
                //            if (_com != null)
                //            {
                //                commodities.Add(new YJB.Deploy.CustomDTO.YJCouponWithCommodityInfoInputCommodity { CommodityId = _com.CommodityId, Price = _com.Price, AppId = ck, });
                //            }
                //        }
                //    }

                //    yjCouponInfo = YJBSV.GetMyYJCouponInfoWithApp(new YJB.Deploy.CustomDTO.YJCouponWithCommodityInfoInput
                //    {
                //        UserCouponIds = yjCouponIds,
                //        Commodities = commodities
                //    });
                //}

                if (isSelfTake == 0)
                {
                    //不同厂家分别计算运费
                    foreach (Guid appId in appIdsAndCommodityIds.Keys)
                    {
                        decimal couponPrice = 0;
                        if (coupons.ContainsKey(appId))
                        {
                            couponPrice = coupons[appId];
                        }
                        decimal yjbPrice = 0;
                        decimal yjCouponPrice = 0;
                        if (yjbInfo != null)
                        {
                            yjbPrice = yjbInfo.CommodityList.Where(_ => _.AppId == appId).Sum(_ => _.InsteadCashAmount);
                        }
                        if (yjCouponInfo != null)
                        {
                            yjCouponPrice = yjCouponInfo.Where(_ => _.AppId == appId).Sum(_ => _.CouponPrice);
                        }

                        List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> newTemplateCounts = new List<TemplateCountDTO>();
                        newTemplateCounts = templateCounts.Where(t => appIdsAndCommodityIds[appId].Contains(t.CommodityId)).ToList();

                        //计算运费
                        LogHelper.Info("计算运费: yjbPrice=" + yjbPrice + "|yjCouponPrice=" + yjCouponPrice + "|yjCouponInfo=" + JsonHelper.JsonSerializer(yjCouponInfo) + "|appId=" + appId);
                        //decimal fDecimal = CalNewFreight(freightTo, newTemplateCounts, couponPrice, yjbPrice, yjCouponPrice);
                        // 优惠金额可算包邮金额--20180622
                        decimal fDecimal = CalNewFreight(freightTo, newTemplateCounts, 0, 0, 0);

                        var appFreight = new AppFreight
                        {
                            AppId = appId,
                            Freight = fDecimal
                        };
                        if (fDecimal > 0)
                        {

                        }
                        else
                        {
                            var newTemplateCount = newTemplateCounts.OrderByDescending(_ => _.Price).FirstOrDefault();
                            var com = Commodity.FindByID(newTemplateCount.CommodityId);
                            var partial = (from f in FreightTemplate.ObjectSet()
                                           join fp in FreightPartialFree.ObjectSet() on f.Id equals fp.FreightTemplateId
                                           where f.Id == com.FreightTemplateId && f.ExpressType == 2
                                           select fp).ToList(); appFreight.FreeFreightStandard = string.Join(",", buildFreightPartialFreeDescription(partial, com.PricingMethod));
                        }
                        appFreightlist.Add(appFreight);
                        totalFDecimal += fDecimal;
                    }
                }
                else
                {
                    foreach (Guid appId in appIdsAndCommodityIds.Keys)
                    {
                        AppFreight model = new AppFreight()
                        {
                            AppId = appId,
                            Freight = 0
                        };
                    }
                    totalFDecimal = 0;
                }

                fResult.Message = "Success";
                fResult.ResultCode = 0;
                fResult.Freight = totalFDecimal;
                fResult.AppFreight = appFreightlist;

                return fResult;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("运费计算错误。freightTo：{0}。isSelfTake：{1}。templateCounts：{2}", freightTo, isSelfTake, templateCounts), ex);
                fResult.Message = "运费计算错误";
                fResult.ResultCode = 1;
                fResult.Freight = 0;
                return fResult;
            }
        }

        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByIdsWithPreSellExt(List<Guid> commodityIds, bool isDefaultOrder = false)
        {
            LogHelper.Debug("开始进入GetCommodityByIdsWithPreSellExt方法，commodityIds：" + JsonHelper.JsSerializer(commodityIds));
            try
            {
                if (commodityIds == null || !commodityIds.Any())
                    return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
                var skuCommodityStockIds = new List<Guid>();
                var now = DateTime.Now;
                var commodityList = (from c in Commodity.ObjectSet()
                                     where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                           && commodityIds.Contains(c.Id)
                                     orderby c.State, c.Salesvolume descending, c.SubTime descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         AppId = c.AppId,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3,
                                         Salesvolume = c.Salesvolume,
                                         OrderWeight = c.OrderWeight
                                     }).ToList();
                if (isDefaultOrder && commodityList.Any())
                {
                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> tmplist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
                    for (int i = 0; i < commodityIds.Count; i++)
                    {
                        var tmp = commodityList.FirstOrDefault(c => c.Id == commodityIds[i]);
                        if (tmp != null && tmp.State == 0)
                            tmplist.Add(tmp);
                    }
                    commodityList = tmplist;
                }

                if (commodityList.Any())
                {
                    List<Guid> appIds = commodityList.Select(c => c.AppId).Distinct().ToList();
                    try
                    {

                        Dictionary<Guid, string> appList = APPSV.GetAppNameListByIds(appIds);
                        if (appList.Any())
                        {
                            foreach (var commoditySdto in commodityList)
                            {
                                if (appList.ContainsKey(commoditySdto.AppId))
                                {
                                    var app = appList[commoditySdto.AppId];
                                    if (!String.IsNullOrEmpty(app))
                                        commoditySdto.AppName = app;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("CommoditySV.GetCommodityByIdsExt异常：调用APPSV.GetAppNameListByIds异常,商品id列表：{0}，\r\n  StackTrace:\r\n{1} ", JsonHelper.JsonSerializer(commodityIds), ex));
                    }
                    #region 众筹
                    if (CustomConfig.CrowdfundingFlag)
                    {
                        var cfAppIds = Crowdfunding.ObjectSet().Where(c => appIds.Contains(c.AppId) && c.StartTime < now && c.State == 0).Select(m => m.AppId).ToList();
                        if (cfAppIds.Any())
                        {
                            foreach (var commodityListCdto in commodityList)
                            {
                                if (cfAppIds.Any(c => c == commodityListCdto.AppId))
                                    commodityListCdto.IsActiveCrowdfunding = true;
                            }
                        }
                    }
                    #endregion


                    //读今日折扣表
                    try
                    {
                        var todayPromotion = (from p in PromotionItems.ObjectSet()
                                              join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                              where !pro.IsDel && pro.IsEnable && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now) && pro.PromotionType != 3
                                              select new TodayPromotionDTO
                                              {
                                                  PromotionId = p.PromotionId,
                                                  CommodityId = p.CommodityId,
                                                  Intensity = (decimal)p.Intensity,
                                                  StartTime = pro.StartTime,
                                                  EndTime = pro.EndTime,
                                                  DiscountPrice = (decimal)p.DiscountPrice,
                                                  LimitBuyEach = p.LimitBuyEach,
                                                  LimitBuyTotal = p.LimitBuyTotal,
                                                  SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                                  AppId = pro.AppId,
                                                  ChannelId = pro.ChannelId,
                                                  OutsideId = pro.OutsideId,
                                                  PresellStartTime = pro.PresellStartTime,
                                                  PresellEndTime = pro.PresellEndTime,
                                                  PromotionType = pro.PromotionType
                                              });
                        var promotionDic = todayPromotion.Where(a => commodityIds.Contains(a.CommodityId)).
                            Select(a => new
                            {
                                ComId = a.CommodityId,
                                Intensity = a.Intensity,
                                DiscountPrice = a.DiscountPrice,
                                LimitBuyEach = a.LimitBuyEach,
                                LimitBuyTotal = a.LimitBuyTotal,
                                SurplusLimitBuyTotal = a.SurplusLimitBuyTotal,
                                PromotionType = a.PromotionType,
                                OutsideId = a.OutsideId
                            }).Distinct();

                        //判断是否是易捷北京的app
                        var temp = ZPHSV.Instance.GetAppIdlist(new List<Guid>() { YJB.Deploy.CustomDTO.YJBConsts.YJAppId });
                        var ids = temp.Select(t => t.AppId).ToList();


                        foreach (var commodity in commodityList)
                        {
                            bool isdi = false;
                            foreach (var com in promotionDic)
                            {
                                if (com.ComId == commodity.Id)
                                {
                                    commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                                    commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                                    commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;
                                    commodity.PromotionType = com.PromotionType;
                                    commodity.PromotionTypeNew = com.PromotionType;
                                    var dprice = Convert.ToDecimal(com.DiscountPrice);
                                    //获取sku活动最小价格
                                    var skulist = ZPHSV.Instance.GetSkuActivityList((Guid)com.OutsideId);
                                    if (skulist.Any())
                                    {
                                        var activitySkuComs = skulist.Where(_ => _.OutSideActivityId == com.OutsideId.Value && _.CommodityId == com.ComId).ToList();
                                        var coms = skulist.Where(t => t.CommodityId == commodity.Id);
                                        if (coms.Any())
                                        {
                                            var a = coms.Min(t => t.JoinPrice);
                                            dprice = a;
                                            skuCommodityStockIds.Add(activitySkuComs.First().CommodityStockId);
                                        }
                                    }

                                    if (dprice > -1)
                                    {
                                        commodity.DiscountPrice = dprice;
                                        commodity.Intensity = 10;

                                        isdi = true;
                                        break;
                                    }
                                    else
                                    {
                                        commodity.DiscountPrice = -1;
                                        commodity.Intensity = com.Intensity;
                                        isdi = true;
                                        break;
                                    }
                                }
                            }
                            if (!isdi)
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = 10;
                                commodity.LimitBuyEach = -1;
                                commodity.LimitBuyTotal = -1;
                                commodity.SurplusLimitBuyTotal = -1;
                                commodity.PromotionType = 9999;
                            }

                            #region 包装规格设置
                            //if (commodityList.Count() > 0)
                            //{
                            //    var comids = commodityList.Select(_ => _.Id).ToList();
                            //    var commoditySpecificationlists = CommoditySpecifications.ObjectSet().Where(p => comids.Contains(p.CommodityId)).ToList();
                            //    foreach (var item in commodityList)
                            //    {
                            //        var commoditySpecificationlist = commoditySpecificationlists.Where(p => p.CommodityId == item.Id).ToList();
                            //        if (commoditySpecificationlist.Count() > 0)
                            //        {
                            //            commoditySpecificationlist.ForEach(p =>
                            //            {

                            //                Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO
                            //                {
                            //                    Id = p.Id,
                            //                    Name = "规格设置",
                            //                    Attribute = p.Attribute ?? 0,
                            //                    strAttribute = "1*" + p.Attribute
                            //                };
                            //                item.Specifications.Add(model);
                            //            });
                            //        }

                            //    }
                            //}

                            // New
                            var commoditySpecificationlist = CacheHelper.CommoditySpecifications.GetCommoditySpecifications(commodity.Id);
                            if (commoditySpecificationlist != null && commoditySpecificationlist.Count > 0)
                            {
                                commodity.Specifications = new List<Deploy.CustomDTO.SpecificationsDTO>();
                                commoditySpecificationlist.ForEach(p =>
                                {
                                    Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO
                                    {
                                        Id = p.Id,
                                        Name = "规格设置",
                                        Attribute = p.Attribute ?? 0,
                                        strAttribute = "1*" + p.Attribute
                                    };
                                    commodity.Specifications.Add(model);
                                });
                            }
                            #endregion

                            if (ids.Contains(commodity.AppId))
                            {
                                //获取分类id 处理检索出来包含移除分类的商品信息
                                var commodityCategory = CacheHelper.CommodityCategory.GetCommodityCategory(YJB.Deploy.CustomDTO.YJBConsts.YJAppId, commodity.Id);
                                if (commodityCategory != null) commodity.CategoryId = commodityCategory.CategoryId;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Jinher.JAP.Common.Loging.LogHelper.Error("商品列表查询错误", e);
                    }

                    // 读取赠品活动
                    var presents = PresentPromotionCommodity.ObjectSet().Where(_ => commodityIds.Contains(_.CommodityId))
                        .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime), pp => pp.PresentPromotionId,
                        ppc => ppc.Id, (c, p) => new { CommodityId = c.CommodityId, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime }).ToList();
                    foreach (var commodity in commodityList)
                    {
                        if (presents.Any(_ => _.CommodityId == commodity.Id))
                        {
                            commodity.PromotionTypeNew = 6;
                        }
                    }

                    //读取易捷优惠信息
                    var mallApplies = MallApply.GetTGQuery(YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Where(_ => appIds.Contains(_.AppId)).ToList();
                    var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;

                    var stocks = CommodityStock.ObjectSet().Where(_ => skuCommodityStockIds.Contains(_.Id)).Select(_ => new { _.CommodityId, _.MarketPrice, _.Price }).ToList();
                    foreach (var commodity in commodityList)
                    {
                        var comApp = mallApplies.Find(_ => _.AppId == commodity.AppId);
                        if (comApp != null)
                        {
                            commodity.MallType = comApp.Type;
                        }
                        var commodityCash = commodityCashes.Find(_ => _.CommodityId == commodity.Id);
                        if (commodityCash != null)
                        {
                            commodity.YJBAmount = commodityCash.YJBAmount;
                            commodity.YoukaAmount = commodityCash.YoukaAmount;
                        }

                        var stock = stocks.Find(_ => _.CommodityId == commodity.Id);
                        if (stock != null)
                        {
                            commodity.Price = stock.Price;
                            commodity.MarketPrice = stock.MarketPrice;
                        }
                    }
                }
                return commodityList;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误。commodityIds：{0}。false：{1}", commodityIds, false), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
        }

        /// <summary>
        /// 获取指定馆下商品信息
        /// </summary>
        /// <param name="beLongTo">电商馆Id</param>
        /// <param name="commodityIds">商品IdList</param>
        /// <param name="isDefaultOrder">是否排序</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongToExt(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder = false)
        {
            return GetCommodityByIdsWithPreSellInBeLongToWithTypeExt(beLongTo, commodityIds, isDefaultOrder, 0);
        }

        /// <summary>
        /// 获取指定馆下商品信息
        /// </summary>
        /// <param name="beLongTo">电商馆Id</param>
        /// <param name="commodityIds">商品IdList</param>
        /// <param name="isDefaultOrder">是否排序</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongToWithTypeExt(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder, int mallAppType)
        {
            try
            {
                if (commodityIds == null || !commodityIds.Any())
                    return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
                var appInfos = ZPHSV.Instance.GetAppIdlist(new List<Guid>() { beLongTo });
                var appIdList = appInfos.Select(t => t.AppId).ToList();
                appIdList.Add(beLongTo);

                var now = DateTime.Now;
                var commodityList = (from scc in CommodityCategory.ObjectSet()
                                     join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                                     join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id

                                     where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                           && commodityIds.Contains(c.Id) && appIdList.Contains(sc.AppId)
                                     orderby c.State, c.Salesvolume descending, c.SubTime descending
                                     select new CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         AppId = c.AppId,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).ToList();
                #region 根据商家类型筛选
                if (mallAppType > 0)
                {
                    var typeList = mallAppType == 1 ? new List<int> { 1 } : new List<int> { 0, 2 };
                    var mallAppIdList = MallApply.ObjectSet()
                        .Where(p => p.EsAppId == beLongTo && typeList.Contains(p.Type) && p.State.Value == (int)MallApplyEnum.TG)
                        .Select(p => p.AppId).ToList();
                    commodityList = commodityList.Where(p => mallAppIdList.Contains(p.AppId)).ToList();
                }
                #endregion
                if (isDefaultOrder && commodityList.Any())
                {
                    List<CommodityListCDTO> tmplist = new List<CommodityListCDTO>();
                    for (int i = 0; i < commodityIds.Count; i++)
                    {
                        var tmp = commodityList.FirstOrDefault(c => c.Id == commodityIds[i]);
                        if (tmp != null && tmp.State == 0)
                            tmplist.Add(tmp);
                    }
                    commodityList = tmplist;
                }
                List<Guid> appIds = commodityList.Select(c => c.AppId).Distinct().ToList();
                if (commodityList.Any())
                {
                    try
                    {
                        Dictionary<Guid, string> appList = APPSV.GetAppNameListByIds(appIds);
                        if (appList.Any())
                        {
                            foreach (var commoditySdto in commodityList)
                            {
                                if (appList.ContainsKey(commoditySdto.AppId))
                                {
                                    var app = appList[commoditySdto.AppId];
                                    if (!String.IsNullOrEmpty(app))
                                        commoditySdto.AppName = app;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("CommoditySV.GetCommodityByIdsExt异常：调用APPSV.GetAppNameListByIds异常,商品id列表：{0}，\r\n  StackTrace:\r\n{1} ", JsonHelper.JsonSerializer(commodityIds), ex));
                    }
                }

                //读今日折扣表
                try
                {
                    var todayPromotion = (from p in PromotionItems.ObjectSet()
                                          join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                          where !pro.IsDel && pro.IsEnable && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now) && pro.PromotionType != 3
                                          select new TodayPromotionDTO
                                          {
                                              PromotionId = p.PromotionId,
                                              CommodityId = p.CommodityId,
                                              Intensity = (decimal)p.Intensity,
                                              StartTime = pro.StartTime,
                                              EndTime = pro.EndTime,
                                              DiscountPrice = (decimal)p.DiscountPrice,
                                              LimitBuyEach = p.LimitBuyEach,
                                              LimitBuyTotal = p.LimitBuyTotal,
                                              SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                              AppId = pro.AppId,
                                              ChannelId = pro.ChannelId,
                                              OutsideId = pro.OutsideId,
                                              PresellStartTime = pro.PresellStartTime,
                                              PresellEndTime = pro.PresellEndTime,
                                              PromotionType = pro.PromotionType
                                          });
                    var promotionDic = todayPromotion.Where(a => commodityIds.Contains(a.CommodityId)).
                        Select(a => new
                        {
                            ComId = a.CommodityId,
                            Intensity = a.Intensity,
                            DiscountPrice = a.DiscountPrice,
                            LimitBuyEach = a.LimitBuyEach,
                            LimitBuyTotal = a.LimitBuyTotal,
                            SurplusLimitBuyTotal = a.SurplusLimitBuyTotal,
                            PromotionType = a.PromotionType,
                            OutsideId = a.OutsideId
                        }).Distinct();

                    foreach (var commodity in commodityList)
                    {
                        bool isdi = false;
                        foreach (var com in promotionDic)
                        {
                            if (com.ComId == commodity.Id)
                            {
                                commodity.PromotionType = com.PromotionType;
                                commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                                commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                                commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;

                                var dprice = Convert.ToDecimal(com.DiscountPrice);
                                //获取sku活动最小价格
                                var skulist = ZPHSV.Instance.GetSkuActivityList((Guid)com.OutsideId);
                                if (skulist.Any())
                                {
                                    var coms = skulist.Where(t => t.CommodityId == commodity.Id);
                                    if (coms.Any())
                                    {
                                        var a = coms.Min(t => t.JoinPrice);
                                        dprice = a;
                                    }
                                }

                                if (dprice > -1)
                                {
                                    commodity.DiscountPrice = dprice;
                                    commodity.Price = dprice;
                                    commodity.Intensity = 10;
                                    isdi = true;
                                    break;
                                }
                                else
                                {
                                    commodity.DiscountPrice = -1;
                                    commodity.Intensity = com.Intensity;
                                    isdi = true;
                                    break;
                                }
                            }
                        }
                        if (!isdi)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                    }
                }
                catch (Exception e)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Error("商品列表查询错误", e);
                }

                #region 众筹
                if (CustomConfig.CrowdfundingFlag)
                {
                    var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appIds.Contains(c.AppId)).Select(c => c.AppId).ToList();
                    if (crowdFundingApps.Any())
                    {
                        for (int i = 0; i < commodityList.Count; i++)
                        {
                            if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                commodityList[i].IsActiveCrowdfunding = true;
                        }
                    }
                }
                #endregion

                return commodityList;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误。commodityIds：{0}。false：{1}", commodityIds, false), ex);
            }
            return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
        }

        /// <summary>
        /// 查询AppID列表下的所有上架的商品
        /// </summary>
        /// <param name="appListSearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityByAppIdListExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchByAppIdListDTO appListSearch)
        {
            //参数判断
            if (appListSearch == null) return null;
            if (appListSearch.pageIndex < 1) return null;
            if (appListSearch.pageSize < 1) return null;
            if (appListSearch.appIdList == null || appListSearch.appIdList.Count < 1) return null;
            appListSearch.appIdList.RemoveAll(delegate (Guid obj)
            {
                if (obj == null || obj == Guid.Empty)
                {
                    return true;
                }
                return false;
            });
            if (appListSearch.appIdList.Count < 1) return null;


            var commodityDTO = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && appListSearch.appIdList.Contains(n.AppId) && n.State == 0 && n.CommodityType == 0).OrderBy(n => n.AppId).OrderByDescending(n => n.SubTime).Skip((appListSearch.pageIndex - 1) * appListSearch.pageSize).Take(appListSearch.pageSize);

            var result = from c in commodityDTO
                         select new CommodityDTO
                         {
                             AppId = c.AppId,
                             Code = c.Code,
                             CategoryName = c.CategoryName,
                             ComAttribute = c.ComAttribute,
                             Description = c.Description,
                             GroundTime = c.GroundTime,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             Name = c.Name,
                             No_Code = c.No_Code,
                             No_Number = c.No_Number,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             State = c.State,
                             MarketPrice = c.MarketPrice,
                             Stock = c.Stock,
                             TotalReview = c.TotalReview,
                             TotalCollection = c.TotalCollection,
                             Salesvolume = c.Salesvolume,
                             IsEnableSelfTake = c.IsEnableSelfTake

                         };

            return result.ToList();
        }

        /// <summary>
        /// 根据商品ID列表获取商品是否支持自提的信息
        /// </summary>
        /// <param name="commodityIdList">商品ID列表</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO> GetCommodityIsEnableSelfTakeListExt(List<Guid> commodityIdList)
        {
            //参数判断
            if (commodityIdList == null) return null;
            commodityIdList.RemoveAll(delegate (Guid obj)
            {
                if (obj == null || obj == Guid.Empty)
                {
                    return true;
                }
                return false;
            });
            if (commodityIdList.Count < 1) return null;

            //查询
            var query = from c in Commodity.ObjectSet()
                        where commodityIdList.Contains(c.Id) && c.CommodityType == 0
                        select new CommoditySelfTakeListDTO
                        {
                            Id = c.Id,
                            IsEnableSelfTake = c.IsEnableSelfTake
                        };

            return query.ToList();
        }

        /// <summary>
        /// 运费计算
        /// </summary>
        /// <param name="FreightTo">目的地</param>
        /// <param name="TemplateCounts">计算模板</param>
        /// <returns>计算结果</returns>
        private decimal CalNewFreight(string FreightTo, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> TemplateCounts, decimal couponPrice, decimal yjbPrice, decimal yjCouponPrice)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO fResult = new Deploy.CustomDTO.FreightResultDTO();
            if (TemplateCounts == null || TemplateCounts.Count < 1 || string.IsNullOrWhiteSpace(FreightTo))
            {
                return 0;
            }
            try
            {
                FreightTo = FreightTo.Trim();

                //区间运费
                decimal rangeFreight = 0;

                //总运费
                decimal fDecimal = 0;

                //订单总价
                decimal orderprice = 0;
                //订单件数
                decimal ordercount = 0;
                //订单总重量
                decimal orderweight = 0;

                //折扣额度
                var discountAmount = couponPrice + yjbPrice + yjCouponPrice;

                //计算订单总价
                orderprice = TemplateCounts.Sum(t => t.Count * t.Price) - discountAmount;


                //由于颜色尺寸问题，对相同商品合并分组
                var query = from templateCount in TemplateCounts
                            group templateCount by templateCount.CommodityId into g
                            select new
                            {
                                g.Key,
                                Num = g.Sum(templateCount => templateCount.Count)
                            };

                //取出商品ID列表
                List<Guid> commodityIds = query.Select(t => t.Key).ToList();
                //取出商品数量列表
                var tmpquery = query.ToDictionary(t => t.Key, t => t.Num);


                //取出商品的计价方式与重量
                var ordersearch = (from c in Commodity.ObjectSet()
                                   where commodityIds.Contains(c.Id) && c.CommodityType == 0
                                   select new
                                   {
                                       CommodityId = c.Id,
                                       CalcType = c.PricingMethod,
                                       Weight = c.Weight
                                   }).ToList();

                //计算订单件数
                ordercount = (from q in query
                              join c in ordersearch on q.Key equals c.CommodityId
                              where c.CalcType == 0
                              select new
                              {
                                  Num = q.Num
                              }).Sum(t => t.Num);
                //计算订单总重量
                orderweight = (from q in query
                               join c in ordersearch on q.Key equals c.CommodityId
                               where c.CalcType == 1
                               select new
                               {
                                   Weight = c.Weight == null ? 0 : q.Num * c.Weight.Value
                               }).Sum(t => t.Weight);

                //包邮的商品
                var tmpSearch = (from c in Commodity.ObjectSet()
                                 join f in FreightTemplate.ObjectSet() on c.FreightTemplateId equals f.Id
                                 where commodityIds.Contains(c.Id) && f.ExpressType == 1 && c.CommodityType == 0
                                 select c.Id).ToList();

                //部分包邮的商品
                var tmpPartailSearch = (from c in Commodity.ObjectSet()
                                        join f in FreightTemplate.ObjectSet() on c.FreightTemplateId equals f.Id
                                        join fp in FreightPartialFree.ObjectSet() on c.FreightTemplateId equals fp.FreightTemplateId
                                        where commodityIds.Contains(c.Id) && f.ExpressType == 2 && fp.DestinationCodes.Contains(FreightTo) &&
                                            (
                                             (fp.FreeType == 1 && fp.FreePrice <= orderprice) ||
                                             (f.PricingMethod == 0 && fp.FreeType == 0 && fp.FreeCount <= ordercount) ||
                                             (f.PricingMethod == 1 && fp.FreeType == 0 && fp.FreeCount >= orderweight)
                                             ) && c.CommodityType == 0
                                        select c.Id).ToList();

                //不包邮的商品，按公式计算运费
                List<Guid> calIds = commodityIds.Except(tmpSearch).Except(tmpPartailSearch).ToList();

                LogHelper.Info(string.Format("计算运费: 包邮商品数量：{0},部分包邮商品数量:{1},不包邮商品数量:{2},运送至:{3}", tmpSearch.Count, tmpPartailSearch.Count, calIds.Count, FreightTo));

                //取计费模板
                var tmpFeightSearch = (from c in Commodity.ObjectSet()
                                       join f in FreightTemplate.ObjectSet() on c.FreightTemplateId equals f.Id
                                       join fd in FreightTemplateDetail.ObjectSet() on c.FreightTemplateId equals fd.FreightTemplateId
                                       into g
                                       from tmp in g.DefaultIfEmpty()
                                       where calIds.Contains(c.Id) && tmp.DestinationCodes.Contains(FreightTo) && c.CommodityType == 0
                                       select new CalFreightTemplate
                                       {
                                           FreightTemplateId = f.Id,
                                           CommodityId = c.Id,
                                           CalcType = c.PricingMethod,
                                           FirstCount = tmp.FirstCount == null ? 0 : tmp.FirstCount,
                                           FirstCountPrice = Math.Ceiling(tmp.FirstCountPrice) == null ? 0 : Math.Ceiling(tmp.FirstCountPrice),
                                           NextCount = tmp.NextCount == null ? 0 : tmp.NextCount,
                                           NextCountPrice = Math.Ceiling(tmp.NextCountPrice) == null ? 0 : Math.Ceiling(tmp.NextCountPrice),
                                           Count = 0,
                                           Weight = c.Weight == null ? 0 : c.Weight.Value
                                       }).ToList();

                //取默认计费模板
                var defaultIds = calIds.Except(tmpFeightSearch.Select(t => t.CommodityId)).ToList();

                var tmpDefaultFeightSearch = (from c in Commodity.ObjectSet()
                                              join f in FreightTemplate.ObjectSet() on c.FreightTemplateId equals f.Id
                                              where c.CommodityType == 0 && defaultIds.Contains(c.Id)
                                              select new CalFreightTemplate
                                              {
                                                  FreightTemplateId = f.Id,
                                                  CommodityId = c.Id,
                                                  CalcType = c.PricingMethod,
                                                  FirstCount = f.FirstCount,
                                                  FirstCountPrice = Math.Ceiling(f.FirstCountPrice),
                                                  NextCount = f.NextCount,
                                                  NextCountPrice = Math.Ceiling(f.NextCountPrice),
                                                  Count = 0,
                                                  Weight = c.Weight == null ? 0 : c.Weight.Value
                                              }).ToList();


                //合并运费模板
                if (tmpDefaultFeightSearch.Count > 0)
                {
                    foreach (var calFreightTemplate in tmpDefaultFeightSearch)
                    {
                        var count = tmpFeightSearch.Count(t => t.FreightTemplateId == calFreightTemplate.FreightTemplateId);
                        if (count == 0)
                        {
                            tmpFeightSearch.Add(calFreightTemplate);
                        }
                    }
                }
                //给count附值 
                foreach (CalFreightTemplate item in tmpFeightSearch)
                {
                    item.Count = tmpquery[item.CommodityId];
                }

                #region 开始尝试计算价格区间运费.
                {
                    LogHelper.Info(string.Format("开始尝试计算价格区间运费. Input:{0}", JsonHelper.JsSerializer(TemplateCounts)));

                    var originalCommodityIds = TemplateCounts.Select(selector => selector.CommodityId);

                    var templateContext = from commodity in Commodity.ObjectSet()
                                          join tpl in FreightTemplate.ObjectSet() on commodity.FreightTemplateId equals tpl.Id
                                          join details in FreightRangeDetails.ObjectSet() on tpl.Id equals details.TemplateId
                                          where originalCommodityIds.Contains(commodity.Id) && tpl.PricingMethod == 3
                                          select details;

                    if (templateContext.Any())
                    {
                        /*
                         * ******************************
                         *  区间运费计算规则
                         * ********************************************************************************
                         * 
                         *  由于商品有多种规格
                         *  
                         *  先对商品进行分组
                         *  
                         *  然后根据公式计算并拿到各商品最终价
                         *   公式：单商品最小的价格（不区分规格） * 商品购买的总数量 = 单商品总价
                         *   
                         *  将多个商品的总价求和并减去本店铺最终折扣价格
                         *  
                         *  最终计算得到的结果，等下要用于计算命中的运费区间
                         * ********************************************************************************
                         */

                        var price = TemplateCounts
                                        .GroupBy(selector => selector.CommodityId)
                                        .Sum(selector =>
                                            Math.Round(selector.Min(minselector => minselector.Price) * selector.Sum(sumselector => sumselector.Count), 2))
                                            - discountAmount;

                        //开始计算可能命中的运费区间
                        var details = templateContext.Where(predicate => predicate.Min <= price && (predicate.Max > price || predicate.Max == 0)).ToList();

                        var specific = details.FirstOrDefault(predicate => predicate.IsSpecific && predicate.ProvinceCodes.Contains(FreightTo));

                        if (specific != null)
                        {
                            rangeFreight = specific.Cost;
                        }
                        else
                        {
                            var defaultCost = details.FirstOrDefault(predicate => !predicate.IsSpecific);

                            if (defaultCost != null)
                            {
                                rangeFreight = defaultCost.Cost;
                            }
                        }
                    }
                }
                #endregion

                LogHelper.Info(string.Format("计算运费: 独立计费数量：{0},默认模版数量:{1}，商品数量{2}，运费：{3},commodityIds=[" + JsonHelper.JsonSerializer(commodityIds) + "]", tmpFeightSearch.Count, tmpDefaultFeightSearch.Count, commodityIds.Count, fDecimal));
                //单种商品的运费计算方式
                if (commodityIds.Count == 1)
                {
                    //尝试使用价格区间运费模板计算运费
                    var commodityId = commodityIds.First();

                    #region
                    //不包邮，且取到运费模板的
                    if (tmpFeightSearch.Count == 1)
                    {
                        var comFeight = tmpFeightSearch[0];
                        //按件数
                        if (comFeight.CalcType == 0)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                        //按重量
                        else if (comFeight.CalcType == 1)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count * comFeight.Weight)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count * comFeight.Weight - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                    }
                    #endregion
                }
                //多种商品的计算
                else if (commodityIds.Count > 1)
                {
                    //计算运费
                    //只有一种商品需要计算运费，与单种商品的运费计算方式模式相同
                    if (tmpFeightSearch.Count == 1)
                    {
                        var comFeight = tmpFeightSearch[0];
                        //按件数
                        if (comFeight.CalcType == 0)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {

                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                        //按重量
                        else if (comFeight.CalcType == 1)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count * comFeight.Weight)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count * comFeight.Weight - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                    }
                    //2种及以上的商品需要计算运费
                    else if (tmpFeightSearch.Count > 1)
                    {
                        //取最大首费，最小增费的商品（首费直接取首费，不用除首费标准；增费时，需要用增费金额除以增费标准）
                        CalFreightTemplate cft = tmpFeightSearch[0];

                        for (int i = 1; i < tmpFeightSearch.Count; i++)
                        {
                            var first1 = cft.FirstCountPrice;
                            var first2 = tmpFeightSearch[i].FirstCountPrice;
                            if (first1 < first2)
                            {
                                cft = tmpFeightSearch[i];
                            }
                            else if (first1 == first2)
                            {
                                //增费标准是空，只有增费金额的，就按其是1个单位的增费金额
                                decimal next1 = 0;
                                decimal next2 = 0;

                                //增费标准与增费金额，可能有一个或两个都是0
                                if (cft.NextCountPrice == 0)
                                {
                                    next1 = 0;
                                }
                                else
                                {
                                    next1 = cft.NextCount > 0 ? cft.NextCountPrice / cft.NextCount : cft.NextCountPrice;
                                }
                                //增费标准与增费金额，可能有一个或两个都是0
                                if (tmpFeightSearch[i].NextCountPrice == 0)
                                {
                                    next2 = 0;
                                }
                                else
                                {
                                    next2 = tmpFeightSearch[i].NextCount > 0 ? tmpFeightSearch[i].NextCountPrice / tmpFeightSearch[i].NextCount : tmpFeightSearch[i].NextCountPrice;
                                }

                                if (next1 > next2)
                                {
                                    cft = tmpFeightSearch[i];
                                }
                            }
                        }

                        //当前计算过的商品运费模板集合
                        var computeFeight = new List<CalFreightTemplate>();
                        computeFeight.Add(cft);
                        //对于只有首费没有增费的，若不算入首费中，则不计算在运费中了。取出没算入到首费的商品    
                        var otherFeightSearch = tmpFeightSearch.Where(t => cft.CommodityId != t.CommodityId).ToList();
                        //计算运费
                        //算入首费
                        fDecimal = cft.FirstCountPrice;
                        LogHelper.Info(string.Format("计算运费-首费: 1当前运费价格为{0},订单总件数{1},cft{2}", fDecimal, ordercount, JsonHelper.JsonSerializer(cft)));

                        //计算首费商品剩余增费
                        //按件数
                        if (cft.CalcType == 0)
                        {
                            if (cft.FirstCount >= cft.Count)
                            {
                                fDecimal = cft.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (cft.NextCount == 0)
                                {
                                    fDecimal += cft.NextCountPrice;
                                }
                                else
                                {
                                    //fDecimal += Math.Ceiling(((cft.Count - cft.FirstCount) / cft.NextCount) * cft.NextCountPrice);
                                    fDecimal += Math.Ceiling((cft.Count - cft.FirstCount) / cft.NextCount) * cft.NextCountPrice;
                                }
                            }
                        }
                        //按重量
                        else if (cft.CalcType == 1)
                        {
                            if (cft.FirstCount >= cft.Count * cft.Weight)
                            {
                                fDecimal = cft.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (cft.NextCount == 0)
                                {
                                    fDecimal += cft.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal += Math.Ceiling((cft.Count * cft.Weight - cft.FirstCount) / cft.NextCount) * cft.NextCountPrice;
                                }
                            }
                        }

                        LogHelper.Info(string.Format("计算运费-首费商品的运费: 2当前运费价格为{0}", fDecimal));

                        LogHelper.Info(string.Format("计算运费:otherFeightSearch{0}", JsonHelper.JsonSerializer(otherFeightSearch)));

                        //其他商品增费金额
                        decimal zDecimal = 0;

                        //算入其它商品的费用
                        foreach (CalFreightTemplate item in otherFeightSearch)
                        {
                            #region 20180822添加多个商品，同一个运费模板使用商品增量计算运费
                            var sumCount = computeFeight.Where(t => t.FreightTemplateId == item.FreightTemplateId).Sum(t => t.Count);
                            LogHelper.Info(string.Format("计算运费-sumCount: sumCount：{0}，computeFeight=【{1}】", sumCount, JsonHelper.JsonSerializer(computeFeight)));
                            if (sumCount > 0)
                            {
                                sumCount = sumCount + item.Count;
                                //按件数
                                if (item.CalcType == 0)
                                {
                                    //增费标准不存在
                                    if (item.NextCount == 0)
                                    {
                                        //fDecimal += item.NextCountPrice;
                                        zDecimal += Math.Ceiling(item.NextCountPrice);
                                    }
                                    else
                                    {
                                        //20181015添加判断 如果未达到初费 会出现运费负数的情况
                                        if (sumCount - item.FirstCount > 0)
                                        {
                                            //fDecimal = Math.Ceiling((sumCount - item.FirstCount) / item.NextCount) * item.NextCountPrice;
                                            zDecimal += Math.Ceiling(sumCount / item.NextCount) * Math.Ceiling(item.NextCountPrice);
                                        }
                                    }
                                }
                                //按重量
                                else if (item.CalcType == 1)
                                {
                                    //增费标准不存在
                                    if (item.NextCount == 0)
                                    {
                                        //fDecimal += item.NextCountPrice;
                                        zDecimal += Math.Ceiling(item.NextCountPrice);
                                    }
                                    else
                                    {
                                        //20181015添加判断 如果未达到初费 会出现运费负数的情况
                                        if (sumCount - item.FirstCount > 0)
                                        {
                                            //fDecimal = Math.Ceiling((sumCount - item.FirstCount) * item.Weight / item.NextCount) * item.NextCountPrice;
                                            zDecimal += Math.Ceiling(sumCount * item.Weight / item.NextCount) * Math.Ceiling(item.NextCountPrice);
                                        }
                                    }
                                }
                            }
                            #endregion
                            else
                            {
                                //按件数
                                if (item.CalcType == 0)
                                {
                                    if (item.FirstCount >= item.Count)
                                    {
                                        //fDecimal = item.FirstCountPrice;
                                        zDecimal += item.NextCountPrice;
                                    }
                                    else
                                    {
                                        //增费标准不存在
                                        if (item.NextCount == 0)
                                        {

                                            //fDecimal += item.NextCountPrice;
                                            zDecimal += item.NextCountPrice;
                                        }
                                        else
                                        {
                                            //fDecimal += Math.Ceiling((item.Count - item.FirstCount) / item.NextCount) * item.NextCountPrice;
                                            zDecimal += Math.Ceiling(item.Count / item.NextCount) * Math.Ceiling(item.NextCountPrice);
                                        }
                                    }
                                }
                                //按重量
                                else if (item.CalcType == 1)
                                {
                                    if (item.FirstCount >= item.Count * item.Weight)
                                    {
                                        //fDecimal = item.FirstCountPrice;
                                        zDecimal += item.NextCountPrice;
                                    }
                                    else
                                    {
                                        //增费标准不存在
                                        if (item.NextCount == 0)
                                        {
                                            //fDecimal += item.NextCountPrice;
                                            zDecimal += item.NextCountPrice;
                                        }
                                        else
                                        {
                                            //fDecimal += Math.Ceiling((item.Count - item.FirstCount) * item.Weight / item.NextCount) * item.NextCountPrice;
                                            zDecimal += Math.Ceiling(item.Count * item.Weight / item.NextCount) * Math.Ceiling(item.NextCountPrice);
                                        }
                                    }
                                }
                            }
                            computeFeight.Add(item);
                        }
                        LogHelper.Info(string.Format("计算增费: 当前运费价格为{0}", zDecimal));
                        //首费+增费
                        fDecimal = Math.Ceiling(fDecimal) + zDecimal;
                    }
                }
                return Math.Ceiling(fDecimal + rangeFreight);
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("运费计算错误。FreightTo：{0}。TemplateCounts：{1}", FreightTo, TemplateCounts), ex);
                return 0;
            }
        }

        /// <summary>
        /// 多app运费计算，运送到的为文字，非编码
        /// </summary>
        /// <param name="freightTo">运送到</param>
        /// <param name="isSelfTake">是否自提</param>
        /// <param name="templateCounts">模板数据集合</param>
        /// <returns>运费计算结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalFreightMultiAppsByTextExt(string freightTo, int isSelfTake, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> templateCounts, Dictionary<Guid, decimal> coupons, Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbInfo, List<Guid> yjCouponIds)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO fResult = new Deploy.CustomDTO.FreighMultiAppResultDTO();
            decimal totalFDecimal = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(freightTo))
                {
                    fResult.Message = "运送的目的地为空";
                    fResult.ResultCode = 3;
                    fResult.Freight = 0;
                    return fResult;
                }

                freightTo = freightTo.Trim();
                //将目的地由汉字转成编码
                string FreightTo = ProvinceCityHelper.GetProvinceCodeByName(freightTo);

                //调用多app运费计算接口
                return CalFreightMultiAppsExt(FreightTo, isSelfTake, templateCounts, coupons, yjbInfo, yjCouponIds);

            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("运费计算错误", ex);
                fResult.Message = "运费计算错误";
                fResult.ResultCode = 1;
                fResult.Freight = 0;
                return fResult;
            }
        }
        /// <summary>
        /// 获取商品属性
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<ComAttStockDTO> CommodityAttrStocksExt(CommoditySearchDTO search)
        {

            if (search == null || search.CommodityId == Guid.Empty)
                return new ResultDTO<ComAttStockDTO>() { ResultCode = 1, Message = "参数为空" };
            var com = Commodity.ObjectSet().Where(c => c.Id == search.CommodityId && !c.IsDel && c.CommodityType == 0).Select(c => new { Id = c.Id, Stock = c.Stock, ComAttribute = c.ComAttribute }).FirstOrDefault();
            if (com == null)
                return new ResultDTO<ComAttStockDTO>() { ResultCode = 1, Message = "未找到商品" };
            ResultDTO<ComAttStockDTO> result = new ResultDTO<ComAttStockDTO> { Message = "Success", Data = new ComAttStockDTO() { ComAttibutes = new List<ComAttributeDTO>(), CommodityStocks = new List<CommodityAttrStockDTO>() } };
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                result.Data.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
                var attrDict = result.Data.ComAttibutes.GroupBy(c => c.Attribute).ToDictionary(x => x.Key, y => y.Count());
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                if (queryStock.Any())
                {
                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                    foreach (var item in queryStock)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        tempStock.Price = item.Price;
                        tempStock.MarketPrice = item.MarketPrice;
                        tempStock.Stock = item.Stock;
                        tempStock.Id = item.Id;
                        tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityStocks.Add(tempStock);
                    }
                    result.Data.CommodityStocks = commodityStocks;
                }
            }
            return result;
        }
        private List<Deploy.CustomDTO.CommodityStockDTO> createAttributes(ServiceCommodityAndCategoryDTO comAttrs)
        {
            List<Deploy.CustomDTO.CommodityStockDTO> result = new List<CommodityStockDTO>();
            if (comAttrs == null)
                comAttrs = new ServiceCommodityAndCategoryDTO();
            if (comAttrs.ActiveSecondAtributes == null)
                comAttrs.ActiveSecondAtributes = new List<ServiceSecondAttr>();

            var communicateAttributes = SecondAttribute.GetCommunicateAttributes().Where(c => comAttrs.ActiveSecondAtributes.Select(d => d.CommunicateAttribute).Distinct().Contains(c.Id)).ToList();
            var billingAttributes = SecondAttribute.GetBillingAttributes().Where(c => comAttrs.ActiveSecondAtributes.Select(d => d.BillingAttribute).Distinct().Contains(c.Id)).ToList();

            foreach (var communicateAttribute in communicateAttributes)
            {
                foreach (var billingAttribute in billingAttributes)
                {
                    Deploy.CustomDTO.CommodityStockDTO dto = new Deploy.CustomDTO.CommodityStockDTO();
                    dto.ComAttribute = new List<ComAttributeDTO>();
                    dto.ComAttribute.Add(new ComAttributeDTO { Attribute = "沟通方式", SecondAttribute = communicateAttribute.Name });
                    dto.ComAttribute.Add(new ComAttributeDTO { Attribute = "计费单位", SecondAttribute = billingAttribute.Name });

                    dto.ComAttributeIds = new List<ComAttributeHaveIdDTO>();
                    dto.ComAttributeIds.Add(new ComAttributeHaveIdDTO { Attribute = "沟通方式", SecondAttribute = communicateAttribute.Name, AttributeId = communicateAttribute.AttributeId, SecondAttributeId = communicateAttribute.Id });
                    dto.ComAttributeIds.Add(new ComAttributeHaveIdDTO { Attribute = "计费单位", SecondAttribute = billingAttribute.Name, AttributeId = billingAttribute.AttributeId, SecondAttributeId = billingAttribute.Id });
                    var activeSecondAtributes = comAttrs.ActiveSecondAtributes.FirstOrDefault(c => c.CommunicateAttribute == communicateAttribute.Id && c.BillingAttribute == billingAttribute.Id || c.CommunicateAttribute == billingAttribute.Id && c.BillingAttribute == communicateAttribute.Id);
                    if (activeSecondAtributes != null)
                    {
                        dto.Price = activeSecondAtributes.Price;
                        dto.Stock = int.MaxValue;
                    }
                    result.Add(dto);
                }
            }
            return result;
        }
        /// <summary>
        /// 添加服务型商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO"></param>
        /// <returns></returns>
        public ResultDTO SaveServiceCommodityExt(ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            try
            {
                Guid userId = this.ContextDTO.LoginUserID;

                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appModel = APPSV.Instance.GetAppOwnerInfo(serviceCommodityAndCategoryDTO.AppId);
                bool isFnull = true;
                if (appModel.OwnerType == 0)
                {
                    CBC.Deploy.CustomDTO.OrgInfoNewDTO orgInfoDTO = CBCSV.Instance.GetOrgInfoNewBySubId(appModel.OwnerId);
                    if (orgInfoDTO == null || string.IsNullOrEmpty(orgInfoDTO.CompanyPhone))
                    {
                        isFnull = false;
                    }
                }
                //保存商品属性实体类列表，要添加到缓存中
                if (isFnull)
                {
                    #region 商品
                    //添加商品时获取排序 最小值新加商品排序在最上面
                    var minSortValueQuery = (from m in Commodity.ObjectSet()
                                             where m.AppId == serviceCommodityAndCategoryDTO.AppId && m.CommodityType == 1
                                             select m);
                    int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                    int minSortValue = 2;
                    if (minSort.HasValue)
                    {
                        minSortValue = minSort.Value;
                    }
                    //对输入参数进行验证
                    var comQuery = (from m in Commodity.ObjectSet()
                                    where m.AppId == serviceCommodityAndCategoryDTO.AppId && m.CommodityType == 1
                                    select m.Id);
                    bool com = comQuery.Contains(serviceCommodityAndCategoryDTO.CommodityId);
                    if (com)
                    {
                        return new ResultDTO { ResultCode = 11, Message = "不能重复添加商品" };
                    }

                    if (string.IsNullOrWhiteSpace(serviceCommodityAndCategoryDTO.Name))
                    {
                        return new ResultDTO { ResultCode = 2, Message = "商品名称不能为空" };
                    }
                    if (serviceCommodityAndCategoryDTO.Name.Length > 30)
                    {
                        return new ResultDTO { ResultCode = 3, Message = "商品名称最多30个字" };
                    }
                    if (string.IsNullOrWhiteSpace(Convert.ToString(serviceCommodityAndCategoryDTO.Price)))
                    {
                        return new ResultDTO { ResultCode = 4, Message = "商品价格不能为空" };
                    }
                    if (serviceCommodityAndCategoryDTO.Price < 0)
                    {
                        return new ResultDTO { ResultCode = 5, Message = "商品价格必须大于等于0" };
                    }
                    if (serviceCommodityAndCategoryDTO.MarketPrice < serviceCommodityAndCategoryDTO.Price)
                    {
                        return new ResultDTO { ResultCode = 6, Message = "市场价不得小于商品现价" };
                    }
                    if (string.IsNullOrWhiteSpace(Convert.ToString(serviceCommodityAndCategoryDTO.Stock)))
                    {
                        return new ResultDTO { ResultCode = 7, Message = "商品数量不能为空" };
                    }
                    if (serviceCommodityAndCategoryDTO.Stock < 0)
                    {
                        return new ResultDTO { ResultCode = 8, Message = "商品数量不能小于0" };
                    }
                    if (string.IsNullOrWhiteSpace(Convert.ToString(serviceCommodityAndCategoryDTO.PicturesPath)))
                    {
                        return new ResultDTO { ResultCode = 9, Message = "商品缩略图不能为空" };
                    }
                    if (serviceCommodityAndCategoryDTO.Picturelist.Count < 0)
                    {
                        return new ResultDTO { ResultCode = 10, Message = "商品详情页图样不能为空" };
                    }

                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    Commodity commodity = Commodity.CreateCommodity();
                    commodity.Id = serviceCommodityAndCategoryDTO.CommodityId;
                    commodity.ComAttribute = serviceCommodityAndCategoryDTO.ActiveSecondAtributes.ToString();
                    commodity.Name = serviceCommodityAndCategoryDTO.Name;
                    commodity.SubTime = DateTime.Now;
                    commodity.GroundTime = DateTime.Now;
                    commodity.No_Code = string.Empty;
                    commodity.Price = serviceCommodityAndCategoryDTO.ActiveSecondAtributes.Min(t => t.Price);
                    commodity.MarketPrice = serviceCommodityAndCategoryDTO.MarketPrice;
                    commodity.PicturesPath = serviceCommodityAndCategoryDTO.PicturesPath;
                    commodity.State = 0;
                    commodity.Stock = int.MaxValue;
                    commodity.SubId = userId;
                    commodity.AppId = serviceCommodityAndCategoryDTO.AppId;
                    commodity.TotalCollection = 0;
                    commodity.TotalReview = 0;
                    commodity.Salesvolume = 0;
                    commodity.Description = serviceCommodityAndCategoryDTO.Description;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.SortValue = minSortValue - 1;
                    commodity.SaleAreas = serviceCommodityAndCategoryDTO.SaleAreas;
                    commodity.CommodityType = 1;

                    #region 商品新属性逻辑

                    serviceCommodityAndCategoryDTO.ComAttributes = createAttributes(serviceCommodityAndCategoryDTO);
                    List<ComAttributeDTO> allAttrList = new List<ComAttributeDTO>();

                    //判断是否有组合属性
                    if (serviceCommodityAndCategoryDTO.ComAttributes != null && serviceCommodityAndCategoryDTO.ComAttributes.Count > 0)
                    {
                        decimal maxMarketPrice = decimal.MaxValue;
                        decimal maxPrice = decimal.MaxValue;

                        foreach (var item in serviceCommodityAndCategoryDTO.ComAttributes)
                        {
                            item.ComAttribute = new List<ComAttributeDTO>();
                            item.ComAttributeIds.ForEach(r => item.ComAttribute.Add(new ComAttributeDTO() { Attribute = r.Attribute, SecondAttribute = r.SecondAttribute }));

                            CommodityStock cs = CommodityStock.CreateCommodityStock();
                            cs.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(item.ComAttribute);

                            foreach (var attr in item.ComAttribute)
                            {
                                if (!allAttrList.Exists(r => r.Attribute == attr.Attribute && r.SecondAttribute == attr.SecondAttribute))
                                {
                                    allAttrList.Add(attr);
                                }
                            }

                            cs.CommodityId = commodity.Id;
                            cs.Price = item.Price;
                            cs.MarketPrice = item.MarketPrice;
                            if (item.Price > 0 && item.Price < maxPrice)
                            {
                                maxPrice = item.Price;
                                commodity.Price = maxPrice;
                            }
                            if (item.MarketPrice > 0 && item.MarketPrice < maxMarketPrice)
                            {
                                maxMarketPrice = item.MarketPrice.Value;
                            }
                            cs.Stock = item.Stock;
                            contextSession.SaveObject(cs);
                        }
                        if (maxMarketPrice < decimal.MaxValue)
                        {
                            commodity.MarketPrice = maxMarketPrice;
                        }
                        else
                        {
                            commodity.MarketPrice = null;
                        }
                    }
                    commodity.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(allAttrList);
                    #endregion

                    contextSession.SaveObject(commodity);

                    #endregion

                    #region 商品图片
                    int sort = 1;
                    foreach (string picPath in serviceCommodityAndCategoryDTO.Picturelist)
                    {
                        ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                        pic.Name = "商品图片";
                        pic.SubId = userId;
                        pic.SubTime = DateTime.Now;
                        pic.PicturesPath = picPath;
                        pic.CommodityId = commodity.Id;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);

                        sort++;
                    }
                    #endregion
                    #region 商品日志

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    #endregion
                    var commodityEntityState = commodity.EntityState;
                    contextSession.SaveChanges();
                    commodity.RefreshCache(commodityEntityState);
                }
                else
                {
                    return new ResultDTO { ResultCode = 2, Message = "Name Is Null" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品发布服务异常。serviceCommodityAndCategoryDTO：{0}", serviceCommodityAndCategoryDTO), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }
        /// <summary>
        /// 修改服务型商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceCommodityExt(ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO)
        {
            Guid userId = this.ContextDTO.LoginUserID;

            //保存商品属性实体类列表，要添加到缓存中
            //List<ComAttibute> comAttrList = new List<ComAttibute>();
            try
            {
                #region 修改商品
                bool isPrice = false;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Commodity commodityDTO = Commodity.ObjectSet().FirstOrDefault(n => n.Id == serviceCommodityAndCategoryDTO.CommodityId && n.AppId == serviceCommodityAndCategoryDTO.AppId && n.CommodityType == 1);

                if (commodityDTO == null)
                {
                    return new ResultDTO { ResultCode = 3, Message = "未找到该商品！" };
                }

                if (commodityDTO.Price > serviceCommodityAndCategoryDTO.Price)
                {
                    isPrice = true;
                }

                //对输入参数进行验证
                if (string.IsNullOrWhiteSpace(serviceCommodityAndCategoryDTO.Name))
                {
                    return new ResultDTO { ResultCode = 2, Message = "商品名称不能为空" };
                }
                if (serviceCommodityAndCategoryDTO.Name.Length > 30)
                {
                    return new ResultDTO { ResultCode = 3, Message = "商品名称最多30个字" };
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(serviceCommodityAndCategoryDTO.Price)))
                {
                    return new ResultDTO { ResultCode = 4, Message = "商品价格不能为空" };
                }
                if (serviceCommodityAndCategoryDTO.Price < 0)
                {
                    return new ResultDTO { ResultCode = 5, Message = "商品价格必须大于等于0" };
                }
                if (serviceCommodityAndCategoryDTO.MarketPrice < serviceCommodityAndCategoryDTO.Price)
                {
                    return new ResultDTO { ResultCode = 6, Message = "市场价不得小于商品现价" };
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(serviceCommodityAndCategoryDTO.Stock)))
                {
                    return new ResultDTO { ResultCode = 7, Message = "商品数量不能为空" };
                }
                if (serviceCommodityAndCategoryDTO.Stock < 0)
                {
                    return new ResultDTO { ResultCode = 8, Message = "商品数量不能小于0" };
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(serviceCommodityAndCategoryDTO.PicturesPath)))
                {
                    return new ResultDTO { ResultCode = 9, Message = "商品缩略图不能为空" };
                }
                if (serviceCommodityAndCategoryDTO.Picturelist.Count < 0)
                {
                    return new ResultDTO { ResultCode = 10, Message = "商品详情页图样不能为空" };
                }
                commodityDTO.EntityState = System.Data.EntityState.Modified;
                commodityDTO.Id = serviceCommodityAndCategoryDTO.CommodityId;
                commodityDTO.ComAttribute = serviceCommodityAndCategoryDTO.ActiveSecondAtributes.ToString();
                commodityDTO.Name = serviceCommodityAndCategoryDTO.Name;
                commodityDTO.SubTime = DateTime.Now;
                commodityDTO.GroundTime = DateTime.Now;
                commodityDTO.Price = serviceCommodityAndCategoryDTO.ActiveSecondAtributes.Min(t => t.Price);
                commodityDTO.MarketPrice = serviceCommodityAndCategoryDTO.MarketPrice;
                commodityDTO.PicturesPath = serviceCommodityAndCategoryDTO.PicturesPath;
                commodityDTO.SubId = userId;
                commodityDTO.AppId = serviceCommodityAndCategoryDTO.AppId;
                commodityDTO.TotalCollection = 0;
                commodityDTO.TotalReview = 0;
                commodityDTO.Salesvolume = 0;
                commodityDTO.Description = serviceCommodityAndCategoryDTO.Description;
                commodityDTO.ModifiedOn = DateTime.Now;
                commodityDTO.SaleAreas = serviceCommodityAndCategoryDTO.SaleAreas;

                #region 商品新属性逻辑

                //删除老的属性，取数据的时候已经记录的Id,所以可以直接删除
                var cStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == commodityDTO.Id).ToList();
                if (cStock != null && cStock.Count > 0) //组合属性
                {
                    foreach (var item in cStock)
                    {
                        item.EntityState = System.Data.EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                }
                serviceCommodityAndCategoryDTO.ComAttributes = createAttributes(serviceCommodityAndCategoryDTO);
                List<ComAttributeDTO> allAttrList = new List<ComAttributeDTO>();

                //判断是否有组合属性
                if (serviceCommodityAndCategoryDTO.ComAttributes != null && serviceCommodityAndCategoryDTO.ComAttributes.Count > 0)
                {
                    decimal maxMarketPrice = decimal.MaxValue;
                    decimal maxPrice = decimal.MaxValue;

                    foreach (var item in serviceCommodityAndCategoryDTO.ComAttributes)
                    {
                        item.ComAttribute = new List<ComAttributeDTO>();
                        item.ComAttributeIds.ForEach(r => item.ComAttribute.Add(new ComAttributeDTO() { Attribute = r.Attribute, SecondAttribute = r.SecondAttribute }));

                        CommodityStock cs = CommodityStock.CreateCommodityStock();
                        cs.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(item.ComAttribute);

                        foreach (var attr in item.ComAttribute)
                        {
                            if (!allAttrList.Exists(r => r.Attribute == attr.Attribute && r.SecondAttribute == attr.SecondAttribute))
                            {
                                allAttrList.Add(attr);
                            }
                        }

                        cs.CommodityId = commodityDTO.Id;
                        cs.Price = item.Price;
                        cs.MarketPrice = item.MarketPrice;

                        if (item.Price > 0 && item.Price < maxPrice)
                        {
                            maxPrice = item.Price;
                            commodityDTO.Price = maxPrice;
                        }
                        if (item.MarketPrice > 0 && item.MarketPrice < maxMarketPrice)
                        {
                            maxMarketPrice = item.MarketPrice.Value;
                        }
                        if (item.Id != Guid.Empty) //判断是否已经存在
                        {
                            cs.Id = item.Id;
                        }
                        cs.Stock = item.Stock;
                        contextSession.SaveObject(cs);
                    }
                    if (maxMarketPrice < decimal.MaxValue)
                    {
                        commodityDTO.MarketPrice = maxMarketPrice;
                    }
                    else
                    {
                        commodityDTO.MarketPrice = null;
                    }
                }
                commodityDTO.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(allAttrList);
                #endregion


                #endregion

                #region 商品图片
                ProductDetailsPictureSV pdpbp = new ProductDetailsPictureSV();
                pdpbp.DeletePictures(serviceCommodityAndCategoryDTO.CommodityId);
                int sort = 1;
                foreach (string picPath in serviceCommodityAndCategoryDTO.Picturelist)
                {
                    ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                    pic.Name = "商品图片";
                    pic.SubId = userId;
                    pic.SubTime = DateTime.Now;
                    pic.PicturesPath = picPath;
                    pic.CommodityId = serviceCommodityAndCategoryDTO.CommodityId;
                    pic.Sort = sort;
                    pic.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(pic);

                    sort++;
                }
                #endregion

                contextSession.SaveObject(commodityDTO);
                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodityDTO);
                contextSession.SaveObject(journal);
                var commodityEntityState = commodityDTO.EntityState;
                contextSession.SaveChanges();
                commodityDTO.RefreshCache(commodityEntityState);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改商品服务异常。serviceCommodityAndCategoryDTO：{0}", JsonHelper.JsonSerializer(serviceCommodityAndCategoryDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 删除服务型商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommodityExt(Guid id)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 1);
                if (commodity == null)
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                commodity.ModifiedOn = DateTime.Now;
                commodity.EntityState = System.Data.EntityState.Modified;
                commodity.IsDel = true;


                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                contextSession.SaveObject(journal);
                contextSession.SaveChanges();

                commodity.RefreshCache(EntityState.Deleted);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除商品服务异常。id：{0}", id), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 删除多个商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceCommoditysExt(List<Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                ids.RemoveAll(c => c == Guid.Empty);
                if (!ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityList = Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 1).ToList();
                foreach (var commodity in commodityList)
                {
                    commodity.IsDel = true;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.EntityState = System.Data.EntityState.Modified;
                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    needRefreshCacheCommoditys.Add(commodity);
                }
                contextSession.SaveChange();

                needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Deleted));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除多个商品服务异常。ids：{0}", ids), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        public ResultDTO<CateringCommodityDTO> GetCateringCommodityExt(CommodityListSearchDTO search)
        {
            ResultDTO<CateringCommodityDTO> result = new ResultDTO<CateringCommodityDTO>();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                    return new ResultDTO<CateringCommodityDTO>() { ResultCode = 1, Message = "参数为空" };
                if (!search.AppId.HasValue || search.AppId == Guid.Empty)
                    return new ResultDTO<CateringCommodityDTO>() { ResultCode = 1, Message = "参数为空" };

                DateTime now = DateTime.Now;
                var appId = search.AppId.Value;


                CateringCommodityDTO resultData = new CateringCommodityDTO();
                resultData.CategoryList = new CategorySV().GetCategoryL1Ext(appId);

                var ocCommodityList = (from c in Commodity.ObjectSet()
                                       where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                       select new Jinher.AMP.BTP.Deploy.CommodityDTO()
                                       {
                                           Id = c.Id,
                                           PicturesPath = c.PicturesPath,
                                           Price = c.Price,
                                           State = c.State,
                                           Stock = c.Stock,
                                           Name = c.Name,
                                           MarketPrice = c.MarketPrice,
                                           AppId = c.AppId,
                                           IsEnableSelfTake = c.IsEnableSelfTake,
                                           ComAttribute = c.ComAttribute
                                       }).ToList();
                List<CommodityListIICDTO> commodityList = new List<CommodityListIICDTO>();
                if (ocCommodityList.Any())
                {
                    //循环取会员价
                    var appIds = ocCommodityList.Select(c => c.AppId).Distinct().ToList();
                    var vipDict = AVMSV.GetVipIntensities(appIds, search.UserId);

                    var comIds = ocCommodityList.Select(c => c.Id).ToList();
                    //应用所有库存信息
                    var commoditiesStocks = CommodityStock.ObjectSet().Where(c => comIds.Contains(c.CommodityId)).ToList();
                    var commodityCategories = (from cat in Category.ObjectSet()
                                               join comCat in CommodityCategory.ObjectSet() on cat.Id equals comCat.CategoryId
                                               where cat.AppId == appId && comCat.AppId == appId && cat.CurrentLevel == 1 && !cat.IsDel
                                               select comCat).Distinct().ToList();
                    //餐饮商品扩展属性
                    var cateringComdtyXData = CateringComdtyXData.ObjectSet().Where(o => comIds.Contains(o.ComdtyId)).ToList();
                    //应用所有促销
                    var todayPromotions = TodayPromotion.ObjectSet().Where(c => c.AppId == appId && c.StartTime <= now && c.EndTime > DateTime.Now).ToList();
                    CommodityListIICDTO commodityListCDTO;
                    foreach (var commodity in ocCommodityList)
                    {
                        commodityListCDTO = new CommodityListIICDTO
                        {
                            Id = commodity.Id,
                            Pic = commodity.PicturesPath,
                            Price = commodity.Price,
                            State = commodity.State,
                            Stock = commodity.Stock,
                            Name = commodity.Name,
                            MarketPrice = commodity.MarketPrice,
                            AppId = commodity.AppId,
                            IsEnableSelfTake = commodity.IsEnableSelfTake,
                            ComAttribute = commodity.ComAttribute,
                            IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute),
                            CategoryId = Guid.Empty,
                            ComAttrType = (commodity.ComAttribute == "[]" || commodity.ComAttribute == null) ? 1 : 3
                        };
                        if (commodityListCDTO.IsMultAttribute)
                        {
                            commodityListCDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodity.ComAttribute);

                            commodityListCDTO.CommodityStocks = new List<CommodityAttrStockDTO>();
                            var commodityStocks = commoditiesStocks.Where(c => c.CommodityId == commodity.Id).ToList();
                            foreach (var item in commodityStocks)
                            {
                                CommodityAttrStockDTO stock = new CommodityAttrStockDTO
                                {
                                    Price = item.Price,
                                    MarketPrice = item.MarketPrice,
                                    Stock = item.Stock,
                                    Id = item.Id,
                                    ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute)
                                };
                                commodityListCDTO.CommodityStocks.Add(stock);
                            }
                        }

                        TodayPromotion todayPromotionDTO = todayPromotions.Where(c => c.CommodityId == commodity.Id).OrderByDescending(c => c.PromotionType).FirstOrDefault();
                        if (todayPromotionDTO != null)
                        {
                            commodityListCDTO.LimitBuyEach = todayPromotionDTO.LimitBuyEach ?? -1;
                            commodityListCDTO.LimitBuyTotal = todayPromotionDTO.LimitBuyTotal ?? -1;
                            commodityListCDTO.SurplusLimitBuyTotal = todayPromotionDTO.SurplusLimitBuyTotal ?? 0;
                            commodityListCDTO.PromotionType = todayPromotionDTO.PromotionType;
                            if (todayPromotionDTO.DiscountPrice > -1)
                            {
                                commodityListCDTO.DiscountPrice = Convert.ToDecimal(todayPromotionDTO.DiscountPrice);
                                commodityListCDTO.Intensity = 10;
                            }
                            else
                            {
                                commodityListCDTO.DiscountPrice = -1;
                                commodityListCDTO.Intensity = todayPromotionDTO.Intensity;
                            }
                        }
                        else
                        {
                            commodityListCDTO.DiscountPrice = -1;
                            commodityListCDTO.Intensity = 10;
                            commodityListCDTO.LimitBuyEach = -1;
                            commodityListCDTO.LimitBuyTotal = -1;
                            commodityListCDTO.SurplusLimitBuyTotal = -1;

                            //会员价
                            VipPromotionDTO privilegeInfo = null;
                            if (vipDict.ContainsKey(commodity.AppId))
                            {
                                privilegeInfo = vipDict[commodity.AppId];
                            }
                            if (privilegeInfo != null && privilegeInfo.Intensity > 0)
                            {
                                commodityListCDTO.ComPromotionStatusEnum = ComPromotionStatusEnum.VipIntensity;
                                commodityListCDTO.Intensity = privilegeInfo.Intensity;
                            }
                            commodityListCDTO.PromotionType = 9999;
                        }

                        var xData = cateringComdtyXData.Where(o => o.ComdtyId == commodityListCDTO.Id).FirstOrDefault();
                        if (xData != null)
                        {
                            commodityListCDTO.MealBoxAmount = xData.MealBoxAmount;
                            commodityListCDTO.MealBoxNum = xData.MealBoxNum;
                        }
                        var commodityCategoryList = commodityCategories.Where(c => c.CommodityId == commodityListCDTO.Id).ToList();
                        if (commodityCategoryList.Any())
                        {
                            foreach (var commodityCategory in commodityCategoryList)
                            {
                                var newCom = commodityListCDTO.Clone();
                                newCom.CategoryId = commodityCategory.CategoryId;
                                commodityList.Add(newCom);
                            }
                        }
                        else
                        {
                            commodityList.Add(commodityListCDTO);
                        }
                    }
                    resultData.CommodityList = commodityList;
                }
                try
                {
                    resultData.AppName = APPSV.GetAppName(appId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCateringCommodity异常：调用APPSV.GetAppName异常,earch：{0}", JsonHelper.JsonSerializer(search)), ex);
                }
                result.Data = resultData;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CommoditySV.GetCateringCommodity,Ext点餐商品列表查询异常。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                return new ResultDTO<CateringCommodityDTO>() { ResultCode = 1, Message = ex.ToString() };
            }
            return result;
        }

        /// <summary>
        /// 判断商品是不是定制应用所发布的商品。
        /// </summary>
        /// <returns></returns>
        public ResultDTO<bool> IsFittedAppCommodityExt(Guid commodityId)
        {
            ResultDTO<bool> result = new ResultDTO<bool>();
            try
            {
                if (commodityId == Guid.Empty)
                {
                    result.Message = "参数错误，商品Id不能为空！";
                    result.ResultCode = 1;
                    return result;
                }
                //从缓存中取数据
                CommodityDTO com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }
                if (com == null || com.AppId == Guid.Empty)
                {
                    result.Message = "未找到商品信息！";
                    result.ResultCode = 2;
                    return result;
                }
                bool isFitted = APPBP.IsFittedApp(com.AppId);
                result.Data = isFitted;
            }
            catch (Exception ex)
            {
                LogHelper.Error("IsFittedAppCommodityExt异常，异常信息：", ex);

                result.Message = "服务异常，请稍后重试！";
                result.ResultCode = -1;
            }
            return result;
        }


        /// <summary>
        /// 获取发布商品的店铺的appId
        /// </summary>
        /// <returns></returns>
        public ResultDTO<Guid> GetCommodityAppIdExt(Guid commodityId)
        {
            ResultDTO<Guid> result = new ResultDTO<Guid>();
            try
            {
                if (commodityId == Guid.Empty)
                {
                    result.Message = "参数错误，商品Id不能为空！";
                    result.ResultCode = 1;
                    return result;
                }

                //从缓存中取数据
                CommodityDTO com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }
                if (com == null || com.AppId == Guid.Empty)
                {
                    result.Message = "未找到商品信息！";
                    result.ResultCode = 2;
                    return result;
                }
                result.Data = com.AppId;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetCommodityAppIdExt异常，异常信息：", ex);

                result.Message = "服务异常，请稍后重试！";
                result.ResultCode = -1;
            }
            return result;
        }


        /// <summary>
        /// 获取商品简略信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO<CommodityThumb> GetCommodityThumbExt(Guid commodityId)
        {
            ResultDTO<CommodityThumb> result = new ResultDTO<CommodityThumb>();
            result.Data = new CommodityThumb();
            try
            {
                if (commodityId == Guid.Empty)
                {
                    result.Message = "参数错误，商品Id不能为空！";
                    result.ResultCode = 1;
                    return result;
                }

                //从缓存中取数据
                CommodityDTO com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }
                if (com == null || com.AppId == Guid.Empty)
                {
                    result.Message = "未找到商品信息！";
                    result.ResultCode = 2;
                    return result;
                }
                result.Data.AppId = com.AppId;
                result.Data.PicturesPath = com.PicturesPath;
                result.Data.Name = com.Name;

                //拼团
                DateTime now = DateTime.Now;
                bool isDiyGroup = (from tp in TodayPromotion.ObjectSet()
                                   where tp.CommodityId == commodityId && tp.PromotionType == 3
                                   && tp.StartTime <= now && tp.EndTime > now
                                   select tp.PromotionId).Any();
                result.Data.IsDiyGroup = isDiyGroup;

            }
            catch (Exception ex)
            {
                LogHelper.Error("GetCommodityAppIdExt异常，异常信息：", ex);

                result.Message = "服务异常，请稍后重试！";
                result.ResultCode = -1;
            }
            return result;
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttributeExt(System.Guid commodityId, Guid userId)
        {
            try
            {
                CommodityDTO com = new CommodityDTO();
                //从缓存中取数据
                com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }
                if (com == null || com.IsDel)
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

                var resultData = GetAttributeAndPromotionOld(com, userId);
                if (resultData != null)
                {
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品的属性和优惠信息异常。commodityId：{0}。userId：{1}", commodityId, userId), ex);
            }
            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<CommoditySDTO> GetCommodityAttributeNewExt(System.Guid commodityId, Guid userId)
        {
            try
            {
                CommodityDTO com = new CommodityDTO();
                //从缓存中取数据
                com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
                if (com == null)
                {
                    var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityId);
                    if (commodity != null)
                    {
                        com = commodity.ToEntityData();
                        Commodity.AddAppCommondityDTOCache(com);
                    }
                }
                if (com == null || com.IsDel)
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

                var resultData = GetAttributeAndPromotion(com, userId);
                if (resultData != null)
                {
                    return new ResultDTO<CommoditySDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品的属性和优惠信息异常。commodityId：{0}。userId：{1}", commodityId, userId), ex);
            }
            return new ResultDTO<CommoditySDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="com"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetAttributeAndPromotionOld(CommodityDTO com, Guid userId)
        {
            if (com == null)
            {
                return null;
            }
            CommoditySDTO commoditySDTO = new CommoditySDTO();
            commoditySDTO.Id = com.Id;
            commoditySDTO.AppId = com.AppId;
            commoditySDTO.Pic = com.PicturesPath;
            commoditySDTO.Price = com.Price;
            commoditySDTO.Stock = com.Stock;
            commoditySDTO.Id = com.Id;
            commoditySDTO.Name = com.Name;
            commoditySDTO.Pic = com.PicturesPath;
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
            }
            if (commoditySDTO.ComAttibutes != null && commoditySDTO.ComAttibutes.Count > 0 && commoditySDTO.ComAttibutes.GroupBy(c => c.Attribute).Count() > 1)
            {
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                if (queryStock != null)
                {

                    List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                    foreach (var item in queryStock)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        tempStock.Price = item.Price;
                        tempStock.MarketPrice = item.MarketPrice;
                        tempStock.Stock = item.Stock;
                        tempStock.Id = item.Id;
                        tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityStocks.Add(tempStock);
                    }
                    commoditySDTO.CommodityStocks = commodityStocks;
                }
            }
            else
            {
                commoditySDTO.CommodityStocks = new List<CommodityAttrStockDTO>();
            }

            DateTime now = DateTime.Now;
            var tpQuery = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable
                           && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                           orderby pro.PromotionType descending
                           select new TodayPromotionDTO()
                           {
                               PromotionId = p.PromotionId,
                               CommodityId = p.CommodityId,
                               Intensity = (decimal)p.Intensity,
                               StartTime = pro.StartTime,
                               EndTime = pro.EndTime,
                               DiscountPrice = (decimal)p.DiscountPrice,
                               LimitBuyEach = p.LimitBuyEach,
                               LimitBuyTotal = p.LimitBuyTotal,
                               SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                               AppId = pro.AppId,
                               ChannelId = pro.ChannelId,
                               OutsideId = pro.OutsideId,
                               PresellStartTime = pro.PresellStartTime,
                               PresellEndTime = pro.PresellEndTime,
                               PromotionType = pro.PromotionType,
                               GroupMinVolume = pro.GroupMinVolume,
                               ExpireSecond = pro.ExpireSecond,
                               Description = pro.Description
                           }).ToList();

            TodayPromotionDTO promotion = (from pro in tpQuery
                                           where pro.PromotionType == 0 || pro.PromotionType == 1 || pro.PromotionType == 2
                                           select pro).FirstOrDefault();
            if (promotion == null)
            {
                commoditySDTO.PromotionTypeNew = ComPromotionStatusEnum.NoPromotion;
                commoditySDTO.Intensity = 10;
                commoditySDTO.DiscountPrice = -1;
            }
            else
            {
                commoditySDTO.PromotionTypeNew = (ComPromotionStatusEnum)promotion.PromotionType;
                commoditySDTO.LimitBuyEach = promotion.LimitBuyEach;
                commoditySDTO.LimitBuyTotal = promotion.LimitBuyTotal;
                commoditySDTO.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;
                commoditySDTO.PromotionType = promotion.PromotionType;
                commoditySDTO.PromotionStartTime = promotion.StartTime;
                commoditySDTO.PromotionEndTime = promotion.EndTime;
                commoditySDTO.PresellStartTime = promotion.PresellStartTime;
                commoditySDTO.PresellEndTime = promotion.PresellEndTime;
                commoditySDTO.PromotionId = promotion.PromotionId;
                commoditySDTO.OutPromotionId = promotion.OutsideId;

                if (promotion.DiscountPrice > -1)
                {
                    commoditySDTO.Intensity = 10;
                    commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                }
                else
                {
                    commoditySDTO.Intensity = promotion.Intensity;
                    commoditySDTO.DiscountPrice = -1;
                }


                if (promotion.EndTime <= now)
                {
                    //已结束
                    commoditySDTO.PromotionState = 4;
                }
                else if (promotion.StartTime <= now)
                {
                    //进行中
                    commoditySDTO.PromotionState = 3;
                }
                else if (!promotion.PresellEndTime.HasValue || promotion.PresellEndTime < now)
                {
                    //等待抢购
                    commoditySDTO.PromotionState = 2;
                }
                else if (promotion.PresellStartTime <= now)
                {
                    //预约预售进行中
                    commoditySDTO.PromotionState = 1;
                }


                //预约
                if (promotion.PromotionType == 2 && promotion.PresellStartTime.HasValue && promotion.PresellStartTime < now)
                {
                    var promotionOutSideId = promotion.OutsideId.HasValue ? promotion.OutsideId.Value : Guid.Empty;
                    try
                    {
                        var presell = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAndCheckPresellInfoById(new CheckPresellInfoCDTO() { comdtyId = com.Id, id = promotionOutSideId });
                        if (presell != null)
                            commoditySDTO.PreselledNum = presell.preselledNum;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("CommoditySV.getCommodityDetailsZPH。获取预约信息异常。 comdtyId：{0}，id：{1}", com.Id, promotionOutSideId), ex);
                    }
                }
            }


            #region 规格设置集合暂时不用
            //var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
            //if (commoditySpecification.Count() > 0)
            //{
            //    var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commoditySDTO.Id).ToList();
            //    commoditySpecificationlist.ForEach(p =>
            //    {

            //        Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
            //        model.Id = p.Id;
            //        model.Attribute = p.Attribute ?? 0;
            //        model.strAttribute = "1*" + p.Attribute;
            //        commoditySDTO.Specifications.Add(model);
            //    });

            //}
            #endregion

            //拼团信息。
            commoditySDTO.DiyGroupPromotion = tpQuery.Where(tp => tp.PromotionType == 3).FirstOrDefault();
            //会员折扣信息
            commoditySDTO.VipPromotion = AVMSV.GetVipIntensity(commoditySDTO.AppId, userId);
            return commoditySDTO;
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="com"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO GetAttributeAndPromotion(CommodityDTO com, Guid userId)
        {
            if (com == null)
            {
                return null;
            }
            CommoditySDTO commoditySDTO = new CommoditySDTO();
            commoditySDTO.Id = com.Id;
            commoditySDTO.AppId = com.AppId;
            commoditySDTO.Pic = com.PicturesPath;
            commoditySDTO.Price = com.Price;
            commoditySDTO.Stock = com.Stock;
            commoditySDTO.Id = com.Id;
            commoditySDTO.Name = com.Name;
            commoditySDTO.Pic = com.PicturesPath;
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySDTO.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
            }
            var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
            if (queryStock != null)
            {

                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO> commodityStocks = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO>();
                foreach (var item in queryStock.OrderBy(t => t.ComAttribute))
                {
                    Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO tempStock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                    tempStock.Price = item.Price;
                    tempStock.MarketPrice = item.MarketPrice;
                    tempStock.Stock = item.Stock;
                    tempStock.Id = item.Id;
                    tempStock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                    commodityStocks.Add(tempStock);
                }
                commoditySDTO.CommodityStocks = commodityStocks;
            }

            DateTime now = DateTime.Now;
            var tpQuery = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable
                           && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                           orderby pro.PromotionType descending
                           select new TodayPromotionDTO()
                           {
                               PromotionId = p.PromotionId,
                               CommodityId = p.CommodityId,
                               Intensity = (decimal)p.Intensity,
                               StartTime = pro.StartTime,
                               EndTime = pro.EndTime,
                               DiscountPrice = (decimal)p.DiscountPrice,
                               LimitBuyEach = p.LimitBuyEach,
                               LimitBuyTotal = p.LimitBuyTotal,
                               SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                               AppId = pro.AppId,
                               ChannelId = pro.ChannelId,
                               OutsideId = pro.OutsideId,
                               PresellStartTime = pro.PresellStartTime,
                               PresellEndTime = pro.PresellEndTime,
                               PromotionType = pro.PromotionType,
                               GroupMinVolume = pro.GroupMinVolume,
                               ExpireSecond = pro.ExpireSecond,
                               Description = pro.Description
                           }).ToList();

            TodayPromotionDTO promotion = (from pro in tpQuery
                                           where pro.PromotionType == 0 || pro.PromotionType == 1 || pro.PromotionType == 2
                                           select pro).FirstOrDefault();
            if (promotion == null)
            {
                commoditySDTO.PromotionTypeNew = ComPromotionStatusEnum.NoPromotion;
                commoditySDTO.Intensity = 10;
                commoditySDTO.DiscountPrice = -1;
            }
            else
            {
                commoditySDTO.PromotionTypeNew = (ComPromotionStatusEnum)promotion.PromotionType;
                commoditySDTO.LimitBuyEach = promotion.LimitBuyEach;
                commoditySDTO.LimitBuyTotal = promotion.LimitBuyTotal;
                commoditySDTO.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;
                commoditySDTO.PromotionType = promotion.PromotionType;
                commoditySDTO.PromotionStartTime = promotion.StartTime;
                commoditySDTO.PromotionEndTime = promotion.EndTime;
                commoditySDTO.PresellStartTime = promotion.PresellStartTime;
                commoditySDTO.PresellEndTime = promotion.PresellEndTime;
                commoditySDTO.PromotionId = promotion.PromotionId;
                commoditySDTO.OutPromotionId = promotion.OutsideId;

                if (promotion.Intensity < 10)
                {
                    commoditySDTO.Intensity = promotion.Intensity;
                    commoditySDTO.DiscountPrice = -1;
                }
                else
                {
                    commoditySDTO.Intensity = 10;
                    commoditySDTO.DiscountPrice = promotion.DiscountPrice;
                }

                if (promotion.EndTime <= now)
                {
                    //已结束
                    commoditySDTO.PromotionState = 4;
                }
                else if (promotion.StartTime <= now)
                {
                    //进行中
                    commoditySDTO.PromotionState = 3;
                }
                else if (!promotion.PresellEndTime.HasValue || promotion.PresellEndTime < now)
                {
                    //等待抢购
                    commoditySDTO.PromotionState = 2;
                }
                else if (promotion.PresellStartTime <= now)
                {
                    //预约预售进行中
                    commoditySDTO.PromotionState = 1;
                }


                //预约
                if (promotion.PromotionType == 2 && promotion.PresellStartTime.HasValue && promotion.PresellStartTime < now)
                {
                    var promotionOutSideId = promotion.OutsideId.HasValue ? promotion.OutsideId.Value : Guid.Empty;
                    try
                    {
                        var presell = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAndCheckPresellInfoById(new CheckPresellInfoCDTO() { comdtyId = com.Id, id = promotionOutSideId });
                        if (presell != null)
                            commoditySDTO.PreselledNum = presell.preselledNum;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(string.Format("CommoditySV.getCommodityDetailsZPH。获取预约信息异常。 comdtyId：{0}，id：{1}", com.Id, promotionOutSideId), ex);
                    }
                }

                //获取活动sku价格
                var skuAList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).Where(t => t.IsJoin);
                List<Deploy.CustomDTO.SkuActivityCDTO> skuActivityCdtos = skuAList.Select(skuActivityCdto => new Deploy.CustomDTO.SkuActivityCDTO
                {
                    id = skuActivityCdto.id,
                    OutSideActivityId = skuActivityCdto.OutSideActivityId,
                    OutSideActivityType = skuActivityCdto.OutSideActivityType,
                    CommodityId = skuActivityCdto.CommodityId,
                    CommodityStockId = skuActivityCdto.CommodityStockId,
                    IsJoin = skuActivityCdto.IsJoin,
                    subId = skuActivityCdto.subId,
                    subTime = skuActivityCdto.subTime,
                    modifiedOn = skuActivityCdto.modifiedOn,
                    JoinPrice = skuActivityCdto.JoinPrice
                }).ToList();
                commoditySDTO.SkuActivityCdtos = skuActivityCdtos;
            }
            //拼团信息。
            commoditySDTO.DiyGroupPromotion = tpQuery.FirstOrDefault(tp => tp.PromotionType == 3);
            if (commoditySDTO.DiyGroupPromotion != null && commoditySDTO.DiyGroupPromotion.OutsideId != null)
            {
                //获取活动sku价格
                var skuAList = ZPHSV.Instance.GetSkuActivityList((Guid)commoditySDTO.DiyGroupPromotion.OutsideId).Where(t => t.IsJoin);
                List<Deploy.CustomDTO.SkuActivityCDTO> skuActivityCdtos = skuAList.Select(skuActivityCdto => new Deploy.CustomDTO.SkuActivityCDTO
                {
                    id = skuActivityCdto.id,
                    OutSideActivityId = skuActivityCdto.OutSideActivityId,
                    OutSideActivityType = skuActivityCdto.OutSideActivityType,
                    CommodityId = skuActivityCdto.CommodityId,
                    CommodityStockId = skuActivityCdto.CommodityStockId,
                    IsJoin = skuActivityCdto.IsJoin,
                    subId = skuActivityCdto.subId,
                    subTime = skuActivityCdto.subTime,
                    modifiedOn = skuActivityCdto.modifiedOn,
                    JoinPrice = skuActivityCdto.JoinPrice
                }).ToList();
                commoditySDTO.SkuActivityCdtos = skuActivityCdtos;
            }
            //会员折扣信息
            commoditySDTO.VipPromotion = AVMSV.GetVipIntensity(commoditySDTO.AppId, userId);

            #region 包装规格设置
            if (commoditySDTO != null)
            {
                List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                var commoditySpecificationlist = CommoditySpecifications.ObjectSet().Where(p => p.CommodityId == commoditySDTO.Id).ToList();
                if (commoditySpecificationlist.Count() > 0)
                {
                    commoditySpecificationlist.ForEach(p =>
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                        model.Id = p.Id;
                        model.Name = "规格设置";
                        model.Attribute = p.Attribute ?? 0;
                        model.strAttribute = "1*" + p.Attribute;
                        Specificationslist.Add(model);
                    });
                }
                commoditySDTO.Specifications = Specificationslist;
            }
            #endregion

            return commoditySDTO;
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3Ext(CheckCommodityParam ccp)
        {
            var selecid = ccp.CommodityIdsList.Select(r => r.CommodityId).ToList();
            var cStockid = ccp.CommodityIdsList.Where(c => c.CommodityStockId.HasValue && c.CommodityStockId != Guid.Empty).Select(r => r.CommodityStockId).Distinct().ToList();
            List<CheckCommodityDTO> commodityList;
            if (cStockid.Count == 0)
            {
                commodityList = (from c in Commodity.ObjectSet()
                                 where selecid.Contains(c.Id) && c.CommodityType == 0
                                 select new CheckCommodityDTO
                                 {
                                     Id = c.Id,
                                     Price = c.Price,
                                     State = c.IsDel ? 3 : c.State,
                                     Stock = c.Stock,
                                     Intensity = 10,
                                     DiscountPrice = -1,
                                     OPrice = c.Price,
                                     LimitBuyEach = -1,
                                     LimitBuyTotal = -1,
                                     SurplusLimitBuyTotal = 0,
                                     IsEnableSelfTake = c.IsEnableSelfTake,
                                     AppId = c.AppId,
                                     Type = c.Type ?? 0
                                 }).ToList();
            }
            else
            {
                commodityList = (
                    from c in Commodity.ObjectSet().Where(c => c.CommodityType == 0 && selecid.Contains(c.Id))
                    join s in CommodityStock.ObjectSet().Where(ss => cStockid.Contains(ss.Id))
                    on c.Id equals s.CommodityId into os
                    from ss in os.DefaultIfEmpty()
                        //where c.CommodityType == 0 && selecid.Contains(c.Id) && (ss == null || cStockid.Contains(ss.Id))
                    select new CheckCommodityDTO
                    {
                        Id = c.Id,
                        Price = ss != null ? ss.Price : c.Price,
                        State = c.IsDel ? 3 : c.State,
                        Stock = ss != null ? ss.Stock : c.Stock,
                        Intensity = 10,
                        DiscountPrice = -1,
                        OPrice = c.Price,
                        LimitBuyEach = -1,
                        LimitBuyTotal = -1,
                        SurplusLimitBuyTotal = 0,
                        CommodityStockId = ss != null ? ss.Id : default(Guid?),
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        AppId = c.AppId,
                        Type = c.Type ?? 0
                    }).ToList();
            }


            #region 包装规格设置
            foreach (var item in commodityList)
            {
                foreach (var _item in ccp.CommodityIdsList)
                {
                    if (item.Id == _item.CommodityId)
                    {
                        item.Specifications = _item.Specifications ?? 0;
                    }
                }
            }
            #endregion

            DateTime now = DateTime.Now;
            //商品在每日促销表里集合 
            //var outPromotionIds = CommodityIdsList.Where(c => c.OutPrommotionId.HasValue && c.OutPrommotionId != Guid.Empty).Select(c => c.OutPrommotionId).Distinct().ToList();
            var promotionQuery = TodayPromotion.ObjectSet()
                .Where(a => selecid.Contains(a.CommodityId) && a.EndTime > now && a.StartTime < now).
                   Select(a => new
                   {
                       ComId = a.CommodityId,
                       Intensity = a.Intensity,
                       DiscountPrice = a.DiscountPrice,
                       LimitBuyTotal = a.LimitBuyTotal,
                       SurplusLimitBuyTotal = a.SurplusLimitBuyTotal,
                       LimitBuyEach = a.LimitBuyEach,
                       PromotionId = a.PromotionId,
                       PromotionType = a.PromotionType,
                       OutSideid = a.OutsideId,
                       GroupMinVolume = a.GroupMinVolume,
                       ExpireSecond = a.ExpireSecond
                   });
            if (ccp.PromotionType == -1)
            {
                promotionQuery = promotionQuery.Where(p => p.PromotionType == 0 || p.PromotionType == 1 || p.PromotionType == 2 || p.PromotionType == 5);
            }
            else if (ccp.PromotionType == 3)
            {
                promotionQuery = promotionQuery.Where(p => p.PromotionType == 3);
            }
            var promotionDic = promotionQuery.Distinct().ToList();

            if (ccp.PromotionType == -1)
            {
                foreach (var commodity in commodityList)
                {
                    var stock = commodity.Stock;
                    var price = commodity.Price;
                    commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.NoPromotion;
                    var promotion = promotionDic.Where(c => c.ComId == commodity.Id && c.PromotionType != 3).FirstOrDefault();
                    if (promotion == null || !ccp.CommodityIdsList.Any(c => c.CommodityId == commodity.Id))
                    {
                        continue;
                    }
                    commodity.ComPromotionStatusEnum = (ComPromotionStatusEnum)promotion.PromotionType;
                    int limie = -1;
                    int limitotal = -1;

                    if (promotion.PromotionType == 2 && promotion.OutSideid.HasValue)
                    {
                        var checkPresellInfoCDTO = new CheckPresellInfoCDTO()
                        {
                            id = promotion.OutSideid.Value,
                            comdtyId = promotion.ComId,
                            userId = ccp.UserID,
                            commodityStockId = commodity.CommodityStockId
                        };
                        try
                        {
                            var result = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAndCheckPresellInfoById(checkPresellInfoCDTO);
                            if (result != null && result.id != Guid.Empty)
                            {
                                commodity.IsNeedPresell = true;
                                commodity.IsPreselled = result.isPreselled;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("CommoditySV.CheckCommodityNewExt.presellFacade.GetAndCheckPresellInfoById 异常。 checkPresellInfoCDTO：{0}", JsonHelper.JsonSerializer(checkPresellInfoCDTO)), ex);
                            continue;
                        }
                    }

                    commodity.SurplusLimitBuyTotal = Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                    if (promotion.LimitBuyEach != null && promotion.LimitBuyEach != -1)
                    {
                        var ul = UserLimited.ObjectSet().Where(n => n.UserId == ccp.UserID && n.PromotionId == promotion.PromotionId && n.CommodityId == commodity.Id).Select(s => s.Count).ToList();
                        if (ul.Count > 0)
                        {
                            limie = Convert.ToInt32(promotion.LimitBuyEach - ul.Sum());
                        }
                        else
                        {
                            limie = Convert.ToInt32(promotion.LimitBuyEach);
                        }
                        commodity.LimitBuyEach = Convert.ToInt32(promotion.LimitBuyEach);
                    }

                    if (promotion.LimitBuyTotal != null && promotion.LimitBuyTotal != -1)
                    {
                        limitotal = Convert.ToInt32(promotion.LimitBuyTotal) - Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                        commodity.LimitBuyTotal = Convert.ToInt32(promotion.LimitBuyTotal);
                    }
                    #region 判断 限购总数 每人限购 库存之间的关系
                    //如果只有每人限购 没有促销商品总量
                    if (limie != -1 && limitotal == -1)
                    {
                        if (commodity.Stock > limie)
                        {
                            commodity.Stock = limie;
                        }
                    }
                    else
                    {
                        //如果只有促销商品总量 没有每人限购 
                        if (limitotal != -1 && limie == -1)
                        {
                            if (commodity.Stock > limitotal)
                            {
                                commodity.Stock = limitotal;
                            }
                        }
                        else
                        {
                            //如果都设置了 每人限购 促销商品总量
                            if (limitotal != -1 && limie != -1)
                            {
                                if (limitotal > limie)
                                {
                                    commodity.Stock = limie;
                                }
                                else
                                {
                                    commodity.Stock = limitotal;
                                }
                            }
                        }
                    }
                    #endregion
                    var discountPrice = promotion.DiscountPrice;
                    if (promotion.Intensity < 10)
                    {
                        discountPrice = -1;
                    }
                    if (discountPrice > -1)
                    {
                        commodity.Price = Convert.ToDecimal(promotion.DiscountPrice);
                        commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                    }
                    else
                    {
                        commodity.Price = Math.Round((commodity.Price * promotion.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                        commodity.Intensity = promotion.Intensity;
                    }
                    //获取活动sku价格
                    LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.(Guid)promotion.OutSideid 值。 (Guid)promotion.OutSideid：{0}", (Guid)promotion.OutSideid));
                    var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutSideid).Where(t => t.CommodityId == commodity.Id);
                    if (skuActivityList.Any())
                    {
                        var commodityStockId = commodity.CommodityStockId;
                        if (string.IsNullOrEmpty(Convert.ToString(commodityStockId)) || commodityStockId == Guid.Empty)
                        {
                            commodityStockId = skuActivityList.ToList()[0].CommodityStockId;
                        }
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.commodity.CommodityStockId 值。 commodity.CommodityStockId：{0}", commodityStockId));
                        var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodityStockId);
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.skuActivity 值。 skuActivity：{0}", JsonHelper.JsonSerializer(skuActivity)));
                        if (skuActivity != null && skuActivity.IsJoin)
                        {
                            commodity.Price = Convert.ToDecimal(skuActivity.JoinPrice);
                            commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                        }
                        else
                        {
                            //不参与活动的 库存值不变 按照库存值做判断
                            commodity.Stock = stock;
                            commodity.LimitBuyEach = commodity.LimitBuyTotal = stock;
                            commodity.Price = commodity.DiscountPrice = price;
                        }
                    }
                }

                //循环取会员价
                var appIds = commodityList.Select(c => c.AppId).Distinct().ToList();
                var vipDict = AVMSV.GetVipIntensities(appIds, ccp.UserID);
                foreach (var commodity in commodityList)
                {
                    //会员价
                    VipPromotionDTO privilegeInfo = null;
                    if (vipDict.ContainsKey(commodity.AppId))
                        privilegeInfo = vipDict[commodity.AppId];

                    if (privilegeInfo != null && privilegeInfo.Intensity > 0)
                    {
                        var isdi = false;
                        var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2,
                                                        MidpointRounding.AwayFromZero);
                        var promotion = promotionDic.Where(c => c.ComId == commodity.Id).OrderByDescending(m => m.PromotionType).FirstOrDefault();
                        if (promotion == null || !ccp.CommodityIdsList.Any(c => c.CommodityId == commodity.Id))
                        {
                            isdi = false;
                        }
                        else
                        {
                            isdi = true;
                        }
                        //没有活动
                        if (isdi)
                        {

                        }
                        else
                        {
                            if (privilegePrice < commodity.Price)
                            {
                                commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.VipIntensity;
                                commodity.DiscountPrice = -1;
                                commodity.Price = privilegePrice;
                                commodity.Intensity = privilegeInfo.Intensity;
                            }
                        }
                    }
                }

            }
            else if (ccp.PromotionType == 3 || ccp.PromotionType == 5)
            {
                //检查拼团活动。
                foreach (var commodity in commodityList)
                {
                    var promotion = promotionDic.Where(c => c.ComId == commodity.Id && c.PromotionType == 3).FirstOrDefault();
                    if (promotion == null || !ccp.CommodityIdsList.Any(c => c.CommodityId == commodity.Id))
                    {
                        continue;
                    }

                    commodity.DiscountPrice = promotion.DiscountPrice.HasValue ? promotion.DiscountPrice.Value : 0;
                    commodity.Price = commodity.DiscountPrice;
                    commodity.LimitBuyTotal = promotion.LimitBuyTotal.HasValue ? promotion.LimitBuyTotal.Value : 0;
                    commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal.HasValue ? promotion.SurplusLimitBuyTotal.Value : 0;
                    commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.DiyGroup;
                    int limie = promotion.GroupMinVolume ?? 0;
                    #region  剩余可入团人数

                    if (ccp.DiygId != Guid.Empty)
                    {
                        var diyGroup = (from dg in DiyGroup.ObjectSet()
                                        where dg.Id == ccp.DiygId
                                        select dg).FirstOrDefault();
                        if (diyGroup == null || diyGroup.State != 1 || diyGroup.JoinNumber >= promotion.GroupMinVolume)
                        {
                            limie = 0;
                        }
                        else
                        {
                            limie = promotion.GroupMinVolume ?? 0 - diyGroup.JoinNumber;
                        }
                    }
                    #endregion

                    #region 距离参团结束剩余时间

                    //开团时间
                    DateTime diyBeginTime = (from dg in DiyGroup.ObjectSet()
                                             where dg.Id == ccp.DiygId
                                             select dg.SubTime).FirstOrDefault();
                    TimeSpan ts = DateTime.Now - diyBeginTime;
                    commodity.DiySecondSurplus = promotion.ExpireSecond.HasValue ? promotion.ExpireSecond.Value : 0;
                    commodity.DiySecondSurplus -= (int)ts.TotalSeconds;

                    #endregion

                    #region 库存，可购买商品总数量。
                    int limitotal = Convert.ToInt32(promotion.LimitBuyTotal) - Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                    commodity.Stock = commodity.Stock >= limitotal ? limitotal : commodity.Stock;
                    commodity.Stock = Math.Min(limie, commodity.Stock);
                    #endregion

                    //获取活动sku价格
                    var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutSideid);
                    if (skuActivityList.Count > 0)
                    {
                        var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                        if (skuActivity != null && skuActivity.IsJoin)
                        {
                            commodity.Price = Convert.ToDecimal(skuActivity.JoinPrice);
                            commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                        }
                    }
                }
            }
            return commodityList;
        }

        /// <summary>
        /// 校验商品信息  金采团购活动
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV3IIExt(CheckCommodityParam ccp)
        {
            LogHelper.Debug("进入金采支付提交商品验证接口CheckCommodityV3IIExt：cpp：" + JsonHelper.JsSerializer(ccp));
            var selecid = ccp.CommodityIdsList.Select(r => r.CommodityId).ToList();
            var cStockid = ccp.CommodityIdsList.Where(c => c.CommodityStockId.HasValue && c.CommodityStockId != Guid.Empty).Select(r => r.CommodityStockId).Distinct().ToList();
            List<CheckCommodityDTO> commodityList;
            if (cStockid.Count == 0)
            {
                commodityList = (from c in Commodity.ObjectSet()
                                 where selecid.Contains(c.Id) && c.CommodityType == 0
                                 select new CheckCommodityDTO
                                 {
                                     Id = c.Id,
                                     Price = c.Price,
                                     State = c.IsDel ? 3 : c.State,
                                     Stock = c.Stock,
                                     Intensity = 10,
                                     DiscountPrice = -1,
                                     OPrice = c.Price,
                                     LimitBuyEach = -1,
                                     LimitBuyTotal = -1,
                                     SurplusLimitBuyTotal = 0,
                                     IsEnableSelfTake = c.IsEnableSelfTake,
                                     AppId = c.AppId,
                                     Type = c.Type ?? 0
                                 }).ToList();
            }
            else
            {
                commodityList = (
                    from c in Commodity.ObjectSet().Where(c => c.CommodityType == 0 && selecid.Contains(c.Id))
                    join s in CommodityStock.ObjectSet().Where(ss => cStockid.Contains(ss.Id))
                    on c.Id equals s.CommodityId into os
                    from ss in os.DefaultIfEmpty()
                    select new CheckCommodityDTO
                    {
                        Id = c.Id,
                        Price = ss != null ? ss.Price : c.Price,
                        State = c.IsDel ? 3 : c.State,
                        Stock = ss != null ? ss.Stock : c.Stock,
                        Intensity = 10,
                        DiscountPrice = -1,
                        OPrice = c.Price,
                        LimitBuyEach = -1,
                        LimitBuyTotal = -1,
                        SurplusLimitBuyTotal = 0,
                        CommodityStockId = ss != null ? ss.Id : default(Guid?),
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        AppId = c.AppId,
                        Type = c.Type ?? 0
                    }).ToList();
            }
            if (ccp.PromotionType == -1)
            {
                foreach (var commodity in commodityList)
                {
                    var stock = commodity.Stock;
                    var price = commodity.Price;

                    //获取金采团购活动sku价格
                    LogHelper.Debug("进入金采支付提交商品验证接口CheckCommodityV3IIExt：commodity.id：" + commodity.Id + ",ccp.JcActivityId:" + ccp.JcActivityId);

                    var jcActivityList = ZPHSV.Instance.GetItemsListByActivityId(ccp.JcActivityId).Data.Where(t => t.ComdtyId == commodity.Id);
                    if (jcActivityList.Any())
                    {
                        var commodityStockId = commodity.CommodityStockId;
                        if (string.IsNullOrEmpty(Convert.ToString(commodityStockId)) || commodityStockId == Guid.Empty)
                        {
                            commodityStockId = jcActivityList.ToList()[0].ComdtyStockId;
                        }
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2获取金采团购活动sku价格.commodity.CommodityStockId 值。 commodity.CommodityStockId：{0}", commodityStockId));
                        var jcActivity = jcActivityList.FirstOrDefault(t => t.ComdtyStockId == commodityStockId);
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.jcActivity 值。 jcActivity：{0}", JsonHelper.JsonSerializer(jcActivity)));
                        if (jcActivity != null)
                        {
                            commodity.Price = Convert.ToDecimal(jcActivity.GroupPrice);
                            commodity.DiscountPrice = Convert.ToDecimal(jcActivity.GroupPrice);
                        }
                        else
                        {
                            //不参与活动的 库存值不变 按照库存值做判断
                            commodity.Stock = stock;
                            commodity.LimitBuyEach = commodity.LimitBuyTotal = stock;
                            commodity.Price = commodity.DiscountPrice = price;
                        }
                    }
                    commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.NoPromotion;
                }
            }
            return commodityList;
        }

        /// <summary>
        /// 校验购物车商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckShopCommodityDTO> CheckCommodityV4Ext(CheckShopCommodityParam ccp)
        {
            var selecid = ccp.ShoppingCartItems.Select(r => r.CommodityId).ToList();
            var cStockid = ccp.ShoppingCartItems.Where(c => c.CommodityStockId.HasValue && c.CommodityStockId != Guid.Empty).Select(r => r.CommodityStockId).Distinct().ToList();
            var shoppingCartItemIds = ccp.ShoppingCartItems.Select(r => r.ShoppingCartItemId).ToList();

            var commodityList = (from s in ShoppingCartItems.ObjectSet()
                                 join c in Commodity.ObjectSet() on s.CommodityId equals c.Id
                                 where shoppingCartItemIds.Contains(s.Id) && c.CommodityType == 0
                                 select new CheckShopCommodityDTO
                                 {
                                     Id = c.Id,
                                     State = c.IsDel ? 3 : c.State,
                                     Stock = c.Stock,
                                     Price = c.Price,
                                     Intensity = 10,
                                     DiscountPrice = -1,
                                     OPrice = c.Price,
                                     LimitBuyEach = -1,
                                     LimitBuyTotal = -1,
                                     SurplusLimitBuyTotal = 0,
                                     CommodityStockId = default(Guid?),
                                     IsEnableSelfTake = c.IsEnableSelfTake,
                                     AppId = c.AppId,
                                     ShoppingCartItemId = s.Id,
                                     Type = c.Type ?? 0,
                                     JcActivityId = s.JcActivityId ?? Guid.Empty
                                 }).ToList();
            foreach (var c in commodityList)
            {
                var csid = ccp.ShoppingCartItems.FirstOrDefault(t => t.ShoppingCartItemId == c.ShoppingCartItemId).CommodityStockId;
                if (csid != Guid.Empty)
                {
                    var commdityStock = CommodityStock.ObjectSet().FirstOrDefault(t => t.Id == csid);
                    if (commdityStock != null)
                    {
                        c.Stock = commdityStock.Stock;
                        c.Price = commdityStock.Price;
                        c.OPrice = commdityStock.Price;
                        c.CommodityStockId = csid;
                    }
                }
            }

            //进货价验证和处理            
            ValidCostPrice(selecid, cStockid.Where(a => a.HasValue).Select(a => a.Value).ToList());

            DateTime now = DateTime.Now;
            //商品在每日促销表里集合 
            var promotionQuery = TodayPromotion.ObjectSet()
                .Where(a => selecid.Contains(a.CommodityId) && a.EndTime > now && a.StartTime < now)
                .Select(a => new
                {
                    ComId = a.CommodityId,
                    a.Intensity,
                    a.DiscountPrice,
                    a.LimitBuyTotal,
                    a.SurplusLimitBuyTotal,
                    a.LimitBuyEach,
                    a.PromotionId,
                    a.PromotionType,
                    OutSideid = a.OutsideId,
                    a.GroupMinVolume,
                    a.ExpireSecond
                });
            if (ccp.PromotionType == -1)
            {
                promotionQuery = promotionQuery.Where(p => p.PromotionType == 0 || p.PromotionType == 1 || p.PromotionType == 2);
            }
            else if (ccp.PromotionType == 3)
            {
                promotionQuery = promotionQuery.Where(p => p.PromotionType == 3);
            }
            var promotionDic = promotionQuery.Distinct().ToList();

            if (ccp.PromotionType == -1)
            {
                foreach (var commodity in commodityList)
                {
                    var stock = commodity.Stock;
                    var price = commodity.Price;
                    commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.NoPromotion;
                    var promotion = promotionDic.FirstOrDefault(c => c.ComId == commodity.Id && c.PromotionType != 3);
                    if (promotion == null || ccp.ShoppingCartItems.All(c => c.CommodityId != commodity.Id))
                    {
                        continue;
                    }
                    commodity.ComPromotionStatusEnum = (ComPromotionStatusEnum)promotion.PromotionType;
                    int limie = -1;
                    int limitotal = -1;

                    if (promotion.PromotionType == 2 && promotion.OutSideid.HasValue)
                    {
                        var checkPresellInfoCdto = new CheckPresellInfoCDTO()
                        {
                            id = promotion.OutSideid.Value,
                            comdtyId = promotion.ComId,
                            userId = ccp.UserID
                        };
                        try
                        {
                            var result = ZPHSV.Instance.GetAndCheckPresellInfoById(checkPresellInfoCdto);
                            if (result != null && result.id != Guid.Empty)
                            {
                                commodity.IsNeedPresell = true;
                                commodity.IsPreselled = result.isPreselled;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(string.Format("CommoditySV.CheckCommodityNewExt.presellFacade.GetAndCheckPresellInfoById 异常。 checkPresellInfoCDTO：{0}", JsonHelper.JsonSerializer(checkPresellInfoCdto)), ex);
                            continue;
                        }
                    }

                    commodity.SurplusLimitBuyTotal = Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                    if (promotion.LimitBuyEach != null && promotion.LimitBuyEach != -1)
                    {
                        var ul = UserLimited.ObjectSet().Where(n => n.UserId == ccp.UserID && n.PromotionId == promotion.PromotionId && n.CommodityId == commodity.Id).Select(s => s.Count).ToList();
                        limie = ul.Count > 0 ? Convert.ToInt32(promotion.LimitBuyEach - ul.Sum()) : Convert.ToInt32(promotion.LimitBuyEach);
                        commodity.LimitBuyEach = Convert.ToInt32(promotion.LimitBuyEach);
                    }

                    if (promotion.LimitBuyTotal != null && promotion.LimitBuyTotal != -1)
                    {
                        limitotal = Convert.ToInt32(promotion.LimitBuyTotal) - Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                        commodity.LimitBuyTotal = Convert.ToInt32(promotion.LimitBuyTotal);
                    }
                    #region 判断 限购总数 每人限购 库存之间的关系
                    //如果只有每人限购 没有促销商品总量
                    if (limie != -1 && limitotal == -1)
                    {
                        if (commodity.Stock > limie)
                        {
                            commodity.Stock = limie;
                        }
                    }
                    else
                    {
                        //如果只有促销商品总量 没有每人限购 
                        if (limitotal != -1 && limie == -1)
                        {
                            if (commodity.Stock > limitotal)
                            {
                                commodity.Stock = limitotal;
                            }
                        }
                        else
                        {
                            //如果都设置了 每人限购 促销商品总量
                            if (limitotal != -1 && limie != -1)
                            {
                                commodity.Stock = limitotal > limie ? limie : limitotal;
                            }
                        }
                    }
                    #endregion
                    var discountPrice = promotion.DiscountPrice;
                    if (promotion.Intensity < 10)
                    {
                        discountPrice = -1;
                    }
                    if (discountPrice > -1)
                    {
                        commodity.Price = Convert.ToDecimal(promotion.DiscountPrice);
                        commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                    }
                    else
                    {
                        commodity.Price = Math.Round((commodity.Price * promotion.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                        commodity.Intensity = promotion.Intensity;
                    }
                    //获取活动sku价格
                    LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.(Guid)promotion.OutSideid 值。 (Guid)promotion.OutSideid：{0}", (Guid)promotion.OutSideid));
                    var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutSideid).Where(t => t.CommodityId == commodity.Id);
                    if (skuActivityList.Any())
                    {
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.commodity.CommodityStockId 值。 commodity.CommodityStockId：{0}", commodity.CommodityStockId));
                        var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.skuActivity 值。 skuActivity：{0}", JsonHelper.JsonSerializer(skuActivity)));
                        if (skuActivity != null && skuActivity.IsJoin)
                        {
                            commodity.Price = Convert.ToDecimal(skuActivity.JoinPrice);
                            commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                            if (promotion.Intensity < 10)
                            {
                                commodity.DiscountPrice = -1;
                            }
                        }
                        else
                        {
                            //不参与活动的 库存值不变 按照库存值做判断
                            commodity.Stock = stock;
                            commodity.LimitBuyEach = commodity.LimitBuyTotal = stock;
                            commodity.Price = commodity.DiscountPrice = price;
                        }
                    }
                }

                //循环取会员价
                var appIds = commodityList.Select(c => c.AppId).Distinct().ToList();
                var vipDict = AVMSV.GetVipIntensities(appIds, ccp.UserID);
                foreach (var commodity in commodityList)
                {
                    //会员价
                    VipPromotionDTO privilegeInfo = null;
                    if (vipDict.ContainsKey(commodity.AppId))
                        privilegeInfo = vipDict[commodity.AppId];

                    if (privilegeInfo != null && privilegeInfo.Intensity > 0)
                    {
                        var isdi = false;
                        var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2,
                                                        MidpointRounding.AwayFromZero);
                        var promotion = promotionDic.Where(c => c.ComId == commodity.Id).OrderByDescending(m => m.PromotionType).FirstOrDefault();
                        if (promotion == null || ccp.ShoppingCartItems.All(c => c.CommodityId != commodity.Id))
                        {
                        }
                        else
                        {
                            isdi = true;
                        }
                        //没有活动
                        if (isdi)
                        {

                        }
                        else
                        {
                            if (privilegePrice < commodity.Price)
                            {
                                commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.VipIntensity;
                                commodity.DiscountPrice = -1;
                                commodity.Price = privilegePrice;
                                commodity.Intensity = privilegeInfo.Intensity;
                            }
                        }
                    }
                }

            }
            else if (ccp.PromotionType == 3)
            {
                //检查拼团活动。
                foreach (var commodity in commodityList)
                {
                    var promotion = promotionDic.FirstOrDefault(c => c.ComId == commodity.Id && c.PromotionType == 3);
                    if (promotion == null || ccp.ShoppingCartItems.All(c => c.CommodityId != commodity.Id))
                    {
                        continue;
                    }

                    commodity.DiscountPrice = promotion.DiscountPrice ?? 0;
                    commodity.Price = commodity.DiscountPrice;
                    commodity.LimitBuyTotal = promotion.LimitBuyTotal ?? 0;
                    commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal ?? 0;
                    commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.DiyGroup;
                    int limie = promotion.GroupMinVolume ?? 0;
                    #region  剩余可入团人数

                    if (ccp.DiygId != Guid.Empty)
                    {
                        var diyGroup = (from dg in DiyGroup.ObjectSet()
                                        where dg.Id == ccp.DiygId
                                        select dg).FirstOrDefault();
                        if (diyGroup == null || diyGroup.State != 1 || diyGroup.JoinNumber >= promotion.GroupMinVolume)
                        {
                            limie = 0;
                        }
                        else
                        {
                            limie = promotion.GroupMinVolume ?? 0 - diyGroup.JoinNumber;
                        }
                    }
                    #endregion

                    #region 距离参团结束剩余时间

                    //开团时间
                    DateTime diyBeginTime = (from dg in DiyGroup.ObjectSet()
                                             where dg.Id == ccp.DiygId
                                             select dg.SubTime).FirstOrDefault();
                    TimeSpan ts = DateTime.Now - diyBeginTime;
                    commodity.DiySecondSurplus = promotion.ExpireSecond.HasValue ? promotion.ExpireSecond.Value : 0;
                    commodity.DiySecondSurplus -= (int)ts.TotalSeconds;

                    #endregion

                    #region 库存，可购买商品总数量。
                    int limitotal = Convert.ToInt32(promotion.LimitBuyTotal) - Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                    commodity.Stock = commodity.Stock >= limitotal ? limitotal : commodity.Stock;
                    commodity.Stock = Math.Min(limie, commodity.Stock);
                    #endregion
                    //获取活动sku价格
                    var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutSideid);
                    if (skuActivityList.Count > 0)
                    {
                        var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                        if (skuActivity != null && skuActivity.IsJoin)
                        {
                            commodity.Price = Convert.ToDecimal(skuActivity.JoinPrice);
                            commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                        }
                    }
                }
            }

            #region 添加购物车商品状态
            List<Deploy.CommodityStockDTO> commodityStockList = new List<Deploy.CommodityStockDTO>();
            if (cStockid.Any())
            {
                commodityStockList = CommodityStock.ObjectSet().Where(c => cStockid.Contains(c.Id)).Select(c => new Deploy.CommodityStockDTO() { Id = c.Id, ComAttribute = c.ComAttribute, Price = c.Price, Stock = c.Stock }).ToList();
            }


            foreach (var checkCommodityDto in commodityList)
            {
                ShopCartStateEnum shopCartState = ShopCartStateEnum.OK;
                switch (checkCommodityDto.State)
                {
                    case 1:
                        shopCartState = ShopCartStateEnum.OffSale;
                        break;
                    case 3:
                        shopCartState = ShopCartStateEnum.Del;
                        break;
                }

                #region 属性判断
                var commodityStock = commodityStockList.FirstOrDefault(c => c.Id == checkCommodityDto.CommodityStockId);
                var commodity = Commodity.ObjectSet().FirstOrDefault(c => c.Id == checkCommodityDto.Id);
                var shoppingCartItem = ShoppingCartItems.ObjectSet().FirstOrDefault(t => t.Id == checkCommodityDto.ShoppingCartItemId);

                var shopCartItemAttrs = shoppingCartItem.ComAttributeIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var comAttrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodity.ComAttribute);
                switch (shopCartItemAttrs.Length)
                {
                    case 0:
                        if (comAttrs != null && comAttrs.Count > 0)
                            shopCartState = ShopCartStateEnum.Attribute;
                        break;
                    case 1:
                        if (comAttrs == null || Commodity.CheckComMultAttrs(commodity.ComAttribute) || !comAttrs.Any() ||
                            comAttrs.All(c => c.SecondAttribute != shopCartItemAttrs[0]))
                            shopCartState = ShopCartStateEnum.Attribute;
                        break;
                    case 2:
                        if (!Commodity.CheckComMultAttrs(commodity.ComAttribute) || commodityStock == null || comAttrs == null || !comAttrs.Any())
                        {
                            shopCartState = ShopCartStateEnum.Attribute;
                        }
                        else
                        {
                            var comAttrDict = comAttrs.GroupBy(c => c.Attribute).ToDictionary(x => x.Key, y => y.ToList());
                            bool isShopCartItemAttr0 = false;
                            bool isShopCartItemAttr1 = false;

                            foreach (var attr in comAttrDict.Keys)
                            {
                                if (!isShopCartItemAttr0 && comAttrDict[attr].Any(c => c.SecondAttribute == shopCartItemAttrs[0]))
                                {
                                    isShopCartItemAttr0 = true;
                                }
                                else if (!isShopCartItemAttr1 && comAttrDict[attr].Any(c => c.SecondAttribute == shopCartItemAttrs[1]))
                                {
                                    isShopCartItemAttr1 = true;
                                }
                            }
                            if (!isShopCartItemAttr0 || !isShopCartItemAttr1)
                                shopCartState = ShopCartStateEnum.Attribute;
                        }
                        break;
                    default:
                        shopCartState = ShopCartStateEnum.Others;
                        break;
                }

                #endregion

                #region 库存判断

                if (shopCartState != ShopCartStateEnum.Attribute)
                {
                    if (shoppingCartItem.CommodityStockId == Guid.Empty || shoppingCartItem.CommodityStockId == shoppingCartItem.CommodityId)
                    {
                        if (commodity.Stock <= 0)
                            shopCartState = ShopCartStateEnum.Stock;
                    }
                    else
                    {
                        if (commodityStock == null || commodityStock.Stock <= 0)
                            shopCartState = ShopCartStateEnum.Stock;
                    }
                }
                #endregion

                #region 规格设置判断
                checkCommodityDto.Specifications = ccp.Specifications;
                #endregion

                checkCommodityDto.ShopCartState = shopCartState;
                #endregion

                //获取金采团购活动价格
                LogHelper.Debug("checkCommodityDto.JcActivityId:" + checkCommodityDto.JcActivityId + ",checkCommodityDto.CommodityStockId:" + checkCommodityDto.CommodityStockId);
                if (checkCommodityDto.CommodityStockId == default(Guid?))
                {
                    checkCommodityDto.CommodityStockId = Guid.Empty;
                }
                var jcActivityData = ZPHSV.Instance.GetItemsListByActivityId((Guid)checkCommodityDto.JcActivityId);
                if (jcActivityData != null && jcActivityData.Data != null && jcActivityData.Data.Count > 0)
                {
                    var itemsList = jcActivityData.Data.Where(t => t.ComdtyStockId == checkCommodityDto.CommodityStockId && t.ComdtyId == checkCommodityDto.Id).ToList();
                    LogHelper.Debug("itemsList:" + JsonHelper.JsSerializer(itemsList));
                    if (itemsList.Any())
                    {
                        checkCommodityDto.DiscountPrice = Convert.ToDecimal(itemsList.FirstOrDefault().GroupPrice.ToString("0.00"));
                        checkCommodityDto.Price = Convert.ToDecimal(itemsList.FirstOrDefault().GroupPrice.ToString("0.00"));
                    }
                }


            }

            if (commodityList.Count != shoppingCartItemIds.Count)
            {
                foreach (var s in shoppingCartItemIds)
                {
                    if (commodityList.FirstOrDefault(t => t.ShoppingCartItemId == s) == null)
                    {
                        commodityList.Add(new CheckShopCommodityDTO
                        {
                            ShopCartState = ShopCartStateEnum.Del,
                            ShoppingCartItemId = s
                        });
                    }
                }
            }
            return commodityList;
        }

        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        public List<CheckCommodityDTO> CheckCommodityV5Ext(CheckCommodityParam ccp)
        {
            using (StopwatchLogHelper.BeginScope("CommoditySV.CheckCommodityV5"))
            {
                var selecid = ccp.CommodityIdsList.Select(r => r.CommodityId).ToList();
                var cStockids = ccp.CommodityIdsList.Where(c => c.CommodityStockId.HasValue && c.CommodityStockId != Guid.Empty).Select(r => r.CommodityStockId).Distinct().ToList();
                var coms = (
                                     from c in Commodity.ObjectSet()
                                     where c.CommodityType == 0 && selecid.Contains(c.Id)
                                     select new
                                     {
                                         Id = c.Id,
                                         Price = c.Price,
                                         State = c.IsDel ? 3 : c.State,
                                         Stock = c.Stock,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         AppId = c.AppId,
                                         ComAttribute = c.ComAttribute,
                                         Type = c.Type ?? 0
                                     }).ToList();
                var comStocks = CommodityStock.ObjectSet().Where(c => cStockids.Contains(c.Id)).Select(c => new Deploy.CommodityStockDTO
                {
                    Id = c.Id,
                    Stock = c.Stock,
                    Price = c.Price,
                    ComAttribute = c.ComAttribute
                }).ToList();

                //进货价验证和处理
                ValidCostPrice(selecid, cStockids.Where(a => a.HasValue).Select(a => a.Value).ToList());

                List<CheckCommodityDTO> commodityList = new List<CheckCommodityDTO>();
                foreach (var commodityIdAndStockId in ccp.CommodityIdsList)
                {
                    CheckCommodityDTO item = new CheckCommodityDTO()
                    {
                        Id = commodityIdAndStockId.CommodityId,
                        CommodityStockId = commodityIdAndStockId.CommodityStockId,
                        ColorAndSize = commodityIdAndStockId.ColorAndSize,
                        Specifications = commodityIdAndStockId.Specifications ?? 0
                    };
                    var com = coms.FirstOrDefault(c => c.Id == commodityIdAndStockId.CommodityId);
                    if (com == null)
                    {
                        item.State = 3;
                        commodityList.Add(item);
                        continue;
                    }

                    item.Type = com.Type;
                    item.Price = com.Price;
                    item.State = com.State;
                    item.Stock = com.Stock;
                    item.Intensity = 10;
                    item.DiscountPrice = -1;
                    item.OPrice = com.Price;
                    item.LimitBuyEach = -1;
                    item.LimitBuyTotal = -1;
                    item.SurplusLimitBuyTotal = 0;
                    item.IsEnableSelfTake = com.IsEnableSelfTake;
                    item.AppId = com.AppId;

                    if (Commodity.CheckComMultAttrs(com.ComAttribute))
                    {
                        if (!commodityIdAndStockId.CommodityStockId.HasValue ||
                            commodityIdAndStockId.CommodityStockId == Guid.Empty)
                        {
                            item.State = 4;
                            commodityList.Add(item);
                            continue;
                        }
                        var comStock = comStocks.FirstOrDefault(c => c.Id == commodityIdAndStockId.CommodityStockId);
                        if (comStock == null)
                        {
                            item.State = 4;
                            commodityList.Add(item);
                            continue;
                        }
                        item.Stock = comStock.Stock;
                        item.OPrice = comStock.Price;
                        item.Price = comStock.Price;
                    }
                    else
                    {
                        if (commodityIdAndStockId.CommodityStockId.HasValue && commodityIdAndStockId.CommodityStockId != Guid.Empty)
                        {
                            var comStock = comStocks.FirstOrDefault(c => c.Id == commodityIdAndStockId.CommodityStockId);
                            if (comStock == null)
                            {
                                item.State = 4;
                                commodityList.Add(item);
                                continue;
                            }
                            else
                            {
                                item.Stock = comStock.Stock;
                                item.OPrice = comStock.Price;
                                item.Price = comStock.Price;
                                commodityList.Add(item);
                                continue;
                            }
                            //item.State = 4;
                            //commodityList.Add(item);
                            //continue;
                        }
                        if (commodityIdAndStockId.ColorAndSize == null)
                            commodityIdAndStockId.ColorAndSize = string.Empty;
                        var arr = commodityIdAndStockId.ColorAndSize.Replace("null", "").Replace("nil", "").Replace("(null)", "").Replace("undefined", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
                        if (attrs == null || !attrs.Any())
                        {
                            if (arr.Any())
                            {
                                item.State = 4;
                                commodityList.Add(item);
                                continue;
                            }
                        }
                        else
                        {
                            if (!arr.Any())
                            {
                                item.State = 4;
                                commodityList.Add(item);
                                continue;
                            }
                            if (attrs.All(c => c.SecondAttribute != arr[0]))
                            {
                                item.State = 4;
                                commodityList.Add(item);
                                continue;
                            }
                        }
                    }
                    commodityList.Add(item);
                }

                DateTime now = DateTime.Now;
                //商品在每日促销表里集合 
                //var outPromotionIds = CommodityIdsList.Where(c => c.OutPrommotionId.HasValue && c.OutPrommotionId != Guid.Empty).Select(c => c.OutPrommotionId).Distinct().ToList();
                var promotionQuery = TodayPromotion.ObjectSet()
                    .Where(a => selecid.Contains(a.CommodityId) && a.EndTime > now && a.StartTime < now).
                       Select(a => new
                       {
                           ComId = a.CommodityId,
                           Intensity = a.Intensity,
                           DiscountPrice = a.DiscountPrice,
                           LimitBuyTotal = a.LimitBuyTotal,
                           SurplusLimitBuyTotal = a.SurplusLimitBuyTotal,
                           LimitBuyEach = a.LimitBuyEach,
                           PromotionId = a.PromotionId,
                           PromotionType = a.PromotionType,
                           OutSideid = a.OutsideId,
                           GroupMinVolume = a.GroupMinVolume,
                           ExpireSecond = a.ExpireSecond
                       });
                if (ccp.PromotionType == -1)
                {
                    promotionQuery = promotionQuery.Where(p => p.PromotionType == 0 || p.PromotionType == 1 || p.PromotionType == 2 || p.PromotionType == 5);
                }
                else if (ccp.PromotionType == 3)
                {
                    promotionQuery = promotionQuery.Where(p => p.PromotionType == 3);
                }
                var promotionDic = promotionQuery.Distinct().ToList();

                if (ccp.PromotionType == -1)
                {
                    foreach (var commodity in commodityList.Where(c => c.State == 0))
                    {
                        var stock = commodity.Stock;
                        var price = commodity.Price;
                        commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.NoPromotion;
                        var promotion = promotionDic.FirstOrDefault(c => c.ComId == commodity.Id && c.PromotionType != 3);
                        if (promotion == null || ccp.CommodityIdsList.All(c => c.CommodityId != commodity.Id))
                        {
                            continue;
                        }
                        commodity.ComPromotionStatusEnum = (ComPromotionStatusEnum)promotion.PromotionType;
                        int limie = -1;
                        int limitotal = -1;

                        if (promotion.PromotionType == 2 && promotion.OutSideid.HasValue)
                        {
                            var checkPresellInfoCDTO = new CheckPresellInfoCDTO()
                            {
                                id = promotion.OutSideid.Value,
                                comdtyId = promotion.ComId,
                                userId = ccp.UserID,
                                commodityStockId = commodity.CommodityStockId
                            };
                            try
                            {
                                var result = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetAndCheckPresellInfoById(checkPresellInfoCDTO);
                                if (result != null && result.id != Guid.Empty)
                                {
                                    commodity.IsNeedPresell = true;
                                    commodity.IsPreselled = result.isPreselled;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error(string.Format("CommoditySV.CheckCommodityNewExt.presellFacade.GetAndCheckPresellInfoById 异常。 checkPresellInfoCDTO：{0}", JsonHelper.JsonSerializer(checkPresellInfoCDTO)), ex);
                                continue;
                            }
                        }

                        commodity.SurplusLimitBuyTotal = Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                        if (promotion.LimitBuyEach != null && promotion.LimitBuyEach != -1)
                        {
                            var ul = UserLimited.ObjectSet().Where(n => n.UserId == ccp.UserID && n.PromotionId == promotion.PromotionId && n.CommodityId == commodity.Id).Select(s => s.Count).ToList();
                            if (ul.Count > 0)
                            {
                                limie = Convert.ToInt32(promotion.LimitBuyEach - ul.Sum());
                            }
                            else
                            {
                                limie = Convert.ToInt32(promotion.LimitBuyEach);
                            }
                            commodity.LimitBuyEach = Convert.ToInt32(promotion.LimitBuyEach);
                        }

                        if (promotion.LimitBuyTotal != null && promotion.LimitBuyTotal != -1)
                        {
                            limitotal = Convert.ToInt32(promotion.LimitBuyTotal) - Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                            commodity.LimitBuyTotal = Convert.ToInt32(promotion.LimitBuyTotal);
                        }
                        #region 判断 限购总数 每人限购 库存之间的关系
                        //如果只有每人限购 没有促销商品总量
                        if (limie != -1 && limitotal == -1)
                        {
                            if (commodity.Stock > limie)
                            {
                                commodity.Stock = limie;
                            }
                        }
                        else
                        {
                            //如果只有促销商品总量 没有每人限购 
                            if (limitotal != -1 && limie == -1)
                            {
                                if (commodity.Stock > limitotal)
                                {
                                    commodity.Stock = limitotal;
                                }
                            }
                            else
                            {
                                //如果都设置了 每人限购 促销商品总量
                                if (limitotal != -1 && limie != -1)
                                {
                                    if (limitotal > limie)
                                    {
                                        commodity.Stock = limie;
                                    }
                                    else
                                    {
                                        commodity.Stock = limitotal;
                                    }
                                }
                            }
                        }
                        #endregion
                        var discountPrice = promotion.DiscountPrice;
                        if (promotion.Intensity < 10)
                        {
                            discountPrice = -1;
                        }
                        if (discountPrice > -1)
                        {
                            commodity.Price = Convert.ToDecimal(promotion.DiscountPrice);
                            commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                        }
                        else
                        {
                            commodity.Price = Math.Round((commodity.Price * promotion.Intensity / 10), 2, MidpointRounding.AwayFromZero);
                            commodity.Intensity = promotion.Intensity;
                        }
                        //获取活动sku价格
                        LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.(Guid)promotion.OutSideid 值。 (Guid)promotion.OutSideid：{0}", (Guid)promotion.OutSideid));
                        //var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutSideid).Where(t => t.CommodityId == commodity.Id);
                        var skuActivityList = CacheHelper.ZPH.GetSkuActivityList((Guid)promotion.OutSideid).Where(t => t.CommodityId == commodity.Id);
                        if (skuActivityList.Any())
                        {
                            var commodityStockId = commodity.CommodityStockId;
                            if (string.IsNullOrEmpty(Convert.ToString(commodityStockId)) || commodityStockId == Guid.Empty)
                            {
                                commodityStockId = skuActivityList.ToList()[0].CommodityStockId;
                            }
                            LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.commodity.CommodityStockId 值。 commodity.CommodityStockId：{0}", commodityStockId));
                            var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodityStockId);
                            LogHelper.Debug(string.Format("CommoditySV.CheckCommodityNewExt2.skuActivity 值。 skuActivity：{0}", JsonHelper.JsonSerializer(skuActivity)));
                            if (skuActivity != null && skuActivity.IsJoin)
                            {
                                commodity.Price = Convert.ToDecimal(skuActivity.JoinPrice);
                                commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                                if (commodity.Intensity < 10)
                                {
                                    commodity.DiscountPrice = -1;
                                }
                            }
                            else
                            {
                                //不参与活动的 库存值不变 按照库存值做判断
                                commodity.Stock = stock;
                                commodity.LimitBuyEach = commodity.LimitBuyTotal = stock;
                                commodity.Price = commodity.DiscountPrice = price;
                            }
                        }
                    }

                    //循环取会员价
                    using (StopwatchLogHelper.BeginScope("GetVipIntensities"))
                    {
                        var appIds = commodityList.Where(c => c.State == 0).Select(c => c.AppId).Distinct().ToList();
                        var vipDict = AVMSV.GetVipIntensities(appIds, ccp.UserID);
                        foreach (var commodity in commodityList.Where(c => c.State == 0))
                        {
                            //会员价
                            VipPromotionDTO privilegeInfo = null;
                            if (vipDict.ContainsKey(commodity.AppId))
                                privilegeInfo = vipDict[commodity.AppId];

                            if (privilegeInfo != null && privilegeInfo.Intensity > 0)
                            {
                                var isdi = false;
                                var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2,
                                                                MidpointRounding.AwayFromZero);
                                var promotion = promotionDic.Where(c => c.ComId == commodity.Id).OrderByDescending(m => m.PromotionType).FirstOrDefault();
                                if (promotion == null || ccp.CommodityIdsList.All(c => c.CommodityId != commodity.Id))
                                {
                                    isdi = false;
                                }
                                else
                                {
                                    isdi = true;
                                }
                                //没有活动
                                if (isdi)
                                {

                                }
                                else
                                {
                                    if (privilegePrice < commodity.Price)
                                    {
                                        commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.VipIntensity;
                                        commodity.DiscountPrice = -1;
                                        commodity.Price = privilegePrice;
                                        commodity.Intensity = privilegeInfo.Intensity;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (ccp.PromotionType == 3)
                {
                    //检查拼团活动。
                    foreach (var commodity in commodityList.Where(c => c.State == 0))
                    {
                        var promotion = promotionDic.FirstOrDefault(c => c.ComId == commodity.Id && c.PromotionType == 3);
                        if (promotion == null || ccp.CommodityIdsList.All(c => c.CommodityId != commodity.Id))
                        {
                            continue;
                        }

                        commodity.DiscountPrice = promotion.DiscountPrice.HasValue ? promotion.DiscountPrice.Value : 0;
                        commodity.Price = commodity.DiscountPrice;
                        commodity.LimitBuyTotal = promotion.LimitBuyTotal.HasValue ? promotion.LimitBuyTotal.Value : 0;
                        commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal.HasValue ? promotion.SurplusLimitBuyTotal.Value : 0;
                        commodity.ComPromotionStatusEnum = ComPromotionStatusEnum.DiyGroup;
                        int limie = promotion.GroupMinVolume ?? 0;
                        #region  剩余可入团人数

                        if (ccp.DiygId != Guid.Empty)
                        {
                            var diyGroup = (from dg in DiyGroup.ObjectSet()
                                            where dg.Id == ccp.DiygId
                                            select dg).FirstOrDefault();
                            if (diyGroup == null || diyGroup.State != 1 || diyGroup.JoinNumber >= promotion.GroupMinVolume)
                            {
                                limie = 0;
                            }
                            else
                            {
                                limie = promotion.GroupMinVolume ?? 0 - diyGroup.JoinNumber;
                            }
                        }
                        #endregion

                        #region 距离参团结束剩余时间

                        //开团时间
                        DateTime diyBeginTime = (from dg in DiyGroup.ObjectSet()
                                                 where dg.Id == ccp.DiygId
                                                 select dg.SubTime).FirstOrDefault();
                        TimeSpan ts = DateTime.Now - diyBeginTime;
                        commodity.DiySecondSurplus = promotion.ExpireSecond.HasValue ? promotion.ExpireSecond.Value : 0;
                        commodity.DiySecondSurplus -= (int)ts.TotalSeconds;

                        #endregion

                        #region 库存，可购买商品总数量。
                        int limitotal = Convert.ToInt32(promotion.LimitBuyTotal) - Convert.ToInt32(promotion.SurplusLimitBuyTotal);
                        commodity.Stock = commodity.Stock >= limitotal ? limitotal : commodity.Stock;
                        commodity.Stock = Math.Min(limie, commodity.Stock);
                        #endregion

                        //获取活动sku价格
                        //var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutSideid);
                        var skuActivityList = CacheHelper.ZPH.GetSkuActivityList((Guid)promotion.OutSideid);
                        if (skuActivityList.Count > 0)
                        {
                            var skuActivity = skuActivityList.FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                            if (skuActivity != null && skuActivity.IsJoin)
                            {
                                commodity.Price = Convert.ToDecimal(skuActivity.JoinPrice);
                                commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice);
                            }
                        }
                    }
                }
                return commodityList;
            }
        }

        /// <summary>
        /// 处理进货价变更
        /// </summary>
        /// <param name="comIds"></param>
        /// <param name="comStockIds"></param>
        private void ValidCostPrice(List<Guid> comIds, List<Guid> comStockIds)
        {
            Action action = () =>
            {
                var dtS = DateTime.Now;
                var key = "ValidCostPrice";

                comIds = comIds ?? new List<Guid>();
                comStockIds = comStockIds ?? new List<Guid>();
                LogHelper.Info(key + ":1:comIds:" + JsonHelper.JsonSerializer(comIds) + ":comStockIds:" + JsonHelper.JsonSerializer(comStockIds));

                //缓存中已经处理过的数据               
                var hisIds = HttpRuntime.Cache.Get(key) as List<KeyValuePair<Guid, DateTime>>;
                hisIds = hisIds ?? new List<KeyValuePair<Guid, DateTime>>();

                if (hisIds.Count > 0)
                {
                    //LogHelper.Info(key + ":cache:hisIds:" + JsonHelper.JsonSerializer(hisIds));
                    hisIds = hisIds.Where(a => a.Value > DateTime.Now.AddMinutes(-1)).ToList();
                    var expIds = hisIds.Select(a => a.Key).ToList();
                    //LogHelper.Info(key + ":cache:expIds:" + JsonHelper.JsonSerializer(expIds));
                    comIds = comIds.Where(a => expIds.Contains(a) == false).ToList();
                    comStockIds = comStockIds.Where(a => expIds.Contains(a) == false).ToList();
                    //LogHelper.Info(key + ":cache:comIds:" + JsonHelper.JsonSerializer(comIds) + ":comStockIds:" + JsonHelper.JsonSerializer(comStockIds));

                    //无需要处理的数据
                    if (comIds.Count == 0 && comStockIds.Count == 0)
                    {
                        LogHelper.Info(key + ":缓存已处理");
                        return;
                    }
                }
                var comList = Commodity.ObjectSet().Where(c => comIds.Contains(c.Id)).ToList();
                List<CommodityStock> comStockList = null;
                if (comStockIds.Count == 0 && comList.Count > 0)
                {
                    comStockList = CommodityStock.ObjectSet().Where(c => comIds.Contains(c.CommodityId)).ToList();
                }
                else
                {
                    comStockList = CommodityStock.ObjectSet().Where(c => comStockIds.Contains(c.Id)).ToList();
                }
                comList = comList ?? new List<Commodity>();
                comStockList = comStockList ?? new List<CommodityStock>();

                var checkComIdsByJD = comList.Where(a => ThirdECommerceHelper.IsJingDongDaKeHu(a.AppId)).Select(a => a.Id).ToList();
                var checkComIdsBySN = comList.Where(a => ThirdECommerceHelper.IsSuNingYiGou(a.AppId)).Select(a => a.Id).ToList();
                var isCheckComIdsByJD = checkComIdsByJD != null && checkComIdsByJD.Count > 0;
                var isCheckComIdsBySN = checkComIdsBySN != null && checkComIdsBySN.Count > 0;
                List<JdPriceDto> jdPrictDtoList = new List<JdPriceDto>();
                List<Deploy.CustomDTO.SN.SNPriceDto> snPrictDtoList = new List<Deploy.CustomDTO.SN.SNPriceDto>();
                if (isCheckComIdsByJD)
                {
                    var skuList = comStockList.Where(a => checkComIdsByJD.Contains(a.CommodityId)).Select(a => a.JDCode).ToList();
                    skuList.AddRange(comList.Where(a => checkComIdsByJD.Contains(a.Id)).Select(a => a.JDCode).ToList());
                    skuList = skuList.Distinct().ToList();
                    if (skuList.Count == 0)
                    {
                        LogHelper.Info(key + ":jdskuList:" + JsonHelper.JsonSerializer(skuList));
                    }
                    else
                    {
                        jdPrictDtoList = JDSV.GetPrice(skuList);
                    }
                    LogHelper.Info(key + ":JDSV.GetPrice prictDtoList:" + JsonHelper.JsonSerializer(jdPrictDtoList));
                }
                if (isCheckComIdsBySN)
                {
                    var skuList = comStockList.Where(a => checkComIdsBySN.Contains(a.CommodityId)).Select(a => a.JDCode).ToList();
                    skuList.AddRange(comList.Where(a => checkComIdsBySN.Contains(a.Id)).Select(a => a.JDCode).ToList());
                    skuList = skuList.Distinct().ToList();
                    if (skuList.Count == 0)
                    {
                        LogHelper.Info(key + ":snskuList:" + JsonHelper.JsonSerializer(skuList));
                    }
                    else
                    {
                        snPrictDtoList = SuningSV.GetPrice(skuList);
                    }
                    LogHelper.Info(key + ":SuningSV.GetPrice prictDtoList:" + JsonHelper.JsonSerializer(snPrictDtoList));
                }

                decimal price = 0;
                decimal costPrice = 0;
                bool isHave = false;
                Action<Guid, string> actionPrice = (appId, skuId) =>
                 {
                     price = 0;
                     costPrice = 0;
                     isHave = false;

                     var isJD = ThirdECommerceHelper.IsJingDongDaKeHu(appId);
                     var isSN = ThirdECommerceHelper.IsSuNingYiGou(appId);
                     if (isJD)
                     {
                         var priceItem = jdPrictDtoList.FirstOrDefault(a => skuId == a.SkuId);
                         if (priceItem != null)
                         {
                             price = priceItem.JdPrice;
                             costPrice = priceItem.Price.HasValue ? priceItem.Price.Value : 0;
                             isHave = true;
                         }
                         else
                         {
                             isHave = false;
                         }
                     }
                     else if (isSN)
                     {
                         var priceItem = snPrictDtoList.FirstOrDefault(a => skuId == a.skuId);
                         if (priceItem != null)
                         {
                             price = Convert.ToDecimal(priceItem.snPrice);
                             costPrice = Convert.ToDecimal(priceItem.price);
                             isHave = true;
                         }
                         else
                         {
                             isHave = false;
                         }
                     }
                 };


                foreach (var comInfo in comList)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;

                    //有库存更新库存的价格，没库存更新商品的价格
                    var curComStockList = comStockList.Where(a => a.CommodityId == comInfo.Id).ToList();
                    if (curComStockList.Count > 0)
                    {
                        foreach (var comStock in curComStockList)
                        {
                            actionPrice(comInfo.AppId, comStock.JDCode);
                            if (comStock.CostPrice != costPrice && isHave)//进货价有变化
                            {
                                LogHelper.Info(key + ":3");
                                comStock.CostPrice = costPrice;
                                comStock.Price = price;
                                comStock.ModifiedOn = DateTime.Now;

                                //如果库存商品的价格比商品价格低，更新商品价格
                                if (comInfo.Price >= comStock.Price && comStock.Price > 0 && (comStock.Price >= comStock.CostPrice))
                                {
                                    LogHelper.Info(key + ":4");
                                    comInfo.Price = price;
                                    comInfo.CostPrice = costPrice;
                                    comInfo.ModifiedOn = DateTime.Now;
                                    comInfo.JDCode = comStock.JDCode;
                                    comInfo.RefreshCache(EntityState.Modified);//修改商品价格，更新商品缓存

                                    if (hisIds.Any(a => a.Key == comInfo.Id) == false)//加到已处理缓存列表
                                    {
                                        hisIds.Add(new KeyValuePair<Guid, DateTime>(comInfo.Id, DateTime.Now));
                                    }
                                }

                                if (hisIds.Any(a => a.Key == comStock.Id) == false)//加到已处理缓存列表
                                {
                                    hisIds.Add(new KeyValuePair<Guid, DateTime>(comStock.Id, DateTime.Now));
                                }
                            }
                        }
                    }
                    else
                    {
                        actionPrice(comInfo.AppId, comInfo.JDCode);
                        if (comInfo.CostPrice != costPrice && isHave)//进货价有变化
                        {
                            LogHelper.Info(key + ":2");
                            comInfo.Price = price;
                            comInfo.CostPrice = costPrice;
                            comInfo.ModifiedOn = DateTime.Now;
                            comInfo.RefreshCache(EntityState.Modified);//修改商品价格，更新商品缓存

                            if (hisIds.Any(a => a.Key == comInfo.Id) == false)//加到已处理缓存列表
                            {
                                hisIds.Add(new KeyValuePair<Guid, DateTime>(comInfo.Id, DateTime.Now));
                            }
                        }
                    }

                    contextSession.SaveChanges();
                }

                HttpRuntime.Cache.Add(key, hisIds, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                var dtE = DateTime.Now;
                LogHelper.Info(key + ":5:耗时: " + (dtE - dtS));
            };

            var task = Task.Factory.StartNew(action);
        }



        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListForCouponExt(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                if (search.couponType == 2 && search.CouponTemplateId != Guid.Empty)
                {
                    return GetCommodityByYJCouponId(search);
                }

                //if (search.CouponTemplateId != Guid.Empty)
                //{
                //    return GetCommodityByCouponId(search);
                //}

                if (!search.AppId.HasValue || search.AppId == Guid.Empty)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                DateTime now = DateTime.Now;

                var appId = search.AppId.Value;

                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()

                                      where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });

                if (!search.CategoryId.HasValue || search.CategoryId == new Guid("11111111-1111-1111-1111-111111111111"))
                {
                    //不限分类
                }
                else if (search.CategoryId == Guid.Empty)
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityCategory.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where
                                             ur.CategoryId == null && data1.Com.IsDel == false && data1.Com.State == 0 &&
                                             data1.Com.AppId == appId
                                         select data1;
                }
                else
                {
                    var categoryId = search.CategoryId.Value;

                    tempOcommodityList = (from c in tempOcommodityList
                                          join data1 in CommodityCategory.ObjectSet() on c.Com.Id equals data1.CommodityId
                                          where data1.CategoryId == categoryId && c.Com.AppId == appId && c.Com.IsDel == false && c.Com.State == 0
                                          select c);
                }

                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com);


                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();
                //价格排序 需要按照 折扣价格排序

                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {

                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.AppId == appId && c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.AppId == appId && c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:

                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.SubTime);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.SubTime);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }

                if (search.CouponTemplateId != Guid.Empty)
                {
                    #region 抵用券
                    // 抵用券
                    if (search.couponType == 2)
                    {
                        var yjCoupon = YJBSV.GetYJCouponInfo(search.CouponTemplateId);
                        LogHelper.Info("GetyjCoupon:" + JsonHelper.JsonSerializer(yjCoupon));
                        if (yjCoupon != null)
                        {
                            if (yjCoupon.CommodityIds == null || yjCoupon.CommodityIds.Count == 0)
                            {
                                if (yjCoupon.AppIds == null || yjCoupon.AppIds.Count == 0)
                                {
                                    var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Select(_ => _.AppId).Distinct().ToList();
                                    if (appIds != null && appIds.Count > 0)
                                        commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                                }
                                else
                                {
                                    if (yjCoupon.IsExcepted)
                                    {
                                        var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Where(_ => !yjCoupon.AppIds.Contains(_.AppId)).Select(_ => _.AppId).Distinct().ToList();
                                        commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                                    }
                                    else
                                    {
                                        commoditiesQuery = commoditiesQuery.Where(_ => yjCoupon.AppIds.Contains(_.AppId));
                                    }
                                }
                            }
                            else
                            {
                                commoditiesQuery = commoditiesQuery.Where(t => yjCoupon.CommodityIds.Contains(t.Id));
                            }
                        }
                    }
                    #endregion
                    else
                    {
                        //获取优惠券模板对应的商品id
                        Jinher.AMP.BTP.TPS.CouponSVFacade couponSvFacade = new Jinher.AMP.BTP.TPS.CouponSVFacade();
                        var comIds = couponSvFacade.GetCouponGoodsList(search.CouponTemplateId);
                        commoditiesQuery = commoditiesQuery.Where(t => comIds.Contains(t.Id));
                    }
                }

                comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

                if (commodities.Any())
                {
                    commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = c.Id,
                        Pic = c.PicturesPath,
                        Price = c.Price,
                        State = c.State,
                        Stock = c.Stock,
                        Name = c.Name,
                        MarketPrice = c.MarketPrice,
                        AppId = c.AppId,
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        ComAttribute = c.ComAttribute,
                        ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                    }).ToList();


                    #region 规格设置集合
                    commodityList.ForEach(s =>
                    {
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                        var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                        if (commoditySpecification.Count() > 0)
                        {
                            Guid commodityId = s.Id;
                            var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                            if (commoditySpecificationlist.Count() > 0)
                            {
                                commoditySpecificationlist.ForEach(p =>
                                {

                                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                    model.Id = p.Id;
                                    model.Name = "规格设置";
                                    model.Attribute = p.Attribute ?? 0;
                                    model.strAttribute = "1*" + p.Attribute + "";
                                    Specificationslist.Add(model);
                                });
                            }
                            s.Specifications = Specificationslist;
                        }
                    });
                    #endregion

                    var appList = commodities.Select(c => c.AppId).Distinct().ToList();

                    #region 众筹
                    if (CustomConfig.CrowdfundingFlag)
                    {
                        var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                        if (crowdFundingApps.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                    commodityList[i].IsActiveCrowdfunding = true;
                            }
                        }
                    }
                    #endregion

                    var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                    var comStockList = CommodityStock.ObjectSet()
                                      .Where(c => commodityIds.Contains(c.CommodityId))
                                      .Select(
                                          c =>
                                          new Deploy.CommodityStockDTO
                                          {
                                              Id = c.Id,
                                              CommodityId = c.CommodityId,
                                              Price = c.Price,
                                              MarketPrice = c.MarketPrice
                                          })
                                      .ToList();
                    var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                        var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);

                        if (todayPromotion != null)
                        {
                            commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                            commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                            commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                            commodity.PromotionType = todayPromotion.PromotionType;
                            if (todayPromotion.DiscountPrice > -1)
                            {
                                commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                commodity.Intensity = 10;
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = todayPromotion.Intensity;
                            }
                        }
                        else
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                        buildShowPrice(commodity, comStocks, todayPromotion);
                    }
                }
                try
                {

                    Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO appInfo = APPSV.Instance.GetAppDetailByIdInfo(appId, null);
                    if (appInfo != null)
                    {
                        if (commodityList != null && commodityList.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                commodityList[i].AppId = appId;
                                commodityList[i].AppName = appInfo.AppName;
                            }
                        }

                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                        var comdtyAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                        {
                            appId = appInfo.AppId,
                            appName = appInfo.AppName,
                            icon = appInfo.AppIcon
                        };
                        comdtyListResultCDTO.appInfoList.Add(comdtyAppInfoCDTO);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCommodityListV2Ext,获取app名称错误。appId：{0}", appId), ex);
                }

                comdtyListResultCDTO.comdtyList = commodityList.ToList();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            return comdtyListResultCDTO;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2Ext(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                if (search.couponType == 2 && search.CouponTemplateId != Guid.Empty)
                {
                    return GetCommodityByYJCouponId(search);
                }

                if (search.couponType == null && search.CouponTemplateId != Guid.Empty)
                {
                    return GetCommodityByCouponId(search);
                }
                if (!search.AppId.HasValue || search.AppId == Guid.Empty)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                DateTime now = DateTime.Now;

                var appId = search.AppId.Value;

                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()

                                      where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });

                if (!search.CategoryId.HasValue || search.CategoryId == new Guid("11111111-1111-1111-1111-111111111111"))
                {
                    //不限分类
                }
                else if (search.CategoryId == Guid.Empty)
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityCategory.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where
                                             ur.CategoryId == null && data1.Com.IsDel == false && data1.Com.State == 0 &&
                                             data1.Com.AppId == appId
                                         select data1;
                }
                else
                {
                    var categoryId = search.CategoryId.Value;

                    tempOcommodityList = (from c in tempOcommodityList
                                          join data1 in CommodityCategory.ObjectSet() on c.Com.Id equals data1.CommodityId
                                          where data1.CategoryId == categoryId && c.Com.AppId == appId && c.Com.IsDel == false && c.Com.State == 0
                                          select c);
                }

                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com);


                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();
                //价格排序 需要按照 折扣价格排序

                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {

                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.AppId == appId && c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.AppId == appId && c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:

                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.SubTime);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.SubTime);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }

                if (search.CouponTemplateId != Guid.Empty)
                {
                    // 抵用券
                    if (search.couponType == 2)
                    {
                        var yjCoupon = YJBSV.GetYJCouponInfo(search.CouponTemplateId);
                        LogHelper.Info("GetyjCoupon:" + JsonHelper.JsonSerializer(yjCoupon));
                        if (yjCoupon != null)
                        {
                            if (yjCoupon.CommodityIds == null || yjCoupon.CommodityIds.Count == 0)
                            {
                                if (yjCoupon.AppIds == null || yjCoupon.AppIds.Count == 0)
                                {
                                    var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Select(_ => _.AppId).Distinct().ToList();
                                    if (appIds != null && appIds.Count > 0)
                                        commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                                }
                                else
                                {
                                    if (yjCoupon.IsExcepted)
                                    {
                                        var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Where(_ => !yjCoupon.AppIds.Contains(_.AppId)).Select(_ => _.AppId).Distinct().ToList();
                                        commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                                    }
                                    else
                                    {
                                        commoditiesQuery = commoditiesQuery.Where(_ => yjCoupon.AppIds.Contains(_.AppId));
                                    }
                                }
                            }
                            else
                            {
                                commoditiesQuery = commoditiesQuery.Where(t => yjCoupon.CommodityIds.Contains(t.Id));
                            }
                        }
                    }
                    else
                    {
                        //获取优惠券模板对应的商品id
                        Jinher.AMP.BTP.TPS.CouponSVFacade couponSvFacade = new Jinher.AMP.BTP.TPS.CouponSVFacade();
                        var comIds = couponSvFacade.GetCouponGoodsList(search.CouponTemplateId);
                        commoditiesQuery = commoditiesQuery.Where(t => comIds.Contains(t.Id));
                    }
                }

                comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

                if (commodities.Any())
                {
                    commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = c.Id,
                        Pic = c.PicturesPath,
                        Price = c.Price,
                        State = c.State,
                        Stock = c.Stock,
                        Name = c.Name,
                        MarketPrice = c.MarketPrice,
                        AppId = c.AppId,
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        ComAttribute = c.ComAttribute,
                        ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                    }).ToList();

                    var mallAppInfo = MallApply.ObjectSet().FirstOrDefault(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId);
                    var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;
                    #region 规格设置集合
                    commodityList.ForEach(s =>
                    {
                        if (mallAppInfo != null)
                        {
                            s.MallType = mallAppInfo.Type;
                        }
                        var commodityCash = commodityCashes.Find(_ => _.CommodityId == s.Id);
                        if (commodityCash != null)
                        {
                            s.YJBAmount = commodityCash.YJBAmount;
                            s.YoukaAmount = commodityCash.YoukaAmount;
                        }
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                        var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                        if (commoditySpecification.Count() > 0)
                        {
                            Guid commodityId = s.Id;
                            var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                            if (commoditySpecificationlist.Count() > 0)
                            {
                                commoditySpecificationlist.ForEach(p =>
                                {

                                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                    model.Id = p.Id;
                                    model.Name = "规格设置";
                                    model.Attribute = p.Attribute ?? 0;
                                    model.strAttribute = "1*" + p.Attribute + "";
                                    Specificationslist.Add(model);
                                });
                            }
                            s.Specifications = Specificationslist;
                        }

                    });
                    #endregion

                    var appList = commodities.Select(c => c.AppId).Distinct().ToList();

                    #region 众筹
                    if (CustomConfig.CrowdfundingFlag)
                    {
                        var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                        if (crowdFundingApps.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                    commodityList[i].IsActiveCrowdfunding = true;
                            }
                        }
                    }
                    #endregion

                    var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                    var comStockList = CommodityStock.ObjectSet()
                                      .Where(c => commodityIds.Contains(c.CommodityId))
                                      .Select(
                                          c =>
                                          new Deploy.CommodityStockDTO
                                          {
                                              Id = c.Id,
                                              CommodityId = c.CommodityId,
                                              Price = c.Price,
                                              MarketPrice = c.MarketPrice
                                          })
                                      .ToList();
                    var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                        var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                        if (todayPromotion != null)
                        {
                            commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                            commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                            commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                            commodity.PromotionType = todayPromotion.PromotionType;

                            if (todayPromotion.DiscountPrice > -1)
                            {
                                commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                commodity.Intensity = 10;
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = todayPromotion.Intensity;
                            }


                        }
                        else
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                        buildShowPrice(commodity, comStocks, todayPromotion);
                    }

                }
                try
                {

                    Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO appInfo = APPSV.Instance.GetAppDetailByIdInfo(appId, null);
                    if (appInfo != null)
                    {
                        if (commodityList != null && commodityList.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                commodityList[i].AppId = appId;
                                commodityList[i].AppName = appInfo.AppName;
                            }
                        }

                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                        var comdtyAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                        {
                            appId = appInfo.AppId,
                            appName = appInfo.AppName,
                            icon = appInfo.AppIcon
                        };

                        comdtyListResultCDTO.appInfoList.Add(comdtyAppInfoCDTO);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCommodityListV2Ext,获取app名称错误。appId：{0}", appId), ex);
                }
                foreach (var item in commodityList)
                {
                    item.Tags = GetCommodityTag(item);
                }
                comdtyListResultCDTO.comdtyList = commodityList.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            return comdtyListResultCDTO;
        }
        /// <summary>
        /// 获取商品标签
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private List<string> GetCommodityTag(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO item)
        {
            var list = new List<string>();

            if (item == null)
            {
                return list;
            }

            if (item.Stock == 0)
            {
                list.Add("售罄");
                return list;
            }

            if (item.MallType.HasValue && item.MallType.Value != 1)
            {
                list.Add("自营");
            }

            if (item.PromotionType >= 0 && item.PromotionType <= 7)
            {
                if (item.PromotionType == 0)
                {
                    list.Add("限时购");
                }
                else if (item.PromotionType == 1)
                {
                    list.Add("秒杀");
                }
                else if (item.PromotionType == 2)
                {
                    list.Add("预售");
                }
                else if (item.PromotionType == 5)
                {
                    list.Add("预售");
                }
                else if (item.PromotionType == 6)
                {
                    list.Add("赠品");
                }
                return list;
            }
            if (item.YJBAmount.HasValue && item.YJBAmount.Value > 0)
            {
                var amount = item.YJBAmount.HasValue ? Math.Round(item.YJBAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                list.Add("易捷币");
            }

            if (item.YoukaAmount.HasValue && item.YoukaAmount.Value > 0)
            {
                var amount = item.YoukaAmount.HasValue ? Math.Round(item.YoukaAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                list.Add("赠油卡");
            }
            if (item.PromotionType >= 0 && item.PromotionType <= 7)
            {
                if (item.PromotionType == 3)
                {
                    list.Add("拼团");
                }
                else if (item.PromotionType == 7)
                {
                    list.Add("套装");
                }
            }
            if (list.Count > 3)
            {
                list = list.Take(3).ToList();
            }

            return list;
        }

        /// <summary>
        ///店铺商品列表6月28日
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityList3Ext(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }
                if (!search.AppId.HasValue || search.AppId == Guid.Empty)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                DateTime now = DateTime.Now;

                var appId = search.AppId.Value;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()

                                      where c.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });
                var commoditiesQuery = tempOcommodityList.Select(c => c.Com);

                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();

                comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

                if (commodities.Any())
                {
                    commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = c.Id,
                        Pic = c.PicturesPath,
                        Price = c.Price,
                        State = c.State,
                        Stock = c.Stock,
                        Name = c.Name,
                        MarketPrice = c.MarketPrice,
                        AppId = c.AppId,
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        ComAttribute = c.ComAttribute,
                        ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                    }).ToList();
                    var appList = commodities.Select(c => c.AppId).Distinct().ToList();

                    var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                    var comStockList = CommodityStock.ObjectSet()
                                      .Where(c => commodityIds.Contains(c.CommodityId))
                                      .Select(
                                          c =>
                                          new Deploy.CommodityStockDTO
                                          {
                                              Id = c.Id,
                                              CommodityId = c.CommodityId,
                                              Price = c.Price,
                                              MarketPrice = c.MarketPrice
                                          })
                                      .ToList();
                    var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                        var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);

                        if (todayPromotion != null)
                        {
                            commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                            commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                            commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                            commodity.PromotionType = todayPromotion.PromotionType;
                            if (todayPromotion.DiscountPrice > -1)
                            {
                                commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                commodity.Intensity = 10;
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = todayPromotion.Intensity;
                            }
                        }
                        else
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                        buildShowPrice(commodity, comStocks, todayPromotion);
                    }

                }
                try
                {
                    Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO appInfo = APPSV.Instance.GetAppDetailByIdInfo(appId, null);
                    if (appInfo != null)
                    {
                        if (commodityList != null && commodityList.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                commodityList[i].AppId = appId;
                                commodityList[i].AppName = appInfo.AppName;
                            }
                        }

                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                        var comdtyAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                        {
                            appId = appInfo.AppId,
                            appName = appInfo.AppName,
                            icon = appInfo.AppIcon
                        };

                        comdtyListResultCDTO.appInfoList.Add(comdtyAppInfoCDTO);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCommodityListV2Ext,获取app名称错误。appId：{0}", appId), ex);
                }
                comdtyListResultCDTO.comdtyList = commodityList.ToList();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            return comdtyListResultCDTO;
        }
        //新添加的用于海淀阳光餐饮用
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV3Ext(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                if (search.couponType == 2 && search.CouponTemplateId != Guid.Empty)
                {
                    return GetCommodityByYJCouponId(search);
                }


                if (!search.AppId.HasValue || search.AppId == Guid.Empty)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }
                DateTime now = DateTime.Now;
                var appId = search.AppId.Value;
                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                List<Guid> AppIds = new List<Guid>();
                string YangAppIds = CustomConfig.YangAppIds;
                JArray jarray = JArray.Parse(YangAppIds);
                if (jarray.Count > 0)
                {
                    foreach (var item in jarray)
                    {
                        JObject objwlgs = JObject.Parse(item.ToString());
                        if (appId == Guid.Parse(objwlgs["Key"].ToString()))
                        {
                            Array arry = objwlgs["Value"].ToString().Split(',').ToArray();
                            foreach (var _item in arry)
                            {
                                AppIds.Add(Guid.Parse(_item.ToString()));
                            }
                        }
                    }
                }
                if (AppIds.Count() == 0)
                {
                    AppIds.Add(appId);
                }

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()

                                      where AppIds.Contains(c.AppId) && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });

                if (!search.CategoryId.HasValue || search.CategoryId == new Guid("11111111-1111-1111-1111-111111111111"))
                {
                    //不限分类
                }
                else if (search.CategoryId == Guid.Empty)
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityCategory.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where
                                             ur.CategoryId == null && data1.Com.IsDel == false && data1.Com.State == 0 &&
                                             data1.Com.AppId == appId
                                         select data1;
                }
                else
                {
                    var categoryId = search.CategoryId.Value;

                    tempOcommodityList = (from c in tempOcommodityList
                                          join data1 in CommodityCategory.ObjectSet() on c.Com.Id equals data1.CommodityId
                                          where data1.CategoryId == categoryId && c.Com.AppId == appId && c.Com.IsDel == false && c.Com.State == 0
                                          select c);
                }

                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com);


                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();
                //价格排序 需要按照 折扣价格排序

                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {

                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.AppId == appId && c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.AppId == appId && c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:

                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.SubTime);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.SubTime);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }

                if (search.CouponTemplateId != Guid.Empty)
                {
                    // 抵用券
                    if (search.couponType == 2)
                    {
                        var yjCoupon = YJBSV.GetYJCouponInfo(search.CouponTemplateId);
                        LogHelper.Info("GetyjCoupon:" + JsonHelper.JsonSerializer(yjCoupon));
                        if (yjCoupon != null)
                        {
                            if (yjCoupon.CommodityIds == null || yjCoupon.CommodityIds.Count == 0)
                            {
                                if (yjCoupon.AppIds == null || yjCoupon.AppIds.Count == 0)
                                {
                                    var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Select(_ => _.AppId).Distinct().ToList();
                                    if (appIds != null && appIds.Count > 0)
                                        commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                                }
                                else
                                {
                                    if (yjCoupon.IsExcepted)
                                    {
                                        var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Where(_ => !yjCoupon.AppIds.Contains(_.AppId)).Select(_ => _.AppId).Distinct().ToList();
                                        commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                                    }
                                    else
                                    {
                                        commoditiesQuery = commoditiesQuery.Where(_ => yjCoupon.AppIds.Contains(_.AppId));
                                    }
                                }
                            }
                            else
                            {
                                commoditiesQuery = commoditiesQuery.Where(t => yjCoupon.CommodityIds.Contains(t.Id));
                            }
                        }
                    }
                    else
                    {
                        //获取优惠券模板对应的商品id
                        Jinher.AMP.BTP.TPS.CouponSVFacade couponSvFacade = new Jinher.AMP.BTP.TPS.CouponSVFacade();
                        var comIds = couponSvFacade.GetCouponGoodsList(search.CouponTemplateId);
                        commoditiesQuery = commoditiesQuery.Where(t => comIds.Contains(t.Id));
                    }
                }

                comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

                if (commodities.Any())
                {
                    commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = c.Id,
                        Pic = c.PicturesPath,
                        Price = c.Price,
                        State = c.State,
                        Stock = c.Stock,
                        Name = c.Name,
                        MarketPrice = c.MarketPrice,
                        AppId = c.AppId,
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        ComAttribute = c.ComAttribute,
                        ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                    }).ToList();


                    #region 规格设置集合
                    commodityList.ForEach(s =>
                    {
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                        var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                        if (commoditySpecification.Count() > 0)
                        {
                            Guid commodityId = s.Id;
                            var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                            if (commoditySpecificationlist.Count() > 0)
                            {
                                commoditySpecificationlist.ForEach(p =>
                                {

                                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO
                                    {
                                        Id = p.Id,
                                        Name = "规格设置",
                                        Attribute = p.Attribute ?? 0,
                                        strAttribute = "1*" + p.Attribute + ""
                                    };
                                    Specificationslist.Add(model);
                                });
                            }
                            s.Specifications = Specificationslist;
                        }

                    });
                    #endregion

                    var appList = commodities.Select(c => c.AppId).Distinct().ToList();

                    #region 众筹
                    if (CustomConfig.CrowdfundingFlag)
                    {
                        var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                        if (crowdFundingApps.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                    commodityList[i].IsActiveCrowdfunding = true;
                            }
                        }
                    }
                    #endregion

                    var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                    var comStockList = CommodityStock.ObjectSet()
                                      .Where(c => commodityIds.Contains(c.CommodityId))
                                      .Select(
                                          c =>
                                          new Deploy.CommodityStockDTO
                                          {
                                              Id = c.Id,
                                              CommodityId = c.CommodityId,
                                              Price = c.Price,
                                              MarketPrice = c.MarketPrice
                                          })
                                      .ToList();
                    var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                        var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);

                        if (todayPromotion != null)
                        {
                            commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                            commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                            commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                            commodity.PromotionType = todayPromotion.PromotionType;
                            if (todayPromotion.DiscountPrice > -1)
                            {
                                commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                commodity.Intensity = 10;
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = todayPromotion.Intensity;
                            }


                        }
                        else
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                        buildShowPrice(commodity, comStocks, todayPromotion);
                    }

                }
                try
                {

                    Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO appInfo = APPSV.Instance.GetAppDetailByIdInfo(appId, null);
                    if (appInfo != null)
                    {
                        if (commodityList != null && commodityList.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                commodityList[i].AppId = appId;
                                commodityList[i].AppName = appInfo.AppName;
                            }
                        }

                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                        var comdtyAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                        {
                            appId = appInfo.AppId,
                            appName = appInfo.AppName,
                            icon = appInfo.AppIcon
                        };
                        comdtyListResultCDTO.appInfoList.Add(comdtyAppInfoCDTO);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCommodityListV2Ext,获取app名称错误。appId：{0}", appId), ex);
                }

                comdtyListResultCDTO.comdtyList = commodityList.ToList();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            return comdtyListResultCDTO;
        }

        private Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityByYJCouponId(CommodityListSearchDTO search)
        {

            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                DateTime now = DateTime.Now;
                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()

                                      where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });


                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com);


                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();
                //价格排序 需要按照 折扣价格排序

                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {

                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:

                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.SubTime);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.SubTime);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }


                var yjCoupon = YJBSV.GetYJCouponInfo(search.CouponTemplateId);
                LogHelper.Info("GetyjCoupon:" + JsonHelper.JsonSerializer(yjCoupon));
                if (yjCoupon != null)
                {
                    if (yjCoupon.CommodityIds == null || yjCoupon.CommodityIds.Count == 0)
                    {
                        if (yjCoupon.AppIds == null || yjCoupon.AppIds.Count == 0)
                        {
                            var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Select(_ => _.AppId).Distinct().ToList();
                            if (appIds != null && appIds.Count > 0)
                                commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                        }
                        else
                        {
                            if (yjCoupon.IsExcepted)
                            {
                                var appIds = MallApply.GetTGQuery(yjCoupon.EsAppId).Where(_ => !yjCoupon.AppIds.Contains(_.AppId)).Select(_ => _.AppId).Distinct().ToList();
                                commoditiesQuery = commoditiesQuery.Where(_ => appIds.Contains(_.AppId));
                            }
                            else
                            {
                                commoditiesQuery = commoditiesQuery.Where(_ => yjCoupon.AppIds.Contains(_.AppId));
                            }
                        }
                    }
                    else
                    {
                        commoditiesQuery = commoditiesQuery.Where(t => yjCoupon.CommodityIds.Contains(t.Id));
                    }

                    comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                    commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

                    if (commodities.Any())
                    {
                        commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                        {
                            Id = c.Id,
                            Pic = c.PicturesPath,
                            Price = c.Price,
                            State = c.State,
                            Stock = c.Stock,
                            Name = c.Name,
                            MarketPrice = c.MarketPrice,
                            AppId = c.AppId,
                            IsEnableSelfTake = c.IsEnableSelfTake,
                            ComAttribute = c.ComAttribute,
                            ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                        }).ToList();

                        var mallAppInfo = MallApply.ObjectSet().FirstOrDefault(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == search.AppId);
                        var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;
                        #region 规格设置集合
                        commodityList.ForEach(s =>
                        {
                            if (mallAppInfo != null)
                            {
                                s.MallType = mallAppInfo.Type;
                            }
                            var commodityCash = commodityCashes.Find(_ => _.CommodityId == s.Id);
                            if (commodityCash != null)
                            {
                                s.YJBAmount = commodityCash.YJBAmount;
                                s.YoukaAmount = commodityCash.YoukaAmount;
                            }
                            List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                            var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                            if (commoditySpecification.Count() > 0)
                            {
                                Guid commodityId = s.Id;
                                var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                                if (commoditySpecificationlist.Count() > 0)
                                {
                                    commoditySpecificationlist.ForEach(p =>
                                    {

                                        Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO
                                        {
                                            Id = p.Id,
                                            Name = "规格设置",
                                            Attribute = p.Attribute ?? 0,
                                            strAttribute = "1*" + p.Attribute + ""
                                        };
                                        Specificationslist.Add(model);
                                    });
                                }
                                s.Specifications = Specificationslist;
                            }

                        });
                        #endregion

                        var appList = commodities.Select(c => c.AppId).Distinct().ToList();

                        #region 众筹
                        if (CustomConfig.CrowdfundingFlag)
                        {
                            var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                            if (crowdFundingApps.Any())
                            {
                                for (int i = 0; i < commodityList.Count; i++)
                                {
                                    if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                        commodityList[i].IsActiveCrowdfunding = true;
                                }
                            }
                        }
                        #endregion

                        var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                        var comStockList = CommodityStock.ObjectSet()
                            .Where(c => commodityIds.Contains(c.CommodityId))
                            .Select(c =>
                                new Deploy.CommodityStockDTO
                                {
                                    Id = c.Id,
                                    CommodityId = c.CommodityId,
                                    Price = c.Price,
                                    MarketPrice = c.MarketPrice
                                }).ToList();

                        var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);
                        foreach (var commodity in commodityList)
                        {
                            commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                            List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                            var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);

                            if (todayPromotion != null)
                            {
                                commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                                commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                                commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                                commodity.PromotionType = todayPromotion.PromotionType;

                                if (todayPromotion.DiscountPrice > -1)
                                {
                                    commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                    commodity.Intensity = 10;
                                }
                                else
                                {
                                    commodity.DiscountPrice = -1;
                                    commodity.Intensity = todayPromotion.Intensity;
                                }
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = 10;
                                commodity.LimitBuyEach = -1;
                                commodity.LimitBuyTotal = -1;
                                commodity.SurplusLimitBuyTotal = -1;
                                commodity.PromotionType = 9999;
                            }
                            buildShowPrice(commodity, comStocks, todayPromotion);
                        }
                    }

                    if (commodityList != null && commodityList.Any())
                    {
                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                        var appInfoes = APPSV.Instance.GetAppListByIdsInfo(commodityList.Select(_ => _.AppId).ToList());
                        foreach (var com in commodityList)
                        {
                            var appInfo = appInfoes.FirstOrDefault(_ => _.AppId == com.AppId);
                            if (appInfo != null)
                            {
                                com.AppName = appInfo.AppName;
                                comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                                {
                                    appId = appInfo.AppId,
                                    appName = appInfo.AppName,
                                    icon = appInfo.AppIcon
                                });
                            }
                        }

                        if (comdtyListResultCDTO.appInfoList.Count > 0)
                        {
                            comdtyListResultCDTO.appInfoList[0].appName = "易捷抵用券凑单";
                        }
                    }
                    foreach (var item in commodityList)
                    {
                        item.Tags = GetCommodityTag(item);
                    }
                    comdtyListResultCDTO.comdtyList = commodityList.ToList();
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            return comdtyListResultCDTO;
        }

        //xiexg2018-01-13
        /// <summary>
        /// 优惠券接口
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2_NewExt(CommodityListSearchDTO search)
        {
            //CommodityListSearchDTO search1 = new CommodityListSearchDTO();
            //search1.couponType = 2;
            //search1.PageIndex = 0;
            //search1.PageSize = 100;
            //search1.FieldSort = 1;
            //search1.OrderState = 1;
            //search1.IsHasStock = false;
            //search1.OrderState = 0;
            //search1.CouponTemplateId = Guid.Parse("F11719D9-C084-E87D-A033-7A49140F3617");
            //search = search1;
            LogHelper.Debug(string.Format("优惠券商品进入 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex < 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                if (search.couponType == 2 || search.couponType == 801)
                {
                    LogHelper.Debug(string.Format("优惠券商品进入抵用券 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                    search.couponType = 2;
                    return GetCommodityByYJCouponId_New(search);
                }
                else
                {
                    LogHelper.Debug(string.Format("优惠券商品进入优惠券 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                    return GetCommodityByCouponId_New(search);
                }

                //if (search.couponType ==1)
                //{
                //    return GetCommodityByCouponId_New(search);
                //}
                return null;
                DateTime now = DateTime.Now;
                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;
                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()
                                      where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });
                Guid esAppId = YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                //易捷北京的自营或者门店自营
                if (search.MallAppType != null && search.MallAppType != 0)
                {
                    List<Guid> mallApply = new List<Guid>();
                    if (search.MallAppType == 2)//自营
                    {
                        mallApply = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && (t.Type == 0 || t.Type == 2 || t.Type == 3)).Select(o => o.AppId).Distinct().ToList();
                    }
                    if (search.MallAppType == 1)//第三方
                    {
                        mallApply = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && t.Type == 1).Select(o => o.AppId).Distinct().ToList();
                    }
                    if (mallApply != null && mallApply.Count > 0)
                    {
                        tempOcommodityList = tempOcommodityList.Where(_ => mallApply.Contains(_.Com.AppId));
                    }

                }
                if (search.CategoryIdList != null && search.CategoryIdList.Count > 0)//分类
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityCategory.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where (from f in search.CategoryIdList select f).Contains(ur.CategoryId)
                                         select data1;
                }
                if (search.BrandList != null && search.BrandList.Count > 0)//品牌
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityInnerBrand.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where (from f in search.BrandList select f).Contains(ur.BrandId)
                                         select data1;
                }
                if (search.AppIdList != null && search.AppIdList.Count > 0)//商铺
                {
                    // tempOcommodityList = tempOcommodityList.Where(p => search.AppIdList.Any(g => p.Com.AppId == g));
                    tempOcommodityList = tempOcommodityList.Where(_ => search.AppIdList.Contains(_.Com.AppId));
                }
                if (search.FieldWhere != null && search.FieldWhere != "")//商品名称的筛选
                {
                    tempOcommodityList = tempOcommodityList.Where(o => o.Com.Name.Contains(search.FieldWhere));
                }

                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com);


                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();
                //价格排序 需要按照 折扣价格排序

                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {

                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.YoukaPercent);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.YoukaPercent);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }
                #region
                List<Guid> comIds = new List<Guid>();
                //获取优惠券模板对应的商品id
                Jinher.AMP.BTP.TPS.CouponSVFacade couponSvFacade = new Jinher.AMP.BTP.TPS.CouponSVFacade();
                if (search.CouponTemplateId == Guid.Empty)
                {
                    comIds = couponSvFacade.GetCouponGoodsListByType();
                }
                else
                {
                    comIds = couponSvFacade.GetCouponGoodsList(search.CouponTemplateId);
                }
                commoditiesQuery = commoditiesQuery.Where(t => comIds.Contains(t.Id));
                #endregion
                List<Guid> appList = commoditiesQuery.Select(o => o.AppId).Distinct().ToList();
                List<Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO> appInfoList = APPSV.GetAppListByIds(appList, null);
                comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                if (search.PageIndex == 0)
                {
                    commodities = commoditiesQuery.Skip((0) * search.PageSize).Take(search.PageSize).ToList();
                }
                else
                {
                    commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                }

                if (commodities.Any())
                {
                    commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                    {
                        Id = c.Id,
                        Pic = c.PicturesPath,
                        Price = c.Price,
                        State = c.State,
                        Stock = c.Stock,
                        Name = c.Name,
                        MarketPrice = c.MarketPrice,
                        AppId = c.AppId,
                        IsEnableSelfTake = c.IsEnableSelfTake,
                        ComAttribute = c.ComAttribute,
                        ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                    }).ToList();

                    // var mallAppInfo = MallApply.ObjectSet().FirstOrDefault(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == appId);
                    var mallAppInfolist = MallApply.ObjectSet().Where(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                    YJB.Deploy.CustomDTO.CommodityCashInput input = new YJB.Deploy.CustomDTO.CommodityCashInput();
                    input.CommodityIds = commodityList.Select(_ => _.Id).ToList();
                    var commodityCashes1 = YJBSV.GetCommodityCashPercent(input);
                    List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashOutput> commodityCashes = new List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashOutput>();
                    if (commodityCashes1 != null)
                    {
                        commodityCashes = commodityCashes1.Data;
                    }
                    //var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;
                    int a = 0;
                    #region 规格设置集合
                    commodityList.ForEach(s =>
                    {
                        if (mallAppInfolist != null && s.AppId != null)
                        {
                            var mallAppInfo = mallAppInfolist.FirstOrDefault(_ => _.AppId == s.AppId);
                            if (mallAppInfo != null)
                            {
                                s.MallType = mallAppInfo.Type;
                            }
                        }
                        var commodityCash = commodityCashes.Find(_ => _.CommodityId == s.Id);
                        if (commodityCash != null)
                        {
                            s.YJBAmount = commodityCash.YJBAmount;
                            s.YoukaAmount = commodityCash.YoukaAmount;
                        }
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                        var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                        if (commoditySpecification.Count() > 0)
                        {
                            Guid commodityId = s.Id;
                            var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                            if (commoditySpecificationlist.Count() > 0)
                            {
                                commoditySpecificationlist.ForEach(p =>
                                {

                                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                    model.Id = p.Id;
                                    model.Name = "规格设置";
                                    model.Attribute = p.Attribute ?? 0;
                                    model.strAttribute = "1*" + p.Attribute + "";
                                    Specificationslist.Add(model);
                                });
                            }
                            s.Specifications = Specificationslist;
                        }

                    });
                    #endregion

                    #region 众筹
                    if (CustomConfig.CrowdfundingFlag)
                    {
                        var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                        if (crowdFundingApps.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {
                                if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                    commodityList[i].IsActiveCrowdfunding = true;
                            }
                        }
                    }
                    #endregion

                    var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                    var comStockList = CommodityStock.ObjectSet()
                                      .Where(c => commodityIds.Contains(c.CommodityId))
                                      .Select(
                                          c =>
                                          new Deploy.CommodityStockDTO
                                          {
                                              Id = c.Id,
                                              CommodityId = c.CommodityId,
                                              Price = c.Price,
                                              MarketPrice = c.MarketPrice
                                          })
                                      .ToList();
                    var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);

                    foreach (var commodity in commodityList)
                    {
                        commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                        List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                        var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                        if (todayPromotion != null)
                        {
                            commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                            commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                            commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                            commodity.PromotionType = todayPromotion.PromotionType;

                            if (todayPromotion.DiscountPrice > -1)
                            {
                                commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                commodity.Intensity = 10;
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = todayPromotion.Intensity;
                            }


                        }
                        else
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = 10;
                            commodity.LimitBuyEach = -1;
                            commodity.LimitBuyTotal = -1;
                            commodity.SurplusLimitBuyTotal = -1;
                            commodity.PromotionType = 9999;
                        }
                        buildShowPrice(commodity, comStocks, todayPromotion);
                    }

                }
                try
                {

                    //  Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO appInfo = APPSV.Instance.GetAppDetailByIdInfo(appId, null);


                    if (appInfoList != null)
                    {
                        if (commodityList != null && commodityList.Any())
                        {
                            for (int i = 0; i < commodityList.Count; i++)
                            {

                                //commodityList[i].AppId = appId;
                                //commodityList[i].AppName = appInfo.AppName;

                                commodityList[i].AppName = appInfoList.Where(o => o.AppId == commodityList[i].Id).FirstOrDefault().AppName;//xiexg更改app改为列表形式

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("CommoditySV.GetCommodityListV2Ext_New,获取app名称错误。"), ex);
                }
                foreach (var item in commodityList)
                {
                    item.Tags = GetTags(item, true);
                    item.TagsSimple = GetTags(item, false);
                }
                comdtyListResultCDTO.comdtyList = commodityList.ToList();
                #region
                if (search.PageIndex == 0)//xiexg2018712第一次访问
                {
                    #region//品牌墙列表
                    List<Guid> comlist = commodityList.Select(o => o.Id).ToList();
                    var brandinner = CommodityInnerBrand.ObjectSet().Where(s => comlist.Contains(s.CommodityId)).Select(s => s.BrandId).ToList();
                    int index = 0;
                    var branlist = Brandwall.ObjectSet().Where(o => brandinner.Contains(o.Id));

                    branlist.OrderByDescending(o => o.SubTime).ToList();
                    comdtyListResultCDTO.BrandWallList = new List<BrandwallDTO>();
                    foreach (var obj in branlist.ToList())
                    {
                        index++;
                        var branAppInfoCDTO = new Jinher.AMP.BTP.Deploy.BrandwallDTO()
                        {
                            Id = obj.Id,
                            Name = obj.Brandname,
                        };
                        comdtyListResultCDTO.BrandWallList.Add(branAppInfoCDTO);
                        if (index > 30)
                        {
                            break;
                        }
                    }
                    index = 0;
                    #endregion
                    #region//分类列表
                    var Categoryinner = from s in CommodityCategory.ObjectSet()
                                        where comlist.Contains(s.CommodityId)
                                        select s.CategoryId;
                    var Categorylist = Category.ObjectSet().Where(o => Categoryinner.Contains(o.Id));

                    comdtyListResultCDTO.CategoryList = new List<CategoryDTO>();
                    Categorylist.OrderByDescending(o => o.SubTime);
                    foreach (var obj in Categorylist.ToList())
                    {
                        index++;
                        var CategoryAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CategoryDTO()
                        {
                            Id = obj.Id,
                            Name = obj.Name,
                        };
                        comdtyListResultCDTO.CategoryList.Add(CategoryAppInfoCDTO);
                        if (index > 30)
                        {
                            break;
                        }
                    }
                    #endregion
                    #region//店铺类型
                    var gettype = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && appList.Contains(t.AppId)).Select(o => o.Type).Distinct().ToList();
                    comdtyListResultCDTO.MallAppList = new List<int?>();
                    foreach (var obj in gettype)
                    {
                        comdtyListResultCDTO.MallAppList.Add(obj);
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            return comdtyListResultCDTO;
        }

        /// <summary>
        /// 获得标签
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isLableWithValues"></param>
        /// <returns></returns>
        public List<string> GetTags(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO item, bool isLableWithValues)
        {
            var list = new List<string>();

            if (item == null)
            {
                return list;
            }

            if (item.Stock == 0)
            {
                list.Add("售罄");
                return list;
            }

            if (item.MallType.HasValue && item.MallType.Value != 1)
            {
                list.Add("自营");
            }

            if (item.PromotionType >= 0 && item.PromotionType <= 7)
            {
                if (item.PromotionType == 0)
                {
                    list.Add("限时购");
                }
                else if (item.PromotionType == 1)
                {
                    list.Add("秒杀");
                }
                else if (item.PromotionType == 2)
                {
                    list.Add("预售");
                }
                else if (item.PromotionType == 5)
                {
                    list.Add("预售");
                }
                else if (item.PromotionType == 6)
                {
                    list.Add("赠品");
                }
                return list;
            }
            if (item.YJBAmount.HasValue && item.YJBAmount.Value > 0)
            {
                var amount = item.YJBAmount.HasValue ? Math.Round(item.YJBAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                if (isLableWithValues) list.Add("易捷币" + amount + "元");
                else
                    list.Add("易捷币");
            }

            if (item.YoukaAmount.HasValue && item.YoukaAmount.Value > 0)
            {
                var amount = item.YoukaAmount.HasValue ? Math.Round(item.YoukaAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                if (isLableWithValues) list.Add("赠油卡" + amount + "元");
                else
                    list.Add("赠油卡");
            }
            if (item.PromotionType >= 0 && item.PromotionType <= 7)
            {
                if (item.PromotionType == 3)
                {
                    list.Add("拼团");
                }
                else if (item.PromotionType == 7)
                {
                    list.Add("套装");
                }
            }
            if (list.Count > 3)
            {
                list = list.Take(3).ToList();
            }

            return list;
        }


        /// <summary>
        /// xiexg优惠券YJCouponIdnew
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityByYJCouponId_New(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();
            Jinher.AMP.YJB.Deploy.CustomDTO.YJCouponInfoDto yjCoupon = null;
            List<Jinher.AMP.YJB.Deploy.CustomDTO.YJCouponInfoDto> yjCoupon1 = null;

            if (search.CouponTemplateId != Guid.Empty)
            {
                yjCoupon = YJBSV.GetYJCouponInfo(search.CouponTemplateId);
                if (yjCoupon == null)
                {
                    return GetCommodityByCouponId_New(search);
                }
            }
            else
            {
                yjCoupon1 = YJBSV.GetYJCouponInfo().Distinct().ToList();
                if (yjCoupon1 == null)
                {
                    return GetCommodityByCouponId_New(search);
                }
            }
            if (search == null)
            {
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = 1;
                comdtyListResultCDTO.Message = "参数不能为空";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }

            if (search.PageSize <= 0)
            {
                search.PageSize = 20;
            }

            var temp = (from scc in CommodityCategory.ObjectSet()
                        join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                        join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                        where scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0 && sc.IsDel == false && sc.IsUse == true
                        orderby scc.MaxSort
                        select new
                        {
                            //esAppId = sc.AppId,
                            AppId = c.AppId,
                            CommodityId = c.Id,
                            CommodityName = c.Name,
                            CommodityPic = c.PicturesPath,
                            CommodityPrice = c.Price,
                            CommodityStock = c.Stock,
                            IsEnableSelfTake = c.IsEnableSelfTake,
                            MarketPrice = c.MarketPrice,
                            State = c.State,
                            Salesvolume = c.Salesvolume,
                            ComAttribute = c.ComAttribute,
                            //CategoryID = scc.CategoryId,
                            OrderWeight = c.OrderWeight ?? 0,
                            YouKaPercent = c.YoukaPercent,
                        }).Distinct();

            List<Guid> appIdSelect;
            Guid esAppId = YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
            if (search.MallAppType > 0)
            {
                var typeList = search.MallAppType == 1 ? new List<int> { 1 } : new List<int> { 0, 2, 3 };
                LogHelper.Info("手机端按商家类型查询商品: typeList" + JsonHelper.JsonSerializer(typeList) + "，DateTime: " + DateTime.Now);
                var mallApply = MallApply.ObjectSet().Where(t => typeList.Contains(t.Type) && t.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId).ToList();
                if (mallApply != null && mallApply.Count > 0)
                {
                    appIdSelect = (from scc in temp join m in MallApply.ObjectSet() on scc.AppId equals m.AppId select scc.AppId).ToList();
                    //tempOcommodityList = tempOcommodityList.Where(_ => mallApply.Contains(_.Com.AppId));

                    temp = temp.Where(o => appIdSelect.Contains(o.AppId));
                }
            }

            if (search.CategoryIdList != null && search.CategoryIdList.Count > 0)//分类
            {
                temp = (from c in temp join mc in CommodityCategory.ObjectSet() on c.CommodityId equals mc.CommodityId where search.CategoryIdList.Contains(mc.CategoryId) && mc.IsDel == false select c).Distinct();
            }

            if (search.BrandList != null && search.BrandList.Count > 0)//品牌
            {
                var comIdList = CommodityInnerBrand.ObjectSet().Where(o => search.BrandList.Contains(o.BrandId)).Select(o => o.CommodityId);
                temp = temp.Where(o => comIdList.Contains(o.CommodityId));
            }

            if (search.AppIdList != null && search.AppIdList.Count > 0)//商铺
            {
                temp = temp.Where(o => search.AppIdList.Contains(o.AppId));
            }

            if (search.FieldWhere != null && search.FieldWhere != "")//商品名称的筛选
            {
                temp = temp.Where(o => o.CommodityName.Contains(search.FieldWhere));
            }

            if (search.MinPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice >= search.MinPrice);
            }

            if (search.MaxPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice <= search.MaxPrice);
            }

            if (search.IsHasStock)
            {
                temp = temp.Where(p => p.CommodityStock > 0);
            }
            temp = temp.Distinct();

            switch (search.FieldSort)
            {
                case 2: //销量
                    temp = temp.OrderByDescending(t => t.Salesvolume); break;
                //temp = temp.OrderByDescending(t => t.Salesvolume).ThenBy(t => t.SetCategorySort);
                case 1:  //价格
                    switch (search.OrderState)
                    {
                        case 1:
                            temp = temp.OrderBy(t => t.CommodityPrice);
                            break;
                        default:
                            temp = temp.OrderByDescending(t => t.CommodityPrice);
                            break;
                    }
                    break;
                case 0:  //综合排序
                    switch (search.OrderState)
                    {
                        case 1:
                            temp = temp.OrderBy(t => t.OrderWeight); break;
                        default:
                            temp = temp.OrderByDescending(t => t.OrderWeight); break;
                    }
                    break;
                case 3:  //油卡排序
                    switch (search.OrderState)
                    {
                        case 1:
                            temp = temp.OrderBy(t => t.YouKaPercent); break;
                        default:
                            temp = temp.OrderByDescending(t => t.YouKaPercent); break;
                    }
                    break;
                default:
                    temp = temp.OrderByDescending(t => t.OrderWeight);
                    break;
            }


            if (yjCoupon1 != null || yjCoupon != null)
            {
                #region
                if (search.couponType == 2)
                {
                    if (search.CouponTemplateId == Guid.Empty)
                    {
                        if (yjCoupon1 != null && yjCoupon1.Count > 0)
                        {
                            List<Guid> couapplist = new List<Guid>();
                            List<Guid> comlist = new List<Guid>();
                            foreach (var obj in yjCoupon1)
                            {
                                if (obj != null)
                                {
                                    if (obj.CommodityIds == null || obj.CommodityIds.Count == 0)
                                    {
                                        if (obj.AppIds != null || obj.AppIds.Count != 0)
                                        {
                                            if (obj.IsExcepted)
                                            {
                                                var appIdsM = MallApply.GetTGQuery(esAppId).Where(_ => !obj.AppIds.Contains(_.AppId)).Select(_ => _.AppId).Distinct().ToList();
                                                couapplist.AddRange(appIdsM);
                                            }
                                            else
                                            {
                                                couapplist.AddRange(obj.AppIds);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        comlist.AddRange(obj.CommodityIds);
                                    }
                                }

                            }

                            if (comlist != null || comlist.Count != 0)
                            {
                                if (couapplist != null && couapplist.Count != 0)
                                {
                                    temp = temp.Where(t => comlist.Contains(t.CommodityId) || couapplist.Contains(t.AppId));
                                }
                                else
                                {
                                    temp = temp.Where(t => comlist.Contains(t.CommodityId));
                                }
                            }
                            else
                            {
                                if (couapplist != null && couapplist.Count != 0)
                                {
                                    temp = temp.Where(_ => couapplist.Contains(_.AppId));
                                }
                                else
                                {
                                    var appIdsM = MallApply.GetTGQuery(esAppId).Select(_ => _.AppId).Distinct().ToList();
                                    if (appIdsM != null && appIdsM.Count > 0)
                                        temp = temp.Where(_ => appIdsM.Contains(_.AppId));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (yjCoupon.CommodityIds == null || yjCoupon.CommodityIds.Count == 0)
                        {
                            if (yjCoupon.AppIds == null || yjCoupon.AppIds.Count == 0)
                            {
                                var appIdsM = MallApply.GetTGQuery(yjCoupon.EsAppId).Select(_ => _.AppId).Distinct().ToList();
                                if (appIdsM != null && appIdsM.Count > 0)
                                    temp = temp.Where(_ => appIdsM.Contains(_.AppId));
                            }
                            else
                            {
                                if (yjCoupon.IsExcepted)
                                {
                                    var appIdsM = MallApply.GetTGQuery(yjCoupon.EsAppId).Where(_ => !yjCoupon.AppIds.Contains(_.AppId)).Select(_ => _.AppId).Distinct().ToList();
                                    temp = temp.Where(_ => appIdsM.Contains(_.AppId));
                                }
                                else
                                {
                                    temp = temp.Where(_ => yjCoupon.AppIds.Contains(_.AppId));
                                }
                            }
                        }
                        else
                        {
                            temp = temp.Where(t => yjCoupon.CommodityIds.Contains(t.CommodityId));
                        }
                    }
                }
                #endregion
                comdtyListResultCDTO.realCount = temp.Count();
                List<Guid> appList = temp.Select(o => o.AppId).Distinct().ToList();

                var appInfoes = APPSV.Instance.GetAppListByIdsInfo(temp.Select(_ => _.AppId).Distinct().ToList());
                #region
                if (search.PageIndex == 0)//xiexg2018712第一次访问
                {
                    #region  //品牌墙列表
                    List<Guid> comlist = temp.Select(o => o.CommodityId).ToList();
                    var brandinner = CommodityInnerBrand.ObjectSet().Where(s => comlist.Contains(s.CommodityId)).Select(s => s.BrandId).ToList();
                    int index = 0;
                    var branlist = Brandwall.ObjectSet().Where(o => brandinner.Contains(o.Id));

                    branlist.OrderByDescending(o => o.SubTime).ToList();
                    comdtyListResultCDTO.BrandWallList = new List<BrandwallDTO>();
                    foreach (var obj in branlist.ToList())
                    {
                        index++;
                        var branAppInfoCDTO = new Jinher.AMP.BTP.Deploy.BrandwallDTO()
                        {
                            Id = obj.Id,
                            Name = obj.Brandname,
                        };
                        comdtyListResultCDTO.BrandWallList.Add(branAppInfoCDTO);
                        if (index > 30)
                        {
                            break;
                        }
                    }
                    index = 0;
                    #endregion
                    #region//分类列表                    
                    var Categoryinner = (from c in CommodityCategory.ObjectSet() where comlist.Contains(c.CommodityId) select c.CategoryId).Distinct();
                    var Categorylist = Category.ObjectSet().Where(o => Categoryinner.Contains(o.Id));

                    comdtyListResultCDTO.CategoryList = new List<CategoryDTO>();
                    Categorylist.OrderByDescending(o => o.SubTime);
                    foreach (var obj in Categorylist.ToList())
                    {
                        index++;
                        var CategoryAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CategoryDTO()
                        {
                            Id = obj.Id,
                            Name = obj.Name,
                        };
                        comdtyListResultCDTO.CategoryList.Add(CategoryAppInfoCDTO);
                        if (index > 30)
                        {
                            break;
                        }
                    }
                    #endregion
                    #region//店铺列表
                    comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                    foreach (var appinfo in appInfoes)
                    {
                        comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                        {
                            appId = appinfo.AppId,
                            appName = appinfo.AppName,
                            icon = appinfo.AppIcon
                        });
                    }
                    #endregion
                    #region//店铺类型
                    var gettype = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && appList.Contains(t.AppId)).Select(o => (int?)o.Type).Distinct().ToArray();
                    comdtyListResultCDTO.MallAppList = new List<int?>();
                    comdtyListResultCDTO.MallAppList.AddRange(gettype);
                    //foreach (var obj in gettype)
                    //{
                    //    comdtyListResultCDTO.MallAppList.Add(obj);
                    //}
                    #endregion

                    search.PageIndex = 1;
                }
                #endregion

                if (temp.Count() == 0)
                {
                    return GetCommodityByCouponId_New(search);
                }

                var now = DateTime.Now;
                var begigTime = DateTime.Now.AddYears(-20);
                var skuCommodityStockIds = new List<Guid>();
                var appIds = temp.Select(o => o.AppId).Distinct().ToList();               //appIdSelect.Distinct().ToList();
                var applist = APPSV.GetAppListByIds(appIds, null);

                temp = temp.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize);
                string listInfo = "";
                foreach (var entity in temp)
                {
                    listInfo += entity.CommodityId + ":{ComAttribute:" + entity.ComAttribute + ",AppId:" + entity.AppId + ",CommodityName:" + entity.CommodityName + ",CommodityPic:" + entity.CommodityPic + ",CommodityPrice:" + entity.CommodityPrice + ",CommodityStock:" + entity.CommodityStock + ",IsEnableSelfTake:" + entity.IsEnableSelfTake + ",MarketPrice:" + entity.MarketPrice + ",OrderWeight:" + entity.OrderWeight + ", Salesvolume:" + entity.Salesvolume + ",State:" + entity.State + ",YouKaPercent:" + entity.YouKaPercent + " },";
                }

                LogHelper.Debug(String.Format("GetCommodityByYJCouponId_New 搜索获得数据 {0}", listInfo));


                var cfAppIds = Crowdfunding.ObjectSet().Where(c => appIds.Contains(c.AppId) && c.StartTime < now && c.State == 0).Select(m => m.AppId).ToList();
                List<CommodityListCDTO> commodityListCdtos = new List<CommodityListCDTO>();
                if (temp.Any())
                {
                    //var mallAppInfo = MallApply.ObjectSet().FirstOrDefault(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == search.AppId);
                    var mallAppInfolist = MallApply.ObjectSet().Where(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                    //var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;
                    YJB.Deploy.CustomDTO.CommodityCashInput input = new YJB.Deploy.CustomDTO.CommodityCashInput();
                    input.CommodityIds = temp.Select(_ => _.CommodityId).ToList();
                    var commodityCashes1 = YJBSV.GetCommodityCashPercent(input);
                    List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashOutput> commodityCashes = new List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashOutput>();
                    if (commodityCashes1 != null)
                    {
                        commodityCashes = YJBSV.GetCommodityCashPercent(input).Data;
                    }
                    var comIds = temp.Select(c => c.CommodityId).Distinct().ToList();
                    var tempt = ZPHSV.Instance.GetAppIdlist(new List<Guid>() { YJB.Deploy.CustomDTO.YJBConsts.YJAppId });
                    var ids = tempt.Select(t => t.AppId).ToList();

                    var presents = PresentPromotionCommodity.ObjectSet().Where(_ => comIds.Contains(_.CommodityId))
                        .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime), pp => pp.PresentPromotionId,
                        ppc => ppc.Id, (c, p) => new { CommodityId = c.CommodityId, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime }).ToList();
                    var todayPromotion = (from p in PromotionItems.ObjectSet()
                                          join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                          where !pro.IsDel && pro.IsEnable && pro.EndTime >= now && ((pro.StartTime <= now && pro.StartTime > begigTime) || (pro.PresellStartTime <= now && pro.PresellStartTime > begigTime)) && pro.PromotionType != 3
                                          select new TodayPromotionDTO
                                          {
                                              PromotionId = p.PromotionId,
                                              CommodityId = p.CommodityId,
                                              Intensity = (decimal)p.Intensity,
                                              StartTime = pro.StartTime,
                                              EndTime = pro.EndTime,
                                              DiscountPrice = (decimal)p.DiscountPrice,
                                              LimitBuyEach = p.LimitBuyEach,
                                              LimitBuyTotal = p.LimitBuyTotal,
                                              SurplusLimitBuyTotal = p.SurplusLimitBuyTotal,
                                              AppId = pro.AppId,
                                              ChannelId = pro.ChannelId,
                                              OutsideId = pro.OutsideId,
                                              PresellStartTime = pro.PresellStartTime,
                                              PresellEndTime = pro.PresellEndTime,
                                              PromotionType = pro.PromotionType,
                                              GroupMinVolume = pro.GroupMinVolume,
                                              ExpireSecond = pro.ExpireSecond,
                                              Description = pro.Description
                                          });

                    foreach (var appSetCommodityDto in temp)
                    {
                        CommodityListCDTO commodityListCdto = new CommodityListCDTO
                        {
                            Id = appSetCommodityDto.CommodityId,
                            AppId = appSetCommodityDto.AppId,
                            IsEnableSelfTake = appSetCommodityDto.IsEnableSelfTake,
                            Name = appSetCommodityDto.CommodityName,
                            appId = appSetCommodityDto.AppId,
                            Pic = appSetCommodityDto.CommodityPic,
                            Price = appSetCommodityDto.CommodityPrice,
                            Stock = appSetCommodityDto.CommodityStock,
                            MarketPrice = appSetCommodityDto.MarketPrice,
                            State = appSetCommodityDto.State,
                            DiscountPrice = -1,
                            Intensity = 10,
                            LimitBuyEach = -1,
                            LimitBuyTotal = -1,
                            SurplusLimitBuyTotal = 0,
                            //SetCategorySort = appSetCommodityDto.SetCategorySort,
                            ComAttrType = (appSetCommodityDto.ComAttribute == "[]" || appSetCommodityDto.ComAttribute == null) ? 1 : 3,
                            //CategoryId = appSetCommodityDto.CategoryID,
                        };

                        #region 众筹
                        if (CustomConfig.CrowdfundingFlag)
                        {
                            if (cfAppIds.Any(c => c == commodityListCdto.AppId))
                                commodityListCdto.IsActiveCrowdfunding = true;
                        }
                        #endregion

                        #region 规格设置集合
                        List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                        var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                        if (commoditySpecification.Count() > 0)
                        {
                            var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityListCdto.Id).ToList();
                            if (commoditySpecificationlist.Count() > 0)
                            {
                                commoditySpecificationlist.ForEach(p =>
                                {
                                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                    model.Id = p.Id;
                                    model.Name = "规格设置";
                                    model.Attribute = p.Attribute ?? 0;
                                    model.strAttribute = "1*" + p.Attribute + "";
                                    Specificationslist.Add(model);
                                });
                            }
                        }
                        commodityListCdto.Specifications = Specificationslist;
                        #endregion

                        var promotionDic = todayPromotion.Where(a => a.CommodityId == appSetCommodityDto.CommodityId).FirstOrDefault();
                        if (promotionDic != null)
                        {
                            commodityListCdto.PromotionTypeNew = (int)(ComPromotionStatusEnum)promotionDic.PromotionType;
                            commodityListCdto.LimitBuyEach = promotionDic.LimitBuyEach;
                            commodityListCdto.LimitBuyTotal = promotionDic.LimitBuyTotal;
                            commodityListCdto.SurplusLimitBuyTotal = promotionDic.SurplusLimitBuyTotal;
                            commodityListCdto.PromotionType = promotionDic.PromotionType;
                            var dprice = Convert.ToDecimal(promotionDic.DiscountPrice);
                            if (promotionDic.DiscountPrice > -1)
                            {
                                commodityListCdto.Intensity = 10;
                                commodityListCdto.DiscountPrice = (decimal)promotionDic.DiscountPrice;
                            }
                            else
                            {
                                commodityListCdto.Intensity = promotionDic.Intensity;
                                commodityListCdto.DiscountPrice = -1;
                            }
                            commodityListCdto.LimitBuyEach = promotionDic.LimitBuyEach;
                            commodityListCdto.LimitBuyTotal = promotionDic.LimitBuyTotal;
                            commodityListCdto.SurplusLimitBuyTotal = promotionDic.SurplusLimitBuyTotal;

                            //commodityListCdto.PromotionStartTime = promotionDic.StartTime;
                            //commodityListCdto.PromotionEndTime = promotionDic.EndTime;
                            //commodityListCdto.PresellStartTime = promotionDic.PresellStartTime;
                            //commodityListCdto.PresellEndTime = promotionDic.PresellEndTime;
                            //commodityListCdto.PromotionId = promotionDic.PromotionId;
                            //commodityListCdto.OutPromotionId = promotionDic.OutsideId;

                            var skulist = ZPHSV.Instance.GetSkuActivityList((Guid)promotionDic.OutsideId);
                            //获取活动sku最小价格
                            if (promotionDic.OutsideId != null)
                            {
                                var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)promotionDic.OutsideId).Where(t => t.IsJoin && t.CommodityId == appSetCommodityDto.CommodityId);
                                var activitySkuComs = skuActivityList.Where(_ => _.OutSideActivityId == promotionDic.OutsideId.Value && _.CommodityId == promotionDic.CommodityId).ToList();

                                if (activitySkuComs.Any())
                                {
                                    dprice = skuActivityList.Min(t => t.JoinPrice);
                                    skuCommodityStockIds.Add(activitySkuComs.First().CommodityStockId);
                                    if (promotionDic.Intensity < 10)
                                    {
                                        commodityListCdto.DiscountPrice = -1;
                                    }

                                    if (dprice > -1)
                                    {
                                        commodityListCdto.DiscountPrice = dprice;
                                        commodityListCdto.Intensity = 10;
                                    }
                                    else
                                    {
                                        commodityListCdto.DiscountPrice = -1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            commodityListCdto.Intensity = 10;
                            commodityListCdto.DiscountPrice = -1;
                            commodityListCdto.PromotionType = 9999;
                        }

                        if (ids.Contains(commodityListCdto.AppId))
                        {
                            //获取分类id 处理检索出来包含移除分类的商品信息
                            var commodityCategory = CommodityCategory.ObjectSet().FirstOrDefault(t => t.CommodityId == commodityListCdto.Id && t.AppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                            if (commodityCategory != null) commodityListCdto.CategoryId = commodityCategory.CategoryId;
                        }

                        if (presents.Any(_ => _.CommodityId == commodityListCdto.Id))
                        {
                            commodityListCdto.PromotionTypeNew = 6;
                        }

                        //读取易捷优惠信息                
                        var comApp = MallApply.GetTGQuery(YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Where(_ => appIds.Contains(_.AppId) && _.AppId == commodityListCdto.AppId).FirstOrDefault();
                        if (comApp != null && comApp.Id != Guid.Empty)
                        {
                            commodityListCdto.MallType = comApp.Type;
                        }

                        var commodityCash = commodityCashes.Find(_ => _.CommodityId == commodityListCdto.Id);
                        if (commodityCash != null)
                        {
                            commodityListCdto.YJBAmount = commodityCash.YJBAmount;
                            commodityListCdto.YoukaAmount = commodityCash.YoukaAmount;
                        }

                        commodityListCdtos.Add(commodityListCdto);
                        commodityListCdto.Tags = GetCommodityTag(commodityListCdto, true);  //赋值标签
                        commodityListCdto.TagsSimple = GetCommodityTag(commodityListCdto, false);
                    }
                    comdtyListResultCDTO.MallAppList = (from n in commodityListCdtos select n.MallType).Distinct().ToList();
                    comdtyListResultCDTO.isSuccess = true;
                    comdtyListResultCDTO.Code = 0;
                    comdtyListResultCDTO.Message = "Success";
                    comdtyListResultCDTO.comdtyList = commodityListCdtos./*OrderByDescending(p=>p.SetCategorySort).*/ToList();
                    return comdtyListResultCDTO;
                }
                else
                {
                    return GetCommodityByCouponId_New(search);
                }
            }
            else
            {
                return GetCommodityByCouponId_New(search);
            }
        }


        /// <summary>
        /// 获取标签信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isLableWithValues"></param>
        /// <returns></returns>
        private List<string> GetCommodityTag(CommodityListCDTO item, bool isLableWithValues)
        {
            var list = new List<string>();

            if (item == null)
            {
                return list;
            }

            if (item.Stock == 0)
            {
                list.Add("售罄");
                return list;
            }

            if (item.MallType.HasValue && item.MallType.Value != 1)
            {
                list.Add("自营");
            }

            if (item.PromotionTypeNew.HasValue)
            {
                if (item.PromotionTypeNew == 0)
                {
                    list.Add("限时购");
                }
                else if (item.PromotionTypeNew == 1)
                {
                    list.Add("秒杀");
                }
                else if (item.PromotionTypeNew == 2)
                {
                    list.Add("预约");
                }
                else if (item.PromotionTypeNew == 3)
                {
                    list.Add("拼团");
                }
                else if (item.PromotionTypeNew == 5)
                {
                    list.Add("预售");
                }
                else if (item.PromotionTypeNew == 6)
                {
                    list.Add("赠品");
                }
                else if (item.PromotionTypeNew == 7)
                {
                    list.Add("套装");
                }
                return list;
            }


            if (item.YJBAmount.HasValue && item.YJBAmount.Value > 0)
            {
                var amount = item.YJBAmount.HasValue ? Math.Round(item.YJBAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                if (isLableWithValues) list.Add("易捷币" + amount + "元");
                else
                    list.Add("易捷币");
            }

            if (item.YoukaAmount.HasValue && item.YoukaAmount.Value > 0)
            {
                var amount = item.YoukaAmount.HasValue ? Math.Round(item.YoukaAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                if (isLableWithValues) list.Add("赠油卡" + amount + "元");
                else
                    list.Add("赠油卡");
                //list.Add("赠油卡" + amount + "元");
            }

            if (list.Count > 3)
            {
                list = list.Take(3).ToList();
            }

            return list;

        }


        // 优惠券商品列表，根据优惠券模板Id获取
        private Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityByCouponId(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO =
                new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();

            try
            {
                if (search == null || search.PageSize <= 0 || search.PageIndex <= 0)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 1;
                    comdtyListResultCDTO.Message = "参数不能为空";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }

                DateTime now = DateTime.Now;
                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()
                                      where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });


                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com);


                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();

                //价格排序 需要按照 折扣价格排序
                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:

                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.SubTime);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.SubTime);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }

                ListCouponNewRequestDTO lnr = new ListCouponNewRequestDTO();
                lnr.CouponIds = new List<Guid>() { search.CouponTemplateId };
                lnr.UserId = search.UserId;
                var coupon = CouponSV.Instance.GetUserCouponsStoresByIds(lnr);

                if (coupon != null && coupon.Data != null && coupon.Data.Count > 0)
                {
                    SpecifyStoreCoupon cnd = coupon.Data[0];

                    switch (cnd.CouponType)
                    {
                        case Jinher.AMP.Coupon.Deploy.Enum.CouponType.BeInCommon:   // 商城通用
                            var appIds = MallApply.GetTGQuery(cnd.EsAppId).Select(ma => ma.AppId).Distinct().ToList();
                            if (appIds != null && appIds.Count > 0)
                                commoditiesQuery = commoditiesQuery.Where(bc => appIds.Contains(bc.AppId));
                            break;
                        case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyGoods: // 指定商品
                            if (cnd.GoodList != null || cnd.GoodList.Count > 0)
                            {
                                commoditiesQuery = commoditiesQuery.Where(sg => cnd.GoodList.Contains(sg.Id));
                            }
                            break;
                        case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyStore: // 指定店铺
                            if (cnd.ShopIds != null && cnd.ShopIds.Count > 0)
                            {
                                commoditiesQuery = commoditiesQuery.Where(ss => cnd.ShopIds.Contains(ss.AppId));
                            }
                            break;
                    }

                    comdtyListResultCDTO.realCount = commoditiesQuery.Count();

                    commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

                    if (commodities.Any())
                    {
                        commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                        {
                            Id = c.Id,
                            Pic = c.PicturesPath,
                            Price = c.Price,
                            State = c.State,
                            Stock = c.Stock,
                            Name = c.Name,
                            MarketPrice = c.MarketPrice,
                            AppId = c.AppId,
                            IsEnableSelfTake = c.IsEnableSelfTake,
                            ComAttribute = c.ComAttribute,
                            ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                        }).ToList();

                        var mallAppInfo = MallApply.ObjectSet().FirstOrDefault(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == search.AppId);
                        var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;
                        #region 规格设置集合
                        commodityList.ForEach(s =>
                        {
                            if (mallAppInfo != null)
                            {
                                s.MallType = mallAppInfo.Type;
                            }
                            var commodityCash = commodityCashes.Find(_ => _.CommodityId == s.Id);
                            if (commodityCash != null)
                            {
                                s.YJBAmount = commodityCash.YJBAmount;
                                s.YoukaAmount = commodityCash.YoukaAmount;
                            }
                            List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                            var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                            if (commoditySpecification.Count() > 0)
                            {
                                Guid commodityId = s.Id;
                                var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                                if (commoditySpecificationlist.Count() > 0)
                                {
                                    commoditySpecificationlist.ForEach(p =>
                                    {

                                        Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                        model.Id = p.Id;
                                        model.Name = "规格设置";
                                        model.Attribute = p.Attribute ?? 0;
                                        model.strAttribute = "1*" + p.Attribute + "";
                                        Specificationslist.Add(model);
                                    });
                                }
                                s.Specifications = Specificationslist;
                            }

                        });
                        #endregion

                        var appList = commodities.Select(c => c.AppId).Distinct().ToList();

                        #region 众筹
                        if (CustomConfig.CrowdfundingFlag)
                        {
                            var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                            if (crowdFundingApps.Any())
                            {
                                for (int i = 0; i < commodityList.Count; i++)
                                {
                                    if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                        commodityList[i].IsActiveCrowdfunding = true;
                                }
                            }
                        }
                        #endregion

                        var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                        var comStockList = CommodityStock.ObjectSet()
                            .Where(c => commodityIds.Contains(c.CommodityId))
                            .Select(c =>
                                new Deploy.CommodityStockDTO
                                {
                                    Id = c.Id,
                                    CommodityId = c.CommodityId,
                                    Price = c.Price,
                                    MarketPrice = c.MarketPrice
                                }).ToList();

                        var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);
                        foreach (var commodity in commodityList)
                        {
                            commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                            List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                            var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);

                            if (todayPromotion != null)
                            {
                                commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                                commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                                commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                                commodity.PromotionType = todayPromotion.PromotionType;
                                if (todayPromotion.DiscountPrice > -1)
                                {
                                    commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                    commodity.Intensity = 10;
                                }
                                else
                                {
                                    commodity.DiscountPrice = -1;
                                    commodity.Intensity = todayPromotion.Intensity;
                                }
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = 10;
                                commodity.LimitBuyEach = -1;
                                commodity.LimitBuyTotal = -1;
                                commodity.SurplusLimitBuyTotal = -1;
                                commodity.PromotionType = 9999;
                            }
                            buildShowPrice(commodity, comStocks, todayPromotion);
                        }
                    }

                    if (commodityList != null && commodityList.Any())
                    {
                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                        var appInfoes = APPSV.Instance.GetAppListByIdsInfo(commodityList.Select(_ => _.AppId).ToList());
                        foreach (var com in commodityList)
                        {
                            var appInfo = appInfoes.FirstOrDefault(_ => _.AppId == com.AppId);
                            if (appInfo != null)
                            {
                                com.AppName = appInfo.AppName;
                                comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                                {
                                    appId = appInfo.AppId,
                                    appName = appInfo.AppName,
                                    icon = appInfo.AppIcon
                                });
                            }
                        }

                        if (comdtyListResultCDTO.appInfoList.Count > 0)
                        {
                            comdtyListResultCDTO.appInfoList[0].appName = "易捷优惠券凑单";
                        }
                    }
                    foreach (var item in commodityList)
                    {
                        item.Tags = GetCommodityTag(item);
                    }
                    comdtyListResultCDTO.comdtyList = commodityList.ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";

            return comdtyListResultCDTO;
        }

        /// <summary>
        /// xiexg优惠券CouponIdnew
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityByCouponId_New(CommodityListSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO =
                new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();

            try
            {
                DateTime now = DateTime.Now;
                IQueryable<Commodity> ocommodityList;
                //用于存储临加入真实价格后的Commodity信息
                IQueryable<TempCommodity> tempOcommodityList;

                tempOcommodityList = (from c in Commodity.ObjectSet()
                                      join pro in
                                          (
                                           from query in TodayPromotion.ObjectSet()
                                           where (query.PromotionType != 3 && (query.StartTime <= now || query.PresellStartTime <= now) && query.EndTime > now)
                                           select query
                                          ) on c.Id equals pro.CommodityId
                                        into todayPros
                                      from promotion in todayPros.DefaultIfEmpty()
                                      where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      select new TempCommodity
                                      {
                                          Com = c,
                                          newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                      });
                LogHelper.Debug(string.Format("优惠券商品进入商品查询1 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                Guid esAppId = YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                //易捷北京的自营或者门店自营
                if (search.MallAppType != null && search.MallAppType != 0)
                {
                    List<Guid> mallApply = new List<Guid>();
                    if (search.MallAppType == 2)//自营
                    {
                        mallApply = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && (t.Type == 0 || t.Type == 2 || t.Type == 3)).Select(o => o.AppId).Distinct().ToList();
                    }
                    if (search.MallAppType == 1)//第三方
                    {
                        mallApply = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && t.Type == 1).Select(o => o.AppId).Distinct().ToList();
                    }
                    if (mallApply != null && mallApply.Count > 0)
                    {
                        tempOcommodityList = tempOcommodityList.Where(_ => mallApply.Contains(_.Com.AppId));
                    }

                }

                #region//根据分类，品牌，商铺列ibao筛选
                if (search.CategoryIdList != null && search.CategoryIdList.Count > 0)//分类
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityCategory.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where (from f in search.CategoryIdList select f).Contains(ur.CategoryId)
                                         select data1;
                }
                if (search.BrandList != null && search.BrandList.Count > 0)//品牌
                {
                    tempOcommodityList = from data1 in tempOcommodityList
                                         join data in CommodityInnerBrand.ObjectSet() on data1.Com.Id equals data.CommodityId
                                             into data2
                                         from ur in data2.DefaultIfEmpty()
                                         where (from f in search.BrandList select f).Contains(ur.BrandId)
                                         select data1;
                }

                if (search.FieldWhere != null && search.FieldWhere != "")//商品名称的筛选
                {
                    tempOcommodityList = tempOcommodityList.Where(o => o.Com.Name.Contains(search.FieldWhere));
                }

                #endregion
                LogHelper.Debug(string.Format("优惠券商品进入分类筛选2 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                #region//价格筛选
                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }
                ocommodityList = tempOcommodityList.Select(c => c.Com);
                if (search.IsHasStock)
                {
                    ocommodityList = ocommodityList.Where(c => c.Stock > 0);
                }

                if (!ProvinceCityHelper.IsTheWholeCountry(search.areaCode))
                {
                    var province = ProvinceCityHelper.GetProvinceByAreaCode(search.areaCode);
                    var city = ProvinceCityHelper.GetCityByAreaCode(search.areaCode);
                    if (province != null && city != null)
                    {
                        if (province.AreaCode == city.AreaCode)
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode));
                        }
                        else
                        {
                            ocommodityList = ocommodityList.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                        }
                    }
                }
                #endregion
                var commoditiesQuery = from c in ocommodityList select c;
                List<Commodity> commodities = null;
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = new List<CommodityListCDTO>();

                #region //价格排序 需要按照 折扣价格排序

                switch (search.FieldSort)
                {
                    case 1:
                        var todaycom = (from c in TodayPromotion.ObjectSet()
                                        where c.EndTime > now && (c.StartTime < now || c.PresellStartTime < now) && c.PromotionType != 3
                                        select c);
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10)
                                                select c);
                        }
                        else
                        {
                            commoditiesQuery = (from c in commoditiesQuery
                                                join data in todaycom
                                                on c.Id equals data.CommodityId
                                                into tempT
                                                from tb3 in tempT.DefaultIfEmpty()
                                                where c.IsDel == false && c.State == 0
                                                orderby (tb3 != null ? (tb3.Intensity == 10 ? tb3.DiscountPrice * 10 : tb3.Intensity * c.Price) : c.Price * 10) descending
                                                select c);
                        }
                        break;
                    case 2:
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.Salesvolume);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.Salesvolume);
                        }
                        break;
                    case 3:

                        //if (search.OrderState == 1)
                        //{
                        //    commoditiesQuery = commoditiesQuery.OrderBy(n => n.SubTime);
                        //}
                        //else
                        //{
                        //    commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.SubTime);
                        //}
                        if (search.OrderState == 1)
                        {
                            commoditiesQuery = commoditiesQuery.OrderBy(n => n.YoukaPercent);
                        }
                        else
                        {
                            commoditiesQuery = commoditiesQuery.OrderByDescending(n => n.YoukaPercent);
                        }
                        break;
                    default:
                        commoditiesQuery = (from c in commoditiesQuery where c != null orderby c.State, c.SortValue, c.SubTime descending, c.Salesvolume descending select c);
                        break;
                }
                #endregion
                LogHelper.Debug(string.Format("优惠券商品进入筛选3 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                ListCouponNewRequestDTO lnr = new ListCouponNewRequestDTO();
                lnr.CouponIds = new List<Guid>() { search.CouponTemplateId };
                lnr.UserId = search.UserId;
                ReturnInfoDTO<IList<SpecifyStoreCoupon>> coupon = new ReturnInfoDTO<IList<SpecifyStoreCoupon>>();
                if (search.CouponTemplateId != Guid.Empty)
                {
                    //string ss = JsonHelper.JsSerializer<ListCouponNewRequestDTO>(lnr);
                    coupon = CouponSV.Instance.GetUserCouponsStoresByIds(lnr);
                }
                else
                {
                    coupon = CouponSV.Instance.GetUserCouponsStoresByType(lnr);
                }
                // var coupon = CouponSV.Instance.GetUserCouponsStoresByType(lnr);
                LogHelper.Debug(string.Format("优惠券商品调优惠券接口结束4 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));

                IQueryable<Commodity> comlist1 = commoditiesQuery;

                if (coupon != null && coupon.Data != null && coupon.Data.Count > 0)
                {

                    if (search.CouponTemplateId != Guid.Empty)
                    {
                        SpecifyStoreCoupon cnd = coupon.Data[0];

                        if (cnd.CouponTemplateType == Jinher.AMP.Coupon.Deploy.Enum.CouponTemplateType.Coupon)
                        {
                            LogHelper.Debug(string.Format("优惠券商品调优惠券进入普通券5 coupontype{1} ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now, cnd.CouponType));
                            switch (cnd.CouponType)
                            {
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.BeInCommon:   // 店铺通用                                    
                                    if (cnd.ShopId != null)
                                    {
                                        comlist1 = commoditiesQuery.Where(ss => cnd.ShopId == ss.AppId);//.ToList();
                                    }
                                    break;
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyGoods: // 指定商品
                                    if (cnd.GoodList != null || cnd.GoodList.Count > 0)
                                    {
                                        comlist1 = commoditiesQuery.Where(sg => cnd.GoodList.Contains(sg.Id));//.ToList();
                                    }
                                    break;
                                    //case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyStore: // 跨店满减
                                    //    var appIds = MallApply.GetTGQuery(cnd.EsAppId).Select(ma => ma.AppId).Distinct().ToList();
                                    //    if (appIds != null && appIds.Count > 0)
                                    //        comlist1 = commoditiesQuery.Where(bc => appIds.Contains(bc.AppId)).ToList();
                                    //    break;
                            }
                            LogHelper.Debug(string.Format("优惠券商品调优惠券进入普通券结束6，CommoditySV.GetCommodityListV2Ext。search：{0}", DateTime.Now));
                        }
                        else
                        {
                            LogHelper.Debug(string.Format("优惠券商品调优惠券进入跨店铺券开始7，CommoditySV.GetCommodityListV2Ext。search：{0}", DateTime.Now));
                            switch (cnd.CouponType)
                            {
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.BeInCommon:   // 商城通用
                                    LogHelper.Debug(string.Format("商品商城开始调用0000，CommoditySV.GetCommodityListV2Ext。search：{0}，商城id{1}", DateTime.Now, YJB.Deploy.CustomDTO.YJBConsts.YJAppId));
                                    //var appIds = MallApply.GetTGQuery(YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Where(_ => _.Type != 1).Select(ma => ma.AppId).Distinct().ToList();
                                    var appIds = MallApply.ObjectSet().Where(t => t.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && t.Type != 1).Select(ma => ma.AppId).Distinct().ToList();
                                    if (appIds != null && appIds.Count > 0)
                                    {
                                        //comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                                        ////commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                                        //if (search.PageIndex == 0) search.PageIndex = 1;
                                        //comlist1 = commoditiesQuery.Where(bc => appIds.Contains(bc.AppId)).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                                        comlist1 = commoditiesQuery.Where(bc => appIds.Contains(bc.AppId));
                                    }
                                    LogHelper.Debug(string.Format("商品商城开始调用结束-商城通用，CommoditySV.GetCommodityListV2Ext。search：{0}", DateTime.Now));
                                    break;
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyGoods: // 指定商品
                                    if (cnd.GoodList != null || cnd.GoodList.Count > 0)
                                    {
                                        comlist1 = commoditiesQuery.Where(sg => cnd.GoodList.Contains(sg.Id));//.ToList();
                                    }
                                    break;
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyStore: // 跨店满减
                                    if (cnd.ShopId != null)
                                    {
                                        // comlist1 = commoditiesQuery.Where(ss => cnd.ShopId == ss.AppId).ToList();
                                        comlist1 = commoditiesQuery.Where(ss => cnd.ShopIds.Contains(ss.AppId));//.ToList();
                                    }
                                    break;
                            }
                            LogHelper.Debug(string.Format("优惠券商品调优惠券进入跨店铺券结束8，CommoditySV.GetCommodityListV2Ext。search：{0}", DateTime.Now));

                        }

                    }
                    else
                    {
                        LogHelper.Debug(string.Format("优惠券商品调优惠券进入没有优惠券id 9，CommoditySV.GetCommodityListV2Ext。search：{0}", DateTime.Now));
                        foreach (SpecifyStoreCoupon cnd in coupon.Data)
                        {
                            switch (cnd.CouponType)
                            {
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.BeInCommon: // 指定店铺
                                    if (cnd.ShopIds != null && cnd.ShopIds.Count > 0)
                                    {
                                        //comlist1.AddRange(commoditiesQuery.Where(ss => cnd.ShopIds.Contains(ss.AppId)));
                                        comlist1 = commoditiesQuery.Where(ss => cnd.ShopIds.Contains(ss.AppId));
                                    }
                                    break;
                                case Jinher.AMP.Coupon.Deploy.Enum.CouponType.SpecifyGoods: // 指定商品
                                    if (cnd.GoodList != null || cnd.GoodList.Count > 0)
                                    {
                                        //comlist1.AddRange(commoditiesQuery.Where(sg => cnd.GoodList.Contains(sg.Id)));
                                        comlist1 = commoditiesQuery.Where(sg => cnd.GoodList.Contains(sg.Id));
                                    }
                                    break;

                            }
                        }
                    }

                    //List<Commodity> comlist2 = comlist1.Distinct().ToList();
                    comlist1 = comlist1.Distinct();

                    //comdtyListResultCDTO.realCount = comlist2.Count();
                    List<Guid> appList = comlist1.Select(o => o.AppId).Distinct().ToList();
                    var appInfoes = APPSV.Instance.GetAppListByIdsInfo(appList);
                    LogHelper.Debug(string.Format("优惠券商品进入品牌墙，分类，店铺筛选结束10 ，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));


                    if (search.PageIndex == 0)//xiexg2018712第一次访问
                    {
                        //LogHelper.Debug(string.Format("优惠券商品进入index为0   11，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                        //#region//品牌墙列表

                        //List<Guid> comlist = comlist1.Select(o => o.Id).ToList();

                        //LogHelper.Error(string.Format("商品列表品牌墙开始时间，{0}", DateTime.Now));
                        //var brandinner = CommodityInnerBrand.ObjectSet().Where(s => comlist.Contains(s.CommodityId)).Select(s => s.BrandId).AsParallel().ToList();
                        //LogHelper.Error(string.Format("商品列表品牌墙结束时间，{0}", DateTime.Now));

                        int index = 0;
                        //var branlist = Brandwall.ObjectSet().Where(o => brandinner.Contains(o.Id));
                        //branlist.OrderByDescending(o => o.SubTime).ToList();
                        //comdtyListResultCDTO.BrandWallList = new List<BrandwallDTO>();
                        //foreach (var obj in branlist.ToList())
                        //{
                        //    index++;
                        //    var branAppInfoCDTO = new Jinher.AMP.BTP.Deploy.BrandwallDTO()
                        //    {
                        //        Id = obj.Id,
                        //        Name = obj.Brandname,
                        //    };
                        //    comdtyListResultCDTO.BrandWallList.Add(branAppInfoCDTO);
                        //    if (index > 30)
                        //    {
                        //        break;
                        //    }
                        //}

                        comdtyListResultCDTO.BrandWallList = new List<BrandwallDTO>();

                        index = 0;
                        //#endregion
                        //#region//分类列表
                        //var Categoryinner = from s in CommodityCategory.ObjectSet()
                        //                    where comlist.Contains(s.CommodityId)
                        //                    select s.CategoryId;
                        //var Categorylist = Category.ObjectSet().Where(o => Categoryinner.Contains(o.Id));
                        //comdtyListResultCDTO.CategoryList = new List<CategoryDTO>();
                        //Categorylist.OrderByDescending(o => o.SubTime);
                        //foreach (var obj in Categorylist.AsParallel().ToList())
                        //{
                        //    index++;
                        //    var CategoryAppInfoCDTO = new Jinher.AMP.BTP.Deploy.CategoryDTO()
                        //    {
                        //        Id = obj.Id,
                        //        Name = obj.Name,
                        //    };
                        //    comdtyListResultCDTO.CategoryList.Add(CategoryAppInfoCDTO);
                        //    if (index > 30)
                        //    {
                        //        break;
                        //    }
                        //}
                        //#endregion

                        comdtyListResultCDTO.CategoryList = new List<CategoryDTO>();

                        #region//店铺类型
                        var gettype = MallApply.ObjectSet().Where(t => t.EsAppId == esAppId && appList.Contains(t.AppId)).Select(o => o.Type).AsParallel().Distinct().ToList();
                        comdtyListResultCDTO.MallAppList = new List<int?>();
                        foreach (var obj in gettype)
                        {
                            comdtyListResultCDTO.MallAppList.Add(obj);
                        }
                        LogHelper.Debug(string.Format("优惠券商品index 为0  结束  12，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                        #endregion
                    }
                    if (search.PageIndex == 0)
                    {
                        commodities = comlist1.OrderBy(_ => _.ModifiedOn).Skip((0) * search.PageSize).Take(search.PageSize).ToList();
                        //commodities = comlist2.ToList();
                    }
                    else
                    {
                        commodities = comlist1.OrderBy(_ => _.ModifiedOn).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                        //commodities = comlist2.ToList();
                    }

                    if (commodities.Any())
                    {
                        commodityList = commodities.Select(c => new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                        {
                            Id = c.Id,
                            Pic = c.PicturesPath,
                            Price = c.Price,
                            State = c.State,
                            Stock = c.Stock,
                            Name = c.Name,
                            MarketPrice = c.MarketPrice,
                            AppId = c.AppId,
                            IsEnableSelfTake = c.IsEnableSelfTake,
                            ComAttribute = c.ComAttribute,
                            ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                        }).ToList();

                        //  var mallAppInfo = MallApply.ObjectSet().FirstOrDefault(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId && _.AppId == search.AppId);
                        var mallAppInfolist = MallApply.ObjectSet().Where(_ => _.EsAppId == YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                        // var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = commodityList.Select(_ => _.Id).ToList() }).Data;
                        YJB.Deploy.CustomDTO.CommodityCashInput input = new YJB.Deploy.CustomDTO.CommodityCashInput();
                        input.CommodityIds = commodityList.Select(_ => _.Id).ToList();
                        var commodityCashes1 = YJBSV.GetCommodityCashPercent(input);
                        List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashOutput> commodityCashes = new List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityCashOutput>();
                        if (commodityCashes1 != null)
                        {
                            commodityCashes = YJBSV.GetCommodityCashPercent(input).Data;
                        }
                        LogHelper.Debug(string.Format("优惠券商品index 为0  结束  13，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                        #region 规格设置集合
                        commodityList.ForEach(s =>
                        {
                            var mallAppInfo = mallAppInfolist.FirstOrDefault(_ => _.AppId == s.AppId);
                            if (mallAppInfo != null)
                            {
                                s.MallType = mallAppInfo.Type;
                            }

                            var commodityCash = commodityCashes.Find(_ => _.CommodityId == s.Id);
                            if (commodityCash != null)
                            {
                                s.YJBAmount = commodityCash.YJBAmount;
                                s.YoukaAmount = commodityCash.YoukaAmount;
                            }
                            List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                            var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                            if (commoditySpecification.Count() > 0)
                            {
                                Guid commodityId = s.Id;
                                var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
                                if (commoditySpecificationlist.Count() > 0)
                                {
                                    commoditySpecificationlist.ForEach(p =>
                                    {

                                        Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                                        model.Id = p.Id;
                                        model.Name = "规格设置";
                                        model.Attribute = p.Attribute ?? 0;
                                        model.strAttribute = "1*" + p.Attribute + "";
                                        Specificationslist.Add(model);
                                    });
                                }
                                s.Specifications = Specificationslist;
                            }

                        });
                        LogHelper.Debug(string.Format("优惠券商品index 为0  结束  14，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                        #endregion

                        #region 众筹
                        if (CustomConfig.CrowdfundingFlag)
                        {
                            var crowdFundingApps = Crowdfunding.ObjectSet().Where(c => c.StartTime < now && c.State == 0 && appList.Contains(c.AppId)).Select(c => c.AppId).ToList();
                            if (crowdFundingApps.Any())
                            {
                                for (int i = 0; i < commodityList.Count; i++)
                                {
                                    if (crowdFundingApps.Any(c => c == commodityList[i].AppId))
                                        commodityList[i].IsActiveCrowdfunding = true;
                                }
                            }
                        }
                        #endregion

                        var commodityIds = commodityList.Select(c => c.Id).Distinct().ToList();
                        var comStockList = CommodityStock.ObjectSet()
                            .Where(c => commodityIds.Contains(c.CommodityId))
                            .Select(c =>
                                new Deploy.CommodityStockDTO
                                {
                                    Id = c.Id,
                                    CommodityId = c.CommodityId,
                                    Price = c.Price,
                                    MarketPrice = c.MarketPrice
                                }).ToList();

                        var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(commodityIds);
                        foreach (var commodity in commodityList)
                        {
                            commodity.IsMultAttribute = Commodity.CheckComMultAttribute(commodity.ComAttribute);
                            List<Deploy.CommodityStockDTO> comStocks = comStockList.Where(c => c.CommodityId == commodity.Id).ToList();

                            var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);

                            if (todayPromotion != null)
                            {
                                commodity.LimitBuyEach = todayPromotion.LimitBuyEach ?? -1;
                                commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal ?? -1;
                                commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? 0;
                                commodity.PromotionType = todayPromotion.PromotionType;
                                if (todayPromotion.DiscountPrice > -1)
                                {
                                    commodity.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                                    commodity.Intensity = 10;
                                }
                                else
                                {
                                    commodity.DiscountPrice = -1;
                                    commodity.Intensity = todayPromotion.Intensity;
                                }
                            }
                            else
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = 10;
                                commodity.LimitBuyEach = -1;
                                commodity.LimitBuyTotal = -1;
                                commodity.SurplusLimitBuyTotal = -1;
                                commodity.PromotionType = 9999;
                            }
                            buildShowPrice(commodity, comStocks, todayPromotion);
                        }
                    }
                    LogHelper.Debug(string.Format("优惠券商品index 为0  结束  15，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                    if (commodityList != null && commodityList.Any())
                    {
                        comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();

                        foreach (var com in commodityList)
                        {
                            var appInfo = appInfoes.FirstOrDefault(_ => _.AppId == com.AppId);
                            if (appInfo != null)
                            {
                                com.AppName = appInfo.AppName;
                            }
                        }
                        foreach (var appinfo in appInfoes)
                        {
                            comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO()
                            {
                                appId = appinfo.AppId,
                                appName = appinfo.AppName,
                                icon = appinfo.AppIcon
                            });
                        }
                    }
                    foreach (var item in commodityList)
                    {
                        item.Tags = GetTags(item, true);
                        item.TagsSimple = GetTags(item, false);
                    }





                    comdtyListResultCDTO.comdtyList = commodityList.ToList();
                    LogHelper.Debug(string.Format("优惠券商品index 为0  结束  16，CommoditySV.GetCommodityListV2_New。search：{0}", DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品列表查询错误，CommoditySV.GetCommodityListV2Ext。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = -1;
                comdtyListResultCDTO.Message = "Error";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";

            return comdtyListResultCDTO;
        }

        /// <summary>
        /// 商品改低价格时，Job发送消息(每1分钟处理一次)
        /// </summary>
        public void AutoPushCommodityModifyPriceExt()
        {
            DateTime now = DateTime.Now;
            DateTime outdate = DateTime.Now.AddMinutes(-1);

            var listNow = (from c in CommodityJournal.ObjectSet()
                           where c.SubTime <= now && c.SubTime >= outdate
                           select new { c.CommodityId, c.Price, c.Name, c.SubTime }).ToList().GroupBy(c => c.CommodityId).ToDictionary(x => x.Key, y => y.OrderByDescending(t => t.SubTime).FirstOrDefault());

            if (!listNow.Any())
            {
                return;
            }

            foreach (var commodityJournal in listNow)
            {
                var commodityId = commodityJournal.Key;
                var commodityPrice = commodityJournal.Value.Price;
                var commodityName = commodityJournal.Value.Name;

                var oldPrice =
                    CommodityJournal.ObjectSet()
                                    .Where(t => t.CommodityId == commodityId && t.SubTime < outdate)
                                    .OrderByDescending(t => t.SubTime).Select(t => t.Price)
                                    .FirstOrDefault();

                if (oldPrice != 0 && oldPrice > commodityPrice)
                {
                    Jinher.AMP.BTP.BE.BELogic.AddMessage addmassage = new Jinher.AMP.BTP.BE.BELogic.AddMessage();

                    var list = (from c in Collection.ObjectSet()
                                where c.CommodityId == commodityId
                                select new
                                {
                                    c.UserId,
                                    c.AppId
                                }).ToList();

                    //推送促销消息
                    list.ForEach(
                        b =>
                        {
                            addmassage.AddMessages(commodityId.ToString(),
                                                    b.UserId.ToString(), b.AppId,
                                                commodityName, null, null, "commodity");
                        });


                    var list2 = (from c in SetCollection.ObjectSet()
                                 where c.ColType == 1 && c.ColKey == commodityId
                                 select new
                                 {
                                     c.UserId,
                                     c.ChannelId
                                 }).ToList();

                    //推送促销消息
                    list2.ForEach(
                        b =>
                        {
                            addmassage.AddMessages(commodityId.ToString(),
                                                   b.UserId.ToString(), b.ChannelId,
                                                   commodityName, null, null, "commodity");
                        });
                }
            }
        }
        /// <summary>
        /// 获取商品列表关税合计
        /// </summary> 
        /// <param name="search">商品列表</param>
        /// <returns></returns>
        public ResultDTO<CreateOrderDutyResultDTO> GetComListDutyExt(List<ComScoreCheckDTO> search)
        {
            ResultDTO<CreateOrderDutyResultDTO> result = new ResultDTO<CreateOrderDutyResultDTO>();
            if (search == null || !search.Any())
            {
                result.ResultCode = 1;
                result.Message = "参数为空";
                return result;
            }
            var jcActivityIds = new List<Guid>();
            var comIds = search.Select(c => c.CommodityId).Distinct().ToList();
            var coms = Commodity.ObjectSet().Where(c => comIds.Contains(c.Id)).Select(m =>
                new CommodityDTO { Id = m.Id, AppId = m.AppId, Duty = m.Duty, Type = m.Type, Isnsupport = m.Isnsupport, IsAssurance = m.IsAssurance }).ToList();
            var comStocks = Commodity.GetComStocks(search);
            Dictionary<Guid, List<ComDutyReDTO>> dict = new Dictionary<Guid, List<ComDutyReDTO>>();
            var now = DateTime.Now;

            foreach (var requestCom in search)
            {
                var com = coms.First(c => c.Id == requestCom.CommodityId);
                decimal duty = 0.0m;
                if (requestCom.CommodityStockId != null && requestCom.CommodityStockId != Guid.Empty)
                {
                    var comStock = comStocks.FirstOrDefault(c => c.Id == requestCom.CommodityStockId);
                    if (comStock != null) duty = comStock.Duty ?? 0;
                }
                else
                {
                    duty = com.Duty ?? 0;
                }
                if (!dict.ContainsKey(com.AppId))
                {
                    dict.Add(com.AppId, new List<ComDutyReDTO>());
                }
                ComDutyReDTO item = ComDutyReDTO.FromRequest(requestCom, com.AppId);
                item.Duty = duty;

                // 赠品信息
                #region 赠品信息
                var commodityStockId = item.CommodityStockId ?? Guid.Empty;
                var present = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == item.CommodityId && (_.CommoditySKUId == commodityStockId || _.CommoditySKUId == item.CommodityId))
                   .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                       pp => pp.PresentPromotionId,
                       ppc => ppc.Id, (c, p) => new { Commodity = c, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime }).FirstOrDefault();
                if (present != null)
                {
                    item.Present = new CommodiyPresentDTO();
                    item.Present.Limit = present.Limit ?? 1;
                    if (item.Present.Limit == 0)
                    {
                        item.Present.Limit = 1;
                    }
                    item.Present.BeginTime = present.BeginTime;
                    item.Present.EndTime = present.EndTime;
                    var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == present.PromotionId).ToList();
                    var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                    var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                    item.Present.Items = new List<CommodiyPresentItem>();
                    foreach (var g in gifts)
                    {
                        var tempCom = GetCommodity(item.AppId, g.CommodityId);
                        var commodiyPresentItem = new CommodiyPresentItem();
                        commodiyPresentItem.Id = g.CommodityId;
                        commodiyPresentItem.StockId = g.CommoditySKUId;
                        commodiyPresentItem.Name = g.CommodityName;
                        commodiyPresentItem.Pic = tempCom.PicturesPath;
                        commodiyPresentItem.Number = g.Number;
                        if (g.CommoditySKUId != Guid.Empty)
                        {
                            var giftCommodityStock = giftCommodityStocks.Find(_ => _.Id == g.CommoditySKUId);
                            if (giftCommodityStock != null && giftCommodityStock.Stock > 0)
                            {
                                commodiyPresentItem.SKU = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(giftCommodityStock.ComAttribute);
                                commodiyPresentItem.Stock = giftCommodityStock.Stock;
                                item.Present.Items.Add(commodiyPresentItem);
                            }
                        }
                        else
                        {
                            if (tempCom.Stock > 0)
                            {
                                commodiyPresentItem.Stock = tempCom.Stock;
                                item.Present.Items.Add(commodiyPresentItem);
                            }
                        }
                    }
                }
                #endregion

                item.IsAssurance = com.IsAssurance ?? false;

                if (requestCom.OutPromotionId.HasValue && requestCom.OutPromotionId != Guid.Empty)
                {
                    var presell = ZPHSV.Instance.GetAndCheckPresellInfoById(new Jinher.AMP.ZPH.Deploy.CustomDTO.CheckPresellInfoCDTO()
                    {
                        comdtyId = requestCom.CommodityId,
                        id = requestCom.OutPromotionId.Value
                    });
                    // LogHelper.Info("CommodiySVExt.GetComListDutyExt.GetAndCheckPresellInfoById, Input: ComdityId = " + requestCom.CommodityId + " Id = " + requestCom.OutPromotionId.Value + " Output: " + JsonHelper.JsonSerializer(presell)); 
                    if (presell != null && presell.PresellType == 3)
                    {
                        if (presell.DeliveryDays.HasValue)
                        {
                            item.DeliveryTime = "支付后" + presell.DeliveryDays.Value + "天内";
                        }
                        else
                        {
                            item.DeliveryTime = presell.DeliveryTime.Value.ToString("yyyy-MM-dd");
                        }
                    }
                }
                item.Type = com.Type ?? 0;
                dict[com.AppId].Add(item);
            }
            result.ResultCode = 0;
            result.isSuccess = true;
            result.Message = "Success";
            result.Data = new CreateOrderDutyResultDTO(dict);
            result.Data.Isnsupport = coms.Any(_ => _.Isnsupport.HasValue && _.Isnsupport.Value);
            return result;
        }

        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public ResultDTO<CommodityDividendListDTO> GetCommodityByNameExt(GetCommodityByNameParam pdto)
        {
            ResultDTO<CommodityDividendListDTO> result = new ResultDTO<CommodityDividendListDTO>();

            if (pdto == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空！";
                return result;
            }
            if (pdto.AppId == Guid.Empty)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，appId不能为空！";
                return result;
            }

            try
            {
                var query = from c in Commodity.ObjectSet()
                            where c.AppId == pdto.AppId && c.IsDel == false && c.State == 0
                            select c;
                if (!string.IsNullOrWhiteSpace(pdto.CommodityName))
                {
                    query = query.Where(c => c.Name.Contains(pdto.CommodityName));
                }
                if (pdto.OnlyShareMoney)
                {
                    query = query.Where(c => c.SharePercent > 0);
                }
                if (pdto.OnlyScoreMoney)
                {
                    query = query.Where(c => c.ScorePercent > 0);
                }
                if (pdto.OnlySpreadMoney)
                {
                    query = query.Where(c => c.SpreadPercent > 0);
                }
                if (!string.IsNullOrWhiteSpace(pdto.CommodityCategory))
                {
                    string[] commodityCategoryID = pdto.CommodityCategory.Split(',');
                    List<Guid> idlist = new List<Guid>();
                    foreach (string commodityCategoryid in commodityCategoryID)
                    {
                        if (!string.IsNullOrEmpty(commodityCategoryid))
                        {
                            idlist.Add(new Guid(commodityCategoryid));
                        }
                    }
                    query = from n in query
                            join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                            where idlist.Contains(m.CategoryId)
                            select n;
                }
                int count = query.Count();
                if (count == 0)
                {
                    result.Data = new CommodityDividendListDTO();
                    result.Data.CommodityList = new List<CommodityListCDTO>();
                    result.Data.Count = 0;
                    return result;
                }
                query = query.OrderBy(n => n.SortValue).ThenByDescending(n => n.SubTime)
                             .Skip((pdto.PageIndex - 1) * pdto.PageSize).Take(pdto.PageSize);
                var commodityList = query.ToList();
                if (!commodityList.Any())
                {
                    result.Data = new CommodityDividendListDTO();
                    result.Data.CommodityList = new List<CommodityListCDTO>();
                    result.Data.Count = 0;
                    return result;
                }
                List<CommodityListCDTO> crList = commodityList.ConvertAll(ConvertCommodity2DTO);
                var resultTuple = new CommodityDividendListDTO();
                resultTuple.CommodityList = crList;
                resultTuple.Count = count;
                result.Data = resultTuple;
                var AppName = APPSV.GetAppNameIcon(pdto.AppId).AppName;
                foreach (var item in result.Data.CommodityList)
                {
                    item.AppName = AppName;
                }

                if (!string.IsNullOrEmpty(pdto.AppName))
                {
                    result.Data.CommodityList = result.Data.CommodityList.Where(p => p.AppName.Contains(pdto.AppName)).ToList();
                }
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("根据商品名称获取商品列表异常", ex);
                result.ResultCode = -1;
                result.Message = "获取商品列表异常,请稍后重试！";
            }

            return result;
        }


        private CommodityListCDTO ConvertCommodity2DTO(Commodity commodity)
        {
            CommodityListCDTO ccDto = new CommodityListCDTO();
            ccDto.FillWith(commodity);
            ccDto.Pic = commodity.PicturesPath;
            return ccDto;
        }

        /// <summary>
        /// 进销存同步京东库存
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDStockExt(Jinher.AMP.BTP.Deploy.CommodityDTO arg)
        {
            if (string.IsNullOrEmpty(arg.Code) || string.IsNullOrEmpty(arg.ErQiCode)) return new ResultDTO { Message = "参数有误" };
            var result = new ResultDTO();
            var param = string.Format("操作唯一编码：{0},商品编码：{1},库存：{2}", arg.Code, arg.ErQiCode, arg.Stock);
            LogHelper.Debug("进销存同步京东库存开始,入参" + param);
            var newStock = arg.Stock >= 5 ? arg.Stock - 5 : 0;
            newStock = newStock < 0 ? 0 : newStock;
            try
            {
                if (JDStockJournal.ObjectSet().Any(p => p.Code == arg.Code)) return new ResultDTO { isSuccess = true, Message = "已同步过" };
                var commodity = Commodity.ObjectSet().FirstOrDefault(p => p.ErQiCode == arg.ErQiCode);
                var commodityStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.ErQiCode == arg.ErQiCode);
                if (commodityStock != null)
                {
                    if (commodity == null) commodity = Commodity.ObjectSet().FirstOrDefault(p => p.Id == commodityStock.CommodityId) ?? new Commodity();
                    var journal = new JDStockJournal
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        AppId = commodity.AppId,
                        CommodityId = commodity.Id,
                        CommodityErQiCode = commodity.ErQiCode,
                        CommodityOldStock = commodity.Stock,
                        CommodityNewStock = newStock,
                        CommodityStockId = commodityStock.Id,
                        CommodityStockErQiCode = commodityStock.ErQiCode,
                        CommodityStockOldStock = commodityStock.Stock,
                        CommodityStockNewStock = newStock,
                        Code = arg.Code,
                        Json = arg.Description,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(journal);
                    commodityStock.Stock = newStock;
                    commodityStock.ModifiedOn = DateTime.Now;
                }
                if (commodity != null)
                {
                    var journal = new JDStockJournal
                    {
                        Id = Guid.NewGuid(),
                        SubTime = arg.SubTime,
                        AppId = commodity.AppId,
                        CommodityId = commodity.Id,
                        CommodityErQiCode = commodity.ErQiCode,
                        CommodityOldStock = commodity.Stock,
                        CommodityNewStock = newStock,
                        Code = arg.Code,
                        Json = arg.Description,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(journal);
                    commodity.Stock = newStock;
                    commodity.ModifiedOn = DateTime.Now;
                }
                if (commodity == null && commodityStock == null)
                {
                    result.Message = "商品不存在";
                }
                else
                {
                    if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                    {
                        result.isSuccess = true;
                        result.Message = "同步成功";
                    }
                    else
                    {
                        LogHelper.Error("进销存同步京东库存失败,入参" + param);
                        result.Message = "数据保存失败";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("进销存同步京东库存异常,入参" + param, ex);
                result.Message = "同步异常";
            }
            return result;
        }

        /// <summary>
        /// 运费计算
        /// </summary>
        /// <param name="FreightTo">目的地</param>
        /// <param name="TemplateCounts">计算模板</param>
        /// <returns>计算结果</returns>
        private decimal CalRefundFreight(string FreightTo, List<TemplateCountDTO> TemplateCounts)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.FreightResultDTO fResult = new Deploy.CustomDTO.FreightResultDTO();
            if (TemplateCounts == null || TemplateCounts.Count < 1 || string.IsNullOrWhiteSpace(FreightTo))
            {
                return 0;
            }
            try
            {
                FreightTo = FreightTo.Trim();

                //总运费
                decimal fDecimal = 0;

                //订单总价
                decimal orderprice = 0;
                //订单件数
                decimal ordercount = 0;
                //订单总重量
                decimal orderweight = 0;


                //计算订单总价
                orderprice = TemplateCounts.Sum(t => t.Count * t.Price);


                //由于颜色尺寸问题，对相同商品合并分组
                var query = from templateCount in TemplateCounts
                            group templateCount by templateCount.CommodityId into g
                            select new
                            {
                                g.Key,
                                Num = g.Sum(templateCount => templateCount.Count)
                            };

                //取出商品ID列表
                List<Guid> commodityIds = query.Select(t => t.Key).ToList();
                //取出商品数量列表
                var tmpquery = query.ToDictionary(t => t.Key, t => t.Num);


                //取出商品的计价方式与重量
                var ordersearch = (from c in Commodity.ObjectSet()
                                   where commodityIds.Contains(c.Id) && c.CommodityType == 0
                                   select new
                                   {
                                       CommodityId = c.Id,
                                       CalcType = c.PricingMethod,
                                       Weight = c.Weight
                                   }).ToList();

                //计算订单件数
                ordercount = (from q in query
                              join c in ordersearch on q.Key equals c.CommodityId
                              where c.CalcType == 0
                              select new
                              {
                                  Num = q.Num
                              }).Sum(t => t.Num);
                //计算订单总重量
                orderweight = (from q in query
                               join c in ordersearch on q.Key equals c.CommodityId
                               where c.CalcType == 1
                               select new
                               {
                                   Weight = c.Weight == null ? 0 : q.Num * c.Weight.Value
                               }).Sum(t => t.Weight);

                //包邮的商品
                var tmpSearch = (from c in Commodity.ObjectSet()
                                 join f in FreightTemplate.ObjectSet() on c.RefundFreightTemplateId equals f.Id
                                 where commodityIds.Contains(c.Id) && f.ExpressType == 1 && c.CommodityType == 0
                                 select c.Id).ToList();

                //部分包邮的商品
                var tmpPartailSearch = (from c in Commodity.ObjectSet()
                                        join f in FreightTemplate.ObjectSet() on c.RefundFreightTemplateId equals f.Id
                                        join fp in FreightPartialFree.ObjectSet() on c.RefundFreightTemplateId equals fp.FreightTemplateId
                                        where commodityIds.Contains(c.Id) && f.ExpressType == 2 && fp.DestinationCodes.Contains(FreightTo) &&
                                            (
                                             (fp.FreeType == 1 && fp.FreePrice <= orderprice) ||
                                             (f.PricingMethod == 0 && fp.FreeType == 0 && fp.FreeCount <= ordercount) ||
                                             (f.PricingMethod == 1 && fp.FreeType == 0 && fp.FreeCount >= orderweight)
                                             ) && c.CommodityType == 0
                                        select c.Id).ToList();

                //不包邮的商品，按公式计算运费
                List<Guid> calIds = commodityIds.Except(tmpSearch).Except(tmpPartailSearch).ToList();

                LogHelper.Debug(string.Format("计算运费: 包邮商品数量：{0},部分包邮商品数量:{1},不包邮商品数量:{2},运送至:{3}", tmpSearch.Count, tmpPartailSearch.Count, calIds.Count, FreightTo));

                //取计费模板
                var tmpFeightSearch = (from c in Commodity.ObjectSet()
                                       join f in FreightTemplate.ObjectSet() on c.RefundFreightTemplateId equals f.Id
                                       join fd in FreightTemplateDetail.ObjectSet() on c.RefundFreightTemplateId equals fd.FreightTemplateId
                                       into g
                                       from tmp in g.DefaultIfEmpty()
                                       where calIds.Contains(c.Id) && tmp.DestinationCodes.Contains(FreightTo) && c.CommodityType == 0
                                       select new CalFreightTemplate
                                       {
                                           FreightTemplateId = f.Id,
                                           CommodityId = c.Id,
                                           CalcType = c.PricingMethod,
                                           FirstCount = tmp.FirstCount == null ? 0 : tmp.FirstCount,
                                           FirstCountPrice = tmp.FirstCountPrice == null ? 0 : tmp.FirstCountPrice,
                                           NextCount = tmp.NextCount == null ? 0 : tmp.NextCount,
                                           NextCountPrice = tmp.NextCountPrice == null ? 0 : tmp.NextCountPrice,
                                           Count = 0,
                                           Weight = c.Weight == null ? 0 : c.Weight.Value
                                       }).ToList();

                //取默认计费模板
                var defaultIds = calIds.Except(tmpFeightSearch.Select(t => t.CommodityId)).ToList();

                var tmpDefaultFeightSearch = (from c in Commodity.ObjectSet()
                                              join f in FreightTemplate.ObjectSet() on c.RefundFreightTemplateId equals f.Id
                                              where c.CommodityType == 0 && defaultIds.Contains(c.Id)
                                              select new CalFreightTemplate
                                              {
                                                  FreightTemplateId = f.Id,
                                                  CommodityId = c.Id,
                                                  CalcType = c.PricingMethod,
                                                  FirstCount = f.FirstCount,
                                                  FirstCountPrice = f.FirstCountPrice,
                                                  NextCount = f.NextCount,
                                                  NextCountPrice = f.NextCountPrice,
                                                  Count = 0,
                                                  Weight = c.Weight == null ? 0 : c.Weight.Value
                                              }).ToList();


                //合并运费模板
                if (tmpDefaultFeightSearch.Count > 0)
                {
                    foreach (var calFreightTemplate in tmpDefaultFeightSearch)
                    {
                        var count = tmpFeightSearch.Count(t => t.FreightTemplateId == calFreightTemplate.FreightTemplateId);
                        if (count == 0)
                        {
                            tmpFeightSearch.Add(calFreightTemplate);
                        }
                    }
                }
                //给count附值 
                foreach (CalFreightTemplate item in tmpFeightSearch)
                {
                    item.Count = tmpquery[item.CommodityId];
                }

                LogHelper.Debug(string.Format("计算运费: 独立计费数量：{0},默认模版数量:{1}，商品数量{2}", tmpFeightSearch.Count, tmpDefaultFeightSearch.Count, commodityIds.Count));
                //单种商品的运费计算方式
                if (commodityIds.Count == 1)
                {
                    //不包邮，且取到运费模板的
                    if (tmpFeightSearch.Count == 1)
                    {
                        var comFeight = tmpFeightSearch[0];
                        //按件数
                        if (comFeight.CalcType == 0)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                        //按重量
                        else if (comFeight.CalcType == 1)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count * comFeight.Weight)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count * comFeight.Weight - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                    }
                }
                //多种商品的计算
                else if (commodityIds.Count > 1)
                {
                    //计算运费
                    //只有一种商品需要计算运费，与单种商品的运费计算方式模式相同
                    if (tmpFeightSearch.Count == 1)
                    {
                        var comFeight = tmpFeightSearch[0];
                        //按件数
                        if (comFeight.CalcType == 0)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {

                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                        //按重量
                        else if (comFeight.CalcType == 1)
                        {
                            //只有首费
                            if (comFeight.FirstCount >= comFeight.Count * comFeight.Weight)
                            {
                                fDecimal = comFeight.FirstCountPrice;
                            }
                            else
                            {
                                //增费标准不存在
                                if (comFeight.NextCount == 0)
                                {
                                    fDecimal = comFeight.FirstCountPrice + comFeight.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal = comFeight.FirstCountPrice + Math.Ceiling((comFeight.Count * comFeight.Weight - comFeight.FirstCount) / comFeight.NextCount) * comFeight.NextCountPrice;
                                }
                            }
                        }
                    }
                    //2种及以上的商品需要计算运费
                    else if (tmpFeightSearch.Count > 1)
                    {
                        //取最大首费，最小增费的商品（首费直接取首费，不用除首费标准；增费时，需要用增费金额除以增费标准）
                        CalFreightTemplate cft = tmpFeightSearch[0];

                        for (int i = 1; i < tmpFeightSearch.Count; i++)
                        {
                            var first1 = cft.FirstCountPrice;
                            var first2 = tmpFeightSearch[i].FirstCountPrice;
                            if (first1 < first2)
                            {
                                cft = tmpFeightSearch[i];
                            }
                            else if (first1 == first2)
                            {
                                //增费标准是空，只有增费金额的，就按其是1个单位的增费金额
                                decimal next1 = 0;
                                decimal next2 = 0;

                                //增费标准与增费金额，可能有一个或两个都是0
                                if (cft.NextCountPrice == 0)
                                {
                                    next1 = 0;
                                }
                                else
                                {
                                    next1 = cft.NextCount > 0 ? cft.NextCountPrice / cft.NextCount : cft.NextCountPrice;
                                }
                                //增费标准与增费金额，可能有一个或两个都是0
                                if (tmpFeightSearch[i].NextCountPrice == 0)
                                {
                                    next2 = 0;
                                }
                                else
                                {
                                    next2 = tmpFeightSearch[i].NextCount > 0 ? tmpFeightSearch[i].NextCountPrice / tmpFeightSearch[i].NextCount : tmpFeightSearch[i].NextCountPrice;
                                }

                                if (next1 > next2)
                                {
                                    cft = tmpFeightSearch[i];
                                }
                            }
                        }

                        //对于只有首费没有增费的，若不算入首费中，则不计算在运费中了。取出没算入到首费的商品    
                        var otherFeightSearch = tmpFeightSearch.Where(t => !commodityIds.Contains(t.CommodityId)).ToList();
                        //计算运费
                        //算入首费
                        fDecimal = cft.FirstCountPrice;

                        LogHelper.Debug(string.Format("计算运费: 1当前运费价格为{0},订单总件数{1}", fDecimal, ordercount));

                        //计算首费商品剩余增费
                        //按件数
                        if (cft.CalcType == 0)
                        {
                            //增费标准不存在
                            if (cft.NextCount == 0)
                            {
                                fDecimal += cft.NextCountPrice;
                            }
                            else
                            {
                                //fDecimal += Math.Ceiling((ordercount - cft.FirstCount) / cft.NextCount) * cft.NextCountPrice;
                                var comIds = TemplateCounts.Select(t => t.CommodityId);
                                foreach (var comId in comIds)
                                {
                                    var coms = TemplateCounts.Where(t => t.CommodityId == comId);
                                    var com = tmpFeightSearch.Where(t => t.CommodityId == comId);
                                    fDecimal += com.FirstOrDefault().FirstCountPrice;
                                    fDecimal += Math.Ceiling((coms.FirstOrDefault().Count - com.FirstOrDefault().FirstCount) / com.FirstOrDefault().NextCount) * com.FirstOrDefault().NextCountPrice;
                                }
                                fDecimal = fDecimal - cft.FirstCountPrice;
                            }
                        }
                        //按重量
                        else if (cft.CalcType == 1)
                        {
                            //只有首费
                            if (cft.FirstCount < cft.Count * cft.Weight)
                                //增费标准不存在
                                if (cft.NextCount == 0)
                                {
                                    fDecimal += cft.NextCountPrice;
                                }
                                else
                                {
                                    //fDecimal += Math.Ceiling((cft.Count * cft.Weight - cft.FirstCount) / cft.NextCount) * cft.NextCountPrice;
                                    var comIds = TemplateCounts.Select(t => t.CommodityId);
                                    foreach (var comId in comIds)
                                    {
                                        var coms = TemplateCounts.Where(t => t.CommodityId == comId);
                                        var com = tmpFeightSearch.Where(t => t.CommodityId == comId);
                                        fDecimal += com.FirstOrDefault().FirstCountPrice;
                                        fDecimal += Math.Ceiling((coms.FirstOrDefault().Count * com.FirstOrDefault().Weight - com.FirstOrDefault().FirstCount) / com.FirstOrDefault().NextCount) * com.FirstOrDefault().NextCountPrice;
                                    }
                                    fDecimal = fDecimal - cft.FirstCountPrice;
                                }
                        }

                        LogHelper.Debug(string.Format("计算运费: 2当前运费价格为{0}", fDecimal));

                        //算入其它商品的费用
                        foreach (CalFreightTemplate item in otherFeightSearch)
                        {
                            //按件数
                            if (item.CalcType == 0)
                            {
                                //增费标准不存在
                                if (item.NextCount == 0)
                                {

                                    fDecimal += item.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal += Math.Ceiling(item.Count / item.NextCount) * item.NextCountPrice;
                                }
                            }
                            //按重量
                            else if (item.CalcType == 1)
                            {
                                //增费标准不存在
                                if (item.NextCount == 0)
                                {
                                    fDecimal += item.NextCountPrice;
                                }
                                else
                                {
                                    fDecimal += Math.Ceiling(item.Count * item.Weight / item.NextCount) * item.NextCountPrice;
                                }
                            }

                            LogHelper.Debug(string.Format("计算运费: 3当前运费价格为{0}", fDecimal));

                        }
                    }
                }
                return fDecimal;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("运费计算错误。FreightTo：{0}。TemplateCounts：{1}", FreightTo, TemplateCounts), ex);
                return 0;
            }
        }

        /// <summary>
        /// 订单或订单项拒收或取件运费计算
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        public FreighMultiAppResultDTO CalRefundFreightExt(Guid orderId, Guid orderItemId)
        {
            string param = string.Format(",orderId={0},orderItemId={1}", orderId, orderItemId);
            if (orderId == Guid.Empty)
            {
                LogHelper.Error("CalRefundFreightExt订单或订单项拒收或取件运费计算:" + param);
                return new FreighMultiAppResultDTO { Message = "orderId有误", AppFreight = new List<AppFreight>() };
            }
            try
            {
                var order = CommodityOrder.ObjectSet().Where(p => p.Id == orderId).Select(p => new { p.Province, p.AppId }).FirstOrDefault();
                if (order == null)
                {
                    LogHelper.Error("CalRefundFreightExt订单或订单项拒收或取件运费计算:订单不存在" + param);
                    return new FreighMultiAppResultDTO { Message = "订单不存在", AppFreight = new List<AppFreight>() };
                }
                var freightTo = order.Province;
                if (string.IsNullOrEmpty(freightTo))
                {
                    LogHelper.Error("CalRefundFreightExt订单或订单项拒收或取件运费计算:未找到收货地址" + param);
                    return new FreighMultiAppResultDTO { Message = "未找到收货地址", AppFreight = new List<AppFreight>() };
                }
                freightTo = freightTo.Trim();
                //将目的地由汉字转成编码
                freightTo = ProvinceCityHelper.GetProvinceCodeByName(freightTo);
                var listCommodity = new List<TemplateCountDTO>();
                if (orderItemId != Guid.Empty)
                {
                    listCommodity = OrderItem.ObjectSet().Where(p => p.Id == orderItemId).Select(p => new TemplateCountDTO
                    {
                        CommodityId = p.CommodityId,
                        Count = p.Number,
                        Price = p.RealPrice ?? 0
                    }).ToList();
                }
                else
                {
                    listCommodity = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == orderId).Select(p => new TemplateCountDTO
                    {
                        CommodityId = p.CommodityId,
                        Count = p.Number,
                        Price = p.RealPrice ?? 0
                    }).ToList();
                }
                if (listCommodity.Count == 0)
                {
                    LogHelper.Error("CalRefundFreightExt订单或订单项拒收或取件运费计算:未找到订单商品" + param);
                    return new FreighMultiAppResultDTO { Message = "未找到订单商品", AppFreight = new List<AppFreight>() };
                }
                var money = CalRefundFreight(freightTo, listCommodity);
                return new FreighMultiAppResultDTO
                {
                    isSuccess = true,
                    Freight = money,
                    AppFreight = new List<AppFreight> { new AppFreight { AppId = order.AppId, Freight = money } }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("CalRefundFreightExt订单或订单项拒收或取件运费计算异常" + param, ex);
                return new FreighMultiAppResultDTO { Message = ex.Message, AppFreight = new List<AppFreight>() };
            }
        }

        /// <summary>
        /// 查询某个APP下的商品 按照销量进行相关排序
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityWithSalesExt(CommoditySearchDTO commoditySearch)
        {
            if (commoditySearch == null) return null;
            if (commoditySearch.appId == Guid.Empty) return null;
            if (commoditySearch.pageIndex < 1) return null;
            if (commoditySearch.pageSize < 1) return null;

            var appInfos = TPS.ZPHSV.Instance.GetAppIdlist(new List<Guid> { commoditySearch.appId });
            var appIds = appInfos.Select(t => t.AppId).ToList();
            appIds.Add(commoditySearch.appId);

            var commodityList =
                from scc in CommodityCategory.ObjectSet()
                join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                where c.IsDel == false && c.State == 0 && c.CommodityType == 0 && appIds.Contains(c.AppId)
                && scc.IsDel == false && scc.CategoryId != Guid.Empty
                select c;

            commodityList = commodityList.Distinct().OrderByDescending(t => t.Salesvolume).Skip((commoditySearch.pageIndex - 1) * commoditySearch.pageSize)
            .Take(commoditySearch.pageSize);

            var result = (from c in commodityList
                          select new CommodityDTO
                          {
                              AppId = c.AppId,
                              Code = c.Code,
                              CategoryName = c.CategoryName,
                              ComAttribute = c.ComAttribute,
                              Description = c.Description,
                              GroundTime = c.GroundTime,
                              Id = c.Id,
                              IsDel = c.IsDel,
                              Name = c.Name,
                              No_Code = c.No_Code,
                              No_Number = c.No_Number,
                              PicturesPath = c.PicturesPath,
                              Price = c.Price,
                              State = c.State,
                              MarketPrice = c.MarketPrice,
                              Stock = c.Stock,
                              TotalReview = c.TotalReview,
                              TotalCollection = c.TotalCollection,
                              Salesvolume = c.Salesvolume,
                              IsEnableSelfTake = c.IsEnableSelfTake,
                              CostPrice = c.CostPrice
                          }).ToList();

            var comIds = result.Select(t => t.Id).ToList();
            var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
            foreach (var commodity in result)
            {
                var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.Id && c.PromotionType != 3);
                if (todayPromotion != null)
                {
                    if (todayPromotion.DiscountPrice > -1)
                    {
                        commodity.Price = Convert.ToDecimal(todayPromotion.DiscountPrice);
                    }
                }
            }
            return result.ToList();
        }


        #region   增加商品查询条件   获取类目集合



        /// <summary>
        /// 增加商品查询条件---分类、毛利率区间，价格区间
        /// </summary>
        /// <param name="input">输入查询实体</param>
        /// <param name="commodityQuery">查询对象</param>
        /// <returns></returns>
        private static IQueryable<Commodity> AddCommoditySelectWhere(CommodityListInputDTO input, IQueryable<Commodity> commodityQuery)
        {
            try
            {
                //根据分类查询
                if (!string.IsNullOrWhiteSpace(input.Categorys))
                {



                    List<Guid> listId = GetRecursiveCategoryId(input);


                    commodityQuery = (from scc in CommodityCategory.ObjectSet()
                                      join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                                      where c.AppId == input.AppId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      && listId.Contains(scc.CategoryId)
                                      orderby scc.MaxSort
                                      select c).Distinct();
                }
                //毛利率区间
                if (!string.IsNullOrWhiteSpace(input.MinInterestRate) && !string.IsNullOrWhiteSpace(input.MaxInterestRate))
                {
                    //4、商品毛利率＝（销售价－进货价）/销售价x100%
                    //获取商品信息，循环计算毛利率
                    decimal minInterestRate, maxInterestRate = 0;
                    decimal.TryParse(input.MinInterestRate, out minInterestRate);
                    decimal.TryParse(input.MaxInterestRate, out maxInterestRate);
                    commodityQuery = commodityQuery.Where(p => (((p.Price - p.CostPrice) / p.Price) * 100) >= minInterestRate && (((p.Price - p.CostPrice) / p.Price) * 100) <= maxInterestRate);

                }
                //价格区间
                if (!string.IsNullOrWhiteSpace(input.MinPrice) && !string.IsNullOrWhiteSpace(input.MaxPrice))
                {
                    decimal minPrice, maxPrice = 0;
                    decimal.TryParse(input.MinPrice, out minPrice);
                    decimal.TryParse(input.MaxPrice, out maxPrice);
                    commodityQuery = commodityQuery.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("CommoditySVExt.AddCommoditySelectWhere：{0}", ex.ToString()));
            }
            return commodityQuery;
        }



        /// <summary>
        /// 获取三层数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static List<Guid> GetRecursiveCategoryId(CommodityListInputDTO input)
        {
            List<Guid> list = new List<Guid>();
            List<Guid> listId2 = new List<Guid>();

            input.Categorys.Split(',').ToList().ForEach(p =>
            {
                if (!string.IsNullOrWhiteSpace(p))
                {
                    Guid id = Guid.Parse(p);

                    list.Add(id);

                    var category = Category.ObjectSet().Where(n => n.AppId == input.AppId && n.IsDel == false && n.ParentId == id).FirstOrDefault();
                    if (category != null)
                    {
                        listId2.Add(category.Id);
                    }
                }

            });


            input.Categorys.Split(',').ToList().ForEach(p =>
            {
                if (!string.IsNullOrWhiteSpace(p))
                {
                    Guid id = Guid.Parse(p);
                    list.Add(id);

                    var category = Category.ObjectSet().Where(n => n.AppId == input.AppId && n.IsDel == false && n.ParentId == id).FirstOrDefault();
                    if (category != null)
                    {
                        list.Add(category.Id);
                    }
                }

            });
            return list;
        }
        #endregion


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityYoukaExt(Guid commodityId, decimal youka)
        {
            var resutlt = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO()
            {
                isSuccess = true,
                Message = "success",
                ResultCode = 1
            };
            var contextSession = ContextFactory.CurrentThreadContext;
            var entity = Commodity.ObjectSet().FirstOrDefault(o => o.Id == commodityId);
            if (entity != null)
            {
                entity.YoukaPercent = youka;
                entity.EntityState = EntityState.Modified;

                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
            }
            else
            {
                resutlt.ResultCode = 0;
                resutlt.Message = "fail";
                resutlt.isSuccess = false;
            }
            return resutlt;
        }

        /// <summary>
        /// 获取商品税收编码列表(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<CommodityTaxRateZphDto> GetSingleCommodityCodeExt()
        {
            ThirdResponse<CommodityTaxRateZphDto> res = new ThirdResponse<CommodityTaxRateZphDto>();

            CommodityTaxRateZphDto commoditytaxratezphdto = new CommodityTaxRateZphDto();

            var commodityTaxRate = from c in CommodityTaxRate.ObjectSet()
                                   orderby c.Code
                                   select new CommodityTaxRateZphDTO
                                   {
                                       Code = c.Code,
                                       Name = c.Name,
                                       TaxRate = (double)c.TaxRate
                                   };
            commoditytaxratezphdto.Count = commodityTaxRate.Count();
            commoditytaxratezphdto.Data = commodityTaxRate.ToList();

            res.Code = 200;
            res.Result = commoditytaxratezphdto;
            res.Msg = "查询成功!";
            return res;
        }


        /// <summary>
        /// 同步商品库存(openApi)
        /// </summary>
        public ThirdResponse ModifyCommodityStockExt(string Code, string skuId, int Stock)
        {
            ThirdResponse dto = null;
            if (string.IsNullOrEmpty(Code))
            {
                return dto = new ThirdResponse { Code = 200, Msg = "Code不能为空" };
            }
            if (string.IsNullOrEmpty(skuId))
            {
                return dto = new ThirdResponse { Code = 200, Msg = "skuId不能为空" };
            }
            if (Stock < 0)
            {
                return dto = new ThirdResponse { Code = 200, Msg = "库存数量不能小于0" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().Where(p => p.JDCode == skuId && p.IsDel == false).ToList();
                var commodityStock = CommodityStock.ObjectSet().Where(p => p.JDCode == skuId).ToList();
                var thirdecstockjournal = ThirdECStockJournal.ObjectSet().Where(p => p.Code == Code).ToList();
                List<ThirdECStockJournal> ThirdECStockJournallist = new List<ThirdECStockJournal>();
                if (thirdecstockjournal.Any())
                {
                    return dto = new ThirdResponse { Code = 200, Msg = "已经同步过了" };
                }
                if (commodity.Any())
                {
                    foreach (var item in commodity)
                    {
                        item.Stock = Stock;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;

                        ThirdECStockJournal model = new ThirdECStockJournal();
                        model.Id = Guid.NewGuid();
                        model.SubTime = DateTime.Now;
                        model.AppId = Guid.Empty;
                        model.CommodityId = item.Id;
                        model.CommoditySkuId = skuId;
                        model.CommodityStockId = Guid.Empty;
                        model.CommodityStockOldStock = 0;
                        model.CommodityStockNewStock = Stock;
                        model.CommodityStockSkuId = skuId;
                        model.CommodityOldStock = item.Stock;
                        model.CommodityNewStock = Stock;
                        model.Code = Code;
                        model.Json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        ThirdECStockJournallist.Add(model);
                    }

                }

                if (commodityStock != null && commodityStock.Count() > 0)
                {
                    foreach (var item in commodityStock)
                    {
                        item.Stock = Stock;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                        ThirdECStockJournal model = new ThirdECStockJournal();
                        model.Id = Guid.NewGuid();
                        model.SubTime = DateTime.Now;
                        model.AppId = Guid.Empty;
                        model.CommodityId = item.CommodityId;
                        model.CommoditySkuId = skuId;
                        model.CommodityStockId = item.Id;
                        model.CommodityStockOldStock = item.Stock;
                        model.CommodityStockNewStock = Stock;
                        model.CommodityStockSkuId = skuId;
                        model.CommodityOldStock = 0;
                        model.CommodityNewStock = Stock;
                        model.Code = Code;
                        model.Json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                        ThirdECStockJournallist.Add(model);

                    }
                }

                if (ThirdECStockJournallist != null && ThirdECStockJournallist.Count() > 0)
                {
                    foreach (var item in ThirdECStockJournallist)
                    {
                        item.EntityState = EntityState.Added;
                        contextSession.SaveObject(item);
                    }
                }

                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 200, Msg = "同步库存成功" };
                }


            }
            catch (Exception ex)
            {
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 添加商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse AddCommodityExt(List<Commoditydto> objlist)
        {
            ThirdResponse dto = null;
            //验证AppId的正确性
            ResultDTO _result = IsExistsAppIdOwnerIdTypeExt(objlist);
            if (_result.isSuccess == false)
            {
                return dto = new ThirdResponse { Code = 200, Msg = _result.Message };
            }

            //验证字段属性规则
            ResultDTO result = IsCheckcommodityExt(objlist);
            if (result.isSuccess == false)
            {
                return dto = new ThirdResponse { Code = 200, Msg = result.Message };
            }
            try
            {
                int count = 0;
                foreach (var item in objlist)
                {
                    count++;
                    dto = _AddCommodityExt(item);
                    if (dto.Code == 1)
                    {
                        return new ThirdResponse { Code = 200, Msg = "第" + count + "条数据添加失败" };
                    }
                }

                dto = new ThirdResponse { Code = 200, Msg = "添加商品成功" };
            }
            catch (Exception ex)
            {
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 添加商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse _AddCommodityExt(Commoditydto model)
        {
            ThirdResponse dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //获取运费模板
                var FreightTemplateInfo = FreightTemplate.ObjectSet().FirstOrDefault(p => p.AppId == model.AppId && p.IsDefault == 1);

                var minSortValueQuery = (from m in Commodity.ObjectSet()
                                         where m.AppId == model.AppId && m.CommodityType == 0
                                         select m);
                int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                int minSortValue = 2;
                if (minSort.HasValue)
                {
                    minSortValue = minSort.Value;
                }
                #region 添加商品
                Commodity commodity = new Commodity();
                Guid userId = this.ContextDTO.LoginUserID;
                commodity.Id = Guid.NewGuid();
                commodity.Name = model.Name;
                commodity.Code = model.Code;
                commodity.SubId = userId;
                commodity.SubTime = DateTime.Now;
                commodity.ModifiedOn = DateTime.Now;
                commodity.ModifiedId = userId;
                commodity.No_Number = model.No_Number;
                commodity.Price = model.Price;
                commodity.Stock = model.Stock;
                commodity.PicturesPath = model.PicturesPath;
                commodity.Description = model.Description;
                commodity.State = 1;//未上架
                commodity.IsDel = false;
                commodity.AppId = model.AppId;
                commodity.No_Code = model.JDCode + "0000";
                commodity.TotalCollection = 0;
                commodity.TotalReview = 0;
                commodity.Salesvolume = 0;
                commodity.ComAttribute = model.ComAttribute;
                commodity.CategoryName = model.CategoryName;
                commodity.SortValue = minSortValue - 1;
                commodity.FreightTemplateId = FreightTemplateInfo.Id;  //99元以下商品8元运费
                commodity.MarketPrice = model.MarketPrice;
                commodity.Weight = model.Weight;
                commodity.SaleService = model.SaleService;
                commodity.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,640000,520000,130000,630000";//出去港澳台 新疆 西藏                
                commodity.SharePercent = model.SharePercent;
                commodity.CommodityType = model.CommodityType;
                commodity.Duty = model.Duty;
                commodity.TaxRate = model.TaxRate;
                commodity.TaxClassCode = model.TaxClassCode;
                commodity.Unit = model.Unit;
                commodity.InputRax = model.InputRax;
                commodity.Barcode = model.Barcode;
                commodity.JDCode = model.JDCode;
                //添加京东商品
                commodity.CostPrice = model.CostPrice ?? 0;
                commodity.ServiceSettingId = null;
                commodity.TechSpecs = model.TechSpecs;
                commodity.Type = model.CommodityType;
                commodity.YJCouponActivityId = model.YJCouponActivityId;
                commodity.YJCouponType = model.YJCouponType;
                commodity.ErQiCode = model.ErQiCode;
                commodity.IsAssurance = null;
                commodity.IsReturns = null;
                commodity.Isnsupport = null;
                commodity.VideoName = null;
                if (FreightTemplateInfo != null)
                {
                    commodity.FreightTemplateId = FreightTemplateInfo.Id;
                }
                else
                {
                    commodity.FreightTemplateId = Guid.Empty;
                }
                commodity.SpuId = model.SpuId;
                commodity.EntityState = EntityState.Added;
                contextSession.SaveObject(commodity);
                commodity.RefreshCache(commodity.EntityState);
                #endregion

                #region 添加商品属性
                List<ComAttributeDTO> ComAttributeslist = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(model.ComAttribute);
                //判断是否有组合属性
                if (ComAttributeslist.Any())
                {
                    var AttributeNamelist = ComAttributeslist.GroupBy(p => p.Attribute).Select(p => p.FirstOrDefault()).Select(p => new { p.Attribute }).ToList();
                    string firstAttributeName = AttributeNamelist[0].Attribute;
                    string secondAttributeName = AttributeNamelist[1].Attribute;
                    List<ComAttributeDTO> firstAttributelist = ComAttributeslist.Where(p => p.Attribute == firstAttributeName).ToList();
                    List<ComAttributeDTO> secondAttributelist = ComAttributeslist.Where(p => p.Attribute == secondAttributeName).ToList();
                    foreach (var item in firstAttributelist)
                    {
                        foreach (var _item in secondAttributelist)
                        {
                            List<ComAttributeDTO> AttrList = new List<ComAttributeDTO>();
                            AttrList.Add(item);
                            AttrList.Add(_item);
                            #region 添加库存
                            CommodityStock commoditystock = new CommodityStock();
                            commoditystock.Id = Guid.NewGuid();
                            commoditystock.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(AttrList);
                            commoditystock.CommodityId = commodity.Id;
                            commoditystock.Price = commodity.Price;
                            commoditystock.Stock = commodity.Stock;
                            commoditystock.Duty = commodity.Duty;
                            commoditystock.Barcode = commodity.Barcode;
                            commoditystock.No_Code = commodity.No_Code;
                            commoditystock.ThumImg = commodity.PicturesPath;
                            commoditystock.JDCode = commodity.JDCode;
                            commoditystock.ErQiCode = commodity.ErQiCode;
                            commoditystock.CostPrice = commodity.CostPrice;
                            commoditystock.EntityState = EntityState.Added;
                            contextSession.SaveObject(commoditystock);
                            #endregion
                        }
                    }
                }
                #endregion

                #region 添加一级属性
                if (ComAttributeslist.Any())
                {
                    foreach (var item in ComAttributeslist)
                    {
                        var exEntity = Jinher.AMP.BTP.BE.Attribute.ObjectSet().FirstOrDefault(p => p.AppId == model.AppId && p.IsDel == false && p.Name == item.Attribute);
                        if (exEntity == null)
                        {
                            Jinher.AMP.BTP.BE.Attribute attribute = new Jinher.AMP.BTP.BE.Attribute();
                            attribute.Id = Guid.NewGuid();
                            attribute.Name = item.Attribute;
                            attribute.Code = model.Code;
                            attribute.SubTime = DateTime.Now;
                            attribute.SubId = userId;
                            attribute.AppId = model.AppId;
                            attribute.IsDel = false;
                            attribute.ModifiedOn = DateTime.Now;
                            attribute.EntityState = EntityState.Added;
                            contextSession.SaveObject(attribute);

                        }
                    }
                }
                #endregion

                #region 商品图片
                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                pic.Name = "商品图片";
                pic.SubId = userId;
                pic.SubTime = DateTime.Now;
                pic.PicturesPath = commodity.PicturesPath;
                pic.CommodityId = commodity.Id;
                pic.Sort = Convert.ToInt32(commodity.SortValue);
                pic.EntityState = EntityState.Added;
                contextSession.SaveObject(pic);
                #endregion

                #region 商品分类
                var CategoryList = Category.ObjectSet().Where(p => p.AppId == model.AppId && p.IsDel == false && p.Name == model.CategoryName).ToList();
                if (CategoryList.Any())
                {
                    foreach (var itemcate in CategoryList)
                    {
                        CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                        comcate.CategoryId = itemcate.Id;
                        comcate.CommodityId = commodity.Id;
                        comcate.SubId = userId;
                        comcate.SubTime = DateTime.Now;
                        comcate.Name = "商品分类";
                        comcate.IsDel = false;
                        comcate.AppId = model.AppId;
                        comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                        contextSession.SaveObject(comcate);
                    }
                }
                #endregion

                #region 商城分类
                var innerCateid = InnerCategory.ObjectSet().Where(p => p.Name == model.CityCategory && p.AppId == model.AppId && p.IsDel == false).FirstOrDefault();
                if (innerCateid != null)
                {
                    CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                    comInnerCate.CategoryId = innerCateid.Id;
                    comInnerCate.CommodityId = commodity.Id;
                    comInnerCate.SubId = userId;
                    comInnerCate.SubTime = DateTime.Now;
                    comInnerCate.Name = "商品分类";
                    comInnerCate.IsDel = false;
                    comInnerCate.AppId = model.AppId;
                    comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                    comInnerCate.EntityState = EntityState.Added;
                    contextSession.SaveObject(comInnerCate);
                }
                #endregion

                #region 关联商品
                if (commodity != null)
                {
                    RelationCommodity relamodel = RelationCommodity.CreateRelationCommodity();
                    relamodel.CommodityId = commodity.Id;
                    relamodel.CommodityPicturesPath = commodity.PicturesPath == null ? "" : commodity.PicturesPath;
                    relamodel.CommodityName = commodity.Name;
                    relamodel.RelationCommodityId = commodity.Id;
                    relamodel.No_Code = commodity.No_Code;
                    relamodel.EntityState = EntityState.Added;
                    contextSession.SaveObject(relamodel);
                }

                #endregion

                #region 商品日志

                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                journal.EntityState = EntityState.Added;
                contextSession.SaveObject(journal);

                #endregion



                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 0, Msg = "添加商品成功" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Debug(string.Format("openapi添加商品异常信息:{0}", ex.Message));
                dto = new ThirdResponse { Code = 1, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 添加京东商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse AddJdCommodityExt(CommodityDTO model)
        {
            ThirdResponse dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //获取运费模板
                var FreightTemplateInfo = FreightTemplate.ObjectSet().FirstOrDefault(p => p.AppId == model.AppId && p.IsDefault == 1);
                //获取京东商品协议价格和京东价格
                List<JdPriceDto> jdPrices = new List<JdPriceDto>();
                List<string> skuIds = new List<string>();
                skuIds.Add(model.JDCode);
                jdPrices.AddRange(JDSV.GetPrice(skuIds));

                List<JdComPicturesDto> JdComPics = new List<JdComPicturesDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    JdComPics.AddRange(JDSV.GetComPictures(skuIds.Skip(i).Take(99).ToList()));
                }

                var minSortValueQuery = (from m in Commodity.ObjectSet()
                                         where m.AppId == model.AppId && m.CommodityType == 0
                                         select m);
                int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                int minSortValue = 2;
                if (minSort.HasValue)
                {
                    minSortValue = minSort.Value;
                }
                //获取京东商品详情
                List<JdComDetailDto> JdComDetailList = new List<JdComDetailDto>();
                foreach (var item in skuIds)
                {
                    JdComDetailList.Add(JDSV.GetJdDetail(item));
                }

                #region 添加商品
                Commodity commodity = new Commodity();
                Guid userId = this.ContextDTO.LoginUserID;
                commodity.Id = Guid.NewGuid();
                commodity.Name = model.Name;
                commodity.Code = null;
                commodity.SubId = userId;
                commodity.SubTime = DateTime.Now;
                commodity.ModifiedOn = DateTime.Now;
                commodity.ModifiedId = userId;
                commodity.No_Number = 0;
                var jdPrice = jdPrices.Where(p => p.SkuId == model.JDCode).FirstOrDefault();
                commodity.Price = jdPrice.JdPrice;
                commodity.Stock = 999;
                commodity.PicturesPath = "http://img13.360buyimg.com/n1/" + JdComPics.FirstOrDefault(p => p.isPrimary == "1").path;
                var JdComDetailInfo = JdComDetailList.Where(p => p.sku == model.JDCode).FirstOrDefault();
                if (JdComDetailInfo.appintroduce != null && JdComDetailInfo.appintroduce != "")
                {
                    string Div = JdComDetailInfo.appintroduce.Substring(0, 10);
                    if (Div.Contains("<div"))
                    {
                        commodity.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.appintroduce.Replace("\'", "\"").Replace("\'", "\"") + "</div>";
                    }
                    else
                    {
                        commodity.Description = JdComDetailInfo.appintroduce.Replace("\'", "\"").Replace("\'", "\"");
                    }
                }
                else
                {
                    string Div = JdComDetailInfo.introduction.Substring(0, 10);
                    if (Div.Contains("<div"))
                    {
                        commodity.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"") + "</div>";
                    }
                    else
                    {
                        commodity.Description = JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"");
                    }
                }
                commodity.State = 1;
                commodity.IsDel = false;
                commodity.AppId = model.AppId;
                commodity.No_Code = model.JDCode + "0000";
                commodity.TotalCollection = 0;
                commodity.TotalReview = 0;
                commodity.Salesvolume = 0;
                commodity.ComAttribute = null;
                commodity.CategoryName = null;
                commodity.SortValue = minSortValue - 1;
                commodity.FreightTemplateId = FreightTemplateInfo.Id;  //99元以下商品8元运费
                commodity.MarketPrice = null;
                commodity.Weight = JdComDetailInfo.weight;
                commodity.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                commodity.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,640000,520000,130000,630000";//出去港澳台 新疆 西藏                
                commodity.SharePercent = null;
                commodity.CommodityType = 0;
                commodity.Duty = null;
                commodity.TaxRate = model.TaxRate;
                commodity.TaxClassCode = model.TaxClassCode;
                commodity.Unit = string.IsNullOrEmpty(JdComDetailInfo.saleUnit) ? "件" : JdComDetailInfo.saleUnit;
                commodity.InputRax = model.InputRax;
                commodity.Barcode = JdComDetailInfo.upc;
                commodity.JDCode = model.JDCode;
                //添加京东商品
                commodity.CostPrice = jdPrice.Price ?? 0;
                commodity.ServiceSettingId = null;
                commodity.TechSpecs = JdComDetailInfo.param;
                commodity.Type = 0;
                commodity.YJCouponActivityId = "";
                commodity.YJCouponType = "";
                commodity.ErQiCode = null;
                commodity.IsAssurance = model.IsAssurance;
                commodity.IsReturns = model.IsReturns;
                commodity.Isnsupport = model.Isnsupport;
                commodity.VideoName = model.VideoName;
                if (FreightTemplateInfo != null)
                {
                    commodity.FreightTemplateId = FreightTemplateInfo.Id;
                }
                else
                {
                    commodity.FreightTemplateId = Guid.Empty;
                }
                commodity.SpuId = model.SpuId;
                commodity.EntityState = EntityState.Added;
                contextSession.SaveObject(commodity);
                commodity.RefreshCache(commodity.EntityState);
                #endregion

                #region 商品图片
                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                pic.Name = "商品图片";
                pic.SubId = userId;
                pic.SubTime = DateTime.Now;
                pic.PicturesPath = commodity.PicturesPath;
                pic.CommodityId = commodity.Id;
                pic.Sort = Convert.ToInt32(commodity.SortValue);
                pic.EntityState = EntityState.Added;
                contextSession.SaveObject(pic);
                #endregion

                #region 商品分类
                var CategoryList = Category.ObjectSet().Where(p => p.AppId == model.AppId && p.IsDel == false && p.Name == model.CategoryName).ToList();
                if (CategoryList.Any())
                {
                    foreach (var itemcate in CategoryList)
                    {
                        CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                        comcate.CategoryId = itemcate.Id;
                        comcate.CommodityId = commodity.Id;
                        comcate.SubId = userId;
                        comcate.SubTime = DateTime.Now;
                        comcate.Name = "商品分类";
                        comcate.IsDel = false;
                        comcate.AppId = model.AppId;
                        comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                        contextSession.SaveObject(comcate);
                    }
                }
                #endregion

                #region 商城分类
                var innerCateid = InnerCategory.ObjectSet().Where(p => p.Name == model.VideoName && p.AppId == model.AppId && p.IsDel == false).FirstOrDefault();
                if (innerCateid != null)
                {
                    CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                    comInnerCate.CategoryId = innerCateid.Id;
                    comInnerCate.CommodityId = commodity.Id;
                    comInnerCate.SubId = userId;
                    comInnerCate.SubTime = DateTime.Now;
                    comInnerCate.Name = "商品分类";
                    comInnerCate.IsDel = false;
                    comInnerCate.AppId = model.AppId;
                    comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                    comInnerCate.EntityState = EntityState.Added;
                    contextSession.SaveObject(comInnerCate);
                }
                #endregion

                #region 关联商品
                if (commodity != null)
                {
                    RelationCommodity relamodel = RelationCommodity.CreateRelationCommodity();
                    relamodel.CommodityId = commodity.Id;
                    relamodel.CommodityPicturesPath = commodity.PicturesPath == null ? "" : commodity.PicturesPath;
                    relamodel.CommodityName = commodity.Name;
                    relamodel.RelationCommodityId = commodity.Id;
                    relamodel.No_Code = commodity.No_Code;
                    relamodel.EntityState = EntityState.Added;
                    contextSession.SaveObject(relamodel);
                }

                #endregion

                #region 商品日志

                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                journal.EntityState = EntityState.Added;
                contextSession.SaveObject(journal);

                #endregion

                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 200, Msg = "添加商品成功" };
                }

            }
            catch (Exception ex)
            {
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 添加网易严选商品(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse AddYXCommodityExt(CommodityDTO model)
        {
            ThirdResponse dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid userId = this.ContextDTO.LoginUserID;
                List<YXComDetailDTO> YXComDetailList = new List<YXComDetailDTO>();
                List<string> skuIds = new List<string>();
                skuIds.Add(model.JDCode);
                YXComDetailList.AddRange(YXSV.GetComDetailList(skuIds));
                //获取运费模板
                var FreightTemplateInfo = FreightTemplate.ObjectSet().FirstOrDefault(p => p.AppId == model.AppId && p.IsDefault == 1);

                var minSortValueQuery = (from m in Commodity.ObjectSet()
                                         where m.AppId == model.AppId && m.CommodityType == 0
                                         select m);
                int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                int minSortValue = 2;
                if (minSort.HasValue)
                {
                    minSortValue = minSort.Value;
                }

                #region 添加商品
                Commodity commodity = new Commodity();
                var YXComDetailInfo = YXComDetailList.FirstOrDefault();
                commodity.Id = Guid.NewGuid();
                commodity.Name = YXComDetailList.FirstOrDefault().name;
                commodity.Code = null;
                commodity.SubId = userId;
                commodity.SubTime = DateTime.Now;
                commodity.ModifiedOn = DateTime.Now;
                commodity.ModifiedId = userId;
                commodity.No_Number = 0;
                commodity.Price = YXComDetailInfo.skuList.Min(p => p.channelPrice);
                commodity.Stock = 999;
                commodity.PicturesPath = YXComDetailInfo.listPicUrl;
                commodity.Description = "<div class=\"JD-goods\">" + YXComDetailInfo.itemDetail.detailHtml + "</div>";

                commodity.State = 1;
                commodity.IsDel = false;
                commodity.AppId = model.AppId;
                commodity.TotalCollection = 0;
                commodity.TotalReview = 0;
                commodity.Salesvolume = 0;
                commodity.ComAttribute = null;
                commodity.CategoryName = null;
                commodity.SortValue = minSortValue - 1;
                commodity.FreightTemplateId = FreightTemplateInfo.Id;
                commodity.MarketPrice = null;
                commodity.Weight = null;
                commodity.SaleService = null;
                commodity.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,640000,520000,130000,630000";//出去港澳台 新疆 西藏                
                commodity.SharePercent = null;
                commodity.CommodityType = 0;
                commodity.Duty = null;
                commodity.TaxRate = model.TaxRate;
                commodity.TaxClassCode = model.TaxClassCode;
                commodity.Unit = "件";
                commodity.InputRax = model.InputRax;
                if (YXComDetailInfo.skuList.Count() == 1)
                {
                    //单条属性与库存表保持一致
                    commodity.JDCode = YXComDetailInfo.skuList.FirstOrDefault().id;
                    commodity.Code = model.JDCode;
                    commodity.Barcode = YXComDetailInfo.id;
                    commodity.No_Code = model.JDCode;
                }
                else
                {
                    //多条属性存储SPU
                    commodity.JDCode = YXComDetailInfo.skuList.OrderBy(p => p.channelPrice).FirstOrDefault().id;
                    commodity.Code = model.JDCode;
                    commodity.Barcode = YXComDetailInfo.id; //存严选商品的SPU
                    commodity.No_Code = model.JDCode;
                }
                commodity.CostPrice = commodity.Price * Convert.ToDecimal(0.8);
                commodity.ServiceSettingId = null;
                commodity.TechSpecs = null;
                commodity.Type = 0;
                commodity.YJCouponActivityId = "";
                commodity.YJCouponType = "";
                commodity.ErQiCode = null;
                commodity.IsAssurance = model.IsAssurance;
                commodity.IsReturns = model.IsReturns;
                commodity.Isnsupport = model.Isnsupport;
                commodity.VideoName = model.VideoName;
                if (FreightTemplateInfo != null)
                {
                    commodity.FreightTemplateId = FreightTemplateInfo.Id;
                }
                else
                {
                    commodity.FreightTemplateId = Guid.Empty;
                }
                commodity.SpuId = model.SpuId;
                List<ComAttributeDTO> ComAttrs = new List<ComAttributeDTO>();
                foreach (var skuitem in YXComDetailInfo.skuList)
                {
                    foreach (var it in skuitem.itemSkuSpecValueList)
                    {
                        ComAttributeDTO ComAtt = new ComAttributeDTO();
                        ComAtt.Attribute = it.skuSpec.name;
                        ComAtt.SecondAttribute = it.skuSpecValue.value;
                        ComAttrs.Add(ComAtt);
                    }
                }
                commodity.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(ComAttrs);
                commodity.EntityState = EntityState.Added;
                contextSession.SaveObject(commodity);
                commodity.RefreshCache(commodity.EntityState);
                #endregion

                #region 商品图片
                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                pic.Name = "商品图片";
                pic.SubId = userId;
                pic.SubTime = DateTime.Now;
                pic.PicturesPath = commodity.PicturesPath;
                pic.CommodityId = commodity.Id;
                pic.Sort = Convert.ToInt32(commodity.SortValue);
                pic.EntityState = EntityState.Added;
                contextSession.SaveObject(pic);
                #endregion

                #region 商品分类
                var CategoryList = Category.ObjectSet().Where(p => p.AppId == model.AppId && p.IsDel == false && p.Name == model.CategoryName).ToList();
                if (CategoryList.Any())
                {
                    foreach (var itemcate in CategoryList)
                    {
                        CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                        comcate.CategoryId = itemcate.Id;
                        comcate.CommodityId = commodity.Id;
                        comcate.SubId = userId;
                        comcate.SubTime = DateTime.Now;
                        comcate.Name = "商品分类";
                        comcate.IsDel = false;
                        comcate.AppId = model.AppId;
                        comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                        contextSession.SaveObject(comcate);
                    }
                }
                #endregion

                #region 商城分类

                var innerCateid = InnerCategory.ObjectSet().Where(p => p.Name == model.VideoName && p.AppId == model.AppId && p.IsDel == false).FirstOrDefault();
                if (innerCateid != null)
                {
                    CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                    comInnerCate.CategoryId = innerCateid.Id;
                    comInnerCate.CommodityId = commodity.Id;
                    comInnerCate.SubId = userId;
                    comInnerCate.SubTime = DateTime.Now;
                    comInnerCate.Name = "商品分类";
                    comInnerCate.IsDel = false;
                    comInnerCate.AppId = model.AppId;
                    comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(model.AppId);
                    comInnerCate.EntityState = EntityState.Added;
                    contextSession.SaveObject(comInnerCate);
                }
                #endregion

                #region 关联商品
                if (commodity != null)
                {
                    RelationCommodity relamodel = RelationCommodity.CreateRelationCommodity();
                    relamodel.CommodityId = commodity.Id;
                    relamodel.CommodityPicturesPath = commodity.PicturesPath == null ? "" : commodity.PicturesPath;
                    relamodel.CommodityName = commodity.Name;
                    relamodel.RelationCommodityId = commodity.Id;
                    relamodel.No_Code = commodity.No_Code;
                    relamodel.EntityState = EntityState.Added;
                    contextSession.SaveObject(relamodel);
                }

                #endregion

                #region 商品日志

                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                journal.EntityState = EntityState.Added;
                contextSession.SaveObject(journal);

                #endregion

                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 200, Msg = "添加商品成功" };
                }

            }
            catch (Exception ex)
            {
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 修改商品名称(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityNameExt(string skuId, string CommodityName)
        {
            ThirdResponse dto = null;
            if (string.IsNullOrEmpty(skuId))
            {
                return dto = new ThirdResponse { Code = 200, Msg = "skuId不能为空" };
            }
            if (string.IsNullOrWhiteSpace(CommodityName))
            {
                return dto = new ThirdResponse { Code = 200, Msg = "商品名称不能为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().Where(p => p.JDCode == skuId && p.IsDel == false).ToList();
                if (commodity.Any())
                {
                    foreach (var item in commodity)
                    {
                        item.Name = CommodityName;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                    }
                }
                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 200, Msg = "修改成功" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改商品名称异常信息:{0}", ex.Message), ex);
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }

        /// <summary>
        /// 修改商品价格(openApi)
        /// </summary>
        /// <returns></returns>
        public ThirdResponse ModifyCommodityPriceExt(string skuId, decimal Price)
        {

            ThirdResponse dto = null;
            if (string.IsNullOrEmpty(skuId))
            {
                return dto = new ThirdResponse { Code = 200, Msg = "skuId不能为空" };
            }
            if (Price < 0)
            {
                return dto = new ThirdResponse { Code = 200, Msg = "商品价格不能小于0" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().Where(p => p.JDCode == skuId && p.IsDel == false).ToList();
                if (commodity.Any())
                {
                    foreach (var item in commodity)
                    {
                        item.Price = Price;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                    }
                }

                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 200, Msg = "修改成功" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改商品价格异常信息:{0}", ex.Message), ex);
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 修改商品上下架(openApi)
        /// </summary>
        /// <param name="SpuId"></param>
        /// <param name="State">0上架，1下架</param>
        /// <returns></returns>
        public ThirdResponse UpperandlowerExt(string SpuId, int State)
        {
            ThirdResponse dto = null;
            if (string.IsNullOrEmpty(SpuId))
            {
                return dto = new ThirdResponse { Code = 200, Msg = "SpuId不能为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().Where(p => p.SpuId == SpuId && p.IsDel == false).ToList();
                if (commodity.Any())
                {
                    foreach (var item in commodity)
                    {
                        item.State = State;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                    }
                }

                if (contextSession.SaveChanges() > 0)
                {
                    dto = new ThirdResponse { Code = 200, Msg = "上下架成功" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改商品上下架异常信息:{0}", ex.Message), ex);
                dto = new ThirdResponse { Code = 200, Msg = ex.Message };
            }
            return dto;
        }


        /// <summary>
        /// 验证商品
        /// </summary>
        /// <returns></returns>
        public ResultDTO IsCheckcommodityExt(List<Commoditydto> objlist)
        {
            ResultDTO dto = null;
            List<JdPriceDto> jdPrices = new List<JdPriceDto>();
            try
            {
                string pattern = @"^\d+(.\d{1,2})?$";
                bool result = false;
                int count = 0;
                foreach (var item in objlist)
                {
                    count++;
                    if (!string.IsNullOrEmpty(item.SpuId))
                    {
                        var commodity = Commodity.ObjectSet().Where(p => p.AppId == item.AppId && p.IsDel == false && p.SpuId == item.SpuId).FirstOrDefault();
                        if (commodity != null)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中SpuId在数据库中已存在~", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中SpuId不能为空~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.JDCode))
                    {
                        if (IsExistsJdCodeExt(item.JDCode, item.AppId))
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中skuId在数据库中已存在~", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中SkuId不能为空~", isSuccess = false };
                    }
                    if (string.IsNullOrWhiteSpace(item.Name))
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品名称不能为空~", isSuccess = false };
                    }

                    var commodityTaxRate = CommodityTaxRate.ObjectSet().Where(p => p.Code == item.TaxClassCode).AsQueryable();
                    if (!commodityTaxRate.Any())
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中税收编码不能为空~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.TaxRate.ToString()))
                    {
                        result = Regex.IsMatch(item.TaxRate.ToString(), pattern);
                        if (result == false)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中销项税只能是正数和最多保留小数2位", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中销项税不能为空~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.InputRax.ToString()))
                    {
                        result = Regex.IsMatch(item.InputRax.ToString(), pattern);
                        if (result == false)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中进项税只能是正数和最多保留小数2位", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中进项税不能为空~", isSuccess = false };
                    }
                    if (string.IsNullOrWhiteSpace(item.Barcode))
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品条码不能为空~", isSuccess = false };
                    }
                    if (item.Stock < 0)
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品库存不能为负数~", isSuccess = false };
                    }
                    if (string.IsNullOrWhiteSpace(item.Unit))
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品计量单位不能为空~", isSuccess = false };
                    }
                    var category = InnerCategory.ObjectSet().Where(p => p.IsDel == false && p.Name == item.CityCategory && p.AppId == item.AppId).AsQueryable();
                    if (!category.Any())
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商城品类不能为空~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.CostPrice.ToString()))
                    {
                        if (item.CostPrice < 0)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品进货价不能为负数~", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品进货价必填~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.Price.ToString()))
                    {
                        if (item.Price < 0)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品现价不能为负数~", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品现价必填~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.MarketPrice.ToString()))
                    {
                        if (item.MarketPrice < 0)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品市场价不能为负数~", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品市场价必填~", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.Stock.ToString()))
                    {
                        if (item.Stock < 0)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品库存不能为负数~", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品库存必填~", isSuccess = false };
                    }

                    if (string.IsNullOrWhiteSpace(item.PicturesPath))
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品详情图必填~", isSuccess = false };
                    }
                    List<int> Commoditylist = new List<int>();
                    Commoditylist.Add(0);
                    Commoditylist.Add(1);
                    if (!Commoditylist.Contains(item.CommodityType))
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品类型只能填0和1", isSuccess = false };
                    }
                    if (!string.IsNullOrWhiteSpace(item.Duty.ToString()))
                    {
                        result = Regex.IsMatch(item.Duty.ToString(), pattern);
                        if (result == false)
                        {
                            return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中关税只能是正数和最多保留小数2位", isSuccess = false };
                        }
                    }
                    else
                    {
                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中税率不能为空~", isSuccess = false };
                    }
                }

            }
            catch (Exception ex)
            {
                return dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }

            return dto = new ResultDTO { ResultCode = 0, isSuccess = true };
        }


        /// <summary>
        /// 验证商品(old方法)
        /// </summary>
        /// <returns></returns>
        //public ResultDTO IsCheckcommodityExt(List<Commoditydto> objlist)
        //{
        //    ResultDTO dto = null;
        //    List<JdPriceDto> jdPrices = new List<JdPriceDto>();
        //    try
        //    {
        //        int count = 0;
        //        foreach (var item in objlist)
        //        {
        //            count++;
        //            if (!string.IsNullOrEmpty(item.SpuId))
        //            {
        //                var commodity = Commodity.ObjectSet().Where(p => p.AppId == item.AppId && p.IsDel == false && p.SpuId == item.SpuId).FirstOrDefault();
        //                if (commodity != null)
        //                {
        //                    return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中SpuId在数据库中已存在~", isSuccess = false };
        //                }
        //            }
        //            else
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中SpuId不能为空~", isSuccess = false };
        //            }
        //            if (!string.IsNullOrWhiteSpace(item.SkuId))
        //            {
        //                if (IsExistsJdCodeExt(item.SkuId, item.AppId))
        //                {
        //                    return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中备注编码在数据库中已存在~", isSuccess = false };
        //                }
        //                if (ThirdECommerceHelper.IsJingDongDaKeHu(item.AppId))
        //                {
        //                    //获取京东商品协议价格和京东价格
        //                    List<string> skuIds = new List<string>();
        //                    skuIds.Add(item.SkuId);
        //                    jdPrices.AddRange(JDSV.GetPrice(skuIds));
        //                    if (!jdPrices.Any())
        //                    {
        //                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中无效备注编码京东商品池中不存在,请重新输入~", isSuccess = false };
        //                    }
        //                }
        //                if (ThirdECommerceHelper.IsWangYiYanXuan(item.AppId))
        //                {
        //                    //获取严选的spu商品id
        //                    var SPUIdLis = YXSV.GetAllSPU();
        //                    if (!SPUIdLis.Contains(item.SkuId))
        //                    {
        //                        return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中无效备注编码网易严选商品池中不存在,请重新输入~", isSuccess = false };
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中备注编码不能为空~", isSuccess = false };
        //            }

        //            if (string.IsNullOrWhiteSpace(item.Name))
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品名称不能为空~", isSuccess = false };
        //            }
        //            if (item.Stock < 0)
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品库存要大于0~", isSuccess = false };
        //            }
        //            var category = InnerCategory.ObjectSet().Where(p => p.IsDel == false && p.Name == item.CityCategory && p.AppId == item.AppId).AsQueryable();
        //            if (!category.Any())
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商城品类不能为空~", isSuccess = false };
        //            }

        //            var commodityTaxRate = CommodityTaxRate.ObjectSet().Where(p => p.Code == item.TaxClassCode).AsQueryable();
        //            if (!commodityTaxRate.Any())
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中税收编码不能为空~", isSuccess = false };
        //            }
        //            if (!string.IsNullOrWhiteSpace(item.TaxRate.ToString()))
        //            {
        //                if (item.TaxRate < 0)
        //                {
        //                    return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中销项税不能为负数~", isSuccess = false };
        //                }
        //            }
        //            else
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中销项税不能为空~", isSuccess = false };
        //            }
        //            if (!string.IsNullOrWhiteSpace(item.InputRax.ToString()))
        //            {
        //                if (item.InputRax < 0)
        //                {
        //                    return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中进项税不能为负数~", isSuccess = false };
        //                }
        //            }
        //            else
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中进项税不能为空~", isSuccess = false };
        //            }

        //            if (string.IsNullOrWhiteSpace(item.Barcode))
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品条码不能为空~", isSuccess = false };
        //            }

        //            if (item.CostPrice <= 0)
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品进货价必填~", isSuccess = false };
        //            }
        //            if (item.Price <= 0)
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品现价必填~", isSuccess = false };
        //            }
        //            if (string.IsNullOrWhiteSpace(item.Unit))
        //            {
        //                return dto = new ResultDTO { ResultCode = 1, Message = "第" + count + "条数据中商品单位不能为空~", isSuccess = false };
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
        //    }

        //    return dto = new ResultDTO { ResultCode = 0, isSuccess = true };
        //}


        /// <summary>
        /// 检查京东编码是否存在
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public bool IsExistsJdCodeExt(string JdCode, System.Guid appId)
        {
            bool bReturn = false;
            if (!string.IsNullOrWhiteSpace(JdCode))
            {
                var jdcommodity = Commodity.ObjectSet().Where(p => p.JDCode == JdCode && p.AppId == appId && p.IsDel == false).FirstOrDefault();
                if (jdcommodity != null)
                {
                    bReturn = true;
                }
            }
            return bReturn;
        }


        /// <summary>
        /// 检查应用信息是否有效
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public ResultDTO IsExistsAppIdOwnerIdTypeExt(List<Commoditydto> objlist)
        {
            ResultDTO dto = null;
            int count = 0;
            foreach (var item in objlist)
            {
                count++;
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appModel = APPSV.Instance.GetAppOwnerInfo(item.AppId);
                if (appModel == null)
                {
                    return dto = new ResultDTO { ResultCode = 0, Message = "第" + count + "条数据中AppId获取应用信息失败,请重新输入AppId", isSuccess = false };
                }

                if (appModel.OwnerType == 0)
                {
                    CBC.Deploy.CustomDTO.OrgInfoNewDTO orgInfoDTO = CBCSV.Instance.GetOrgInfoNewBySubId(appModel.OwnerId);
                    if (orgInfoDTO == null || string.IsNullOrEmpty(orgInfoDTO.CompanyPhone))
                    {
                        return dto = new ResultDTO { ResultCode = 0, Message = "第" + count + "条数据中AppId获取组织信息失败,请重新输入AppId", isSuccess = false };
                    }
                }
            }

            return dto = new ResultDTO { ResultCode = 1, isSuccess = true }; ;
        }


        #region NetCore刷新缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodity1Ext()
        {

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCommodityExt(JdBTPRefreshCache dict)
        {
            if (dict != null)
            {
                foreach (JdBTPRefreshCacheList item in dict.DictList)
                {
                    Commodity comm = Commodity.ObjectSet().Where(p => p.Id.Equals(item.Key)).FirstOrDefault();
                    if (item.State.Equals(0))
                    {
                        comm.RefreshCache(EntityState.Deleted);
                    }
                    else if (item.State.Equals(1))
                    {
                        comm.RefreshCache(EntityState.Modified);
                    }
                    else if (item.State.Equals(2))
                    {
                        comm.RefreshCache(EntityState.Added);
                    }
                }
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdPromotionsExt(JdBTPRefreshCache dict)
        {
            if (dict != null)
            {

                foreach (JdBTPRefreshCacheList item in dict.DictList)
                {
                    TodayPromotion comm = TodayPromotion.ObjectSet().Where(p => p.Id.Equals(item.Key)).FirstOrDefault();
                    if (item.State.Equals(0))
                    {
                        comm.RefreshCache(EntityState.Deleted);
                    }
                    else if (item.State.Equals(1))
                    {
                        comm.RefreshCache(EntityState.Modified);
                    }
                    else if (item.State.Equals(2))
                    {
                        comm.RefreshCache(EntityState.Added);
                    }
                }
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>

        public ResultDTO NetCoreAutoAuditJdCountInfoExt(JdBTPRefreshCache dict)
        {
            if (dict != null)
            {
                foreach (JdBTPRefreshCacheList item in dict.DictList)
                {
                    if (item.State.Equals(0))
                    {
                        Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", item.Key.ToString(), "BTPCache");
                    }
                }
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        #endregion


        /// <summary>
        /// 根据appid和订单id查找该订单下所有的商品信息(评价用)
        /// </summary>
        /// <param name="Code">订单编号</param>
        /// <param name="OrderId">订单号id</param>
        /// <param name="Commodityid">商品id（单个评价用）</param>
        /// <returns></returns>
        public ResultDTO<List<CommodityDTO>> GetOrderIdComInfoExt(string Code, Guid OrderId, Guid Commodityid)
        {
            ResultDTO<List<CommodityDTO>> result = new ResultDTO<List<CommodityDTO>>() { ResultCode = 0 };
            List<CommodityDTO> com2 = new List<CommodityDTO>();
            try
            {
                var orderinfo = (from info in OrderItem.ObjectSet()
                                 where info.Code == Code && info.CommodityOrderId == OrderId
                                 select info).ToList();

                if (Commodityid != Guid.Empty)
                {
                    orderinfo.Where(p => p.CommodityId == Commodityid);
                }

                if (orderinfo.Count > 0)
                {
                    foreach (var item in orderinfo)
                    {

                        CommodityDTO com = new CommodityDTO();

                        com.AppId = (Guid)item.AppId;
                        com.PicturesPath = item.PicturesPath;
                        com.Name = item.Name;
                        com.Id = item.CommodityId;
                        com.No_Code = item.Id.ToString();//No_Code暂时代替订单项id
                        LogHelper.Debug("根据订单编号和订单id查到的信息：" + com.PicturesPath + "时间：" + DateTime.Now);
                        com2.Add(com);
                    }
                    result.Data = com2;
                    result.isSuccess = true;
                    result.Message = "OK";
                }
                else
                {
                    result.isSuccess = false;
                    result.Message = "未找到相关信息";
                    result.ResultCode = -1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetOrderIdComInfo异常，异常信息：", ex);

                result.Message = "服务异常，请稍后重试！";
                result.ResultCode = -1;
            }

            return result;
        }
    }
}
