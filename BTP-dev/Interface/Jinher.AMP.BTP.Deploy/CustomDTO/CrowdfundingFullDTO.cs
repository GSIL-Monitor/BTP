using System;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 众筹详细信息DTO
    /// </summary>
    [Serializable]
    [DataContract]
    public class CrowdfundingFullDTO
    {
        /// <summary>
        /// 众筹Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid AppId { get; set; }
        /// <summary>
        /// 每股金额
        /// </summary>
        [DataMember]
        public decimal PerShareMoney { get; set; }
        /// <summary>
        /// 每股分红比例
        /// </summary>
        [DataMember]
        public decimal DividendPercent { get; set; }
        /// <summary>
        /// 总股数
        /// </summary>
        [DataMember]
        public long ShareCount { get; set; }
        /// <summary>
        /// 众筹开始时间
        /// </summary>
        [DataMember]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 众筹状态0,进行中|1众筹成功
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 众筹宣传语
        /// </summary>
        [DataMember]
        public string Slogan { get; set; }
        /// <summary>
        /// 众筹详情
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 已募得股数
        /// </summary>
        [DataMember]
        public long CurrentShareCount { get; set; }

        /// <summary>
        /// 剩余股点数
        /// </summary>
        [DataMember]
        public long SurplusShareCount { get; set; }
        /// <summary>
        /// 已分红金额
        /// </summary>
        [DataMember]
        public decimal TotalDividend { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public DateTime SubTime { get; set; }
        /// <summary>
        /// 状态：进行中、众筹成功
        /// </summary>
        [DataMember]
        public string strState { get; set; }
        /// <summary>
        /// 已募得股数/金额 
        /// </summary>
        [DataMember]
        public string strCurrentShareCount { get; set; }

        /// <summary>
        ///  剩余股点/金额
        /// </summary>
        [DataMember]
        public string strSurplusShareCount { get; set; }


        /// <summary>
        ///  百分比 2%
        /// </summary>
        [DataMember]
        public string strDividendPercent { get; set; }


        /// <summary>
        /// 已分红金额 
        /// </summary>
        [DataMember]
        public string strTotalDividend { get; set; }


    }
}
