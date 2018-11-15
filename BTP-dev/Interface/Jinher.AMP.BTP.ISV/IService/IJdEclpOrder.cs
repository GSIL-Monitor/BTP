using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.ISV.IService
{
    [ServiceContract]
    public interface IJdEclpOrder : ICommand
    {
        /// <summary>
        /// 进销存-同步京东订单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SynchronizeJDOrderState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDOrderState(Jinher.AMP.BTP.Deploy.JDEclpOrderJournalDTO arg);

        /// <summary>
        /// 进销存-同步京东服务单状态
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SynchronizeJDServiceState", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynchronizeJDServiceState(Jinher.AMP.BTP.Deploy.CustomDTO.SynchronizeJDServiceStateDTO arg);
    }
}
