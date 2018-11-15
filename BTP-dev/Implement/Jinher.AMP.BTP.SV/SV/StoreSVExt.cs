using System;
using System.Collections.Generic;
using System.Linq;
using Jinher.AMP.BTP.BE.MongoCollection;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using MongoDB.Driver;
using Jinher.AMP.BTP.Deploy.MongoDTO;
using Jinher.AMP.BTP.Common;
using MongoDB.Driver.Builders;
using Jinher.JAP.PL;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 门店接口类
    /// </summary>
    public partial class StoreSV : BaseSv, IStore
    {
        // private static MongoCollection<StoreMgDTO> _collection;

        //static StoreSV()
        //{
        //    var mm = MongoManager.getDB();
        //    _collection = mm.GetCollection<StoreMgDTO>(MongoKeyConst.CollectionName);

        //    var geoindexMapLocation = IndexKeys<StoreMgDTO>.GeoSpatial(c => c.Location);
        //    _collection.EnsureIndex(geoindexMapLocation, IndexOptions.SetGeoSpatialRange(0, 300));
        //}

        /// <summary>
        /// 获取门店
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetStoreExt(System.Guid appId, int pageIndex, int pageSize)
        {

            pageSize = pageSize == 0 ? 10 : pageSize;
            NStoreSDTO storedto = new NStoreSDTO();
            var storeQuery = Query<StoreMgDTO>.Where(c => c.AppId == appId);
            var temp1 = MongoCollections.Store.FindAs<StoreMgDTO>(storeQuery);
            List<string> provincelist = temp1.Select(n => n.Province).Distinct().ToList();
            storedto.Proviences = provincelist;

            List<StoreSDTO> slist = temp1.ToList().ConvertAll(MongoToDto);
            storedto.Stroes = slist;
            return storedto;
        }
        /// <summary>
        /// 将mongodb返回的结果转为StoreSDTO
        /// </summary>
        /// <param name="storeMg">mongo store dto</param>
        /// <returns></returns>
        private StoreSDTO MongoToDto(StoreMgDTO storeMg)
        {
            StoreSDTO storeDto = new StoreSDTO();
            storeDto.FillWith(storeMg);
            storeDto.StoreName = storeMg.Name;
            storeDto.PicPath = storeMg.picture;
            storeDto.Phone = new List<PhoneSDTO>();
            string[] phones = storeMg.Phone.Split(";；,，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string p in phones)
            {
                storeDto.Phone.Add(new PhoneSDTO() { PhoneNumber = p });
            }
            try
            {
                storeDto.XAxis = (decimal)storeMg.Location[0];
                storeDto.YAxis = (decimal)storeMg.Location[1];
            }
            catch { }
            return storeDto;
        }

        /// <summary>
        /// 按地区查询门店
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="province">省</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> GetStoreByProvinceExt
            (string province, System.Guid appId, int pageIndex, int pageSize = 10)
        {
            pageSize = pageSize == 0 ? 10 : pageSize;

            var queryList = new List<IMongoQuery>();
            var q1 = Query<StoreMgDTO>.Where(n => n.AppId == appId);
            queryList.Add(q1);
            var q2 = Query<StoreMgDTO>.Where(n => n.Province == province);
            queryList.Add(q2);

            var mglist = MongoCollections.Store.Find(Query.And(queryList)).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).SetSortOrder(SortBy.Descending("SubTime")).ToList();
            List<StoreSDTO> slist = mglist.ConvertAll(MongoToDto);
            return slist;
        }

        #region 获取电话列表
        public List<PhoneSDTO> GetPhone(string phones)
        {
            List<PhoneSDTO> phone = new List<PhoneSDTO>();
            string[] plist = phones.Split(',');
            for (int i = 0; i < plist.Length; i++)
            {
                PhoneSDTO bb = new PhoneSDTO();
                bb.PhoneNumber = plist[i];
                phone.Add(bb);
            }
            return phone;
        }
        #endregion

        /// <summary>
        /// 按地区查询门店
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetProvince
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<string> GetProvinceExt(System.Guid appId)
        {
            var provinceList = (from s in BE.Store.ObjectSet()
                                where s.AppId == appId
                                group s by s.Province into g
                                select g.Key).ToList();
            return provinceList;
        }

        /// <summary>
        /// 获取门店缓存
        /// </summary>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        private void WriteStoreCache(Guid appId)
        {
            var list = BE.Store.ObjectSet().Where(s => s.AppId == appId).Select(
                          a => new StoreCacheDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Address = a.Address,
                              City = a.City,
                              District = a.District,
                              Phone = a.Phone,
                              picture = a.picture,
                              Province = a.Province,
                              SubTime = a.SubTime
                          }).ToList();
            var temp = (from n in list
                        select new StoreSDTO()
                        {
                            Id = n.Id,
                            StoreName = n.Name,
                            Phone = GetPhone(n.Phone),
                            PicPath = n.picture,
                            Address = n.Address,
                            Province = n.Province,
                            City = n.City,
                            District = n.District
                        }).ToList();

            Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_StoreInfo", appId.ToString(), temp, "BTPCache");
        }


        /// <summary>
        ///  获取门店列表（按用户当前位置到门店的距离排序）
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStoreByLocation
        /// </para>
        /// </summary> 
        /// <param name="slp">参数实体类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetStoreByLocationExt(StoreLocationParam slp)
        {
            if (slp == null)
            {
                return null;
            }
            else if (slp.AppId == Guid.Empty)
            {
                return null;
            }
            else if (slp.Longitude > 180 || slp.Longitude < -180)
            {
                return null;
            }
            else if (slp.Latitude > 90 || slp.Latitude < -90)
            {
                return null;
            }

            if (slp.CurrentPageIndex < 1)
            {
                slp.CurrentPageIndex = 1;
            }
            if (slp.PageSize <= 0)
            {
                slp.PageSize = 20;
            }

            //GeoNearOptions.SetSpherical
            var queryList = new List<IMongoQuery>();
            queryList.Add(Query<StoreMgDTO>.Where(c => c.AppId == slp.AppId));

            GeoNearResult<StoreMgDTO> ss = MongoCollections.Store.GeoNear(Query.And(queryList), (double)slp.Longitude, (double)slp.Latitude, int.MaxValue);
            var storeMgs = ss.Hits.Skip((slp.CurrentPageIndex - 1) * slp.PageSize).Take(slp.PageSize).ToList();

            NStoreSDTO nsr = new NStoreSDTO();
            if (storeMgs.Any())
            {
                List<StoreSDTO> srList = storeMgs.ConvertAll<StoreSDTO>(MongoToDto2);
                List<string> proviences = srList.Select(s => s.Province).Distinct().ToList();
                nsr.Proviences = proviences;
                nsr.Stroes = srList;
            }
            return nsr;
        }

        /// <summary>
        /// 获取餐饮平台聚合门店
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NStoreSDTO GetCateringPlatformStoreExt(StoreLocationParam param)
        {
            if (param == null)
            {
                return null;
            }
            else if (param.AppId == Guid.Empty)
            {
                return null;
            }
            else if (param.Longitude > 180 || param.Longitude < -180)
            {
                return null;
            }
            else if (param.Latitude > 90 || param.Latitude < -90)
            {
                return null;
            }

            if (param.CurrentPageIndex < 1)
            {
                param.CurrentPageIndex = 1;
            }
            if (param.PageSize <= 0)
            {
                param.PageSize = 20;
            }
            GeoNearResult<StoreMgDTO> storeList;
            //if (JAP.Cache.GlobalCacheWrapper.ContainsCache("StoreFromMongo", "BTPArea"))
            //{
            //    storeList = (GeoNearResult<StoreMgDTO>)JAP.Cache.GlobalCacheWrapper.GetDataCache("StoreFromMongo", "BTPArea");
            //    int cnt = 0;
            //    if (storeList != null)
            //        cnt = storeList.Hits.Count();
            //    LogHelper.Info("Get_StoreFromMongo___数量："+cnt);
            //}
            //else
            //{
            List<Guid> pavilionApp = GetPavilionApp(new ZPH.Deploy.CustomDTO.QueryPavilionAppParam { Id = param.AppId, pageIndex = 1, pageSize = int.MaxValue });
            var queryList = new List<IMongoQuery>();
            queryList.Add(Query<StoreMgDTO>.Where(c => pavilionApp.Contains(c.AppId)));
            storeList = MongoCollections.Store.GeoNear(Query.And(queryList), (double)param.Longitude, (double)param.Latitude, int.MaxValue);
            //JAP.Cache.GlobalCacheWrapper.AddCache("StoreFromMongo", storeList, 300, "BTPArea");
            var storeMgs = storeList.Hits.Skip((param.CurrentPageIndex - 1) * param.PageSize).Take(param.PageSize).ToList();

            NStoreSDTO nsr = new NStoreSDTO();
            if (storeMgs.Any())
            {
                List<StoreSDTO> srList = storeMgs.ConvertAll<StoreSDTO>(MongoToDto2);
                List<string> proviences = srList.Select(s => s.Province).Distinct().ToList();
                nsr.Proviences = proviences;
                nsr.Stroes = srList;
            }
            return nsr;
        }

        private List<Guid> GetPavilionApp(ZPH.Deploy.CustomDTO.QueryPavilionAppParam param)
        {
            try
            {
                ZPH.ISV.Facade.AppPavilionFacade facade = new ZPH.ISV.Facade.AppPavilionFacade();
                return facade.GetPavilionApp(param).Data.Select(o => o.appId).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("StoreSV=>GetCateringPlatformStoreExt=>GetPavilionApp异常，异常信息：", ex);
                return new List<Guid>();
            }
        }

        private StoreSDTO MongoToDto2(GeoNearResult<StoreMgDTO>.GeoNearHit storeMgHit)
        {
            StoreSDTO store = MongoToDto(storeMgHit.Document);
            store.Distance = (decimal)storeMgHit.Distance * 100000;
            return store;
        }

        public void InitMongoFromSqlExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int pageSize = 20;
                int pageIndex = 1;

                while (true)
                {
                    var storeQuery = from s in BE.Store.ObjectSet()
                                     orderby s.SubTime ascending
                                     select s;
                    List<BE.Store> storeList = storeQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    foreach (BE.Store store in storeList)
                    {
                        StoreMgDTO storeMG = new StoreMgDTO();
                        storeMG.FillWith(store);
                        GetLocationByAddress(storeMG);

                        var q1 = Query<StoreMgDTO>.Where(s => s.Id == store.Id);
                        StoreMgDTO smg = MongoCollections.Store.FindOneAs<StoreMgDTO>(q1);
                        if (smg != null)
                        {
                            continue;
                        }
                        MongoCollections.Store.Save(storeMG);
                    }

                    if (storeList.Count < pageSize)
                    {
                        return;
                    }
                    pageIndex++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("InitMongoFromSqlExt异常，异常信息：", ex);
            }
        }


        private ResultDTO GetLocationByAddress(StoreMgDTO smgDto)
        {
            ResultDTO result = new ResultDTO();
            bool isll = false;
            try
            {
                string llStr = AampHelper.GetLongLatitudeByAddress(smgDto.Address, smgDto.CityCode);
                string[] llArr = llStr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (llArr != null && llArr.Length > 1)
                {
                    smgDto.Location = new double[2] { double.Parse(llArr[0]), double.Parse(llArr[1]) };
                    isll = true;
                }
            }
            catch
            {
                isll = false;
            }
            if (!isll)
            {
                result.Message = "未找到门店地址，请确认后再保存！";
                result.ResultCode = 1;
            }
            return result;
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> GetOnlyStoreInAppExt(System.Guid appId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.StoreSDTO> storeResult = new ResultDTO<StoreSDTO>();

            try
            {
                if (appId == Guid.Empty)
                {
                    storeResult.Message = "参数错误，appId不能为空！";
                    storeResult.ResultCode = 1;
                    return storeResult;
                }

                var q = Query<StoreMgDTO>.Where(c => c.AppId == appId);
                long len = MongoCollections.Store.Count(q);
                if (len > 1)
                {
                    storeResult.Message = "有多个门店";
                    storeResult.ResultCode = 2;
                }
                else if (len == 1)
                {
                    StoreMgDTO smgDto = MongoCollections.Store.FindOneAs<StoreMgDTO>(q);
                    if (smgDto != null)
                    {
                        StoreSDTO ssDto = MongoToDto(smgDto);
                        storeResult.Data = ssDto;
                    }
                    storeResult.Message = "只有一个门店";
                    storeResult.ResultCode = 3;
                }
                return storeResult;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetOnlyStoreInAppExt异常，异常信息：", ex);
            }
            return storeResult;
        }

        /// <summary>
        /// 获取馆信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Jinher.AMP.ZPH.Deploy.CustomDTO.AppPavilionInfoIICDTO GetAppPavilionInfoExt(Jinher.AMP.ZPH.Deploy.CustomDTO.QueryAppPavilionParam param)
        {
            return ZPHSV.Instance.GetAppPavilionInfo(param);
        }

        /// <summary>
        /// 获取门店 有效参数：AppId（必填），SubName（非必填）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<StoreSResultDTO> GetAppStoresExt(StoreLocationParam search)
        {
            ResultDTO<StoreSResultDTO> result = new ResultDTO<StoreSResultDTO>() { isSuccess = true, Message = "Success", ResultCode = 0, Data = new StoreSResultDTO() };
            if (search == null || search.AppId == Guid.Empty)
            {
                result.isSuccess = false;
                result.ResultCode = -1;
                result.Message = "参数为空";
                return result;
            }

            var queryList = new List<IMongoQuery>();
            var q1 = Query<StoreMgDTO>.Where(n => n.AppId == search.AppId);
            queryList.Add(q1);
            if (!search.SubName.IsNullVauleFromWeb())
            {
                queryList.Add(Query<StoreMgDTO>.Where(n => n.Name.Contains(search.SubName)));
            }
            result.Data.Count = MongoCollections.Store.Find(Query.And(queryList)).Count();
            var mglist = MongoCollections.Store.Find(Query.And(queryList)).SetSortOrder(SortBy.Descending("SubTime")).SetSkip((search.CurrentPageIndex - 1) * search.PageSize).SetLimit(search.PageSize).ToList();
            result.Data.Stroes = mglist.ConvertAll(MongoToDto);
            return result;
        }
        /// <summary>
        /// 获取附近门店 有效参数：AppId（必填），Longitude（必填），Latitude（必填），MaxDistance（非必填）
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetAppStoresByLocation
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<StoreSDTO>> GetAppStoresByLocationExt(StoreLocationParam search)
        {
            ResultDTO<List<StoreSDTO>> result = new ResultDTO<List<StoreSDTO>>() { isSuccess = true, Message = "Success", ResultCode = 0, Data = new List<StoreSDTO>() };
            if (search == null || search.AppId == Guid.Empty)
            {
                result.isSuccess = false;
                result.ResultCode = -1;
                result.Message = "参数为空";
                return result;
            }
            if (search.Longitude > 180 || search.Longitude < -180 || search.Latitude > 90 || search.Latitude < -90)
            {
                result.isSuccess = false;
                result.ResultCode = -2;
                result.Message = "参数有误";
                return result;
            }

            if (search.CurrentPageIndex < 1)
            {
                search.CurrentPageIndex = 1;
            }
            if (search.PageSize <= 0)
            {
                search.PageSize = 20;
            }

            var queryList = new List<IMongoQuery>();
            queryList.Add(Query<StoreMgDTO>.Where(c => c.AppId == search.AppId));
            double maxDistance = 1.0;
            if (search.MaxDistance > 0)
            {
                maxDistance = (double)search.MaxDistance / DistanceHelper.EarthRadius;

            }
            queryList.Add(Query<StoreMgDTO>.Near(c => c.Location, (double)search.Longitude, (double)search.Latitude, maxDistance, true));
            var list = MongoCollections.Store.Find(Query.And(queryList)).SetSkip((search.CurrentPageIndex - 1) * search.PageSize).SetLimit(search.PageSize).ToList();
            if (list.Any())
            {
                result.Data = list.ConvertAll(c => MongoToDto3(c, search.Latitude, search.Longitude));
            }
            return result;
        }
        private StoreSDTO MongoToDto3(StoreMgDTO entity, decimal lat, decimal lon)
        {
            StoreSDTO store = MongoToDto(entity);
            store.Distance = (decimal)DistanceHelper.GetGreatCircleDistance((double)lat, (double)lon, entity.Location[1], entity.Location[0]);
            return store;
        }
    }
}
