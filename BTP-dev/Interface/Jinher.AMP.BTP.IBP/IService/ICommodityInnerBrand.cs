
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/23 16:04:00
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
    public interface ICommodityInnerBrand : ICommand
    {

         /// <summary>
        /// 添加商品品牌
        /// </summary>
        /// <param name="brandWallDto">商品品牌实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddComInnerBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddComInnerBrand(CommodityInnerBrandDTO innerBrandDto);
        /// <summary>
        /// 根据商品Id查询商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetComInnerBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityInnerBrandDTO GetComInnerBrand(Guid commodityId);
        /// <summary>
        /// 删除商品品牌
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelComInnerBrand", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelComInnerBrand(Guid commodityId);
    }
}
