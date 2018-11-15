
/***************
功能描述: BTPIService
作    者: 
创建时间: 2017/2/15 13:40:59
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IExpressOrderTemplate : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressOrderTemplate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderPrintTemplate> GetExpressOrderTemplate(Guid appId);

        [WebInvoke(Method = "POST", UriTemplate = "/GetExpressOrderTemplateByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.ExpressTemplateDTO> GetExpressOrderTemplateByAppId(Guid appId);

        [WebInvoke(Method = "POST", UriTemplate = "/Save", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ExpressOrderTemplateDTO> Save(ExpressOrderTemplateDTO dto);

        [WebInvoke(Method = "POST", UriTemplate = "/Remove", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(ExpressOrderTemplateDTO dto);

        [WebInvoke(Method = "POST", UriTemplate = "/SaveUsed", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(Guid appId,List<Guid> templateIdList);

        [WebInvoke(Method = "POST", UriTemplate = "/GetUsed", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Guid>> GetUsed(Guid appId);

        [WebInvoke(Method = "POST", UriTemplate = "/SaveProperty", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveProperty(Guid templateId, List<ExpressOrderTemplatePropertyDTO> propertyList);

    }
}
