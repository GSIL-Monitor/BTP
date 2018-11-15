/***************
功能描述: BTPIService
作    者: LSH
创建时间: 2017/09/16 13:32:08
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
    public interface IInnerCategory : ICommand
    {
        /// <summary>
        /// 添加同级类别
        /// </summary>
        /// <param name="categoryName">分类名称</param>
        /// <param name="appId">卖家ID</param>
        /// <param name="targetId">目标类别ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddCategory(string categoryName, Guid appId, Guid targetId);

        /// <summary>
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategorySDTO> GetCategories(Guid appId);

        /// <summary>
        /// 查询卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategories2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategoryS2DTO> GetCategories2(Guid appId);

        /// <summary>
        /// 删除卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCategory(Guid appId, Guid myId);

        /// <summary>
        /// 编辑卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="name">类别名称</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCategory(Guid appId, string name, Guid myId);

        /// <summary>
        /// 编辑卖家类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="name">类别名称</param>
        /// <param name="myId">被操作类别ID</param>
        /// <param name="icon">图标</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCategory2(Guid appId, string name, Guid myId, string icon);

        /// <summary>
        /// 升级类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/LevelUpCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO LevelUpCategory(Guid appId, Guid targetId, Guid myId);

        /// <summary>
        /// 降级类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/LevelDownCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO LevelDownCategory(Guid appId, Guid targetId, Guid myId);

        /// <summary>
        /// 升序类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpCategory(Guid appId, Guid targetId, Guid myId);

        /// <summary>
        /// 降序类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DownCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DownCategory(Guid appId, Guid targetId, Guid myId);

        /// <summary>
        /// 拖动类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DragCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DragCategory(Guid appId, Guid targetId, Guid myId, string moveType);

        /// <summary>
        /// 添加子级类别
        /// </summary>
        /// <param name="name">类目名称</param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddChildCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddChildCategory(string name, Guid targetId, Guid appId);

        /// <summary>
        /// 添加子级类别
        /// </summary>
        /// <param name="name">类目名称</param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddChildCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddChildCategory2(string name, Guid targetId, Guid appId, string icon);

        /// <summary>
        /// 创建初始类别
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CreatCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CreatCategory(Guid appId);

        /// <summary>
        /// 创建初始类别（三级分类）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="targetId">目标类别ID</param>
        /// <param name="myId">被操作类别ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CreatCategory2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CreatCategory2(Guid appId);

        /// <summary>
        /// 校验app是否显示search菜单
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckIsShowSearchMenu", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<bool> CheckIsShowSearchMenu(CategorySearchDTO search);

        /// <summary>
        /// 保存是否显示菜单标志
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateIsShowSearchMenu", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateIsShowSearchMenu(CategorySearchDTO search);



        /// <summary>
        /// 查询卖家类别提供给中石化的接口
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetZshCategories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategorySDTO> GetZshCategories();

    }
}
