using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 众销分享明细DTO
    /// </summary>
    [DataContract]
    public class ShareItemDTO
    {
        /// <summary>
        /// 分享人Id
        /// </summary>
        [DataMember]
        public Guid ShareUserId { get; set; }
        /// <summary>
        /// 分享人账号
        /// </summary>
        [DataMember]
        public string ShareUserCode { get; set; }
        /// <summary>
        /// 分享人姓名
        /// </summary>
        [DataMember]
        public string ShareUserName { get; set; }
        /// <summary>
        /// 第三方平台名称
        /// </summary>
        [DataMember]
        public string ThirdPartName { get; set; }
        /// <summary>
        /// 分享分红（人民币）
        /// </summary>
        [DataMember]
        public decimal TotalDividend
        {
            get { return 1.0m * DividendGold / 1000; }
            set { }
        }
        /// <summary>
        /// 分享分红（金币）
        /// </summary>
        [DataMember]
        public long DividendGold { get; set; }
        /// <summary>
        /// 来源订单编号
        /// </summary>
        [DataMember]
        public string OrderCode { get; set; }

        /// <summary>
        /// 分享时间
        /// </summary>
        [DataMember]
        public DateTime ShareDate { get; set; }

    }
    /// <summary>
    /// 众销分享结果
    /// </summary>
    [DataContract]
    [KnownType(typeof(ShareItemDTO))]
    public class ShareListResult
    {
        /// <summary>
        /// 用户合计
        /// </summary>
        [DataMember]
        public int SumUserCount { get; set; }
        /// <summary>
        /// 获奖金额合计（人民币）
        /// </summary>
        [DataMember]
        public decimal SumTotalDividend
        {
            get { return 1.0m * SumDividendGold / 1000; }
            set { }
        }
        /// <summary>
        /// 获奖金额合计(金币)
        /// </summary>
        [DataMember]
        public decimal SumDividendGold { get; set; }

        /// <summary>
        /// 分享列表
        /// </summary>
        [DataMember]
        public List<ShareItemDTO> ShareItems { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
    }


}
