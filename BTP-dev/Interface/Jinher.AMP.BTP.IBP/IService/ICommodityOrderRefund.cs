
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/6/28 18:44:00
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
    public interface ICommodityOrderRefund : ICommand
    {
        /// <summary>
        /// 添加回款记录
        /// </summary>
        /// <param name="model">回款实体模型</param>
        [WebInvoke(Method = "POST", UriTemplate = "/Insert", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool Insert(CommodityOrderRefundDTO model);
        
        /// <summary>
        /// 根据CommodityOrderId 获取回款记录
        /// </summary>
        /// <param name="CommodityOrderId"></param>
        /// <returns>回款记录列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetListByCommodityOrderId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityOrderRefundDTO> GetListByCommodityOrderId(Guid CommodityOrderId);

       /// <summary>
       /// 根据类型，时间获取回款记录
       /// </summary>
       /// <param name="RefundType">类型0电汇1支票2内部挂帐</param>
       /// <param name="StartTime"></param>
       /// <param name="EndTime"></param>
       /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetListByOther", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityOrderRefundDTO> GetListByOther(Jinher.AMP.BTP.Deploy.Enum.RefundTypeEnum RefundType, DateTime StartTime, DateTime EndTime);
    }
}
