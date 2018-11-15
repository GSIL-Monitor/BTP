
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/4/12 11:05:52
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.TPS;
using System.Data;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class JdCommodityBP : BaseBP, IJdCommodity
    {
        /// <summary>
        /// 查询列表信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> GetJdCommodityListExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            try
            {
                var query = JdCommodity.ObjectSet().Where(p => p.AppId == input.AppId && p.State == 0 && p.IsDel == false).AsQueryable();

                if (!string.IsNullOrWhiteSpace(input.JDCode))
                {
                    query = query.Where(p => p.JDCode == input.JDCode);
                }

                var count = query.Count();
                var data = (from n in query
                            select new Jinher.AMP.BTP.Deploy.JdCommodityDTO
                            {
                                Id = n.Id,
                                JDCode = n.JDCode,
                                Barcode = n.Barcode,
                                TaxClassCode = n.TaxClassCode,//税收编码
                                TaxRate = n.TaxRate,//销项税
                                InputRax = n.InputRax,//进项税
                                CategoryName = n.CategoryName,
                                VideoName = n.VideoName,
                                SubTime = n.SubTime,
                                Price = n.Price //商品售价（易派客）
                            }).OrderByDescending(q => q.SubTime).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList();
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>>
                {
                    isSuccess = true,
                    Data = new ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO> { List = data, Count = count }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdCommodityBP.GetJdCommodityListExt 异常", ex);
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> { isSuccess = false, Data = null };
            }

        }
        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddJdCommodityExt(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
        {
            try
            {
                if (input == null)
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "参数不能为空~" };
                }
                if (!string.IsNullOrEmpty(input.JDCode) && !IsExistsJdCodeExt(input.JDCode, input.AppId))
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "列表中已存在该备注编码~" };
                }
                Guid userId = this.ContextDTO.LoginUserID;
                //获取协议价格和京东价格
                List<string> skuIds = new List<string>();
                skuIds.Add(input.JDCode);
                List<JdPriceDto> jdPrices = new List<JdPriceDto>();
                jdPrices.AddRange(JDSV.GetPrice(skuIds));
                if (!jdPrices.Any())
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "备注编码在京东商品池中不存在" };
                }
                var JdPriceinfo = jdPrices.FirstOrDefault(p => p.SkuId == input.JDCode);
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                JdCom.Price = JdPriceinfo.JdPrice;
                JdCom.CostPrice = JdPriceinfo.Price;
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
                LogHelper.Error("JdCommodityBP.AddJdCommodityExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "添加成功" };
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
        /// 检查京东编码是否存在
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public bool IsExistsJdCodeExt(string JdCode, System.Guid appId)
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
        /// 获取商品详情
        /// </summary>
        public Jinher.AMP.BTP.Deploy.JdCommodityDTO GetJdCommodityInfoExt(System.Guid Id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 批量删除商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelJdCommodityAllExt(System.Collections.Generic.List<System.Guid> Ids)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var JdComList = JdCommodity.ObjectSet().Where(p => Ids.Contains(p.Id)).ToList();
                foreach (var item in JdComList)
                {
                    item.EntityState = EntityState.Deleted;
                    contextSession.SaveObject(item);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdCommodityBP.DelJdCommodityAllExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "删除失败" };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "删除成功" };
        }
        /// <summary>
        /// 导出商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> ExportJdCommodityDataExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
        {
            var query = JdCommodity.ObjectSet().Where(p => p.AppId == input.AppId && p.State == 0 && p.IsDel == false).AsQueryable();

            if (!string.IsNullOrWhiteSpace(input.JDCode))
            {
                query = query.Where(p => p.JDCode == input.JDCode);
            }
            var count = query.Count();
            var data = (from n in query
                        select new Jinher.AMP.BTP.Deploy.JdCommodityDTO
                        {
                            Id = n.Id,
                            JDCode = n.JDCode,
                            CategoryName = n.CategoryName,
                            TaxClassCode = n.TaxClassCode,//税收编码
                            TaxRate = n.TaxRate,//销项税
                            InputRax = n.InputRax,//进项税
                            VideoName = n.VideoName,
                            SubTime = n.SubTime,
                            Price=n.Price //售价（易派客）
                        }).OrderByDescending(q => q.SubTime).ToList();
            return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>>
            {
                isSuccess = true,
                Data = new ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO> { List = data, Count = count }
            };
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportJdCommodityDataExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, Guid AppId)
        {
            try
            {
                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
                List<JdPriceDto> jdPrices = new List<JdPriceDto>();
                for (int i = 0; i < skuIds.Count; i += 99)
                {
                    jdPrices.AddRange(JDSV.GetPrice(skuIds.Skip(i).Take(99).ToList()));
                }
                //京东商品池不存在的SKUid
                JdComDTO.InvalidData = skuIds.Except(jdPrices.Select(s => s.SkuId)).ToList();
                //店铺中已存在的京东编码
                JdComDTO.RepeatData = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).Select(s => s.JDCode).ToList();
                //京东商品表中已存在的备注编码
                JdComDTO.JdRepeatData = JdCommodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId && p.State != 1).Select(s => s.JDCode).ToList();
                if (JdComDTO.InvalidData.Any() || JdComDTO.JdRepeatData.Any())
                {
                    return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 1, isSuccess = false, Message = "存在重复备注编码和无效备注编码,请核对后再导入~" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid userId = this.ContextDTO.LoginUserID;
                //获取不到商品类目的
                List<string> NoCategoryData = new List<string>();
                foreach (var input in JdComList)
                {
                    var jdPrice = jdPrices.Where(p => p.SkuId == input.JDCode).FirstOrDefault();
                    JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                    JdCom.CostPrice = jdPrice.Price;
                    JdCom.Price = jdPrice.JdPrice;
                    JdCom.Barcode = input.JDCode;
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
                LogHelper.Error("YJEmployeeBP.AddYJEmployeeExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { isSuccess = false, ResultCode = 2, Message = ex.Message };
            }
        }
        /// <summary>
        /// 自动同步商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncCommodityInfoExt(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {
            try
            {
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appModel =
                    APPSV.Instance.GetAppOwnerInfo(AppId);
                bool isFnull = true;
                if (appModel.OwnerType == 0)
                {
                    CBC.Deploy.CustomDTO.OrgInfoNewDTO orgInfoDTO = CBCSV.Instance.GetOrgInfoNewBySubId(appModel.OwnerId);
                    if (orgInfoDTO == null || string.IsNullOrEmpty(orgInfoDTO.CompanyPhone))
                    {
                        isFnull = false;
                    }
                }
                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                Guid userId = this.ContextDTO.LoginUserID;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                if (isFnull)
                {
                    //取出补全的商品信息
                    var JdComList = JdCommodity.ObjectSet().Where(p => p.AppId == AppId && Ids.Contains(p.Id)).ToList();
                    List<string> skuIds = JdComList.Select(s => s.JDCode).ToList();
                    List<JdComPicturesDto> JdComPics = new List<JdComPicturesDto>();
                    for (int i = 0; i < skuIds.Count; i += 99)
                    {
                        JdComPics.AddRange(JDSV.GetComPictures(skuIds.Skip(i).Take(99).ToList()));
                    }
                    //获取京东商品详情
                    List<JdComDetailDto> JdComDetailList = new List<JdComDetailDto>();
                    foreach (var item in skuIds)
                    {
                        var JdComDetailInfo = JDSV.GetJdDetail(item);
                        JdComDetailList.Add(JdComDetailInfo);
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
                    var CategoryList = Category.ObjectSet().Where(p => p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel && CategoryNameList.Contains(p.Name)).ToList();
                    var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel).ToList();
                    //本地调试
                    //var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == AppId && !p.IsDel).ToList();
                    //var CategoryList = Category.ObjectSet().Where(p => p.AppId == AppId && !p.IsDel && list.Distinct().Contains(p.Name)).ToList();

                    //获取不到商品类目的
                    List<string> NoCategoryData = new List<string>();
                    //店铺中存在的备注编码进行更新  不存在则插入
                    var ExistCommodity = Commodity.ObjectSet().Where(p => skuIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).ToList();
                    JdComDTO.RepeatData = ExistCommodity.Select(s => s.JDCode).ToList();

                    foreach (var item in ExistCommodity)
                    {
                        var JdCom = JdComList.FirstOrDefault(p => p.JDCode == item.JDCode);
                        var JdComPic = JdComPics.Where(p => p.skuId == item.JDCode);
                        var JdComDetailInfo = JdComDetailList.Where(p => p.sku == item.JDCode).FirstOrDefault();

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
                        Com.Id = item.Id;
                        Com.Name = JdComDetailInfo.name;
                        Com.Code = JdCom.Code;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = JdCom.No_Number ?? 0;
                        Com.Price = JdCom.Price ?? 0;
                        Com.Stock = JdCom.Stock;
                        Com.PicturesPath = "http://img13.360buyimg.com/n1/" + JdComPic.FirstOrDefault(p => p.isPrimary == "1").path;
                        if (JdComDetailInfo.appintroduce != null && JdComDetailInfo.appintroduce != "")
                        {
                            string Div = JdComDetailInfo.appintroduce.Substring(0, 10);
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.appintroduce.Replace("\'", "\"").Replace("\'", "\"") + "</div>";
                            }
                            else
                            {
                                Com.Description = JdComDetailInfo.appintroduce.Replace("\'", "\"").Replace("\'", "\"");
                            }
                        }
                        else
                        {
                            string Div = JdComDetailInfo.introduction.Substring(0, 10);
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"") + "</div>";
                            }
                            else
                            {
                                Com.Description = JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"");
                            }
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
                        Com.FreightTemplateId = FreightTemplateInfo.Id;  //99元以下商品8元运费
                        Com.MarketPrice = JdCom.MarketPrice;
                        Com.Weight = JdComDetailInfo.weight;
                        Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
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
                        Com.TechSpecs = JdComDetailInfo.param;
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
                        foreach (var itempic in JdComPic)
                        {
                            ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                            pic.Name = "商品图片";
                            pic.SubId = userId;
                            pic.SubTime = DateTime.Now;
                            pic.PicturesPath = "http://img13.360buyimg.com/n1/" + itempic.path;
                            pic.CommodityId = Com.Id;
                            pic.Sort = Convert.ToInt32(itempic.orderSort);
                            contextSession.SaveObject(pic);
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
                                //comcate.SubId = AppId;
                                //comcate.AppId = AppId;
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                                #endregion
                                comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
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
                        var JdComPic = JdComPics.Where(p => p.skuId == item.JDCode);
                        var JdComDetailInfo = JdComDetailList.Where(p => p.sku == item.JDCode).FirstOrDefault();

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
                        Com.Id = item.Id;
                        Com.Name = JdComDetailInfo.name;
                        Com.Code = item.Code;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = item.No_Number ?? 0;
                        Com.Price = item.Price ?? 0;
                        Com.Stock = item.Stock;
                        Com.PicturesPath = "http://img13.360buyimg.com/n1/" + JdComPic.FirstOrDefault(p => p.isPrimary == "1").path;
                        if (JdComDetailInfo.appintroduce != null && JdComDetailInfo.appintroduce != "")
                        {
                            string Div = JdComDetailInfo.appintroduce.Substring(0, 10);
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.appintroduce.Replace("\'", "\"").Replace("\'", "\"") + "</div>";
                            }
                            else
                            {
                                Com.Description = JdComDetailInfo.appintroduce.Replace("\'", "\"").Replace("\'", "\"");
                            }
                        }
                        else
                        {
                            string Div = JdComDetailInfo.introduction.Substring(0, 10);
                            if (Div.Contains("<div"))
                            {
                                Com.Description = "<div class=\"JD-goods\">" + JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"") + "</div>";
                            }
                            else
                            {
                                Com.Description = JdComDetailInfo.introduction.Replace("\'", "\"").Replace("\'", "\"");
                            }
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
                        Com.FreightTemplateId = FreightTemplateInfo.Id;  //99元以下商品8元运费
                        Com.MarketPrice = item.MarketPrice;
                        Com.Weight = JdComDetailInfo.weight;
                        Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                        Com.SaleAreas = item.SaleAreas;
                        Com.SharePercent = item.SharePercent;
                        Com.CommodityType = 0;
                        Com.Duty = item.Duty;
                        Com.TaxRate = item.TaxRate;
                        Com.TaxClassCode = item.TaxClassCode;
                        Com.Unit =string.IsNullOrEmpty(JdComDetailInfo.saleUnit)? "件" : JdComDetailInfo.saleUnit;
                        Com.InputRax = item.InputRax;
                        Com.Barcode = JdComDetailInfo.upc;
                        Com.JDCode = item.JDCode;
                        Com.CostPrice = item.CostPrice;
                        Com.ServiceSettingId = item.ServiceSettingId;
                        Com.TechSpecs = JdComDetailInfo.param;
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
                        foreach (var itempic in JdComPic)
                        {
                            ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                            pic.Name = "商品图片";
                            pic.SubId = userId;
                            pic.SubTime = DateTime.Now;
                            pic.PicturesPath = "http://img13.360buyimg.com/n1/" + itempic.path;
                            pic.CommodityId = Com.Id;
                            pic.Sort = Convert.ToInt32(itempic.orderSort);
                            contextSession.SaveObject(pic);
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
                                //comcate.SubId = AppId;
                                //comcate.AppId = AppId;
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                                #endregion
                                comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
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
                }
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 0, isSuccess = true, Message = "自动同步成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.AutoSyncCommodityInfoExt 异常", ex);
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
                LogHelper.Error(string.Format("商品发布服务保存库存异常。SaveCommodityStock"), ex);
            }
        }
        /// <summary>
        /// 保存库存
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
                LogHelper.Error(string.Format("商品发布服务保存库存异常。SaveCommodityStock"), ex);
            }
        }
        /// <summary>
        /// 获取商城品类
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.InnerCategoryDTO> GetCategoriesExt(System.Guid AppId)
        {
            var ParentIds = InnerCategory.ObjectSet().Where(p => p.ParentId != Guid.Empty && p.AppId == AppId && p.IsDel == false).Select(s => s.ParentId).ToList();
            var query = InnerCategory.ObjectSet().Where(p => !ParentIds.Contains(p.Id) && p.AppId == AppId && p.IsDel == false).Select(s => new InnerCategoryDTO { Id = s.Id, Name = s.Name }).ToList();
            return query;
        }
        /// <summary>
        /// 获取商城类目
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryListExt(System.Guid AppId)
        {
            var ParentIds = Category.ObjectSet().Where(p => p.ParentId != Guid.Empty && p.AppId == AppId && p.IsDel == false).Select(s => s.ParentId).ToList();
            var query = Category.ObjectSet().Where(p => !ParentIds.Contains(p.Id) && p.AppId == AppId && p.IsDel == false).Select(s => new CategoryDTO { Id = s.Id, Name = s.Name }).ToList();
            return query;
        }
    }
}