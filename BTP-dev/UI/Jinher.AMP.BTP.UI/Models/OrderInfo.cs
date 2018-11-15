using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    public class OrderInfo
    {
        /// <summary>
        /// app的Id
        /// </summary>
        public Guid AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserPhoto
        {
            get;
            set;
        }

        /// <summary>
        /// 订单中商品数量
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string CommdityName
        {
            get;
            set;
        }

        /// <summary>
        /// 销量
        /// </summary>
        public int SalesNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 收藏量
        /// </summary>
        public int CollectNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 商品缩略图
        /// </summary>
        public string CommdityImage
        {
            get;
            set;
        }

        /// <summary>
        /// 实际支付价格
        /// </summary>
        public decimal? RealPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 距离促销还剩下多少天
        /// </summary>
        public int Days
        {
            get;
            set;
        }
    }
}