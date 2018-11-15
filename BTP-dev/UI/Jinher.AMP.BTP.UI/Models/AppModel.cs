using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jinher.AMP.BTP.UI.Models
{
    [Serializable]
    public class AppModel
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
        public string AdProduct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreatedDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShareUrl { get; set; }
    }
}