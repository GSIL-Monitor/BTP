using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 自提点信息返回结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class SelfTakeStationResultDTO
    {
        public SelfTakeStationResultDTO()
        {
            SelfTakeStationList = new List<SelfTakeStationSearchResultDTO>();
        }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 自提点信息列表
        /// </summary>
        [DataMember]
        public List<SelfTakeStationSearchResultDTO> SelfTakeStationList { get; set; }

    }
    /// <summary>
    /// 自提点信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SelfTakeStationSearchResultDTO
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 城市总代Id
        /// </summary>
        [DataMember]
        public Guid CityOwnerId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 省   
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [DataMember]
        public string City { get; set; }
        /// <summary>
        /// 区县
        /// </summary>
        [DataMember]
        public string District { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 推广Url
        /// </summary>
        [DataMember]
        public string SpreadUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        /// <summary>
        /// 自提点地址(省+市+详细地址)
        /// </summary>
        [DataMember]
        public string AddressDetail { get; set; }
        /// <summary>
        /// 二维码地址
        /// </summary>
        [DataMember]
        public string QRCodeUrl { get; set; }
        /// <summary>
        /// 推广码
        /// </summary>
        [DataMember]
        public Guid SpreadCode { get; set; }
        /// <summary>
        /// 自提电话号码
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
    }
}
