using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class ApplyNameCategoryConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("items", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ApplyNameCategoryConfigSourceCollection), AddItemName = "add")]
        public ApplyNameCategoryConfigSourceCollection ConfigCollection
        {
            get { return (ApplyNameCategoryConfigSourceCollection)this["items"]; }
            set { this["items"] = value; }
        }
    }

    public class ApplyNameCategoryConfigSourceCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 创建新元素
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApplyNameCategoryConfigSource();
        }

        /// <summary>
        /// 获取元素的键
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApplyNameCategoryConfigSource)element).Category;
        }

        /// <summary>
        /// 获取所有键
        /// </summary>
        public IEnumerable<string> AllKeys { get { return BaseGetAllKeys().Cast<string>(); } }
        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public List<ApplyNameCategoryConfigSource> GetAll()
        {
            List<ApplyNameCategoryConfigSource> result =new List<ApplyNameCategoryConfigSource>();
            int count = Count;
            for (var i = 0; i < count; i++)
            {
                result.Add(BaseGet(i) as ApplyNameCategoryConfigSource);
            }
            return result;
        }
         
    }

    public class ApplyNameCategoryConfigSource : ConfigurationElement
    {
        /// <summary>
        /// 字段类型
        /// </summary>
        [ConfigurationProperty("category")]
        public int Category 
        {
            get { return (int)this["category"]; }
            set { this["category"] = value; }
        }

        /// <summary>
        /// 字段名称
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

  
    }
}
