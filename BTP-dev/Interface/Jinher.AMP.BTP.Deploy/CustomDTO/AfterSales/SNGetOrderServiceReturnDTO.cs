using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales
{
    /// <summary>
    ///  商品服务类型返回
    /// </summary>
    [Serializable]
    [DataContract]
    public class SNGetOrderServiceReturnDTO
    {
        /// <summary>
        /// 是否支持开增票(01-支持；02-不支持)
        /// </summary>
        public string IncreaseTicket { get; set; }
        /// <summary>
        /// 支持无理由退货的天数
        /// </summary>
        public string NoReasonLimit { get; set; }
        /// <summary>
        /// 退货描述
        /// </summary>
        public string NoReasonTip { get; set; }
        /// <summary>
        /// 是否支持无理由退货(01-7天无理由退货；02-不支持退货)
        /// </summary>
        public string ReturnGoods { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string SkuId { get; set; }
    }
}
