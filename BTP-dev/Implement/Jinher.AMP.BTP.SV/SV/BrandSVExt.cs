
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/6/15 13:57:27
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
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class BrandSV : BaseSv, IBrand
    {

        /// <summary>
        /// 根据一级分类ID获取品牌墙信息（热门品牌）
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<IList<Jinher.AMP.BTP.Deploy.BrandwallDTO>> getBrandByCateIDExt(System.Guid CategoryID)
        {
            ResultDTO<IList<BrandwallDTO>> dto = null;
            try
            {
                var list = (from c in Category.ObjectSet()
                            join cib in CategoryInnerBrand.ObjectSet() on c.Id equals cib.CategoryId into c_join
                            from category in c_join.DefaultIfEmpty()
                            join b in Brandwall.ObjectSet() on category.BrandId equals b.Id into b_join
                            from cb in b_join.DefaultIfEmpty()
                            where c.Id.Equals(CategoryID) && cb.Brandstatu.Equals(1)
                            select new BrandwallDTO()
                            {
                                Id = cb.Id,
                                Brandname = cb.Brandname,
                                BrandLogo = cb.BrandLogo,
                                Brandstatu = cb.Brandstatu
                            }).ToList();
                dto = new ResultDTO<IList<BrandwallDTO>>()
                {
                    Data = list.ToList(),
                    isSuccess = true,
                    Message = "Sucess",
                    ResultCode = 0
                };

                return dto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取一级分类下的品牌墙信息错误getBrandByCateIDExt-CategoryID", CategoryID), ex);
                dto = new ResultDTO<IList<BrandwallDTO>>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "fail",
                    ResultCode = 1
                };
                return dto;
            }
        }
        /// <summary>
        /// 获取指定品牌下的商品
        /// </summary>
        /// <param name="BrandID"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.IList<Jinher.AMP.BTP.Deploy.CommodityDTO>> getBrandCommodityExt(System.Guid BrandID)
        {
            ResultDTO<IList<CommodityDTO>> dto = null;
            try
            {
                var list = (from b in Brandwall.ObjectSet()
                            join cib in CommodityInnerBrand.ObjectSet() on b.Id equals cib.BrandId into cib_join
                            from brand in cib_join.DefaultIfEmpty()
                            join c in Commodity.ObjectSet() on brand.CommodityId equals c.Id into cb_join
                            from cb in cb_join.DefaultIfEmpty()
                            where b.Id.Equals(BrandID) && b.Brandstatu.Equals(1)
                            select new CommodityDTO()
                            {
                                Name = cb.Name,
                                Code = cb.Code,
                                SubId = cb.SubId,
                                Price = cb.Price,
                                No_Number = cb.No_Number,
                                Stock = cb.Stock,
                                PicturesPath = cb.PicturesPath,
                                Description = cb.Description,
                                State = cb.State,
                                IsDel = cb.IsDel,
                                AppId = cb.AppId,
                                No_Code = cb.No_Code,
                                TotalReview = cb.TotalReview,
                                Salesvolume = cb.Salesvolume,
                                GroundTime = cb.GroundTime,
                                ComAttribute = cb.ComAttribute,
                                CategoryName = cb.CategoryName,
                                SortValue = cb.SortValue,
                                FreightTemplateId = cb.FreightTemplateId,
                                MarketPrice = cb.MarketPrice,
                                IsEnableSelfTake = cb.IsEnableSelfTake,
                                Weight = cb.Weight,
                                PricingMethod = cb.PricingMethod,
                                SaleAreas = cb.SaleAreas,
                                SharePercent = cb.SharePercent,
                                CommodityType = cb.CommodityType,
                                HtmlVideoPath = cb.HtmlVideoPath,
                                MobileVideoPath = cb.MobileVideoPath,
                                VideoPic = cb.VideoPic,
                                VideoName = cb.VideoName,
                                ScorePercent = cb.ScorePercent,
                                Duty = cb.Duty,
                                SpreadPercent = cb.SpreadPercent,
                                ScoreScale = cb.ScoreScale,
                                TaxRate = cb.TaxRate,
                                TaxClassCode = cb.TaxClassCode,
                                InputRax = cb.InputRax,
                                Unit = cb.Unit,
                                Barcode = cb.Barcode,
                                JDCode = cb.JDCode,
                                CostPrice = cb.CostPrice,
                                TechSpecs = cb.TechSpecs,
                                SaleService = cb.SaleService,
                                IsAssurance = cb.IsAssurance,
                                Assurance = cb.Assurance,
                                IsReturns = cb.IsReturns,
                                Type = cb.Type,
                                YJCouponActivityId = cb.YJCouponActivityId,
                                YJCouponType = cb.YJCouponType,
                                Isnsupport = cb.Isnsupport,
                                RefundFreightTemplateId = cb.RefundFreightTemplateId,
                                BasketCount = cb.BasketCount,
                                OrderWeight = cb.OrderWeight
                            }).ToList();
                dto = new ResultDTO<IList<CommodityDTO>>()
                {
                    Data = list.ToList(),
                    isSuccess = true,
                    Message = "Sucess",
                    ResultCode = 0
                };

                return dto;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取指定品牌下的商品错误getBrandCommodityExt-BrandID", BrandID), ex);
                dto = new ResultDTO<IList<CommodityDTO>>()
                {
                    Data = null,
                    isSuccess = false,
                    Message = "fail",
                    ResultCode = 1
                };
                return dto;
            }
        }
    }
}