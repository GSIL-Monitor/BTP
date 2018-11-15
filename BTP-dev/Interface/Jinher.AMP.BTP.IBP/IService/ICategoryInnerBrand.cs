
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/12 9:13:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using System.Collections;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface ICategoryInnerBrand : ICommand
    {
        [WebInvoke(Method = "POST", UriTemplate = "/Add", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Add(CategoryInnerBrandDTO model);
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrandWallList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<IList<CategoryInnerBrandDTO>> GetBrandWallList(Guid CategoryId);
        /// <summary>
        /// 删除多个分类品牌
        /// </summary>
        /// <param name="Id">分类Id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCateBrandsByCategoryId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCateBrandsByCategoryId(Guid CategoryId);
        /// <summary>
        /// 添加多个分类频偏
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddList(List<CategoryInnerBrandDTO> models);

    }
}
