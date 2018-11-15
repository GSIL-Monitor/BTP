using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 后台订单分页查询结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class CommodityOrderListDTO
    {
        /// <summary>
        /// 总数
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        [DataMember]
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityOrderVM> Data { get; set; }
    }
}
