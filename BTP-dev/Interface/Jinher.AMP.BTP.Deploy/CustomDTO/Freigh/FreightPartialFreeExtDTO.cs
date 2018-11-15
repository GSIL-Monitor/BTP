using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    [Serializable()]
    [DataContract]
    public class FreightPartialFreeExtDTO : FreightPartialFreeDTO
    {

        /// <summary>
        /// 运送到
        /// </summary>
        [DataMemberAttribute()]
        public string FreightTo
        {
            get
            {
                return _FreightTo;
            }
            set
            {
                if (_FreightTo != value)
                {
                    _FreightTo = value;
                }
            }
        }

        private string _FreightTo;    

    }
}
