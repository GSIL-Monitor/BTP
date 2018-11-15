
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/6/13 11:28:26
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
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
    public partial class SpreadInfoBP : BaseBP, ISpreadInfo
    {

        /// <summary>
        /// 保存推广信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSpreadInfo(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSaveDTO dto)
        {
            base.Do();
            return this.SaveSpreadInfoExt(dto);
        }
        /// <summary>
        /// 获取推广主列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SpreadInfoShowDTO>> GetSpreadInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSearchDTO search)
        {
            base.Do();
            return this.GetSpreadInfoListExt(search);
        }
        /// <summary>
        /// 绑定微信二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO BindWeChatQrCode(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadBindWeChatQrCodeDTO search)
        {
            base.Do();
            return this.BindWeChatQrCodeExt(search);
        }
        /// <summary>
        /// 启用、停用二维码
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateState(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadUpdateStateDTO search)
        {
            base.Do();
            return this.UpdateStateExt(search);
        }
        /// <summary>
        /// 修改子代理数量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSubCount(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadUpdateSubSpreadCountDTO dto)
        {
            base.Do();
            return this.UpdateSubCountExt(dto);
        }
        /// <summary>
        /// 修改总代分佣比例
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDividendPercent(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadUpdateDividendPercentDTO dto)
        {
            base.Do();
            return this.UpdateDividendPercentExt(dto);
        }
        /// <summary>
        /// 查询一级代理推广App列表
        /// </summary>
        /// <param name="iwId">组织ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SpreadAppDTO>> GetLv1SpreadApps(System.Guid iwId)
        {
            base.Do();
            return this.GetLv1SpreadAppsExt(iwId);
        }
        /// <summary>
        /// 查询一级代理指定APP的旺铺列表
        /// </summary>
        /// <param name="iwId">组织ID</param>
        /// <param name="appId">应用ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SpreadAppDTO>> GetLv1SpreadHotshops(System.Guid iwId, System.Guid appId)
        {
            base.Do();
            return this.GetLv1SpreadHotshopsExt(iwId, appId);
        }
    }
}