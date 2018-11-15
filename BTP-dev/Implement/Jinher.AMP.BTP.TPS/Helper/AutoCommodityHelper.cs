using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using System.Threading.Tasks;
using System.Data;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.TPS.Helper
{
    public static class AutoCommodityHelper
    {
        /// <summary>
        /// 获取浮动价格（当进价大于等于售价时售价调整为进价+浮动价格）
        /// </summary>
        /// <returns></returns>
        public static decimal GetFloatPrice()
        {
            try
            {
                var floatPrice = CommodityPriceFloat.ObjectSet()
                    .Where(p => p.EsAppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel)
                    .OrderByDescending(p => p.ModifiedOn)
                    .Select(p => p.FloatPrice)
                    .FirstOrDefault();
                floatPrice = floatPrice > 0 ? floatPrice : new decimal(0.60);
                return floatPrice;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return new decimal(0.60);
        }
        /// <summary>
        /// 商品上架
        /// </summary>
        /// <param name="skuIds"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int OnSalesCommodity(List<string> skuIds, ThirdECommerceTypeEnum type)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<Commodity> commoditys = new List<Commodity>();
                if (type == ThirdECommerceTypeEnum.SuNingYiGou)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && p.State == 1
                                            && p.CostPrice > 0 && p.Price > p.CostPrice && CustomConfig.SnAppIdList.Contains(p.AppId)).ToList();
                }
                if (type == ThirdECommerceTypeEnum.JingDongDaKeHu)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && p.State == 1
                                            && p.CostPrice > 0 && p.Price > p.CostPrice && CustomConfig.JdAppIdList.Contains(p.AppId)).ToList();
                }
                commoditys.ForEach(p =>
                {
                    LogHelper.Info("商品上架：CommodityId=" + p.Id);
                    p.State = 0;
                    p.ModifiedOn = DateTime.Now;
                });
                var count = contextSession.SaveChanges();
                if (count > 0)
                {
                    LogHelper.Info("商品上架：count=" + count);
                }
                return count;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return 0;
        }

        /// <summary>
        /// 商品下架
        /// </summary>
        /// <param name="skuIds"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int OffSalesCommodity(List<string> skuIds, ThirdECommerceTypeEnum type)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                List<Commodity> commoditys = new List<Commodity>();
                if (type == ThirdECommerceTypeEnum.SuNingYiGou)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && p.State == 0 && CustomConfig.SnAppIdList.Contains(p.AppId)).ToList();
                }
                if (type == ThirdECommerceTypeEnum.JingDongDaKeHu)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && p.State == 0 && CustomConfig.JdAppIdList.Contains(p.AppId)).ToList();
                }
                commoditys.ForEach(p =>
                {
                    LogHelper.Info("商品下架：CommodityId=" + p.Id);
                    p.State = 1;
                    p.ModifiedOn = DateTime.Now;
                });
                var count = contextSession.SaveChanges();
                if (count > 0)
                {
                    LogHelper.Info("商品下架：count=" + count);
                }
                return count;

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return 0;
        }

        /// <summary>
        /// 同步价格
        /// </summary>
        /// <param name="skuIds"></param>
        /// <param name="priceDtos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int UpdateCommodityPrice(List<string> skuIds, List<SNPriceDto> priceDtos, ThirdECommerceTypeEnum type)
        {
            try
            {

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var floatPrice = GetFloatPrice();
                List<Commodity> commoditys = new List<Commodity>();
                if (type == ThirdECommerceTypeEnum.SuNingYiGou)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && CustomConfig.SnAppIdList.Contains(p.AppId)).ToList();
                }
                if (type == ThirdECommerceTypeEnum.JingDongDaKeHu)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && CustomConfig.JdAppIdList.Contains(p.AppId)).ToList();
                }
                var commodityIds = commoditys.Select(p => p.Id).ToList();
                var commodityStocks = CommodityStock.ObjectSet().Where(p => commodityIds.Contains(p.CommodityId)).ToList();
                var commodityStockIds = commodityStocks.Select(p => p.Id).ToList();
                commoditys.ForEach(p =>
                {
                    var priceDto = priceDtos.Where(x => x.skuId == p.JDCode).FirstOrDefault();
                    var price = Decimal.Zero;
                    var snPrice = Decimal.Zero;
                    if (priceDto != null)
                    {
                        if (!string.IsNullOrEmpty(priceDto.price))
                        {
                            price = Convert.ToDecimal(priceDto.price);
                        }
                        if (!string.IsNullOrEmpty(priceDto.snPrice))
                        {
                            snPrice = Convert.ToDecimal(priceDto.snPrice);
                        }
                    }
                    if (priceDto == null)
                    {
                        if (p.State == 0)
                        {
                            LogHelper.Info("未获取到商品价格信息，商品下架：CommodityId=" + p.Id);
                            p.State = 1;
                            p.ModifiedOn = DateTime.Now;
                        }
                        return;
                    }
                    if ((price <= 0 || snPrice <= 0))
                    {
                        if (p.State == 0)
                        {
                            LogHelper.Info("进价或售价为0，商品下架：CommodityId=" + p.Id);
                            p.State = 1;
                            p.ModifiedOn = DateTime.Now;
                        }
                        return;
                    }
                    var commodityStock = commodityStocks.FirstOrDefault(x => x.CommodityId == p.Id);
                    if (p.CostPrice != price)
                    {
                        if (price >= snPrice)
                        {
                            snPrice = Convert.ToDecimal(priceDto.price) + floatPrice;
                            priceDto.snPrice = snPrice.ToString();
                        }
                        LogHelper.Info(string.Format("商品进价从{0}调整为{1}：CommodityId={2}", p.CostPrice, priceDto.price, p.Id));
                        LogHelper.Info(string.Format("商品售价从{0}调整为{1}", p.Price, priceDto.snPrice));
                        p.CostPrice = price;
                        p.Price = snPrice;
                        p.ModifiedOn = DateTime.Now;
                        if (commodityStock != null)
                        {
                            LogHelper.Info(string.Format("商品单品进价从{0}调整为{1}：CommodityStockId={2}", commodityStock.CostPrice, priceDto.price, commodityStock.Id));
                            LogHelper.Info(string.Format("商品单品售价从{0}调整为{1}", commodityStock.Price, priceDto.snPrice));
                            commodityStock.CostPrice = price;
                            commodityStock.Price = snPrice;
                            commodityStock.ModifiedOn = DateTime.Now;
                        }
                    }
                    else if (p.CostPrice >= p.Price || (commodityStock != null && commodityStock.CostPrice >= commodityStock.Price))
                    {
                        if (price >= snPrice)
                        {
                            snPrice = Convert.ToDecimal(priceDto.price) + floatPrice;
                            priceDto.snPrice = snPrice.ToString();
                        }
                        LogHelper.Info(string.Format("商品售价从{0}调整为{1}：CommodityId={2}", p.Price, priceDto.snPrice, p.Id));
                        p.Price = snPrice;
                        p.ModifiedOn = DateTime.Now;
                        if (commodityStock != null)
                        {
                            LogHelper.Info(string.Format("商品单品售价从{0}调整为{1}：CommodityStockId={2}", commodityStock.Price, priceDto.snPrice, commodityStock.Id));
                            commodityStock.Price = snPrice;
                            commodityStock.ModifiedOn = DateTime.Now;
                        }
                    }
                });
                var count = contextSession.SaveChanges();
                if (count > 0)
                {
                    LogHelper.Info("调整商品价格：count=" + count);
                }
                return count;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return 0;
        }

        /// <summary>
        /// 同步库存
        /// </summary>
        /// <param name="skuStockDir"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int UpdateCommodityStock(Dictionary<string, int> skuStockDir, ThirdECommerceTypeEnum type)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var skuIds = skuStockDir.Select(p => p.Key).ToList();
                List<Commodity> commoditys = new List<Commodity>();
                List<CommodityStock> commodityStocks = new List<CommodityStock>();
                if (type == ThirdECommerceTypeEnum.JingDongDaKeHu)
                {
                    commoditys = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && p.State == 0 && CustomConfig.JdAppIdList.Contains(p.AppId)).ToList();
                }
                List<Guid> commodityIds = new List<Guid>();
                if (commoditys != null && commoditys.Count > 0)
                {
                    commodityIds = commoditys.Select(p => p.Id).ToList();
                }
                commodityStocks = CommodityStock.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && p.State == 0 && commodityIds.Contains(p.CommodityId)).ToList();
                commoditys.ForEach(p =>
                {
                    LogHelper.Info(string.Format("商品(Commodity)调整库存：CommodityId={0},Stock={1}", p.Id, skuStockDir[p.JDCode]));
                    p.Stock = skuStockDir[p.JDCode];
                    p.ModifiedOn = DateTime.Now;
                    var commodityStock = commodityStocks.FirstOrDefault(x => x.CommodityId == p.Id);
                    if (commodityStock != null)
                    {
                        LogHelper.Info(string.Format("商品(CommodityStock)调整库存：CommodityId={0},Stock={1}", p.Id, skuStockDir[p.JDCode]));
                        commodityStock.Stock = skuStockDir[p.JDCode];
                        commodityStock.ModifiedOn = DateTime.Now;
                    }
                });
                var count = contextSession.SaveChanges();
                if (count > 0)
                {
                    LogHelper.Info("商品(Commodity)调整库存：count=" + count);
                }
                return count;

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return 0;
        }
    }
}
