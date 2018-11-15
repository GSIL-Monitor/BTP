using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 根据商品名称获取商品列表 参数
    /// </summary>
    [Serializable()]
    [DataContract]
    public class GetCommodityByNameParam
    {
        /// <summary>
        /// appid
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }

        /// <summary>
        /// appid
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }

        /// <summary>
        /// 只获取已设置过“分享分成”的记录。
        /// </summary>
        [DataMember]
        public bool OnlyShareMoney { get; set; }

        /// <summary>
        /// 商品分类(以,分隔的分类id)
        /// </summary>
        [DataMember]
        public string CommodityCategory { get; set; }

        /// <summary>
        /// 当前第几页
        /// </summary>
        [DataMember]
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数据行数
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 只获取已设置过积分抵用的记录。
        /// </summary>
        [DataMember]
        public bool OnlyScoreMoney { get; set; }

        /// <summary>
        /// 只获取已设置过推广佣金的记录。
        /// </summary>
        [DataMember]
        public bool OnlySpreadMoney{ get; set; }
    }
}
