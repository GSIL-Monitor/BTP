
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2017/11/30 16:01:56
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{
    public partial class PresentPromotionBP : BaseBP, IPresentPromotion
    {
        /// <summary>
        /// 查询活动
        /// </summary>
        public ResultDTO<ListResult<PresentPromotionSearchResultDTO>> GetPromotionsExt(PresentPromotionSearchDTO input)
        {
            var query = PresentPromotion.ObjectSet().Where(_ => _.AppId == input.AppId);
            var now = DateTime.Now;
            if (input.Status.HasValue)
            {
                switch (input.Status.Value)
                {
                    case 0:
                        query = query.Where(_ => _.BeginTime > now);
                        break;
                    case 1:
                        query = query.Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime);
                        break;
                    case 2:
                        query = query.Where(_ => _.IsEnd || _.EndTime < now);
                        break;
                }
            }
            if (input.BeginDate.HasValue)
            {
                query = query.Where(_ => _.BeginTime >= input.BeginDate);
            }
            if (input.EndDate.HasValue)
            {
                query = query.Where(_ => _.EndTime <= input.EndDate);
            }
            if (!string.IsNullOrWhiteSpace(input.CommodityName))
            {
                var ids = PresentPromotionCommodity.ObjectSet().Where(_ => _.CommodityName.Contains(input.CommodityName)).Select(_ => _.PresentPromotionId);
                query = query.Where(_ => ids.Contains(_.Id));
            }
            var count = query.Count();
            var data = query.OrderByDescending(q => q.SubTime).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).Select(_ => new PresentPromotionSearchResultDTO
            {
                Id = _.Id,
                BeginTime = _.BeginTime,
                EndTime = _.EndTime,
                Name = _.Name,
                IsEnd = _.IsEnd
            }).ToList();
            data.ForEach(_ =>
            {
                // 已结束:当前日期晚于促销结束时间
                if (_.IsEnd || now > _.EndTime)
                {
                    _.Status = 2;
                }
                // 活动中:当前日期早于促销开始时间
                else if (_.BeginTime < now && now < _.EndTime)
                {
                    _.Status = 1;
                }
                // 未开始
                else
                {
                    _.Status = 0;
                }
            });

            return new ResultDTO<ListResult<PresentPromotionSearchResultDTO>>
            {
                isSuccess = true,
                Data = new ListResult<PresentPromotionSearchResultDTO> { List = data, Count = count }
            };
        }

        /// <summary>
        /// 结束活动
        /// </summary>
        public ResultDTO EndPromotionExt(System.Guid id)
        {
            var presentPromotion = PresentPromotion.ObjectSet().Where(_ => _.Id == id).FirstOrDefault();
            if (presentPromotion != null)
            {
                presentPromotion.IsEnd = true;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(presentPromotion);
                contextSession.SaveChange();
            }
            return new ResultDTO { isSuccess = true };

        }

        /// <summary>
        /// 删除活动
        /// </summary>
        public ResultDTO DeletePromotionExt(System.Guid id)
        {
            var presentPromotion = PresentPromotion.ObjectSet().Where(_ => _.Id == id).FirstOrDefault();
            if (presentPromotion != null)
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                presentPromotion.EntityState = System.Data.EntityState.Deleted;
                contextSession.SaveObject(presentPromotion);
                contextSession.SaveChange();
            }
            return new ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 获取活动详情
        /// </summary>
        public ResultDTO<PresentPromotionCreateDTO> GetPromotionDetailsExt(Guid id)
        {
            var presentPromotion = PresentPromotion.ObjectSet().Where(_ => _.Id == id).Select(_ => new PresentPromotionCreateDTO
            {
                Id = _.Id,
                AppId = _.AppId,
                BeginTime = _.BeginTime,
                EndTime = _.EndTime,
                Limit = _.Limit,
                Name = _.Name
            }).FirstOrDefault();
            if (presentPromotion != null)
            {
                presentPromotion.Commodities = PresentPromotionCommodity.ObjectSet().Where(_ => _.PresentPromotionId == presentPromotion.Id).Select(_ => new PresentPromotionCommodityDetailsDTO
                {
                    Id = _.Id,
                    CommodityId = _.CommodityId,
                    Code = _.CommodityCode,
                    Name = _.CommodityName,
                    Price = _.CommodityPrice,
                    Limit = _.Limit,
                    SKUCode = _.CommoditySKUCode,
                    SKUId = _.CommoditySKUId,
                    SKUName = _.CommoditySKU
                }).ToList();
                presentPromotion.Gifts = PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == presentPromotion.Id).Select(_ => new PresentPromotionCommodityDetailsDTO
                {
                    Id = _.Id,
                    CommodityId = _.CommodityId,
                    Code = _.CommodityCode,
                    Name = _.CommodityName,
                    Price = _.CommodityPrice,
                    Limit = _.Number,
                    SKUCode = _.CommoditySKUCode,
                    SKUId = _.CommoditySKUId,
                    SKUName = _.CommoditySKU
                }).ToList();
                return new ResultDTO<PresentPromotionCreateDTO> { isSuccess = true, Data = presentPromotion };
            }
            return new ResultDTO<PresentPromotionCreateDTO> { isSuccess = false, Message = "参数错误" };
        }

        /// <summary>
        /// 发布活动
        /// </summary>
        public ResultDTO CreatePromotionExt(PresentPromotionCreateDTO input)
        {
            if (input.Commodities == null || input.Commodities.Count == 0)
            {
                return new ResultDTO { isSuccess = false, Message = "请选择主商品" };
            }
            if (input.Gifts == null || input.Gifts.Count == 0)
            {
                return new ResultDTO { isSuccess = false, Message = "请选择赠品" };
            }
            var comIds = input.Commodities.Select(_ => _.CommodityId);
            // 检查活动是否冲突
            // 该商品正在参与XX（秒杀、预约、预售、拼团、限时打折、赠品促销、套餐促销）活动，请更换商品或者更改活动时间
            var prop = (from item in PromotionItems.ObjectSet()
                        join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                        where
                            !pro.IsDel && (comIds.Contains(item.CommodityId) &&
                            pro.EndTime >= input.BeginTime && pro.StartTime <= input.EndTime)
                        select pro).FirstOrDefault();
            if (prop != null)
            {
                var propName = "限时打折";
                switch (prop.PromotionType)
                {
                    case 1:
                        propName = "秒杀";
                        break;
                    case 2:
                        propName = "预约";
                        break;
                    case 3:
                        propName = "拼团";
                        break;
                    case 5:
                        propName = "预售";
                        break;
                }

                Jinher.AMP.ZPH.ISV.Facade.CommodityFacade facadeCheck = new Jinher.AMP.ZPH.ISV.Facade.CommodityFacade();

                foreach (var item in comIds)
                {
                    Jinher.AMP.ZPH.Deploy.CustomDTO.CheckComdtyActInSameTimeCDTO dto = new ZPH.Deploy.CustomDTO.CheckComdtyActInSameTimeCDTO()
                    {
                        comdtyId = item,
                        startTime = input.BeginTime,
                        endTime = input.EndTime
                    };

                    Jinher.AMP.ZPH.Deploy.CustomDTO.ReturnInfo requst = facadeCheck.CheckComdtyActInSameTime(dto);
                    if (requst.Message.Contains("套装"))
                    {
                        propName = "套装";
                        break;
                    }
                }

                return new ResultDTO { isSuccess = false, Message = "该商品正在参与" + propName + "活动，请更换商品或者更改活动时间" };
            }
            var ppCount = PresentPromotionCommodity.ObjectSet().Where(_ => comIds.Contains(_.CommodityId)).Join(PresentPromotion.ObjectSet().Where(p => !p.IsEnd && p.EndTime >= input.BeginTime && p.BeginTime <= input.EndTime), pc => pc.PresentPromotionId, p => p.Id, (pc, p) => 1).Count();
            if (ppCount > 0)
            {
                return new ResultDTO { isSuccess = false, Message = "该商品正在参与赠品促销活动，请更换商品或者更改活动时间" };
            }

            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            var presentPromotion = PresentPromotion.CreatePresentPromotion();
            presentPromotion.AppId = input.AppId;
            presentPromotion.UserId = ContextDTO.LoginUserID;
            presentPromotion.Name = input.Name;
            presentPromotion.BeginTime = input.BeginTime;
            presentPromotion.EndTime = input.EndTime;
            presentPromotion.Limit = input.Limit;
            presentPromotion.IsEnd = false;
            contextSession.SaveObject(presentPromotion);

            foreach (var item in input.Commodities)
            {
                PresentPromotionCommodity ppc = PresentPromotionCommodity.CreatePresentPromotionCommodity();
                ppc.PresentPromotionId = presentPromotion.Id;
                ppc.AppId = input.AppId;
                ppc.UserId = ContextDTO.LoginUserID;
                ppc.CommodityId = item.CommodityId;
                ppc.CommodityCode = item.Code;
                ppc.CommodityName = item.Name;
                ppc.CommoditySKUId = item.SKUId;
                ppc.CommoditySKU = item.SKUName;
                ppc.CommoditySKUCode = item.SKUCode;
                ppc.CommodityPrice = item.Price;
                ppc.Limit = item.Limit;
                contextSession.SaveObject(ppc);
            }

            foreach (var item in input.Gifts)
            {
                PresentPromotionGift ppg = PresentPromotionGift.CreatePresentPromotionGift();
                ppg.PresentPromotionId = presentPromotion.Id;
                ppg.AppId = input.AppId;
                ppg.UserId = ContextDTO.LoginUserID;
                ppg.CommodityId = item.CommodityId;
                ppg.CommodityCode = item.Code;
                ppg.CommodityName = item.Name;
                ppg.CommoditySKUId = item.SKUId;
                ppg.CommoditySKU = item.SKUName;
                ppg.CommoditySKUCode = item.SKUCode;
                ppg.CommodityPrice = item.Price;
                ppg.Number = item.Limit;
                contextSession.SaveObject(ppg);
            }
            try
            {
                contextSession.SaveChange(Jinher.JAP.Common.SaveExceptionEnum.BF);
            }
            catch (Exception ex)
            {
                LogHelper.Error("PresentPromotionBPExt.CreatePromotionExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 更新活动
        /// </summary>
        public ResultDTO UpdatePromotionExt(PresentPromotionCreateDTO input)
        {
            if (input.Id == Guid.Empty)
            {
                return new ResultDTO { isSuccess = false, Message = "参数错误" };
            }
            if (input.Commodities == null || input.Commodities.Count == 0)
            {
                return new ResultDTO { isSuccess = false, Message = "请选择主商品" };
            }
            if (input.Gifts == null || input.Gifts.Count == 0)
            {
                return new ResultDTO { isSuccess = false, Message = "请选择赠品" };
            }

            var comIds = input.Commodities.Select(_ => _.CommodityId);
            // 检查活动是否冲突
            // 该商品正在参与XX（秒杀、预约、预售、拼团、限时打折、赠品促销、套餐促销）活动，请更换商品或者更改活动时间
            var prop = (from item in PromotionItems.ObjectSet()
                        join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                        where
                            !pro.IsDel && (comIds.Contains(item.CommodityId) &&
                            pro.EndTime >= input.BeginTime && pro.StartTime <= input.EndTime)
                        select pro).FirstOrDefault();
            if (prop != null)
            {
                var propName = "限时打折";
                switch (prop.PromotionType)
                {
                    case 1:
                        propName = "秒杀";
                        break;
                    case 2:
                        propName = "预约";
                        break;
                    case 3:
                        propName = "拼团";
                        break;
                    case 5:
                        propName = "预售";
                        break;
                }
                return new ResultDTO { isSuccess = false, Message = "该商品正在参与" + propName + "活动，请更换商品或者更改活动时间" };
            }
            var ppCount = PresentPromotionCommodity.ObjectSet().Where(_ => comIds.Contains(_.CommodityId)).Join(PresentPromotion.ObjectSet().Where(p => p.Id != input.Id && p.EndTime >= input.BeginTime && p.BeginTime <= input.EndTime), pc => pc.PresentPromotionId, p => p.Id, (pc, p) => 1).Count();
            if (ppCount > 0)
            {
                return new ResultDTO { isSuccess = false, Message = "该商品正在参与赠品促销活动，请更换商品或者更改活动时间" };
            }

            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                var presentPromotion = PresentPromotion.FindByID(input.Id);
                if (presentPromotion == null)
                {
                    return new ResultDTO { isSuccess = false, Message = "参数错误" };
                }
                var now = DateTime.Now;
                if (presentPromotion.IsEnd || now > presentPromotion.EndTime)
                {
                    return new ResultDTO { isSuccess = false, Message = "活动已结束" };
                }
                if (presentPromotion.BeginTime < now && now < presentPromotion.EndTime)
                {
                    return new ResultDTO { isSuccess = false, Message = "活动已开始" };
                }
                //presentPromotion.AppId = input.AppId;
                //presentPromotion.UserId = ContextDTO.LoginUserID;
                presentPromotion.Name = input.Name;
                presentPromotion.BeginTime = input.BeginTime;
                presentPromotion.EndTime = input.EndTime;
                presentPromotion.Limit = input.Limit;
                presentPromotion.IsEnd = false;
                contextSession.SaveObject(presentPromotion);

                foreach (var item in PresentPromotionCommodity.ObjectSet().Where(_ => _.PresentPromotionId == presentPromotion.Id))
                {
                    item.EntityState = System.Data.EntityState.Deleted;
                    contextSession.SaveObject(item);
                }
                foreach (var item in PresentPromotionGift.ObjectSet().Where(_ => _.PresentPromotionId == presentPromotion.Id))
                {
                    item.EntityState = System.Data.EntityState.Deleted;
                    contextSession.SaveObject(item);
                }

                foreach (var item in input.Commodities)
                {
                    PresentPromotionCommodity ppc = PresentPromotionCommodity.CreatePresentPromotionCommodity();
                    ppc.PresentPromotionId = presentPromotion.Id;
                    ppc.AppId = input.AppId;
                    ppc.UserId = ContextDTO.LoginUserID;
                    ppc.CommodityId = item.CommodityId;
                    ppc.CommodityCode = item.Code;
                    ppc.CommodityName = item.Name;
                    ppc.CommoditySKUId = item.SKUId;
                    ppc.CommoditySKU = item.SKUName;
                    ppc.CommoditySKUCode = item.SKUCode;
                    ppc.CommodityPrice = item.Price;
                    ppc.Limit = item.Limit;
                    contextSession.SaveObject(ppc);
                }

                foreach (var item in input.Gifts)
                {
                    PresentPromotionGift ppg = PresentPromotionGift.CreatePresentPromotionGift();
                    ppg.PresentPromotionId = presentPromotion.Id;
                    ppg.AppId = input.AppId;
                    ppg.UserId = ContextDTO.LoginUserID;
                    ppg.CommodityId = item.CommodityId;
                    ppg.CommodityCode = item.Code;
                    ppg.CommodityName = item.Name;
                    ppg.CommoditySKUId = item.SKUId;
                    ppg.CommoditySKU = item.SKUName;
                    ppg.CommoditySKUCode = item.SKUCode;
                    ppg.CommodityPrice = item.Price;
                    ppg.Number = item.Limit;
                    contextSession.SaveObject(ppg);
                }
                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error("PresentPromotionBPExt.CreatePromotionExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true };
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        public ResultDTO<ListResult<PresentPromotionCommoditySearchResultDTO>> GetCommoditiesExt(PresentPromotionCommoditySearchDTO input)
        {
            if (input.AppId == Guid.Empty)
            {
                return new ResultDTO<ListResult<PresentPromotionCommoditySearchResultDTO>> { isSuccess = false, Message = "参数错误" };
            }

            // 今日活动商品
            var hasAddCommodityIds = TodayPromotion.ObjectSet().Select(_ => _.CommodityId).ToList();

            var query = Commodity.ObjectSet().Where(_ => !_.IsDel && _.State == 0 && _.AppId == input.AppId && !hasAddCommodityIds.Contains(_.Id));

            #region 增加商品查询条件---分类、毛利率区间，价格区间
            query = AddCommoditySelectWhere(input, query, hasAddCommodityIds);

            #endregion
            
            if (!string.IsNullOrWhiteSpace(input.Name))
            {
                query = query.Where(_ => _.Name.Contains(input.Name));
            }
            var count = query.Count();
            var data = query.OrderByDescending(q => q.SubTime).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).Select(_ => new PresentPromotionCommoditySearchResultDTO
            {
                Id = _.Id,
                Code = _.No_Code,
                Name = _.Name,
                Pic = _.PicturesPath,
                Price = _.Price,
                Stock = _.Stock
            }).ToList();
            var ids = data.Select(_ => _.Id).ToList();
            var stocks = CommodityStock.ObjectSet().Where(_ => ids.Contains(_.CommodityId));
            foreach (var d in data)
            {
                d.SKU = new List<PresentPromotionCommoditySearchResultDTO.CommoditySKUModel>();
                foreach (var s in stocks.Where(_ => _.CommodityId == d.Id))
                {
                    var sku = new PresentPromotionCommoditySearchResultDTO.CommoditySKUModel();
                    sku.Id = s.Id;
                    sku.Code = s.No_Code;
                    sku.Price = s.Price;
                    sku.Stock = s.Stock;
                    var skuAttr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ComAttributeDTO>>(s.ComAttribute);
                    sku.Name = string.Join("；", skuAttr.Select(_ => _.Attribute + "：" + _.SecondAttribute));
                    d.SKU.Add(sku);
                }
            }
            return new ResultDTO<ListResult<PresentPromotionCommoditySearchResultDTO>>
            {
                isSuccess = true,
                Data = new ListResult<PresentPromotionCommoditySearchResultDTO> { List = data, Count = count }
            };
        }



        #region 增加商品查询条件     获取类目集合
        /// <summary>
        /// 增加商品查询条件---分类、毛利率区间，价格区间
        /// </summary>
        /// <param name="input">输入查询实体</param>
        /// <param name="commodityQuery">查询对象</param>
        /// <returns></returns>
        private static IQueryable<Commodity> AddCommoditySelectWhere(PresentPromotionCommoditySearchDTO input, IQueryable<Commodity> commodityQuery, List<Guid> hasAddCommodityIds)
        {
            try
            {
                //根据分类查询
                if (input.Categorys != null)
                {
                    List<Guid> listId = GetRecursiveCategoryId(input);


                    commodityQuery = (from scc in CommodityCategory.ObjectSet()
                                      join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                                      where c.AppId == input.AppId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                      && listId.Contains(scc.CategoryId) && !hasAddCommodityIds.Contains(c.Id)
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
        private static List<Guid> GetRecursiveCategoryId(PresentPromotionCommoditySearchDTO input)
        {
            List<Guid> list = new List<Guid>();
            List<Guid> listId2 = new List<Guid>();

            input.Categorys.ForEach(p =>
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


            input.Categorys.ForEach(p =>
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
    }
}