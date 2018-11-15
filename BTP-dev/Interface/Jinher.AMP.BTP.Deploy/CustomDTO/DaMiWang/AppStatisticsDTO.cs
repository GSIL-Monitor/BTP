using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.DaMiWang
{
    /// <summary>
    /// 大米网统计信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppStatisticsDTO
    {
        /// <summary>
        /// 请求参数原值返回
        /// </summary>
        [DataMember]
        public string msg_type { get; set; }
        /// <summary>
        /// 请求参数原值返回
        /// </summary>
        [DataMember]
        public string msg_code { get; set; }
        /// <summary>
        /// 请求参数原值返回
        /// </summary>
        [DataMember]
        public string msg_client_id { get; set; }
        /// <summary>
        /// 请求参数原值返回
        /// </summary>
        [DataMember]
        public string msg_time { get; set; }
        /// <summary>
        /// 总销售额
        /// </summary>
        [DataMember]
        public string msg_salesvolume { get; set; }
        /// <summary>
        /// 总浏览量
        /// </summary>
        [DataMember]
        public string msg_browser { get; set; }
        /// <summary>
        /// 总会员数量
        /// </summary>
        [DataMember]
        public string msg_membership { get; set; }
        /// <summary>
        /// 当天会员增长量
        /// </summary>
        [DataMember]
        public string msg_membershipgrowth { get; set; }
        /// <summary>
        /// 总订单数
        /// </summary>
        [DataMember]
        public string msg_ordernumber { get; set; }
        /// <summary>
        /// 总商品数量
        /// </summary>
        [DataMember]
        public string msg_productquantity { get; set; }
        /// <summary>
        /// btp外网ip
        /// </summary>
        [DataMember]
        public string sign { get; set; }
    }
}
