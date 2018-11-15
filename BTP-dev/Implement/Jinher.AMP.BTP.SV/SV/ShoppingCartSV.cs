
/***************
功能描述: BTPSV
作    者: 
创建时间: 2014/3/26 10:15:22
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
    public partial class ShoppingCartSV : BaseSv, IShoppingCart
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
            return this.SaveShoppingCartExt(shoppingCartItemsSDTO, userId, appId);

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
            return this.GetShoppongCartItemsExt(userId, appId);

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
            return this.GetShoppongCartItemsNewExt(userId, appId);

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
            return this.UpdateShoppingCartExt(shopCartCommodityUpdateDTOs, userId, appId);

        }
        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="commodityId">商品Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteShoppingCart(System.Guid shopCartItemId, System.Guid userId, System.Guid appId)
        {
            base.Do();
            return this.DeleteShoppingCartExt(shopCartItemId, userId, appId);

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
            return this.GetShoppingCartNumExt(userId, appId);

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
            base.Do(false);
            return this.CopyOrderToShoppingCartExt(userId, orderId, appId);
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
            return this.CopyShareOrderToShoppingCartExt(userId, orderId, appId);
        }

        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetMyShoppongCart(Guid userId)
        {
            base.Do(false);
            return this.GetMyShoppongCartExt(userId);

        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSetShoppingCart(Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemSDTO, Guid userId)
        {
            base.Do(false);
            return this.SaveSetShoppingCartExt(shoppingCartItemSDTO, userId);

        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSetShoppingCart(Guid userId, List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs)
        {
            base.Do(false);
            return this.UpdateSetShoppingCartExt(userId, shopCartCommodityUpdateDTOs);

        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetSetShoppingCartNum(Guid userId)
        {
            base.Do(false);
            return this.GetSetShoppingCartNumExt(userId);

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
            base.Do(false);
            return this.GetShoppongCartListExt(userId, appId);
        }


        /// <summary>
        /// 批量删除购物车中的商品。
        /// </summary>
        /// <param name="commodityId">商品Id列表</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditysFromShoppingCart(List<Guid> shopCartItemIds, Guid userId, Guid appId)
        {
            base.Do();
            return this.DeleteCommoditysFromShoppingCartExt(shopCartItemIds, userId, appId);

        }

        /// <summary>
        /// 添加购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/SaveShoppingCartNew
        /// </para>
        /// </summary>
        /// <param name="shoppingCartItemsSDTO">购物车商品实体</param>
        ///<param name="sscDto">加入购物车的商品、数量等信息</param>
        /// <returns></returns>
        public ResultDTO<Guid> SaveShoppingCartNew(SaveShoppingCartParamDTO sscDto)
        {
            base.Do(false);
            return this.SaveShoppingCartNewExt(sscDto);
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommoditySDTO> GetShoppongCartItemsNew2(System.Guid userId, System.Guid appId)
        {
            base.Do(false);
            return this.GetShoppongCartItemsNew2Ext(userId, appId);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ShopCartListDTO> GetShoppongCartItemsNew3(System.Guid userId, System.Guid appId)
        {
            base.Do(false);
            return this.GetShoppongCartItemsNew3Ext(userId, appId);
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shopCartItemIds">购物车Ids</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public ResultDTO DeleteShoppingCart2(List<Guid> shopCartItemIds, System.Guid userId, System.Guid appId)
        {
            base.Do(false);
            return this.DeleteShoppingCart2Ext(shopCartItemIds, userId, appId);
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttribute(System.Guid commodityId, Guid userId)
        {
            base.Do(false);
            return this.GetShoppongCartItemAttributeExt(commodityId, userId);
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttributeNew(System.Guid commodityId, Guid userId)
        {
            base.Do(false);
            return this.GetShoppongCartItemAttributeNewExt(commodityId, userId);
        }

        /// <summary>
        /// 编辑购物车项数量
        /// </summary>
        /// <param name="shopCartCommodityUpdateDto">购物车编辑实体</param>
        /// <returns></returns>
        public ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingCart2(Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO shopCartCommodityUpdateDto)
        {
            base.Do(false);
            return this.UpdateShoppingCart2Ext(shopCartCommodityUpdateDto);
        }

        /// <summary>
        /// 编辑购物车项属性
        /// </summary>
        /// <param name="shopAttribute">商品属性对实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingAttribute(ShopAttributeCommodityUpdateDto shopAttribute)
        {
            base.Do(false);
            return this.UpdateShoppingAttributeExt(shopAttribute);
        }
    }
}
