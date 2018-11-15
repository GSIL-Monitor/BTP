using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// App自提点
    /// </summary>
    [ServiceContract]
    public interface IAppSelfTakeStation : ICommand
    {
        /// <summary>
        /// 下订单页获取自提点信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppSelfTakeStationDefault", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationDefaultInfoDTO GetAppSelfTakeStationDefault(AppSelfTakeStationSearchDTO search);
    }
}
