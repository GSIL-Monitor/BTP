
/***************
功能描述: BTPBP
作    者: LSH
创建时间: 2017/9/21 15:02:29
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
    public partial class MallApplyBP : BaseBP, IMallApply
    {

        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO search)
        {
            base.Do(false);
            return this.GetMallApplyInfoListExt(search);
        }
        /// <summary>
        /// 保存商城信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveMallApply(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            base.Do();
            return this.SaveMallApplyExt(model);
        }
        /// <summary>
        /// 修改商城信息状态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateMallApply(Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO model)
        {
            base.Do();
            return this.UpdateMallApplyExt(model);
        }
        /// <summary>
        /// 根据id获取商城信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO GetMallApply(System.Guid id)
        {
            base.Do();
            return this.GetMallApplyExt(id);
        }
        /// <summary>
        /// 验证是否存在入驻商家
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO IsHaveMallApply(System.Guid esAppId, System.Guid appId)
        {
            base.Do();
            return this.IsHaveMallApplyExt(esAppId, appId);
        }
        /// <summary>
        /// 获取商城下入住的APP
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppInfoDTO> GetMallApps(System.Guid esAppId)
        {
            base.Do();
            return this.GetMallAppsExt(esAppId);
        }

        /// <summary>
        /// 给盈科同步指定商城数据
        /// </summary>
        /// <returns></returns>
        public void GetMallAppsForJob(System.Guid esAppId)
        {
            base.Do();
            this.GetMallAppsForJobExt(esAppId);
        }
    }
}