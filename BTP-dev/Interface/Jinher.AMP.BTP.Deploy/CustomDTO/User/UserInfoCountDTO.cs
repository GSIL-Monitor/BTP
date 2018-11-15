using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class UserInfoCountDTO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }

        /// <summary>
        /// 未付款订单数量
        /// </summary>
        [DataMemberAttribute()]
        public int OrderTotalState0 { get; set; }

        /// <summary>
        /// 未发货订单数量
        /// </summary>
        [DataMemberAttribute()]
        public int OrderTotalState1 { get; set; }
        /// <summary>
        /// 已发货订单数量
        /// </summary>
        [DataMemberAttribute()]
        public int OrderTotalState2 { get; set; }
        /// <summary>
        /// 已完成订单数量
        /// </summary>
        [DataMemberAttribute()]
        public int OrderTotalState3 { get; set; }
        /// <summary>
        /// 失败订单数量
        /// </summary>
        [DataMemberAttribute()]
        public int OrderTotalStateTui { get; set; }

        /// <summary>
        /// 收藏商品数量
        /// </summary>
        [DataMemberAttribute()]
        public int ColCommodityCnt { get; set; }
        /// <summary>
        /// 收藏店铺数量
        /// </summary>
        [DataMemberAttribute()]
        public int ColAppCnt { get; set; }

        /// <summary>
        /// 预约数量
        /// </summary>
        [DataMemberAttribute()]
        public int ForespeakCnt { get; set; }

        /// <summary>
        /// 金币
        /// </summary>
        [DataMemberAttribute()]
        public ulong Gold { get; set; }

        /// <summary>
        /// 用户图像url
        /// </summary>
        [DataMemberAttribute()]
        public string PicUrl { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMemberAttribute()]
        public string UserName { get; set; }

        /// <summary>
        /// 显示哪个馆的商品
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }
        /// <summary>
        /// 浏览记录
        /// </summary>
        [DataMemberAttribute()]
        public int BrowseCount { get; set; }
        
    }
}
