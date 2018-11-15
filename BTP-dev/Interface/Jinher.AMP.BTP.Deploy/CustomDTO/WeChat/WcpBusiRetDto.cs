using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// WCP消息返回值 请参考：Jinher.AMP.WCP.Deploy.CustomDTO.BusiRetDto
    /// </summary>
    [Serializable]
    [DataContract]
    public class WcpBusiRetDto
    {

        /// <summary>
        /// 是否给微信返回
        /// </summary>
        [DataMember]
        public bool isReturn;
        /// <summary>
        /// 返回给微信的xml
        /// </summary>
        [DataMember]
        public string WXContent;


    }
}
