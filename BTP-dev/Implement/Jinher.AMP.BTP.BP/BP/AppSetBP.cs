
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015-1-9 11:21:55
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
    [Obsolete("已过期，新功能在zph项目中实现", false)]
    public partial class AppSetBP : BaseBP, IAppSet
    {

        /// <summary>
        /// 分页获取所有电商App
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="addToAppSetStatus">-1全部,1已加入到直销,0未加入直销</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetAppGridDTO GetAllCommodityApp(int pageIndex, int pageSize, string appNameForQry, int addToAppSetStatus)
        {
            base.Do();
            return this.GetAllCommodityAppExt(pageIndex, pageSize, appNameForQry, addToAppSetStatus);
        }
        /// <summary>
        /// 添加应用到应用组
        /// </summary>
        /// <param name="appInfoList">应用信息列表</param>
        /// <param name="appSetId">应用组id</param>
        /// <param name="appSetType">应用组类型</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAppToAppSet(List<Tuple<Guid, string, string, DateTime>> appInfoList, System.Guid appSetId, int appSetType)
        {
            base.Do();
            return this.AddAppToAppSetExt(appInfoList, appSetId, appSetType);
        }
        /// <summary>
        /// 从应用组移除应用
        /// </summary>
        /// <param name="appIdList">应用id列表</param>
        /// <param name="appSetId">应用组id</param>
        /// <param name="appSetType">应用组类型</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelAppFromAppSet(System.Collections.Generic.List<System.Guid> appIdList, System.Guid appSetId, int appSetType)
        {
            base.Do();
            return this.DelAppFromAppSetExt(appIdList, appSetId, appSetType);
        }
        /// <summary>
        /// 获取树分类列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO> GetCategoryListForTree()
        {
            base.Do();
            return this.GetCategoryListForTreeExt();
        }

        /// <summary>
        /// 获取树分类列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCategoryDTO> GetCategoryListForTree(Guid appId)
        {
            base.Do();
            return this.GetCategoryListForTreeExt(appId);
        }

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="name">分类名称</param>
        /// <param name="parentId">父分类Id</param>
        /// <param name="picturesPath">图片路径</param>
        /// <returns></returns>
        public Tuple<Guid, int, string> AddCategory(string name, Guid parentId, string picturesPath)
        {
            base.Do();
            return this.AddCategoryExt(name, parentId, picturesPath);
        }
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id">分类id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelCategory(System.Guid id)
        {
            base.Do();
            return this.DelCategoryExt(id);
        }
        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="id">分类id</param>
        /// <param name="name">分类名称</param>
        /// <param name="picturesPath">分类图片</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategory(System.Guid id, string name, string picturesPath)
        {
            base.Do();
            return this.UpdateCategoryExt(id, name, picturesPath);
        }
        /// <summary>
        /// 分类移动
        /// </summary>
        /// <param name="categoryId">被调序分类的id</param>
        /// <param name="targetCategoryId">与被调序分类互换顺序的分类</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ChangeCategorySort(System.Guid categoryId, System.Guid targetCategoryId)
        {
            base.Do();
            return this.ChangeCategorySortExt(categoryId, targetCategoryId);
        }
        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <param name="categoryId">分类的id</param>

        /// <returns></returns>
        public int GetCommodityCountInCategory(System.Guid categoryId)
        {
            base.Do();
            return this.GetCommodityCountInCategoryExt(categoryId);
        }
        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <param name="categoryId">分类的id</param>

        /// <returns></returns>
        public int GetCommodityCountInCategory2(System.Guid categoryId)
        {
            base.Do();
            return this.GetCommodityCountInCategory2Ext(categoryId);
        }
        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="commodityNameForQry">商品名称查询字符串</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGridDTO GetCommodityInCategory(System.Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry)
        {
            base.Do();
            return this.GetCommodityInCategoryExt(categoryId, pageIndex, pageSize, appNameForQry, commodityNameForQry);
        }
        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="commodityNameForQry">商品名称查询字符串</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppSetCommodityGrid2DTO GetCommodityInCategory2(Guid belongTo, System.Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry)
        {
            base.Do(false);
            return this.GetCommodityInCategory2Ext(belongTo, categoryId, pageIndex, pageSize, appNameForQry, commodityNameForQry);
        }


        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryIdList">分类id列表</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCommodityToCategory(System.Collections.Generic.List<System.Guid> commodityIdList, System.Collections.Generic.List<System.Guid> categoryIdList)
        {
            base.Do();
            return this.AddCommodityToCategoryExt(commodityIdList, categoryIdList);
        }
        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryIdList">分类id列表</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCommodityToCategory2(System.Collections.Generic.List<System.Guid> commodityIdList, System.Collections.Generic.List<System.Guid> categoryIdList)
        {
            base.Do();
            return this.AddCommodityToCategory2Ext(commodityIdList, categoryIdList);
        }
        /// <summary>
        /// 分类下商品排序
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="commoditySortList">商品序号列表</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ReOrderCommodityInCategory(System.Guid categoryId, System.Collections.Generic.List<System.Guid> commodityIdList, System.Collections.Generic.List<int> commoditySortList)
        {
            base.Do();
            return this.ReOrderCommodityInCategoryExt(categoryId, commodityIdList, commoditySortList);
        }
        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryId">分类id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelCommodityFromCategory(System.Collections.Generic.List<System.Guid> commodityIdList, System.Guid categoryId)
        {
            base.Do();
            return this.DelCommodityFromCategoryExt(commodityIdList, categoryId);
        }
        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryId">分类id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelCommodityFromCategory2(System.Collections.Generic.List<System.Guid> commodityIdList, System.Guid categoryId)
        {
            base.Do();
            return this.DelCommodityFromCategory2Ext(commodityIdList, categoryId);
        }
        /// <summary>
        /// 调整分类中商品排序(上移\下移)
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityId">商品id</param>
        /// <param name="direction">调整方向(正数下移,负数上移)</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO ChangeCommodityOrderInCategory(Guid categoryId, Guid commodityId, int direction)
        {
            base.Do();
            return this.ChangeCommodityOrderInCategoryExt(categoryId, commodityId, direction);
        }
        /// <summary>
        /// 商品在分类中置顶
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityId">商品id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO TopCommodityOrderInCategory(Guid categoryId, Guid commodityId)
        {
            base.Do();
            return this.TopCommodityOrderInCategoryExt(categoryId, commodityId);
        }
        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="appSetSortDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetSetCommodityOrder(AppSetSortDTO appSetSortDTO)
        {
            base.Do();
            return this.SetSetCommodityOrderExt(appSetSortDTO);
        }

        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="appSetSortDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetSetCommodityOrder2(AppSetSortDTO appSetSortDTO)
        {
            base.Do();
            return this.SetSetCommodityOrder2Ext(appSetSortDTO);
        }
    }
}