
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/19 18:07:27
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class StoreBP : BaseBP, IStore
    {

        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        public ResultDTO AddStore(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            base.Do();
            return this.AddStoreExt(storeDTO);
        }
        /// <summary>
        /// 删除门店
        /// </summary>
        /// <param name="id">门店ID</param>
        public ResultDTO DelStore(System.Guid id)
        {
            base.Do();
            return this.DelStoreExt(id);
        }


        /// <summary>
        /// 根据AppId得到门店详细信息 舌尖在线，目前只有一个
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAppStore(Guid appId)
        {
            base.Do();
            return this.GetAppStoreExt(appId);
        }


        /// <summary>
        /// 得到门店详细信息
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.StoreDTO GetStoreDTO(System.Guid id, Guid appid)
        {
            base.Do();
            return this.GetStoreDTOExt(id, appid);
        }
        /// <summary>
        /// 修改门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        public ResultDTO UpdateStore(Jinher.AMP.BTP.Deploy.StoreDTO storeDTO)
        {
            base.Do();
            return this.UpdateStoreExt(storeDTO);
        }
        /// <summary>
        /// 得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStore(System.Guid sellerId, int pageSize, int pageIndex, out int rowCount)
        {
            base.Do();
            return this.GetAllStoreExt(sellerId, pageSize, pageIndex, out rowCount);
        }

        /// <summary>
        /// 根据条件得到所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAllStoreByWhere(Guid sellerId, int pageSize, int pageIndex, out int rowCount, string storeName, string provice, string city, string district)
        {
            base.Do();
            return this.GetAllStoreByWhereExt(sellerId, pageSize, pageIndex, out rowCount, storeName, provice, city, district);
        }
    }
}