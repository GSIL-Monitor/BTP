
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
    public interface IExpress : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/GetSystem", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<ExpressCodeDTO>> GetSystem();

        [WebInvoke(Method = "POST", UriTemplate = "/GetAll", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<ExpressCodeDTO>> GetAll(Guid appId);

        [WebInvoke(Method = "POST", UriTemplate = "/Save", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ExpressCodeDTO> Save(ExpressCodeDTO dto);

        [WebInvoke(Method = "POST", UriTemplate = "/Remove", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Remove(ExpressCodeDTO dto);

        [WebInvoke(Method = "POST", UriTemplate = "/SaveUsed", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUsed(Guid appId,List<string> expressCodeList);

        [WebInvoke(Method = "POST", UriTemplate = "/GetUsed", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<string>> GetUsed(Guid appId);

    }
}
