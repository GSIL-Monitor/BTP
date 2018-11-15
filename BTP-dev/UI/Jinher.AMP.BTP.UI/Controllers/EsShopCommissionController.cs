using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO.MallApply;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;


namespace Jinher.AMP.BTP.UI.Controllers
{   
    //商城佣金查看
    public class EsShopCommissionController : BaseController
    {

        public ActionResult Index()
        {
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }


        /// <summary>
        ///  基础佣金数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBaseCommission(BaseCommissionDTO search)
        {
            try
            {
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                BaseCommissionFacade facade = new BaseCommissionFacade();
                var result = facade.GetBaseCommissionList(search);
                int count = result.Count;
                result = result.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                return Json(new { data = result, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  获取历史类目佣金数据列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GethistoryCategoryCommissionlist(CategoryCommissionDTO search)
        {
            try
            {
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                CategoryCommissionFacade facade = new CategoryCommissionFacade();
                var result = facade.GetCategoryCommissionList(search);
                int count = result.Count;
                result = result.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                return Json(new { data = result, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        ///  获取历史商品佣金数据列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GethistoryCommodityCommissionlist(CommodityCommissionDTO search)
        {
            try
            {
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                CommodityCommissionFacade facade = new CommodityCommissionFacade();
                var result = facade.GetCommodityCommissionList(search);
                int count = result.Count;
                result = result.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                return Json(new { data = result, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        ///  类目佣金数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCategoryCommission(CategoryCommissionDTO search,Guid EsAppId)
        {
            try
            {
                List<object> objlist = new List<object>();
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                CategoryCommissionFacade facade = new CategoryCommissionFacade();
                CategoryFacade categoryfacade = new CategoryFacade();
                var category = categoryfacade.GetCategories(EsAppId);
                category = category.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                int count = category.Count;
                var result = facade.GetCategoryCommissionList(search);
                result = result.GroupBy(p => p.CategoryId).Select(p => p.OrderByDescending(t => t.SubTime).FirstOrDefault()).ToList();
                if (category.Count() > 0)
                {
                    foreach (var item in category)
                    {
                        decimal Commission = 0;
                        string MallApplyId = null;
                        string Id = null;
                        foreach (var _item in result)
                        {
                            if (item.Id == _item.CategoryId)
                            {
                                Id = _item.Id.ToString();
                                Commission = _item.Commission;
                                MallApplyId = _item.MallApplyId.ToString();
                            }
                        }
                        objlist.Add(new
                        {
                            Id = Id,
                            CategoryId = item.Id,
                            Name = item.Name,
                            Commission = Commission,
                            MallApplyId = MallApplyId
                        });
                    }
                }
                return Json(new { data = objlist, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  商城佣金数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommodityCommission(CommodityCommissionDTO search, Guid EsAppId)
        {
            try
            {
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                CategoryFacade categoryfacade = new CategoryFacade();
                CommodityFacade commodityfacade = new CommodityFacade();
                var category = categoryfacade.GetCategories(EsAppId);
                List<Jinher.AMP.BTP.Deploy.CommodityDTO> commoditylist = new List<Jinher.AMP.BTP.Deploy.CommodityDTO>();
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var item in category)
                {
                    if (!dic.Keys.Contains(item.Id.ToString()))
                    {   
                        dic.Add(item.Id.ToString(), item.Name);
                    }
                    //先查一级分类
                    var commodity = commodityfacade.GetCommodityByCategoryId(EsAppId, item.Id);
                    if (commodity.Count() > 0)
                    {
                        commodity.ForEach(p =>
                        {
                            p.CategoryName = item.Name;
                        });
                        commoditylist.AddRange(commodity);
                    }
                    if (item.SecondCategory.Count() > 0)
                    {
                        foreach (var Seconditem in item.SecondCategory)
                        {
                            //查二级分类
                            var Secondcommodity = commodityfacade.GetCommodityByCategoryId(EsAppId, Seconditem.Id);
                            if (Secondcommodity.Count() > 0)
                            {
                                Secondcommodity.ForEach(p =>
                                {
                                    p.CategoryName = item.Name;
                                });
                                commoditylist.AddRange(Secondcommodity);
                            }
                            if (Seconditem.ThirdCategory.Count() > 0)
                            {
                                foreach (var Thirditem in Seconditem.ThirdCategory)
                                {
                                    var Thirdcommodity = commodityfacade.GetCommodityByCategoryId(EsAppId, Thirditem.Id);
                                    if (Thirdcommodity.Count() > 0)
                                    {
                                        Thirdcommodity.ForEach(p =>
                                        {
                                            p.CategoryName = item.Name;
                                        });
                                        commoditylist.AddRange(Thirdcommodity);
                                    }
                                }
                            }
                        }
                    }

                }
                //去除重复项
                List<Jinher.AMP.BTP.Deploy.CommodityDTO> _commoditylist = new List<Jinher.AMP.BTP.Deploy.CommodityDTO>();
                if (!string.IsNullOrWhiteSpace(search.CommodityName))
                {
                    commoditylist = commoditylist.Where(p => p.Name.Contains(search.CommodityName.Trim())).ToList();
                }
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                foreach (var item in commoditylist)
                {
                    if (!_dic.Keys.Contains(item.Name))
                    {
                        _dic.Add(item.Name, item.Id.ToString());
                        _commoditylist.Add(item);
                    }
                }
                int count = _commoditylist.Count();
                _commoditylist = _commoditylist.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                List<object> objlist = new List<object>();
                CommodityCommissionFacade facade = new CommodityCommissionFacade();
                var result = facade.GetCommodityCommissionList(search);
                result = result.GroupBy(p => p.CommodityId).Select(p => p.OrderByDescending(t => t.SubTime).FirstOrDefault()).ToList();
                if (_commoditylist.Count() > 0)
                {
                    foreach (var item in _commoditylist)
                    {
                        string Id = null;
                        decimal Commission = 0;
                        string MallApplyId = null;
                        string CategoryId = null;
                        foreach (var _item in result)
                        {
                            if (item.Id==_item.CommodityId)
                            {
                                Id = _item.Id.ToString();
                                Commission = _item.Commission;
                            }
                        }

                        foreach (var dicitem in dic)
                        {
                            if (item.CategoryName==dicitem.Value)
                            {
                                CategoryId = dicitem.Key;
                            }
                        }

                        objlist.Add(new {
                            Id=Id,
                            CategoryId = CategoryId,
                            CommodityId= item.Id,
                            CommodityName = item.Name,
                            CategoryName = item.CategoryName,
                            Commission = Commission,
                            MallApplyId = MallApplyId
                        });
                    }
                }

                return Json(new { data = objlist, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
