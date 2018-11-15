using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class SecondAttributeController : Controller
    {

        [CheckAppId]
        public ActionResult SizeIndex()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            Guid sizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            SecondAttributeFacade com = new SecondAttributeFacade();
            List<ColorAndSizeAttributeVM> colorAndSizeAttributeList = com.GetAttributeBySellerID(appId, sizeId);
            ViewBag.SizeList = colorAndSizeAttributeList;
            return View();
        }
        [HttpPost]
        [CheckAppId]
        public ActionResult AddSize(FormCollection collection)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            Guid sizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            string sizes = collection["sizes"];
            SecondAttributeFacade com = new SecondAttributeFacade();

            string[] sizeArray = sizes.Split(new char[] { ',', ';', '，', '；' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                //foreach (var item in sizeArray)
                //{
                //    if (com.IsExists(item, appId, sizeId))//true 表示不存在
                //    {
                //        ResultDTO result = com.AddSecondAttribute(sizeId, item, appId);
                //    }
                //}

                //获取数据库中已经存在的尺寸列表
                List<ColorAndSizeAttributeVM> list = com.GetAttributeBySellerID(appId, sizeId);
                List<string> existcolors = (from l in list
                                            where sizeArray.Contains(l.SecondAttributeName)
                                            select l.SecondAttributeName).ToList();

                //新添加的尺寸在数据库中存在，返回报错，否则新增到数据库
                if (existcolors.Count() > 0)
                {
                    string resultStr = "";
                    for (int i = 0; i < existcolors.Count(); i++)
                    {
                        if (i > 0)
                        {
                            resultStr += "、" + existcolors[i];
                        }
                        else
                        {
                            resultStr += existcolors[i];
                        }
                    }

                    return Json(new { Result = false, Messages = string.Format("尺寸{0}已经存在，请重新输入", resultStr) });
                }
                else
                {
                    ResultDTO result = com.AddSecondAttribute(sizeId, string.Join(",", sizeArray), appId);
                }
                return Json(new { Result = true, Messages = "添加成功" });
            }
            catch
            {
                return Json(new { Result = false, Messages = "添加失败" });
            }
            //bool res = com.IsExists(sizes, appId);
            //if (!res) 
            //{
            //    return Json(new { Result = false, Messages = "已存在属性" });
            //}
            //ResultDTO result = com.AddSecondAttribute(sizeId, sizes, appId);
            //if (result.ResultCode == 0)
            //{
            //    return Json(new { Result = true, Messages = "添加成功" });
            //}
            //else 
            //{
            //    return Json(new { Result = false, Messages = "添加失败" });
            //}
        }

        [HttpPost]
        [CheckAppId]
        public ActionResult DeleteAttribute(FormCollection collection)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            Guid sizeId = new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");
            string attrid = collection["attrid"];
            if (!String.IsNullOrEmpty(attrid))
            {
                SecondAttributeFacade com = new SecondAttributeFacade();
                ResultDTO result = com.DelSecondAttribute(new Guid(attrid), appId);
                if (result.ResultCode == 0)
                {
                    return Json(new { Result = true, Messages = "删除成功" });
                }
                else
                {
                    return Json(new { Result = false, Messages = "删除失败" });
                }
            }
            else
            {
                return Json(new { Result = false, Messages = "删除失败" });
            }
        }

        [CheckAppId]
        public ActionResult ColorIndex()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            Guid colorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            SecondAttributeFacade com = new SecondAttributeFacade();
            List<ColorAndSizeAttributeVM> colorAndSizeAttributeList = com.GetAttributeBySellerID(appId, colorId);
            ViewBag.ColorList = colorAndSizeAttributeList;
            return View();
        }

        [HttpPost]
        [CheckAppId]
        public ActionResult AddColor(FormCollection collection)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            Guid colorId = new Guid("324244CB-8E9F-45B3-A1E4-53FC1A25A11C");
            string colors = collection["colors"];
            SecondAttributeFacade com = new SecondAttributeFacade();
            string[] colorArray = colors.Split(new char[] { ',', ';', '，', '；' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                //foreach (var item in colorArray)
                //{
                //    if (com.IsExists(item, appId,colorId))//true 表示不存在
                //    {
                //        ResultDTO result = com.AddSecondAttribute(colorId, item, appId);
                //    }
                //}

                //获取数据库中已经存在的颜色列表
                List<ColorAndSizeAttributeVM> list = com.GetAttributeBySellerID(appId, colorId);
                List<string> existcolors = (from l in list
                                            where colorArray.Contains(l.SecondAttributeName)
                                            select l.SecondAttributeName).ToList();

                //新添加的颜色在数据库中存在，返回报错，否则新增到数据库
                if (existcolors.Count() > 0)
                {
                    string resultStr = "";
                    for (int i = 0; i < existcolors.Count(); i++)
                    {
                        if (i > 0)
                        {
                            resultStr += "、" + existcolors[i];
                        }
                        else
                        {
                            resultStr += existcolors[i];
                        }
                    }

                    return Json(new { Result = false, Messages = string.Format("颜色{0}已经存在，请重新输入", resultStr) });
                }
                else
                {
                    ResultDTO result = com.AddSecondAttribute(colorId, string.Join(",", colorArray), appId);
                }

                return Json(new { Result = true, Messages = "添加成功" });
            }
            catch
            {
                return Json(new { Result = false, Messages = "添加失败" });
            }

        }


        [CheckAppId]
        public ActionResult AttributeIndex()
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];
            string attributeValueStr = Request["AttributeId"];
            Guid attributeId;
            SecondAttributeFacade com = new SecondAttributeFacade();

            //获取属性列表 
            List<ColorAndSizeAttributeVM> attributeList = com.GetAttributeByAppID(appId);

            Guid.TryParse(attributeValueStr, out attributeId);

            //判断是否添加商品
            if (Request["addAttr"] != "1")
            {
                //如果不是添加商品，选中第一个属性
                if (attributeId == Guid.Empty)
                    attributeId = attributeList[0].AttributeId;// new Guid("844D8816-1692-45CB-9FE5-F82C061A30E7");    
            }

            List<ColorAndSizeAttributeVM> attributeValueList = com.GetAttributeBySellerID(appId, attributeId);
            ViewBag.AttributeList = attributeList;
            ViewBag.AttributeValueList = attributeValueList;
            ViewBag.AttributeId = attributeId;

            return View();
        }

        [HttpPost]
        [CheckAppId]
        public ActionResult AddAttribute(FormCollection collection)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            string attributeIdStr = collection["attributeId"];
            Guid attributeId = Guid.Empty;
            string attributeName = collection["attributeName"];
            SecondAttributeFacade com = new SecondAttributeFacade();

            if (!Guid.TryParse(attributeIdStr, out attributeId))
            {
                return Json(new { Result = false, Messages = "添加失败" });
            }

            string colors = collection["attrs"];
            string[] colorArray = colors.Split(new char[] { ',', ';', '，', '；' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {

                //获取数据库中已经存在的属性值列表
                List<ColorAndSizeAttributeVM> list = com.GetAttributeBySellerID(appId, attributeId);
                List<string> existcolors = (from l in list
                                            where colorArray.Contains(l.SecondAttributeName)
                                            select l.SecondAttributeName).ToList();

                //新添加的属性值在数据库中存在，返回报错，否则新增到数据库
                if (existcolors.Count() > 0)
                {
                    string resultStr = "";
                    for (int i = 0; i < existcolors.Count(); i++)
                    {
                        if (i > 0)
                        {
                            resultStr += "、" + existcolors[i];
                        }
                        else
                        {
                            resultStr += existcolors[i];
                        }
                    }

                    return Json(new { Result = false, Messages = string.Format("{0}{1}已经存在，请重新输入", attributeName, resultStr) });
                }
                else
                {
                    ResultDTO result = com.AddSecondAttribute(attributeId, string.Join(",", colorArray), appId);
                }

                return Json(new { Result = true, Messages = "添加成功" });
            }
            catch
            {
                return Json(new { Result = false, Messages = "添加失败" });
            }

        }

        [HttpPost]
        [CheckAppId]
        public ActionResult EidtAttributeName(FormCollection collection)
        {
            Guid appId = (Guid)System.Web.HttpContext.Current.Session["APPID"];

            string attributeIdStr = collection["attributeId"];
            Guid attributeId = Guid.Empty;
            string attributeName = collection["attributeName"];
            SecondAttributeFacade com = new SecondAttributeFacade();
            ResultDTO re = null;

            //编辑
            if (!string.IsNullOrEmpty(attributeIdStr) && Guid.TryParse(attributeIdStr, out attributeId) && attributeId != Guid.Empty)
            {
                //更新属性名称 判断属性名称是否能修改  获取属性 判断属性名称是否存在 
                re = com.UpdateAttribute(attributeId, attributeName, appId);
            }
            else //添加
            {
                attributeId = Guid.NewGuid();
                re = com.AddAttribute(attributeId, attributeName, appId);
            }

            if (re.ResultCode == 1)
            {
                return Json(new { Result = false, Messages = re.Message });
            }
            return Json(new { Result = true, Messages = "修改成功", AttributeId = attributeId });
        }

        /// <summary>
        /// 包装规格设置首页
        /// </summary>
        /// <returns></returns>
        public ActionResult SpecificationsIndex()
        {
            return View();
        }



        [HttpPost]
        public JsonResult SearchSpecifications(Jinher.AMP.BTP.Deploy.SpecificationsDTO model)
        {
            SpecificationsFacade facade = new SpecificationsFacade();
            var result = facade.GetSpecificationsList(model);
            return Json(result);
        }


        [HttpPost]
        public JsonResult AddSpecifications(Jinher.AMP.BTP.Deploy.SpecificationsDTO model)
        {
            ResultDTO result = null;
            try
            {
                SpecificationsFacade facade = new SpecificationsFacade();

                var attritue = facade.GetSpecificationsList(model);
                if (attritue.Count() > 0)
                {
                    throw new Exception("改规格系数已存在");
                }
                model.Id = Guid.NewGuid();
                model.SubTime = DateTime.Now;
                model.IsDel = false;
                model.ModifiedOn = DateTime.Now;
                result = facade.SaveSpecifications(model);
            }
            catch (Exception ex)
            {
                result = new ResultDTO() { ResultCode = 1, Message = ex.Message, isSuccess = false };
            }
            return Json(result);
        }


        [HttpPost]
        public JsonResult DelSpecifications(Jinher.AMP.BTP.Deploy.SpecificationsDTO model)
        {
            SpecificationsFacade facade = new SpecificationsFacade();
            var result = facade.Del(model.Id);
            return Json(result);
        }

    }
}
