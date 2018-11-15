using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 用于晒单
    /// </summary>
    [Serializable()]
    [DataContract]
    public class OrderForShareDTO
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        [DataMember]
        public string UserPhoto
        {
            get;
            set;
        }

        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// 实际支付价格
        /// </summary>
        [DataMember]
        public decimal? RealPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 距离促销还剩下多少天
        /// </summary>
        [DataMember]
        public int Days
        {
            get;
            set;
        }
        /// <summary>
        /// 订单中随机的一件商品信息
        /// </summary>
        [DataMember]
        public CommodityDTO CommodityDTO
        {
            get;
            set;
        }
    }
}
