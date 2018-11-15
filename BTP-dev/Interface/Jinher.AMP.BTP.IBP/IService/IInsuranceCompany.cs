
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/11/6 11:08:25
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IInsuranceCompany : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/GetInsuranceCompany", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BTP.Deploy.CustomDTO.InsuranceCompanyDTO>> GetInsuranceCompany();
    }
}
