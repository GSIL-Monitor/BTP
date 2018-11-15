using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 众筹相关接口
    /// </summary>
    [ServiceContract]
    public interface ICrowdfunding : ICommand
    {
        /// <summary>
        /// 获取众筹
        /// </summary>
        /// <param name="appId">appId</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCrowdfundingByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CrowdfundingDTO GetCrowdfundingByAppId(Guid appId);
        /// <summary>
        /// 更多众筹项目
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMoreCrowdfundings", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<MoreCrowdfundingDTO> GetMoreCrowdfundings(Guid userId, int pageIndex, int pageSize);
        /// <summary>
        /// 领取单个分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="dividendId">分红Id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DrawDividend", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawDividend(Guid userId, Guid dividendId);
        /// <summary>
        /// 领取用户所有分红
        /// </summary>
        /// <param name="userId">用户Id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DrawUserDividends", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawUserDividends(Guid userId);

        /// <summary>
        /// 获取宣传语
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCrowdfundingSlogan", BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetCrowdfundingSlogan(Guid appId);

        /// <summary>
        /// 获取重筹详细说明
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCrowdfundingDesc", BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetCrowdfundingDesc(Guid appId);

        /// <summary>
        /// 下订单或购物车获取众筹信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>

        [WebInvoke(Method = "POST", UriTemplate = "/GetUserCrowdfundingBuy", BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO GetUserCrowdfundingBuy(Guid appId, Guid userId);


        /// <summary>
        /// 获取用户众筹汇总信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserCrowdfundingStatistics", BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CrowdfundingStatisticsDTO GetUserCrowdfundingStatistics(Guid userId);

        /// <summary>
        /// 消息订阅更新用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateUserName", BodyStyle = WebMessageBodyStyle.WrappedRequest,
 ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateUserName(Tuple<Guid, string> userIdName);

        /// <summary>
        /// 消息订阅更新应用名
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAppName", BodyStyle = WebMessageBodyStyle.WrappedRequest,
   ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateAppName(Tuple<Guid, string> appIdName);

        /// <summary>
        /// 获得众筹分红更新条数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsCfDividendMore", BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CfDividendMoreDTO IsCfDividendMore(Guid userId);


        /// <summary>
        ///  获取众筹状态
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCrowdfundingState", BodyStyle = WebMessageBodyStyle.WrappedRequest,
     ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO GetCrowdfundingState(Guid appId);

        /// <summary>
        /// 众筹每日统计、众筹股东每日统计，更新CrowdfundingDaily、UserCrowdfundingDaily表
        /// </summary>
        /// <param name="calcDate">被统计日期</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalcUserCrowdfundingDaily", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool CalcUserCrowdfundingDaily(DateTime calcDate);

        /// <summary>
        /// 每日计算众筹分红，更新CfOrderDividendDetail、CfDividend表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalcCfDividend", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool CalcCfDividend();

        /// <summary>
        /// 众筹汇总统计，更新CrowdfundingStatistics表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CalcCfStatistics", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool CalcCfStatistics();

        /// <summary>
        /// 众筹测试方法
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeDate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool ChangeDate(Guid appId, int day);

        /// <summary>
        /// 众筹测试方法
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelCf", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool DelCf(Guid appId);


        /// <summary>
        /// 向前修改众筹时间，重新计算股东
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ChangeCfStartTimeEarlier", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool ChangeCfStartTimeEarlier(Guid appId);



        /// <summary>
        /// 下订单或购物车获取众筹信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetUserCrowdfundingBuyer", BodyStyle = WebMessageBodyStyle.WrappedRequest,
           ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.UserOrderCarDTO> GetUserCrowdfundingBuyer(List<Guid> appId, Guid userId);

    }
}
