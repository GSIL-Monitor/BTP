using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// QryCommodityDTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public class QryCommodityDTO
    {
        /// <summary>
        /// SetCategoryId
        /// </summary>
        [DataMemberAttribute()]
        public Guid SetCategoryId { get; set; }
        /// <summary>
        /// PageIndex
        /// </summary>
        [DataMemberAttribute()]
        public int PageIndex { get; set; }
        /// <summary>
        /// PageSize
        /// </summary>
        [DataMemberAttribute()]
        public int PageSize { get; set; }
        /// <summary>
        /// FieldSort
        /// </summary>
        [DataMemberAttribute()]
        public int FieldSort { get; set; }
        /// <summary>
        /// Order
        /// </summary>
        [DataMemberAttribute()]
        public string Order { get; set; }

        /// <summary>
        /// 是否有库存
        /// </summary>
        [DataMember]
        public bool IsHasStock { get; set; }

        /// <summary>
        /// 最小价格
        /// </summary>
        [DataMember]
        public decimal? MinPrice { get; set; }
        /// <summary>
        /// 最大价格
        /// </summary>
        [DataMember]
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// 正品会Id
        /// </summary>
        [DataMember]
        public Guid ChannelId { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid? AppId { get; set; }

    }
}
