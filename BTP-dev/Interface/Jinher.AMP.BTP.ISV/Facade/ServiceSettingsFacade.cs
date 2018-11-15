/***************
功能描述: BTPFacade
作    者: 
创建时间: 2015/8/7 13:57:06
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class ServiceSettingsFacade : BaseFacade<IServiceSettings>
    {
        /// <summary>
        /// 查询所有的服务项设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetALLServiceSettingsList(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            this.Do();
            return this.Command.GetALLServiceSettingsList(model);
        }

        /// <summary>
        /// 根据ids集合获取所有的的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetServiceSettingsList(List<Guid> ids)
        {
            this.Do();
            return this.Command.GetServiceSettingsList(ids);
        }

        /// <summary>
        /// 保存ServiceSettings信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            this.Do();
            return this.Command.SaveServiceSettings(model);
        }

        /// <summary>
        /// 根据id修改ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            this.Do();
            return this.Command.UpdateServiceSettings(model);
        }

        /// <summary>
        /// 根据id删除ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceSettings(Guid id)
        {
            this.Do();
            return this.Command.DeleteServiceSettings(id);
        }

        /// <summary>
        /// 根据AppId获取实体的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.ServiceSettingsDTO GetServiceSettings(Guid AppId)
        {
            this.Do();
            return this.Command.GetServiceSettings(AppId);
        }
    }
}
