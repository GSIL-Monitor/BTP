using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单商品
    /// </summary>
    [Serializable()]
    [DataContract]
     public  class OrderItemSDTO
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }
        /// <summary>
        /// 属性字段--订单和购物车显示  已选商品属性——规则：“次级属性ID,次级属性ID” 中间逗号分割
        /// </summary>
        [DataMemberAttribute()]
        public string AttributesString { get; set; }
        /// <summary>
        /// 商品数量--订单和购物车显示
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityNumber { get; set; }
        /// <summary>
        /// 已选商品属性列表 -- 只有在购物车和订单相关的接口里面返回
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttibuteDTO> SelectedComAttibutes { get; set; }
    }
}
