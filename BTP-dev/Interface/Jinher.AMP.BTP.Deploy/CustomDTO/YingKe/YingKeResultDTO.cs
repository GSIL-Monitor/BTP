using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO.YingKe
{
    /// <summary>
    /// 盈科接口返回值
    /// </summary>
    [Serializable]
    [DataContract]
    public class YingKeResultDTO
    {
        /// <summary>
        /// 接口是否成功(SUCCESS|FAIL)
        /// </summary>
        [DataMember]
        public string status { get; set; }

        /// <summary>
        /// 接口返回数据(成功则为电子券信息，失败则为错误信息)
        /// </summary>
        [DataMember]
        public string msg { get; set; }

        /// <summary>
        /// 接口是否成功
        /// </summary>
        public bool IsSuccess
        {
            get { return !string.IsNullOrEmpty(status) && status.ToLower() == "success"; }
        }
    }
}
