using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jinher.JAP.Common.TypeDefine;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 生成用户获奖记录
    /// </summary>
    [Serializable()]
    [DataContract]
    public partial class UserPrizeRecordDTO
    {
        

        /// <summary>
        /// 商品Id
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid CommodityId
        {
            get
            {
                return _CommodityId;
            }
            set
            {
                if (_CommodityId != value)
                {
                    _CommodityId = value;
                }
            }
        }

        private System.Guid _CommodityId;

        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMemberAttribute()]
        public Guid UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                if (_UserId != value)
                {
                    _UserId = value;
                }
            }
        }

        private Guid _UserId;

        /// <summary>
        /// 价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price
        {
            get
            {
                return _Price;
            }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                }
            }
        }

        private decimal _Price;

        /// <summary>
        /// 截止有效时间
        /// </summary>
        [DataMemberAttribute()]
        public System.DateTime ValTime
        {
            get
            {
                if (_ValTime.Equals(DateTime.MinValue))
                {
                    _ValTime = Constant.DbMinValue;
                }
                return _ValTime;
            }
            set
            {
                if (_ValTime != value)
                {
                    _ValTime = value;
                }
            }
        }

        private System.DateTime _ValTime;

        
        /// <summary>
        /// 活动Id
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid PromotionId
        {
            get
            {
                return _PromotionId;
            }
            set
            {
                if (_PromotionId != value)
                {
                    _PromotionId = value;
                }
            }
        }

        private System.Guid _PromotionId;

        
    }

    /// <summary>
    /// 用户获奖记录
    /// </summary>
    [Serializable()]
    [DataContract]
    public partial class PrizeRecordDTO : ResultDTO
    {


        /// <summary>
        /// 价格
        /// </summary>
        [DataMemberAttribute()]
        public decimal Price { get; set; }

        /// <summary>
        /// 是否已经购买
        /// </summary>
        [DataMemberAttribute()]
        public bool IsBuyed { get; set; }
    }
}
