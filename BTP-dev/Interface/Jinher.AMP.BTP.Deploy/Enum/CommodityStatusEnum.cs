using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 商品状态
    /// </summary>
    [DataContract]
    public enum CommodityStatusEnum
    {
        /// <summary>
        /// 上架
        /// </summary>
        [EnumMemberAttribute]
        Ready = 0,
        /// <summary>
        /// 未上架
        /// </summary>
        [EnumMemberAttribute]
        off = 1,
        /// <summary>
        /// 仓库
        /// </summary>
        [EnumMemberAttribute]
        Warehouse = 2
    }
}
