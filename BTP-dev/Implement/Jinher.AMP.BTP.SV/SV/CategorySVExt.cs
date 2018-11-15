using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CategorySDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO;
using ComdtySearch4SelCDTO = Jinher.AMP.ZPH.Deploy.CustomDTO.ComdtySearch4SelCDTO;
using CommodityListCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO;


namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 商品分类接口类
    /// </summary>
    public partial class CategorySV : BaseSv, ICategory
    {
        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryExt(System.Guid appId)
        {
            //读取缓存
            List<CategorySDTO> storeCache = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_CategoryInfo", appId.ToString(), "BTPCache") as List<CategorySDTO>;
            if (storeCache != null && storeCache.Count > 0)
            {
                return storeCache;
            }


            //所有的类目列表
            List<CategorySDTO> categorylist = new List<CategorySDTO>();

            //获取类目信息
            var category = Category.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false).OrderBy(n => n.Sort);
            var query = from n in category
                        select new CategorySDTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort
                        };
            categorylist = query.ToList<CategorySDTO>();

            //根据级别分类，获取三个级别对应的数据字典
            var dics = categorylist.GroupBy(
                a => a.CurrentLevel,
                (key, group) => new { CurrentLevel = key, CategorySDTOList = group }).
                ToDictionary(a => a.CurrentLevel, a => a.CategorySDTOList);

            if (dics.ContainsKey(1))
            {
                if (!dics.ContainsKey(2))
                {
                    dics.Add(2, new List<CategorySDTO>());
                }
                if (!dics.ContainsKey(3))
                {
                    dics.Add(3, new List<CategorySDTO>());
                }
                dics[1].ToList().ForEach(
                    first =>
                    {
                        if (dics.ContainsKey(2))
                        {
                            //添加第一级分类下的二级分类
                            first.SecondCategory = (from second in dics[2].ToList()
                                                    where second.ParentId == first.Id
                                                    orderby second.Sort
                                                    select new SCategorySDTO
                                                    {
                                                        CurrentLevel = second.CurrentLevel,
                                                        Id = second.Id,
                                                        Name = second.Name,
                                                        ParentId = second.ParentId,
                                                        Sort = second.Sort
                                                    }).ToList();


                            if (dics.ContainsKey(3))
                            {
                                //添加第二级分类下的三级分类
                                first.SecondCategory.ForEach(
                                    second =>
                                    {
                                        second.ThirdCategory =
                                            (from third in dics[3].ToList()
                                             where third.ParentId == second.Id
                                             orderby third.Sort
                                             select new TCategorySDTO
                                             {
                                                 CurrentLevel = third.CurrentLevel,
                                                 Id = third.Id,
                                                 Name = third.Name,
                                                 ParentId = third.ParentId,
                                                 Sort = third.Sort
                                             }).ToList();
                                    });
                            }
                        }
                    });

                Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_CategoryInfo", appId.ToString(), dics[1].ToList(), "BTPCache");
                return dics[1].ToList();
            }
            else
            {
                return new List<CategorySDTO>();
            }
        }

        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategory2Ext(System.Guid appId)
        {
            //所有的类目列表
            List<CategoryS2DTO> categorylist;

            if (JAP.Cache.GlobalCacheWrapper.ContainsCache(string.Format("G_Category2Info_{0}", appId), "BTPCache"))
            {
                categorylist = (List<CategoryS2DTO>)GlobalCacheWrapper.GetDataCache(string.Format("G_Category2Info_{0}", appId), "BTPCache");
                return categorylist;
            }
            else
            {
                //获取类目信息
                var category = Category.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false).OrderBy(n => n.Sort);
                var query = from n in category
                            where n.CurrentLevel > 0
                            select new CategoryS2DTO
                            {
                                CurrentLevel = n.CurrentLevel,
                                Id = n.Id,
                                Name = n.Name,
                                ParentId = n.ParentId,
                                Sort = n.Sort,
                                Icno = n.icon
                            };
                categorylist = query.ToList();

                //根据级别分类，获取四个级别对应的数据字典
                var dics = categorylist.GroupBy(
                    a => a.CurrentLevel,
                    (key, group) => new { CurrentLevel = key, CategorySDTOList = group }).
                    ToDictionary(a => a.CurrentLevel, a => a.CategorySDTOList);

                if (!dics.ContainsKey(1))
                {
                    dics.Add(1, new List<CategoryS2DTO>());
                }

                if (dics.ContainsKey(1))
                {
                    if (!dics.ContainsKey(2))
                    {
                        dics.Add(2, new List<CategoryS2DTO>());
                    }
                    if (!dics.ContainsKey(3))
                    {
                        dics.Add(3, new List<CategoryS2DTO>());
                    }
                    dics[1].ToList().ForEach(
                        first =>
                        {
                            if (dics.ContainsKey(2))
                            {
                                //添加第一级分类下的二级分类
                                first.SecondCategory = (from second in dics[2].ToList()
                                                        where second.ParentId == first.Id
                                                        orderby second.Sort
                                                        select new SCategorySDTO
                                                        {
                                                            CurrentLevel = second.CurrentLevel,
                                                            Id = second.Id,
                                                            Name = second.Name,
                                                            ParentId = second.ParentId,
                                                            Sort = second.Sort,
                                                            Icno = second.Icno
                                                        }).ToList();
                                if (dics.ContainsKey(3))
                                {
                                    //添加第二级分类下的三级分类
                                    first.SecondCategory.ForEach(
                                        second =>
                                        {
                                            second.ThirdCategory =
                                                (from third in dics[3].ToList()
                                                 where third.ParentId == second.Id
                                                 orderby third.Sort
                                                 select new TCategorySDTO
                                                 {
                                                     CurrentLevel = third.CurrentLevel,
                                                     Id = third.Id,
                                                     Name = third.Name,
                                                     ParentId = third.ParentId,
                                                     Sort = third.Sort,
                                                     Icno = third.Icno
                                                 }).ToList();
                                        });
                                }
                            }
                        });

                    JAP.Cache.GlobalCacheWrapper.AddCache(string.Format("G_Category2Info_{0}", appId), dics[1].ToList(), 60, "BTPCache");
                    return dics[1].ToList();
                }
            }
            return new List<CategoryS2DTO>();
        }

        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <param name="levelCount">分类级别</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetCategoryByDrawerExt(System.Guid appId, out int levelCount)
        {
            //所有的类目列表
            var listInnerBrand = CategoryInnerBrand.ObjectSet();
            var listBrandList = Brandwall.ObjectSet();
            var listCategoryList = CategoryAdvertise.ObjectSet();
            List<CategoryS2DTO> categorylist;
            levelCount = 0;

            //获取类目信息
            var category = Category.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false && (bool)n.IsUse).OrderBy(n => n.Sort);
            var query = from n in category
                        where n.CurrentLevel > 0
                        select new CategoryS2DTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort,
                            Icno = n.icon
                        };
            categorylist = query.ToList();
            if (categorylist.Count > 0)
            {
                levelCount = categorylist.Max(t => t.CurrentLevel);
                if (levelCount > 3)
                {
                    //当前级别最高为3级 过滤历史数据
                    levelCount = 3;
                }
            }

            //根据级别分类，获取四个级别对应的数据字典
            var dics = categorylist.GroupBy(
                a => a.CurrentLevel,
                (key, group) => new { CurrentLevel = key, CategorySDTOList = group }).
                ToDictionary(a => a.CurrentLevel, a => a.CategorySDTOList);

            if (!dics.ContainsKey(1))
            {
                dics.Add(1, new List<CategoryS2DTO>());
            }

            if (dics.ContainsKey(1))
            {
                if (!dics.ContainsKey(2))
                {
                    dics.Add(2, new List<CategoryS2DTO>());
                }
                if (!dics.ContainsKey(3))
                {
                    dics.Add(3, new List<CategoryS2DTO>());
                }
                dics[1].ToList().ForEach(
                    first =>
                    {
                        if (dics.ContainsKey(2))
                        {
                            //添加第一级分类下的二级分类
                            first.SecondCategory = (from second in dics[2].ToList()
                                                    where second.ParentId == first.Id
                                                    orderby second.Sort
                                                    select new SCategorySDTO
                                                    {
                                                        CurrentLevel = second.CurrentLevel,
                                                        Id = second.Id,
                                                        Name = second.Name,
                                                        ParentId = second.ParentId,
                                                        Sort = second.Sort,
                                                        Icno = second.Icno
                                                    }).ToList();

                            var listBrandWall = from n in listInnerBrand.Where(o => o.CategoryId == first.Id) select n.BrandId;
                            var listBrand = from n in listBrandList.Where(o => listBrandWall.Contains(o.Id) && o.Brandstatu == 1)
                                            select new BrandwallDTO()
                                            {
                                                Id = n.Id,
                                                BrandLogo = n.BrandLogo,
                                                Brandname = n.Brandname,
                                                Brandstatu = n.Brandstatu,
                                                AppId = n.AppId,
                                            }; //添加品牌墙

                            first.BrandWallDto = listBrand.ToList();

                            if (listCategoryList.Count() > 0)
                            {
                                var nowDate = DateTime.Now;
                                var defaultCategoryAdvertise = listCategoryList.FirstOrDefault(o => o.CategoryId == first.Id && o.PutTime <= nowDate && o.PushTime >= nowDate);
                                if (defaultCategoryAdvertise != null && defaultCategoryAdvertise.Id != Guid.Empty)
                                {
                                    first.CategoryAdvertise = defaultCategoryAdvertise.ToEntityData();
                                }
                            }

                            //添加品类广告

                            if (dics.ContainsKey(3))
                            {
                                //添加第二级分类下的三级分类
                                first.SecondCategory.ForEach(
                                    second =>
                                    {
                                        //List<TCategorySDTO> tCategorySdtos = new List<TCategorySDTO>();
                                        //var a = (from third in dics[3].ToList()
                                        //        where third.ParentId == second.Id
                                        //        orderby third.Sort
                                        //        select new TCategorySDTO
                                        //        {
                                        //            CurrentLevel = third.CurrentLevel,
                                        //            Id = third.Id,
                                        //            Name = third.Name,
                                        //            ParentId = third.ParentId,
                                        //            Sort = third.Sort,
                                        //            Icno = third.Icno
                                        //        }).ToList();
                                        //foreach (var categorySdto in a)
                                        //{
                                        //    var commodityCategorys = from cc in CommodityCategory.ObjectSet()
                                        //        join c in Commodity.ObjectSet() on cc.CommodityId equals c.Id
                                        //        where cc.CategoryId == categorySdto.Id && !(bool) cc.IsDel && !c.IsDel && c.State == 0
                                        //        select cc;
                                        //    if (commodityCategorys.Any())
                                        //    {
                                        //        tCategorySdtos.Add(categorySdto);
                                        //    }
                                        //}
                                        //second.ThirdCategory = tCategorySdtos;
                                        second.ThirdCategory =
                                            (from third in dics[3].ToList()
                                             where third.ParentId == second.Id
                                             orderby third.Sort
                                             select new TCategorySDTO
                                             {
                                                 CurrentLevel = third.CurrentLevel,
                                                 Id = third.Id,
                                                 Name = third.Name,
                                                 ParentId = third.ParentId,
                                                 Sort = third.Sort,
                                                 Icno = third.Icno
                                             }).ToList();
                                    });
                            }
                        }
                    });

                return dics[1].ToList();
            }
            return new List<CategoryS2DTO>();
        }

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="commodityListInfer"></param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityListExt(CommodityListInferSearchDTO commodityListInfer)
        {
            List<CommodityListCDTO> commodityListCdtos = new List<CommodityListCDTO>();
            var newResult = GetCommodityListV2Ext(commodityListInfer);
            commodityListCdtos = newResult.comdtyList;
            return commodityListCdtos;
        }

        public CommodityNewSearchResultDTO GetNewCommodityListExt(CommodityListInferSearchDTO commodityListInfer)
        {
            var dtoResult = new CommodityNewSearchResultDTO();
            var list = GetCommodityListExt(commodityListInfer);
            dtoResult.CommodityList = list;

            //添加品牌墙 BrandWallList
            //添加搜索类  CategoryList
            //添加店铺信息 AppInfoList
            return dtoResult;
        }


        /// <summary>
        /// 校验app是否显示search菜单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<bool> CheckIsShowSearchMenuExt(CategorySearchDTO search)
        {
            ResultDTO<bool> result = new ResultDTO<bool>() { Data = true };
            if (search == null || search.AppId == Guid.Empty)
                return new ResultDTO<bool>() { ResultCode = -1, Data = false, Message = "入参为空" };
            var appExt = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == search.AppId);
            if (appExt == null || !appExt.IsShowSearchMenu)
                return new ResultDTO<bool>() { ResultCode = 0, Data = false };
            return result;
        }

        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        public List<CategoryCache2DTO> GetCacheCateGoriesExt(Guid appid)
        {
            List<CategoryCache2DTO> cate = new List<CategoryCache2DTO>();
            var temp = Category.ObjectSet()
                    .Where(a => a.AppId == appid && a.IsDel == false && (bool)a.IsUse)
                    .OrderBy(a => a.Sort)
                    .Select(a => new CategoryCache2DTO
                    {
                        AppId = a.AppId,
                        CurrentLevel = a.CurrentLevel,
                        Id = a.Id,
                        Name = a.Name,
                        ParentId = a.ParentId,
                        icno = a.icon,
                        Sort = a.Sort
                    }).ToList();

            foreach (var categoryCache2Dto in temp)
            {
                //判断三级分类下是否有商品 不存在再不展示该分类
                if (categoryCache2Dto.CurrentLevel == 3)
                {
                    var commodityCategorys = from cc in CommodityCategory.ObjectSet()
                                             join c in Commodity.ObjectSet() on cc.CommodityId equals c.Id
                                             where cc.CategoryId == categoryCache2Dto.Id && !(bool)cc.IsDel && !c.IsDel && c.State == 0
                                             select cc;
                    if (commodityCategorys.Any())
                    {
                        cate.Add(categoryCache2Dto);
                    }
                }
                else
                {
                    cate.Add(categoryCache2Dto);
                }
            }

            cate = Category.ObjectSet().Where(a => a.AppId == appid && a.IsDel == false && (bool)a.IsUse).OrderBy(a => a.Sort).Select(a => new CategoryCache2DTO
            {
                AppId = a.AppId,
                CurrentLevel = a.CurrentLevel,
                Id = a.Id,
                Name = a.Name,
                ParentId = a.ParentId,
                icno = a.icon,
                Sort = a.Sort
            }).ToList();
            return cate;
        }

        /// <summary>
        /// 删除指定“电商馆”下applist下的商品分类关系
        /// </summary>
        /// <param name="belongTo">电商馆APPId</param>
        /// <param name="appList">applist</param>
        /// <returns></returns>
        public ResultDTO DeleteCommodityCategoryExt(Guid belongTo, List<Guid> appList)
        {
            var temp = from scc in CommodityCategory.ObjectSet()
                       join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       where sc.AppId == belongTo && appList.Contains(c.AppId) && c.CommodityType == 0
                       select scc;

            ResultDTO resultDto = new ResultDTO { Message = "删除成功", ResultCode = 0 };
            if (!temp.Any())
            {
                return resultDto;
            }
            foreach (CommodityCategory item in temp.ToList())
            {
                item.EntityState = EntityState.Deleted;
                ContextFactory.CurrentThreadContext.Delete(item);
            }
            try
            {
                int i = ContextFactory.CurrentThreadContext.SaveChanges();
                if (i > 0)
                {
                    JAP.Common.Loging.LogHelper.Debug("DeleteCommodityCategoryExt resultDto:" + resultDto.Message + ";code:" + resultDto.ResultCode);
                    return resultDto;
                }
                resultDto.ResultCode = 1;
                resultDto.Message = "删除失败";
            }
            catch (Exception ex)
            {
                resultDto.ResultCode = 1;
                resultDto.Message = "删除失败";
            }
            JAP.Common.Loging.LogHelper.Debug("DeleteCommodityCategoryExt resultDto:" + resultDto.Message + ";code:" + resultDto.ResultCode);
            return resultDto;
        }

        /// <summary>
        /// 分页获取电商馆下商品
        /// </summary>
        /// <param name="comdtySearch4SelCdto"></param>
        /// <param name="comdtyCount">查询到的商品数量</param>
        /// <returns></returns>
        public List<CommodityListCDTO> GetCommodityLisByBeLongToExt(ComdtySearch4SelCDTO comdtySearch4SelCdto, out int comdtyCount)
        {
            comdtyCount = 0;
            QueryPavilionAppParam query = new QueryPavilionAppParam
            {
                Id = comdtySearch4SelCdto.belongTo,
                pageIndex = 1,
                pageSize = int.MaxValue
            };
            var eReturnInfo = ZPHSV.Instance.GetPavilionApp(query);
            JAP.Common.Loging.LogHelper.Debug("GetCommodityLisByBeLongTotExt eReturnInfo belongTo:" + query.Id);
            if (eReturnInfo == null || eReturnInfo.Data == null || !eReturnInfo.Data.Any())
                return new List<CommodityListCDTO>();
            //AppId
            if (comdtySearch4SelCdto.AppId != null)
            {
                eReturnInfo.Data = eReturnInfo.Data.Where(c => c.appId == comdtySearch4SelCdto.AppId.Value).ToList();
            }
            //AppName 模糊查询
            if (!string.IsNullOrEmpty(comdtySearch4SelCdto.AppName))
            {
                eReturnInfo.Data = eReturnInfo.Data.Where(o => o.appName.Contains(comdtySearch4SelCdto.AppName)).ToList();
            }
            if (!eReturnInfo.Data.Any())
                return new List<CommodityListCDTO>();

            var ab = eReturnInfo.Data.Select(t => t.appId).ToList();

            var temp = from c in Commodity.ObjectSet()
                       where c.IsDel == false && c.State == 0 && ab.Contains(c.AppId) && c.CommodityType == 0
                       orderby c.Name
                       select new BTP.Deploy.CustomDTO.CommodityListCDTO
                       {
                           Id = c.Id,
                           Name = c.Name,
                           Pic = c.PicturesPath,
                           Price = c.Price,
                           AppId = c.AppId,
                           Stock = c.Stock,
                           IsEnableSelfTake = c.IsEnableSelfTake,
                           MarketPrice = c.MarketPrice ?? 0,
                           State = c.State,
                           DiscountPrice = -1,
                           Intensity = 10,
                           LimitBuyEach = -1,
                           LimitBuyTotal = -1,
                           SurplusLimitBuyTotal = 0,
                           ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                       };

            //CommodityName 模糊查询
            if (!string.IsNullOrEmpty(comdtySearch4SelCdto.CommodityName))
            {
                temp = temp.Where(p => p.Name.Contains(comdtySearch4SelCdto.CommodityName));
            }
            //选中促销中商品 按照促销时间进行过滤
            if (comdtySearch4SelCdto.IsChkTime == 0)
            {
                var commodityIds = from item in PromotionItems.ObjectSet()
                                   join pro in Promotion.ObjectSet() on item.PromotionId equals pro.Id
                                   where
                                       !pro.IsDel && pro.PromotionType == 0 && pro.AppId == comdtySearch4SelCdto.belongTo &&
                                       pro.EndTime >= comdtySearch4SelCdto.beginTime &&
                                       (pro.StartTime <= comdtySearch4SelCdto.endTime || pro.PresellStartTime <= comdtySearch4SelCdto.endTime) && pro.PromotionType != 3
                                   select item.CommodityId;
                temp = temp.Where(p => commodityIds.Contains(p.Id));
            }

            comdtyCount = temp.Count();
            if (comdtyCount == 0)
                return new List<CommodityListCDTO>();

            var comdtyList4SelCdtos = temp.OrderBy(t => t.AppId).Skip((comdtySearch4SelCdto.pageIndex - 1) * comdtySearch4SelCdto.pageSize).Take(comdtySearch4SelCdto.pageSize).ToList();

            DateTime now = DateTime.Now;
            var comIds = comdtyList4SelCdtos.Select(c => c.Id).Distinct().ToList();
            var todayPromotons = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
            foreach (var commodity in comdtyList4SelCdtos)
            {
                var todayPromotion = todayPromotons.FirstOrDefault(c => c.CommodityId == commodity.Id);
                if (todayPromotion != null)
                {
                    commodity.LimitBuyEach = todayPromotion.LimitBuyEach;
                    commodity.LimitBuyTotal = todayPromotion.LimitBuyTotal;
                    commodity.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal;
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
                var app = eReturnInfo.Data.FirstOrDefault(c => c.appId == commodity.AppId);
                if (app != null)
                {
                    commodity.AppName = app.appName;
                }
            }
            return comdtyList4SelCdtos;
        }


        /// <summary>
        /// 获取应用的一级商品分类
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.CategorySV.svc/GetCategoryL1
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public List<CategorySDTO> GetCategoryL1Ext(Guid appId)
        {
            //Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CategoryInfo", appId.ToString(), "BTPCache");
            //读取缓存
            List<CategorySDTO> storeCache = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_CategoryInfo", appId.ToString(), "BTPCache") as List<CategorySDTO>;
            if (storeCache != null && storeCache.Count > 0)
            {
                storeCache = storeCache.Where(n => n.CurrentLevel == 1).ToList();
                return storeCache;
            }
            //获取类目信息
            var category = Category.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false && n.CurrentLevel == 1).OrderBy(n => n.Sort);
            var query = from n in category
                        select new CategorySDTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort
                        };
            List<CategorySDTO> categorylist = query.ToList<CategorySDTO>();
            if (categorylist == null)
            {
                categorylist = new List<CategorySDTO>();
            }

            Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_CategoryInfo", appId.ToString(), categorylist, "BTPCache");
            return categorylist;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2Ext(CommodityListInferSearchDTO commodityListInfer)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();

            if (commodityListInfer == null)
            {
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = 1;
                comdtyListResultCDTO.Message = "参数不能为空";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            List<Guid> appIdSelect;
            if (commodityListInfer.MallAppType > 0)  //商家类型检索
            {
                var typeList = commodityListInfer.MallAppType == 1 ? new List<int> { 1 } : new List<int> { 0, 2, 3 };
                LogHelper.Info("手机端按商家类型查询商品: typeList" + JsonHelper.JsonSerializer(typeList) + "，DateTime: " + DateTime.Now);
                appIdSelect = (from scc in CommodityCategory.ObjectSet()
                               join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                               join m in MallApply.ObjectSet() on c.AppId equals m.AppId
                               where sc.Id == commodityListInfer.CategoryId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                    && m.EsAppId == commodityListInfer.AppId && typeList.Contains(m.Type) && m.State.Value == (int)MallApplyEnum.TG
                               select c.AppId).ToList();
            }
            else
            {
                appIdSelect = (from scc in CommodityCategory.ObjectSet()
                               join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                               where sc.Id == commodityListInfer.CategoryId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                               select c.AppId).ToList();
            }

            if (!appIdSelect.Any())
            {
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = 1;
                comdtyListResultCDTO.Message = "找不到数据";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }


            var appIds = appIdSelect.Distinct().ToList();
            var applist = APPSV.GetAppListByIds(appIds, null);   //店铺信息筛选

            var temp = from scc in CommodityCategory.ObjectSet()
                       join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       where sc.Id == commodityListInfer.CategoryId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0 && appIds.Contains(c.AppId)
                       orderby scc.MaxSort
                       select new AppSetCommodity2DTO
                       {
                           AppId = c.AppId,
                           CommodityId = c.Id,
                           CommodityName = c.Name,
                           CommodityPic = c.PicturesPath,
                           CommodityPrice = c.Price,
                           CommodityStock = c.Stock,
                           IsEnableSelfTake = c.IsEnableSelfTake,
                           SetCategorySort = scc.MaxSort ?? 0,
                           SaleAreas = c.SaleAreas,
                           Salesvolume = c.Salesvolume,
                           MarketPrice = c.MarketPrice,
                           State = c.State,
                           ComAttribute = c.ComAttribute
                       };

            //ByAppId
            if (commodityListInfer.ByAppId != Guid.Empty)
            {
                temp = temp.Where(p => p.AppId == commodityListInfer.ByAppId);
            }

            if (commodityListInfer.MinPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice >= commodityListInfer.MinPrice);
            }
            if (commodityListInfer.MaxPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice <= commodityListInfer.MaxPrice);
            }

            //IsHasStock
            if (commodityListInfer.IsHasStock)
            {
                temp = temp.Where(p => p.CommodityStock > 0);
            }
            //areaCode
            if (!ProvinceCityHelper.IsTheWholeCountry(commodityListInfer.areaCode))
            {
                var areaName = ProvinceCityHelper.GetAreaNameByCode(commodityListInfer.areaCode);
                var province = ProvinceCityHelper.GetProvinceByAreaCode(commodityListInfer.areaCode);
                var city = ProvinceCityHelper.GetCityByAreaCode(commodityListInfer.areaCode);
                if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 2;
                    comdtyListResultCDTO.Message = "区域信息错误";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }
                if (province.AreaCode == city.AreaCode)
                {
                    string provinceCode = province.AreaCode ?? "";
                    temp = temp.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode));
                }
                else
                {
                    temp = temp.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                }
            }


            switch (commodityListInfer.FieldSort)
            {
                case FieldSort4Mobile.Sales:
                    temp = temp.OrderByDescending(t => t.Salesvolume).ThenBy(t => t.SetCategorySort);
                    break;
                case FieldSort4Mobile.Price:
                    switch (commodityListInfer.Order)
                    {
                        case OrderType.ASC:
                            temp = temp.OrderBy(t => t.CommodityPrice).ThenBy(t => t.SetCategorySort);
                            break;
                        default:
                            temp = temp.OrderByDescending(t => t.CommodityPrice).ThenBy(t => t.SetCategorySort);
                            break;
                    }
                    break;
                default:
                    temp = temp.OrderBy(t => t.SetCategorySort);
                    break;
            }
            comdtyListResultCDTO.realCount = temp.Count();


            temp = temp.Skip((commodityListInfer.PageIndex - 1) * commodityListInfer.PageSize).Take(commodityListInfer.PageSize);

            var tempList = temp.ToList();
            var comIds = tempList.Select(c => c.CommodityId).Distinct().ToList();
            var promotionList = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);

            List<CommodityListCDTO> commodityListCdtos = new List<CommodityListCDTO>();
            foreach (var appSetCommodityDto in tempList)
            {
                CommodityListCDTO commodityListCdto = new CommodityListCDTO
                {
                    Id = appSetCommodityDto.CommodityId,
                    AppId = appSetCommodityDto.AppId,
                    IsEnableSelfTake = appSetCommodityDto.IsEnableSelfTake,
                    Name = appSetCommodityDto.CommodityName,
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
                    SetCategorySort = appSetCommodityDto.SetCategorySort,
                    ComAttrType = (appSetCommodityDto.ComAttribute == "[]" || appSetCommodityDto.ComAttribute == null) ? 1 : 3


                };

                #region 规格设置集合
                List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                if (commoditySpecification.Count() > 0)
                {
                    Guid commodityId = commodityListCdto.Id;
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
                commodityListCdto.Specifications = Specificationslist;
                #endregion

                var todayPromotion = promotionList.FirstOrDefault(c => c.CommodityId == appSetCommodityDto.CommodityId && c.PromotionType != 3);
                if (todayPromotion != null)
                {
                    commodityListCdto.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                    commodityListCdto.Intensity = Convert.ToDecimal(todayPromotion.Intensity);
                    commodityListCdto.LimitBuyEach = Convert.ToInt32(todayPromotion.LimitBuyEach);
                    commodityListCdto.LimitBuyTotal = Convert.ToInt32(todayPromotion.LimitBuyTotal);
                    commodityListCdto.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? Convert.ToInt32(todayPromotion.SurplusLimitBuyTotal);
                    commodityListCdto.PromotionType = todayPromotion.PromotionType;

                    //获取活动sku价格
                    if (todayPromotion.OutsideId != null)
                    {
                        var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)todayPromotion.OutsideId).Where(t => t.IsJoin && t.CommodityId == appSetCommodityDto.CommodityId);
                        if (skuActivityList.Any())
                        {
                            commodityListCdto.DiscountPrice = skuActivityList.Min(t => t.JoinPrice);
                            if (todayPromotion.Intensity < 10)
                            {
                                commodityListCdto.DiscountPrice = -1;
                            }
                        }
                    }
                }
                else
                {
                    commodityListCdto.PromotionType = 9999;
                }
                commodityListCdtos.Add(commodityListCdto);
            }
            if (applist != null && applist.Any())
            {
                comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                foreach (var appIdNameIconDTO in applist)
                {
                    comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO
                    {
                        appId = appIdNameIconDTO.AppId,
                        icon = appIdNameIconDTO.AppIcon,
                        appName = appIdNameIconDTO.AppName
                    });
                }
            }

            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            comdtyListResultCDTO.comdtyList = commodityListCdtos./*OrderByDescending(p=>p.SetCategorySort).*/ToList();
            return comdtyListResultCDTO;
        }



        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterListExt(CommodityListInferSearchDTO commodityListInfer)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();

            if (commodityListInfer == null)
            {
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = 1;
                comdtyListResultCDTO.Message = "参数不能为空";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            List<Guid> appIdSelect;

            if (commodityListInfer.MallAppType > 0)
            {
                var typeList = commodityListInfer.MallAppType == 1 ? new List<int> { 1 } : new List<int> { 0, 2, 3 };
                LogHelper.Info("手机端按商家类型查询商品: typeList" + JsonHelper.JsonSerializer(typeList) + "，DateTime: " + DateTime.Now);
                //appIdSelect = (from scc in CommodityCategory.ObjectSet()
                //               join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                //               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                //               join m in MallApply.ObjectSet() on c.AppId equals m.AppId
                //               where sc.Id == commodityListInfer.CategoryId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                //                    && m.EsAppId == commodityListInfer.AppId && typeList.Contains(m.Type) && m.State.Value == (int)MallApplyEnum.TG
                //               select c.AppId).ToList();
                appIdSelect = (from scc in CommodityInnerBrand.ObjectSet()
                               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                               join m in MallApply.ObjectSet() on c.AppId equals m.AppId
                               where scc.BrandId == commodityListInfer.BrandId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                    && m.EsAppId == commodityListInfer.AppId && typeList.Contains(m.Type) && m.State.Value == (int)MallApplyEnum.TG
                               select c.AppId).ToList();
            }
            else
            {
                //appIdSelect = (from scc in CommodityCategory.ObjectSet()
                //               join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                //               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                //               where sc.Id == commodityListInfer.CategoryId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                //               select c.AppId).ToList();
                appIdSelect = (from scc in CommodityInnerBrand.ObjectSet()
                               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                               where scc.BrandId == commodityListInfer.BrandId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                               select c.AppId).ToList();
            }
            if (!appIdSelect.Any())
            {
                comdtyListResultCDTO.isSuccess = false;
                comdtyListResultCDTO.Code = 1;
                comdtyListResultCDTO.Message = "找不到数据";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }
            var appIds = appIdSelect.Distinct().ToList();

            LogHelper.Debug("appIds的数量：" + appIds.Count + "appIds:" + appIds[0]);
            LogHelper.Debug("GetAppListByIds执行之前");
            var applist = APPSV.GetAppListByIds(appIds, null);
            LogHelper.Debug("GetAppListByIds执行之后");
            var temp = from scc in CommodityInnerBrand.ObjectSet()
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       where scc.BrandId == commodityListInfer.BrandId && scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0 && appIds.Contains(c.AppId)
                       orderby scc.MaxSort
                       select new AppSetCommodity2DTO
                       {
                           AppId = c.AppId,
                           CommodityId = c.Id,
                           CommodityName = c.Name,
                           CommodityPic = c.PicturesPath,
                           CommodityPrice = c.Price,
                           CommodityStock = c.Stock,
                           IsEnableSelfTake = c.IsEnableSelfTake,
                           SetCategorySort = scc.MaxSort ?? 0,
                           SaleAreas = c.SaleAreas,
                           Salesvolume = c.Salesvolume,
                           MarketPrice = c.MarketPrice,
                           State = c.State,
                           ComAttribute = c.ComAttribute,
                           YouKaPercent = c.YoukaPercent
                       };
            //ByAppId
            if (commodityListInfer.ByAppId != Guid.Empty)
            {
                temp = temp.Where(p => p.AppId == commodityListInfer.ByAppId);
            }

            if (commodityListInfer.MinPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice >= commodityListInfer.MinPrice);
            }
            if (commodityListInfer.MaxPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice <= commodityListInfer.MaxPrice);
            }

            //IsHasStock
            if (commodityListInfer.IsHasStock)
            {
                temp = temp.Where(p => p.CommodityStock > 0);
            }
            //areaCode
            if (!ProvinceCityHelper.IsTheWholeCountry(commodityListInfer.areaCode))
            {
                var areaName = ProvinceCityHelper.GetAreaNameByCode(commodityListInfer.areaCode);
                var province = ProvinceCityHelper.GetProvinceByAreaCode(commodityListInfer.areaCode);
                var city = ProvinceCityHelper.GetCityByAreaCode(commodityListInfer.areaCode);
                if (string.IsNullOrEmpty(areaName) || province == null || city == null)
                {
                    comdtyListResultCDTO.isSuccess = false;
                    comdtyListResultCDTO.Code = 2;
                    comdtyListResultCDTO.Message = "区域信息错误";
                    comdtyListResultCDTO.realCount = 0;
                    comdtyListResultCDTO.comdtyList = null;
                    comdtyListResultCDTO.appInfoList = null;
                    return comdtyListResultCDTO;
                }
                if (province.AreaCode == city.AreaCode)
                {
                    string provinceCode = province.AreaCode ?? "";
                    temp = temp.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(provinceCode));
                }
                else
                {
                    temp = temp.Where(c => c.SaleAreas == null || c.SaleAreas == "" || c.SaleAreas == ProvinceCityHelper.CountryCode || c.SaleAreas.Contains(province.AreaCode) || c.SaleAreas.Contains(city.AreaCode));
                }
            }
            switch (commodityListInfer.FieldSort)
            {
                case FieldSort4Mobile.Sales:
                    temp = temp.OrderByDescending(t => t.Salesvolume).ThenBy(t => t.SetCategorySort);
                    break;
                case FieldSort4Mobile.Price:
                    switch (commodityListInfer.Order)
                    {
                        case OrderType.ASC:
                            temp = temp.OrderBy(t => t.CommodityPrice).ThenBy(t => t.SetCategorySort);
                            break;
                        default:
                            temp = temp.OrderByDescending(t => t.CommodityPrice).ThenBy(t => t.SetCategorySort);
                            break;
                    }
                    break;
                default:
                    temp = temp.OrderBy(t => t.SetCategorySort);
                    break;
            }
            comdtyListResultCDTO.realCount = temp.Count();
            temp = temp.Skip((commodityListInfer.PageIndex - 1) * commodityListInfer.PageSize).Take(commodityListInfer.PageSize);

            var tempList = temp.ToList();
            var comIds = tempList.Select(c => c.CommodityId).Distinct().ToList();
            var promotionList = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);

            List<CommodityListCDTO> commodityListCdtos = new List<CommodityListCDTO>();
            foreach (var appSetCommodityDto in tempList)
            {
                CommodityListCDTO commodityListCdto = new CommodityListCDTO
                {
                    Id = appSetCommodityDto.CommodityId,
                    AppId = appSetCommodityDto.AppId,
                    IsEnableSelfTake = appSetCommodityDto.IsEnableSelfTake,
                    Name = appSetCommodityDto.CommodityName,
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
                    SetCategorySort = appSetCommodityDto.SetCategorySort,
                    ComAttrType = (appSetCommodityDto.ComAttribute == "[]" || appSetCommodityDto.ComAttribute == null) ? 1 : 3


                };

                #region 规格设置集合
                List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specificationslist = new List<Deploy.CustomDTO.SpecificationsDTO>();
                var commoditySpecification = CommoditySpecifications.ObjectSet().AsQueryable();
                if (commoditySpecification.Count() > 0)
                {
                    Guid commodityId = commodityListCdto.Id;
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
                commodityListCdto.Specifications = Specificationslist;
                #endregion

                var todayPromotion = promotionList.FirstOrDefault(c => c.CommodityId == appSetCommodityDto.CommodityId && c.PromotionType != 3);
                if (todayPromotion != null)
                {
                    commodityListCdto.DiscountPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                    commodityListCdto.Intensity = Convert.ToDecimal(todayPromotion.Intensity);
                    commodityListCdto.LimitBuyEach = Convert.ToInt32(todayPromotion.LimitBuyEach);
                    commodityListCdto.LimitBuyTotal = Convert.ToInt32(todayPromotion.LimitBuyTotal);
                    commodityListCdto.SurplusLimitBuyTotal = todayPromotion.SurplusLimitBuyTotal ?? Convert.ToInt32(todayPromotion.SurplusLimitBuyTotal);
                    commodityListCdto.PromotionType = todayPromotion.PromotionType;

                    //获取活动sku价格
                    if (todayPromotion.OutsideId != null)
                    {
                        var skuActivityList = ZPHSV.Instance.GetSkuActivityList((Guid)todayPromotion.OutsideId).Where(t => t.IsJoin && t.CommodityId == appSetCommodityDto.CommodityId);
                        if (skuActivityList.Any())
                        {
                            commodityListCdto.DiscountPrice = skuActivityList.Min(t => t.JoinPrice);
                            if (todayPromotion.Intensity < 10)
                            {
                                commodityListCdto.DiscountPrice = -1;
                            }
                        }
                    }
                }
                else
                {
                    commodityListCdto.PromotionType = 9999;
                }
                commodityListCdtos.Add(commodityListCdto);
            }
            LogHelper.Debug("applist的数量：" + applist.Count + "applist:" + applist[0]);
            if (applist != null && applist.Any())
            {
                comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();
                foreach (var appIdNameIconDTO in applist)
                {
                    comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO
                    {
                        appId = appIdNameIconDTO.AppId,
                        icon = appIdNameIconDTO.AppIcon,
                        appName = appIdNameIconDTO.AppName
                    });
                }
            }

            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            comdtyListResultCDTO.comdtyList = commodityListCdtos./*OrderByDescending(p=>p.SetCategorySort).*/ToList();
            return comdtyListResultCDTO;

        }

        /// <summary>
        /// 商品列表筛选结果
        /// </summary>
        /// <param name="commodityListInfer"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterListSecondExt(CommodityListInferSearchDTO commodityListInfer)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO comdtyListResultCDTO = new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO();

            if (commodityListInfer == null)
            {
                comdtyListResultCDTO.Code = 1;
                comdtyListResultCDTO.Message = "参数不能为空";
                comdtyListResultCDTO.realCount = 0;
                comdtyListResultCDTO.comdtyList = null;
                comdtyListResultCDTO.appInfoList = null;
                return comdtyListResultCDTO;
            }

            if (commodityListInfer.PageSize <= 0)
            {
                commodityListInfer.PageSize = 20;
            }

            var temp = from scc in CommodityCategory.ObjectSet()
                       join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       where scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                       orderby scc.MaxSort
                       select new
                       {
                           esAppId = sc.AppId,
                           AppId = c.AppId,
                           CommodityId = c.Id,
                           appId = c.AppId,
                           CommodityName = c.Name,
                           CommodityPic = c.PicturesPath,
                           CommodityPrice = c.Price,
                           CommodityStock = c.Stock,
                           IsEnableSelfTake = c.IsEnableSelfTake,
                           SetCategorySort = scc.MaxSort ?? 0,
                           MarketPrice = c.MarketPrice,
                           State = c.State,
                           Salesvolume = c.Salesvolume,
                           ComAttribute = c.ComAttribute,
                           CategoryID = scc.CategoryId,
                           OrderWeight = c.OrderWeight ?? 0,
                           YouKaPercent = c.YoukaPercent,
                       };

            var appIdSelect = new List<Guid>();
            if (commodityListInfer.MallAppType > 0)
            {
                var typeList = commodityListInfer.MallAppType == 1 ? new List<int> { 1 } : new List<int> { 0, 2, 3 };
                LogHelper.Info("手机端按商家类型查询商品: typeList" + JsonHelper.JsonSerializer(typeList) + "，DateTime: " + DateTime.Now);

                appIdSelect = (from scc in CommodityCategory.ObjectSet()
                               join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                               join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                               join m in MallApply.ObjectSet() on c.AppId equals m.AppId
                               where scc.IsDel == false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                                    && m.EsAppId == commodityListInfer.AppId && typeList.Contains(m.Type) && m.State.Value == (int)MallApplyEnum.TG
                               select c.AppId).ToList();
            }


            if (appIdSelect.Any())
            {
                temp = temp.Where(o => appIdSelect.Contains(o.AppId));
            }

            if (commodityListInfer.IsHasStock)
            {
                temp = temp.Where(p => p.CommodityStock > 0);
            }

            //if (commodityListInfer.ByAppId != Guid.Empty)
            //{
            //    temp = temp.Where(p => p.AppId == commodityListInfer.ByAppId);
            //}

            if (commodityListInfer.MinPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice >= commodityListInfer.MinPrice);
            }
            if (commodityListInfer.MaxPrice > 0)
            {
                temp = temp.Where(p => p.CommodityPrice <= commodityListInfer.MaxPrice);
            }

            //多筛选开始

            if (commodityListInfer.BrandId != null && commodityListInfer.BrandId != Guid.Empty)  //筛选品牌
            {
                var brandList = from n in CommodityInnerBrand.ObjectSet().Where(o => o.BrandId == commodityListInfer.BrandId && (o.IsDel == null || o.IsDel == false)) select n.CommodityId;
                temp = temp.Where(o => brandList.Contains(o.CommodityId));
            }

            if (commodityListInfer.BrandIdList != null && commodityListInfer.BrandIdList.Any())
            {
                var brandList = from n in CommodityInnerBrand.ObjectSet().Where(o => commodityListInfer.BrandIdList.Contains(o.BrandId) && (o.IsDel == null || o.IsDel == false)) select n.CommodityId;
                temp = temp.Where(o => brandList.Contains(o.CommodityId));
            }

            if (commodityListInfer.CategoryId != null && commodityListInfer.CategoryId != Guid.Empty) //筛选分类
            {
                temp = temp.Where(o => o.CategoryID == commodityListInfer.CategoryId);
            }

            if (commodityListInfer.CategoryIdList != null && commodityListInfer.CategoryIdList.Any())
            {
                if (commodityListInfer.CategoryId == null || commodityListInfer.CategoryId == Guid.Empty)
                {
                    var cate0 = Category.ObjectSet().Where(o => o.Id == commodityListInfer.CategoryIdList[0]).FirstOrDefault();
                    if (cate0.CurrentLevel == 1)
                    {
                        var secondList = Category.ObjectSet().Where(o => o.ParentId == commodityListInfer.CategoryIdList[0]).Select(o => o.Id);
                        var thirdLiST = Category.ObjectSet().Where(O => secondList.Contains((Guid)O.ParentId)).Select(O => O.Id);
                        temp = temp.Where(o => thirdLiST.Contains(o.CategoryID));
                    }
                }
            }

            if (commodityListInfer.AppId != null && commodityListInfer.AppId != Guid.Empty)
            {
                temp = temp.Where(o => o.esAppId == commodityListInfer.AppId);
            }

            if (commodityListInfer.StoreIdList != null && commodityListInfer.StoreIdList.Any())
            {
                temp = temp.Where(o => commodityListInfer.StoreIdList.Contains(o.AppId));
            }
            comdtyListResultCDTO.realCount = temp.Count();

            var appIds = temp.Select(o => o.AppId).Distinct().ToList();               //appIdSelect.Distinct().ToList();
            var applist = APPSV.GetAppListByIds(appIds, null);

            LogHelper.Info(string.Format("调用GetCommodityFilterList返回商家数量{0}", JsonHelper.JsonSerializer(appIds)));

            try
            {
                if (commodityListInfer.PageIndex == 0)   //第一次筛选返回筛选条件
                {
                    //var appIDList = (from n in temp select n.AppId).Distinct();
                    //  applist = applist.Where(o => appIDList.Contains(o.AppId)).ToList();  //返回店铺筛选条件

                    var comodityIdList = temp.Select(o => o.CommodityId);
                    var brandList = from n in CommodityInnerBrand.ObjectSet().Where(o => comodityIdList.Contains(o.CommodityId)) select n.BrandId;

                    comdtyListResultCDTO.BrandWallList = (from n in Brandwall.ObjectSet().Where(O => brandList.Contains(O.Id))
                                                          select new BrandwallDTO()
                                                          {
                                                              AppId = n.AppId,
                                                              BrandLogo = n.BrandLogo,
                                                              Brandname = n.Brandname,
                                                              Id = n.Id
                                                          }).ToList();

                    //分类获取开始
                    var categoryList = (from n in temp select n.CategoryID).Distinct();

                    if (categoryList.Count() == 1)
                    {
                        var Thirdcategory = Category.ObjectSet().Where(o => o.Id == categoryList.FirstOrDefault() && o.IsDel == false && o.CurrentLevel == 3);  //判断是只有三级分类
                        if (Thirdcategory != null)
                        {
                            comdtyListResultCDTO.CategoryList = new List<CategoryDTO>();   //只有一项，不加载
                        }
                        else
                        {
                            var categorySecordList = Category.ObjectSet().Where(o => o.IsDel == false && o.CurrentLevel == 2 && categoryList.Contains(o.Id)).ToList();//获取对应二级分类

                            var categoryThirdList = from n in Category.ObjectSet().Where(o => o.IsDel == false && o.CurrentLevel == 3 && categoryList.Contains(o.Id)) select n;  //获取其中的三级分类

                            var categorySecondList2 = from n in categoryThirdList select n.ParentId;

                            categorySecordList.AddRange(Category.ObjectSet().Where(o => o.IsDel == false && categorySecondList2.Contains(o.Id)));  //二级分类合并

                            var firstList = Category.ObjectSet().Where(o => o.IsDel == false && o.CurrentLevel == 1 && categoryList.Contains(o.Id)).Select(O => O.Id).ToList();

                            var firstList2 = (from n in categorySecordList select (Guid)n.ParentId).Distinct().Where(o => o != Guid.Empty).ToList();

                            firstList.AddRange(firstList2);

                            if (firstList.Count() == 1)
                            {
                                //var tempFirstList = from n in firstList
                                //var firstCategory = from n in Category.ObjectSet().Where(o=> firstList.Contains(o.ParentId));

                                var firstID = firstList.FirstOrDefault();
                                var secordIDList = from n in Category.ObjectSet().Where(o => o.IsDel == false && o.ParentId == firstID && o.CurrentLevel == 2) select n.Id;

                                if (secordIDList.Any())
                                {
                                    var thirdIDList = from n in Category.ObjectSet().Where(o => o.IsDel == false && secordIDList.Contains((Guid)o.ParentId) && o.CurrentLevel == 3) select n.Id;

                                    if (thirdIDList.Count() == 1)
                                    {
                                        comdtyListResultCDTO.CategoryList = new List<CategoryDTO>();
                                    }
                                    else if (thirdIDList.Any())
                                    {
                                        comdtyListResultCDTO.CategoryList = (from n in Category.ObjectSet().Where(o => thirdIDList.Contains(o.Id))
                                                                             select new CategoryDTO()
                                                                             {
                                                                                 Id = n.Id,
                                                                                 Name = n.Name,
                                                                                 icon = n.icon,
                                                                                 AppId = n.AppId,
                                                                             }).ToList(); //获取三级类目
                                    }
                                    else
                                    {
                                        comdtyListResultCDTO.CategoryList = (from n in Category.ObjectSet().Where(o => secordIDList.Contains(o.Id))
                                                                             select new CategoryDTO()
                                                                             {
                                                                                 Id = n.Id,
                                                                                 Name = n.Name,
                                                                                 icon = n.icon,
                                                                                 AppId = n.AppId,
                                                                             }).ToList(); //获取二级类目
                                    }
                                }
                                else
                                {
                                    comdtyListResultCDTO.CategoryList = (from n in Category.ObjectSet().Where(o => o.Id == firstID)
                                                                         select new CategoryDTO()
                                                                         {
                                                                             Id = n.Id,
                                                                             Name = n.Name,
                                                                             icon = n.icon,
                                                                             AppId = n.AppId,
                                                                         }).ToList();
                                }

                            }
                            else
                            {
                                comdtyListResultCDTO.CategoryList = (from n in Category.ObjectSet().Where(o => firstList.Contains(o.Id))
                                                                     select new CategoryDTO()
                                                                     {
                                                                         Id = n.Id,
                                                                         Name = n.Name,
                                                                         icon = n.icon,
                                                                         AppId = n.AppId,
                                                                     }).ToList(); //获取一级类目;
                            }


                        }


                    }


                    commodityListInfer.PageIndex = 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info(string.Format("调用获取分类数据错误信息:{0},{1}", JsonHelper.JsonSerializer(commodityListInfer), ex), "GetCommodityFilterList");
            }

            switch (commodityListInfer.FieldSort)
            {
                case FieldSort4Mobile.Sales: //销量
                    temp = temp.OrderByDescending(t => t.Salesvolume); break;
                //temp = temp.OrderByDescending(t => t.Salesvolume).ThenBy(t => t.SetCategorySort);
                case FieldSort4Mobile.Price:  //价格
                    switch (commodityListInfer.OrderState)
                    {
                        case 1:
                            temp = temp.OrderBy(t => t.CommodityPrice).ThenBy(t => t.SetCategorySort);
                            break;
                        default:
                            temp = temp.OrderByDescending(t => t.CommodityPrice).ThenBy(t => t.SetCategorySort);
                            break;
                    }
                    break;
                case FieldSort4Mobile.Default:  //综合排序
                    switch (commodityListInfer.OrderState)
                    {
                        case 1:
                            temp = temp.OrderBy(t => t.OrderWeight); break;
                        default:
                            temp = temp.OrderByDescending(t => t.OrderWeight); break;
                    }
                    break;
                case FieldSort4Mobile.YouKaPercent:  //油卡排序
                    switch (commodityListInfer.OrderState)
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

            var skuCommodityStockIds = new List<Guid>();
            temp = temp.Skip((commodityListInfer.PageIndex - 1) * commodityListInfer.PageSize).Take(commodityListInfer.PageSize);

            var now = DateTime.Now;
            var begigTime = DateTime.Now.AddYears(-20);
            var tempList = temp.ToList();
            var comIds = tempList.Select(c => c.CommodityId).Distinct().ToList();
            var promotionList = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);

            var commodityCashes = YJBSV.GetCommodityCashPercent(new YJB.Deploy.CustomDTO.CommodityCashInput { CommodityIds = tempList.Select(_ => _.CommodityId).ToList() }).Data;

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

            var cfAppIds = Crowdfunding.ObjectSet().Where(c => appIds.Contains(c.AppId) && c.StartTime < now && c.State == 0).Select(m => m.AppId).ToList();

            var tempt = ZPHSV.Instance.GetAppIdlist(new List<Guid>() { YJB.Deploy.CustomDTO.YJBConsts.YJAppId });
            var ids = tempt.Select(t => t.AppId).ToList();

            List<CommodityListCDTO> commodityListCdtos = new List<CommodityListCDTO>();

            var presents = PresentPromotionCommodity.ObjectSet().Where(_ => comIds.Contains(_.CommodityId))
                        .Join(PresentPromotion.ObjectSet().Where(_ => !_.IsEnd && _.BeginTime < now && now < _.EndTime), pp => pp.PresentPromotionId,
                        ppc => ppc.Id, (c, p) => new { CommodityId = c.CommodityId, PromotionId = p.Id, Limit = p.Limit, BeginTime = p.BeginTime, EndTime = p.EndTime }).ToList();






            foreach (var appSetCommodityDto in tempList)
            {
                CommodityListCDTO commodityListCdto = new CommodityListCDTO
                {
                    Id = appSetCommodityDto.CommodityId,
                    AppId = appSetCommodityDto.AppId,
                    IsEnableSelfTake = appSetCommodityDto.IsEnableSelfTake,
                    Name = appSetCommodityDto.CommodityName,
                    appId = appSetCommodityDto.appId,
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
                    SetCategorySort = appSetCommodityDto.SetCategorySort,
                    ComAttrType = (appSetCommodityDto.ComAttribute == "[]" || appSetCommodityDto.ComAttribute == null) ? 1 : 3,
                    CategoryId = appSetCommodityDto.CategoryID,
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

            LogHelper.Debug("applist的数量：" + applist.Count + "applist:" + (applist.Count > 0 ? applist[0].ToString() : ""));
            if (applist != null && applist.Any())
            {
                comdtyListResultCDTO.appInfoList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO>();

                foreach (var appIdNameIconDTO in applist)
                {
                    comdtyListResultCDTO.appInfoList.Add(new Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO
                    {
                        appId = appIdNameIconDTO.AppId,
                        icon = appIdNameIconDTO.AppIcon,
                        appName = appIdNameIconDTO.AppName
                    });
                }

                //var mallApp = (from n in applist select (int?)n.AppType).ToList();

            }
            var stocks = CommodityStock.ObjectSet().Where(_ => skuCommodityStockIds.Contains(_.Id)).Select(_ => new { _.CommodityId, _.MarketPrice, _.Price }).ToList();
            foreach (var item in commodityListCdtos)
            {
                var stock = stocks.Find(_ => _.CommodityId == item.Id);
                if (stock != null)
                {
                    item.Price = stock.Price;
                    item.MarketPrice = stock.MarketPrice;
                }
            }

            comdtyListResultCDTO.MallAppList = (from n in commodityListCdtos select n.MallType).Distinct().ToList();
            comdtyListResultCDTO.isSuccess = true;
            comdtyListResultCDTO.Code = 0;
            comdtyListResultCDTO.Message = "Success";
            comdtyListResultCDTO.comdtyList = commodityListCdtos./*OrderByDescending(p=>p.SetCategorySort).*/ToList();
            return comdtyListResultCDTO;
        }

        /// <summary>
        /// 返回筛选条件
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityFilterExt()
        {
            return null;
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO> GetBrandAndAdvertiseExt(System.Guid CategoryID)
        {
            var result = new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO>()
            {
                isSuccess = true,
                Message = "success",
                ResultCode = 0
            };

            if (CategoryID == null || CategoryID == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "fail";
                result.ResultCode = -1;
                return result;
            }

            var resultData = new Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO();
            var listInnerBrand = CategoryInnerBrand.ObjectSet();
            var listBrandList = Brandwall.ObjectSet();
            var listCategoryList = CategoryAdvertise.ObjectSet();

            var listBrandWall = from n in listInnerBrand.Where(o => o.CategoryId == CategoryID) select n.BrandId;
            var listBrand = from n in listBrandList.Where(o => listBrandWall.Contains(o.Id) && o.Brandstatu == 1)
                            select new BrandwallDTO()
                            {
                                Id = n.Id,
                                BrandLogo = n.BrandLogo,
                                Brandname = n.Brandname,
                                Brandstatu = n.Brandstatu,
                                AppId = n.AppId,
                            }; //添加品牌墙

            resultData.BrandWallDto = listBrand.ToList();

            if (listCategoryList.Count() > 0)
            {
                var nowDate = DateTime.Now;
                var defaultCategoryAdvertise = listCategoryList.FirstOrDefault(o => o.CategoryId == CategoryID && o.PutTime <= nowDate && o.PushTime >= nowDate);
                if (defaultCategoryAdvertise != null && defaultCategoryAdvertise.Id != Guid.Empty)
                {
                    resultData.CategoryAdvertise = defaultCategoryAdvertise.ToEntityData();
                }
                result.Data = resultData;
                return result;
            }
            else
            {
                result.isSuccess = false;
                result.Message = "fail";
                result.ResultCode = -1;
                return result;
            }
            //添加品类广告

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

                var amount = item.YJBAmount.HasValue ? Math.Truncate(item.YJBAmount.Value * 100) / 100 : 0;
                if (isLableWithValues) list.Add("易捷币" + amount + "元");
                else
                    list.Add("易捷币");
            }

            if (item.YoukaAmount.HasValue && item.YoukaAmount.Value > 0)
            {
                var amount = item.YoukaAmount.HasValue ? Math.Truncate(item.YJBAmount.Value * 100) / 100 : 0;
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

        /// <summary>
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto> GetZshCategoriesExt()
        {
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto> res = new ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto>();
            Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto categorydto = new Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto();
            var innercategory = InnerCategory.ObjectSet().Where(p => p.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && p.IsDel == false && p.CurrentLevel > 0).ToList();
            List<Categorysdto> categorysdtolist = new List<Categorysdto>();
            if (innercategory.Count() > 0)
            {
                foreach (var item in innercategory)
                {
                    Categorysdto model = new Categorysdto();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    categorysdtolist.Add(model);
                }
            }
            categorydto.Count = innercategory.Count();
            categorydto.Data = categorysdtolist;
            res.Code = 200;
            res.Result = categorydto;
            res.Msg = "查询成功!";
            return res;
        }


        /// <summary>
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>MallApply
        public System.Collections.Generic.List<CategorySDTO> GetCategoriesExt(System.Guid appId)
        {
            //读取缓存
            List<CategorySDTO> storeCache = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_InnerCategoryInfo", appId.ToString(), "BTPCache") as List<CategorySDTO>;

            if (storeCache != null && storeCache.Count > 0)
            {
                return storeCache;
            }

            //所有的类目列表
            List<CategorySDTO> categorylist = new List<CategorySDTO>();

            //获取类目信息
            var category = InnerCategory.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false).OrderBy(n => n.Sort);
            var query = from n in category
                        select new CategorySDTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort
                        };
            categorylist = query.ToList<CategorySDTO>();

            //根据级别分类，获取三个级别对应的数据字典
            var dics = categorylist.GroupBy(
                a => a.CurrentLevel,
                (key, group) => new { CurrentLevel = key, CategorySDTOList = group }).
                ToDictionary(a => a.CurrentLevel, a => a.CategorySDTOList);


            if (dics.ContainsKey(1))
            {
                if (!dics.ContainsKey(2))
                {
                    dics.Add(2, new List<CategorySDTO>());
                }
                if (!dics.ContainsKey(3))
                {
                    dics.Add(3, new List<CategorySDTO>());
                }
                dics[1].ToList().ForEach(
                    first =>
                    {
                        if (dics.ContainsKey(2))
                        {
                            //添加第一级分类下的二级分类
                            first.SecondCategory = (from second in dics[2].ToList()
                                                    where second.ParentId == first.Id
                                                    orderby second.Sort
                                                    select new SCategorySDTO
                                                    {
                                                        CurrentLevel = second.CurrentLevel,
                                                        Id = second.Id,
                                                        Name = second.Name,
                                                        ParentId = second.ParentId,
                                                        Sort = second.Sort
                                                    }).ToList();

                            if (dics.ContainsKey(3))
                            {
                                //添加第二级分类下的三级分类
                                first.SecondCategory.ForEach(
                                    second =>
                                    {
                                        second.ThirdCategory =
                                            (from third in dics[3].ToList()
                                             where third.ParentId == second.Id
                                             orderby third.Sort
                                             select new TCategorySDTO
                                             {
                                                 CurrentLevel = third.CurrentLevel,
                                                 Id = third.Id,
                                                 Name = third.Name,
                                                 ParentId = third.ParentId,
                                                 Sort = third.Sort
                                             }).ToList();
                                    });
                            }
                        }
                    });

                Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_InnerCategoryInfo", appId.ToString(), dics[1].ToList(), "BTPCache");
                return dics[1].ToList();
            }
            else
            {
                return new List<CategorySDTO>();
            }

        }


        /// <summary>
        /// 创建初始类别（三级分类）
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatCategory2Ext(System.Guid appId)
        {
            try
            {
                LogHelper.Info("CreatCategory2Ext:AppId" + appId);
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                InnerCategory fcategory = new InnerCategory();
                fcategory.Id = Guid.NewGuid();
                fcategory.Name = "根目录";
                fcategory.Code = "000";
                fcategory.SubId = appId;
                fcategory.AppId = appId;
                fcategory.SubTime = DateTime.Now;
                fcategory.Sort = 1;
                fcategory.CurrentLevel = 0;
                fcategory.IsDel = false;
                fcategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(fcategory);
                InnerCategory scategory = new InnerCategory();
                scategory.Id = Guid.NewGuid();
                scategory.Name = "一级类目";
                scategory.Code = "001";
                scategory.SubId = appId;
                scategory.AppId = appId;
                scategory.SubTime = DateTime.Now;
                scategory.Sort = 1;
                scategory.CurrentLevel = 1;
                scategory.IsDel = false;
                scategory.ParentId = fcategory.Id;
                scategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(scategory);
                //InnerCategory tcategory = new InnerCategory();
                //tcategory.Id = Guid.NewGuid();
                //tcategory.Name = "二级类目";
                //tcategory.SubId = appId;
                //tcategory.AppId = appId;
                //tcategory.SubTime = DateTime.Now;
                //tcategory.Sort = 1;
                //tcategory.CurrentLevel = 2;
                //tcategory.IsDel = false;
                //tcategory.ParentId = scategory.Id;
                //tcategory.EntityState = System.Data.EntityState.Added;
                //contextSession.SaveObject(tcategory);
                //InnerCategory focategory = new InnerCategory();
                //focategory.Id = Guid.NewGuid();
                //focategory.Name = "三级类目";
                //focategory.SubId = appId;
                //focategory.AppId = appId;
                //focategory.SubTime = DateTime.Now;
                //focategory.Sort = 1;
                //focategory.CurrentLevel = 3;
                //focategory.IsDel = false;
                //focategory.ParentId = tcategory.Id;
                //focategory.EntityState = System.Data.EntityState.Added;
                //contextSession.SaveObject(focategory);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("服务异常。appId：{0}", appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 异步更新类目缓存
        /// </summary>
        /// <param name="appid">appid</param>
        private void UpdateCategoryCache(Guid appid)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                a =>
                {
                    string key = appid.ToString();
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_InnerCategoryInfo", key, "BTPCache");//删除缓存

                    var categoryDic = GetCategoriesExt(appid);

                    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_InnerCategoryInfo", key, categoryDic, "BTPCache");//添加缓存

                    Jinher.JAP.Common.Loging.LogHelper.Info(string.Format("更新了商品类目缓存appid：{0}", appid));
                });
        }
    }
}
