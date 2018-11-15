
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/4/3 11:48:54
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
    /// 发票
    /// </summary>
    [ServiceContract]
    public interface IInvoice : ICommand
    {
        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="search">发票设置必传参数，AppIds，UserId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<InvoiceSettingDTO> GetInvoiceSetting(InvoiceSearchDTO search);

        /// <summary>
        /// 获取发票历史数据
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="category">发票类型 1:增值税专用发票,2:电子发票,4:增值税专用发票</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceInfoList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<List<InvoiceInfoDTO>> GetInvoiceInfoList(Guid appId, Guid userId, int category);
    }
}
