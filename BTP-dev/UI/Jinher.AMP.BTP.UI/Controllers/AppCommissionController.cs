using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.Deploy.CustomDTO.MallApply;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Controllers
{
    //用户佣金查看
    public class AppCommissionController : BaseController
    {
        public ActionResult Index()
        {
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }

        public ActionResult SellerIndex()
        {
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }

        /// <summary>
        ///  获取历史佣金数据列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBaseCommissionlist(BaseCommissionDTO search)
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
        ///  获取类目佣金数据列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCategoryCommissionlist(CategoryCommissionDTO search, Guid EsAppId)
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
        ///  获取商城佣金数据列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommodityCommissionlist(CommodityCommissionDTO search, Guid EsAppId)
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
                    var commodity = commodityfacade.GetCommodityByCategoryId(search.AppId, item.Id);
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
                            var Secondcommodity = commodityfacade.GetCommodityByCategoryId(search.AppId, Seconditem.Id);
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
                                    var Thirdcommodity = commodityfacade.GetCommodityByCategoryId(search.AppId, Thirditem.Id);
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
                            if (item.Id == _item.CommodityId)
                            {
                                Id = _item.Id.ToString();
                                Commission = _item.Commission;
                            }
                        }

                        foreach (var dicitem in dic)
                        {
                            if (item.CategoryName == dicitem.Value)
                            {
                                CategoryId = dicitem.Key;
                            }
                        }

                        objlist.Add(new
                        {
                            Id = Id,
                            CategoryId = CategoryId,
                            CommodityId = item.Id,
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
                LogHelper.Error("AppCommissionController.GetCommodityCommissionlist 异常", ex);
                throw ex;
            }
        }




        /// <summary>
        /// 获取历史佣金信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBaseCommission(BaseCommissionDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()) && (!model.Id.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("Id不能为空");
                }
                BaseCommissionFacade facade = new BaseCommissionFacade();
                var result = facade.GetBaseCommission(model.Id, model.MallApplyId);
                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 获取类目佣金信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCategoryCommission(CategoryCommissionDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()) && (!model.Id.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("Id不能为空");
                }
                CategoryCommissionFacade facade = new CategoryCommissionFacade();
                var result = facade.GetCategoryCommission(model.Id);
                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 获取商品佣金信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommodityCommission(CommodityCommissionDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()) && (!model.Id.ToString().Contains("00000000-0000-0000-0000-000000000000")))
                {
                    throw new Exception("Id不能为空");
                }
                CommodityCommissionFacade facade = new CommodityCommissionFacade();
                var result = facade.GetCommodityCommission(model.Id);
                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        /// <summary>
        /// 修改历史佣金信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateBaseCommission(BaseCommissionDTO model)
        {
            ResultDTO result = null;
            try
            {

                if (string.IsNullOrEmpty(model.EffectiveTime.ToString()))
                {
                    throw new Exception("生效时间不能为空!");
                }
                BaseCommissionFacade facade = new BaseCommissionFacade();
                BaseCommissionDTO seach = new BaseCommissionDTO();
                seach.MallApplyId = model.MallApplyId;
                var effective = facade.GetBaseCommissionList(seach);
                var checkeffective = effective.FirstOrDefault(p => p.MallApplyId == model.MallApplyId && p.EffectiveTime == model.EffectiveTime);
                if (checkeffective != null)
                {
                    throw new Exception("生效时间不能重复");
                }
                BaseCommissionDTO basecommission = new BaseCommissionDTO();
                basecommission.Id = Guid.NewGuid();
                basecommission.MallApplyId = model.MallApplyId;
                basecommission.SubTime = DateTime.Now;
                basecommission.ModifiedOn = DateTime.Now;
                basecommission.UserId = model.UserId;
                basecommission.AppName = model.AppName;
                basecommission.Commission = model.Commission;
                basecommission.EffectiveTime = model.EffectiveTime;
                basecommission.IsDel = true;
                result = facade.SaveBaseCommission(basecommission);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }


        /// <summary>
        /// 修改类目佣金信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCategoryCommission(CategoryCommissionDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrEmpty(model.EffectiveTime.ToString()))
                {
                    throw new Exception("生效时间不能为空!");
                }
                CategoryCommissionFacade facade = new CategoryCommissionFacade();
                CategoryCommissionDTO seach = new CategoryCommissionDTO();
                seach.MallApplyId = model.MallApplyId;
                var effective = facade.GetCategoryCommissionList(seach);
                var checkeffective = effective.FirstOrDefault(p => p.MallApplyId == model.MallApplyId && p.EffectiveTime == model.EffectiveTime);
                if (checkeffective != null)
                {
                    throw new Exception("生效时间不能重复");
                }
                model.Id = Guid.NewGuid();
                model.SubTime = DateTime.Now;
                model.ModifiedOn = DateTime.Now;
                result = facade.SaveCategoryCommission(model);

            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }


        /// <summary>
        /// 修改商品佣金信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateCommodityCommission(CommodityCommissionDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrEmpty(model.EffectiveTime.ToString()))
                {
                    throw new Exception("生效时间不能为空!");
                }
                CommodityCommissionFacade facade = new CommodityCommissionFacade();
                CommodityCommissionDTO seach = new CommodityCommissionDTO();
                seach.MallApplyId = model.MallApplyId;
                var effective = facade.GetCommodityCommissionList(seach);
                var checkeffective = effective.FirstOrDefault(p => p.MallApplyId == model.MallApplyId && p.EffectiveTime == model.EffectiveTime);
                if (checkeffective != null)
                {
                    throw new Exception("生效时间不能重复");
                }
                model.Id = Guid.NewGuid();
                model.SubTime = DateTime.Now;
                model.ModifiedOn = DateTime.Now;
                result = facade.SaveCommodityCommission(model);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }


        /// <summary>
        /// 删除历史基础佣金
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelBaseCommission(BaseCommissionDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()))
                {
                    throw new Exception("Id不能为空");
                }
                BaseCommissionFacade facade = new BaseCommissionFacade();
                result = facade.DelBaseCommission(model);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 0, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }


        /// <summary>
        /// 删除历史类目佣金
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelCategoryCommission(CategoryCommissionDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()))
                {
                    throw new Exception("Id不能为空");
                }
                CategoryCommissionFacade facade = new CategoryCommissionFacade();
                result = facade.DelCategoryCommission(model);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 0, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }


        /// <summary>
        /// 删除历史商品佣金
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DelCommodityCommission(CommodityCommissionDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()))
                {
                    throw new Exception("Id不能为空");
                }
                CommodityCommissionFacade facade = new CommodityCommissionFacade();
                result = facade.DelCommodityCommission(model);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 0, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }


        /// <summary>
        ///  获取商品结算价列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCommoditySettleAmountList(CommoditySettleAmountSearchDTO search)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetCommoditySettleAmount(search), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  设置商品的结算价
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetCommoditySettleAmount(CommoditySettleAmountInputDTO input)
        {
            input.UserId = ContextDTO.LoginUserID;
            input.UserName = ContextDTO.LoginUserName;
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.SetCommoditySettleAmount(input));
        }

        /// <summary>
        ///  设置商品的结算价
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommoditySettleAmountHistories(Guid commodityId, int pageIndex, int pageSize)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetCommoditySettleAmountHistories(commodityId, pageIndex, pageSize));
        }

        /// <summary>
        ///  设置商品的结算价
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteCommoditySettleAmountHistory(Guid id)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.DeleteCommoditySettleAmountHistory(id));
        }
    }
}
