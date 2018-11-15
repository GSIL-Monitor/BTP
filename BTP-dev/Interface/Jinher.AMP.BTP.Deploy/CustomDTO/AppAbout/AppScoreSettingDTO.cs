using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 积分抵现设置dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppScoreSettingDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否启用积分抵现
        /// </summary>
        [DataMember]
        public bool IsCashForScore { get; set; }
        /// <summary>
        /// 商品列表是否显示加入购物车
        /// </summary>
        [DataMember]
        public int ScoreCost { get; set; }
    }
}
