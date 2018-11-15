
/***************
功能描述: BTPIService
作    者: 
创建时间: 2015/8/27 15:43:55
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
    public interface ISpreadInfo : ICommand
    {
        /// <summary>
        /// 保存推广主信息
        /// </summary>
        /// <param name="spreadInfo">推广主信息</param>
        /// <returns>ResultDTO</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveToSpreadInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveToSpreadInfo(SpreadInfoDTO spreadInfo);

        /// <summary>
        /// 查询推广主信息
        /// </summary>
        /// <param name="spreadInfoSearchDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSpreadInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoResultDTO GetSpreadInfo(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoSearchDTO spreadInfoSearchDTO);
    }
}
