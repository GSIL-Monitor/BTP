using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    ///  分销统计信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributorProfitsInfoDTO
    {
        /// <summary>
        /// 分销商Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        [DataMember]
        public string UserCode { get; set; }
        /// <summary>
        /// 头像Url
        /// </summary>
        [DataMember]
        public string PicturePath { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 销量
        /// </summary>
        [DataMember]
        public decimal OrderAmount { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        [DataMember]
        public decimal CommissionAmount { get; set; }
        /// <summary>
        /// 待收益佣金
        /// </summary>
        [DataMember]
        public decimal CommmissionUnPay { get; set; }
        /// <summary>
        /// 下级分销商数量
        /// </summary>
        [DataMember]
        public int UnderlingCount { get; set; }
        /// <summary>
        /// 下下级分销商数量
        /// </summary>
        [DataMember]
        public int SubUnderlingCount { get; set; }
        /// <summary>
        /// 扩展子类
        /// </summary>
        [DataMember]
        public DistributorProfitsInfoDTO SubList { get; set; }
        /// <summary>
        /// 上级用户昵称
        /// </summary>
        [DataMember]
        public string ParentUserName { get; set; }

    }
}
