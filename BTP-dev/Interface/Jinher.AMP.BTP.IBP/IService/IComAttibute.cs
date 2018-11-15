
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/25 14:32:16
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
    public interface IComAttibute : ICommand
    {
        /// <summary>
        /// 添加商品颜色/尺寸
        /// </summary>
        /// <param name="secondAttributeIds">尺寸、颜色ID</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="attributeId">一级属性ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddComAttibute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AddComAttibute(List<Guid> secondAttributeIds, Guid commodityId, Guid attributeId);

        /// <summary>
        /// 获取商品颜色/尺寸
        /// </summary>
        /// <param name="appId">appid</param>
        /// <param name="commodityId">商品ID</param>
        /// <param name="attributeId">属性ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetColorOrSizeByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ColorAndSizeAttributeVM> GetColorOrSizeByAppId(Guid appId, Guid commodityId, Guid attributeId);

        /// <summary>
        /// 获取商家颜色/尺寸
        /// </summary>
        /// <param name="appId">appid</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSecondAttributeExt", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SecondAttributeDTO> GetSecondAttribute(System.Guid appId);        
    }
}
