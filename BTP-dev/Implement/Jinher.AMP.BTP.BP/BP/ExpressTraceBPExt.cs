/***************
功能描述: BTP-OPTBP
作    者: 
创建时间: 2015/7/30 17:59:54
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
using Jinher.JAP.Common.Loging;
using System.Data;

namespace Jinher.AMP.BTP.BP
{  

    public partial class ExpressTraceBP : BaseBP, IExpressTrace
    {
        /// <summary>
        /// 根据ExpRouteId查询物流详细信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<ExpressTraceDTO> GetExpressTraceListExt(ExpressTraceDTO search)
        {
            var expressTracelist = ExpressTrace.ObjectSet().ToList();
            if (search.ExpRouteId!=Guid.Empty)
            {
                expressTracelist = expressTracelist.Where(p => p.ExpRouteId == search.ExpRouteId).ToList();
            }
            List<ExpressTraceDTO> searchlist = new List<ExpressTraceDTO>();
            if (expressTracelist.Count() > 0)
            {
                foreach (var item in expressTracelist)
                {
                    ExpressTraceDTO model = new ExpressTraceDTO();
                    model.Id = item.Id;
                    model.ExpRouteId = item.ExpRouteId;
                    model.AcceptTime = item.AcceptTime;
                    model.AcceptStation = item.AcceptStation;
                    model.Remark = item.Remark;
                    searchlist.Add(model);
                }
            }
            return searchlist;
        }
        /// <summary>
        /// 保存物流详细信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveExpressTraceListExt(List<ExpressTraceDTO> list)
        {
           
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        ExpressTrace model = new ExpressTrace();
                        model.Id = item.Id;
                        model.ExpRouteId = item.ExpRouteId;
                        model.AcceptTime = item.AcceptTime;
                        model.AcceptStation = item.AcceptStation;
                        model.Remark = item.Remark;
                        model.EntityState = EntityState.Added;
                        contextSession.SaveObject(model);
                    }
                }
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("佣金信息保存异常。ExpressTrace：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据ExpRouteId删除物流详细信息
        /// </summary>
        /// <returns></returns>
        public ResultDTO DelExpressTraceExt(Guid ExpRouteId)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var expressTrace = ExpressTrace.ObjectSet().Where(p => p.ExpRouteId == ExpRouteId).ToList();
                if (expressTrace.Count()>0)
                {
                    foreach (var item in expressTrace)
                    {
                        item.EntityState = EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                    contextSession.SaveChanges();
                }
                dto = new ResultDTO() { ResultCode = 0, Message = "删除物流信息成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("佣金信息删除异常。MallApply：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
    }
}
