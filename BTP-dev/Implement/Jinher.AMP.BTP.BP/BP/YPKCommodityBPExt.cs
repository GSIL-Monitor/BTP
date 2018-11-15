
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/8/30 15:23:22
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
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;
using Jinher.AMP.BTP.Deploy.CustomDTO.YPK;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Data;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class YPKCommodityBP : BaseBP, IYPKCommodity
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYPKCommodityExt(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            try
            {
                if (input == null)
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "参数不能为空~" };
                }
                if (!string.IsNullOrEmpty(input.JDCode) && !IsExistsSNCodeExt(input.JDCode, input.AppId))
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "列表中已存在该备注编码~" };
                }
                Guid userId = this.ContextDTO.LoginUserID;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                JdCom.TaxRate = input.TaxRate;
                JdCom.InputRax = input.InputRax;
                JdCom.TaxClassCode = input.TaxClassCode;
                JdCom.Stock = 999;
                JdCom.State = 0; //是否补全 
                JdCom.SubId = userId;
                JdCom.SubTime = DateTime.Now;
                JdCom.AppId = input.AppId;
                JdCom.JDCode = input.JDCode;
                JdCom.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,640000,520000,130000,630000";//出去港澳台 新疆 西藏                
                JdCom.No_Code = JdCom.JDCode + "0000";
                JdCom.ErQiCode = input.ErQiCode;
                JdCom.CostPrice = Decimal.Zero;
                JdCom.Price = input.Price;
                JdCom.IsAssurance = input.IsAssurance;
                JdCom.IsReturns = input.IsReturns;
                JdCom.Isnsupport = input.Isnsupport;
                JdCom.ServiceSettingId = input.ServiceSettingId;
                JdCom.TechSpecs = input.TechSpecs;
                JdCom.SaleService = input.SaleService;
                JdCom.CategoryName = input.CategoryName;
                //商城分类借用字段
                JdCom.VideoName = input.VideoName;
                contextSession.SaveObject(JdCom);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNCommodityBP.AddSNCommodityExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "添加成功" };
        }
        /// <summary>
        /// 异常数据
        /// </summary>
        /// <param name="skuIds"></param>
        /// <param name="JdComList"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<YPKException> ExportData(List<string> skuIds, List<JdCommodityDTO> JdComList, int type)
        {
            List<YPKException> ypkes = new List<YPKException>();
            foreach (var item in skuIds)
            {
                var jdCom = JdComList.Where(j => j.JDCode == item && !j.IsDel).FirstOrDefault();
                var ypke = new YPKException();
                ypke.SkuId = item;
                ypke.CategoryName = jdCom.CategoryName;
                ypke.VideoName = jdCom.VideoName;
                ypke.TaxClassCode = jdCom.TaxClassCode;
                ypke.TaxRate = jdCom.TaxRate;
                ypke.InputRax = jdCom.InputRax;
                ypke.Price = jdCom.Price;
                switch (type)
                {
                    case 1:
                        ypke.Remark = "接口中获取不到商品信息";
                        break;
                    case 2:
                        ypke.Remark = "店铺中已存在";
                        break;
                    case 3:
                        ypke.Remark = "商品同步列表中已存在";
                        break;
                    default:
                        ypke.Remark = "数据异常";
                        break;
                }
                ypkes.Add(ypke);
            }
            return ypkes;
        }
        /// <summary>
        /// 导入易派客商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYPKCommodityDataExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
            JdComDTO.ExceptionData = new List<YPKException>();
            List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
            //List<SNPriceDto> jdPrices = new List<SNPriceDto>();
            //for (int i = 0; i < skuIds.Count; i += 30)
            //{
            //    jdPrices.AddRange(SuningSV.GetPrice(skuIds.Skip(i).Take(30).ToList()));
            //}
            //List<string> YPKSkuIds = new List<string>();
            //YPKSkuIds.AddRange(jdPrices.Select(_ => _.skuId.ToString()).ToList());

            ////易派客商品通过接口获取不到的数据
            //JdComDTO.InvalidData = skuIds.Except(YPKSkuIds).ToList();
            //ExportData(JdComDTO.InvalidData, JdComList, 1);
            //店铺中已存在的易派客编码
            JdComDTO.RepeatData = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).Select(s => s.JDCode).ToList();
            if (JdComDTO.RepeatData.Count > 0)
            {
                JdComDTO.ExceptionData.AddRange(ExportData(JdComDTO.RepeatData, JdComList, 2));
            }
            //易派客商品表中已存在的备注编码
            JdComDTO.JdRepeatData = JdCommodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId && p.State != 1).Select(s => s.JDCode).ToList();
            if (JdComDTO.JdRepeatData.Count > 0)
            {
                JdComDTO.ExceptionData.AddRange(ExportData(JdComDTO.JdRepeatData, JdComList, 3));
            }
            //excel表中总商品数
            var totalCount = skuIds.Count;
            //成功商品数
            var successCount = 0;
            //异常商品数
            var ExcepitonCount = JdComDTO.RepeatData.Count + JdComDTO.JdRepeatData.Count;
            //有效商品
            var validData = skuIds.Except(JdComDTO.RepeatData).Except(JdComDTO.JdRepeatData).ToList();
            JdComList = JdComList.Where(j => validData.Contains(j.JDCode)).ToList();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            Guid userId = this.ContextDTO.LoginUserID;
            //获取不到商品类目的
            List<string> NoCategoryData = new List<string>();
            foreach (var input in JdComList)
            {
                try
                {
                    JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                    JdCom.CostPrice = Convert.ToDecimal(input.CostPrice);
                    JdCom.Price = Convert.ToDecimal(input.Price);
                    JdCom.Barcode = input.JDCode;
                    JdCom.TaxRate = input.TaxRate;
                    JdCom.InputRax = input.InputRax;
                    JdCom.TaxClassCode = input.TaxClassCode;
                    JdCom.Stock = 0;
                    JdCom.State = 0; //是否补全
                    JdCom.SubId = userId;
                    JdCom.SubTime = DateTime.Now;
                    JdCom.AppId = input.AppId;
                    JdCom.JDCode = input.JDCode;
                    JdCom.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,520000,130000,630000";//出去港澳台 新疆 西藏                
                    JdCom.No_Code = JdCom.JDCode + "0000";
                    JdCom.ErQiCode = input.ErQiCode;
                    JdCom.IsAssurance = input.IsAssurance;
                    JdCom.IsReturns = input.IsReturns;
                    JdCom.Isnsupport = input.Isnsupport;
                    JdCom.ServiceSettingId = input.ServiceSettingId;
                    JdCom.TechSpecs = input.TechSpecs;
                    JdCom.SaleService = input.SaleService;
                    JdCom.CategoryName = input.CategoryName;
                    JdCom.VideoName = input.VideoName;
                    contextSession.SaveObject(JdCom);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("YPKCommodityBP.ImportYPKCommodityDataExt 异常", ex);
                    YPKException exYpk = new YPKException();
                    exYpk.SkuId = input.JDCode;
                    exYpk.CategoryName = input.CategoryName;
                    exYpk.VideoName = input.VideoName;
                    exYpk.TaxClassCode = input.TaxClassCode;
                    exYpk.TaxRate = input.TaxRate;
                    exYpk.InputRax = input.InputRax;
                    exYpk.Price = input.Price;
                    exYpk.Remark = ex.Message;
                    JdComDTO.ExceptionData.Add(exYpk);
                    ExcepitonCount++;
                }
            }
            int count = contextSession.SaveChanges();
            successCount = totalCount - ExcepitonCount;
            JdComDTO.FaildCount = ExcepitonCount;
            JdComDTO.SuccessCount = successCount;
            return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 0, isSuccess = true, Message = "导入成功" };
        }
        /// <summary>
        /// 自动同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYPKCommodityInfoExt(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            try
            {

                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                Guid userId = this.ContextDTO.LoginUserID;
                List<YPKException> YpkExceptionList = new List<YPKException>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //取出补全的商品信息
                var JdComList = JdCommodity.ObjectSet().Where(p => p.AppId == AppId && Ids.Contains(p.Id) && p.State == 0).ToList();
                List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
                //要同步的商品总数量
                var totalCount = skuIds.Count;
                //成功商品数
                var successCount = 0;
                //异常商品数
                var ExcepitonCount = 0;
                //获取易派客商品详情 
                List<YPKComDetailDto> YPKComDetailList = new List<YPKComDetailDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    YPKComDetailList.AddRange(YPKSV.GetYPKComDetail(skuIds.Skip(i).Take(99).ToList()));
                }
                var ypkSkuids = YPKComDetailList.Select(y => y.skuId).ToList();
                var NullData = skuIds.Except(ypkSkuids).ToList();
                if (NullData.Count > 0)
                {
                    YPKException yPKException = new YPKException();
                    yPKException.SkuId = string.Join(",", NullData.ToArray());
                    yPKException.Remark = "接口中获取不到商品数据";
                    YpkExceptionList.Add(yPKException);
                }
                //有效商品
                var ValidData = JdComList.Where(j => ypkSkuids.Contains(j.JDCode));
                //获取商城品类
                var InnerCategoryNameList = JdComList.Select(s => s.VideoName.Trim()).ToList();
                //获取运费模板
                var FreightTemplateInfo = FreightTemplate.ObjectSet().FirstOrDefault(p => p.AppId == AppId && p.IsDefault == 1);
                //获取商品分类
                var CategoryNameList = JdComList.Select(s => s.CategoryName).ToList();//获取所有的商品分类               
                string ggg = string.Join(",", CategoryNameList.ToArray());
                List<string> list = new List<string>(ggg.Split(','));
                //易捷北京
                //var CategoryList = Category.ObjectSet().Where(p => p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel && CategoryNameList.Contains(p.Name)).ToList();
                //var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel).ToList();
                //本地调试
                var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && !p.IsDel).ToList();
                var CategoryList = Category.ObjectSet().Where(p => p.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && !p.IsDel && list.Contains(p.Name)).ToList();

                //获取不到商品类目的
                List<string> NoCategoryData = new List<string>();
                //获取不到商城品类的
                List<string> NoPinLeiData = new List<string>();
                //店铺中存在的备注编码进行更新  不存在则插入
                var ExistCommodity = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).ToList();
                JdComDTO.RepeatData = ExistCommodity.Select(s => s.JDCode).ToList();
                //店铺中存在的商品异常数量
                var existExcepitonCount = 0;
                foreach (var item in ExistCommodity)
                {
                    try
                    {
                        var YpkComDetailInfo = YPKComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();

                        var JdCom = JdComList.FirstOrDefault(p => p.JDCode == item.JDCode);
                        var minSortValueQuery = (from m in Commodity.ObjectSet()
                                                 where m.AppId == AppId && m.CommodityType == 0
                                                 select m);
                        int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                        int minSortValue = 2;
                        if (minSort.HasValue)
                        {
                            minSortValue = minSort.Value;
                        }
                        Commodity Com = new Commodity();
                        //默认“七天无理由退”
                        Com.IsAssurance = true;
                        Com.IsReturns = false;
                        Com.Isnsupport = false;

                        Com.Id = item.Id;
                        Com.Name = YpkComDetailInfo.name;
                        Com.Code = JdCom.Code;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = JdCom.No_Number ?? 0;
                        Com.Price = JdCom.Price ?? 0;
                        Com.Stock = JdCom.Stock;
                        if (YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            //图片数组第一张为主图
                            Com.PicturesPath = YpkComDetailInfo.picturesPath[0];
                        }
                        if (YpkComDetailInfo.description != null && YpkComDetailInfo.description != "")
                        {
                            string Div = YpkComDetailInfo.description;
                            if (YpkComDetailInfo.description.Length > 10)
                            {
                                Div = YpkComDetailInfo.description.Substring(0, 10);
                            }
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>") + "</div>";
                            }
                            else
                            {
                                Com.Description = YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>");
                            }
                        }
                        else
                        {
                            Com.Description = string.Empty;
                        }
                        //Com.State = 1; 只更新商品信息,不更新商品上下架状态
                        Com.IsDel = false;
                        Com.AppId = JdCom.AppId;
                        Com.No_Code = JdCom.No_Code;
                        Com.TotalCollection = 0;
                        Com.TotalReview = 0;
                        Com.Salesvolume = 0;
                        Com.ModifiedOn = DateTime.Now;
                        Com.ComAttribute = JdCom.ComAttribute;
                        Com.CategoryName = JdCom.CategoryName;
                        Com.SortValue = minSortValue - 1;
                        if (FreightTemplateInfo != null)
                            Com.FreightTemplateId = FreightTemplateInfo.Id;
                        Com.MarketPrice = JdCom.MarketPrice;
                        Com.Weight = string.IsNullOrEmpty(YpkComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(YpkComDetailInfo.weight);
                        //Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                        Com.SaleAreas = JdCom.SaleAreas;
                        Com.SharePercent = JdCom.SharePercent;
                        Com.CommodityType = 0;
                        Com.Duty = JdCom.Duty;
                        Com.TaxRate = JdCom.TaxRate;
                        Com.TaxClassCode = JdCom.TaxClassCode;
                        Com.Unit = string.IsNullOrEmpty(YpkComDetailInfo.unit) ? "件" : YpkComDetailInfo.unit;
                        Com.InputRax = JdCom.InputRax;
                        Com.Barcode = YpkComDetailInfo.barCode;
                        Com.ErQiCode = YpkComDetailInfo.erQiCode;
                        Com.JDCode = JdCom.JDCode;
                        Com.CostPrice = Convert.ToDecimal(YpkComDetailInfo.price);
                        Com.ServiceSettingId = JdCom.ServiceSettingId;
                        Com.TechSpecs = string.Empty;
                        Com.SaleService = JdCom.SaleService;
                        Com.Type = 0;
                        Com.YJCouponActivityId = "";
                        Com.YJCouponType = "";
                        Com.ModifieId = userId;
                        Com.EntityState = EntityState.Modified;
                        contextSession.SaveObject(Com);
                        Com.RefreshCache(EntityState.Modified);
                        //更新库存表
                        UpdateCommodityStock(Com, contextSession);
                        int count1 = contextSession.SaveChanges();
                        //更新JdCommodity表中状态
                        JdCom.State = 1;
                        //JdCom.EntityState = EntityState.Modified;
                        contextSession.SaveObject(JdCom);
                        #region 商品图片
                        //删除图片
                        ProductDetailsPictureBP pdpbp = new ProductDetailsPictureBP();
                        pdpbp.DeletePictures(item.Id);
                        //添加图片
                        int sort = 1;
                        if (YpkComDetailInfo.picturesPath != null && YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            for (int i = 0; i < YpkComDetailInfo.picturesPath.Count(); i++)
                            {
                                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                                pic.Name = "商品图片";
                                pic.SubId = userId;
                                pic.SubTime = DateTime.Now;
                                pic.PicturesPath = YpkComDetailInfo.picturesPath[i];
                                pic.CommodityId = Com.Id;
                                pic.Sort = sort;
                                contextSession.SaveObject(pic);
                                sort++;

                            }
                        }
                        #endregion
                        #region 商品分类
                        //删除商品分类
                        var catList = CommodityCategory.ObjectSet().Where(c => c.CommodityId == item.Id).ToList();
                        foreach (var commodityCategory in catList)
                        {
                            contextSession.Delete(commodityCategory);
                        }
                        //添加商品分类
                        var ComCategory = CategoryList.Where(p => JdCom.CategoryName.Contains(p.Name)).ToList();
                        if (ComCategory.Any())
                        {
                            foreach (var itemcate in ComCategory)
                            {
                                CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                                comcate.CategoryId = itemcate.Id;
                                comcate.CommodityId = Com.Id;
                                comcate.SubId = userId;
                                comcate.SubTime = DateTime.Now;
                                comcate.Name = "商品分类";
                                comcate.IsDel = false;
                                #region 本地测试
                                comcate.SubId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                                #endregion
                                //comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
                                contextSession.SaveObject(comcate);
                            }
                        }
                        else
                        {
                            NoCategoryData.Add(JdCom.JDCode);
                        }
                        #endregion
                        #region 商城分类
                        //删除商城分类
                        var oldCCs = CommodityInnerCategory.ObjectSet().Where(c => c.CommodityId == item.Id).ToList();
                        foreach (var commodityCategory in oldCCs)
                        {
                            contextSession.Delete(commodityCategory);
                        }
                        //添加商城分类
                        var innerCateid = categoryList.Where(p => p.Name == JdCom.VideoName.Trim()).FirstOrDefault();
                        if (innerCateid != null)
                        {
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                            comInnerCate.CategoryId = innerCateid.Id;
                            comInnerCate.CommodityId = Com.Id;
                            comInnerCate.SubId = userId;
                            comInnerCate.SubTime = DateTime.Now;
                            comInnerCate.Name = "商品分类";
                            comInnerCate.IsDel = false;
                            comInnerCate.SubId = AppId;
                            comInnerCate.AppId = AppId;
                            comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                            contextSession.SaveObject(comInnerCate);
                        }
                        else
                        {
                            NoPinLeiData.Add(JdCom.JDCode);
                        }
                        #endregion
                        #region 品牌墙
                        //存在该品牌，判断该商品与该品牌是否有关联，没有则创建关联，有关联无操作
                        //不存在创建该品牌，并将该商品与该品牌建立关联 
                        var brand = Brandwall.ObjectSet().FirstOrDefault(_ => _.Brandname == YpkComDetailInfo.brandName && _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                        if (brand != null)
                        {
                            var cominbrand = CommodityInnerBrand.ObjectSet().Where(_ => _.CommodityId == Com.Id).FirstOrDefault();
                            if (cominbrand == null)
                            {
                                #region 添加商品品牌
                                CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                                comBrand.BrandId = brand.Id;
                                comBrand.Name = brand.Brandname;
                                comBrand.CommodityId = Com.Id;
                                comBrand.SubTime = DateTime.Now;
                                comBrand.ModifiedOn = comBrand.SubTime;
                                comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comBrand.CrcAppId = 0;
                                contextSession.SaveObject(comBrand);
                                contextSession.SaveChanges();
                                #endregion
                            }
                        }
                        else
                        {
                            #region 添加品牌
                            brand = Brandwall.CreateBrandwall();
                            brand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            brand.Brandname = YpkComDetailInfo.brandName;
                            brand.BrandLogo = "";
                            brand.Brandstatu = 1;
                            brand.SubTime = DateTime.Now;
                            brand.ModifiedOn = brand.SubTime;
                            contextSession.SaveObject(brand);
                            contextSession.SaveChanges();
                            #endregion
                            #region 添加商品品牌
                            CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                            comBrand.BrandId = brand.Id;
                            comBrand.Name = brand.Brandname;
                            comBrand.CommodityId = Com.Id;
                            comBrand.SubTime = DateTime.Now;
                            comBrand.ModifiedOn = comBrand.SubTime;
                            comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            comBrand.CrcAppId = 0;
                            contextSession.SaveObject(comBrand);
                            contextSession.SaveChanges();
                            #endregion
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        YPKException yPKException = new YPKException();
                        yPKException.SkuId = string.Join(",", NullData.ToArray());
                        yPKException.Remark = "异常信息:" + ex.InnerException + " " + ex.Message;
                        YpkExceptionList.Add(yPKException);
                        existExcepitonCount++;
                    }
                }
                #region 商品中不存在的插入
                var NoExist = ValidData.Where(p => !JdComDTO.RepeatData.Contains(p.JDCode)).ToList();
                //店铺中不存在的商品异常数量
                var noExistExcepitonCount = 0;
                foreach (var item in NoExist)
                {
                    try
                    {
                        var YpkComDetailInfo = YPKComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();

                        var JdCom = JdComList.FirstOrDefault(p => p.JDCode == item.JDCode);
                        var minSortValueQuery = (from m in Commodity.ObjectSet()
                                                 where m.AppId == AppId && m.CommodityType == 0
                                                 select m);
                        int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                        int minSortValue = 2;
                        if (minSort.HasValue)
                        {
                            minSortValue = minSort.Value;
                        }
                        Commodity Com = Commodity.CreateCommodity();
                        //默认“七天无理由退” 
                        Com.IsAssurance = true;
                        Com.IsReturns = false;
                        Com.Isnsupport = false;

                        Com.Id = item.Id;
                        Com.Name = YpkComDetailInfo.name;
                        Com.Code = item.Code;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = item.No_Number ?? 0;
                        Com.Price = item.Price ?? 0;
                        Com.Stock = item.Stock;
                        if (YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            //图片数组第一张为主图
                            Com.PicturesPath = YpkComDetailInfo.picturesPath[0];
                        }
                        if (YpkComDetailInfo.description != null && YpkComDetailInfo.description != "")
                        {
                            string Div = YpkComDetailInfo.description;
                            if (YpkComDetailInfo.description.Length > 10)
                            {
                                Div = YpkComDetailInfo.description.Substring(0, 10);
                            }
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>") + "</div>";
                            }
                            else
                            {
                                Com.Description = YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>");
                            }
                        }
                        else
                        {
                            Com.Description = string.Empty;
                        }
                        Com.State = 1;
                        Com.IsDel = false;
                        Com.AppId = AppId;
                        Com.No_Code = item.No_Code;
                        Com.TotalCollection = 0;
                        Com.TotalReview = 0;
                        Com.Salesvolume = 0;
                        Com.ModifiedOn = DateTime.Now;
                        Com.ComAttribute = item.ComAttribute;
                        Com.CategoryName = item.CategoryName;
                        Com.SortValue = minSortValue - 1;
                        if (FreightTemplateInfo != null)
                            Com.FreightTemplateId = FreightTemplateInfo.Id;
                        Com.MarketPrice = item.MarketPrice;
                        Com.Weight = string.IsNullOrEmpty(YpkComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(YpkComDetailInfo.weight);
                        //Com.SaleService = SNComDetailInfo.wareQD + "<br>" + SNComDetailInfo.shouhou;
                        Com.SaleAreas = item.SaleAreas;
                        Com.SharePercent = item.SharePercent;
                        Com.CommodityType = 0;
                        Com.Duty = item.Duty;
                        Com.TaxRate = item.TaxRate;
                        Com.TaxClassCode = item.TaxClassCode;
                        Com.Unit = string.IsNullOrEmpty(YpkComDetailInfo.unit) ? "件" : YpkComDetailInfo.unit;
                        Com.InputRax = item.InputRax;
                        Com.Barcode = YpkComDetailInfo.barCode;
                        Com.ErQiCode = YpkComDetailInfo.erQiCode;
                        Com.JDCode = item.JDCode;
                        Com.CostPrice = Convert.ToDecimal(YpkComDetailInfo.price);
                        Com.ServiceSettingId = item.ServiceSettingId;
                        Com.TechSpecs = string.Empty;// JdComDetailInfo.prodParams;
                        Com.SaleService = item.SaleService;
                        Com.Type = 0;
                        Com.YJCouponActivityId = "";
                        Com.YJCouponType = "";
                        Com.ModifieId = userId;
                        contextSession.SaveObject(Com);
                        SaveCommodityStock(Com, contextSession);

                        //更新JdCommodity表中状态
                        item.State = 1;
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                        #region 商品图片
                        int sort = 1;
                        if (YpkComDetailInfo.picturesPath != null && YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            for (int i = 0; i < YpkComDetailInfo.picturesPath.Count(); i++)
                            {
                                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                                pic.Name = "商品图片";
                                pic.SubId = userId;
                                pic.SubTime = DateTime.Now;
                                pic.PicturesPath = YpkComDetailInfo.picturesPath[i];
                                pic.CommodityId = Com.Id;
                                pic.Sort = sort;
                                contextSession.SaveObject(pic);
                                sort++;

                            }
                        }
                        #endregion
                        #region 商品分类
                        var ComCategory = CategoryList.Where(p => item.CategoryName.Contains(p.Name)).ToList();
                        if (ComCategory.Any())
                        {
                            foreach (var itemcate in ComCategory)
                            {
                                CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                                comcate.CategoryId = itemcate.Id;
                                comcate.CommodityId = Com.Id;
                                comcate.SubId = userId;
                                comcate.SubTime = DateTime.Now;
                                comcate.Name = "商品分类";
                                comcate.IsDel = false;
                                #region 本地测试
                                comcate.SubId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                                #endregion
                                //comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
                                contextSession.SaveObject(comcate);
                            }
                        }
                        else
                        {
                            NoCategoryData.Add(item.JDCode);
                        }
                        #endregion
                        #region 商城分类
                        var innerCateid = categoryList.Where(p => p.Name == item.VideoName.Trim()).FirstOrDefault();
                        if (innerCateid != null)
                        {
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                            comInnerCate.CategoryId = innerCateid.Id;
                            comInnerCate.CommodityId = Com.Id;
                            comInnerCate.SubId = userId;
                            comInnerCate.SubTime = DateTime.Now;
                            comInnerCate.Name = "商品分类";
                            comInnerCate.IsDel = false;
                            comInnerCate.SubId = AppId;
                            comInnerCate.AppId = AppId;
                            comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                            contextSession.SaveObject(comInnerCate);
                        }
                        else
                        {
                            NoPinLeiData.Add(JdCom.JDCode);
                        }
                        #endregion
                        #region 品牌墙
                        //存在该品牌，判断该商品与该品牌是否有关联，没有则创建关联，有关联无操作
                        //不存在创建该品牌，并将该商品与该品牌建立关联 
                        var brand = Brandwall.ObjectSet().FirstOrDefault(_ => _.Brandname == YpkComDetailInfo.brandName && _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                        if (brand != null)
                        {
                            var cominbrand = CommodityInnerBrand.ObjectSet().Where(_ => _.CommodityId == Com.Id).FirstOrDefault();
                            if (cominbrand == null)
                            {
                                #region 添加商品品牌
                                CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                                comBrand.BrandId = brand.Id;
                                comBrand.Name = brand.Brandname;
                                comBrand.CommodityId = Com.Id;
                                comBrand.SubTime = DateTime.Now;
                                comBrand.ModifiedOn = comBrand.SubTime;
                                comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comBrand.CrcAppId = 0;
                                contextSession.SaveObject(comBrand);
                                #endregion
                            }
                        }
                        else
                        {
                            #region 添加品牌
                            brand = Brandwall.CreateBrandwall();
                            brand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            brand.Brandname = YpkComDetailInfo.brandName;
                            brand.BrandLogo = "";
                            brand.Brandstatu = 1;
                            brand.SubTime = DateTime.Now;
                            brand.ModifiedOn = brand.SubTime;
                            contextSession.SaveObject(brand);
                            #endregion
                            #region 添加商品品牌
                            CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                            comBrand.BrandId = brand.Id;
                            comBrand.Name = brand.Brandname;
                            comBrand.CommodityId = Com.Id;
                            comBrand.SubTime = DateTime.Now;
                            comBrand.ModifiedOn = comBrand.SubTime;
                            comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            comBrand.CrcAppId = 0;
                            contextSession.SaveObject(comBrand);
                            #endregion
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        YPKException yPKException = new YPKException();
                        yPKException.SkuId = item.JDCode;
                        yPKException.Remark = "异常信息:" + ex.InnerException + " " + ex.Message;
                        YpkExceptionList.Add(yPKException);
                        noExistExcepitonCount++;
                    }
                }
                #endregion
                JdComDTO.FaildCount = NullData.Count + existExcepitonCount + noExistExcepitonCount;
                JdComDTO.SuccessCount = totalCount - JdComDTO.FaildCount;
                JdComDTO.ExceptionData = YpkExceptionList;
                int count = contextSession.SaveChanges();
                JdComDTO.NoCategoryData = NoCategoryData;

                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 0, isSuccess = true, Message = "自动同步成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YPKCommodityBP.AutoSyncYPKCommodityInfoExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { ResultCode = 1, isSuccess = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// 全量同步易派客商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoYPKSyncCommodityExt(System.Guid AppId)
        {
            try
            {
                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                Guid userId = this.ContextDTO.LoginUserID;
                List<YPKException> YpkExceptionList = new List<YPKException>();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //取出补全的商品信息
                var JdComList = JdCommodity.ObjectSet().Where(p => p.AppId == AppId && !p.IsDel && p.State == 0).ToList();
                List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
                //要同步的商品总数量
                var totalCount = skuIds.Count;
                //成功商品数
                var successCount = 0;
                //异常商品数
                var ExcepitonCount = 0;
                //获取易派客商品详情 
                List<YPKComDetailDto> YPKComDetailList = new List<YPKComDetailDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    YPKComDetailList.AddRange(YPKSV.GetYPKComDetail(skuIds.Skip(i).Take(99).ToList()));
                }
                var ypkSkuids = YPKComDetailList.Select(y => y.skuId).ToList();
                var NullData = skuIds.Except(ypkSkuids).ToList();
                if (NullData.Count > 0)
                {
                    YPKException yPKException = new YPKException();
                    yPKException.SkuId = string.Join(",", NullData.ToArray());
                    yPKException.Remark = "接口中获取不到商品数据";
                    YpkExceptionList.Add(yPKException);
                }
                //有效商品
                var ValidData = JdComList.Where(j => ypkSkuids.Contains(j.JDCode));
                //获取商城品类
                var InnerCategoryNameList = JdComList.Select(s => s.VideoName.Trim()).ToList();
                //获取运费模板
                var FreightTemplateInfo = FreightTemplate.ObjectSet().FirstOrDefault(p => p.AppId == AppId && p.IsDefault == 1);
                //获取商品分类
                var CategoryNameList = JdComList.Select(s => s.CategoryName).ToList();//获取所有的商品分类               
                string ggg = string.Join(",", CategoryNameList.ToArray());
                List<string> list = new List<string>(ggg.Split(','));
                //易捷北京
                //var CategoryList = Category.ObjectSet().Where(p => p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel && CategoryNameList.Contains(p.Name)).ToList();
                //var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel).ToList();
                //本地调试
                var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && !p.IsDel).ToList();
                var CategoryList = Category.ObjectSet().Where(p => p.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && !p.IsDel && list.Contains(p.Name)).ToList();

                //获取不到商品类目的
                List<string> NoCategoryData = new List<string>();
                //获取不到商城品类的
                List<string> NoPinLeiData = new List<string>();
                //店铺中存在的备注编码进行更新  不存在则插入
                var ExistCommodity = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).ToList();
                JdComDTO.RepeatData = ExistCommodity.Select(s => s.JDCode).ToList();
                //店铺中存在的商品异常数量
                var existExcepitonCount = 0;
                foreach (var item in ExistCommodity)
                {
                    try
                    {
                        var YpkComDetailInfo = YPKComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();

                        var JdCom = JdComList.FirstOrDefault(p => p.JDCode == item.JDCode);
                        var minSortValueQuery = (from m in Commodity.ObjectSet()
                                                 where m.AppId == AppId && m.CommodityType == 0
                                                 select m);
                        int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                        int minSortValue = 2;
                        if (minSort.HasValue)
                        {
                            minSortValue = minSort.Value;
                        }
                        Commodity Com = new Commodity();
                        //默认“七天无理由退”
                        Com.IsAssurance = true;
                        Com.IsReturns = false;
                        Com.Isnsupport = false;

                        Com.Id = item.Id;
                        Com.Name = YpkComDetailInfo.name;
                        Com.Code = JdCom.Code;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = JdCom.No_Number ?? 0;
                        Com.Price = JdCom.Price ?? 0;
                        Com.Stock = JdCom.Stock;
                        if (YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            //图片数组第一张为主图
                            Com.PicturesPath = YpkComDetailInfo.picturesPath[0];
                        }
                        if (YpkComDetailInfo.description != null && YpkComDetailInfo.description != "")
                        {
                            string Div = YpkComDetailInfo.description;
                            if (YpkComDetailInfo.description.Length > 10)
                            {
                                Div = YpkComDetailInfo.description.Substring(0, 10);
                            }
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>") + "</div>";
                            }
                            else
                            {
                                Com.Description = YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>");
                            }
                        }
                        else
                        {
                            Com.Description = string.Empty;
                        }
                        //Com.State = 1; 只更新商品信息,不更新商品上下架状态
                        Com.IsDel = false;
                        Com.AppId = JdCom.AppId;
                        Com.No_Code = JdCom.No_Code;
                        Com.TotalCollection = 0;
                        Com.TotalReview = 0;
                        Com.Salesvolume = 0;
                        Com.ModifiedOn = DateTime.Now;
                        Com.ComAttribute = JdCom.ComAttribute;
                        Com.CategoryName = JdCom.CategoryName;
                        Com.SortValue = minSortValue - 1;
                        if (FreightTemplateInfo != null)
                            Com.FreightTemplateId = FreightTemplateInfo.Id;
                        Com.MarketPrice = JdCom.MarketPrice;
                        Com.Weight = string.IsNullOrEmpty(YpkComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(YpkComDetailInfo.weight);
                        //Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                        Com.SaleAreas = JdCom.SaleAreas;
                        Com.SharePercent = JdCom.SharePercent;
                        Com.CommodityType = 0;
                        Com.Duty = JdCom.Duty;
                        Com.TaxRate = JdCom.TaxRate;
                        Com.TaxClassCode = JdCom.TaxClassCode;
                        Com.Unit = string.IsNullOrEmpty(YpkComDetailInfo.unit) ? "件" : YpkComDetailInfo.unit;
                        Com.InputRax = JdCom.InputRax;
                        Com.Barcode = YpkComDetailInfo.barCode;
                        Com.ErQiCode = YpkComDetailInfo.erQiCode;
                        Com.JDCode = JdCom.JDCode;
                        Com.CostPrice = Convert.ToDecimal(YpkComDetailInfo.price);
                        Com.ServiceSettingId = JdCom.ServiceSettingId;
                        Com.TechSpecs = string.Empty;
                        Com.SaleService = JdCom.SaleService;
                        Com.Type = 0;
                        Com.YJCouponActivityId = "";
                        Com.YJCouponType = "";
                        Com.ModifieId = userId;
                        Com.EntityState = EntityState.Modified;
                        contextSession.SaveObject(Com);
                        Com.RefreshCache(EntityState.Modified);
                        //更新库存表
                        UpdateCommodityStock(Com, contextSession);
                        int count1 = contextSession.SaveChanges();
                        //更新JdCommodity表中状态
                        JdCom.State = 1;
                        //JdCom.EntityState = EntityState.Modified;
                        contextSession.SaveObject(JdCom);
                        #region 商品图片
                        //删除图片
                        ProductDetailsPictureBP pdpbp = new ProductDetailsPictureBP();
                        pdpbp.DeletePictures(item.Id);
                        //添加图片
                        int sort = 1;
                        if (YpkComDetailInfo.picturesPath != null && YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            for (int i = 0; i < YpkComDetailInfo.picturesPath.Count(); i++)
                            {
                                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                                pic.Name = "商品图片";
                                pic.SubId = userId;
                                pic.SubTime = DateTime.Now;
                                pic.PicturesPath = YpkComDetailInfo.picturesPath[i];
                                pic.CommodityId = Com.Id;
                                pic.Sort = sort;
                                contextSession.SaveObject(pic);
                                sort++;

                            }
                        }
                        #endregion
                        #region 商品分类
                        //删除商品分类
                        var catList = CommodityCategory.ObjectSet().Where(c => c.CommodityId == item.Id).ToList();
                        foreach (var commodityCategory in catList)
                        {
                            contextSession.Delete(commodityCategory);
                        }
                        //添加商品分类
                        var ComCategory = CategoryList.Where(p => JdCom.CategoryName.Contains(p.Name)).ToList();
                        if (ComCategory.Any())
                        {
                            foreach (var itemcate in ComCategory)
                            {
                                CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                                comcate.CategoryId = itemcate.Id;
                                comcate.CommodityId = Com.Id;
                                comcate.SubId = userId;
                                comcate.SubTime = DateTime.Now;
                                comcate.Name = "商品分类";
                                comcate.IsDel = false;
                                #region 本地测试
                                comcate.SubId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                                #endregion
                                //comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
                                contextSession.SaveObject(comcate);
                            }
                        }
                        else
                        {
                            NoCategoryData.Add(JdCom.JDCode);
                        }
                        #endregion
                        #region 商城分类
                        //删除商城分类
                        var oldCCs = CommodityInnerCategory.ObjectSet().Where(c => c.CommodityId == item.Id).ToList();
                        foreach (var commodityCategory in oldCCs)
                        {
                            contextSession.Delete(commodityCategory);
                        }
                        //添加商城分类
                        var innerCateid = categoryList.Where(p => p.Name == JdCom.VideoName.Trim()).FirstOrDefault();
                        if (innerCateid != null)
                        {
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                            comInnerCate.CategoryId = innerCateid.Id;
                            comInnerCate.CommodityId = Com.Id;
                            comInnerCate.SubId = userId;
                            comInnerCate.SubTime = DateTime.Now;
                            comInnerCate.Name = "商品分类";
                            comInnerCate.IsDel = false;
                            comInnerCate.SubId = AppId;
                            comInnerCate.AppId = AppId;
                            comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                            contextSession.SaveObject(comInnerCate);
                        }
                        else
                        {
                            NoPinLeiData.Add(JdCom.JDCode);
                        }
                        #endregion
                        #region 品牌墙
                        //存在该品牌，判断该商品与该品牌是否有关联，没有则创建关联，有关联无操作
                        //不存在创建该品牌，并将该商品与该品牌建立关联 
                        var brand = Brandwall.ObjectSet().FirstOrDefault(_ => _.Brandname == YpkComDetailInfo.brandName && _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                        if (brand != null)
                        {
                            var cominbrand = CommodityInnerBrand.ObjectSet().Where(_ => _.CommodityId == Com.Id).FirstOrDefault();
                            if (cominbrand == null)
                            {
                                #region 添加商品品牌
                                CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                                comBrand.BrandId = brand.Id;
                                comBrand.Name = brand.Brandname;
                                comBrand.CommodityId = Com.Id;
                                comBrand.SubTime = DateTime.Now;
                                comBrand.ModifiedOn = comBrand.SubTime;
                                comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comBrand.CrcAppId = 0;
                                contextSession.SaveObject(comBrand);
                                #endregion
                            }
                        }
                        else
                        {
                            #region 添加品牌
                            brand = Brandwall.CreateBrandwall();
                            brand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            brand.Brandname = YpkComDetailInfo.brandName;
                            brand.BrandLogo = "";
                            brand.Brandstatu = 1;
                            brand.SubTime = DateTime.Now;
                            brand.ModifiedOn = brand.SubTime;
                            contextSession.SaveObject(brand);
                            #endregion
                            #region 添加商品品牌
                            CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                            comBrand.BrandId = brand.Id;
                            comBrand.Name = brand.Brandname;
                            comBrand.CommodityId = Com.Id;
                            comBrand.SubTime = DateTime.Now;
                            comBrand.ModifiedOn = comBrand.SubTime;
                            comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            comBrand.CrcAppId = 0;
                            contextSession.SaveObject(comBrand);
                            #endregion
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        YPKException yPKException = new YPKException();
                        yPKException.SkuId = string.Join(",", NullData.ToArray());
                        yPKException.Remark = "异常信息:" + ex.InnerException + " " + ex.Message;
                        YpkExceptionList.Add(yPKException);
                        existExcepitonCount++;
                    }
                }
                #region 商品中不存在的插入
                var NoExist = ValidData.Where(p => !JdComDTO.RepeatData.Contains(p.JDCode)).ToList();
                //店铺中不存在的商品异常数量
                var noExistExcepitonCount = 0;
                foreach (var item in NoExist)
                {
                    try
                    {
                        var YpkComDetailInfo = YPKComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();

                        var JdCom = JdComList.FirstOrDefault(p => p.JDCode == item.JDCode);
                        var minSortValueQuery = (from m in Commodity.ObjectSet()
                                                 where m.AppId == AppId && m.CommodityType == 0
                                                 select m);
                        int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                        int minSortValue = 2;
                        if (minSort.HasValue)
                        {
                            minSortValue = minSort.Value;
                        }
                        Commodity Com = Commodity.CreateCommodity();
                        //默认“七天无理由退” 
                        Com.IsAssurance = true;
                        Com.IsReturns = false;
                        Com.Isnsupport = false;

                        Com.Id = item.Id;
                        Com.Name = YpkComDetailInfo.name;
                        Com.Code = item.Code;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = item.No_Number ?? 0;
                        Com.Price = item.Price ?? 0;
                        Com.Stock = item.Stock;
                        if (YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            //图片数组第一张为主图
                            Com.PicturesPath = YpkComDetailInfo.picturesPath[0];
                        }
                        if (YpkComDetailInfo.description != null && YpkComDetailInfo.description != "")
                        {
                            string Div = YpkComDetailInfo.description;
                            if (YpkComDetailInfo.description.Length > 10)
                            {
                                Div = YpkComDetailInfo.description.Substring(0, 10);
                            }
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>") + "</div>";
                            }
                            else
                            {
                                Com.Description = YpkComDetailInfo.description.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>");
                            }
                        }
                        else
                        {
                            Com.Description = string.Empty;
                        }
                        Com.State = 1;
                        Com.IsDel = false;
                        Com.AppId = AppId;
                        Com.No_Code = item.No_Code;
                        Com.TotalCollection = 0;
                        Com.TotalReview = 0;
                        Com.Salesvolume = 0;
                        Com.ModifiedOn = DateTime.Now;
                        Com.ComAttribute = item.ComAttribute;
                        Com.CategoryName = item.CategoryName;
                        Com.SortValue = minSortValue - 1;
                        if (FreightTemplateInfo != null)
                            Com.FreightTemplateId = FreightTemplateInfo.Id;
                        Com.MarketPrice = item.MarketPrice;
                        Com.Weight = string.IsNullOrEmpty(YpkComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(YpkComDetailInfo.weight);
                        //Com.SaleService = SNComDetailInfo.wareQD + "<br>" + SNComDetailInfo.shouhou;
                        Com.SaleAreas = item.SaleAreas;
                        Com.SharePercent = item.SharePercent;
                        Com.CommodityType = 0;
                        Com.Duty = item.Duty;
                        Com.TaxRate = item.TaxRate;
                        Com.TaxClassCode = item.TaxClassCode;
                        Com.Unit = string.IsNullOrEmpty(YpkComDetailInfo.unit) ? "件" : YpkComDetailInfo.unit;
                        Com.InputRax = item.InputRax;
                        Com.Barcode = YpkComDetailInfo.barCode;
                        Com.ErQiCode = YpkComDetailInfo.erQiCode;
                        Com.JDCode = item.JDCode;
                        Com.CostPrice = Convert.ToDecimal(YpkComDetailInfo.price);
                        Com.ServiceSettingId = item.ServiceSettingId;
                        Com.TechSpecs = string.Empty;
                        Com.SaleService = item.SaleService;
                        Com.Type = 0;
                        Com.YJCouponActivityId = "";
                        Com.YJCouponType = "";
                        Com.ModifieId = userId;
                        contextSession.SaveObject(Com);
                        SaveCommodityStock(Com, contextSession);

                        //更新JdCommodity表中状态
                        item.State = 1;
                        //item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                        #region 商品图片
                        int sort = 1;
                        if (YpkComDetailInfo.picturesPath != null && YpkComDetailInfo.picturesPath.Count() > 0)
                        {
                            for (int i = 0; i < YpkComDetailInfo.picturesPath.Count(); i++)
                            {
                                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                                pic.Name = "商品图片";
                                pic.SubId = userId;
                                pic.SubTime = DateTime.Now;
                                pic.PicturesPath = YpkComDetailInfo.picturesPath[i];
                                pic.CommodityId = Com.Id;
                                pic.Sort = sort;
                                contextSession.SaveObject(pic);
                                sort++;

                            }
                        }
                        #endregion
                        #region 商品分类
                        var ComCategory = CategoryList.Where(p => item.CategoryName.Contains(p.Name)).ToList();
                        if (ComCategory.Any())
                        {
                            foreach (var itemcate in ComCategory)
                            {
                                CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                                comcate.CategoryId = itemcate.Id;
                                comcate.CommodityId = Com.Id;
                                comcate.SubId = userId;
                                comcate.SubTime = DateTime.Now;
                                comcate.Name = "商品分类";
                                comcate.IsDel = false;
                                #region 本地测试
                                comcate.SubId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                                #endregion
                                //comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
                                contextSession.SaveObject(comcate);
                            }
                        }
                        else
                        {
                            NoCategoryData.Add(item.JDCode);
                        }
                        #endregion
                        #region 商城分类
                        var innerCateid = categoryList.Where(p => p.Name == item.VideoName.Trim()).FirstOrDefault();
                        if (innerCateid != null)
                        {
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                            comInnerCate.CategoryId = innerCateid.Id;
                            comInnerCate.CommodityId = Com.Id;
                            comInnerCate.SubId = userId;
                            comInnerCate.SubTime = DateTime.Now;
                            comInnerCate.Name = "商品分类";
                            comInnerCate.IsDel = false;
                            comInnerCate.SubId = AppId;
                            comInnerCate.AppId = AppId;
                            comInnerCate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                            contextSession.SaveObject(comInnerCate);
                        }
                        else
                        {
                            NoPinLeiData.Add(JdCom.JDCode);
                        }
                        #endregion
                        #region 品牌墙
                        //存在该品牌，判断该商品与该品牌是否有关联，没有则创建关联，有关联无操作
                        //不存在创建该品牌，并将该商品与该品牌建立关联 
                        var brand = Brandwall.ObjectSet().FirstOrDefault(_ => _.Brandname == YpkComDetailInfo.brandName && _.AppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId);
                        if (brand != null)
                        {
                            var cominbrand = CommodityInnerBrand.ObjectSet().Where(_ => _.CommodityId == Com.Id).FirstOrDefault();
                            if (cominbrand == null)
                            {
                                #region 添加商品品牌
                                CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                                comBrand.BrandId = brand.Id;
                                comBrand.Name = brand.Brandname;
                                comBrand.CommodityId = Com.Id;
                                comBrand.SubTime = DateTime.Now;
                                comBrand.ModifiedOn = comBrand.SubTime;
                                comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                                comBrand.CrcAppId = 0;
                                contextSession.SaveObject(comBrand);
                                #endregion
                            }
                        }
                        else
                        {
                            #region 添加品牌
                            brand = Brandwall.CreateBrandwall();
                            brand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            brand.Brandname = YpkComDetailInfo.brandName;
                            brand.BrandLogo = "";
                            brand.Brandstatu = 1;
                            brand.SubTime = DateTime.Now;
                            brand.ModifiedOn = brand.SubTime;
                            contextSession.SaveObject(brand);
                            #endregion
                            #region 添加商品品牌
                            CommodityInnerBrand comBrand = CommodityInnerBrand.CreateCommodityInnerBrand();
                            comBrand.BrandId = brand.Id;
                            comBrand.Name = brand.Brandname;
                            comBrand.CommodityId = Com.Id;
                            comBrand.SubTime = DateTime.Now;
                            comBrand.ModifiedOn = comBrand.SubTime;
                            comBrand.AppId = Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId;
                            comBrand.CrcAppId = 0;
                            contextSession.SaveObject(comBrand);
                            #endregion
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        YPKException yPKException = new YPKException();
                        yPKException.SkuId = item.JDCode;
                        yPKException.Remark = "异常信息:" + ex.InnerException + " " + ex.Message;
                        YpkExceptionList.Add(yPKException);
                        noExistExcepitonCount++;
                    }
                }
                #endregion
                JdComDTO.FaildCount = NullData.Count + existExcepitonCount + noExistExcepitonCount;
                JdComDTO.SuccessCount = totalCount - JdComDTO.FaildCount;
                JdComDTO.ExceptionData = YpkExceptionList;
                int count = contextSession.SaveChanges();
                JdComDTO.NoCategoryData = NoCategoryData;

                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 0, isSuccess = true, Message = "自动同步成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YPKCommodityBP.AutoYPKSyncCommodityExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { ResultCode = 1, isSuccess = false, Message = ex.Message };
            }

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
        /// <summary>
        /// 检查易派客编码是否存在
        /// </summary>
        /// <param name="JdCode">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public bool IsExistsSNCodeExt(string JdCode, System.Guid appId)
        {
            bool bReturn = false;
            if (!string.IsNullOrWhiteSpace(JdCode))
            {
                var jdcommodity = JdCommodity.ObjectSet().Where(p => p.JDCode == JdCode && p.AppId == appId && !p.IsDel).FirstOrDefault();
                bReturn = (null == jdcommodity);
            }
            return bReturn;
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
                LogHelper.Error(string.Format("SNCommodityBP.SaveCommodityStock 保存库存异常。"), ex);
            }
        }
        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void UpdateCommodityStock(Commodity item, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {
                var cs = CommodityStock.ObjectSet().FirstOrDefault(p => p.Id == item.Id);
                cs.ComAttribute = "[]";
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
                cs.EntityState = EntityState.Modified;
                contextSession.SaveObject(cs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNCommodityBP.UpdateCommodityStock 更新库存异常。"), ex);
            }
        }
    }
}