using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    ///  订单物流详情
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNOrderLogistStatusResDTO
    {
        /// <summary>
        /// 物流状态
        /// </summary>
        [DataMember]
        public string OperateState { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public string OperateTime { get; set; }
    }
}
