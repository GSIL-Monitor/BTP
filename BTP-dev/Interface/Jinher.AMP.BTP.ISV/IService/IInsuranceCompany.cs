
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/11/5 19:48:08
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{

    [ServiceContract]
    public interface IInsuranceCompany : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/GetInsuranceCompany", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<InsuranceCompanyDTO>> GetInsuranceCompany();
    }
}
