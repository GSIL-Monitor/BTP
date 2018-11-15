
/***************
功能描述: BTPIService
作    者: LSH
创建时间: 2014/3/19 11:25:47
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
    /// 商品为易捷币提供接口
    /// </summary>
    [ServiceContract]
    public interface ICommodityForYJB : ICommand
    {
        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="input">参数dto</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodities", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<CommodityListOutPut>> GetCommodities(CommoditySearchInput input);

        /// <summary>
        /// 根据馆下所有商品
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="commodityCategory">栏目名称</param>
        /// <param name="commodityName">商品名称</param>
        /// <param name="pageIndex">第几页（从1开始）</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodities", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommoditySearchResultDTO GetAllCommodities(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize);
        /// <summary>
        /// 根据店铺名称获取商品列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="appId">店铺id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppIdCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<ComAttrDTO>> GetAppIdCommodity(string name, Guid appId, decimal price, int pageIndex, int pageSize);
        /// <summary>
        /// 根据id显示商品信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityById", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<ComAttrDTO>> GetCommodityById(Guid appid, List<System.Guid> ids, int pageIndex, int pageSize);
        /// <summary>
        /// 根据id获取商品信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByIds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ComAttrDTO> GetCommodityByIds(Guid appid, List<System.Guid> ids);
        /// <summary>
        /// 定时修改商品价格
        /// </summary>
        /// <param name="CkPriceList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodityPrice(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo);
        /// <summary>
        /// 审核通过后撤销.编辑,恢复已变更的数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RecoverCommodityPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RecoverCommodityPrice(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo);
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMallApplyInfoList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<MallApplyDTO> GetMallApplyInfoList(Guid EsappId);
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSupplierInfoList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SupplierDTO> GetSupplierInfoList(Guid appId);
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMallApplyList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<MallApplyDTO> GetMallApplyList();
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSupplierList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SupplierDTO> GetSupplierList();
        /// <summary>
        ///  查询App入驻信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMallApplyByIds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<MallApplyDTO> GetMallApplyByIds(Guid esAppId, List<Guid> appIds);
        /// <summary>
        /// 获取所有的严选appId
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYXappIds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Guid> GetYXappIds();
        /// <summary>
        ///导出定时改价未改变价格的订单信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderItemList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderItemDTO> GetOrderItemList(string StarTime,string EndTime);
        /// <summary>
        ///导出定时改价未改变价格的订单信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderInfoByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<OrderItemDTO> GetOrderInfoByAppId(Guid AppId, string StarTime, string EndTime);
    }
}
