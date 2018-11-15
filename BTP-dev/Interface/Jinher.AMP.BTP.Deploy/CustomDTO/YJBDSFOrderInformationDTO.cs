using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 用于数据接收和传输
    /// </summary>
    [Serializable()]
    [DataContract]
    public class YJBDSFOrderInformationDTO
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public String OrderNo { get; set; }
        /// <summary>
        /// 平台名称
        /// </summary>
        [DataMember]
        public string PlatformName { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public string OrderPayState { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        [DataMember]
        public string OrderPayDate { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal OrderPayMoney { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        [DataMember]
        public List<DSFCommodityDTO> Commodity { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }
        /// <summary>
        /// 提交人Id
        /// </summary>
        [DataMember]
        public Guid SubId { get; set; }
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
        /// 订单手机号（获取UseId用）
        /// </summary>
        [DataMember]
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// 商品实体
    /// </summary>
    [Serializable()]
    [DataContract]
    public class DSFCommodityDTO
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        [DataMember]
        public int Num { get; set; }
        /// <summary>
        /// 商品缩略图
        /// </summary>
        [DataMember]
        public string Thumbnail { get; set; }
    }
}
