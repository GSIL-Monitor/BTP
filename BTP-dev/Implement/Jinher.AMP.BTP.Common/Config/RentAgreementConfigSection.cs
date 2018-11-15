using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    public class RentAgreementConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("items", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(RentAgreementConfigSourceCollection), AddItemName = "add")]
        public RentAgreementConfigSourceCollection ConfigCollection
        {
            get { return (RentAgreementConfigSourceCollection)this["items"]; }
            set { this["items"] = value; }
        }
    }

    public class RentAgreementConfigSourceCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 创建新元素
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new RentAgreementConfigSource();
        }

        /// <summary>
        /// 获取元素的键
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RentAgreementConfigSource)element).Id;
        }

        /// <summary>
        /// 获取所有键
        /// </summary>
        public IEnumerable<string> AllKeys { get { return BaseGetAllKeys().Cast<string>(); } }
        /// <summary>
        /// 获取所有值
        /// </summary>
        /// <returns></returns>
        public List<RentAgreementConfigSource> GetAll()
        {
            List<RentAgreementConfigSource> result =new List<RentAgreementConfigSource>();
            int count = Count;
            for (var i = 0; i < count; i++)
            {
                result.Add(BaseGet(i) as RentAgreementConfigSource);
            }
            return result;
        }
         
    }

    public class RentAgreementConfigSource : ConfigurationElement
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [ConfigurationProperty("id")]
        public Guid Id
        {
            get { return (Guid)this["id"]; }
            set { this["id"] = value; }
        }

        /// <summary>
        /// 地址
        /// </summary>
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

  
    }
}
