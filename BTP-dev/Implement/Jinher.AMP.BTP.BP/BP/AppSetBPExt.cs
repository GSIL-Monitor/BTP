using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Common.Search;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.AMP.ZPH.Deploy.Enum;
using AppSearch = Jinher.AMP.BTP.Common.Search.AppSearch;
using AppSetAppDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO;
using AppSetAppGridDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO;
using AppSetCategoryDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO;
using AppSetCommodityDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityDTO;
using AppSetCommodityGridDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGridDTO;
using AppSetSortDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSortDTO;
using ReturnInfo = Jinher.AMP.ZPH.Deploy.CustomDTO.ReturnInfo;
using SetCommodityOrderDTO = Jinher.AMP.BTP.Deploy.CustomDTO.SetCommodityOrderDTO;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AppSetBP : BaseBP, IAppSet
    {
        /// <summary>
        /// 分页获取所有电商App
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="addToAppSetStatus">-1全部,1已加入到直销,0未加入直销</param>
        /// <returns></returns>
        public AppSetAppGridDTO GetAllCommodityAppExt(int pageIndex, int pageSize, string appNameForQry, int addToAppSetStatus)
        {
            int totalCount = 0;
            List<Guid> appIdList = new List<Guid>();
            List<AppSetAppDTO> appList = new List<AppSetAppDTO>();
            if (addToAppSetStatus == -1 || addToAppSetStatus == 0)
            {
                AppSearch appSearch = SearchHelper.GetTemplateSearchResult(appNameForQry, pageIndex, pageSize);
                if (appSearch != null && appSearch.Paragraph != null && appSearch.Paragraph.Count > 0)
                {
                    totalCount = int.Parse(appSearch.Head.Summary.ResultCount);
                    foreach (Jinher.AMP.BTP.Common.Search.ParagraphDetail a in appSearch.Paragraph)
                    {
                        AppSetAppDTO app = new AppSetAppDTO();
                        app.AppCreateOn = Convert.ToDateTime(a.subtime);
                        app.AppIcon = a.Content.icon;
                        app.AppId = Guid.Parse(a.Content.id);
                        app.AppName = a.Content.name;
                        app.IsAddToAppSet = false;
                        appList.Add(app);

                        appIdList.Add(Guid.Parse(a.Content.id));
                    }
                }
                if (addToAppSetStatus == -1 && appIdList.Count > 0)
                {
                    List<Guid> existAppIdList = AppSet.ObjectSet().Where(l => appIdList.Contains(l.AppId.Value)).Select(l => l.AppId.Value).ToList();
                    if (existAppIdList != null && existAppIdList.Count > 0)
                    {
                        foreach (AppSetAppDTO ap in appList)
                        {
                            if (existAppIdList.Contains(ap.AppId))
                            {
                                ap.IsAddToAppSet = true;
                            }
                        }
                    }
                }
            }
            else
            {
                var temp = AppSet.ObjectSet().Where(l => l.AppName.Contains(appNameForQry))
                    .Select(l => new AppSetAppDTO()
                    {
                        AppCreateOn = l.AppCreateOn,
                        AppIcon = l.AppIcon,
                        AppId = l.AppId.Value,
                        AppName = l.AppName,
                        IsAddToAppSet = true
                    });
                totalCount = temp.Count();
                appList = temp
                    .OrderByDescending(l => l.AppCreateOn)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            return new AppSetAppGridDTO()
            {
                AppList = appList,
                TotalAppCount = totalCount
            };
        }

        /// <summary>
        /// 添加应用到应用组
        /// </summary>
        /// <param name="appInfoList">应用信息列表</param>
        /// <param name="appSetId">应用组id</param>
        /// <param name="appSetType">应用组类型</param>
        /// <returns></returns>
        public ResultDTO AddAppToAppSetExt(List<Tuple<Guid, string, string, DateTime>> appInfoList, Guid appSetId, int appSetType)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "添加成功", ResultCode = 0 };
            appSetId = appSetId.Equals(Guid.Empty) ? Guid.Parse("2CC0C0D0-20A5-4572-AAC7-2984252E6F6A") : appSetId;
            if (appInfoList != null && appInfoList.Count > 0)
            {
                List<Guid> appIdList = appInfoList.Select(p => p.Item1).ToList();
                List<Guid> existAppIdList = AppSet.ObjectSet().Where(l => appIdList.Contains(l.AppId.Value) && l.AppSetId.Equals(appSetId) && l.AppSetType == appSetType).Select(l => l.AppId.Value).ToList();
                foreach (var item in appInfoList)
                {
                    if (!existAppIdList.Contains(item.Item1))
                    {
                        AppSet appSet = new AppSet();
                        appSet.Id = Guid.NewGuid();
                        appSet.AppId = item.Item1;
                        appSet.AppName = item.Item2;
                        appSet.AppIcon = item.Item3;
                        appSet.AppSetId = appSetId;
                        appSet.AppSetType = appSetType;
                        appSet.SubTime = DateTime.Now;
                        appSet.AppAccount = string.Empty;
                        appSet.AppCreateOn = item.Item4;
                        appSet.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(appSet);
                    }
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "添加失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 从应用组移除应用
        /// </summary>
        /// <param name="appIdList">应用id列表</param>
        /// <param name="appSetId">应用组id</param>
        /// <param name="appSetType">应用组类型</param>
        /// <returns></returns>
        public ResultDTO DelAppFromAppSetExt(List<Guid> appIdList, Guid appSetId, int appSetType)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "删除成功", ResultCode = 0 };
            if (appIdList != null && appIdList.Count > 0)
            {
                appSetId = appSetId.Equals(Guid.Empty) ? Guid.Parse("2CC0C0D0-20A5-4572-AAC7-2984252E6F6A") : appSetId;
                var appSetList = AppSet.ObjectSet().Where(l => appIdList.Contains(l.AppId.Value) && l.AppSetId.Equals(appSetId) && l.AppSetType == appSetType).ToList();
                foreach (var item in appSetList)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.Delete(item);
                }
                var commodityList = (from scc in SetCommodityCategory.ObjectSet()
                                     join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                                     where (appIdList.Contains(c.AppId) && c.CommodityType == 0)
                                     select scc).ToList();
                foreach (var item in commodityList)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.Delete(item);
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "删除失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 获取树分类列表
        /// </summary>
        /// <returns></returns>
        public List<AppSetCategoryDTO> GetCategoryListForTreeExt()
        {
            var categoryList = (from c in SetCategory.ObjectSet()
                                orderby c.Sort
                                select new AppSetCategoryDTO
                                {
                                    CategoryId = c.Id,
                                    CategoryName = c.Name,
                                    ParentId = c.ParentId,
                                    PicturesPath = c.PicturesPath
                                }).ToList();
            var categoryIdList = categoryList.Select(p => p.CategoryId).ToList();
            var dir = (from scc in SetCommodityCategory.ObjectSet()
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       join s in AppSet.ObjectSet() on c.AppId equals s.AppId
                       where categoryIdList.Contains(scc.SetCategoryId) && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                       group scc.CommodityId by scc.SetCategoryId into g
                       select new { id = g.Key, count = g.Count() }).ToDictionary(p => p.id, p => p.count);

            var parnetIds = categoryList.Where(c => c.ParentId != Guid.Empty).Select(c => c.ParentId).ToList();

            for (int i = 0; i < categoryList.Count; i++)
            {
                if (parnetIds.Any(c => c == categoryList[i].CategoryId))
                {
                    categoryList[i].HasChildren = true;
                    continue;
                }
                if (categoryList[i].CategoryId == Guid.Parse("324517BD-E303-48D1-977F-43203F5B88BC"))
                {
                    categoryList[i].CommodityCount = this.GetCommodityCountInCategoryExt(categoryList[i].CategoryId);
                    continue;
                }
                if (dir.ContainsKey(categoryList[i].CategoryId))
                {
                    categoryList[i].CommodityCount = dir[categoryList[i].CategoryId];
                    continue;
                }
                if (parnetIds.Any(c => c == categoryList[i].CategoryId))
                    categoryList[i].HasChildren = true;
            }
            return categoryList;
        }

        /// <summary>
        /// 获取树分类列表
        /// </summary>
        /// <returns></returns>
        public List<AppSetCategoryDTO> GetCategoryListForTreeExt(Guid appId)
        {
            var categoryList = (from c in Category.ObjectSet()
                                where c.AppId == appId && c.IsDel == false
                                orderby c.Sort
                                select new AppSetCategoryDTO
                                {
                                    CategoryId = c.Id,
                                    CategoryName = c.Name,
                                    ParentId = c.ParentId ?? Guid.Empty,
                                    PicturesPath = c.icon
                                }).ToList();

            var categoryIdList = categoryList.Select(p => p.CategoryId);
            var dir = (from scc in CommodityCategory.ObjectSet()
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       where categoryIdList.Contains(scc.CategoryId) && scc.IsDel==false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                       group scc.CommodityId by scc.CategoryId
                           into g
                           select new { id = g.Key, count = g.Count() }).ToDictionary(p => p.id, p => p.count);

            var parnetIds = categoryList.Where(c => c.ParentId != Guid.Empty).Select(c => c.ParentId).ToList();
            for (int i = 0; i < categoryList.Count(); i++)
            {
                if (parnetIds.Any(c => c == categoryList[i].CategoryId))
                {
                    categoryList[i].HasChildren = true;
                    continue;
                }
                if (dir.ContainsKey(categoryList[i].CategoryId))
                {
                    categoryList[i].CommodityCount = dir[categoryList[i].CategoryId];
                }
            }
            return categoryList;
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="name">分类名称</param>
        /// <param name="parentId">父分类Id</param>
        /// <param name="picturesPath">图片路径</param>
        /// <returns></returns>
        public Tuple<Guid, int, string> AddCategoryExt(string name, Guid parentId, string picturesPath)
        {
            //判断父分类是否存在
            var curLevel = 1;
            if (parentId != Guid.Empty)
            {
                var parent = SetCategory.ObjectSet().FirstOrDefault(c => c.Id == parentId && c.IsDel == false);
                if (parent == null)
                {
                    var resultDTO = new Tuple<Guid, int, string>(Guid.Empty, 1, "父分类不存在");
                    return resultDTO;
                }
                curLevel = parent.CurrentLevel + 1;
            }



            var category = SetCategory.ObjectSet().FirstOrDefault(p => p.Name == name && p.ParentId == parentId);
            if (category != null)
            {
                var resultDTO = new Tuple<Guid, int, string>(Guid.Empty, 1, "已存在同名分类");
                return resultDTO;
            }

            if (parentId != Guid.Empty && string.IsNullOrEmpty(picturesPath))
            {
                var resultDTO = new Tuple<Guid, int, string>(Guid.Empty, 1, "分类图片不能为空");
                return resultDTO;
            }


            category = new SetCategory();
            category.Id = Guid.NewGuid();
            category.Name = name;
            category.Code = string.Empty;
            category.SubTime = DateTime.Now;
            category.ParentId = parentId;
            category.SubId = this.ContextDTO.LoginUserID;
            category.CurrentLevel = curLevel;
            category.Sort = SetCategory.ObjectSet().Max(p => p.Sort) + 1;
            category.PicturesPath = picturesPath;
            category.EntityState = EntityState.Added;
            ContextFactory.CurrentThreadContext.SaveObject(category);
            try
            {
                ContextFactory.CurrentThreadContext.SaveChanges();
                var resultDTO = new Tuple<Guid, int, string>(category.Id, 0, "添加成功");
                return resultDTO;
            }
            catch (Exception)
            {
                var resultDTO = new Tuple<Guid, int, string>(Guid.Empty, 1, "添加失败");
                return resultDTO;
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id">分类id</param>
        /// <returns></returns>
        public ResultDTO DelCategoryExt(Guid id)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "删除成功", ResultCode = 0 };
            if (id == Guid.Empty)
            {
                resultDTO.ResultCode = 1;
                resultDTO.Message = "参数不能为空";
                return resultDTO;
            }
            if (id.Equals(Guid.Parse("324517BD-E303-48D1-977F-43203F5B88BC")))
            {
                resultDTO.ResultCode = 1;
                resultDTO.Message = "此分类不可删除";
                return resultDTO;
            }
            var tmp = SetCategory.ObjectSet().Where(p => p.ParentId == id).Count();
            if (tmp > 0)
            {
                resultDTO.ResultCode = 1;
                resultDTO.Message = "分类下存在子目录，不能删除";
                return resultDTO;
            }
            if (this.GetCommodityCountInCategoryExt(id) > 0)
            {
                resultDTO.ResultCode = 1;
                resultDTO.Message = "分类下存在商品，不可删除";
                return resultDTO;
            }
            var category = SetCategory.ObjectSet().Where(p => p.Id == id).FirstOrDefault();
            if (category != null)
            {
                category.EntityState = EntityState.Deleted;
                ContextFactory.CurrentThreadContext.SaveObject(category);
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "删除失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="id">分类id</param>
        /// <param name="name">分类名称</param>
        /// <param name="picturesPath"></param>
        /// <returns></returns>
        public ResultDTO UpdateCategoryExt(Guid id, string name, string picturesPath)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "修改成功", ResultCode = 0 };
            var category = SetCategory.ObjectSet().Where(p => p.Id == id).FirstOrDefault();
            if (category != null)
            {

                if (SetCategory.ObjectSet().FirstOrDefault(p => p.Name == name && p.ParentId == category.ParentId && p.Id != category.Id) != null)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "已存在同名分类";
                    return resultDTO;
                }
                if (category.ParentId != Guid.Empty && string.IsNullOrEmpty(picturesPath))
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "分类图片不能为空";
                    return resultDTO;
                }
                category.Name = name;
                category.PicturesPath = picturesPath;
                category.EntityState = EntityState.Modified;
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "修改失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 分类移动
        /// </summary>
        /// <param name="categoryId">被调序分类的id</param>
        /// <param name="targetCategoryId">与被调序分类互换顺序的分类</param>
        /// <returns></returns>
        public ResultDTO ChangeCategorySortExt(Guid categoryId, Guid targetCategoryId)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "移动成功", ResultCode = 0 };
            var sourceCategory = SetCategory.ObjectSet().Where(p => p.Id == categoryId).FirstOrDefault();
            var targetCategory = SetCategory.ObjectSet().Where(p => p.Id == targetCategoryId).FirstOrDefault();
            if (sourceCategory != null && targetCategory != null)
            {
                var tempSort = sourceCategory.Sort;
                sourceCategory.Sort = targetCategory.Sort;
                targetCategory.Sort = tempSort;
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "移动失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <param name="categoryId">分类的id</param>

        /// <returns></returns>
        public int GetCommodityCountInCategoryExt(Guid categoryId)
        {
            if (categoryId.Equals(Guid.Parse("324517BD-E303-48D1-977F-43203F5B88BC")))
            {
                var temp = from c in Commodity.ObjectSet()
                           join s in AppSet.ObjectSet() on c.AppId equals s.AppId
                           where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                           select new { c, s };
                return temp.Count();
            }
            else
            {
                var temp = from scc in SetCommodityCategory.ObjectSet()
                           join sc in SetCategory.ObjectSet() on scc.SetCategoryId equals sc.Id
                           join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                           join s in AppSet.ObjectSet() on c.AppId equals s.AppId
                           where sc.Id == categoryId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                           select new { scc, sc, c, s };
                return temp.Count();
            }
        }

        ///<summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <param name="categoryId">分类的id</param>
        /// <returns></returns>
        public int GetCommodityCountInCategory2Ext(Guid categoryId)
        {
            var temp = from scc in CommodityCategory.ObjectSet()
                       join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                       join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                       where sc.Id == categoryId && scc.IsDel==false && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                       select new { scc, sc, c };
            return temp.Count();
        }

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="commodityNameForQry">商品名称查询字符串</param>
        /// <returns></returns>
        public AppSetCommodityGridDTO GetCommodityInCategoryExt(Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry)
        {
            AppSetCommodityGridDTO dto = new AppSetCommodityGridDTO();
            if (categoryId.Equals(Guid.Parse("324517BD-E303-48D1-977F-43203F5B88BC")))
            {
                var temp = from c in Commodity.ObjectSet()
                           join s in AppSet.ObjectSet() on c.AppId equals s.AppId
                           where c.IsDel == false && c.State == 0 && c.CommodityType == 0
                           orderby s.AppCreateOn descending, s.AppName, c.SubTime descending
                           select new AppSetCommodityDTO
                           {
                               AppIcon = s.AppIcon,
                               AppId = s.AppId.Value,
                               AppName = s.AppName,
                               CommodityId = c.Id,
                               CommodityName = c.Name,
                               CommodityPic = c.PicturesPath,
                               CommodityPrice = c.Price,
                               CommodityStock = c.Stock,
                               IsEnableSelfTake = c.IsEnableSelfTake
                           };
                if (!string.IsNullOrEmpty(appNameForQry))
                {
                    temp = temp.Where(p => p.AppName.Contains(appNameForQry));
                }
                if (!string.IsNullOrEmpty(commodityNameForQry))
                {
                    temp = temp.Where(p => p.CommodityName.Contains(commodityNameForQry));
                }
                dto.TotalCommodityCount = temp.Count();
                temp = temp.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                dto.CommodityList = temp.ToList();
            }
            else
            {
                var temp = from scc in SetCommodityCategory.ObjectSet()
                           join sc in SetCategory.ObjectSet() on scc.SetCategoryId equals sc.Id
                           join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                           join s in AppSet.ObjectSet() on c.AppId equals s.AppId
                           where sc.Id == categoryId && c.IsDel == false && c.State == 0 && c.CommodityType == 0
                           orderby scc.SetCategorySort
                           select new AppSetCommodityDTO
                           {
                               AppIcon = s.AppIcon,
                               AppId = s.AppId.Value,
                               AppName = s.AppName,
                               CommodityId = c.Id,
                               CommodityName = c.Name,
                               CommodityPic = c.PicturesPath,
                               CommodityPrice = c.Price,
                               CommodityStock = c.Stock,
                               IsEnableSelfTake = c.IsEnableSelfTake
                           };
                if (!string.IsNullOrEmpty(appNameForQry))
                {
                    temp = temp.Where(p => p.AppName.Contains(appNameForQry));
                }
                if (!string.IsNullOrEmpty(commodityNameForQry))
                {
                    temp = temp.Where(p => p.CommodityName.Contains(commodityNameForQry));
                }
                dto.TotalCommodityCount = temp.Count();
                temp = temp.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                dto.CommodityList = temp.ToList();
            }

            DateTime now = DateTime.Now;
            foreach (var commodity in dto.CommodityList)
            {
                TodayPromotionDTO com = null;
                var comList = TodayPromotion.GetListDTOByCommodityIdFromCache(commodity.AppId, commodity.CommodityId);
                if (comList != null && comList.Any())
                {
                    com = comList.FirstOrDefault(a => a.EndTime > now && a.StartTime < now);
                }
                if (com != null)
                {
                    if (com.DiscountPrice > -1)
                    {
                        commodity.CommodityPrice = Convert.ToDecimal(com.DiscountPrice);
                        continue;
                    }
                }
            }

            return dto;
        }

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="belongTo"></param>
        /// <param name="categoryId">分类id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="commodityNameForQry">商品名称查询字符串</param>
        /// <returns></returns>
        public AppSetCommodityGrid2DTO GetCommodityInCategory2Ext(Guid belongTo, Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry)
        {
            AppSetCommodityGrid2DTO dto = new AppSetCommodityGrid2DTO();
            var temp = (from scc in CommodityCategory.ObjectSet()
                        join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                        join c in Commodity.ObjectSet() on scc.CommodityId equals c.Id
                        where sc.Id == categoryId && c.IsDel == false && c.State == 0 && c.CommodityType == 0 && scc.IsDel==false
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
                            SetCategorySort = scc.MaxSort ?? 0
                        });
            if (!string.IsNullOrEmpty(commodityNameForQry))
            {
                temp = temp.Where(p => p.CommodityName.Contains(commodityNameForQry));
            }

            List<AppIdNameIconDTO> apps = new List<AppIdNameIconDTO>();
            var appIds = temp.Select(c => c.AppId).Distinct().ToList();
            if (!appIds.Any())
                return new AppSetCommodityGrid2DTO();
            apps = APPSV.GetAppListByIds(appIds, null);

            if (!string.IsNullOrEmpty(appNameForQry))
            {
                var realAppIds = apps.Where(c => c.AppName.Contains(appNameForQry)).Select(c => c.AppId).ToList();
                if (!realAppIds.Any())
                    return new AppSetCommodityGrid2DTO();
                temp = temp.Where(c => realAppIds.Contains(c.AppId));
            }
            dto.TotalCommodityCount = temp.Count();
            dto.CommodityList = temp.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            foreach (var appSetCommodity2Dto in dto.CommodityList)
            {
                var app = apps.FirstOrDefault(c => c.AppId == appSetCommodity2Dto.AppId);
                if (app != null)
                {
                    appSetCommodity2Dto.AppName = app.AppName;
                    appSetCommodity2Dto.AppIcon = app.AppIcon;
                }
                //获取加入的分类名称
                var temp1 = (from scc in CommodityCategory.ObjectSet()
                             join sc in Category.ObjectSet() on scc.CategoryId equals sc.Id
                             where scc.CommodityId == appSetCommodity2Dto.CommodityId && scc.IsDel == false && scc.AppId == belongTo
                             orderby scc.MaxSort
                             select sc);
                var cnames = "";
                foreach (var category in temp1)
                {
                    cnames += category.Name + "<br>";
                }
                appSetCommodity2Dto.CommodityCategory = cnames;
            }

            DateTime now = DateTime.Now;
            var comIds = dto.CommodityList.Select(c => c.CommodityId).Distinct().ToList();
            var todayPromotions = TodayPromotion.GetCurrentPromotionsWithPresell(comIds);
            foreach (var commodity in dto.CommodityList)
            {
                var todayPromotion = todayPromotions.FirstOrDefault(c => c.CommodityId == commodity.CommodityId);
                if (todayPromotion != null)
                {
                    if (todayPromotion.DiscountPrice > -1)
                    {
                        commodity.CommodityPrice = Convert.ToDecimal(todayPromotion.DiscountPrice);
                    }
                }
            }
            return dto;
        }

        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryIdList">分类id列表</param>
        /// <returns></returns>
        public ResultDTO AddCommodityToCategoryExt(List<Guid> commodityIdList, List<Guid> categoryIdList)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "添加成功", ResultCode = 0 };
            if (commodityIdList != null && commodityIdList.Count > 0)
            {
                Dictionary<Guid, string> categoryIdNameDir = SetCategory.ObjectSet()
                    .Where(p => categoryIdList.Contains(p.Id))
                    .Select(p => new { p.Id, p.Name })
                    .ToDictionary(p => p.Id, p => p.Name);
                Dictionary<Guid, int> maxSortDir = SetCommodityCategory.ObjectSet()
                    .Where(p => categoryIdList.Contains(p.SetCategoryId))
                    .GroupBy(p => p.SetCategoryId)
                    .Select(p => new { Id = p.Key, MaxSort = (Int32?)p.Max(c => c.SetCategorySort) })
                    .ToDictionary(p => p.Id, p => p.MaxSort.HasValue ? p.MaxSort.Value : 0);
                var existList = SetCommodityCategory.ObjectSet()
                    .Where(p => commodityIdList.Contains(p.CommodityId) && categoryIdList.Contains(p.SetCategoryId))
                    .Select(p => new { categoryId = p.SetCategoryId, commodityId = p.CommodityId }).ToList();
                foreach (var categoryId in categoryIdList)
                {
                    int maxSort = maxSortDir.ContainsKey(categoryId) ? maxSortDir[categoryId] : 0;
                    foreach (var commodityId in commodityIdList)
                    {
                        if (!existList.Contains(new { categoryId = categoryId, commodityId = commodityId }))
                        {
                            SetCommodityCategory scc = new SetCommodityCategory();
                            scc.Id = Guid.NewGuid();
                            scc.SetCategoryId = categoryId;
                            scc.SetCategoryName = categoryIdNameDir.ContainsKey(categoryId) ? categoryIdNameDir[categoryId] : string.Empty;
                            scc.SetCategorySort = ++maxSort;
                            scc.CommodityId = commodityId;
                            scc.SubTime = DateTime.Now;
                            scc.EntityState = EntityState.Added;
                            ContextFactory.CurrentThreadContext.SaveObject(scc);
                        }
                    }
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "添加失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryIdList">分类id列表</param>
        /// <returns></returns>
        public ResultDTO AddCommodityToCategory2Ext(List<Guid> commodityIdList, List<Guid> categoryIdList)
        {
            ResultDTO resultDto = new ResultDTO { Message = "添加成功", ResultCode = 0 };
            if (commodityIdList != null && commodityIdList.Count > 0)
            {
                Dictionary<Guid, double> maxSortDir = CommodityCategory.ObjectSet()
                    .Where(p => categoryIdList.Contains(p.CategoryId))
                    .GroupBy(p => p.CategoryId)
                    .Select(p => new { Id = p.Key, MaxSort = p.Max(c => c.MaxSort) })
                    .ToDictionary(p => p.Id, p => p.MaxSort ?? 0);

                var categoryQuery = (from c in Category.ObjectSet()
                                     where categoryIdList.Contains(c.Id)
                                     select new { c.Id, c.AppId }).ToList();

                foreach (var categoryId in categoryIdList)
                {
                    double maxSort = maxSortDir.ContainsKey(categoryId) ? maxSortDir[categoryId] : 0;
                    foreach (var commodityId in commodityIdList)
                    {
                        var existList = CommodityCategory.ObjectSet()
                            .Where(p => commodityId == p.CommodityId && categoryIdList.Contains(p.CategoryId)&&p.IsDel==false)
                            .Select(p => new { categoryId = p.CategoryId, commodityId = p.CommodityId });
                        if (!existList.Any())
                        {
                            Guid cAppId = categoryQuery.Where(c => c.Id == categoryId).Select(c => c.AppId).FirstOrDefault();

                            var cc = CommodityCategory.ObjectSet().FirstOrDefault(t => t.AppId == cAppId && t.CommodityId == commodityId && t.CategoryId == Guid.Empty);
                            if (cc != null)
                            {
                                cc.EntityState = EntityState.Deleted;
                                ContextFactory.CurrentThreadContext.SaveObject(cc);
                            }

                            CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                            comcate.Id = Guid.NewGuid();
                            comcate.Name = "商品分类";
                            comcate.SubTime = DateTime.Now;
                            comcate.SubId = ContextDTO.LoginUserID;
                            comcate.CategoryId = categoryId;
                            comcate.CommodityId = commodityId;
                            comcate.ModifiedOn = DateTime.Now;
                            comcate.MaxSort = ++maxSort;
                            comcate.AppId = cAppId;
                            comcate.CrcAppId = JAP.Common.Crc64.ComputeAsAsciiGuid(cAppId);
                            comcate.IsDel = false;
                            comcate.EntityState = EntityState.Added;
                            ContextFactory.CurrentThreadContext.SaveObject(comcate);

                            //同步添加数据到ZPH商品数据表
                            var commodityCdto = (from c in Commodity.ObjectSet()
                                                 where c.Id == commodityId && c.CommodityType == 0
                                                 select new CommodityCDTO
                                                 {
                                                     Id = c.Id,
                                                     AppId = c.AppId,
                                                     ComdtyName = c.Name,
                                                     ComdtyPic = c.PicturesPath,
                                                     Price = c.Price,
                                                     ComdtyId = c.Id,
                                                     isDel = c.IsDel,
                                                     Stock = c.Stock,
                                                     State = (ComdytState)c.State,
                                                     Description = c.Description
                                                 }).FirstOrDefault();
                            var subId = Commodity.ObjectSet().Where(t => t.Id == commodityId && t.CommodityType == 0).ToList()[0].SubId;

                            commodityCdto.AppName = APPSV.GetAppName(commodityCdto.AppId);

                            ReturnInfo returnInfo = ZPHSV.Instance.AddCommodity(subId, commodityCdto);
                            if (!returnInfo.isSuccess)
                            {
                                JAP.Common.Loging.LogHelper.Debug("同步添加数据到ZPH商品数据表 commodityCdto.Id：" + commodityCdto.Id);
                            }
                        }
                    }
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception)
                {
                    resultDto.ResultCode = 1;
                    resultDto.Message = "添加失败";
                }
            }
            return resultDto;
        }


        /// <summary>
        /// 分类下商品排序
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="commoditySortList">商品序号列表</param>
        /// <returns></returns>
        public ResultDTO ReOrderCommodityInCategoryExt(Guid categoryId, List<Guid> commodityIdList, List<int> commoditySortList)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "排序成功", ResultCode = 0 };
            if (commodityIdList != null && commodityIdList.Count > 0 && commoditySortList != null && commoditySortList.Count == commodityIdList.Count)
            {
                var list = SetCommodityCategory.ObjectSet().Where(p => commodityIdList.Contains(p.CommodityId) && p.SetCategoryId == categoryId).ToList();
                var sortList = list.Select(c => c.SetCategorySort).OrderBy(m => m).ToList();
                for (int i = 0; i < commodityIdList.Count; i++)
                {
                    var item = list.FirstOrDefault(c => c.CommodityId == commodityIdList[i]);
                    if (item != null)
                    {
                        item.SetCategorySort = sortList.First();
                        sortList.RemoveAt(0);
                    }
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "排序成功";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryId">分类id</param>
        /// <returns></returns>
        public ResultDTO DelCommodityFromCategoryExt(List<Guid> commodityIdList, Guid categoryId)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "删除成功", ResultCode = 0 };
            if (commodityIdList != null && commodityIdList.Count > 0)
            {
                var list = SetCommodityCategory.ObjectSet().Where(p => commodityIdList.Contains(p.CommodityId) && p.SetCategoryId == categoryId).ToList();
                foreach (SetCommodityCategory item in list)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.Delete(item);
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "删除失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryId">分类id</param>
        /// <returns></returns>
        public ResultDTO DelCommodityFromCategory2Ext(List<Guid> commodityIdList, Guid categoryId)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "删除成功", ResultCode = 0 };
            if (commodityIdList != null && commodityIdList.Count > 0)
            {
                var list = CommodityCategory.ObjectSet().Where(p => commodityIdList.Contains(p.CommodityId) && p.CategoryId == categoryId).ToList();
                foreach (CommodityCategory item in list)
                {
                    item.IsDel = true;
                    item.EntityState = EntityState.Modified;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }
                try
                {
                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    resultDTO.ResultCode = 1;
                    resultDTO.Message = "删除失败";
                }
            }
            return resultDTO;
        }

        /// <summary>
        /// 调整分类中商品排序(上移\下移)
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityId">商品id</param>
        /// <param name="direction">调整方向(正数下移,负数上移)</param>
        /// <returns></returns>
        public ResultDTO ChangeCommodityOrderInCategoryExt(Guid categoryId, Guid commodityId, int direction)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "移动成功", ResultCode = 0 };
            var sort = SetCommodityCategory.ObjectSet()
                .Where(p => p.CommodityId == commodityId && p.SetCategoryId == categoryId)
                .Select(p => p.SetCategorySort).FirstOrDefault();

            List<SetCommodityCategory> list = null;
            if (direction > 0)
            {
                list = SetCommodityCategory.ObjectSet()
                    .Where(p => p.SetCategoryId == categoryId && p.SetCategorySort >= sort)
                    .OrderBy(p => p.SetCategorySort)
                    .Take(2).ToList();
            }
            else if (direction < 0)
            {
                list = SetCommodityCategory.ObjectSet()
                    .Where(p => p.SetCategoryId == categoryId && p.SetCategorySort <= sort)
                    .OrderByDescending(p => p.SetCategorySort)
                    .Take(2).ToList();
            }
            if (list != null && list.Count == 2)
            {
                var tempSort = list[0].SetCategorySort;
                list[0].SetCategorySort = list[1].SetCategorySort;
                list[1].SetCategorySort = tempSort;
            }

            try
            {
                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                resultDTO.ResultCode = 1;
                resultDTO.Message = "移动失败";
            }

            return resultDTO;
        }

        /// <summary>
        /// 商品在分类中置顶
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityId">商品id</param>
        /// <returns></returns>
        public ResultDTO TopCommodityOrderInCategoryExt(Guid categoryId, Guid commodityId)
        {
            ResultDTO resultDTO = new ResultDTO() { Message = "移动成功", ResultCode = 0 };
            try
            {
                var commodity = SetCommodityCategory.ObjectSet()
                                        .FirstOrDefault(c => c.SetCategoryId == categoryId && c.CommodityId == commodityId);
                if (commodity != null)
                {
                    var minSort = SetCommodityCategory.ObjectSet()
        .Where(p => p.SetCategoryId == categoryId).Min(c => c.SetCategorySort);
                    commodity.SetCategorySort = --minSort;
                }
                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                resultDTO.ResultCode = 1;
                resultDTO.Message = "移动失败";
            }
            return resultDTO;
        }

        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="appSetSortDto"></param>
        /// <returns></returns>
        public ResultDTO SetSetCommodityOrderExt(AppSetSortDTO appSetSortDto)
        {
            ResultDTO returnInfo = new ResultDTO { ResultCode = 1, Message = "" };
            try
            {
                if (appSetSortDto.DtoList.Count <= 0)
                {
                    returnInfo.ResultCode = 0;
                    return returnInfo;
                }
                foreach (SetCommodityOrderDTO item in appSetSortDto.DtoList)
                {
                    if (IsReapetRankNo(appSetSortDto.CategoryId, item.Id, item.RankNo))
                    {
                        returnInfo.Message = "排序号重复，请重新修改";
                        return returnInfo;
                    }

                    var setCmdty = (from p in SetCommodityCategory.ObjectSet()
                                    where p.CommodityId == item.Id && p.SetCategoryId == appSetSortDto.CategoryId
                                    select p).FirstOrDefault();
                    if (setCmdty == null)
                    {
                        continue;
                    }
                    setCmdty.SetCategorySort = item.RankNo;
                    setCmdty.ModifiedOn = DateTime.Now;
                    setCmdty.EntityState = EntityState.Modified;
                }
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count > 0)
                {
                    returnInfo.ResultCode = 0;
                    returnInfo.Message = "排序成功";
                    return returnInfo;
                }
                returnInfo.ResultCode = 1;
                returnInfo.Message = "排序失败";
                return returnInfo;
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("AppSetBP.SetSetCommodityOrderExt", ex.ToString());
                returnInfo.ResultCode = 1;
                returnInfo.Message = "出现异常";
                return returnInfo;
            }
        }

        /// <summary>
        /// 核查排序号重复
        /// </summary>
        /// <param name="selfId"></param>
        /// <param name="rankNo"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        bool IsReapetRankNo(Guid categoryId, Guid selfId, double rankNo)
        {
            int count = SetCommodityCategory.ObjectSet().Count(o => o.SetCategoryId == categoryId && o.CommodityId != selfId && o.SetCategorySort == rankNo);
            return count > 0;
        }

        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="appSetSortDto"></param>
        /// <returns></returns>
        public ResultDTO SetSetCommodityOrder2Ext(AppSetSortDTO appSetSortDto)
        {
            ResultDTO returnInfo = new ResultDTO { ResultCode = 1, Message = "" };
            try
            {
                if (appSetSortDto.DtoList.Count <= 0)
                {
                    returnInfo.ResultCode = 0;
                    return returnInfo;
                }
                foreach (SetCommodityOrderDTO item in appSetSortDto.DtoList)
                {
                    if (IsReapetRankNo2(appSetSortDto.CategoryId, item.Id, item.RankNo))
                    {
                        returnInfo.Message = "排序号重复，请重新修改";
                        return returnInfo;
                    }

                    var setCmdty = (from p in CommodityCategory.ObjectSet()
                                    where p.CommodityId == item.Id && p.CategoryId == appSetSortDto.CategoryId && !(bool)p.IsDel
                                    select p).FirstOrDefault();
                    if (setCmdty == null)
                    {
                        continue;
                    }
                    setCmdty.MaxSort = item.RankNo;
                    setCmdty.ModifiedOn = DateTime.Now;
                    setCmdty.EntityState = EntityState.Modified;
                }
                int count = ContextFactory.CurrentThreadContext.SaveChanges();
                if (count > 0)
                {
                    returnInfo.ResultCode = 0;
                    returnInfo.Message = "排序成功";
                    return returnInfo;
                }
                returnInfo.ResultCode = 1;
                returnInfo.Message = "排序失败";
                return returnInfo;
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("AppSetBP.SetSetCommodityOrder2Ext", ex.ToString());
                returnInfo.ResultCode = 1;
                returnInfo.Message = "出现异常";
                return returnInfo;
            }
        }

        /// <summary>
        /// 核查排序号重复
        /// </summary>
        /// <param name="selfId"></param>
        /// <param name="rankNo"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        bool IsReapetRankNo2(Guid categoryId, Guid selfId, double rankNo)
        {
            int count = CommodityCategory.ObjectSet().Count(o => o.CategoryId == categoryId && o.CommodityId != selfId && o.MaxSort == rankNo && o.MaxSort != -999);
            return count > 0;
        }
    }
}
