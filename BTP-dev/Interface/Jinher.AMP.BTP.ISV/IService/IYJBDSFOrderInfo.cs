
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/7/28 15:15:05
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.IService
{

    [ServiceContract]
    public interface IYJBDSFOrderInfo : ICommand
    {
        /// <summary>
        /// 获取第三方订单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDSFOrderInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<YJBDSFOrderInfoDTO> GetDSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInfoSearchDTO arg);
        /// <summary>
        /// 插入第三方订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/InsertTODSFOrderInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTODSFOrderInfo(BTP.Deploy.CustomDTO.YJBDSFOrderInformationDTO model);
        
        /// <summary>
        /// 获取汇款单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCarInsuranceRebate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg);
        /// <summary>
        /// 插入汇款单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/InsertTOCarInsuranceRebate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceRebate(BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDTO model);

        /// <summary>
        /// 获取保险统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCarInsuranceReport", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<BTP.Deploy.YJBCarInsuranceReportDTO> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg);
        /// <summary>
        /// 插入保险统计报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/InsertTOCarInsuranceReport", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceReport(BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO model);

    }
}
