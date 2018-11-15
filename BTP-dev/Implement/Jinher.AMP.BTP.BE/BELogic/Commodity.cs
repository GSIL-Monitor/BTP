

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Common;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using CommodityStockDTO = Jinher.AMP.BTP.Deploy.CommodityStockDTO;

namespace Jinher.AMP.BTP.BE
{
    public partial class Commodity
    {
        #region 基类抽象方法重载

        public override void BusinessRuleValidate()
        {
        }
        #endregion
        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion

        /// <summary>
        /// 查询商家所有在售商品 - 有修改
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerID(System.Guid id, int pageSize, int pageIndex)
        {
            var commodityDTO = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId.Equals(id) && n.State == 0 && n.CommodityType == 0).OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Commodity().ToEntityDataList(commodityDTO);
        }

        /// <summary>
        /// 查询商家所有在售商品按照销售量排序 -新增
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerIDBySalesvolume(System.Guid id, int pageSize, int pageIndex, string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice, out int rowCount)
        {

            var query = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId.Equals(id) && n.State == 0 && n.CommodityType == 0);
            #region 条件查询
            if (!string.IsNullOrEmpty(commodityName))
            {
                query = query.Where(n => n.Name.Contains(commodityName));
            }
            if (!string.IsNullOrEmpty(sSalesvolume))
            {
                int s = int.Parse(sSalesvolume);
                query = query.Where(n => n.Salesvolume >= s);
            }
            if (!string.IsNullOrEmpty(eSalesvolume))
            {
                int e = int.Parse(eSalesvolume);
                query = query.Where(n => n.Salesvolume <= e);
            }
            if (!string.IsNullOrEmpty(sPrice))
            {
                decimal s = 0;
                if (!decimal.TryParse(sPrice, out s))//长度越界
                {
                    rowCount = 0;
                    return new System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO>();
                }
                query = query.Where(n => n.Price >= s);
            }
            if (!string.IsNullOrEmpty(ePrice))
            {
                decimal e = 0;
                if (!decimal.TryParse(ePrice, out e))//长度越界
                {
                    rowCount = 0;
                    return new System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO>();
                }
                query = query.Where(n => n.Price <= e);
            }
            if (!string.IsNullOrEmpty(commodityCategory))
            {
                string[] commodityCategoryID = commodityCategory.Split(',');
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
            #endregion
            rowCount = query.Count();
            var list = query.OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new Commodity().ToEntityDataList(list);
        }

