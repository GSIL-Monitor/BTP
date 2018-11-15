using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 订单状态修改 参数实体。
    /// </summary>
    [Serializable]
    [DataContract]
    public class UpdateCommodityOrderParamDTO
    {

        /// <summary>
        /// 订单目标状态
        /// </summary>
        [DataMember]
        public int targetState { get; set; }


        /// <summary>
        /// 订单id
        /// </summary>
        [DataMember]
        public Guid orderId { get; set; }

        /// <summary>
        /// 订单项id
        /// </summary>
        [DataMember]
        public Guid orderItemId { get; set; }

        /// <summary>
        /// 当前用户Id
        /// </summary>
        [DataMember]
        public Guid userId { get; set; }

        /// <summary>
        /// 应用id
        /// </summary>
        [DataMember]
        public Guid appId { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public int payment { get; set; }

        /// <summary>
        /// 金币支付密码
        /// </summary>
        [DataMember]
        public string goldpwd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string remessage { get; set; }


        #region 直接到账状态变为7（已退款）时使用。

        /// <summary>
        /// 收款人账号
        /// </summary>
        [DataMember]
        public string ReceiverAccount { get; set; }


        /// <summary>
        /// 收款人姓名
        /// </summary>
        [DataMember]
        public string Receiver { get; set; }


        /// <summary>
        /// 退款金额
        /// </summary>
        [DataMember]
        public decimal RefundMoney { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string RefundDesc { get; set; }

        #endregion

        #region 餐饮订单整单、部分退款

        /// <summary>
        /// 餐饮订单退款方式
        /// 0：整单，1：部分
        /// </summary>
        public int CYRefundType { get; set; }

        /// <summary>
        /// 餐饮订单部分退款，部分商品Id列表
        /// </summary>
        public List<Guid> OrderItemIds { get; set; }

        /// <summary>
        /// 部分退款是否包含运费
        /// </summary>
        public bool PartRefundHasFreight { get; set; }

        #endregion 

        /// <summary>
        /// 订单拒收运费
        /// </summary>
        public decimal RejectFreightMoney { get; set; }
    }
}
