using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 门店列表DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class StoreSDTO
    {
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMemberAttribute()]
        public string StoreName { get; set; }
        /// <summary>
        /// 电话
        /// </summary> 
        [DataMemberAttribute()]
        public List<PhoneSDTO> Phone { get; set; }
        /// <summary>
        /// 门店图片
        /// </summary>
        [DataMemberAttribute()]
        public string PicPath { get; set; }
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
        /// <summary>
        /// 分页
        /// </summary>
        [DataMember]
        public ParamDTO ParamSDTO { get; set; }

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

        /// <summary>
        ///当前位置到门店的直线距离
        /// </summary>
        [DataMember]
        public decimal Distance { get; set; }
    }
}
