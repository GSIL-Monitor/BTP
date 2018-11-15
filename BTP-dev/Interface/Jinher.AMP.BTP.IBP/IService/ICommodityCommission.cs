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
    /// 商品佣金
    /// </summary>
    [ServiceContract]
    public interface ICommodityCommission : ICommand
    {
        /// <summary>
        /// 查询商品佣金信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityCommissionList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO> GetCommodityCommissionList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO search);
        /// <summary>
        /// 保存商品佣金信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommodityCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model);


        /// <summary>
        /// 修改商品佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model);

        /// <summary>
        /// 删除商品佣金信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelCommodityCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelCommodityCommission(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO model);

        /// <summary>
        /// 根据id获取商品佣金实体
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityCommission", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.CommodityCommissionDTO GetCommodityCommission(Guid id);

    }
}
