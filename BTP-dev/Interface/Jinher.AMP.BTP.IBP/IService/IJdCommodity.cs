/***************
功能描述: BTPIService
作    者:  LSH
创建时间: 2017/9/21 10:52:36
***************/

using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface IJdCommodity : ICommand
    {
        /// <summary>
        /// 查询列表信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJdCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<JdCommodityDTO>> GetJdCommodityList(JdCommoditySearchDTO input);
        /// <summary>
        /// 新建商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AddJdCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddJdCommodity(JdCommodityDTO input);       
        /// <summary>
        /// 获取商品详情
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJdCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        JdCommodityDTO GetJdCommodityInfo(Guid Id);       
        /// <summary>
        /// 批量删除商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DelJdCommodityAll", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelJdCommodityAll(List<Guid> Ids);
        /// <summary>
        /// 导出商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportJdCommodityData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<JdCommodityDTO>> ExportJdCommodityData(JdCommoditySearchDTO input);        
        /// <summary>
        /// 导入京东商品信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ImportJdCommodityData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> ImportJdCommodityData(List<JdCommodityDTO> JdComList,Guid AppId);
        /// <summary>
        /// 自动同步商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AutoSyncCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<JdCommoditySearchDTO> AutoSyncCommodityInfo(Guid AppId, List<Guid> Ids);
        /// <summary>
        /// 获取商城品类
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<InnerCategoryDTO> GetCategories(Guid AppId);
        /// <summary>
        /// 获取商城类目
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CategoryDTO> GetCategoryList(Guid AppId); 
    }
}
