
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/8/16 9:51:27
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
    public interface ICouponRefund : ICommand
    {

        [WebInvoke(Method = "POST", UriTemplate = "/GetCouponRefundList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<CouponRefundDetailDTO>> GetCouponRefundList(RefundCouponSearchDTO search);

        /// <summary>
        /// 更新备注
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Remark"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCouponRefundList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateRemark(Guid Id, String Remark);

        /// <summary>
        /// 根据优惠券ID获取使用优惠券的订单信息
        /// </summary>
        /// <param name="CouponId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderInfoByCouponId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<KeyValuePair<Guid, string>>> GetOrderInfoByCouponId(Guid CouponId);
    }
}
