using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Jinher.JAP.BF.IService.Interface;

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    [ServiceContract]
    public interface ICache : ICommand
    {
        /// <summary>
        /// 清空App的缓存
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/RemoveAppCache")]
        [OperationContract]
        void RemoveAppCache();
    }
}
