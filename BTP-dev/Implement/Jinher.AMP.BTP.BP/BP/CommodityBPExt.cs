
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/3/18 15:15:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using System.Data;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp;


namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityBP : BaseBP, ICommodity
    {


        #region 商品发布

        /// <summary>
        /// 保存商品
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品实体</param>
        /// <returns></returns>
        public ResultDTO SaveCommodityExt(CommodityAndCategoryDTO commodityAndCategoryDTO)
        {
            try
            {
                if (commodityAndCategoryDTO == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "参数不能为空" };
                }
                Guid userId = this.ContextDTO.LoginUserID;
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appModel =
                    APPSV.Instance.GetAppOwnerInfo(commodityAndCategoryDTO.AppId);
                if (appModel == null) return new ResultDTO { ResultCode = 2, Message = "获取应用信息失败" };
                bool isFnull = true;
                if (appModel.OwnerType == 0)
                {
                    CBC.Deploy.CustomDTO.OrgInfoNewDTO orgInfoDTO = CBCSV.Instance.GetOrgInfoNewBySubId(appModel.OwnerId);
                    if (orgInfoDTO == null || string.IsNullOrEmpty(orgInfoDTO.CompanyPhone))
                    {
                        isFnull = false;
                    }
                }
                if (string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code))
                {
                    commodityAndCategoryDTO.No_Code = "";
                }
                else
                {
                    //if (commodityAndCategoryDTO.No_Code.Length > 30)
                    //{
                    //    return new ResultDTO { ResultCode = 2, Message = "商品编号不能超过30位" };
                    //}
                }

                if (isFnull)
                {
                    #region 商品
                    //添加商品时获取排序 最小值新加商品排序在最上面
                    var minSortValueQuery = (from m in Commodity.ObjectSet()
                                             where m.AppId == commodityAndCategoryDTO.AppId && m.CommodityType == 0
                                             select m);
                    int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                    int minSortValue = 2;
                    if (minSort.HasValue)
                    {
                        minSortValue = minSort.Value;
                    }
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code) && !IsExistsExt(commodityAndCategoryDTO.No_Code, commodityAndCategoryDTO.AppId))
                    {
                        return new ResultDTO { ResultCode = 2, Message = "商品编号不能重复，该编号已存在" };
                    }

                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    Commodity commodity = Commodity.CreateCommodity();
                    commodity.Name = commodityAndCategoryDTO.Name;
                    commodity.SubTime = DateTime.Now;
                    commodity.No_Code = commodityAndCategoryDTO.No_Code;
                    commodity.No_Number = commodityAndCategoryDTO.No_Number;
                    commodity.Price = commodityAndCategoryDTO.Price;
                    commodity.MarketPrice = commodityAndCategoryDTO.MarketPrice;
                    commodity.Duty = commodityAndCategoryDTO.CommodityDuty;
                    commodity.TaxRate = commodityAndCategoryDTO.CommodityTaxRate;
                    commodity.InputRax = commodityAndCategoryDTO.CommodityInputTax;
                    commodity.TaxClassCode = commodityAndCategoryDTO.TaxClassCode;
                    commodity.Unit = commodityAndCategoryDTO.Unit;
                    commodity.PicturesPath = commodityAndCategoryDTO.PicturesPath;
                    commodity.State = commodityAndCategoryDTO.State;
                    commodity.Stock = commodityAndCategoryDTO.Stock;
                    commodity.SubId = userId;
                    commodity.AppId = commodityAndCategoryDTO.AppId;
                    commodity.TotalCollection = 0;
                    commodity.TotalReview = 0;
                    commodity.Salesvolume = 0;
                    commodity.Description = commodityAndCategoryDTO.Description;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.CategoryName = commodityAndCategoryDTO.CateNames;
                    commodity.SortValue = minSortValue - 1;
                    commodity.IsEnableSelfTake = commodityAndCategoryDTO.IsEnableSelfTake;
                    commodity.PricingMethod = commodityAndCategoryDTO.PricingMethod;
                    commodity.Weight = commodity.PricingMethod == 1 ? (decimal?)commodityAndCategoryDTO.Weight : null;
                    commodity.SaleAreas = commodityAndCategoryDTO.SaleAreas;
                    commodity.HtmlVideoPath = commodityAndCategoryDTO.VideoclientUrl;
                    commodity.MobileVideoPath = commodityAndCategoryDTO.VideoUrl;
                    commodity.VideoPic = commodityAndCategoryDTO.VideoPicUrl;
                    commodity.VideoName = commodityAndCategoryDTO.VideoName;
                    commodity.ScoreScale = commodityAndCategoryDTO.ScoreScale;

                    commodity.Barcode = commodityAndCategoryDTO.BarCode;
                    if (string.IsNullOrEmpty(commodity.No_Code) && !string.IsNullOrEmpty(commodity.Barcode))
                    {
                        commodity.No_Code = commodity.Barcode + "0000";
                    }
                    commodity.JDCode = commodityAndCategoryDTO.JDCode;
                    commodity.ErQiCode = commodityAndCategoryDTO.ErQiCode;
                    commodity.CostPrice = commodityAndCategoryDTO.CostPrice;
                    commodity.IsAssurance = commodityAndCategoryDTO.IsAssurance;
                    commodity.IsReturns = commodityAndCategoryDTO.IsReturns;
                    commodity.Isnsupport = commodityAndCategoryDTO.Isnsupport;
                    commodity.ServiceSettingId = commodityAndCategoryDTO.ServiceSettingId;
                    commodity.TechSpecs = commodityAndCategoryDTO.TechSpecs;
                    commodity.SaleService = commodityAndCategoryDTO.SaleService;
                    commodity.Type = commodityAndCategoryDTO.CommodityType;
                    commodity.YJCouponActivityId = commodityAndCategoryDTO.YJCouponActivityId;
                    commodity.YJCouponType = commodityAndCategoryDTO.YJCouponType;
                    Guid freightId = Guid.Empty;
                    if (Guid.TryParse(commodityAndCategoryDTO.FreightId, out freightId))
                    {
                        var freightTemplate = FreightTemplate.ObjectSet().FirstOrDefault(c => c.Id == freightId);
                        if (freightTemplate == null ||
                            (freightTemplate.PricingMethod != commodityAndCategoryDTO.PricingMethod &&
                             freightTemplate.ExpressType != 1))
                        {
                            return new ResultDTO { ResultCode = 2, Message = "运费模板选择错误" };
                        }

                        commodity.FreightTemplateId = freightId;
                    }

                    #region 商品属性

                    /* //zgx-modify
                    List<ComAttributeDTO> attrList = new List<ComAttributeDTO>();
                    if (commodityAndCategoryDTO.ColorNames != null)
                    {
                        string[] colors = commodityAndCategoryDTO.ColorNames.Split(',').Distinct().ToArray();
                        if (colors.Length > 0)
                        {
                            foreach (string color in colors)
                            {
                                attrList.Add(new ComAttributeDTO { Attribute = "颜色", SecondAttribute = color });
                            }
                        }
                    }
                    if (commodityAndCategoryDTO.SizeNames != null)
                    {
                        string[] sizes = commodityAndCategoryDTO.SizeNames.Split(',').Distinct().ToArray();
                        if (sizes.Length > 0)
                        {
                            foreach (string size in sizes)
                            {
                                attrList.Add(new ComAttributeDTO { Attribute = "尺寸", SecondAttribute = size });
                            }
                        }
                    }
                    commodity.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(attrList);
                    */

                    #endregion

                    #region 商品新属性逻辑
                    //zgx-modify
                    List<ComAttributeDTO> allAttrList = new List<ComAttributeDTO>();
                    int StockTotal = 0;
                    //判断是否有组合属性
                    if (commodityAndCategoryDTO.ComAttributes != null && commodityAndCategoryDTO.ComAttributes.Count > 0)
                    {
                        decimal maxMarketPrice = decimal.MaxValue;
                        decimal maxDuty = decimal.MaxValue;
                        decimal maxPrice = decimal.MaxValue;
                        decimal maxCostPrice = decimal.MaxValue;
                        foreach (var item in commodityAndCategoryDTO.ComAttributes)
                        {
                            item.ComAttribute = new List<ComAttributeDTO>();
                            item.ComAttributeIds.ForEach(
                                r =>
                                item.ComAttribute.Add(new ComAttributeDTO()
                                {
                                    Attribute = r.Attribute,
                                    SecondAttribute = r.SecondAttribute
                                }));

                            CommodityStock cs = CommodityStock.CreateCommodityStock();
                            cs.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(item.ComAttribute);

                            foreach (var attr in item.ComAttribute)
                            {
                                if (
                                    !allAttrList.Exists(
                                        r => r.Attribute == attr.Attribute && r.SecondAttribute == attr.SecondAttribute))
                                {
                                    allAttrList.Add(attr);
                                }
                            }

                            cs.CommodityId = commodity.Id;
                            cs.Price = item.Price;
                            cs.MarketPrice = item.MarketPrice;
                            cs.Duty = item.Duty;
                            cs.Stock = item.Stock;

                            // 20170918新增
                            cs.Barcode = item.BarCode;
                            cs.No_Code = item.Code;
                            // 20171102新增
                            cs.ThumImg = item.ThumImg;
                            cs.CarouselImgs = item.CarouselImgs;

                            if (string.IsNullOrEmpty(cs.Code) && !string.IsNullOrEmpty(cs.Barcode))
                            {
                                cs.No_Code = cs.Barcode + "0000";
                            }
                            cs.JDCode = item.JDCode;
                            cs.ErQiCode = item.ErQiCode;
                            cs.CostPrice = item.CostPrice;

                            if (StockTotal < int.MaxValue && cs.Stock < int.MaxValue)
                                StockTotal += cs.Stock;
                            else
                                StockTotal = int.MaxValue;
                            if (item.Price > 0 && item.Price < maxPrice)
                            {
                                maxPrice = item.Price;
                                commodity.Price = maxPrice;
                                if (!string.IsNullOrEmpty(cs.No_Code)) commodity.No_Code = cs.No_Code;
                                commodity.JDCode = item.JDCode;
                                commodity.Barcode = item.BarCode;
                            }

                            if (item.CostPrice > 0 && item.CostPrice < maxCostPrice)
                            {
                                maxCostPrice = item.CostPrice.Value;
                                commodity.CostPrice = maxCostPrice;
                            }

                            if (item.MarketPrice > 0 && item.MarketPrice < maxMarketPrice)
                            {
                                maxMarketPrice = item.MarketPrice.Value;
                            }
                            if (item.Duty > 0 && item.Duty < maxDuty)
                            {
                                maxDuty = item.Duty.Value;
                            }
                            contextSession.SaveObject(cs);
                        }
                        if (maxMarketPrice < decimal.MaxValue)
                        {
                            commodity.MarketPrice = maxMarketPrice;
                        }
                        else
                        {
                            commodity.MarketPrice = null;
                        }
                        if (maxDuty < decimal.MaxValue)
                        {
                            commodity.Duty = maxDuty;
                        }
                        else
                        {
                            commodity.Duty = null;
                        }
                        commodity.Stock = StockTotal;

                    }
                    //判断是否是一种属性
                    else if (!string.IsNullOrEmpty(commodityAndCategoryDTO.AttrValueNames))
                    {
                        string[] attrValues = commodityAndCategoryDTO.AttrValueNames.Split(',').Distinct().ToArray();
                        if (attrValues.Length > 0)
                        {
                            foreach (string attrValue in attrValues)
                            {
                                allAttrList.Add(new ComAttributeDTO
                                {
                                    Attribute = commodityAndCategoryDTO.AttrName,
                                    SecondAttribute = attrValue
                                });
                            }
                        }
                    }

                    commodity.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(allAttrList);

                    #endregion

                    contextSession.SaveObject(commodity);
                    //无属性商品库存处理20170308
                    if (commodityAndCategoryDTO.ComAttributes != null && commodityAndCategoryDTO.ComAttributes.Count == 0)
                    {
                        SaveCommodityStock(commodity, contextSession);
                    }
                    if (commodityAndCategoryDTO.State == 0)
                    {
                        commodity.GroundTime = DateTime.Now;
                    }

                    #endregion

                    #region 商品分类
                    string[] catelist = commodityAndCategoryDTO.CategoryPath.Split(',');
                    List<Guid> cateidlist = new List<Guid>();
                    List<Guid?> cateidlist2 = new List<Guid?>();
                    foreach (string s in catelist)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            cateidlist.Add(new Guid(s));
                            cateidlist2.Add(new Guid(s));
                        }
                    }
                    if (cateidlist.Any())
                    {
                        //只有最后一级分类才能添加商品
                        var categoryList =
                            Category.ObjectSet()
                                    .Where(t => cateidlist.Contains(t.Id) && !cateidlist2.Contains(t.ParentId))
                                    .ToList();
                        var isLast = categoryList.Count();
                        if (isLast < cateidlist.Count)
                        {
                            return new ResultDTO { ResultCode = 2, Message = "只有最后一级分类才能添加商品" };
                        }

                        for (int i = 0; i < cateidlist.Count(); i++)
                        {
                            CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                            comcate.CategoryId = cateidlist[i];
                            comcate.CommodityId = commodity.Id;
                            comcate.SubId = userId;
                            comcate.SubTime = DateTime.Now;
                            comcate.Name = "商品分类";
                            comcate.IsDel = false;
                            comcate.SubId = commodityAndCategoryDTO.AppId;
                            var categoryItem = categoryList.Where(t => t.Id == cateidlist[i]).FirstOrDefault();
                            comcate.AppId = categoryItem.AppId;
                            comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(categoryItem.AppId);
                            contextSession.SaveObject(comcate);
                        }
                    }
                    else
                    {
                        CommodityCategory comcate = CommodityCategory.CreateDefaultCategory(commodity.Id,
                                                                                            commodity.AppId,
                                                                                            commodity.SubId);
                        contextSession.SaveObject(comcate);
                    }

                    #endregion

                    #region 商品品牌
                    if (!string.IsNullOrEmpty(commodityAndCategoryDTO.BrandName))
                    {
                        CommodityInnerBrandFacade cibf = new CommodityInnerBrandFacade();
                        CommodityInnerBrandDTO comBrand = new CommodityInnerBrandDTO();
                        comBrand.BrandId = commodityAndCategoryDTO.BrandId;
                        comBrand.Name = commodityAndCategoryDTO.BrandName;
                        comBrand.CommodityId = commodity.Id;
                        comBrand.SubTime = DateTime.Now;
                        comBrand.ModifiedOn = comBrand.SubTime;
                        comBrand.AppId = commodityAndCategoryDTO.AppId;
                        comBrand.SubId = commodityAndCategoryDTO.SubId;
                        comBrand.CrcAppId = 0;
                        ResultDTO resultCb = cibf.AddComInnerBrand(comBrand);
                    }
                    #endregion

                    #region 商城分类
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.InnerCategoryPath))
                    {
                        string[] innerCatelist = commodityAndCategoryDTO.InnerCategoryPath.Split(',');
                        List<Guid> innerCateidlist = new List<Guid>();
                        List<Guid?> innerCateidlist2 = new List<Guid?>();
                        foreach (string s in innerCatelist)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                innerCateidlist.Add(new Guid(s));
                                innerCateidlist2.Add(new Guid(s));
                            }
                        }
                        if (innerCateidlist.Any())
                        {
                            //只有最后一级分类才能添加商品
                            var categoryList =
                                InnerCategory.ObjectSet()
                                        .Where(t => innerCateidlist.Contains(t.Id) && !innerCateidlist2.Contains(t.ParentId))
                                        .ToList();
                            var isLast = categoryList.Count();
                            if (isLast < innerCateidlist.Count)
                            {
                                return new ResultDTO { ResultCode = 2, Message = "只有最后一级分类才能添加商品" };
                            }

                            for (int i = 0; i < innerCateidlist.Count(); i++)
                            {
                                CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                                comInnerCate.CategoryId = innerCateidlist[i];
                                comInnerCate.CommodityId = commodity.Id;
                                comInnerCate.SubId = userId;
                                comInnerCate.SubTime = DateTime.Now;
                                comInnerCate.Name = "商品分类";
                                comInnerCate.IsDel = false;
                                comInnerCate.SubId = commodityAndCategoryDTO.AppId;
                                var categoryItem = categoryList.Where(t => t.Id == innerCateidlist[i]).FirstOrDefault();
                                comInnerCate.AppId = commodityAndCategoryDTO.AppId;
                                comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(comInnerCate.AppId);
                                contextSession.SaveObject(comInnerCate);
                            }
                        }
                        else
                        {
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateDefaultInnerCategory(commodity.Id, commodity.AppId, commodity.SubId);
                            contextSession.SaveObject(comInnerCate);
                        }
                    }
                    #endregion

                    #region 入驻店铺分类
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.EsCategory))
                    {
                        string[] innerCatelist = commodityAndCategoryDTO.EsCategory.Split(',');
                        var categoryidlist = new List<Guid>();
                        var categorylist2 = new List<Guid?>();
                        foreach (string s in innerCatelist)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                categoryidlist.Add(new Guid(s));
                                categorylist2.Add(new Guid(s));
                            }
                        }
                        if (categoryidlist.Any())
                        {
                            //只有最后一级分类才能添加商品
                            var categoryList =
                                Category.ObjectSet()
                                        .Where(t => categoryidlist.Contains(t.Id) && !categorylist2.Contains(t.ParentId))
                                        .ToList();
                            var isLast = categoryList.Count();
                            if (isLast < categoryidlist.Count)
                            {
                                return new ResultDTO { ResultCode = 2, Message = "只有最后一级分类才能添加商品" };
                            }

                            for (int i = 0; i < categoryidlist.Count(); i++)
                            {
                                CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                                comcate.CategoryId = categoryidlist[i];
                                comcate.CommodityId = commodity.Id;
                                comcate.SubId = userId;
                                comcate.SubTime = DateTime.Now;
                                comcate.Name = "商品分类";
                                comcate.IsDel = false;
                                comcate.SubId = commodityAndCategoryDTO.AppId;
                                var categoryItem = categoryList.Where(t => t.Id == categoryidlist[i]).FirstOrDefault();
                                comcate.AppId = categoryItem.AppId;
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(categoryItem.AppId);
                                contextSession.SaveObject(comcate);
                            }
                        }
                        else
                        {
                            CommodityCategory comcate = CommodityCategory.CreateDefaultCategory(commodity.Id,
                                                                                                commodity.AppId,
                                                                                                commodity.SubId);
                            contextSession.SaveObject(comcate);
                        }
                    }
                    #endregion

                    #region 商品图片

                    int sort = 1;
                    foreach (string picPath in commodityAndCategoryDTO.Picturelist)
                    {
                        ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                        pic.Name = "商品图片";
                        pic.SubId = userId;
                        pic.SubTime = DateTime.Now;
                        pic.PicturesPath = picPath;
                        pic.CommodityId = commodity.Id;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);

                        sort++;
                    }

                    #endregion

                    #region 商品属性

                    //string[] attlist = commodityAndCategoryDTO.AttributeIds.Split(',');
                    //List<Guid> attidlist = new List<Guid>();
                    //foreach (string s in attlist)
                    //{
                    //    if (!string.IsNullOrEmpty(s))
                    //    {
                    //        attidlist.Add(new Guid(s));
                    //    }
                    //}
                    //for (int f = 0; f < attidlist.Count; f++)
                    //{
                    //    var id = attidlist[f];
                    //    var a = SecondAttribute.ObjectSet().Where(n => n.Id == id).FirstOrDefault();
                    //    var b = Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(n => n.Id == a.AttributeId).FirstOrDefault();
                    //    ComAttibute comatt = ComAttibute.CreateComAttibute();
                    //    comatt.SubTime = DateTime.Now;
                    //    comatt.Name = "商品属性";
                    //    comatt.SubId = userId;
                    //    comatt.SecondAttributeId = a.Id;
                    //    comatt.SecondAttributeName = a.Name;
                    //    comatt.AttributeId = b.Id;
                    //    comatt.AttributeName = b.Name;

                    //    comatt.CommodityId = commodity.Id;

                    //    //comAttrList.Add(comatt);//添加到缓存列表
                    //    contextSession.SaveObject(comatt);
                    //}


                    #endregion

                    #region 关联商品
                    if (!string.IsNullOrEmpty(commodityAndCategoryDTO.RelaCommodityList))
                    {
                        string[] relist = commodityAndCategoryDTO.RelaCommodityList.Split(',');
                        for (int i = 0; i < relist.Length; i++)
                        {
                            RelationCommodity relamodel = RelationCommodity.CreateRelationCommodity();
                            //LogHelper.Error("code:" + relist[i].Split('|')[0] + ",appid:" + commodityAndCategoryDTO.AppId);
                            string reIdStr = relist[i].Split('|')[0];
                            Guid reId;
                            Guid.TryParse(reIdStr, out reId);
                            var commodel =
                                Commodity.ObjectSet()
                                         .Where(
                                             p =>
                                             p.Id == reId && p.AppId == commodityAndCategoryDTO.AppId &&
                                             p.CommodityType == 0)
                                         .FirstOrDefault();
                            if (commodel != null)
                            {
                                relamodel.CommodityId = commodity.Id;
                                relamodel.CommodityPicturesPath = commodel.PicturesPath == null
                                                                      ? ""
                                                                      : commodel.PicturesPath;
                                relamodel.CommodityName = commodel.Name;
                                relamodel.RelationCommodityId = commodel.Id;
                                relamodel.No_Code = commodel.No_Code;
                                contextSession.SaveObject(relamodel);
                            }
                        }
                    }

                    #endregion

                    #region 商品日志

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);

                    #endregion

                    #region 舌尖餐盒相关信息
                    if (commodityAndCategoryDTO.From == 1 && (commodityAndCategoryDTO.CommodityBoxCount != -1 || commodityAndCategoryDTO.CommodityBoxPrice != -1))
                    {
                        CateringComdtyXData cbox = CateringComdtyXData.CreateCateringComdtyXData();
                        cbox.ComdtyId = commodity.Id;
                        cbox.SubId = userId;
                        cbox.ModifiedOn = cbox.SubTime = DateTime.Now;
                        cbox.MealBoxNum = commodityAndCategoryDTO.CommodityBoxCount;
                        cbox.MealBoxAmount = commodityAndCategoryDTO.CommodityBoxPrice;
                        cbox.Unit = "";
                        cbox.IsValid = true;
                        contextSession.SaveObject(cbox);
                    }
                    #endregion

                    #region 包装规格设置
                    LogHelper.Info("发布商品时，包装规格参数信息commodityAndCategoryDTO：" + JsonHelper.JsonSerializer(commodityAndCategoryDTO));
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.Specifications))
                    {
                        string[] str = commodityAndCategoryDTO.Specifications.Split(',');
                        foreach (var item in str)
                        {
                            CommoditySpecifications model = new CommoditySpecifications();
                            model.Id = Guid.NewGuid();
                            model.CommodityId = commodity.Id;
                            string[] _str = item.Split('*');
                            int? Attribute = Convert.ToInt32(_str[1]);
                            model.Attribute = Attribute;
                            model.IsDel = false;
                            model.SubTime = DateTime.Now;
                            model.ModifiedOn = DateTime.Now;
                            model.EntityState = EntityState.Added;
                            contextSession.SaveObject(model);
                        }
                    }
                    #endregion

                    var commodityEntityState = commodity.EntityState;
                    contextSession.SaveChanges();
                    commodity.RefreshCache(commodityEntityState);
                    #region //取出新增的商品变动明细插入CommodityChange
                    List<System.Guid> ids = new List<Guid>();
                    ids.Add(commodity.Id);
                    SaveCommodityChange(ids);
                    #endregion
                }
                else
                {
                    return new ResultDTO { ResultCode = 2, Message = "您需要先完善发布者信息才能发布商品" };
                }

                //后台线程发布广场消息
                //System.Threading.ThreadPool.QueueUserWorkItem(a =>
                //{
                if (commodityAndCategoryDTO.State != 2)
                {
                    IUS.Deploy.CustomDTO.PicFromUrlCDTO addDataDTO = new IUS.Deploy.CustomDTO.PicFromUrlCDTO();
                    addDataDTO.AppId = commodityAndCategoryDTO.AppId;
                    addDataDTO.Content = APPSV.GetAppName(commodityAndCategoryDTO.AppId) + "有新品上架了哦，快来看看吧~";
                    addDataDTO.PhotoUrl = commodityAndCategoryDTO.PicturesPath;
                    addDataDTO.ShareUrl = string.Format("{0}Mobile/CommodityList?AppId={1}&sortType=New&type=tuwen",
                                                        CustomConfig.BtpDomain, commodityAndCategoryDTO.AppId);
                    addDataDTO.Source = Jinher.AMP.IUS.Deploy.Enum.SourceEnum.EBusinessInfo;
                    addDataDTO.Title = addDataDTO.Content;
                    addDataDTO.UserId = userId;
                    addDataDTO.UserName = (ContextDTO != null && ContextDTO.LoginUserName != null)
                                              ? ContextDTO.LoginUserName
                                              : "btp";

                    var result = Jinher.AMP.BTP.TPS.IUSSV.Instance.AddPicFromUrl(addDataDTO);

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品发布服务异常。commodityAndCategoryDTO：{0}", commodityAndCategoryDTO), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }

        /// <summary>
        /// 保存库存
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void SaveCommodityStock(Commodity item, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {
                CommodityStock cs = CommodityStock.CreateCommodityStock();
                cs.ComAttribute = "[]";
                cs.Id = item.Id;
                cs.CommodityId = item.Id;
                cs.Price = item.Price;
                cs.MarketPrice = item.MarketPrice;
                cs.Stock = item.Stock;
                cs.Duty = item.Duty;
                cs.Barcode = item.Barcode;
                cs.No_Code = item.No_Code;
                cs.JDCode = item.JDCode;
                cs.ErQiCode = item.ErQiCode;
                cs.CostPrice = item.CostPrice;
                if (isUpdate == true)
                {
                    cs.EntityState = EntityState.Modified;
                }
                contextSession.SaveObject(cs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品发布服务保存库存异常。SaveCommodityStock"), ex);
            }
        }


        private ComAttributeCacheDTO GetComAttributeCacheDTO(ComAttibute comAttr)
        {
            var dto = new ComAttributeCacheDTO
            {
                AttributeId = comAttr.AttributeId,
                AttributeName = comAttr.AttributeName,
                Code = comAttr.Code,
                CommodityId = comAttr.CommodityId,
                Name = comAttr.Name,
                SecondAttributeId = comAttr.SecondAttributeId,
                SecondAttributeName = comAttr.SecondAttributeName,
                SubTime = comAttr.SubTime
            };

            return dto;
        }

        #endregion

        #region 修改商品

        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品实体</param>
        public ResultDTO UpdateCommodityExt(
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO commodityAndCategoryDTO)
        {
            Guid userId = this.ContextDTO.LoginUserID;

            //保存商品属性实体类列表，要添加到缓存中
            //List<ComAttibute> comAttrList = new List<ComAttibute>();
            if (commodityAndCategoryDTO == null)
            {
                return new ResultDTO { ResultCode = 1, Message = "参数不能为空" };
            }
            if (string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code))
            {
                commodityAndCategoryDTO.No_Code = "";
            }
            else
            {
                //if (commodityAndCategoryDTO.No_Code.Length > 30)
                //{
                //    return new ResultDTO { ResultCode = 1, Message = "商品编号不能超过30位" };
                //}
            }

            try
            {
                #region 修改商品
                bool isPrice = false;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Commodity commodityDTO = Commodity.ObjectSet()
                    .FirstOrDefault(n => n.Id == commodityAndCategoryDTO.CommodityId
                        && n.AppId == commodityAndCategoryDTO.AppId && n.CommodityType == 0);

                if (commodityDTO == null)
                {
                    return new ResultDTO { ResultCode = 3, Message = "未找到该商品！" };
                }
                if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code) && commodityAndCategoryDTO.No_Code != commodityDTO.No_Code)
                {
                    if (!IsExistsExt(commodityAndCategoryDTO.No_Code, commodityAndCategoryDTO.AppId))
                    {
                        return new ResultDTO { ResultCode = 2, Message = "商品编号不能重复，该编号已存在" };
                    }
                }
                //到货提醒商品Id集合
                List<Guid> NoticeComIds = new List<Guid>();
                if (commodityDTO.Stock == 0 && commodityAndCategoryDTO.Stock > 0)
                {
                    NoticeComIds.Add(commodityDTO.Id);
                }
                if (commodityDTO.Price > commodityAndCategoryDTO.Price)
                {
                    isPrice = true;
                }
                //2017-11-20新增
                commodityDTO.Type = commodityAndCategoryDTO.CommodityType;
                commodityDTO.YJCouponActivityId = commodityAndCategoryDTO.YJCouponActivityId;
                commodityDTO.YJCouponType = commodityAndCategoryDTO.YJCouponType;

                commodityDTO.EntityState = System.Data.EntityState.Modified;
                commodityDTO.Price = commodityAndCategoryDTO.Price;
                commodityDTO.MarketPrice = commodityAndCategoryDTO.MarketPrice;
                commodityDTO.Duty = commodityAndCategoryDTO.CommodityDuty;
                commodityDTO.TaxRate = commodityAndCategoryDTO.CommodityTaxRate;
                commodityDTO.InputRax = commodityAndCategoryDTO.CommodityInputTax;
                commodityDTO.TaxClassCode = commodityAndCategoryDTO.TaxClassCode;
                commodityDTO.Unit = commodityAndCategoryDTO.Unit;
                commodityDTO.Stock = commodityAndCategoryDTO.Stock;
                commodityDTO.Name = commodityAndCategoryDTO.Name;
                commodityDTO.PicturesPath = commodityAndCategoryDTO.PicturesPath;
                commodityDTO.Description = commodityAndCategoryDTO.Description;
                commodityDTO.IsDel = false;
                commodityDTO.AppId = commodityAndCategoryDTO.AppId;
                commodityDTO.No_Code = commodityAndCategoryDTO.No_Code;
                commodityDTO.State = commodityAndCategoryDTO.State;
                commodityDTO.ModifiedOn = DateTime.Now;
                commodityDTO.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                commodityDTO.CategoryName = commodityAndCategoryDTO.CateNames;
                commodityDTO.IsEnableSelfTake = commodityAndCategoryDTO.IsEnableSelfTake;
                commodityDTO.PricingMethod = commodityAndCategoryDTO.PricingMethod;
                commodityDTO.Weight = commodityDTO.PricingMethod == 1 ? (decimal?)commodityAndCategoryDTO.Weight : null;
                commodityDTO.SaleAreas = commodityAndCategoryDTO.SaleAreas;
                commodityDTO.HtmlVideoPath = commodityAndCategoryDTO.VideoclientUrl;
                commodityDTO.MobileVideoPath = commodityAndCategoryDTO.VideoUrl;
                commodityDTO.VideoPic = commodityAndCategoryDTO.VideoPicUrl;
                commodityDTO.VideoName = commodityAndCategoryDTO.VideoName;
                commodityDTO.ScoreScale = commodityAndCategoryDTO.ScoreScale;

                commodityDTO.Barcode = commodityAndCategoryDTO.BarCode;
                if (string.IsNullOrEmpty(commodityDTO.No_Code) && !string.IsNullOrEmpty(commodityDTO.Barcode))
                {

                    commodityDTO.No_Code = commodityDTO.Barcode + "0000";
                }
                commodityDTO.JDCode = commodityAndCategoryDTO.JDCode;
                commodityDTO.ErQiCode = commodityAndCategoryDTO.ErQiCode;
                commodityDTO.CostPrice = commodityAndCategoryDTO.CostPrice;
                commodityDTO.IsAssurance = commodityAndCategoryDTO.IsAssurance;
                commodityDTO.IsReturns = commodityAndCategoryDTO.IsReturns;
                commodityDTO.Isnsupport = commodityAndCategoryDTO.Isnsupport;
                commodityDTO.ServiceSettingId = commodityAndCategoryDTO.ServiceSettingId;
                commodityDTO.TechSpecs = commodityAndCategoryDTO.TechSpecs;
                commodityDTO.SaleService = commodityAndCategoryDTO.SaleService;
                Guid freightId = Guid.Empty;
                if (Guid.TryParse(commodityAndCategoryDTO.FreightId, out freightId))
                {
                    var freightTemplate = FreightTemplate.ObjectSet().FirstOrDefault(c => c.Id == freightId);
                    if (freightTemplate == null ||
                        (freightTemplate.PricingMethod != commodityAndCategoryDTO.PricingMethod &&
                         freightTemplate.ExpressType != 1))
                    {
                        return new ResultDTO { ResultCode = 2, Message = "运费模板选择错误" };
                    }

                    commodityDTO.FreightTemplateId = freightId;
                }
                else
                {
                    commodityDTO.FreightTemplateId = null;
                }

                #region 商品属性
                List<ComAttributeDTO> attrList = new List<ComAttributeDTO>();
                if (commodityAndCategoryDTO.ColorNames != null)
                {
                    string[] colors = commodityAndCategoryDTO.ColorNames.Split(',').Distinct().ToArray();
                    if (colors.Length > 0)
                    {
                        foreach (string color in colors)
                        {
                            attrList.Add(new ComAttributeDTO { Attribute = "颜色", SecondAttribute = color });
                        }
                    }
                }
                if (commodityAndCategoryDTO.SizeNames != null)
                {
                    string[] sizes = commodityAndCategoryDTO.SizeNames.Split(',').Distinct().ToArray();
                    if (sizes.Length > 0)
                    {
                        foreach (string size in sizes)
                        {
                            attrList.Add(new ComAttributeDTO { Attribute = "尺寸", SecondAttribute = size });
                        }
                    }
                }
                commodityDTO.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(attrList);
                #endregion

                #region 商品新属性逻辑
                //删除老的属性，取数据的时候已经记录的Id,所有可以直接删除
                var cStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == commodityDTO.Id).ToList();
                if (cStock != null && cStock.Count > 0) //组合属性
                {
                    foreach (var item in cStock)
                    {
                        item.EntityState = System.Data.EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                }

                //zgx-modify
                List<ComAttributeDTO> allAttrList = new List<ComAttributeDTO>();
                int StockTotal = 0;
                //判断是否有组合属性
                if (commodityAndCategoryDTO.ComAttributes != null && commodityAndCategoryDTO.ComAttributes.Count > 0)
                {
                    decimal maxMarketPrice = decimal.MaxValue;
                    decimal maxDuty = decimal.MaxValue;
                    decimal maxPrice = decimal.MaxValue;
                    decimal? maxCostPrice = decimal.MaxValue;

                    foreach (var item in commodityAndCategoryDTO.ComAttributes)
                    {
                        item.ComAttribute = new List<ComAttributeDTO>();
                        item.ComAttributeIds.ForEach(
                            r =>
                            item.ComAttribute.Add(new ComAttributeDTO()
                            {
                                Attribute = r.Attribute,
                                SecondAttribute = r.SecondAttribute
                            }));

                        CommodityStock cs = CommodityStock.CreateCommodityStock();
                        cs.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(item.ComAttribute);

                        foreach (var attr in item.ComAttribute)
                        {
                            if (
                                !allAttrList.Exists(
                                    r => r.Attribute == attr.Attribute && r.SecondAttribute == attr.SecondAttribute))
                            {
                                allAttrList.Add(attr);
                            }
                        }

                        cs.CommodityId = commodityDTO.Id;
                        cs.Price = item.Price;
                        cs.MarketPrice = item.MarketPrice;
                        cs.Duty = item.Duty;
                        cs.Stock = item.Stock;

                        // 20170918新增
                        cs.Barcode = item.BarCode;
                        cs.No_Code = item.Code;

                        // 20171102新增
                        cs.ThumImg = item.ThumImg;
                        cs.CarouselImgs = item.CarouselImgs;

                        if (string.IsNullOrEmpty(cs.No_Code) && !string.IsNullOrEmpty(cs.Barcode))
                        {
                            cs.No_Code = cs.Barcode + "0000";
                        }
                        cs.JDCode = item.JDCode;
                        cs.ErQiCode = item.ErQiCode;
                        cs.CostPrice = item.CostPrice;

                        if (StockTotal < int.MaxValue && cs.Stock < int.MaxValue)
                            StockTotal += cs.Stock;
                        else
                            StockTotal = cs.Stock;
                        if (item.Price > 0 && item.Price < maxPrice)
                        {
                            maxPrice = item.Price;
                            commodityDTO.Price = maxPrice;
                            if (!string.IsNullOrEmpty(cs.No_Code)) commodityDTO.No_Code = cs.No_Code;
                            commodityDTO.JDCode = item.JDCode;
                            commodityDTO.Barcode = item.BarCode;
                        }

                        if (item.CostPrice > 0 && item.CostPrice < maxCostPrice)
                        {
                            maxCostPrice = item.CostPrice;
                            commodityDTO.CostPrice = maxCostPrice;
                        }

                        if (item.MarketPrice > 0 && item.MarketPrice < maxMarketPrice)
                        {
                            maxMarketPrice = item.MarketPrice.Value;
                        }
                        if (item.Duty > 0 && item.Duty < maxDuty)
                        {
                            maxDuty = item.Duty.Value;
                        }
                        if (item.Id != Guid.Empty) //判断是否已经存在
                        {
                            cs.Id = item.Id;
                        }

                        contextSession.SaveObject(cs);
                    }
                    if (maxMarketPrice < decimal.MaxValue)
                    {
                        commodityDTO.MarketPrice = maxMarketPrice;
                    }
                    else
                    {
                        commodityDTO.MarketPrice = null;
                    }
                    if (maxDuty < decimal.MaxValue)
                    {
                        commodityDTO.Duty = maxDuty;
                    }
                    else
                    {
                        commodityDTO.Duty = null;
                    }

                    commodityDTO.Stock = StockTotal;
                } //判断是否是一种属性
                else if (!string.IsNullOrEmpty(commodityAndCategoryDTO.AttrValueNames))
                {
                    string[] attrValues = commodityAndCategoryDTO.AttrValueNames.Split(',').Distinct().ToArray();
                    if (attrValues.Length > 0)
                    {
                        foreach (string attrValue in attrValues)
                        {
                            allAttrList.Add(new ComAttributeDTO
                            {
                                Attribute = commodityAndCategoryDTO.AttrName,
                                SecondAttribute = attrValue
                            });
                        }
                    }
                }
                commodityDTO.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(allAttrList);



                #endregion


                #endregion

                //if (commodityDTO.State == 0)
                //{

                //    UpdateOrderPrice(commodityDTO.Id, commodityDTO.Price, contextSession);
                //}

                #region 关联商品

                RelationCommodity.ObjectSet()
                                 .Context.ExecuteStoreCommand("delete from RelationCommodity where CommodityId ='" +
                                                              commodityAndCategoryDTO.CommodityId + "'");
                if (!string.IsNullOrEmpty(commodityAndCategoryDTO.RelaCommodityList))
                {
                    string[] relist = commodityAndCategoryDTO.RelaCommodityList.Split(',');
                    for (int i = 0; i < relist.Length; i++)
                    {
                        RelationCommodity relamodel = RelationCommodity.CreateRelationCommodity();
                        //LogHelper.Error("code:" + relist[i].Split('|')[0] + ",appid:" + commodityAndCategoryDTO.AppId);
                        string reIdStr = relist[i].Split('|')[0];
                        Guid reId;
                        Guid.TryParse(reIdStr, out reId);

                        var commodel =
                            Commodity.ObjectSet()
                                     .Where(
                                         p =>
                                         p.Id == reId && p.AppId == commodityAndCategoryDTO.AppId &&
                                         p.CommodityType == 0)
                                     .FirstOrDefault();
                        if (commodel != null)
                        {
                            relamodel.CommodityId = commodityAndCategoryDTO.CommodityId;
                            relamodel.CommodityPicturesPath = commodel.PicturesPath == null ? "" : commodel.PicturesPath;
                            relamodel.CommodityName = commodel.Name;
                            relamodel.RelationCommodityId = commodel.Id;
                            relamodel.No_Code = commodel.No_Code;
                            contextSession.SaveObject(relamodel);
                        }
                    }
                }

                #endregion

                #region 删除原商品分类
                var catList = CommodityCategory.ObjectSet()
                    .Where(c => c.CommodityId == commodityDTO.Id && commodityDTO.AppId == c.AppId).ToList();
                foreach (var commodityCategory in catList)
                {
                    contextSession.Delete(commodityCategory);
                }
                if (!string.IsNullOrEmpty(commodityAndCategoryDTO.CategoryPath) &&
                    commodityAndCategoryDTO.CategoryPath != ",")
                {
                    #region 添加商品分类
                    if (commodityAndCategoryDTO.CategoryPath != null)
                    {
                        string[] catelist = commodityAndCategoryDTO.CategoryPath.Split(',');
                        List<Guid> cateidlist = new List<Guid>();
                        List<Guid?> cateidlist2 = new List<Guid?>();
                        foreach (string s in catelist)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                cateidlist.Add(new Guid(s));
                                cateidlist2.Add(new Guid(s));
                            }
                        }
                        //只有最后一级分类才能添加商品
                        //只有最后一级分类才能添加商品
                        var categoryList = Category.ObjectSet()
                            .Where(t => cateidlist.Contains(t.Id) && !cateidlist2.Contains(t.ParentId)).ToList();
                        var isLast = categoryList.Count();
                        if (isLast < cateidlist.Count)
                        {
                            return new ResultDTO { ResultCode = 2, Message = "只有最后一级分类才能添加商品" };
                        }
                        for (int i = 0; i < cateidlist.Count(); i++)
                        {
                            CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                            comcate.CategoryId = cateidlist[i];
                            comcate.CommodityId = commodityDTO.Id;
                            comcate.SubTime = DateTime.Now;
                            comcate.Name = "商品分类";
                            comcate.IsDel = false;
                            var categoryItem = categoryList.Where(t => t.Id == cateidlist[i]).FirstOrDefault();
                            comcate.AppId = categoryItem.AppId;
                            comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(categoryItem.AppId);
                            comcate.EntityState = System.Data.EntityState.Added;
                            contextSession.SaveObject(comcate);
                        }
                    }

                    #endregion
                }
                else
                {
                    CommodityCategory comcate = CommodityCategory.CreateDefaultCategory(commodityDTO.Id, commodityDTO.AppId, commodityDTO.SubId);
                    contextSession.SaveObject(comcate);
                }
                #endregion

                #region 删除原商品品牌
                CommodityInnerBrandFacade cibf = new CommodityInnerBrandFacade();
                ResultDTO res = new ResultDTO() { isSuccess = false };
                var cibrand = CommodityInnerBrand.ObjectSet().FirstOrDefault(c => c.CommodityId == commodityDTO.Id);
                if (cibrand != null)
                {
                    res = cibf.DelComInnerBrand(cibrand.CommodityId);
                }
                #region 添加商品品牌
                if (!string.IsNullOrEmpty(commodityAndCategoryDTO.BrandName))
                {
                    CommodityInnerBrandDTO comBrand = new CommodityInnerBrandDTO();
                    comBrand.BrandId = commodityAndCategoryDTO.BrandId;
                    comBrand.Name = commodityAndCategoryDTO.BrandName;
                    comBrand.CommodityId = commodityAndCategoryDTO.CommodityId;
                    comBrand.SubTime = DateTime.Now;
                    comBrand.ModifiedOn = comBrand.SubTime;
                    comBrand.AppId = commodityAndCategoryDTO.AppId;
                    comBrand.SubId = commodityAndCategoryDTO.SubId;
                    comBrand.CrcAppId = 0;
                    ResultDTO resultCb = cibf.AddComInnerBrand(comBrand);
                }
                #endregion
                #endregion

                #region 删除原商城分类
                if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.InnerCategoryPath))
                {
                    var oldCCs = CommodityInnerCategory.ObjectSet()
                        .Where(c => c.CommodityId == commodityDTO.Id && commodityDTO.AppId == c.AppId).ToList();
                    foreach (var commodityCategory in oldCCs)
                    {
                        contextSession.Delete(commodityCategory);
                    }
                    if (!string.IsNullOrEmpty(commodityAndCategoryDTO.InnerCategoryPath) &&
                        commodityAndCategoryDTO.InnerCategoryPath != ",")
                    {
                        #region 添加商品分类
                        string[] innerCatelist = commodityAndCategoryDTO.InnerCategoryPath.Split(',');
                        List<Guid> innerCateidlist = new List<Guid>();
                        List<Guid?> innerCateidlist2 = new List<Guid?>();
                        foreach (string s in innerCatelist)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                innerCateidlist.Add(new Guid(s));
                                innerCateidlist2.Add(new Guid(s));
                            }
                        }
                        if (innerCateidlist.Any())
                        {
                            //只有最后一级分类才能添加商品
                            var categoryList = InnerCategory.ObjectSet()
                                .Where(t => innerCateidlist.Contains(t.Id) && !innerCateidlist2.Contains(t.ParentId)).ToList();
                            var isLast = categoryList.Count();
                            if (isLast < innerCateidlist.Count)
                            {
                                return new ResultDTO { ResultCode = 2, Message = "只有最后一级分类才能添加商品" };
                            }
                            for (int i = 0; i < innerCateidlist.Count(); i++)
                            {
                                CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                                comInnerCate.CategoryId = innerCateidlist[i];
                                comInnerCate.CommodityId = commodityDTO.Id;
                                comInnerCate.SubId = userId;
                                comInnerCate.SubTime = DateTime.Now;
                                comInnerCate.Name = "商品分类";
                                comInnerCate.IsDel = false;
                                comInnerCate.SubId = commodityAndCategoryDTO.AppId;
                                var categoryItem = categoryList.Where(t => t.Id == innerCateidlist[i]).FirstOrDefault();
                                comInnerCate.AppId = commodityAndCategoryDTO.AppId;
                                comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(comInnerCate.AppId);
                                contextSession.SaveObject(comInnerCate);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateDefaultInnerCategory(commodityDTO.Id, commodityDTO.AppId, commodityDTO.SubId);
                        contextSession.SaveObject(comInnerCate);
                    }
                }
                #endregion

                #region 商品属性

                //if (!string.IsNullOrEmpty(commodityAndCategoryDTO.AttributeIds) && commodityAndCategoryDTO.AttributeIds != ",")
                //{
                //    var comatt = ComAttibute.ObjectSet().Where(n => n.CommodityId == commodityDTO.Id);
                //    foreach (var itme in comatt)
                //    {
                //        contextSession.Delete(itme);
                //    }
                //    if (commodityAndCategoryDTO.AttributeIds != null && commodityAndCategoryDTO.AttributeIds != ",")
                //    {
                //        string[] comatts = commodityAndCategoryDTO.AttributeIds.Trim().Split(',');
                //        for (int f = 0; f < comatts.Length; f++)
                //        {
                //            if (comatts[f] != "")
                //            {
                //                var id = new Guid(comatts[f]);
                //                var a = SecondAttribute.ObjectSet().Where(n => n.Id == id).FirstOrDefault();
                //                var b = Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(n => n.Id == a.AttributeId).FirstOrDefault();
                //                ComAttibute newcomatt = ComAttibute.CreateComAttibute();
                //                newcomatt.SubTime = DateTime.Now;
                //                newcomatt.Name = "商品属性";
                //                newcomatt.SecondAttributeId = a.Id;
                //                newcomatt.SecondAttributeName = a.Name;
                //                newcomatt.AttributeId = b.Id;
                //                newcomatt.AttributeName = b.Name;
                //                newcomatt.CommodityId = commodityDTO.Id;
                //                newcomatt.EntityState = System.Data.EntityState.Added;

                //                //comAttrList.Add(newcomatt);//添加到缓存列表
                //                contextSession.SaveObject(newcomatt);
                //            }
                //        }
                //    }
                //}

                #endregion

                #region 商品图片

                ProductDetailsPictureBP pdpbp = new ProductDetailsPictureBP();
                pdpbp.DeletePictures(commodityAndCategoryDTO.CommodityId);
                int sort = 1;
                foreach (string picPath in commodityAndCategoryDTO.Picturelist)
                {
                    ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                    pic.Name = "商品图片";
                    pic.SubId = userId;
                    pic.SubTime = DateTime.Now;
                    pic.PicturesPath = picPath;
                    pic.CommodityId = commodityAndCategoryDTO.CommodityId;
                    pic.Sort = sort;
                    pic.EntityState = System.Data.EntityState.Added;
                    contextSession.SaveObject(pic);

                    sort++;
                }

                #endregion

                #region 舌尖餐盒相关信息
                if (commodityAndCategoryDTO.From == 1)
                {
                    CateringComdtyXData cbox = CateringComdtyXData.ObjectSet().Where(r => r.ComdtyId == commodityAndCategoryDTO.CommodityId).FirstOrDefault();
                    if ((commodityAndCategoryDTO.CommodityBoxCount != -1 || commodityAndCategoryDTO.CommodityBoxPrice != -1))
                    {
                        if (cbox == null)
                        {
                            cbox = CateringComdtyXData.CreateCateringComdtyXData();
                            cbox.SubTime = DateTime.Now;
                            cbox.Unit = "";
                            cbox.SubId = userId;
                            cbox.IsValid = true;
                        }
                        else
                        {
                            cbox.EntityState = EntityState.Modified;
                        }
                        cbox.MealBoxNum = commodityAndCategoryDTO.CommodityBoxCount;
                        cbox.MealBoxAmount = commodityAndCategoryDTO.CommodityBoxPrice;
                    }
                    else if (cbox != null)
                    {
                        cbox.IsValid = false;
                    }
                    if (cbox != null)
                    {
                        cbox.ModifiedOn = DateTime.Now;
                        contextSession.SaveObject(cbox);
                    }
                }
                #endregion

                #region 包装规格设置
                var commoditySpecifications = CommoditySpecifications.ObjectSet().Where(p => p.CommodityId == commodityAndCategoryDTO.CommodityId).ToList();
                if (commoditySpecifications.Count() > 0)
                {
                    foreach (var item in commoditySpecifications)
                    {
                        item.EntityState = EntityState.Deleted;
                        contextSession.Delete(item);
                    }
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.Specifications))
                    {
                        string[] str = commodityAndCategoryDTO.Specifications.Split(',');
                        foreach (var item in str)
                        {
                            CommoditySpecifications model = new CommoditySpecifications();
                            model.Id = Guid.NewGuid();
                            model.CommodityId = commodityAndCategoryDTO.CommodityId;
                            string[] _str = item.Split('*');
                            int? Attribute = Convert.ToInt32(_str[1]);
                            model.Attribute = Attribute;
                            model.IsDel = false;
                            model.SubTime = DateTime.Now;
                            model.ModifiedOn = DateTime.Now;
                            model.EntityState = EntityState.Added;
                            contextSession.SaveObject(model);
                        }

                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.Specifications))
                    {
                        string[] str = commodityAndCategoryDTO.Specifications.Split(',');
                        foreach (var item in str)
                        {
                            CommoditySpecifications model = new CommoditySpecifications();
                            model.Id = Guid.NewGuid();
                            model.CommodityId = commodityAndCategoryDTO.CommodityId;
                            string[] _str = item.Split('*');
                            int? Attribute = Convert.ToInt32(_str[1]);
                            model.Attribute = Attribute;
                            model.IsDel = false;
                            model.SubTime = DateTime.Now;
                            model.ModifiedOn = DateTime.Now;
                            model.EntityState = EntityState.Added;
                            contextSession.SaveObject(model);
                        }
                    }
                }

                #endregion

                if (commodityDTO.State == 0)
                {
                    HotCommodity hotCommodity =
                        HotCommodity.ObjectSet()
                                    .FirstOrDefault(c => c.CommodityId == commodityAndCategoryDTO.CommodityId);
                    if (hotCommodity != null)
                    {
                        hotCommodity.Name = commodityDTO.Name;
                        hotCommodity.PicturesPath = commodityDTO.PicturesPath;
                        hotCommodity.Price = commodityDTO.Price;
                        hotCommodity.TotalReview = commodityDTO.TotalReview;
                        hotCommodity.TotalCollection = commodityDTO.TotalCollection;
                        hotCommodity.State = commodityDTO.State;
                        hotCommodity.Stock = commodityDTO.Stock;
                        hotCommodity.Salesvolume = commodityDTO.Salesvolume;
                        hotCommodity.AppId = commodityDTO.AppId;
                        hotCommodity.ModifiedOn = DateTime.Now;
                        hotCommodity.EntityState = EntityState.Modified;
                        contextSession.SaveObject(hotCommodity);

                    }
                }

                contextSession.SaveObject(commodityDTO);
                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodityDTO);
                contextSession.SaveObject(journal);
                var commodityEntityState = commodityDTO.EntityState;
                contextSession.SaveChanges();
                commodityDTO.RefreshCache(commodityEntityState);

                contextSession.SaveChanges();

                //调用到货提醒接口
                if (NoticeComIds.Any())
                {
                    ZPHSV.SendStockNotifications(NoticeComIds);
                }

                //更新商品变动表数据
                List<System.Guid> ids = new List<Guid>();
                ids.Add(commodityAndCategoryDTO.CommodityId);
                SaveCommodityChange(ids);
                ////后台线程更新商品属性缓存
                //System.Threading.ThreadPool.QueueUserWorkItem(a =>
                //{
                //    List<ComAttributeCacheDTO> cache = new List<ComAttributeCacheDTO>();

                //    foreach (ComAttibute comAttr in comAttrList)
                //    {
                //        cache.Add(GetComAttributeCacheDTO(comAttr));
                //    }
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CommodityAttrInfo", commodityDTO.Id.ToString(), "BTPCache");
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Add("G_CommodityAttrInfo", commodityDTO.Id.ToString(), cache, "BTPCache");
                //});
            }
            catch (Exception ex)
            {
                LogHelper.Error(
                    string.Format("修改商品服务异常。commodityAndCategoryDTO：{0}",
                                  JsonHelper.JsonSerializer(commodityAndCategoryDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 查询某个商家所有上架商品

        /// <summary>
        /// 查询某个商家所有上架商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM>
            GetAllCommodityBySellerIDBySalesvolumeExt(CommoditySearchDTO search, out int rowCount)
        {
            var query =
                Commodity.ObjectSet()
                         .Where(
                             n =>
                             n.IsDel.Equals(false) && n.AppId.Equals(search.appId) && n.State == 0 &&
                             n.CommodityType == 0);

            #region 条件查询

            if (!string.IsNullOrEmpty(search.commodityName))
            {
                query = query.Where(n => n.Name.Contains(search.commodityName));
            }
            if (!string.IsNullOrEmpty(search.sSalesvolume))
            {
                int s = int.Parse(search.sSalesvolume);
                query = query.Where(n => n.Salesvolume >= s);
            }
            if (!string.IsNullOrEmpty(search.eSalesvolume))
            {
                int e = int.Parse(search.eSalesvolume);
                query = query.Where(n => n.Salesvolume <= e);
            }
            if (!string.IsNullOrEmpty(search.sPrice))
            {
                decimal s = 0;
                if (!decimal.TryParse(search.sPrice, out s)) //长度越界
                {
                    rowCount = 0;
                    return new System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM>();
                }
                query = query.Where(n => n.Price >= s);
            }
            if (!string.IsNullOrEmpty(search.ePrice))
            {
                decimal e = 0;
                if (!decimal.TryParse(search.ePrice, out e)) //长度越界
                {
                    rowCount = 0;
                    return new System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM>();
                }
                query = query.Where(n => n.Price <= e);
            }
            if (!string.IsNullOrEmpty(search.commodityCategory))
            {
                string[] commodityCategoryID = search.commodityCategory.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string commodityCategoryid in commodityCategoryID)
                {
                    if (!string.IsNullOrEmpty(commodityCategoryid))
                    {
                        idlist.Add(new Guid(commodityCategoryid));
                    }
                }
                query = from n in query
                        join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                        where idlist.Contains(m.CategoryId)
                        select n;
            }

            #endregion

            query = query.Distinct();
            rowCount = query.Count();
            query =
                query.OrderBy(n => n.SortValue)
                     .ThenByDescending(n => n.SubTime)
                     .Skip((search.pageIndex - 1) * search.pageSize)
                     .Take(search.pageSize);

            List<CommodityVM> comcalist = (from n in query
                                           select new CommodityVM
                                           {
                                               Name = n.Name,
                                               Id = n.Id,
                                               CollectNum = n.TotalCollection,
                                               ReviewNum = n.TotalReview,
                                               Pic = n.PicturesPath,
                                               Price = n.Price,
                                               MarketPrice = n.MarketPrice,
                                               State = n.State,
                                               Stock = n.Stock,
                                               Subtime = n.SubTime,
                                               Total = n.Salesvolume,
                                               Code = n.No_Code,
                                               IsEnableSelfTake = n.IsEnableSelfTake,
                                               CostPrice = n.CostPrice
                                           }).ToList();


            //获取商品id数组
            List<Guid> commodityIdList = new List<Guid>();
            foreach (var item in comcalist)
            {
                commodityIdList.Add(item.Id);
            }

            //获取三级佣金设置
            var commodityDistributionList =
                CommodityDistribution.ObjectSet().Where(t => commodityIdList.Contains(t.Id)).ToList();

            foreach (var item in comcalist)
            {
                var commodityDistribution = commodityDistributionList.Where(t => t.Id == item.Id).FirstOrDefault();
                if (commodityDistribution != null)
                {
                    item.IsDistribute = 1;
                    item.L1Percent = commodityDistribution.L1Percent;
                    item.L2Percent = commodityDistribution.L2Percent;
                    item.L3Percent = commodityDistribution.L3Percent;
                }
            }

            //zgx-modify
            var comAttributeList = (from a in CommodityStock.ObjectSet()
                                    where commodityIdList.Contains(a.CommodityId)
                                    group a by a.CommodityId
                                        into g
                                    select new
                                    {
                                        minPrice = g.Min(a => a.Price),
                                        maxPrice = g.Max(a => a.Price),
                                        CommodityId = g.Key
                                    }).ToList();


            //获取类目列表
            var categoryList = (from a in Category.ObjectSet()
                                join b in CommodityCategory.ObjectSet()
                                    on a.Id equals b.CategoryId
                                where commodityIdList.Contains(b.CommodityId) && b.AppId == search.appId
                                select new
                                {
                                    Category = a,
                                    CommodityId = b.CommodityId
                                }).ToList();
            //将类目添加到对应的商品
            comcalist.ForEach(a =>
            {
                if (a.Categorys == null)
                    a.Categorys = new List<CategoryDTO>();
                categoryList.ForEach(b =>
                {
                    if (b.CommodityId == a.Id)
                    {
                        a.Categorys.Add(b.Category.ToEntityData());
                    }
                });

                //zgx-modify
                if (comAttributeList != null && comAttributeList.Count > 0)
                {
                    var comAttribute = comAttributeList.Find(r => r.CommodityId == a.Id);
                    if (comAttribute != null)
                    {
                        a.MaxPrice = comAttribute.maxPrice;
                        a.MinPrice = comAttribute.minPrice;
                        var commodityStock = CommodityStock.ObjectSet().FirstOrDefault(t => t.CommodityId == comAttribute.CommodityId);
                        if (commodityStock != null && (commodityStock.ComAttribute != "[]" && commodityStock.ComAttribute != ""))
                        {
                            a.HaveAttr = 1;
                        }
                    }
                }
            });

            return comcalist;
        }

        #endregion

        #region 查询某个商家所有下架商品

        /// <summary>
        /// 查询某个商家所有下架商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM>
            GetAllNoOnSaleCommodityBySellerIDBySalesvolumeExt(CommoditySearchDTO search, out int rowCount)
        {
            var query =
                Commodity.ObjectSet()
                         .Where(
                             n =>
                             n.IsDel.Equals(false) && n.AppId.Equals(search.appId) && (n.State == 1 || n.State == 2) &&
                             n.CommodityType == 0);

            #region 条件查询

            if (!string.IsNullOrEmpty(search.commodityName))
            {
                query = query.Where(n => n.Name.Contains(search.commodityName));
            }
            if (!string.IsNullOrEmpty(search.sSalesvolume))
            {
                int s = int.Parse(search.sSalesvolume);
                query = query.Where(n => n.Salesvolume >= s);
            }
            if (!string.IsNullOrEmpty(search.eSalesvolume))
            {
                int e = int.Parse(search.eSalesvolume);
                query = query.Where(n => n.Salesvolume <= e);
            }
            if (!string.IsNullOrEmpty(search.sPrice))
            {
                decimal s = 0;
                if (!decimal.TryParse(search.sPrice, out s)) //长度越界
                {
                    rowCount = 0;
                    return new List<CommodityVM>();
                }
                query = query.Where(n => n.Price >= s);
            }
            if (!string.IsNullOrEmpty(search.ePrice))
            {
                decimal e = 0;
                if (!decimal.TryParse(search.ePrice, out e)) //长度越界
                {
                    rowCount = 0;
                    return new List<CommodityVM>();
                }
                query = query.Where(n => n.Price <= e);
            }
            if (!string.IsNullOrEmpty(search.commodityCategory))
            {
                string[] commodityCategoryID = search.commodityCategory.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string commodityCategoryid in commodityCategoryID)
                {
                    if (!string.IsNullOrEmpty(commodityCategoryid))
                    {
                        idlist.Add(new Guid(commodityCategoryid));
                    }
                }
                query = from n in query
                        join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                        where idlist.Contains(m.CategoryId)
                        select n;
            }

            #endregion

            rowCount = query.Count();
            query =
                query.Distinct()
                     .OrderByDescending(n => n.SubTime)
                     .Skip((search.pageIndex - 1) * search.pageSize)
                     .Take(search.pageSize);

            List<CommodityVM> comcalist = (from n in query
                                           select new CommodityVM
                                           {
                                               Name = n.Name,
                                               Id = n.Id,
                                               CollectNum = n.TotalCollection,
                                               ReviewNum = n.TotalReview,
                                               Pic = n.PicturesPath,
                                               Price = n.Price,
                                               MarketPrice = n.MarketPrice,
                                               State = n.State,
                                               Stock = n.Stock,
                                               Subtime = n.SubTime,
                                               Total = n.Salesvolume,
                                               IsEnableSelfTake = n.IsEnableSelfTake
                                           }).ToList();
            List<Guid> commodityIdList = new List<Guid>();
            foreach (var item in comcalist)
            {
                commodityIdList.Add(item.Id);
            }


            //zgx-modify
            var comAttributeList = (from a in CommodityStock.ObjectSet()
                                    where commodityIdList.Contains(a.CommodityId)
                                    group a by a.CommodityId
                                        into g
                                    select new
                                    {
                                        minPrice = g.Min(a => a.Price),
                                        maxPrice = g.Max(a => a.Price),
                                        CommodityId = g.Key
                                    }).ToList();

            //获取类目列表
            var categoryList = (from a in Category.ObjectSet()
                                join b in CommodityCategory.ObjectSet()
                                    on a.Id equals b.CategoryId
                                where commodityIdList.Contains(b.CommodityId) && b.AppId == search.appId
                                select new
                                {
                                    Category = a,
                                    CommodityId = b.CommodityId
                                }).ToList();
            //将类目添加到对应的商品
            comcalist.ForEach(a =>
            {
                if (a.Categorys == null)
                    a.Categorys = new List<CategoryDTO>();
                categoryList.ForEach(b =>
                {
                    if (b.CommodityId == a.Id)
                    {
                        a.Categorys.Add(b.Category.ToEntityData());
                    }
                });

                //zgx-modify
                if (comAttributeList != null && comAttributeList.Count > 0)
                {
                    var comAttribute = comAttributeList.Find(r => r.CommodityId == a.Id);
                    if (comAttribute != null)
                    {
                        a.MaxPrice = comAttribute.maxPrice;
                        a.MinPrice = comAttribute.minPrice;
                        a.HaveAttr = 1;
                    }
                }

            });

            return comcalist;
        }

        #endregion

        #region 查询商家所有商品

        /// <summary>
        /// 查询商家所有商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetAllCommodityBySellerIDExt(
            System.Guid id, int pageSize, int pageIndex)
        {
            var commodityDTO =
                Commodity.ObjectSet()
                         .Where(n => n.IsDel.Equals(false) && n.AppId.Equals(id) && n.State == 0 && n.CommodityType == 0)
                         .OrderByDescending(n => n.SubTime)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize);

            var result = from c in commodityDTO
                         select new CommodityDTO
                         {
                             AppId = c.AppId,
                             Code = c.Code,
                             CategoryName = c.CategoryName,
                             ComAttribute = c.ComAttribute,
                             Description = c.Description,
                             GroundTime = c.GroundTime,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             Name = c.Name,
                             No_Code = c.No_Code,
                             No_Number = c.No_Number,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             State = c.State,
                             MarketPrice = c.MarketPrice,
                             Duty = c.Duty,
                             Stock = c.Stock,
                             TotalReview = c.TotalReview,
                             TotalCollection = c.TotalCollection,
                             Salesvolume = c.Salesvolume,
                             IsEnableSelfTake = c.IsEnableSelfTake
                         };
            return result.ToList();

        }

        #endregion

        #region 商品详情

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO GetCommodityExt(System.Guid commodityId,
                                                                                       Guid appId)
        {
            CommodityAndCategoryDTO Cac = new CommodityAndCategoryDTO();
            Guid sizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            Guid colorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            Commodity commodity = new Commodity();

            //尽可能并行执行这些方法
            System.Threading.Tasks.Parallel.Invoke(() =>
            {
                commodity = Commodity.ObjectSet().Where(n => n.Id == commodityId && n.CommodityType == 0).FirstOrDefault();

                List<CommodityCategory> category = CommodityCategory.ObjectSet()
                    .Where(n => n.CommodityId == commodityId && n.AppId == appId).ToList();

                //获取关联商品
                List<RelationCommodity> relacomList =
                    RelationCommodity.ObjectSet().Where(p => p.CommodityId == commodityId).ToList();
                if (relacomList.Count > 0)
                {
                    relacomList.ForEach(a =>
                    {
                        string strRela = "";
                        foreach (var item in relacomList)
                        {
                            strRela += item.RelationCommodityId + "|1,";
                        }
                        strRela = strRela.Substring(0, strRela.Length - 1);
                        Cac.RelaCommodityList = strRela;
                    });
                }
                else
                {
                    Cac.RelaCommodityList = "";
                }

                //获取需要取的类目信息
                //类目id列表
                List<Guid> categoryIdList = new List<Guid>();
                category.ForEach(a => { categoryIdList.Add(a.CategoryId); });
                List<Category> categoryList =
                    Category.ObjectSet().Where(a => categoryIdList.Contains(a.Id)).ToList();
                category.ForEach(a =>
                {
                    Category c = categoryList.Where(cat => cat.Id == a.CategoryId).FirstOrDefault();
                    if (c != null)
                    {
                        Cac.CategoryPath += a.Id + ",";
                        Cac.CateNames += c.Name + ",";
                        Cac.CategoryIds += c.Id.ToString() + ",";
                    }
                });

                if (Cac.CategoryPath != null)
                {
                    Cac.CategoryPath = Cac.CategoryPath.Substring(0, Cac.CategoryPath.Length - 1);
                    Cac.CateNames = Cac.CateNames.Substring(0, Cac.CateNames.Length - 1);
                    Cac.CategoryIds = Cac.CategoryIds.Substring(0, Cac.CategoryIds.Length - 1);
                }

                //获取需要取的商城类目信息
                List<CommodityInnerCategory> innerCategory = CommodityInnerCategory.ObjectSet()
.Where(n => n.CommodityId == commodityId && n.AppId == appId).ToList();
                List<Guid> innerCategoryIdList = innerCategory.Select(_ => _.CategoryId).ToList();
                List<InnerCategory> innerCategoryList = InnerCategory.ObjectSet().Where(a => innerCategoryIdList.Contains(a.Id)).ToList();
                innerCategory.ForEach(a =>
                {
                    InnerCategory c = innerCategoryList.Where(cat => cat.Id == a.CategoryId).FirstOrDefault();
                    if (c != null)
                    {
                        Cac.InnerCategoryPath += a.Id + ",";
                        Cac.InnerCateNames += c.Name + ",";
                        Cac.InnerCategoryIds += c.Id.ToString() + ",";
                    }
                });

                if (Cac.InnerCategoryPath != null)
                {
                    Cac.InnerCategoryPath = Cac.InnerCategoryPath.Substring(0, Cac.InnerCategoryPath.Length - 1);
                    Cac.InnerCateNames = Cac.InnerCateNames.Substring(0, Cac.InnerCateNames.Length - 1);
                    Cac.InnerCategoryIds = Cac.InnerCategoryIds.Substring(0, Cac.InnerCategoryIds.Length - 1);
                }
            }
            , () =>
            {
                List<ProductDetailsPicture> picturelist = ProductDetailsPicture.ObjectSet().Where(n => n.CommodityId == commodityId).OrderBy(p => p.Sort).ToList();
                List<string> imgList = new List<string>();
                foreach (var temppic in picturelist)
                {
                    string path = temppic.PicturesPath;
                    imgList.Add(path);
                }
                Cac.Picturelist = imgList;
            });
            //2017-11-22增加
            Cac.CommodityType = Convert.ToInt32(commodity.Type); //商品类型
            Cac.YJCouponActivityId = commodity.YJCouponActivityId;//活动编码
            Cac.YJCouponType = commodity.YJCouponType;//类型编码

            Cac.Name = commodity.Name;
            Cac.CommodityId = commodity.Id;
            Cac.Price = commodity.Price;
            Cac.MarketPrice = commodity.MarketPrice;
            Cac.CommodityDuty = commodity.Duty;
            Cac.CommodityTaxRate = commodity.TaxRate;
            Cac.CommodityInputTax = commodity.InputRax;
            Cac.TaxClassCode = commodity.TaxClassCode;
            Cac.Unit = commodity.Unit;
            Cac.PicturesPath = commodity.PicturesPath;
            Cac.No_Code = commodity.No_Code;
            Cac.Stock = commodity.Stock;
            Cac.State = commodity.State;
            Cac.Description = commodity.Description;
            Cac.CostPrice = commodity.CostPrice;
            Cac.BarCode = commodity.Barcode;
            Cac.IsAssurance = commodity.IsAssurance ?? false;
            Cac.IsReturns = commodity.IsReturns ?? false;
            Cac.Isnsupport = commodity.Isnsupport ?? false;
            Cac.ServiceSettingId = commodity.ServiceSettingId;
            Cac.TechSpecs = commodity.TechSpecs;
            Cac.SaleService = commodity.SaleService;
            Cac.JDCode = commodity.JDCode;
            Cac.ErQiCode = commodity.ErQiCode;
            Cac.SizeNames = "";
            Cac.ColorNames = "";
            //2018年1月8日添加
            Cac.TotalCollection = commodity.TotalCollection;
            Cac.TotalReview = commodity.TotalReview;
            Cac.GroundTime = commodity.GroundTime;
            Cac.ComAttribute = commodity.ComAttribute;
            Cac.CategoryName = commodity.CategoryName;
            Cac.SortValue = commodity.SortValue;
            Cac.FreightTemplateId = commodity.FreightTemplateId;
            Cac.SharePercent = commodity.SharePercent;
            Cac.ScorePercent = commodity.ScorePercent;
            Cac.SpreadPercent = commodity.SpreadPercent;
            Cac.SubTime = commodity.SubTime;
            Cac.SubId = commodity.SubId;
            Cac.ModifiedId = commodity.ModifieId ?? Guid.Empty;

            Cac.FreightId = commodity.FreightTemplateId.HasValue ? commodity.FreightTemplateId.Value.ToString() : "";
            List<string> attrs = new List<string>();
            //zgx-modify
            var cStock = CommodityStock.ObjectSet().Where(n => n.CommodityId == commodityId).ToList();
            if (cStock != null && cStock.Count > 0) //组合属性
            {
                Cac.ComAttributes = new List<Deploy.CustomDTO.CommodityStockDTO>();
                foreach (var item in cStock)
                {
                    var t = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                    Cac.ComAttributes.Add(new Deploy.CustomDTO.CommodityStockDTO()
                    {
                        ComAttribute = t,
                        Id = item.Id,
                        Price = item.Price,
                        MarketPrice = item.MarketPrice,
                        Duty = item.Duty,
                        Stock = item.Stock,
                        CostPrice = item.CostPrice,
                        BarCode = item.Barcode,
                        JDCode = item.JDCode,
                        ErQiCode = item.ErQiCode,
                        Code = item.No_Code,
                        ThumImg = item.ThumImg,
                        CarouselImgs = item.CarouselImgs
                    });
                }
            }
            else if (!string.IsNullOrEmpty(commodity.ComAttribute))
            {
                List<ComAttributeDTO> comAttr = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(commodity.ComAttribute);
                if (comAttr != null && comAttr.Count > 0)
                {
                    List<string> colors = new List<string>();
                    List<string> sizes = new List<string>();
                    foreach (ComAttributeDTO dto in comAttr)
                    {
                        if (!string.IsNullOrEmpty(dto.SecondAttribute))
                        {
                            if (string.Equals(dto.Attribute, "颜色"))
                            {
                                colors.Add(dto.SecondAttribute);
                            }
                            else if (string.Equals(dto.Attribute, "尺寸"))
                            {
                                sizes.Add(dto.SecondAttribute);
                            }
                            attrs.Add(dto.SecondAttribute);
                            Cac.AttrName = dto.Attribute;
                        }
                    }
                    Cac.SizeNames = string.Join(",", sizes);
                    Cac.ColorNames = string.Join(",", colors);
                    Cac.AttrValueNames = string.Join(",", attrs);
                    if (sizes.Count > 0 && colors.Count > 0) //老数据
                    {
                        Cac.ComAttributes = new List<Deploy.CustomDTO.CommodityStockDTO>();
                        foreach (var color in colors)
                        {
                            var s = new ComAttributeDTO() { Attribute = "颜色", SecondAttribute = color };
                            foreach (var size in sizes)
                            {
                                List<ComAttributeDTO> attr = new List<ComAttributeDTO>();
                                attr.Add(new ComAttributeDTO() { Attribute = "尺寸", SecondAttribute = size });
                                attr.Add(s);
                                Cac.ComAttributes.Add(new Deploy.CustomDTO.CommodityStockDTO()
                                {
                                    ComAttribute = attr,
                                    Id = Guid.Empty,
                                    Price = -1,
                                    Stock = -1
                                });
                            }
                        }
                    }
                }
            }

            Cac.AppId = commodity.AppId;
            //获取根据属性名称获取属性ID
            if (Cac.ComAttributes != null && Cac.ComAttributes.Count > 0)
            {
                List<Jinher.AMP.BTP.BE.Attribute> attr1 =
                    new Jinher.AMP.BTP.BE.Attribute().GetAttributeByName(
                        Cac.ComAttributes[0].ComAttribute.Select(r => r.Attribute.ToLower()).ToList(), Cac.AppId);
                List<Jinher.AMP.BTP.BE.SecondAttribute> attrv =
                    new Jinher.AMP.BTP.BE.Attribute().GetAttributeValueById(attr1.Select(r => r.Id).ToList(), Cac.AppId);
                foreach (var item in Cac.ComAttributes)
                {
                    item.ComAttributeIds = new List<ComAttributeHaveIdDTO>();
                    item.ComAttributeIdOrders = new List<ComAttributeOrder>();
                    //var f = CreateAttrDTO(attr1, attrv, item.ComAttribute[0]);
                    //var t = CreateAttrDTO(attr1, attrv, item.ComAttribute[1]);
                    //if (f != null && t != null)
                    //{
                    //    item.ComAttributeIds.Add(f);
                    //    item.ComAttributeIds.Add(t);
                    //}
                    if (item.ComAttribute.Count > 0)
                    {
                        var f = CreateAttrDTO(attr1, attrv, item.ComAttribute[0]);
                        item.ComAttributeIdOrders.Add(f);
                        item.ComAttributeIds.Add(new ComAttributeHaveIdDTO()
                        {
                            AttributeId = f.AttributeId,
                            Attribute = f.Attribute,
                            SecondAttributeId = f.SecondAttributeId,
                            SecondAttribute = f.SecondAttribute
                        });
                    }
                    if (item.ComAttribute.Count > 1)
                    {
                        var t = CreateAttrDTO(attr1, attrv, item.ComAttribute[1]);
                        item.ComAttributeIdOrders.Add(t);
                        item.ComAttributeIds.Add(new ComAttributeHaveIdDTO()
                        {
                            AttributeId = t.AttributeId,
                            Attribute = t.Attribute,
                            SecondAttributeId = t.SecondAttributeId,
                            SecondAttribute = t.SecondAttribute
                        });
                    }
                }
            }
            else if (!string.IsNullOrEmpty(commodity.ComAttribute) && attrs.Count > 0)
            {
                List<string> aName = new List<string>();
                aName.Add(Cac.AttrName);
                List<Jinher.AMP.BTP.BE.Attribute> attr1 = new Jinher.AMP.BTP.BE.Attribute().GetAttributeByName(aName,
                    Cac.AppId);
                List<Jinher.AMP.BTP.BE.SecondAttribute> attrv;
                if (attr1 != null)
                {
                    attrv = new Jinher.AMP.BTP.BE.Attribute().GetAttributeValueById(attr1.Select(r => r.Id).ToList(),
                        Cac.AppId);
                }
                else
                {
                    attrv = new List<SecondAttribute>();
                }

                List<string> attrids = new List<string>();
                foreach (var item in attrs)
                {
                    var a2 = attrv.Find(r => r.Name.ToLower() == item.ToLower());
                    if (a2 != null)
                    {
                        attrids.Add(string.Format("{0}", a2.Id));
                    }
                    else
                    {
                        attrids.Add(string.Format("{0}", Guid.Empty));
                    }
                }
                Cac.AttrValueIds = string.Join(",", attrids);
                if (attr1 != null && attr1.Any())
                {
                    Cac.AttrId = attr1[0].Id.ToString();
                }
                else
                {
                    Cac.AttrId = string.Format("{0}", Guid.Empty);
                }
            }
            //是否支持自提。
            Cac.IsEnableSelfTake = commodity.IsEnableSelfTake;
            Cac.PricingMethod = commodity.PricingMethod;
            Cac.Weight = commodity.Weight.HasValue ? commodity.Weight.Value : 0;
            Cac.SaleAreas = commodity.SaleAreas;
            Cac.VideoName = commodity.VideoName;
            Cac.VideoUrl = commodity.MobileVideoPath;
            Cac.VideoclientUrl = commodity.HtmlVideoPath;
            Cac.VideoPicUrl = commodity.VideoPic;
            Cac.ScoreScale = commodity.ScoreScale;

            #region 包装规格设置
            string str = null;
            var commoditySpecificationslist = CommoditySpecifications.ObjectSet().Where(p => p.CommodityId == commodityId).ToList();
            if (commoditySpecificationslist.Count() > 0)
            {
                foreach (var item in commoditySpecificationslist)
                {
                    str += "1*" + item.Attribute + ",";
                }
                str = str.Remove(str.Length - 1, 1);
            }
            Cac.Specifications = str;
            #endregion

            return Cac;
        }

        private ComAttributeOrder CreateAttrDTO(List<Jinher.AMP.BTP.BE.Attribute> attr1,
                                                List<Jinher.AMP.BTP.BE.SecondAttribute> attrv, ComAttributeDTO attr)
        {
            LogHelper.Info("" + attr1.Count + "," + attrv.Count + "，商品属性分类值：" + attr.Attribute.ToLower() + " 属性名称值 " + attr.SecondAttribute.ToLower());
            var a1 = attr1.Find(r => r.Name.ToLower() == attr.Attribute.ToLower());
            var a2 = new SecondAttribute();
            if (a1 != null)
            {
                a2 = attrv.Find(r => r.Name.ToLower() == attr.SecondAttribute.ToLower() && r.AttributeId == a1.Id);
            }


            //if (a1 == null || a2 == null)
            //{
            //    return null;
            //}
            //return new ComAttributeHaveIdDTO() { AttributeId = a1.Id, Attribute = a1.Name, SecondAttributeId = a2.Id, SecondAttribute = a2.Name };
            // return new ComAttributeHaveIdDTO() { AttributeId =(a1!=null?a1.Id:Guid.Empty), Attribute = attr.Attribute, SecondAttributeId =(a2!=null?a2.Id:Guid.Empty), SecondAttribute = attr.SecondAttribute };

            return new ComAttributeOrder()
            {
                OrderTime = (a2 != null ? a2.SubTime : default(DateTime)),
                AttributeId = (a1 != null ? a1.Id : Guid.Empty),
                Attribute = attr.Attribute,
                SecondAttributeId = (a2 != null ? a2.Id : Guid.Empty),
                SecondAttribute = attr.SecondAttribute,
            };
        }

        #endregion

        #region 删除商品

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id">商品ID</param>
        public ResultDTO DeleteGetCommodityExt(System.Guid id)
        {
            try
            {
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity == null)
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                commodity.ModifiedOn = DateTime.Now;
                commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                commodity.EntityState = System.Data.EntityState.Modified;
                commodity.IsDel = true;

                //删除热门商品表
                var hotCommodity = HotCommodity.ObjectSet().FirstOrDefault(n => n.CommodityId == id);
                if (hotCommodity != null)
                {
                    contextSession.Delete(hotCommodity);
                }

                //删除今日促销表信息
                var todayPromotion = TodayPromotion.ObjectSet().Where(n => n.CommodityId == id).ToList();
                foreach (TodayPromotion pro in todayPromotion)
                {
                    contextSession.Delete(pro);
                    needRefreshCacheTodayPromotions.Add(pro);
                }

                //删除促销缓存
                Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", id.ToString(), "BTPCache");

                //删除促销商品表
                var promotionItems = PromotionItems.ObjectSet().Where(n => n.CommodityId == id).ToList();
                foreach (PromotionItems items in promotionItems)
                {
                    contextSession.Delete(items);
                }
                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                contextSession.SaveObject(journal);
                contextSession.SaveChanges();

                commodity.RefreshCache(EntityState.Deleted);

                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
                //将删除商品插入Commoditychange表
                //更新商品变动表数据
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
                ////后台线程更新商品属性缓存
                //System.Threading.ThreadPool.QueueUserWorkItem(a =>
                //{
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CommodityAttrInfo", commodity.Id.ToString(), "BTPCache");
                //});
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除商品服务异常。id：{0}", id), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 删除多个商品

        /// <summary>
        /// 删除多个商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public ResultDTO DeleteCommoditysExt(System.Collections.Generic.List<System.Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                ids.RemoveAll(c => c == Guid.Empty);
                if (!ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodityList =
                    Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 0).ToList();
                foreach (var commodity in commodityList)
                {
                    commodity.IsDel = true;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.EntityState = System.Data.EntityState.Modified;
                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    needRefreshCacheCommoditys.Add(commodity);
                }

                //删除热门商品表
                var hotCommodity = HotCommodity.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (HotCommodity hc in hotCommodity)
                {
                    contextSession.Delete(hc);
                }

                //删除今日促销表信息
                var todayPromotion = TodayPromotion.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (TodayPromotion pro in todayPromotion)
                {
                    contextSession.Delete(pro);
                    needRefreshCacheTodayPromotions.Add(pro);
                }

                //删除促销缓存
                for (int i = 0; i < ids.Count; i++)
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", ids[i].ToString(), "BTPCache");
                }

                //删除促销商品表
                var promotionItems = PromotionItems.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (PromotionItems items in promotionItems)
                {
                    contextSession.Delete(items);
                }

                //删除购物车商品
                var shoplist = ShoppingCartItems.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (ShoppingCartItems shop in shoplist)
                {
                    shop.EntityState = System.Data.EntityState.Deleted;
                    contextSession.Delete(shop);
                }

                contextSession.SaveChange();
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
                //删除没有商品的促销
                string ss = "update  promotion set IsDel=1 where id not in " +
                            "(select distinct promotionid  " +
                            "from promotionitems )";
                Promotion.ObjectSet().Context.ExecuteStoreCommand(ss);
                //将删除商品插入Commoditychange表
                //更新商品变动表数据   
                SaveCommodityChange(ids);
                ////后台线程更新商品属性缓存
                //System.Threading.ThreadPool.QueueUserWorkItem(a =>
                //{
                //    ids.ForEach(id =>
                //    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_CommodityAttrInfo", id.ToString(), "BTPCache")
                //    );
                //});
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("删除多个商品服务异常。ids：{0}", ids), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion


        #region 下架商品

        /// <summary>
        /// 下架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public ResultDTO OffShelvesExt(System.Collections.Generic.List<System.Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                ids.RemoveAll(c => c == Guid.Empty);
                if (!ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };

                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                List<TodayPromotion> needRefreshCacheTodayPromotions = new List<TodayPromotion>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commoditys = Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 0).ToList();
                if (!commoditys.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                foreach (var commodity in commoditys)
                {
                    commodity.State = 1;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.EntityState = System.Data.EntityState.Modified;
                    needRefreshCacheCommoditys.Add(commodity);

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    //库存表中商品上架下架状态更改
                    var commoditystock = CommodityStock.ObjectSet().Where(p => p.CommodityId == commodity.Id);
                    foreach (var stock in commoditystock)
                    {
                        stock.State = 1;
                        stock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(stock);
                    }
                }

                //删除热门商品表
                var hotCommodity = HotCommodity.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (HotCommodity hc in hotCommodity)
                {
                    contextSession.Delete(hc);
                }

                //删除今日促销表信息
                var todayPromotion = TodayPromotion.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (TodayPromotion pro in todayPromotion)
                {
                    contextSession.Delete(pro);
                    needRefreshCacheTodayPromotions.Add(pro);
                }

                //删除促销缓存
                for (int i = 0; i < ids.Count; i++)
                {
                    Jinher.JAP.Cache.GlobalCacheWrapper.Remove("G_DiscountInfo", ids[i].ToString(), "BTPCache");
                }

                //删除促销商品表
                var promotionItems = PromotionItems.ObjectSet().Where(n => ids.Contains(n.CommodityId)).ToList();
                foreach (PromotionItems items in promotionItems)
                {
                    contextSession.Delete(items);
                }

                contextSession.SaveChange();
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }
                if (needRefreshCacheTodayPromotions.Any())
                {
                    needRefreshCacheTodayPromotions.ForEach(c => c.RefreshCache(EntityState.Deleted));
                }
                //将下架商品插入Commoditychange表
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("下架商品服务异常。ids：{0}", ids), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 上架商品

        /// <summary>
        /// 上架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public ResultDTO ShelvesExt(System.Collections.Generic.List<System.Guid> ids)
        {
            if (ids == null || !ids.Any())
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            ids.RemoveAll(c => c == Guid.Empty);
            if (!ids.Any())
                return new ResultDTO { ResultCode = 0, Message = "Success" };

            Guid appId = Guid.Empty;
            Guid userId = Guid.Empty;
            string photoUrl = string.Empty;
            try
            {
                List<Commodity> needRefreshCacheCommoditys = new List<Commodity>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commoditys = Commodity.ObjectSet().Where(n => ids.Contains(n.Id) && n.CommodityType == 0).ToList();
                if (!commoditys.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                foreach (var commodity in commoditys)
                {
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.EntityState = System.Data.EntityState.Modified;
                    commodity.State = 0;
                    commodity.GroundTime = DateTime.Now;
                    needRefreshCacheCommoditys.Add(commodity);

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    //库存表中商品上架下架状态更改
                    var commoditystock = CommodityStock.ObjectSet().Where(p => p.CommodityId == commodity.Id);
                    foreach (var stock in commoditystock)
                    {
                        stock.State = 0;
                        stock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(stock);
                    }
                }
                if (commoditys.Count > 0)
                {
                    appId = commoditys[0].AppId;
                    userId = commoditys[0].SubId;
                    photoUrl = commoditys[0].PicturesPath;
                }
                contextSession.SaveChange();
                if (needRefreshCacheCommoditys.Any())
                {
                    needRefreshCacheCommoditys.ForEach(c => c.RefreshCache(EntityState.Modified));
                }

                #region 上架发布广场

                IUS.Deploy.CustomDTO.PicFromUrlCDTO addDataDTO = new IUS.Deploy.CustomDTO.PicFromUrlCDTO();
                addDataDTO.AppId = appId;
                addDataDTO.Content = APPSV.GetAppName(appId) + "有新品上架了哦，快来看看吧~";
                addDataDTO.PhotoUrl = photoUrl;
                addDataDTO.ShareUrl = string.Format("{0}Mobile/CommodityList?AppId={1}&sortType=New&type=tuwen",
                                                    CustomConfig.BtpDomain, appId);
                addDataDTO.Source = Jinher.AMP.IUS.Deploy.Enum.SourceEnum.EBusinessInfo;
                addDataDTO.Title = addDataDTO.Content;
                addDataDTO.UserId = userId;
                addDataDTO.UserName = (ContextDTO != null && ContextDTO.LoginUserName != null)
                                          ? ContextDTO.LoginUserName
                                          : "btp";
                var result = Jinher.AMP.BTP.TPS.IUSSV.Instance.AddPicFromUrl(addDataDTO);

                #endregion
                //将上架商品插入Commoditychange表
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("上架商品服务异常。ids：{0}", JsonHelper.JsonSerializer(ids)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 修改价格

        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="price">价格</param>
        public ResultDTO UpdatePriceExt(System.Guid id, decimal price)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                bool isPrice = false;

                if (commodity != null)
                {
                    if (commodity.Price > price)
                    {
                        isPrice = true;
                    }
                    commodity.Price = price;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.EntityState = EntityState.Modified;

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    //修改库存表中商品价格
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.Price = price;
                        ComStock.ModifiedOn = DateTime.Now;
                        ComStock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(ComStock);
                    }
                    if (commodity.State == 0)
                    {
                        HotCommodity hotCommodity = HotCommodity.ObjectSet().FirstOrDefault(c => c.CommodityId == id);
                        if (hotCommodity != null)
                        {
                            hotCommodity.Price = price;
                            hotCommodity.ModifiedOn = DateTime.Now;
                            hotCommodity.EntityState = EntityState.Modified;
                        }
                    }
                }
                int result = contextSession.SaveChange();
                if (result > 0 && commodity != null)
                    commodity.RefreshCache(EntityState.Modified);
                //将修改价格商品插入Commoditychange表
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
                LogHelper.Info("商品是否修改成功" + result.ToString() + "   " + isPrice.ToString() + "   " +
                               commodity.State.ToString());
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改价格服务异常。id：{0}，price：{1}", id, price), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }

        #endregion

        #region 修改市场价

        /// <summary>
        /// 修改市场价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="marketPrice">市场价</param>
        public ResultDTO UpdateMarketPriceExt(System.Guid id, decimal? marketPrice)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.MarketPrice = marketPrice;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.EntityState = EntityState.Modified;
                    //修改库存表中商品市场价
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.MarketPrice = marketPrice;
                        ComStock.ModifiedOn = DateTime.Now;
                        ComStock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(ComStock);
                    }
                }

                int result = contextSession.SaveChange();
                if (result > 0 && commodity != null)
                    commodity.RefreshCache(EntityState.Modified);
                //将修改市场价的商品插入Commoditychange表
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改市场价服务异常。id：{0}，marketPrice：{1}", id, marketPrice), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }

        #endregion

        #region 修改进货价

        /// <summary>
        /// 修改进货价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="costPrice">进货价</param>
        public ResultDTO UpdateCostPriceExt(System.Guid id, decimal? costPrice)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.CostPrice = costPrice;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.EntityState = EntityState.Modified;
                    //修改库存表中商品市场价
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.CostPrice = costPrice;
                        ComStock.ModifiedOn = DateTime.Now;
                        ComStock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(ComStock);
                    }
                }

                int result = contextSession.SaveChange();
                if (result > 0 && commodity != null)
                    commodity.RefreshCache(EntityState.Modified);
                //将修改市场价的商品插入Commoditychange表
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改进货价服务异常。id：{0}，costPrice：{1}", id, costPrice), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };

        }

        #endregion

        /// <summary>
        /// 商品价格发生变化时，更新定单中价格相关。
        /// </summary>
        private void UpdateOrderPrice(Guid commodityId, Guid? appid, decimal price, ContextSession contextSession)
        {

            //商品状态：上架=0，未上架=1

            //上架商品，改价格时， 才更新。

            //1，更改未支付商品定单里的商品价格。
            //订单状态：未支付=0，未发货=1，已发货=2，确认收货=3，删除=4（必填）

            Guid appId = new Guid();
            Guid.TryParse(appid.ToString(), out appId);

            //查找当前商品所关联的所有定单。
            var cos = (from co in CommodityOrder.ObjectSet()
                       join oi in OrderItem.ObjectSet() on co.Id equals oi.CommodityOrderId
                       where co.AppId == appid.Value && co.State == 0 && oi.CommodityId == commodityId
                       select co).ToList();

            //查出这些定单下所有定单详情。
            var coIds = cos.Select(co => co.Id).Distinct();
            var ois = (from oi in OrderItem.ObjectSet() where coIds.Contains(oi.CommodityOrderId) select oi).ToList();

            //当前商品关联的定单详情，更新这些定单详情的当前价格。
            var orderItems = (from oi in ois where oi.CommodityId == commodityId select oi).ToList();
            foreach (var oi in orderItems)
            {
                oi.CurrentPrice = price;
            }
            contextSession.SaveObject(orderItems);

            //查定单下的所有商品的促销信息，重算总价和实付款。
            var cids = (from io in ois select io.CommodityId).Distinct();
            var dt = DateTime.Now;
            var proms = (from pis in PromotionItems.ObjectSet()
                         join p in Promotion.ObjectSet() on pis.PromotionId equals p.Id
                         where p.StartTime <= dt && dt <= p.EndTime && p.IsEnable && !p.IsDel && p.PromotionType == 0
                               && cids.Contains(pis.CommodityId)
                         select new
                         {
                             CommodityId = pis.CommodityId,
                             Intensity = p.Intensity
                         }).Distinct();


            //更新商品定单的总价
            foreach (var co in cos)
            {
                co.Price = 0;
                var oiInOrder = from oi in ois where oi.CommodityOrderId == co.Id select oi;
                foreach (var oi in oiInOrder)
                {
                    //没有打折信息相当于打10折。
                    decimal intensity =
                        (from prom in proms where prom.CommodityId == oi.CommodityId select prom.Intensity)
                            .FirstOrDefault();
                    intensity = intensity == 0 ? 10 : intensity;
                    co.Price += decimal.Round(oi.CurrentPrice * oi.Number * intensity * (decimal)0.1, 2,
                                              MidpointRounding.AwayFromZero);
                }
                //如果未手动修改过实付款，更改实付款。
                if (!co.IsModifiedPrice)
                {
                    co.RealPrice = co.Price;
                }
            }
            contextSession.SaveObject(cos);

        }

        #region 修改库存

        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="stock">库存</param>
        public ResultDTO UpdateStockExt(System.Guid id, int stock)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                List<Guid> NoticeComIds = new List<Guid>();
                if (commodity != null)
                {

                    commodity.EntityState = System.Data.EntityState.Modified;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    //到货提醒商品Id集合                    
                    if (commodity.Stock == 0 && stock > 0)
                    {
                        NoticeComIds.Add(id);
                    }
                    commodity.Stock = stock;
                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);

                    //修改库存表中商品市场价
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.Stock = stock;
                        ComStock.ModifiedOn = DateTime.Now;
                        ComStock.EntityState = EntityState.Modified;
                        contextSession.SaveObject(ComStock);
                    }
                    if (commodity.State == 0)
                    {
                        HotCommodity hotCommodity = HotCommodity.ObjectSet().FirstOrDefault(c => c.CommodityId == id);
                        if (hotCommodity != null)
                        {
                            hotCommodity.Stock = stock;
                            hotCommodity.ModifiedOn = DateTime.Now;
                            hotCommodity.EntityState = EntityState.Modified;
                        }
                    }
                }

                contextSession.SaveChange();
                if (commodity != null)
                {
                    commodity.RefreshCache(EntityState.Modified);
                }
                //调用到货提醒接口
                if (NoticeComIds.Any())
                {
                    ZPHSV.SendStockNotifications(NoticeComIds);
                }
                //将修改价格商品插入Commoditychange表
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改库存服务异常。id：{0}，stock：{1}", id, stock), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 修改销量

        /// <summary>
        /// 修改销量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="salesvolume"></param>
        /// <returns></returns>
        public ResultDTO UpdateSalesvolumeExt(System.Guid id, int salesvolume)
        {
            try
            {

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.Stock = commodity.Stock - salesvolume;
                    commodity.Salesvolume = commodity.Salesvolume + salesvolume;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.EntityState = System.Data.EntityState.Modified;

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);

                    HotCommodity hotCommodity = HotCommodity.ObjectSet().FirstOrDefault(c => c.CommodityId == id);
                    if (hotCommodity != null)
                    {
                        hotCommodity.Stock = commodity.Stock;
                        hotCommodity.Salesvolume = commodity.Salesvolume;
                        hotCommodity.ModifiedOn = DateTime.Now;
                        hotCommodity.EntityState = EntityState.Modified;
                    }
                }
                contextSession.SaveChange();
                if (commodity != null)
                    commodity.RefreshCache(EntityState.Modified);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改库存服务异常。id：{0}，salesvolume：{1}", id, salesvolume), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 修改名称

        /// <summary>
        /// 修改商品名称
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="name">商品名称</param>
        public ResultDTO UpdateNameExt(System.Guid id, string name)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.EntityState = System.Data.EntityState.Modified;
                    commodity.ModifiedOn = DateTime.Now;
                    commodity.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    commodity.Name = name;

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);


                    HotCommodity hotCommodity = HotCommodity.ObjectSet().FirstOrDefault(c => c.CommodityId == id);
                    if (hotCommodity != null)
                    {
                        hotCommodity.Name = name;
                        hotCommodity.ModifiedOn = DateTime.Now;
                        hotCommodity.EntityState = EntityState.Modified;
                    }

                }
                contextSession.SaveChange();
                if (commodity != null)
                {
                    commodity.RefreshCache(EntityState.Modified);
                }
                //将修改名称的商品插入Commoditychange表
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改名称服务异常。id：{0}，name：{1}", id, name), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion


        #region 获取商品类别

        /// <summary>
        /// 获取商品类别   
        /// </summary>
        /// <param name="commodityId">商品ID</param>
        [Obsolete("已废弃", false)]
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryBycommodityIdExt(
            System.Guid commodityId)
        {
            var comcate = CommodityCategory.ObjectSet().Where(n => n.CommodityId == commodityId);
            List<CategoryDTO> categoryDTO = new List<CategoryDTO>();

            List<Guid> idList = new List<Guid>();
            foreach (var item in comcate)
            {
                idList.Add(item.CategoryId);
            }

            List<Category> list = Category.ObjectSet().Where(n => idList.Distinct().Contains(n.Id)).ToList();
            foreach (Category cate in list)
            {
                categoryDTO.Add(cate.ToEntityData());
            }

            return categoryDTO;
        }

        #endregion

        #region 编辑商品类别

        /// <summary>
        ///编辑商品类别
        /// </summary>
        /// <param name="uCategoryVM">编辑商品分类VM</param>
        public ResultDTO UpdateCategoryBycommodityIdExt(UCategoryVM uCategoryVM)
        {
            if (uCategoryVM == null || uCategoryVM.CommodityId == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Commodity commodity = Commodity.ObjectSet().FirstOrDefault(c => c.Id == uCategoryVM.CommodityId);
                if (commodity == null)
                    return new ResultDTO { ResultCode = 1, Message = "商品不存在" };

                #region 删除原商品分类

                var orgCatList =
                    CommodityCategory.ObjectSet()
                                     .Where(c => c.CommodityId == commodity.Id && commodity.AppId == c.AppId)
                                     .ToList();
                foreach (var commodityCategory in orgCatList)
                {
                    contextSession.Delete(commodityCategory);
                }

                #endregion

                #region 添加商品分类

                if (!string.IsNullOrEmpty(uCategoryVM.ComCateIds) && uCategoryVM.ComCateIds.Trim() != ",")
                {
                    string[] arr = uCategoryVM.ComCateIds.Split(',');
                    List<Guid> catelist = new List<Guid>();
                    foreach (var s in arr)
                    {
                        Guid cat;
                        if (Guid.TryParse(s, out cat))
                            catelist.Add(cat);
                    }
                    for (int i = 0; i < catelist.Count; i++)
                    {
                        CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                        comcate.CategoryId = catelist[i];
                        comcate.CommodityId = uCategoryVM.CommodityId;
                        comcate.SubTime = DateTime.Now;
                        comcate.Name = "商品分类";
                        comcate.IsDel = false;
                        comcate.AppId = commodity.AppId;
                        comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(comcate.AppId);

                        contextSession.SaveObject(comcate);
                    }
                }

                #endregion

                contextSession.SaveChange();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑商品类别服务异常。uCategoryVM：{0}", JsonHelper.JsonSerializer(uCategoryVM)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion

        #region 根据分类获得商品

        /// <summary>
        /// 根据分类获得商品
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetCommodityByCategoryIdExt(
            Guid appId, Guid categoryId)
        {
            //List<Guid> comids =
            //    CommodityCategory.ObjectSet().Where(n => n.CategoryId == categoryId).Select(n => n.CommodityId).ToList();
            //List<Guid> idList = new List<Guid>();
            //foreach (Guid id in comids)
            //{
            //    idList.Add(id);
            //}
            //var list = Commodity.ObjectSet().Where(n => idList.Distinct().Contains(n.Id) && n.CommodityType == 0);

            var commodityCategories = CommodityCategory.ObjectSet().Where(n => n.CategoryId == categoryId);
            var commodities = Commodity.ObjectSet().Where(n => n.AppId == appId && n.CommodityType == 0 && commodityCategories.Any(cc => cc.CommodityId == n.Id));
            var categoryName = Category.FindByID(categoryId).Name;

            var result = from c in commodities
                         select new CommodityDTO
                         {
                             AppId = c.AppId,
                             Code = c.Code,
                             CategoryName = categoryName,
                             ComAttribute = c.ComAttribute,
                             Description = c.Description,
                             GroundTime = c.GroundTime,
                             Id = c.Id,
                             IsDel = c.IsDel,
                             Name = c.Name,
                             No_Code = c.No_Code,
                             No_Number = c.No_Number,
                             PicturesPath = c.PicturesPath,
                             Price = c.Price,
                             MarketPrice = c.MarketPrice,
                             Duty = c.Duty,
                             State = c.State,
                             Stock = c.Stock,
                             TotalReview = c.TotalReview,
                             TotalCollection = c.TotalCollection,
                             Salesvolume = c.Salesvolume,
                             IsEnableSelfTake = c.IsEnableSelfTake
                         };
            return result.ToList();
        }

        #endregion

        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="commoditySearch"></param>
        /// <returns></returns>
        public List<CommodityPromVM> GetCommodityVMExt(CommoditySearchDTO search, Guid promotionId)
        {
            var query =
                Commodity.ObjectSet()
                         .Where(
                             n =>
                             n.IsDel.Equals(false) && n.AppId.Equals(search.appId) && n.State == 0 &&
                             n.CommodityType == 0);

            #region 条件查询

            if (!string.IsNullOrEmpty(search.commodityName))
            {
                query = query.Where(n => n.Name.Contains(search.commodityName));
            }
            if (!string.IsNullOrEmpty(search.commodityCode))
            {
                query = query.Where(n => n.No_Code.Contains(search.commodityCode));
            }
            if (!string.IsNullOrEmpty(search.sSalesvolume))
            {
                int s = int.Parse(search.sSalesvolume);
                query = query.Where(n => n.Salesvolume >= s);
            }
            if (!string.IsNullOrEmpty(search.eSalesvolume))
            {
                int e = int.Parse(search.eSalesvolume);
                query = query.Where(n => n.Salesvolume <= e);
            }
            if (!string.IsNullOrEmpty(search.sPrice))
            {
                decimal s = 0;
                if (!decimal.TryParse(search.sPrice, out s)) //长度越界
                {
                    return new List<CommodityPromVM>();
                }
                query = query.Where(n => n.Price >= s);
            }
            if (!string.IsNullOrEmpty(search.ePrice))
            {
                decimal e = 0;
                if (!decimal.TryParse(search.ePrice, out e)) //长度越界
                {
                    return new List<CommodityPromVM>();
                }
                query = query.Where(n => n.Price <= e);
            }
            if (!string.IsNullOrEmpty(search.commodityCategory))
            {
                string[] commodityCategoryID = search.commodityCategory.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string commodityCategoryid in commodityCategoryID)
                {
                    if (!string.IsNullOrEmpty(commodityCategoryid))
                    {
                        idlist.Add(new Guid(commodityCategoryid));
                    }
                }
                query = (from n in query
                         join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                         where idlist.Distinct().Contains(m.CategoryId)
                         select n).Distinct();
            }
            if (search.commodityIdList != null && search.commodityIdList.Count > 0)
            {
                query = query.Where(n => search.commodityIdList.Contains(n.Id)).Distinct();
            }

            #endregion

            search.rowCount = query.Count();
            query =
                query.Distinct()
                     .OrderBy(n => n.Salesvolume)
                     .ThenByDescending(n => n.SubTime)
                     .Skip((search.pageIndex - 1) * search.pageSize)
                     .Take(search.pageSize);
            List<CommodityPromVM> result = new List<CommodityPromVM>();
            if (promotionId != null && promotionId.ToString() != "" &&
                promotionId.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                var prolist = from a in Promotion.ObjectSet()
                              join b in PromotionItems.ObjectSet()
                                  on a.Id equals b.PromotionId
                              where a.Id == promotionId && a.IsEnable && !a.IsDel && a.PromotionType == 0
                              select b;

                result = (from n in query
                          join p in prolist
                              on n.Id equals p.CommodityId into copro
                          from g in copro.DefaultIfEmpty()
                          select new CommodityPromVM
                          {
                              Name = n.Name,
                              Id = n.Id,
                              CollectNum = n.TotalCollection,
                              ReviewNum = n.TotalReview,
                              Pic = n.PicturesPath,
                              Price = n.Price,
                              MarketPrice = n.MarketPrice,
                              State = n.State,
                              Stock = n.Stock,
                              Subtime = n.SubTime,
                              Total = n.Salesvolume,
                              Code = n.No_Code,
                              DiscountPrice = g.DiscountPrice,
                              LimitBuyEach = g.LimitBuyEach,
                              LimitBuyTotal = g.LimitBuyTotal,
                              IsPro = (g.Id == null ? 0 : 1),
                              IsEnableSelfTake = n.IsEnableSelfTake
                          }).ToList();
            }
            else
            {
                result = (from n in query
                          select new CommodityPromVM
                          {
                              Name = n.Name,
                              Id = n.Id,
                              CollectNum = n.TotalCollection,
                              ReviewNum = n.TotalReview,
                              Pic = n.PicturesPath,
                              Price = n.Price,
                              MarketPrice = n.MarketPrice,
                              State = n.State,
                              Stock = n.Stock,
                              Subtime = n.SubTime,
                              Total = n.Salesvolume,
                              Code = n.No_Code,
                              DiscountPrice = -1,
                              LimitBuyEach = -1,
                              LimitBuyTotal = -1,
                              IsEnableSelfTake = n.IsEnableSelfTake
                          }).ToList();
            }

            var list = result;

            //获取商品id数组
            List<Guid> cmdyIdList = new List<Guid>();
            foreach (var item in list)
            {
                cmdyIdList.Add(item.Id);
            }

            //获取类目列表
            var categoryList = (from a in Jinher.AMP.BTP.BE.Category.ObjectSet()
                                join b in Jinher.AMP.BTP.BE.CommodityCategory.ObjectSet()
                                    on a.Id equals b.CategoryId
                                where cmdyIdList.Contains(b.CommodityId) && b.AppId == search.appId
                                select new
                                {
                                    Category = a,
                                    CommodityId = b.CommodityId
                                }).ToList();
            //将类目添加到对应的商品
            list.ForEach(a =>
            {
                if (a.Categorys == null)
                    a.Categorys = new List<CategoryDTO>();
                categoryList.ForEach(b =>
                {
                    if (b.CommodityId == a.Id)
                    {
                        a.Categorys.Add(b.Category.ToEntityData());
                    }
                });
            });

            return list;
        }

        /// <summary>
        /// 检查编号是否存在
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public bool IsExistsExt(string code, System.Guid appId)
        {
            bool bReturn = false;
            if (!string.IsNullOrWhiteSpace(code))
            {
                var commodity =
               Commodity.ObjectSet()
                        .Where(n => n.No_Code == code && n.AppId == appId && !n.IsDel && n.CommodityType == 0)
                        .FirstOrDefault();
                bReturn = (null == commodity);
            }
            return bReturn;
        }

        public int GetCommodityNumExt(Guid appId, bool isDel, int state)
        {
            return
                Jinher.AMP.BTP.BE.Commodity.ObjectSet()
                      .Count(
                          n =>
                          n.IsDel.Equals(isDel) && n.AppId.Equals(appId) && n.State == state && n.CommodityType == 0);
        }

        public List<string> GetCommodityCodesExt(Guid appid, List<Guid> commodityIds)
        {
            return (from a in Jinher.AMP.BTP.BE.Commodity.ObjectSet()
                    where commodityIds.Contains(a.Id) && a.CommodityType == 0
                    select a.No_Code).ToList();
        }

        public string GetLastCommodityCodeExt(Guid appId)
        {
            return
                Commodity.ObjectSet()
                         .Where(n => n.AppId == appId && n.CommodityType == 0)
                         .OrderByDescending(n => n.SubTime)
                         .Select(n => n.No_Code)
                         .FirstOrDefault();
        }

        public ResultDTO SaveCommoditySortValueExt(List<Guid> comIds)
        {
            List<Commodity> needRefreshCacheList = new List<Commodity>();

            var comSortQuery = (from e in Commodity.ObjectSet()
                                where comIds.Contains(e.Id) && e.CommodityType == 0
                                select e).ToDictionary(x => x.Id, y => y);
            List<Int32> sortValues = (from e in comSortQuery.Values
                                      orderby e.SortValue
                                      select e.SortValue).ToList();
            sortValues.Sort();

            ContextSession session = ContextFactory.CurrentThreadContext;
            int i = 0;
            bool isNeedSave = false;
            foreach (Guid comId in comIds)
            {
                if (comSortQuery.ContainsKey(comId))
                {
                    Commodity com = comSortQuery[comId];
                    if (com.SortValue != sortValues[i])
                    {
                        com.SortValue = sortValues[i];
                        com.ModifiedOn = DateTime.Now;
                        com.EntityState = EntityState.Modified;
                        isNeedSave = true;
                        needRefreshCacheList.Add(com);
                    }
                }
                i++;
            }
            int rows = -1;
            if (isNeedSave)
            {
                try
                {
                    rows = session.SaveChanges();
                }
                catch (Exception ex)
                {
                    Jinher.JAP.Common.Loging.LogHelper.Error("保存商品排序结果错误", ex);
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                }
                finally
                {
                    //刷新缓存
                    if (rows > 0 && needRefreshCacheList.Any())
                    {
                        foreach (var commodity in needRefreshCacheList)
                        {
                            commodity.RefreshCache(EntityState.Modified);
                        }
                    }
                }
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        public ResultDTO SetCommodityFirstExt(Guid comId)
        {
            var comSortQuery = (from e in Commodity.ObjectSet()
                                where comId.Equals(e.Id) && e.CommodityType == 0
                                select e).FirstOrDefault();
            if (comSortQuery == null)
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            var minSortValue = (from e in Commodity.ObjectSet()
                                where e.CommodityType == 0
                                orderby e.SortValue
                                select e.SortValue).Take(1).FirstOrDefault();
            ContextSession session = ContextFactory.CurrentThreadContext;
            comSortQuery.SortValue = minSortValue - 1;
            int rows = -1;
            try
            {
                comSortQuery.ModifiedOn = DateTime.Now;
                comSortQuery.EntityState = EntityState.Modified;
                rows = session.SaveChanges();
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("保存商品置顶排序错误", ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                if (rows > 0)
                {
                    comSortQuery.RefreshCache(EntityState.Modified);
                }
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #region 关联商品列表

        /// <summary>
        /// 关联商品列表
        /// </summary>
        /// <param name="comId">商品ID</param>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<CommodityPromVM> RelationCommodityListExt(Guid comId, CommoditySearchDTO search)
        {
            var query =
                Commodity.ObjectSet()
                         .Where(
                             n =>
                             n.IsDel.Equals(false) && n.AppId.Equals(search.appId) && n.State == 0 &&
                             n.CommodityType == 0);

            #region 条件查询

            if (!string.IsNullOrEmpty(search.commodityName))
            {
                query = query.Where(n => n.Name.Contains(search.commodityName));
            }
            if (!string.IsNullOrEmpty(search.commodityCode))
            {
                query = query.Where(n => n.No_Code.Contains(search.commodityCode));
            }
            if (!string.IsNullOrEmpty(search.commodityCategory))
            {
                string[] commodityCategoryID = search.commodityCategory.Split(',');
                List<Guid> idlist = new List<Guid>();
                foreach (string commodityCategoryid in commodityCategoryID)
                {
                    if (!string.IsNullOrEmpty(commodityCategoryid))
                    {
                        idlist.Add(new Guid(commodityCategoryid));
                    }
                }
                query = (from n in query
                         join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                         where idlist.Distinct().Contains(m.CategoryId)
                         select n).Distinct();
            }
            if (search.commodityIdList != null && search.commodityIdList.Count > 0)
            {
                query = query.Where(n => search.commodityIdList.Contains(n.Id)).Distinct();
            }

            #endregion

            search.rowCount = query.Count();
            query =
                query.Distinct()
                     .OrderBy(n => n.Salesvolume)
                     .ThenByDescending(n => n.SubTime)
                     .Skip((search.pageIndex - 1) * search.pageSize)
                     .Take(search.pageSize);
            List<CommodityPromVM> result = new List<CommodityPromVM>();

            var relalist = from r in RelationCommodity.ObjectSet()
                           where r.CommodityId == comId
                           select r;

            if (relalist.Count() > 0 && comId.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                result = (from n in query
                          join r in relalist
                              on n.Id equals r.RelationCommodityId into comlist
                          from g in comlist.DefaultIfEmpty()
                          select new CommodityPromVM
                          {
                              Name = n.Name,
                              Id = n.Id,
                              CollectNum = n.TotalCollection,
                              ReviewNum = n.TotalReview,
                              Pic = n.PicturesPath,
                              Price = n.Price,
                              MarketPrice = n.MarketPrice,
                              State = n.State,
                              Stock = n.Stock,
                              Subtime = n.SubTime,
                              Total = n.Salesvolume,
                              Code = n.No_Code,
                              IsPro = g.RelationCommodityId == null ? 0 : 1,
                              IsEnableSelfTake = n.IsEnableSelfTake
                          }).ToList();

            }
            else
            {
                result = (from n in query
                          select new CommodityPromVM
                          {
                              Name = n.Name,
                              Id = n.Id,
                              CollectNum = n.TotalCollection,
                              ReviewNum = n.TotalReview,
                              Pic = n.PicturesPath,
                              Price = n.Price,
                              MarketPrice = n.MarketPrice,
                              State = n.State,
                              Stock = n.Stock,
                              Subtime = n.SubTime,
                              Total = n.Salesvolume,
                              Code = n.No_Code,
                              IsPro = 0,
                              IsEnableSelfTake = n.IsEnableSelfTake
                          }).ToList();
            }


            var list = result;

            //获取商品id数组
            List<Guid> cmdyIdList = new List<Guid>();
            foreach (var item in list)
            {
                cmdyIdList.Add(item.Id);
            }

            //获取类目列表
            var categoryList = (from a in Jinher.AMP.BTP.BE.Category.ObjectSet()
                                join b in Jinher.AMP.BTP.BE.CommodityCategory.ObjectSet()
                                    on a.Id equals b.CategoryId
                                where cmdyIdList.Contains(b.CommodityId) && b.AppId == search.appId
                                select new
                                {
                                    Category = a,
                                    CommodityId = b.CommodityId
                                }).ToList();
            //将类目添加到对应的商品
            list.ForEach(a =>
            {
                if (a.Categorys == null)
                    a.Categorys = new List<CategoryDTO>();
                categoryList.ForEach(b =>
                {
                    if (b.CommodityId == a.Id)
                    {
                        a.Categorys.Add(b.Category.ToEntityData());
                    }
                });
            });

            return list;
        }

        #endregion

        #region 设置销售地区

        /// <summary>
        /// 设置销售地区
        /// </summary>
        /// <param name="ids">商品Id列表</param>
        /// <param name="saleAreas">销售地区</param>
        /// <returns>结果</returns>
        public ResultDTO UpdateCommodityListSaleAreasExt(System.Collections.Generic.List<System.Guid> ids,
                                                         string saleAreas)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                if (ids == null || !ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                ids.RemoveAll(c => c == Guid.Empty);
                if (!ids.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };

                var comList = Commodity.ObjectSet().Where(t => ids.Contains(t.Id) && t.CommodityType == 0).ToList();
                if (!comList.Any())
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                foreach (var item in comList)
                {
                    item.SaleAreas = saleAreas;
                    item.ModifiedOn = DateTime.Now;
                    item.ModifieId = this.ContextDTO.LoginUserID;//修改人id
                    item.EntityState = EntityState.Modified;

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(item);
                    contextSession.SaveObject(journal);
                }

                contextSession.SaveChanges();
                comList.ForEach(c => c.RefreshCache(EntityState.Modified));
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("设置销售地区服务异常。ids：{0}，saleAreas：{1}，ex：{2}", ids, saleAreas, ex));
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        #endregion


        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public ResultDTO<CommodityDividendListDTO> GetCommodityByNameExt(GetCommodityByNameParam pdto)
        {
            ResultDTO<CommodityDividendListDTO> result = new ResultDTO<CommodityDividendListDTO>();

            if (pdto == null)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，参数不能为空！";
                return result;
            }
            if (pdto.AppId == Guid.Empty)
            {
                result.ResultCode = 1;
                result.Message = "参数错误，appId不能为空！";
                return result;
            }


            try
            {
                var query = from c in Commodity.ObjectSet()
                            where c.AppId == pdto.AppId && c.IsDel == false && c.State == 0
                            select c;
                if (!string.IsNullOrWhiteSpace(pdto.CommodityName))
                {
                    query = query.Where(c => c.Name.Contains(pdto.CommodityName));
                }
                if (pdto.OnlyShareMoney)
                {
                    query = query.Where(c => c.SharePercent > 0);
                }
                if (pdto.OnlyScoreMoney)
                {
                    query = query.Where(c => c.ScorePercent > 0);
                }
                if (pdto.OnlySpreadMoney)
                {
                    query = query.Where(c => c.SpreadPercent > 0);
                }
                if (!string.IsNullOrWhiteSpace(pdto.CommodityCategory))
                {
                    string[] commodityCategoryID = pdto.CommodityCategory.Split(',');
                    List<Guid> idlist = new List<Guid>();
                    foreach (string commodityCategoryid in commodityCategoryID)
                    {
                        if (!string.IsNullOrEmpty(commodityCategoryid))
                        {
                            idlist.Add(new Guid(commodityCategoryid));
                        }
                    }
                    query = from n in query
                            join m in CommodityCategory.ObjectSet() on n.Id equals m.CommodityId
                            where idlist.Contains(m.CategoryId)
                            select n;
                }
                int count = query.Count();
                if (count == 0)
                {
                    result.Data = new CommodityDividendListDTO();
                    result.Data.CommodityList = new List<CommodityListCDTO>();
                    result.Data.Count = 0;
                    return result;
                }
                query = query.OrderBy(n => n.SortValue).ThenByDescending(n => n.SubTime)
                             .Skip((pdto.PageIndex - 1) * pdto.PageSize).Take(pdto.PageSize);
                var commodityList = query.ToList();
                if (!commodityList.Any())
                {
                    result.Data = new CommodityDividendListDTO();
                    result.Data.CommodityList = new List<CommodityListCDTO>();
                    result.Data.Count = 0;
                    return result;
                }
                List<CommodityListCDTO> crList = commodityList.ConvertAll(ConvertCommodity2DTO);
                var resultTuple = new CommodityDividendListDTO();
                resultTuple.CommodityList = crList;
                resultTuple.Count = count;
                result.Data = resultTuple;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("根据商品名称获取商品列表异常", ex);
                result.ResultCode = -1;
                result.Message = "获取商品列表异常,请稍后重试！";
            }

            return result;
        }

        private CommodityListCDTO ConvertCommodity2DTO(Commodity commodity)
        {
            CommodityListCDTO ccDto = new CommodityListCDTO();
            ccDto.FillWith(commodity);
            ccDto.Pic = commodity.PicturesPath;
            return ccDto;
        }

        /// <summary>
        /// 保存商品分享分成比例。
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveCommoditySharePercentExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            ResultDTO result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO();
            try
            {
                if (dto == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                //部分分享，没有分享商品列表。
                if (dto.IsDividendAll == false && (dto.CShareList == null || !dto.CShareList.Any()))
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，分享商品列表不能为空！";
                    return result;
                }
                ContextSession session = ContextFactory.CurrentThreadContext;

                bool isDividendChanged = false;
                //不是全部商品分成,则“全部商品分成比例”为0.
                if (dto.IsDividendAll == null || dto.IsDividendAll == false)
                {
                    dto.SharePercent = 0;
                }

                //保存这个店铺（应用）的分成范围、分成比例。
                var appExtList = (from ae in AppExtension.ObjectSet()
                                  where ae.Id == dto.AppId
                                  select ae).ToList();

                AppExtension appExt = null;
                if (!appExtList.Any())
                {
                    appExt = AppExtension.CreateAppExtension();
                    appExt.Id = dto.AppId;
                    appExt.IsShowSearchMenu = false;
                    appExt.IsShowAddCart = false;
                    appExt.SubTime = DateTime.Now;
                    appExt.ModifiedOn = DateTime.Now;

                    appExt.IsDividendAll = dto.IsDividendAll;
                    appExt.SharePercent = dto.SharePercent;

                    session.SaveObject(appExt);
                }
                else
                {
                    appExt = appExtList.First();

                    //有属性改变才修改AppExtension.
                    if (appExt.IsDividendAll != dto.IsDividendAll || appExt.SharePercent != dto.SharePercent)
                    {
                        if (appExt.IsDividendAll != dto.IsDividendAll)
                        {
                            isDividendChanged = true;
                        }

                        appExt.IsDividendAll = dto.IsDividendAll;
                        appExt.SharePercent = dto.SharePercent;
                        appExt.ModifiedOn = DateTime.Now;

                        appExt.EntityState = System.Data.EntityState.Modified;
                    }
                }

                if (dto.IsDividendAll == null)
                {
                    //没有设置分享分成 ；清除每个商品上的分成比例。
                    List<Commodity> commodityList =
                        (from c in Commodity.ObjectSet() where c.AppId == dto.AppId select c).ToList();
                    if (commodityList.Any())
                    {
                        foreach (var c in commodityList)
                        {
                            c.SharePercent = null;
                            c.ModifiedOn = DateTime.Now;
                            c.EntityState = EntityState.Modified;

                            //记商品信息变化日志.
                            CommodityJournal journal = CommodityJournal.CreateCommodityJournal(c);
                            session.SaveObject(journal);

                            //更新商品缓存。
                            c.RefreshCache(EntityState.Modified);

                        }
                    }
                }
                //部分商品分享分成。
                else if (dto.IsDividendAll == false)
                {
                    var cShareList = dto.CShareList;
                    var cids = (from c in cShareList select c.CommodityId).Distinct().ToList();
                    var commodityList = (from c in Commodity.ObjectSet()
                                         where cids.Contains(c.Id)
                                         select c).ToList();
                    foreach (var commodity in commodityList)
                    {
                        decimal csp = (from c in cShareList
                                       where c.CommodityId == commodity.Id
                                       select c.SharePercent).FirstOrDefault();
                        if (csp == commodity.SharePercent)
                        {
                            continue;
                        }
                        commodity.SharePercent = csp;
                        commodity.ModifiedOn = DateTime.Now;

                        //记商品信息变化日志.
                        CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                        session.SaveObject(journal);

                        //更新商品缓存。
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
                else if (dto.IsDividendAll == true)
                {
                    //全部商品分享分成 分成比例保存在AppExtension中；清除每个商品上的分成比例。
                    List<Commodity> commodityList =
                        (from c in Commodity.ObjectSet() where c.AppId == dto.AppId select c).ToList();
                    if (commodityList.Any())
                    {
                        foreach (var c in commodityList)
                        {
                            c.SharePercent = null;
                            c.ModifiedOn = DateTime.Now;
                            c.EntityState = EntityState.Modified;

                            //记商品信息变化日志.
                            CommodityJournal journal = CommodityJournal.CreateCommodityJournal(c);
                            session.SaveObject(journal);

                            //更新商品缓存。
                            c.RefreshCache(EntityState.Modified);
                        }
                    }
                }
                session.SaveChanges();
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("保存商品分享分成比例异常", ex);
                result.ResultCode = -1;
                result.Message = "保存商品分享分成比例异常,请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 获取店铺的分享分成信息。
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityShareInfoDTO> GetCommoditySharePercentByAppIdExt(
            Guid appId)
        {
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityShareInfoDTO>();
            try
            {
                if (appId == Guid.Empty)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，appId不能为空！";
                    return result;
                }

                CommodityShareInfoDTO csi = new CommodityShareInfoDTO();
                csi.AppId = appId;

                var appExtQuery = (from ae in AppExtension.ObjectSet()
                                   where ae.Id == appId
                                   select ae).ToList();
                if (appExtQuery != null && appExtQuery.Any())
                {
                    AppExtension appExt = appExtQuery.FirstOrDefault();
                    csi.IsDividendAll = appExt.IsDividendAll;
                    csi.SharePercent = appExt.SharePercent;
                }
                result.Data = csi;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("获取店铺的分享分成信息异常", ex);
                result.ResultCode = -1;
                result.Message = "获取店铺的分享分成信息异常,请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 设置商品是否参加分销
        /// </summary>
        /// <param name="commodityIdList">商品Id列表</param>
        /// <param name="isDistribute">是否分销(false：取消分销。1：参加分销)</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityDistributionExt(List<Guid> commodityIdList,
                                                                                     bool isDistribute)
        {
            ResultDTO result = new ResultDTO() { ResultCode = 1, Message = "参数不能为空" };
            if (commodityIdList == null || commodityIdList.Count < 1)
            {
                return result;
            }
            var exitsList = CommodityDistribution.ObjectSet().Where(t => commodityIdList.Contains(t.Id)).ToList();
            Guid subId = ContextDTO == null ? Guid.Empty : ContextDTO.LoginUserID;
            if (isDistribute)
            {
                var exitsIdList = exitsList.Select(t => t.Id).ToList();
                var insertList = commodityIdList.Except(exitsIdList).ToList();
                if (insertList != null && insertList.Count > 0)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    foreach (Guid id in insertList)
                    {
                        CommodityDistribution model = new CommodityDistribution()
                        {
                            Id = id,
                            SubTime = DateTime.Now,
                            SubId = subId,
                            ModifiedOn = DateTime.Now,
                            L1Percent = null,
                            L2Percent = null,
                            L3Percent = null
                        };
                        model.EntityState = System.Data.EntityState.Added;
                        contextSession.SaveObject(model);

                        CommodityDistributionJounal modelJounal = new CommodityDistributionJounal()
                        {
                            Id = Guid.NewGuid(),
                            SubTime = model.SubTime,
                            ModifiedOn = model.ModifiedOn,
                            SubId = model.SubId,
                            L1Percent = model.L1Percent,
                            L2Percent = model.L2Percent,
                            L3Percent = model.L3Percent,
                            CommodityId = model.Id,
                            IsDel = false
                        };
                        modelJounal.EntityState = System.Data.EntityState.Added;

                        contextSession.SaveObject(modelJounal);
                    }
                    contextSession.SaveChanges();
                }
            }
            else
            {
                if (exitsList != null && exitsList.Count > 0)
                {
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    foreach (var model in exitsList)
                    {
                        contextSession.Delete(model);

                        CommodityDistributionJounal modelJounal = new CommodityDistributionJounal()
                        {
                            Id = Guid.NewGuid(),
                            SubTime = model.SubTime,
                            ModifiedOn = model.ModifiedOn,
                            SubId = model.SubId,
                            L1Percent = model.L1Percent,
                            L2Percent = model.L2Percent,
                            L3Percent = model.L3Percent,
                            CommodityId = model.Id,
                            IsDel = true
                        };
                        modelJounal.EntityState = System.Data.EntityState.Added;

                        contextSession.SaveObject(modelJounal);
                    }
                    contextSession.SaveChanges();
                }
            }
            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }

        /// <summary>
        /// 设置各分销商品佣金比例
        /// </summary>
        /// <param name="commodityDistributionList">佣金比例列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDistributionAccountExt(
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDistributionDTO> commodityDistributionList)
        {
            ResultDTO result = new ResultDTO() { ResultCode = 1, Message = "参数不能为空" };
            if (commodityDistributionList == null || commodityDistributionList.Count < 1)
            {
                return result;
            }
            var idList = commodityDistributionList.Select(t => t.Id).ToList();
            var exitsList = CommodityDistribution.ObjectSet().Where(t => idList.Contains(t.Id)).ToList();
            var idUpdateList = exitsList.Select(t => t.Id).ToList();
            var idInsertList = idList.Except(idUpdateList).ToList();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            //修改
            if (idUpdateList != null && idUpdateList.Count > 0)
            {
                var updateList = exitsList.Where(t => idUpdateList.Contains(t.Id)).ToList();
                foreach (var model in updateList)
                {
                    var inputModel = commodityDistributionList.Where(t => t.Id == model.Id).FirstOrDefault();
                    model.L1Percent = inputModel.L1Percent;
                    model.L2Percent = inputModel.L2Percent;
                    model.L3Percent = inputModel.L3Percent;
                    model.ModifiedOn = DateTime.Now;
                    model.EntityState = System.Data.EntityState.Modified;

                    CommodityDistributionJounal modelJounal = new CommodityDistributionJounal()
                    {
                        Id = Guid.NewGuid(),
                        SubTime = model.SubTime,
                        ModifiedOn = model.ModifiedOn,
                        SubId = model.SubId,
                        L1Percent = model.L1Percent,
                        L2Percent = model.L2Percent,
                        L3Percent = model.L3Percent,
                        CommodityId = model.Id,
                        IsDel = false
                    };
                    modelJounal.EntityState = System.Data.EntityState.Added;

                    contextSession.SaveObject(modelJounal);
                }
            }
            //添加
            //if (idInsertList != null && idInsertList.Count > 0)
            //{
            //    Guid subId = ContextDTO == null ? Guid.Empty : ContextDTO.LoginUserID;
            //    foreach (var id in idInsertList)
            //    {
            //        var inputModel = commodityDistributionList.Where(t => t.Id == id).FirstOrDefault();
            //        CommodityDistribution model = new CommodityDistribution()
            //        {
            //            Id = id,
            //            SubTime = DateTime.Now,
            //            SubId = subId,
            //            ModifiedOn = DateTime.Now,
            //            L1Percent = inputModel.L1Percent,
            //            L2Percent = inputModel.L2Percent,
            //            L3Percent = inputModel.L3Percent
            //        };
            //        model.EntityState = System.Data.EntityState.Added;
            //        contextSession.SaveObject(model);

            //        CommodityDistributionJounal modelJounal = new CommodityDistributionJounal()
            //        {
            //            Id = Guid.NewGuid(),
            //            SubTime = model.SubTime,
            //            ModifiedOn = model.ModifiedOn,
            //            SubId = model.SubId,
            //            L1Percent = model.L1Percent,
            //            L2Percent = model.L2Percent,
            //            L3Percent = model.L3Percent,
            //            CommodityId = model.Id,
            //            IsDel = false
            //        };
            //        modelJounal.EntityState = System.Data.EntityState.Added;

            //        contextSession.SaveObject(modelJounal);
            //    }
            //}
            contextSession.SaveChanges();

            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }

        /// <summary>
        /// 设置分销商品默认佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaulDistributionAccountExt(
            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            ResultDTO result = new ResultDTO() { ResultCode = 1, Message = "参数不能为空" };
            if (appExtension == null || appExtension.Id == Guid.Empty)
            {
                return result;
            }
            var model = AppExtension.ObjectSet().Where(t => t.Id == appExtension.Id).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            if (model != null)
            {
                model.DistributeL1Percent = appExtension.DistributeL1Percent;
                model.DistributeL2Percent = appExtension.DistributeL2Percent;
                model.DistributeL3Percent = appExtension.DistributeL3Percent;
                model.ModifiedOn = DateTime.Now;
                model.EntityState = System.Data.EntityState.Modified;

                contextSession.SaveChanges();
            }
            else
            {
                model = new AppExtension();
                model.Id = appExtension.Id;
                model.AppName = appExtension.AppName;
                model.SubTime = DateTime.Now;
                model.ModifiedOn = DateTime.Now;
                model.IsShowSearchMenu = false;
                model.IsShowAddCart = false;
                model.IsDividendAll = null;
                model.SharePercent = 0;
                model.DistributeL1Percent = appExtension.DistributeL1Percent;
                model.DistributeL2Percent = appExtension.DistributeL2Percent;
                model.DistributeL3Percent = appExtension.DistributeL3Percent;

                model.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(model);
            }

            contextSession.SaveChanges();
            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }

        /// <summary>
        /// 获取分销商品默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulDistributionAccountExt(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return null;
            }
            var model = AppExtension.ObjectSet().Where(t => t.Id == appId).FirstOrDefault();
            if (model == null)
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var appName = APPSV.GetAppName(appId);
                model = new AppExtension();
                model.Id = appId;
                model.AppName = appName;
                model.SubTime = DateTime.Now;
                model.ModifiedOn = DateTime.Now;
                model.IsShowSearchMenu = false;
                model.IsShowAddCart = false;
                model.IsDividendAll = null;
                model.SharePercent = 0;
                model.DistributeL1Percent = null;
                model.DistributeL2Percent = null;
                model.DistributeL3Percent = null;
                model.IsCashForScore = false;
                model.IsScoreAll = null;
                model.ScorePercent = 0;

                model.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(model);
                contextSession.SaveChanges();

            }

            Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO result = new Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO
                ()
            {
                Id = model.Id,
                AppName = model.AppName,
                SubTime = model.SubTime,
                ModifiedOn = model.ModifiedOn,
                IsShowSearchMenu = model.IsShowSearchMenu,
                IsShowAddCart = model.IsShowAddCart,
                IsDividendAll = model.IsDividendAll,
                SharePercent = model.SharePercent,
                DistributeL1Percent = model.DistributeL1Percent,
                DistributeL2Percent = model.DistributeL2Percent,
                DistributeL3Percent = model.DistributeL3Percent,
                IsCashForScore = model.IsCashForScore,
                IsScoreAll = model.IsScoreAll,
                ScorePercent = model.ScorePercent

            };
            return result;
        }

        /// <summary>
        /// 上移一页保存商品
        /// </summary>
        /// <param name="comIds"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUpCommoditySortValueExt(Guid appId, List<Guid> comIds,
                                                                                     Guid id)
        {
            List<Commodity> needRefreshCacheList = new List<Commodity>();
            //查询当前页的第一个商品的SortValue
            var sysp = (from e in Commodity.ObjectSet()
                        where
                            e.AppId == appId && comIds.Contains(e.Id) && e.CommodityType == 0 && e.State == 0 &&
                            e.IsDel == false
                        select e).ToList();
            var sZid = (from e in sysp
                        select e.SortValue).Min();
            //除去第二页第一个商品的SortValue
            List<Int32> sortValues = (from e in sysp
                                      where e.SortValue != sZid
                                      orderby e.SortValue
                                      select e.SortValue).ToList();
            //上一页最后一个商品
            var syzh = (from e in Commodity.ObjectSet()
                        where
                            e.AppId == appId && e.CommodityType == 0 && e.SortValue < sZid && e.State == 0 &&
                            e.IsDel == false
                        orderby e.SortValue descending
                        select e).FirstOrDefault();
            //查询选中的商品
            var xzsp = (from e in sysp where e.Id == id select e).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            //当前页的选中商品的SortValue赋值为上一页最后一个商品的SortValue
            if (xzsp == null || syzh == null)
                return new ResultDTO { ResultCode = 2, Message = "未找到商品" };
            xzsp.SortValue = syzh.SortValue;
            xzsp.EntityState = EntityState.Modified;
            xzsp.ModifiedOn = DateTime.Now;
            needRefreshCacheList.Add(xzsp);
            //上一页最后一个商品的SortValue赋值为当前页第一个商品的SortValue
            syzh.SortValue = sZid;
            syzh.EntityState = EntityState.Modified;
            syzh.ModifiedOn = DateTime.Now;
            needRefreshCacheList.Add(syzh);

            //除去第二页选中的商品进行排序
            var comSortQuery = (from e in sysp where e.Id != id select e).ToDictionary(x => x.Id, y => y);

            int i = 0;
            //对当前页的商品除去选中的商品排序，排序的SortValue是当前页除去第一个商品的SortValue
            foreach (Guid comId in comIds)
            {
                if (comId != id)
                {
                    if (comSortQuery.ContainsKey(comId))
                    {
                        Commodity com = comSortQuery[comId];
                        if (com.SortValue != sortValues[i])
                        {
                            com.SortValue = sortValues[i];
                            com.ModifiedOn = DateTime.Now;
                            needRefreshCacheList.Add(com);
                            com.EntityState = EntityState.Modified;
                        }

                        i++;
                    }
                }
            }
            int rows = -1;
            try
            {
                rows = contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("上移一页错误", ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                //刷新缓存
                if (rows > 0 && needRefreshCacheList.Any())
                {
                    foreach (var commodity in needRefreshCacheList)
                    {
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 下移一页保存商品
        /// </summary>
        /// <param name="comIds"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDownCommoditySortValueExt(Guid appId, List<Guid> comIds,
                                                                                       Guid id)
        {
            List<Commodity> needRefreshCacheList = new List<Commodity>();
            //查询当前页的最后一个商品的SortValue
            var sysp = (from e in Commodity.ObjectSet()
                        where
                            e.AppId == appId && comIds.Contains(e.Id) && e.CommodityType == 0 && e.State == 0 &&
                            e.IsDel == false
                        select e).ToList();
            var sZid = (from e in sysp
                        select e.SortValue).Max();
            //除去第二页第一个商品的SortValue
            List<Int32> sortValues = (from e in sysp
                                      where e.SortValue != sZid
                                      orderby e.SortValue
                                      select e.SortValue).ToList();
            //下一页第一个商品
            var syzh = (from e in Commodity.ObjectSet()
                        where
                            e.AppId == appId && e.CommodityType == 0 && e.SortValue > sZid && e.State == 0 &&
                            e.IsDel == false
                        orderby e.SortValue
                        select e).FirstOrDefault();
            //查询选中的商品
            var xzsp = (from e in sysp where e.Id == id select e).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            //当前页的选中商品的SortValue赋值为下一页第一个商品的SortValue
            if (xzsp == null || syzh == null)
                return new ResultDTO { ResultCode = 2, Message = "未找到商品" };
            xzsp.SortValue = syzh.SortValue;
            xzsp.EntityState = EntityState.Modified;
            xzsp.ModifiedOn = DateTime.Now;
            needRefreshCacheList.Add(xzsp);
            //下一页第一个商品的SortValue赋值为当前页最后一个商品的SortValue
            syzh.SortValue = sZid;
            syzh.EntityState = EntityState.Modified;
            syzh.ModifiedOn = DateTime.Now;
            needRefreshCacheList.Add(syzh);

            //除去当前页选中的商品进行排序
            var comSortQuery = (from e in sysp where e.Id != id select e).ToDictionary(x => x.Id, y => y);
            int i = 0;
            //对当前页的商品除去选中的商品排序，排序的SortValue是当前页除去最后一个商品的SortValue
            foreach (Guid comId in comIds)
            {
                if (comId != id)
                {
                    if (comSortQuery.ContainsKey(comId))
                    {
                        Commodity com = comSortQuery[comId];
                        if (com.SortValue != sortValues[i])
                        {
                            com.SortValue = sortValues[i];
                            com.ModifiedOn = DateTime.Now;
                            needRefreshCacheList.Add(com);
                            com.EntityState = EntityState.Modified;
                        }

                        i++;
                    }
                }
            }
            int rows = -1;
            try
            {
                rows = contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("下移一页错误", ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            finally
            {
                //刷新缓存
                if (rows > 0 && needRefreshCacheList.Any())
                {
                    foreach (var commodity in needRefreshCacheList)
                    {
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 保存商品积分抵用比例。
        /// </summary>
        /// <returns></returns>
        private ResultDTO SaveCommodityScorePercentExt(CommodityScoreDTO dto)
        {
            ResultDTO result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO();
            try
            {
                if (dto == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                //部分商品
                if (dto.IsAll == false && (dto.CScoreList == null || !dto.CScoreList.Any()))
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，积分抵用商品列表不能为空！";
                    return result;
                }
                ContextSession session = ContextFactory.CurrentThreadContext;

                //如果不是全部商品，则全局积分抵用比例为0
                if (dto.IsAll == null || dto.IsAll == false)
                {
                    dto.ScorePercent = 0;
                }

                //app设置
                var appExtension = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == dto.AppId);


                if (appExtension == null)
                {
                    appExtension = AppExtension.CreateAppExtension();
                    appExtension.Id = dto.AppId;
                    appExtension.IsShowSearchMenu = false;
                    appExtension.IsShowAddCart = false;
                    appExtension.SubTime = DateTime.Now;
                    appExtension.ModifiedOn = DateTime.Now;

                    appExtension.IsScoreAll = dto.IsAll;
                    appExtension.ScorePercent = dto.ScorePercent;

                    session.SaveObject(appExtension);
                }
                else
                {

                    //有属性改变才修改AppExtension.
                    if (appExtension.IsScoreAll != dto.IsAll || appExtension.ScorePercent != dto.ScorePercent)
                    {
                        appExtension.IsScoreAll = dto.IsAll;
                        appExtension.ScorePercent = dto.ScorePercent;
                        appExtension.ModifiedOn = DateTime.Now;

                        appExtension.EntityState = System.Data.EntityState.Modified;
                    }
                }

                if (dto.IsAll == null)
                {
                    //没有设置积分抵现 ；清除每个商品上的比例。
                    List<Commodity> commodityList =
                        (from c in Commodity.ObjectSet() where c.AppId == dto.AppId select c).ToList();
                    if (commodityList.Any())
                    {
                        foreach (var c in commodityList)
                        {
                            c.ScorePercent = null;
                            c.ModifiedOn = DateTime.Now;
                            c.EntityState = EntityState.Modified;

                            //记商品信息变化日志.
                            CommodityJournal journal = CommodityJournal.CreateCommodityJournal(c);
                            session.SaveObject(journal);

                            //更新商品缓存。
                            c.RefreshCache(EntityState.Modified);

                        }
                    }
                }
                //部分商品。
                else if (dto.IsAll == false)
                {
                    var cScoreList = dto.CScoreList;
                    var cids = (from c in cScoreList select c.CommodityId).Distinct().ToList();
                    var commodityList = (from c in Commodity.ObjectSet()
                                         where cids.Contains(c.Id)
                                         select c).ToList();
                    foreach (var commodity in commodityList)
                    {
                        decimal csp = (from c in cScoreList
                                       where c.CommodityId == commodity.Id
                                       select c.ScorePercent).FirstOrDefault();
                        if (csp == commodity.ScorePercent)
                        {
                            continue;
                        }
                        commodity.ScorePercent = csp;
                        commodity.ModifiedOn = DateTime.Now;

                        //记商品信息变化日志.
                        CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                        session.SaveObject(journal);

                        //更新商品缓存。
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
                else if (dto.IsAll == true)
                {
                    //全部商品 比例保存在AppExtension中；清除每个商品上的积分抵用比例。
                    List<Commodity> commodityList =
                        (from c in Commodity.ObjectSet() where c.AppId == dto.AppId select c).ToList();
                    if (commodityList.Any())
                    {
                        foreach (var c in commodityList)
                        {
                            c.ScorePercent = null;
                            c.ModifiedOn = DateTime.Now;
                            c.EntityState = EntityState.Modified;

                            //记商品信息变化日志.
                            CommodityJournal journal = CommodityJournal.CreateCommodityJournal(c);
                            session.SaveObject(journal);

                            //更新商品缓存。
                            c.RefreshCache(EntityState.Modified);
                        }
                    }
                }
                session.SaveChanges();
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("保存商品积分抵现比例异常", ex);
                result.ResultCode = -1;
                result.Message = "保存商品积分抵现比例异常,请稍后重试！";
            }
            return result;
        }
        /// <summary>
        /// 获取店铺的积分抵用信息。
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns>商品列表</returns>
        private ResultDTO<CommodityScoreDTO> GetCommodityScorePercentByAppIdExt(Guid appId)
        {
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityScoreDTO>();
            try
            {
                if (appId == Guid.Empty)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，appId不能为空！";
                    return result;
                }

                CommodityScoreDTO csi = new CommodityScoreDTO();
                csi.AppId = appId;

                var appExtension = AppExtension.ObjectSet().FirstOrDefault(c => c.Id == appId);
                if (appExtension != null)
                {
                    csi.IsAll = appExtension.IsScoreAll;
                    csi.ScorePercent = appExtension.ScorePercent;
                }
                result.Data = csi;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("获取店铺的积分抵现信息异常", ex);
                result.ResultCode = -1;
                result.Message = "获取店铺的积分抵现信息异常,请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 根据商品Id获取商品餐盒信息，舌尖项目专用。
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public CateringComdtyXDataDTO GetCommodityBoxInfoExt(Guid commodityId)
        {
            var cbox = CateringComdtyXData.ObjectSet().Where(r => r.ComdtyId == commodityId && r.IsValid).FirstOrDefault();
            if (cbox != null)
            {
                return cbox.ToEntityData();
            }
            return null;
        }

        /// <summary>
        /// 获取店铺的分享分成信息。
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityShareInfoDTO> GetCommoditySpreadPercentByAppIdExt(
            Guid appId)
        {
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityShareInfoDTO>();
            try
            {
                if (appId == Guid.Empty)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，appId不能为空！";
                    return result;
                }

                CommodityShareInfoDTO csi = new CommodityShareInfoDTO();
                csi.AppId = appId;

                var appExtQuery = (from ae in AppExtension.ObjectSet()
                                   where ae.Id == appId
                                   select ae).ToList();
                if (appExtQuery != null && appExtQuery.Any())
                {
                    AppExtension appExt = appExtQuery.FirstOrDefault();
                    csi.IsDividendAll = appExt.IsSpreadAll;
                    csi.SharePercent = appExt.SpreadPercent;
                }
                result.Data = csi;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("获取店铺的分享分成信息异常", ex);
                result.ResultCode = -1;
                result.Message = "获取店铺的分享分成信息异常,请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 保存商品分享分成比例。
        /// </summary>
        /// <returns></returns>
        public ResultDTO SaveCommoditySpreadPercentExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            ResultDTO result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO();
            try
            {
                if (dto == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                //部分分享，没有分享商品列表。
                if (dto.IsDividendAll == false && (dto.CShareList == null || !dto.CShareList.Any()))
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，分享商品列表不能为空！";
                    return result;
                }
                ContextSession session = ContextFactory.CurrentThreadContext;

                bool isDividendChanged = false;
                //不是全部商品分成,则“全部商品分成比例”为0.
                if (dto.IsDividendAll == null || dto.IsDividendAll == false)
                {
                    dto.SharePercent = 0;
                }

                //保存这个店铺（应用）的分成范围、分成比例。
                var appExtList = (from ae in AppExtension.ObjectSet()
                                  where ae.Id == dto.AppId
                                  select ae).ToList();

                AppExtension appExt = null;
                if (!appExtList.Any())
                {
                    appExt = AppExtension.CreateAppExtension();
                    appExt.Id = dto.AppId;
                    appExt.IsShowSearchMenu = false;
                    appExt.IsShowAddCart = false;
                    appExt.SubTime = DateTime.Now;
                    appExt.ModifiedOn = DateTime.Now;

                    appExt.IsSpreadAll = dto.IsDividendAll;
                    appExt.SpreadPercent = dto.SharePercent;

                    session.SaveObject(appExt);
                }
                else
                {
                    appExt = appExtList.First();

                    //有属性改变才修改AppExtension.
                    if (appExt.IsSpreadAll != dto.IsDividendAll || appExt.SpreadPercent != dto.SharePercent)
                    {
                        if (appExt.IsDividendAll != dto.IsDividendAll)
                        {
                            isDividendChanged = true;
                        }

                        appExt.IsSpreadAll = dto.IsDividendAll;
                        appExt.SpreadPercent = dto.SharePercent;
                        appExt.ModifiedOn = DateTime.Now;

                        appExt.EntityState = System.Data.EntityState.Modified;
                    }
                }

                if (dto.IsDividendAll == null)
                {
                    //没有设置分享分成 ；清除每个商品上的分成比例。
                    List<Commodity> commodityList =
                        (from c in Commodity.ObjectSet() where c.AppId == dto.AppId select c).ToList();
                    if (commodityList.Any())
                    {
                        foreach (var c in commodityList)
                        {
                            c.SpreadPercent = null;
                            c.ModifiedOn = DateTime.Now;
                            c.EntityState = EntityState.Modified;

                            //记商品信息变化日志.
                            CommodityJournal journal = CommodityJournal.CreateCommodityJournal(c);
                            session.SaveObject(journal);

                            //更新商品缓存。
                            c.RefreshCache(EntityState.Modified);

                        }
                    }
                }
                //部分商品分享分成。
                else if (dto.IsDividendAll == false)
                {
                    var cShareList = dto.CShareList;
                    var cids = (from c in cShareList select c.CommodityId).Distinct().ToList();
                    var commodityList = (from c in Commodity.ObjectSet()
                                         where cids.Contains(c.Id)
                                         select c).ToList();
                    foreach (var commodity in commodityList)
                    {
                        decimal csp = (from c in cShareList
                                       where c.CommodityId == commodity.Id
                                       select c.SharePercent).FirstOrDefault();
                        if (csp == commodity.SpreadPercent)
                        {
                            continue;
                        }
                        commodity.SpreadPercent = csp;
                        commodity.ModifiedOn = DateTime.Now;

                        //记商品信息变化日志.
                        CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                        session.SaveObject(journal);

                        //更新商品缓存。
                        commodity.RefreshCache(EntityState.Modified);
                    }
                }
                else if (dto.IsDividendAll == true)
                {
                    //全部商品分享分成 分成比例保存在AppExtension中；清除每个商品上的分成比例。
                    List<Commodity> commodityList =
                        (from c in Commodity.ObjectSet() where c.AppId == dto.AppId select c).ToList();
                    if (commodityList.Any())
                    {
                        foreach (var c in commodityList)
                        {
                            c.SpreadPercent = null;
                            c.ModifiedOn = DateTime.Now;
                            c.EntityState = EntityState.Modified;

                            //记商品信息变化日志.
                            CommodityJournal journal = CommodityJournal.CreateCommodityJournal(c);
                            session.SaveObject(journal);

                            //更新商品缓存。
                            c.RefreshCache(EntityState.Modified);
                        }
                    }
                }
                session.SaveChanges();
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error("保存商品分享分成比例异常", ex);
                result.ResultCode = -1;
                result.Message = "保存商品分享分成比例异常,请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 获取商品税收编码列表
        /// </summary>
        /// <returns></returns>
        public ResultDTO<List<CommodityTaxRateCDTO>> GetSingleCommodityCodeExt(string name, double taxrate, int pageIndex, int pageSize)
        {
            var commodityTaxRate = from c in CommodityTaxRate.ObjectSet()
                                   orderby c.Code
                                   select new CommodityTaxRateCDTO
                                   {
                                       Id = Guid.NewGuid(),
                                       Code = c.Code,
                                       Name = c.Name,
                                       TaxRate = (double)c.TaxRate,
                                       VersionCode = c.VersionCode
                                   };
            if (!string.IsNullOrEmpty(name))
            {
                commodityTaxRate = commodityTaxRate.Where(o => o.Name.Contains(name));
            }
            if (taxrate != null && taxrate != -1)
            {
                commodityTaxRate = commodityTaxRate.Where(o => o.TaxRate == taxrate);
            }


            ResultDTO<List<CommodityTaxRateCDTO>> retInfo = new ResultDTO<List<CommodityTaxRateCDTO>>
            {
                ResultCode = commodityTaxRate.Count(),
                Data = commodityTaxRate.Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList()
            };
            return retInfo;
        }

        /// <summary>
        /// 根据id获取商品详情内容
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodityDetailExt(Guid CommodityId)
        {
            Jinher.AMP.BTP.Deploy.CommodityDTO model = new Jinher.AMP.BTP.Deploy.CommodityDTO();
            var commodity = Commodity.ObjectSet().Where(p => p.Id == CommodityId && p.IsDel == false).FirstOrDefault();
            if (commodity != null)
            {
                model = CommonUtil.ReadObjectExchange(model, commodity);

            }
            return model;
        }
        private Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityChange(List<System.Guid> ids)
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list = new List<Deploy.CustomDTO.CommodityChangeDTO>();
            foreach (var item in ids)
            {
                #region //取出商品变动明细插入CommodityChange
                //取出Commodity表中编辑的数据
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list1 = (from n in Commodity.ObjectSet()
                                                                                  where n.Id == item
                                                                                  select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                  {
                                                                                      CommodityId = n.Id,
                                                                                      Name = n.Name,
                                                                                      Code = n.Code,
                                                                                      No_Number = n.No_Number,
                                                                                      SubId = n.SubId,
                                                                                      Price = n.Price,
                                                                                      Stock = n.Stock,
                                                                                      PicturesPath = n.PicturesPath,
                                                                                      Description = n.Description,
                                                                                      State = n.State,
                                                                                      IsDel = n.IsDel,
                                                                                      AppId = n.AppId,
                                                                                      No_Code = n.No_Code,
                                                                                      TotalCollection = n.TotalCollection,
                                                                                      TotalReview = n.TotalReview,
                                                                                      Salesvolume = n.Salesvolume,
                                                                                      ModifiedOn = n.ModifiedOn,
                                                                                      GroundTime = n.GroundTime,
                                                                                      ComAttribute = n.ComAttribute,
                                                                                      CategoryName = n.CategoryName,
                                                                                      SortValue = n.SortValue,
                                                                                      FreightTemplateId = n.FreightTemplateId,
                                                                                      MarketPrice = n.MarketPrice,
                                                                                      IsEnableSelfTake = n.IsEnableSelfTake,
                                                                                      Weight = n.Weight,
                                                                                      PricingMethod = n.PricingMethod,
                                                                                      SaleAreas = n.SaleAreas,
                                                                                      SharePercent = n.SharePercent,
                                                                                      CommodityType = n.CommodityType,
                                                                                      HtmlVideoPath = n.HtmlVideoPath,
                                                                                      MobileVideoPath = n.MobileVideoPath,
                                                                                      VideoPic = n.VideoPic,
                                                                                      VideoName = n.VideoName,
                                                                                      ScorePercent = n.ScorePercent,
                                                                                      Duty = n.Duty,
                                                                                      SpreadPercent = n.SpreadPercent,
                                                                                      ScoreScale = n.ScoreScale,
                                                                                      TaxRate = n.TaxRate,
                                                                                      TaxClassCode = n.TaxClassCode,
                                                                                      Unit = n.Unit,
                                                                                      InputRax = n.InputRax,
                                                                                      Barcode = n.Barcode,
                                                                                      JDCode = n.JDCode,
                                                                                      CostPrice = n.CostPrice,
                                                                                      IsAssurance = n.IsAssurance,
                                                                                      TechSpecs = n.TechSpecs,
                                                                                      SaleService = n.SaleService,
                                                                                      IsReturns = n.IsReturns,
                                                                                      Isnsupport = n.Isnsupport,
                                                                                      ServiceSettingId = n.ServiceSettingId,
                                                                                      Type = n.Type,
                                                                                      YJCouponActivityId = n.YJCouponActivityId,
                                                                                      YJCouponType = n.YJCouponType,
                                                                                      SubOn = n.SubTime,
                                                                                      ModifiedId = n.ModifieId
                                                                                  }).ToList();
                //取出CommodityStock表中编辑的数据
                List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO> list2 = (from n in Commodity.ObjectSet()
                                                                                  join m in CommodityStock.ObjectSet() on n.Id equals m.CommodityId
                                                                                  where m.CommodityId == item
                                                                                  select new Jinher.AMP.BTP.Deploy.CustomDTO.CommodityChangeDTO
                                                                                  {
                                                                                      CommodityId = m.Id,
                                                                                      Name = n.Name,
                                                                                      Code = n.Code,
                                                                                      No_Number = n.No_Number,
                                                                                      SubId = n.SubId,
                                                                                      Price = m.Price,
                                                                                      Stock = m.Stock,
                                                                                      PicturesPath = n.PicturesPath,
                                                                                      Description = n.Description,
                                                                                      State = n.State,
                                                                                      IsDel = n.IsDel,
                                                                                      AppId = n.AppId,
                                                                                      No_Code = m.No_Code,
                                                                                      TotalCollection = n.TotalCollection,
                                                                                      TotalReview = n.TotalReview,
                                                                                      Salesvolume = n.Salesvolume,
                                                                                      ModifiedOn = n.ModifiedOn,
                                                                                      GroundTime = n.GroundTime,
                                                                                      ComAttribute = n.ComAttribute,
                                                                                      CategoryName = n.CategoryName,
                                                                                      SortValue = n.SortValue,
                                                                                      FreightTemplateId = n.FreightTemplateId,
                                                                                      MarketPrice = m.MarketPrice,
                                                                                      IsEnableSelfTake = n.IsEnableSelfTake,
                                                                                      Weight = n.Weight,
                                                                                      PricingMethod = n.PricingMethod,
                                                                                      SaleAreas = n.SaleAreas,
                                                                                      SharePercent = n.SharePercent,
                                                                                      CommodityType = n.CommodityType,
                                                                                      HtmlVideoPath = n.HtmlVideoPath,
                                                                                      MobileVideoPath = n.MobileVideoPath,
                                                                                      VideoPic = n.VideoPic,
                                                                                      VideoName = n.VideoName,
                                                                                      ScorePercent = n.ScorePercent,
                                                                                      Duty = m.Duty,
                                                                                      SpreadPercent = n.SpreadPercent,
                                                                                      ScoreScale = n.ScoreScale,
                                                                                      TaxRate = n.TaxRate,
                                                                                      TaxClassCode = n.TaxClassCode,
                                                                                      Unit = n.Unit,
                                                                                      InputRax = n.InputRax,
                                                                                      Barcode = m.Barcode,
                                                                                      JDCode = m.JDCode,
                                                                                      CostPrice = m.CostPrice,
                                                                                      IsAssurance = n.IsAssurance,
                                                                                      TechSpecs = n.TechSpecs,
                                                                                      SaleService = n.SaleService,
                                                                                      IsReturns = n.IsReturns,
                                                                                      Isnsupport = n.Isnsupport,
                                                                                      ServiceSettingId = n.ServiceSettingId,
                                                                                      Type = n.Type,
                                                                                      YJCouponActivityId = n.YJCouponActivityId,
                                                                                      YJCouponType = n.YJCouponType,
                                                                                      SubOn = n.SubTime,
                                                                                      ModifiedId = n.ModifieId
                                                                                  }).ToList();
                if (list2.Count() > 0)
                {
                    list.AddRange(list2);
                }
                else
                {
                    list.AddRange(list1);
                }
                #endregion
            }
            CommodityChangeFacade ChangeFacade = new CommodityChangeFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DTO = ChangeFacade.SaveCommodityChange(list);
            return DTO;
        }





        /// <summary>
        /// 设置退货运费物流模板
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <param name="TempId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityRefoundFreightTempExt(Guid CommodityId, Guid TempId)
        {
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO() { Message = "sucess", isSuccess = true, ResultCode = 1 };
            try
            {
                var entyt = Commodity.ObjectSet().FirstOrDefault(o => o.Id == CommodityId);
                if (entyt != null)
                {
                    entyt.RefundFreightTemplateId = TempId;
                    Commodity.Update(entyt);
                    result.isSuccess = true;
                    result.ResultCode = 1;
                }
                else
                {
                    result.isSuccess = false;
                    result.ResultCode = 0;
                    result.Message = "商品信息不存在";
                }
            }
            catch (Exception ex)
            {
                result.ResultCode = -1;
                result.Message = "获取商品信息异常,请稍后重试！";
            }

            return result;
        }

        /// <summary>
        /// 查询商品物流模板
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="GoodName"></param>
        /// <param name="State"></param>
        /// <param name="FreightID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.CommodityAndTemplateDTO>> GetCommodityFreightTemplateExt(Guid AppId, string GoodName, string State, string FreightID, int pageIndex, int pageSize)
        {
            try
            {
                var jquery = from c in Commodity.ObjectSet()
                             where c.AppId == AppId
                             select new CommodityAndTemplateDTO()
                             {
                                 Id = c.Id,
                                 No_Code = c.No_Code,
                                 goodName = c.Name,
                                 State = c.State,
                                 No_Number = c.No_Number,
                                 Price = c.Price,
                                 Stock = c.Stock,
                                 PicturesPath = c.PicturesPath,
                                 FreightTemplateId = c.FreightTemplateId,
                                 RefundTemplateId = c.RefundFreightTemplateId
                             };

                if (!String.IsNullOrEmpty(GoodName))
                {
                    jquery = jquery.Where(o => o.goodName.Contains(GoodName));
                }

                if (!String.IsNullOrEmpty(State))
                {
                    var _state = int.Parse(State);
                    jquery = jquery.Where(o => o.State == _state);
                }

                if (!String.IsNullOrEmpty(FreightID))
                {
                    jquery = jquery.Where(o => o.RefundTemplateId != Guid.Empty && FreightID != null);
                }

                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndTemplateDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndTemplateDTO>>
                {
                    ResultCode = jquery.Count()
                };

                var result = jquery.OrderByDescending(p => p.No_Code).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var listFreightTemplate = FreightTemplate.ObjectSet();

                foreach (var item in result)
                {
                    if (item.FreightTemplateId != null && item.FreightTemplateId != Guid.Empty)
                    {
                        var freightTemp = listFreightTemplate.FirstOrDefault(o => o.Id == item.FreightTemplateId);
                        if (freightTemp != null)
                            //item.FreightTemplateId = freightTemp.Id;
                            item.FirstCountPrice = freightTemp.FirstCountPrice;
                    }

                    if (item.RefundTemplateId != null && item.RefundTemplateId != Guid.Empty)
                    {
                        var freightTemp = listFreightTemplate.FirstOrDefault(o => o.Id == item.RefundTemplateId);
                        //item.RefundTemplateId = freightTemp.Id;
                        if (freightTemp != null)
                            item.RefundFreightPrice = freightTemp.FirstCountPrice;
                    }
                }

                retInfo.Data = result;

                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取商品退货运费信息失败。GetCommodityFreightTemplate", ex);
                return null;
            }
        }
    }
}
