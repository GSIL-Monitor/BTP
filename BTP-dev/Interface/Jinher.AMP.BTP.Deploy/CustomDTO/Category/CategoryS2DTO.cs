using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分类列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CategoryS2DTO : CategorySDTO
    {
        /// <summary>
        /// 子分类
        /// </summary>
        [DataMemberAttribute()]
        public List<CategoryS2DTO> ChildCategory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public string Icno { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        public bool IsUse { get; set; }
        
        /// <summary>
        /// 分类所对应的品类广告
        /// </summary>
        [DataMember]
        public CategoryAdvertiseDTO CategoryAdvertise { get; set; }


        [DataMember]
        public IList<BrandwallDTO> BrandWallDto { get; set; }

    }
}
