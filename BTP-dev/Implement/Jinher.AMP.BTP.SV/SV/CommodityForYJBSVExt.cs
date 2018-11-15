/***************
功能描述: BTPSV
作    者: LSH
创建时间: 2017/9/14 14:49:43
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.SV
{
    public partial class CommodityForYJBSV : BaseSv, ICommodityForYJB
    {
        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public ResultDTO<ListResult<CommodityListOutPut>> GetCommoditiesExt(CommoditySearchInput input)
        {
            var result = new ResultDTO<ListResult<CommodityListOutPut>>() { isSuccess = false };
            if (input == null)
            {
                result.ResultCode = (int)ReturnCodeEnum.ParamEmpty;
                result.Message = "参数错误，参数不能为空！";
                return result;
            }
            //if (!input.EsAppId.HasValue)
            //{
            //    result.ResultCode = (int)ReturnCodeEnum.ParamEmpty;
            //    result.Message = "参数错误，参数不能为空！";
            //    return result;
            //}

            LogHelper.Info("GetCommoditiesExt，Input: " + JsonHelper.JsonSerializer(input));

            List<Tuple<Guid, string>> maList = new List<Tuple<Guid, string>>();

            IQueryable<MallApply> maQ = null;
            if (input.EsAppId.HasValue && input.EsAppId.Value != Guid.Empty)
            {
                maQ = MallApply.GetTGQuery(input.EsAppId.Value);
                if (!string.IsNullOrWhiteSpace(input.AppName))
                {
                    maQ = maQ.Where(ma => ma.AppName.Contains(input.AppName));
                    if (!maQ.Any())
                    {
                        result.Data = new ListResult<CommodityListOutPut> { Count = 0, List = new List<CommodityListOutPut>() };
                        return result;
                    }

                    maList = maQ.Select(ma => new { ma.AppId, ma.AppName }).ToList().ConvertAll(ma => Tuple.Create(ma.AppId, ma.AppName));
                }
                else if (input.AppIds == null || input.AppIds.Count == 0)
                {
                    maQ = maQ.Where(t => t.Type != 1);
                    if (maQ.Any())
                    {
                        maList = maQ.Select(ma => new { ma.AppId, ma.AppName }).ToList().ConvertAll(ma => Tuple.Create(ma.AppId, ma.AppName));
                    }
                }
            }
            else if (input.AppIds != null && input.AppIds.Any())
            {
                var maQx = (from ma in MallApply.ObjectSet()
                            where input.AppIds.Contains(ma.AppId)
                            select ma);
                if (maQx.Any())
                {
                    maList = maQx.Select(ma => new { ma.AppId, ma.AppName }).ToList().ConvertAll(ma => Tuple.Create(ma.AppId, ma.AppName));
                }
            }



            try
            {
                IQueryable<CommodityListOutPut> query = null;

                if (maQ == null)
                {
                    query = from c in Commodity.ObjectSet()
                            where !c.IsDel && c.State == 0
                            select new CommodityListOutPut
                            {
                                AppId = c.AppId,
                                CommodityId = c.Id,
                                CommodityName = c.Name,
                                JdCode = c.JDCode,
                                Pic = c.PicturesPath,
                                Price = c.Price,
                                CostPrice = c.CostPrice,
                                Stock = c.Stock,
                                SubTime = c.SubTime,
                                ModifiedOn = c.ModifiedOn
                            };
                }
                else
                {

                    query = from c in Commodity.ObjectSet()
                            join ma in maQ on c.AppId equals ma.AppId
                            where !c.IsDel && c.State == 0
                            select new CommodityListOutPut
                            {
                                AppId = c.AppId,
                                CommodityId = c.Id,
                                CommodityName = c.Name,
                                JdCode = c.JDCode,
                                Pic = c.PicturesPath,
                                Price = c.Price,
                                CostPrice = c.CostPrice,
                                Stock = c.Stock,
                                SubTime = c.SubTime,
                                ModifiedOn = c.ModifiedOn
                            };
                }

                if (input.AppIds != null)
                {
                    if (input.AppIds.Count == 1)
                    {
                        Guid aid = input.AppIds[0];
                        query = query.Where(_ => aid == _.AppId);
                    }
                    else if (input.AppIds.Count > 1)
                    {
                        query = query.Where(_ => input.AppIds.Contains(_.AppId));
                    }
                }




                if (!string.IsNullOrWhiteSpace(input.CommodityName))
                {
                    query = query.Where(_ => _.CommodityName.Contains(input.CommodityName));
                }

                if (input.LastModificationTime.HasValue)
                {
                    query = query.Where(_ => _.ModifiedOn > input.LastModificationTime);
                }
                if (input.HaveCostPrice.HasValue && input.HaveCostPrice.Value)
                {
                    query = query.Where(_ => _.CostPrice.HasValue);
                }

                #region 增加分类查询条件，传入一级分类，递归查询一级分类下面的所有商品
                //添加商品分类条件          
                if (input.CategoryId != Guid.Empty)
                {
                    var listCategory = CommodityCategory.ObjectSet().Where(O => O.CategoryId == input.CategoryId).Select(c => c.CommodityId);
                    query = query.Where(_ => listCategory.Contains(_.CommodityId));
                }


                if (input.CatgoryIdList != null)
                {
                    List<string> listId1 = new List<string>();
                    input.CatgoryIdList.ForEach(p => { listId1.Add(p.ToString()); });

                    Guid xx = Guid.Empty;

                    if (input.AppIds != null && input.AppIds.Count() > 0)
                    {
                        xx = input.AppIds[0];
                    }
                    else if (input.EsAppId != null && input.EsAppId.HasValue && input.EsAppId != Guid.Empty)
                    {
                        xx = input.EsAppId.Value;
                    }
                    if (xx != Guid.Empty)
                    {
                        List<Guid> listId = GetRecursiveCategoryId(new PresentPromotionCommoditySearchDTO() { Categorys = listId1, AppId = xx });
                        var listCategory = CommodityCategory.ObjectSet().Where(O => listId.Contains(O.CategoryId)).Select(c => c.CommodityId);
                        query = query.Where(_ => listCategory.Contains(_.CommodityId));
                    }
                    else
                        query = query.Where(_ => false);
                }

                if (((input.MinRate > input.MaxRate) && (input.MaxRate != 0)) || ((input.MinPrice > input.MaxPrice) && (input.MaxPrice != 0)))
                    query = query.Where(t => false);
                else
                {
                    if (input.MaxPrice > 0)
                        query = query.Where(t => t.Price <= input.MaxPrice);
                    if (input.MaxRate > 0)
                        query = query.Where(t => (t.CostPrice != null) && ((t.Price - t.CostPrice) / t.Price) * 100 <= input.MaxRate);

                    if (input.MinPrice > 0)
                        query = query.Where(t => t.Price >= input.MinPrice);

                    if (input.MinRate > 0)
                        query = query.Where(t => (t.CostPrice != null) && ((t.Price - t.CostPrice) / t.Price) * 100 >= input.MinRate);
                }


                #endregion

                // 修改为内存中过滤
                bool notExUseMemoryFilter = false;
                bool exUseMemoryFilter = false;
                if (input.NotExistedCommodityId != null && input.NotExistedCommodityId.Count < 1000)
                {
                    query = query.Where(_ => !input.NotExistedCommodityId.Contains(_.CommodityId));
                }
                else
                {
                    notExUseMemoryFilter = input.NotExistedCommodityId != null;
                }

                if (input.ExistedCommodityId != null && input.ExistedCommodityId.Count < 1000)
                {
                    query = query.Where(_ => input.ExistedCommodityId.Contains(_.CommodityId));
                }
                else
                {
                    exUseMemoryFilter = input.ExistedCommodityId != null;
                }


                var count = 0;
                var data = new List<CommodityListOutPut>();
                if (notExUseMemoryFilter || exUseMemoryFilter)
                {
                    if (notExUseMemoryFilter)
                    {
                        query = query.Where(_ => !input.NotExistedCommodityId.Contains(_.CommodityId));
                    }
                    if (exUseMemoryFilter)
                    {
                        query = query.Where(_ => input.ExistedCommodityId.Contains(_.CommodityId));
                    }
                    LogHelper.Debug("GetCommoditiesExt sql:" + LinqHelper.GetEFCommandSql(query));
                    count = query.Count();
                    data = query.OrderByDescending(n => n.SubTime).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList();
                }
                else
                {
                    LogHelper.Debug("GetCommoditiesExt sql:" + LinqHelper.GetEFCommandSql(query));
                    count = query.Count();
                    data = query.OrderByDescending(n => n.SubTime).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList();
                }

                if (count == 0)
                {
                    result.isSuccess = true;
                    result.Data = new ListResult<CommodityListOutPut> { Count = count, List = new List<CommodityListOutPut>() };
                    return result;
                }
                else
                {
                    IEnumerable<Guid> appIdLit = maList.Select(_ => _.Item1).Distinct();
                    if (appIdLit != null && appIdLit.Any())
                    {
                        var suppliers = Supplier.ObjectSet().Where(_ => appIdLit.Contains(_.AppId)).ToList();
                        foreach (var d in data)
                        {
                            Tuple<Guid, string> tupleX = maList.FirstOrDefault(_ => _.Item1 == d.AppId);
                            if (tupleX != null)
                            {
                                d.AppName = tupleX.Item2;
                            }
                            d.SupplierName = string.Join(",", suppliers.Where(_ => _.AppId == d.AppId).Select(_ => _.SupplierName).ToList());
                        }
                    }
                    result.isSuccess = true;
                    result.Data = new ListResult<CommodityListOutPut> { Count = count, List = data };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityForYJBSV.GetCommoditiesExt 异常：", ex);
                result.Message = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <returns>商品列表</returns>
        public CommoditySearchResultDTO GetAllCommoditiesExt(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            if (appId == Guid.Empty)
            {
                return null;
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            try
            {
                // 查询入驻的App
                var appIds = Jinher.AMP.BTP.BE.MallApply.ObjectSet().Where(_ => _.EsAppId == appId && _.State.Value == (int)Jinher.AMP.BTP.Deploy.Enum.MallApplyEnum.TG).Select(_ => _.AppId).Distinct().ToList();
                var apps = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetPavilionApp(new ZPH.Deploy.CustomDTO.QueryPavilionAppParam()
                {
                    Id = appId,
                    pageIndex = 1,
                    pageSize = int.MaxValue
                }).Data;
                if (apps.Count > 0)
                {
                    foreach (var item in apps)
                    {
                        if (!appIds.Contains(appId))
                        {
                            appIds.Add(item.appId);
                        }
                    }
                }
                if (!appIds.Contains(appId))
                {
                    appIds.Add(appId);
                }
                var query = Commodity.ObjectSet().Where(c => !c.IsDel && c.State == 0 && appIds.Contains(c.AppId));

                if (!string.IsNullOrEmpty(commodityCategory))
                {
                    query = query.Where(_ => _.CategoryName.Contains(commodityCategory));
                }
                if (!string.IsNullOrEmpty(commodityName))
                {
                    query = query.Where(_ => _.Name.Contains(commodityName) || commodityName.Contains(_.Name));
                }
                var count = query.Count();
                if (count == 0)
                {
                    return new CommoditySearchResultDTO { TotalCount = count, CommodityList = new List<CommodityListDTO>() };
                }
                var data = query.OrderByDescending(n => n.SubTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).Select(c =>
                        new CommodityListDTO
                        {
                            //AppId = c.AppId,
                            CommodityCategory = c.CategoryName,
                            CommodityId = c.Id,
                            CommodityName = c.Name,
                            CommodityPicture = c.PicturesPath,
                            IsEnableSelfTake = c.IsEnableSelfTake
                        }).ToList();
                return new CommoditySearchResultDTO { TotalCount = count, CommodityList = data };
            }
            catch (Exception ex)
            {
                LogHelper.Error("CommodityForYJBSV.GetAllCommoditiesExt 异常：", ex);
                return new CommoditySearchResultDTO { TotalCount = 0, CommodityList = new List<CommodityListDTO>() };
            }
        }


        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <returns>商品列表</returns>
        //public CommoditySearchResultDTO GetAllCommoditiesExt(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        //{
        //    if (appId == Guid.Empty)
        //    {
        //        return null;
        //    }
        //    if (pageIndex <= 0)
        //    {
        //        pageIndex = 1;
        //    }
        //    if (pageSize == 0)
        //    {
        //        pageSize = 10;
        //    }
        //    try
        //    {
        //        // 查询入驻的App
        //        var apps = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetPavilionApp(new ZPH.Deploy.CustomDTO.QueryPavilionAppParam()
        //        {
        //            Id = appId,
        //            pageIndex = 1,
        //            pageSize = int.MaxValue
        //        }).Data;

        //        var query = Commodity.ObjectSet().Where(c => !c.IsDel && c.State == 0 && c.AppId==appId);

        //        if (!string.IsNullOrEmpty(commodityCategory))
        //        {
        //            query = query.Where(_ => _.CategoryName.Contains(commodityCategory));
        //        }
        //        if (!string.IsNullOrEmpty(commodityName))
        //        {
        //            query = query.Where(_ => _.Name.Contains(commodityName) || commodityName.Contains(_.Name));
        //        }
        //        var count = query.Count();
        //        if (count == 0)
        //        {
        //            return new CommoditySearchResultDTO { TotalCount = count, CommodityList = new List<CommodityListDTO>() };
        //        }
        //        var data = query.OrderByDescending(n => n.SubTime)
        //            .Skip((pageIndex - 1) * pageSize)
        //            .Take(pageSize).Select(c =>
        //                new CommodityListDTO
        //                {
        //                    //AppId = c.AppId,
        //                    CommodityCategory = c.CategoryName,
        //                    CommodityId = c.Id,
        //                    CommodityName = c.Name,
        //                    CommodityPicture = c.PicturesPath,
        //                    IsEnableSelfTake = c.IsEnableSelfTake
        //                }).ToList();
        //        return new CommoditySearchResultDTO { TotalCount = count, CommodityList = data };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("CommodityForYJBSV.GetAllCommoditiesExt 异常：", ex);
        //        return new CommoditySearchResultDTO { TotalCount = 0, CommodityList = new List<CommodityListDTO>() };
        //    }
        //}


        /// <summary>
        /// 根据搜索条件获取商品
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="want">搜索关键字</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public ResultDTO<List<ComAttrDTO>> GetAppIdCommodityExt(string name, System.Guid appId, decimal price, int pageIndex, int pageSize)
        {
            try
            {
                if (appId == Guid.Empty)
                {
                    return null;
                }
                pageSize = pageSize == 0 ? 20 : pageSize;
                var query = Commodity.ObjectSet().Where(data => data.AppId == appId && data.IsDel == false && data.State == 0).AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(data => name.Contains(data.Name) || data.Name.Contains(name));
                }
                if (price != 0)
                {
                    query = query.Where(date => date.Price == price);
                }

                //获取appName
                string AppName = APPSV.GetAppName(appId);
                List<ComAttrDTO> result = (from c in query
                                           select new Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO
                                           {
                                               Id = c.Id,
                                               AppId = c.AppId,
                                               AppName = AppName,
                                               Pic = c.PicturesPath,
                                               Price = c.Price,
                                               Stock = c.Stock,
                                               Name = c.Name
                                           }).ToList();
                ResultDTO<List<ComAttrDTO>> retInfo = new ResultDTO<List<ComAttrDTO>>
                {
                    ResultCode = result.Count(),
                    Data = result.OrderBy(p => p.AppId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。name{0}，appId{1}，price{2} pageIndex{3}，pageSize{4}", name, appId, price, pageIndex, pageSize), ex);
                return null;
            }
        }


        /// <summary>
        /// 根据id显示商品信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> GetCommodityByIdExt(Guid appid, System.Collections.Generic.List<System.Guid> ids, int pageIndex, int pageSize)
        {
            try
            {
                //获取appName
                string AppName = APPSV.GetAppName(appid);
                List<ComAttrDTO> query = (from data in Commodity.ObjectSet()
                                          join att in CommodityStock.ObjectSet() on data.Id equals att.CommodityId
                                          where data.AppId == appid && ids.Contains(att.CommodityId)
                                          select new Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO
                                          {
                                              Id = att.Id,
                                              AppId = data.AppId,
                                              AppName = AppName,
                                              Pic = data.PicturesPath,
                                              Price = att.Price,
                                              Stock = att.Stock,
                                              Name = data.Name,
                                              MarketPrice = att.MarketPrice,
                                              CostPrice = att.CostPrice,
                                              ComAttribute = att.ComAttribute
                                          }).ToList();
                List<ComAttrDTO> query1 = (from com in Commodity.ObjectSet()
                                           where com.AppId == appid && ids.Contains(com.Id)
                                           select new Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO
                                           {
                                               Id = com.Id,
                                               AppId = com.AppId,
                                               AppName = AppName,
                                               Pic = com.PicturesPath,
                                               Price = com.Price,
                                               Stock = com.Stock,
                                               Name = com.Name,
                                               MarketPrice = com.MarketPrice,
                                               CostPrice = com.CostPrice,
                                               ComAttribute = com.ComAttribute
                                           }).ToList();
                List<ComAttrDTO> result = query.Union(query1).Distinct().ToList();
                ResultDTO<List<ComAttrDTO>> retInfo = new ResultDTO<List<ComAttrDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result.OrderBy(p => p.AppId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。appid{0} ids{1}", appid, ids), ex);
                return null;
            }

        }
        /// <summary>
        /// 根据id获取商品信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO> GetCommodityByIdsExt(System.Guid appid, System.Collections.Generic.List<System.Guid> ids)
        {
            try
            {
                List<ComAttrDTO> stockid = (from data in Commodity.ObjectSet()
                                            join att in CommodityStock.ObjectSet() on data.Id equals att.CommodityId
                                            where data.AppId == appid && (ids.Contains(att.CommodityId) || ids.Contains(att.Id))
                                            select new Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO
                                            {
                                                Id = data.Id
                                            }).ToList();
                List<System.Guid> id = new List<Guid>();
                foreach (var item in stockid)
                {
                    id.Add(item.Id);
                }
                List<System.Guid> comid = ids.Except(id).ToList();
                //获取appName
                string AppName = APPSV.GetAppName(appid);
                List<ComAttrDTO> query = (from data in Commodity.ObjectSet()
                                          join att in CommodityStock.ObjectSet() on data.Id equals att.CommodityId
                                          where data.AppId == appid && (ids.Contains(att.CommodityId) || ids.Contains(att.Id))
                                          orderby att.Price
                                          select new Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO
                                          {
                                              Id = att.Id,
                                              AppId = data.AppId,
                                              AppName = AppName,
                                              Pic = data.PicturesPath,
                                              Price = att.Price,
                                              Stock = att.Stock,
                                              Name = data.Name,
                                              MarketPrice = att.MarketPrice,
                                              CostPrice = att.CostPrice,
                                              ComAttribute = att.ComAttribute
                                          }).ToList();

                List<ComAttrDTO> query1 = (from com in Commodity.ObjectSet()
                                           where com.AppId == appid && comid.Contains(com.Id)
                                           select new Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO
                                           {
                                               Id = com.Id,
                                               AppId = com.AppId,
                                               AppName = AppName,
                                               Pic = com.PicturesPath,
                                               Price = com.Price,
                                               Stock = com.Stock,
                                               Name = com.Name,
                                               MarketPrice = com.MarketPrice,
                                               CostPrice = com.CostPrice,
                                               ComAttribute = com.ComAttribute
                                           }).ToList();

                List<ComAttrDTO> result = query.Union(query1).GroupBy(p => p.Id).Select(p => p.FirstOrDefault()).OrderBy(p => p.Name).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("根据搜索条件获取商品异常。appid{0} ids{1}", appid, ids), ex);
                return null;
            }

        }
        /// <summary>
        /// 定时修改商品价格
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityPriceExt(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo)
        {
            try
            {
                // state==0  定时生效价格
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //取出需修改价格的商品信息
                var ComInfo = Commodity.ObjectSet().FirstOrDefault(p => p.Id == CkPriceInfo.CommodityId);
                var ComStockInfo = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == CkPriceInfo.CommodityId);

                List<Guid> ChangeComIds = new List<Guid>();
                List<Guid> ChangeComStockIds = new List<Guid>();
                if (ComInfo != null)
                {
                    if (CkPriceInfo.status == 0)
                    {
                        ComInfo.Price = CkPriceInfo.TargetPrice == null || CkPriceInfo.TargetPrice == 0 ? ComInfo.Price : decimal.Parse(CkPriceInfo.TargetPrice.ToString());
                        ComInfo.MarketPrice = CkPriceInfo.TargetMarketPrice == null || CkPriceInfo.TargetMarketPrice == 0 ? ComInfo.MarketPrice : decimal.Parse(CkPriceInfo.TargetMarketPrice.ToString());
                        ComInfo.CostPrice = CkPriceInfo.TargetCostPrice == null || CkPriceInfo.TargetCostPrice == 0 ? ComInfo.CostPrice : decimal.Parse(CkPriceInfo.TargetCostPrice.ToString());
                    }
                    else if (CkPriceInfo.status == 1)
                    {
                        ComInfo.Price = CkPriceInfo.CurrentPrice == null || CkPriceInfo.CurrentPrice == 0 ? ComInfo.Price : decimal.Parse(CkPriceInfo.CurrentPrice.ToString());
                        ComInfo.MarketPrice = CkPriceInfo.CurrentMarketPrice == null || CkPriceInfo.CurrentMarketPrice == 0 ? ComInfo.MarketPrice : decimal.Parse(CkPriceInfo.CurrentMarketPrice.ToString());
                        ComInfo.CostPrice = CkPriceInfo.CurrentCostPrice == null || CkPriceInfo.CurrentCostPrice == 0 ? ComInfo.CostPrice : decimal.Parse(CkPriceInfo.CurrentCostPrice.ToString());
                    }
                    ComInfo.ModifiedOn = DateTime.Now;
                    ComInfo.RefreshCache(EntityState.Modified);//清楚缓存
                    ChangeComIds.Add(ComInfo.Id);
                }
                if (ComStockInfo != null)
                {
                    if (CkPriceInfo.status == 0)
                    {
                        ComStockInfo.Price = CkPriceInfo.TargetPrice == null || CkPriceInfo.TargetPrice == 0 ? ComStockInfo.Price : decimal.Parse(CkPriceInfo.TargetPrice.ToString());
                        ComStockInfo.MarketPrice = CkPriceInfo.TargetMarketPrice == null || CkPriceInfo.TargetMarketPrice == 0 ? ComStockInfo.MarketPrice : decimal.Parse(CkPriceInfo.TargetMarketPrice.ToString());
                        ComStockInfo.CostPrice = CkPriceInfo.TargetCostPrice == null || CkPriceInfo.TargetCostPrice == 0 ? ComStockInfo.CostPrice : decimal.Parse(CkPriceInfo.TargetCostPrice.ToString());
                    }
                    else if (CkPriceInfo.status == 1)
                    {
                        ComStockInfo.Price = CkPriceInfo.CurrentPrice == null || CkPriceInfo.CurrentPrice == 0 ? ComStockInfo.Price : decimal.Parse(CkPriceInfo.CurrentPrice.ToString());
                        ComStockInfo.MarketPrice = CkPriceInfo.CurrentMarketPrice == null || CkPriceInfo.CurrentMarketPrice == 0 ? ComStockInfo.MarketPrice : decimal.Parse(CkPriceInfo.CurrentMarketPrice.ToString());
                        ComStockInfo.CostPrice = CkPriceInfo.CurrentCostPrice == null || CkPriceInfo.CurrentCostPrice == 0 ? ComStockInfo.CostPrice : decimal.Parse(CkPriceInfo.CurrentCostPrice.ToString());
                    }
                    ComStockInfo.ModifiedOn = DateTime.Now;
                    //取出commoditystock表中最小价格
                    var CommodityInfo = Commodity.ObjectSet().FirstOrDefault(p => p.Id == ComStockInfo.CommodityId);
                    if (CommodityInfo != null)
                    {
                        if (ComStockInfo.Price < CommodityInfo.Price && ComStockInfo.Price > 0)
                        {
                            CommodityInfo.Price = ComStockInfo.Price;
                            CommodityInfo.CostPrice = ComStockInfo.CostPrice;
                            CommodityInfo.ModifiedOn = DateTime.Now;
                        }
                    }
                    ChangeComStockIds.Add(ComStockInfo.Id);
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    //取出单属性商品ChangeComIds，多数想商品ChangeComStockIds，执行保存到commoditychange表中
                    SaveComChange(ChangeComIds, ChangeComStockIds);
                    return new ResultDTO { ResultCode = 0, isSuccess = true };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("定时修改库存服务异常。异常信息:"), ex);
                return new ResultDTO { ResultCode = 1, isSuccess = false };
            }
        }
        /// <summary>
        /// 审核通过后撤销.编辑,恢复已变更的数据
        /// </summary>
        /// <param name="CkPriceList"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RecoverCommodityPriceExt(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                List<Guid> ComIdList = new List<Guid>();//修改商品库存表商品Id
                List<Guid> ChangeComIds = new List<Guid>();
                List<Guid> ChangeComStockIds = new List<Guid>();
                //修改库存表价格信息
                var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == CkPriceInfo.CommodityId);
                if (ComStock != null)
                {
                    ComStock.Price = CkPriceInfo.CurrentPrice == null || CkPriceInfo.CurrentPrice == 0 ? ComStock.Price : decimal.Parse(CkPriceInfo.CurrentPrice.ToString());
                    ComStock.MarketPrice = CkPriceInfo.CurrentMarketPrice == null || CkPriceInfo.CurrentMarketPrice == 0 ? ComStock.MarketPrice : CkPriceInfo.CurrentMarketPrice;
                    ComStock.CostPrice = CkPriceInfo.CurrentCostPrice == null || CkPriceInfo.CurrentCostPrice == 0 ? ComStock.CostPrice : CkPriceInfo.CurrentCostPrice;
                    ComStock.ModifiedOn = DateTime.Now;
                    ComStock.EntityState = EntityState.Modified;
                    ChangeComStockIds.Add(ComStock.Id);
                    var ComInfo = Commodity.ObjectSet().FirstOrDefault(p => p.Id == ComStock.CommodityId);
                    if (ComInfo != null)
                    {
                        if (ComStock.Price < ComInfo.Price && ComStock.Price > 0)
                        {
                            ComInfo.Price = ComStock.Price;
                            ComInfo.CostPrice = ComStock.CostPrice;
                            ComInfo.ModifiedOn = DateTime.Now;
                            ComInfo.EntityState = EntityState.Modified;
                        }
                    }
                }
                //修改商品表价格信息
                var Com = Commodity.ObjectSet().FirstOrDefault(p => p.Id == CkPriceInfo.CommodityId);
                if (Com != null)
                {

                    Com.Price = CkPriceInfo.CurrentPrice == null || CkPriceInfo.CurrentPrice == 0 ? Com.Price : decimal.Parse(CkPriceInfo.CurrentPrice.ToString());
                    Com.MarketPrice = CkPriceInfo.CurrentMarketPrice == null || CkPriceInfo.CurrentMarketPrice == 0 ? Com.MarketPrice : CkPriceInfo.CurrentMarketPrice;
                    Com.CostPrice = CkPriceInfo.CurrentCostPrice == null || CkPriceInfo.CurrentCostPrice == 0 ? Com.CostPrice : CkPriceInfo.CurrentCostPrice;
                    Com.ModifiedOn = DateTime.Now;
                    Com.ModifieId = this.ContextDTO.LoginUserID;
                    Com.EntityState = EntityState.Modified;
                    Com.RefreshCache(EntityState.Modified);
                    ChangeComIds.Add(Com.Id);
                }
                int count = contextSession.SaveChange();
                if (count > 0)
                {
                    //执行保存到commoditychange表中
                    SaveComChange(ChangeComIds, ChangeComStockIds);
                    return new ResultDTO { ResultCode = 0, isSuccess = true };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, isSuccess = false };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("定时修改库存服务异常。异常信息:"), ex);
                return new ResultDTO { ResultCode = 1, isSuccess = false };
            }

        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyInfoListExt(System.Guid esappId)
        {
            try
            {
                var mallapplylist = MallApply.GetTGQuery(esappId);
                List<MallApplyDTO> result = (from c in mallapplylist
                                             select new MallApplyDTO
                                             {
                                                 AppId = c.AppId,
                                                 AppName = c.AppName
                                             }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询商城信息GetMallApplyInfoListExt。异常信息：appid{0}", esappId), ex);
                return null;
            }
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierInfoListExt(System.Guid appId)
        {
            try
            {
                var SupplierList = Supplier.ObjectSet().AsQueryable();
                if (appId != Guid.Empty)
                {
                    SupplierList = SupplierList.Where(p => p.EsAppId == appId).AsQueryable();
                }
                List<SupplierDTO> result = (from a in SupplierList
                                            select new SupplierDTO
                                            {
                                                AppId = a.AppId,
                                                EsAppId = a.EsAppId,
                                                SupplierName = a.SupplierName,
                                                SupplierCode = a.SupplierCode
                                            }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询供应商信息GetSupplierInfoListExt。异常信息：appid{0}", appId), ex);
                return null;
            }
        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyListExt()
        {
            try
            {
                var mallapplylist = MallApply.ObjectSet().AsQueryable();
                List<MallApplyDTO> result = (from c in mallapplylist
                                             select new MallApplyDTO
                                             {
                                                 AppId = c.AppId,
                                                 EsAppId = c.EsAppId,
                                                 AppName = c.AppName
                                             }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询商城信息GetMallApplyListExt。异常信息"), ex);
                return null;
            }
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierListExt()
        {
            try
            {
                var SupplierList = Supplier.ObjectSet().AsQueryable();
                List<SupplierDTO> result = (from a in SupplierList
                                            select new SupplierDTO
                                            {
                                                AppId = a.AppId,
                                                EsAppId = a.EsAppId,
                                                SupplierName = a.SupplierName
                                            }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询供应城信息GetSupplierListExt。异常信息"), ex);
                return null;
            }

        }
        /// <summary>
        /// 定时修改的数据信息插入到commodityChange表中
        /// </summary>
        /// <param name="commodityids">无属性和单属性商品id</param>
        /// <param name="stockisd">多属性商品id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveComChange(List<System.Guid> commodityids, List<System.Guid> stockisd)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list = new List<Deploy.CustomDTO.CommodityChangeDTO>();

            #region //取出commodity表中商品变动明细
            //取出Commodity表中编辑的数据
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> Commoditylist = (from n in Commodity.ObjectSet()
                                                                                      where commodityids.Contains(n.Id)
                                                                                      select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                      {
                                                                                          CommodityId = n.Id,
                                                                                          Name = n.Name,
                                                                                          Code = n.Code,
                                                                                          No_Number = n.No_Number,
                                                                                          SubId = n.SubId,
                                                                                          Price = n.Price,
                                                                                          Stock = n.Stock,
                                                                                          PicturesPath = n.PicturesPath,
                                                                                          Description = n.Description,
                                                                                          State = n.State,
                                                                                          IsDel = n.IsDel,
                                                                                          AppId = n.AppId,
                                                                                          No_Code = n.No_Code,
                                                                                          TotalCollection = n.TotalCollection,
                                                                                          TotalReview = n.TotalReview,
                                                                                          Salesvolume = n.Salesvolume,
                                                                                          ModifiedOn = n.ModifiedOn,
                                                                                          GroundTime = n.GroundTime,
                                                                                          ComAttribute = n.ComAttribute,
                                                                                          CategoryName = n.CategoryName,
                                                                                          SortValue = n.SortValue,
                                                                                          FreightTemplateId = n.FreightTemplateId,
                                                                                          MarketPrice = n.MarketPrice,
                                                                                          IsEnableSelfTake = n.IsEnableSelfTake,
                                                                                          Weight = n.Weight,
                                                                                          PricingMethod = n.PricingMethod,
                                                                                          SaleAreas = n.SaleAreas,
                                                                                          SharePercent = n.SharePercent,
                                                                                          CommodityType = n.CommodityType,
                                                                                          HtmlVideoPath = n.HtmlVideoPath,
                                                                                          MobileVideoPath = n.MobileVideoPath,
                                                                                          VideoPic = n.VideoPic,
                                                                                          VideoName = n.VideoName,
                                                                                          ScorePercent = n.ScorePercent,
                                                                                          Duty = n.Duty,
                                                                                          SpreadPercent = n.SpreadPercent,
                                                                                          ScoreScale = n.ScoreScale,
                                                                                          TaxRate = n.TaxRate,
                                                                                          TaxClassCode = n.TaxClassCode,
                                                                                          Unit = n.Unit,
                                                                                          InputRax = n.InputRax,
                                                                                          Barcode = n.Barcode,
                                                                                          JDCode = n.JDCode,
                                                                                          CostPrice = n.CostPrice,
                                                                                          IsAssurance = n.IsAssurance,
                                                                                          TechSpecs = n.TechSpecs,
                                                                                          SaleService = n.SaleService,
                                                                                          IsReturns = n.IsReturns,
                                                                                          ServiceSettingId = n.ServiceSettingId,
                                                                                          Type = n.Type,
                                                                                          YJCouponActivityId = n.YJCouponActivityId,
                                                                                          YJCouponType = n.YJCouponType,
                                                                                          SubOn = n.SubTime,
                                                                                          ModifiedId = n.ModifieId
                                                                                      }).ToList();
            list.AddRange(Commoditylist);

            #endregion

            #region  取出CommodityStock表中编辑的数据

            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> CommodityStockList = (from n in Commodity.ObjectSet()
                                                                                           join m in CommodityStock.ObjectSet() on n.Id equals m.CommodityId
                                                                                           where stockisd.Contains(m.Id)
                                                                                           select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                           {
                                                                                               CommodityId = m.Id,
                                                                                               Name = n.Name,
                                                                                               Code = n.Code,
                                                                                               No_Number = n.No_Number,
                                                                                               SubId = n.SubId,
                                                                                               Price = m.Price,
                                                                                               Stock = m.Stock,
                                                                                               PicturesPath = n.PicturesPath,
                                                                                               Description = n.Description,
                                                                                               State = n.State,
                                                                                               IsDel = n.IsDel,
                                                                                               AppId = n.AppId,
                                                                                               No_Code = m.No_Code,
                                                                                               TotalCollection = n.TotalCollection,
                                                                                               TotalReview = n.TotalReview,
                                                                                               Salesvolume = n.Salesvolume,
                                                                                               ModifiedOn = m.ModifiedOn,
                                                                                               GroundTime = n.GroundTime,
                                                                                               ComAttribute = n.ComAttribute,
                                                                                               CategoryName = n.CategoryName,
                                                                                               SortValue = n.SortValue,
                                                                                               FreightTemplateId = n.FreightTemplateId,
                                                                                               MarketPrice = m.MarketPrice,
                                                                                               IsEnableSelfTake = n.IsEnableSelfTake,
                                                                                               Weight = n.Weight,
                                                                                               PricingMethod = n.PricingMethod,
                                                                                               SaleAreas = n.SaleAreas,
                                                                                               SharePercent = n.SharePercent,
                                                                                               CommodityType = n.CommodityType,
                                                                                               HtmlVideoPath = n.HtmlVideoPath,
                                                                                               MobileVideoPath = n.MobileVideoPath,
                                                                                               VideoPic = n.VideoPic,
                                                                                               VideoName = n.VideoName,
                                                                                               ScorePercent = n.ScorePercent,
                                                                                               Duty = m.Duty,
                                                                                               SpreadPercent = n.SpreadPercent,
                                                                                               ScoreScale = n.ScoreScale,
                                                                                               TaxRate = n.TaxRate,
                                                                                               TaxClassCode = n.TaxClassCode,
                                                                                               Unit = n.Unit,
                                                                                               InputRax = n.InputRax,
                                                                                               Barcode = m.Barcode,
                                                                                               JDCode = m.JDCode,
                                                                                               CostPrice = m.CostPrice,
                                                                                               IsAssurance = n.IsAssurance,
                                                                                               TechSpecs = n.TechSpecs,
                                                                                               SaleService = n.SaleService,
                                                                                               IsReturns = n.IsReturns,
                                                                                               ServiceSettingId = n.ServiceSettingId,
                                                                                               Type = n.Type,
                                                                                               YJCouponActivityId = n.YJCouponActivityId,
                                                                                               YJCouponType = n.YJCouponType,
                                                                                               SubOn = n.SubTime,
                                                                                               ModifiedId = n.ModifieId
                                                                                           }).ToList();
            list.AddRange(CommodityStockList);
            #endregion
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DTO = ChangeFacade.SaveCommodityChange(list);
            return DTO;
        }


        /// <summary>
        /// 查询App入驻信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyByIdsExt(Guid esAppId, List<Guid> appIds)
        {
            try
            {
                return MallApply.GetTGQuery(esAppId).Where(_ => appIds.Contains(_.AppId)).ToList().Select(_ => _.ToEntityData()).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("查询商城信息GetMallApplyListExt。异常信息"), ex);
                return null;
            }
        }
        /// <summary>
        /// 获取所有的严选appId
        /// </summary>
        public System.Collections.Generic.List<System.Guid> GetYXappIdsExt()
        {
            try
            {
                return CustomConfig.YxAppIdList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取所有的严选appId服务异常"), ex);
                return new List<Guid>();
            }

        }
        /// <summary>
        /// 导出定时改价未改变价格的订单信息
        ///  </summary>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> GetOrderItemListExt(string StarTime, string EndTime)
        {
            try
            {
                List<OrderItemDTO> OrderItemList = new List<OrderItemDTO>();
                //易捷北京下所有商家
                List<Guid> AppId = new List<Guid>();
                AppId.Add(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                var PriceChangeList = YJBSV.GetAllChangeInfo(AppId, StarTime, EndTime).Data;
                LogHelper.Info(string.Format("定时改价商品信息条数:{0}", PriceChangeList.Count()));
                foreach (var item in PriceChangeList)
                {
                    List<string> OrderExcept = new List<string>();
                    var PriceA = PriceChangeList.Where(p => p.CommodityId == item.CommodityId && item.EffectiveTime <= p.EffectiveTime && item.LoseTime >= p.LoseTime).ToList();

                    foreach (var it in PriceA)
                    {
                        List<string> Orderex = new List<string>();
                        Orderex = (from com in CommodityOrder.ObjectSet()
                                   join ord in OrderItem.ObjectSet() on com.Id equals ord.CommodityOrderId
                                   where com.State == 3 && item.TargetCostPrice != ord.CostPrice && (ord.CommodityId == item.CommodityId || ord.CommodityStockId == item.CommodityId) && ord.SubTime >= item.EffectiveTime && ord.SubTime <= item.LoseTime
                                   select new OrderItemDTO()
                                   {
                                       Code = ord.Code
                                   }).Select(s => s.Code).ToList();
                        OrderExcept.AddRange(Orderex);
                    }

                    List<OrderItemDTO> OrderItems = new List<OrderItemDTO>();
                    OrderItems = (from com in CommodityOrder.ObjectSet()
                                  join ord in OrderItem.ObjectSet() on com.Id equals ord.CommodityOrderId
                                  where com.State == 3 && !OrderExcept.Contains(ord.Code) && item.TargetCostPrice <= ord.CostPrice && (ord.CommodityId == item.CommodityId || ord.CommodityStockId == item.CommodityId) && ord.SubTime >= item.EffectiveTime && ord.SubTime <= item.LoseTime
                                  select new OrderItemDTO()
                                  {
                                      Name = ord.Name,
                                      Number = ord.Number,
                                      CostPrice = item.TargetCostPrice,//进货价
                                      YjbPrice = ord.Number * item.TargetCostPrice,//进货金额
                                      State_Value = "确认收货",
                                      Code = ord.Code,
                                      SubTime = ord.SubTime,
                                      SubId = com.AppId,//暂时存储店铺名称 
                                      ChangeRealPrice = com.Price,
                                      CurrentPrice = ord.RealPrice ?? 0,//订单实际价格                                     
                                      FreightPrice = com.Freight,
                                      Barcode = com.Payment.ToString()//支付方式                                      
                                  }).ToList();
                    LogHelper.Info(string.Format("定时改价商品信息:{0},产生订单数量:{1}", JsonHelper.JsonSerializer(item), OrderItems.Count()));
                    OrderItemList.AddRange(OrderItems);
                }
                List<Guid> AppidList = OrderItemList.Select(s => s.SubId).Distinct().ToList();
                //匹配供应商名称
                var SupplierList = Supplier.ObjectSet().Where(p => AppidList.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName, s.SupplierType }).Distinct().ToList();
                //匹配店铺名称
                Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(AppidList); //获取商铺名称
                foreach (var it in OrderItemList)
                {
                    //获取供应商名称
                    var SupplierName = SupplierList.Where(p => p.AppId == it.SubId).Select(s => s.SupplierName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(SupplierName))
                    {
                        it.Unit = SupplierName;
                    }
                    //获取商铺名称
                    if (listApps.ContainsKey(it.SubId))
                    {
                        var listAppName = listApps[it.SubId];
                        if (!String.IsNullOrEmpty(listAppName))
                        {
                            it.PicturesPath = listAppName;
                        }
                    }
                }
                return OrderItemList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("导出定时改价未改变价格的订单信息服务异常"), ex);
                return new List<Jinher.AMP.BTP.Deploy.OrderItemDTO>();
            }
        }
        /// <summary>
        /// 导出定时改价未改变价格的订单信息
        ///  </summary>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> GetOrderInfoByAppIdExt(System.Guid AppId, string StarTime, string EndTime)
        {
            try
            {
                List<OrderItemDTO> OrderItemList = new List<OrderItemDTO>();
                //易捷北京下所有商家
                List<Guid> appId = new List<Guid>();
                appId.Add(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
                var PriceChangeList = YJBSV.GetAllChangeInfo(appId, "2018-07-01", "2018-09-01").Data.ToList();
                LogHelper.Info(string.Format("定时改价商品信息条数:{0}", PriceChangeList.Count()));
                DateTime star = Convert.ToDateTime(StarTime);
                DateTime End = Convert.ToDateTime(EndTime);
                //取出店铺所有订单
                List<int> OrderState = new List<int>() { 1, 2, 3, 7, 8, 9, 13, 14 };
                OrderItemList = (from com in CommodityOrder.ObjectSet()
                                 join ord in OrderItem.ObjectSet() on com.Id equals ord.CommodityOrderId
                                 where OrderState.Contains(com.State) && com.AppId == AppId && ord.SubTime >= star && ord.SubTime <= End
                                 select new OrderItemDTO()
                                 {
                                     Name = ord.Name,
                                     Number = ord.Number,
                                     CostPrice = ord.CostPrice,//进货价  
                                     YjbPrice = ord.Number * ord.CostPrice,
                                     State = com.State,
                                     Code = ord.Code,
                                     SubTime = ord.SubTime,
                                     SubId = com.AppId,//暂时存储店铺名称    
                                     CommodityId = ord.CommodityId,
                                     CommodityStockId = ord.CommodityStockId,
                                     FreightPrice = com.Freight
                                 }).ToList();

                List<Guid> AppidList = OrderItemList.Select(s => s.SubId).Distinct().ToList();
                //匹配供应商名称
                var SupplierList = Supplier.ObjectSet().Where(p => AppidList.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName, s.SupplierType }).Distinct().ToList();
                //匹配店铺名称
                Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(AppidList); //获取商铺名称
                foreach (var it in OrderItemList)
                {
                    //获取供应商名称
                    var SupplierName = SupplierList.Where(p => p.AppId == it.SubId).Select(s => s.SupplierName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(SupplierName))
                    {
                        it.Unit = SupplierName;
                    }
                    //获取商铺名称
                    if (listApps.ContainsKey(it.SubId))
                    {
                        var listAppName = listApps[it.SubId];
                        if (!String.IsNullOrEmpty(listAppName))
                        {
                            it.PicturesPath = listAppName;
                        }
                    }
                    //匹配定时改价进货金                  
                    var ChangeInfo = PriceChangeList.Where(p => p.AppId == it.SubId && (p.CommodityId == it.CommodityId || p.CommodityId == it.CommodityStockId) && (p.EffectiveTime <= it.SubTime || p.EffectiveTime == null)).OrderBy(p => p.EffectiveTime).FirstOrDefault();

                    LogHelper.Info(string.Format("店铺:{0},取出的定时改价信息:{1}", it.SubId, JsonHelper.JsonSerializer(ChangeInfo)));
                    if (ChangeInfo != null)
                    {
                        it.CostPrice = ChangeInfo.TargetCostPrice;
                        it.YjbPrice = it.Number * ChangeInfo.TargetCostPrice;
                    }
                    switch (it.State)
                    {
                        case 1:
                            it.State_Value = "未发货";
                            break;
                        case 2:
                            it.State_Value = "已发货";
                            break;
                        case 3:
                            it.State_Value = "确认收货";
                            break;
                        case 7:
                            it.State_Value = "已退款";
                            break;
                        case 8:
                            it.State_Value = "待发货退款中";
                            break;
                        case 9:
                            it.State_Value = "已发货退款中";
                            break;
                        case 13:
                            it.State_Value = "出库中";
                            break;
                        case 14:
                            it.State_Value = "出库中退款中";
                            break;
                    }
                }
                return OrderItemList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("导出定时改价未改变价格的订单信息服务异常"), ex);
                return new List<Jinher.AMP.BTP.Deploy.OrderItemDTO>();
            }
        }
        #region  获取一级分类
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
