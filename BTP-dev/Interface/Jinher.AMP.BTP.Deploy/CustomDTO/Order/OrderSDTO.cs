using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 添加订单DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderSDTO
    {
        /// <summary>
        /// 用户Id √
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid UserId { get; set; }
        /// <summary>
        /// AppId √
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid AppId { get; set; }
        /// <summary>
        /// 订单总价 √
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单含运费总价 √
        /// </summary>
        [DataMemberAttribute()]
        public Decimal RealPrice { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary> 
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 支付方式 
        /// </summary> 
        [DataMemberAttribute()]
        public int Payment { get; set; }
        /// <summary>
        /// 收货人姓名 √
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话 √
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址 √
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }

        /// <summary>
        /// 省 √
        /// </summary> 
        [DataMemberAttribute()]
        public string Province { get; set; }


        /// <summary>
        /// 省编码
        /// </summary> 
        [DataMemberAttribute()]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 市 √
        /// </summary> 
        [DataMemberAttribute()]
        public string City { get; set; }

        /// <summary>
        /// 市编码
        /// </summary> 
        [DataMemberAttribute()]
        public string CityCode { get; set; }

        /// <summary>
        /// 区 √
        /// </summary>
        [DataMemberAttribute()]
        public string District { get; set; }


        /// <summary>
        /// 区编码
        /// </summary>
        [DataMemberAttribute()]
        public string DistrictCode { get; set; }

        /// <summary>
        /// 街道 √
        /// </summary>
        [DataMemberAttribute()]
        public string Street { get; set; }


        /// <summary>
        /// 街道编码
        /// </summary>
        [DataMemberAttribute()]
        public string StreetCode { get; set; }

        /// <summary>
        /// 订单备注 √
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }
        /// <summary>
        /// 邮编 √
        /// </summary>
        [DataMemberAttribute()]
        public string RecipientsZipCode { get; set; }
        /// <summary>
        /// 来源类型 √
        /// </summary>
        [DataMemberAttribute()]
        public int SrcType { get; set; }
        /// <summary>
        /// 来源标识Id √
        /// </summary>
        [DataMemberAttribute()]
        public Guid SrcTagId { get; set; }

        /// <summary>
        /// CPSId √
        /// </summary>
        [DataMemberAttribute()]
        public string CPSId { get; set; }
        /// <summary>
        /// 订单商品--订单列表时 √
        /// </summary>
        [DataMemberAttribute()]
        public List<ShoppingCartItemSDTO> ShoppingCartItemSDTO { get; set; }


        /// <summary>
        /// 分享平台
        /// </summary>
        [DataMemberAttribute()]
        public int SharePlatform { get; set; }

        /// <summary>
        /// 分享Id √
        /// </summary>
        [DataMemberAttribute()]
        public string ShareId { get; set; }


        /// <summary>
        /// 金币付款金额 √
        /// </summary>
        [DataMemberAttribute()]
        public Decimal GoldPrice { get; set; }

        /// <summary>
        /// 支付密码 √
        /// </summary>
        [DataMemberAttribute()]
        public string PayPassword { get; set; }

        /// <summary>
        /// 代金券使用金额 √
        /// </summary>
        [DataMemberAttribute()]
        public Decimal PayCouponValue { get; set; }

        /// <summary>
        /// 代金券使用编码 √
        /// </summary>
        [DataMemberAttribute()]
        public string PaycouponCodes { get; set; }

        /// <summary>
        /// 自提标志，0：非自提，1：自提 √
        /// </summary>
        [DataMemberAttribute()]
        public int SelfTakeFlag { get; set; }

        public OrderSDTO Clone()
        {
            return this.MemberwiseClone() as OrderSDTO;
        }

        /// <summary>
        /// 推广码 √
        /// </summary>
        [DataMemberAttribute()]
        public Guid? SpreadCode { get; set; }


        /// <summary>
        /// 原appid √
        /// </summary>
        [DataMemberAttribute()]
        public Guid SrcAppId { get; set; }

        /// <summary>
        /// 优惠券抵用金额 √
        /// </summary>
        [DataMemberAttribute()]
        public Decimal CouponValue { get; set; }

        /// <summary>
        /// 优惠券ID √
        /// </summary>
        [DataMemberAttribute()]
        public Guid CouponId { get; set; }

        /// <summary>
        /// 使用优惠券的商品Id,如果为空值，则没有优惠券或优惠券作用在店铺上 √
        /// </summary>
        [DataMemberAttribute()]
        public Guid CouponComId { get; set; }
        /// <summary>
        /// 自提点id √
        /// </summary>
        [DataMemberAttribute()]
        public Guid SelfTakeStationId { get; set; }

        /// <summary>
        /// 电商馆Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }

        /// <summary>
        /// 积分抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal ScorePrice { get; set; }

        /// <summary>
        /// 订单类型: 订单类型:0实体商品订单；1虚拟商品订单；2.餐饮订单；3(虚拟商品)易捷卡密订单
        /// </summary>
        [DataMember]
        public int OrderType { get; set; }
        /// <summary>
        /// 服务id（与btp订单的关联id）
        /// </summary>
        [DataMember]
        public Guid? ServiceId { get; set; }
        /// <summary>
        /// 分销商UserId
        /// </summary>
        [DataMemberAttribute()]
        public Guid DistributorId { get; set; }
        /// <summary>
        /// Source
        /// </summary>
        [DataMemberAttribute()]
        public string Source { get; set; }
        /// <summary>
        /// WxOpenId
        /// </summary>
        [DataMemberAttribute()]
        public string WxOpenId { get; set; }
        /// <summary>
        /// 0：担保；1：直接到账
        /// </summary>
        [DataMemberAttribute()]
        public int TradeType { get; set; }
        /// <summary>
        /// 上传图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 活动类型（ 0：普通活动，1：秒杀，2：预售，3：拼团，5：预售(不用预约)，6：赠品，7：套装）
        /// </summary>
        [DataMemberAttribute()]
        public int PromotionType { get; set; }
        /// <summary>
        /// 拼团Id
        /// </summary>
        public Guid? DiyGroupId { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        [DataMemberAttribute()]
        public InvoiceDTO InvoiceInfo { get; set; }
        /// <summary>
        /// App自提信息
        /// </summary>
        [DataMember]
        public AppOrderPickUpInfoDTO AppOrderPickUpInfo { get; set; }
        /// <summary>
        /// 是否微信支付
        /// </summary>
        [DataMember]
        public bool IsWeixinPay { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        [DataMember]
        public int? Batch { get; set; }
        /// <summary>
        /// 请求协议
        /// </summary>
        [DataMember]
        public string Scheme { get; set; }
        /// <summary>
        /// 积分类型：1店铺积分，2通用积分
        /// </summary>
        [DataMember]
        public ScoreTypeEnum ScoreType { get; set; }
        /// <summary>
        /// 餐盒费
        /// </summary>
        [DataMember]
        public Decimal? mealBoxFee { get; set; }
        /// <summary>
        /// 配送费优惠
        /// </summary>
        [DataMember]
        public Decimal? deliveryFeeDiscount { get; set; }
        /// <summary>
        /// 配送费优惠
        /// </summary>
        [DataMember]
        public Decimal? Duty { get; set; }

        ///// <summary>
        ///// 易捷币抵用个数
        ///// </summary>
        //[DataMemberAttribute()]
        //public long YJBCount { get; set; }

        /// <summary>
        /// 易捷币抵用金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal YJBPrice { get; set; }

        /// <summary>
        /// 易捷抵用卷
        /// </summary>
        [DataMember]
        public List<Guid> YJCouponIds { get; set; }

        /// <summary>
        /// 易捷抵用卷金额
        /// </summary>
        [DataMember]
        public decimal YJCouponPrice { get; set; }


        /// <summary>
        /// 这个订单使用的跨店铺满减券的Id
        /// </summary>
        [DataMember]
        public Guid StoreCouponId { get; set; }

        /// <summary>
        /// 跨店铺满减卷商品金额
        /// </summary>
        [DataMember]
        public decimal StoreCouponCommdityPrice { get; set; }

        /// <summary>
        /// 跨店铺满减卷面值
        /// </summary>
        [DataMember]
        public decimal StoreCouponPrice { get; set; }

        /// <summary>
        /// 跨店铺满减 这个订单拆分到的优惠金额
        /// </summary>
        [DataMember]
        public decimal StoreCouponShopDivid { get; set; }

        /// <summary>
        /// 跨店铺满减卷商品的数量，用以拆单的时候，最后一单的计算
        /// </summary>
        [DataMember]
        public int StoreCouponCommdityCount { get; set; }


        /// <summary>
        /// 阳关餐饮字段1
        /// </summary>
        [DataMember]
        public string FirstContent { get; set; }


        /// <summary>
        /// 阳关餐饮字段2
        /// </summary>
        [DataMember]
        public string SecondContent { get; set; }

        /// <summary>
        /// 阳关餐饮字段3
        /// </summary>
        [DataMember]
        public string ThirdContent { get; set; }

        /// <summary>
        /// 订单是否赠送油卡兑换券
        /// </summary>
        [DataMember]
        public bool IsUseYouKa { get; set; }

        /// <summary>
        /// 订单是否是优惠套装
        /// </summary>
        [DataMember]
        public bool IsSetMeal { get; set; }

        /// <summary>
        /// 优惠套装Id
        /// </summary>
        [DataMember]
        public Guid SetMealId { get; set; }

        /// <summary>
        /// 金采团购活动Id
        /// </summary>
        [DataMember]
        public Guid JcActivityId { get; set; }

        /// <summary>
        /// 订单是否是金采团购活动
        /// </summary>
        [DataMember]
        public bool IsJcActivity { get; set; }

        /// <summary>
        /// 邀请人手机号
        /// </summary>
        [DataMember]
        public string InviterMobile { get; set; }

        /// <summary>
        /// 订单中使用的易捷卡列表
        /// </summary>
        [DataMember]
        public List<PayItem> YjCards { get; set; }

        /// <summary>
        /// 易捷卡抵用金额
        /// </summary>
        [DataMember]
        public decimal YjCardPrice { get; set; }


        /// <summary>
        /// 运费
        /// </summary>
        [DataMember]
        public decimal Freight { get; set; }

        /// <summary>
        /// 主订单Id
        /// </summary>
        [DataMember]
        public Guid MainOrderId { get; set; }
    }

    /// <summary>
    /// 添加订单DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PrizeOrderSDTO
    {
        /// <summary>
        /// 是否中奖订单
        /// </summary>
        [DataMember]
        public bool IsPrizeOrder { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid UserId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid AppId { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        [DataMemberAttribute()]
        public Decimal Price { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary> 
        [DataMemberAttribute()]
        public int State { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary> 
        [DataMemberAttribute()]
        public int Payment { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary> 
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }
        /// <summary>
        /// 省
        /// </summary> 
        [DataMemberAttribute()]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary> 
        [DataMemberAttribute()]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMemberAttribute()]
        public string District { get; set; }
        /// <summary>
        /// 订单备注
        /// </summary>
        [DataMemberAttribute()]
        public string Details { get; set; }
        /// <summary>
        /// 订单商品--订单列表时
        /// </summary>
        [DataMemberAttribute()]
        public List<ShoppingCartItemSDTO> ShoppingCartItemSDTO { get; set; }
        /// <summary>
        /// Source
        /// </summary>
        [DataMemberAttribute()]
        public string Source { get; set; }
        /// <summary>
        /// WxOpenId
        /// </summary>
        [DataMemberAttribute()]
        public string WxOpenId { get; set; }
        /// <summary>
        /// 上传图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
    }

    /// <summary>
    /// App自提信息
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AppOrderPickUpInfoDTO
    {
        /// <summary>
        /// 自提点id
        /// </summary>
        [DataMember]
        public Guid SelfTakeStationId { get; set; }
        /// <summary>
        /// 提货人姓名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 提货人联系方式
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 预约提货日期
        /// </summary>
        [DataMember]
        public DateTime? BookDate { get; set; }
        /// <summary>
        /// 预约提货开始时间
        /// </summary>
        [DataMember]
        public TimeSpan? BookStartTime { get; set; }
        /// <summary>
        /// 预约提货截止时间
        /// </summary>
        [DataMember]
        public TimeSpan? BookEndTime { get; set; }
    }

    /// <summary>
    /// 支付项（每一个用来抵用订单金额的支付方式）
    /// </summary>
    [Serializable()]
    [DataContract]
    public class PayItem
    {
        /// <summary>
        /// 卡券id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 卡券余额（未在当前订单中被消费之前）
        /// </summary>
        [DataMember]
        public decimal Balance { get; set; }

        /// <summary>
        /// 当前卡券在订单中的使用金额
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        ///// <summary>
        ///// 是否已处理（下订单易捷卡拆分到各订单时使用）
        ///// </summary>
        //[DataMember]
        //public bool IsDeal { get; set; }
    }
}
