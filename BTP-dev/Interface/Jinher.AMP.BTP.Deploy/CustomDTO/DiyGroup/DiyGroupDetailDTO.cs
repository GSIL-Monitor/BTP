using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 拼团管理输出dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class DiyGroupDetailDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 团编号
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 团长id
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 活动Id
        /// </summary>
        [DataMember]
        public Guid PromotionId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        [DataMember]
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 组团状态 0：待成团，1：组团成功，2：已成团，3：未成团（组团失败），4：退款审核，5：已退款
        /// </summary>
        [DataMember]
        public Int32 State { get; set; }
        /// <summary>
        /// 参团人数
        /// </summary>
        [DataMember]
        public Int32 JoinNumber { get; set; }
        /// <summary>
        /// 确认成团审核人
        /// </summary>
        [DataMember]
        public Guid? SuccessProcessorId { get; set; }
        /// <summary>
        /// 确认成团审核时间
        /// </summary>
        [DataMember]
        public DateTime? SuccessTime { get; set; }
        /// <summary>
        /// 退款审核人
        /// </summary>
        [DataMember]
        public Guid? FailProcessorId { get; set; }
        /// <summary>
        /// 退款审核时间
        /// </summary>
        [DataMember]
        public DateTime? FailTime { get; set; }
        /// <summary>
        /// EsAppId
        /// </summary>
        [DataMember]
        public Guid EsAppId { get; set; }

        #region promotion
        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 成团人数
        /// </summary>
        [DataMember]
        public Int32 GroupMinVolume { get; set; }
        /// <summary>
        /// 超时时间单位(秒)
        /// </summary>
        [DataMember]
        public Int32 ExpireSecond { get; set; }

        /// <summary>
        /// 活动描术
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 外部活动Id
        /// </summary>
        [DataMember]
        public Guid OutsideId { get; set; }

        #endregion

        #region PromotionItem
        /// <summary>
        /// 每人限购数量
        /// </summary>
        [DataMember]
        public Int32 LimitBuyEach { get; set; }
        /// <summary>
        /// 参加促销商品的总数
        /// </summary>
        [DataMember]
        public Int32 LimitBuyTotal { get; set; }
        /// <summary>
        /// 优惠价
        /// </summary>
        [DataMember]
        public decimal? DiscountPrice { get; set; }
        /// <summary>
        /// 促销商品的销量
        /// </summary>
        [DataMember]
        public Int32 SurplusLimitBuyTotal { get; set; }
        #endregion

        #region commodity
        /// <summary>
        /// 图片路径
        /// </summary>
        [DataMember]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }
        /// <summary>
        /// 第一张大图路径（微信分享使用）
        /// </summary>
        [DataMember]
        public string ProductDetailPicture { get; set; }
        #endregion
        
        /// <summary>
        /// 拼团的订单列表
        /// </summary>
        [DataMember]
        public List<DiyGroupOrderDetailDTO> DiyGroupOrderList { get; set; }

        /// <summary>
        /// 系统当前时间，倒计时用
        /// </summary>
        [DataMember]
        public DateTime DateTimeNow { get; set; }

    }
}
