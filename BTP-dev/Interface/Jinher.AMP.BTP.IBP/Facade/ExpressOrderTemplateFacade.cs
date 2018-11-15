
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017-09-06 15:08:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class ExpressOrderTemplateFacade : BaseFacade<IExpressOrderTemplate>
    {

        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderPrintTemplate> GetExpressOrderTemplate(System.Guid appId)
        {
            base.Do();
            return this.Command.GetExpressOrderTemplate(appId);
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ExpressTemplateDTO> GetExpressOrderTemplateByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetExpressOrderTemplateByAppId(appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO> Save(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            base.Do();
            return this.Command.Save(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(Jinher.AMP.BTP.Deploy.ExpressOrderTemplateDTO dto)
        {
            base.Do();
            return this.Command.Remove(dto);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(System.Guid appId, System.Collections.Generic.List<System.Guid> templateIdList)
        {
            base.Do();
            return this.Command.SaveUsed(appId, templateIdList);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<System.Guid>> GetUsed(System.Guid appId)
        {
            base.Do();
            return this.Command.GetUsed(appId);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveProperty(System.Guid templateId, System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.ExpressOrderTemplatePropertyDTO> propertyList)
        {
            base.Do();
            return this.Command.SaveProperty(templateId, propertyList);
        }
    }
}