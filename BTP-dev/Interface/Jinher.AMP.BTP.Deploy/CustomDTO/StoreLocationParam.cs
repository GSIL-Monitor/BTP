using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 获取门店列表（按用户当前位置到门店的距离排序）
    /// </summary>
    [Serializable()]
    [DataContract]
    public class StoreLocationParam : ParamDTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        [DataMember()]
        public Guid AppId { get; set; } 

        /// <summary>
        /// 经度
        /// </summary>
        [DataMember()]
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [DataMember()]
        public decimal Latitude { get; set; }
        /// <summary>
        /// 门店名称(模糊匹配)
        /// </summary>
        [DataMember]
        public string SubName { get; set; }
        /// <summary>
        /// 最大距离（单位：米），-1代表不限
        /// </summary>
        [DataMember()]
        public decimal MaxDistance { get; set; }

         
    }
}
