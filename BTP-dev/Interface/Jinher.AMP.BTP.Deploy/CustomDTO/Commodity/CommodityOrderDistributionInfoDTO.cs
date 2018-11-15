using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销订单
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderDistributionInfoDTO
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        [DataMember]
        public decimal DistributeMoney { get; set; }
        /// <summary>
        /// 订单完成时间
        /// </summary>
        [DataMember]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 分销商Id
        /// </summary>
        [DataMember]
        public Guid DistributorId { get; set; }

        public string DistributorIdStr { get; set; }
    }
}
