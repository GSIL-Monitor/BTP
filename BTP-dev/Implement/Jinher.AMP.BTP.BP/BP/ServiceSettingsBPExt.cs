
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
    public partial class ServiceSettingsBP : BaseBP, IServiceSettings
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
                searchlist = searchlist.Where(p=>p.Isdisable==false&&p.AppId==model.AppId);
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
                    servicesettings.Isdisable = model.Isdisable;
                    servicesettings.ModifiedOn = DateTime.Now;
                    servicesettings.EntityState = EntityState.Modified;
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
            Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model =new Jinher.AMP.BTP.Deploy.ServiceSettingsDTO();
            var servicesetting = ServiceSettings.ObjectSet().FirstOrDefault(p=>p.Isdisable==false&&p.AppId==AppId);
            if (servicesetting!=null)
            {
                Jinher.AMP.BTP.Deploy.ServiceSettingsDTO entity = new Jinher.AMP.BTP.Deploy.ServiceSettingsDTO();
                model = CommonUtil.ReadObjectExchange(model, servicesetting);
            }
            return model;
        }
    }
}