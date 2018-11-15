using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    public class JdResultDto
    {
        public int? ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public bool Success { get; set; }
    }

    public class JdResultDto<TResult> : JdResultDto
    {
        public TResult Result { get; set; }
    }
}
