
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/25 17:48:14
***************/
using System;
using System.Collections.Generic;
using System.Data;
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
using Jinher.JAP.Cache;

namespace Jinher.AMP.BTP.BP
{

   
    public partial class CategoryBP : BaseBP, ICategory
    {
        /// <summary>
        /// 添加同级类别
        /// </summary>
        /// <param name="categoryName">名称</param>
        /// <param name="targetId">目标ID</param>
        /// <param name="appId">卖家ID</param>
        public ResultDTO AddCategoryExt(string categoryName, Guid appId, Guid targetId)
        {
            if (string.IsNullOrWhiteSpace(categoryName) || appId == Guid.Empty || targetId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数错误" };
            }           
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category target = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                if (target == null)
                    return new ResultDTO { ResultCode = 1, Message = "Error" };

                List<Category> targetlist = new List<Category>();
                if (target.CurrentLevel == 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "不能在根结点添加同级" };
                }
                else if (target.CurrentLevel == 1)
                {
                    targetlist = Category.ObjectSet().Where(n => n.CurrentLevel == 1 && n.Sort > target.Sort && n.IsDel == false).ToList();
                }
                else
                {
                    targetlist = Category.ObjectSet().Where(n => n.ParentId == target.ParentId && n.Sort > target.Sort && n.IsDel == false).ToList();
                }
                foreach (Category temp in targetlist)
                {
                    temp.Sort += 1;
                    temp.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp);
                }

                Category categorydto = new Category();
                categorydto.Id = Guid.NewGuid();
                categorydto.Name = categoryName;
                categorydto.AppId = appId;
                categorydto.SubId = appId;
                categorydto.SubTime = DateTime.Now;
                categorydto.CurrentLevel = target.CurrentLevel;
                categorydto.Sort = target.Sort + 1;
                categorydto.ParentId = target.ParentId;
                categorydto.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(categorydto);
                contextSession.SaveChanges();

                //更新类目缓存
                UpdateCategoryCache(appId);

