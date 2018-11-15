
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
    public partial class JdlogsBP : BaseBP, IJdlogs
    {
        /// <summary>
        /// 查询所有的京东日志信息
        /// </summary>
        /// <param name="search">查询类</param>
        public List<Jinher.AMP.BTP.Deploy.JdlogsDTO> GetALLJdlogsListExt(Jinher.AMP.BTP.Deploy.CustomDTO.JdlogsDTO model)
        {
            List<Jinher.AMP.BTP.Deploy.JdlogsDTO> Jdlogslist = new List<Jinher.AMP.BTP.Deploy.JdlogsDTO>();
            var jdlogslist = Jdlogs.ObjectSet().Where(p => p.Isdisable == false).AsQueryable();
            if (model.AppId != Guid.Empty)
            {
                jdlogslist = jdlogslist.Where(p => p.AppId == model.AppId);
            }
            if (!string.IsNullOrEmpty(model.Content))
            {
                jdlogslist = jdlogslist.Where(p => p.Content.Contains(model.Content));
            }
            if (model.StartDate.ToString() != "0001/1/1 0:00:00" && model.EndDate.ToString() != "0001/1/1 0:00:00")
            {
                DateTime startTime = DateTime.Parse(model.StartDate.AddDays(-1).ToShortDateString().ToString() + " 23:59:59");
                DateTime endTime = DateTime.Parse(model.EndDate.ToShortDateString().ToString() + " 23:59:59");
                jdlogslist = jdlogslist.Where(p => p.ModifiedOn >= startTime && p.ModifiedOn <= endTime);
            }
            if (model.ThirdECommerceType == ThirdECommerceTypeEnum.NotThirdECommerce || model.ThirdECommerceType == ThirdECommerceTypeEnum.JingDongDaKeHu)
            {
                jdlogslist = jdlogslist.Where(p => !p.ThirdECommerceType.HasValue);
            }
            else
            {
                jdlogslist = jdlogslist.Where(p => p.ThirdECommerceType.HasValue && p.ThirdECommerceType.Value == (int)model.ThirdECommerceType);
            }
            if (jdlogslist.Count() > 0)
            {
                foreach (var item in jdlogslist.OrderByDescending(p => p.SubTime).ToList())
                {
                    Jinher.AMP.BTP.Deploy.JdlogsDTO entity = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
                    entity = CommonUtil.ReadObjectExchange(entity, item);
                    Jdlogslist.Add(entity);
                }
            }
            return Jdlogslist;
        }


        /// <summary>
        /// 根据Id获取京东的日志内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.JdlogsDTO GetJdlogsExt(Guid Id)
        {
            Jinher.AMP.BTP.Deploy.JdlogsDTO exEntity = new Jinher.AMP.BTP.Deploy.JdlogsDTO();
            var model = Jdlogs.ObjectSet().FirstOrDefault(p => p.Id == Id);
            if (model != null)
            {
                exEntity = CommonUtil.ReadObjectExchange(exEntity, model);
            }
            return exEntity;
        }



        /// <summary>
        /// 保存京东日志信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdlogsExt(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Jdlogs entity = new Jdlogs();
                entity = CommonUtil.ReadObjectExchange(entity, model);
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存异常。Jdlogs：{0}", ex));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        ///修改京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateJdlogsExt(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var jdlogs = Jdlogs.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (jdlogs != null)
                {
                    if (!string.IsNullOrEmpty(model.Remark))
                    {
                        jdlogs.Remark = model.Remark;
                    }
                    else
                    {
                        jdlogs.Remark = string.Empty;
                    }
                    jdlogs.ModifiedOn = DateTime.Now;
                    jdlogs.EntityState = EntityState.Modified;
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
                LogHelper.Error(string.Format("jdlogs信息保存异常。jdlogs：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据id删除京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdlogsExt(Guid id)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var jdlogs = Jdlogs.ObjectSet().FirstOrDefault(p => p.Id == id);
                if (jdlogs != null)
                {
                    jdlogs.Isdisable = true;
                    jdlogs.ModifiedOn = DateTime.Now;
                    jdlogs.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "删除成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "该信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("jdlogs信息保存异常。jdlogs：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }
    }
}