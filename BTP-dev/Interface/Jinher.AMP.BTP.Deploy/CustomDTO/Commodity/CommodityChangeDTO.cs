using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class BigAutocomplete
    {
        public string title { get; set; }
        public string result { get; set; }
        public string code { get; set; }
    }
    //统计数据
    [Serializable]
    [DataContract]
    public class totalNum
    {
        /// <summary>
        /// 商品总数
        /// </summary>
        [DataMember]
        public int totalGoods { get; set; }
        /// <summary>
        ///  在售中
        /// </summary>
        [DataMember]
        public int selling { get; set; }
        /// <summary>
        /// 已下架
        /// </summary>
        [DataMember]
        public int soldout { get; set; }
        /// <summary>
        ///  已删除
        /// </summary>
        [DataMember]
        public int isdel { get; set; }
    }
    //     用户id姓名
    [Serializable]
    [DataContract]
    public class UserNameDTO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        ///  用户名称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
    }
    /// <summary>
    /// 商品信息更改表
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityChangeDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 商品主键
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        [DataMember]
        public DateTime SubOn { get; set; }
        /// <summary>
        /// 发布人id
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// 发布人ids
        /// </summary>
        [DataMember]
        public List<Guid> SubIdList { get; set; }
        /// <summary>
        /// 发布人名称
        /// </summary>
        [DataMember]
        public string SubName { get; set; }
        /// <summary>
        /// 发布人code
        /// </summary>
        [DataMember]
        public string SubCode { get; set; }
        /// <summary>
        /// 商品编号数字
        /// </summary>
        [DataMember]
        public int No_Number { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }
        /// <summary>
        /// 商品图片路径
        /// </summary>
        [DataMember]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 商品状态：上架=0，未上架=1
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 是否删除：删除=TRUE,未删除=false
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        [DataMember]
        public string StateName { get; set; }
        /// <summary>
        /// 店铺id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }
        /// 供应商类型（0-自营他配；1-第三方；2-自营自配）
        /// </summary>
        [DataMember]
        public string SupplierTypeName { get; set; }
        /// <summary>
        /// 商品编号字符
        /// </summary>
        [DataMember]
        public string No_Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TotalCollection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TotalReview { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Salesvolume { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// 最后上架时间
        /// </summary>
        [DataMember]
        public DateTime? GroundTime { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMember]
        public string ComAttribute { get; set; }
        /// <summary>
        /// 商品类目信息
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }
        /// <summary>
        /// 商品类目信息
        /// </summary>
        [DataMember]
        public int SortValue { get; set; }
        /// <summary>
        ///运费模板
        /// </summary>
        [DataMember]
        public Guid? FreightTemplateId { get; set; }
        /// <summary>
        ///重量
        /// </summary>
        [DataMember]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 商品市场价
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        [DataMember]
        public byte PricingMethod { get; set; }
        /// <summary>
        /// 销售区域
        /// </summary>
        [DataMember]
        public string SaleAreas { get; set; }
        /// <summary>
        /// 分成比例
        /// </summary>
        [DataMember]
        public decimal? SharePercent { get; set; }
        /// <summary>
        /// 是否支持自提
        /// </summary>
        [DataMember]
        public int IsEnableSelfTake { get; set; }
        /// <summary>
        /// 商品类型:0实体商品；1虚拟商品
        /// </summary>
        [DataMember]
        public int CommodityType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string HtmlVideoPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MobileVideoPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string VideoPic { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string VideoName { get; set; }
        /// <summary>
        /// 推广分成比例
        /// </summary>
        [DataMember]
        public decimal? SpreadPercent { get; set; }
        /// <summary>
        /// 积分比例
        /// </summary>
        [DataMember]
        public decimal? ScoreScale { get; set; }
        /// <summary>
        /// 七天无理由退货
        /// </summary>
        [DataMember]
        public bool? IsAssurance { get; set; }
        /// <summary>
        /// 规格参数
        /// </summary>
        [DataMember]
        public string TechSpecs { get; set; }
        /// <summary>
        /// 售后说明
        /// </summary>
        [DataMember]
        public string SaleService { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool? IsReturns { get; set; }

        [DataMember]
        public bool? Isnsupport { get; set; }

        /// <summary>
        /// 服务项设置Id
        /// </summary>
        [DataMember]
        public string ServiceSettingId { get; set; }
        /// <summary>
        /// 商品类型:0实体商品；1虚拟商品
        /// </summary>
        [DataMember]
        public string CommodityTypeName { get; set; }
        /// <summary>
        /// 积分抵用上限
        /// </summary>
        [DataMember]
        public decimal? ScorePercent { get; set; }
        /// <summary>
        ///关税
        /// </summary>
        [DataMember]
        public decimal? Duty { get; set; }
        /// <summary>
        /// 销项税
        /// </summary>
        [DataMember]
        public decimal? TaxRate { get; set; }
        /// <summary>
        /// 税收分类编码
        /// </summary>
        [DataMember]
        public string TaxClassCode { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// 商品进项税
        /// </summary>
        [DataMember]
        public decimal? InputRax { get; set; }
        /// <summary>
        /// 商品条码
        /// </summary>
        [DataMember]
        public string Barcode { get; set; }
        /// <summary>
        /// 京东商品编号
        /// </summary>
        [DataMember]
        public string JDCode { get; set; }
        /// <summary>
        /// 进货价
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// 商品类型：实物=null或0，易捷卡密=1
        /// </summary>
        [DataMember]
        public int? Type { get; set; }
        /// <summary>
        /// 活动编码
        /// </summary>
        [DataMember]
        public string YJCouponActivityId { get; set; }
        /// <summary>
        /// 类型编码
        /// </summary>
        [DataMember]
        public string YJCouponType { get; set; }
        /// <summary>
        /// 修改人id
        /// </summary>
        [DataMember]
        public Guid? ModifiedId { get; set; }
        /// <summary>
        /// 修改人ids
        /// </summary>
        [DataMember]
        public List<Guid> ModifiedIdList { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        [DataMember]
        public string ModifiedName { get; set; }
        /// <summary>
        /// 修改人code
        /// </summary>
        [DataMember]
        public string ModifiedCode { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [DataMember]
        public int AuditState { get; set; }
        /// <summary>
        /// 搜索发布开始时间
        /// </summary>
        [DataMember]
        public string SubStarTime { get; set; }
        /// <summary>
        /// 搜索发布结束时间
        /// </summary>
        [DataMember]
        public string SubEndTime { get; set; }
        /// <summary>
        /// 搜索修改开始时间
        /// </summary>
        [DataMember]
        public string ModStarTime { get; set; }
        /// <summary>
        /// 搜索修改结束时间
        /// </summary>
        [DataMember]
        public string ModEndTime { get; set; }
        /// <summary>
        /// 供应商下所有的商铺id集合
        /// </summary>
        [DataMember]
        public List<Guid> AppidsList { get; set; }
        /// <summary>
        /// 馆下所有的商铺id集合及自己
        /// </summary>
        [DataMember]
        public List<Guid> Appids { get; set; }
    }
}
