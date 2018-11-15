using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 次级分类列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SCategorySDTO
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid Id { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }
        /// <summary>
        /// 分类编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        [DataMemberAttribute()]
        public int CurrentLevel { get; set; }
        /// <summary>
        /// 父级全路径
        /// </summary>
        [DataMemberAttribute()]
        public string ParentCategoryPath { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMemberAttribute()]
        public int Sort { get; set; }
        /// <summary>
        /// 三级分类
        /// </summary>
         [DataMemberAttribute()]
        public List<TCategorySDTO> ThirdCategory { get; set;}
         /// <summary>
         /// 图片路径
         /// </summary>
         [DataMemberAttribute()]
         public string PicturesPath { get; set; }
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
    }
}
