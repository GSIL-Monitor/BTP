
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/2/15 13:45:41
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Data;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ExpressOrderTemplateBP : BaseBP, IExpressOrderTemplate
    {
        /// <summary>
        /// 收藏
        /// PrintExpressOrder
        /// PrintInvoice
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderPrintTemplate> GetExpressOrderTemplateExt(System.Guid appId)
        {
            var opts = new List<OrderPrintTemplate>();

            var eots = (from c in ExpressTemplateCollection.ObjectSet()
                        where c.AppId == appId
                        join e in ExpressOrderTemplate.ObjectSet() on c.TemplateId equals e.Id
                        select e).ToList();
            if (eots == null || eots.Count == 0) return opts;

            var etIds = eots.Select(r => r.Id).ToList();

            var eotps = (from p in ExpressOrderTemplateProperty.ObjectSet()
                         join t in etIds on p.TemplateId equals t
                         select p).ToList();

            if (eotps != null && eotps.Count > 0)
            {
                eots.ForEach(r =>
                {
                    var attrs = eotps.FindAll(p => p.TemplateId == r.Id);
                    opts.Add(new OrderPrintTemplate()
                    {
                        Template = r.ToEntityData(),
                        Property = attrs != null ? new ExpressOrderTemplateProperty().ToEntityDataList(attrs) : new List<ExpressOrderTemplatePropertyDTO>()
                    });
                });
            }

            return opts;
        }

        /// <summary>
        /// 系统和自定义
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExpressTemplateDTO> GetExpressOrderTemplateByAppIdExt(System.Guid appId)
        {
            var opts = new List<ExpressTemplateDTO>();

            var eots = ExpressOrderTemplate.ObjectSet().Where(r => ((r.AppId == appId && r.TemplateType == 1) || r.TemplateType == 0) && r.Status == 0).ToList();
            if (eots == null || eots.Count == 0) return opts;

            var etIds = eots.Select(r => r.Id).ToList();

            var eotps = (from p in ExpressOrderTemplateProperty.ObjectSet()
                         join t in etIds on p.TemplateId equals t
                         select p).ToList();

            if (eotps != null && eotps.Count > 0)
            {
                eots.ForEach(r =>
                {
                    var attrs = eotps.FindAll(p => p.TemplateId == r.Id);
                    opts.Add(new ExpressTemplateDTO()
                    {
                        Id = r.Id,
                        TemplateName = r.TemplateName,
                        TemplateType = r.TemplateType,
                        ExpressImage = r.ExpressImage,
                        Width = r.Width,
                        Height = r.Height,
                        ExpCode = r.ExpressCode,
                        Property = attrs != null ? new ExpressOrderTemplateProperty().ToEntityDataList(attrs) : new List<ExpressOrderTemplatePropertyDTO>()
                    });
                });
            }

            return opts;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO> SaveExt(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            ResultDTO<ExpressOrderTemplateDTO> result = new ResultDTO<ExpressOrderTemplateDTO>() { isSuccess = true };
            if (dto == null || dto.AppId == Guid.Empty || string.IsNullOrEmpty(dto.TemplateName) || string.IsNullOrEmpty(dto.ExpressCode) || (string.IsNullOrEmpty(dto.ExpressImage) && dto.Id == Guid.Empty) || dto.Width < 0 || dto.Height < 0)
            {
                result.isSuccess = false;
                result.Message = "参数错误";
                return result;
            }

            try
            {
                dto.TemplateName = dto.TemplateName.Replace(" ", "");

                var model = ExpressOrderTemplate.ObjectSet().Where(d => d.TemplateName == dto.TemplateName && d.AppId == dto.AppId && d.Status == 0 && d.TemplateType == 1 && d.Id != dto.Id).FirstOrDefault();
                if (model != null)
                {
                    result.isSuccess = false;
                    result.Message = "重复名称";
                }
                else
                {
                    if (dto.Id != Guid.Empty)
                    {
                        model = ExpressOrderTemplate.ObjectSet().Where(d => d.Id == dto.Id).FirstOrDefault();
                        if (model != null)
                        {
                            model.TemplateName = dto.TemplateName;
                            model.ExpressCode = dto.ExpressCode;
                            model.Width = dto.Width;
                            model.Height = dto.Height;
                            if (!string.IsNullOrEmpty(dto.ExpressImage))
                            {
                                model.ExpressImage = dto.ExpressImage;
                            }
                            model.ModifiedOn = DateTime.Now;
                            model.EntityState = EntityState.Modified;
                            ContextFactory.CurrentThreadContext.SaveChanges();
                        }
                        else
                        {
                            result.isSuccess = false;
                            result.Message = "数据不存在";
                        }
                    }
                    else
                    {
                        dto.Id = Guid.NewGuid();
                        dto.Status = 0;
                        dto.TemplateType = 1;
                        dto.CreateTime = DateTime.Now;
                        dto.ModifiedOn = dto.CreateTime;
                        model = ExpressOrderTemplate.FromDTO(dto);
                        model.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(model);
                        ContextFactory.CurrentThreadContext.SaveChanges();
                    }
                }
                if (model != null)
                {
                    result.Data = model.ToEntityData();
                }
                else
                {
                    result.Data = dto;
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "保存异常";
                LogHelper.Error("保存模板异常", ex);
            }

            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RemoveExt(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            if (dto == null || dto.Id == Guid.Empty || dto.AppId == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                var model = ExpressOrderTemplate.ObjectSet().Where(s => s.Id == dto.Id && s.AppId == dto.AppId).FirstOrDefault();
                if (model != null)
                {
                    model.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(model);

                    var items = ExpressOrderTemplateProperty.ObjectSet().Where(e => e.TemplateId == model.Id).ToList();
                    foreach (var item in items)
                    {
                        item.EntityState = EntityState.Deleted;
                        ContextFactory.CurrentThreadContext.SaveObject(item);
                    }

                    ContextFactory.CurrentThreadContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "删除失败";
                LogHelper.Error("删除模板异常", ex);
            }

            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsedExt(System.Guid appId, System.Collections.Generic.List<System.Guid> templateIdList)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            if (templateIdList == null || appId == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                var oldList = ExpressTemplateCollection.ObjectSet().Where(e => e.AppId == appId).ToList();
                foreach (var item in oldList)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }

                foreach (var id in templateIdList)
                {
                    var item = new ExpressTemplateCollection();
                    item.Id = Guid.NewGuid();
                    item.AppId = appId;
                    item.TemplateId = id;
                    item.EntityState = EntityState.Added;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }

                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "保存失败";
                LogHelper.Error("保存常用模板异常", ex);
            }

            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<System.Guid>> GetUsedExt(System.Guid appId)
        {
            ResultDTO<List<Guid>> result = new ResultDTO<List<Guid>>() { isSuccess = true };
            if (appId == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                result.Data = ExpressTemplateCollection.ObjectSet().Where(e => e.AppId == appId).Select(e => e.TemplateId).ToList();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "查询失败";
                LogHelper.Error("查询常用模板异常", ex);
            }

            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SavePropertyExt(System.Guid templateId, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressOrderTemplatePropertyDTO> propertyList)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            if (propertyList == null || templateId == Guid.Empty)
            {
                result.isSuccess = false;
                result.Message = "参数不能为空";
                return result;
            }

            try
            {
                var oldItems = ExpressOrderTemplateProperty.ObjectSet().Where(e => e.TemplateId == templateId).ToList();
                foreach (var item in oldItems)
                {
                    item.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(item);
                }

                foreach (var item in propertyList)
                {
                    item.Id = Guid.NewGuid();
                    item.TemplateId = templateId;
                    item.ModifiedOn = DateTime.Now;

                    var model = new ExpressOrderTemplateProperty().FromEntityData(item);
                    model.EntityState = EntityState.Added;
                    ContextFactory.CurrentThreadContext.SaveObject(model);
                }

                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "保存失败";
                LogHelper.Error("保存模板打印信息异常", ex);
            }

            return result;
        }
    }
}