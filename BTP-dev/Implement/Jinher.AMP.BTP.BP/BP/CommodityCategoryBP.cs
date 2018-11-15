
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/15 13:42:07
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class CommodityCategoryBP : BaseBP, ICommodityCategory
    {

        /// <summary>
        /// 添加商品分类
        /// </summary>
        /// <param name="commodityCategoryDTO"></param>
        public void AddCommodityCategory(Jinher.AMP.BTP.Deploy.CommodityCategoryDTO commodityCategoryDTO)
        {
            base.Do();
            this.AddCommodityCategoryExt(commodityCategoryDTO);
        }
        /// <summary>
        /// 删除商品分类
        /// </summary>
        /// <param name="commodityId">商品id</param>
        public void DeleteCommodityCategory(System.Guid commodityId)
        {
            base.Do();
            this.DeleteCommodityCategoryExt(commodityId);
        }
        /// <summary>
        /// 按商品分类查询商品
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <param name="pageSize">分页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetCommodityCategoryByCategory(System.Guid categoryId, int pageSize, int pageIndex)
        {
            base.Do();
            return this.GetCommodityCategoryByCategoryExt(categoryId, pageSize, pageIndex);
        }
        /// <summary>
        /// 查询所有商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetAllCommodityCategory()
        {
            base.Do();
            return this.GetAllCommodityCategoryExt();
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetCommodityCategoryByAppId(System.Guid appId, int pageSize, int pageIndex)
        {
            base.Do();
            return this.GetCommodityCategoryByCategoryExt(appId, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据appId和commodityId查询所有商品
        /// </summary>
        /// <param name="appIid"></param>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetAllCommodityCategoryByAppId(Guid appId, Guid commodityId, int pageSize, int pageIndex)
        {
            base.Do();
            return this.GetAllCommodityCategoryByAppId(appId, commodityId, pageSize, pageIndex);
        }
        
    }
}