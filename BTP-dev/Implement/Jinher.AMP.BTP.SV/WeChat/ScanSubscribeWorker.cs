using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV.WeChat
{
    /// <summary>
    /// 扫码关注公众号事件处理类
    /// </summary>
    public class ScanSubscribeWorker : WxMessageWorkerBase
    {
        public ScanSubscribeWorker(WebChatMessageDTO log)
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

