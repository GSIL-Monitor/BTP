
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/14 14:54:40
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
    public interface ICategoryAdvertise : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/CreateCategoryAdvertise", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CreateCategoryAdvertise(CategoryAdvertiseDTO entity);

        [WebInvoke(Method = "POST", UriTemplate = "/CateGoryAdvertiseList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<IList<CategoryAdvertiseDTO>> CateGoryAdvertiseList(String advertiseName, int state, Guid CategoryId,int pageIndex, int pageSize,out int rowCount);

        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCategoryAdvertise", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCategoryAdvertise(Guid advertiseId);

        [WebInvoke(Method = "POST", UriTemplate = "/EditCategoryAdvertise", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO EditCategoryAdvertise(CategoryAdvertiseDTO entity);

        [WebInvoke(Method = "POST", UriTemplate = "/EditCategoryAdvertise", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CategoryAdvertiseDTO> GetCategoryAdvertise(Guid advertiseId);
    }
}
