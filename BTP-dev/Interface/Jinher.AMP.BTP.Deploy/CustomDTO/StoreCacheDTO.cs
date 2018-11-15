using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 门店信息(缓存使用)
    /// </summary>
    [Serializable()]
    [DataContract]
    public class StoreCacheDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember()]
        public Guid Id { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 电话
        /// </summary> 
        [DataMemberAttribute()]
        public string Phone { get; set; }
        /// <summary>
        /// 门店图片
        /// </summary>
        [DataMemberAttribute()]
        public string picture { get; set; }
        /// <summary>
        /// 地址
        /// </summary>  
        [DataMemberAttribute()]
        public string Address { get; set; }
        /// <summary>
        /// 省
        /// </summary> 
        [DataMemberAttribute()]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary> 
        [DataMemberAttribute()]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [DataMemberAttribute()]
        public string District { get; set; }
        //[DataMemberAttribute()]
        //public string CityCode { get; set; }
        //[DataMemberAttribute()]
        //public string Code { get; set; }
        //[DataMemberAttribute()]
        //public string DistrictCode { get; set; }
        //[DataMemberAttribute()]
        //public string ProvinceCode { get; set; }

        ///// <summary>
        ///// appid
        ///// </summary>
        //[DataMemberAttribute()]
        //public Guid AppId { get; set; }

        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }


        /// <summary>
        /// 地理位置x坐标
        /// </summary>
        [DataMember]
        public decimal XAxis { get; set; }
        /// <summary>
        /// 地理位置Y坐标
        /// </summary>
        [DataMember]
        public decimal YAxis { get; set; }
    }
}
