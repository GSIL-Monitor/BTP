
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/12 10:07:14
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
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Data;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 添加品牌分类关系
    /// </summary>
    public partial class CategoryInnerBrandBP : BaseBP, ICategoryInnerBrand
    {

        public ResultDTO AddExt(Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO model)
        {
            try
            {
                var categoryInnerBrand = CategoryInnerBrand.CreateCategoryInnerBrand();

                categoryInnerBrand.BrandId = model.BrandId;
                categoryInnerBrand.Brandname = model.Brandname;
                categoryInnerBrand.CategoryId = model.CategoryId;
                categoryInnerBrand.CategoryName = model.CategoryName;
                categoryInnerBrand.SubId = model.SubId;
                categoryInnerBrand.SubName = model.SubName;
                categoryInnerBrand.SubTime = DateTime.Now;
                categoryInnerBrand.EntityState = System.Data.EntityState.Added;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(categoryInnerBrand);
                contextSession.SaveChanges();
                return new ResultDTO { ResultCode = 0, Message = "", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加添加品牌分类关系发生错误。参数：{0}", JsonHelper.JsonSerializer(model)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error", isSuccess = false };
            }
        }

        public Deploy.CustomDTO.ResultDTO<IList<CategoryInnerBrandDTO>> GetBrandWallListExt(System.Guid CategoryId)
        {
            try
            {
                var list = CategoryInnerBrand.ObjectSet().Where(_ => _.CategoryId == CategoryId);
                var listDto = from n in list
                              select new CategoryInnerBrandDTO
                              {
                                  Id = n.Id,
                                  BrandId = n.BrandId,
                                  Brandname = n.Brandname,
                                  CategoryId = n.CategoryId,
                                  CategoryName = n.CategoryName
                              };


                var dto = new ResultDTO<IList<CategoryInnerBrandDTO>>()
                {
                    Data = listDto.ToList(),
                    isSuccess = true,
                    Message = "Sucess",
                    ResultCode = 0
                };

                return dto;
            }
            catch (Exception ex)
            {
                var dto = new ResultDTO<IList<CategoryInnerBrandDTO>>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "fail",
                    ResultCode = 1
                };
                return dto;
            }
        }
        /// <summary>
        /// 删除多个分类品牌
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public ResultDTO DeleteCateBrandsByCategoryIdExt(System.Guid CategoryId)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var categoryInnerBrand = CategoryInnerBrand.ObjectSet().Where(p => p.CategoryId.Equals(CategoryId)).AsQueryable();
                if (categoryInnerBrand.Count() > 0)
                {
                    foreach (var item in categoryInnerBrand)
                    {
                        item.EntityState = EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除分类品牌成功", isSuccess = true };
                }
                else
                {
                    //第一次添加时，分类品牌不存在
                    dto = new ResultDTO() { ResultCode = 0, Message = "分类品牌信息不存在", isSuccess = true };
                }
            }
            catch (Exception ex)
            {
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 添加多个分类品牌
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddListExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryInnerBrandDTO> models)
        {
            ResultDTO dto = null;
            try
            {
                foreach (var model in models)
                {
                    var item = CategoryInnerBrand.CreateCategoryInnerBrand();
                    item.BrandId = model.BrandId;
                    item.Brandname = model.Brandname;
                    item.CategoryId = model.CategoryId;
                    item.CategoryName = model.CategoryName;
                    item.SubTime = model.SubTime;
                    item.ModifiedOn = model.ModifiedOn;
                    item.EntityState = EntityState.Added;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }
                ContextFactory.CurrentThreadContext.SaveChanges();

                dto = new ResultDTO() { ResultCode = 0, Message = "添加分类品牌成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
    }
}