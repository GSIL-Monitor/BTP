
/***************
功能描述: BTPBP
作    者: 
创建时间: 2017/8/26 15:11:16
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
    public partial class SettleAccountBP : BaseBP, ISettleAccount
    {

        /// <summary>
        /// 获取商城结算数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetMallSettleAccounts(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountSearchDTO searchDto)
        {
            base.Do();
            return this.GetMallSettleAccountsExt(searchDto);
        }
        /// <summary>
        /// 获取商家数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetSellerSettleAccounts(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountSearchDTO searchDto)
        {
            base.Do();
            return this.GetSellerSettleAccountsExt(searchDto);
        }
        /// <summary>
        ///  获取结算单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountDetailsDTO> GetSettleAccountDetails(System.Guid id)
        {
            base.Do();
            return this.GetSettleAccountDetailsExt(id);
        }
        /// <summary>
        /// 获取结算单订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountOrderDTO>> GetSettleAccountOrders(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountOrderSearchDTO searchDto)
        {
            base.Do();
            return this.GetSettleAccountOrdersExt(searchDto);
        }
        /// <summary>
        ///  获取结算单订单项详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountOrderItemDTO>> GetSettleAccountOrderItems(System.Guid id)
        {
            base.Do();
            return this.GetSettleAccountOrderItemsExt(id);
        }
        /// <summary>
        /// 修改结算单状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateState(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountUpdateStateDTO dto)
        {
            base.Do();
            return this.UpdateStateExt(dto);
        }
        /// <summary>
        /// 获取商家结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetSellerSettleAccountHistories(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountHistorySearchDTO searchDto)
        {
            base.Do();
            return this.GetSellerSettleAccountHistoriesExt(searchDto);
        }
        /// <summary>
        /// 获取商城结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetMallSettleAccountHistories(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountHistorySearchDTO searchDto)
        {
            base.Do();
            return this.GetMallSettleAccountHistoriesExt(searchDto);
        }
        /// <summary>
        /// 修改计算结果
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSettleStatue(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountUpdateSettleStatueDto dto)
        {
            base.Do();
            return this.UpdateSettleStatueExt(dto);
        }
        /// <summary>
        /// 获取商城的结算周期
        /// </summary>
        /// <param name="esAppId">商城id</param>
        /// <returns>结算周期</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountPeriodDTO> GetSettleAccountPeriod(System.Guid esAppId)
        {
            base.Do();
            return this.GetSettleAccountPeriodExt(esAppId);
        }
        /// <summary>
        /// 修改商城的结算周期
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSettleAccountPeriod(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountPeriodDTO dto)
        {
            base.Do();
            return this.UpdateSettleAccountPeriodExt(dto);
        }
        /// <summary>
        /// 导入历史订单，生成结算项
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateSettleAccountDetails(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountDetailsCreateDTO dto)
        {
            base.Do();
            return this.CreateSettleAccountDetailsExt(dto);
        }
        /// <summary>
        /// 获取商品的结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountListDTO>> GetCommoditySettleAmount(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountSearchDTO searchDto)
        {
            base.Do();
            return this.GetCommoditySettleAmountExt(searchDto);
        }
        /// <summary>
        /// 设置商品的结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommoditySettleAmount(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountInputDTO input)
        {
            base.Do();
            return this.SetCommoditySettleAmountExt(input);
        }
        /// <summary>
        /// 获取商品的历史结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountHistoryDTO>> GetCommoditySettleAmountHistories(System.Guid commodityId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetCommoditySettleAmountHistoriesExt(commodityId, pageIndex, pageSize);
        }
        /// <summary>
        /// 删除商品的历史结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditySettleAmountHistory(System.Guid id)
        {
            base.Do();
            return this.DeleteCommoditySettleAmountHistoryExt(id);
        }
    }
}