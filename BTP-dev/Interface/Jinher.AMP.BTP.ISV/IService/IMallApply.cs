
/***************
功能描述: BTPIService
作    者: LSH
创建时间: 2017/8/19 11:14:32
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
    public interface IMallApply : ICommand
    {
        /// <summary>
        /// 获取易捷北京商家信息
        /// </summary>
        /// <param name="appIds">商家Id列表</param>
        /// <returns>商家信息</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYJSellerInfoes", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SellerInfoDTO> GetYJSellerInfoes(List<Guid> appIds);

        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMallApplyInfoList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(Guid appId);

        /// <summary>
        /// 获取商品列表 轮播图片 直播列表      
        /// </summary>
        /// <param name="search">查询条件model</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityListV3", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ComdtyListResultCDTO GetCommodityListV3(CommodityListSearchDTO search);

        /// <summary>
        /// 获取App入驻类型
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMallTypeListByEsAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<MallTypeDTO> GetMallTypeListByEsAppId(Guid esAppId);
    }
}
