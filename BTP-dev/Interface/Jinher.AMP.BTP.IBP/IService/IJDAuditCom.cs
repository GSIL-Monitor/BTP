
/***************
功能描述: BTPIService
作    者: 
创建时间: 2018/2/27 10:15:51
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
    public interface IJDAuditCom : ICommand
    {
        /// <summary>
        /// 京东售价审核列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetEditPriceList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndCategoryDTO>> GetEditPriceList(Guid AppId, string Name,string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize);

        /// <summary>
        ///设置售价审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetEditPriceMode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetEditPriceMode(Guid Appid, int ModeStatus);
        /// <summary>
        ///设置进货价审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetEditCostPriceMode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetEditCostPriceMode(Guid Appid, int ModeStatus);
        /// <summary>
        /// 审核京东售价
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AuditJDPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AuditJDPrice(List<Guid> ids, int state, decimal SetPrice, string AuditRemark, int JdAuditMode);
        /// <summary>
        /// 审核京东进货价
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AuditJDCostPrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AuditJDCostPrice(List<Guid> ids, int state, string AuditRemark, int Dispose, int JdAuditMode);
        /// <summary>
        /// 获取商铺审核方式
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAuditMode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        JDAuditModeDTO GetAuditMode(Guid AppId);
        /// <summary>
        /// 导出京东进货价审核列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportPriceList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndCategoryDTO>> ExportPriceList(Guid AppId, string Name,string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action);

        /// <summary>
        /// 获取下架无货商品审核列表
        /// </summary>
        /// <param name="AppIds"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="EditStartime"></param>
        /// <param name="EditEndTime"></param>
        /// <param name="Action"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOffSaleAndNoStockList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndCategoryDTO>> GetOffSaleAndNoStockList(Guid AppId, string Name,string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize);
        /// <summary>
        ///设置下架无货商品审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetOffAndNoStockMode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetOffAndNoStockMode(Guid Appid, int ModeStatus);
        /// <summary>
        /// 导出下架无货商品审核列表数据
        /// </summary>
        /// <param name="AppIds"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="EditStartime"></param>
        /// <param name="EditEndTime"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/ExportOffSaleAndNoStockData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndCategoryDTO>> ExportOffSaleAndNoStockData(Guid AppId, string Name,string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action);
        /// <summary>
        /// 置为有货
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetInStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetInStore(List<Guid> ids, int JdAuditMode);
        /// <summary>
        /// 置为上架
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetPutaway", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetPutaway(List<Guid> ids, int JdAuditMode);
        /// <summary>
        /// 置为售罄
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetNoStock", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetNoStock(List<Guid> ids, int JdAuditMode);
        /// <summary>
        /// 置为下架
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetOffShelf", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetOffShelf(List<Guid> ids, int JdAuditMode);
    }
}
