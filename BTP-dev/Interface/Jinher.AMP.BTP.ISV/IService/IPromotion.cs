
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/21 14:24:22
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
    /// 促销接口
    /// </summary>
    [ServiceContract]
    public interface IPromotion : ICommand
    {
        /// <summary>
        /// 获取最新促销
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.PromotionSV.svc/GetNewPromotion
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetNewPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        PromotionHotSDTO GetNewPromotion(Guid appId);


        /// <summary>
        /// 易捷点滴接口
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.PromotionSV.svc/GetYJDianDi
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYJDianDi", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetYJDianDi(Guid appId);

        /// <summary>
        /// 获取最新促销商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.PromotionSV.svc/GetPromotionItems
        /// </para>
        /// </summary>
        /// <param name="promotionId">促销ID</param>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetPromotionItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetPromotionItems(Guid promotionId, Guid appId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取所有商品折扣信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllPromotionItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("已过时,刷新每日优惠请参见方法GetAppPromotions", false)]
        List<PromotionItemShortCDTO> GetAllPromotionItems();

        /// <summary>
        /// 获取当日商品促销信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppPromotions", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Dictionary<Guid, List<TodayPromotionDTO>> GetAppPromotions();

        [WebInvoke(Method = "POST", UriTemplate = "/AddHotCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AddHotCommodity();

        /// <summary>
        /// 获取所有门店信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllStores", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.StoreCacheDTO> GetAllStores();

        /// <summary>
        /// 获取所有商品属性
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommAttributes", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttributeCacheDTO> GetAllCommAttributes();

        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCateGories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryCacheDTO> GetAllCateGories();

        /// <summary>
        /// 推送促销信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/PromotionPush", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void PromotionPush();

        /// <summary>
        /// 修改评价表用户信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateUserInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void UpdateUserInfo();

        [OperationContract]
        void PromotionPushIUS();

        /// <summary>
        /// 查询商品即将开始的秒杀活动
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSecKillPromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        PromotionItemShortCDTO GetSecKillPromotion(Guid commodityId);

        /// <summary>
        /// 判断是否可以购买 商品活动进行中，或者没有即将开始的秒杀活动可以购买
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckSecKillBuy", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool CheckSecKillBuy(Guid commodityId);

        /// <summary>
        /// 添加折扣
        /// </summary>
        /// <param name="discountsDTO">折扣属性</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddOutsidePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddOutsidePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO);

        /// <summary>
        /// 删除折扣
        /// </summary>
        /// <param name="outsideId">外部活动id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DelOutsidePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelOutsidePromotion(Guid outsideId);

        /// <summary>
        /// 修改折扣(活动开始后不允许修改)
        /// </summary>
        /// <param name="discountsDTO">折扣属性</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOutsidePromotion", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateOutsidePromotion(Jinher.AMP.BTP.Deploy.CustomDTO.PromotionOutSideVM discountsDTO);

        /// <summary>
        /// 设置外部活动订单不支付过期时间
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetExpireSeconds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetExpireSeconds(Guid appId, long seconds);

        /// <summary>
        /// 数据库中商品活动信息与Redis中保存商品活动信息同步
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/CommodityDataAndRedisDataSynchronization", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CommodityDataAndRedisDataSynchronization();
        /// <summary>
        ///根据活动ID数据库中商品活动信息与Redis中保存商品活动信息同步
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/PromotionRedis", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO PromotionRedis(Guid guid);

        /// <summary>
        /// 获取当日商品促销信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GeTodayPromotions", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<TodayPromotionDTO> GeTodayPromotions(List<Guid?> outsideId);

        /// <summary>
        /// 获取当日商品促销购买数量
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSurplusLimitBuyTotal", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<PromotionSurplusLimitBuyTotalDto> GetSurplusLimitBuyTotal(List<Guid> outsideId);

        /// <summary>
        /// 获取商城类目
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategoryDTO> GetCategoryList(System.Guid AppId);

          /// <summary>
        /// 获取应用的一级商品分类
        /// <para>
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryL1", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategoryL1(Guid appId);
    }
}
