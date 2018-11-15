using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 更新QrCode的请求DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class UpdateQrCodeRequestDTO
    {
        /// <summary>
        /// 微小店Id
        /// </summary>
        [DataMember]
        public Guid MicroShopId { get; set; }

        /// <summary>
        /// 微小店Id
        /// </summary>
        [DataMember]
        public string QRCodeUrl { get; set; }
    }
}
