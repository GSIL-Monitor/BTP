
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

    /// <summary>
    /// 
    /// </summary>
    public partial class JdOrderItemBP : BaseBP, IJdOrderItem
    {
        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemListExt(Jinher.AMP.BTP.Deploy.JdOrderItemDTO search)
        {
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> JdOrderItemlist = new List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO>();
            var searchlist = JdOrderItem.ObjectSet().AsQueryable();
            try
            {
                if (!string.IsNullOrEmpty(search.JdPorderId))
                {
                    searchlist = searchlist.Where(p => p.JdPorderId == search.JdPorderId);
                }
                if (search.TempId != Guid.Empty)
                {
                    searchlist = searchlist.Where(p => p.TempId == search.TempId);
                }
                if (!string.IsNullOrEmpty(search.JdOrderId) && search.JdOrderId != "00000000-0000-0000-0000-000000000000")
                {
                    searchlist = searchlist.Where(p => p.JdOrderId == search.JdOrderId);
                }
                if (!string.IsNullOrEmpty(search.MainOrderId) && search.MainOrderId != "00000000-0000-0000-0000-000000000000")
                {
                    searchlist = searchlist.Where(p => p.MainOrderId == search.MainOrderId.Trim().ToLower());
                }
                if (!string.IsNullOrEmpty(search.CommodityOrderId))
                {
                    searchlist = searchlist.Where(p => p.CommodityOrderId == search.CommodityOrderId.Trim().ToLower());
                }
                if (search.State != 0)
                {
                    searchlist = searchlist.Where(p => p.State == search.State);
                }
                JdOrderItemlist=searchlist.ToList().Select(p => p.ToEntityData()).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdOrderItemBP.GetJdOrderItemListExt:", ex);
            }
            return JdOrderItemlist;
        }

        /// <summary>
        /// 保存JdOrderItem信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdOrderItemExt(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                JdOrderItem entity = new JdOrderItem();
                entity.Id = model.Id;
                entity.JdPorderId = model.JdPorderId;
                entity.TempId = model.TempId;
                entity.JdOrderId = model.JdOrderId;
                entity.MainOrderId = model.MainOrderId;
                entity.CommodityOrderId = model.CommodityOrderId;
                entity.State = model.State;
                entity.StateContent = model.StateContent;
                entity.SubTime = model.SubTime;
                entity.ModifiedOn = model.ModifiedOn;
                entity.CommoditySkuId = model.CommoditySkuId;
                entity.CommodityOrderItemId = model.CommodityOrderItemId;
                entity.IsRefund = model.IsRefund;
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("JdOrderItem保存异常。JdOrderItem：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 修改JdOrderItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateJdOrderItemExt(Jinher.AMP.BTP.Deploy.JdOrderItemDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var jdOrderItem = JdOrderItem.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (jdOrderItem != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.JdPorderId))
                    {
                        jdOrderItem.JdPorderId = model.JdPorderId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.JdOrderId))
                    {
                        jdOrderItem.JdOrderId = model.JdOrderId;
                    }
                    if (model.TempId!=Guid.Empty)
                    {
                        jdOrderItem.TempId = model.TempId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.MainOrderId))
                    {
                        jdOrderItem.MainOrderId = model.MainOrderId;
                    }
                    if (!string.IsNullOrWhiteSpace(model.CommodityOrderId))
                    {
                        jdOrderItem.CommodityOrderId = model.CommodityOrderId;
                    }
                    jdOrderItem.State = model.State;
                    if (!string.IsNullOrWhiteSpace(model.StateContent))
                    {
                        jdOrderItem.StateContent = model.StateContent;
                    }
                    jdOrderItem.CommoditySkuId = model.CommoditySkuId;
                    jdOrderItem.CommodityOrderItemId = model.CommodityOrderItemId;
                    jdOrderItem.SubTime = model.SubTime;
                    jdOrderItem.ModifiedOn = model.ModifiedOn;
                    jdOrderItem.IsRefund = model.IsRefund;
                    jdOrderItem.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "修改成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "该信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("JdOrderItem信息保存异常。JdOrderItem：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 删除JdOrderItem
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdOrderItemExt(List<string> jdorders)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var jdorderitem = JdOrderItem.ObjectSet().Where(p => jdorders.Contains(p.JdPorderId)).AsQueryable();
                if (jdorderitem.Count()>0)
                {
                    foreach (var item in jdorderitem)
                    {
                        item.EntityState = EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除JdOrderItem成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "JdOrderItem信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetListExt(List<string> jdporders)
        {
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> JdOrderItemlist = new List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO>();
            var searchlist = JdOrderItem.ObjectSet().AsQueryable();
            if (jdporders.Count>0)
            {
                searchlist = searchlist.Where(p => jdporders.Contains(p.JdPorderId));
            }
            JdOrderItemlist = searchlist.ToList().Select(p => p.ToEntityData()).ToList();
            return JdOrderItemlist;
        }

        /// <summary>
        /// 查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderIdListExt(List<string> jdorders)
        {
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> JdOrderItemlist = new List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO>();
            var searchlist = JdOrderItem.ObjectSet().AsQueryable();
            if (jdorders.Count > 0)
            {
                searchlist = searchlist.Where(p => jdorders.Contains(p.JdOrderId));
            }
            JdOrderItemlist = searchlist.ToList().Select(p => p.ToEntityData()).ToList();
            return JdOrderItemlist;
        }

        /// <summary>
        /// 根据订单Id查询JdOrderItem信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> GetJdOrderItemListsExt(List<Guid> TempIds)
        {
            List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO> JdOrderItemlist = new List<Jinher.AMP.BTP.Deploy.JdOrderItemDTO>();
            var searchlist = JdOrderItem.ObjectSet().AsQueryable();
            if (TempIds.Count > 0)
            {
                searchlist = searchlist.Where(p => TempIds.Contains(p.TempId));
            }
            JdOrderItemlist = searchlist.ToList().Select(p => p.ToEntityData()).ToList();
            return JdOrderItemlist;
        }
    }
}