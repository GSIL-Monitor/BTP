
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/12 9:47:23
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
    public interface IBrand : ICommand
    {
        /// <summary>
        /// 添加品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddBrand(BrandwallDTO brandWallDto);

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="brandWallDto">品牌实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateBrand(BrandwallDTO brandWallDto);

        /// <summary>
        /// 查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrandList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BrandwallDTO>> GetBrandList(string brandName, int status, Guid appId);

        /// <summary>
        /// 分页查询品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <param name="status">品牌状态</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrandPageList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<BrandwallDTO>> GetBrandPageList(string brandName, int status, int pageSize, int pageIndex, Guid appId);

        /// <summary>
        /// 是否存在同名品牌
        /// </summary>
        /// <param name="brandName">品牌名称</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool CheckBrand(string brandName, out int rowCount, Guid appId);

        /// <summary>
        /// 更新品牌状态
        /// </summary>
        /// <param name="id">品牌ID</param>
        /// <param name="status">品牌状态</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateBrandStatus", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateBrandStatus(Guid id, int status, Guid appId);

        /// <summary>
        /// 品牌详情
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        BrandwallDTO GetBrand(Guid id, Guid appId);
    }
}
