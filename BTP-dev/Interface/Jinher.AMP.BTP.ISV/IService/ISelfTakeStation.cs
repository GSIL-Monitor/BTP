
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/19 11:25:47
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
    /// <summary>
    /// 自提点
    /// </summary>
    [ServiceContract]
    public interface ISelfTakeStation : ICommand
    {
        /// <summary>
        /// 查询自提点
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSelfTakeStation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        SelfTakeStationResultDTO GetSelfTakeStation(SelfTakeStationSearchDTO search);

        /// <summary>
        /// 删除总代关联删除
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCityOwner", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCityOwner(SelfTakeStationSearchDTO search);
    }
}
