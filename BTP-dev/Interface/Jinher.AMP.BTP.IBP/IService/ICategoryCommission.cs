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
    /// 类别佣金
    /// </summary>
    [ServiceContract]
    public interface ICategoryCommission : ICommand
    {
        /// <summary>
        /// 查询类别佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryCommissionList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO> GetCategoryCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO search);
        /// <summary>
        /// 保存类别佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCategoryCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model);


        /// <summary>
        /// 修改类别佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCategoryCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model);

        /// <summary>
        /// 删除类别佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelCategoryCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelCategoryCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO model);


        /// <summary>
        /// 根据id获取类别佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CategoryCommissionDTO GetCategoryCommission(Guid id);
    }
}
