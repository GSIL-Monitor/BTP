using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 查询基类
    /// </summary>
    [Serializable]
    [DataContract]
    public class RefundInfoDTO
    {
        private int _pageSize = 20;
        /// <summary>
        /// 每页返回的最大数据条数 默认为20条
        /// </summary>
        [DataMember]
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        private int _pageIndex = 1;

        /// <summary>
        /// 返回第几页的数据 默认返回第一页
        /// </summary>
        [DataMember]
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderId { get; set; }

        /// <summary>
        /// 订单单项ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid CommodityOrderItemId { get; set; }
    }
}
