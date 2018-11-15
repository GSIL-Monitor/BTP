using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 正品会收藏查询
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class SetCollectionSearchDTO : SearchBase
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember()]
        public Guid UserId { get; set; }

        /// <summary>
        /// 渠道Id
        /// </summary>
        [DataMember()]
        public Guid ChannelId { get; set; }
        /// <summary>
        /// 收藏类型   1：商品收藏，2：店铺收藏
        /// </summary>
        [DataMember()]
        public int ColType { get; set; }
        /// <summary>
        /// 收藏Id列表
        /// </summary>
        [DataMember()]
        public List<Guid> ColKeyList { get; set; }
    }

}