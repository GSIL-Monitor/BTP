
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/24 13:19:55
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

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 商品分类接口
    /// </summary>
    [ServiceContract]
    public interface ICategory : ICommand
    {
        /// <summary>
        /// 获取商品分类
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.CategorySV.svc/GetCategory
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategorySDTO> GetCategory(Guid appId);

        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategoryS2DTO> GetCategory2(Guid appId);

        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="levelCount">分类级别</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryByDrawer", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategoryS2DTO> GetCategoryByDrawer(Guid appId, out int levelCount);

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("请参见新版本", false)]
        List<CommodityListCDTO> GetCommodityList(CommodityListInferSearchDTO commodityListInfer);

        /// <summary>
        /// 校验app是否显示search菜单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckIsShowSearchMenu", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<bool> CheckIsShowSearchMenu(CategorySearchDTO search);

        /// <summary>
        /// 获取所有类目信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCacheCateGories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategoryCache2DTO> GetCacheCateGories(Guid appid);

        /// <summary>
        /// 删除指定“电商馆”下applist下的商品分类关系
        /// </summary>
        /// <param name="belongTo">电商馆APPId</param>
        /// <param name="appList">applist</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCommodityCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCommodityCategory(Guid belongTo, List<Guid> appList);

        /// <summary>
        /// 分页获取电商馆下商品
        /// </summary>
        /// <param name="comdtySearch4SelCdto"></param>
        /// <param name="comdtyCount"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityLisByBeLongTo", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityListCDTO> GetCommodityLisByBeLongTo(Jinher.AMP.ZPH.Deploy.CustomDTO.ComdtySearch4SelCDTO comdtySearch4SelCdto, out int comdtyCount);


        /// <summary>
        /// 获取应用的一级商品分类
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.CategorySV.svc/GetCategoryL1
        /// </para>
        /// </summary>        
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryL1", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategorySDTO> GetCategoryL1(Guid appId);

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityListV2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityListV2(CommodityListInferSearchDTO commodityListInfer);

        /// <summary>
        /// 分页获取分类下筛选商品
        /// </summary>
        /// <param name="commodityListInfer"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityFilterList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityFilterList(CommodityListInferSearchDTO commodityListInfer);


        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityFilterListSecond", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityFilterListSecond(CommodityListInferSearchDTO commodityListInfer);


        /// <summary>
        /// 获取一级分类下的品牌及广告信息
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrandAndAdvertise", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<CategoryS2DTO> GetBrandAndAdvertise(Guid CategoryID);
		
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetZshCategories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.CategoryDto> GetZshCategories();
    }
}
