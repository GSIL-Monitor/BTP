/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/24 13:19:55
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
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 商品分类接口
    /// </summary>
    [ServiceContract]
    public interface IYJEmployee : ICommand
    {
        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdataYJEmployeeInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdataYJEmployeeInfo();
        /// <summary>
        ///获取员工code
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserCodeByAcccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<YJEmployeeCodeDTO> GetUserCodeByAcccount(System.Guid AppId, System.Guid UserId, string UserAccount);
    }
}