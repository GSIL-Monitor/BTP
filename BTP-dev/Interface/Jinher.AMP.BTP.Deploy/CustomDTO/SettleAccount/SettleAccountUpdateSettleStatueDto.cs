using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 修改计算结果DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountUpdateSettleStatueDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 结算结果
        /// </summary>
        [DataMember]
        public bool SettleStatue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
    }
}
