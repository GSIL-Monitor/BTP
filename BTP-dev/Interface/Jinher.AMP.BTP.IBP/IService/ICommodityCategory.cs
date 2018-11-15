
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/25 13:31:58
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface ICommodityCategory : ICommand
    {



        /// <summary>
        /// 添加商品分类
        /// </summary>
        /// <param name="commodityCategoryDTO"></param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddCommodityCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AddCommodityCategory(CommodityCategoryDTO commodityCategoryDTO);

        /// <summary>
        /// 删除商品分类
        /// </summary>
        /// <param name="commodityId">商品id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCommodityCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("已废弃", false)]
        void DeleteCommodityCategory(Guid commodityId);

        /// <summary>
        /// 按商品分类查询商品
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <param name="pageSize">分页数</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityCategoryByCategory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("已废弃", false)]
        List<CommodityCategoryDTO> GetCommodityCategoryByCategory(Guid categoryId, int pageSize, int pageIndex);

        /// <summary>
        /// 查询所有商品分类
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        List<CommodityCategoryDTO> GetAllCommodityCategory();

        /// <summary>
        /// 根据AppId和查询所有商品分类
        /// </summary>
        /// <returns></returns>
        List<CommodityCategoryDTO> GetAllCommodityCategoryByAppId(Guid commodityId, Guid appId, int pageSize, int pageIndex);



    }
}
