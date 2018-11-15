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
    public class ZCategoryDTO
    {
        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 分类列表
        /// </summary>
        [DataMember]
        public List<CategorySDTO> Data { get; set; }

    }

    /// <summary>
    /// 分类列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CategoryDto
    {
        /// <summary>
        /// 记录数
        /// </summary>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// 分类列表
        /// </summary>
        [DataMember]
        public List<Categorysdto> Data { get; set; }

    }



    /// <summary>
    /// 分类列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class Categorysdto
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

      

    }


    /// <summary>
    /// 分类列表
    /// </summary>
    [Serializable()]
    [DataContract]
    public class CategorySDTO
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
        /// 排序
        /// </summary>
        [DataMemberAttribute()]
        public int Sort { get; set; }

        /// <summary>
        /// 次级分类
        /// </summary>
        [DataMemberAttribute()]
        public List<SCategorySDTO> SecondCategory { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [DataMemberAttribute()]
        public string PicturesPath { get; set; }

    }
}
