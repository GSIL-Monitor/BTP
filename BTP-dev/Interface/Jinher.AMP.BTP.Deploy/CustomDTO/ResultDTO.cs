using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 返回结果
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ResultDTO
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public virtual bool isSuccess { get; set; }
        /// <summary>
        /// 返回代号
        /// </summary>
        [DataMemberAttribute()]
        public int ResultCode { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        [DataMemberAttribute()]
        public string Message { get; set; }

    }

    /// <summary>
    /// 返回结果
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ResultDTO<T> : ResultDTO
    {
        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public T Data { get; set; }



    }



    /// <summary>
    /// 返回结果
    /// </summary>
    [Serializable()]
    [DataContract]
    public class NewResultDTO : ResultDTO
    {
        /// <summary>
        /// 返回运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal Freight { get; set; }

        /// <summary>
        /// 是否修改过价格
        /// </summary>
        [DataMemberAttribute()]
        public int IsModifiedPrice { get; set; }
        /// <summary>
        /// 不支付超时时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 优惠券抵用金额。
        /// </summary>
        [DataMemberAttribute()]
        public decimal CouponAmount { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        [DataMemberAttribute()]
        public decimal Duty { get; set; }

    }
    /// <summary>
    /// 返回结果
    /// </summary>
    [Serializable()]
    [DataContract]
    [KnownType(typeof(OrderShippingExtDTO))]
    public class ResultShipDTO : ResultDTO
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public OrderShippingExtDTO OrderShippingExt { get; set; }

    }

    /// <summary>
    /// 结果列表
    /// </summary>
    [Serializable()]
    [DataContract]
    [KnownType(typeof(OrderShippingExtDTO))]
    public class ListResult<T>
    {
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        [DataMember]
        public List<T> List { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommodityPriceFloatList<T> : ListResult<T>
    {
        [DataMember]
        public List<OptionModel> Apps { get; set; }
    }

}
