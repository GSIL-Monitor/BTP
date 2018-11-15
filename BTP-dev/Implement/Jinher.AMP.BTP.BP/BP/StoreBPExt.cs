
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/19 18:07:29
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE.MongoCollection;
using Jinher.AMP.BTP.Deploy.MongoDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Jinher.AMP.BTP.BP
{
    /// <summary>
    /// 
    /// </summary>
    public partial class StoreBP : BaseBP, IStore
    {


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
        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        public ResultDTO AddStoreExt(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                StoreMgDTO smgDto = new StoreMgDTO();
                smgDto.FillWith(storeDTO);
                smgDto.Location = new double[] { (double)storeDTO.XAxis, (double)storeDTO.YAxis };
                MongoCollections.Store.Save(smgDto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加门店服务异常。storeDTO：{0}", JsonHelper.JsonSerializer(storeDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 删除门店
        /// </summary>
        /// <param name="id">门店ID</param>
        public ResultDTO DelStoreExt(System.Guid id)
        {
            try
            {
                var storeQuery = Query<StoreMgDTO>.Where(c => c.Id == id);
                MongoCollections.Store.Remove(storeQuery, RemoveFlags.Single);
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("删除门店服务异常。id：{0}", id), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }


        /// <summary>
        /// 根据AppId得到门店详细信息 舌尖在线，目前只有一个
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAppStoreExt(Guid appId)
        {
            var storeQuery = Query<StoreMgDTO>.Where(c => c.AppId == appId);
            List<StoreMgDTO> storeList = MongoCollections.Store.Find(storeQuery).ToList();
            List<StoreDTO> srList = storeList.ConvertAll<StoreDTO>(MongoToDto);
            return srList;
        }


        /// <summary>
        /// 得到门店详细信息
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.StoreDTO GetStoreDTOExt(System.Guid id, Guid appid)
        {
            var storeQuery = Query<StoreMgDTO>.Where(c => c.Id == id);
            StoreMgDTO smg = MongoCollections.Store.FindOneAs<StoreMgDTO>(storeQuery);
            StoreDTO store = MongoToDto(smg);
            return store;
        }
        /// <summary>
        /// 修改门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        public ResultDTO UpdateStoreExt(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            try
            {
                StoreMgDTO storeMg = new StoreMgDTO();
                storeMg.FillWith(storeDTO);
                storeMg.Location = new double[] { (double)storeDTO.XAxis, (double)storeDTO.YAxis };
                WriteConcernResult wcResult = MongoCollections.Store.Save(storeMg);
                //var ub = Update.Set("Id",storeDTO.Id);
                //var storeQuery = Query<StoreMgDTO>.Where(c => c.Id == storeDTO.Id);
                //_collection.FindAndModify(storeQuery, SortBy.Null, ub);
                //_collection.Update(storeQuery,);
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("修改门店服务异常。storeDTO：{0}", JsonHelper.JsonSerializer(storeDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }



            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }
        /// <summary>
        ///  得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStoreExt(System.Guid sellerId, int pageSize, int pageIndex, out int rowCount)
        {
            var storeQuery = Query<StoreMgDTO>.Where(c => c.AppId == sellerId);
            //todo 将int改为long.
            rowCount = (int)MongoCollections.Store.Count(storeQuery);
            List<StoreMgDTO> storeList = MongoCollections.Store.Find(storeQuery).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).SetSortOrder(SortBy.Descending("SubTime")).ToList();
            List<StoreDTO> srList = storeList.ConvertAll<StoreDTO>(MongoToDto);
            return srList;
        }

        /// <summary>
        /// 将mongodb返回的结果StoreMgDTO转为StoreDTO
        /// </summary>
        /// <param name="storeMg">mongo store dto</param>
        /// <returns></returns>
        private StoreDTO MongoToDto(StoreMgDTO storeMg)
        {
            StoreDTO storeDto = new StoreDTO();
            storeDto.FillWith(storeMg);
            if (storeMg.Location != null
                && storeMg.Location.Length > 1)
            {
                storeDto.XAxis = (decimal)storeMg.Location[0];
                storeDto.YAxis = (decimal)storeMg.Location[1];
            }
            return storeDto;
        }

        /// <summary>
        /// 根据条件查询所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="storeName">门店名称</param>
        /// <param name="provice">所在省份</param>
        /// <param name="city">所在城市</param>
        /// <param name="district">所在区域</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStoreByWhereExt(Guid sellerId, int pageSize, int pageIndex, out int rowCount, string storeName, string provice, string city, string district)
        {
            var queryList = new List<IMongoQuery>();
            var q1 = Query<StoreMgDTO>.Where(n => n.AppId == sellerId);
            queryList.Add(q1);


            if (!string.IsNullOrEmpty(storeName))
            {
                var q2 = Query<StoreMgDTO>.Where(n => n.Name.Contains(storeName));
                queryList.Add(q2);
            }
            if (!string.IsNullOrEmpty(provice) && provice != "000000")
            {
                var q3 = Query<StoreMgDTO>.Where(n => n.ProvinceCode == provice);
                queryList.Add(q3);
            }
            if (!string.IsNullOrEmpty(city))
            {
                var q4 = Query<StoreMgDTO>.Where(n => n.CityCode == city);
                queryList.Add(q4);
            }
            if (!string.IsNullOrEmpty(district))
            {
                var q5 = Query<StoreMgDTO>.Where(n => n.DistrictCode == district);
                queryList.Add(q5);
            }

            rowCount = (int)MongoCollections.Store.Count(Query.And(queryList));

            var mglist = MongoCollections.Store.Find(Query.And(queryList)).SetSkip((pageIndex - 1) * pageSize).SetLimit(pageSize).SetSortOrder(SortBy.Descending("SubTime")).ToList();
            List<StoreDTO> slist = mglist.ConvertAll(MongoToDto);
            return slist;

        }

        //private StoreCacheDTO GetStoreCacheDTO(StoreDTO storeDTO)
        //{
        //    var storeCacheDTO = new StoreCacheDTO
        //    {
        //        Id = storeDTO.Id,
        //        Address = storeDTO.Address,
        //        AppId = storeDTO.AppId,
        //        City = storeDTO.City,
        //        District = storeDTO.District,
        //        Name = storeDTO.Name,
        //        Phone = storeDTO.Phone,
        //        picture = storeDTO.picture,
        //        Province = storeDTO.Province,
        //        SubTime = storeDTO.SubTime
        //    };

        //    return storeCacheDTO;
        //}

        /// <summary>
        /// 获取门店缓存
        /// </summary>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        private void WriteStoreCache(Guid appId)
        {
            var list = Store.ObjectSet().Where(s => s.AppId == appId).Select(
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
                              //SubTime = a.SubTime
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
        public string GetStrPhone(List<PhoneSDTO> phones)
        {
            string strphone = string.Empty;

            for (int i = 0; i < phones.Count; i++)
            {
                strphone = string.Format("{0},", phones[i].PhoneNumber);
            }
            strphone = strphone.Substring(0, strphone.Length - 1);
            return strphone;
        }
        #endregion
    }
}
