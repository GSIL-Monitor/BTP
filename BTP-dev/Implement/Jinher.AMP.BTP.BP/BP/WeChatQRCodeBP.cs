
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/6/5 14:38:05
***************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class WeChatQRCodeBP : BaseBP, IWeChatQRCode
    {

        /// <summary>
        /// 创建公众号带参二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQRCode(Jinher.AMP.BTP.Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO dto)
        {
            base.Do();
            return this.CreateWeChatQRCodeExt(dto);
        }
        /// <summary>
        /// 获取最大自增号
        /// </summary>
        /// <returns></returns>
        public int GetWeChatQRNo()
        {
            base.Do();
            return this.GetWeChatQRNoExt();
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
            return this.AddWeChatMenuExt(appId, menuJson);
        }
        /// <summary>
        /// 获取二维码类型
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.QrTypeDTO>> GetQrCodeTypeList(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeSearchDTO search)
        {
            base.Do();
            return this.GetQrCodeTypeListExt(search);
        }
        /// <summary>
        /// 获取二维码列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeShowDTO>> GetWechatQrCodeList(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeSearchDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.GetWechatQrCodeListExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("WeChatQRCodeBP.GetWechatQrCodeList：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 批量创建公众号二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQrCodeBatch(Jinher.AMP.BTP.Deploy.CustomDTO.QrCodeCreateDTO dto)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.CreateWeChatQrCodeBatchExt(dto);
            timer.Stop();
            LogHelper.Debug(string.Format("WeChatQRCodeBP.CreateWeChatQrCodeBatch：耗时：{0}。入参：dto:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsSerializer(dto), JsonHelper.JsonSerializer(result)));
            return result;
        }
        /// <summary>
        /// 启用、停用二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateState(Jinher.AMP.BTP.Deploy.CustomDTO.WeChatQRCodeUpdateStateDTO search)
        {
            base.Do();
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var result = this.UpdateStateExt(search);
            timer.Stop();
            LogHelper.Debug(string.Format("WeChatQRCodeBP.UpdateStateExt：耗时：{0}。入参：search:{1},\r\n出参：{2}", timer.ElapsedMilliseconds, JsonHelper.JsSerializer(search), JsonHelper.JsonSerializer(result)));
            return result;
        }
    }
}