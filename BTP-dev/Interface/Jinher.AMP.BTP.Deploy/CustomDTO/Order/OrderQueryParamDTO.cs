using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class OrderQueryParamDTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// PageIndex
        /// </summary>
        [DataMemberAttribute()]
        public int PageIndex { get; set; }
        /// <summary>
        /// PageSize
        /// </summary>
        [DataMemberAttribute()]
        public int PageSize { get; set; }
        /// <summary>
        /// state 订单状态0：未付款|1:未发货|2:已发货|3:交易成功|-1：失败|空为不限
        /// </summary>
        [DataMemberAttribute()]
        public int? State { get; set; }
        /// <summary>
        /// EsAppId区分App
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }



    }
}