        /// <summary>
        /// 查询商家所有下架商品 - 新增
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllNoOnSaleCommodityBySellerID(System.Guid id, int pageSize, int pageIndex)
        {
            var commodityDTO = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId.Equals(id) && n.State == 1 && n.CommodityType == 0).OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Commodity().ToEntityDataList(commodityDTO);
        }

        /// <summary>
        /// 查询商家所有下架商品按照销售量排序 - 新增
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllNoOnSaleCommodityBySellerIDBySalesvolume(System.Guid id, int pageSize, int pageIndex, string commodityName, string commodityCategory, string sSalesvolume, string eSalesvolume, string sPrice, string ePrice, out int rowCount)
        {
            var query = Commodity.ObjectSet().Where(n => n.IsDel.Equals(false) && n.AppId.Equals(id) && (n.State == 1 || n.State == 2) && n.CommodityType == 0);
            #region 条件查询
            if (!string.IsNullOrEmpty(commodityName))
            {
                query = query.Where(n => n.Name.Contains(commodityName));
            }
            if (!string.IsNullOrEmpty(sSalesvolume))
            {
                int s = int.Parse(sSalesvolume);
                query = query.Where(n => n.Salesvolume >= s);
            }
            if (!string.IsNullOrEmpty(eSalesvolume))
            {
                int e = int.Parse(eSalesvolume);
                query = query.Where(n => n.Salesvolume <= e);
            }
            if (!string.IsNullOrEmpty(sPrice))
            {
                decimal s = 0;
                if (!decimal.TryParse(sPrice, out s))//长度越界
                {
                    rowCount = 0;
                    return new Commodity().ToEntityDataList(new List<Commodity>());
                }
                query = query.Where(n => n.Price >= s);
            }
            if (!string.IsNullOrEmpty(ePrice))
            {
                decimal e = 0;
                if (!decimal.TryParse(ePrice, out e))//长度越界
                {
                    rowCount = 0;
                    return new Commodity().ToEntityDataList(new List<Commodity>());
                }
                query = query.Where(n => n.Price <= e);
            }
            if (!string.IsNullOrEmpty(commodityCategory))
            {
                string[] commodityCategoryID = commodityCategory.Split(',');
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
            #endregion
            rowCount = query.Count();
            var list = query.OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Commodity().ToEntityDataList(list);
        }

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <returns></returns>
        public void SaveCommodity(CommodityDTO commodityDTO)
        {
            Commodity commodity = new Commodity().FromEntityData(commodityDTO);
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            commodity.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(commodity);
            contextSession.SaveChanges();
            RefreshCache(EntityState.Added);
        }
        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <param name="name">属性名称</param>
        /// <returns></returns>
        public void UpdateCommodity(System.Guid appId, CommodityDTO commodityDTO)
        {
            Commodity commodity = Commodity.ObjectSet().Where(n => n.Id == commodityDTO.Id && n.AppId == appId && n.CommodityType == 0).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            commodity.EntityState = System.Data.EntityState.Modified;
            commodity.Price = commodityDTO.Price;
            commodity.Stock = commodityDTO.Stock;
            commodity.Name = commodityDTO.Name;
            commodity.PicturesPath = commodityDTO.PicturesPath;
            commodity.Description = commodityDTO.Description;
            commodity.IsDel = false;
            commodity.AppId = commodityDTO.AppId;
            commodity.No_Code = commodityDTO.No_Code;
            contextSession.SaveObject(commodity);
            contextSession.SaveChanges();
            RefreshCache(EntityState.Modified);

        }
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="reviewId">评价ID</param>
        /// <returns></returns>
        public void DeleteCommodity(System.Guid appId, System.Guid commodityId)
        {
            Commodity commodity = Commodity.ObjectSet().Where(n => n.Id == commodityId && n.AppId == appId && n.CommodityType == 0).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            commodity.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(commodity);
            contextSession.SaveChanges();
            RefreshCache(EntityState.Deleted);
        }

        public Guid GetCommodityidByCode(Guid appid, string code)
        {
            Guid id = Commodity.ObjectSet().Where(n => n.No_Code == code && n.AppId == appid && n.CommodityType == 0).Select(n => n.Id).FirstOrDefault();
            return id;
        }

        public string GetCommodityNameByCode(string code)
        {
            string name = Commodity.ObjectSet().Where(n => n.No_Code == code && n.CommodityType == 0).Select(n => n.Name).FirstOrDefault();
            return name;
        }

        public List<CommodityDTO> GetCommodityList(Guid promotionID)
        {
            var comIds = from b in PromotionItems.ObjectSet() where promotionID.Equals(promotionID) select b.CommodityId;
            var comList = from a in Commodity.ObjectSet() where (comIds.Contains(a.Id) && a.CommodityType == 0) select a;
            return new Commodity().ToEntityDataList(comList.ToList());
        }
        /// <summary>
        /// 校验商品是否拥有多个(组)属性值
        /// </summary>
        /// <param name="comAttribute"></param>
        /// <returns></returns>
        public static bool CheckComMultAttribute(string comAttribute)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(comAttribute))
            {
                var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(comAttribute);
                if (attrs.Count > 1)
                {
                    var attrDict = attrs.GroupBy(c => c.Attribute).ToDictionary(x => x.Key, y => y.Count());
                    if (attrDict.Count > 1 || attrDict.Values.Any(c => c > 1))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 校验商品是否拥有多个(组)属性值 并返回单属性值
        /// </summary>
        /// <returns></returns>
        public static bool CheckComMultAttribute(string comAttribute, out string singleAttr)
        {
            singleAttr = string.Empty;
            bool result = CheckComMultAttribute(comAttribute);
            if (result)
            {
                var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(comAttribute);
                if (attrs.Any())
                {
                    singleAttr = attrs.First().SecondAttribute;
                }
            }
            return result;
        }
        /// <summary>
        /// 校验商品是否拥有多属性
        /// </summary>
        /// <param name="comAttribute"></param>
        /// <returns></returns>
        public static bool CheckComMultAttrs(string comAttribute)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(comAttribute))
            {
                var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(comAttribute);
                if (attrs.Count > 1)
                {
                    var attrDict = attrs.GroupBy(c => c.Attribute).ToDictionary(x => x.Key, y => y.Count());
                    if (attrDict.Count > 1)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获取商品多属性库存
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public static List<Deploy.CommodityStockDTO> GetComStocks(List<ComScoreCheckDTO> search)
        {
            List<Deploy.CommodityStockDTO> result = new List<CommodityStockDTO>();

            if (search == null || !search.Any())
                return result;
            Dictionary<Guid, Tuple<string, string>> dict = new Dictionary<Guid, Tuple<string, string>>();
            List<Guid> comIds = new List<Guid>();
            foreach (var comScoreCheckDTO in search)
            {
                if (string.IsNullOrEmpty(comScoreCheckDTO.ColorAndSize))
                    continue;
                comScoreCheckDTO.ColorAndSize = comScoreCheckDTO.ColorAndSize.Replace("null", "").Replace("nil", "").Replace("undefined", "").Replace("(null)", "").Replace("，", ",");
                var arr = comScoreCheckDTO.ColorAndSize.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 2)
                {
                    comIds.Add(comScoreCheckDTO.CommodityId);
                    if (!dict.ContainsKey(comScoreCheckDTO.ItemId))
                        dict.Add(comScoreCheckDTO.ItemId, new Tuple<string, string>(arr[0], arr[1]));
                }
                else if (arr.Length == 1)
                {
                    comIds.Add(comScoreCheckDTO.CommodityId);
                    if (!dict.ContainsKey(comScoreCheckDTO.ItemId))
                        dict.Add(comScoreCheckDTO.ItemId, new Tuple<string, string>(arr[0], null));
                }
            }
            comIds = comIds.Distinct().ToList();
            List<CommodityStockDTO> temp = CommodityStock.ObjectSet().Where(c => comIds.Contains(c.CommodityId)).Select(m => new CommodityStockDTO
            {
                Id = m.Id,
                Price = m.Price,
                Stock = m.Stock,
                CommodityId = m.CommodityId,
                ComAttribute = m.ComAttribute,
                Duty = m.Duty
            }).ToList();
            foreach (var comScoreCheckDTO in search)
            {
                if (comIds.All(c => c != comScoreCheckDTO.CommodityId))
                {
                    comScoreCheckDTO.CommodityStockId = Guid.Empty;
                    continue;
                }
                var comStockDto = GetComStock(temp, comScoreCheckDTO.CommodityId, dict[comScoreCheckDTO.ItemId].Item1, dict[comScoreCheckDTO.ItemId].Item2);
                if (comStockDto != null)
                {
                    comScoreCheckDTO.CommodityStockId = comStockDto.Id;
                    result.Add(comStockDto);
                }
            }
            return result;


        }
        private static CommodityStockDTO GetComStock(IEnumerable<CommodityStockDTO> stocks, Guid comId, string attr1, string attr2)
        {
            CommodityStockDTO result = null;
            foreach (var commodityStockDTO in stocks.Where(c => c.CommodityId == comId))
            {
                var comAttrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStockDTO.ComAttribute);
                if (comAttrs != null && comAttrs.Count == 2 && comAttrs.Any(c => c.SecondAttribute == attr1) && comAttrs.Any(c => c.SecondAttribute == attr2))
                {
                    result = commodityStockDTO;
                }
                else if (comAttrs != null && comAttrs.Count == 1 && comAttrs.Any(c => c.SecondAttribute == attr1))
                {
                    result = commodityStockDTO;
                }
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }

        #region Cache
        private void SyncComToZph(EntityState state)
        {
            const string synUrl = "{0}zph.iuoooo.com/Jinher.AMP.ZPH.SV.SynBTPCommdityDataSV.svc/SynBTPCommdityDada";
            //商品改边发布消息
            SyncComData requestData = new SyncComData()
                {
                    comdty = this.ToEntityData(),
                    entityState = this.IsDel ? EntityState.Deleted : state
                };
            string requestUrl = string.Format(synUrl, CustomConfig.UrlPrefix);
            string requestDataStr = JsonHelper.JsonSerializer(requestData);
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    int errorCnt = 0;
                    bool hasError = false;
                    while (errorCnt < 3)
                    {
                        var result = BaseRequest.CreateRequest<SyncComReturnInfo>(requestUrl, requestDataStr, false);
                        if (result == null || result.Code != 0)
                        {
                            errorCnt += 1;
                            hasError = true;
                            continue;
                        }
                        hasError = false;
                        break;
                    }
                    if (hasError)
                    {
                        LogHelper.Error(string.Format("商品详情同步商品数据异常.Commodity.SyncComToZph,requestUrl:{0},requestDataStr:{1}", requestUrl, requestDataStr));
                    }
                });
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        public void RefreshCache(EntityState state)
        {
            SyncComToZph(state);
            switch (state)
            {
                case EntityState.Added:
                    ResfreshCacheAdd();
                    break;
                case EntityState.Deleted:
                    RefreshCacheRemove();
                    break;
                case EntityState.Modified:
                    RefreshCacheUpdate();
                    break;
            }
        }
        /// <summary>
        /// 向商品详情缓存中增加Cache
        /// </summary>
        private void ResfreshCacheAdd()
        {
            if (!IsDel && State < 2)
            {
                //商品详情
                //GlobalCacheWrapper.Add("G_Commodity:" + this.AppId, this.Id.ToString(), this.ToEntityData(), CacheTypeEnum.redisSS, "BTPCache");

                GlobalCacheWrapper.Add("G_CommodityDetail", Id.ToString(), this.ToEntityData(), CacheTypeEnum.redisSS, "BTPCache");

                //商品列表
                GlobalCacheWrapper.RemoveCache("G_CommodityList:" + this.AppId, "BTPCache", CacheTypeEnum.redisSS);

            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        private void RefreshCacheRemove()
        {
            //商品详情
            //GlobalCacheWrapper.Remove("G_Commodity:" + this.AppId, this.Id.ToString(), CacheTypeEnum.redisSS, "BTPCache");
            GlobalCacheWrapper.Remove("G_CommodityDetail", Id.ToString(), CacheTypeEnum.redisSS, "BTPCache");

            //商品列表
            GlobalCacheWrapper.RemoveCache("G_CommodityList:" + this.AppId, "BTPCache", CacheTypeEnum.redisSS);
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        private void RefreshCacheUpdate()
        {
            if (!IsDel && State < 2)
            {
                //商品详情
                //GlobalCacheWrapper.Add("G_Commodity:" + AppId, Id.ToString(), ToEntityData(), CacheTypeEnum.redisSS, "BTPCache");
                GlobalCacheWrapper.Add("G_CommodityDetail", Id.ToString(), ToEntityData(), CacheTypeEnum.redisSS, "BTPCache");
                //商品列表
                var commodityCache = GlobalCacheWrapper.GetDataCache("G_CommodityList:" + AppId, "BTPCache", CacheTypeEnum.redisSS) as List<CommodityDTO>;

                //商品列表
                GlobalCacheWrapper.RemoveCache("G_CommodityList:" + this.AppId, "BTPCache", CacheTypeEnum.redisSS);
                return;
            }
            RefreshCacheRemove();

        }

        #region 商品详情
        /// <summary>
        /// 从缓存中取得商品信息实体
        /// </summary>
        /// <param name="appId">应用Id(此参数无用，以商品Id为准)</param>
        /// <param name="id">商品Id</param>
        /// <returns></returns>
        public static CommodityDTO GetDTOFromCache(Guid appId, Guid id)
        {
            //return GlobalCacheWrapper.GetData("G_Commodity:" + appId, id.ToString(), CacheTypeEnum.redisSS, "BTPCache") as CommodityDTO;
            return GlobalCacheWrapper.GetData("G_CommodityDetail", id.ToString(), CacheTypeEnum.redisSS, "BTPCache") as CommodityDTO;

        }
        /// <summary>
        /// 增加商品缓存
        /// </summary>
        /// <param name="item"></param>
        public static void AddAppCommondityCache(Commodity item)
        {
            if (item != null)
                AddAppCommondityDTOCache(item.ToEntityData());
        }
        /// <summary>
        /// 增加商品缓存
        /// </summary>
        /// <param name="item"></param>
        public static void AddAppCommondityDTOCache(CommodityDTO item)
        {
            if (item != null && !item.IsDel && item.State < 2)
            {
                //   GlobalCacheWrapper.Add("G_Commodity:" + item.AppId, item.Id.ToString(), item, CacheTypeEnum.redisSS, "BTPCache");
                GlobalCacheWrapper.Add("G_CommodityDetail", item.Id.ToString(), item, CacheTypeEnum.redisSS, "BTPCache");
            }
        }
        #endregion

        #region 商品列表
        /// <summary>
        /// 根据应用Id获取所有缓存商品实体
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <returns></returns>
        public static List<CommodityDTO> GetListDTOByAppIdFromCache(Guid appId)
        {
            return GlobalCacheWrapper.GetDataCache("G_CommodityList:" + appId, "BTPCache", CacheTypeEnum.redisSS) as List<CommodityDTO>;
        }
        /// <summary>
        /// 商品列表增加缓存（目前只加载app默认列表缓存，缓存当天有效）
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="list">商品列表</param>
        /// <param name="pageIndex">页码(未启用)</param>
        /// <param name="pageSize">每页数量(未启用)</param>
        public static void AddAppListCache(Guid appId, List<Commodity> list, int pageIndex, int pageSize)
        {
            if (pageIndex == 1 && pageSize == 10 && list != null && list.Any())
            {
                List<CommodityDTO> newList = list.Select(c => c.ToEntityData()).ToList();
                GlobalCacheWrapper.AddCache("G_CommodityList:" + appId, newList, DateTime.Today.AddDays(1).AddMilliseconds(-1), "BTPCache", CacheTypeEnum.redisSS);
            }
        }
        #endregion

        #endregion

        public static string RepairAttrs(string colorAndSize)
        {
            if (colorAndSize.IsNullVauleFromWeb())
                return ",";
            return colorAndSize.RemoveNullStr().Replace("，", ",");
        }

        /// <summary>
        /// 获得商品展示价格
        /// </summary>
        /// <param name="price">商品原价(Commodity.Price)</param>
        /// <param name="discountPrice">商品活动价，默认为-1(PromotionItems.DisCountPrice|TodayPromotion.DisCountPrice)</param>
        /// <param name="intensity">商品活动折扣，默认为10(PromotionItems.Intensity|TodayPromotion.Intensity)</param>
        /// <returns></returns>
        public static decimal GetShowRealPrice(decimal price, decimal discountPrice = -1, decimal intensity = 10)
        {
            return discountPrice > 0 ? discountPrice : Math.Round(price * intensity / 10.0m, 2);


        }
        /// <summary>
        /// 获得商品展示原价（中划线价格）
        /// </summary>
        /// <param name="price">商品原价(Commodity.Price)</param>
        /// <param name="markePrice">商品市场价(Commodity.MarketPrice)</param>
        /// <param name="discountPrice">商品活动价，默认为-1(PromotionItems.DisCountPrice|TodayPromotion.DisCountPrice)</param>
        /// <param name="intensity">商品活动折扣，默认为10(PromotionItems.Intensity|TodayPromotion.Intensity)</param>
        /// <returns></returns>
        public static decimal? GetShowOriPrice(decimal price, decimal? markePrice, decimal discountPrice = -1, decimal intensity = 10)
        {
            if (discountPrice > 0 || intensity < 10)
                return price;
            return markePrice;
        }
    }
    [DataContract]
    public class SyncComData
    {
        /// <summary>
        /// 商品
        /// </summary>
        [DataMember]
        public Deploy.CommodityDTO comdty { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public System.Data.EntityState entityState { get; set; }
    }
    // 摘要:
    //     接口返回值类型。
    [Serializable]
    [DataContract]
    public class SyncComReturnInfo
    {

        // 摘要:
        //     执行结果代码。
        [DataMember]
        public virtual int Code { get; set; }
        //
        // 摘要:
        //     是否成功
        [DataMember]
        public virtual bool isSuccess { get; set; }
        //
        // 摘要:
        //     返回消息。
        [DataMember]
        public virtual string Message { get; set; }
    }
}



