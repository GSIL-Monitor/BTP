using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable]
    [DataContract]
    public class CommodityCookie
    {
        [DataMember]
        public string Picture { get; set; }
        [DataMember]
        public string CommodityName { get; set; }
        [DataMember]
        public string CommodityStock { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string CommodityCode { get; set; }
        [DataMember]
        public string CommodityPrice { get; set; }
        [DataMember]
        public string ImgList { get; set; }
        [DataMember]
        public string listImgShowSrcString { get; set; }
        [DataMember]
        public string CommodityDetails { get; set; }
        [DataMember]
        public string TechSpecs { get; set; }
        [DataMember]
        public string SaleService { get; set; }

        /// <summary>
        /// 商品类目
        /// </summary>
        [DataMember]
        public string CommoditCategory { get; set; }
        /// <summary>
        /// 商品类目名称
        /// </summary>
        [DataMember]
        public string CommoditCategoryName { get; set; }
        [DataMember]
        public string CommoditySizeids { get; set; }
        [DataMember]
        public string CommodityColorids { get; set; }
        [DataMember]
        public string CommoditySizeName { get; set; }
        [DataMember]
        public string CommodityColorName { get; set; }
        [DataMember]
        public string Hidpic { get; set; }
        [DataMember]
        public string SelectAttr { get; set; }
        [DataMember]
        public string FristAttrValueList { get; set; }
        [DataMember]
        public string TwoAttrValueList { get; set; }
        [DataMember]
        public string InputAttrValue { get; set; }
        [DataMember]
        public string FreightId { get; set; }
        [DataMember]
        public string FreightName { get; set; }
        [DataMember]
        public string CommodityMarketPrice { get; set; }
        [DataMember]
        public int IsEnableSelfTake { get; set; }
        /// <summary>
        /// 商品类型:0实体商品；1虚拟商品；
        /// </summary>
         [DataMember]
        public string  CommodityType { get; set; }
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
        /// 0按件计价，1按重量计价。
        /// </summary>
        [DataMember]
        public string PricingMethod { get; set; }
        /// <summary>
        /// 重量(单位：kg)
        /// </summary>
        [DataMember]
        public string Weight { get; set; }
        /// <summary>
        /// 销售地区
        /// </summary>
        [DataMember]
        public string SaleAreas { get; set; }
        /// <summary>
        /// 销售地区的名称
        /// </summary>
        [DataMember]
        public string SaleAreasText { get; set; }

        /// <summary>
        /// 餐盒价格
        /// </summary>
        [DataMember]
        public string CommodityBoxPrice { get; set; }

        /// <summary>
        /// 餐盒数量
        /// </summary>
        [DataMember]
        public string CommodityBoxCount { get; set; }
        /// <summary>
        /// 商品关税
        /// </summary>
        [DataMember]
        public string CommodityDuty { get; set; }

        /// <summary>
        /// 积分比例
        /// </summary>
        [DataMember]
        public string ScoreScale { get; set; }

        /// <summary>
        /// 商品销项税
        /// </summary>
        [DataMember]
        public string CommodityTaxRate { get; set; }

        /// <summary>
        /// 商品进项税
        /// </summary>
        [DataMember]
        public string CommodityInputTax { get; set; }

        /// <summary>
        /// 税收分类编码
        /// </summary>  
        [DataMember]
        public string TaxClassCode { get; set; }

        /// <summary>
        /// 计价方式
        /// </summary>  
        [DataMember]
        public string Unit { get; set; }

        /// <summary>
        /// 商城品类
        /// </summary>
        [DataMember]
        public string CommoditInnerCategory { get; set; }
        /// <summary>
        /// 商城品类名称
        /// </summary>
        [DataMember]
        public string CommoditInnerCategoryName { get; set; }
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
        public string CostPrice { get; set; }

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
        /// 二期系统商品编码
        /// </summary>
        [DataMember]
        public string ErQiCode { get; set; }

        /// <summary>
        /// 品牌Id
        /// </summary>
        [DataMember]
        public string BrandId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }

        /// <summary>
        /// 入驻店铺分类
        /// </summary>
        [DataMember]
        public string EsCategory { get; set; }

        /// <summary>
        /// 入驻店铺分类名称
        /// </summary>
        [DataMember]
        public string EsCategoryName { get; set; }        

    }
}
