using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface ICommodityOrderException : ICommand
    {
        /// <summary>
        /// 按条件获取订单异常日志
        /// </summary>
        ///<param name="dto">参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOrderExceptionByAppId", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
       ResultDTO<List<CommodityOrderExceptionDTO>> GetOrderExceptionByAppId(CommodityOrderExceptionParamDTO dto);



        /// <summary>
        /// 更新订单异常日志状态
        /// </summary>
        ///<param name="dto">参数实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateOrderException", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateOrderException(CommodityOrderExceptionDTO dto);

    }
}
