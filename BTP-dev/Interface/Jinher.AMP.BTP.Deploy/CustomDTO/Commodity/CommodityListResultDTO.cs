using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品列表查询结果
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityListResultDTO : ResultDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<CommodityListCDTO> CommodityList { get; set; }
    }
    /// <summary>
    /// 商品列表查询dto
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityListSearchDTO : SearchBase
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid? AppId { get; set; }

        /// <summary>
        /// app下的商品分类Id(空为不限，"00000000-0000-0000-0000-000000000000"为未分类) 
        /// </summary>
        [DataMember]
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// 是否有库存
        /// </summary>
        [DataMember]
        public bool IsHasStock { get; set; }

        /// <summary>
        /// 最小价格(0为不限)
        /// </summary>
        [DataMember]
        public decimal? MinPrice { get; set; }
        /// <summary>
        /// 最大价格(0为不限)
        /// </summary>
        [DataMember]
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// 排序方式 0:默认，1：价格，2：销量，3：新品
        /// </summary>
        [DataMember]
        public int FieldSort { get; set; }
        /// <summary>
        /// 所在地区
        /// </summary>
        [DataMember]
        public string areaCode { get; set; }

        /// <summary>
        /// 当前用户id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 优惠券模板Id
        /// </summary>
        [DataMember]
        public Guid CouponTemplateId { get; set; }

        /// <summary>
        /// 优惠券类型（1-优惠券 2-抵用券）
        /// </summary>
        [DataMember]
        public int? couponType { get; set; }

        /// <summary>
        /// 应用Id列表
        /// </summary>
        [DataMember]
        public List<Guid> AppIdList { get; set; }

        /// <summary>
        /// app下的商品分类Id列表(空为不限，"00000000-0000-0000-0000-000000000000"为未分类) 
        /// </summary>
        [DataMember]
        public List<Guid> CategoryIdList { get; set; }

        /// <summary>
        /// app品牌Id列表(空为不限，"00000000-0000-0000-0000-000000000000"为未分类) 
        /// </summary>
        [DataMember]
        public List<Guid> BrandList { get; set; }

        /// <summary>
        /// 搜索字段
        /// </summary>
        [DataMember]
        public string FieldWhere { get; set; }

        /// <summary>
        /// 商城类型
        /// </summary>
        [DataMember]
        public int? MallAppType { get; set; }
    }



}
