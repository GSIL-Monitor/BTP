using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 应用相关接口搜索dto
    /// </summary>
    [Serializable]
    [DataContract]
    public class AppExtDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否正品会应用
        /// </summary>
        [DataMember]
        public bool IsZphApp { get; set; }
        /// <summary>
        /// 商品列表是否显示加入购物车
        /// </summary>
        [DataMember]
        public bool IsShowAddCart { get; set; }
    }
}
