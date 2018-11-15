using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.JAP.Common.TypeDefine;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品详情+评价
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommoditySDTO
    {
        public CommoditySDTO()
        {
            Intensity = 10;
            DiscountPrice = -1;
            LimitBuyEach = -1;
            LimitBuyTotal = -1;
            SurplusLimitBuyTotal = 0;
            PromotionTypeNew = ComPromotionStatusEnum.NoPromotion;
            Score = new BtpScoreDTO();
        }
        /// <summary>
        /// 0实物商品，1(虚拟商品)易捷卡密
        /// </summary>
        [DataMemberAttribute()]
        public int Type { get; set; }
        /// <summary>
        /// 是否自营
        /// </summary>
        [DataMemberAttribute()]
        public string SelfSupport { get; set; }
        /// <summary>
        /// 缺货提醒类型
        /// </summary>
        [DataMemberAttribute()]
        public NotificationsDTO Notice { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 手机视频地址
        /// </summary>
        [DataMemberAttribute()]
        public string VideoUrl { get; set; }
        /// <summary>
        /// 视频名称
        /// </summary>
        [DataMemberAttribute()]
        public string VideoName { get; set; }
        /// <summary>
        /// 网页视频地址
        /// </summary>
        [DataMemberAttribute()]
        public string VideoWebUrl { get; set; }
        /// <summary>
        /// 视频图片地址
        /// </summary>
        [DataMemberAttribute()]
        public string VideoPicUrl { get; set; }
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
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }

        /// <summary>
        /// App名称
        /// </summary>
        [DataMemberAttribute()]
        public string AppName { get; set; }
        /// <summary>
        /// App图标
        /// </summary>
        [DataMemberAttribute()]
        public string AppIcon { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        [DataMemberAttribute()]
        public string Pic { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMemberAttribute()]
        public decimal? Intensity { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        [DataMemberAttribute()]
        public int? Stock { get; set; }
        /// <summary>
        /// 上下架
        /// </summary>
        [DataMemberAttribute()]
        public int? State { get; set; }
        /// <summary>
        /// 商品销量
        /// </summary>
        [DataMemberAttribute()]
        public int? Total { get; set; }
        /// <summary>
        /// 是否收藏
        /// </summary>
        [DataMemberAttribute()]
        public bool IsCollect { get; set; }
        /// <summary>
        /// 收藏数量
        /// </summary>
        [DataMemberAttribute()]
        public int? CollectNum { get; set; }
        /// <summary>
        /// 评价数量
        /// </summary>
        [DataMemberAttribute()]
        public int? ReviewNum { get; set; }
        /// <summary>
        /// 商品介绍
        /// </summary>
        [DataMemberAttribute()]
        public string Description { get; set; }
        /// <summary>
        /// 规格参数
        /// </summary>
        [DataMemberAttribute()]
        public string TechSpecs { get; set; }
        /// <summary>
        /// 售后服务
        /// </summary>
        [DataMemberAttribute()]
        public string SaleService { get; set; }
        /// <summary>
        /// 商品属性列表 -- 商品列表返回的时候这个列表为空
        /// </summary>
        [DataMemberAttribute()]
        public List<ComAttributeDTO> ComAttibutes { get; set; }
        /// <summary>
        /// 商品详情图片列表 -- 商品列表返回的时候这个列表为空
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityPictureCDTO> Pictures { get; set; }

        /// <summary>
        /// 商品数量--订单和购物车显示
        /// </summary>
        [DataMemberAttribute()]
        public int CommodityNumber { get; set; }

        /// <summary>
        /// 商品尺寸
        /// </summary>
        [DataMemberAttribute()]
        public string Size { get; set; }
        /// <summary>
        /// 购物车ID --获取购物车时返回
        /// </summary>
        [DataMemberAttribute()]
        public Guid ShopCartItemId { get; set; }
        /// <summary>
        /// 优惠价 
        /// </summary>
        [DataMemberAttribute()]
        public decimal? DiscountPrice { get; set; }
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
        /// 促销商品销量
        /// </summary>
        [DataMemberAttribute()]
        public int? SurplusLimitBuyTotal { get; set; }


        /// <summary>
        /// 商品价格最小值 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// 商品价格最大值 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// 商品组合属性集合 //zgx-modify
        /// </summary>
        [DataMemberAttribute()]
        public List<CommodityAttrStockDTO> CommodityStocks { get; set; }

        /// <summary>
        /// 运送到
        /// </summary>
        [DataMemberAttribute()]
        public string FreightTo { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }
        /// <summary>
        /// 是否有运费明细
        /// </summary>
        [DataMemberAttribute()]
        public bool IsSetMulti { get; set; }

        /// <summary>
        /// 属性ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityStockId { get; set; }
        /// <summary>
        /// 购物车商品状态
        /// </summary>
        [DataMemberAttribute()]
        public ShopCartStateEnum ShopCartState { get; set; }
        /// <summary>
        /// 关联商品的集合
        /// </summary>
        [DataMemberAttribute()]
        public List<RelationCommodityDTO> RelationCommoditys { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        [DataMemberAttribute()]
        public decimal? MarketPrice { get; set; }

        /// <summary>
        /// 活动状态 0：没有活动或已失效 ,1:预约预售进行中，2：等待抢购：3：活动进行中，4：活动已结束
        /// </summary>
        [DataMemberAttribute()]
        public int PromotionState { get; set; }

        /// <summary>
        /// 活动类型 0：普通活动，1：秒杀，2：预约，3：拼团，9999是没活动
        /// </summary>
        [DataMemberAttribute()]
        [Obsolete("客户端老版本兼容，已废弃，请使用PromotionTypeNew", false)]
        public int PromotionType { get; set; }

        /// <summary>
        /// 活动类型 0：普通活动，1：秒杀，2：预约，3：拼团
        /// </summary>
        [DataMemberAttribute()]
        public ComPromotionStatusEnum PromotionTypeNew { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PromotionStartTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PromotionEndTime { get; set; }

        /// <summary>
        /// 预约、预售开始时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PresellStartTime { get; set; }
        /// <summary>
        /// 预约、预售结束时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PresellEndTime { get; set; }

        /// <summary>
        /// 是否进行中的众筹
        /// </summary>
        [DataMember]
        public bool IsActiveCrowdfunding { get; set; }

        /// <summary>
        /// 已预约人数
        /// </summary>
        [DataMember]
        public int PreselledNum { get; set; }

        /// <summary>
        /// 发货日期
        /// </summary>
        [DataMember]
        public string DeliveryTime { get; set; }

        ///// <summary>
        ///// 发货日期（支付成功后天数）
        ///// </summary>
        //[DataMember]
        //public int? DeliveryDays { get; set; }

        /// <summary>
        /// 优惠活动Id
        /// </summary>
        [DataMember]
        public Guid? PromotionId { get; set; }
        /// <summary>
        /// 外部优惠活动Id
        /// </summary>
        [DataMember]
        public Guid? OutPromotionId { get; set; }
        private DateTime _currentTime = DateTime.Now;
        /// <summary>
        /// 当前时间
        /// </summary>
        [DataMember]
        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; }
        }

        /// <summary>
        /// 是否支持自提:0不支持，1支持。
        /// </summary>
        [DataMember]
        public int IsEnableSelfTake { get; set; }
        /// <summary>
        /// 是否正品会应用
        /// </summary>
        [DataMember]
        public bool IsAppSet { get; set; }
        /// <summary>
        /// 包邮条件
        /// </summary>
        [DataMember]
        public List<string> FreeFreightStandard { get; set; }

        /// <summary>
        /// 是否参加分成推广
        /// </summary>
        [DataMember]
        public bool IsShare { get; set; }
        /// <summary>
        /// 商品分享分成比例
        /// </summary>
        [DataMember]
        public decimal? SharePercent { get; set; }

        /// <summary>
        /// 是否参加三级分销
        /// </summary>
        [DataMember]
        public bool IsDistribute { get; set; }


        /// <summary>
        /// 商品类型:0实体商品；1虚拟商品
        /// </summary>
        [DataMember]
        public int CommodityType { get; set; }

        /// <summary>
        /// 商品的拼团活动信息
        /// </summary>
        [DataMember]
        public TodayPromotionDTO DiyGroupPromotion { get; set; }

        /// <summary>
        /// 已参加活动人数（对应拼团中的参团人数）
        /// </summary>
        [DataMember]
        public int AlreadyJoinCount { get; set; }

        /// <summary>
        /// 会员折扣信息
        /// </summary>
        [DataMemberAttribute()]
        public VipPromotionDTO VipPromotion { get; set; }

        /// <summary>
        /// 商品属性（1无属性，2单属性，3，两组属性组合）
        /// </summary>
        [DataMember]
        public int ComAttrType { get; set; }
        /// <summary>
        /// 是否启用评价功能（定制功能）
        /// </summary>
        [DataMember]
        public bool HasReviewFunction { get; set; }
        /// <summary>
        /// 评价
        /// </summary>
        [DataMember]
        public BtpScoreDTO Score { get; set; }

        /// <summary>
        /// 爱尔目直播地址
        /// </summary>
        [DataMember]
        public string EquipmentUrl { get; set; }

        /// <summary>
        /// 720云景地址
        /// </summary>
        [DataMember]
        public string CloudviewUrl { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        [DataMember]
        public decimal Duty { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public string No_Code { get; set; }

        /// <summary>
        /// 商品标签(已废弃)
        /// </summary>
        [DataMember]
        public string[] Labels { get; set; }

        /// <summary>
        /// 优惠券
        /// </summary>
        [DataMember]
        public string[] Coupons { get; set; }

        /// <summary>
        /// 易捷币信息
        /// </summary>
        [DataMember]
        public Jinher.AMP.YJB.Deploy.CustomDTO.OrderInsteadCashDTO YJBInfo { get; set; }

        /// <summary>
        /// 商品油卡兑换券设置相关信息
        /// </summary>
        [DataMember]
        public CommodityYouKaDTO YouKaInfo { get; set; }

        /// <summary>
        /// 赠品信息
        /// </summary>
        [DataMember]
        public CommodiyPresentDTO Present { get; set; }

        /// <summary>
        /// 优惠券列表
        /// </summary>
        [DataMember]
        public List<CouponTemplatDetailDTO> CouponList { get; set; }

        /// <summary>
        /// 活动sku属性集合
        /// </summary>
        [DataMember]
        public List<SkuActivityCDTO> SkuActivityCdtos { get; set; }

        /// <summary>
        /// 金采团购活动sku属性集合
        /// </summary>
        [DataMember]
        public List<JCActivityItemsListCDTO> JCActivityItemsListCdtos { get; set; }

        /// <summary>
        /// 金采活动商品sku集合 最小价格
        /// </summary>
        [DataMember]
        public decimal MinJcSkuPrice { get; set; }

        /// <summary>
        /// 金采活动商品sku集合 最大价格
        /// </summary>
        [DataMember]
        public decimal MaxJcSkuPrice { get; set; }

        /// <summary>
        /// 金采活动Id
        /// </summary>
        [DataMember]
        public Guid? JcActivityId { get; set; }

        /// <summary>
        /// 金采活动名称
        /// </summary>
        [DataMember]
        public string JcActivityName { get; set; }

        /// <summary>
        /// 是否参与活动sku
        /// </summary>
        [DataMember]
        public bool IsJoin { get; set; }

        /// <summary>
        /// 商品sku集合 最小价格
        /// </summary>
        [DataMember]
        public decimal MinSkuPrice { get; set; }

        /// <summary>
        /// 商品sku集合 最大价格
        /// </summary>
        [DataMember]
        public decimal MaxSkuPrice { get; set; }


        [DataMember]
        /// <summary>
        /// 包邮信息
        /// </summary>
        public string PostAge { get; set; }

        /// <summary>
        /// 服务项设置信息
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.ServiceSettingDTO> ServiceSettings { get; set; }


        /// <summary>
        /// 包装规格设置
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.SpecificationsDTO> Specifications { get; set; }


        /// <summary>
        /// 优惠套装 最大优惠信息
        /// </summary>
        [DataMember]
        public string MealActivityInfo { get; set; }

        /// <summary>
        /// 优惠套装名称
        /// </summary>
        [DataMember]
        public string SetMealTheme { get; set; }

        /// <summary>
        /// 商品分类Id
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }
    }

    /// <summary>
    /// 商品油卡兑换券设置相关
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CommodityYouKaDTO
    {
        /// <summary>
        /// 购买商品赠送油卡兑换券金额
        /// </summary>
        [DataMember]
        public decimal GiveMoney { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 购买商品赠送油卡兑换券比例
        /// </summary>
        [DataMember]
        public decimal YouKaPersent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CommodityYouKaDTO(decimal giveMoney, decimal YouKaPersent)
        {
            this.YouKaPersent = YouKaPersent;
            this.GiveMoney = giveMoney;
            this.Title = "油卡券";
            this.Message = string.Format("购买本商品，支付后即赠送{0}元油卡券", giveMoney);
        }
    }


    /// <summary>
    /// 服务项设置信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ServiceSettingDTO
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Content { get; set; }

    }


    /// <summary>
    /// 包装规格设置
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SpecificationsDTO
    {

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Attribute { get; set; }

        [DataMember]
        public string strAttribute { get; set; }

    }
    
}
