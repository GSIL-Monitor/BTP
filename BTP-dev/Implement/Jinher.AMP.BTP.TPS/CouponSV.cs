using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.Coupon.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */


    public class CouponSV : OutSideServiceBase<CouponSVFacade>
    {

        public static bool RefundCoupon(ContextSession contextSession, CommodityOrder commodityOrder)
        {
            var result = Instance.RefundCoupon(new SpendCouponRequestDTO
            {
                EsAppId = commodityOrder.EsAppId ?? Guid.Empty,
                ShopID = commodityOrder.AppId,
                OrderID = commodityOrder.Id,
                UserID = commodityOrder.UserId
            });
            if (result.IsSuccess) return result.IsSuccess;
            LogHelper.Error(string.Format("取消订单时回退优惠券失败。OrderId：{0}，", commodityOrder.Id));
            ErrorCommodityOrder errorOrder = new ErrorCommodityOrder
            {
                Id = Guid.NewGuid(),
                ErrorOrderId = commodityOrder.Id,
                ResourceType = 8,
                Description = "取消订单时回退优惠券失败",
                Source = commodityOrder.State,
                State = 0,
                AppId = commodityOrder.EsAppId.HasValue ? commodityOrder.EsAppId.Value : commodityOrder.AppId,
                UserId = commodityOrder.UserId,
                OrderCode = commodityOrder.Code,
                CouponId = Guid.Empty,
                SubTime = DateTime.Now,
                ModifiedOn = DateTime.Now,
                EntityState = System.Data.EntityState.Added
            };
            contextSession.SaveObject(errorOrder);
            return result.IsSuccess;
        }

        /// <summary>
        /// 保险返利赠送油卡兑换券
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO AddYouKaByInsuranceRebate(Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderDTO arg)
        {
            return Instance.AddYouKaByInsuranceRebate(arg);
        }

        /// <summary>
        /// 退回优惠券
        /// </summary>
        /// <param name="couponId"></param>
        /// <returns></returns>
        public static bool RetreatCoupon(Guid couponId)
        {
            RetreatCouponParamDTO arg = new RetreatCouponParamDTO();
            arg.CouponIds = new List<Guid>();
            arg.CouponIds.Add(couponId);
            var result = Instance.RetreatCoupon(arg);
            if (result.Code == 0)
            {
                return true;
            }
            return false;
        }
    }

    public class CouponSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 根据Id获取优惠券信息
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<IList<CouponNewDTO>> GetUserCouponsByIds(ListCouponNewRequestDTO condition)
        {
            ReturnInfoDTO<IList<CouponNewDTO>> reDTO = new ReturnInfoDTO<IList<CouponNewDTO>>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                reDTO = couponFacade.GetUserCouponsByIds(condition);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUserCouponsByIds服务异常:获取应用信息异常。 condition：{0}", condition), ex);
            }
            return reDTO;
        }

    

        /// <summary>
        /// 根据优惠券模板Id获取优惠券信息，包含跨店铺
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<IList<SpecifyStoreCoupon>> GetUserCouponsStoresByIds(ListCouponNewRequestDTO condition)
        {
            ReturnInfoDTO<IList<SpecifyStoreCoupon>> reDTO = new ReturnInfoDTO<IList<SpecifyStoreCoupon>>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                if (condition.UserId == Guid.Empty)
                {
                    condition.UserId = couponFacade.ContextDTO.LoginUserID;
                }
                reDTO = couponFacade.GetUserCouponsStoresByIds(condition);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUserCouponsStoresByIds服务异常:获取应用信息异常。 condition：{0}", condition), ex);
            }
            return reDTO;
        }

        /// <summary>
        /// 根据优惠券模板类型获取优惠券信息，包含跨店铺
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<IList<SpecifyStoreCoupon>> GetUserCouponsStoresByType(ListCouponNewRequestDTO condition)
        {
            ReturnInfoDTO<IList<SpecifyStoreCoupon>> reDTO = new ReturnInfoDTO<IList<SpecifyStoreCoupon>>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                if (condition.UserId == Guid.Empty)
                {
                    condition.UserId = couponFacade.ContextDTO.LoginUserID;
                }
                reDTO = couponFacade.GetUserCouponsStoresByType(condition);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUserCouponsStoresByType服务异常:获取应用信息异常。 condition：{0}", condition), ex);
            }
            return reDTO;
        }

        /// <summary>
        ///  优惠券消费
        /// </summary>
        /// <param name="SpendParams"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO SpendCoupon(SpendCouponRequestDTO SpendParams)
        {
            ReturnInfoDTO reDTO = new ReturnInfoDTO();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                reDTO = couponFacade.SpendCoupon(SpendParams);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.SpendCoupon服务异常:获取应用信息异常。 SpendParams：{0}", SpendParams), ex);
            }
            return reDTO;
        }


        /// <summary>
        ///  优惠券消费
        /// </summary>
        /// <param name="SpendParams"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO SpendStoreCoupon(Guid SpendParams)
        {
            ReturnInfoDTO reDTO = new ReturnInfoDTO();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                reDTO = couponFacade.SpendStoreCoupon(SpendParams);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.SpendCoupon服务异常:获取应用信息异常。 SpendParams：{0}", SpendParams), ex);
            }
            return reDTO;
        }

        /// <summary>
        ///  退回券消费
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO RefundCoupon(SpendCouponRequestDTO arg)
        {
            ReturnInfoDTO reDTO = new ReturnInfoDTO();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                reDTO = couponFacade.RefundCoupon(arg);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.RefundCoupon服务异常:arg：{0}", arg), ex);
            }
            return reDTO;
        }

        /// <summary>
        ///  退回券消费
        /// </summary>
        /// <param name="arg">优惠券Id</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO RetreatCoupon(RetreatCouponParamDTO arg)
        {
            ReturnInfoDTO reDTO = new ReturnInfoDTO();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                reDTO = couponFacade.RetreatCoupon(arg);

                string resultLog = string.Format("CouponSV.RetreatCoupon参数：{0}，结果：{1}", JsonHelper.JsonSerializer(arg), JsonHelper.JsonSerializer(reDTO));
                LogHelper.Debug(resultLog);
            }
            catch (Exception ex)
            {
                reDTO.Code = (int)Coupon.Deploy.Enum.ReturnCodeEnum.ServiceException;
                reDTO.Info = "服务异常！";//Coupon.Deploy.Enum.ReturnCodeEnum.ServiceException.GetDescription();
                reDTO.IsSuccess = false;

                LogHelper.Error(string.Format("CouponSV.RetreatCoupon服务异常:arg：{0}", JsonHelper.JsonSerializer(arg)), ex);
            }
            return reDTO;
        }
        /// <summary>
        /// 根据appId列表获取用户优惠券
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<IList<CouponNewDTO>> GetUserCouponsByShopList(ListCouponRequestDTO condition)
        {
            ReturnInfoDTO<IList<CouponNewDTO>> reDTO = new ReturnInfoDTO<IList<CouponNewDTO>>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                reDTO = couponFacade.GetUserCouponsByShopList(condition);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUserCouponsByShopList服务异常:获取应用信息异常。 condition：{0}", condition), ex);
            }
            return reDTO;
        }


        /// <summary>
        /// 获取所有可用的优惠券模板（不区分店铺）
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplateCanUseListDTO GetUsableCouponsTemplateList(Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplateUsableRequestDTO condition, bool isCentre)
        {
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                return couponFacade.GetCanUseCouponsTemplateList(condition, isCentre);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUsableCouponsTemplateList服务异常:获取应用信息异常。 condition：{0}", condition), ex);
            }
            return null;
        }

        /// <summary>
        ///  购物车店铺获取优惠券信息xiexg
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.Coupon.Deploy.CustomDTO.CouponTemplateResultResponseDTO GetUsableCouponsTemplateListForCart(Jinher.AMP.Coupon.Deploy.CustomDTO.CouponGetByAppComIds condition)
        {
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                return couponFacade.GetUsableCouponsTemplateListForCart(condition);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUsableCouponsTemplateListForCart服务异常:获取应用信息异常。 condition：{0}", condition), ex);
            }
            return null;
        }
        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public Jinher.AMP.Coupon.Deploy.CustomDTO.ReturnInfoDTO BindCouponToUser(Jinher.AMP.Coupon.Deploy.CustomDTO.CouponCreateRequestDTO newCoupon)
        {
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                return couponFacade.CreateCoupon(newCoupon);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.BindCouponToUser(CreateCoupon)服务异常:获取应用信息异常。 newCoupon：{0}", newCoupon), ex);
                return new ReturnInfoDTO() { IsSuccess = false, Info = "领取优惠券发生异常" };
            }
        }


        /// <summary>
        /// 获取商品的优惠券信息
        /// </summary>
        [BTPAopLogMethod]
        public List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.CouponOutput> GetCommodityCoupons(Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.CouponInput condition)
        {
            var result = new List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.CouponOutput>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CommodityCouponFacade couponFacade = new Coupon.ISV.Facade.CommodityCouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                var coResult = couponFacade.GetCoupons(condition);
                if (coResult.IsSuccess)
                {
                    return coResult.Data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetCommodityCoupons服务异常，获取获取商品的优惠券信息异常。 condition：{0}", condition), ex);
            }
            return result;
        }

        /// <summary>
        /// 获取我的优惠券
        /// </summary>
        [BTPAopLogMethod]
        public List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponOutput> GetMyUsableCoupons(Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponInput condition)
        {
            var result = new List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponOutput>();
            try
            {
                var couponFacade = new Coupon.ISV.Facade.CommodityCouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;

            
                var coResult = couponFacade.GetUsableCoupons(condition);
               
                if (coResult.IsSuccess)
                {
                    return coResult.Data;
                }
                else
                {
                    LogHelper.Info(string.Format("CouponSV.GetMyUsableCoupons获取我的商品优惠券信息失败。 condition：{0}, result: {1}", JsonHelper.JsonSerializer(condition), JsonHelper.JsonSerializer(coResult)));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetMyUsableCoupons服务异常，获取我的商品优惠券信息。 condition：{0}", JsonHelper.JsonSerializer(condition)), ex);
            }
            return result;
        }


        /// <summary>
        /// 根据用户获取，跨店铺满减券 qgb
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponOutput> GetStoreCoupon(Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponInput condition)
        {
            var result = new List<Jinher.AMP.Coupon.Deploy.CustomDTO.ZSH.UserCouponOutput>();
           
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CouponFacade couponFacade = new Coupon.ISV.Facade.CouponFacade();
                couponFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                //if (UserId == Guid.Empty)
                //{
                //    UserId = couponFacade.ContextDTO.LoginUserID;
                //}
                //var condition = new ListCouponRequestDTO() { UserId = UserId, GoodList = GoodList, CouponState = Coupon.Deploy.Enum.CouponState.Bind };
                 
                var coResult = couponFacade.GetStoreCoupon(condition);
                if (coResult.IsSuccess)
                {
                    return coResult.Data;
                }
                else
                {
                    LogHelper.Info(string.Format("CouponSV.GetMyUsableCoupons获取我的商品优惠券信息失败。 condition：{0}, result: {1}", JsonHelper.JsonSerializer(condition), JsonHelper.JsonSerializer(coResult)));
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetUserCouponsStoresByIds服务异常:获取应用信息异常。 "), ex);
            }
            return result;
        }

        /// <summary>
        /// 获取我的商品优惠券信息
        /// </summary>
        [BTPAopLogMethod]
        public List<Guid> GetCouponGoodsList(Guid couponTemplateId)
        {
            var result = new List<Guid>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CommodityCouponFacade couponFacade = new Coupon.ISV.Facade.CommodityCouponFacade { ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo() };
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;

                result = couponFacade.GetCouponGoodsList(couponTemplateId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetCouponGoodsList服务异常，获取我的商品优惠券信息。 couponTemplateId：{0}", couponTemplateId), ex);
            }
            return result;
        }

        /// <summary>
        /// 根据类型获取我的商品优惠券信息
        /// </summary>
        [BTPAopLogMethod]
        public List<Guid> GetCouponGoodsListByType()
        {
            var result = new List<Guid>();
            try
            {
                Jinher.AMP.Coupon.ISV.Facade.CommodityCouponFacade couponFacade = new Coupon.ISV.Facade.CommodityCouponFacade { ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo() };
                couponFacade.ContextDTO.LoginOrg = Guid.Empty;
                result = couponFacade.GetCouponGoodsListByType(0);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("CouponSV.GetCouponGoodsListByType服务异常，获取我的商品优惠券信息。"), ex);
            }
            return result;
        }

        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO AddYouKaByInsuranceRebate(Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderDTO arg)
        {
            YJB.ISV.Facade.CouponFacade cf = new YJB.ISV.Facade.CouponFacade();
            var result = cf.AddYouKaByInsuranceRebate(arg);
            return result;
        }

    }

}
