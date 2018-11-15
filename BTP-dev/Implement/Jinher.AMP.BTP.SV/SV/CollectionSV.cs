
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/3/26 10:14:45
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CollectionSV : BaseSv, ICollection
    {

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCollection(System.Guid commodityId, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.SaveCollectionExt(commodityId, userId, appId);

        }
        /// <summary>
        /// 根据用户ID查询收藏商品
        /// </summary>
        /// <param name="userId">商品ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetCollectionItems(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.GetCollectionItemsExt(userId, appId);

        }
        /// <summary>
        /// 删除收藏
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCollection(System.Guid commodityId, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.DeleteCollectionExt(commodityId, userId, appId);

        }
    }
}