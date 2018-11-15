
/***************
功能描述: BTPIService
作    者: 
创建时间: 2017/1/10 10:25:40
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IShiftManage : ICommand
    {
        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ShiftExchange", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO ShiftExchange(Deploy.CustomDTO.ShiftLogDTO dto);

        /// <summary>
        /// 获取交班信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetLastShiftInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.FShiftLogCDTO GetLastShiftInfo(Guid appId);
    }
}
