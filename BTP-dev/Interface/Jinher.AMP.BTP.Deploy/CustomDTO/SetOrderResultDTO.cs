using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 返回结果 --- 厂家直销
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SetOrderResultDTO
    {
        /// <summary>
        /// 返回代号
        /// </summary>
        [DataMemberAttribute()]
        public int ResultCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        [DataMemberAttribute()]
        public string Message { get; set; }
        /// <summary>
        /// 主订单Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid MainOrderId { get; set; }
        /// <summary>
        /// OrderInfo
        /// </summary>
        [DataMemberAttribute()]
        public List<MyOrderResultDTO> OrderInfo { get; set; }
        /// <summary>
        /// 未支付超时时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ExpireTime { get; set; }
        /// <summary>
        /// 在线支付包括支付宝与U付时,要跳转到的支付页面
        /// </summary>
        [DataMemberAttribute()]
        public string PayUrl { get; set; }
    }



}