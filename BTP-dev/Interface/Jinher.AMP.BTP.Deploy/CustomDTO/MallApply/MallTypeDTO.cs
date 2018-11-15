using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 应用信息DTO
    /// </summary>
    [DataContract]
    [Serializable]
    public class MallTypeDTO
    {
        /// <summary>
        /// AppId
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        ///  商家类型（0-自营他配；1-第三方；2-自营自配自采；3-自营自配统采）
        /// </summary>
        [DataMember]
        public short Type { get; set; }
    }
}
