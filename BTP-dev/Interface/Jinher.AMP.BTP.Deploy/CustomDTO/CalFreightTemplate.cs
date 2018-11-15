using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 多商品运费计算类
    /// </summary>
    [Serializable()]
    [DataContract]
   public class CalFreightTemplate
    {
        /// <summary>
        /// 运费模板ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid FreightTemplateId { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 计价规则(0按件数，1按重量，2按体积)
        /// </summary>
        [DataMemberAttribute()]
        public int CalcType { get; set; }
        /// <summary>
        /// 首费标谁
        /// </summary>
        [DataMemberAttribute()]
        public decimal FirstCount { get; set; }
        /// <summary>
        /// 首费金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal FirstCountPrice { get; set; }
        /// <summary>
        /// 增费标谁
        /// </summary>
        [DataMemberAttribute()]
        public decimal NextCount { get; set; }
        /// <summary>
        /// 增费金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal NextCountPrice { get; set; }
        /// <summary>
        /// 购买数量
        /// </summary>
        [DataMemberAttribute()]
        public decimal Count { get; set; }
        /// <summary>
        /// 物流重量（单位商品的重量）
        /// </summary>
        [DataMemberAttribute()]
        public decimal Weight { get; set; }
    }
}
