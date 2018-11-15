using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 会员折扣
    /// </summary>
    [Serializable]
    [DataContract]
    public class VipPromotionDTO
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        [DataMember]
        public decimal Intensity { get; set; }
        /// <summary>
        /// 会员等级描述
        /// </summary>
        [DataMember]
        public string VipLevelDesc { get; set; }
        /// <summary>
        /// 是否会员
        /// </summary>
        [DataMember]
        public bool IsVip { get; set; }
        /// <summary>
        /// 会员优惠价
        /// </summary>
        [DataMember]
        public decimal DiscountPrice { get; set; }
        /// <summary>
        /// 是否启用会员体系
        /// </summary>
        [DataMember]
        public bool IsVipActive { get; set; }
        /// <summary>
        /// 会员等级Id
        /// </summary>
        [DataMember]
        public Guid VipLevlelId { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public VipPromotionDTO()
        {
            IsVip = false;
            Intensity = 10;
            DiscountPrice = -1;
            IsVipActive = false;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public VipPromotionDTO(Guid appId,Guid userId)
        {
            IsVip = false;
            Intensity = 10;
            DiscountPrice = -1;
            AppId = appId;
            UserId = userId;
            IsVipActive = false;

        }
    }
}
