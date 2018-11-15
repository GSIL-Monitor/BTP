
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/7/9 18:37:31
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
    public partial class SNCommodityBP : BaseBP, ISNCommodity
    {

        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddSNCommodityExt(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
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
                //获取协议价格和京东价格
                List<string> skuIds = new List<string>();
                skuIds.Add(input.JDCode);
                List<SNPriceDto> snPrices = new List<SNPriceDto>();
                snPrices.AddRange(SuningSV.GetPrice(skuIds));
                if (!snPrices.Any())
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "备注编码在苏宁易购商品池中不存在" };
                }
                var snPrice = snPrices.Where(p => p.skuId == input.JDCode).FirstOrDefault();
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
                JdCom.CostPrice = string.IsNullOrEmpty(snPrice.price) ? Decimal.Zero : Convert.ToDecimal(snPrice.price);
                JdCom.Price = string.IsNullOrEmpty(snPrice.snPrice) ? Decimal.Zero : Convert.ToDecimal(snPrice.snPrice);
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
        /// 同步商品列表图片
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateComPicExt()
        {
            ResultDTO result = new ResultDTO();
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //取出苏宁商品信息
                Guid Appid = new Guid("D59968DB-FD26-4447-ACB2-D80B08F1157F");
                var CommodityList = Commodity.ObjectSet().Where(p => p.AppId == Appid && p.IsDel == false && p.State == 0).ToList();
                LogHelper.Info(string.Format("取出商品条数:{0}", CommodityList.Count()));
                int CommodityCount = CommodityList.Count();
                int count = 0;
                for (int i = 0; i < CommodityCount; i += 100)
                {
                    var IdList = CommodityList.Select(s => s.Id).Skip(i).Take(100).ToList();//取出商品id
                    LogHelper.Info(string.Format("取出商品id:{0}", JsonHelper.JsonSerializer(IdList)));

                    var productDetailsPictures = ProductDetailsPicture.ObjectSet().Where(n => IdList.Contains(n.CommodityId)).ToList();//取出图片信息
                    LogHelper.Info(string.Format("取出图片条数:{0}", productDetailsPictures.Count()));
                    foreach (var item in CommodityList)
                    {
                        var PicPath = productDetailsPictures.FirstOrDefault(p => p.CommodityId == item.Id && p.Sort == 3);
                        if (PicPath != null)
                        {
                            item.PicturesPath = PicPath.PicturesPath;
                        }
                    }
                    count += contextSession.SaveChanges();
                    LogHelper.Info(string.Format("保存条数:{0}", count));
                }
                if (count > 0)
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, ResultCode = 0, Message = "保存成功" + count + "条" };
                }
                else
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, ResultCode = 1, Message = "保存失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNCommodityBP.UpdateComPicExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
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
        /// 检查苏宁易购编码是否存在
        /// </summary>
        /// <param name="code">编号</param>
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
        /// 导入苏宁商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportSNCommodityDataExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            try
            {
                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
                List<SNPriceDto> jdPrices = new List<SNPriceDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    jdPrices.AddRange(SuningSV.GetPrice(skuIds.Skip(i).Take(30).ToList()));
                }
                //List<string> categoryIds = new List<string>();
                //categoryIds = SuningSV.GetCategory();
                List<string> SNSkuIds = new List<string>();
                SNSkuIds.AddRange(jdPrices.Select(_ => _.skuId.ToString()).ToList());
                //foreach (var item in categoryIds)
                //{
                //    SNSkuIds.AddRange(SuningSV.GetProdPool(item));
                //}
                //价格为空的商品
                List<string> nullPricesSkuList = new List<string>();
                foreach (var price in jdPrices)
                {
                    if (string.IsNullOrEmpty(price.price) || string.IsNullOrEmpty(price.snPrice))
                    {
                        nullPricesSkuList.Add(price.skuId);
                    }
                }
                //11-1-lhx 税率为空的商品
                List<string> nullTaxSkuList = new List<string>();
                foreach (var price in jdPrices)
                {
                    if (string.IsNullOrEmpty(price.tax))
                    {
                        nullTaxSkuList.Add(price.skuId);
                    }
                }
                //苏宁易购商品池不存在的SKUid
                JdComDTO.InvalidData = skuIds.Except(SNSkuIds).ToList();
                //店铺中已存在的苏宁易购编码
                JdComDTO.RepeatData = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).Select(s => s.JDCode).ToList();
                //苏宁易购商品表中已存在的备注编码
                JdComDTO.JdRepeatData = JdCommodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId && p.State != 1).Select(s => s.JDCode).ToList();
                if (JdComDTO.InvalidData.Any() || JdComDTO.JdRepeatData.Any())
                {
                    return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 1, isSuccess = false, Message = "存在重复备注编码和无效备注编码,请核对后再导入~" };
                }
                if (nullPricesSkuList.Count > 0)
                {
                    JdComDTO.NullPriceData = nullPricesSkuList;
                    //return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 1, isSuccess = false, Message = "此备注编码下的商品价格为空，请核对后再导入~" };
                }
                //11-1-lhx
                if (nullTaxSkuList.Count > 0)
                {
                    JdComDTO.NullTaxData = nullTaxSkuList;
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid userId = this.ContextDTO.LoginUserID;
                //获取不到商品类目的
                List<string> NoCategoryData = new List<string>();
                foreach (var input in JdComList)
                {
                    var jdPrice = jdPrices.Where(p => p.skuId == input.JDCode).FirstOrDefault();
                    //价格为空时继续循环其余商品 11-1-lhx税率为空继续循环其余商品
                    if (string.IsNullOrEmpty(jdPrice.price) || string.IsNullOrEmpty(jdPrice.snPrice) || string.IsNullOrEmpty(jdPrice.tax))
                        continue;
                    JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                    JdCom.CostPrice = string.IsNullOrEmpty(jdPrice.price) ? Decimal.Zero : Convert.ToDecimal(jdPrice.price);
                    JdCom.Price = string.IsNullOrEmpty(jdPrice.snPrice) ? Decimal.Zero : Convert.ToDecimal(jdPrice.snPrice);
                    JdCom.Barcode = input.JDCode;
                    JdCom.TaxRate = Convert.ToDecimal(jdPrice.tax) * 100;
                    JdCom.InputRax = Convert.ToDecimal(jdPrice.tax) * 100;
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
                int count = contextSession.SaveChanges();
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 0, isSuccess = true, Message = "导入成功" };

            }
            catch (Exception ex)
            {
                LogHelper.Error("SNCommodityBP.ImportSNCommodityDataExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { isSuccess = false, ResultCode = 2, Message = ex.Message };
            }
        }
        /// <summary>
        /// 自动同步苏宁商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncSNCommodityInfoExt(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            try
            {

                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                Guid userId = this.ContextDTO.LoginUserID;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                //取出补全的商品信息
                var JdComList = JdCommodity.ObjectSet().Where(p => p.AppId == AppId && Ids.Contains(p.Id)).ToList();
                List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
                List<SNComPicturesDto> SNComPics = new List<SNComPicturesDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    SNComPics.AddRange(SuningSV.GetComPictures(skuIds.Skip(i).Take(30).ToList()));
                }
                //商品价格
                List<SNPriceDto> SNPrices = new List<SNPriceDto>();
                for (int i = 0; i < skuIds.Count; i += 30)
                {
                    SNPrices.AddRange(SuningSV.GetPrice(skuIds.Skip(i).Take(30).ToList()));
                }
                //商品扩展信息
                List<SNComExtendDto> SNComExtendList = new List<SNComExtendDto>();
                SNComExtendList = SuningSV.GetComExtend(SNPrices);
                //获取苏宁易购商品详情
                List<SNComDetailDto> SNComDetailList = new List<SNComDetailDto>();
                foreach (var item in skuIds)
                {
                    var SNComDetailInfo = SuningSV.GetSNDetail(item);
                    if (string.IsNullOrEmpty(SNComDetailInfo.name))
                    {
                        return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { ResultCode = 1, isSuccess = false, Message = item + "商品详情为空，请检查！" };
                    }
                    SNComDetailList.Add(SNComDetailInfo);
                }
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
                //店铺中存在的备注编码进行更新  不存在则插入
                var ExistCommodity = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).ToList();
                JdComDTO.RepeatData = ExistCommodity.Select(s => s.JDCode).ToList();

                foreach (var item in ExistCommodity)
                {
                    var JdCom = JdComList.FirstOrDefault(p => p.JDCode == item.JDCode);
                    var JdComPic = SNComPics.Where(p => p.skuId == item.JDCode);
                    var JdComDetailInfo = SNComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();

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
                    //售后信息
                    var SNComExtendInfo = SNComExtendList.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                    if (SNComExtendInfo != null)
                    {
                        if (SNComExtendInfo.returnGoods == "01")
                        {
                            Com.IsAssurance = true;
                            Com.IsReturns = false;
                            Com.Isnsupport = false;
                        }
                        if (SNComExtendInfo.returnGoods == "02")
                        {
                            Com.IsAssurance = false;
                            Com.IsReturns = true;
                            Com.Isnsupport = true;
                        }
                    }
                    Com.Id = item.Id;
                    Com.Name = JdComDetailInfo.name.Replace("苏宁", "").Replace("超市", "").Replace("易购", "").Replace("自营", "").Replace("【", "").Replace("】", "").Replace("[", "").Replace("]", "");
                    Com.Code = JdCom.Code;
                    Com.SubTime = DateTime.Now;
                    Com.SubId = userId;
                    Com.No_Number = JdCom.No_Number ?? 0;
                    Com.Price = JdCom.Price ?? 0;
                    Com.Stock = JdCom.Stock;
                    if (JdComPic.Count() > 2)
                    {
                        //如果图片大于两张，取第三张
                        Com.PicturesPath = JdComPic.ToList()[2].path;// JdComPic.FirstOrDefault(p => p.primary == "1").path;
                    }
                    if (JdComDetailInfo.introduction != null && JdComDetailInfo.introduction != "")
                    {
                        string Div = JdComDetailInfo.introduction;
                        if (JdComDetailInfo.introduction.Length > 10)
                        {
                            Div = JdComDetailInfo.introduction.Substring(0, 10);
                        }
                        if (Div.Contains("<div"))
                        {
                            Com.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>") + "</div>";
                        }
                        else
                        {
                            Com.Description = JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>");
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
                        Com.FreightTemplateId = FreightTemplateInfo.Id;  //30元以下商品8元运费
                    Com.MarketPrice = JdCom.MarketPrice;
                    Com.Weight = string.IsNullOrEmpty(JdComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(JdComDetailInfo.weight);
                    //Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                    Com.SaleAreas = JdCom.SaleAreas;
                    Com.SharePercent = JdCom.SharePercent;
                    Com.CommodityType = 0;
                    Com.Duty = JdCom.Duty;
                    Com.TaxRate = JdCom.TaxRate;
                    Com.TaxClassCode = JdCom.TaxClassCode;
                    Com.Unit = string.IsNullOrEmpty(JdComDetailInfo.saleUnit) ? "件" : JdComDetailInfo.saleUnit;
                    Com.InputRax = JdCom.InputRax;
                    Com.Barcode = JdComDetailInfo.upc;
                    Com.JDCode = JdCom.JDCode;
                    Com.CostPrice = JdCom.CostPrice;
                    Com.ServiceSettingId = JdCom.ServiceSettingId;
                    Com.TechSpecs = string.Empty;// JdComDetailInfo.prodParams;
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
                    #region 商品图片
                    //删除图片
                    ProductDetailsPictureBP pdpbp = new ProductDetailsPictureBP();
                    pdpbp.DeletePictures(item.Id);
                    //添加图片
                    int sort = 1;
                    foreach (var itempic in JdComPic)
                    {
                        ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                        pic.Name = "商品图片";
                        pic.SubId = userId;
                        pic.SubTime = DateTime.Now;
                        pic.PicturesPath = itempic.path;
                        pic.CommodityId = Com.Id;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);
                        sort++;
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
                    #endregion
                }
                #region 商品中不存在的插入
                var NoExist = JdComList.Where(p => !JdComDTO.RepeatData.Contains(p.JDCode)).ToList();
                foreach (var item in NoExist)
                {
                    var SNComPic = SNComPics.Where(p => p.skuId == item.JDCode);
                    var SNComDetailInfo = SNComDetailList.Where(p => p.skuId == item.JDCode).FirstOrDefault();

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
                    //售后信息
                    var SNComExtendInfo = SNComExtendList.Where(_ => _.skuId == item.JDCode).FirstOrDefault();
                    if (SNComExtendInfo != null)
                    {
                        if (SNComExtendInfo.returnGoods == "01")
                        {
                            Com.IsAssurance = true;
                            Com.IsReturns = false;
                            Com.Isnsupport = false;
                        }
                        if (SNComExtendInfo.returnGoods == "02")
                        {
                            Com.IsAssurance = false;
                            Com.IsReturns = true;
                            Com.Isnsupport = true;
                        }
                    }
                    Com.Id = item.Id;
                    Com.Name = SNComDetailInfo.name.Replace("苏宁", "").Replace("超市", "").Replace("易购", "").Replace("自营", "").Replace("【", "").Replace("】", "").Replace("[", "").Replace("]", "");
                    Com.Code = item.Code;
                    Com.SubTime = DateTime.Now;
                    Com.SubId = userId;
                    Com.No_Number = item.No_Number ?? 0;
                    Com.Price = item.Price ?? 0;
                    Com.Stock = item.Stock;
                    if (SNComPic.Count() > 2)
                    {
                        //如果图片大于两张，取第三张
                        Com.PicturesPath = SNComPic.ToList()[2].path;// SNComPic.FirstOrDefault(p => p.primary == "1").path;
                    }
                    if (SNComDetailInfo.introduction != null && SNComDetailInfo.introduction != "")
                    {
                        string Div = SNComDetailInfo.introduction;
                        if (SNComDetailInfo.introduction.Length > 10)
                        {
                            Div = SNComDetailInfo.introduction.Substring(0, 10);
                        }
                        if (Div.Contains("<div"))
                        {
                            Com.Description = "<div class=\"JD-goods\">" + SNComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>") + "</div>";
                        }
                        else
                        {
                            Com.Description = SNComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"").Replace("<a>", "<span>").Replace("</a>", "</span>");
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
                        Com.FreightTemplateId = FreightTemplateInfo.Id;  //30元以下商品8元运费
                    Com.MarketPrice = item.MarketPrice;
                    Com.Weight = string.IsNullOrEmpty(SNComDetailInfo.weight) ? Decimal.Zero : Convert.ToDecimal(SNComDetailInfo.weight);
                    //Com.SaleService = SNComDetailInfo.wareQD + "<br>" + SNComDetailInfo.shouhou;
                    Com.SaleAreas = item.SaleAreas;
                    Com.SharePercent = item.SharePercent;
                    Com.CommodityType = 0;
                    Com.Duty = item.Duty;
                    Com.TaxRate = item.TaxRate;
                    Com.TaxClassCode = item.TaxClassCode;
                    Com.Unit = string.IsNullOrEmpty(SNComDetailInfo.saleUnit) ? "件" : SNComDetailInfo.saleUnit;
                    Com.InputRax = item.InputRax;
                    Com.Barcode = SNComDetailInfo.upc;
                    Com.JDCode = item.JDCode;
                    Com.CostPrice = item.CostPrice;
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
                    #region 商品图片
                    int sort = 1;
                    foreach (var itempic in SNComPic)
                    {
                        ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                        pic.Name = "商品图片";
                        pic.SubId = userId;
                        pic.SubTime = DateTime.Now;
                        pic.PicturesPath = itempic.path;
                        pic.CommodityId = Com.Id;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);
                        sort++;
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
                    #endregion
                }
                #endregion

                int count = contextSession.SaveChanges();
                JdComDTO.NoCategoryData = NoCategoryData;

                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 0, isSuccess = true, Message = "自动同步成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNCommodityBP.AutoSyncCommodityInfoExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { ResultCode = 1, isSuccess = false, Message = ex.Message };

            }
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
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNCommodityBP.UpdateCommodityStock 更新库存异常。"), ex);
            }
        }
        /// <summary>
        /// 导出苏宁进货价差异商品列表
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.SNComCostDiffDTO> GetSNDiffCostPriceExt()
        {
            List<Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.SNComCostDiffDTO> SNComList = new List<Deploy.CustomDTO.Commodity.SNComCostDiffDTO>();
            try
            {
                Guid appId = new Guid("D59968DB-FD26-4447-ACB2-D80B08F1157F");
                var SNCom = Commodity.ObjectSet().Where(p => p.AppId == appId && p.IsDel == false).
                    Select(s => new Jinher.AMP.BTP.Deploy.CustomDTO.Commodity.SNComCostDiffDTO { Id = s.Id, Name = s.Name, JdCode = s.JDCode, CostPrice = s.CostPrice, state = s.State }).ToList();
                LogHelper.Info(string.Format("获取苏宁商品条数:{0}", SNCom.Count()));
                var JdCodeList = SNCom.Select(s => s.JdCode).ToList();

                List<SNPriceDto> jdPrices = new List<SNPriceDto>();
                for (int i = 0; i < JdCodeList.Count; i += 30)
                {
                    jdPrices.AddRange(SuningSV.GetPrice(JdCodeList.Skip(i).Take(30).ToList()));
                }
                LogHelper.Info(string.Format("获取苏宁商品对比价格Begining"));
                foreach (var item in SNCom)
                {

                    var NewPriceInfo = jdPrices.FirstOrDefault(p => p.skuId == item.JdCode);
                    if (NewPriceInfo != null)
                    {
                        if (Convert.ToDecimal(NewPriceInfo.price) != item.CostPrice)
                        {

                            item.SNCostprice = Convert.ToDecimal(NewPriceInfo.price);
                            SNComList.Add(item);
                        }
                    }
                    else
                    {
                        item.SNCostprice = 999999999;
                        SNComList.Add(item);
                    }
                }
                LogHelper.Info(string.Format("获取苏宁价格差异条数:{0}", SNComList.Count()));
                return SNComList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNCommodityBP.GetSNDiffCostPriceExt。导出苏宁进货价差异商品列表服务异常"), ex);
                return SNComList;
            }
        }
        /// <summary>
        /// 全量同步苏宁进货价
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNCostPriceExt()
        {
            ResultDTO result = new ResultDTO() { isSuccess = false, ResultCode = 1 };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid appId = new Guid("D59968DB-FD26-4447-ACB2-D80B08F1157F");
                var SNComList = Commodity.ObjectSet().Where(p => p.AppId == appId && p.IsDel == false).OrderBy(p => p.SubTime).AsQueryable();
                int count = 0;
                int ciout1 = 0;
                for (int j = 0; j < SNComList.Count(); j += 1000)
                {
                    List<Commodity> SNCom = new List<Commodity>();
                    SNCom = SNComList.Skip(j).Take(1000).ToList();
                    var JdCodeList = SNCom.Select(s => s.JDCode).ToList();
                    List<SNPriceDto> jdPrices = new List<SNPriceDto>();
                    for (int i = 0; i < JdCodeList.Count; i += 30)
                    {
                        jdPrices.AddRange(SuningSV.GetPrice(JdCodeList.Skip(i).Take(30).ToList()));
                    }
                    LogHelper.Info(string.Format("获取苏宁商品对比价格Begining"));
                    for (int i = 0; i < SNCom.Count; i += 100)
                    {
                        var SnComList = SNCom.Skip(i).Take(100).ToList();//取出商品id
                        var ComIds = SnComList.Select(s => s.Id);
                        var SNComStock = CommodityStock.ObjectSet().Where(p => ComIds.Contains(p.CommodityId)).ToList();
                        foreach (var item in SnComList)
                        {
                            var NewPriceInfo = jdPrices.FirstOrDefault(p => p.skuId == item.JDCode);
                            LogHelper.Info(string.Format("获取苏宁商品价格:{0}", JsonHelper.JsonSerializer(NewPriceInfo)));
                            if (NewPriceInfo != null && !string.IsNullOrEmpty(NewPriceInfo.snPrice) && !string.IsNullOrEmpty(NewPriceInfo.price))
                            {
                                if (Convert.ToDecimal(NewPriceInfo.snPrice) > 0 && Convert.ToDecimal(NewPriceInfo.price) > 0)
                                {
                                    ciout1 += 1;
                                    LogHelper.Info(string.Format("获取苏宁商品上架条数;{0}", ciout1));
                                    var ComStockInfo = SNComStock.FirstOrDefault(p => p.CommodityId == item.Id);
                                    item.Price = Convert.ToDecimal(NewPriceInfo.snPrice);
                                    item.CostPrice = Convert.ToDecimal(NewPriceInfo.price);
                                    item.State = 0;
                                    item.ModifiedOn = DateTime.Now;
                                    if (ComStockInfo != null)
                                    {
                                        ComStockInfo.Price = Convert.ToDecimal(NewPriceInfo.snPrice);
                                        ComStockInfo.CostPrice = Convert.ToDecimal(NewPriceInfo.price);
                                        ComStockInfo.ModifiedOn = DateTime.Now;
                                    }
                                }
                                else if ((Convert.ToDecimal(NewPriceInfo.snPrice) <= 0 || Convert.ToDecimal(NewPriceInfo.price) <= 0) && item.State == 0)
                                {
                                    LogHelper.Info(string.Format("获取苏宁商品价格小于0下架:{0},{1}", item.Id, JsonHelper.JsonSerializer(NewPriceInfo)));
                                    item.State = 1;
                                    item.ModifiedOn = DateTime.Now;
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                LogHelper.Info(string.Format("苏宁下架商品:{0},{1}", item.Id, JsonHelper.JsonSerializer(NewPriceInfo)));
                                item.State = 1;
                                item.ModifiedOn = DateTime.Now;
                            }
                        }
                        count += contextSession.SaveChanges();
                        LogHelper.Info(string.Format("获取苏宁价格保存条数:{0}", count));
                    }
                }
                if (count > 0)
                {
                    return new ResultDTO() { isSuccess = true, ResultCode = count, Message = "全量同步苏宁价格成功" };
                }
                else
                {
                    return new ResultDTO() { isSuccess = false, ResultCode = 1, Message = "全量同步苏宁价格失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNCommodityBP.SynSNCostPriceExt。全量同步苏宁价格服务异常"), ex);
                return result;
            }
        }
        /// <summary>
        /// 苏宁对账
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNBillExt()
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                //取出所有苏宁订单信息
                var SNOrder = SNBill.ObjectSet().Where(p => p.YjOrderId == null).OrderBy(p => p.SNSubtime).AsQueryable();
                LogHelper.Info(string.Format("取出SNOrder表数据条数:{0}", SNOrder.Count()));
                int count = 0;
                for (int i = 0; i < SNOrder.Count(); i += 100)
                {
                    List<SNBill> SNBillList = new List<SNBill>();
                    SNBillList = SNOrder.Skip(i).Take(100).ToList();
                    LogHelper.Info(string.Format("取出SNBillList数据条数:{0}", SNBillList.Count()));
                    List<string> CustomOrderId = new List<string>();
                    CustomOrderId = SNBillList.Select(s => s.CustomOrderId).ToList();
                    LogHelper.Info(string.Format("取出CustomOrderId条数:{0}", CustomOrderId.Count()));
                    List<SNOrderItem> SNOrderItems = SNOrderItem.ObjectSet().Where(p => CustomOrderId.Contains(p.CustomOrderId)).ToList();
                    List<Guid> YJOrderId = SNOrderItems.Select(s => s.OrderId).ToList();
                    //根据订单ID 取出订单及明细
                    Guid Appid = new Guid("D59968DB-FD26-4447-ACB2-D80B08F1157F");
                    var YJOrder = CommodityOrder.ObjectSet().Where(p => p.AppId == Appid && YJOrderId.Contains(p.Id)).ToList();
                    var OrderItemDetal = OrderItem.ObjectSet().Where(p => YJOrderId.Contains(p.CommodityOrderId)).ToList();
                    LogHelper.Info(string.Format("苏宁对账1"));
                    foreach (var item in SNBillList)
                    {
                        var SnOrderItem = SNOrderItems.Where(p => p.CustomOrderId == item.CustomOrderId);
                        if (!SnOrderItem.Any())
                        {
                            continue;
                        }
                        foreach (var ite in SnOrderItem)
                        {
                            LogHelper.Info(string.Format("苏宁对账2"));
                            item.SNState += ite.Status + "~";
                            item.SnWuLiuState += ite.ExpressStatus + "~";
                        }

                        LogHelper.Info(string.Format("苏宁对账OrderId:{0}", SnOrderItem.FirstOrDefault().OrderId));
                        var YJOrderInfo = YJOrder.FirstOrDefault(p => p.Id == SnOrderItem.FirstOrDefault().OrderId);
                        item.YjOrderId = YJOrderInfo.Id;
                        item.OrderCode = YJOrderInfo.Code;
                        item.YJFreight = YJOrderInfo.Freight;
                        if (item.YJFreight == item.SNFreight)
                        {
                            item.IsEqualFreight = "运费相等";
                        }
                        else
                        {
                            item.IsEqualFreight = "不相等";
                        }
                        item.State = YJOrderInfo.State;
                        var YJOrderItem = OrderItemDetal.Where(p => p.CommodityOrderId == YJOrderInfo.Id);
                        LogHelper.Info(string.Format("苏宁对账3"));
                        Decimal OrderMoney = 0;
                        foreach (var it in YJOrderItem)
                        {
                            LogHelper.Info(string.Format("苏宁对账4"));
                            item.OrderDetail += "商品名称:" + it.Name + "进货价:" + it.CostPrice + "购买数量:" + it.Number + "====";
                            OrderMoney += (it.CostPrice ?? 0) * it.Number;
                        }
                        item.YjOrderTotal = OrderMoney;

                        if (item.YjOrderTotal.HasValue && item.YjOrderTotal == item.SNOrderTotalMoney)
                        {
                            LogHelper.Info(string.Format("苏宁订单价格0:{0},易捷订单价格:{1}", item.SNOrderTotalMoney, item.YjOrderTotal));
                            item.IsEqualMoney = "订单总价相等";
                        }
                        else
                        {
                            LogHelper.Info(string.Format("苏宁订单价格1:{0},易捷订单价格:{1}", item.SNOrderTotalMoney, item.YjOrderTotal));
                            item.IsEqualMoney = "订单总价不等";
                        }

                        if (item.IsEqualMoney == "订单总价相等" && item.IsEqualFreight == "运费相等")
                        {
                            item.IsHandle = 0;
                        }
                        else
                        {
                            item.IsHandle = 1;
                        }
                        LogHelper.Info(string.Format("苏宁对账5"));
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                    }
                    count += contextSession.SaveChanges();
                }
                if (count > 0)
                {
                    return new ResultDTO() { isSuccess = true, ResultCode = count, Message = "成功" };
                }
                else
                {
                    return new ResultDTO() { isSuccess = false, ResultCode = count, Message = "失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNCommodityBP.SynSNBillExt。苏宁对账服务异常"), ex);
                return new ResultDTO() { isSuccess = false, ResultCode = 1, Message = ex.ToString() };
            }
        }
        /// <summary>
        /// 苏宁对账2
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SynSNBill2Ext()
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            try
            {
                //取出所有苏宁订单信息
                var SNOrder = SNBill.ObjectSet().Where(p => p.YjOrderId == null).OrderBy(p => p.SNSubtime).AsQueryable();
                LogHelper.Info(string.Format("取出SNOrder表数据条数2:{0}", SNOrder.Count()));
                int count = 0;
                for (int i = 0; i < SNOrder.Count(); i += 100)
                {
                    List<SNBill> SNBillList = new List<SNBill>();
                    SNBillList = SNOrder.Skip(i).Take(100).ToList();
                    LogHelper.Info(string.Format("取出SNBillList数据条数2:{0}", SNBillList.Count()));
                    List<Guid?> OrderIds = new List<Guid?>();
                    OrderIds = SNBillList.Select(s => s.OrderId).ToList();
                    LogHelper.Info(string.Format("取出OrderId条数2:{0}", OrderIds.Count()));
                    List<SNOrderItem> SNOrderItems = SNOrderItem.ObjectSet().Where(p => OrderIds.Contains(p.OrderId)).ToList();
                    //根据订单ID 取出订单及明细
                    Guid Appid = new Guid("D59968DB-FD26-4447-ACB2-D80B08F1157F");
                    var YJOrder = CommodityOrder.ObjectSet().Where(p => p.AppId == Appid && OrderIds.Contains(p.Id)).ToList();
                    var OrderItemDetal = OrderItem.ObjectSet().Where(p => OrderIds.Contains(p.CommodityOrderId)).ToList();
                    LogHelper.Info(string.Format("苏宁对账12"));
                    foreach (var item in SNBillList)
                    {
                        var SnOrderItem = SNOrderItems.Where(p => p.OrderId == item.OrderId);
                        if (!SnOrderItem.Any())
                        {
                            foreach (var ite in SnOrderItem)
                            {
                                LogHelper.Info(string.Format("苏宁对账22"));
                                item.SNState += ite.Status + "~";
                                item.SnWuLiuState += ite.ExpressStatus + "~";
                            }
                        }
                        var YJOrderInfo = YJOrder.FirstOrDefault(p => p.Id == item.OrderId);
                        if (YJOrderInfo == null)
                        {
                            LogHelper.Info(string.Format("易捷查询不到的苏宁订单OrderId:{0}", item.OrderId));
                            continue;
                        }
                        item.YjOrderId = YJOrderInfo.Id;
                        item.OrderCode = YJOrderInfo.Code;
                        item.YJFreight = YJOrderInfo.Freight;
                        if (item.YJFreight == item.SNFreight)
                        {
                            item.IsEqualFreight = "运费相等";
                        }
                        else
                        {
                            item.IsEqualFreight = "不相等";
                        }
                        item.State = YJOrderInfo.State;
                        var YJOrderItem = OrderItemDetal.Where(p => p.CommodityOrderId == YJOrderInfo.Id);
                        LogHelper.Info(string.Format("苏宁对账32"));
                        Decimal OrderMoney = 0;
                        foreach (var it in YJOrderItem)
                        {
                            LogHelper.Info(string.Format("苏宁对账42"));
                            item.OrderDetail += "商品名称:" + it.Name + "进货价:" + it.CostPrice + "购买数量:" + it.Number + "====";
                            OrderMoney += (it.CostPrice ?? 0) * it.Number;
                        }
                        item.YjOrderTotal = OrderMoney;

                        if (item.YjOrderTotal.HasValue && item.YjOrderTotal == item.SNOrderTotalMoney)
                        {
                            LogHelper.Info(string.Format("苏宁订单价格0:{0},易捷订单价格:{1}", item.SNOrderTotalMoney, item.YjOrderTotal));
                            item.IsEqualMoney = "订单总价相等";
                        }
                        else
                        {
                            LogHelper.Info(string.Format("苏宁订单价格12:{0},易捷订单价格:{1}", item.SNOrderTotalMoney, item.YjOrderTotal));
                            item.IsEqualMoney = "订单总价不等";
                        }

                        if (item.IsEqualMoney == "订单总价相等" && item.IsEqualFreight == "运费相等")
                        {
                            item.IsHandle = 0;
                        }
                        else
                        {
                            item.IsHandle = 1;
                        }
                        LogHelper.Info(string.Format("苏宁对账52"));
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                    }
                    count += contextSession.SaveChanges();
                }
                if (count > 0)
                {
                    return new ResultDTO() { isSuccess = true, ResultCode = count, Message = "成功" };
                }
                else
                {
                    return new ResultDTO() { isSuccess = false, ResultCode = count, Message = "失败" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNCommodityBP.SynSNBillExt。苏宁对账服务异常"), ex);
                return new ResultDTO() { isSuccess = false, ResultCode = 1, Message = ex.ToString() };
            }
        }
    }
}