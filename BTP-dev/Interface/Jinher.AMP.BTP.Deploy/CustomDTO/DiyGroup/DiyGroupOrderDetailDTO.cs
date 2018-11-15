using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 拼团订单信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class DiyGroupOrderDetailDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// SubId
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public Guid OrderId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderCode { get; set; }
        /// <summary>
        /// 角色 0：团长，1：团员
        /// </summary>
        [DataMember]
        public Int32 Role { get; set; }
        /// <summary>
        /// 团Id
        /// </summary>
        [DataMember]
        public Guid DiyGroupId { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        [DataMember]
        public string SubCode { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 团订单状态 0：待付款，1付款完成 
        /// </summary>
        [DataMember]
        public Int32 State { get; set; }
        /// <summary>
        /// 拼团人帐号
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 拼团人头像
        /// </summary>
        [DataMember]
        public string UserPicture { get; set; }
        /// <summary>
        /// 拼团价格
        /// </summary>
        [DataMember]
        public decimal DiyGroupPrice { get; set; }
    }
}
