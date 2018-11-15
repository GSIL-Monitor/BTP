
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/5/18 16:44:50
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
using System.Data;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using NPOI.Util;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class YXCommodityBP : BaseBP, IYXCommodity
    {
        /// <summary>
        /// 新建商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYXCommodityExt(Jinher.AMP.BTP.Deploy.JdCommodityDTO input)
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

                var SPUIdLis = YXSV.GetAllSPU();
                if (SPUIdLis.IndexOf(input.Code)>0)
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { ResultCode = 2, isSuccess = false, Message = "备注编码在严选商品池中不存在" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                JdCom.TaxRate = input.TaxRate;
                JdCom.InputRax = input.InputRax;
                JdCom.TaxClassCode = input.TaxClassCode;                
                JdCom.State = 0; //是否补全 
                JdCom.SubId = userId;
                JdCom.SubTime = DateTime.Now;
                JdCom.AppId = input.AppId;
                JdCom.JDCode = input.JDCode;
                JdCom.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,640000,520000,130000,630000";//出去港澳台 新疆 西藏                
                JdCom.No_Code = JdCom.JDCode + "0000";
                JdCom.ErQiCode = input.ErQiCode;
                JdCom.CostPrice = input.CostPrice;
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
                LogHelper.Error("YXCommodityBP.AddYXCommodityExt 异常", ex);
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
        /// 检查严选编码是否存在
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public bool IsExistsJdCodeExt(string JDCode, System.Guid appId)
        {
            bool bReturn = false;
            if (!string.IsNullOrWhiteSpace(JDCode))
            {
                var YXCom = JdCommodity.ObjectSet().Where(p => p.JDCode == JDCode && p.AppId == appId && !p.IsDel&&p.State==0).FirstOrDefault();
                bReturn = (null == YXCom);
            }
            return bReturn;
        }
        /// <summary>
        /// 批量删除商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelYXCommodityAllExt(System.Collections.Generic.List<System.Guid> Ids)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var YXComList = JdCommodity.ObjectSet().Where(p => Ids.Contains(p.Id)).ToList();
                foreach (var item in YXComList)
                {
                    item.EntityState = EntityState.Deleted;
                    contextSession.SaveObject(item);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YXCommodityBP.DelYXCommodityAllExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "删除失败" };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "删除成功" };
        }
        /// <summary>
        /// 导出商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>> ExportYXCommodityDataExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO input)
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
                            SubTime = n.SubTime
                        }).OrderByDescending(q => q.SubTime).ToList();
            return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO>>
            {
                isSuccess = true,
                Data = new ListResult<Jinher.AMP.BTP.Deploy.JdCommodityDTO> { List = data, Count = count }
            };
        }
        /// <summary>
        /// 导入严选商品信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> ImportYXCommodityDataExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JdCommodityDTO> JdComList, System.Guid AppId)
        {
            try
            {
                JdCommoditySearchDTO JdComDTO = new JdCommoditySearchDTO();
                var SPUIdLis = YXSV.GetAllSPU();
                List<string> SPUIds = JdComList.Select(s => s.JDCode).ToList();
                //严选商品池不存在的SKUid
                JdComDTO.InvalidData = SPUIds.Except(SPUIdLis).ToList();
                //店铺中已存在的严选
                JdComDTO.RepeatData = Commodity.ObjectSet().Where(p => SPUIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId).Select(s => s.JDCode).ToList();
                //严选商品表中已存在的备注编码
                JdComDTO.JdRepeatData = JdCommodity.ObjectSet().Where(p => SPUIds.Contains(p.JDCode) && !p.IsDel && p.AppId == AppId && p.State != 1).Select(s => s.JDCode).ToList();
                if (JdComDTO.InvalidData.Any() || JdComDTO.JdRepeatData.Any())
                {
                    return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 1, isSuccess = false, Message = "存在重复备注编码和无效备注编码,请核对后再导入~" };
                }
                //获取运费模板
                var FreightTemplateInfo = FreightTemplate.ObjectSet().FirstOrDefault(p => p.AppId == AppId && p.IsDefault == 1);
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid userId = this.ContextDTO.LoginUserID;
                //获取不到商品类目的
                List<string> NoCategoryData = new List<string>();
                foreach (var input in JdComList)
                {
                    JdCommodity JdCom = JdCommodity.CreateJdCommodity();
                    JdCom.Barcode = input.JDCode;
                    JdCom.TaxRate = input.TaxRate;
                    JdCom.InputRax = input.InputRax;
                    JdCom.TaxClassCode = input.TaxClassCode;
                    JdCom.State = 0; //是否补全
                    JdCom.SubId = userId;
                    JdCom.SubTime = DateTime.Now;
                    JdCom.AppId = input.AppId;
                    JdCom.JDCode = input.JDCode;
                    JdCom.FreightTemplateId = FreightTemplateInfo.Id;
                    JdCom.SaleAreas = "430000,220000,420000,210000,310000,120000,140000,410000,320000,340000,350000,510000,440000,450000,500000,370000,530000,460000,610000,110000,230000,360000,620000,330000,640000,520000,130000,630000";//出去港澳台 新疆 西藏                
                    JdCom.No_Code = input.JDCode;
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
                LogHelper.Error("YXCommodityBP.ImportYXCommodityDataExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { isSuccess = false, ResultCode = 2, Message = ex.Message };
            }
        }
        /// <summary>
        /// 自动同步严选商品信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> AutoSyncYXCommodityInfoExt(System.Guid AppId, System.Collections.Generic.List<System.Guid> Ids)
        {

            try
            {
                Jinher.AMP.App.Deploy.CustomDTO.AppIdOwnerIdTypeDTO appModel = APPSV.Instance.GetAppOwnerInfo(AppId);
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
                    var YXComList = JdCommodity.ObjectSet().Where(p => p.AppId == AppId && Ids.Contains(p.Id)).ToList();
                    List<string> skuIds = YXComList.Select(s => s.JDCode).ToList();
                    List<YXComDetailDTO> YXComDetailList = new List<YXComDetailDTO>();

                    for (int i = 0; i < skuIds.Count; i += 30)
                    {
                        YXComDetailList.AddRange(YXSV.GetComDetailList(skuIds.Skip(i).Take(30).ToList()));
                    }
                    if (!YXComDetailList.Any())
                    {
                        return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = JdComDTO, ResultCode = 1, isSuccess = false, Message = "未获取到严选商品信息" };
                    }
                    #region 获取所有skuid库存 处理严选商品属性

                    List<StockDTO> YXstockNumList = new List<StockDTO>(); //库存信息
                    List<itemSkuSpecValueList> skuAttr = new List<itemSkuSpecValueList>();//商品属性

                    if (YXComDetailList.Any())
                    {
                        List<string> Sku = new List<string>();
                        foreach (var item in YXComDetailList)
                        {
                            Sku.AddRange(item.skuList.Select(s => s.id).ToList());
                            foreach (var it in item.skuList)
                            {
                                skuAttr.AddRange(it.itemSkuSpecValueList);
                            }
                        }
                        //批量获取严选库存信息
                        for (int i = 0; i < Sku.Count; i += 90)
                        {
                            YXstockNumList.AddRange(YXSV.GetStockNum(Sku.Skip(i).Take(90).ToList()));
                            //Thread.Sleep(1000);
                        }
                        //写入严选商品属性
                        List<ComAttribute> ComAttr = new List<ComAttribute>();
                        foreach (var item in skuAttr)
                        {
                            ComAttribute Com = new ComAttribute();
                            Com.AttrName = item.skuSpec.name;
                            Com.Attrvalue = item.skuSpecValue.value;
                            ComAttr.Add(Com);
                        }
                        var firstAttributeName = ComAttr.Select(s => s.AttrName).ToList();
                        //写入不存在的商品属性
                        List<string> colorAndSize = new List<string>() { "颜色", "尺寸" };
                        var NoExistAttr = firstAttributeName.Except(Jinher.AMP.BTP.BE.Attribute.ObjectSet().Where(p => p.AppId == AppId && p.IsDel == false).Select(s => s.Name).ToList().Union(colorAndSize)).ToList();
                        if (NoExistAttr.Any())
                        {
                            foreach (var item in NoExistAttr)
                            {

                                AttributeDTO attrDTO = new AttributeDTO();
                                attrDTO.Id = Guid.NewGuid();
                                attrDTO.Name = item;
                                attrDTO.AppId = AppId;
                                attrDTO.EntityState = System.Data.EntityState.Added;
                                Jinher.AMP.BTP.BE.Attribute attribute = new Jinher.AMP.BTP.BE.Attribute().FromEntityData(attrDTO);
                                attribute.SubId = userId;
                                attribute.SubTime = DateTime.Now;
                                contextSession.SaveObject(attribute);
                            }
                        }
                    }
                    #endregion
                    #region 获取所有的商城分类  商品类目
                    //获取商城品类
                    var InnerCategoryNameList = YXComList.Select(s => s.VideoName.Trim()).ToList();

                    //获取商品分类
                    var CategoryNameList = YXComList.Select(s => s.CategoryName).ToList();//获取所有的商品分类               
                    string ggg = string.Join(",", CategoryNameList.ToArray());
                    List<string> list = new List<string>(ggg.Split(','));
                    //易捷北京
                    var CategoryList = Category.ObjectSet().Where(p => p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel && CategoryNameList.Contains(p.Name)).ToList();
                    var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6") && !p.IsDel).ToList();
                    //本地调试
                    //var categoryList = InnerCategory.ObjectSet().Where(p => InnerCategoryNameList.Contains(p.Name) && p.AppId == AppId && !p.IsDel).ToList();
                    //var CategoryList = Category.ObjectSet().Where(p => p.AppId == AppId && !p.IsDel && list.Distinct().Contains(p.Name)).ToList();
                    #endregion

                    //获取不到商品类目的
                    List<string> NoCategoryData = new List<string>();
                    //店铺中存在的备注编码进行更新  不存在则插入
                    var ExistCommodity = Commodity.ObjectSet().Where(p => skuIds.Contains(p.Barcode) && !p.IsDel && p.AppId == AppId).ToList();
                    JdComDTO.RepeatData = ExistCommodity.Select(s => s.Barcode).ToList();

                    foreach (var item in ExistCommodity)
                    {
                        var YXCom = YXComList.FirstOrDefault(p => p.JDCode == item.Barcode);
                        var YXComDetailInfo = YXComDetailList.Where(p => p.id == item.Barcode).FirstOrDefault();

                        var minSortValueQuery = (from m in Commodity.ObjectSet()
                                                 where m.AppId == AppId && m.CommodityType == 0
                                                 select m);
                        int? minSort = minSortValueQuery.Min(m => (int?)m.SortValue);
                        int minSortValue = 2;
                        if (minSort.HasValue)
                        {
                            minSortValue = minSort.Value;
                        }
                        item.Name = "网易严选 " + YXComDetailInfo.name;
                        item.SubId = userId;
                        item.No_Number = YXCom.No_Number ?? 0;
                        item.Price = YXComDetailInfo.skuList.Min(p => p.channelPrice);
                        item.Stock = YXCom.Stock;
                        item.PicturesPath = YXComDetailInfo.listPicUrl;
                        item.Description = "<div class=\"JD-goods\">" + YXComDetailInfo.itemDetail.detailHtml + "</div>";

                        //Com.State = 1; 只更新商品信息,不更新商品上下架状态
                        item.IsDel = false;
                        item.AppId = YXCom.AppId;
                        item.TotalCollection = 0;
                        item.TotalReview = 0;
                        item.Salesvolume = 0;
                        item.ModifiedOn = DateTime.Now;
                        item.ComAttribute = YXCom.ComAttribute;
                        item.CategoryName = YXCom.CategoryName;
                        item.SortValue = minSortValue - 1;
                        item.FreightTemplateId = YXCom.FreightTemplateId;  //99元以下商品8元运费
                        item.MarketPrice = YXCom.MarketPrice;
                        //Com.Weight = YXComDetailInfo.;
                        //Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                        item.SaleAreas = YXCom.SaleAreas;
                        item.SharePercent = YXCom.SharePercent;
                        item.CommodityType = 0;
                        item.Duty = YXCom.Duty;
                        item.TaxRate = YXCom.TaxRate;
                        item.TaxClassCode = YXCom.TaxClassCode;
                        item.Unit = "件";
                        item.InputRax = YXCom.InputRax;

                        if (YXComDetailInfo.skuList.Count() == 1)
                        {
                            //单条属性与库存表保持一致
                            item.JDCode = YXComDetailInfo.skuList.FirstOrDefault().id;
                            item.Code = item.JDCode;
                            item.Barcode = YXComDetailInfo.id;
                            item.No_Code = item.JDCode;
                        }
                        else
                        {
                            //多条属性存储SPU
                            item.JDCode = YXComDetailInfo.skuList.OrderBy(p => p.channelPrice).FirstOrDefault().id;
                            item.Code = item.JDCode;
                            item.Barcode = YXComDetailInfo.id; //存严选商品的SPU
                            item.No_Code = item.JDCode;
                        }

                        item.CostPrice = item.Price * Convert.ToDecimal(0.8);
                        item.ServiceSettingId = YXCom.ServiceSettingId;
                        //Com.TechSpecs = JdComDetailInfo.param;
                        item.SaleService = YXCom.SaleService;
                        item.Type = 0;
                        item.YJCouponActivityId = "";
                        item.YJCouponType = "";
                        item.ModifieId = userId;
                        List<ComAttributeDTO> ComAttr = new List<ComAttributeDTO>();
                        foreach (var skuitem in YXComDetailInfo.skuList)
                        {
                            foreach (var it in skuitem.itemSkuSpecValueList)
                            {
                                ComAttributeDTO ComAtt = new ComAttributeDTO();
                                ComAtt.Attribute = it.skuSpec.name;
                                ComAtt.SecondAttribute = it.skuSpecValue.value;
                                ComAttr.Add(ComAtt);
                            }
                        }
                        item.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(ComAttr);
                        item.RefreshCache(EntityState.Modified);
                        //更新库存表
                        UpdateCommodityStock(item, YXComDetailInfo, YXstockNumList, contextSession);
                        int count1 = contextSession.SaveChanges();
                        //更新JdCommodity表中状态
                        YXCom.State = 1;
                        #region 商品图片
                        //删除图片
                        ProductDetailsPictureBP pdpbp = new ProductDetailsPictureBP();
                        pdpbp.DeletePictures(item.Id);

                        List<string> PicList = new List<string>();
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl1);
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl2);
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl3);
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl4);
                        //添加图片
                        int sort = 1;
                        foreach (var itempic in PicList)
                        {
                            if (!string.IsNullOrWhiteSpace(itempic) && itempic.Length >= 50)
                            {
                                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                                pic.Name = "商品图片";
                                pic.SubId = userId;
                                pic.SubTime = DateTime.Now;
                                pic.PicturesPath = itempic;
                                pic.CommodityId = item.Id;
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
                        var ComCategory = CategoryList.Where(p => YXCom.CategoryName.Contains(p.Name)).ToList();
                        if (ComCategory.Any())
                        {
                            foreach (var itemcate in ComCategory)
                            {
                                CommodityCategory comcate = CommodityCategory.CreateCommodityCategory();
                                comcate.CategoryId = itemcate.Id;
                                comcate.CommodityId = item.Id;
                                comcate.SubId = userId;
                                comcate.SubTime = DateTime.Now;
                                comcate.Name = "商品分类";
                                comcate.IsDel = false;
                                #region 本地测试
                                //comcate.SubId = AppId;
                                //comcate.AppId = AppId;
                                //comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(AppId);
                                #endregion
                                //正式环境
                                comcate.SubId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                comcate.AppId = new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6");
                                comcate.CrcAppId = Jinher.JAP.Common.Crc64.ComputeAsAsciiGuid(new Guid("8B4D3317-6562-4D51-BEF1-0C05694AC3A6"));
                                contextSession.SaveObject(comcate);
                            }
                        }
                        else
                        {
                            NoCategoryData.Add(YXCom.JDCode);
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
                        var innerCateid = categoryList.Where(p => p.Name == YXCom.VideoName.Trim()).FirstOrDefault();
                        if (innerCateid != null)
                        {
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateCommodityInnerCategory();
                            comInnerCate.CategoryId = innerCateid.Id;
                            comInnerCate.CommodityId = item.Id;
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
                    var NoExist = YXComList.Where(p => !JdComDTO.RepeatData.Contains(p.Barcode)).ToList();
                    foreach (var item in NoExist)
                    {

                        var YXComDetailInfo = YXComDetailList.Where(p => p.id == item.JDCode).FirstOrDefault();

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
                        Com.Id = Guid.NewGuid();
                        Com.Name = "网易严选 " + YXComDetailInfo.name;
                        Com.SubTime = DateTime.Now;
                        Com.SubId = userId;
                        Com.No_Number = item.No_Number ?? 0;
                        Com.Price = YXComDetailInfo.skuList.Min(p => p.channelPrice);
                        Com.Stock = item.Stock;
                        Com.PicturesPath = YXComDetailInfo.listPicUrl;
                        Com.Description = "<div class=\"JD-goods\">" + YXComDetailInfo.itemDetail.detailHtml + "</div>";
                        Com.State = 1; //只更新商品信息,不更新商品上下架状态
                        Com.IsDel = false;
                        Com.AppId = item.AppId;
                        Com.TotalCollection = 0;
                        Com.TotalReview = 0;
                        Com.Salesvolume = 0;
                        Com.ModifiedOn = DateTime.Now;
                        Com.ComAttribute = item.ComAttribute;
                        Com.CategoryName = item.CategoryName;
                        Com.SortValue = minSortValue - 1;
                        Com.FreightTemplateId = item.FreightTemplateId;  //99元以下商品8元运费
                        Com.MarketPrice = item.MarketPrice;
                        //Com.Weight = YXComDetailInfo.;
                        //Com.SaleService = JdComDetailInfo.wareQD + "<br>" + JdComDetailInfo.shouhou;
                        Com.SaleAreas = item.SaleAreas;
                        Com.SharePercent = item.SharePercent;
                        Com.CommodityType = 0;
                        Com.Duty = item.Duty;
                        Com.TaxRate = item.TaxRate;
                        Com.TaxClassCode = item.TaxClassCode;
                        Com.Unit = "件";
                        Com.InputRax = item.InputRax;

                        if (YXComDetailInfo.skuList.Count() == 1)
                        {
                            //单条属性与库存表保持一致
                            Com.JDCode = YXComDetailInfo.skuList.FirstOrDefault().id;
                            Com.Code = Com.JDCode;
                            Com.Barcode = YXComDetailInfo.id;
                            Com.No_Code = Com.JDCode;
                        }
                        else
                        {
                            //多条属性存储SPU
                            Com.JDCode = YXComDetailInfo.skuList.OrderBy(p => p.channelPrice).FirstOrDefault().id;
                            Com.Code = Com.JDCode;
                            Com.Barcode = YXComDetailInfo.id; //存严选商品的SPU
                            Com.No_Code = Com.JDCode;
                        }

                        Com.CostPrice = Com.Price * Convert.ToDecimal(0.8);
                        Com.ServiceSettingId = item.ServiceSettingId;
                        //Com.TechSpecs = JdComDetailInfo.param;
                        Com.SaleService = item.SaleService;
                        Com.Type = 0;
                        Com.YJCouponActivityId = "";
                        Com.YJCouponType = "";
                        Com.ModifieId = userId;
                        List<ComAttributeDTO> ComAttr = new List<ComAttributeDTO>();
                        foreach (var skuitem in YXComDetailInfo.skuList)
                        {
                            foreach (var it in skuitem.itemSkuSpecValueList)
                            {
                                ComAttributeDTO ComAtt = new ComAttributeDTO();
                                ComAtt.Attribute = it.skuSpec.name;
                                ComAtt.SecondAttribute = it.skuSpecValue.value;
                                ComAttr.Add(ComAtt);
                            }
                        }
                        Com.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(ComAttr);
                        contextSession.SaveObject(Com);
                        SaveCommodityStock(Com, YXComDetailInfo, YXstockNumList, contextSession);

                        //更新JdCommodity表中状态
                        item.State = 1;
                        #region 商品图片
                        List<string> PicList = new List<string>();
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl1);
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl2);
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl3);
                        PicList.Add(YXComDetailInfo.itemDetail.picUrl4);
                        //添加图片
                        int sort = 1;
                        foreach (var itempic in PicList)
                        {
                            if (!string.IsNullOrEmpty(itempic))
                            {
                                ProductDetailsPicture pic = ProductDetailsPicture.CreateProductDetailsPicture();
                                pic.Name = "商品图片";
                                pic.SubId = userId;
                                pic.SubTime = DateTime.Now;
                                pic.PicturesPath = itempic;
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
        /// 更新保存库存
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void UpdateCommodityStock(Commodity Com, YXComDetailDTO YXComDetailInfo, List<StockDTO> stockNumList, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {
                LogHelper.Debug(string.Format("更新保存库存", JsonHelper.JsonSerializer(YXComDetailInfo.skuList)));
                var ComStock = CommodityStock.ObjectSet().Where(p => p.CommodityId == Com.Id).ToList();
                //根据SkuId获取商品库存
                var SkuId = YXComDetailInfo.skuList.Select(p => p.id).ToList();
                var stockNum = stockNumList.Where(p => SkuId.Contains(p.skuId)).ToList();
                Com.Stock = stockNum.Sum(p => p.inventory);
                //库存表中存在则更新 
                foreach (var item in ComStock)
                {
                    var YXStockInfo = YXComDetailInfo.skuList.FirstOrDefault(p => p.id == item.JDCode);
                    if (YXStockInfo != null)
                    {
                        List<ComAttributeDTO> ComAttribute = new List<ComAttributeDTO>();
                        foreach (var it in YXStockInfo.itemSkuSpecValueList)
                        {
                            ComAttributeDTO ComAtt = new ComAttributeDTO();
                            ComAtt.Attribute = it.skuSpec.name;
                            ComAtt.SecondAttribute = it.skuSpecValue.value;
                            ComAttribute.Add(ComAtt);
                        }
                        item.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(ComAttribute);
                        item.Price = YXStockInfo.channelPrice;
                        item.Stock = stockNum.FirstOrDefault(p => p.skuId == YXStockInfo.id).inventory;
                        item.Duty = Com.Duty;
                        item.Barcode = Com.Barcode;
                        item.No_Code = Com.No_Code;
                        item.ErQiCode = Com.ErQiCode;
                        item.CostPrice = YXStockInfo.channelPrice * Convert.ToDecimal(0.8);
                        item.JDCode = YXStockInfo.id;
                        if (isUpdate == true)
                        {
                            item.EntityState = EntityState.Modified;
                        }
                        item.EntityState = EntityState.Modified;
                    }
                    else
                    {
                        item.EntityState = EntityState.Deleted;
                    }
                    contextSession.SaveObject(item);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("更新保存库存服务保存库存异常。UpdateCommodityStock"), ex);
            }
        }
        /// <summary>
        /// 发布保存库存
        /// </summary>
        /// <param name="item"></param>
        /// <param name="contextSession"></param>
        /// <param name="isUpdate"></param>
        void SaveCommodityStock(Commodity Com, YXComDetailDTO YXComDetailInfo, List<StockDTO> stockNumList, ContextSession contextSession)
        {
            try
            {
                LogHelper.Info(string.Format("发布保存库存SaveCommodityStock:", JsonHelper.JsonSerializer(YXComDetailInfo.skuList)));
                //根据SkuId获取商品库存
                var SkuId = YXComDetailInfo.skuList.Select(p => p.id).ToList();
                var stockNum = stockNumList.Where(p => SkuId.Contains(p.skuId)).ToList();
                Com.Stock = stockNum.Sum(p => p.inventory);
                foreach (var item in YXComDetailInfo.skuList)
                {
                    List<ComAttributeDTO> ComAttribute = new List<ComAttributeDTO>();
                    foreach (var it in item.itemSkuSpecValueList)
                    {
                        ComAttributeDTO ComAtt = new ComAttributeDTO();
                        ComAtt.Attribute = it.skuSpec.name;
                        ComAtt.SecondAttribute = it.skuSpecValue.value;
                        ComAttribute.Add(ComAtt);
                    }
                    CommodityStock cs = CommodityStock.CreateCommodityStock();
                    cs.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(ComAttribute);
                    cs.CommodityId = Com.Id;
                    cs.Price = item.channelPrice;
                    cs.Stock = stockNum.FirstOrDefault(p => p.skuId == item.id).inventory;
                    cs.Duty = Com.Duty;
                    cs.Barcode = Com.Barcode;
                    cs.No_Code = Com.No_Code;
                    cs.ThumImg = item.picUrl;
                    cs.JDCode = item.id;
                    cs.ErQiCode = Com.ErQiCode;
                    cs.CostPrice = item.channelPrice * Convert.ToDecimal(0.8);
                    contextSession.SaveObject(cs);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("商品发布服务保存库存异常。SaveCommodityStock"), ex);
            }
        }

    }
}
