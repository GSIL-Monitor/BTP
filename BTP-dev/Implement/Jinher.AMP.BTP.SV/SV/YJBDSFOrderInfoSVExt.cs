
/***************
功能描述: BTPSV
作    者: 
创建时间: 2018/7/28 15:16:09
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.YJB.Deploy.Enums;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class YJBDSFOrderInfoSV : BaseSv, IYJBDSFOrderInfo
    {

        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO> GetDSFOrderInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInfoSearchDTO arg)
        {
            var result = new Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO>()
            {
                IsSuccess = true,
                Message = "sucess",
                Code = "200"
            };

            if (arg == null)
            {
                result.IsSuccess = false;
                result.Message = "参数信息不正确";
                result.Code = "404";

                LogHelper.Error("查询第三方订单信息发生错误:参数信息不正确");
            }

            LogHelper.Debug("查询第三方订单信息;查询依据" + JsonHelper.JsonSerializer(arg));

            try
            {
                var data = YJBDSFOrderInfo.ObjectSet().ToList();

                if (arg.OrderPayState == "0")
                {
                    data = data.Where(o => o.OrderPayState == "待支付").ToList();
                }

                var count = data.Count();

                var Data = new BE.YJBDSFOrderInfo().ToEntityDataList(data.OrderBy(o => o.SubTime).Skip((arg.PageIndex - 1) * arg.PageSize).Take(arg.PageSize).OrderByDescending(O => O.OrderPayDate).ToList<YJBDSFOrderInfo>());

                return Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO>.Success(count, Data);
                
            }
            catch (Exception ex)
            {
                result.Code = "";
                result.IsSuccess = false;
                result.Message = "fail";
                LogHelper.Error("查询第三方订单信息发生错误:参数信息:" + JsonHelper.JsonSerializer(arg), ex);
            }
            return result;
        }
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTODSFOrderInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInformationDTO model)
        {
            LogHelper.Info("开始访问第三方订单数据接口！");
            var result = new Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO()
            {
                Code = "200",
                Message = "",
                IsSuccess = true
            };

            if (model == null)
            {
                result.IsSuccess = false;
                result.Message = "参数信息不正确";
                result.Code = "404";
                LogHelper.Error("增加第三方订单数据发生错误:参数信息不正确");
            }

            try
            {
                var OrderInfo = YJBDSFOrderInfo.ObjectSet().Where(x => x.OrderNo == model.OrderNo).ToList();

                Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO dto = new YJBDSFOrderInfoDTO();
                if (OrderInfo.Any() && OrderInfo != null && OrderInfo.Count > 0)
                {
                    dto.Id = OrderInfo[0].Id;
                    dto.EntityState = System.Data.EntityState.Modified;
                }
                else
                {
                    dto.Id = Guid.NewGuid();
                    dto.EntityState = System.Data.EntityState.Added;
                }
                dto.OrderNo = model.OrderNo;
                dto.OrderPayDate = !string.IsNullOrEmpty(model.OrderPayDate) ? DateTime.Parse(model.OrderPayDate) : DateTime.Parse("1970-1-1");
                dto.OrderPayMoney = model.OrderPayMoney;
                dto.OrderPayState = model.OrderPayState;
                dto.UserID = TPS.CBCSV.GetUserAccountByPhone(model.PhoneNumber).userId;
                dto.SubId = model.UserID;
                dto.SubTime = DateTime.Now;
                dto.PlatformName = model.PlatformName;
                dto.ModifiedOn = DateTime.Now;
                if (model.Commodity.Any() && model.Commodity != null)
                {
                    foreach (var item in model.Commodity)
                    {
                        if (item.Name.Equals("商业险"))
                        {
                            item.Thumbnail = "/Images/CarInstanceBusiness.png";
                        }
                        if (item.Name.Equals("交强险"))
                        {
                            item.Thumbnail = "/Images/CarInstanceStrong.png";
                        }
                    }
                }
                dto.Commodity = JsonHelper.JsonSerializer(model.Commodity);

                var dbmodel = YJBDSFOrderInfo.FromDTO(dto);


                ContextFactory.CurrentThreadContext.SaveObject(dbmodel);
                var count = ContextFactory.CurrentThreadContext.SaveChanges();
                LogHelper.Info("增加第三方订单数据更新数据:" + count);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "fail";
                result.Code = "";
                LogHelper.Error("增加第三方订单数据发生错误:参数信息:" + JsonHelper.JsonSerializer(model), ex);
            }

            return result;
        }

        /// <summary>
        /// 获取汇款单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO> GetCarInsuranceRebateExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg) {
            var result = new Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>()
            {
                IsSuccess = true,
                Message = "sucess",
                Code = "200"
            };

            if (arg == null)
            {
                result.IsSuccess = false;
                result.Message = "参数信息不正确";
                result.Code = "404";

                LogHelper.Error("查询车险返利信息发生错误:参数信息不正确");
            }

            LogHelper.Debug("查询车险返利信息;查询依据" + JsonHelper.JsonSerializer(arg));

            try
            {
                var query = YJBCarInsuranceRebate.ObjectSet()
                    .GroupBy(x =>
                        new { x.RemittanceNo, x.RebateDate, x.RebateMoney, x.RebateNum }).Select(x =>
                    new CarInsurancePolymerizationDTO()
                    {
                        RemittanceNo = x.Key.RemittanceNo,
                        RebateDate = x.Key.RebateDate,
                        RebateMoney = x.Sum(o => o.RebateMoney),
                        RebateNum = x.Sum(o => o.RebateNum.Value),
                        AfterTaxMoney = x.Sum(o => o.RebateMoney.Value) * (decimal)1.06
                    });
                var count = query.Count();
                var Data = query.OrderByDescending(o => o.RebateDate).Skip((arg.PageIndex - 1) * arg.PageSize).Take(arg.PageSize).ToList();

                return Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>.Success(count, Data);

            }
            catch (Exception ex)
            {
                result.Code = "";
                result.IsSuccess = false;
                result.Message = "fail";
                LogHelper.Error("查询车险返利信息发生错误:参数信息:" + JsonHelper.JsonSerializer(arg), ex);
            }
            return result;
        }

        /// <summary>
        /// 插入汇款单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceRebateExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDTO model) {
            LogHelper.Info("开始访问车险返利数据接口！");
            var result = new Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO()
            {
                Code = "200",
                Message = "",
                IsSuccess = true
            };

            if (model == null)
            {
                result.IsSuccess = false;
                result.Message = "参数信息不正确";
                result.Code = "404";
                LogHelper.Error("增加车险返利数据发生错误:参数信息不正确");
            }

            try
            {
                if (model.OrderInfoList.Any() && model.OrderInfoList.Count>0)
                {
                    var RebateTime = DateTime.Parse(model.RebateDate);
                    decimal BusinessRate = 0;
                    decimal StrongRate = 0;
                    decimal CarShipRate = 0;
                    decimal TaxRate = 0;
                    foreach (var item in model.OrderInfoList)
                    {
                        BusinessRate = 0;
                        StrongRate = 0;
                        CarShipRate = 0;
                        TaxRate = 1.06M;
                        if (item != null && !string.IsNullOrEmpty(item.InsuranceCompanyCode))
                        {
                            var activity = InsuranceCompanyActivity.ObjectSet().Where(p => p.InsuranceCompanyCode == item.InsuranceCompanyCode && p.IsAvailable == 1).FirstOrDefault();
                            BusinessRate= activity.BusinessRate;
                            StrongRate = activity.StrongRate;
                            CarShipRate = activity.CarShipRate;
                            TaxRate = activity.TaxRate;
                        }
                        BTP.Deploy.YJBCarInsuranceRebateDTO dto = new BTP.Deploy.YJBCarInsuranceRebateDTO();
                        dto.Id = Guid.NewGuid();
                        dto.EntityState = System.Data.EntityState.Added;
                        dto.OrderNo = item.OrderNo;
                        dto.RebateDate = RebateTime;
                        dto.RebateNum = item.RebateNum;
                        dto.RemittanceNo = model.RemittanceNo;
                        dto.PhoneNum = item.PhoneNum;
                        dto.RebateState = (short)CarInsuranceStateEnum.NOAUDIT;
                        dto.AuditDate = null;
                        dto.InsuranceAmount = item.InsuranceAmount;
                        dto.DouRebateMoney = item.RebateMoney;
                        dto.CompanyRebateMoney = Math.Round((item.BusinessInsuranceAmount * (BusinessRate / 100) + item.StrongInsuranceAmount * (StrongRate / 100) + item.CarShipAmount * (CarShipRate / 100)) / TaxRate, 2);
                        dto.RebateMoney = item.RebateMoney + dto.CompanyRebateMoney;
                        //dto.DouRebatePersent = Math.Round(item.RebateMoney / item.InsuranceAmount,2);
                        dto.DouRebatePersent = 11.7M;
                        dto.CompanyRebatePersent = Math.Round(BusinessRate / 100, 2);
                        dto.BusinessInsuranceAmount = item.BusinessInsuranceAmount;
                        dto.StrongInsuranceAmount = item.StrongInsuranceAmount;
                        dto.InsuranceCompanyCode = item.InsuranceCompanyCode;
                        dto.CarShipAmount = item.CarShipAmount;
                        var dbmodel = YJBCarInsuranceRebate.FromDTO(dto);
                        ContextFactory.CurrentThreadContext.SaveObject(dbmodel);
                        var count = ContextFactory.CurrentThreadContext.SaveChanges();
                        var report = YJBCarInsuranceReport.ObjectSet().Where(x => x.OrderNo == item.OrderNo).FirstOrDefault();
                        if (report != null)
                        {
                            ContextSession contextSession = ContextFactory.CurrentThreadContext;
                            report.ModifiedOn = DateTime.Now;
                            report.RebateState = (short)CarInsuranceStateEnum.NOAUDIT;
                            report.EntityState = System.Data.EntityState.Modified;
                            contextSession.SaveObject(report);
                            contextSession.SaveChanges();
                        }
                        LogHelper.Info("增加车险返利数据更新数据:" + count);
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "fail";
                result.Code = "";
                LogHelper.Error("增加车险返利数据发生错误:参数信息:" + JsonHelper.JsonSerializer(model), ex);
            }

            return result;
        }

        /// <summary>
        /// 获取保险统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO> GetCarInsuranceReportExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            var result = new Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO>()
            {
                IsSuccess = true,
                Message = "sucess",
                Code = "200"
            };

            if (arg == null)
            {
                result.IsSuccess = false;
                result.Message = "参数信息不正确";
                result.Code = "404";

                LogHelper.Error("查询车险报表信息发生错误:参数信息不正确");
            }

            LogHelper.Debug("查询车险报表信息查询依据" + JsonHelper.JsonSerializer(arg));

            try
            {
                var query = YJBCarInsuranceReport.ObjectSet();
                var count = query.Count();

                var Data = new BE.YJBCarInsuranceReport().ToEntityDataList(query.OrderByDescending(o => o.InsuranceTime).Skip((arg.PageIndex - 1) * arg.PageSize).Take(arg.PageSize).ToList());

                return Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO>.Success(count, Data);

            }
            catch (Exception ex)
            {
                result.Code = "";
                result.IsSuccess = false;
                result.Message = "fail";
                LogHelper.Error("查询车险报表信息发生错误:参数信息:" + JsonHelper.JsonSerializer(arg), ex);
            }
            return result;
        }

        /// <summary>
        /// 插入保险统计报表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceReportExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO model)
        {
            LogHelper.Info("开始访问车险报表信息数据接口！");
            var result = new Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO()
            {
                Code = "200",
                Message = "",
                IsSuccess = true
            };

            if (model == null)
            {
                result.IsSuccess = false;
                result.Message = "参数信息不正确";
                result.Code = "404";
                LogHelper.Error("增加车险报表信息数据发生错误:参数信息不正确");
            }

            try
            {
                var Report = YJBCarInsuranceReport.ObjectSet().Where(x => x.OrderNo == model.OrderNo).ToList();

                YJBCarInsReportDetailDTO detaildto = new YJBCarInsReportDetailDTO();
                var DetailId = Guid.NewGuid();
                if (Report.Any() && Report != null && Report.Count > 0)
                {
                    detaildto.Id = Report[0].DetailId.Value;
                    detaildto.EntityState = System.Data.EntityState.Modified;
                }
                else
                {
                    detaildto.Id = DetailId;
                    detaildto.EntityState = System.Data.EntityState.Added;
                }
                detaildto.StrongInsuranceAmount = model.StrongInsuranceAmount;
                detaildto.BusinessAmount = model.BusinessAmount;
                //detaildto.BusinessFreeAmount = model.BusinessFreeAmount;
                detaildto.StrongInsuranceOrderId = model.StrongInsuranceOrderId;
                detaildto.BusinessStartTime = model.BusinessStartTime;
                detaildto.BusinessEndTime = model.BusinessEndTime;
                detaildto.BusinessOrderId = model.BusinessOrderId;
                detaildto.StrongInsuranceStartTime = model.StrongInsuranceStartTime;
                detaildto.StrongInsuranceEndTime = model.StrongInsuranceEndTime;
                detaildto.PlateNumber = model.PlateNumber;
                detaildto.ChassisNumber = model.ChassisNumber;
                detaildto.EngineNumber = model.EngineNumber;
                detaildto.CarTypeName = model.CarTypeName;
                detaildto.RegisterTime = model.RegisterTime;
                detaildto.IsTransfer = model.IsTransfer;
                detaildto.CarOwnerName = model.CarOwnerName;
                detaildto.CarOwnerIdType = model.CarOwnerIdType;
                detaildto.CarOwnerId = model.CarOwnerId;
                detaildto.CarOwnerAddress = model.CarOwnerAddress;
                detaildto.CarOwnerPhone = model.CarOwnerPhone;
                detaildto.PolicyHolderName = model.PolicyHolderName;
                detaildto.PolicyHolderIdType = model.PolicyHolderIdType;
                detaildto.PolicyHolderId = model.PolicyHolderId;
                detaildto.PolicyHolderPhone = model.PolicyHolderPhone;
                detaildto.PolicyHolderAddress = model.PolicyHolderAddress;
                detaildto.StrongInsurance_SI = model.StrongInsurance_SI;
                detaildto.StrongInsurance_Car = model.StrongInsurance_Car;
                detaildto.Business_Car = model.Business_Car;
                detaildto.Business_Three = model.Business_Three;
                detaildto.Business_Driver = model.Business_Driver;
                detaildto.Business_Passenger = model.Business_Passenger;
                detaildto.Business_AllCar = model.Business_AllCar;
                detaildto.Business_Glass = model.Business_Glass;
                detaildto.Business_Body = model.Business_Body;
                detaildto.Business_Engine = model.Business_Engine;
                detaildto.Business_Natural = model.Business_Natural;
                detaildto.Business_Garage = model.Business_Garage;
                detaildto.Business_Third = model.Business_Third;
                detaildto.Business_Spirit = model.Business_Spirit;
                detaildto.NoDeductibles_Car = model.NoDeductibles_Car;
                detaildto.NoDeductibles_Three = model.NoDeductibles_Three;
                detaildto.NoDeductibles_Driver = model.NoDeductibles_Driver;
                detaildto.NoDeductibles_Passenger = model.NoDeductibles_Passenger;
                detaildto.NoDeductibles_AllCar = model.NoDeductibles_AllCar;
                detaildto.NoDeductibles_Body = model.NoDeductibles_Body;
                detaildto.NoDeductibles_Engine = model.NoDeductibles_Engine;
                detaildto.NoDeductibles_Natural = model.NoDeductibles_Natural;
                detaildto.NoDeductibles__Spirit = model.NoDeductibles__Spirit;

                var detailmodel = YJBCarInsReportDetail.FromDTO(detaildto);

                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                contextSession.SaveObject(detailmodel);

                BTP.Deploy.YJBCarInsuranceReportDTO dto = new BTP.Deploy.YJBCarInsuranceReportDTO();
                if (Report.Any() && Report != null && Report.Count > 0)
                {
                    dto.Id = Report[0].Id;
                    dto.EntityState = System.Data.EntityState.Modified;
                    dto.DetailId = Report[0].DetailId;
                    dto.ModifiedOn = DateTime.Now;
                }
                else
                {
                    dto.Id = Guid.NewGuid();
                    dto.DetailId = DetailId;
                    dto.EntityState = System.Data.EntityState.Added;
                    dto.SubTime = DateTime.Now;
                }

                dto.InsuranceCompanyCode = model.InsuranceCompanyCode;
                dto.OrderNo = model.OrderNo;
                dto.MemberPhone = model.MemberPhone;
                dto.CustomPhone = model.CustomPhone;
                dto.InsuranceAmount = model.InsuranceAmount;

                dto.InsuranceTime = model.InsuranceTime;
                dto.State = model.State;
                dto.RecommendName = model.RecommendName;
                dto.RecommendAmount = model.CustomAmount;
                dto.CustomAmount = model.CustomAmount;
                dto.SinopecAmount = model.SinopecAmount;
                dto.SubId = Guid.Empty;
                dto.RebateState = 0;

                var dbmodel = YJBCarInsuranceReport.FromDTO(dto);

                contextSession.SaveObject(dbmodel);
                var count = contextSession.SaveChanges();
                LogHelper.Info("增加车险报表信息数据更新数据:" + count);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "fail";
                result.Code = "";
                LogHelper.Error("增加车险报表信息数据发生错误:参数信息:" + JsonHelper.JsonSerializer(model), ex);
            }

            return result;
        }
    }
}