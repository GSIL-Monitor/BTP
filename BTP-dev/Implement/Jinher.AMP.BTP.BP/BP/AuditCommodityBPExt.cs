
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/2/3 9:31:52
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
using Jinher.JAP.Common.Loging;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using System.Data;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.IBP.Facade;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class AuditCommodityBP : BaseBP, IAuditCommodity
    {
        /// <summary>
        /// 获取审核商品信息(商铺提交)
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Name"></param>
        /// <param name="CateNames"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetApplyCommodityListExt(System.Collections.Generic.List<System.Guid> AppidList, string Name, string CateNames, int AuditState, int pageIndex, int pageSize)
        {
            try
            {
                var query = from m in AuditManage.ObjectSet()
                            join n in AuditCommodity.ObjectSet() on m.Id equals n.Id
                            where AppidList.Contains(m.AppId) & m.Action < 9
                            select new CommodityAndCategoryDTO
                            {
                                AuditId = m.Id,
                                CommodityId = n.CommodityId,
                                AppId = n.AppId,
                                PicturesPath = n.PicturesPath,
                                Name = n.Name,
                                CateNames = n.CategoryName,
                                Price = n.Price,
                                CostPrice = n.CostPrice,
                                Stock = n.Stock,
                                BarCode = n.Barcode,
                                Unit = n.Unit,
                                Action = m.Action,
                                AuditRemark = m.AuditRemark,
                                AuditState = m.Status,
                                ApplyTime = m.ApplyTime
                            };
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(p => Name.Contains(p.Name) || p.Name.Contains(Name));
                }
                if (!string.IsNullOrEmpty(CateNames))
                {
                    query = query.Where(p => p.CateNames.Contains(CateNames) || CateNames.Contains(p.CateNames));
                }
                if (AuditState > -1)
                {
                    if (AuditState == 2)
                    {
                        query = query.Where(p => p.AuditState == 2 || p.AuditState == 3);
                    }
                    else
                    {
                        query = query.Where(p => p.AuditState == AuditState);
                    }
                }
                var result = query.OrderByDescending(p => p.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (result != null)
                {
                    result.ForEach(p =>
                    {
                        //匹配操作
                        p.ActionName = Enum.GetName(typeof(Jinher.AMP.BTP.Deploy.Enum.OperateTypeEnum), p.Action);
                    });
                }

                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取审核商品信息异常。GetApplyCommodityListExt：{0}", AppidList), ex);
                return null;
            }
        }
        /// <summary>
        /// 获取审核信息(馆审核)
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetAuditCommodityListExt(System.Guid EsAppId, System.Collections.Generic.List<System.Guid> AppidList, string Name, int AuditState, int pageIndex, int pageSize)
        {
            try
            {
                var query = from m in AuditManage.ObjectSet()
                            join n in AuditCommodity.ObjectSet() on m.Id equals n.Id
                            where m.EsAppId == EsAppId & m.Status == 0 & m.Action < 9   //待审核状态为1 
                            select new CommodityAndCategoryDTO
                            {
                                AuditId = m.Id,
                                CommodityId = n.CommodityId,
                                AppId = n.AppId,
                                PicturesPath = n.PicturesPath,
                                Name = n.Name,
                                No_Number = n.No_Number,
                                CateNames = n.CategoryName,
                                Price = n.Price,
                                CostPrice = n.CostPrice,
                                Stock = n.Stock,
                                BarCode = n.Barcode,
                                Unit = n.Unit,
                                Action = m.Action,
                                AuditRemark = m.AuditRemark,
                                AuditState = m.Status,
                                ApplyTime = m.ApplyTime
                            };
                if (AppidList != null & AppidList.Count > 0)
                {
                    query = query.Where(p => AppidList.Contains(p.AppId));
                }
                if (!string.IsNullOrEmpty(Name))
                {
                    query = query.Where(p => Name.Contains(p.Name) || p.Name.Contains(Name));
                }
                var result = query.OrderByDescending(p => p.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                //匹配供应商名称和App名称
                if (result != null)
                {
                    List<Guid> appIds = (from it in result select it.AppId).Distinct().ToList();
                    //获取商铺名称
                    Dictionary<Guid, string> listApps = APPSV.GetAppNameListByIds(appIds);
                    //获取供应商名称
                    var SupplierList = Supplier.ObjectSet().Where(p => appIds.Contains(p.AppId)).Select(s => new { s.AppId, s.SupplierName }).Distinct().ToList();
                    foreach (var item in result)
                    {
                        //匹配供应商名称
                        item.SupplyName = SupplierList.Where(p => p.AppId == item.AppId).Select(s => s.SupplierName).FirstOrDefault();
                        //获取商铺名称
                        if (listApps.ContainsKey(item.AppId))
                        {
                            var listAppName = listApps[item.AppId];
                            if (!String.IsNullOrEmpty(listAppName))
                            {
                                item.AppName = listAppName;
                            }
                        }
                        //匹配操作
                        item.ActionName = Enum.GetName(typeof(Jinher.AMP.BTP.Deploy.Enum.OperateTypeEnum), item.Action);
                    }
                }
                ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> retInfo = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>>
                {
                    ResultCode = query.Count(),
                    Data = result
                };
                return retInfo;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("馆获取待审核商品信息异常。GetApplyCommodityListExt：{0}", AppidList), ex);
                return null;
            }
        }

        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CommodityId"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>       
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO GetAuditCommodityExt(System.Guid Id, System.Guid commodityId, Guid appId)
        {
            CommodityAndCategoryDTO Cac = new CommodityAndCategoryDTO();
            Guid sizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            Guid colorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            AuditCommodity Auditcommodity = new AuditCommodity();

            //尽可能并行执行这些方法
            System.Threading.Tasks.Parallel.Invoke(() =>
            {
                //获取商品信息
                Auditcommodity = AuditCommodity.ObjectSet().Where(n => n.Id == Id && n.CommodityId == commodityId).FirstOrDefault();//获取商品信息
                //获取商品分类信息
                List<CommodityCategory> category = CommodityCategory.ObjectSet().Where(n => n.CommodityId == commodityId && n.AppId == appId).ToList();
                //获取关联商品
                List<RelationCommodity> relacomList = RelationCommodity.ObjectSet().Where(p => p.CommodityId == commodityId).ToList();
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
            Cac.CommodityType = Convert.ToInt32(Auditcommodity.Type); //商品类型
            Cac.YJCouponActivityId = Auditcommodity.YJCouponActivityId;//活动编码
            Cac.YJCouponType = Auditcommodity.YJCouponType;//类型编码
            Cac.Name = Auditcommodity.Name;
            Cac.AuditId = Auditcommodity.Id;//审核表唯一ID
            Cac.CommodityId = Auditcommodity.CommodityId;//商品ID
            Cac.Price = Auditcommodity.Price;
            Cac.MarketPrice = Auditcommodity.MarketPrice;
            Cac.CommodityDuty = Auditcommodity.Duty;
            Cac.CommodityTaxRate = Auditcommodity.TaxRate;
            Cac.CommodityInputTax = Auditcommodity.InputRax;
            Cac.TaxClassCode = Auditcommodity.TaxClassCode;
            Cac.Unit = Auditcommodity.Unit;
            Cac.PicturesPath = Auditcommodity.PicturesPath;
            Cac.No_Code = Auditcommodity.No_Code;
            Cac.Stock = Auditcommodity.Stock;
            Cac.State = Auditcommodity.State;
            Cac.Description = Auditcommodity.Description;
            Cac.CostPrice = Auditcommodity.CostPrice;
            Cac.BarCode = Auditcommodity.Barcode;
            Cac.IsAssurance = Auditcommodity.IsAssurance ?? false;
            Cac.IsReturns = Auditcommodity.IsReturns ?? false;
            Cac.ServiceSettingId = Auditcommodity.ServiceSettingId;
            Cac.TechSpecs = Auditcommodity.TechSpecs;
            Cac.SaleService = Auditcommodity.SaleService;
            Cac.JDCode = Auditcommodity.JDCode;
            Cac.SizeNames = "";
            Cac.ColorNames = "";
            Cac.FreightId = Auditcommodity.FreightTemplateId.HasValue ? Auditcommodity.FreightTemplateId.Value.ToString() : "";
            List<string> attrs = new List<string>();
            //zgx-modify
            var cStock = AuditCommodityStock.ObjectSet().Where(n => n.AuditId == Id & n.CommodityId == commodityId).ToList();
            if (cStock != null && cStock.Count > 0) //组合属性
            {
                Cac.ComAttributes = new List<Deploy.CustomDTO.CommodityStockDTO>();
                foreach (var item in cStock)
                {
                    var t = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(item.ComAttribute);
                    Cac.ComAttributes.Add(new Deploy.CustomDTO.CommodityStockDTO()
                    {
                        ComAttribute = t,
                        Id = item.CommodityStockId,
                        Price = item.Price,
                        MarketPrice = item.MarketPrice,
                        Duty = item.Duty,
                        Stock = item.Stock,
                        CostPrice = item.CostPrice,
                        BarCode = item.Barcode,
                        JDCode = item.JDCode,
                        Code = item.No_Code,
                        ThumImg = item.ThumImg,
                        CarouselImgs = item.CarouselImgs
                    });
                }
            }
            else if (!string.IsNullOrEmpty(Auditcommodity.ComAttribute))
            {
                List<ComAttributeDTO> comAttr = JsonHelper.JsonDeserialize<List<ComAttributeDTO>>(Auditcommodity.ComAttribute);
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

            Cac.AppId = Auditcommodity.AppId;
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
            else if (!string.IsNullOrEmpty(Auditcommodity.ComAttribute) && attrs.Count > 0)
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
            Cac.IsEnableSelfTake = Auditcommodity.IsEnableSelfTake;
            Cac.PricingMethod = Auditcommodity.PricingMethod;
            Cac.Weight = Auditcommodity.Weight.HasValue ? Auditcommodity.Weight.Value : 0;
            Cac.SaleAreas = Auditcommodity.SaleAreas;
            Cac.VideoName = Auditcommodity.VideoName;
            Cac.VideoUrl = Auditcommodity.MobileVideoPath;
            Cac.VideoclientUrl = Auditcommodity.HtmlVideoPath;
            Cac.VideoPicUrl = Auditcommodity.VideoPic;
            Cac.ScoreScale = Auditcommodity.ScoreScale;

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
        #region 发布商品插入AuditCommodity表
        /// <summary>
        /// 发布商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>       
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAuditCommodityExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO commodityAndCategoryDTO)
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
                    ContextSession contextSession = ContextFactory.CurrentThreadContext;
                    //添加审核表信息
                    AuditManage AuditInfo = AuditManage.CreateAuditManage();
                    AuditInfo.Id = Guid.NewGuid();
                    AuditInfo.Status = 0; //0 待审核   1 审核通过  2 3审核不通过  4 已撤销
                    AuditInfo.EsAppId = GetEsAppId(commodityAndCategoryDTO.AppId);
                    AuditInfo.AppId = commodityAndCategoryDTO.AppId;
                    AuditInfo.ApplyUserId = userId;
                    AuditInfo.ApplyTime = DateTime.Now;
                    AuditInfo.Action = commodityAndCategoryDTO.Action;
                    contextSession.SaveObject(AuditInfo);
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
                    if (commodityAndCategoryDTO.Action == 0)
                    {
                        //发布商品
                        if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code) && !IsExistsExt(commodityAndCategoryDTO.No_Code, commodityAndCategoryDTO.AppId))
                        {
                            return new ResultDTO { ResultCode = 2, Message = "商品编号不能重复，该编号已存在" };
                        }
                    }
                    else
                    {
                        //编辑商品
                        Commodity commodityDTO = Commodity.ObjectSet().FirstOrDefault(n => n.Id == commodityAndCategoryDTO.CommodityId
                       && n.AppId == commodityAndCategoryDTO.AppId && n.CommodityType == 0);
                        if (commodityDTO != null)
                        {
                            if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code) && commodityAndCategoryDTO.No_Code != commodityDTO.No_Code)
                            {
                                if (!IsExistsExt(commodityAndCategoryDTO.No_Code, commodityAndCategoryDTO.AppId))
                                {
                                    return new ResultDTO { ResultCode = 2, Message = "商品编号不能重复，该编号已存在" };
                                }
                            }
                        }
                    }
                    AuditCommodity commodity = AuditCommodity.CreateAuditCommodity();
                    if (commodityAndCategoryDTO.Action == 0)
                    {
                        commodity.CommodityId = Guid.NewGuid();
                    }
                    else
                    {
                        commodity.CommodityId = commodityAndCategoryDTO.CommodityId;
                    }
                    commodity.Id = AuditInfo.Id;
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
                    if (commodityAndCategoryDTO.Action == 0)
                    {
                        commodity.SortValue = minSortValue - 1;  //商品通过审核后再排序
                    }
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
                    commodity.CostPrice = commodityAndCategoryDTO.CostPrice;
                    commodity.IsAssurance = commodityAndCategoryDTO.IsAssurance;
                    commodity.IsReturns = commodityAndCategoryDTO.IsReturns;
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

                            AuditCommodityStock cs = AuditCommodityStock.CreateAuditCommodityStock();
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
                            cs.AuditId = AuditInfo.Id;
                            if (commodityAndCategoryDTO.Action == 0)
                            {
                                cs.CommodityStockId = Guid.NewGuid();
                            }
                            else
                            {
                                cs.CommodityStockId = item.Id;
                            }
                            cs.CommodityId = commodity.CommodityId;
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
                            int count1 = contextSession.SaveChange();
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
                    //如果为编辑需要删除原商品分类 
                    if (commodityAndCategoryDTO.Action == 1)
                    {
                        var catList = CommodityCategory.ObjectSet()
                    .Where(c => c.CommodityId == commodityAndCategoryDTO.CommodityId && c.AppId == commodityAndCategoryDTO.AppId).ToList();
                        foreach (var commodityCategory in catList)
                        {
                            contextSession.Delete(commodityCategory);
                        }
                    }
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
                            comcate.CommodityId = commodity.CommodityId;
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
                        CommodityCategory comcate = CommodityCategory.CreateDefaultCategory(commodity.CommodityId,
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
                        comBrand.CommodityId = commodityAndCategoryDTO.Id;
                        comBrand.SubTime = DateTime.Now;
                        comBrand.ModifiedOn = comBrand.SubTime;
                        comBrand.AppId = commodityAndCategoryDTO.AppId;
                        comBrand.SubId = commodityAndCategoryDTO.SubId;
                        comBrand.CrcAppId = 0;
                        ResultDTO resultCb = cibf.AddComInnerBrand(comBrand);
                    }
                    #endregion

                    #region 商城分类
                    LogHelper.Info("添加商城分类Begining，商品Id: " + commodity.CommodityId + "，InnerCategoryPath: " + commodityAndCategoryDTO.InnerCategoryPath);
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.EsCategory))
                    {
                        //如果为编辑删除原商城分类
                        if (commodityAndCategoryDTO.Action == 1)
                        {
                            var oldCCs = CommodityInnerCategory.ObjectSet()
                            .Where(c => c.CommodityId == commodityAndCategoryDTO.CommodityId && c.AppId == commodityAndCategoryDTO.AppId).ToList();
                            foreach (var commodityCategory in oldCCs)
                            {
                                contextSession.Delete(commodityCategory);
                            }
                        }
                        string[] innerCatelist = commodityAndCategoryDTO.EsCategory.Split(',');
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
                                comInnerCate.CommodityId = commodity.CommodityId;
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
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateDefaultInnerCategory(commodity.CommodityId, commodity.AppId, commodity.SubId);
                            contextSession.SaveObject(comInnerCate);
                        }
                    }
                    #endregion

                    LogHelper.Info("添加商城分类Begining，商品Id: " + commodity.CommodityId + "，InnerCategoryPath: " + commodityAndCategoryDTO.InnerCategoryPath);
                    if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.InnerCategoryPath))
                    {
                        //如果为编辑删除原商城分类
                        if (commodityAndCategoryDTO.Action == 1)
                        {
                            var oldCCs = CommodityInnerCategory.ObjectSet()
                            .Where(c => c.CommodityId == commodityAndCategoryDTO.CommodityId && c.AppId == commodityAndCategoryDTO.AppId).ToList();
                            foreach (var commodityCategory in oldCCs)
                            {
                                contextSession.Delete(commodityCategory);
                            }
                        }
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
                                comInnerCate.CommodityId = commodity.CommodityId;
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
                            CommodityInnerCategory comInnerCate = CommodityInnerCategory.CreateDefaultInnerCategory(commodity.CommodityId, commodity.AppId, commodity.SubId);
                            contextSession.SaveObject(comInnerCate);
                        }
                    }
                    #region

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
                        pic.CommodityId = commodity.CommodityId;
                        pic.Sort = sort;
                        contextSession.SaveObject(pic);

                        sort++;
                    }

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
                                relamodel.CommodityId = commodity.CommodityId;
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
                    //审核通过后加入商品日志
                    //CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    //contextSession.SaveObject(journal);
                    #endregion

                    #region 舌尖餐盒相关信息
                    if (commodityAndCategoryDTO.From == 1 && (commodityAndCategoryDTO.CommodityBoxCount != -1 || commodityAndCategoryDTO.CommodityBoxPrice != -1))
                    {
                        CateringComdtyXData cbox = CateringComdtyXData.CreateCateringComdtyXData();
                        cbox.ComdtyId = commodity.CommodityId;
                        cbox.SubId = userId;
                        cbox.ModifiedOn = cbox.SubTime = DateTime.Now;
                        cbox.MealBoxNum = commodityAndCategoryDTO.CommodityBoxCount;
                        cbox.MealBoxAmount = commodityAndCategoryDTO.CommodityBoxPrice;
                        cbox.Unit = "";
                        cbox.IsValid = true;
                        contextSession.SaveObject(cbox);
                    }
                    #endregion

                    var commodityEntityState = commodity.EntityState;
                    contextSession.SaveChanges();
                }
                else
                {
                    return new ResultDTO { ResultCode = 2, Message = "您需要先完善发布者信息才能发布商品" };
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
        void SaveCommodityStock(AuditCommodity item, ContextSession contextSession, bool isUpdate = false)
        {
            try
            {
                AuditCommodityStock cs = AuditCommodityStock.CreateAuditCommodityStock();
                cs.ComAttribute = "[]";
                cs.Id = Guid.NewGuid();
                cs.CommodityStockId = item.CommodityId;
                cs.AuditId = item.Id;
                cs.CommodityId = item.CommodityId;
                cs.Price = item.Price;
                cs.MarketPrice = item.MarketPrice;
                cs.Stock = item.Stock;
                cs.Duty = item.Duty;
                cs.Barcode = item.Barcode;
                cs.No_Code = item.No_Code;
                cs.JDCode = item.JDCode;
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
                //商品表中检查编号是否存在
                var commodity = Commodity.ObjectSet()
                        .Where(n => n.No_Code == code && n.AppId == appId && !n.IsDel && n.CommodityType == 0)
                        .FirstOrDefault();
                //检查审核表中待审核商品
                var AuditCom = (from m in AuditManage.ObjectSet()
                                join n in AuditCommodity.ObjectSet() on m.Id equals n.Id
                                where n.No_Code == code & n.AppId == appId & !n.IsDel & n.CommodityType == 0 & m.Status == 0
                                select n).FirstOrDefault();
                bReturn = (null == commodity) & (null == AuditCom);
            }
            return bReturn;
        }
        #endregion
        /// <summary>
        /// 修改的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>           
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditAuditCommodityExt(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            try
            {
                LogHelper.Debug("AuditCommodityBP.EditAuditCommodityExt,入参：" + JsonHelper.JsonSerializer(com));
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //添加审核表信息
                AuditManage AuditInfo = AuditManage.CreateAuditManage();
                AuditInfo.Id = Guid.NewGuid();
                AuditInfo.Status = 0; //0 待审核   1 审核通过  2 3审核不通过  4 已撤销
                AuditInfo.EsAppId = GetEsAppId(com.AppId);
                AuditInfo.AppId = com.AppId;
                AuditInfo.ApplyUserId = this.ContextDTO.LoginUserID;
                AuditInfo.ApplyTime = DateTime.Now;
                AuditInfo.Action = com.Action;
                contextSession.SaveObject(AuditInfo);
                //插入审核主表
                AuditCommodity AuditCom = AuditCommodity.CreateAuditCommodity();
                AuditCom.Id = AuditInfo.Id;
                AuditCom.CommodityId = com.CommodityId;
                AuditCom.Name = com.Name;
                AuditCom.Code = com.Code;
                AuditCom.SubTime = com.SubTime;
                AuditCom.SubId = com.SubId;
                AuditCom.No_Number = com.No_Number;
                AuditCom.Price = com.Price;
                AuditCom.Stock = com.Stock;
                AuditCom.PicturesPath = com.PicturesPath;
                AuditCom.Description = com.Description;
                AuditCom.State = com.State;
                AuditCom.IsDel = com.IsDel;
                AuditCom.AppId = com.AppId;
                AuditCom.No_Code = com.No_Code;
                AuditCom.TotalCollection = com.TotalCollection;
                AuditCom.TotalReview = com.TotalReview;
                AuditCom.Salesvolume = com.Salesvolume;
                AuditCom.ModifiedOn = DateTime.Now;
                AuditCom.GroundTime = com.GroundTime;
                AuditCom.ComAttribute = com.ComAttribute;
                AuditCom.CategoryName = com.CategoryName;
                AuditCom.SortValue = com.SortValue;
                AuditCom.FreightTemplateId = com.FreightTemplateId;
                AuditCom.MarketPrice = com.MarketPrice;
                AuditCom.IsEnableSelfTake = com.IsEnableSelfTake;
                AuditCom.Weight = com.Weight;
                AuditCom.PricingMethod = com.PricingMethod;
                AuditCom.SaleAreas = com.SaleAreas;
                AuditCom.SharePercent = com.SharePercent;
                AuditCom.CommodityType = com.CommodityType;
                AuditCom.HtmlVideoPath = com.VideoclientUrl;
                AuditCom.MobileVideoPath = com.VideoUrl;
                AuditCom.VideoPic = com.VideoPicUrl;
                AuditCom.VideoName = com.VideoName;
                AuditCom.ScorePercent = com.ScorePercent;
                AuditCom.Duty = com.CommodityDuty;
                AuditCom.SpreadPercent = com.SpreadPercent;
                AuditCom.ScoreScale = com.ScoreScale;
                AuditCom.TaxRate = com.CommodityTaxRate;
                AuditCom.TaxClassCode = com.TaxClassCode;
                AuditCom.Unit = com.Unit;
                AuditCom.InputRax = com.CommodityInputTax;
                AuditCom.Barcode = com.BarCode;
                AuditCom.JDCode = com.JDCode;
                AuditCom.CostPrice = com.CostPrice;
                AuditCom.IsAssurance = com.IsAssurance;
                AuditCom.TechSpecs = com.TechSpecs;
                AuditCom.SaleService = com.SaleService;
                AuditCom.IsReturns = com.IsReturns;
                AuditCom.ServiceSettingId = com.ServiceSettingId;
                AuditCom.Type = com.CommodityType;
                AuditCom.YJCouponActivityId = com.YJCouponActivityId;
                AuditCom.YJCouponType = com.YJCouponType;
                AuditCom.ModifieId = this.ContextDTO.LoginUserID;
                AuditCom.FieldName = "";
                contextSession.SaveObject(AuditCom);
                //如果是编辑将属性表数据插入审核属性表中
                if (com.ComAttributes != null && com.ComAttributes.Count() > 0)
                {
                    foreach (var item in com.ComAttributes)
                    {
                        item.ComAttribute = new List<ComAttributeDTO>();
                        item.ComAttributeIds.ForEach(
                            r =>
                            item.ComAttribute.Add(new ComAttributeDTO()
                            {
                                Attribute = r.Attribute,
                                SecondAttribute = r.SecondAttribute
                            }));


                        var ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(item.ComAttribute);
                        AuditCommodityStock AuditComStock = AuditCommodityStock.CreateAuditCommodityStock();
                        AuditComStock.CommodityStockId = item.Id;
                        AuditComStock.AuditId = AuditInfo.Id;
                        AuditComStock.CommodityId = com.CommodityId;
                        AuditComStock.ComAttribute = ComAttribute;
                        AuditComStock.Price = com.Price;
                        AuditComStock.Stock = com.Stock;
                        AuditComStock.MarketPrice = com.MarketPrice;
                        AuditComStock.SubTime = com.SubTime;
                        AuditComStock.ModifiedOn = DateTime.Now;
                        AuditComStock.Duty = item.Duty;
                        AuditComStock.Barcode = item.BarCode;
                        AuditComStock.No_Code = item.Code;
                        AuditComStock.JDCode = item.JDCode;
                        AuditComStock.CostPrice = item.CostPrice;
                        AuditComStock.ThumImg = item.ThumImg;
                        AuditComStock.CarouselImgs = item.CarouselImgs;
                        contextSession.SaveObject(AuditComStock);
                    }
                }
                int count = contextSession.SaveChanges();

                if (count > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "成功提交,待审核" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "提交失败" };
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改的商品插入AuditCommodity表服务异常。datetime：{0}", DateTime.Now), ex);
                return new ResultDTO();
            }
        }
        /// <summary>
        /// 设置审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetModeStatusExt(System.Guid Appid, int ModeStatus)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var auditMode = AuditMode.ObjectSet().Where(p => p.AppId == Appid).ToList();
                if (auditMode != null && auditMode.Count() > 0)
                {
                    foreach (var item in auditMode)
                    {
                        item.ModeStatus = ModeStatus;
                        item.ModifiedId = this.ContextDTO.LoginUserID;//修改人id
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                    }
                }
                int result = contextSession.SaveChange();
                if (result > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("设置审核方式服务异常。Appid：{0}，ModeStatus：{1}", Appid, ModeStatus), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }
        /// <summary>
        /// 获取设置的审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.AuditModeDTO GetModeStatusExt(System.Guid Appid)
        {
            try
            {
                var query = (from n in AuditMode.ObjectSet()
                             where n.AppId == Appid
                             select new AuditModeDTO
                             {
                                 ModeStatus = n.ModeStatus
                             }).FirstOrDefault();
                return query;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取设置的审核方式服务异常。Appid：{0}", Appid), ex);
                return new AuditModeDTO();
            }
        }
        /// <summary>
        /// 手动审核商品
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditApplyExt(List<System.Guid> ids, int state, string AuditRemark)
        {
            try
            {
                //如果审核通过则将通过的商品数据插入commodity和commoditstock表
                if (state == 1)
                {
                    var query = AuditCommodity.ObjectSet().Where(p => ids.Contains(p.Id)).ToList();
                    foreach (var item in query)
                    {
                        var AuditQuery = AuditManage.ObjectSet().Where(p => p.Id == item.Id).FirstOrDefault();
                        ResultDTO dto;
                        switch (AuditQuery.Action)
                        {
                            case 0:
                                dto = AddCommodity(item.Id);//发布商品
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 1:
                                dto = UpdateCommodity(item.Id, item.CommodityId, item.AppId); //编辑商品
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 2:
                                dto = OnSale(item.CommodityId, item.ModifiedId, item.ModifiedOn); //上架
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 3:
                                dto = UpdateComName(item.CommodityId, item.Name, item.ModifiedId, item.ModifiedOn);//修改商品名称
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 4:
                                dto = UpdateCategoryBycommodityId(item.CommodityId, item.CategoryName);//修改商品类别
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 5:
                                dto = UpdatePrice(item.CommodityId, item.Price, item.ModifiedId, item.ModifiedOn);//修改商品价格
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 6:
                                dto = UpdateMarketPrice(item.CommodityId, item.MarketPrice, item.ModifiedId, item.ModifiedOn); //修改商品市场价
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 7:
                                dto = UpdateStock(item.CommodityId, item.Stock, item.ModifiedId, item.ModifiedOn);//修改商品库存
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                            case 8:
                                dto = UpdateSalesvolume(item.CommodityId, item.Salesvolume, item.ModifiedId, item.ModifiedOn);
                                if (dto.ResultCode == 1) { return new ResultDTO { ResultCode = 1, Message = "false" }; }
                                break;
                        }
                    }
                }
                ContextFactory.ReleaseContextSession();
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var AuditManageList = AuditManage.ObjectSet().Where(p => ids.Contains(p.Id));
                if (AuditManageList != null)
                {
                    foreach (var item in AuditManageList)
                    {
                        item.Status = state;
                        if (state != 3) //编辑提交时状态为3
                        {
                            item.AuditRemark = AuditRemark;//审核意见
                            item.AuditUserId = this.ContextDTO.LoginUserID;//修改人id
                            item.AuditTime = DateTime.Now;
                        }
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                    }
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "false" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("手动审核商品服务异常。ids：{0}，ModeStatus：{1}", ids, state), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }
        /// <summary>
        /// 获取易捷馆及入住商铺的Appids
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetYiJieAppidsExt()
        {
            try
            {
                //易捷appid
                Guid YJAppid = new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6");
                List<Guid> AppidsList = MallApply.ObjectSet().Where(p => p.EsAppId == YJAppid).Select(p => p.AppId).ToList();
                return AppidsList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取易捷馆及入住商铺的Appids服务异常。datetime：{0}", DateTime.Now), ex);
                return new List<Guid>();
            }

        }
        /// <summary>
        /// 判断该商铺商品是否需要被审核
        /// </summary>
        /// <returns></returns>
        public bool IsAuditAppidExt(System.Guid AppId)
        {
            try
            {
                bool flag = false;
                //根据AppId获取馆信息
                List<Guid> AppidsList = MallApply.ObjectSet().Where(p => p.AppId == AppId && (p.State.Value == 2 || p.State.Value == 4)).Select(p => p.EsAppId).ToList();
                //审核设置表中需要审核的馆id列表
                List<Guid> AuditAppid = AuditMode.ObjectSet().Where(p => p.ModeStatus == 1).Select(p => p.AppId).ToList();
                //存在交集则需要审核
                flag = AppidsList.Intersect(AuditAppid).Count() > 0;
                return flag;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("判断该商铺商品是否需要被审核服务异常。AppId{0} datetime：{1}", AppId, DateTime.Now), ex);
                return false;
            }
        }
        /// <summary>
        /// 根据appid获取馆id 判断是否包含易捷馆
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Guid GetEsAppId(Guid AppId)
        {
            try
            {
                List<Guid> EsAppIdList = MallApply.ObjectSet().Where(p => p.AppId == AppId).Select(p => p.EsAppId).ToList();
                Guid YJAppid = new Guid("8b4d3317-6562-4d51-bef1-0c05694ac3a6");
                if (EsAppIdList.Contains(YJAppid))
                {
                    return YJAppid;
                }
                else
                {
                    return EsAppIdList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据appid获取馆id服务异常。AppId{0},datetime：{1}", AppId, DateTime.Now), ex);
                return new Guid();
            }
        }
        /// <summary>
        /// 根据AppId获取审核方式
        /// </summary>
        /// <returns></returns>
        public bool IsAutoModeStatusExt(System.Guid EsAppId)
        {
            try
            {
                bool flag = false;
                //根据EsAppId获取馆信息
                int AuditState = AuditMode.ObjectSet().Where(p => p.AppId == EsAppId).Select(p => p.ModeStatus).FirstOrDefault();
                if (AuditState == 0)  //状态为0   自动审核  否则手动审核
                {
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("根据AppId获取审核方式服务异常。EsAppId{0} datetime：{1}", EsAppId, DateTime.Now), ex);
                return false;
            }
        }
        /// <summary>
        /// 判断馆或店铺是否需要审核
        /// </summary>
        /// <returns></returns>
        public bool IsNeedAuditExt(System.Guid EsAppId)
        {
            try
            {
                bool flag = false;
                //根据EsAppId获取馆信息
                var AuditModeList = AuditMode.ObjectSet().Where(p => p.AppId == EsAppId).ToList();
                if (AuditModeList.Any() && AuditModeList.Count > 0)
                {
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("判断馆或店铺是否需要审核服务异常。EsAppId{0} datetime：{1}", EsAppId, DateTime.Now), ex);
                return false;
            }
        }
        /// <summary>
        /// 取出最后提交的待审核商品信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.AuditCommodityDTO GetApplyCommodityInfoExt(System.Guid CommodityId, System.Guid AppId)
        {
            try
            {
                var AuditCom = (from m in AuditCommodity.ObjectSet()
                                join n in AuditManage.ObjectSet() on m.Id equals n.Id
                                where m.CommodityId == CommodityId & m.AppId == AppId & !m.IsDel & n.Status == 0 & n.Action != 0 & n.Action != 1
                                orderby n.ApplyTime descending
                                select m).FirstOrDefault();
                return AuditCom.ToEntityData();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("取出最后提交的待审核商品信息服务异常。CommodityId{0},AppId：{1}", CommodityId, AppId), ex);
                return new AuditCommodityDTO();
            }
        }
        /// <summary>
        /// 判断商品是否存在
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <returns></returns>
        public bool IsExistComExt(System.Guid CommodityId, System.Guid AppId)
        {
            bool bReturn = false;
            if (CommodityId != Guid.Empty && AppId != Guid.Empty)
            {
                //商品表中检查编号是否存在
                var commodity = Commodity.ObjectSet()
                        .Where(n => n.Id == CommodityId && n.AppId == AppId)
                        .FirstOrDefault();
                bReturn = (null == commodity);
            }
            return bReturn;
        }
        /// <summary>
        /// 审核通过后新发布商品
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO AddCommodity(System.Guid Id)
        {
            try
            {
                LogHelper.Debug("AuditCommodityBP.AddCommodity,入参：{0}", Id.ToString());
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //取出商品详情
                AuditCommodity AuditCom = AuditCommodity.ObjectSet().Where(P => P.Id == Id).FirstOrDefault();
                //取出属性表数据
                List<AuditCommodityStock> StockList = AuditCommodityStock.ObjectSet().Where(p => p.AuditId == Id).ToList();
                //插入到商品表
                string strsql = string.Format(@"INSERT INTO Commodity (	Id,Name,Code,No_Number,SubId,Price,Stock,PicturesPath,Description,State,IsDel,AppId,No_Code,TotalCollection,
	                            TotalReview,Salesvolume,ModifiedOn,GroundTime,ComAttribute,CategoryName,SortValue,FreightTemplateId,MarketPrice,IsEnableSelfTake,
	                            Weight,PricingMethod,SaleAreas,SharePercent,CommodityType,HtmlVideoPath,MobileVideoPath,VideoPic,VideoName,ScorePercent,Duty,	SpreadPercent,
	                            ScoreScale,TaxRate,TaxClassCode,Unit,InputRax,Barcode,JDCode,CostPrice,IsAssurance,TechSpecs,	SaleService,IsReturns,ServiceSettingId,
	                            Type,YJCouponActivityId,YJCouponType,SubTime,ModifieId) 
                               SELECT
	                            CommodityId,Name,Code,No_Number,SubId,Price,Stock,PicturesPath,Description,State,IsDel,AppId,No_Code,TotalCollection,	TotalReview,
	                            Salesvolume,ModifiedOn,GroundTime,ComAttribute,CategoryName,SortValue,FreightTemplateId,MarketPrice,IsEnableSelfTake,Weight,PricingMethod,
	                            SaleAreas,SharePercent,CommodityType,HtmlVideoPath,MobileVideoPath,VideoPic,VideoName,ScorePercent,Duty,SpreadPercent,ScoreScale,TaxRate,
	                            TaxClassCode,Unit,InputRax,Barcode,JDCode,CostPrice,IsAssurance,TechSpecs,SaleService,IsReturns,ServiceSettingId,Type,YJCouponActivityId,
	                            YJCouponType,SubTime,ModifieId FROM
	                            AuditCommodity
                            WHERE Id ='{0}';", AuditCom.Id);
                LogHelper.Debug("AuditCommodityBP.AddCommodity,执行sql：{0}", strsql);
                Commodity.ObjectSet().Context.ExecuteStoreCommand(strsql);
                //有属性的商品插入到商品属性表中
                if (StockList != null && StockList.Count > 0)
                {
                    foreach (var item in StockList)
                    {
                        CommodityStock ComStock = CommodityStock.CreateCommodityStock();
                        ComStock.Id = item.Id;
                        ComStock.CommodityId = AuditCom.CommodityId;
                        ComStock.ComAttribute = item.ComAttribute;
                        ComStock.Price = item.Price;
                        ComStock.Stock = item.Stock;
                        ComStock.MarketPrice = item.MarketPrice;
                        ComStock.SubTime = item.SubTime;
                        ComStock.ModifiedOn = item.ModifiedOn;
                        ComStock.Duty = item.Duty;
                        ComStock.Barcode = item.Barcode;
                        ComStock.No_Code = item.No_Code;
                        ComStock.JDCode = item.JDCode;
                        ComStock.CostPrice = item.CostPrice;
                        ComStock.ThumImg = item.ThumImg;
                        ComStock.CarouselImgs = item.CarouselImgs;
                        contextSession.SaveObject(ComStock);
                    }
                }
                contextSession.SaveChanges();
                #region 取出新增的商品变动明细插入CommodityChange
                List<System.Guid> ids = new List<Guid>();
                ids.Add(AuditCom.CommodityId);
                SaveCommodityChange(ids);
                #endregion
                return new ResultDTO { ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("审核通过后添加商品服务异常。id：{0}", Id), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }
        /// <summary>
        /// 编辑商品
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="commodityId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ResultDTO UpdateCommodity(System.Guid Id, System.Guid commodityId, Guid appId)
        {
            try
            {
                //取出商品详情
                CommodityAndCategoryDTO commodityAndCategoryDTO = GetAuditCommodityExt(Id, commodityId, appId);
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
                bool isPrice = false;
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Commodity commodityDTO = Commodity.ObjectSet()
                    .FirstOrDefault(n => n.Id == commodityAndCategoryDTO.CommodityId
                        && n.AppId == commodityAndCategoryDTO.AppId && n.CommodityType == 0);

                if (commodityDTO == null)
                {
                    return new ResultDTO { ResultCode = 3, Message = "未找到该商品！" };
                }
                //编辑添加商品审核已进行商品编号验证
                //if (!string.IsNullOrWhiteSpace(commodityAndCategoryDTO.No_Code) && commodityAndCategoryDTO.No_Code != commodityDTO.No_Code)
                //{
                //    if (!IsExistsExt(commodityAndCategoryDTO.No_Code, commodityAndCategoryDTO.AppId))
                //    {
                //        return new ResultDTO { ResultCode = 2, Message = "商品编号不能重复，该编号已存在" };
                //    }
                //}

                if (commodityDTO.Price > commodityAndCategoryDTO.Price)
                {
                    isPrice = true;
                }
                //2017-11-20新增
                commodityDTO.Type = commodityAndCategoryDTO.CommodityType;
                commodityDTO.YJCouponActivityId = commodityAndCategoryDTO.YJCouponActivityId;
                commodityDTO.YJCouponType = commodityAndCategoryDTO.YJCouponType;
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
                commodityDTO.CostPrice = commodityAndCategoryDTO.CostPrice;
                commodityDTO.IsAssurance = commodityAndCategoryDTO.IsAssurance;
                commodityDTO.IsReturns = commodityAndCategoryDTO.IsReturns;
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
                    LogHelper.Debug("AuditCommodityBP.UpdateCommodity老的商品属性删除成功");
                }

                //zgx-modify
                List<ComAttributeDTO> allAttrList = new List<ComAttributeDTO>();
                int StockTotal = 0;
                //判断是否有组合属性
                if (commodityAndCategoryDTO.ComAttributes != null && commodityAndCategoryDTO.ComAttributes.Count > 0)
                {
                    LogHelper.Debug("UpdateCommodity添加新商品属性参数" + JsonHelper.JsonSerializer(commodityAndCategoryDTO.ComAttributes));
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
                #endregion
                commodityDTO.ComAttribute = JsonHelper.JsonSerializer<List<ComAttributeDTO>>(allAttrList);
                commodityDTO.EntityState = System.Data.EntityState.Modified;
                contextSession.SaveObject(commodityDTO);
                CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodityDTO);
                contextSession.SaveObject(journal);
                var commodityEntityState = commodityDTO.EntityState;
                commodityDTO.RefreshCache(commodityEntityState);

                int count = contextSession.SaveChanges();

                //更新商品变动表数据
                List<System.Guid> ids = new List<Guid>();
                ids.Add(commodityAndCategoryDTO.CommodityId);
                SaveCommodityChange(ids);

                if (count > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 1, Message = "False" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("编辑商品服务异常。id：{0}，name：{1},appId{2}", Id, commodityId, appId), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }
        /// <summary>
        /// 上架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public ResultDTO OnSale(Guid id, Guid ModifieId, DateTime ModifieOn)
        {
            List<Guid> ids = new List<Guid>();
            ids.Add(id);
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
                    commodity.ModifiedOn = ModifieOn;
                    commodity.ModifieId = ModifieId;//修改人id
                    commodity.EntityState = System.Data.EntityState.Modified;
                    commodity.State = 0;
                    commodity.GroundTime = DateTime.Now;
                    needRefreshCacheCommoditys.Add(commodity);

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
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
        /// <summary>
        /// 修改商品名称
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="name">商品名称</param>
        public ResultDTO UpdateComName(System.Guid id, string name, Guid ModifieId, DateTime ModifieOn)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.EntityState = System.Data.EntityState.Modified;
                    commodity.ModifiedOn = ModifieOn;
                    commodity.ModifieId = ModifieId;//修改人id
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
        /// <summary>
        ///编辑商品类别
        /// </summary>
        /// <param name="uCategoryVM">编辑商品分类VM</param>
        public ResultDTO UpdateCategoryBycommodityId(Guid id, string ComCateIds)
        {
            UCategoryVM uCategoryVM = new UCategoryVM();
            uCategoryVM.CommodityId = id;
            uCategoryVM.ComCateIds = ComCateIds;
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
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="price">价格</param>
        public ResultDTO UpdatePrice(System.Guid id, decimal price, Guid ModifieId, DateTime ModifieOn)
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
                    commodity.ModifiedOn = ModifieOn;
                    commodity.ModifieId = ModifieId;//修改人id
                    commodity.EntityState = EntityState.Modified;

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    //修改库存表中商品价格
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.Price = price;
                        ComStock.ModifiedOn = ModifieOn;
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
        /// <summary>
        /// 修改市场价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="marketPrice">市场价</param>
        public ResultDTO UpdateMarketPrice(System.Guid id, decimal? marketPrice, Guid ModifieId, DateTime ModifieOn)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.MarketPrice = marketPrice;
                    commodity.ModifiedOn = ModifieOn;
                    commodity.ModifieId = ModifieId;//修改人id
                    commodity.EntityState = EntityState.Modified;
                    //修改库存表中商品市场价
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.MarketPrice = marketPrice;
                        ComStock.ModifiedOn = ModifieOn;
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
        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="stock">库存</param>
        public ResultDTO UpdateStock(System.Guid id, int stock, Guid ModifieId, DateTime ModifieOn)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.EntityState = System.Data.EntityState.Modified;
                    commodity.ModifiedOn = ModifieOn;
                    commodity.ModifiedId = ModifieId;//修改人id
                    commodity.Stock = stock;

                    CommodityJournal journal = CommodityJournal.CreateCommodityJournal(commodity);
                    contextSession.SaveObject(journal);
                    //修改库存表中商品库存
                    var ComStock = CommodityStock.ObjectSet().FirstOrDefault(p => p.CommodityId == id);
                    if (ComStock != null)
                    {
                        ComStock.Stock = stock;
                        ComStock.ModifiedOn = ModifieOn;
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
        /// <summary>
        /// 修改销量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="salesvolume"></param>
        /// <returns></returns>
        public ResultDTO UpdateSalesvolume(System.Guid id, int salesvolume, Guid ModifieId, DateTime ModifieOn)
        {
            try
            {

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var commodity = Commodity.ObjectSet().FirstOrDefault(n => n.Id == id && n.CommodityType == 0);
                if (commodity != null)
                {
                    commodity.Stock = commodity.Stock - salesvolume;
                    commodity.Salesvolume = commodity.Salesvolume + salesvolume;
                    commodity.ModifiedOn = ModifieOn;
                    commodity.ModifiedId = ModifieId;//修改人id
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
                //将修改价格商品插入Commoditychange表
                List<System.Guid> ids = new List<Guid>();
                ids.Add(id);
                SaveCommodityChange(ids);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改库存服务异常。id：{0}，salesvolume：{1}", id, salesvolume), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 修的的数据插入commoditychange表中
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
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
                                                                                      ServiceSettingId = n.ServiceSettingId,
                                                                                      Type = n.Type,
                                                                                      YJCouponActivityId = n.YJCouponActivityId,
                                                                                      YJCouponType = n.YJCouponType,
                                                                                      SubOn = n.SubTime,
                                                                                      ModifiedId = this.ContextDTO.LoginUserID
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
                                                                                      ModifiedOn = m.ModifiedOn,
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
                                                                                      ServiceSettingId = n.ServiceSettingId,
                                                                                      Type = n.Type,
                                                                                      YJCouponActivityId = n.YJCouponActivityId,
                                                                                      YJCouponType = n.YJCouponType,
                                                                                      SubOn = n.SubTime,
                                                                                      ModifiedId = this.ContextDTO.LoginUserID
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

    }
}
