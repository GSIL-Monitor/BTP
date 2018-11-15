
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/6/5 14:38:07
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class WeChatQRCodeAgent : BaseBpAgent<IWeChatQRCode>, IWeChatQRCode
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQRCode(Jinher.AMP.BTP.Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CreateWeChatQRCode(dto);

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
        public int GetWeChatQRNo()
        {
            //定义返回值
            int result;

            try
            {
                //调用代理方法
                result = base.Channel.GetWeChatQRNo();

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
        public bool AddWeChatMenu(System.Guid appId, string menuJson)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.AddWeChatMenu(appId, menuJson);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QrTypeDTO>> GetQrCodeTypeList(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QrTypeDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetQrCodeTypeList(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeShowDTO>> GetWechatQrCodeList(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeShowDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetWechatQrCodeList(search);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQrCodeBatch(Jinher.AMP.BTP.Deploy.CustomDTO.QrCodeCreateDTO dto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CreateWeChatQrCodeBatch(dto);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateState(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeUpdateStateDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateState(search);

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
