
/***************
功能描述: ZPHBP
作    者: 
创建时间: 2015/3/10 11:38:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.BF.BP.Base;
using AppSetAppDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO;
using AppSetAppGridDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO;
using AppSetSearchDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO;
using ComdtyList4SelCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyList4SelCDTO;
using ComdtySearch4SelCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.ComdtySearch4SelCDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Linq.Expressions;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class SelectCommodityBP : BaseBP, ISelectCommodity
    {
        /// <summary>
        /// 应用列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public AppSetAppGridDTO GetAppListExt(AppSetSearch2DTO search)
        {
            ZPH.Deploy.CustomDTO.QueryPavilionAppParam query = new ZPH.Deploy.CustomDTO.QueryPavilionAppParam
            {
                Id = search.belongTo,
                pageIndex = 1,
                pageSize = int.MaxValue
            };
            var eReturnInfo = ZPHSV.Instance.GetPavilionApp(query);

            AppSetAppGridDTO appSet = new AppSetAppGridDTO { TotalAppCount = eReturnInfo.Data.Count() };
            List<AppSetAppDTO> appSetAppDtos = eReturnInfo.Data.Select(r => new AppSetAppDTO { AppId = r.appId, AppName = r.appName, AppIcon = r.appIcon, AppCreateOn = r.appCreateOn }).ToList();
            appSet.AppList = appSetAppDtos.OrderBy(o => o.AppName).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList(); ;
            return appSet;
        }
        /// <summary>
        /// 查询商品 电商馆
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<ComdtyList4SelCDTO>> SearchCommodityExt(ComdtySearch4SelCDTO search)
        {
            QueryPavilionAppParam query = new QueryPavilionAppParam
            {
                Id = search.belongTo,
                pageIndex = 1,
                pageSize = int.MaxValue
            };
            var eReturnInfo = ZPHSV.Instance.GetPavilionApp(query);
            JAP.Common.Loging.LogHelper.Debug("SearchCommodityExt eReturnInfo belongTo:" + query.Id);

            List<Guid> appIds = new List<Guid>();
            if (search.AppId != null && search.AppId != Guid.Empty)
            {
                appIds.Add(search.AppId.Value);
            }
            else
            {
                appIds.AddRange(eReturnInfo.Data.Select(t => t.appId));
            }
            if (!string.IsNullOrEmpty(search.AppName))
            {
                var matchAppNameAppIds = eReturnInfo.Data.Where(_ => _.appName.Contains(search.AppName)).Select(t => t.appId).ToList();
                appIds = appIds.Intersect(matchAppNameAppIds).ToList();
            }

            var commodityQuery = Commodity.ObjectSet().Where(c => !c.IsDel && c.State == 0);

            #region 增加商品查询条件---分类、毛利率区间，价格区间
            commodityQuery = AddCommoditySelectWhere(search, commodityQuery);

            #endregion
            
            
            if (appIds.Count == 1)
            {
                var appId = appIds[0];
                commodityQuery = commodityQuery.Where(c => c.AppId == appId);
            }
            else
            {
                commodityQuery = commodityQuery.Where(c => appIds.Contains(c.AppId));
            }
            if (!string.IsNullOrEmpty(search.CommodityName))
            {
                commodityQuery = commodityQuery.Where(c => c.Name.Contains(search.CommodityName));
            }
            var commodityList = commodityQuery
                .OrderBy(_ => _.Name)
                .Select(c => new ComdtyList4SelCDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Pic = c.PicturesPath,
                    Price = c.Price,
                    AppId = c.AppId,
                    Stock = c.Stock
                }).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

            var commodityIds = commodityList.Select(_ => _.Id).ToList();
            //var comCategories = CommodityCategory.ObjectSet().Where(_ => _.IsDel == false && commodityIds.Contains(_.CommodityId))
            //    .OrderBy(_ => _.MaxSort).ToList();
            var comCategories = (from comCategory in CommodityCategory.ObjectSet()
                                 join category in Category.ObjectSet() on comCategory.CategoryId equals category.Id
                                 where commodityIds.Contains(comCategory.CommodityId) && comCategory.IsDel == false && !category.IsDel
                                 orderby comCategory.MaxSort
                                 select new { category.Name, comCategory.AppId, comCategory.CommodityId });

            foreach (var comdty in commodityList)
            {
                // 获取AppName
                var a = eReturnInfo.Data.Where(t => t.appId == comdty.AppId).ToList();
                if (a.Count > 0)
                {
                    comdty.AppName = a[0].appName;
                }

                //获取加入的分类名称
                var currentComCategories = comCategories.Where(_ => /*_.AppId == comdty.AppId &&*/ _.CommodityId == comdty.Id).ToList();
                var cnames = "";
                foreach (var category in currentComCategories)
                {
                    cnames += category.Name + "<br>";
                }
                if (string.IsNullOrEmpty(cnames))
                {
                    cnames = "无分类";
                }
                comdty.CommodityCategory = cnames;
            }

            return new ResultDTO<List<ComdtyList4SelCDTO>>
            {
                ResultCode = commodityQuery.Count(),
                Data = commodityList.OrderBy(_ => _.AppName).ToList()
            };


            /************************************************Old Code************************************************/
            //var comdtyList = from c in Commodity.ObjectSet()
            //                 where c.IsDel == false && c.State == 0 && ab.Contains(c.AppId)
            //                 orderby c.Name
            //                 select new ComdtyList4SelCDTO
            //                 {
            //                     Id = c.Id,
            //                     Name = c.Name,
            //                     Pic = c.PicturesPath,
            //                     Price = c.Price,
            //                     AppId = c.AppId,
            //                     Stock = c.Stock
            //                 };



            //List<ComdtyList4SelCDTO> comdtyList4SelCdtos = new List<ComdtyList4SelCDTO>();
            //foreach (var comdty in comdtyList)
            //{
            //    var a = eReturnInfo.Data.Where(t => t.appId == comdty.AppId).ToList();
            //    if (a.Count > 0)
            //    {
            //        comdty.AppName = a[0].appName;
            //    }
            //    //获取加入的分类名称
            //    var temp1 = (from scc in CommodityCategory.ObjectSet()
            //                 join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
            //                 where scc.CommodityId == comdty.Id && scc.IsDel == false && scc.AppId == comdty.AppId
            //                 orderby scc.MaxSort
            //                 select sc);
            //    var cnames = "";
            //    foreach (var category in temp1)
            //    {
            //        cnames += category.Name + "<br>";
            //    }
            //    comdty.CommodityCategory = cnames;

            //    comdtyList4SelCdtos.Add(comdty);
            //}
            //if (search.AppId != null)
            //{
            //    comdtyList4SelCdtos = comdtyList4SelCdtos.Where(o => o.AppId == search.AppId).ToList();
            //}
            //if (!string.IsNullOrEmpty(search.AppName))
            //{
            //    comdtyList4SelCdtos = comdtyList4SelCdtos.Where(o => o.AppName.Contains(search.AppName)).ToList();
            //}
            //if (!string.IsNullOrEmpty(search.CommodityName))
            //{
            //    comdtyList4SelCdtos = comdtyList4SelCdtos.Where(o => o.Name.Contains(search.CommodityName)).ToList();
            //}

            //ResultDTO<List<ComdtyList4SelCDTO>> retInfo = new ResultDTO<List<ComdtyList4SelCDTO>>
            //{
            //    ResultCode = comdtyList4SelCdtos.Count(),
            //    Data = comdtyList4SelCdtos.OrderBy(o => o.AppName)
            //            .Skip((search.PageIndex - 1) * search.PageSize)
            //            .Take(search.PageSize)
            //            .ToList()
            //};
            //return retInfo;
        }

        /// <summary>
        /// 查询商品 非电商馆
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<ComdtyList4SelCDTO>> SearchCommodity2Ext(ComdtySearch4SelCDTO search)
        {
            string appName = APPSV.GetAppName(search.AppId ?? Guid.Empty);
            var comdtyList = from c in Commodity.ObjectSet()
                             where c.IsDel == false && c.State == 0 && c.AppId == search.AppId && c.CommodityType == 0
                             orderby c.Name
                             select new ComdtyList4SelCDTO
                             {
                                 Id = c.Id,
                                 Name = c.Name,
                                 Pic = c.PicturesPath,
                                 Price = c.Price,
                                 AppId = c.AppId,
                                 Stock = c.Stock,
                                 AppName = appName
                             };
            if (!string.IsNullOrEmpty(search.CommodityName))
            {
                comdtyList = comdtyList.Where(o => o.Name.Contains(search.CommodityName));
            }

            List<ComdtyList4SelCDTO> comdtyList4SelCdtos = new List<ComdtyList4SelCDTO>();
            foreach (var comdty in comdtyList)
            {
                //获取加入的分类名称
                var temp1 = (from scc in CommodityCategory.ObjectSet()
                             join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                             where scc.CommodityId == comdty.Id && scc.IsDel == false && scc.AppId == comdty.AppId
                             orderby scc.MaxSort
                             select sc);
                var cnames = "";
                foreach (var category in temp1)
                {
                    cnames += category.Name + "<br>";
                }
                comdty.CommodityCategory = cnames;

                comdtyList4SelCdtos.Add(comdty);
            }

            ResultDTO<List<ComdtyList4SelCDTO>> retInfo = new ResultDTO<List<ComdtyList4SelCDTO>>
            {
                ResultCode = comdtyList4SelCdtos.Count(),
                Data = comdtyList4SelCdtos.Skip((search.PageIndex - 1) * search.PageSize)
                        .Take(search.PageSize)
                        .ToList()
            };
            return retInfo;
        }

        public Deploy.CustomDTO.ResultDTO<SearchCommodityByFreightTemplateOutputDTO> GetCommodityByFreightTemplateExt(SearchCommodityByFreightTemplateInputDTO inputDTO)
        {
            if (inputDTO == null)
            {
                throw new ArgumentNullException();
            }

            var appId = inputDTO.AppId;
            var templateId = inputDTO.TemplateId;
            var showAssociated = inputDTO.ShowAssociated;
            var commodityName = inputDTO.CommodityName;
            var takeCount = inputDTO.PageSize;
            var skipCount = inputDTO.PageIndex > 0 ? --inputDTO.PageIndex * takeCount : 0;
            var defaultTemplateId = Guid.Empty;
            var joinPromotion = inputDTO.JoinPromotion;

            var query = Commodity.ObjectSet().Where(predicate =>
                                                    predicate.AppId == appId
                                                    && (showAssociated ? predicate.FreightTemplateId == templateId : (predicate.FreightTemplateId == null || predicate.FreightTemplateId == defaultTemplateId))
                                                    && (!string.IsNullOrEmpty(commodityName) ? predicate.Name.Contains(commodityName) : true));

            if (!showAssociated && joinPromotion)
            {
                //过滤出搞活动的商品
                query = from commodity in query
                        join promo in Promotion.ObjectSet() on commodity.AppId equals promo.AppId into cpcontainer
                        from promo in cpcontainer.DefaultIfEmpty()
                        join promoItem in PromotionItems.ObjectSet()
                        on new
                        {
                            promoId = promo.Id,
                            commodityId = commodity.Id
                        } equals
                        new
                        {
                            promoId = promoItem.PromotionId,
                            commodityId = promoItem.CommodityId
                        }
                        where promo.StartTime >= DateTime.Now && promo.EndTime <= DateTime.Now
                        select commodity;
            }

            var total = query.Count();

            var list = query.OrderBy(selector => selector.SubTime).Skip(skipCount).Take(takeCount).Select(selector => new ComdtyList4SelCDTO
            {
                Id = selector.Id,
                Name = selector.Name,
                Pic = selector.PicturesPath,
                Price = selector.Price,
                Stock = selector.Stock
            }).ToList();

            var outputDTO = new ResultDTO<SearchCommodityByFreightTemplateOutputDTO>
            {
                Data = new SearchCommodityByFreightTemplateOutputDTO
                {
                    Total = total,
                    Commodities = list
                },
                isSuccess = true,
                ResultCode = 0
            };

            return outputDTO;
        }



        #region 增加商品查询条件     获取类目集合
        /// <summary>
        /// 增加商品查询条件---分类、毛利率区间，价格区间
        /// </summary>
        /// <param name="input">输入查询实体</param>
        /// <param name="commodityQuery">查询对象</param>
        /// <returns></returns>
        private static IQueryable<Commodity> AddCommoditySelectWhere(ComdtySearch4SelCDTO input, IQueryable<Commodity> commodityQuery)
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
        private static List<Guid> GetRecursiveCategoryId(ComdtySearch4SelCDTO input)
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
    }
}
