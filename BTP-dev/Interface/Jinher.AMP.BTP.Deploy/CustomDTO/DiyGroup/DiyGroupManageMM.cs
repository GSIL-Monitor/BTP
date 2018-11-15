using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class DiyGroupManageMM
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMemberAttribute()]
        public string DiyGroupOrderCode { get; set; }
        /// <summary>
        /// 参团人员编号
        /// </summary>
        [DataMemberAttribute()]
        public string DiyGroupPersonCode { get; set; }
        /// <summary>
        /// 参团人员角色
        /// </summary>
        [DataMemberAttribute()]
        public int DiyGroupPersonRole { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid DiyGroupOrderId { get; set; }
        /// <summary>
        /// 拼团ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid DiyGroupId { get; set; }
        /// <summary>
        /// 商品缩略图
        /// </summary>
        [DataMember]
        public string Pic { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 拼团数量
        /// </summary>
        [DataMemberAttribute()]
        public int DiyNumber { get; set; }
        /// <summary>
        /// 拼团价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiyGroupPrice { get; set; }
        /// <summary>
        /// 拼团商品属性
        /// </summary>
        [DataMember]
        public string attributes { get; set; }
    }
}
