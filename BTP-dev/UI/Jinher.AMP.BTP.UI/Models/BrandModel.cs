using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    [Serializable]
    public class BrandModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BrandLogo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int BrandStatu { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid AppId { get; set; }
    }
}