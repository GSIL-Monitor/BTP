
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class ServiceSettingsSV : BaseSv, IServiceSettings
    {
        /// <summary>
        /// 查询所有的服务项设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetALLServiceSettingsListExt(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> ServiceSettingslist = new List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO>();
            var searchlist = ServiceSettings.ObjectSet().AsQueryable();
            if (model.AppId!=Guid.Empty)
            {   
                //查询该Appid下面没有逻辑删除的所有服务项设置信息
                searchlist = searchlist.Where(p => p.Isdisable == false && p.AppId == model.AppId).OrderBy(p =>p.SubTime);
            }
            if (searchlist.Count()>0)
            {
                foreach (var item in searchlist.ToList())
                {
                    Jinher.AMP.BTP.Deploy.ServiceSettingsDTO entity = new Jinher.AMP.BTP.Deploy.ServiceSettingsDTO();
                    entity = CommonUtil.ReadObjectExchange(entity, item);
                    ServiceSettingslist.Add(entity);
                }
            }
            return ServiceSettingslist;
        }

        /// <summary>
        /// 根据ids集合获取所有的的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetServiceSettingsListExt(List<Guid> ids)
        {
            List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> ServiceSettingslist = new List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO>();
            var searchlist = ServiceSettings.ObjectSet().Where(p=>ids.Contains(p.Id)).AsQueryable();
            if (searchlist.Count()>0)
            {
                foreach (var item in searchlist.ToList())
                {
                    Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model = new Jinher.AMP.BTP.Deploy.ServiceSettingsDTO();
                    model = CommonUtil.ReadObjectExchange(model, item);
                    ServiceSettingslist.Add(model);
                }
            }
            return ServiceSettingslist;
        }

        /// <summary>
        /// 保存ServiceSettings信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveServiceSettingsExt(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            ResultDTO dto = null;
            ServiceSettings entity = new ServiceSettings();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                model.OrderNo =0;
                entity = CommonUtil.ReadObjectExchange(entity, model);
                entity.EntityState = EntityState.Added;
                contextSession.SaveObject(entity);
                contextSession.SaveChanges();
                dto = new ResultDTO { ResultCode = 0, Message = "保存成功", isSuccess = true };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ServiceSettings保存异常。ServiceSettings：{0}", ex.Message));
                dto = new ResultDTO { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 根据id修改ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceSettingsExt(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var servicesettings = ServiceSettings.ObjectSet().FirstOrDefault(p => p.Id == model.Id);
                if (servicesettings != null)
                {
                    if (!string.IsNullOrEmpty(model.Title))
                    {
                        servicesettings.Title = model.Title;
                    }
                    if (!string.IsNullOrEmpty(model.Content))
                    {
                        servicesettings.Content = model.Content;
                    }
                    servicesettings.OrderNo = 0;
                    servicesettings.Isdisable = model.Isdisable;
                    servicesettings.ModifiedOn = DateTime.Now;
                    servicesettings.EntityState = EntityState.Modified;
                    contextSession.SaveChanges();
                    dto = new ResultDTO() { ResultCode = 0, Message = "保存成功", isSuccess = true };
                }
                else
                {
                    dto = new ResultDTO() { ResultCode = 1, Message = "该信息不存在", isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("ServiceSettings信息保存异常。ServiceSettings：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }

        /// <summary>
        /// 根据id删除ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceSettingsExt(Guid id)
        {
            ResultDTO dto = null;
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var servicesettings = ServiceSettings.ObjectSet().FirstOrDefault(p => p.Id == id);
                if (servicesettings != null)
                {
                    servicesettings.EntityState = EntityState.Deleted;
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
                LogHelper.Error(string.Format("ServiceSettings信息保存异常。ServiceSettings：{0}", ex.Message));
                dto = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return dto;
        }


        /// <summary>
        /// 根据AppId获取实体的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.ServiceSettingsDTO GetServiceSettingsExt(Guid AppId)
        {
            Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model = new Jinher.AMP.BTP.Deploy.ServiceSettingsDTO();
            var servicesetting = ServiceSettings.ObjectSet().FirstOrDefault(p => p.Isdisable == false && p.AppId == AppId);
            if (servicesetting != null)
            {
                Jinher.AMP.BTP.Deploy.ServiceSettingsDTO entity = new Jinher.AMP.BTP.Deploy.ServiceSettingsDTO();
                model = CommonUtil.ReadObjectExchange(model, servicesetting);
            }
            return model;
        }
    }
}