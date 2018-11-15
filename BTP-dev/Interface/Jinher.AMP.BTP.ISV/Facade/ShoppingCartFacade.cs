
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2014/3/26 10:15:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class ShoppingCartFacade : BaseFacade<IShoppingCart>
    {

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="shoppingCartItemsSDTO">购物车商品实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveShoppingCart(Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemsSDTO, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.SaveShoppingCart(shoppingCartItemsSDTO, userId, appId);
        }
        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetShoppongCartItems(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetShoppongCartItems(userId, appId);
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetShoppongCartItemsNew(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetShoppongCartItemsNew(userId, appId);
        }
        /// <summary>
        /// 编辑购物车
        /// </summary>
        /// <param name="shopCartCommodityUpdateDTOs">购物车编辑实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateShoppingCart(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.UpdateShoppingCart(shopCartCommodityUpdateDTOs, userId, appId);
        }
        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="commodityId">商品Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteShoppingCart(System.Guid commodityId, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.DeleteShoppingCart(commodityId, userId, appId);
        }
        /// <summary>
        /// 购物车商品数量 -- 可以不用提供，因为直接返回所有的购物车商品，直接调用商品list.count获取总数
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetShoppingCartNum(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetShoppingCartNum(userId, appId);
        }

        /// <summary>
        /// 复制订单中的商品到购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CopyOrderToShoppingCart(Guid userId, Guid orderId, Guid appId)
        {
            base.Do();
            return this.Command.CopyOrderToShoppingCart(userId, orderId, appId);
        }

        /// <summary>
        /// 分享的订单复制订单中的商品到购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Guid>> CopyShareOrderToShoppingCart(Guid userId, Guid orderId, Guid appId)
        {
            base.Do();
            return this.Command.CopyShareOrderToShoppingCart(userId, orderId, appId);
        }


        /// <summary>
        /// 厂家直营获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCarCommodityListDTO> GetShoppongCartList
            (System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetShoppongCartList(userId, appId);
        }


        /// <summary>
        /// 批量删除购物车中的商品
        /// </summary>
        /// <param name="commodityId">商品Id列表</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditysFromShoppingCart(List<Guid> shopCartItemIds, Guid userId, Guid appId)
        {
            base.Do();
            return this.Command.DeleteCommoditysFromShoppingCart(shopCartItemIds, userId, appId);
        }


        /// <summary>
        /// 添加购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/SaveShoppingCartNew
        /// </para>
        /// </summary>
        /// <param name="shoppingCartItemsSDTO">购物车商品实体</param>
        ///<param name="sscDto">加入购物车的商品、数量等信息</param>
        /// <returns></returns>
        public ResultDTO SaveShoppingCartNew(SaveShoppingCartParamDTO sscDto)
        {
            base.Do();
            return this.Command.SaveShoppingCartNew(sscDto);
        }


        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttribute(System.Guid commodityId, Guid userId)
        {
            base.Do();
            return this.Command.GetShoppongCartItemAttribute(commodityId, userId);
        }

        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttributeNew(System.Guid commodityId, Guid userId)
        {
            base.Do();
            return this.Command.GetShoppongCartItemAttributeNew(commodityId, userId);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ShopCartListDTO> GetShoppongCartItemsNew3(System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.Command.GetShoppongCartItemsNew3(userId, appId);
        }
    }
}