                return new ResultDTO { ResultCode = 0, Message = categorydto.Id.ToString() };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加分类服务异常。categoryName：{0}，appId：{1}，targetId：{2}", categoryName, appId, targetId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }


        }

        /// <summary>
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<CategorySDTO> GetCategoriesExt(System.Guid appId)
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
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<CategoryS2DTO> GetCategories2Ext(System.Guid appId)
        {
            //读取缓存
            //Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_Category2Info", appId.ToString(), "BTPCache");
            //List<CategoryS2DTO> storeCache = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_Category2Info", appId.ToString(), "BTPCache") as List<CategoryS2DTO>;

            //if (storeCache != null && storeCache.Count > 0)
            //{
            //    return storeCache;
            //}

            //所有的类目列表
            List<CategoryS2DTO> categorylist = new List<CategoryS2DTO>();

            //获取类目信息
            var category = Category.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false).OrderBy(n => n.Sort);
            var query = from n in category
                        orderby n.Sort
                        select new CategoryS2DTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort,
                            Icno = n.icon,
                            IsUse = (bool)n.IsUse
                        };
            categorylist = query.ToList<CategoryS2DTO>();

            //根据级别分类，获取四个级别对应的数据字典
            var dics = categorylist.GroupBy(
                a => a.CurrentLevel,
                (key, group) => new { CurrentLevel = key, CategorySDTOList = group }).
                ToDictionary(a => a.CurrentLevel, a => a.CategorySDTOList);


            if (dics.ContainsKey(0))
            {
                if (!dics.ContainsKey(1))
                {
                    dics.Add(1, new List<CategoryS2DTO>());
                }
                if (!dics.ContainsKey(2))
                {
                    dics.Add(2, new List<CategoryS2DTO>());
                }
                if (!dics.ContainsKey(3))
                {
                    dics.Add(3, new List<CategoryS2DTO>());
                }
                dics[0].ToList().ForEach(
                    first =>
                    {
                        if (dics.ContainsKey(1))
                        {
                            //添加第一级分类下的二级分类
                            first.SecondCategory = (from second in dics[1].ToList()
                                                    where second.ParentId == first.Id
                                                    orderby second.Sort
                                                    select new SCategorySDTO
                                                    {
                                                        CurrentLevel = second.CurrentLevel,
                                                        Id = second.Id,
                                                        Name = second.Name,
                                                        ParentId = second.ParentId,
                                                        Sort = second.Sort,
                                                        Icno = second.Icno,
                                                        IsUse = second.IsUse
                                                    }).ToList();
                            if (dics.ContainsKey(2))
                            {
                                //添加第二级分类下的三级分类
                                first.SecondCategory.ForEach(
                                    second =>
                                    {
                                        second.ThirdCategory =
                                            (from third in dics[2].ToList()
                                             where third.ParentId == second.Id
                                             orderby third.Sort
                                             select new TCategorySDTO
                                             {
                                                 CurrentLevel = third.CurrentLevel,
                                                 Id = third.Id,
                                                 Name = third.Name,
                                                 ParentId = third.ParentId,
                                                 Sort = third.Sort,
                                                 Icno = third.Icno,
                                                 IsUse = third.IsUse
                                             }).ToList();
                                        if (dics.ContainsKey(3))
                                        {
                                            //添加第三级分类下的四级分类
                                            second.ThirdCategory.ForEach(
                                                third =>
                                                {
                                                    third.FourCategory =
                                                        (from four in dics[3].ToList()
                                                         where four.ParentId == third.Id
                                                         orderby four.Sort
                                                         select new FCategorySDTO
                                                         {
                                                             CurrentLevel = four.CurrentLevel,
                                                             Id = four.Id,
                                                             Name = four.Name,
                                                             ParentId = four.ParentId,
                                                             Sort = four.Sort,
                                                             Icno = four.Icno,
                                                             IsUse = four.IsUse
                                                         }).ToList();
                                                });
                                        }
                                    });
                            }
                        }
                    });

                //Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_Category2Info", appId.ToString(), dics[0].ToList(), "BTPCache");
                LogHelper.Debug("获取的分类数据列表如下：" + JsonHelper.JsSerializer(dics[0]));
                return dics[0].ToList();
            }
            else
            {
                return new List<CategoryS2DTO>();
            }

        }

        /// <summary>
        /// 删除卖家类别
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="myId">分类id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCategoryExt(System.Guid appId, System.Guid myId)
        {
            try
            {
                AppSetBP appSetBp = new AppSetBP();
                int commdityCount = appSetBp.GetCommodityCountInCategory2(myId);
                if (commdityCount > 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "此分类下有商品存在，不能删除" };
                }

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                CommodityCategory delcomcate = new CommodityCategory();
                Category cate = Category.ObjectSet().FirstOrDefault(n => n.Id == myId);
                if (cate == null)
                    return new ResultDTO { ResultCode = 1, Message = "此分类下有商品存在，不能删除" };
                List<Category> catelist1 = new List<Category>();
                if (cate.CurrentLevel == 0)
                {
                    catelist1 = Category.ObjectSet().Where(n => n.CurrentLevel == 0 && n.Sort > cate.Sort && n.IsDel == false).ToList();
                }
                else
                {
                    catelist1 = Category.ObjectSet().Where(n => n.ParentId == cate.ParentId && n.Sort > cate.Sort && n.IsDel == false).ToList();
                }
                foreach (Category temp in catelist1)
                {
                    temp.Sort -= 1;
                    temp.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp);
                }
                string myID = myId.ToString();

                //获取类目以及其所有子类目，并设置状态为删除

                var ids = Category.ObjectSet().Where(n => n.ParentId == myId).Select(n => n.Id);
                List<Category> catelist2 = Category.ObjectSet().
                    Where(n => (n.Id == myId || ids.Contains(n.Id) || (n.ParentId.HasValue && ids.Contains(n.ParentId.Value))) && n.IsDel == false).ToList();

                foreach (Category temp1 in catelist2)
                {
                    temp1.IsDel = true;
                    temp1.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp1);
                }

                //删除商品类目属性值
                var cguids = from c in catelist2 select c.Id;
                List<CommodityCategory> comcatelist = CommodityCategory.ObjectSet().Where(n => cguids.Contains(n.CategoryId)).ToList();
                foreach (CommodityCategory temp3 in comcatelist)
                {
                    temp3.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(temp3);
                }

                cate.EntityState = System.Data.EntityState.Modified;
                cate.IsDel = true;
                contextSession.SaveObject(cate);
                contextSession.SaveChanges();

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除类别服务异常。appId：{0}，myId：{1}", appId, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "删除失败" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 编辑卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategoryExt(System.Guid appId, string name, Guid myId)
        {
            if (appId == Guid.Empty || string.IsNullOrWhiteSpace(name) || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                Category cate = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (cate == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "数据库中不存在此类别" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                cate.EntityState = System.Data.EntityState.Modified;
                cate.Name = name;
                contextSession.SaveObject(cate);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑类别服务异常。appId：{0}，name：{1}，myId：{2}", appId, name, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }

        /// <summary>
        /// 编辑卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategory2Ext(System.Guid appId, string name, Guid myId, string icon, int isuse)
        {
            if (appId == Guid.Empty || string.IsNullOrWhiteSpace(name) || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                Category cate = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (cate == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "数据库中不存在此类别" };
                }
                var currentLevelCategoryList = Category.ObjectSet().Where(n => n.ParentId == cate.ParentId && n.AppId == appId && n.IsDel == false && n.Id != cate.Id);
                int count = currentLevelCategoryList.Count(t => t.Name == name);
                if (count > 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "同级目录下分类名称不能重复" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                cate.EntityState = System.Data.EntityState.Modified;
                cate.Name = name;
                cate.icon = icon;
                cate.IsUse = isuse == 0;
                contextSession.SaveObject(cate);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑类别服务异常。appId：{0}，name：{1}，myId：{2}", appId, name, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "更新失败！" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }
        /// <summary>
        /// 升级类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO LevelUpCategoryExt(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            if (appId == Guid.Empty || targetId == Guid.Empty || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category targetcate = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                Category mycategory = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                string idmy = myId.ToString();

                var ids = Category.ObjectSet().Where(n => n.ParentId == myId).Select(n => n.Id);

                if (targetcate == null || mycategory == null || !ids.Any())
                {
                    return new ResultDTO { ResultCode = 1, Message = "类别不存在" };
                }
                List<Category> mycatelist = Category.ObjectSet().
                    Where(n => (ids.Contains(n.Id) || (n.ParentId.HasValue && ids.Contains(n.ParentId.Value))) && n.IsDel == false).ToList();
                foreach (Category temp in mycatelist)
                {
                    temp.CurrentLevel = temp.CurrentLevel - 1;
                    temp.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp);
                }
                List<Category> mycatelist1 = new List<Category>();
                List<Category> targetcatelist = new List<Category>();
                if (mycategory.CurrentLevel == 0)
                {
                    mycatelist1 = Category.ObjectSet().
                        Where(n => n.CurrentLevel == 0 && n.Sort > mycategory.Sort && n.IsDel == false).ToList();
                    targetcatelist = Category.ObjectSet().
                        Where(n => n.CurrentLevel == 0 && n.Sort > targetcate.Sort && n.IsDel == false).ToList();
                }
                else
                {
                    mycatelist1 = Category.ObjectSet().
                        Where(n => n.ParentId == mycategory.ParentId && n.Sort > mycategory.Sort && n.IsDel == false).ToList();
                    targetcatelist = Category.ObjectSet().
                        Where(n => n.ParentId == targetcate.ParentId && n.Sort > targetcate.Sort && n.IsDel == false).ToList();

                }
                foreach (Category temp2 in mycatelist1)
                {
                    temp2.Sort -= 1;
                    temp2.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp2);
                }
                foreach (Category temp4 in targetcatelist)
                {
                    temp4.Sort += 1;
                    temp4.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp4);
                }
                mycategory.Sort = targetcate.Sort + 1;
                mycategory.CurrentLevel = targetcate.CurrentLevel;
                mycategory.ParentId = targetcate.ParentId;
                mycategory.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(mycategory);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("升级类别服务异常。appId：{0}，targetId：{1}，myId：{2}", appId, targetId, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }
        /// <summary>
        /// 降级类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO LevelDownCategoryExt(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            if (appId == Guid.Empty || targetId == Guid.Empty || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            //注意：降级时只会降一级，所以一些级别标识值只需要+1就行
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category targetcate = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                Category mycategory = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);

                //获取要移动类目下所有类目
                string myGuid = myId.ToString();
                var ids = Category.ObjectSet().Where(n => n.ParentId == myId && n.IsDel == false).Select(n => n.Id);
                if (targetcate == null || mycategory == null || !ids.Any())
                {
                    return new ResultDTO { ResultCode = 1, Message = "类别不存在" };
                }
                List<Category> catelist = Category.ObjectSet().
                    Where(n => n.Id != myId && (ids.Contains(n.Id) || (n.ParentId.HasValue && ids.Contains(n.ParentId.Value))) && n.IsDel == false).ToList();
                foreach (Category cate in catelist)
                {
                    cate.CurrentLevel = cate.CurrentLevel + 1;
                    cate.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(cate);
                }
                mycategory.ParentId = targetcate.Id;
                mycategory.Sort = 1;
                mycategory.CurrentLevel = targetcate.CurrentLevel + 1;
                mycategory.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(mycategory);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("降级类别服务异常。appId：{0}，targetId：{1}，myId：{2}", appId, targetId, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }

        /// <summary>
        /// 升序类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpCategoryExt(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            if (appId == Guid.Empty || targetId == Guid.Empty || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category targetcate = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                Category mycategory = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (targetcate == null || mycategory == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "类别不存在" };
                }
                targetcate.Sort += 1;
                mycategory.Sort -= 1;
                targetcate.EntityState = System.Data.EntityState.Modified;
                mycategory.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(mycategory);
                contextSession.SaveObject(targetcate);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("升序类别服务异常。appId：{0}，targetId：{1}，myId：{2}", appId, targetId, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 降序类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DownCategoryExt(System.Guid appId, System.Guid targetId, System.Guid myId)
        {
            if (appId == Guid.Empty || targetId == Guid.Empty || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category targetcate = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                Category mycategory = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (targetcate == null || mycategory == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "类别不存在" };
                }
                targetcate.Sort -= 1;
                mycategory.Sort += 1;
                targetcate.EntityState = System.Data.EntityState.Modified;
                mycategory.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(mycategory);
                contextSession.SaveObject(targetcate);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("降序类别服务异常。appId：{0}，targetId：{1}，myId：{2}", appId, targetId, myId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 拖动类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <param name="moveType">移动类型</param> 
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DragCategoryExt(System.Guid appId, System.Guid targetId, System.Guid myId, string moveType)
        {
            if (appId == Guid.Empty || targetId == Guid.Empty || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //获取目标类别
                Category targetcate = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);

                //获取被操作类别
                Category mycategory = Category.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (targetcate == null || mycategory == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "类别不存在" };
                }
                string mid = mycategory.Id.ToString();
                int b = 0;

                //获取被拖拽类目的所有子节点
                var ids = Category.ObjectSet().Where(n => n.ParentId == myId).Select(n => n.Id);
                if (!ids.Any())
                {
                    return new ResultDTO { ResultCode = 3, Message = "被拖拽类目的子节点不存在" };
                }
                List<Category> a = Category.ObjectSet().
                    Where(n => (ids.Contains(n.Id) || n.ParentId.HasValue && ids.Contains(n.ParentId.Value)) && n.IsDel == false).ToList();

                if (a.Count != 0)
                {
                    //最深子节点减去被推拽节点级别,相当于子节点跨越层数
                    int c = a.Select(n => n.CurrentLevel).Max() - mycategory.CurrentLevel;

                    #region 判断是否可移动 mw 2014.5.29
                    //拖到目标节点的平级
                    if (string.Equals(moveType, "prev") || string.Equals(moveType, "next"))//这样直接传字符串真的好吗？？
                    {
                        //保证层级不超过3
                        if (c + targetcate.CurrentLevel > 3)
                        {
                            return new ResultDTO { ResultCode = 4, Message = "操作会导致节点丢失，请重新操作" };
                        }
                    }
                    //拖到目标节点的子级
                    else if (string.Equals(moveType, "inner"))
                    {
                        //保证层级不超过3
                        if (c + targetcate.CurrentLevel > 2)
                        {
                            return new ResultDTO { ResultCode = 4, Message = "操作会导致节点丢失，请重新操作" };
                        }
                    }
                    #endregion

                    b = targetcate.CurrentLevel + c;
                }
                else//被推拽对象没有子节点时执行
                {
                    if (moveType == "prev" || moveType == "next")
                    {
                        b = targetcate.CurrentLevel;
                        if (b > 3)
                        {
                            return new ResultDTO { ResultCode = 5, Message = "不能超过三级节点" };
                        }
                    }
                    else
                    {
                        b = targetcate.CurrentLevel + 1;
                        if (b > 3)
                        {
                            return new ResultDTO { ResultCode = 5, Message = "不能超过三级节点" };
                        }
                    }
                }
                if (moveType == "prev")
                {
                    //if (b < 3)
                    //{
                    mycategory.ParentId = targetcate.ParentId;//父节点和目标节点一样 modified by mw 2014.5.29

                    //升级为1，降级为2,不变为0
                    int compResult = 0;
                    if (mycategory.CurrentLevel == targetcate.CurrentLevel)
                    {
                        compResult = 0;
                    }
                    else if (mycategory.CurrentLevel > targetcate.CurrentLevel)
                    {
                        compResult = 1;
                    }
                    else
                    {
                        compResult = 2;
                    }

                    if (mycategory.CurrentLevel != targetcate.CurrentLevel)
                    {
                        mycategory.Sort = targetcate.Sort;
                        mycategory.CurrentLevel = targetcate.CurrentLevel;
                        mycategory.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(mycategory);
                        List<Category> catelist = new List<Category>();

                        #region 对后面的节点排序
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }
                        foreach (Category temp in catelist)
                        {
                            temp.Sort += 1;
                            temp.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp);
                        }
                        #endregion

                        #region 同步子节点和商品对应的类目信息

                        //获取它的子节点
                        List<Category> mycatelist = new List<Category>();
                        if (mycategory.CurrentLevel == 1)
                        {
                            //原来
                            //mycatelist = Category.ObjectSet().Where(n => n.CurrentLevel == 0 && n.IsDel == false).ToList();

                            //修改后
                            mycatelist = Category.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                        }
                        else
                        {
                            mycatelist = Category.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                        }
                        List<Guid> mycateids = mycatelist.Select(n => n.Id).ToList();

                        foreach (var temp1 in mycatelist)//同步子节点和商品对应的类目信息
                        {
                            if (compResult == 1)
                            {
                                temp1.CurrentLevel = mycategory.CurrentLevel + 1;
                            }
                            else if (compResult == 2)
                            {
                                temp1.CurrentLevel = mycategory.CurrentLevel - 1;
                            }
                            temp1.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp1);
                        }
                        #endregion

                        contextSession.SaveChanges();

                        //更新类目缓存
                        UpdateCategoryCache(appId);
                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    else
                    {
                        List<Category> catelist = new List<Category>();
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }
                        foreach (Category temp in catelist)
                        {
                            temp.Sort += 1;
                            temp.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp);
                        }
                        mycategory.Sort = targetcate.Sort - 1;
                        mycategory.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(mycategory);
                        contextSession.SaveChanges();

                        //更新类目缓存
                        UpdateCategoryCache(appId);
                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    //}
                    //else
                    //{
                    //    return new ResultDTO { ResultCode = 2, Message = "Error" };
                    //}
                }
                if (moveType == "next")
                {
                    //if (b < 3)
                    //{
                    mycategory.ParentId = targetcate.ParentId;//父节点和目标节点一样 modified by mw 2014.5.29

                    //升级为1，降级为2,不变为0
                    int compResult = 0;
                    if (mycategory.CurrentLevel == targetcate.CurrentLevel)
                    {
                        compResult = 0;
                    }
                    else if (mycategory.CurrentLevel > targetcate.CurrentLevel)
                    {
                        compResult = 1;
                    }
                    else
                    {
                        compResult = 2;
                    }

                    if (mycategory.CurrentLevel != targetcate.CurrentLevel)
                    {
                        mycategory.Sort = targetcate.Sort + 1;
                        mycategory.CurrentLevel = targetcate.CurrentLevel;
                        mycategory.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(mycategory);

                        #region 对后面的节点排序
                        List<Category> catelist = new List<Category>();
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }
                        foreach (Category temp in catelist)
                        {
                            temp.Sort += 1;
                            temp.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp);
                        }
                        #endregion

                        #region 同步子节点和商品对应的类目信息
                        List<Category> mycatelist = new List<Category>();
                        if (mycategory.CurrentLevel == 1)
                        {
                            //修改前(我不明白他为什么要这么做，但感觉是错的)
                            //mycatelist = Category.ObjectSet().Where(n => n.CurrentLevel == 0 && n.IsDel == false).ToList();

                            //修改后
                            mycatelist = Category.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                        }
                        else
                        {
                            mycatelist = Category.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                        }
                        List<Guid> mycateids = mycatelist.Select(n => n.Id).ToList();
                        foreach (var temp1 in mycatelist)//同步子节点和商品对应的类目信息
                        {
                            if (compResult == 1)
                            {
                                temp1.CurrentLevel = mycategory.CurrentLevel + 1;
                            }
                            else if (compResult == 2)
                            {
                                temp1.CurrentLevel = mycategory.CurrentLevel - 1;
                            }
                            temp1.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp1);
                        }
                        #endregion

                        contextSession.SaveChanges();

                        //更新类目缓存
                        UpdateCategoryCache(appId);
                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    else
                    {
                        List<Category> catelist = new List<Category>();
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = Category.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }

                        foreach (Category temp in catelist)
                        {
                            temp.Sort += 1;
                            temp.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp);
                        }
                        mycategory.Sort = targetcate.Sort + 1;
                        mycategory.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(mycategory);
                        contextSession.SaveChanges();

                        //更新类目缓存
                        UpdateCategoryCache(appId);
                        return new ResultDTO { ResultCode = 0, Message = "Success" };
                    }
                    //}
                    //else
                    //{
                    //    return new ResultDTO { ResultCode = 2, Message = "Error" };
                    //}
                }

                if (moveType == "inner")
                {
                    //if (b <= 2)
                    //{

                    //原来代码
                    //Category nexttarget = Category.ObjectSet().Where(n => n.ParentId == targetId && n.IsDel == false).FirstOrDefault();
                    //修改
                    List<Category> nexttarget = Category.ObjectSet().Where(n => n.ParentId == targetId && n.IsDel == false).ToList();
                    if (nexttarget.Count > 0)
                    {
                        //原来的代码(为什么感觉不靠谱？？)
                        //this.LevelUpCategory(appId, nexttarget.Id, myId);

                        //修改代码
                        mycategory.Sort = nexttarget.Max(tar => tar.Sort) + 1;
                    }
                    else
                    {
                        mycategory.Sort = 1;
                    }
                    mycategory.ParentId = targetcate.Id;
                    mycategory.CurrentLevel = targetcate.CurrentLevel + 1;
                    mycategory.EntityState = System.Data.EntityState.Modified;

                    List<Category> catelist = Category.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                    foreach (Category cate in catelist)
                    {
                        cate.CurrentLevel = mycategory.CurrentLevel + 1;
                        cate.EntityState = System.Data.EntityState.Modified;
                        contextSession.SaveObject(cate);
                    }
                    contextSession.SaveObject(mycategory);
                    contextSession.SaveChanges();

                    //更新类目缓存
                    UpdateCategoryCache(appId);

                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                    //}
                    //else
                    //{
                    //    return new ResultDTO { ResultCode = 2, Message = "Error" };
                    //}
                }
                return new ResultDTO { ResultCode = 2, Message = "Error" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("服务异常。appId：{0}，targetId：{1}，myId：{2}，moveType：{3}", appId, targetId, myId, moveType), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }

        /// <summary>
        /// 添加子级类别
        /// </summary>
        /// <param name="name">类目名称</param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="appId">被操作类别ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddChildCategoryExt(string name, System.Guid targetId, Guid appId)
        {
            Guid rcategoryId = new Guid();

            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category pcategory = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.AppId == appId);
                if (pcategory == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                }
                Category category = Category.ObjectSet().Where(n => n.ParentId == targetId && n.AppId == appId && n.IsDel == false).OrderByDescending(n => n.Sort).FirstOrDefault();
                if (category != null)
                {
                    Category ncate = new Category();
                    ncate.Id = Guid.NewGuid();
                    ncate.Name = name;
                    ncate.AppId = appId;
                    ncate.SubId = appId;
                    ncate.SubTime = DateTime.Now;
                    ncate.Sort = category.Sort + 1;
                    ncate.ParentId = pcategory.Id;
                    ncate.IsDel = false;
                    ncate.CurrentLevel = pcategory.CurrentLevel + 1;
                    ncate.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(ncate);

                    rcategoryId = ncate.Id;
                    contextSession.SaveChanges();
                }
                else
                {
                    Category ncate = new Category();
                    ncate.Id = Guid.NewGuid();
                    ncate.Name = name;
                    ncate.AppId = appId;
                    ncate.SubId = appId;
                    ncate.SubTime = DateTime.Now;
                    ncate.Sort = 1;
                    ncate.ParentId = pcategory.Id;
                    ncate.IsDel = false;
                    ncate.CurrentLevel = pcategory.CurrentLevel + 1;
                    ncate.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(ncate);

                    rcategoryId = ncate.Id;
                    contextSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("服务异常。name：{0}，targetId：{1}，appId：{2}", name, targetId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            //更新类目缓存
            UpdateCategoryCache(appId);

            return new ResultDTO { ResultCode = 0, Message = rcategoryId.ToString() };
        }

        /// <summary>
        /// 添加子级类别
        /// </summary>
        /// <param name="name">类目名称</param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="appId">被操作类别ID</param>
        /// <param name="isuse">是否启用 0启用 1停用</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddChildCategory2Ext(string name, System.Guid targetId, Guid appId, string icon, int isuse)
        {
            ResultDTO resultDto = new ResultDTO() { ResultCode = 0, Message = "添加成功！" };
            Guid rcategoryId;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category pcategory = Category.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.AppId == appId);
                if (pcategory == null)
                {
                    resultDto.ResultCode = 1;
                    resultDto.Message = "父级分类不存在！";
                    return resultDto;
                }
                var currentLevelCategoryList = Category.ObjectSet().Where(n => n.ParentId == targetId && n.AppId == appId && n.IsDel == false).OrderByDescending(n => n.Sort);
                int count = currentLevelCategoryList.Count(t => t.Name == name);
                if (count > 0)
                {
                    resultDto.ResultCode = 1;
                    resultDto.Message = "同级目录下分类名称不能重复！";
                    return resultDto;
                }
                Category category = currentLevelCategoryList.FirstOrDefault();
                Category ncate = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    AppId = appId,
                    SubId = appId,
                    SubTime = DateTime.Now,
                    ParentId = pcategory.Id,
                    IsDel = false,
                    CurrentLevel = pcategory.CurrentLevel + 1,
                    icon = icon,
                    IsUse = isuse == 0
                };
                if (category != null)
                {
                    ncate.Sort = category.Sort + 1;
                }
                else
                {
                    ncate.Sort = 1;
                }
                ncate.EntityState = EntityState.Added;
                contextSession.SaveObject(ncate);

                rcategoryId = ncate.Id;
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("服务异常。name：{0}，targetId：{1}，appId：{2}", name, targetId, appId), ex);
                resultDto.ResultCode = 1;
                resultDto.Message = "添加失败！";
                return resultDto;
            }
            //更新类目缓存
            UpdateCategoryCache(appId);
            return resultDto;
        }

        /// <summary>
        /// 创建初始类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatCategoryExt(System.Guid appId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category fcategory = new Category();
                fcategory.Id = Guid.NewGuid();
                fcategory.Name = "一级类目";
                fcategory.SubId = appId;
                fcategory.AppId = appId;
                fcategory.SubTime = DateTime.Now;
                fcategory.Sort = 1;
                fcategory.CurrentLevel = 0;
                fcategory.IsDel = false;
                fcategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(fcategory);
                Category scategory = new Category();
                scategory.Id = Guid.NewGuid();
                scategory.Name = "二级类目";
                scategory.SubId = appId;
                scategory.AppId = appId;
                scategory.SubTime = DateTime.Now;
                scategory.Sort = 1;
                scategory.CurrentLevel = 1;
                scategory.IsDel = false;
                scategory.ParentId = fcategory.Id;
                scategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(scategory);
                Category tcategory = new Category();
                tcategory.Id = Guid.NewGuid();
                tcategory.Name = "三级类目";
                tcategory.SubId = appId;
                tcategory.AppId = appId;
                tcategory.SubTime = DateTime.Now;
                tcategory.Sort = 1;
                tcategory.CurrentLevel = 2;
                tcategory.IsDel = false;
                tcategory.ParentId = scategory.Id;
                tcategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(tcategory);
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
        /// 创建初始类别（三级分类）
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreatCategory2Ext(System.Guid appId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Category fcategory = new Category();
                fcategory.Id = Guid.NewGuid();
                fcategory.Name = "根目录";
                fcategory.SubId = appId;
                fcategory.AppId = appId;
                fcategory.SubTime = DateTime.Now;
                fcategory.Sort = 1;
                fcategory.CurrentLevel = 0;
                fcategory.IsDel = false;
                fcategory.IsUse = true;
                fcategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(fcategory);
                Category scategory = new Category();
                scategory.Id = Guid.NewGuid();
                scategory.Name = "一级类目";
                scategory.SubId = appId;
                scategory.AppId = appId;
                scategory.SubTime = DateTime.Now;
                scategory.Sort = 1;
                scategory.CurrentLevel = 1;
                scategory.IsDel = false;
                scategory.IsUse = true;
                scategory.ParentId = fcategory.Id;
                scategory.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(scategory);
                //Category tcategory = new Category();
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
                //Category focategory = new Category();
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
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CategoryInfo", key, "BTPCache");//删除缓存

                    var categoryDic = GetCategoriesExt(appid);

                    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_CategoryInfo", key, categoryDic, "BTPCache");//添加缓存

                    Jinher.JAP.Common.Loging.LogHelper.Info(string.Format("更新了商品类目缓存appid：{0}", appid));
                });
        }

        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        private List<CategoryCacheDTO> GetCacheCateGories(Guid appid)
        {
            return Category.ObjectSet().Where(a => a.AppId == appid && a.IsDel == false).OrderBy(a => a.Sort).Select(
                a => new CategoryCacheDTO
                {
                    AppId = a.AppId,
                    CurrentLevel = a.CurrentLevel,
                    Id = a.Id,
                    Name = a.Name,
                    ParentId = a.ParentId,
                    Sort = a.Sort
                }).ToList();
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
        /// 保存是否显示菜单标志
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO UpdateIsShowSearchMenuExt(CategorySearchDTO search)
        {
            if (search == null || search.AppId == Guid.Empty)
            {
                return new ResultDTO<bool>() { ResultCode = -1, Message = "入参为空" };
            }
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var appExt = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == search.AppId);
            if (appExt != null)
            {
                appExt.IsShowSearchMenu = search.IsShowSearchMenu;
                appExt.ModifiedOn = DateTime.Now;
                appExt.EntityState = EntityState.Modified;
            }
            else
            {
                appExt = AppExtension.CreateAppExtension();
                appExt.Id = search.AppId;
                appExt.IsShowSearchMenu = search.IsShowSearchMenu;
                contextSession.SaveObject(appExt);
            }
            contextSession.SaveChanges();
            return new ResultDTO<bool>() { ResultCode = 0, Message = "Success" };
        }



        /// <summary>
        /// 获取应用的一级商品分类
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<CategorySDTO> GetCategoryL1Ext(Guid appId)
        {            
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


        public Jinher.AMP.BTP.Deploy.CustomDTO.CategoryS2DTO GetBrandAndAdvertiseExt(System.Guid CategoryID)
        {
            if(CategoryID != null && CategoryID != Guid.Empty)
            {
                return null;
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
                            }

            //添加品类广告
            return resultData;
        }


        /// <summary>
        /// 同步店铺分类
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO InitAppCategoryExt(Guid appId)
        {
            var thiscommodity = Commodity.ObjectSet().Where(o => o.AppId == appId && o.IsDel == false);  //获取本店铺的商品
            var commodityList = thiscommodity.Select(o => o.Id);  //获取商品信息

            var commodity = CommodityCategory.ObjectSet().Where(o => o.AppId == appId && o.IsDel == false);  //删除以前的分类关系

            foreach (var com in commodity)
            {
                com.IsDel = true;
                com.EntityState = EntityState.Modified;
            }

            var categoryID = Category.ObjectSet().Where(o => o.CurrentLevel == 0 && o.IsDel == false && o.AppId == appId).FirstOrDefault().Id;  //不改变根类目只取ID供下级目录更新父ID

            var thirdCategory = CommodityCategory.ObjectSet().Where(o => o.IsDel == false && commodityList.Contains(o.CommodityId) && o.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId).Select(o => (Guid?)o.CategoryId).Distinct();

            var secondCategory = Category.ObjectSet().Where(o => thirdCategory.Contains(o.Id)).Select(o => o.ParentId).Distinct();

            var firstCategory = Category.ObjectSet().Where(o => secondCategory.Contains(o.Id)).Select(o => o.ParentId).Distinct();

            var listCategory = new List<Guid?>();
            
            if (thirdCategory.Count() == 0)
            {
                LogHelper.Info(string.Format("商品未入驻易捷北京店铺:{0},一级分类数量为0",appId));
                return new ResultDTO
                {
                    isSuccess = false,
                    Message = "sucess",
                    ResultCode = 404
                };
            }

            if (secondCategory.Count() == 0)
            {
                LogHelper.Info(string.Format("商品未入驻易捷北京店铺:{0}，二级分类数量为0", appId));
                return new ResultDTO
                {
                    isSuccess = false,
                    Message = "sucess",
                    ResultCode = 404
                };
            }

            if (firstCategory.Count() == 0)
            {
                LogHelper.Info(string.Format("商品未入驻易捷北京店铺:{0},三级分类数量为0", appId));
                return new ResultDTO
                {
                    isSuccess = false,
                    Message = "sucess",
                    ResultCode = 404
                };
            }

            LogHelper.Info(string.Format("商品入驻易捷北京,一级分类数量{0},二级分类数量{1},三级分类数量{2},appId:{3}",firstCategory.Count(),secondCategory.Count(),thirdCategory.Count(),appId));
            listCategory.AddRange(firstCategory);
            listCategory.AddRange(secondCategory);
            listCategory.AddRange(thirdCategory);

            //var categoryList = from n in CommodityCategory.ObjectSet().Where(o => commodityList.Contains(o.CommodityId)) select n.CategoryId;  //获取三级分类ID
            var categoryDel = Category.ObjectSet().Where(o => o.AppId == appId && o.IsDel == false && o.Id != categoryID).Select(o=>o.Id);  //获取要删除的CagegoryId
            //var category

            var categoryYJ = Category.ObjectSet().Where(o => o.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && o.IsDel == false && listCategory.Contains(o.Id)); //获取易捷北京的分类ID
            
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            var commodityListAdd = CommodityCategory.ObjectSet().Where(o=>commodityList.Contains(o.CommodityId) && o.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && o.IsDel == false); //获取易捷对应分类和商品数据

            foreach (var entity in categoryYJ.Where(o=>o.CurrentLevel == 1))
            {
                var newGuid = Guid.NewGuid();
                foreach (var SecondEntity in categoryYJ.Where(o => o.CurrentLevel == 2 && o.ParentId == entity.Id))
                {
                    var secondNewGuid = Guid.NewGuid();
                    foreach (var ThirdEntity in categoryYJ.Where(o => o.CurrentLevel == 3 && o.ParentId == SecondEntity.Id))
                    {
                        var thirdGuid = Guid.NewGuid();
                        foreach (var comEntity in commodityListAdd.Where(o =>o.CategoryId == ThirdEntity.Id))  //添加商品对应新分类
                        {
                            comEntity.CategoryId = thirdGuid;
                            comEntity.AppId = appId;
                            comEntity.Id = Guid.NewGuid();
                            comEntity.EntityState = EntityState.Added;
                            contextSession.SaveObject(comEntity);
                        }

                        ThirdEntity.Id = thirdGuid;
                        ThirdEntity.ParentId = secondNewGuid;
                        ThirdEntity.AppId = appId;
                        ThirdEntity.EntityState = EntityState.Added;
                        contextSession.SaveObject(ThirdEntity);
                    }
                    SecondEntity.Id = secondNewGuid;
                    SecondEntity.ParentId = newGuid;
                    SecondEntity.AppId = appId;
                    SecondEntity.EntityState = EntityState.Added;
                    contextSession.SaveObject(SecondEntity);
                }
                entity.Id = newGuid;
                entity.AppId = appId;
                entity.ParentId = categoryID;  //一级目录设置根目录
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
            }

            foreach (var category in Category.ObjectSet().Where(o => categoryDel.Contains(o.Id)))  //删除指定店铺下的分类
            {
                category.IsDel = true;
                category.EntityState = EntityState.Modified;
            }

            LogHelper.Debug(String.Format("Jinher.AMP.BTP.BP.InitAppCategoryExt:CategoryID :{0}",categoryDel.ToString()));

            contextSession.SaveChanges();
            UpdateCategoryCache(appId);
            var resultDto = new ResultDTO
            {
                isSuccess = true,
                Message = "sucess",
                ResultCode = 200
            };

            return resultDto;

        }

    }
}
