using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 拼团的订单列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class DiyGroupOrderListDTO
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
        /// 提交人
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
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
        ///  应用名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }
        /// <summary>
        /// 订单价格
        /// </summary>
        [DataMemberAttribute()]
        public Decimal? Price { get; set; }
       
        /// <summary>
        /// 拼团价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiyGroupPrice { get; set; }
        /// <summary>
        /// 拼团订单提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime DiyOrderSubTime { get; set; }
        /// <summary>
        /// 拼团状态
        /// </summary>
        [DataMemberAttribute()]
        public int DiyGroupState { get; set; }
       
        /// <summary>
        /// 拼团订单信息列表
        /// </summary>
        [DataMember]
        public List<DiyGroupManageMM> OrderDataList { get; set; }
        /// <summary>
        /// 成团人数
        /// </summary>
        [DataMember]
        public Int32 GroupMinVolume { get; set; }
        /// <summary>
        /// 参团人数
        /// </summary>
        [DataMember]
        public Int32 JoinNumber { get; set; }
        /// <summary>
        /// 拼团商品属性
        /// </summary>
        [DataMember]
        public string attributes { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }        
    }
}
