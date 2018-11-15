
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

    public partial class CommodityCommissionBP : BaseBP, ICommodityCommission
    {
        /// <summary>
        /// 查询佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> GetCommodityCommissionListExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO search)
        {

            var CommodityCommissionlist = CommodityCommission.ObjectSet().Where(p => p.IsDel == true).OrderByDescending(p => p.SubTime).ToList();
            if (!string.IsNullOrWhiteSpace(search.MallApplyId.ToString()) && (!search.MallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                CommodityCommissionlist = CommodityCommissionlist.Where(p => p.MallApplyId == search.MallApplyId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(search.CommodityId.ToString()) && (!search.CommodityId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
            {
                CommodityCommissionlist = CommodityCommissionlist.Where(p => p.CommodityId == search.CommodityId).ToList();
            }
            List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> searchlist = new List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO>();
            if (CommodityCommissionlist.Count() > 0)
            {
                foreach (var item in CommodityCommissionlist)
                {
                    CBC.Deploy.CustomDTO.UserBasicInfoDTO commodityuser = CBCSV.Instance.GetUserBasicInfoNew(item.UserId);
                    Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO();
                    model.Id = item.Id;
                    model.SubTime = item.SubTime;
                    model.ModifiedOn = item.ModifiedOn;
                    model.Commission = item.Commission;
                    model.EffectiveTime = item.EffectiveTime;
                    model.UserId = item.UserId;
                    if (commodityuser != null)
                    {
                        model.UserName = commodityuser.Name;
                    }
                    model.CategoryId = item.CategoryId;
                    model.MallApplyId = item.MallApplyId;
                    model.CommodityId = item.CommodityId;
                    model.CommodityName = item.CommodityName;
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
        public ResultDTO SaveCommodityCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            ResultDTO dto = null;
            CommodityCommission entity = new CommodityCommission();
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
                entity.CommodityId = model.CommodityId;
                entity.CommodityName = model.CommodityName;
                entity.EffectiveTime = model.EffectiveTime;
                entity.IsDel = model.IsDel;
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品佣金信息保存异常。MallApply：{0}", ex.Message), ex);
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 修改佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodityCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityCommission = CommodityCommission.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (commodityCommission != null)
                {

                    if (!string.IsNullOrWhiteSpace(model.CategoryId.ToString()) && (!model.CategoryId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        commodityCommission.CategoryId = model.CategoryId;
                    }
                    commodityCommission.ModifiedOn = DateTime.Now;
                    if (!string.IsNullOrWhiteSpace(model.MallApplyId.ToString()) && (!model.MallApplyId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        commodityCommission.MallApplyId = model.MallApplyId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.UserId.ToString()) && (!model.UserId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        commodityCommission.UserId = model.UserId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.CommodityId.ToString()) && (!model.CommodityId.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                    {
                        commodityCommission.CommodityId = model.CommodityId;
                    }
                    if (!string.IsNullOrWhiteSpace(commodityCommission.CommodityName))
                    {
                        commodityCommission.CommodityName = model.CommodityName;
                    }
                    commodityCommission.Commission = model.Commission;
                    commodityCommission.EffectiveTime = DateTime.Now;
                    commodityCommission.IsDel = model.IsDel;
                    commodityCommission.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "商品佣金信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品佣金信息修改异常。MallApply：{0}", ex.Message), ex);
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 删除佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DelCommodityCommissionExt(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityCommission = CommodityCommission.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (commodityCommission != null)
                {
                    commodityCommission.EntityState = EntityState.Deleted;
                    contextSession.Delete(commodityCommission);
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除商品佣金成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "商品佣金信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品信息删除异常。MallApply：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;

        }


        /// <summary>
        /// 根据id获取商品佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO GetCommodityCommissionExt(Guid id)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model = new Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO();
            var commodityCommission = CommodityCommission.ObjectSet().FirstOrDefault(p => p.Id == id);
            if (commodityCommission != null)
            {
                var mallapplyid = MallApply.ObjectSet().FirstOrDefault(p => p.Id == commodityCommission.MallApplyId);
                if (mallapplyid != null)
                {
                    model.AppId = mallapplyid.AppId;
                }
                model.Id = commodityCommission.Id;
                model.MallApplyId = commodityCommission.MallApplyId;
                model.ModifiedOn = commodityCommission.ModifiedOn;
                model.SubTime = commodityCommission.SubTime;
                model.EffectiveTime = commodityCommission.EffectiveTime;
                model.Commission = commodityCommission.Commission;
                model.UserId = commodityCommission.UserId;
                model.IsDel = commodityCommission.IsDel;
                model.CategoryId = commodityCommission.CategoryId;
                model.CommodityId = commodityCommission.CommodityId;
                model.CommodityName = commodityCommission.CommodityName;
            }
            return model;
        }

    }
}
