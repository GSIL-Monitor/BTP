
/***************
功能描述: BTPIService
作    者: 
创建时间: 2015/8/19 13:43:05
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface ISpreadInfo : ICommand
    {
        /// <summary>
        /// 保存推广信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSpreadInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveSpreadInfo(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSaveDTO dto);


        /// <summary>
        /// 获取推广主列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSpreadInfoList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<ListResult<Deploy.CustomDTO.SpreadInfoShowDTO>> GetSpreadInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSearchDTO search);

        /// <summary>
        /// 绑定微信二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/BindWeChatQrCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO BindWeChatQrCode(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadBindWeChatQrCodeDTO search);

        /// <summary>
        /// 启用、停用二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO UpdateState(Deploy.CustomDTO.SpreadUpdateStateDTO search);

        /// <summary>
        /// 修改子代理数量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSubCount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO UpdateSubCount(Deploy.CustomDTO.SpreadUpdateSubSpreadCountDTO dto);

        /// <summary>
        /// 修改总代分佣比例
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateDividendPercent", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO UpdateDividendPercent(Deploy.CustomDTO.SpreadUpdateDividendPercentDTO dto);

        /// <summary>
        /// 查询一级代理推广App列表
        /// </summary>
        /// <param name="iwId">组织ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetLv1SpreadApps", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<List<SpreadAppDTO>> GetLv1SpreadApps(Guid iwId);


        /// <summary>
        /// 查询一级代理指定APP的旺铺列表
        /// </summary>
        /// <param name="iwId">组织ID</param>
        /// <param name="appId">应用ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetLv1SpreadHotshops", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<List<SpreadAppDTO>> GetLv1SpreadHotshops(Guid iwId, Guid appId);
    }
}
