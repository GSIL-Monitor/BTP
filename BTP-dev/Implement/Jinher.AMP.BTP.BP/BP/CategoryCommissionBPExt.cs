
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:09
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using NPOI.SS.Formula.Functions;
using AppExtensionDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.BP
{

    public partial class CategoryCommissionBP : BaseBP, ICategoryCommission
    {
        /// <summary>
        /// 查询佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO> GetCategoryCommissionListExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO search)
        {
            var CategoryCommissionlist = CategoryCommission.ObjectSet().Where(p =>p.IsDel==true).OrderByDescending(p=>p.SubTime).ToList();
            if (!string.IsNullOrWhiteSpace(search.MallApplyId.ToString()) && (!search.MallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                CategoryCommissionlist = CategoryCommissionlist.Where(p => p.MallApplyId == search.MallApplyId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(search.CategoryId.ToString()) && (!search.CategoryId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                CategoryCommissionlist = CategoryCommissionlist.Where(p => p.CategoryId == search.CategoryId).ToList();
            }
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO> searchlist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO>();
            if (CategoryCommissionlist.Count() > 0)
            {
                foreach (var item in CategoryCommissionlist)
                {
                    CBC.Deploy.CustomDTO.UserBasicInfoDTO commodityuser = CBCSV.Instance.GetUserBasicInfoNew(item.UserId);
                    Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO();
                    model.Id = item.Id;
                    model.SubTime = item.SubTime;
                    model.ModifiedOn = item.ModifiedOn;
                    model.Commission = item.Commission;
                    model.EffectiveTime = item.EffectiveTime;
                    model.UserId = item.UserId;
                    if (commodityuser!=null)
                    {
                        model.UserName = commodityuser.Name;
                    }
                    model.CategoryId = item.CategoryId;
                    model.MallApplyId = item.MallApplyId;
                    model.IsDel = item.IsDel;
                    searchlist.Add(model);
                }
            }
            return searchlist;
        }
        /// <summary>
        /// 保存佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveCategoryCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            ResultDTO dto = null;
            CategoryCommission entity = new CategoryCommission();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                entity.Id = model.Id;
                entity.SubTime = model.SubTime;
                entity.ModifiedOn = model.ModifiedOn;
                entity.CategoryId = model.CategoryId;
                entity.MallApplyId = model.MallApplyId;
                entity.UserId = model.UserId;
                entity.Commission = model.Commission;
                entity.EffectiveTime = model.EffectiveTime;
                entity.IsDel = model.IsDel;
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("类别佣金信息保存异常。MallApply：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 修改佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCategoryCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var categoryCommission = CategoryCommission.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (categoryCommission != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.CategoryId.ToString()))
                    {
                        categoryCommission.CategoryId= model.CategoryId;
                    }
                    categoryCommission.ModifiedOn = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(model.MallApplyId.ToString()) && (!model.MallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        categoryCommission.MallApplyId = model.MallApplyId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.UserId.ToString()) && (!model.UserId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        categoryCommission.UserId = model.UserId;
                    }
                    categoryCommission.Commission = model.Commission;
                    categoryCommission.EffectiveTime = DateTime.Now;
                    categoryCommission.IsDel = model.IsDel;
                    categoryCommission.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "佣金信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("类别佣金信息修改异常。MallApply：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 删除佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelCategoryCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var categoryCommission = CategoryCommission.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (categoryCommission != null)
                {
                    categoryCommission.EntityState = EntityState.Deleted;
                    contextSession.Delete(categoryCommission);
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除类别佣金成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "类别佣金信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("类别信息删除异常。MallApply：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据id获取类别佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO GetCategoryCommissionExt(Guid id)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO();
            var categoryCommission = CategoryCommission.ObjectSet().FirstOrDefault(p => p.Id == id);
            if (categoryCommission != null)
            {
                var mallapplyid = MallApply.ObjectSet().FirstOrDefault(p => p.Id == categoryCommission.MallApplyId);
                if (mallapplyid != null)
                {
                    model.AppId = mallapplyid.AppId;
                }
                model.Id = categoryCommission.Id;
                model.MallApplyId = categoryCommission.MallApplyId;
                model.ModifiedOn = categoryCommission.ModifiedOn;
                model.SubTime = categoryCommission.SubTime;
                model.EffectiveTime = categoryCommission.EffectiveTime;
                model.Commission = categoryCommission.Commission;
                model.UserId = categoryCommission.UserId;
                model.IsDel = categoryCommission.IsDel;
                model.CategoryId = categoryCommission.CategoryId;
            }
            return model;
        }

        
    }
}
