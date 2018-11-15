using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 地址详情DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class AddressSDTO
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId { get; set; }
        /// <summary>
        /// 收货地址ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AddressId { get; set; }
        /// <summary>
        /// 收货人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptUserName { get; set; }
        /// <summary>
        /// 收货人电话
        /// </summary>    
        [DataMemberAttribute()]
        public string ReceiptPhone { get; set; }
        /// <summary>
        /// 收货人地址
        /// </summary>
        [DataMemberAttribute()]
        public string ReceiptAddress { get; set; }
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
        ///  街道
        /// </summary>
        [DataMemberAttribute()]
        public string Street{ get; set; }

        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        [DataMemberAttribute()]
        public string RecipientsZipCode { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMemberAttribute()]
        public int? IsDefault { get; set; }

        /// <summary>
        /// 省编码
        /// </summary>
        [DataMemberAttribute()]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 市编码
        /// </summary>
        [DataMemberAttribute()]
        public string CityCode { get; set; }

        /// <summary>
        /// 区编码
        /// </summary>
        [DataMemberAttribute()]
        public string DistrictCode { get; set; }


        /// <summary>
        /// 街道编码
        /// </summary>
        [DataMemberAttribute()]
        public string StreetCode { get; set; }
    }
}
