
/***************
功能描述: ZPHIService
作    者: 
创建时间: 2015/3/10 11:21:13
***************/
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
    public interface ISelectCommodity : ICommand
    {
        /// <summary>
        /// 获取APP
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAppList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.AppSetAppGridDTO GetAppList(Deploy.CustomDTO.AppSetSearch2DTO search);
        
        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SearchCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity(Deploy.CustomDTO.ComdtySearch4SelCDTO search);

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SearchCommodity2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity2(Deploy.CustomDTO.ComdtySearch4SelCDTO search);   

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByFreightTemplate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Deploy.CustomDTO.ResultDTO<SearchCommodityByFreightTemplateOutputDTO> GetCommodityByFreightTemplate(SearchCommodityByFreightTemplateInputDTO inputDTO);  
    }
}
