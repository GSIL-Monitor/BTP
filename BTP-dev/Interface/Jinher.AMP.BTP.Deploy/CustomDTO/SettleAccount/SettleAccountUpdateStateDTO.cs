using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 修改结算单状态DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class SettleAccountUpdateStateDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        public List<Guid> Ids { get; set; }

        /// <summary>
        /// 结算状态(0、待结算 1、等待商家确认 2、待打款 3、已结算)
        /// </summary>
        public int State { get; set; }
    }
}
