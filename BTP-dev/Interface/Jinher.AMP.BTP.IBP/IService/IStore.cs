
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/19 18:03:02
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
    public interface IStore : ICommand
    {
        /// <summary>
        /// 添加门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/AddStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO AddStore(StoreDTO storeDTO);
        /// <summary>
        /// 删除门店
        /// </summary>
        /// <param name="id">门店ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DelStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DelStore(Guid id);


        [WebInvoke(Method = "POST", UriTemplate = "/GetAppStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<Jinher.AMP.BTP.Deploy.StoreDTO> GetAppStore(Guid appId);
        


        /// <summary>
        /// 得到门店详细信息
        /// </summary>
        /// <param name="id">门店ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetStoreDTO", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        StoreDTO GetStoreDTO(Guid id,Guid appid);
        /// <summary>
        /// 修改门店
        /// </summary>
        /// <param name="storeDTO">门店实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateStore(StoreDTO storeDTO);
       /// <summary>
       /// 得到所有门店
       /// </summary>
       /// <param name="sellerId">卖家id</param>
       /// <param name="pageSize"></param>
       /// <param name="pageIndex"></param>
       /// <param name="rowCount"></param>
       /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllStore", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<StoreDTO> GetAllStore(Guid sellerId,int pageSize,int pageIndex,out int rowCount);

        /// <summary>
        /// 根据条件查询所有门店
        /// </summary>
        /// <param name="sellerId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="storeName"></param>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllStoreByWhere", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<StoreDTO> GetAllStoreByWhere(Guid sellerId, int pageSize, int pageIndex, out int rowCount, string storeName, string provice, string city, string district);
    }
}
