using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV.WeChat
{
    /// <summary>
    /// 扫公众号二维码事件处理类
    /// </summary>
    public class ScanWorker : WxMessageWorkerBase
    {
        public ScanWorker(WebChatMessageDTO log)
            : base(log)
        {
        }

        protected override bool Init()
        {
            //TODO  需要加载的内容
            return true;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //protected override void DealEvent()
        //{
        //    //TODO 需要处理的事件
        //}
    }
}

