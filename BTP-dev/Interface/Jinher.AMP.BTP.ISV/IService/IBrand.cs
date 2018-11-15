
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/15 11:18:21
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

namespace Jinher.AMP.BTP.ISV.IService
{

    [ServiceContract]
    public interface IBrand : ICommand
    {
        /// <summary>
        /// 根据一级分类ID获取品牌墙信息（热门品牌）
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/getBrandByCateID", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<IList<BrandwallDTO>> getBrandByCateID(Guid CategoryID);

        /// <summary>
        /// 获取指定品牌下的商品
        /// </summary>
        /// <param name="BrandID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/getBrandCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<IList<CommodityDTO>> getBrandCommodity(Guid BrandID);
    }
}
