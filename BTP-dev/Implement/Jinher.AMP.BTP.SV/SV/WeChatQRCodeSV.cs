
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/12/10 15:46:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
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
    public partial class WeChatQRCodeSV : BaseSv, IWeChatQRCode
    {

        /// <summary>
        /// 永久二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateForeverQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.ForeverQrcodeDTO param)
        {
            base.Do();
            return this.CreateForeverQrcodeExt(param);

        }
        /// <summary>
        /// 临时二维码请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateTempQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.TempQrcodeDTO param)
        {
            base.Do();
            return this.CreateTempQrcodeExt(param);

        }
        /// <summary>
        /// AccessToken请求
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetAccessToken(string appId, string appSecret)
        {
            base.Do();
            return this.GetAccessTokenExt(appId, appSecret);

        }
        /// <summary>
        /// 发送消息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> SendMsg(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.SendMsgDTO param)
        {
            base.Do();
            return this.SendMsgExt(param);

        }

        public ResultDTO Repaire()
        {
            return this.RepaireExt();
        }
    }
}