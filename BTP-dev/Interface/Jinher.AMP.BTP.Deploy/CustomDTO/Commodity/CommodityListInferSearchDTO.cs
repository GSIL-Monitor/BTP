using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品检索实体
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityListInferSearchDTO : SearchBase
    {
        /// <summary>
        /// 分类Id
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 最小价格
        /// </summary>
        [DataMember]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 最大价格
        /// </summary>
        [DataMember]
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 是否有货
        /// </summary>
        [DataMember]
        public bool IsHasStock { get; set; }

        /// <summary>
        /// 商家类型(0全部，2自营，1第三方)
        /// </summary>
        [DataMember]
        public int MallAppType { get; set; }

        /// <summary>
        /// 所在城市代码（空代表不限制）
        /// </summary>
        [DataMember]
        public string areaCode { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        [DataMember]
        public FieldSort4Mobile FieldSort { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public OrderType Order { get; set; }

        /// <summary>
        /// 过滤APPID
        /// </summary>
        [DataMember]
        public Guid ByAppId { get; set; }

        /// <summary>
        /// 品牌Id
        /// </summary>
        [DataMember]
        public Guid BrandId { get; set; }

        /// <summary>
        ///筛选分类Id 
        /// </summary>
        [DataMember]
        public List<Guid> CategoryIdList { get; set; }

        /// <summary>
        ///筛选品牌Id 
        /// </summary>
        [DataMember]
        public List<Guid> BrandIdList { get; set; }

        /// <summary>
        ///筛选店铺Id 
        /// </summary>
        [DataMember]
        public List<Guid> StoreIdList { get; set; }
    }
}
