using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    
    [Serializable]
    public class CommodityModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Pic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Intensity { get; set; }

        public Guid AppId { get; set; }
    }
}