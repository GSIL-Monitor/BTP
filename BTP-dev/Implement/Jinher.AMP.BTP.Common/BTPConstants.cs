using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// BTP常用常量类
    /// </summary>
    public class BTPConstants
    {
        /// <summary>
        /// 外部接口调用日志名
        /// </summary>
        public const string BTP_Outside = "BTP_Outside";

        /// <summary>
        /// 耗时跟踪日志名
        /// </summary>
        public const string StopwatchLog = "StopwatchLog";
    }
}