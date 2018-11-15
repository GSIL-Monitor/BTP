
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/3/26 10:15:25
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class ShoppingCartAgent : BaseBpAgent<IShoppingCart>, IShoppingCart
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveShoppingCart(Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemsSDTO, System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveShoppingCart(shoppingCartItemsSDTO, userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetShoppongCartItems(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartItems(userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetShoppongCartItemsNew(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartItemsNew(userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateShoppingCart(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs, System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateShoppingCart(shopCartCommodityUpdateDTOs, userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteShoppingCart(System.Guid shopCartItemId, System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteShoppingCart(shopCartItemId, userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetShoppingCartNum(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppingCartNum(userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CopyOrderToShoppingCart(Guid userId, Guid orderId, Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CopyOrderToShoppingCart(userId, orderId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Guid>> CopyShareOrderToShoppingCart(Guid userId, Guid orderId, Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Guid>> result;

            try
            {
                //调用代理方法
                result = base.Channel.CopyShareOrderToShoppingCart(userId, orderId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> GetMyShoppongCart(Guid userId)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMyShoppongCart(userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSetShoppingCart(Jinher.AMP.BTP.Deploy.CustomDTO.ShoppingCartItemSDTO shoppingCartItemSDTO, Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSetShoppingCart(shoppingCartItemSDTO, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSetShoppingCart(Guid userId, List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateSetShoppingCart(userId, shopCartCommodityUpdateDTOs);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO GetSetShoppingCartNum(Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.NumResultSDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSetShoppingCartNum(userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
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
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCarCommodityListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartList(userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }



        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditysFromShoppingCart(List<Guid> shopCartItemIds, Guid userId, Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteCommoditysFromShoppingCart(shopCartItemIds, userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
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
            //定义返回值
            ResultDTO<Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveShoppingCartNew(sscDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommoditySDTO> GetShoppongCartItemsNew2(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommoditySDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartItemsNew2(userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartListDTO> GetShoppongCartItemsNew3(System.Guid userId, System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartItemsNew3(userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
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
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteShoppingCart2(shopCartItemIds, userId, appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttribute(System.Guid commodityId, Guid userId)
        {
            //定义返回值
            ResultDTO<ShopCartCommodityAttrDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartItemAttribute(commodityId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttributeNew(System.Guid commodityId, Guid userId)
        {
            //定义返回值
            ResultDTO<ShopCartCommodityAttrDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetShoppongCartItemAttributeNew(commodityId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 编辑购物车项数量
        /// </summary>
        /// <param name="shopCartCommodityUpdateDto">购物车编辑实体</param>
        /// <returns></returns>
        public ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingCart2(Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO shopCartCommodityUpdateDto)
        {
            //定义返回值
            ResultDTO<ShopCartUpdateResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateShoppingCart2(shopCartCommodityUpdateDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 编辑购物车项属性
        /// </summary>
        /// <param name="shopAttribute">商品属性对实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingAttribute(ShopAttributeCommodityUpdateDto shopAttribute)
        {
            //定义返回值
            ResultDTO<ShopCartUpdateResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateShoppingAttribute(shopAttribute);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
