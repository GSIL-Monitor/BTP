using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 后台订单查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderSearchDTO
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }


        /// <summary>
        /// AppIds
        /// </summary>
        public List<Guid> AppIds { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        [DataMember]
        public int PageIndex { get; set; }
        /// <summary>
        /// 自定义消费金额最小值
        /// </summary>
        [DataMember]
        public string PriceLow { get; set; }
        /// <summary>
        /// 自定义消费金额最大值
        /// </summary>
        [DataMember]
        public string PriceHight { get; set; }
        /// <summary>
        /// 订单编号，收货人姓名，收货人手机号，厂商名称
        /// </summary>
        [DataMember]
        public string SeacrhContent { get; set; }
        /// <summary>
        /// 下单人手机号
        /// </summary>
        [DataMember]
        public string RegisterPhone { get; set; }
        /// <summary>
        /// 下单人手机号对应的UserId
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 订单时间
        /// </summary>
        [DataMember]
        public string DayCount { get; set; }
        /// <summary>
        /// 订单状态（待自提订单状态为99）
        /// </summary>
        [DataMember]
        public string State { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public string Payment { get; set; }
        /// <summary>
        /// 自定义订单时间开始
        /// </summary>
        [DataMember]
        public DateTime? StartOrderTime { get; set; }
        /// <summary>
        /// 自定义订单时间结束
        /// </summary>
        [DataMember]
        public DateTime? EndOrderTime { get; set; }
        /// <summary>
        /// 自定义订单时间结束
        /// </summary>
        [DataMember]
        public Guid? EsAppId { get; set; }
        /// <summary>
        /// 最后支付时间(如果该字段不为空，则查询支付时间大于该时间的订单)
        /// </summary>
        [DataMember]
        public DateTime? LastPayTime { get; set; }

        /// <summary>
        ///订单来源
        /// </summary>
        [DataMember]
        public Guid? OrderSourceId { get; set; }

        /// <summary>
        /// 获取或设置 优惠活动标识(0.全部 1.易捷币抵现 2.返油卡优惠券)
        /// <para>tips: 2018-04-13 张剑 添加优惠活动筛选项</para>
        /// </summary>
        [DataMember]
        public int Marketing { get; set; }
        /// <summary>
        /// 是否为金彩支付订单查询
        /// </summary>
        [DataMember]
        public bool? Isjczf { get; set; }
    }
}
