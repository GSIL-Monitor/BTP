using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    ///商品操作类型枚举
    /// </summary>    
    [DataContract]
    public enum OperateCommodiyEnum
    {
        /// <summary>
        /// 发布
        /// </summary>
        [EnumMemberAttribute]
        Publish = 0,
        /// <summary>
        /// 编辑
        /// </summary>
        [EnumMemberAttribute]
        Redact = 1,
        /// <summary>
        /// 上架
        /// </summary>
        [EnumMemberAttribute]
        OnSale = 2,
        /// <summary>
        /// 修改名称
        /// </summary>
        [EnumMemberAttribute]
        EditName = 3,
        /// <summary>
        /// 修改类别
        /// </summary>
        [EnumMemberAttribute]
        EditSort = 4,
        /// <summary>
        /// 修改价格
        /// </summary>
        [EnumMemberAttribute]
        EditPrice = 5,        
        /// <summary>
        /// 修改市场价
        /// </summary>
        [EnumMemberAttribute]
        EditMarket = 6,
        /// <summary>
        /// 修改库存
        /// </summary>
        [EnumMemberAttribute]
        EditStock = 7,
        /// <summary>
        /// 修改库存
        /// </summary>
        [EnumMemberAttribute]
        Others =999        
    }
}
