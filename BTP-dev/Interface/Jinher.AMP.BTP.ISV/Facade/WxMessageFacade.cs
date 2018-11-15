
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/5/26 14:14:18
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class WxMessageFacade : BaseFacade<IWxMessage>
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
            base.Do();
            return this.Command.DealMessage(message);
        }
    }
}