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
    /// <summary>
    /// 京东日志
    /// </summary>
    [ServiceContract]
    public interface IJdlogs : ICommand
    {
        /// <summary>
        /// 查询所有的京东日志信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetALLJdlogsList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.JdlogsDTO> GetALLJdlogsList(Jinher.AMP.BTP.Deploy.CustomDTO.JdlogsDTO model);


        /// <summary>
        /// 根据Id获取京东的日志内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetJdlogs", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.JdlogsDTO GetJdlogs(Guid Id);



        /// <summary>
        /// 保存京东日志信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveJdlogs", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model);


        /// <summary>
        ///修改京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateJdlogs", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model);


        /// <summary>
        /// 根据id删除京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteJdlogs", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteJdlogs(Guid id);

       

     
    }
}
