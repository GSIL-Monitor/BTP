
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/3/19 18:07:26
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class StoreFacade : BaseFacade<IStore>
    {

        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        public ResultDTO AddStore(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            base.Do();
            return this.Command.AddStore(storeDTO);
        }
        /// <summary>
        /// 删除门店
        /// </summary>
        /// <param name="id">门店ID</param>
        public ResultDTO DelStore(System.Guid id)
        {
            base.Do();
            return this.Command.DelStore(id);
        }

        /// <summary>
        /// 根据AppId得到门店详细信息 舌尖在线，目前只有一个
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAppStore(Guid appId)
        {
            base.Do();
            return this.Command.GetAppStore(appId);
        }


        /// <summary>
        /// 得到门店详细信息
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.StoreDTO GetStoreDTO(System.Guid id, Guid appid)
        {
            base.Do();
            return this.Command.GetStoreDTO(id,appid);
        }
        /// <summary>
        /// 修改门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        public ResultDTO UpdateStore(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            base.Do();
            return this.Command.UpdateStore(storeDTO);
        }
        /// <summary>
        /// 得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStore(System.Guid sellerId,int pageSize,int pageIndex,out int rowCount)
        {
            base.Do();
            return this.Command.GetAllStore(sellerId,pageSize,pageIndex,out rowCount);
        }

        /// <summary>
        /// 根据条件查询得到所有门店
        /// </summary>
        /// <param name="sellerId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="storeName"></param>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStoreByWhere(Guid sellerId, int pageSize, int pageIndex, out int rowCount, string storeName, string provice, string city, string district)
        {
            base.Do();
            return this.Command.GetAllStoreByWhere(sellerId, pageSize, pageIndex, out rowCount,storeName,provice,city,district);
        }
    }
}