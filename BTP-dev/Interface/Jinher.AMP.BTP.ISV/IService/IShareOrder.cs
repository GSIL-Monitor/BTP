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
    /// <summary>
    /// 众销分成
    /// </summary>
    [ServiceContract]
    public interface IShareOrder : ICommand
    {
        /// <summary>
        /// 获取众销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShareOrderMoneySumInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumDTO GetShareOrderMoneySumInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySumSearchDTO search);

        /// <summary>
        /// 获取众销入账信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShareOrderMoneyInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneyResultDTO GetShareOrderMoneyInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ShareOrderMoneySearchDTO search);

    }
}
