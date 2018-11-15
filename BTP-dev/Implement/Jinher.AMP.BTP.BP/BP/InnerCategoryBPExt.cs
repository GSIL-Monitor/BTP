
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2017/9/16 13:47:14
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BP
{
    public partial class InnerCategoryBP : BaseBP, IInnerCategory
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
                InnerCategory target = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                if (target == null)
                    return new ResultDTO { ResultCode = 1, Message = "Error" };

                List<InnerCategory> targetlist = new List<InnerCategory>();
                if (target.CurrentLevel == 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "不能在根结点添加同级" };
                }
                else if (target.CurrentLevel == 1)
                {
                    targetlist = InnerCategory.ObjectSet().Where(n => n.CurrentLevel == 1 && n.Sort > target.Sort && n.IsDel == false).ToList();
                }
                else
                {
                    targetlist = InnerCategory.ObjectSet().Where(n => n.ParentId == target.ParentId && n.Sort > target.Sort && n.IsDel == false).ToList();
                }
                foreach (InnerCategory temp in targetlist)
                {
                    temp.Sort += 1;
                    temp.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp);
                }

                InnerCategory categorydto = new InnerCategory();
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
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<CategoryS2DTO> GetCategories2Ext(System.Guid appId)
        {
            //读取缓存
            List<CategoryS2DTO> storeCache = Jinher.JAP.Cache.GlobalCacheWrapper.GetData("G_InnerCategory2Info", appId.ToString(), "BTPCache") as List<CategoryS2DTO>;

            if (storeCache != null && storeCache.Count > 0)
            {
                return storeCache;
            }

            //所有的类目列表
            List<CategoryS2DTO> categorylist = new List<CategoryS2DTO>();

            //获取类目信息
            var category = InnerCategory.ObjectSet().Where(n => n.AppId == appId && n.IsDel == false).OrderBy(n => n.Sort);
            var query = from n in category
                        orderby n.Sort
                        select new CategoryS2DTO
                        {
                            CurrentLevel = n.CurrentLevel,
                            Id = n.Id,
                            Name = n.Name,
                            ParentId = n.ParentId,
                            Sort = n.Sort,
                            Code = n.Code
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
                                                        Code = second.Code
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
                                                 Code = third.Code
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
                                                             Code = four.Code
                                                         }).ToList();
                                                });
                                        }
                                    });
                            }
                        }
                    });

                //Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_InnerCategory2Info", appId.ToString(), dics[0].ToList(), "BTPCache");
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
                InnerCategory cate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId);
                if (cate == null)
                    return new ResultDTO { ResultCode = 1, Message = "此分类下有商品存在，不能删除" };
                List<InnerCategory> catelist1 = new List<InnerCategory>();
                if (cate.CurrentLevel == 0)
                {
                    catelist1 = InnerCategory.ObjectSet().Where(n => n.CurrentLevel == 0 && n.Sort > cate.Sort && n.IsDel == false).ToList();
                }
                else
                {
                    catelist1 = InnerCategory.ObjectSet().Where(n => n.ParentId == cate.ParentId && n.Sort > cate.Sort && n.IsDel == false).ToList();
                }
                foreach (InnerCategory temp in catelist1)
                {
                    temp.Sort -= 1;
                    temp.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp);
                }
                string myID = myId.ToString();

                //获取类目以及其所有子类目，并设置状态为删除

                var ids = InnerCategory.ObjectSet().Where(n => n.ParentId == myId).Select(n => n.Id);
                List<InnerCategory> catelist2 = InnerCategory.ObjectSet().
                    Where(n => (n.Id == myId || ids.Contains(n.Id) || (n.ParentId.HasValue && ids.Contains(n.ParentId.Value))) && n.IsDel == false).ToList();

                foreach (InnerCategory temp1 in catelist2)
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
                InnerCategory cate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategory2Ext(System.Guid appId, string name, Guid myId, string code)
        {
            if (appId == Guid.Empty || string.IsNullOrWhiteSpace(name) || myId == Guid.Empty)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            }
            try
            {
                InnerCategory cate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (cate == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "数据库中不存在此类别" };
                }
                var currentLevelCategoryList = InnerCategory.ObjectSet().Where(n => n.ParentId == cate.ParentId && n.AppId == appId && n.IsDel == false && n.Id != cate.Id);
                int count = currentLevelCategoryList.Count(t => t.Name == name);
                if (count > 0)
                {
                    return new ResultDTO { ResultCode = 1, Message = "同级目录下分类名称不能重复" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                cate.EntityState = System.Data.EntityState.Modified;
                cate.Name = name;
                cate.Code = code;
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
                InnerCategory targetcate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                InnerCategory mycategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                string idmy = myId.ToString();

                var ids = InnerCategory.ObjectSet().Where(n => n.ParentId == myId).Select(n => n.Id);

                if (targetcate == null || mycategory == null || !ids.Any())
                {
                    return new ResultDTO { ResultCode = 1, Message = "类别不存在" };
                }
                List<InnerCategory> mycatelist = InnerCategory.ObjectSet().
                    Where(n => (ids.Contains(n.Id) || (n.ParentId.HasValue && ids.Contains(n.ParentId.Value))) && n.IsDel == false).ToList();
                foreach (InnerCategory temp in mycatelist)
                {
                    temp.CurrentLevel = temp.CurrentLevel - 1;
                    temp.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp);
                }
                List<InnerCategory> mycatelist1 = new List<InnerCategory>();
                List<InnerCategory> targetcatelist = new List<InnerCategory>();
                if (mycategory.CurrentLevel == 0)
                {
                    mycatelist1 = InnerCategory.ObjectSet().
                        Where(n => n.CurrentLevel == 0 && n.Sort > mycategory.Sort && n.IsDel == false).ToList();
                    targetcatelist = InnerCategory.ObjectSet().
                        Where(n => n.CurrentLevel == 0 && n.Sort > targetcate.Sort && n.IsDel == false).ToList();
                }
                else
                {
                    mycatelist1 = InnerCategory.ObjectSet().
                        Where(n => n.ParentId == mycategory.ParentId && n.Sort > mycategory.Sort && n.IsDel == false).ToList();
                    targetcatelist = InnerCategory.ObjectSet().
                        Where(n => n.ParentId == targetcate.ParentId && n.Sort > targetcate.Sort && n.IsDel == false).ToList();

                }
                foreach (InnerCategory temp2 in mycatelist1)
                {
                    temp2.Sort -= 1;
                    temp2.EntityState = System.Data.EntityState.Modified;
                    contextSession.SaveObject(temp2);
                }
                foreach (InnerCategory temp4 in targetcatelist)
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
                InnerCategory targetcate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                InnerCategory mycategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);

                //获取要移动类目下所有类目
                string myGuid = myId.ToString();
                var ids = InnerCategory.ObjectSet().Where(n => n.ParentId == myId && n.IsDel == false).Select(n => n.Id);
                if (targetcate == null || mycategory == null || !ids.Any())
                {
                    return new ResultDTO { ResultCode = 1, Message = "类别不存在" };
                }
                List<InnerCategory> catelist = InnerCategory.ObjectSet().
                    Where(n => n.Id != myId && (ids.Contains(n.Id) || (n.ParentId.HasValue && ids.Contains(n.ParentId.Value))) && n.IsDel == false).ToList();
                foreach (InnerCategory cate in catelist)
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
                InnerCategory targetcate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                InnerCategory mycategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
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
                InnerCategory targetcate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);
                InnerCategory mycategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
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
                InnerCategory targetcate = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.IsDel == false);

                //获取被操作类别
                InnerCategory mycategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == myId && n.IsDel == false);
                if (targetcate == null || mycategory == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "类别不存在" };
                }
                string mid = mycategory.Id.ToString();
                int b = 0;

                //获取被拖拽类目的所有子节点
                var ids = InnerCategory.ObjectSet().Where(n => n.ParentId == myId).Select(n => n.Id);
                if (!ids.Any())
                {
                    return new ResultDTO { ResultCode = 3, Message = "被拖拽类目的子节点不存在" };
                }
                List<InnerCategory> a = InnerCategory.ObjectSet().
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
                        List<InnerCategory> catelist = new List<InnerCategory>();

                        #region 对后面的节点排序
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }
                        foreach (InnerCategory temp in catelist)
                        {
                            temp.Sort += 1;
                            temp.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp);
                        }
                        #endregion

                        #region 同步子节点和商品对应的类目信息

                        //获取它的子节点
                        List<InnerCategory> mycatelist = new List<InnerCategory>();
                        if (mycategory.CurrentLevel == 1)
                        {
                            //原来
                            //mycatelist = InnerCategory.ObjectSet().Where(n => n.CurrentLevel == 0 && n.IsDel == false).ToList();

                            //修改后
                            mycatelist = InnerCategory.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                        }
                        else
                        {
                            mycatelist = InnerCategory.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
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
                        List<InnerCategory> catelist = new List<InnerCategory>();
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }
                        foreach (InnerCategory temp in catelist)
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
                        List<InnerCategory> catelist = new List<InnerCategory>();
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort >= targetcate.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }
                        foreach (InnerCategory temp in catelist)
                        {
                            temp.Sort += 1;
                            temp.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(temp);
                        }
                        #endregion

                        #region 同步子节点和商品对应的类目信息
                        List<InnerCategory> mycatelist = new List<InnerCategory>();
                        if (mycategory.CurrentLevel == 1)
                        {
                            //修改前(我不明白他为什么要这么做，但感觉是错的)
                            //mycatelist = InnerCategory.ObjectSet().Where(n => n.CurrentLevel == 0 && n.IsDel == false).ToList();

                            //修改后
                            mycatelist = InnerCategory.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                        }
                        else
                        {
                            mycatelist = InnerCategory.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
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
                        List<InnerCategory> catelist = new List<InnerCategory>();
                        if (targetcate.CurrentLevel == 1)
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.CurrentLevel == 1 && n.IsDel == false).ToList();
                        }
                        else
                        {
                            catelist = InnerCategory.ObjectSet().Where(n => n.Sort > targetcate.Sort && n.Sort < mycategory.Sort && n.ParentId == targetcate.ParentId && n.IsDel == false).ToList();
                        }

                        foreach (InnerCategory temp in catelist)
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
                    //InnerCategory nexttarget = InnerCategory.ObjectSet().Where(n => n.ParentId == targetId && n.IsDel == false).FirstOrDefault();
                    //修改
                    List<InnerCategory> nexttarget = InnerCategory.ObjectSet().Where(n => n.ParentId == targetId && n.IsDel == false).ToList();
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

                    List<InnerCategory> catelist = InnerCategory.ObjectSet().Where(n => n.ParentId == mycategory.Id && n.IsDel == false).ToList();
                    foreach (InnerCategory cate in catelist)
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
                InnerCategory pcategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.AppId == appId);
                if (pcategory == null)
                {
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                }
                InnerCategory category = InnerCategory.ObjectSet().Where(n => n.ParentId == targetId && n.AppId == appId && n.IsDel == false).OrderByDescending(n => n.Sort).FirstOrDefault();
                if (category != null)
                {
                    InnerCategory ncate = new InnerCategory();
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
                    InnerCategory ncate = new InnerCategory();
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
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddChildCategory2Ext(string name, System.Guid targetId, Guid appId, string code)
        {
            ResultDTO resultDto = new ResultDTO() { ResultCode = 0, Message = "添加成功！" };
            Guid rcategoryId;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                InnerCategory pcategory = InnerCategory.ObjectSet().FirstOrDefault(n => n.Id == targetId && n.AppId == appId);
                if (pcategory == null)
                {
                    resultDto.ResultCode = 1;
                    resultDto.Message = "父级分类不存在！";
                    return resultDto;
                }
                var currentLevelCategoryList = InnerCategory.ObjectSet().Where(n => n.ParentId == targetId && n.AppId == appId && n.IsDel == false).OrderByDescending(n => n.Sort);
                int count = currentLevelCategoryList.Count(t => t.Name == name);
                if (count > 0)
                {
                    resultDto.ResultCode = 1;
                    resultDto.Message = "同级目录下分类名称不能重复！";
                    return resultDto;
                }
                InnerCategory category = currentLevelCategoryList.FirstOrDefault();
                InnerCategory ncate = new InnerCategory
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    AppId = appId,
                    SubId = appId,
                    SubTime = DateTime.Now,
                    ParentId = pcategory.Id,
                    IsDel = false,
                    CurrentLevel = pcategory.CurrentLevel + 1,
                    Code = code
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
                InnerCategory fcategory = new InnerCategory();
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
                InnerCategory scategory = new InnerCategory();
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
                InnerCategory tcategory = new InnerCategory();
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

        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        private List<CategoryCacheDTO> GetCacheCateGories(Guid appid)
        {
            return InnerCategory.ObjectSet().Where(a => a.AppId == appid && a.IsDel == false).OrderBy(a => a.Sort).Select(
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
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetZshCategoriesExt()
        {
            var innerCatelist = GetCategoriesExt(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
            if (innerCatelist.Count() == 0)
            {
                CreatCategory2Ext(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                GetCategoriesExt(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
            }
            return innerCatelist;
        }
    }
}
