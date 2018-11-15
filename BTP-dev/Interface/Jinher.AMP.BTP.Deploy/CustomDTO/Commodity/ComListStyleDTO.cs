using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{

    /// <summary>
    /// 商品列表配置
    /// </summary>
    [Serializable]
    [DataContract]
    public class ComListSettingDTO
    {
        /// <summary>
        /// 是否在列表中显示加入购物车
        /// </summary>
        [DataMember]
        public bool IsAddShopCartInComList { get; set; }
        /// <summary>
        /// 商品列表默认版式 0卡片式，1列表式
        /// </summary>
        public int GoodsDefaultFormat { get; set; }

    }
}
