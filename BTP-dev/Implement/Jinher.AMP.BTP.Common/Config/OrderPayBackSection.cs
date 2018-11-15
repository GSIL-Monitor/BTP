using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 不同类型订单回调地址配置
    /// </summary>
    public class OrderPayBackSection : ConfigurationSection
    {
        /// <summary>
        /// 普通订单
        /// </summary>   
        [ConfigurationProperty("common", IsRequired = false)]
        public string CommonUrl
        {
            get
            {
                return this["common"].ToString();
            }
        }

        /// <summary>
        /// 餐饮订单
        /// </summary>   
        [ConfigurationProperty("cy", IsRequired = false)]
        public string CyUrl
        {
            get
            {
                return this["cy"].ToString();
            }
        }
        /// <summary>
        /// 拼团
        /// </summary>   
        [ConfigurationProperty("diyGroup", IsRequired = false)]
        public string DiyGroupUrl
        {
            get
            {
                return this["diyGroup"].ToString();
            }
        }

    }

}
