using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class FCateringSettingCDTO
    {
        /// <summary>
        /// 表Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [DataMember]
        public string Specification { get; set; }
        /// <summary>
        /// 起送金额
        /// </summary>
        [DataMember]
        public decimal DeliveryAmount { get; set; }
        /// <summary>
        /// 配送范围
        /// </summary>
        [DataMember]
        public double DeliveryRange { get; set; }
        /// <summary>
        /// 配送费
        /// </summary>
        [DataMember]
        public decimal DeliveryFee { get; set; }
        /// <summary>
        /// 最大使用优惠券
        /// </summary>
        [DataMember]
        public decimal MostCoupon { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        [DataMember]
        public Guid StoreId { get; set; }
        /// <summary>
        /// 满?免配送费
        /// </summary>
        [DataMember]
        public Nullable<decimal> FreeAmount { get; set; }
        /// <summary>
        /// 运费打折开始时间
        /// </summary>
        [DataMember]        
        public Nullable<System.DateTime> DeliveryFeeStartT { get; set; }
        /// <summary>
        /// 运费打折结束时间
        /// </summary>
        [DataMember]        
        public Nullable<System.DateTime> DeliveryFeeEndT { get; set; }
        /// <summary>
        /// 运费折扣
        /// </summary>
        [DataMember]        
        public Nullable<double> DeliveryFeeDiscount { get; set; }
    }
}
