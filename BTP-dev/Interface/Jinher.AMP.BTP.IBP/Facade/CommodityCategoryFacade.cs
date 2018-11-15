
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/4/15 13:42:06
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class CommodityCategoryFacade : BaseFacade<ICommodityCategory>
    {

        /// <summary>
        /// 添加商品分类
        /// </summary>
        /// <param name="commodityCategoryDTO"></param>
        public void AddCommodityCategory(Jinher.AMP.BTP.Deploy.CommodityCategoryDTO commodityCategoryDTO)
        {
            base.Do();
            this.Command.AddCommodityCategory(commodityCategoryDTO);
        }
        /// <summary>
        /// 删除商品分类
        /// </summary>
        /// <param name="commodityId">商品id</param>
        [Obsolete("已废弃", false)]
        public void DeleteCommodityCategory(System.Guid commodityId)
        {
            base.Do();
            this.Command.DeleteCommodityCategory(commodityId);
        }
        /// <summary>
        /// 按商品分类查询商品
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <param name="pageSize">分页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetCommodityCategoryByCategory(System.Guid categoryId, int pageSize, int pageIndex)
        {
            base.Do();
            return this.Command.GetCommodityCategoryByCategory(categoryId, pageSize, pageIndex);
        }
        /// <summary>
        /// 查询所有商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetAllCommodityCategory()
        {
            base.Do();
            return this.Command.GetAllCommodityCategory();
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
            return this.Command.GetAllCommodityCategoryByAppId(appId, commodityId, pageSize, pageIndex);
        }
      
    }
}