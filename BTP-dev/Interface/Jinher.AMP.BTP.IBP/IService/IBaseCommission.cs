using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO.MallApply;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{    
     /// <summary>
    /// 基础佣金
    /// </summary>
    [ServiceContract]
    public interface IBaseCommission : ICommand
    {
        /// <summary>
        /// 查询佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBaseCommissionList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO> GetBaseCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO search);
        /// <summary>
        /// 保存佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveBaseCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model);


        /// <summary>
        /// 修改佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateBaseCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model);

        /// <summary>
        /// 删除佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelBaseCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelBaseCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO model);

        /// <summary>
        /// 根据id获取基础佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBaseCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.BaseCommissionDTO GetBaseCommission(Guid id, Guid mallApplyId);


    }
}
