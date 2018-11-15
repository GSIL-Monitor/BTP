
/***************
功能描述: BTPSV
作    者: 
创建时间: 2015/1/8 9:01:56
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.FSP.ISV.Facade;
//using Jinher.AMP.ZPH.ISV.Facade;
using Jinher.AMP.ZPH.Deploy.CustomDTO;
using Jinher.AMP.ZPH.ISV.Facade;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using System.Data.SqlClient;
using Jinher.AMP.App.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Cache;
using Jinher.AMP.BTP.Common;
using AppSetAppDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO;
using AppSetSearchDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO;
using ComdtyAppInfoCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyAppInfoCDTO;
using CommodityListCDTO = Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AppSetSV : BaseSv, IAppSet
    {

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="qryCommodityDTO"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetCommodityListExt(Jinher.AMP.BTP.Deploy.CustomDTO.QryCommodityDTO qryCommodityDTO)
        {
            if (qryCommodityDTO == null)
            {
                return new List<CommodityListCDTO>();
            }

            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;
            Jinher.AMP.ZPH.Deploy.CustomDTO.QryCommodityDTO query = new ZPH.Deploy.CustomDTO.QryCommodityDTO()
            {
                SetCategoryId = qryCommodityDTO.SetCategoryId,
                PageIndex = qryCommodityDTO.PageIndex,
                PageSize = qryCommodityDTO.PageSize,
                FieldSort = qryCommodityDTO.FieldSort,
                Order = qryCommodityDTO.Order,
                IsHasStock = qryCommodityDTO.IsHasStock,
                MinPrice = qryCommodityDTO.MinPrice,
                MaxPrice = qryCommodityDTO.MaxPrice,
                ChannelId = qryCommodityDTO.ChannelId,
                AppId = qryCommodityDTO.AppId
            };
            try
            {
                result = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
                List<Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityListCDTO> comListReuslt = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetCommodityList(query);
                //类转换一下
                foreach (Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityListCDTO comTmp in comListReuslt)
                {
                    Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO comTmpBTP = new CommodityListCDTO();
                    comTmpBTP.Name = comTmp.Name;
                    comTmpBTP.Price = comTmp.Price;
                    comTmpBTP.Id = comTmp.Id;
                    comTmpBTP.Pic = comTmp.Pic;
                    comTmpBTP.Intensity = comTmp.Intensity;
                    comTmpBTP.State = comTmp.State;
                    comTmpBTP.Stock = comTmp.Stock;
                    comTmpBTP.DiscountPrice = comTmp.DiscountPrice;
                    comTmpBTP.LimitBuyEach = comTmp.LimitBuyEach;
                    comTmpBTP.LimitBuyTotal = comTmp.LimitBuyTotal;
                    comTmpBTP.SurplusLimitBuyTotal = comTmp.SurplusLimitBuyTotal;
                    comTmpBTP.AppId = comTmp.AppId;
                    comTmpBTP.IsActiveCrowdfunding = comTmp.IsActiveCrowdfunding;
                    comTmpBTP.AppName = comTmp.AppName;
                    comTmpBTP.MarketPrice = comTmp.MarketPrice;
                    comTmpBTP.IsEnableSelfTake = comTmp.IsEnableSelfTake;
                    result.Add(comTmpBTP);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品列表接口异常。qryCommodityDTO：{0}", JsonHelper.JsonSerializer(qryCommodityDTO)), ex);
                return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
            }
        }

        /// <summary>
        /// 获取商品分类列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryExt(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                appId = CustomConfig.ZPHAppId;
            }
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> result1;
            try
            {
                result1 = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO>();
                List<Jinher.AMP.ZPH.Deploy.CustomDTO.CategorySDTO> cateResult = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetCategory(appId);
                if (cateResult == null || cateResult.Count < 1)
                {
                    return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO>();
                }

                //一个三级循环取数据 
                foreach (Jinher.AMP.ZPH.Deploy.CustomDTO.CategorySDTO firstTmp in cateResult)
                {
                    if (firstTmp != null && firstTmp.SecondCategory != null && firstTmp.SecondCategory.Count > 0)
                    {
                        var result2 = new List<Jinher.AMP.BTP.Deploy.CustomDTO.SCategorySDTO>();
                        foreach (Jinher.AMP.ZPH.Deploy.CustomDTO.CategorySDTO secondTmp in firstTmp.SecondCategory)
                        {
                            if (secondTmp != null && secondTmp.SecondCategory != null && secondTmp.SecondCategory.Count > 0)
                            {

                                var result3 = new List<Jinher.AMP.BTP.Deploy.CustomDTO.TCategorySDTO>();
                                foreach (Jinher.AMP.ZPH.Deploy.CustomDTO.CategorySDTO thirdTmp in secondTmp.SecondCategory)
                                {
                                    if (thirdTmp != null)
                                    {
                                        var tmp3 = new Jinher.AMP.BTP.Deploy.CustomDTO.TCategorySDTO()
                                        {
                                            Id = thirdTmp.Id,
                                            Name = thirdTmp.Name,
                                            ParentId = secondTmp.Id,
                                            CurrentLevel = thirdTmp.CurrentLevel,
                                            Sort = thirdTmp.Sort,
                                            PicturesPath = thirdTmp.PicturesPath
                                        };
                                        result3.Add(tmp3);
                                    }
                                }
                                var tmp2 = new Jinher.AMP.BTP.Deploy.CustomDTO.SCategorySDTO()
                                {
                                    Id = secondTmp.Id,
                                    Name = secondTmp.Name,
                                    ParentId = firstTmp.Id,
                                    CurrentLevel = secondTmp.CurrentLevel,
                                    Sort = secondTmp.Sort,
                                    ThirdCategory = result3,
                                    PicturesPath = secondTmp.PicturesPath
                                };
                                result2.Add(tmp2);
                            }
                            else
                            {
                                var tmp2 = new Jinher.AMP.BTP.Deploy.CustomDTO.SCategorySDTO()
                                {
                                    Id = secondTmp.Id,
                                    Name = secondTmp.Name,
                                    ParentId = firstTmp.Id,
                                    CurrentLevel = secondTmp.CurrentLevel,
                                    Sort = secondTmp.Sort,
                                    ThirdCategory = new List<TCategorySDTO>(),
                                    PicturesPath = secondTmp.PicturesPath
                                };
                                result2.Add(tmp2);
                            }
                        }

                        var tmp1 = new Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO()
                        {
                            Id = firstTmp.Id,
                            Name = firstTmp.Name,
                            ParentId = firstTmp.ParentId,
                            CurrentLevel = firstTmp.CurrentLevel,
                            Sort = firstTmp.Sort,
                            SecondCategory = result2,
                            PicturesPath = firstTmp.PicturesPath
                        };
                        result1.Add(tmp1);
                    }
                    else
                    {
                        var tmp1 = new Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO()
                        {
                            Id = firstTmp.Id,
                            Name = firstTmp.Name,
                            ParentId = firstTmp.ParentId,
                            CurrentLevel = firstTmp.CurrentLevel,
                            Sort = firstTmp.Sort,
                            SecondCategory = new List<SCategorySDTO>(),
                            PicturesPath = firstTmp.PicturesPath
                        };
                        result1.Add(tmp1);
                    }
                }
                return result1;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取商品分类列表接口异常。appId：{0}", appId), ex);
                return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO>();
            }
        }

        /// <summary>
        /// 按关键字获取商品列表
        /// </summary>
        /// <param name="want">关键字</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetWantCommodityExt(string want, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(want) || pageIndex < 1 || pageSize < 1)
            {
                return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
            }

            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> result;

            try
            {
                result = new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
                List<Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityListCDTO> comListReuslt = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetWantCommodity(want, pageIndex, pageSize);
                //类转换一下
                foreach (Jinher.AMP.ZPH.Deploy.CustomDTO.CommodityListCDTO comTmp in comListReuslt)
                {
                    result.Add((Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO)comTmp);
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("按关键字获取商品列表接口异常。want：{0}，pageIndex：{1}，pageSize：{2}", want, pageIndex, pageSize), ex);
                return new List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO>();
            }
        }

        /// <summary>
        /// 厂家直营app查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO GetAppSetExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppSetSearchDTO search)
        {
            if (search == null)
            {
                return new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO();
            }

            Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO result;
            Jinher.AMP.ZPH.Deploy.CustomDTO.AppSetSearchDTO query = new ZPH.Deploy.CustomDTO.AppSetSearchDTO()
            {
                CategoryId = search.CategoryId,
                pageIndex = search.PageIndex,
                pageSize = search.PageSize
            };
            try
            {
                result = new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO();
                var comListReuslt = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetPavilionApp(new QueryPavilionAppParam { Id = CustomConfig.ZPHAppId, pageIndex = search.PageIndex, pageSize = search.PageSize });
                if (comListReuslt == null || comListReuslt.Data == null || !comListReuslt.Data.Any())
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO();

                //类转换一下
                result.TotalAppCount = comListReuslt.Data.Count;

                result.AppList = new List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO>();
                foreach (var tmpAppSetAppZPH in comListReuslt.Data)
                {
                    if (tmpAppSetAppZPH != null)
                    {
                        Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO tmp = new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO()
                        {
                            AppId = tmpAppSetAppZPH.appId,
                            AppName = tmpAppSetAppZPH.appName,
                            AppCreateOn = tmpAppSetAppZPH.appCreateOn,
                            AppIcon = tmpAppSetAppZPH.appIcon,
                            IsAddToAppSet = true
                        };
                        result.AppList.Add(tmp);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("厂家直营app查询接口异常。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO();
            }
        }

        /// <summary>
        /// 根据分类Id获取该分类下的app列表
        /// </summary>
        /// <param name="search"></param>
        public List<AppSetAppDTO> GetCategoryAppListExt(AppSetSearchDTO search)
        {
            if (search == null)
            {
                return new List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO>();
            }

            List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO> result;
            Jinher.AMP.ZPH.Deploy.CustomDTO.AppSetSearchDTO query = new ZPH.Deploy.CustomDTO.AppSetSearchDTO()
            {
                CategoryId = search.CategoryId
            };
            try
            {
                result = new List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO>();
                List<Jinher.AMP.ZPH.Deploy.CustomDTO.AppSetAppDTO> comListReuslt = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetCategoryAppList(query);
                //类转换一下
                if (comListReuslt != null)
                {
                    foreach (Jinher.AMP.ZPH.Deploy.CustomDTO.AppSetAppDTO tmpAppSetAppZPH in comListReuslt)
                    {
                        if (tmpAppSetAppZPH != null)
                        {
                            Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO tmp = new Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO()
                            {
                                AppId = tmpAppSetAppZPH.AppId,
                                AppName = tmpAppSetAppZPH.AppName,
                                AppCreateOn = tmpAppSetAppZPH.AppCreateOn,
                                AppIcon = tmpAppSetAppZPH.AppIcon,
                                IsAddToAppSet = tmpAppSetAppZPH.IsAddToAppSet
                            };
                            result.Add(tmp);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据分类Id获取该分类下的app列表接口异常。search：{0}", JsonHelper.JsonSerializer(search)), ex);
                return new List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppDTO>();
            }
        }

        /// <summary>
        /// 获取正品会“我的”，各栏目数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfoCountDTO GetUserInfoCountExt(Guid userId, Guid esAppId)
        {
            try
            {
                UserInfoCountDTO result = new UserInfoCountDTO() { UserId = userId, EsAppId = esAppId };

                #region 订单
                CommodityOrderSV commodityOrderSV = new CommodityOrderSV();

                var orderCount = commodityOrderSV.GetOrderCountExt(userId, esAppId);
                if (orderCount != null)
                {
                    result.OrderTotalState0 = orderCount.OrderTotalState0;
                    result.OrderTotalState1 = orderCount.OrderTotalState1;
                    result.OrderTotalState2 = orderCount.OrderTotalState2;
                    result.OrderTotalState3 = orderCount.OrderTotalState3;
                    result.OrderTotalStateTui = orderCount.OrderTotalStateTui;
                }
                #endregion

                #region 收藏


                result.ColCommodityCnt = (from com in Commodity.ObjectSet()
                                          join col in SetCollection.ObjectSet() on com.Id equals col.ColKey
                                          where col.UserId == userId && col.ColType == 1 && com.State == 0 && !com.IsDel && com.CommodityType == 0 && col.ChannelId == esAppId
                                          select com.Id).Count();

                result.ColAppCnt = SetCollection.ObjectSet().Count(c => c.UserId == userId && c.ColType == 2 && c.ChannelId == esAppId);
                #endregion

                #region 预约

                try
                {
                    result.ForespeakCnt = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetMyPresellComdtyNum(userId, esAppId);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("AppSetSV.GetUserInfoCountExt。获取我的预约数量异常。 userId：{0}", userId), ex);
                }
                #endregion

                #region 金币
                result.Gold = FSPSV.Instance.GetBalance(userId);
                #endregion

                #region 浏览记录

                try
                {
                    string sql = "select count(distinct CommodityId) from UserBrowseList where appid='" + esAppId.ToString() + "' and UserId='" + userId.ToString() + "'";

                    int BrowseCount = Convert.ToInt32(Jinher.AMP.BTP.Common.SQLHelper.ExecuteScalar(SQLHelper.UserBrowse, CommandType.Text, sql, null).ToString());
                    result.BrowseCount = BrowseCount;
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("AppSetSV.GetUserInfoCountExt。获取浏览记录失败。 userId：{0}，esAppId：{1}", userId, esAppId), ex);
                    result.BrowseCount = 0;
                    throw;
                }

                #endregion

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AppSetSV.GetUserInfoCountExt异常：userId:{0}", userId), ex);
                return new UserInfoCountDTO { UserId = userId, BrowseCount = -1 };
            }
        }
        /// <summary>
        /// 浏览过的店铺（20个）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<AppIdNameIconDTO> GetBrowseAppInfoExt(Guid userId, Guid appId)
        {
            try
            {
                //获取浏览记录商品ID
                string sql = "select CommodityId from UserBrowseList where appid='" + appId + "' and UserId='" + userId + "' order by InsertDate desc";

                DataTable UserBrowseList = Jinher.AMP.BTP.Common.SQLHelper.ExecuteDataset(SQLHelper.UserBrowse, CommandType.Text, sql, null).Tables[0];
                List<Guid> ids = (from DataRow item in UserBrowseList.Rows select Guid.Parse(item["CommodityId"].ToString())).ToList();

                //获取app下的APP
                //Jinher.AMP.ZPH.ISV.Facade.AppPavilionFacade apppa=new AppPavilionFacade();
                //ReturnInfo<List<Jinher.AMP.ZPH.Deploy.CustomDTO.PavilionAppListCDTO>> list= apppa.GetPavilionApp(new Jinher.AMP.ZPH.Deploy.CustomDTO.QueryPavilionAppParam(){Id = appId,pageIndex = 1,pageSize =100,srcAppId = appId})
                //获取店铺ID
                var info = from comm in BE.Commodity.ObjectSet()
                           join id in ids on comm.Id equals id
                           where ids.Contains(comm.Id)
                           select comm.AppId;
                LogHelper.Info(string.Format("获取浏览过的店铺id：count:{0}", info.ToList().Count));
                //获取浏览过的店铺信息
                List<AppIdNameIconDTO> listapp = APPSV.GetAppListByIds(info.ToList()).Distinct().Take(20).ToList();
                LogHelper.Info(string.Format("获取浏览过的店铺信息：count:{0}", listapp.Count));
                return listapp;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AppSetSV.GetBrowseAppInfoExt异常：userId:{0},appId:{1}", userId, appId), ex);
                throw;
            }

        }
        /// <summary>
        /// 分页获取浏览商品记录
        /// </summary>
        /// <param name="par"></param>
        public List<BTP.Deploy.CustomDTO.CommodityListCDTO> GetBrowseCommdityExt(BrowseParameter par)
        {
            try
            {
                string sql = @"SELECT CommodityId 
                               FROM (
	                                select CommodityId,ROW_NUMBER() OVER(ORDER BY CommodityId desc) AS numb     
		                                from 
			                                (select distinct(CommodityId)
			                                 from UserBrowseList  
                                             where appid='" + par.appId + "' and UserId='" + par.userId + @"' 
                                             ) as y 
                                    ) 
                                    as [browse] 
                               WHERE [browse].numb > ((@pageindex)-1)*(@pagesize) AND [browse].numb<=(@pageindex)*(@pagesize)";
                SqlParameter[] pms = new SqlParameter[]
                    {
                        new SqlParameter("@pageindex",SqlDbType.Int){Value=par.PageIndex},
                        new SqlParameter("@pagesize",SqlDbType.Int){Value=par.PageSize}
                    };
                DataTable UserBrowseList = Jinher.AMP.BTP.Common.SQLHelper.ExecuteDataset(SQLHelper.UserBrowse, CommandType.Text, sql, pms).Tables[0];
                LogHelper.Info(string.Format("获取浏览记录个数：{0}", UserBrowseList.Rows.Count));
                List<Guid> ids = (from DataRow item in UserBrowseList.Rows select Guid.Parse(item["CommodityId"].ToString())).ToList();
                LogHelper.Info(string.Format("获取浏览记录ID个数：{0}", ids.Count));
                var commodityList = (from c in Commodity.ObjectSet()
                                     where ids.Contains(c.Id)
                                     orderby c.State, c.SubTime descending
                                     select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO
                                     {
                                         Id = c.Id,
                                         Pic = c.PicturesPath,
                                         Price = c.Price,
                                         RealPrice = c.Price,
                                         Intensity = 10,
                                         DiscountPrice = -1,
                                         State = c.State,
                                         Stock = c.Stock,
                                         Name = c.Name,
                                         AppId = c.AppId,
                                         MarketPrice = c.MarketPrice,
                                         IsEnableSelfTake = c.IsEnableSelfTake,
                                         ComAttribute = c.ComAttribute,
                                         ComAttrType = (c.ComAttribute == "[]" || c.ComAttribute == null) ? 1 : 3
                                     }).ToList();

                //Jinher.AMP.BTP.ISV.Facade.CommodityFacade facade = new Jinher.AMP.BTP.ISV.Facade.CommodityFacade();
                //List<BTP.Deploy.CustomDTO.CommodityListCDTO> BTPComdty = facade.GetCommodityByIdsWithPreSell(ids, true);
                //LogHelper.Info(string.Format("获取浏览记录调用接口后个数：{0}", BTPComdty.Count));
                return commodityList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AppSetSV.GetBrowseCommdityExt异常：userId:{0},appId:{1}", par.userId, par.appId), ex);
                throw;
            }


        }
        /// <summary>
        /// 删除商品浏览记录
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="UserId"></param>
        /// <param name="CommdityId"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO DeletebrowseCommdityExt(Guid AppId, Guid UserId, Guid CommdityId)
        {
            try
            {
                string sql = "delete from UserBrowseList where  UserId='" + UserId + "' and CommodityId='" + CommdityId + "'";

                var info = Jinher.AMP.BTP.Common.SQLHelper.ExecuteNonQuery(SQLHelper.UserBrowse, CommandType.Text, sql, null);
                if (info > 0)
                {
                    return new ResultDTO() { isSuccess = true, Message = "操作成功", ResultCode = info };
                }
                else
                {
                    LogHelper.Error(string.Format("AppSetSV.DeletebrowseCommdityExt删除失败：userId:{0},appId:{1},CommdityId:{2}", UserId, AppId, CommdityId));
                    return new ResultDTO() { isSuccess = false, Message = "操作失败", ResultCode = info };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("AppSetSV.DeletebrowseCommdityExt异常：userId:{0},appId:{1},CommdityId:{2}", UserId, AppId, CommdityId), ex);
                return new ResultDTO() { isSuccess = false, Message = "服务器异常", ResultCode = 0 };
                throw;
            }


        }



        /// <summary>
        /// 清理正品会APP缓存
        /// </summary>
        /// <returns>结果</returns>
        public ResultDTO RemoveAppInZPHCacheExt()
        {
            ResultDTO result = new ResultDTO { ResultCode = 1, Message = "" };

            try
            {
                Jinher.JAP.Cache.GlobalCacheWrapper.RemoveCache(RedisKeyConst.AppInZPH, "BTPCache", CacheTypeEnum.redisSS);
                result = new ResultDTO { ResultCode = 0, Message = "清理成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("AppSetSV.RemoveAppInZPHCacheExt。", ex);
                result = new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return result;
        }
        private class TempCommodity
        {
            public Commodity Com { get; set; }
            public decimal? newPrice { get; set; }
        }
        private void buildShowPrice(CommodityListCDTO commodity, List<Deploy.CommodityStockDTO> comStocks,
                                    TodayPromotion todayPromotion)
        {
            if (commodity == null)
                return;
            commodity.RealPrice = commodity.Price;
            commodity.OriPrice = commodity.MarketPrice;
            if (todayPromotion != null)
            {
                commodity.RealPrice = todayPromotion.DiscountPrice > 0
                                          ? todayPromotion.DiscountPrice.Value
                                          : Math.Round(commodity.Price * todayPromotion.Intensity / 10.0m, 2);
                commodity.OriPrice = commodity.Price;
            }
            else
            {
                if (comStocks != null && comStocks.Any())
                {
                    var minMarkPriceStock =
                        comStocks.Where(c => c.MarketPrice.HasValue).OrderBy(c => c.MarketPrice).FirstOrDefault();
                    if (minMarkPriceStock != null)
                        commodity.OriPrice = comStocks.Min(c => c.MarketPrice);
                }
            }
        }
        /// <summary>
        /// 获取商品列表（平台获取平台商品、店铺获取店铺商品）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
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
                //根据是否平台区分
                bool isPavilion = Jinher.AMP.BTP.TPS.ZPHSV.Instance.IsAppPavilion(search.AppId.Value);
                if (isPavilion)
                {
                    tempOcommodityList = (from cs in CommodityCategory.ObjectSet()
                                          join cate in Category.ObjectSet() on cs.CategoryId equals cate.Id
                                          join c in Commodity.ObjectSet() on cs.CommodityId equals c.Id
                                          join pro in
                                              (
                                                  from query in TodayPromotion.ObjectSet()
                                                  where
                                                      (query.PromotionType != 3 &&
                                                       (query.StartTime <= now || query.PresellStartTime <= now) &&
                                                       query.EndTime > now)
                                                  select query
                                              ) on c.Id equals pro.CommodityId
                                              into todayPros
                                          from promotion in todayPros.DefaultIfEmpty()
                                          where cs.AppId == appId && c.IsDel == false && c.State == 0 && c.CommodityType == 0 && !cate.IsDel
                                          orderby c.Salesvolume descending, c.SubTime descending
                                          select new TempCommodity
                                          {
                                              Com = c,
                                              newPrice = (promotion.Id == null) ? c.Price : (promotion.DiscountPrice > 0 ? promotion.DiscountPrice : c.Price * promotion.Intensity / 10)
                                          });
                }
                else
                {
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
                }

                if (search.MinPrice.HasValue && search.MinPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice >= search.MinPrice);
                }
                if (search.MaxPrice.HasValue && search.MaxPrice != 0)
                {
                    tempOcommodityList = tempOcommodityList.Where(c => c.newPrice <= search.MaxPrice);
                }

                ocommodityList = tempOcommodityList.Select(c => c.Com).Distinct();


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
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> commodityList = null;

                comdtyListResultCDTO.realCount = commoditiesQuery.Count();
                if (isPavilion)
                {
                    commodities = (from c in commoditiesQuery
                                   orderby c.Salesvolume descending, c.SubTime descending
                                   select c).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                }
                else
                {
                    commodities = (from c in commoditiesQuery
                                   orderby c.SortValue
                                   select c).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();
                }

                //commodities = commoditiesQuery.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();


                if (!commodities.Any())
                    return comdtyListResultCDTO;

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

                try
                {
                    var apps = APPSV.GetAppListByIds(appList);
                    if (apps != null && apps.Any())
                    {
                        comdtyListResultCDTO.appInfoList = new List<ComdtyAppInfoCDTO>();
                        foreach (var appInfo in apps)
                        {
                            comdtyListResultCDTO.appInfoList.Add(new ComdtyAppInfoCDTO
                            {
                                appId = appInfo.AppId,
                                appName = appInfo.AppName,
                                icon = appInfo.AppIcon
                            });
                        }

                        foreach (var commodityListCdto in commodityList)
                        {
                            var appInfo = apps.FirstOrDefault(c => c.AppId == commodityListCdto.AppId);
                            if (appInfo != null)
                            {
                                commodityListCdto.AppName = appInfo.AppName;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(string.Format("AppSetSV.GetCommodityListExt,获取app名称错误。appId：{0}", appId), ex);
                }

                comdtyListResultCDTO.comdtyList = commodityList;

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
    }
}
