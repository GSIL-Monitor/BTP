
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/7/9 18:21:04
***************/
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using System.Collections.Generic;
using Jinher.AMP.BTP.Deploy.CustomDTO.Commodity;


namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface ISNCommodity : ICommand
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AddSNCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddSNCommodity(JdCommodityDTO input);
        /// <summary>
        /// 导入苏宁商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ImportSNCommodityData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> ImportSNCommodityData(List<JdCommodityDTO> JdComList, Guid AppId);
        /// <summary>
        /// 自动同步苏宁商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoSyncSNCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> AutoSyncSNCommodityInfo(Guid AppId, List<Guid> Ids);
        /// <summary>
        ///同步商品列表图片
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateComPic", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateComPic();
        /// <summary>
        /// 导出苏宁进货价差异商品列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSNDiffCostPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SNComCostDiffDTO> GetSNDiffCostPrice();
        /// <summary>
        ///全量同步苏宁进货价
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SynSNCostPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SynSNCostPrice();
        /// <summary>
        ///苏宁对账1
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SynSNBill", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SynSNBill();
        /// <summary>
        ///苏宁对账2
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SynSNBill2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SynSNBill2();
    }
}
