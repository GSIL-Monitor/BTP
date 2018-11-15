using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 自提与负责人类
    /// </summary>
    [Serializable]
    [DataContract]
    public class SelfTakeStationAndManagerDTO
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
        /// 自提点负责人信息
        /// </summary>
        [DataMember]
        public List<SelfTakeStationManagerSDTO> selfTakeStationManager { get; set; }
        /// <summary>
        /// 类型：总代=0，电商馆=1
        /// </summary>
        [DataMember]
        public int SelfTakeStationType { get; set; }
        /// <summary>
        /// 应用Id，电商馆使用
        /// </summary>
        [DataMember]
        public Guid? AppId { get; set; }
    }
}
