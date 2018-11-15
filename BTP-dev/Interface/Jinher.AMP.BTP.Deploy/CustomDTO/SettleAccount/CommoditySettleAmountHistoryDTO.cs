using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品历史结算价
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommoditySettleAmountHistoryDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        [DataMember]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品结算价
        /// </summary>
        [DataMember]
        public decimal SettlePrice { get; set; }

        /// <summary>
        /// 是否有一级属性
        /// </summary>
        [DataMember]
        public bool HasAttributes { get; set; }

        /// <summary>
        /// 是否有二级属性
        /// </summary>
        [DataMember]
        public bool HasSecondAttribute { get; set; }

        /// <summary>
        /// 一级属性名
        /// </summary>
        [DataMember]
        public string AttributeName { get; set; }

        /// <summary>
        /// 二级属性名
        /// </summary>
        [DataMember]
        public string SecondAttributeName { get; set; }

        /// <summary>
        /// 属性数据
        /// </summary>
        [DataMember]
        public List<CommodityAttribute> Attributes { get; set; }
    }
}
