using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品自提类
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class CommoditySelfTakeListDTO
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }

        /// <summary>
        /// 是否支持自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }

    }
}
