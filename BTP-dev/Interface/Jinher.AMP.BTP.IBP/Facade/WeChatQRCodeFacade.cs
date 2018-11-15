
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/6/5 14:38:03
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class WeChatQRCodeFacade : BaseFacade<IWeChatQRCode>
    {

        /// <summary>
        /// 创建公众号带参二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQRCode(Jinher.AMP.BTP.Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO dto)
        {
            base.Do();
            return this.Command.CreateWeChatQRCode(dto);
        }
        /// <summary>
        /// 获取最大自增号
        /// </summary>
        /// <returns></returns>
        public int GetWeChatQRNo()
        {
            base.Do();
            return this.Command.GetWeChatQRNo();
        }
        /// <summary>
        /// 添加微信菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="menuJson"></param>
        /// <returns></returns>
        public bool AddWeChatMenu(System.Guid appId, string menuJson)
        {
            base.Do();
            return this.Command.AddWeChatMenu(appId, menuJson);
        }
        /// <summary>
        /// 获取二维码类型
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QrTypeDTO>> GetQrCodeTypeList(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeSearchDTO search)
        {
            base.Do();
            return this.Command.GetQrCodeTypeList(search);
        }
        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeShowDTO>> GetWechatQrCodeList(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeSearchDTO search)
        {
            base.Do();
            return this.Command.GetWechatQrCodeList(search);
        }
        /// <summary>
        /// 批量创建公众号二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQrCodeBatch(Jinher.AMP.BTP.Deploy.CustomDTO.QrCodeCreateDTO dto)
        {
            base.Do();
            return this.Command.CreateWeChatQrCodeBatch(dto);
        }
        /// <summary>
        /// 启用、停用二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateState(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeUpdateStateDTO search)
        {
            base.Do();
            return this.Command.UpdateState(search);
        }
    }
}