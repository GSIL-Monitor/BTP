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
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using System.Collections.Generic;

namespace Jinher.AMP.BTP.IBP.IService
{
    [ServiceContract]
    public interface IYJEmployee : ICommand
    {
        /// <summary>
        /// 查询员工信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYJEmployeeList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<YJEmployeeDTO>> GetYJEmployeeList(YJEmployeeSearchDTO input);
        /// <summary>
        /// 根据条件查询员工信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYJEmployeeListBySearch", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<YJEmployeeDTO>> GetYJEmployeeListBySearch(YJEmployeeSearchDTO input);
        /// <summary>
        /// 新建员工
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/AddYJEmployee", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddYJEmployee(YJEmployeeDTO input);
        /// <summary>
        /// 编辑员工
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateYJEmployee", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateYJEmployee(YJEmployeeDTO input);
        /// <summary>
        /// 获取员工详情
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYJEmployeeInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        YJEmployeeDTO GetYJEmployeeInfo(Guid Id);
        /// <summary>
        /// 删除员工
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DelYJEmployee", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelYJEmployee(Guid Id);
        /// <summary>
        /// 批量删除员工
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/DelYJEmployeeAll", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelYJEmployeeAll(List<Guid> Ids);
        /// <summary>
        /// 导出员工信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportYJEmployeeList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<YJEmployeeDTO>> ExportYJEmployeeList(YJEmployeeSearchDTO input);
        /// <summary>
        /// 导出当次无效账户信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportInvalidData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<YJEmployeeDTO>> ExportInvalidData(List<string> UserAccounts,Guid AppId);
        /// <summary>
        /// 导出全部无效账户信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportInvalidDataByAppid", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ListResult<YJEmployeeDTO>> ExportInvalidDataByAppid(Guid AppId);
        /// <summary>
        /// 导入员工信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/ImportYJEmployeeList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<YJEmployeeSearchDTO> ImportYJEmployeeList(List<YJEmployeeDTO> YJEmpList,Guid AppId);
        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdataYJEmployeeInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdataYJEmployeeInfo();
        /// <summary>
        /// 获取易捷员工所属区域信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAreaInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<YJEmployeeDTO> GetAreaInfo(Guid AppId);
        /// <summary>
        ///根据站编码修改站名称
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdataStationNameByCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdataStationNameByCode();
        /// <summary>
        ///根据员工账号更新员工编码
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdataUserCodeByUserAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdataUserCodeByUserAccount();
    }
}
