
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/1/25 11:40:16
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
    public partial class CommodityChangeBP : BaseBP, ICommodityChange
    {

        /// <summary>
        /// 获取商品变更表信息列表
        /// </summary>
        /// <param name="Search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO>> GetCommodityChangeList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search, int pageIndex, int pageSize)
        {
            base.Do(false);
            return this.GetCommodityChangeListExt(Search, pageIndex, pageSize);
        }
        /// <summary>
        /// 导出商品信息变更信息
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> GetCommodityChangeExport(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            base.Do(false);
            return this.GetCommodityChangeExportExt(Search);
        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyList(System.Guid EsAppid)
        {
            base.Do(false);
            return this.GetMallApplyListExt(EsAppid);
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierList(System.Guid EsAppid)
        {
            base.Do(false);
            return this.GetSupplierListExt(EsAppid);
        }
        /// <summary>
        /// 查询供应商对应的所有商铺ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetAppIdsBySupplier(System.Guid EsAppId)
        {
            base.Do(false);
            return this.GetAppIdsBySupplierExt(EsAppId);
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.UserNameDTO> GetUserList()
        {
            base.Do(false);
            return this.GetUserListExt();
        }
        /// <summary>
        /// 更新商品变动表
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityChange(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> CommodityChange)
        {
            base.Do(false);
            return this.SaveCommodityChangeExt(CommodityChange);
        }
        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.totalNum> GetTotalList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            base.Do(false);
            return this.GetTotalListExt(Search);
        }

        /// <summary>
        /// 根据商品id获取活动类型
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public int JudgeActivityType(Guid commodityId)
        {
            base.Do(false);
            return this.JudgeActivityTypeExt(commodityId);
        }
    }
}