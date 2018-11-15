
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/5/18 14:39:02
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class DiyGroupSV : BaseSv, IDiyGroup
    {

        /// <summary>
        /// 获取拼团详情
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailDTO> GetDiyGroupDetail(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupDetailSearchDTO search)
        {
            base.Do(false);
            return this.GetDiyGroupDetailExt(search);

        }
        /// <summary>
        /// 处理超时未成团
        /// </summary>
        public void DealUnDiyGroupTimeout()
        {
            base.Do(false);
            this.DealUnDiyGroupTimeoutExt();

        }
        /// <summary>
        /// 处理 未成团退款
        /// </summary>
        public void DealUnDiyGroupRefund()
        {
            base.Do(false);
            this.DealUnDiyGroupRefundExt();

        }
        /// <summary>
        /// 我的拼团订单列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupOrderListDTO> GetDiyGroupList(Jinher.AMP.BTP.Deploy.CustomDTO.DiyGroupSearchDTO search)
        {
            base.Do(false);
            return this.GetDiyGroupListExt(search);
        }

        /// <summary>
        /// 自动确认成团 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyConfirmDiyGroup()
        {
            base.Do(false);
            return this.VoluntarilyConfirmDiyGroupExt();
        }

        /// <summary>
        /// 成团自动退款 -- JOB调用
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO VoluntarilyRefundDiyGroup()
        {
            base.Do(false);
            return this.VoluntarilyRefundDiyGroupExt();
        }

        /// <summary>
        /// 检查拼团状态
        /// </summary>
        /// <param name="inputDTO"></param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<CheckDiyGroupOutputDTO> CheckDiyGroup(CheckDiyGroupInputDTO inputDTO)
        {
            base.Do(false);
            return this.CheckDiyGroupExt(inputDTO);
        }
    }
}