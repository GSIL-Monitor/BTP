using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 物流路由信息扩展类。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderExpressRouteExtendDTO : OrderExpressRouteDTO
    {
        /// <summary>
        /// 路由详细列表
        /// </summary>
        [DataMember]
        public List<ExpressTraceDTO> Traces { get; set; }
    }
}
