using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [DataContract]
    public class BtpScoreDTO
    {
        public BtpScoreDTO()
        {
            LastRecordTime = DateTime.Now;
            Evaluate = new EvaluationDTO();
            Records = new List<ScoreDTO>();
        }
        /// <summary>
        /// 评价统计
        /// </summary>
        [DataMember]
        public EvaluationDTO Evaluate { get; set; }
        /// <summary>
        /// 最后评价时间
        /// </summary>
        [DataMember]
        public DateTime LastRecordTime { get; set; }
        /// <summary>
        /// 评价记录
        /// </summary>
        [DataMember]
        public List<ScoreDTO> Records { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        [DataMember]
        public double Score { get; set; }
        /// <summary>
        /// 总评价数量
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }
    }
    /// <summary>
    /// 评价统计
    /// </summary>
    [DataContract]
    public class EvaluationDTO
    {
        /// <summary>
        /// 差评数量
        /// </summary>
        [DataMember]
        public int BadCount { get; set; }
        /// <summary>
        /// 好评数量
        /// </summary>
        [DataMember]
        public int GoodCount { get; set; }
        /// <summary>
        /// 中评数量
        /// </summary>
        [DataMember]
        public int MediumCount { get; set; }
    }
    /// <summary>
    /// 评价
    /// </summary>
    [DataContract]
    public class ScoreDTO
    {
        /// <summary>
        /// 评价id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 评价人id
        /// </summary>
        [DataMember]
        public Guid UserId { get; set; }
        /// <summary>
        /// 评价人用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 评价时间
        /// </summary>
        [DataMember]
        public string ShowTime { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [DataMember]
        public string Content { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [DataMember]
        public List<string> PhotosArr { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        [DataMember]
        public bool Anonymous { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string Icon { get; set; }
    }
}
