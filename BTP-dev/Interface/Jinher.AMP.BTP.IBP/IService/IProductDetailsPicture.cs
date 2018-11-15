
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/26 13:44:11
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
    public interface IProductDetailsPicture : ICommand
    {

        /// <summary>
        /// 根据商品ID得到商品所有图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetProductDetailsPictureByCommodityId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ProductDetailsPictureDTO> GetProductDetailsPictureByCommodityId(Guid commodityId);

        /// <summary>
        /// 删除商品图片
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeletePicture", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeletePicture(Guid commodityId);
    }
}
