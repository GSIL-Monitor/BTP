using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 返回值，0代表成功，错误码分级BBBCCCC，分别代表
    /// 前三位（BBB）是业务类型：如公共的、用户、用户收货地址（公共的分配1开头的号段）
    /// 后四位（CCCC）是错误码：如用户名或密码错误 
    /// </summary>
    [DataContract]
    public enum ReturnCodeEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("Success")]
        [EnumMember]
        Success = 0,

       

        

        #region  公共的消息  100AAAA
        /// <summary>
        /// 服务异常
        /// </summary>
        [Description("服务异常")]
        [EnumMember]
        ServiceException = 1010003,


        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        [EnumMember]
        Params = 1010001,

        /// <summary>
        /// 参数错误，参数不能为空
        /// </summary>
        [Description("参数错误，参数不能为空！")]
        [EnumMember]
        ParamEmpty = 1010001,

        /// <summary>
        /// 分页参数不能为空
        /// </summary>
        [Description("分页参数不能为空！")]
        [EnumMember]
        PaginationEmpty = 1010002,

        /// <summary>
        /// UI MVC Action 异常
        /// </summary>
        [Description("服务异常，请重试！")]
        [EnumMember]
        MvcActionException = 1010003, 

        /// <summary>
        /// 数据不存在
        /// </summary>
        [Description("数据不存在")]
        [EnumMember]
        DataNotFound = 1010005,

        /// <summary>
        /// 参数超长，参数不能为超过设定长度
        /// </summary>
        [Description("参数超长，参数不能为超过设定长度！")]
        [EnumMember]
        ParamTooLong = 1010006,

        /// <summary>
        /// 数据库无任何更改
        /// </summary>
        [Description("数据库无任何更改！")]
        [EnumMember]
        DbNoModify = 1010007,


        #endregion






        #region 订单 102AAAA 
        /// <summary>
        /// 非京东店铺
        /// </summary>
        [Description("非京东店铺！")]
        [EnumMember]
        NotJdShop = 1020001,


        #endregion


        #region 商品 103AAAA 
        /// <summary>
        /// 商品已售馨,请选择其他商品
        /// </summary>
        [Description("商品已售馨,请选择其他商品！")]
        [EnumMember]
        CommoditySold = 1030001,

        /// <summary>
        /// 没有商品要去京东下单
        /// </summary>
        [Description("没有商品要去京东下单！")]
        [EnumMember]
        NoCommodityNeedJdOrder = 1030002,

        /// <summary>
        /// 该地区无货
        /// </summary>
        [Description("该地区无货！")]
        [EnumMember]
        NoCargoArea = 1030003,

        /// <summary>
        /// 抢光了~
        /// </summary>
        [Description("抢光了~")]
        [EnumMember]
        QiangGuang = 1030004,

        /// <summary>
        /// 商品已删除，请重新选择商品下单！
        /// </summary>
        [Description("商品已删除，请重新选择商品下单！")]
        [EnumMember]
        CommodityNotExists = 1030005,



        #endregion

        #region 下订单 104AAAA

        /// <summary>
        ///消费积分失败
        /// </summary>
        [Description("消费积分失败，请重新下单！")]
        [EnumMember]
        SpendScoreFail = 1040001,

        /// <summary>
        /// 消费优惠券失败，请重新下单
        /// </summary>
        [Description("消费优惠券失败，请重新下单！")]
        [EnumMember]
        SpendCouponFail = 1040002,

        /// <summary>
        /// 消费易捷币失败，请重新下单！
        /// </summary>
        [Description("消费易捷币失败，请重新下单！")]
        [EnumMember]
        SpendYJBFail = 1040003,

        /// <summary>
        /// 易捷币抵现券不能使用多张
        /// </summary>
        [Description("易捷币抵现券不能使用多张！")]
        [EnumMember]
        YJCouponCannotMultiple = 1040004,

        /// <summary>
        /// 优惠券、易捷币抵现券、易捷币、赠送油卡兑换券、优惠套装不能同时享受，请修改~
        /// </summary>
        [Description("优惠券、易捷币抵现券、易捷币、赠送油卡兑换券、优惠套装不能同时享受，请修改~")]
        [EnumMember]
        CannotEnjoyMultiplePreferentialActivities = 1040005,

        /// <summary>
        /// 发票信息有误
        /// </summary>
        [Description("发票信息有误！")]
        [EnumMember]
        InvoiceWrong = 1040006,

         
        /// <summary>
        /// 您设置的购买数量超过了可购上限,请按可购数量下单
        /// </summary>
        [Description("您设置的购买数量超过了可购上限,请按可购数量下单！")]
        [EnumMember]
        PurchaseQuantityOverflowLimit = 1040007,

        /// <summary>
        /// 优惠券状态错误
        /// </summary>
        [Description("优惠券状态错误！")]
        [EnumMember]
        CouponStateError = 1040008,

        /// <summary>
        /// 优惠的实际金额不能大于订单总价
        /// </summary>
        [Description("优惠的实际金额不能大于订单总价！")]
        [EnumMember]
        CouponExceedTotalPrice = 1040009,

        /// <summary>
        /// 优惠金额大于优惠券的面额
        /// </summary>
        [Description("优惠金额大于优惠券的面额！")]
        [EnumMember]
        CouponPirceExceedCash = 1040010,

        /// <summary>
        /// 效验优惠券服务异常
        /// </summary>
        [Description("效验优惠券服务异常！")]
        [EnumMember]
        CheckCouponException = 1040011,

        /// <summary>
        /// 未找到积分抵现的配置信息
        /// </summary>
        [Description("未找到积分抵现的配置信息！")]
        [EnumMember]
        NoScoreSetting = 1040012,
 
        /// <summary>
        /// 获取积分汇率失败
        /// </summary>
        [Description("获取积分汇率失败！")]
        [EnumMember]
        GetScoreCostFail = 1040013,


        /// <summary>
        /// 积分实际抵现金额和订单中积分抵现金额不匹配
        /// </summary>
        [Description("积分实际抵现金额和订单中积分抵现金额不匹配！")]
        [EnumMember]
        ScoreRealAmountNotEqualOrder = 1040014,



        /// <summary>
        /// 服务订单一次只能购买一个服务
        /// </summary>
        [Description("服务订单一次只能购买一个服务！")]
        [EnumMember]
        ServiceOrderOnlyOneItemAllowed = 1040015,

        /// <summary>
        /// 虚拟商品和实体商品不能同时下单
        /// </summary>
        [Description("虚拟商品和实体商品不能同时下单！")]
        [EnumMember]
        PhysicalAndVirualInOneOrder = 1040016,


        /// <summary>
        /// 配送费优惠信息有误
        /// </summary>
        [Description("配送费优惠信息有误！")]
        [EnumMember]
        DeliveryFeeDiscountFail = 1040017,

        
        /// <summary>
        /// 未达到起送金额
        /// </summary>
        [Description("未达到起送金额！")]
        [EnumMember]
        NotReachedStartingAmount = 1040018,


        /// <summary>
        /// 提交订单失败
        /// </summary>
        [Description("提交订单失败！")]
        [EnumMember]
        SubmitOrderFail = 1040019,


        /// <summary>
        /// 提交订单异常
        /// </summary>
        [Description("提交订单异常！")]
        [EnumMember]
        SubmitOrderException = 1040020,



        /// <summary>
        /// 消费易捷抵用券失败，请重新下单！
        /// </summary>
        [Description("消费易捷抵用券失败，请重新下单！")]
        [EnumMember]
        SpendYJCouponFail = 1040021,




        #endregion


        #region 自提点 105AAAA

        /// <summary>
        /// 该自提点已关闭，请重新选择~
        /// </summary>
        [Description("该自提点已关闭，请重新选择~")]
        [EnumMember]
        SelfTakeStationClosed = 1050001,

        /// <summary>
        /// 请选择自提时间
        /// </summary>
        [Description("请选择自提时间~")]
        [EnumMember]
        PleaseSelectPickDate = 1050002,


        /// <summary>
        /// 您选择的自提时间已失效，请重新选择！
        /// </summary>
        [Description("您选择的自提时间已失效，请重新选择！")]
        [EnumMember]
        PickupDateInvalid = 1050003,



        /// <summary>
        /// 您选择的自提时间有误，请重新选择！
        /// </summary>
        [Description("您选择的自提时间有误，请重新选择！")]
        [EnumMember]
        PickupDateError = 1050004,

        /// <summary>
        /// 请填写提货人！
        /// </summary>
        [Description("请填写提货人！")]
        [EnumMember]
        PleaseFillInPicker = 1050005,


        /// <summary>
        /// 自提订单没有自提信息！
        /// </summary>
        [Description("自提订单没有自提信息！")]
        [EnumMember]
        OrderNoPickUpInfo = 1050006,
        #endregion


        #region 拼团 105AAAA

        /// <summary>
        /// 未找到拼团
        /// </summary>
        [Description("未找到拼团！")]
        [EnumMember]
        DiyGroupNotExists = 1060001,

        /// <summary>
        /// 您已经成功参团，请邀请好友参加吧
        /// </summary>
        [Description("您已经成功参团，请邀请好友参加吧！")]
        [EnumMember]
        JoinDiyGroupSuccess = 1060002,

        /// <summary>
        /// 处理拼团异常
        /// </summary>
        [Description("处理拼团异常！")]
        [EnumMember]
        JoinOrCreateDiyGroupException = 1060003,


        #endregion


        #region  拼团 106AAAA
        /// <summary>
        /// 该商品每人限购{0}件，您已超限
        /// </summary>
        [Description("该商品每人限购{0}件，您已超限！")]
        [EnumMember]
        CommodityBuyLimit = 1060001,


        /// <summary>
        ///获取活动数据异常
        /// </summary>
        [Description("获取活动数据异常！")]
        [EnumMember]
        GetPromotionDataError = 1060002,


        

        #endregion


    }
}
