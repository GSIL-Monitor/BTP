
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/5/31 18:12:34
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class SpreadCategoryBP : BaseBP, ISpreadCategory
    {

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddExt(Jinher.AMP.BTP.Deploy.SpreadCategoryDTO dto)
        {
            try
            {
                var result = validAdd(dto);
                if (result.ResultCode != 0)
                    return result;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                var spreadCategory = SpreadCategory.CreateSpreadCategory();
                spreadCategory.SpreadType = dto.SpreadType;
                spreadCategory.SpreaderPercent = dto.SpreaderPercent;
                spreadCategory.Priority = dto.Priority;
                spreadCategory.CategoryDesc = dto.Desc;
                contextSession.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadCategoryBP.AddExt 异常：dto:{0}", JsonHelper.JsonSerializer(dto)), ex);
                return new ResultDTO { ResultCode = -1, Message = "操作失败" };
            }
        }

        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO validAdd(Jinher.AMP.BTP.Deploy.SpreadCategoryDTO dto)
        {
            if (dto == null)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            if (dto.CategoryDesc.IsNullVauleFromWeb())
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            var already = SpreadCategory.ObjectSet().FirstOrDefault(c => c.SpreadType == dto.SpreadType);
            if (already != null)
                return new ResultDTO { ResultCode = 2, Message = "分类编码已存在" };
            return new ResultDTO { isSuccess = true, Message = "success" };
        }


        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteExt(System.Guid id)
        {
            try
            {
                ResultDTO result = new ResultDTO { isSuccess = true, Message = "success" };
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var spreadCategory = SpreadCategory.ObjectSet().FirstOrDefault(c => c.Id == id);
                if (spreadCategory == null)
                    return result;
                var usedCount = SpreadInfo.ObjectSet().Count(c => c.IsDel == 1 && c.SpreadType == spreadCategory.SpreadType);
                if (usedCount > 0)
                {
                    result.ResultCode = 2;
                    result.isSuccess = false;
                    result.Message = "该分类已被使用，无法删除";
                }
                spreadCategory.EntityState = EntityState.Deleted;
                contextSession.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadCategoryBP.DeleteExt 异常：dto:{0}", id), ex);
                return new ResultDTO { ResultCode = -1, Message = "操作失败" };
            }
        }

        /// <summary>
        /// 获取推广主类型
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public ResultDTO<List<SpreadCategoryDTO>> GetSpreadCategoryListExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSearchDTO search)
        {
            List<SpreadCategoryDTO> dtos = new List<SpreadCategoryDTO>();
            var list = SpreadCategory.ObjectSet().ToList();
            if (list.Any())
            {
                dtos.AddRange(list.Select(spreadCategory => spreadCategory.ToEntityData()));
            }
            return new ResultDTO<List<SpreadCategoryDTO>> { isSuccess = true, Data = dtos };
        }
    }
}