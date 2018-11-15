using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.ISV.IService
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
    }
}
