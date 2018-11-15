using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;


namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 发票相关
    /// </summary>
    [ServiceContract]
    public interface IInvoice : ICommand
    {
        /// <summary>
        /// 查询发票信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceInfoList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceResultDTO> GetInvoiceInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search);
        /// <summary>
        /// 保存增值税发票资质信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveVatInvoiceProof", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveVatInvoiceProof(Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO VatInvoiceP);
        /// <summary>
        /// 设置发票类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetInvoiceCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInvoiceCategory(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO model);
        /// <summary>
        /// 修改发票
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateInvoice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateInvoice(List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceUpdateDTO> list);
        /// <summary>
        /// 获取全局设置的发票类型
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO> GetInvoiceCategory(Guid appId);
        /// <summary>
        /// 显示增值税发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ShowVatInvoiceProof", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProof(Guid userId);

        /// <summary>
        /// 显示增值税发票信息 金采支付专用
        /// </summary>
        /// <param name="jcActivityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ShowVatInvoiceProofII", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProofII(Guid jcActivityId);


        /// <summary>
        /// 获取导出的Excel数据
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceExport", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<InvoiceExportDTO> GetInvoiceExport(InvoiceExportDTO search);

        /// <summary>
        /// 获取导出的电子发票的详细数据
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetInvoiceExportDetail", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ElectronicInvoiceDTO> GetInvoiceExportDetail(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search);
    }
}
