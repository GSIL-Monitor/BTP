using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using CommodityStockDTO = Jinher.AMP.BTP.Deploy.CommodityStockDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 购物车接口类
    /// </summary>
    public partial class ShoppingCartSV : BaseSv, IShoppingCart
    {

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="shoppingCartItemsSDTO">购物车商品实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveShoppingCartExt
            (Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemsSDTO, System.Guid userId, System.Guid appId)
        {

            var re = DoSaveShoppingCart(shoppingCartItemsSDTO, userId, appId);
            return new ResultDTO() { ResultCode = re.ResultCode, Message = re.Message };
        }

        private static ResultDTO<Guid> DoSaveShoppingCart(Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemsSDTO, System.Guid userId, System.Guid appId)
        {
            var shoppingCartId = Guid.Empty;
            try
            {
                Guid commodityStockId = Guid.Empty;
                if (shoppingCartItemsSDTO == null || userId == Guid.Empty || shoppingCartItemsSDTO.Id == Guid.Empty)
                    return new ResultDTO<Guid> { ResultCode = 1, Message = "Error" };

                //根据条件查询购物车中此商品是否已经存在
                ShoppingCartItems oldsc = null;

                //兼容
                string comAttributeIdsFirst = ",";
                string comAttributeIdsSecond = ",";
                if (!string.IsNullOrEmpty(shoppingCartItemsSDTO.SizeAndColorId))
                {
                    shoppingCartItemsSDTO.SizeAndColorId = shoppingCartItemsSDTO.SizeAndColorId.Replace("null", "").Replace("nil", "").Replace("(null)", "").Replace("undefined", "");
                    var arr = shoppingCartItemsSDTO.SizeAndColorId.Split(',');
                    if (arr.Length == 1)
                    {
                        comAttributeIdsFirst = shoppingCartItemsSDTO.SizeAndColorId + ",";
                        comAttributeIdsSecond = "," + shoppingCartItemsSDTO.SizeAndColorId;
                    }
                    else
                    {
                        comAttributeIdsFirst = arr[0] + "," + arr[1];
                        comAttributeIdsSecond = arr[1] + "," + arr[0];
                        var comAttr = CommodityStock.ObjectSet().Where(c => c.CommodityId == shoppingCartItemsSDTO.Id).ToList();
                        string s = "";
                        for (int i = 0; i < comAttr.Count; i++)
                        {
                            s = comAttr[i].ComAttribute;
                            var attrs = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(s);
                            if (attrs[0].SecondAttribute == arr[0] && attrs[1].SecondAttribute == arr[1])
                            {
                                commodityStockId = comAttr[i].Id;
                            }
                        }
                    }
                }
                if (comAttributeIdsFirst == ",")
                {
                    var com = Commodity.ObjectSet().Where(c => c.Id == shoppingCartItemsSDTO.Id && c.CommodityType == 0).Select(m => new Deploy.CommodityDTO { Id = m.Id, ComAttribute = m.ComAttribute }).FirstOrDefault();
                    if (com == null)
                        return new ResultDTO<Guid> { ResultCode = 1, Message = "Error" };
                    string singleAttr;
                    if (!Commodity.CheckComMultAttribute(com.ComAttribute, out singleAttr))
                    {
                        comAttributeIdsFirst = singleAttr + ",";
                        comAttributeIdsSecond = "," + singleAttr;
                    }
                }

                Guid esAppId = appId;
                bool b = BTP.TPS.ZPHSV.Instance.CheckIsAppInZPH(appId);
                if (b)
                {
                    esAppId = Common.CustomConfig.ZPHAppId;
                }

                oldsc = (from n in ShoppingCartItems.ObjectSet()
                         where
                         n.CommodityId == shoppingCartItemsSDTO.Id
                          && n.UserId == userId
                          && n.EsAppId == esAppId
                          && (n.ComAttributeIds == comAttributeIdsFirst || n.ComAttributeIds == comAttributeIdsSecond)
                         select n
                             ).FirstOrDefault();

                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //购物车里存在产品属性相同的，对应数量增加
                if (oldsc != null)
                {
                    oldsc.CommodityNumber += shoppingCartItemsSDTO.CommodityNumber;
                    oldsc.EntityState = System.Data.EntityState.Modified;
                    oldsc.ModifiedOn = DateTime.Now;
                }
                else
                {
                    oldsc = ShoppingCartItems.CreateShoppingCartItems();
                    oldsc.AppId = appId;
                    oldsc.Name = "购物车信息";
                    oldsc.UserId = userId;
                    oldsc.SubId = userId;
                    oldsc.SubTime = DateTime.Now;
                    oldsc.CommodityId = shoppingCartItemsSDTO.Id;
                    oldsc.CommodityNumber = shoppingCartItemsSDTO.CommodityNumber;
                    oldsc.ComAttributeIds = comAttributeIdsFirst;
                    oldsc.EsAppId = esAppId;
                    oldsc.CommodityStockId = commodityStockId;
                    contextSession.SaveObject(oldsc);
                }
                contextSession.SaveChanges();
                shoppingCartId = oldsc.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加购物车服务异常。shoppingCartItemsSDTO：{0}，userId：{1}，appId：{2}，", JsonHelper.JsonSerializer(shoppingCartItemsSDTO), userId, appId), ex);
                return new ResultDTO<Guid> { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO<Guid> { ResultCode = 0, Message = "Success", Data = shoppingCartId };
        }

        /// <summary>
        /// 厂家直营获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCarCommodityListDTO> GetShoppongCartListExt
            (System.Guid userId, System.Guid appId)
        {

            List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCarCommodityListDTO> ShopCarCommodityLists = new List<ShopCarCommodityListDTO>();

            ShopCarCommodityListDTO ShopCarCommodityList = new ShopCarCommodityListDTO();

            IQueryable<CommoditySDTO> query = null;
            if (appId == Guid.Empty)
            {
                query = from data in ShoppingCartItems.ObjectSet()
                        join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                        where data.UserId == userId && data1.CommodityType == 0
                        select new CommoditySDTO
                        {
                            Id = data1.Id,
                            Pic = data1.PicturesPath,
                            Name = data1.Name,
                            Price = data1.Price,
                            CommodityNumber = data.CommodityNumber,
                            Size = data.ComAttributeIds,
                            ShopCartItemId = data.Id,
                            State = data1.State,
                            Stock = data1.Stock,
                            AppId = data1.AppId,
                            IsEnableSelfTake = data1.IsEnableSelfTake,
                            CommodityType = data1.CommodityType
                        };
            }
            else
            {
                query = from data in ShoppingCartItems.ObjectSet()
                        join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                        where data.UserId == userId && data.AppId == appId && data1.CommodityType == 0
                        select new CommoditySDTO
                        {
                            Id = data1.Id,
                            Pic = data1.PicturesPath,
                            Name = data1.Name,
                            Price = data1.Price,
                            CommodityNumber = data.CommodityNumber,
                            Size = data.ComAttributeIds,
                            ShopCartItemId = data.Id,
                            State = data1.State,
                            Stock = data1.Stock,
                            AppId = data1.AppId,
                            IsEnableSelfTake = data1.IsEnableSelfTake,
                            CommodityType = data1.CommodityType
                        };
            }

            List<CommoditySDTO> commoditySDTOList = query.ToList<CommoditySDTO>();
            //读今日折扣表
            try
            {
                List<Guid> commodityIds = commoditySDTOList.Select(c => c.Id).ToList();
                DateTime now = DateTime.Now;
                var promotionDic = TodayPromotion.GetCurrentPromotions(commodityIds);

                //循环取会员价
                var appIds = commoditySDTOList.Select(c => c.AppId).Distinct().ToList();
                var vipDict = AVMSV.GetVipIntensities(appIds, userId);

                foreach (var commodity in commoditySDTOList)
                {
                    bool isdi = false;
                    foreach (var com in promotionDic)
                    {
                        if (com.CommodityId == commodity.Id)
                        {
                            commodity.LimitBuyEach = com.LimitBuyEach == null ? -1 : com.LimitBuyEach;
                            commodity.LimitBuyTotal = com.LimitBuyTotal == null ? -1 : com.LimitBuyTotal;
                            commodity.SurplusLimitBuyTotal = com.SurplusLimitBuyTotal == null ? 0 : com.SurplusLimitBuyTotal;
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

                    //会员价
                    VipPromotionDTO privilegeInfo = null;
                    if (vipDict.ContainsKey(commodity.AppId))
                        privilegeInfo = vipDict[commodity.AppId];

                    if (privilegeInfo != null && privilegeInfo.Intensity > 0)
                    {
                        var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2,
                                                         MidpointRounding.AwayFromZero);
                        //有活动
                        if (isdi)
                        {
                            //
                        }
                        else
                        {
                            if (privilegePrice < commodity.Price)
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = privilegeInfo.Intensity;
                            }
                        }
                    }

                    var appList = commoditySDTOList.GroupBy(q => q.AppId);

                    foreach (var item in appList)
                    {
                        Dictionary<Guid, string> lists = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(new List<Guid> { item.Key });
                        if (lists.Any() && lists.ContainsKey(item.Key))
                        {
                            ShopCarCommodityList.AppName = lists[item.Key];
                        }
                        ShopCarCommodityList.CommoditySDTOList = item.ToList();
                    }
                    ShopCarCommodityLists.Add(ShopCarCommodityList);
                }
            }
            catch (Exception e)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("商品列表查询错误userId：{0}，appId：{1}，", userId, appId), e);
            }
            return ShopCarCommodityLists;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetShoppongCartItemsExt
            (System.Guid userId, System.Guid appId)
        {
            return GetShoppongCartItemsNewExt(userId, appId);
        }


        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId:电商馆Id</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetShoppongCartItemsNewExt
            (System.Guid userId, System.Guid appId)
        {
            List<CommoditySDTO> result = new List<CommoditySDTO>();
            List<ShoppingCartItems> shoppingCartItems;

            if (appId == Guid.Empty)
            {
                shoppingCartItems = (from data in ShoppingCartItems.ObjectSet()
                                     where data.UserId == userId
                                     orderby data.SubTime descending
                                     select data).ToList();
            }
            else
            {
                shoppingCartItems = (from data in ShoppingCartItems.ObjectSet()
                                     where data.UserId == userId && data.EsAppId == appId
                                     orderby data.SubTime descending
                                     select data).ToList();
            }
            if (!shoppingCartItems.Any())
                return new List<CommoditySDTO>();
            var comIds = shoppingCartItems.Select(c => c.CommodityId).Distinct().ToList();

            var commodityList = Commodity.ObjectSet()
                         .Where(c => comIds.Contains(c.Id))
                         .Select(c => new CommodityDTO
                         {
                             Id = c.Id,
                             State = c.State,
                             Stock = c.Stock,
                             CommodityType = c.CommodityType,
                             Name = c.Name,
                             AppId = c.AppId,
                             IsEnableSelfTake = c.IsEnableSelfTake,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             IsDel = c.IsDel,
                             ComAttribute = c.ComAttribute,
                             Type = c.Type
                         }).ToList();

            List<Deploy.CommodityStockDTO> commodityStockList = new List<CommodityStockDTO>();
            var comStockIds = shoppingCartItems.Where(c => c.CommodityStockId != Guid.Empty).Select(c => c.CommodityStockId).Distinct().ToList();
            if (comStockIds.Any())
            {
                commodityStockList = CommodityStock.ObjectSet().Where(c => comStockIds.Contains(c.Id)).Select(c => new Deploy.CommodityStockDTO() { Id = c.Id, ComAttribute = c.ComAttribute, Price = c.Price, Stock = c.Stock }).ToList();
            }

            foreach (var shoppingCartItem in shoppingCartItems)
            {

                ShopCartStateEnum shopCartState = ShopCartStateEnum.OK;
                var commodity = commodityList.FirstOrDefault(c => c.Id == shoppingCartItem.CommodityId);
                if (commodity == null)
                    continue;

                int stock = 0;
                decimal price = commodity.Price;

                var commodityStock =
                    commodityStockList.FirstOrDefault(c => c.Id == shoppingCartItem.CommodityStockId);

                #region 购物车失效判断

                #region 是否删除判断

                if (commodity.IsDel)
                {
                    shopCartState = ShopCartStateEnum.Del;
                }

                #endregion

                #region 上架状态判断

                if (commodity.State == 1)
                {
                    shopCartState = ShopCartStateEnum.OffSale;
                }

                #endregion

                #region 属性判断

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
                        if (!Commodity.CheckComMultAttrs(commodity.ComAttribute) || commodityStock == null ||
                            comAttrs == null || !comAttrs.Any())
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
                        stock = commodity.Stock;

                    }
                    else
                    {
                        if (commodityStock == null || commodityStock.Stock <= 0)
                            shopCartState = ShopCartStateEnum.Stock;
                        else
                        {
                            stock = commodityStock.Stock;
                            price = commodityStock.Price;
                        }

                    }
                }


                #endregion

                #endregion

                #region 包装规格设置

                List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
                model.Id = Guid.Empty;
                model.Name = "规格设置";
                model.Attribute = shoppingCartItem.Specifications ?? 0;
                model.strAttribute = "1*" + shoppingCartItem.Specifications + "";
                Specificationslist.Add(model);

                #endregion

                result.Add(new CommoditySDTO()
                {
                    Id = commodity.Id,
                    Pic = commodity.PicturesPath,
                    Name = commodity.Name,
                    Price = price,
                    CommodityNumber = shoppingCartItem.CommodityNumber,
                    Size = shoppingCartItem.ComAttributeIds,
                    ShopCartItemId = shoppingCartItem.Id,
                    State = commodity.State,
                    Stock = stock,
                    AppId = commodity.AppId,
                    AppName = "",
                    IsEnableSelfTake = commodity.IsEnableSelfTake,
                    CommodityType = commodity.CommodityType,
                    CommodityStockId = shoppingCartItem.CommodityStockId,
                    ShopCartState = shopCartState,
                    Type = commodity.Type ?? 0,
                    JcActivityId = shoppingCartItem.JcActivityId,
                    Specifications = Specificationslist
                });

            }
            #region 优惠
            var promotionDic = TodayPromotion.GetCurrentPromotions(comIds);
            //循环取会员价
            var appIds = result.Select(c => c.AppId).Distinct().ToList();
            var vipDict = AVMSV.GetVipIntensities(appIds, userId);

            foreach (var commodity in result)
            {
                var promotion = promotionDic.FirstOrDefault(c => c.CommodityId == commodity.Id);
                if (promotion != null)
                {
                    commodity.LimitBuyEach = promotion.LimitBuyEach ?? -1;
                    commodity.LimitBuyTotal = promotion.LimitBuyTotal ?? -1;
                    commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal ?? 0;
                    if (promotion.DiscountPrice > -1)
                    {
                        commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                    }
                    else
                    {
                        commodity.Intensity = promotion.Intensity;
                    }
                    var skuActivity = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                    if (skuActivity != null)
                    {
                        commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice.ToString("0.00"));
                        commodity.IsJoin = skuActivity.IsJoin;
                    }
                }
                else
                {
                    //会员价
                    VipPromotionDTO privilegeInfo = null;
                    if (vipDict.ContainsKey(commodity.AppId))
                        privilegeInfo = vipDict[commodity.AppId];

                    if (privilegeInfo != null && (privilegeInfo.DiscountPrice > -1 || privilegeInfo.Intensity < 10))
                    {
                        var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2, MidpointRounding.AwayFromZero);

                        if (privilegePrice < commodity.Price)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = privilegeInfo.Intensity;
                        }

                    }
                }
                #region 应用名称
                if (commodity.JcActivityId != null && commodity.JcActivityId != Guid.Empty)
                {
                    LogHelper.Debug(string.Format("进入ZPHSV.Instance.GetItemsListByActivityId方法，commodity.JcActivityId：{0}", commodity.JcActivityId));
                    var itemsList = ZPHSV.Instance.GetItemsListByActivityId((Guid)commodity.JcActivityId).Data.Where(t => t.ComdtyId == commodity.Id && t.ComdtyStockId == commodity.CommodityStockId).ToList();
                    if (itemsList.Any())
                    {
                        commodity.DiscountPrice = Convert.ToDecimal(itemsList.FirstOrDefault().GroupPrice.ToString("0.00"));
                        commodity.JcActivityName = itemsList.FirstOrDefault().JCActivityName;
                    }
                }

                #endregion
            }
            #endregion

            #region 赠品信息
            var now = DateTime.Now;
            foreach (var commodity in result)
            {
                // && _.CommoditySKUId == commodity.CommodityStockId
                var presents = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == commodity.Id)
                            .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                                pp => pp.PresentPromotionId, ppc => ppc.Id,
                                (c, p) => new { Commodity = c, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime })
                            .ToList();

                if (presents.Count > 0)
                {
                    var present = presents.First();
                    commodity.Present = new CommodiyPresentDTO();
                    commodity.Present.Limit = present.Limit ?? 1;
                    if (commodity.Present.Limit == 0)
                    {
                        commodity.Present.Limit = 1;
                    }
                    commodity.Present.BeginTime = present.BeginTime;
                    commodity.Present.EndTime = present.EndTime;
                    commodity.Present.CommodityStockIds = presents.Where(_ => _.Commodity.CommoditySKUId != Guid.Empty).Select(_ => _.Commodity.CommoditySKUId).ToList();
                    commodity.Present.IsAll = true;
                    commodity.Present.Title = "购买即送超值赠品 （赠完即止）";
                    if (commodity.CommodityStocks != null && commodity.CommodityStocks.Count > 0)
                    {
                        bool isAll = true;
                        var titles = new List<string>();
                        foreach (var item in commodity.CommodityStocks)
                        {
                            if (commodity.Present.CommodityStockIds.Any(_ => _ == item.Id))
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
                            commodity.Present.IsAll = false;
                            commodity.Present.Title = "购买 “" + string.Join("”、 “", titles) + "” 送超值赠品 （赠完即止）";
                        }
                    }

                    var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == present.PromotionId).ToList();
                    var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                    var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                    commodity.Present.Items = new List<CommodiyPresentItem>();
                    foreach (var item in gifts)
                    {
                        var tempCom = GetCommodity(commodity.AppId, item.CommodityId);
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
                                commodity.Present.Items.Add(commodiyPresentItem);
                            }
                        }
                        else
                        {
                            if (tempCom.Stock > 0)
                            {
                                commodiyPresentItem.Stock = tempCom.Stock;
                                commodity.Present.Items.Add(commodiyPresentItem);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 应用名称
            Dictionary<Guid, string> listApps = Jinher.AMP.BTP.TPS.APPSV.GetAppNameListByIds(appIds);
            if (!listApps.Any())
            {
                return result;
            }
            foreach (var item in result)
            {
                if (!listApps.ContainsKey(item.AppId))
                {
                    continue;
                }
                var listAppName = listApps[item.AppId];
                if (!String.IsNullOrEmpty(listAppName))
                {
                    item.AppName = listAppName;
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 编辑购物车
        /// </summary>
        /// <param name="shopCartCommodityUpdateDTOs">购物车编辑实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateShoppingCartExt
            (System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs,
            System.Guid userId, System.Guid appId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //获取购物车id列表
                List<Guid> shopCartIdList = new List<Guid>();
                shopCartCommodityUpdateDTOs.ForEach(
                    a =>
                    {
                        shopCartIdList.Add(a.ShopCartItemId);
                    });

                //从数据库获取购物车节点信息
                List<ShoppingCartItems> shoppingCartItemList = null;
                if (appId == Guid.Empty)
                {
                    shoppingCartItemList = ShoppingCartItems.ObjectSet()
                    .Where(s => s.UserId == userId && shopCartIdList.Contains(s.Id))
                    .ToList();
                }
                else
                {
                    shoppingCartItemList = ShoppingCartItems.ObjectSet()
                    .Where(s => s.UserId == userId && s.EsAppId == appId && shopCartIdList.Contains(s.Id))
                    .ToList();
                }

                //循环更新购物车节点信息
                shoppingCartItemList.ForEach(
                    item =>
                    {
                        ShopCartCommodityUpdateDTO dto = shopCartCommodityUpdateDTOs.Where(a => a.ShopCartItemId == item.Id).FirstOrDefault();
                        if (dto != null)
                        {
                            item.CommodityNumber = dto.Number;
                        }
                        item.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(item);
                    });
                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑购物车服务异常。shopCartCommodityUpdateDTOs：{0}，userId：{1}，appId：{2}，", JsonHelper.JsonSerializer(shopCartCommodityUpdateDTOs), userId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shopCartItemId">购物车Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteShoppingCartExt
            (System.Guid shopCartItemId, System.Guid userId, System.Guid appId)
        {

            ResultDTO result = new ResultDTO();
            try
            {
                ShoppingCartItems sci = new ShoppingCartItems();
                var shop = ShoppingCartItems.ObjectSet().Where(n => n.Id == shopCartItemId).FirstOrDefault();
                //var shop = ShoppingCartItems.ObjectSet().Where(n => n.UserId == userId && n.AppId==appId).FirstOrDefault();
                if (shop != null)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    shop.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(shop);
                    contextSession.SaveChange();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除购物车商品服务异常。shopCartItemId：{0}，userId：{1}，appId：{2}", JsonHelper.JsonSerializer(shopCartItemId), userId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 购物车商品数量 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetShoppingCartNumExt(System.Guid userId, System.Guid appId)
        {
            if (appId == Guid.Empty)
            {
                var shoppingCartItems = ShoppingCartItems.ObjectSet().Where(n => n.UserId == userId).Select(c => c.CommodityNumber).ToList();
                //TODO yangjz 新版收藏用的SetCollection表
                var collection = (from data in Collection.ObjectSet()
                                  join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                                  where data.UserId == userId && data1.IsDel == false && data1.CommodityType == 0
                                  select data.Id).Count();
                return new NumResultSDTO
                {
                    CollectNum = collection,
                    ShopCartNum = shoppingCartItems.Sum()
                };
            }
            else
            {
                var shoppingCartItems = ShoppingCartItems.ObjectSet().Where(n => n.UserId == userId && n.EsAppId == appId).Select(c => c.CommodityNumber).ToList();
                var collection = (from data in Collection.ObjectSet()
                                  join data1 in Commodity.ObjectSet() on data.CommodityId equals data1.Id
                                  where data.UserId == userId && data.AppId == appId && data1.IsDel == false && data1.CommodityType == 0
                                  select data.Id).Count();
                return new NumResultSDTO
                {
                    CollectNum = collection,
                    ShopCartNum = shoppingCartItems.Sum()
                };
            }

        }
        /// <summary>
        /// 复制订单中的商品到购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CopyOrderToShoppingCartExt(Guid userId, Guid orderId, Guid appId)
        {
            try
            {
                var order = OrderItem.ObjectSet().Where(p => p.CommodityOrderId == orderId).ToList();

                if (order.Count > 0)
                {
                    foreach (var o in order)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemsSDTO = new ShoppingCartItemSDTO();
                        shoppingCartItemsSDTO.CommodityNumber = o.Number;
                        shoppingCartItemsSDTO.Id = o.CommodityId;
                        shoppingCartItemsSDTO.SizeAndColorId = o.CommodityAttributes;
                        shoppingCartItemsSDTO.UserId = userId;
                        SaveShoppingCartExt(shoppingCartItemsSDTO, userId, appId);
                    }
                    return new ResultDTO { ResultCode = 0, Message = "Success" };

                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "订单不存在" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("复制订单中的商品到购物车服务异常。userId：{0}，orderId：{1}，appId：{2}", userId, orderId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

        }


        /// <summary>
        /// 分享的订单复制订单中的商品到购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Guid>> CopyShareOrderToShoppingCartExt(Guid userId, Guid orderId, Guid appId)
        {
            try
            {
                var shoppingCartIds = new List<Guid>();
                var orderIds = new List<Guid>() { orderId };

                //判断订单是否是拆单
                var mainOrders = MainOrder.ObjectSet().Where(r => r.MainOrderId == orderId).ToList();
                if (mainOrders != null && mainOrders.Count > 0)
                {
                    orderIds = mainOrders.Select(r => r.SubOrderId).Distinct().ToList();
                }
                // var order = OrderItem.ObjectSet().Where(p =>orderIds.Contains(p.CommodityOrderId)).ToList();

                var orders = (from ot in OrderItem.ObjectSet()
                              join c in CommodityOrder.ObjectSet() on ot.CommodityOrderId equals c.Id
                              where orderIds.Contains(ot.CommodityOrderId)
                              select new { item = ot, order = c }).ToList();

                if (orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        var o = order.item;
                        var shoppingCartItemsSDTO = new SaveShoppingCartParamDTO();
                        shoppingCartItemsSDTO.CommodityNumber = 1;
                        shoppingCartItemsSDTO.AppId = order.order.AppId;
                        shoppingCartItemsSDTO.EsAppId = appId;
                        shoppingCartItemsSDTO.SizeAndColorId = o.CommodityAttributes;
                        shoppingCartItemsSDTO.UserId = userId;
                        shoppingCartItemsSDTO.CommodityId = o.CommodityId;
                        shoppingCartItemsSDTO.CommodityStockId = o.CommodityStockId;
                        var re = SaveShoppingCartNewExt(shoppingCartItemsSDTO);
                        if (re.ResultCode == 0) shoppingCartIds.Add(re.Data);
                    }

                    return new ResultDTO<List<Guid>> { ResultCode = 0, Message = "Success", Data = shoppingCartIds };
                }
                else
                {
                    return new ResultDTO<List<Guid>> { ResultCode = 1, Message = "订单不存在" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("复制订单中的商品到购物车服务异常。userId：{0}，orderId：{1}，appId：{2}", userId, orderId, appId), ex);
                return new ResultDTO<List<Guid>> { ResultCode = 1, Message = "Error" };
            }

        }

        /// <summary>
        /// 获取我的购物车 --- 厂家直销
        /// 这个方法不再使用，用GetShoppongCartItems方法代替，appId传Guid.Empty
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CommoditySDTO> GetMyShoppongCartExt(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加到购物车 --- 厂家直销
        /// 这个方法不再使用，用SaveShoppingCart方法代替
        /// </summary>
        /// <param name="shoppingCartItemSDTO"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultDTO SaveSetShoppingCartExt(ShoppingCartItemSDTO shoppingCartItemSDTO, Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改购物车 --- 厂家直销
        /// 这个方法不再使用，用UpdateShoppingCart方法代替，appId传Guid.Empty
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="shopCartCommodityUpdateDTOs"></param>
        /// <returns></returns>
        public ResultDTO UpdateSetShoppingCartExt(Guid userId, List<ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询购物车商品数量 --- 厂家直销
        /// 这个方法不再使用，用GetShoppingCartNum方法代替，appId传Guid.Empty
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public NumResultSDTO GetSetShoppingCartNumExt(Guid userId)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// 批量删除购物车中的商品
        /// </summary>
        /// <param name="shopCartItemId">购物车Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditysFromShoppingCartExt(List<Guid> shopCartItemIds, Guid userId, Guid appId)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                ShoppingCartItems sci = new ShoppingCartItems();
                var cItems = ShoppingCartItems.ObjectSet().Where(n => shopCartItemIds.Contains(n.Id));
                if (cItems != null)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    foreach (var ci in cItems)
                    {
                        ci.EntityState = System.Data.EntityState.Deleted;
                    }
                    int v = contextSession.SaveChanges();
                    LogHelper.Info("DeleteCommoditysFromShoppingCartExt, v:" + v + "    ,SaveChanges:" + JsonHelper.JsSerializer(shopCartItemIds));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除购物车商品服务异常。shopCartItemIds：{0}，userId：{1}，appId：{2}", JsonHelper.JsonSerializer(shopCartItemIds), userId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 添加购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/SaveShoppingCartNew
        /// </para>
        /// </summary> 
        ///<param name="sscDto">加入购物车的商品、数量等信息</param>
        /// <returns></returns>
        public ResultDTO<Guid> SaveShoppingCartNewExt(SaveShoppingCartParamDTO sscDto)
        {
            ResultDTO<Guid> result = new ResultDTO<Guid>() { ResultCode = 0, Message = "Success" };
            result.Data = new Guid();
            try
            {
                result = rebuildSaveDto(sscDto);
                if (result.ResultCode != 0)
                    return result;

                #region 校验商品
                CommoditySV commoditySv = new CommoditySV();
                var cpp = new CheckCommodityParam();
                cpp.CommodityIdsList = new List<CommodityIdAndStockId> { new CommodityIdAndStockId() { CommodityId = sscDto.CommodityId, CommodityStockId = sscDto.CommodityStockId } };
                cpp.PromotionType = -1;
                cpp.UserID = sscDto.UserId;
                var checkResult = commoditySv.CheckCommodityV3Ext(cpp);
                if (checkResult == null || !checkResult.Any())
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                var checkCommodityResult = checkResult.First();
                if (checkCommodityResult.State == 1)
                {
                    result.ResultCode = 1;
                    result.Message = "商品已下架，不能购买！";
                    return result;
                }
                if (checkCommodityResult.State == 3)
                {
                    result.ResultCode = 1;
                    result.Message = "商品已删除，不能购买！";
                    return result;

                }
                //if (checkCommodityResult.ComPromotionStatusEnum == ComPromotionStatusEnum.Presell || checkCommodityResult.ComPromotionStatusEnum == ComPromotionStatusEnum.Seckill)
                //{
                //    result.ResultCode = 1;
                //    result.Message = "此商品暂不支持加入购物车！";
                //    return result;
                //}
                if (checkCommodityResult.IsNeedPresell && !checkCommodityResult.IsPreselled)
                {
                    result.ResultCode = 1;
                    result.Message = "未预约，不能购买！";
                    return result;
                }
                if (checkCommodityResult.LimitBuyEach > 0 && sscDto.CommodityNumber > checkCommodityResult.LimitBuyEach)
                {

                    result.ResultCode = 1;
                    result.Message = string.Format("每人限购{0}件，超出范围！", checkCommodityResult.LimitBuyEach);
                    return result;
                }
                if (sscDto.CommodityNumber > checkCommodityResult.Stock || (checkCommodityResult.LimitBuyTotal > 0 && sscDto.CommodityNumber > (checkCommodityResult.LimitBuyTotal - checkCommodityResult.SurplusLimitBuyTotal)))
                {

                    result.ResultCode = 1;
                    result.Message = "数量不足或超出限购数量.不能购买！";
                    return result;
                }
                if(sscDto.CommodityNumber<1)
                {
                    result.ResultCode = 1;
                    result.Message = "商品数量不能小于一！";
                    return result;
                }
                #endregion

                var strArr = "";
                int index = sscDto.SizeAndColorId.IndexOf("null");
                if (index > -1)
                {
                    strArr = sscDto.SizeAndColorId.Replace("null", "");
                }
                else
                {
                    strArr = sscDto.SizeAndColorId;
                }
                //兼容
                var arr = strArr.Split(',');
                string comAttributeIdsFirst = strArr;
                string comAttributeIdsSecond = arr[1] + "," + arr[0];

                //根据条件查询购物车中此商品是否已经存在
                ShoppingCartItems oldsc = new ShoppingCartItems();
                var list = (from n in ShoppingCartItems.ObjectSet()
                            where
                                n.CommodityId == sscDto.CommodityId
                                && n.UserId == sscDto.UserId
                                && n.AppId == sscDto.AppId
                                && (n.ComAttributeIds == comAttributeIdsFirst || n.ComAttributeIds == comAttributeIdsSecond)
                                && n.EsAppId == sscDto.EsAppId
                            select n);
                if (sscDto.JcActivityId != null && sscDto.JcActivityId != Guid.Empty)
                {
                    oldsc = list.FirstOrDefault(t => t.JcActivityId == sscDto.JcActivityId);
                }
                else
                {
                    oldsc = list.FirstOrDefault();
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                #region 属性判断
                ShopCartStateEnum shopCartState = ShopCartStateEnum.OK;
                if (oldsc != null)
                {
                    var commodityStock = CommodityStock.ObjectSet().FirstOrDefault(c => c.Id == oldsc.CommodityStockId);
                    var commodity = Commodity.ObjectSet().FirstOrDefault(c => c.Id == oldsc.CommodityId);

                    var shopCartItemAttrs = oldsc.ComAttributeIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (commodity != null)
                    {
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
                                if (!Commodity.CheckComMultAttrs(commodity.ComAttribute) || commodityStock == null ||
                                    comAttrs == null || !comAttrs.Any())
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
                    }
                }
                #endregion

                //购物车里存在产品属性相同的，对应数量增加
                if (oldsc != null && shopCartState != ShopCartStateEnum.Attribute)
                {
                    oldsc.ModifiedOn = DateTime.Now;
                    if (oldsc.Specifications != 0 && oldsc.Specifications != sscDto.Specifications)
                    {
                        oldsc.CommodityNumber = sscDto.CommodityNumber;
                        oldsc.Specifications = sscDto.Specifications;
                    }
                    else
                    {
                        oldsc.CommodityNumber += sscDto.CommodityNumber;
                    }
                    oldsc.EntityState = System.Data.EntityState.Modified;
                    oldsc.ModifiedOn = DateTime.Now;
                }
                else
                {
                    oldsc = ShoppingCartItems.CreateShoppingCartItems();
                    oldsc.AppId = sscDto.AppId;
                    oldsc.Name = "购物车信息";
                    oldsc.UserId = sscDto.UserId;
                    oldsc.SubId = sscDto.UserId;
                    oldsc.CommodityId = sscDto.CommodityId;
                    oldsc.CommodityNumber = sscDto.CommodityNumber;
                    oldsc.ComAttributeIds = comAttributeIdsFirst;
                    oldsc.EsAppId = sscDto.EsAppId;
                    oldsc.CommodityStockId = sscDto.CommodityStockId == null ? Guid.Empty : sscDto.CommodityStockId.Value;
                    oldsc.JcActivityId = sscDto.JcActivityId;
                    //包装规格设置
                    oldsc.Specifications = sscDto.Specifications ?? 0;
                    oldsc.SubTime = DateTime.Now;
                    contextSession.SaveObject(oldsc);
                }
                oldsc.SubTime = DateTime.Now;
                contextSession.SaveChanges();

                result.Data = oldsc.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加购物车服务异常。参数：{0}", JsonHelper.JsonSerializer(sscDto)), ex);
                return new ResultDTO<Guid> { ResultCode = -1, Message = "添加购物车服务异常!" };
            }
            return result;
        }
        /// <summary>
        /// 兼容老版本，校验加入购物车商品是否有效
        /// </summary>
        /// <param name="sscDto"></param>
        /// <returns></returns>
        private ResultDTO<Guid> rebuildSaveDto(SaveShoppingCartParamDTO sscDto)
        {
            ResultDTO<Guid> result = new ResultDTO<Guid> { ResultCode = 0, Message = "Success" };
            if (sscDto == null
                    || sscDto.UserId == Guid.Empty
                    || sscDto.EsAppId == Guid.Empty)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空！";
                return result;
            }

            var com = Commodity.ObjectSet().Where(c => c.Id == sscDto.CommodityId).Select(m => new Deploy.CommodityDTO { Id = m.Id, AppId = m.AppId, ComAttribute = m.ComAttribute }).FirstOrDefault();
            if (com == null)
            {
                result.ResultCode = 2;
                result.Message = "未找到要加入购物车的商品不存在！";
                return result;
            }
            sscDto.AppId = com.AppId;
            //兼容
            if (string.IsNullOrEmpty(sscDto.SizeAndColorId))
                sscDto.SizeAndColorId = ",";
            sscDto.SizeAndColorId = sscDto.SizeAndColorId.Replace("null", "").Replace("nil", "").Replace("(null)", "").Replace("undefined", "");
            var arr = sscDto.SizeAndColorId.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            switch (arr.Length)
            {
                case 0:
                    sscDto.SizeAndColorId = ",";
                    //sscDto.CommodityStockId = sscDto.CommodityId;
                    break;

                case 1:
                    sscDto.SizeAndColorId = arr[0] + ",";
                    //20170608 单属性商品加入购物车
                    //兼容老版本，补全CommodityStockId
                    if (!sscDto.CommodityStockId.HasValue || sscDto.CommodityStockId == Guid.Empty)
                    {
                        var comStocks = CommodityStock.ObjectSet().Where(c => c.CommodityId == sscDto.CommodityId).Select(c => new Deploy.CommodityStockDTO { Id = c.Id, ComAttribute = c.ComAttribute }).ToList();
                        if (comStocks.Any())
                        {
                            foreach (var commodityStockDTO in comStocks)
                            {
                                var comAttributeDtos = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStockDTO.ComAttribute);
                                if (comAttributeDtos.Any(c => c.SecondAttribute == arr[0]))
                                {
                                    sscDto.CommodityStockId = commodityStockDTO.Id;
                                }
                            }
                        }
                    }
                    break;
                default:
                    sscDto.SizeAndColorId = arr[0] + "," + arr[1];
                    //兼容老版本，补全CommodityStockId
                    if (!sscDto.CommodityStockId.HasValue || sscDto.CommodityStockId == Guid.Empty)
                    {
                        var comStocks = CommodityStock.ObjectSet().Where(c => c.CommodityId == sscDto.CommodityId).Select(c => new Deploy.CommodityStockDTO { Id = c.Id, ComAttribute = c.ComAttribute }).ToList();
                        if (comStocks.Any())
                        {
                            foreach (var commodityStockDTO in comStocks)
                            {
                                var comAttributeDtos = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStockDTO.ComAttribute);
                                if (comAttributeDtos.Any(c => c.SecondAttribute == arr[0]) && comAttributeDtos.Any(c => c.SecondAttribute == arr[1]))
                                {
                                    sscDto.CommodityStockId = commodityStockDTO.Id;
                                }
                            }

                        }
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId:电商馆Id</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommoditySDTO> GetShoppongCartItemsNew2Ext(System.Guid userId, System.Guid appId)
        {
            List<ShopCartCommoditySDTO> result = new List<ShopCartCommoditySDTO>();
            List<ShoppingCartItems> shoppingCartItems;


            if (appId == Guid.Empty)
            {
                shoppingCartItems = ShoppingCartItems.ObjectSet()
                                    .OrderByDescending(t => t.SubTime)
                                    .Where(t => t.UserId == userId)
                                    .ToList();
            }
            else
            {
                shoppingCartItems = ShoppingCartItems.ObjectSet()
                                    .OrderByDescending(t => t.SubTime)
                                    .Where(t => t.UserId == userId && t.EsAppId == appId)
                                    .ToList();
            }

            //获取所有的平台id下的应用ids
            List<Guid> esappids = new List<Guid>();
            esappids.Add(appId);
            List<Guid> applist = TPS.ZPHSV.Instance.GetAppIdlist(esappids).Select(p => p.AppId).ToList();
            applist.Add(appId);
            shoppingCartItems = shoppingCartItems.Where(p => applist.Contains(p.AppId)).ToList();

            if (!shoppingCartItems.Any())
                return new List<ShopCartCommoditySDTO>();
            var comIds = shoppingCartItems.Select(c => c.CommodityId).Distinct().ToList();

            var commodityList = Commodity.ObjectSet()
                         .Where(c => comIds.Contains(c.Id))
                         .Select(c => new CommodityDTO
                         {
                             Id = c.Id,
                             State = c.State,
                             Stock = c.Stock,
                             Name = c.Name,
                             AppId = c.AppId,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             IsDel = c.IsDel,
                             ComAttribute = c.ComAttribute,
                             Type = c.Type
                         }).ToList();

            List<Deploy.CommodityStockDTO> commodityStockList = new List<CommodityStockDTO>();
            var comStockIds = shoppingCartItems.Where(c => c.CommodityStockId != Guid.Empty).Select(c => c.CommodityStockId).Distinct().ToList();
            if (comStockIds.Any())
            {
                commodityStockList = CommodityStock.ObjectSet().Where(c => comStockIds.Contains(c.Id)).Select(c => new Deploy.CommodityStockDTO() { Id = c.Id, ComAttribute = c.ComAttribute, Price = c.Price, Stock = c.Stock }).ToList();
            }

            foreach (var shoppingCartItem in shoppingCartItems)
            {
                ShopCartStateEnum shopCartState = ShopCartStateEnum.OK;
                var commodity = commodityList.FirstOrDefault(c => c.Id == shoppingCartItem.CommodityId);
                if (commodity == null)
                    continue;

                int stock = 0;
                decimal price = commodity.Price;

                var commodityStock =
                    commodityStockList.FirstOrDefault(c => c.Id == shoppingCartItem.CommodityStockId);

                #region 购物车失效判断

                #region 是否删除判断

                if (commodity.IsDel)
                {
                    shopCartState = ShopCartStateEnum.Del;
                }

                #endregion

                #region 上架状态判断

                if (commodity.State == 1)
                {
                    shopCartState = ShopCartStateEnum.OffSale;
                }

                #endregion

                #region 属性判断

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
                        if (!Commodity.CheckComMultAttrs(commodity.ComAttribute) || commodityStock == null ||
                            comAttrs == null || !comAttrs.Any())
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
                        stock = commodity.Stock;

                    }
                    else
                    {
                        if (commodityStock == null || commodityStock.Stock <= 0)
                            shopCartState = ShopCartStateEnum.Stock;
                        else
                        {
                            stock = commodityStock.Stock;
                            price = commodityStock.Price;
                        }

                    }
                }


                #endregion

                #endregion
                ShopCartCommoditySDTO sdto = new ShopCartCommoditySDTO
                {
                    Id = commodity.Id,
                    Pic = commodity.PicturesPath,
                    Name = commodity.Name,
                    Price = price,
                    CommodityNumber = shoppingCartItem.CommodityNumber,
                    ShopCartItemId = shoppingCartItem.Id,
                    State = commodity.State,
                    Stock = stock,
                    AppId = commodity.AppId,
                    AppName = "",
                    CommodityStockId = shoppingCartItem.CommodityStockId,
                    ShopCartState = shopCartState,
                    AddShopCartTime = shoppingCartItem.SubTime,
                    Size = shoppingCartItem.ComAttributeIds,
                    Type = commodity.Type ?? 0,
                    JcActivityId = shoppingCartItem.JcActivityId ?? Guid.Empty
                };

                Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO entity = new Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO();
                var specification = CommoditySpecifications.ObjectSet().FirstOrDefault(p => p.CommodityId == sdto.Id && p.Attribute == shoppingCartItem.Specifications);
                if (specification != null)
                {
                    entity.Id = specification.Id;
                    entity.Name = "规格设置";
                    entity.Attribute = specification.Attribute ?? 0;
                    entity.strAttribute = "1*" + specification.Attribute;
                    sdto.Specifications = entity;
                }


                #region 规格设置集合
                List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslists = new List<Deploy.CustomDTO.SpecificationsDTO>();
                var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                if (commoditySpecification.Count() > 0)
                {
                    Guid commodityId = sdto.Id;
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
                            Specificationslists.Add(model);

                        });
                    }

                }
                sdto.Specificationslist = Specificationslists;
                #endregion

                if (shoppingCartItem.ComAttributeIds == ",")
                {
                    sdto.Size = "";
                }
                else if (shoppingCartItem.ComAttributeIds.Substring(shoppingCartItem.ComAttributeIds.Length - 1, 1) == ",")
                {
                    sdto.Size = shoppingCartItem.ComAttributeIds.Substring(0, shoppingCartItem.ComAttributeIds.Length - 1);
                }
                //修改
                var itemsList = ZPHSV.Instance.GetItemsListByActivityId((Guid)sdto.JcActivityId).Data;
                if (itemsList != null && itemsList.Count() > 0)
                {
                    itemsList = itemsList.Where(t => t.ComdtyStockId == sdto.CommodityStockId && t.ComdtyId == sdto.Id).ToList();
                    if (itemsList.Any())
                    {
                        sdto.Name = "[" + itemsList.FirstOrDefault().JCActivityName + "] " + sdto.Name;
                        sdto.Price = Convert.ToDecimal(itemsList.FirstOrDefault().GroupPrice.ToString("0.00"));
                    }
                }

                result.Add(sdto);
            }

            #region 优惠
            var promotionDic = TodayPromotion.GetCurrentPromotions(comIds);
            //循环取会员价
            var appIds = result.Select(c => c.AppId).Distinct().ToList();
            var vipDict = AVMSV.GetVipIntensities(appIds, userId);

            foreach (var commodity in result)
            {
                commodity.DiscountPrice = commodity.LimitBuyEach = commodity.LimitBuyTotal = -1;
                commodity.Intensity = 10;
                var promotion = promotionDic.FirstOrDefault(c => c.CommodityId == commodity.Id);
                if (promotion != null)
                {
                    commodity.LimitBuyEach = promotion.LimitBuyEach ?? -1;
                    commodity.LimitBuyTotal = promotion.LimitBuyTotal ?? -1;
                    commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal ?? 0;
                    if (promotion.Intensity < 10)
                    {
                        commodity.DiscountPrice = -1;
                        commodity.Intensity = promotion.Intensity;
                    }
                    else
                    {
                        commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                        commodity.Intensity = 10;
                    }
                    var skuActivity = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                    if (skuActivity != null)
                    {
                        commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice.ToString("0.00"));
                        commodity.IsJoin = skuActivity.IsJoin;
                        if (promotion.Intensity < 10)
                        {
                            commodity.DiscountPrice = -1;
                        }
                    }
                }
                else
                {
                    //会员价
                    VipPromotionDTO privilegeInfo = null;
                    if (vipDict.ContainsKey(commodity.AppId))
                        privilegeInfo = vipDict[commodity.AppId];

                    if (privilegeInfo != null && (privilegeInfo.DiscountPrice > -1 || privilegeInfo.Intensity < 10))
                    {
                        var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2, MidpointRounding.AwayFromZero);

                        if (privilegePrice < commodity.Price)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = privilegeInfo.Intensity;
                        }

                    }
                }

            }
            #endregion

            #region 赠品信息
            var now = DateTime.Now;
            foreach (var commodity in result)
            {
                // && _.CommoditySKUId == commodity.CommodityStockId
                var presents = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == commodity.Id)
                            .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                                pp => pp.PresentPromotionId, ppc => ppc.Id,
                                (c, p) => new { Commodity = c, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime })
                            .ToList();

                if (presents.Count > 0)
                {
                    var present = presents.First();
                    commodity.Present = new ShopCartCommodiyPresentDTO();
                    commodity.Present.Limit = present.Limit ?? 1;
                    if (commodity.Present.Limit == 0)
                    {
                        commodity.Present.Limit = 1;
                    }
                    //commodity.Present.BeginTime = present.BeginTime;
                    //commodity.Present.EndTime = present.EndTime;
                    commodity.Present.CommodityStockIds = presents/*.Where(_ => _.Commodity.CommoditySKUId != Guid.Empty)*/.Select(_ => _.Commodity.CommoditySKUId).ToList();
                    //commodity.Present.IsAll = true;
                    //commodity.Present.Title = "购买即送超值赠品 （赠完即止）";
                    //if (commodity.CommodityStocks != null && commodity.CommodityStocks.Count > 0)
                    //{
                    //    bool isAll = true;
                    //    var titles = new List<string>();
                    //    foreach (var item in commodity.CommodityStocks)
                    //    {
                    //        if (commodity.Present.CommodityStockIds.Any(_ => _ == item.Id))
                    //        {
                    //            titles.Add(string.Join("，", item.ComAttribute.Select(_ => _.SecondAttribute)));
                    //        }
                    //        else
                    //        {
                    //            isAll = false;
                    //        }
                    //    }
                    //    if (!isAll)
                    //    {
                    //        commodity.Present.IsAll = false;
                    //        commodity.Present.Title = "购买 “" + string.Join("”、 “", titles) + "” 送超值赠品 （赠完即止）";
                    //    }
                    //}

                    var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == present.PromotionId).ToList();
                    var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                    var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                    commodity.Present.Items = new List<CommodiyPresentItem>();
                    foreach (var item in gifts)
                    {
                        var tempCom = GetCommodity(commodity.AppId, item.CommodityId);
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
                                commodity.Present.Items.Add(commodiyPresentItem);
                            }
                        }
                        else
                        {
                            if (tempCom.Stock > 0)
                            {
                                commodiyPresentItem.Stock = tempCom.Stock;
                                commodity.Present.Items.Add(commodiyPresentItem);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 应用名称
            Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
            if (!listApps.Any())
            {
                return result;
            }
            foreach (var item in result)
            {
                if (!listApps.ContainsKey(item.AppId))
                {
                    continue;
                }
                var listAppName = listApps[item.AppId];
                if (!String.IsNullOrEmpty(listAppName))
                {
                    item.AppName = listAppName;
                }
            }
            #endregion

            var a = result.GroupBy(t => t.AppId);
            List<ShopCartCommoditySDTO> rList = new List<ShopCartCommoditySDTO>();
            foreach (var info in a)
            {
                rList.AddRange(info.ToList());
            }
            return rList;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ShopCartListDTO> GetShoppongCartItemsNew3Ext(System.Guid userId, System.Guid appId)
        {
            LogHelper.Info(string.Format("/GetShoppongCartItemsNew3   userId:{0}  appId:{1}", userId, appId));

            var returnInfo = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ShopCartListDTO>()
            {
                Data = new ShopCartListDTO()
                {
                    ShopList = new List<ShopCartOfShopDto>(),
                    InvalidList = new List<ShopCartCommoditySDTO>()
                }
            };

            List<ShopCartCommoditySDTO> result = new List<ShopCartCommoditySDTO>();
            List<ShoppingCartItems> shoppingCartItems;

            try
            {
                if (appId == Guid.Empty)
                {
                    shoppingCartItems = ShoppingCartItems.ObjectSet()
                                        .OrderByDescending(t => t.SubTime)
                                        .Where(t => t.UserId == userId)
                                        .ToList();
                }
                else
                {
                    shoppingCartItems = ShoppingCartItems.ObjectSet()
                                        .OrderByDescending(t => t.SubTime)
                                        .Where(t => t.UserId == userId && t.EsAppId == appId)
                                        .ToList();
                }

                LogHelper.Info(string.Format("/GetShoppongCartItemsNew3   shoppingCartItems:{0}", JsonHelper.JsSerializer(shoppingCartItems)));

                //获取所有的平台id下的应用ids
                //List<Guid> esappids = new List<Guid>();
                //esappids.Add(appId);
                //List<Guid> applist = TPS.ZPHSV.Instance.GetAppIdlist(esappids).Select(p => p.AppId).ToList();
                //applist.Add(appId);
                //shoppingCartItems = shoppingCartItems.Where(p => applist.Contains(p.AppId)).ToList();

                var comIds = shoppingCartItems.Select(c => c.CommodityId).Distinct().ToList();
                //var shopcomIds = shoppingCartItems.Select(c =>new Jinher.AMP.Coupon.Deploy.CustomDTO.AppComs{c.CommodityId,c.AppId}).Distinct().ToList();//xiexg
                var mallApply = MallApply.ObjectSet().Where(t => t.EsAppId == appId).Select(o => new { o.AppId, o.Type }).Distinct().ToList();


                List<Jinher.AMP.Coupon.Deploy.CustomDTO.AppComs> shopcomIds = new List<Coupon.Deploy.CustomDTO.AppComs>();
                if (shoppingCartItems != null && shoppingCartItems.Count > 0)
                {
                    foreach (var item in shoppingCartItems)
                    {
                        var currentMall = mallApply.FirstOrDefault(_ => _.AppId == item.AppId);
                        var shopcom = new Coupon.Deploy.CustomDTO.AppComs { AppId = item.AppId, CommodityId = item.CommodityId };
                        if (currentMall != null)
                        {
                            shopcom.type = currentMall.Type;
                        }
                        else
                        {
                            shopcom.type = 1;
                        }
                        shopcomIds.Add(shopcom);
                    }
                    //shopcomIds =(from a in shoppingCartItems
                    //             join b in mallApply on a.AppId equals b.AppId into c
                    //             from dept in c.DefaultIfEmpty()
                    //             select new Jinher.AMP.Coupon.Deploy.CustomDTO.AppComs
                    //             {
                    //                 AppId = a.AppId,
                    //                 CommodityId = a.CommodityId,
                    //                 type = dept.Type
                    //             }).ToList();
                }



                var commodityList = Commodity.ObjectSet()
                             .Where(c => comIds.Contains(c.Id))
                             .Select(c => new CommodityDTO
                             {
                                 Id = c.Id,
                                 State = c.State,
                                 Stock = c.Stock,
                                 Name = c.Name,
                                 AppId = c.AppId,
                                 PicturesPath = c.PicturesPath,
                                 Price = c.Price,
                                 IsDel = c.IsDel,
                                 ComAttribute = c.ComAttribute,
                                 Type = c.Type
                             }).ToList();

                List<Deploy.CommodityStockDTO> commodityStockList = new List<CommodityStockDTO>();
                var comStockIds = shoppingCartItems.Where(c => c.CommodityStockId != Guid.Empty).Select(c => c.CommodityStockId).Distinct().ToList();
                if (comStockIds.Any())
                {
                    commodityStockList = CommodityStock.ObjectSet().Where(c => comStockIds.Contains(c.Id)).Select(c => new Deploy.CommodityStockDTO() { Id = c.Id, ComAttribute = c.ComAttribute, Price = c.Price, Stock = c.Stock }).ToList();
                }

                foreach (var shoppingCartItem in shoppingCartItems)
                {
                    ShopCartStateEnum shopCartState = ShopCartStateEnum.OK;
                    var commodity = commodityList.FirstOrDefault(c => c.Id == shoppingCartItem.CommodityId);
                    if (commodity == null)
                        continue;

                    int stock = 0;
                    decimal price = commodity.Price;

                    var commodityStock =
                        commodityStockList.FirstOrDefault(c => c.Id == shoppingCartItem.CommodityStockId);

                    #region 购物车失效判断

                    #region 是否删除判断

                    if (commodity.IsDel)
                    {
                        shopCartState = ShopCartStateEnum.Del;
                    }

                    #endregion

                    #region 上架状态判断

                    if (commodity.State == 1)
                    {
                        shopCartState = ShopCartStateEnum.OffSale;
                    }

                    #endregion

                    #region 属性判断

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
                            if (!Commodity.CheckComMultAttrs(commodity.ComAttribute) || commodityStock == null ||
                                comAttrs == null || !comAttrs.Any())
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
                            stock = commodity.Stock;

                        }
                        else
                        {
                            if (commodityStock == null || commodityStock.Stock <= 0)
                                shopCartState = ShopCartStateEnum.Stock;
                            else
                            {
                                stock = commodityStock.Stock;
                                price = commodityStock.Price;
                            }

                        }
                    }


                    #endregion

                    #endregion
                    ShopCartCommoditySDTO sdto = new ShopCartCommoditySDTO
                    {
                        Id = commodity.Id,
                        Pic = commodity.PicturesPath,
                        Name = commodity.Name,
                        Price = price,
                        CommodityNumber = shoppingCartItem.CommodityNumber,
                        ShopCartItemId = shoppingCartItem.Id,
                        State = commodity.State,
                        Stock = stock,
                        AppId = commodity.AppId,
                        AppName = "",
                        CommodityStockId = shoppingCartItem.CommodityStockId,
                        ShopCartState = shopCartState,
                        AddShopCartTime = shoppingCartItem.SubTime,
                        Size = shoppingCartItem.ComAttributeIds,
                        Type = commodity.Type ?? 0,
                        JcActivityId = shoppingCartItem.JcActivityId ?? Guid.Empty,
                    };

                    Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO entity = new Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO();
                    var specification = CommoditySpecifications.ObjectSet().FirstOrDefault(p => p.CommodityId == sdto.Id && p.Attribute == shoppingCartItem.Specifications);
                    if (specification != null)
                    {
                        entity.Id = specification.Id;
                        entity.Name = "规格设置";
                        entity.Attribute = specification.Attribute ?? 0;
                        entity.strAttribute = "1*" + specification.Attribute;
                        sdto.Specifications = entity;
                    }

                    #region 规格设置集合
                    List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslists = new List<Deploy.CustomDTO.SpecificationsDTO>();
                    var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                    if (commoditySpecification.Count() > 0)
                    {
                        Guid commodityId = sdto.Id;
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
                                Specificationslists.Add(model);

                            });
                        }

                    }
                    sdto.Specificationslist = Specificationslists;
                    #endregion

                    if (shoppingCartItem.ComAttributeIds == ",")
                    {
                        sdto.Size = "";
                    }
                    else if (shoppingCartItem.ComAttributeIds.Substring(shoppingCartItem.ComAttributeIds.Length - 1, 1) == ",")
                    {
                        sdto.Size = shoppingCartItem.ComAttributeIds.Substring(0, shoppingCartItem.ComAttributeIds.Length - 1);
                    }
                    //修改
                    var itemsList = ZPHSV.Instance.GetItemsListByActivityId((Guid)sdto.JcActivityId).Data;
                    if (itemsList != null && itemsList.Count() > 0)
                    {
                        itemsList = itemsList.Where(t => t.ComdtyStockId == sdto.CommodityStockId && t.ComdtyId == sdto.Id).ToList();
                        if (itemsList.Any())
                        {
                            sdto.Name = "[" + itemsList.FirstOrDefault().JCActivityName + "] " + sdto.Name;
                            sdto.Price = Convert.ToDecimal(itemsList.FirstOrDefault().GroupPrice.ToString("0.00"));
                        }
                    }

                    result.Add(sdto);
                }

                #region 优惠
                var promotionDic = TodayPromotion.GetCurrentPromotions(comIds);
                //循环取会员价
                var appIds = result.Select(c => c.AppId).Distinct().ToList();
                var vipDict = AVMSV.GetVipIntensities(appIds, userId);

                foreach (var commodity in result)
                {
                    commodity.DiscountPrice = commodity.LimitBuyEach = commodity.LimitBuyTotal = -1;
                    commodity.Intensity = 10;
                    var promotion = promotionDic.FirstOrDefault(c => c.CommodityId == commodity.Id);
                    if (promotion != null)
                    {
                        commodity.LimitBuyEach = promotion.LimitBuyEach ?? -1;
                        commodity.LimitBuyTotal = promotion.LimitBuyTotal ?? -1;
                        commodity.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal ?? 0;
                        if (promotion.Intensity < 10)
                        {
                            commodity.DiscountPrice = -1;
                            commodity.Intensity = promotion.Intensity;
                        }
                        else
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(promotion.DiscountPrice);
                            commodity.Intensity = 10;
                        }
                        var skuActivity = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).FirstOrDefault(t => t.CommodityStockId == commodity.CommodityStockId);
                        if (skuActivity != null)
                        {
                            commodity.DiscountPrice = Convert.ToDecimal(skuActivity.JoinPrice.ToString("0.00"));
                            commodity.IsJoin = skuActivity.IsJoin;
                            if (promotion.Intensity < 10)
                            {
                                commodity.DiscountPrice = -1;
                            }
                        }
                    }
                    else
                    {
                        //会员价
                        VipPromotionDTO privilegeInfo = null;
                        if (vipDict.ContainsKey(commodity.AppId))
                            privilegeInfo = vipDict[commodity.AppId];

                        if (privilegeInfo != null && (privilegeInfo.DiscountPrice > -1 || privilegeInfo.Intensity < 10))
                        {
                            var privilegePrice = decimal.Round((commodity.Price * privilegeInfo.Intensity / 10), 2, MidpointRounding.AwayFromZero);

                            if (privilegePrice < commodity.Price)
                            {
                                commodity.DiscountPrice = -1;
                                commodity.Intensity = privilegeInfo.Intensity;
                            }

                        }
                    }

                }
                #endregion

                #region 赠品信息
                var now = DateTime.Now;
                foreach (var commodity in result)
                {
                    // && _.CommoditySKUId == commodity.CommodityStockId
                    var presents = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityId == commodity.Id)
                                .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime),
                                    pp => pp.PresentPromotionId, ppc => ppc.Id,
                                    (c, p) => new { Commodity = c, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime })
                                .ToList();

                    if (presents.Count > 0)
                    {
                        var present = presents.First();
                        commodity.Present = new ShopCartCommodiyPresentDTO();
                        commodity.Present.Limit = present.Limit ?? 1;
                        if (commodity.Present.Limit == 0)
                        {
                            commodity.Present.Limit = 1;
                        }
                        //commodity.Present.BeginTime = present.BeginTime;
                        //commodity.Present.EndTime = present.EndTime;
                        commodity.Present.CommodityStockIds = presents/*.Where(_ => _.Commodity.CommoditySKUId != Guid.Empty)*/.Select(_ => _.Commodity.CommoditySKUId).ToList();
                        //commodity.Present.IsAll = true;
                        //commodity.Present.Title = "购买即送超值赠品 （赠完即止）";
                        //if (commodity.CommodityStocks != null && commodity.CommodityStocks.Count > 0)
                        //{
                        //    bool isAll = true;
                        //    var titles = new List<string>();
                        //    foreach (var item in commodity.CommodityStocks)
                        //    {
                        //        if (commodity.Present.CommodityStockIds.Any(_ => _ == item.Id))
                        //        {
                        //            titles.Add(string.Join("，", item.ComAttribute.Select(_ => _.SecondAttribute)));
                        //        }
                        //        else
                        //        {
                        //            isAll = false;
                        //        }
                        //    }
                        //    if (!isAll)
                        //    {
                        //        commodity.Present.IsAll = false;
                        //        commodity.Present.Title = "购买 “" + string.Join("”、 “", titles) + "” 送超值赠品 （赠完即止）";
                        //    }
                        //}

                        var gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == present.PromotionId).ToList();
                        var giftCommodityStockIds = gifts.Where(_ => _.CommoditySKUId != Guid.Empty).Select(_ => _.CommoditySKUId).ToList();
                        var giftCommodityStocks = CommodityStock.ObjectSet().Where(_ => giftCommodityStockIds.Contains(_.Id)).ToList();
                        commodity.Present.Items = new List<CommodiyPresentItem>();
                        foreach (var item in gifts)
                        {
                            var tempCom = GetCommodity(commodity.AppId, item.CommodityId);
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
                                    commodity.Present.Items.Add(commodiyPresentItem);
                                }
                            }
                            else
                            {
                                if (tempCom.Stock > 0)
                                {
                                    commodiyPresentItem.Stock = tempCom.Stock;
                                    commodity.Present.Items.Add(commodiyPresentItem);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 应用名称
                Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                if (!listApps.Any())
                {
                    returnInfo.Message = "002";
                    return returnInfo;
                }
                foreach (var item in result)
                {
                    if (!listApps.ContainsKey(item.AppId))
                    {
                        continue;
                    }
                    var listAppName = listApps[item.AppId];
                    if (!String.IsNullOrEmpty(listAppName))
                    {
                        item.AppName = listAppName;
                    }
                }
                #endregion

                if (result == null || result.Count == 0)
                {
                    returnInfo.Message = "003";
                    return returnInfo;
                }

                //mahb 6.27购物车需求
                if (result != null && result.Count > 0)
                {
                    LogHelper.Info("/GetShoppongCartItemsNew3   result:" + JsonHelper.JsSerializer(result));

                    //失效商品列表
                    var invalidCommodityList = new List<ShopCartCommoditySDTO>();
                    foreach (var item in result)
                    {
                        item.ShopCartStateDesc = ProcessShopCartCommodityState(item);

                        var shopCartStateEnumList = new List<ShopCartStateEnum>();
                        shopCartStateEnumList.Add(ShopCartStateEnum.Stock);
                        shopCartStateEnumList.Add(ShopCartStateEnum.Attribute);
                        shopCartStateEnumList.Add(ShopCartStateEnum.OffSale);
                        shopCartStateEnumList.Add(ShopCartStateEnum.Del);
                        if (shopCartStateEnumList.Contains(item.ShopCartState))
                        {
                            invalidCommodityList.Add(item);
                        }
                    }

                    if (invalidCommodityList != null && invalidCommodityList.Count > 0)
                    {
                        foreach (var item in invalidCommodityList)
                        {
                            result.Remove(item);
                        }
                        returnInfo.Data.InvalidList = invalidCommodityList;
                    }

                    //处理数据               
                    ProcessShopCartCommodity(result);

                    //店铺优惠券shopcomIds
                    var couponList = GetCouponDto(userId, shopcomIds.Distinct().ToList(),appId);
                    //var couponList = GetCouponDto(userId, result);

                    //店铺列表
                    var groupList = result.GroupBy(t => t.AppId);
                    foreach (var info in groupList)
                    {
                        var shopDto = new ShopCartOfShopDto();
                        shopDto.AppId = info.Key;
                        shopDto.AppName = info.First().AppName;
                        shopDto.List = info.ToList();
                        List<ShopCartCouponDTO> list = new List<ShopCartCouponDTO>();
                        foreach (var obj in couponList.Where(a => a.AppId == info.Key).Distinct().ToList())
                        {
                            if (list.Where(o => o.Id == obj.Id) == null || list.Where(o => o.Id == obj.Id).Count() == 0)
                            {
                                list.Add(obj);
                            }
                        }
                        // shopDto.CouponList = couponList.Where(a => a.AppId == info.Key).Distinct().ToList();
                        shopDto.CouponList = list.ToList();
                        shopDto.IsHasCoupon = shopDto.CouponList.Count > 0;
                        returnInfo.Data.ShopList.Add(shopDto);
                    }
                }
                returnInfo.isSuccess = true;
                returnInfo.Message = "success";
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("/GetShoppongCartItemsNew3   ex:{0}", ex));
                returnInfo.isSuccess = false;
                returnInfo.Message = ex.Message;
            }
            return returnInfo;
        }


        /// <summary>
        /// 处理购物车数据，增加商品标签等
        /// </summary>
        /// <param name="list"></param>
        private void ProcessShopCartCommodity(List<ShopCartCommoditySDTO> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            //初始化
            foreach (var item in list)
            {
                if (item.Present == null)
                {
                    item.Present = new ShopCartCommodiyPresentDTO() { Items = new List<CommodiyPresentItem>() };
                }
                item.Tags = new List<string>();
            }

            var commodityList = new CommoditySV().GetCommodityByIdsWithPreSellExt(list.Select(a => a.Id).ToList());
            if (commodityList == null || commodityList.Count == 0)
            {
                return;
            }

            {//处理多属性
                var idList = commodityList.Where(a => a.ComAttrType > 1).Select(a => a.Id).ToList();
                if (idList.Count > 0)
                {
                    var stockList = CommodityStock.ObjectSet().Where(a => idList.Contains(a.CommodityId)).ToList();
                    foreach (var item in stockList)
                    {
                        var commodityItem = list.FirstOrDefault(a => a.Id == item.CommodityId);
                        if (commodityItem == null)
                        {
                            continue;
                        }
                        commodityItem.CommodityStocks = commodityItem.CommodityStocks ?? new List<CommodityAttrStockDTO>();

                        var stock = new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAttrStockDTO();
                        stock.Price = item.Price;
                        stock.MarketPrice = item.MarketPrice;
                        stock.Stock = item.Stock;
                        stock.Id = item.Id;
                        stock.ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                        commodityItem.CommodityStocks.Add(stock);
                    }
                }
            }

            {//处理标签
                var promotionList = new List<int> { 0, 1, 2, 3, 5, 6, 7 };
                var tagName = "";
                foreach (var item in commodityList)
                {
                    tagName = "";
                    var commodityItem = list.FirstOrDefault(a => a.Id == item.Id);
                    if (commodityItem == null)
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(item.ComAttribute))
                    {
                        commodityItem.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                    }

                    if (item.PromotionTypeNew.HasValue)
                    {
                        if (promotionList.Contains(item.PromotionTypeNew.Value))
                        {
                            continue;
                        }
                    }

                    if (item.YoukaAmount.HasValue && item.YoukaAmount.Value > 0)
                    {
                        var amount = item.YoukaAmount.HasValue ? Math.Round(item.YoukaAmount.Value, 2, MidpointRounding.AwayFromZero) : 0;
                        tagName = ("赠油卡" + amount + "元");
                    }

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        commodityItem.Tags.Add(tagName);
                    }
                }
            }
        }

        private string ProcessShopCartCommodityState(ShopCartCommoditySDTO item)
        {
            var desc = "已失效";
            if (item == null)
            {
                return desc;
            }
            if (item.ShopCartState == ShopCartStateEnum.OffSale)
            {
                desc = "已下架";
            }
            else if (item.ShopCartState == ShopCartStateEnum.Stock)
            {
                desc = "已售罄";
            }
            return desc;
        }

        /// <summary>
        /// 获取购物车全部优惠券(谢晓光)
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="list">店铺商品信息</param>
        /// <param name="esappid">商城ID</param>
        /// <returns></returns>
        public List<ShopCartCouponDTO> GetCouponDto(Guid userId, List<Jinher.AMP.Coupon.Deploy.CustomDTO.AppComs> list,Guid esappid)
        {
            var couponList = new List<ShopCartCouponDTO>();

            //var param = new Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplateUsableRequestDTO();
            //param.UserId = userId;
            //param.CurrentPage = 0;
            //param.PageSize = 100;
            //param.AppList = list.Select(item => item.AppId).ToList();
            //var couponRet = CouponSV.Instance.GetUsableCouponsTemplateList(param, true);

            var param = new Jinher.AMP.Coupon.Deploy.CustomDTO.CouponGetByAppComIds();
            param.userid = userId;
            param.CurrentPage = 0;
            param.PageSize = 100;
            param.Data = list;
            param.EsAppId = esappid;
            string ssd = JsonHelper.JsSerializer(param);
            var couponRet = CouponSV.Instance.GetUsableCouponsTemplateListForCart(param);
            LogHelper.Debug("/GetShoppongCartItemsNew3   couponRes:" + JsonHelper.JsSerializer(param) + "  couponRet:" + JsonHelper.JsSerializer(couponRet));
            if (couponRet != null && couponRet.Data.Count > 0)
            {
                couponRet.Data.ForEach(c =>
                {
                    var coupon = new ShopCartCouponDTO();
                    coupon.Cash = c.Cash;
                    coupon.Id = c.Id;
                    coupon.AppId = c.AppId;
                    coupon.CouponGoodsList = null;
                    coupon.CouponTemplateState = 1;//1111
                    coupon.CouponType = 1;//1111
                    coupon.BeginTime = c.BeginTime;
                    coupon.EndTime = c.EndTime;
                    coupon.LimitCondition = c.LimitCondition;
                    coupon.Description = c.Description;
                    coupon.Direction = c.Direction;
                    coupon.LimitUse = c.LimitUse;
                    coupon.RemainCount = c.RemainCount;
                    coupon.ModifiedOn = c.ModifiedOn;
                    coupon.Name = c.Name;
                    coupon.UseType = 1;//11
                    coupon.ThrowType = 1;///11
                    coupon.ThrowTime = c.ThrowTime;
                    coupon.SubTime = c.SubTime;
                    coupon.UserId = c.UserId;
                    coupon.IsDraw = c.IsDraw;
                    coupon.EndTimeStr = coupon.EndTime.ToString("yyyy-MM-dd");
                    coupon.UseNum = c.UseNum;

                    coupon.CouponName = "";
                    coupon.AppName = "";
                    coupon.Description = "";
                    coupon.ThrowTime = DateTime.Now;
                    coupon.Description = "";
                    coupon.CouponDescription = "";
                    couponList.Add(coupon);
                });
            }
            return couponList;
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shopCartItemIds">购物车Ids</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ResultDTO DeleteShoppingCart2Ext(List<Guid> shopCartItemIds, System.Guid userId, System.Guid appId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                foreach (var shopCartItemId in shopCartItemIds)
                {
                    var shop = ShoppingCartItems.ObjectSet().FirstOrDefault(n => n.Id == shopCartItemId);
                    if (shop != null)
                    {
                        shop.EntityState = EntityState.Deleted;
                        contextSession.Delete(shop);
                    }
                }
                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除购物车商品服务异常。shopCartItemIds：{0}，userId：{1}，appId：{2}", JsonHelper.JsonSerializer(shopCartItemIds), userId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttributeExt(System.Guid commodityId, Guid userId)
        {
            try
            {
                //从缓存中取数据
                var com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
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
                    return new ResultDTO<ShopCartCommodityAttrDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

                var resultData = GetAttributeAndPromotionOld(com, userId);
                if (resultData != null)
                {
                    return new ResultDTO<ShopCartCommodityAttrDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品的属性和优惠信息异常。commodityId：{0}。userId：{1}", commodityId, userId), ex);
            }
            return new ResultDTO<ShopCartCommodityAttrDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttributeNewExt(System.Guid commodityId, Guid userId)
        {
            try
            {
                //从缓存中取数据
                var com = Commodity.GetDTOFromCache(Guid.Empty, commodityId);
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
                    return new ResultDTO<ShopCartCommodityAttrDTO>() { ResultCode = 1, Message = "商品不存在或已删除" };

                var resultData = GetAttributeAndPromotion(com, userId);
                if (resultData != null)
                {
                    return new ResultDTO<ShopCartCommodityAttrDTO>() { ResultCode = 0, Message = "Success", Data = resultData };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品的属性和优惠信息异常。commodityId：{0}。userId：{1}", commodityId, userId), ex);
            }
            return new ResultDTO<ShopCartCommodityAttrDTO>() { ResultCode = 1, Message = "Error" };
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="com"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityAttrDTO GetAttributeAndPromotionOld(CommodityDTO com, Guid userId)
        {
            if (com == null)
            {
                return null;
            }
            var commoditySdto = new ShopCartCommodityAttrDTO
            {
                Price = com.Price,
                Stock = com.Stock,
                Pic = com.PicturesPath,
                LimitBuyEach = -1,
                LimitBuyTotal = -1
            };
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySdto.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
            }
            if (commoditySdto.ComAttibutes != null && commoditySdto.ComAttibutes.Count > 0 && commoditySdto.ComAttibutes.GroupBy(c => c.Attribute).Count() > 1)
            {
                var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                {
                    var commodityStocks = queryStock.Select(item => new CommodityAttrStockDTO
                    {
                        Price = item.Price,
                        MarketPrice = item.MarketPrice,
                        Stock = item.Stock,
                        Id = item.Id,
                        ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute)
                    }).ToList();
                    commoditySdto.CommodityStocks = commodityStocks;
                }
            }
            else
            {
                commoditySdto.CommodityStocks = new List<CommodityAttrStockDTO>();
            }


            #region 规格设置集合 暂时不用
            //List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
            //var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
            //if (commoditySpecification.Count() > 0)
            //{
            //    Guid commodityId = com.Id;
            //    var commoditySpecificationlist = commoditySpecification.Where(p => p.CommodityId == commodityId).ToList();
            //    if (commoditySpecificationlist.Count() > 0)
            //    {
            //        commoditySpecificationlist.ForEach(p =>
            //        {
            //            Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO model = new Deploy.CustomDTO.SpecificationsDTO();
            //            model.Id = p.Id;
            //            model.Attribute = p.Attribute ?? 0;
            //            model.strAttribute = "1*" + p.Attribute + "";
            //            Specificationslist.Add(model);
            //        });
            //    }
            //    commoditySdto.Specifications = Specificationslist;
            //}
            #endregion

            var now = DateTime.Now;
            var tpQuery = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable
                           && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                           orderby pro.PromotionType descending
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
                           }).ToList();

            TodayPromotionDTO promotion = (from pro in tpQuery
                                           where pro.PromotionType == 0 || pro.PromotionType == 1 || pro.PromotionType == 2
                                           select pro).FirstOrDefault();
            if (promotion == null)
            {
                commoditySdto.PromotionTypeNew = ComPromotionStatusEnum.NoPromotion;
                commoditySdto.Intensity = 10;
                commoditySdto.DiscountPrice = -1;
            }
            else
            {
                commoditySdto.PromotionTypeNew = (ComPromotionStatusEnum)promotion.PromotionType;
                commoditySdto.LimitBuyEach = promotion.LimitBuyEach;
                commoditySdto.LimitBuyTotal = promotion.LimitBuyTotal;
                commoditySdto.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;

                if (promotion.DiscountPrice > -1)
                {
                    commoditySdto.Intensity = 10;
                    commoditySdto.DiscountPrice = promotion.DiscountPrice;
                }
                else
                {
                    commoditySdto.Intensity = promotion.Intensity;
                    commoditySdto.DiscountPrice = -1;
                }
            }
            return commoditySdto;
        }


        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="com"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityAttrDTO GetAttributeAndPromotion(CommodityDTO com, Guid userId)
        {
            if (com == null)
            {
                return null;
            }
            var commoditySdto = new ShopCartCommodityAttrDTO
            {
                Price = com.Price,
                Stock = com.Stock,
                Pic = com.PicturesPath,
                LimitBuyEach = -1,
                LimitBuyTotal = -1
            };
            if (!string.IsNullOrEmpty(com.ComAttribute))
            {
                commoditySdto.ComAttibutes = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(com.ComAttribute);
            }
            var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
            {
                var commodityStocks = queryStock.Select(item => new CommodityAttrStockDTO
                {
                    Price = item.Price,
                    MarketPrice = item.MarketPrice,
                    Stock = item.Stock,
                    Id = item.Id,
                    ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute)
                }).ToList();
                commoditySdto.CommodityStocks = commodityStocks;
            }

            var now = DateTime.Now;
            var tpQuery = (from p in PromotionItems.ObjectSet()
                           join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                           where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable
                           && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                           orderby pro.PromotionType descending
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
                           }).ToList();

            TodayPromotionDTO promotion = (from pro in tpQuery
                                           where pro.PromotionType == 0 || pro.PromotionType == 1 || pro.PromotionType == 2
                                           select pro).FirstOrDefault();
            if (promotion == null)
            {
                commoditySdto.PromotionTypeNew = ComPromotionStatusEnum.NoPromotion;
                commoditySdto.Intensity = 10;
                commoditySdto.DiscountPrice = -1;
            }
            else
            {
                commoditySdto.PromotionTypeNew = (ComPromotionStatusEnum)promotion.PromotionType;
                commoditySdto.LimitBuyEach = promotion.LimitBuyEach;
                commoditySdto.LimitBuyTotal = promotion.LimitBuyTotal;
                commoditySdto.SurplusLimitBuyTotal = promotion.SurplusLimitBuyTotal;

                if (promotion.Intensity < 10)
                {
                    commoditySdto.Intensity = promotion.Intensity;
                    commoditySdto.DiscountPrice = -1;
                }
                else
                {
                    commoditySdto.Intensity = 10;
                    commoditySdto.DiscountPrice = promotion.DiscountPrice;
                }

                //获取活动sku价格
                if (promotion.OutsideId != null)
                {
                    var skuAList = ZPHSV.Instance.GetSkuActivityList((Guid)promotion.OutsideId).Where(t => t.IsJoin && t.CommodityId == com.Id);
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
                    commoditySdto.SkuActivityCdtos = skuActivityCdtos;
                    if (skuActivityCdtos.Count > 0)
                    {
                        commoditySdto.DiscountPrice = skuActivityCdtos.Min(t => t.JoinPrice);
                        if (promotion.Intensity < 10)
                        {
                            commoditySdto.DiscountPrice = -1;
                        }
                    }
                }
            }
            return commoditySdto;
        }

        /// <summary>
        /// 编辑购物车项数量
        /// </summary>
        /// <param name="shopCartCommodityUpdateDtO">购物车编辑实体</param>
        /// <returns></returns>
        public ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingCart2Ext(ShopCartCommodityUpdateDTO shopCartCommodityUpdateDtO)
        {
            ResultDTO<ShopCartUpdateResultDTO> result = new ResultDTO<ShopCartUpdateResultDTO>();
            result.Data = new ShopCartUpdateResultDTO();
            result.Data.CommodityAttrStock = result.Data.LimitBuyEach = result.Data.LimitBuyTotal = -1;
            result.Data.SurplusLimitBuyTotal = 0;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //从数据库获取购物车节点信息
                ShoppingCartItems shoppingCartItem = ShoppingCartItems.ObjectSet().FirstOrDefault(t => t.Id == shopCartCommodityUpdateDtO.ShopCartItemId);

                if (shoppingCartItem != null)
                {
                    shoppingCartItem.CommodityNumber = shopCartCommodityUpdateDtO.Number;

                    #region 获取商品库存
                    Commodity com = Commodity.ObjectSet().FirstOrDefault(t => t.Id == shoppingCartItem.CommodityId);
                    if (com != null)
                    {
                        result.Data.Stock = com.Stock;
                        if (shopCartCommodityUpdateDtO.Number > com.Stock)
                        {
                            shoppingCartItem.CommodityNumber = com.Stock;
                        }
                    }
                    #endregion

                    #region 若商品为多属性 则获取当前属性下的库存

                    if (shoppingCartItem.CommodityStockId != Guid.Empty)
                    {
                        var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                        {
                            var commodityStocks = queryStock.Select(item => new CommodityAttrStockDTO
                            {
                                Price = item.Price,
                                MarketPrice = item.MarketPrice,
                                Stock = item.Stock,
                                Id = item.Id,
                                ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute)
                            }).ToList();
                            var commodityAttrStockDto =
                                commodityStocks.FirstOrDefault(t => t.Id == shoppingCartItem.CommodityStockId);
                            if (commodityAttrStockDto != null)
                            {
                                int stock = commodityAttrStockDto.Stock;
                                result.Data.CommodityAttrStock = stock;
                                if (shopCartCommodityUpdateDtO.Number > stock)
                                {
                                    shoppingCartItem.CommodityNumber = stock;
                                }
                            }
                        }
                    }

                    #endregion

                    #region 获取限购数量
                    var now = DateTime.Now;
                    TodayPromotionDTO tpQuery = (from p in PromotionItems.ObjectSet()
                                                 join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                                 where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable
                                                       && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                                                 orderby pro.PromotionType descending
                                                 select new TodayPromotionDTO
                                                 {
                                                     LimitBuyEach = p.LimitBuyEach,
                                                     LimitBuyTotal = p.LimitBuyTotal,
                                                     SurplusLimitBuyTotal = p.SurplusLimitBuyTotal
                                                 }).FirstOrDefault();
                    if (tpQuery != null)
                    {
                        if (tpQuery.LimitBuyEach != null && tpQuery.LimitBuyEach != -1)
                        {
                            result.Data.LimitBuyEach = (int)tpQuery.LimitBuyEach;
                            if (shopCartCommodityUpdateDtO.Number > tpQuery.LimitBuyEach)
                            {
                                shoppingCartItem.CommodityNumber = (int)tpQuery.LimitBuyEach;
                            }
                        }
                        if (tpQuery.LimitBuyTotal != null && tpQuery.LimitBuyTotal != -1)
                        {
                            result.Data.LimitBuyTotal = (int)tpQuery.LimitBuyTotal;
                        }
                        if (tpQuery.SurplusLimitBuyTotal != null && tpQuery.SurplusLimitBuyTotal != 0)
                        {
                            result.Data.SurplusLimitBuyTotal = (int)tpQuery.SurplusLimitBuyTotal;
                        }
                    }

                    #endregion

                    result.Data.CommodityNumber = shoppingCartItem.CommodityNumber;
                    //更新购物车节点信息
                    shoppingCartItem.EntityState = EntityState.Modified;
                    contextSession.SaveObject(shoppingCartItem);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑购物车项数量服务异常。shoppingCartItem：{0}，userId：{1}，appId：{2}，", JsonHelper.JsonSerializer(shopCartCommodityUpdateDtO), shopCartCommodityUpdateDtO.UserId, shopCartCommodityUpdateDtO.AppId), ex);
                result.ResultCode = 1;
                result.Message = "Error";
                return result;
            }
            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }

        /// <summary>
        /// 编辑购物车项属性
        /// </summary>
        /// <param name="shopAttribute">属性对实体</param>
        /// <returns></returns>
        public ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingAttributeExt(ShopAttributeCommodityUpdateDto shopAttribute)
        {
            ResultDTO<ShopCartUpdateResultDTO> result = new ResultDTO<ShopCartUpdateResultDTO>();
            result.Data = new ShopCartUpdateResultDTO();
            result.Data.LimitBuyEach = result.Data.CommodityAttrStock = result.Data.LimitBuyTotal = -1;
            result.Data.SurplusLimitBuyTotal = 0;
            try
            {
                //目标属性库存id
                Guid cCommodityStockId = new Guid();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //从数据库获取购物车项信息
                var shoppingCartItem = ShoppingCartItems.ObjectSet().FirstOrDefault(s => s.Id == shopAttribute.ShopCartItemId);
                if (shoppingCartItem != null)
                {
                    //根据传递的商品属性对 获取商品属性库存表Id
                    bool b = false;
                    var commodityStockList = CommodityStock.ObjectSet().Where(t => t.CommodityId == shoppingCartItem.CommodityId);
                    var arr = shopAttribute.StrComAttributes.Split(',');
                    foreach (var commodityStock in commodityStockList)
                    {
                        var comAttributeDtos = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodityStock.ComAttribute);
                        LogHelper.Info(string.Format("编辑购物车项属性服务。comAttributeDtos：{0}", comAttributeDtos));
                        if (comAttributeDtos.Any(c => c.SecondAttribute == arr[0]) && comAttributeDtos.Any(c => c.SecondAttribute == arr[1]))
                        {
                            cCommodityStockId = commodityStock.Id;
                            break;
                        }
                        //单属性商品
                        else if (comAttributeDtos.Any(c => c.SecondAttribute == arr[0]))
                        {
                            cCommodityStockId = commodityStock.Id;
                        }
                    }
                    LogHelper.Info(string.Format("编辑购物车项属性服务。cCommodityStockId：{0}", cCommodityStockId));

                    //没有改变属性对值 直接返回
                    if (cCommodityStockId == shoppingCartItem.CommodityStockId && arr[1] != "")
                    {
                        return new ResultDTO<ShopCartUpdateResultDTO>() { ResultCode = 0, Message = "Success" };
                    }

                    #region 判断当前购物车列表是否有相同商品以及相同属性的数据 若存在做数量相加处理
                    var shoppongCartItems = GetShoppongCartItemsNew2Ext(shopAttribute.UserId, shopAttribute.AppId);
                    int num = shoppingCartItem.CommodityNumber;
                    foreach (var s in shoppongCartItems)
                    {
                        if (s.ShopCartItemId != shoppingCartItem.Id && s.Id == shoppingCartItem.CommodityId)
                        {
                            //获取购物车多属性商品库存信息
                            if ((cCommodityStockId == s.CommodityStockId && arr[1] != "") || (arr[1] == "" && shopAttribute.StrComAttributes == s.Size + ","))
                            {
                                //相同属性的商品数量相加
                                num = shoppingCartItem.CommodityNumber + s.CommodityNumber;
                                //删除当前商品购物车数据
                                var shop = ShoppingCartItems.ObjectSet().FirstOrDefault(n => n.Id == s.ShopCartItemId);
                                if (shop != null)
                                {
                                    shop.EntityState = EntityState.Deleted;
                                    contextSession.Delete(shop);
                                }
                            }
                        }
                    }
                    shoppingCartItem.CommodityNumber = num;
                    #endregion

                    #region 获取商品库存
                    Commodity com = Commodity.ObjectSet().FirstOrDefault(t => t.Id == shoppingCartItem.CommodityId);
                    if (com != null)
                    {
                        result.Data.Stock = com.Stock;
                        if (num > com.Stock)
                        {
                            shoppingCartItem.CommodityNumber = com.Stock;
                        }
                    }
                    #endregion

                    #region 若商品为多属性 则获取当前属性下的库存
                    var queryStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == com.Id).ToList();
                    {
                        var commodityStocks = queryStock.Select(item => new CommodityAttrStockDTO
                        {
                            Price = item.Price,
                            MarketPrice = item.MarketPrice,
                            Stock = item.Stock,
                            Id = item.Id,
                            ComAttribute = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute)
                        }).ToList();
                        var commodityAttrStockDto = commodityStocks.FirstOrDefault(t => t.Id == cCommodityStockId);
                        if (commodityAttrStockDto != null)
                        {
                            int stock = commodityAttrStockDto.Stock;
                            result.Data.CommodityAttrStock = stock;
                            if (com != null && num > stock)
                            {
                                shoppingCartItem.CommodityNumber = stock;
                            }
                        }
                    }
                    #endregion

                    #region 获取限购数量
                    var now = DateTime.Now;
                    TodayPromotionDTO tpQuery = (from p in PromotionItems.ObjectSet()
                                                 join pro in Promotion.ObjectSet() on p.PromotionId equals pro.Id
                                                 where p.CommodityId == com.Id && !pro.IsDel && pro.IsEnable
                                                     && pro.EndTime >= now && (pro.StartTime <= now || pro.PresellStartTime <= now)
                                                 orderby pro.PromotionType descending
                                                 select new TodayPromotionDTO
                                                 {
                                                     LimitBuyEach = p.LimitBuyEach,
                                                     LimitBuyTotal = p.LimitBuyTotal,
                                                     SurplusLimitBuyTotal = p.SurplusLimitBuyTotal
                                                 }).FirstOrDefault();
                    if (tpQuery != null)
                    {
                        if (tpQuery.LimitBuyEach != null && tpQuery.LimitBuyEach != -1)
                        {
                            result.Data.LimitBuyEach = (int)tpQuery.LimitBuyEach;
                            if (num > tpQuery.LimitBuyEach)
                            {
                                shoppingCartItem.CommodityNumber = (int)tpQuery.LimitBuyEach;
                            }
                        }
                        if (tpQuery.LimitBuyTotal != null && tpQuery.LimitBuyTotal != -1)
                        {
                            result.Data.LimitBuyTotal = (int)tpQuery.LimitBuyTotal;
                        }
                        if (tpQuery.SurplusLimitBuyTotal != null && tpQuery.SurplusLimitBuyTotal != 0)
                        {
                            result.Data.SurplusLimitBuyTotal = (int)tpQuery.SurplusLimitBuyTotal;
                        }
                    }

                    #endregion
                    shoppingCartItem.ComAttributeIds = shopAttribute.StrComAttributes;
                    shoppingCartItem.CommodityStockId = cCommodityStockId;
                    //规格设置
                    shoppingCartItem.Specifications = shopAttribute.Specifications;
                    result.Data.CommodityNumber = shoppingCartItem.CommodityNumber;
                    //更新购物车节点信息
                    shoppingCartItem.EntityState = EntityState.Modified;
                    contextSession.SaveObject(shoppingCartItem);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑购物车项属性服务异常。shopAttributeCommodityUpdateDtos：{0}，userId：{1}，appId：{2}，", JsonHelper.JsonSerializer(shopAttribute), shopAttribute.UserId, shopAttribute.AppId), ex);
                result.ResultCode = 1;
                result.Message = "Error";
                return result;
            }
            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }

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
    }
}
