using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 下订单所需数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class CreateOrderNeedDTO
    {
        /// <summary>
        /// 下订单所需数据
        /// </summary>
        public CreateOrderNeedDTO()
        {
            ShipmentInfo = new ShipmentInfoDTO();
            UserScore = new UserScoreDTO();

        }

        /// <summary>
        /// 是否商城
        /// </summary>
        [DataMember]
        public bool IsPlatformApp { get; set; }

        /// <summary>
        /// 交易类型：0:担保交易;1：非担保交易（直接到账）
        /// </summary>
        [DataMember]
        public int TradeType { get; set; }

        /// <summary>
        /// 是不是订单中的所有app都是代运营app
        /// </summary>
        [DataMember]
        public bool IsAllAppInZPH { get; set; }

        /// <summary>
        /// 配送方式相关
        /// </summary>
        [DataMember]
        public ShipmentInfoDTO ShipmentInfo { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        [DataMember]
        public UserScoreDTO UserScore { get; set; }


        /// <summary>
        /// 当前用户金币余额
        /// </summary>
        [DataMember]
        public ulong GoldBalance { get; set; }

        /// <summary>
        /// 当前用户代金券张数。
        /// </summary>
        [DataMember]
        public int CouponCount { get; set; }

        /// <summary>
        ///  是不是所有店铺app都支持“货到付款”。
        /// </summary>
        [DataMember]
        public bool IsAllAppSupportCOD { get; set; }
        /// <summary>
        ///  是不是所有店铺app都支持“货到付款”。
        /// </summary>
        [DataMember]
        public bool IsShowScore { get; set; }

    }
    /// <summary>
    /// 配送方式相关
    /// </summary>
    [DataContract]
    public class ShipmentInfoDTO
    {
        /// <summary>
        /// 配送方式相关
        /// </summary>
        public ShipmentInfoDTO()
        {
            HasSelfTakeFunc = false;
            ShipmentWays = 1;
            AppSelfTakeStationDefaultInfo = new AppSelfTakeStationDefaultInfoDTO();
            DefaultAddressInfo = new AddressSDTO();
        }

        /// <summary>
        /// 是否启用自提功能
        /// </summary>
        [DataMember]
        public bool HasSelfTakeFunc { get; set; }
        /// <summary>
        /// 配送方式 1快递，2自提，3两者
        /// </summary>
        [DataMember]
        public int ShipmentWays { get; set; }

        /// <summary>
        /// App自提点信息
        /// </summary>
        [DataMember]
        public AppSelfTakeStationDefaultInfoDTO AppSelfTakeStationDefaultInfo { get; set; }

        /// <summary>
        /// 默认的地址信息。
        /// </summary>
        [DataMember]
        public AddressSDTO DefaultAddressInfo { get; set; }

    }
}
