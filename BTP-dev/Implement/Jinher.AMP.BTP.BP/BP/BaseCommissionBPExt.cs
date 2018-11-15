
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

    public partial class BaseCommissionBP : BaseBP, IBaseCommission
    {
        /// <summary>
        /// 查询佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> GetBaseCommissionListExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO search)
        {

            var BaseCommissionlist = BaseCommission.ObjectSet().Where(p => p.IsDel == true).ToList();
            if (!string.IsNullOrWhiteSpace(search.MallApplyId.ToString()) && (!search.MallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                BaseCommissionlist = BaseCommissionlist.Where(p => p.MallApplyId == search.MallApplyId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(search.UserId.ToString()) && (!search.UserId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                BaseCommissionlist = BaseCommissionlist.Where(p => p.UserId == search.UserId).ToList();
            }
            var mallapplylist = MallApply.ObjectSet().ToList();
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> searchlist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO>();
            if (BaseCommissionlist.Count() > 0)
            {
                foreach (var item in BaseCommissionlist)
                {
                    CBC.Deploy.CustomDTO.UserBasicInfoDTO commodityuser = CBCSV.Instance.GetUserBasicInfoNew(item.UserId);
                    var mallapply = mallapplylist.FirstOrDefault(p => p.Id == item.MallApplyId);
                    Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO();
                    model.Id = item.Id;
                    model.SubTime = item.SubTime;
                    model.ModifiedOn = item.ModifiedOn;
                    if (mallapply != null)
                    {
                        model.EsAppName = mallapply.EsAppName;
                    }
                    if (commodityuser != null)
                    {
                        model.UserName = commodityuser.Name;
                    }
                    model.AppName = item.AppName;
                    model.Commission = item.Commission;
                    model.EffectiveTime = item.EffectiveTime;
                    model.MallApplyId = item.MallApplyId;
                    model.UserId = item.UserId;
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
        public ResultDTO SaveBaseCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            ResultDTO dto = null;
            BaseCommission entity = new BaseCommission();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                entity.Id = model.Id;
                entity.SubTime = model.SubTime;
                entity.ModifiedOn = model.ModifiedOn;
                entity.AppName = model.AppName;
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
                LogHelper.Error(string.Format("佣金信息保存异常。MallApply：{0}", ex.Message), ex);
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 修改佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateBaseCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var baseCommission = BaseCommission.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (baseCommission != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.AppName))
                    {
                        baseCommission.AppName = model.AppName;
                    }
                    baseCommission.ModifiedOn = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(model.MallApplyId.ToString()) && (!model.MallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        baseCommission.MallApplyId = model.MallApplyId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.UserId.ToString()) && (!model.UserId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        baseCommission.UserId = model.UserId;
                    }
                    baseCommission.Commission = model.Commission;
                    baseCommission.EffectiveTime = model.EffectiveTime;
                    baseCommission.IsDel = model.IsDel;
                    baseCommission.EntityState = EntityState.Modified;
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
                LogHelper.Error(string.Format("佣金信息修改异常。MallApply：{0}", ex.Message), ex);
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 删除佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelBaseCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var baseCommission = BaseCommission.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (baseCommission != null)
                {
                    baseCommission.EntityState = EntityState.Deleted;
                    contextSession.Delete(baseCommission);
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除佣金成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "基础佣金信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("佣金信息删除异常。MallApply：{0}", ex.Message), ex);
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据id获取基础佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO GetBaseCommissionExt(Guid id, Guid mallApplyId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO();
            BaseCommission baseCommission = null;
            if (!string.IsNullOrWhiteSpace(id.ToString()) && (!id.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                baseCommission = BaseCommission.ObjectSet().OrderByDescending(p => p.SubTime).FirstOrDefault(p => p.Id == id);
            }
            if (!string.IsNullOrWhiteSpace(mallApplyId.ToString()) && (!mallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                baseCommission = BaseCommission.ObjectSet().OrderByDescending(p => p.SubTime).FirstOrDefault(p => p.MallApplyId == mallApplyId);
            }
            if (baseCommission != null)
            {
                CBC.Deploy.CustomDTO.UserBasicInfoDTO commodityuser = CBCSV.Instance.GetUserBasicInfoNew(baseCommission.UserId);
                var mallapplyid = MallApply.ObjectSet().FirstOrDefault(p => p.Id == baseCommission.MallApplyId);
                if (mallapplyid != null)
                {
                    model.AppId = mallapplyid.AppId;
                    model.EsAppName = model.AppName;
                }
                if (commodityuser != null)
                {
                    model.UserName = commodityuser.Name;
                }
                model.Id = baseCommission.Id;
                model.MallApplyId = baseCommission.MallApplyId;
                model.ModifiedOn = baseCommission.ModifiedOn;
                model.SubTime = baseCommission.SubTime;
                model.EffectiveTime = baseCommission.EffectiveTime;
                model.Commission = baseCommission.Commission;
                model.UserId = baseCommission.UserId;
                model.IsDel = baseCommission.IsDel;
                model.AppName = baseCommission.AppName;
            }
            return model;
        }

    }
}
