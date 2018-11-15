
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/25 14:56:38
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
    public interface ISecondAttribute : ICommand
    {

        /// <summary>
        /// 添加尺寸/颜色
        /// </summary>
        /// <param name="attributeId">属性ID</param>
        /// <param name="name">二级属性名</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddSecondAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddSecondAttribute(Guid attributeId, string name, Guid appid);

        /// <summary>
        /// 查询卖家所有已存在尺寸/颜色
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAttributeBySellerID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ColorAndSizeAttributeVM> GetAttributeBySellerID(Guid sellerID, Guid attributeid);

        /// <summary>
        /// 属性删除
        /// </summary>
        /// <param name="secondAttributeId">次级属性ID</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelSecondAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelSecondAttribute(Guid secondAttributeId, Guid appid);
        /// <summary>
        /// 是否已有属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //[WebInvoke(Method = "POST", UriTemplate = "/IsExists", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        //[OperationContract]
        bool IsExists(string name, Guid appid, Guid attId);

        /// <summary>
        ///查询卖家所有已存属性
        /// </summary>
        /// <param name="sellerID">卖家ID</param>
        /// <param name="attributeid">属性ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAttributeByAppID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ColorAndSizeAttributeVM> GetAttributeByAppID(Guid appID);

          /// <summary>
        /// 商品属性添加
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddAttribute(System.Guid attributeId, string name, Guid appid);

        
        /// <summary>
        /// 商品属性编辑
        /// </summary>
        /// <param name="attributeId"></param>
        /// <param name="name"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateAttribute(System.Guid attributeId, string name, Guid appid);
    }
}
