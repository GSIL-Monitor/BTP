using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 数据库公共字段
    /// </summary>
    [DataContract]
    [Serializable]
    public class DBBase
    {
        /// <summary>
        /// 标识ID
        /// </summary>
        [DataMember]
        public Guid id { get; set; }
        /// <summary>
        /// 提交人
        /// </summary>
        [DataMember]
        public Guid subId { get; set; }
        private DateTime _subTime;
        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime subTime
        {
            get
            {
                if (_subTime == DateTime.MinValue)
                    return DateTime.MinValue.AddDays(1);
                return _subTime;
            }
            set
            {
                _subTime = value;
            }
        }
        private DateTime _modifiedOn;
        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        public DateTime modifiedOn
        {
            get
            {
                if (_modifiedOn == DateTime.MinValue)
                    return DateTime.MinValue.AddDays(1);
                return _modifiedOn;
            }
            set
            {
                _modifiedOn = value;
            }
        }
    }
}
