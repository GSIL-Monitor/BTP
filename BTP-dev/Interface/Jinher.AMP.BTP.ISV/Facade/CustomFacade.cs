
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/11 19:03:34
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class CustomFacade : BaseFacade<ICustom>
    {

        /// <summary>
        /// 根据userid获取用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.CBC.Deploy.CustomDTO.UserInfoWithAccountDTO> GetCustomInfo(System.Guid userId)
        {
            base.Do();
            return this.Command.GetCustomInfo(userId);
        }
        /// <summary>
        /// 根据commodityId获取商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityInfoListDTO> GetCommodityInfo(System.Guid commodityId)
        {
            base.Do();
            return this.Command.GetCommodityInfo(commodityId);
        }
        /// <summary>
        /// 根据orderId获取订单信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CommodityOrderDTO> GetCommodityOrder(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetCommodityOrder(orderId);
        }
        /// <summary>
        /// 商家的移动坐席数据
        /// </summary>
        /// <param name="pageIndex">数据分页数</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSceneUserDTO>> GetAppSceneContent(int pageIndex)
        {
            base.Do();
            return this.Command.GetAppSceneContent(pageIndex);
        }
        /// <summary>
        /// 获取易捷北京下所有店铺信息
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBJAppInfo>> GetYJAppInfo(int pageIndex)
        {
            base.Do();
            return this.Command.GetYJAppInfo(pageIndex);
        }
        public int GetNoInfoCount(string guid)
        {
            base.Do();
            return this.Command.GetNoInfoCount(guid);
        }
    }
}