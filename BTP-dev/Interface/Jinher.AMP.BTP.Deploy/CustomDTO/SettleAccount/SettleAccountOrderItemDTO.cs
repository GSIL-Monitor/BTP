using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算单订单项信息DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountOrderItemDTO
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 购买数量 
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// 基础佣金比例 
        /// </summary>
        [DataMember]
        public decimal? BaseCommission { get; set; }

        /// <summary>
        /// 类目佣金比例 
        /// </summary>
        [DataMember]
        public decimal? CategoryCommission { get; set; }

        /// <summary>
        /// 商品佣金比例 
        /// </summary>
        [DataMember]
        public decimal? CommodityCommission { get; set; }

        /// <summary>
        /// 商城佣金 
        /// </summary>
        [DataMember]
        public decimal PromotionAmount { get; set; }

        /// <summary>
        /// 推广佣金
        /// </summary>
        [DataMember]
        public decimal PromotionCommissionAmount { get; set; }

        ///// <summary>
        ///// 是否结算成功 
        ///// </summary>
        //[DataMember]
        //public bool Successed { get; set; }

        /// <summary>
        ///   商城易捷币抵用金额
        /// </summary>
        [DataMember]
        public decimal OrderItemYJBAmount { get; set; }

        /// <summary>
        /// 商品结算价
        /// </summary>
        [DataMember]
        public decimal SettleAmount { get; set; }
    }
}
