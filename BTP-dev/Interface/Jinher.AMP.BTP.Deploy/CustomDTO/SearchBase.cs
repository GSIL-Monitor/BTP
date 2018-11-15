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
    public class SearchBase
    {
        private string _sortedColName = string.Empty;

        /// <summary>
        /// 排序字段名（只支持单个字段）
        /// </summary>
        [DataMember]
        public string SortedColName
        {
            get { return _sortedColName; }
            set { _sortedColName = value; }
        }

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

        private int _orderState = 1;
        /// <summary>
        /// 排序类型，1为升序；0，降序
        /// </summary>
        [DataMember]
        public int OrderState
        {
            get { return _orderState; }
            set { _orderState = value; }
        }
    }
}
