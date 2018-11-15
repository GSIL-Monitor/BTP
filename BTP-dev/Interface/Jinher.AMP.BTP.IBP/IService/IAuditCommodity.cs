
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/2/2 17:25:51
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.MallApply;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface IAuditCommodity : ICommand
    {
        /// <summary>
        /// 获取审核商品信息(商铺提交)
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Name"></param>
        /// <param name="CateNames"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetApplyCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndCategoryDTO>> GetApplyCommodityList(List<Guid> AppidList, string Name, string CateNames, int AuditState, int pageIndex, int pageSize);
        /// <summary>
        /// 获取审核信息(馆审核)
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndCategoryDTO>> GetAuditCommodityList(Guid EsAppId, List<Guid> AppidList, string Name, int AuditState, int pageIndex, int pageSize);
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CommodityId"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityAndCategoryDTO GetAuditCommodity(Guid Id,Guid CommodityId, Guid AppId);
        /// <summary>
        /// 发布的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddAuditCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddAuditCommodity(CommodityAndCategoryDTO com);
        /// <summary>
        /// 编辑的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/EditAuditCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO EditAuditCommodity(CommodityAndCategoryDTO com);        
        /// <summary>
        ///设置审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetModeStatus", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetModeStatus(Guid Appid, int ModeStatus);
        /// <summary>
        /// 获取设置的审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetModeStatus", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AuditModeDTO GetModeStatus(Guid Appid);
        /// <summary>
        /// 手动审核商品
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AuditApply", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AuditApply(List<Guid> ids, int state, string AuditRemark);
         /// <summary>
        /// 获取易捷馆及入住商铺的Appids
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetYiJieAppids", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Guid> GetYiJieAppids();
        /// <summary>
        /// 判断该商铺商品是否需要被审核
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsAuditAppid", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool IsAuditAppid(Guid AppId);
        /// <summary>
        /// 根据AppId获取审核方式
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsAutoModeStatus", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool IsAutoModeStatus(Guid EsAppId);
        /// <summary>
        /// 判断商品是否存在
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsExistCom", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool IsExistCom(Guid CommodityId,Guid AppId);
        /// <summary>
        /// 判断馆或店铺是否需要审核
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/IsNeedAudit", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool IsNeedAudit(Guid EsAppId);
        /// <summary>
        /// 取出最后提交的待审核商品信息
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetApplyCommodityInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        AuditCommodityDTO GetApplyCommodityInfo(Guid CommodityId,Guid AppId);
    }
}
