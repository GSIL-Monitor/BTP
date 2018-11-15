using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// SaveSpreaderAndBuyerWxRel\UpdateOrderSpreader接口参数类。
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SpreaderAndBuyerWxDTO
    {
        /// <summary>
        /// 推广码
        /// </summary>
        [DataMemberAttribute()]
        public Guid SpreadCode { get; set; }

        /// <summary>
        /// 买家微信OpenId.
        /// </summary>
        [DataMemberAttribute()]
        public string WxOpenId { get; set; }

        /// <summary>
        ///买家id
        /// </summary>
        [DataMemberAttribute()]
        public Guid BuyerId { get; set; }

        /// <summary>
        ///订单id
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }
        /// <summary>
        ///推广主id
        /// </summary>
        [DataMemberAttribute()]
        public Guid SpreaderId { get; set; }
    }
}
