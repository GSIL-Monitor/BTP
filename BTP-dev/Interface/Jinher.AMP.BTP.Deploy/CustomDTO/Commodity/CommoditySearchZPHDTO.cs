using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 商品检索类
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class CommoditySearchZPHDTO : SearchBase
    {
        /// <summary>
        /// 应用名称（模糊查询）
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 商品所在appId，
        /// </summary>
        [DataMember()]
        public Guid? AppId { get; set; }
        /// <summary>
        /// 商品名称（模糊查询）
        /// </summary>
        [DataMember]
        public string CommodityName { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        public string CommodityCode { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMember]
        public Guid? CommodityId { get; set; }
        /// <summary>
        /// 正品会活动Id
        /// </summary>
        [DataMember]
        public Guid? ActId { get; set; }
        /// <summary>
        /// 商品服务项ID
        /// </summary>
        [DataMember]
        public Guid ServiceSettingId { get; set; }
        /// <summary>
        ///是否已关联表 0已关联  1 未关联
        /// </summary>
        [DataMember]
        public int IsJoinServiceSetting { get; set; }

    }
}
