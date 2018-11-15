using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 请求导出数据消息体
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExportParamDTO
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 当前appId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }


        /// <summary>
        /// 多个AppIds
        /// </summary>
        [DataMember]
        public List<Guid> AppIds { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public string State { get; set; }

        /// <summary>
        /// 直接传输导出订单的ids(","分割)
        /// </summary>
        [DataMember]
        public string orderIds { get; set; }

        [DataMember]
        public List<Guid> _orderIds { get; set; }
    }
}
