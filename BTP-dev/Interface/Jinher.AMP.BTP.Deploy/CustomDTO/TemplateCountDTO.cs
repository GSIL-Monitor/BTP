using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品的运费计算类（同一商品由于属性不同，传的金额可能不同）
    /// </summary>
    [Serializable()]
    [DataContract]
    public class TemplateCountDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 购买数量(重量)
        /// </summary>
        [DataMemberAttribute()]
        public decimal Count { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }
    }

    //[Serializable]
    //[DataContract]
    //public class CalcCouponFreightDTO
    //{
    //    /// <summary>
    //    /// 商品ID
    //    /// </summary>
    //    [DataMember]
    //    public Guid CommodityId { get; set; }
    //    /// <summary>
    //    /// 商品单价
    //    /// </summary>
    //    [DataMember]
    //    public decimal Price { get; set; }
    //}
}
