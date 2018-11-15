
/***************
功能描述: BTPIService
作    者: 
创建时间: 2015/1/7 16:26:53
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.Apm.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 商品列表 --- 厂家直销
    /// </summary>
    [ServiceContract]
    public interface IAppSet : ICommand
    {
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="QryCommodityDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityList(QryCommodityDTO qryCommodityDTO);

        /// <summary>
        /// 获取商品分类列表
        /// </summary>
        /// <param name="QryCommodityDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CategorySDTO> GetCategory(Guid appId);

        /// <summary>
        /// 按关键字获取商品列表
        /// </summary>
        /// <param name="want">关键字</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetWantCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListCDTO> GetWantCommodity(string want, int pageIndex, int pageSize);

        /// <summary>
        /// 厂家直营app查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppSet", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AppSetAppGridDTO GetAppSet(AppSetSearchDTO search);

        /// <summary>
        /// 根据分类Id获取该分类下的app列表
        /// </summary>
        /// <param name="search"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryAppList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<AppSetAppDTO> GetCategoryAppList(AppSetSearchDTO search);

        /// <summary>
        /// 获取正品会“我的”，各栏目数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserInfoCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        UserInfoCountDTO GetUserInfoCount(Guid userId, Guid esAppId);

        /// <summary>
        /// 清理正品会APP缓存
        /// </summary>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveAppInZPHCache", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO RemoveAppInZPHCache();
        /// <summary>
        /// 获取商品列表（平台获取平台商品、店铺获取店铺商品）
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityListV2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV2(CommodityListSearchDTO search);

        /// <summary>
        /// 浏览过的店铺（20个）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrowseAppInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.App.Deploy.CustomDTO.AppIdNameIconDTO> GetBrowseAppInfo(Guid userId, Guid appId);

        /// <summary>
        /// 分页获取浏览商品记录
        /// </summary>
        /// <param name="par"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrowseCommdity", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<BTP.Deploy.CustomDTO.CommodityListCDTO> GetBrowseCommdity(BrowseParameter par);

        /// <summary>
        /// 删除商品浏览记录
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="UserId"></param>
        /// <param name="CommdityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeletebrowseCommdity", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO DeletebrowseCommdity(Guid AppId, Guid UserId, Guid CommdityId);
    }
}
