using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 运费列表DTO
    /// </summary>
    [Serializable()]
    [DataContract]
    public partial class FreightDTO
    {
        public FreightDTO()
        {
            RangeFreights = new List<FreightRangeDTO>();
            DefaultRangeFreightDetails = new List<FreightRangeDefaultDetailsDTO>();
            SpecificRangeFreightDetails = new List<FreightRangeSpecificDetailsDTO>();
        }

        #region 简单类型属性
        /// <summary>
        /// 模板编号
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 运送方式：快递=0，EMS=1，平邮=2
        /// </summary>
        [DataMemberAttribute()]
        public int FreightMethod { get; set; }


        /// <summary>
        /// 运送到
        /// </summary>
        [DataMemberAttribute()]
        public string FreightTo { get; set; }


        /// <summary>
        /// 首件
        /// </summary>
        [DataMemberAttribute()]
        public decimal FirstCount { get; set; }


        /// <summary>
        /// 首件运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal FirstCountPrice { get; set; }


        /// <summary>
        /// 续件
        /// </summary>
        [DataMemberAttribute()]
        public decimal NextCount { get; set; }

        /// <summary>
        /// 续件运费
        /// </summary>
        [DataMemberAttribute()]
        public decimal NextCountPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <LongDescription>
        /// 是否包邮
        /// </LongDescription>
        [DataMemberAttribute()]
        public bool IsFreeExp { get; set; }

        /// <summary>
        /// 没有元数据文档可用。
        /// </summary>
        [DataMemberAttribute()]
        public System.Guid AppId { get; set; }



        /// <summary>
        /// 计价方式：0按件数，1按重量，2按体积，3按价格区间
        /// </summary>
        [DataMemberAttribute()]
        public int PricingMethod
        {
            get;
            set;
        }


        /// <summary>
        /// 包邮类型：0不包邮，1包邮，2部分包邮
        /// </summary>
        [DataMemberAttribute()]
        public byte ExpressType
        {
            get;
            set;
        }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime SubTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMemberAttribute()]
        public DateTime ModifiedOn { get; set; }



        /// <summary>
        /// 运费模板详情。
        /// </summary>
        [DataMemberAttribute()]
        public List<FreightTemplateDetailDTO> FreightDetailList { get; set; }


        /// <summary>
        /// 部分地区包邮信息。
        /// </summary>
        [DataMemberAttribute()]
        public List<FreightPartialFreeExtDTO> PartialFreeList { get; set; }

        /// <summary>
        /// 区间运费
        /// </summary>
        [DataMember]
        public List<FreightRangeDTO> RangeFreights { get; set; }

        /// <summary>
        /// 默认区间运费明细
        /// </summary>
        [DataMember]
        public List<FreightRangeDefaultDetailsDTO> DefaultRangeFreightDetails { get; set; }

        /// <summary>
        /// 特定区间运费明细
        /// </summary>
        [DataMember]
        public List<FreightRangeSpecificDetailsDTO> SpecificRangeFreightDetails { get; set; }

        /// <summary>
        /// 获取 是否是价格区间运费模板
        /// </summary>
        [DataMember]
        public bool IsRangeFreight
        {
            get { return PricingMethod == 3; }
            set
            {
                /*
                 *  写这个 set 就是为了防止 DTO 自动映射的时候报下面的这个错误
                 * 
                 *  没有类型为“Jinher.AMP.BTP.Deploy.CustomDTO.FreightDTO”，属性为“IsRangeFreight”的 Set 方法。
                 */
            }
        }
        #endregion
    }
}
