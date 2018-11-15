
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/24 13:21:16
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
    /// 门店接口
    /// </summary>
    [ServiceContract]
    public interface IStore : ICommand
    {
        /// <summary>
        /// 获取门店
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStore
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <param name="province">省份名称</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NStoreSDTO GetStore(Guid appId, int pageIndex, int pageSize);

        /// <summary>
        /// 按地区查询门店
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStoreByProvince
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="province">省</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetStoreByProvince", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<StoreSDTO> GetStoreByProvince(string province, Guid appId, int pageIndex, int pageSize);

        /// <summary>
        /// 查询有门店的省份列表
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetProvince
        /// </para>
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetProvince", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetProvince(Guid appId);


        /// <summary>
        ///  获取门店列表（按用户当前位置到门店的距离排序）
        /// <para>Service Url: http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetStoreByLocation
        /// </para>
        /// </summary> 
        /// <param name="slp">参数实体类</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetStoreByLocation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NStoreSDTO GetStoreByLocation(StoreLocationParam slp);
        /// <summary>
        /// 获取餐饮平台聚合门店
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCateringPlatformStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NStoreSDTO GetCateringPlatformStore(StoreLocationParam param);

        /// <summary>
        /// 初始化数据 http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/InitMongoFromSql
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/InitMongoFromSql", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void InitMongoFromSql();

        /// <summary>
        /// 获取应用下是否只有一个门店（如果只有一个门店返回门店信息） http://testbtp.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetOnlyStoreInApp
        /// <param name="appId">应用id</param>
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOnlyStoreInApp", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<StoreSDTO> GetOnlyStoreInApp(Guid appId);

        /// <summary>
        /// 获取馆信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppPavilionInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ZPH.Deploy.CustomDTO.AppPavilionInfoIICDTO GetAppPavilionInfo(ZPH.Deploy.CustomDTO.QueryAppPavilionParam param);


        /// <summary>
        /// 获取门店 有效参数：AppId（必填），SubName（非必填）
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetAppStores
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppStores", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<StoreSResultDTO> GetAppStores(StoreLocationParam search);

        /// <summary>
        /// 获取附近门店 有效参数：AppId（必填），Longitude（必填），Latitude（必填），MaxDistance（非必填）
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.StoreSV.svc/GetAppStoresByLocation
        /// </para>
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppStoresByLocation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<StoreSDTO>> GetAppStoresByLocation(StoreLocationParam search);
    }
}
