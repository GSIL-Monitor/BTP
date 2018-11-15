using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品赠品
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodiyPresentDTO
    {
        /// <summary>
        /// 赠品标题
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 商品单次最少购买数量
        /// </summary>
        [DataMember]
        public int Limit { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 参加活动的SKUId
        /// </summary>
        [DataMember]
        public List<Guid> CommodityStockIds { get; set; }

        /// <summary>
        /// 是否全部赠送
        /// </summary>
        [DataMember]
        public bool IsAll { get; set; }

        /// <summary>
        /// 赠品商品
        /// </summary>
        [DataMember]
        public List<CommodiyPresentItem> Items { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommodiyPresentItem
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 商品库存ID
        /// </summary>
        [DataMember]
        public Guid StockId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMember]
        public string Pic { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }

        /// <summary>
        /// 商品SKU
        /// </summary>
        [DataMember]
        public List<ComAttributeDTO> SKU { get; set; }
    }
}
