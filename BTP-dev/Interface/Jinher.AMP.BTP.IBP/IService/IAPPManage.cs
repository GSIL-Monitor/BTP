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
    public interface IAPPManage : ICommand
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="AppManageDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddAPPManage", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddAPPManage(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="AppManageDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateAPPManage", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateAPPManage(Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO AppManageDTO);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DelAPPManage", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelAPPManage(Guid Id);


        /// <summary>
        /// 查询没有删除的app
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAPPManageList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.CustomDTO.APPManageDTO> GetAPPManageList();

        /// <summary>
        /// 过滤非法应用
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ForbitApp", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO ForbitApp();
    }
}
