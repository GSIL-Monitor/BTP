
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/25 13:47:03
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityCategoryBP : BaseBP, ICommodityCategory
    {

        #region 数据方法区
        /// <summary>
        /// 添加商品分类
        /// </summary>
        /// <param name="commodityDTO">商品分类实体</param>
        public void SaveCommodityCategory(CommodityCategoryDTO commodityCategoryDTO)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            commodityCategoryDTO.EntityState = System.Data.EntityState.Added;
            commodityCategoryDTO.Id = Guid.NewGuid();
            CommodityCategory commodityCategory = new CommodityCategory().FromEntityData(commodityCategoryDTO);
            contextSession.SaveObject(commodityCategory);
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 删除商品分类 
        /// </summary>
        /// <param name="commodityDTO">商品分类实体</param>
        [Obsolete("已废弃", false)]
        public void DelCommodityCategory(Guid commodityId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            List<CommodityCategory> commodityCategorys = this.GetCommodityCategoryByCommodityId(commodityId);
            foreach (CommodityCategory commodityCategory in commodityCategorys)
            {
                contextSession.Delete(commodityCategory);
            }
            contextSession.SaveChanges();
        }
        /// <summary>
        /// 根据商品ID得到商品分类
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public List<CommodityCategory> GetCommodityCategoryByCommodityId(Guid commodityId)
        {
            List<CommodityCategory> commodityCategory = CommodityCategory.ObjectSet().Where(n => n.CommodityId == commodityId).ToList();
            return commodityCategory;
        }
        #endregion
        /// <summary>
        /// 添加商品分类
        /// </summary>
        /// <param name="commodityCategoryDTO"></param>
        public void AddCommodityCategoryExt(Jinher.AMP.BTP.Deploy.CommodityCategoryDTO commodityCategoryDTO)
        {
            this.SaveCommodityCategory(commodityCategoryDTO);
        }
        /// <summary>
        /// 删除商品分类
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        [Obsolete("已废弃", false)]
        public void DeleteCommodityCategoryExt(Guid commodityId)
        {
            this.DelCommodityCategory(commodityId);
        }
        /// <summary>
        /// 按商品ID查询商品分类 
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <param name="pageSize">分页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetCommodityCategoryByCategoryExt(Guid commodityId, int pageSize, int pageIndex)
        {
            var commodityCategory = CommodityCategory.ObjectSet().Where(n => n.CommodityId == commodityId);
            var result = from c in commodityCategory
                         select new CommodityCategoryDTO
                         {
                             CategoryId = c.CategoryId,
                             Code = c.Code,
                             CommodityId = c.CommodityId,
                             Id = c.Id,
                             Name = c.Name

                         };

            return result.ToList();
        }

        /// <summary>
        /// 根据AppId和CommodityId查询商品分类名称
        /// </summary>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<CommodityCategoryDTO> GetAllCommodityCategory(Guid commodityId, Guid appId, int pageSize, int pageIndex)
        {
            var commodityCategory = CommodityCategory.ObjectSet().Where(n => n.CommodityId == commodityId &&n.AppId==appId);
            var result = from c in commodityCategory
                         join s in Category.ObjectSet() on c.CategoryId equals s.Id
                         select new CommodityCategoryDTO
                         {
                             CategoryId=s.Id,
                             Name=s.Name,
                             AppId=c.AppId,
                             Code = c.Code,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             IsValidate = c.IsValidate,
                             IsValidated = c.IsValidated
                         };
            return result.ToList();
        }

        /// <summary>
        /// 按商品ID查询商品分类DTO 
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <param name="pageSize">分页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [Obsolete("已废弃", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryBycommodityId(Guid commodityId)
        {

            //IQueryable<CommodityCategory> commodityCategory = CommodityCategory.ObjectSet().Where(n => n.CommodityId == commodityId);

            var query = from data in CommodityCategory.ObjectSet()
                        join data1 in Category.ObjectSet() on data.CategoryId equals data1.Id
                        where data.CommodityId == commodityId
                        select data1;
            var result = from c in query
                         select new CategoryDTO
                         {
                             AppId = c.AppId,
                             Code = c.Code,
                             CurrentLevel = c.CurrentLevel,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             IsValidate = c.IsValidate,
                             IsValidated = c.IsValidated,
                             Name = c.Name,
                             ParentId = c.ParentId,
                             Sort = c.Sort
                         };
            return result.ToList();
        }

        /// <summary>
        /// 查询所有商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityCategoryDTO> GetAllCommodityCategoryExt()
        {
            var commodityCategoryList = CommodityCategory.ObjectSet();
            var result = from c in commodityCategoryList
                         select new CommodityCategoryDTO
                         {
                             CategoryId = c.CategoryId,
                             Code = c.Code,
                             CommodityId = c.CommodityId,
                             Id = c.Id,
                             Name = c.Name,
                             IsValidate = c.IsValid,
                             IsValidated = c.IsValidated

                         };
            return result.ToList();
        }
    }
}