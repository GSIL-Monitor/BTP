using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 赠品活动搜索DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class PresentPromotionSearchDTO : SearchBase
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 活动状态(0:未开始 1:活动中 2:已结束)
        /// </summary>
        [DataMember]
        public int? Status { get; set; }


    }

    /// <summary>
    /// 赠品活动列表DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class PresentPromotionSearchResultDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        ///// <summary>
        ///// 用户IDs
        ///// </summary>
        //[DataMember]
        //public Guid UserId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否结束
        /// </summary>
        [DataMember]
        public bool IsEnd { get; set; }

        /// <summary>
        /// 活动状态
        /// </summary>
        [DataMember]
        public int Status { get; set; }
    }
}
