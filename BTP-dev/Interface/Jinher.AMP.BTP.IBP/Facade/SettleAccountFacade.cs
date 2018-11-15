
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/8/26 15:11:15
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
    public class SettleAccountFacade : BaseFacade<ISettleAccount>
    {

        /// <summary>
        /// 获取商城结算数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetMallSettleAccounts(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountSearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetMallSettleAccounts(searchDto);
        }
        /// <summary>
        /// 获取商家数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetSellerSettleAccounts(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountSearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetSellerSettleAccounts(searchDto);
        }
        /// <summary>
        ///  获取结算单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountDetailsDTO> GetSettleAccountDetails(System.Guid id)
        {
            base.Do();
            return this.Command.GetSettleAccountDetails(id);
        }
        /// <summary>
        /// 获取结算单订单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountOrderDTO>> GetSettleAccountOrders(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountOrderSearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetSettleAccountOrders(searchDto);
        }
        /// <summary>
        ///  获取结算单订单项详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountOrderItemDTO>> GetSettleAccountOrderItems(System.Guid id)
        {
            base.Do();
            return this.Command.GetSettleAccountOrderItems(id);
        }
        /// <summary>
        /// 修改结算单状态
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateState(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountUpdateStateDTO dto)
        {
            base.Do();
            return this.Command.UpdateState(dto);
        }
        /// <summary>
        /// 获取商家结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetSellerSettleAccountHistories(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountHistorySearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetSellerSettleAccountHistories(searchDto);
        }
        /// <summary>
        /// 获取商城结算历史数据
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountListDTO>> GetMallSettleAccountHistories(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountHistorySearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetMallSettleAccountHistories(searchDto);
        }
        /// <summary>
        /// 修改计算结果
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSettleStatue(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountUpdateSettleStatueDto dto)
        {
            base.Do();
            return this.Command.UpdateSettleStatue(dto);
        }
        /// <summary>
        /// 获取商城的结算周期
        /// </summary>
        /// <param name="esAppId">商城id</param>
        /// <returns>结算周期</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountPeriodDTO> GetSettleAccountPeriod(System.Guid esAppId)
        {
            base.Do();
            return this.Command.GetSettleAccountPeriod(esAppId);
        }
        /// <summary>
        /// 修改商城的结算周期
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSettleAccountPeriod(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountPeriodDTO dto)
        {
            base.Do();
            return this.Command.UpdateSettleAccountPeriod(dto);
        }
        /// <summary>
        /// 导入历史订单，生成结算项
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateSettleAccountDetails(Jinher.AMP.BTP.Deploy.CustomDTO.SettleAccountDetailsCreateDTO dto)
        {
            base.Do();
            return this.Command.CreateSettleAccountDetails(dto);
        }
        /// <summary>
        /// 获取商品的结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountListDTO>> GetCommoditySettleAmount(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountSearchDTO searchDto)
        {
            base.Do();
            return this.Command.GetCommoditySettleAmount(searchDto);
        }
        /// <summary>
        /// 设置商品的结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommoditySettleAmount(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountInputDTO input)
        {
            base.Do();
            return this.Command.SetCommoditySettleAmount(input);
        }
        /// <summary>
        /// 获取商品的历史结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySettleAmountHistoryDTO>> GetCommoditySettleAmountHistories(System.Guid commodityId, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetCommoditySettleAmountHistories(commodityId, pageIndex, pageSize);
        }
        /// <summary>
        /// 删除商品的历史结算价
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditySettleAmountHistory(System.Guid id)
        {
            base.Do();
            return this.Command.DeleteCommoditySettleAmountHistory(id);
        }
    }
}