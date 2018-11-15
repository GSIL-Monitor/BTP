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
    public class UserOrderCountDTO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// 显示哪个馆的商品
        /// </summary>
        [DataMemberAttribute()]
        public Guid EsAppId { get; set; }
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

        public UserOrderCountDTO()
        {

        }

        /// <summary>
        /// 根据OrderListCDTO生成DTO
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="esAppId">馆Id</param>
        /// <param name="orderListCDTO">订单Model</param>
        public UserOrderCountDTO(Guid userId, Guid esAppId, OrderListCDTO orderListCDTO)
        {
            UserId = userId;
            EsAppId = esAppId;
            if (orderListCDTO != null)
            {
                OrderTotalState0 = orderListCDTO.totalState0;
                OrderTotalState1 = orderListCDTO.totalState1;
                OrderTotalState2 = orderListCDTO.totalState2;
                OrderTotalState3 = orderListCDTO.totalState3;
                OrderTotalStateTui = orderListCDTO.totalStateTui;
            }
        }
    }
}
