using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class JdPriceMessageDto
    {
        public string Id { get; set; }

        public JdPriceMessageResultDto Result { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string time { get; set; }
    }

    public class JdPriceMessageResultDto
    {
        public string SkuId { get; set; }
    }
}
