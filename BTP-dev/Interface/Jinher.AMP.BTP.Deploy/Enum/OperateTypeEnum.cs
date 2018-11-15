using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.Enum
{
    /// <summary>
    /// 商品操作枚举
    /// </summary>
    [DataContract]
    public enum OperateTypeEnum
    {
        /// <summary>
        /// 商品发布
        /// </summary>
        [EnumMemberAttribute]
        商品发布 = 0,
        /// <summary>
        /// 商品编辑
        /// </summary>
        [EnumMemberAttribute]
        商品编辑 = 1,
        /// <summary>
        /// 商品上架
        /// </summary>
        [EnumMemberAttribute]
        商品上架 = 2,
        /// <summary>
        /// 修改商品名称
        /// </summary>
        [EnumMemberAttribute]
        修改名称 = 3,
        /// <summary>
        /// 修改商品类别
        /// </summary>
        [EnumMemberAttribute]
        修改类别 = 4,
        /// <summary>
        /// 修改现价
        /// </summary>
        [EnumMemberAttribute]
        修改现价 = 5,
        /// <summary>
        ///修改市场价
        /// </summary>
        [EnumMemberAttribute]
        修改市场价 = 6,
        /// <summary>
        /// 修改库存
        /// </summary>
        [EnumMemberAttribute]
        修改库存 = 7,
        /// <summary>
        /// 修改销量
        /// </summary>
        [EnumMemberAttribute]
        修改销量 = 8,
        /// <summary>
        /// 京东修改现价
        /// </summary>
        [EnumMemberAttribute]
        京东修改现价 = 9,
        /// <summary>
        /// 京东修改进货价
        /// </summary>
        [EnumMemberAttribute]
        京东修改进货价 = 10,
        /// <summary>
        /// 下架无货商品审核
        /// </summary>
        [EnumMemberAttribute]
        下架无货商品审核 = 11,
        /// <summary>
        ///修改进货价
        /// </summary>
        [EnumMemberAttribute]
        修改进货价 = 12,
        /// <summary>
        /// 其他
        /// </summary>
        [EnumMemberAttribute]
        其他 = 999
    }
}
