using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class CommodityAndCategoryDTO : BusinessDTO
    {
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
        /// 颜色ID串
        /// </summary>
        [DataMember]
        public string ColorIds { get; set; }
        /// <summary>
        /// 颜色名称串
        /// </summary>
        [DataMember]
        public string ColorNames { get; set; }
        /// <summary>
        /// 尺寸ID串
        /// </summary>
        [DataMember]
        public string SizeIds { get; set; }
        /// <summary>
        /// 尺寸名称串
        /// </summary>
        [DataMember]
        public string SizeNames { get; set; }
        /// <summary>
        /// 商品分类ID串
        /// </summary>
        [DataMember]
        public string CategoryIds { get; set; }
        /// <summary>
        /// 商品分类名称串
        /// </summary>
        [DataMember]
        public string CateNames { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 商品状态（是否上架）
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        public bool IsDel { get; set; }
        /// <summary>
        /// AppID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商品编号字符
        /// </summary>
        [DataMember]
        public string No_Code { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 类别路径
        /// </summary>
        [DataMember]
        public string CategoryPath { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMember]
        public List<ComAttibuteDTO> comAttributes { get; set; }
        /// <summary>
        /// 图片列表地址串
        /// </summary>
        [DataMember]
        public List<string> Picturelist { get; set; }
        /// <summary>
        /// 次级属性ID串
        /// </summary>
        [DataMember]
        public string AttributeIds { get; set; }

        /// <summary>
        /// 组合属性列表  //zgx-modify
        /// </summary>
        [DataMember]
        public List<CommodityStockDTO> ComAttributes { get; set; }

        /// <summary>
        /// 属性名称串
        /// </summary>
        [DataMember]
        public string AttrName { get; set; }

        /// <summary>
        /// 属性值名称串
        /// </summary>
        [DataMember]
        public string AttrValueNames { get; set; }

        /// <summary>
        /// 属性Id称串
        /// </summary>
        [DataMember]
        public string AttrId { get; set; }

        /// <summary>
        /// 属性值ID称串
        /// </summary>
        [DataMember]
        public string AttrValueIds { get; set; }
        /// <summary>
        /// 运费模板ID
        /// </summary>
        [DataMember]
        public string FreightId { get; set; }
        /// <summary>
        /// 运费模板名称
        /// </summary>
        [DataMember]
        public string FreightName { get; set; }
        /// <summary>
        /// 关联商品集合
        /// </summary>
        [DataMember]
        public string RelaCommodityList { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 是否支持自提
        /// </summary>
        [DataMember]
        public int IsEnableSelfTake { get; set; }

        /// <summary>
        /// 0按件计价，1按重量计价。
        /// </summary>
        [DataMember]
        public byte PricingMethod { get; set; }
        /// <summary>
        /// 重量(单位：kg)
        /// </summary>
        [DataMember]
        public decimal Weight { get; set; }
        /// <summary>
        /// 销售地区
        /// </summary>
        [DataMember]
        public string SaleAreas { get; set; }
        /// <summary>
        /// 手机视频Url
        /// </summary>
        [DataMember]
        public string VideoUrl { get; set; }
        /// <summary>
        /// 网页视频Url
        /// </summary>
        [DataMember]
        public string VideoclientUrl { get; set; }
        /// <summary>
        /// 视频图片地址
        /// </summary>
        [DataMember]
        public string VideoPicUrl { get; set; }
        /// <summary>
        /// 视频名称
        /// </summary>
        [DataMember]
        public string VideoName { get; set; }
        /// <summary>
        /// 商品类型:0实体商品；1虚拟商品；
        /// </summary>
        [DataMember]
        public int CommodityType { get; set; }
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
        /// 前端提交来源：0：默认，1：舌尖在线
        /// </summary>
        [DataMember]
        public int From { get; set; }

        /// <summary>
        /// 餐盒价格
        /// </summary>
        [DataMember]
        public decimal CommodityBoxPrice { get; set; }

        /// <summary>
        /// 餐盒数量
        /// </summary>
        [DataMember]
        public int CommodityBoxCount { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        [DataMember]
        public decimal? CommodityDuty { get; set; }

        /// <summary>
        /// 积分比例
        /// </summary>
        [DataMember]
        public decimal? ScoreScale { get; set; }

        /// <summary>
        /// 商品销项税
        /// </summary>
        [DataMember]
        public decimal? CommodityTaxRate { get; set; }

        /// <summary>
        /// 商品进项税
        /// </summary>
        [DataMember]
        public decimal? CommodityInputTax { get; set; }

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
        /// 商城类别路径
        /// </summary>
        [DataMember]
        public string InnerCategoryPath { get; set; }
        /// <summary>
        /// 商品商城分类ID串
        /// </summary>
        [DataMember]
        public string InnerCategoryIds { get; set; }
        /// <summary>
        /// 商品商城分类名称串
        /// </summary>
        [DataMember]
        public string InnerCateNames { get; set; }
        /// <summary>
        /// 商品条码
        /// </summary>
        [DataMember]
        public string BarCode { get; set; }
        /// <summary>
        /// 京东商品编码
        /// </summary>
        [DataMember]
        public string JDCode { get; set; }
        /// <summary>
        /// 进货价
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// 是否七天无理由退货
        /// </summary>
        [DataMember]
        public bool IsAssurance { get; set; }
        /// <summary>
        /// 是否七天无理由退货
        /// </summary>
        [DataMember]
        public bool IsReturns { get; set; }

        /// <summary>
        /// 是否支持七天无理由退货
        /// </summary>
        [DataMember]
        public bool Isnsupport { get; set; }
        /// <summary>
        /// 服务项设置
        /// </summary>
        [DataMember]
        public string ServiceSettingId { get; set; }
        /// <summary>
        /// 规格参数
        /// </summary>
        [DataMember]
        public string TechSpecs { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        [DataMember]
        public int Salesvolume { get; set; }
        /// <summary>
        /// 售后说明
        /// </summary>
        [DataMember]
        public string SaleService { get; set; }
        /// <summary>
        /// 规格设置
        /// </summary>
        [DataMember]
        public string Specifications { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        [DataMember]
        public int TotalCollection { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        [DataMember]
        public int TotalReview { get; set; }
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
        /// 分成比例
        /// </summary>
        [DataMember]
        public decimal? SharePercent { get; set; }
        /// <summary>
        /// 积分抵用上限
        /// </summary>
        [DataMember]
        public decimal? ScorePercent { get; set; }
        /// <summary>
        /// 推广分成比例
        /// </summary>
        [DataMember]
        public decimal? SpreadPercent { get; set; }
        /// <summary>
        /// 审核id
        /// </summary>
        [DataMember]
        public Guid AuditId { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [DataMember]
        public int AuditState { get; set; }
        /// <summary>
        /// 处理方式名称
        /// </summary>
        [DataMember]
        public string AuditStateName { get; set; }        
        /// <summary>
        /// 馆id
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        ///提交审核时间
        /// </summary>
        [DataMember]
        public DateTime ApplyTime { get; set; }
        /// <summary>
        ///审核时间
        /// </summary>
        [DataMember]
        public DateTime? AuditTime { get; set; }
        /// <summary>
        ///审核人Id
        /// </summary>
        [DataMember]
        public Guid? AuditUserId { get; set; }
        /// <summary>
        ///审核人名称
        /// </summary>
        [DataMember]
        public string AuditUserName { get; set; }
        /// <summary>
        ///审核人code
        /// </summary>
        [DataMember]
        public string AuditUserCode { get; set; } 
        //  Action
        /// <summary>
        /// 审核意见
        /// </summary>
        [DataMember]
        public string AuditRemark { get; set; }
        /// <summary>
        /// 编辑行为  对应OperateTypeEnum
        /// </summary>
        [DataMember]
        public int Action { get; set; }
        /// <summary>
        /// 编辑行为  对应OperateTypeEnum
        /// </summary>
        [DataMember]
        public string ActionName { get; set; }
        /// <summary>
        ///供应商名称
        /// </summary>
        [DataMember]
        public string SupplyName { get; set; }
        /// <summary>
        /// App名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 京东售价
        /// </summary>
        [DataMember]
        public decimal? JdPrice { get; set; }
        /// <summary>
        /// 京东进货价
        /// </summary>
        [DataMember]
        public decimal? JdCostPrice { get; set; }
        /// <summary>
        /// 当前售价毛利率
        /// </summary>
        [DataMember]
        public string NowPriceProfit { get; set; }
        /// <summary>
        /// 最新售价毛利率
        /// </summary>
        [DataMember]
        public string NewPriceProfit { get; set; }
        /// <summary>
        /// 当前进价毛利率
        /// </summary>
        [DataMember]
        public string NowCostPriceProfit { get; set; }
        /// <summary>
        /// 最新进价毛利率
        /// </summary>
        [DataMember]
        public string NewCostPriceProfit { get; set; }
        /// <summary>
        /// 京东商品状态
        /// </summary>
        [DataMember]
        public int JdStatus { get; set; }
        /// <summary>
        /// 京东商品状态名称
        /// </summary>
        [DataMember]
        public string JdStatusName { get; set; }
        /// <summary>
        ///更新后商品最新进价
        /// </summary>
        [DataMember]
        public decimal? NewCostPrice { get; set; }
        /// <summary>
        ///最新的商品价格
        /// </summary>
        [DataMember]
        public decimal? NewPrice { get; set; }
        /// <summary>
        ///库存表ID
        /// </summary>
        [DataMember]
        public Guid ComStockId { get; set; }
        /// <summary>
        /// 二期系统商品编码
        /// </summary>
        [DataMember]
        public string ErQiCode { get; set; }
        /// <summary>
        /// 品牌Id
        /// </summary>
        [DataMember]
        public Guid BrandId { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }

        /// <summary>
        /// 入驻店铺所在分类
        /// </summary>
        [DataMember]
        public string EsCategory { get; set; }

        /// <summary>
        /// 入驻店铺所在名称
        /// </summary>
        [DataMember]
        public string EsCagetoryName { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class CommodityAndTemplateDTO : BusinessDTO
    {
        /// <summary>
        /// 商品标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodName { get; set; }

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
        /// 颜色ID串
        /// </summary>
        [DataMember]
        public string ColorIds { get; set; }
        /// <summary>
        /// 颜色名称串
        /// </summary>
        [DataMember]
        public string ColorNames { get; set; }
        /// <summary>
        /// 尺寸ID串
        /// </summary>
        [DataMember]
        public string SizeIds { get; set; }
        /// <summary>
        /// 尺寸名称串
        /// </summary>
        [DataMember]
        public string SizeNames { get; set; }

        /// <summary>
        /// 商品状态（是否上架）
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 商品运费模板ID
        /// </summary>
        [DataMember]
        public Guid? FreightTemplateId
        {
            get;
            set;
        }

        /// <summary>
        /// 商品编号字符
        /// </summary>
        [DataMember]
        public string No_Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <LongDescription>
        /// 是否包邮(0不包邮 1包邮)
        /// </LongDescription>
        [DataMember]
        public bool? IsFreeExp { get; set; }

        /// <summary>
        /// 物流费用
        /// </summary>
        [DataMember]
        public decimal? FirstCountPrice { get; set; }


        /// <summary>
        /// 退货物流运费模板ID
        /// </summary>
        [DataMember]
        public Guid? RefundTemplateId
        {
            get;
            set;
        }
        

        /// <summary>
        /// 退货物流费用
        /// </summary>
        [DataMember]
        public decimal? RefundFreightPrice { get; set; }

        /// <summary>
        /// SupId
        /// </summary>
        [DataMember]
        public string SupId { get; set; }

    }
}
