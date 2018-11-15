
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017-09-06 15:08:23
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ExpressOrderTemplateBP : BaseBP, IExpressOrderTemplate
    {

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderPrintTemplate> GetExpressOrderTemplate(System.Guid appId)
        {
            base.Do();
            return this.GetExpressOrderTemplateExt(appId);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExpressTemplateDTO> GetExpressOrderTemplateByAppId(System.Guid appId)
        {
            base.Do();
            return this.GetExpressOrderTemplateByAppIdExt(appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO> Save(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            base.Do();
            return this.SaveExt(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            base.Do();
            return this.RemoveExt(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(System.Guid appId, System.Collections.Generic.List<System.Guid> templateIdList)
        {
            base.Do();
            return this.SaveUsedExt(appId, templateIdList);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<System.Guid>> GetUsed(System.Guid appId)
        {
            base.Do();
            return this.GetUsedExt(appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveProperty(System.Guid templateId, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressOrderTemplatePropertyDTO> propertyList)
        {
            base.Do();
            return this.SavePropertyExt(templateId, propertyList);
        }
    }
}