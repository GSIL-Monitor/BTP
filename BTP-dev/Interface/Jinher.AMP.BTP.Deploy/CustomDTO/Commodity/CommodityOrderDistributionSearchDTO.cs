using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销订单查询类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderDistributionSearchDTO
    {
        /// <summary>
        /// 分销者Id
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 查询类型。1：按销量；2：按佣金
        /// </summary>
        [DataMember]
        public int SearchType { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderCode { get; set; }
        /// <summary>
        /// 订单完成时间开始
        /// </summary>
        [DataMember]
        public DateTime? FinishTimeStart { get; set; }
        /// <summary>
        /// 订单完成时间结束
        /// </summary>
        [DataMember]
        public DateTime? FinishTimeEnd { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        [DataMember]
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

    }
}
