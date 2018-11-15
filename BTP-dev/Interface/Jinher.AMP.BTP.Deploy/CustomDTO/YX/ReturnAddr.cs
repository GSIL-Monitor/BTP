using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YX
{
    /// <summary>
    /// 退货地址信息
    /// </summary>
    public class RefundAddress
    {
        /// <summary>
        /// 申请单Id
        /// </summary>
        public string applyId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 申请单类型   1：无理由(普通退货)，2：质量问题(顺丰到付)
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 退货地址
        /// </summary>
        public ReturnAddr returnAddr { get; set; }
    }

    /// <summary>
    /// 退货地址详情信息
    /// </summary>
    public class ReturnAddr
    {
        /// <summary>
        /// 省份名称	
        /// </summary>
        public string provinceName { get; set; }

        /// <summary>
        /// 城市名称	
        /// </summary>
        public string cityName { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string districtName { get; set; }

        /// <summary>
        /// 具体街道地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 完整地址
        /// </summary>
        public string fullAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 收件人手机
        /// </summary>
        public string mobile { get; set; }
    }
}
