using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.SN
{
    public class SNPriceDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string skuId { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 易购价
        /// </summary>
        public string snPrice { get; set; }
        /// <summary>
        /// 折扣率
        /// </summary>
        public string discountRate { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public string tax { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public string taxprice { get; set; }
        /// <summary>
        /// 裸价
        /// </summary>
        public string nakedprice { get; set; }
    }

    public class SNPriceMessageDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        public string cmmdtyCode { get; set; }
        /// <summary>
        /// 城市编码
        /// </summary>
        public string cityId { get; set; }
        /// <summary>
        /// 变动时间
        /// </summary>
        public DateTime time { get; set; }
    }
}
