
/***************
功能描述: BTPIService
作    者: 
创建时间: 2016/12/10 16:31:24
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
    public interface IWeChatQRCode : ICommand
    {
        /// <summary>
        /// 创建公众号带参二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateWeChatQRCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO CreateWeChatQRCode(Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO dto);

        /// <summary>
        /// 获取最大自增号
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetWeChatQRNo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        int GetWeChatQRNo();

        /// <summary>
        /// 添加微信菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="menuJson"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddWeChatMenu", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool AddWeChatMenu(Guid appId, string menuJson);

        /// <summary>
        /// 获取二维码类型
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetQrCodeTypeList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.QrTypeDTO>> GetQrCodeTypeList(Deploy.CustomDTO.WeChatQRCodeSearchDTO search);

        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetWechatQrCodeList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<Deploy.CustomDTO.ListResult<Deploy.CustomDTO.WeChatQRCodeShowDTO>> GetWechatQrCodeList(Deploy.CustomDTO.WeChatQRCodeSearchDTO search);

        /// <summary>
        /// 批量创建公众号二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CreateWeChatQRCodeBatch", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO CreateWeChatQrCodeBatch(Deploy.CustomDTO.QrCodeCreateDTO dto);

        /// <summary>
        /// 启用、停用二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO UpdateState(Deploy.CustomDTO.WeChatQRCodeUpdateStateDTO search);
    }
}
