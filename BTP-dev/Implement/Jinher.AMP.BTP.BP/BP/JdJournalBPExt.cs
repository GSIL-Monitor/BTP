
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
    public partial class JdJournalBP : BaseBP, IJdJournal
    {

        /// <summary>
        /// 查询JdJournal信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>

        public List<Jinher.AMP.BTP.Deploy.JdJournalDTO> GetJdJournalListExt(Jinher.AMP.BTP.Deploy.JdJournalDTO search)
        {
            List<Jinher.AMP.BTP.Deploy.JdJournalDTO> JdOrderItemlist = new List<Jinher.AMP.BTP.Deploy.JdJournalDTO>();
            var searchlist = JdJournal.ObjectSet().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search.JdPorderId))
            {
                searchlist = searchlist.Where(p => p.JdPorderId == search.JdPorderId);
            }
            if (search.TempId != Guid.Empty)
            {
                searchlist = searchlist.Where(p => p.TempId == search.TempId);
            }
            if (!string.IsNullOrWhiteSpace(search.JdOrderId)&&search.JdOrderId != "00000000-0000-0000-0000-000000000000")
            {
                searchlist = searchlist.Where(p => p.JdOrderId == search.JdOrderId);
            }
            if (!string.IsNullOrWhiteSpace(search.MainOrderId)&&search.MainOrderId != "00000000-0000-0000-0000-000000000000")
            {
                searchlist = searchlist.Where(p => p.MainOrderId == search.MainOrderId);
            }
            if (!string.IsNullOrWhiteSpace(search.CommodityOrderId))
            {
                searchlist = searchlist.Where(p => p.CommodityOrderId == search.CommodityOrderId);
            }
            if (!string.IsNullOrEmpty(search.Name))
            {
                searchlist = searchlist.Where(p => p.Name.Contains(search.Name));
            }
            if (!string.IsNullOrEmpty(search.Details))
            {
                searchlist = searchlist.Where(p => p.Details.Contains(search.Details));
            }
            foreach (var item in searchlist.ToList())
            {
                Jinher.AMP.BTP.Deploy.JdJournalDTO model = new Jinher.AMP.BTP.Deploy.JdJournalDTO();
                model = CommonUtil.ReadObjectExchange(model, item);
                JdOrderItemlist.Add(model);
            }
            return JdOrderItemlist;
        }

        /// <summary>
        /// 保存JdJournal信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdJournalExt(Jinher.AMP.BTP.Deploy.JdJournalDTO model)
        {
            ResultDTO dto = null;
            JdJournal entity = new JdJournal();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                entity = CommonUtil.ReadObjectExchange(entity, model);
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
        /// 删除JdJournal
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdJournalExt(List<string> jdorders)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var jdjournal = JdJournal.ObjectSet().Where(p => jdorders.Contains(p.JdPorderId)).AsQueryable();
                if (jdjournal.Count() > 0)
                {
                    foreach (var item in jdjournal)
                    {
                        item.EntityState = EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除JdJournal成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "JdJournal信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
       

    }
}