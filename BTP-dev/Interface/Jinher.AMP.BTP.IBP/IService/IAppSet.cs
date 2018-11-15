using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 应用组业务处理
    /// </summary>
    [ServiceContract]
    [Obsolete("已过期，新功能在zph项目中实现", false)]
    public interface IAppSet : ICommand
    {
        /// <summary>
        /// 分页获取所有电商App
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="addToAppSetStatus">-1全部,1已加入到直销,0未加入直销</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityApp", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AppSetAppGridDTO GetAllCommodityApp(int pageIndex, int pageSize, string appNameForQry, int addToAppSetStatus);

        /// <summary>
        /// 添加应用到应用组
        /// </summary>
        /// <param name="appInfoList">应用信息列表</param>
        /// <param name="appSetId">应用组id</param>
        /// <param name="appSetType">应用组类型</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddAppToAppSet", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddAppToAppSet(List<Tuple<Guid, string, string, DateTime>> appInfoList, Guid appSetId, int appSetType);

        /// <summary>
        /// 从应用组移除应用
        /// </summary>
        /// <param name="appIdList">应用id列表</param>
        /// <param name="appSetId">应用组id</param>
        /// <param name="appSetType">应用组类型</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelAppFromAppSet", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelAppFromAppSet(List<Guid> appIdList, Guid appSetId, int appSetType);

        /// <summary>
        /// 获取树分类列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryListForTree", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<AppSetCategoryDTO> GetCategoryListForTree();

        /// <summary>
        /// 获取树分类列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryListForTree", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<AppSetCategoryDTO> GetCategoryListForTree(Guid appId);

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="name">分类名称</param>
        /// <param name="parentId">父分类Id</param>
        /// <param name="picturesPath">图片路径</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Tuple<Guid, int, string> AddCategory(string name, Guid parentId, string picturesPath);

        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="id">分类id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelCategory(Guid id);

        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="id">分类id</param>
        /// <param name="name">分类名称</param>
        /// <param name="picturesPath">分类图片</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCategory(Guid id, string name, string picturesPath);

        /// <summary>
        /// 分类移动
        /// </summary>
        /// <param name="categoryId">被调序分类的id</param>
        /// <param name="targetCategoryId">与被调序分类互换顺序的分类</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeCategorySort", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ChangeCategorySort(Guid categoryId, Guid targetCategoryId);

        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <param name="categoryId">分类的id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityCountInCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetCommodityCountInCategory(Guid categoryId);

        /// <summary>
        /// 获取分类下的商品数
        /// </summary>
        /// <param name="categoryId">分类的id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityCountInCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetCommodityCountInCategory2(Guid categoryId);

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="commodityNameForQry">商品名称查询字符串</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityInCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AppSetCommodityGridDTO GetCommodityInCategory(Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry);

        /// <summary>
        /// 分页获取分类下商品
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="appNameForQry">应用名查询字符串</param>
        /// <param name="commodityNameForQry">商品名称查询字符串</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityInCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AppSetCommodityGrid2DTO GetCommodityInCategory2(Guid belongTo, Guid categoryId, int pageIndex, int pageSize, string appNameForQry, string commodityNameForQry);


        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryIdList">分类id列表</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCommodityToCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddCommodityToCategory(List<Guid> commodityIdList, List<Guid> categoryIdList);

        /// <summary>
        /// 添加商品到指定分类
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryIdList">分类id列表</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCommodityToCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddCommodityToCategory2(List<Guid> commodityIdList, List<Guid> categoryIdList);

        /// <summary>
        /// 分类下商品排序
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="commoditySortList">商品序号列表</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ReOrderCommodityInCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ReOrderCommodityInCategory(Guid categoryId, List<Guid> commodityIdList, List<int> commoditySortList);

        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryId">分类id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelCommodityFromCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelCommodityFromCategory(List<Guid> commodityIdList, Guid categoryId);

        /// <summary>
        /// 从指定分类中移除商品
        /// </summary>
        /// <param name="commodityIdList">商品id列表</param>
        /// <param name="categoryId">分类id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelCommodityFromCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelCommodityFromCategory2(List<Guid> commodityIdList, Guid categoryId);

        /// <summary>
        /// 调整分类中商品排序(上移\下移)
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityId">商品id</param>
        /// <param name="direction">调整方向(正数下移,负数上移)</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeCommodityOrderInCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ChangeCommodityOrderInCategory(Guid categoryId, Guid commodityId, int direction);

        /// <summary>
        /// 商品在分类中置顶
        /// </summary>
        /// <param name="categoryId">分类id</param>
        /// <param name="commodityId">商品id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/TopCommodityOrderInCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO TopCommodityOrderInCategory(Guid categoryId, Guid commodityId);

        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="appSetSortDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetSetCommodityOrder", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetSetCommodityOrder(AppSetSortDTO appSetSortDto);

        /// <summary>
        /// 设置排序
        /// </summary>
        /// <param name="appSetSortDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetSetCommodityOrder2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetSetCommodityOrder2(AppSetSortDTO appSetSortDto);

    }
}