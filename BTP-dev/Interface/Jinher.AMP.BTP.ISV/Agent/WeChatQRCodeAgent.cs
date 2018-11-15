
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/12/10 15:46:25
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class WeChatQRCodeAgent : BaseBpAgent<IWeChatQRCode>, IWeChatQRCode
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateForeverQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.ForeverQrcodeDTO param)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.CreateForeverQrcode(param);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> CreateTempQrcode(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.TempQrcodeDTO param)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.CreateTempQrcode(param);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetAccessToken(string appId, string appSecret)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAccessToken(appId, appSecret);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> SendMsg(Jinher.AMP.BTP.Deploy.CustomDTO.WeChat.SendMsgDTO param)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.SendMsg(param);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public ResultDTO Repaire()
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.Repaire();

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
