
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2016/12/10 15:46:19
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
    public class WeChatQRCodeFacade : BaseFacade<IWeChatQRCode>
    {

        /// <summary>
        /// 永久二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateForeverQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.ForeverQrcodeDTO param)
        {
            base.Do();
            return this.Command.CreateForeverQrcode(param);
        }
        /// <summary>
        /// 临时二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateTempQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.TempQrcodeDTO param)
        {
            base.Do();
            return this.Command.CreateTempQrcode(param);
        }
        /// <summary>
        /// AccessToken请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetAccessToken(string appId, string appSecret)
        {
            base.Do();
            return this.Command.GetAccessToken(appId, appSecret);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> SendMsg(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.SendMsgDTO param)
        {
            base.Do();
            return this.Command.SendMsg(param);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Repaire()
        {
            return this.Command.Repaire();
        }
    }
}