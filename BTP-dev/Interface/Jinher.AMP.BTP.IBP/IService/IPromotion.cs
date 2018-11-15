
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/19 13:38:51
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
    public interface IPromotion : ICommand
    {
        /// <summary>
        /// 添加折扣
        /// </summary>
        /// <param name="discountsDTO">自定义折扣属性</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddPromotion(DiscountsVM discountsDTO);

        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="id">促销ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DelPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelPromotion(Guid id);

        /// <summary>
        /// 修改折扣
        /// </summary>
        /// <param name="discountsDTO">自定义属性</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdatePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdatePromotion(DiscountsVM discountsDTO);

        /// <summary>
        /// 查询所有折扣
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<DiscountsVM> GetAllPromotion(Guid sellerID, int pageSize, int pageIndex, string startTime, string endTime, string sintensity, string eintensity, string commodityName, string state, out int rowCount);

        /// <summary>
        /// 根据促销ID查询促销商品
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPromotionItemsByPromotionID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PromotionItemsVM> GetPromotionItemsByPromotionID(Guid promotionID, Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO search, int pageSize, int pageIndex, out int rownum);

        /// <summary>
        /// 根据促销ID查询促销详情
        /// </summary>
        /// <param name="promotionID">促销ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPromotionByPromotionID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        DiscountsVM GetPromotionByPromotionID(Guid promotionID);

        /// <summary>
        /// 查询所有能打折所有商品ID
        /// </summary>
        /// <param name="appID">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Guid> GetCommodityID(Guid appID, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 查询所有折扣商品的编号
        /// </summary>
        /// <param name="appid">appID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityCodeByPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetCommodityCodeByPromotion(Guid appid, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 得到促销下所有的促销商品
        /// </summary>
        /// <param name="promotionId">促销ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPromotionItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PromotionItemsDTO> GetAllPromotionItems(Guid promotionId);
        /// <summary>
        /// 删除促销商品
        /// </summary>
        /// <param name="promotionId"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/DeletePromotionItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void DeletePromotionItems(Guid promotionId);

        /// <summary>
        /// 判断同一时期不能超过5个促销
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsAddPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool IsAddPromotion(DateTime starTime, DateTime endTime, Guid appId);

        /// <summary>
        /// 同一促销时期可以添加的商品
        /// </summary>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsCommodityCan", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Guid> IsCommodityCan(System.DateTime starTime, System.DateTime endTime, Guid appId);

        /// <summary>
        /// 根据促销商品id获得自己所有的促销商品信息
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByPromotionID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityDTO> GetCommodityByPromotionID(Guid promotionID);


        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityNum", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetAllCommodityNum(System.DateTime starTime, System.DateTime endTime, Guid appId, Guid? promotionid);

        List<string> GetCodes(List<string> commCodes, Guid appId);

        List<Guid> GetCommodityIds(Guid promotionId);

        /// <summary>
        /// 获取促销商品数量
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        int GetCommodityNum(Guid promotionId);
    }
}
