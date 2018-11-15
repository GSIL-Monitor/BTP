
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/18 15:06:51
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
    /// <summary>
    /// 结算操作接口
    /// </summary>
    [ServiceContract]
    public interface ISettlingAccount : ICommand
    {
        /// <summary>
        /// 获取当前商品结算列表
        /// </summary>
        /// <param name="search">商品结算价检索类</param>
        /// <param name="rowCount">记录数</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetNowSettlingAccount", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.SettlingAccountVM> GetNowSettlingAccount(SettlingAccountSearchDTO search, out int rowCount);

        /// <summary>
        /// 添加商品的厂家结算价
        /// </summary>
        /// <param name="settlingAccountDTO">结算价实体</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSettlingAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveSettlingAccount(SettlingAccountDTO settlingAccountDTO);

        /// <summary>
        /// 获取当前商品厂家结算价设置的历史列表
        /// </summary>
        /// <param name="search">商品结算价修改历史检索类</param>
        /// <param name="rowCount">记录数</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetHistorySettlingAccount", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.SettlingAccountDTO> GetHistorySettlingAccount(SettlingAccountHistorySearchDTO search, out int rowCount);

        /// <summary>
        /// 删除厂家结算记录
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteSettlingAccountById", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteSettlingAccountById(List<Guid> ids);
    }
}
