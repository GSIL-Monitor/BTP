
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/15 16:34:58
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

namespace Jinher.AMP.BTP.ISV.IService
{
    [ServiceContract]
    public interface ICategoryAdvertise : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/getBrandWallSpecialByCateID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CategoryAdvertiseDTO> getBrandWallSpecialByCateID(Guid CategoryID);
    }
}
