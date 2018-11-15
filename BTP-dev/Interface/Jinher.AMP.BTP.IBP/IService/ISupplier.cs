/***************
功能描述: BTPIService
作    者:  LSH
创建时间: 2017/9/21 10:52:36
***************/

using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.IService.Interface;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.IBP.IService
{
    /// <summary>
    /// 供应商管理
    /// </summary>
    [ServiceContract]
    public interface ISupplier : ICommand
    {
        /// <summary>
        /// 获取供应商数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSuppliers", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<SupplierListDTO>> GetSuppliers(SupplierSearchDTO searchDto);

        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddSupplier", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddSupplier(SupplierUpdateDTO dto);

        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSupplier", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateSupplier(SupplierUpdateDTO dto);

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteSupplier", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteSupplier(Guid id);

        /// <summary>
        /// 检查供应商编码
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CheckSupplerCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CheckSupplerCode(string code);
        /// <summary>
        /// 获取商城下所有的app信息
        /// </summary>
        /// <param name="esAppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSupplierApps", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<SupplierListDTO> GetSupplierApps(Guid esAppId);
    }
}
