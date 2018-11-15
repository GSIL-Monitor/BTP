using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class CommodityForOrderDTO : CommoditySDTO
    {
        /// <summary>
        /// 是否已经评价 -- 订单详情页使用
        /// </summary>
        [DataMember]
        public bool HasReview { get; set; }

        /// <summary>
        /// 已选商品属性列表 -- 在订单相关接口中使用
        /// </summary>
        [DataMember]
        public List<SimpleAttributeDTO> SelectedAttributes { get; set; }

        /// <summary>
        /// 订单ID --获取订单商品时
        /// </summary>
        [DataMemberAttribute()]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 属性字段--订单和购物车显示  已选商品属性——规则：“次级属性ID,次级属性ID” 中间逗号分割
        /// </summary>
        [DataMemberAttribute()]
        public string AttributesString { get; set; }
        
    }
}
