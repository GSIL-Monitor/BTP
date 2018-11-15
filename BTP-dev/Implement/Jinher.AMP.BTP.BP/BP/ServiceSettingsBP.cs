
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2017/9/21 15:02:29
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ServiceSettingsBP : BaseBP, IServiceSettings
    {
        /// <summary>
        /// 查询所有的服务项设置信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetALLServiceSettingsList(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            this.Do();
            return this.GetALLServiceSettingsListExt(model);
        }

        /// <summary>
        /// 根据ids集合获取所有的的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.ServiceSettingsDTO> GetServiceSettingsList(List<Guid> ids)
        {
            this.Do();
            return this.GetServiceSettingsListExt(ids);
        }

        /// <summary>
        /// 保存ServiceSettings信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            this.Do();
            return this.SaveServiceSettingsExt(model);
        }

        /// <summary>
        /// 根据id修改ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateServiceSettings(Jinher.AMP.BTP.Deploy.ServiceSettingsDTO model)
        {
            this.Do();
            return this.UpdateServiceSettingsExt(model);
        }

        /// <summary>
        /// 根据id删除ServiceSettings设置信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteServiceSettings(Guid id)
        {
            this.Do();
            return this.DeleteServiceSettingsExt(id);
        }


        /// <summary>
        /// 根据AppId获取实体的内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.ServiceSettingsDTO GetServiceSettings(Guid AppId)
        {
            this.Do();
            return this.GetServiceSettingsExt(AppId);
        }

    }
}