
/***************
功能描述: BTPSV
作    者: 
创建时间: 2017/5/26 14:14:21
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class WxMessageSV : BaseSv, IWxMessage
    {

        /// <summary>
        /// 处理微信消息推送
        /// <para>Service URL：
        /// <a href="http://btp.iuoooo.com/Jinher.AMP.BTP.SV.WxMessageSV.svc/DealMessage">
        /// http://btp.iuoooo.com/Jinher.AMP.BTP.SV.WxMessageSV.svc/DealMessage
        /// </a></para>
        /// </summary>
        /// <param name="message">微信消息</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiRetDto DealMessage(Jinher.AMP.BTP.Deploy.CustomDTO.WcpBusiDto message)
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.DealMessageExt(message);
            timer.Stop();
            LogHelper.Debug(string.Format("WxMessageSV.DealMessage：耗时：{0}。入参：message:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsonSerializer(message), JsonHelper.JsonSerializer(result)));
            return result;
        }
    }
}