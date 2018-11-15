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
    [ServiceContract]
    public interface IFreight : ICommand
    {
        /// <summary>
        /// 获取运费模板列表
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="pageIndex">查询第几页的数据</param>
        /// <param name="pageSize">每页查询几条数据</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFreightTemplateListByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<FreightDTO> GetFreightTemplateListByAppId(Guid appId, int pageIndex, int pageSize, out int rowCount);
        /// <summary>
        /// 获取运费模板详细数据
        /// </summary>
        /// <param name="freightTemplateId">运费模板编号</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFreightTemplateDetailListByTemId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<FreightTemplateDetailDTO> GetFreightTemplateDetailListByTemId(Guid freightTemplateId);
        /// <summary>
        /// 增加一条运费模板详细信息
        /// </summary>
        /// <param name="freightDetail"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddFreightDetail", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddFreightDetail(FreightTemplateDetailDTO freightDetail);
        /// <summary>
        /// 删除一条运费模板明细信息
        /// </summary>
        /// <param name="freightDetailId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteFreightDetail", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteFreightDetail(Guid freightDetailId);
        /// <summary>
        /// 删除一条运费模板信息
        /// </summary>
        /// <param name="Id">运费模板编号</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteFreight", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteFreight(Guid Id);
        /// <summary>
        /// 保存运费模板及其明细
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/AddFreightAndFreightDetail", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddFreightAndFreightDetail(FreightTemplateDTO freight, List<FreightTemplateDetailDTO> freightDetailList);
        /// <summary>
        /// 更新运费模板和运费详细信息列表
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFreightTemplateDetailListByTemId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateFreightAndFreightDetail(FreightTemplateDTO freight, List<FreightTemplateDetailDTO> freightDetailList);
        /// <summary>
        /// 获取一条运费记录
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetOneFreight", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        FreightDTO GetOneFreight(Guid freightTemplateId);


        /// <summary>
        ///获取运费模板列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFreightListByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<FreightDTO> GetFreightListByAppId(Guid appId);
        /// <summary>
        /// 运费模板是否关联了商品
        /// </summary>
        /// <param name="freightTemplateId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetFreightListByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO IsContactCommodity(Guid freightTemplateId);



        /// <summary>
        /// 保存运费模板及其明细
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveFreightTemplate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveFreightTemplateFull(FreightTempFullDTO freightDTO);


        /// <summary>
        /// 保存区间运费模板及其明细
        /// </summary>
        /// <param name="freight"></param>
        /// <param name="freightDetailList"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveRangeFreightTemplate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveRangeFreightTemplate(RangeFreightTemplateInputDTO freightDTO);

        /// <summary>
        /// 建立运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/JoinCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO JoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO);

        /// <summary>
        /// 解除运费模板与选定商品的关联
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UnjoinCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UnjoinCommodity(FreightTemplateAssociationCommodityInputDTO inputDTO);
    }
}
