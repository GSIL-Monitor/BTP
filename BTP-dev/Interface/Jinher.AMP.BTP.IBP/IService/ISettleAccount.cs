using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using System;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 结算服务接口
    /// </summary>
    [ServiceContract]
    public interface ISettleAccount : ICommand
    {
        /// <summary>
        /// 获取商城结算数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccounts", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SettleAccountListDTO>> GetMallSettleAccounts(SettleAccountSearchDTO searchDto);

        /// <summary>
        /// 获取商家数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccounts", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SettleAccountListDTO>> GetSellerSettleAccounts(SettleAccountSearchDTO searchDto);

        /// <summary>
        ///  获取结算单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccountDetails", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<SettleAccountDetailsDTO> GetSettleAccountDetails(Guid id);

        /// <summary>
        /// 获取结算单订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccountOrders", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SettleAccountOrderDTO>> GetSettleAccountOrders(SettleAccountOrderSearchDTO searchDto);

        /// <summary>
        ///  获取结算单订单项详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccountOrderItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SettleAccountOrderItemDTO>> GetSettleAccountOrderItems(Guid id);

        /// <summary>
        /// 修改结算单状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateState(SettleAccountUpdateStateDTO dto);


        /// <summary>
        /// 获取商家结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccountHistories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SettleAccountListDTO>> GetSellerSettleAccountHistories(SettleAccountHistorySearchDTO searchDto);

        /// <summary>
        /// 获取商城结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccountHistories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SettleAccountListDTO>> GetMallSettleAccountHistories(SettleAccountHistorySearchDTO searchDto);

        /// <summary>
        /// 修改计算结果
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSettleStatue", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateSettleStatue(SettleAccountUpdateSettleStatueDto dto);

        /// <summary>
        /// 获取商城的结算周期
        /// </summary>
        /// <param name="esAppId">商城id</param>
        /// <returns>结算周期</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSettleAccountPeriod", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<SettleAccountPeriodDTO> GetSettleAccountPeriod(Guid esAppId);

        /// <summary>
        /// 修改商城的结算周期
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSettleAccountPeriod", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateSettleAccountPeriod(SettleAccountPeriodDTO dto);

        /// <summary>
        /// 导入历史订单，生成结算项
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateSettleAccountDetails", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CreateSettleAccountDetails(SettleAccountDetailsCreateDTO dto);


        /// <summary>
        /// 获取商品的结算价
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditySettleAmount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<CommoditySettleAmountListDTO>> GetCommoditySettleAmount(CommoditySettleAmountSearchDTO searchDto);

        /// <summary>
        /// 设置商品的结算价
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/SetCommoditySettleAmount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetCommoditySettleAmount(CommoditySettleAmountInputDTO input);

        /// <summary>
        /// 获取商品的历史结算价
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditySettleAmountHistories", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<CommoditySettleAmountHistoryDTO>> GetCommoditySettleAmountHistories(Guid commodityId, int pageIndex, int pageSize);

        /// <summary>
        /// 删除商品的历史结算价
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCommoditySettleAmountHistory", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCommoditySettleAmountHistory(Guid id);
    }
}
