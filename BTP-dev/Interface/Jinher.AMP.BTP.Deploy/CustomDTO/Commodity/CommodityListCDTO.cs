using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 前端商品列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityListCDTO
    {
        /// <summary>
        /// 排序号
        /// </summary>
        [DataMember]
        public double SetCategorySort { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 商品店铺Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid appId { get; set; }
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
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int Stock { get; set; }
        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMemberAttribute()]
        public decimal DiscountPrice { get; set; }

        /// <summary>
        /// 每人限购
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyEach { get; set; }

        /// <summary>
        /// 促销商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int? LimitBuyTotal { get; set; }

        /// <summary>
        /// 剩余促销商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int? SurplusLimitBuyTotal { get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否进行中的众筹
        /// </summary>
        [DataMember]
        public bool IsActiveCrowdfunding { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 进货价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMemberAttribute()]
        public string ComAttribute { get; set; }
        /// <summary>
        /// 是否多属性商品
        /// </summary>
        [DataMemberAttribute()]
        public bool IsMultAttribute { get; set; }

        /// <summary>
        /// 商品分成比例。
        /// </summary>
        [DataMemberAttribute()]
        public decimal SharePercent { get; set; }

        /// <summary>
        /// 商品分类id
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 商品组合属性集合 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityAttrStockDTO> CommodityStocks { get; set; }
        /// <summary>
        /// 商品属性列表 -- 商品列表返回的时候这个列表为空
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttributeDTO> ComAttibutes { get; set; }


        /// <summary>
        /// 包装规格设置
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specifications { get; set; }

        /// <summary>
        /// 商品现价（展示用）
        /// </summary>
        [DataMember]
        public decimal RealPrice { get; set; }
        /// <summary>
        /// 商品原价（展示用）
        /// </summary>
        [DataMember]
        public decimal? OriPrice { get; set; }

        /// <summary>
        /// 商品使用的活动类型
        /// </summary>
        [DataMember]
        public ComPromotionStatusEnum ComPromotionStatusEnum { get; set; }
        /// <summary>
        /// 活动类型（0，限时打折；1，秒杀；2，预约；9999，表示无活动）
        /// </summary>
        [DataMember]
        public int PromotionType { get; set; }

        /// <summary>
        /// 活动类型New（ 0：普通活动，1：秒杀，2：预售，3：拼团，5：预售(不用预约)，6：赠品，7：套装）
        /// </summary>
        [DataMember]
        public int? PromotionTypeNew { get; set; }

        /// <summary>
        /// 活动类型New（ 0：普通活动，1：秒杀，2：预售，3：拼团，5：预售(不用预约)，6：赠品，7：套装）
        /// </summary>
        [DataMember]
        public string PromotionTypeNews { get; set; }
        /// <summary>
        ///商品标签（ 0：普通活动，1：秒杀，2：预售，3：拼团，5：预售(不用预约)，6：赠品，7：套装）
        /// </summary>
        [DataMember]
        public List<string> Tags { get; set; }

        /// <summary>
        ///商品标签（ 0：普通活动，1：秒杀，2：预售，3：拼团，5：预售(不用预约)，6：赠品，7：套装）
        /// </summary>
        [DataMember]
        public List<string> TagsSimple { get; set; }

        /// <summary>
        /// 商品属性（1无属性，2单属性，3，两组属性组合）
        /// </summary>
        [DataMember]
        public int ComAttrType { get; set; }

        /// <summary>
        /// 商品积分抵用比例。
        /// </summary>
        [DataMemberAttribute()]
        public decimal ScorePercent { get; set; }

        public CommodityListCDTO Clone()
        {
            return this.MemberwiseClone() as CommodityListCDTO;
        }

        /// <summary>
        /// 推广商品分成比例。
        /// </summary>
        [DataMemberAttribute()]
        public decimal SpreadPercent { get; set; }

        /// <summary>
        /// 商家类型（0-自营他配；1-第三方；2-自营自配自采；3-自营自配统采）
        /// </summary>
        [DataMember]
        public int? MallType { get; set; }

        /// <summary>
        /// 可抵用易捷币金额
        /// </summary>
        [DataMember]
        public decimal? YJBAmount { get; set; }

        /// <summary>
        /// 赠送油卡金额
        /// </summary>
        [DataMember]
        public decimal? YoukaAmount { get; set; }

        /// <summary>
        /// 商品销量
        /// </summary>
        [DataMember]
        public int Salesvolume { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        [DataMember]
        public decimal? OrderWeight { get; set; }

    }

    [Serializable()]
    [DataContract]
    public class CommodityListIICDTO : CommodityListCDTO
    {
        /// <summary>
        /// 状态
        /// </summary>
        [DataMemberAttribute()]
        public decimal MealBoxAmount { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMemberAttribute()]
        public int MealBoxNum { get; set; }

        public new CommodityListIICDTO Clone()
        {
            return this.MemberwiseClone() as CommodityListIICDTO;
        }
    }
    /// <summary>
    /// 前端商品列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityListCategoryDTO
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
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal Intensity { get; set; }


        /// <summary>
        /// 类目名称
        /// </summary>
        [DataMember()]
        public string CommodityCategory { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 商品市场价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }
        /// <summary>
        /// 活动类型（0，限时打折；1，秒杀；2，预约；9999，表示无活动）
        /// </summary>
        [DataMember]
        public int PromotionType { get; set; }
        /// <summary>
        /// 商品属性（1无属性，2单属性，3，两组属性组合）
        /// </summary>
        [DataMember]
        public int ComAttrType { get; set; }

        /// <summary>
        /// 包装规格设置
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specifications { get; set; }

    }
    /// <summary>
    /// 商品列表应用信息
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComdtyAppInfoCDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid appId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string appName { get; set; }
        /// <summary>
        /// 应用图标
        /// </summary>
        [DataMember]
        public string icon { get; set; }
    }
    /// <summary>
    /// 商品列表结果
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComdtyListResultCDTO
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public virtual bool isSuccess { get; set; }

        /// <summary>
        /// 执行结果代码。
        /// </summary>
        [DataMember]
        public virtual int Code { get; set; }

        /// <summary>
        /// 返回消息。
        /// </summary>
        [DataMember]
        public virtual string Message { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int realCount { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<CommodityListCDTO> comdtyList { get; set; }
        /// <summary>
        /// 应用列表
        /// </summary>
        [DataMember]
        public List<ComdtyAppInfoCDTO> appInfoList { get; set; }

        /// <summary>
        /// 轮播图片列表
        /// </summary>
        [DataMember]
        public Jinher.AMP.LBP.Deploy.CustomDTO.LBListReturnDTO LBList { get; set; }
        /// <summary>
        /// 直播列表首条
        /// </summary>
        [DataMember]
        public Jinher.AMP.ZPH.Deploy.CustomDTO.LiveActivityListCDTO LiveActivity { get; set; }

        /// <summary>
        /// 分类数据
        /// </summary>
        [DataMember]
        public List<CategoryDTO> CategoryList { get; set; }

        /// <summary>
        /// 搜索品牌信息
        /// </summary>
        [DataMember]
        public List<BrandwallDTO> BrandWallList { get; set; }

        /// <summary>
        /// 商家类型
        /// </summary>
        [DataMember]
        public List<int?> MallAppList { get; set; }
    }
    /// <summary>
    /// YJB商品显示列表
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComAttrDTO
    {
        /// <summary>
        /// 商品id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 店铺编号
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        public string EsAppName { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [DataMember]
        public string Pic { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal? Price { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMember]
        public int Stock { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 市场价格
        /// </summary>
        [DataMember]
        public decimal? MarketPrice { get; set; }
        /// <summary>
        /// 进货价格
        /// </summary>
        [DataMember]
        public decimal? CostPrice { get; set; }
        /// <summary>
        /// 商品属性
        /// </summary>
        [DataMember]
        public string ComAttribute { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 提交人id
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }        
    }


}
