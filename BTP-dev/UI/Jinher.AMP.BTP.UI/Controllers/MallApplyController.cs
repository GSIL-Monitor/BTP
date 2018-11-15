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
namespace Jinher.AMP.BTP.UI.Controllers
{
    public class MallApplyController : BaseController
    {
        /// <summary>
        /// 入驻申请页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }

        /// <summary>
        /// 商户管理
        /// </summary>
        /// <returns></returns>
        public ActionResult MallApply()
        {
            string appId = Request["appId"];
            string userId = Request["userId"];
            ViewBag.Name = "商户管理";
            ViewBag.appId = appId;
            ViewBag.userId = userId;
            return View();
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMallAppy(MallApplyDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(model.EsAppId.ToString()))
                {
                    throw new Exception("EsAppId不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.EsAppName))
                {
                    throw new Exception("EsAppName不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.AppId.ToString()))
                {
                    throw new Exception("AppId不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.UserId.ToString()))
                {
                    throw new Exception("UserId不能为空");
                }
                MallApplyFacade facade = new MallApplyFacade();
                var Ischeck = facade.GetMallApplyInfoList(new MallApplyDTO { AppId = model.AppId, EsAppId = model.EsAppId, EsAppName = model.EsAppName });
                if (Ischeck.Count()>0)
                {
                    throw new Exception("已存在该条记录");
                }
                MallApplyDTO entity = new MallApplyDTO();
                entity.Id = Guid.NewGuid();
                entity.AppId = model.AppId;
                entity.AppName = APPSV.GetAppName(model.AppId);
                entity.EsAppId = model.EsAppId;
                entity.EsAppName = model.EsAppName;
                entity.SubTime = DateTime.Now;
                entity.ModifiedOn = DateTime.Now;
                entity.UserId = model.UserId;
                entity.State = 0;
                entity.ApplyContent = null;
                entity.Type = null;
                result = facade.SaveMallApply(entity);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 0, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateState(MallApplyDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()))
                {
                    throw new Exception("Id不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.State.ToString()))
                {
                    throw new Exception("State状态值不能为空");
                }
                MallApplyFacade facade = new MallApplyFacade();
                result = facade.UpdateMallApply(model);
                MallApplyDTO entity = facade.GetMallApply(model.Id);
                #region 返回给前端的各种状态
                if (result.isSuccess == true)
                {
                    switch (model.State)
                    {
                        case 0:
                            result = new ResultDTO() { ResultCode = 0, Message = "重新申请成功", isSuccess = true };
                            break;
                        case 1:
                            result = new ResultDTO() { ResultCode = 0, Message = "提交成功", isSuccess = true };
                            break;
                        case 2:
                            if (string.IsNullOrWhiteSpace(model.ApplyContent))
                            {
                                result = new ResultDTO() { ResultCode = 0, Message = "恢复成功", isSuccess = true };
                            }
                            else
                            {
                                BaseCommissionFacade bcFace = new BaseCommissionFacade();
                                BaseCommissionDTO basecommission = new BaseCommissionDTO();
                                basecommission.Id = Guid.NewGuid();
                                basecommission.MallApplyId = model.Id;
                                basecommission.SubTime = DateTime.Now;
                                basecommission.ModifiedOn = DateTime.Now;
                                basecommission.UserId = model.UserId;
                                basecommission.AppName = model.AppName;
                                basecommission.Commission = model.Commission;
                                basecommission.EffectiveTime = DateTime.Now;
                                basecommission.IsDel = true;
                                ResultDTO resdto = bcFace.SaveBaseCommission(basecommission);
                                result = new ResultDTO() { ResultCode = 0, Message = "审核成功", isSuccess = true };
                            }
                            string str=TestHelper.AddPavilionApp(entity);
                            break;
                        case 3:
                            if (!string.IsNullOrWhiteSpace(model.ApplyContent))
                            {
                                result = new ResultDTO() { ResultCode = 0, Message = "审核成功", isSuccess = true };
                            }
                            break;
                        case 4:
                            TestHelper.delPavilionApp(entity);
                            result = new ResultDTO() { ResultCode = 0, Message = "挂起成功", isSuccess = true };
                            break;
                        case 5:
                            TestHelper.delPavilionApp(entity);
                            result = new ResultDTO() { ResultCode = 0, Message = "取消入驻成功", isSuccess = true };
                            break;
                        default:
                            break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }







        /// <summary>
        /// 修改商家类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveShangjiaType(MallApplyDTO model)
        {
            ResultDTO result = null;
            try
            {
                if (string.IsNullOrWhiteSpace(model.Id.ToString()))
                {
                    throw new Exception("Id不能为空");
                }
                if (string.IsNullOrWhiteSpace(model.Type.ToString()))
                {
                    throw new Exception("商家类型不能为空");
                }
                MallApplyFacade facade = new MallApplyFacade();
                MallApplyDTO Mallapply = facade.GetMallApply(model.Id);
                Mallapply.Type = model.Type;
                result = facade.UpdateMallApply(Mallapply);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(new { data = result });
        }



        /// <summary>
        ///  获取商城列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMallAppy(MallApplyDTO search)
        {
            try
            {
                int[] arr = { 0, 1, 2, 3, 4 };
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                MallApplyFacade facade = new MallApplyFacade();
                var result = facade.GetMallApplyInfoList(search);
                int count = result.Count;
                result = result.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                result = result.Where(p => arr.Contains(p.State)).ToList();
                return Json(new { data = result, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  获取商户列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserMallAppy(MallApplyDTO search)
        {
            try
            {
                int[] arr = { 0, 1, 2, 4 };
                int PageIndex = 1;
                int PageSize = search.PageSize;
                if (search.PageIndex != 0)
                {
                    PageIndex = (int)search.PageIndex;
                }
                MallApplyFacade facade = new MallApplyFacade();
                var result = facade.GetMallApplyInfoList(search);
                int count = result.Count;
                result = result.OrderByDescending(p => p.SubTime).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                result = result.Where(p => arr.Contains(p.State)).ToList();
                return Json(new { data = result, count = count });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //获取checkBox中的商城数据
        [HttpGet]
        public JsonResult GetShopAppy()
        {
            IList<object> objlist = new List<object>();
            ZPHSVFacade zphsvface = new ZPHSVFacade();
            var result = zphsvface.GetEsNetList();
            if (result.Count() > 0)
            {
                foreach (var item in result)
                {
                    if (item.pavilionType==Jinher.AMP.ZPH.Deploy.Enum.PavilionType.ESNet)
                    {
                        objlist.Add(new
                        {
                            EsAppId = item.ESAppId,
                            EsAppName = item.ESName
                        });
                    }
                    
                }
            }
            return Json(new { data = objlist, count = objlist.Count() }, JsonRequestBehavior.AllowGet);
        }

    }
}
