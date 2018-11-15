using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 请求导出数据消息体
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExpressTemplateDTO:ExpressOrderTemplateDTO
    {
        [DataMember]
        public List<ExpressOrderTemplatePropertyDTO> Property { get; set; }

        public string ExpCode { get; set; }
    }
}
