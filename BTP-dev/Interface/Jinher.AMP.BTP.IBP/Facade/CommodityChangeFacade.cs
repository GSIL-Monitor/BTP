
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/1/25 11:40:13
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
    public class CommodityChangeFacade : BaseFacade<ICommodityChange>
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
            base.Do();
            return this.Command.GetCommodityChangeList(Search, pageIndex, pageSize);
        }
        /// <summary>
        /// 导出商品信息变更信息
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> GetCommodityChangeExport(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            base.Do();
            return this.Command.GetCommodityChangeExport(Search);
        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyList(System.Guid EsAppid)
        {
            base.Do();
            return this.Command.GetMallApplyList(EsAppid);
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierList(System.Guid EsAppid)
        {
            base.Do();
            return this.Command.GetSupplierList(EsAppid);
        }
        /// <summary>
        /// 查询供应商对应的所有商铺ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetAppIdsBySupplier(System.Guid EsAppId)
        {
            base.Do();
            return this.Command.GetAppIdsBySupplier(EsAppId);
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.UserNameDTO> GetUserList()
        {
            base.Do();
            return this.Command.GetUserList();
        }
        /// <summary>
        /// 更新商品变动表
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityChange(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> CommodityChange)
        {
            base.Do();
            return this.Command.SaveCommodityChange(CommodityChange);
        }
        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.totalNum> GetTotalList(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO Search)
        {
            base.Do();
            return this.Command.GetTotalList(Search);
        }

        /// <summary>
        /// 根据商品id获取活动类型
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public int JudgeActivityType(Guid commodityId)
        {
            base.Do();
            return this.Command.JudgeActivityType(commodityId);
        }
    }
}