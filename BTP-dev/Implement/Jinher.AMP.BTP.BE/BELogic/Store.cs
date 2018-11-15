
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
namespace Jinher.AMP.BTP.BE
{
	public  partial class Store
	{
		#region 基类抽象方法重载
			         
		public override void BusinessRuleValidate()				
		{
					}
				#endregion 
		#region 基类虚方法重写
		public override void SetDefaultValue()
		{
		    base.SetDefaultValue();
		}     
		#endregion 		
        /// <summary>
        /// 得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStore(System.Guid sellerId, int pageSize, int pageIndex, string province)
        {
            var storeDTO = Store.ObjectSet().Where(n => n.AppId.Equals(sellerId)).Where(n=>n.Province==province).OrderByDescending(n => n.SubTime).Skip((pageIndex-1)*pageSize).Take(pageSize);
            return new Store().ToEntityDataList(storeDTO.ToList());
        }
        /// <summary>
        /// 得到地区门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.StoreDTO> GetProvinceStore(System.Guid sellerId, int pageSize, int pageIndex, string province)
        {
            var storeDTO = Store.ObjectSet().Where(n => n.AppId.Equals(sellerId)).Where(n=>n.Province==province).OrderByDescending(n => n.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new Store().ToEntityDataList(storeDTO.ToList());
        }
        /// <summary>
        /// 得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public List<StoreDTO> GetAllStore(System.Guid sellerId,int pageSize,int pageIndex,out int rowCount)
        {
            var store = Store.ObjectSet().Where(n => n.AppId == sellerId).OrderByDescending(n => n.SubTime);
            rowCount = store != null ? store.Count() : 0;
            var query = store.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList();
            return new Store().ToEntityDataList(query);
        }

        /// <summary>
        /// 根据条件得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public List<StoreDTO> GetAllStoreWhere(System.Guid sellerId, int pageSize, int pageIndex, out int rowCount, string storeName, string province, string city, string district)
        {
            var store = Store.ObjectSet().Where(n => n.AppId == sellerId).OrderByDescending(n => n.SubTime);
            var query=from a in store select a;
            if (!string.IsNullOrEmpty(storeName))
            {
                query = store.Where(n => n.Name.Contains(storeName));
            }
            if (!string.IsNullOrEmpty(province) && province != "000000")
            {
                query = store.Where(n => n.ProvinceCode == province);
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = store.Where(n => n.CityCode == city);
            }
            if (!string.IsNullOrEmpty(district))
            {
                query = store.Where(n => n.DistrictCode == district);
            }
            rowCount = query != null ? query.Count() : 0;
            var quer = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Store().ToEntityDataList(quer);
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        public void Add(StoreDTO storeDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            storeDTO.EntityState = System.Data.EntityState.Added;
            Store store = new Store().FromEntityData(storeDTO);
            contextSession.SaveObject(store);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        public void Updates(StoreDTO storeDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            storeDTO.EntityState = System.Data.EntityState.Modified;
            Store store = new Store().FromEntityData(storeDTO);
            contextSession.SaveObject(store);
            contextSession.SaveChanges();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        public void Del(Store store)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            store.EntityState = System.Data.EntityState.Deleted;
            contextSession.Delete(store);
            contextSession.SaveChange();
        }
	}
}



