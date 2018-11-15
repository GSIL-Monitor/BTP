using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 结算列表WEB页
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SettlingAccountVM
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid? Id { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityId { get; set; }
        /// <summary>
        /// 厂家结算金额
        /// </summary>
        [DataMemberAttribute()]
        public decimal? ManufacturerClearingPrice { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 是否有效 (暂时附值为0)
        /// </summary>
        [DataMemberAttribute()]
        public int? Effectable { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? EffectiveTime { get; set; }
        /// <summary>
        /// 提交人ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid? SubId { get; set; }
        /// <summary>
        /// 提交人姓名
        /// </summary>
        [DataMemberAttribute()]
        public string SubName { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? SubTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ModifiedOn { get; set; }
        /// <summary>
        /// 提交人账号
        /// </summary>
        [DataMemberAttribute()]
        public string UserCode { get; set; }



        /// <summary>
        /// 商品图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMemberAttribute()]
        public string CommodityName { get; set; }
        /// <summary>
        /// 销售金额IsEnableSelfTake
        /// </summary>
        [DataMemberAttribute()]
        public Decimal? SalePrice { get; set; }
        /// <summary>
        /// 是否自提
        /// </summary>
        [DataMemberAttribute()]
        public int IsEnableSelfTake { get; set; }
    }
}
