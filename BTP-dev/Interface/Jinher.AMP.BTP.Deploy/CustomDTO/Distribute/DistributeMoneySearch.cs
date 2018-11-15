using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销佣金查询
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributeMoneySearch
    {
        /// <summary>
        /// 分销者Id
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }
        /// <summary>
        /// 分销商用户Id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// 查询类型：1 佣金累计；2 待收益
        /// </summary>
        public int SearchType { get; set; }

        /// <summary>
        /// 当前页，从1开始
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }
    }
}
