using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using Jinher.AMP.BTP.Deploy;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using Jinher.AMP.BTP.Common;
using System.IO;
using Jinher.JAP.Common.Loging;
using NPOI.SS.UserModel;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 易捷员工管理
    /// </summary>
    public class YJEmployeeController : BaseController
    {
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            ViewBag.AppId = appId;
            //所属区域信息
            var facade = new YJEmployeeFacade();
            var AreaList = facade.GetAreaInfo(appId);
            ViewBag.AreaList = JsonHelper.JsonSerializer(AreaList);
            return View();
        }

        [HttpPost]
        public ActionResult GetData(YJEmployeeSearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.AppId = appId;
            var facade = new YJEmployeeFacade();
            return Json(facade.GetYJEmployeeList(dto));
        }

        public ActionResult Details(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            ViewBag.AppId = appId;
            ViewBag.Id = id;
            return View();
        }

        public ActionResult GetDeatils(Guid id)
        {
            var facade = new YJEmployeeFacade();
            return Json(facade.GetYJEmployeeInfo(id), JsonRequestBehavior.AllowGet);
        }

        [CheckAppId]
        public ActionResult Create()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(YJEmployeeDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.AppId = appId;
            var facade = new YJEmployeeFacade();
            return Json(facade.AddYJEmployee(dto));
        }
        [HttpPost]
        public ActionResult UpdateUserId()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            var facade = new YJEmployeeFacade();
            return Json(facade.UpdataYJEmployeeInfo());
        }

        [CheckAppId]
        public ActionResult Update(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            var facade = new YJEmployeeFacade();
            ViewBag.YJEmployeeInfo = facade.GetYJEmployeeInfo(id);
            ViewBag.AppId = appId;
            return View();
        }

        [HttpPost]
        public ActionResult Update(YJEmployeeDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            dto.AppId = appId;
            var facade = new YJEmployeeFacade();
            return Json(facade.UpdateYJEmployee(dto));
        }


        public ActionResult Delete(Guid Id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            var facade = new YJEmployeeFacade();
            return Json(facade.DelYJEmployee(Id));
        }

        public ActionResult DeleteAll(List<Guid> Ids)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (ContextDTO.LoginUserID == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            }
            var facade = new YJEmployeeFacade();
            return Json(facade.DelYJEmployeeAll(Ids));
        }
        /// <summary>
        /// 导出数据
        /// </summary>
        [HttpGet]
        public ActionResult ExportYJEmployee(string UserAccount, string UserName, string Area, string Phone, string StationName, string StationCode, string IdentityNum, string IsManager, string Department, string Station)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            YJEmployeeSearchDTO dto = new YJEmployeeSearchDTO();
            dto.AppId = appId;
            dto.UserAccount = UserAccount;
            dto.UserName = UserName;
            dto.Area = Area;
            dto.Phone = Phone;
            dto.StationName = StationName;
            dto.StationCode = StationCode;
            dto.IdentityNum = IdentityNum;
            dto.IsManager = Convert.ToInt32(IsManager);
            dto.Department = Department;
            dto.Station = Station;
            var facade = new YJEmployeeFacade();
            var result = facade.ExportYJEmployeeList(dto);
            if (!result.isSuccess)
            {
                return Json(result);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("登录帐号（即注册手机号码）", typeof(string));
            dt.Columns.Add("所在分公司", typeof(string));
            dt.Columns.Add("站编号", typeof(string));
            dt.Columns.Add("站名称", typeof(string));
            dt.Columns.Add("姓名", typeof(string));
            dt.Columns.Add("身份证号", typeof(string));
            dt.Columns.Add("联系电话", typeof(string));
            dt.Columns.Add("是否管理岗", typeof(string));
            dt.Columns.Add("部门", typeof(string));
            dt.Columns.Add("岗位", typeof(string));
            foreach (var d in result.Data.List)
            {
                dt.Rows.Add(d.UserAccount, d.Area, d.StationCode, d.StationName, d.UserName, d.IdentityNum, d.Phone, d.IsManager == 1 ? "是" : "否", d.Department, d.Station);
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        /// <summary>
        /// 导出当次导入的无效员工信息
        /// </summary>
        [HttpGet]
        public ActionResult ExportInvalidData(string UserAccount)
        {
            List<string> UserAccounts = new List<string>(UserAccount.Split(','));
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            var facade = new YJEmployeeFacade();
            var result = facade.ExportInvalidData(UserAccounts, appId);
            if (!result.isSuccess)
            {
                return Json(result);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("登录帐号（即注册手机号码）", typeof(string));
            dt.Columns.Add("所在分公司", typeof(string));
            dt.Columns.Add("站编号", typeof(string));
            dt.Columns.Add("站名称", typeof(string));
            dt.Columns.Add("姓名", typeof(string));
            dt.Columns.Add("身份证号", typeof(string));
            dt.Columns.Add("联系电话", typeof(string));
            dt.Columns.Add("是否管理岗", typeof(string));
            dt.Columns.Add("部门", typeof(string));
            dt.Columns.Add("岗位", typeof(string));
            foreach (var d in result.Data.List)
            {
                dt.Rows.Add(d.UserAccount, d.Area, d.StationCode, d.StationName, d.UserName, d.IdentityNum, d.Phone, d.IsManager == 1 ? "是" : "否", d.Department, d.Station);
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        /// <summary>
        /// 导出全部导入的无效员工信息
        /// </summary>
        /// <param name="UserAccount"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportAllInvalidData()
        {            
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            var facade = new YJEmployeeFacade();
            var result = facade.ExportInvalidDataByAppid(appId);
            if (!result.isSuccess)
            {
                return Json(result);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("登录帐号（即注册手机号码）", typeof(string));
            dt.Columns.Add("所在分公司", typeof(string));
            dt.Columns.Add("站编号", typeof(string));
            dt.Columns.Add("站名称", typeof(string));
            dt.Columns.Add("姓名", typeof(string));
            dt.Columns.Add("身份证号", typeof(string));
            dt.Columns.Add("联系电话", typeof(string));
            dt.Columns.Add("是否管理岗", typeof(string));
            dt.Columns.Add("部门", typeof(string));
            dt.Columns.Add("岗位", typeof(string));
            foreach (var d in result.Data.List)
            {
                dt.Rows.Add(d.UserAccount, d.Area, d.StationCode, d.StationName, d.UserName, d.IdentityNum, d.Phone, d.IsManager == 1 ? "是" : "否", d.Department, d.Station);
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        #region
        public ActionResult ImportExcel()
        {
            var appId = (Guid)Session["APPID"];
            var file = Request.Files["file"];
            if (file == null) return Json(new { Success = false, Messages = "请选择文件~" });
            string excelType = GetExcelFileType(file.FileName).ToString();
            if (excelType != "xlsx" && excelType != "xls")
            {
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { Data = new YJEmployeeSearchDTO(), ResultCode = 2, isSuccess = false, Message = "只能上传.xlsx格式文件" });
            }
            var dt = ExcelHelper.Import(file.InputStream, excelType);
            if (!(dt.Columns.Contains("登录帐号（即注册手机号码)") && dt.Columns.Contains("所在分公司") && dt.Columns.Contains("站编号") && dt.Columns.Contains("站名称") && dt.Columns.Contains("姓名") && dt.Columns.Contains("身份证号") && dt.Columns.Contains("联系电话")))
            {
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { Data = new YJEmployeeSearchDTO(), ResultCode = 2, isSuccess = false, Message = "上传文件模板错误~" });
            }
            YJEmployeeSearchDTO YJEmpl = new YJEmployeeSearchDTO();

            try
            {
                #region 1.获取Excel文件并转换为一个List集合
                #endregion
                // 1.2解析文件，存放到一个List集合里
                List<YJEmployeeDTO> enlist = new List<YJEmployeeDTO>();
                foreach (DataRow d in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(d["登录帐号（即注册手机号码)"].ToString()))
                    {
                        var e = new YJEmployeeDTO { UserAccount = d["登录帐号（即注册手机号码）"].ToString(), UserName = d["姓名"].ToString(), IdentityNum = d["身份证号"].ToString(), Phone = d["联系电话"].ToString(), Area = d["所在分公司"].ToString(), StationCode = d["站编号"].ToString(), StationName = d["站名称"].ToString(), AppId = appId, IsManager = d["是否管理岗"].ToString() == "是" ? 1 : 2, Department = d["部门"].ToString(), Station = d["岗位"].ToString() };
                        enlist.Add(e);
                    }
                }
                if (enlist.Count == 0)
                {
                    return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { Data = new YJEmployeeSearchDTO(), ResultCode = 2, isSuccess = false, Message = "上传Excel无有效数据,请核实后重新上传~" });
                }
                var facade = new YJEmployeeFacade();
                return Json(facade.ImportYJEmployeeList(enlist, appId));
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeController.ImportExcel 异常", ex);
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { Data = YJEmpl, ResultCode = 2, isSuccess = false, Message = "服务异常,稍候再试~" });
            }
        }
        #endregion
        private enum ExcelExtType
        {
            xls,
            xlsx,
        }
        private static Nullable<ExcelExtType> GetExcelFileType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (!string.IsNullOrWhiteSpace(ext) && (ext.ToLower() == ".xls" || ext.ToLower() == ".xlsx"))
                return ext.ToLower() == ".xls" ? ExcelExtType.xls : ExcelExtType.xlsx;
            else
                return null;
        }
        /// <summary>
        /// 更新加油站信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateYJstation()
        {
            var facade = new YJEmployeeFacade();
            var result = facade.UpdataStationNameByCode();
            if (result.isSuccess)
            {
                return Content("ok");
            }
            else
            {
                return Content("No");
            }
        }
        /// <summary>
        /// 更新员工编码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateUserCode()
        {
            var facade = new YJEmployeeFacade();
            var result = facade.UpdataUserCodeByUserAccount();
            if (result.isSuccess)
            {
                return Content("ok");
            }
            else
            {
                return Content("No");
            }
        }
    }
}
