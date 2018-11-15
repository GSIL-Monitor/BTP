using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 严选系统中订单信息
    /// </summary>
    public class OrderBaseVO
    {
        /// <summary>
        /// 订单号(最大128位)
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 下单时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string submitTime { get; set; }

        /// <summary>
        /// 支付时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public string payTime { get; set; }

        /// <summary>
        /// 买家用户名，可省略
        /// </summary>
        public string buyerAccount { get; set; }

        /// <summary>
        /// 收件人姓名	
        /// </summary>
        public string receiverName { get; set; }

        /// <summary>
        /// 收件人手机，可省略
        /// </summary>
        public string receiverMobile { get; set; }

        /// <summary>
        /// 收件人电话	
        /// </summary>
        public string receiverPhone { get; set; }

        /// <summary>
        /// 收件人省名称	
        /// </summary>
        public string receiverProvinceName { get; set; }

        /// <summary>
        /// 收件人市名称	
        /// </summary>
        public string receiverCityName { get; set; }

        /// <summary>
        /// 收件人区名称	
        /// </summary>
        public string receiverDistrictName { get; set; }

        /// <summary>
        /// 收件人详细地址	
        /// </summary>
        public string receiverAddressDetail { get; set; }

        /// <summary>
        /// 订单实付金额	
        /// </summary>
        public decimal realPrice { get; set; }

        /// <summary>
        /// 邮费
        /// </summary>
        public decimal expFee { get; set; }

        /// <summary>
        /// 支付方式	
        /// </summary>
        public string payMethod { get; set; }

        /// <summary>
        /// 发票抬头，可省略
        /// </summary>
        //public string invoiceTitle { get; set; }

        /// <summary>
        /// 发票金额，可省略
        /// </summary>
        //public string invoiceAmount { get; set; }
    }
}
