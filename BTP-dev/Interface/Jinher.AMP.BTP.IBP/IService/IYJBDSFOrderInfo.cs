
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/7/29 12:36:50
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

    [ServiceContract]
    public interface IYJBDSFOrderInfo : ICommand
    {
        /// <summary>
        /// 根据订单号获取订单数据
        /// </summary>
        /// <param name="OrderNos"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDSFOrderInfoByOrderNos", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>> GetDSFOrderInfoByOrderNos(List<string> OrderNos);

        /// <summary>
        /// 获取保险返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCarInsuranceRebate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg);

        /// <summary>
        /// 根据汇款单号获取返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCarRebateByRemittanceNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>> GetCarRebateByRemittanceNo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg);

        /// <summary>
        /// 查询统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCarInsuranceReport", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg);

        /// <summary>
        /// 更新返利状态
        /// </summary>
        /// <param name="OrderNO"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCarRebateState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCarRebateState(string OrderNO,int State);

    }
}
