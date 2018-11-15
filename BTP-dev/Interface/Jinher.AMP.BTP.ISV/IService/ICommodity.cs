
/***************
功能描述: BTPIService
作    者: 
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
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;
using Jinher.AMP.BTP.Deploy.CustomDTO.JD;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 商品接口
    /// </summary>
    [ServiceContract]
    public interface ICommodity : ICommand
    {
        /// <summary>
        /// 获取商品列表  排序
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetOrByCommodity
        /// </para>
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        /// <param name="appId">appid</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="fieldSort">排序字段，枚举有对应的值</param>
        /// <param name="order">0为降序，1为升序</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrByCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetOrByCommodity(System.Guid categoryId, Guid appId, int pageIndex, int pageSize, int fieldSort, int order, string areaCode);


        /// <summary>
        /// 获取商品列表    
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityByWhere
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByWhere", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByWhere(Jinher.AMP.ZPH.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, int pageIndex, int pageSize, string areaCode);




        /// <summary>
        /// 获取商品列表    
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodity
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodity(Guid appId, int pageIndex, int pageSize, string areaCode);

        /// <summary>
        /// 获取商品列表    
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodity2
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <param name="isChkTime"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodity2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodity2(Guid appId, int pageIndex, int pageSize, string areaCode, int isChkTime, DateTime beginTime, DateTime endTime);

        /// <summary>
        /// 获取商品列表    
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodity3
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="areaCode"></param>
        /// <param name="isChkTime"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodity3", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ListResult<CommodityListCDTO> GetCommodity3(CommodityListInputDTO input);

        /// <summary>
        /// 根据搜索条件获取商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetWantCommodity
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="want">搜索关键字</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetWantCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetWantCommodity(string want, Guid appId, int pageIndex, int pageSize);

        /// <summary>
        /// 根据分类获取商品
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityByCategory
        /// </para>
        /// </summary>
        /// <param name="categoryId">分类ID</param>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByCategory(Guid categoryId, Guid appId, int pageIndex, int pageSize);

        /// <summary>
        /// 商品详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityDetails
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetails", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommoditySDTO GetCommodityDetails(Guid commodityId, Guid appId, Guid userId, string freightTo = "");

        /// <summary>
        /// 商品详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityDetailsNew
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetailsNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommoditySDTO GetCommodityDetailsNew(Guid commodityId, Guid appId, Guid userId, string freightTo = "");

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="commodityCategory"></param>
        /// <param name="commodityName"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CommoditySearch", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommoditySearchResultDTO CommoditySearch(Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize);
        /// <summary>
        /// 从多个App获取商品：按照商品名称模糊查询商品
        /// </summary>
        /// <param name="appIds"></param>
        /// <param name="commodityCategory"></param>
        /// <param name="commodityName"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CommoditySearchFromApps", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommoditySearchForAppsResultDTO CommoditySearchFromApps(List<Guid> appIds, string commodityCategory, string commodityName, int pageIndex, int pageSize);
        /// <summary>
        /// 按照商品名称模糊查询商品
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="commodityCategory"></param>
        /// <param name="commodityName"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditySearch", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        GetCommoditySearchResultDTO GetCommoditySearch(Guid appId, string commodityName, int pageIndex, int pageSize);


        /// <summary>
        /// 商品详情
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetCommodityInfo
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityInfoListDTO GetCommodityInfo(Guid commodityId);

        /// <summary>
        /// 生成用户获奖记录
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/CreateUserPrizeRecord
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        //[WebInvoke(Method = "POST", UriTemplate = "/CreateUserPrizeRecord", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CreateUserPrizeRecord(UserPrizeRecordDTO userPrizeRecordDTO);

        /// <summary>
        /// 获取用户获奖记录
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CommoditySV.svc/GetUserPrizeRecord
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserPrizeRecord", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        PrizeRecordDTO GetUserPrizeRecord(Guid promotionId, Guid commodityId, Guid userId);

        /// <summary>
        /// 多应用：根据商品Ids获取商品信息列表
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByIds", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByIds(List<Guid> commodityIds, bool isDefaultOrder);

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CommodityIdsList">商品list</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CheckCommodityDTO> CheckCommodity(Guid UserID, List<Guid> CommodityIdsList);

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CommodityIdsList">商品list</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodityNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CheckCommodityDTO> CheckCommodityNew(Guid UserID, List<CommodityIdAndStockId> CommodityIdsList);

        /// <summary>
        /// 广场热门商品
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetHotCommoditis", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SquareHotCommodityDTO> GetHotCommoditis();
        /// <summary>
        /// 运费计算
        /// </summary>
        /// <param name="FreightTo">运送到</param>
        /// <param name="TemplateCounts">模板数据集合</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalFreight", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        FreightResultDTO CalFreight(string FreightTo, List<TemplateCountDTO> TemplateCounts);
        /// <summary>
        /// 运费详细列表
        /// </summary>
        /// <param name="CommodityId">商品ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFreightDetails", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        FreightDetailDTO GetFreightDetails(Guid CommodityId);

        /// <summary>
        /// 商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditysZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityZPHResultDTO GetCommoditysZPH(CommoditySearchZPHDTO search);

        /// <summary>
        /// ZPH商品服务项商品查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditysforZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityZPHResultDTO GetCommoditysforZPH(CommoditySearchZPHDTO search);
        /// <summary>
        /// ZPH关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/JoinComdtyServiceSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO JoinComdtyServiceSetting(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId);
        /// <summary>
        /// ZPH取消关联服务商品
        /// </summary>
        /// <param name="ComIds"></param>
        /// <param name="AppId"></param>
        /// <param name="ServiceSettingId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CancelComdtyServiceSetting", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CancelComdtyServiceSetting(List<Guid> ComIds, Guid AppId, Guid ServiceSettingId);

        /// <summary>
        /// 根据正品会活动id获取对应分页信息的商品列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByZPHActId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByZPHActId(CommoditySearchZPHDTO search);

        /// <summary>
        /// 获取商品列表       
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityListResultDTO GetCommodityList(CommodityListSearchDTO search);


        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetailsZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityDetailsZPH(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId, string areaCode);

        /// <summary>
        /// 商品详情新(支持单属性SKU)
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetailsZPHNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNew(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId, string areaCode);

        /// <summary>
        /// 商品详情新(支持单属性SKU 获取活动sku集合)
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="outPromotionId">正品会活动Id</param>
        /// <param name="actId"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetailsZPHNewSku", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSku(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? outPromotionId, System.Guid actId, string areaCode);
        /// <summary>
        /// 商品详情新(支持单属性SKU 获取活动sku集合)
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <param name="appId">appId（此参数已改变语义，实际传递esAppId）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <param name="jcActivityId">金采团购活动Id</param>
        /// <param name="actId"></param>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetailsZPHNewSkuII", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityDetailsZPHNewSkuII(System.Guid commodityId, System.Guid appId, Guid userId, string freightTo, Guid? jcActivityId, System.Guid actId, string areaCode);

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="actId">正品会活动Id</param>
        /// <param name="appId">appId（此参数无效）</param>
        /// <param name="userId">用户ID</param>
        /// <param name="freightTo">目的地</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetailsByActId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityDetailsByActId(System.Guid actId, System.Guid appId, Guid userId, string freightTo, string areaCode);

        /// <summary>
        /// 活动预约
        /// </summary>
        /// <param name="outPromotionId">外部活动Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="esAppId">预约app</param>
        /// <param name="commodityId"></param>
        /// <param name="commodityStockId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveMyPresellComdtyZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveMyPresellComdtyZPH(Guid outPromotionId, Guid userId, string verifyCode, Guid esAppId, Guid commodityId, Guid commodityStockId);

        [WebInvoke(Method = "POST", UriTemplate = "/GetVerifyCodeZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<byte[]> GetVerifyCodeZPH();
        /// <summary>
        /// 添加和取消到货消息,短信提醒
        /// </summary>       
        [WebInvoke(Method = "POST", UriTemplate = "/SaveStockNotificationsZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<NotificationsDTO> SaveStockNotificationsZPH(Guid commodityId, Guid userId, Guid esAppId, int noticeType, bool Iscancel);

        [WebInvoke(Method = "POST", UriTemplate = "/SendNotificationsZPH", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SendNotificationsZPH(Guid commodityId, Guid outPromotionId, Guid esAppId);

        /// <summary>
        /// 校验商品信息 
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="commodityIdsList">商品list</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodityWithPreSell", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("已废弃，请调用CheckCommodity", false)]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CheckCommodityDTO> CheckCommodityWithPreSell(Guid userId, List<CommodityIdAndStockId> commodityIdsList);

        /// <summary>
        /// 清空商品信息的缓存 
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveCache")]
        [OperationContract]
        void RemoveCache();

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityBySellerID", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityDTO> GetAllCommodityBySellerID(CommoditySearchDTO commoditySearch);

        /// <summary>
        /// 查询某个APP下的商品
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <param name="count"></param>
        /// <returns>查询结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityForCoupon", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityDTO> GetAllCommodityForCoupon(CommoditySearchDTO commoditySearch, out int count);

        /// <summary>
        /// 多app运费计算
        /// </summary>
        /// <param name="freightTo">运送到</param>
        /// <param name="isSelfTake">是否自提</param>
        /// <param name="templateCounts">模板数据集合</param>
        /// <returns>运费计算结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalFreightMultiApps", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalFreightMultiApps(string freightTo, int isSelfTake, List<Jinher.AMP.BTP.Deploy.CustomDTO.TemplateCountDTO> templateCounts, Dictionary<Guid, decimal> coupons, Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO yjbInfo, List<Guid> yjCouponIds);

        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByIdsWithPreSell", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByIdsWithPreSell(List<Guid> commodityIds, bool isDefaultOrder);

        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByIdsWithPreSellInBeLongTo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongTo(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder);

        /// <summary>
        /// 根据商品id获取商品信息（包含预售逻辑，预售阶段商品显示预售活动价）
        /// </summary>
        /// <param name="commodityIds"></param>
        /// <param name="isDefaultOrder"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByIdsWithPreSellInBeLongToWithType", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByIdsWithPreSellInBeLongToWithType(Guid beLongTo, List<Guid> commodityIds, bool isDefaultOrder, int mallAppType);

        /// <summary>
        /// 查询AppID列表下的所有上架的商品
        /// </summary>
        /// <param name="appListSearch">查询类</param>
        /// <returns>查询结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityByAppIdList", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityByAppIdList(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchByAppIdListDTO appListSearch);

        /// <summary>
        /// 根据商品ID列表获取商品是否支持自提的信息
        /// </summary>
        /// <param name="commodityIdList">商品ID列表</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityIsEnableSelfTakeList", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySelfTakeListDTO> GetCommodityIsEnableSelfTakeList(List<Guid> commodityIdList);

        /// <summary>
        /// 获取商品属性
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CommodityAttrStocks", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ComAttStockDTO> CommodityAttrStocks(CommoditySearchDTO search);

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveServiceCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO);
        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="serviceCommodityAndCategoryDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateServiceCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateServiceCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.ServiceCommodityAndCategoryDTO serviceCommodityAndCategoryDTO);
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteServiceCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteServiceCommodity(Guid id);
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteServiceCommoditys", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteServiceCommoditys(List<Guid> ids);

        /// <summary>
        /// 获取点餐列表数据       
        /// </summary>
        /// <param name="search">查询条件model：必传参数AppId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCateringCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CateringCommodityDTO> GetCateringCommodity(CommodityListSearchDTO search);

        /// <summary>
        /// 判断商品是不是定制应用所发布的商品。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsFittedAppCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<bool> IsFittedAppCommodity(Guid commodityId);


        /// <summary>
        /// 获取发布商品的店铺的appId
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Guid> GetCommodityAppId(Guid commodityId);


        /// <summary>
        /// 获取商品简略信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityThumb", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommodityThumb> GetCommodityThumb(Guid commodityId);

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityAttribute(System.Guid commodityId, Guid userId);

        /// <summary>
        /// 获取商品的属性和优惠信息（支持单属性SKU）
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityAttributeNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CommoditySDTO> GetCommodityAttributeNew(System.Guid commodityId, Guid userId);

        /// <summary>
        /// 多应用：根据商品Ids获取商品信息列表
        /// </summary>
        /// <param name="search">查询条件，有效参数CommodityIdList,IsDefaultOrder,AreaCode</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByIdsNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityByIdsNew(CommoditySearchDTO search);



        /// <summary>
        /// 校验商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodityV3", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CheckCommodityDTO> CheckCommodityV3(CheckCommodityParam ccp);
        /// <summary>
        /// 校验商品信息  金采团购活动 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodityV3II", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CheckCommodityDTO> CheckCommodityV3II(CheckCommodityParam ccp);

        /// <summary>
        /// 校验购物车商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodityV4", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CheckShopCommodityDTO> CheckCommodityV4(CheckShopCommodityParam ccp);

        /// <summary>
        /// 获取商品列表，提供跨店铺优惠券
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityListForCoupon", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityListForCoupon(CommodityListSearchDTO search);

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityListV2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search);
        /// <summary>
        ///xiexg 优惠券形式获取商品列表
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityListV2_New", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityListV2_New(CommodityListSearchDTO search);
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityList3", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityList3(CommodityListSearchDTO search);
        /// <summary>
        /// 商品改低价格时，Job发送消息(每10分钟处理一次)
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoPushCommodityModifyPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AutoPushCommodityModifyPrice();

        /// <summary>
        /// 校验单个商品信息 
        /// </summary> 
        /// <param name="ccp">校验商品信息参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckCommodityV5", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CheckCommodityDTO> CheckCommodityV5(CheckCommodityParam ccp);

        /// <summary>
        /// 获取商品列表关税
        /// </summary> 
        /// <param name="search">商品列表</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetComListDuty", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CreateOrderDutyResultDTO> GetComListDuty(List<ComScoreCheckDTO> search);



        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByName", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto);

        /// <summary>
        /// 同步京东库存
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SynchronizeJDStock", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDStock(Jinher.AMP.BTP.Deploy.CommodityDTO arg);

        /// <summary>
        /// 订单或订单项拒收或取件运费计算
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalRefundFreight", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.FreighMultiAppResultDTO CalRefundFreight(Guid orderId, Guid orderItemId);

        /// <summary>
        /// 查询某个APP下的商品 按照销量进行相关排序
        /// </summary>
        /// <param name="commoditySearch">查询类</param>
        /// <returns>查询结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityWithSales", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityWithSales(CommoditySearchDTO commoditySearch);

        /// <summary>
        /// 更新商品表的赠送油卡量
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityYouka", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodityYouka(Guid commodityId, decimal youka);


        /// <summary>
        /// 获取商品税收编码列表(openApi)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSingleCommodityCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse<CommodityTaxRateZphDto> GetSingleCommodityCode();


        /// <summary>
        /// 同步商品库存(openApi)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ModifyCommodityStock", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse ModifyCommodityStock(string Code, string skuId, int Stock);


        /// <summary>
        /// 添加商品(openApi)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse AddCommodity(List<Commoditydto> objlist);



        /// <summary>
        /// 修改商品名称(openApi)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ModifyCommodityName", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse ModifyCommodityName(string skuId, string CommodityName);

        /// <summary>
        /// 修改商品价格(openApi)
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ModifyCommodityPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse ModifyCommodityPrice(string skuId, decimal Price);

        /// <summary>
        /// 修改商品上下架(openApi)
        /// </summary>
        /// <param name="SupId"></param>
        /// <param name="State">0上架，1下架</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/Upperandlower", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse Upperandlower(string SupId,int State);


        #region NetCore刷新缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/NetCoreAutoAuditJdCommodity1", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO NetCoreAutoAuditJdCommodity1();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/NetCoreAutoAuditJdCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO NetCoreAutoAuditJdCommodity(JdBTPRefreshCache dict);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/NetCoreAutoAuditJdPromotions", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO NetCoreAutoAuditJdPromotions(JdBTPRefreshCache dict);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/NetCoreAutoAuditJdCountInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO NetCoreAutoAuditJdCountInfo(JdBTPRefreshCache dict);

        #endregion


        /// <summary>
        /// 根据appid和订单id查找该订单下所有的商品信息(评价用)
        /// </summary>
        /// <param name="Code">订单编号</param>
        /// <param name="OrderId">订单号id</param>
        /// <param name="Commodityid">商品id（单个评价用）</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderIdComInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityDTO>> GetOrderIdComInfo(string Code, Guid OrderId, Guid Commodityid);
    }

}
