using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class CommodityOrderItemDTO
    {
        /// <summary>
        /// 商品的orderitemId
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商品缩略图
        /// </summary>
        [DataMember]
        public string PicturesPath { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品的单价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        /// 商品的折扣
        /// </summary>
        [DataMember]
        public decimal? Intensity { get; set; }

        /// <summary>
        /// 商品的优惠价
        /// </summary>
        [DataMember]
        public decimal? DiscountPrice { get; set; }
        [DataMember]
        public decimal RealPrice { get; set; }


        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        [DataMember]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 商品所选属性Id
        /// </summary>
        [DataMember]
        public string ComAttributeIds { get; set; }

        /// <summary>
        /// 商品所选属性
        /// </summary>
        [DataMember]
        public string CommodityAttributes { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public int OrderState { get; set; }

        /// <summary>
        /// 是否已评价
        /// </summary>
        [DataMember]
        public bool HasReview { get; set; }
    }
}
