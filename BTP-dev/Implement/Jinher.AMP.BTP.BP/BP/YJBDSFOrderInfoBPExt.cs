
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/7/29 12:46:44
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
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Data.Objects.SqlClient;
using Jinher.AMP.YJB.Deploy.Enums;
using Jinher.AMP.BTP.Common;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class YJBDSFOrderInfoBP : BaseBP, IYJBDSFOrderInfo
    {

        /// <summary>
        /// 根据订单号获取订单数据
        /// </summary>
        /// <param name="OrderNos"></param>
        /// <returns></returns>
        public ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>> GetDSFOrderInfoByOrderNosExt(System.Collections.Generic.List<string> OrderNos)
        {
            var result = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>>()
            {
                isSuccess = true,
                Message = "sucess"
            };
            if (OrderNos == null || OrderNos.Count <= 0)
            {
                result.isSuccess = false;
                result.Message = "参数信息不正确";

                LogHelper.Error("查询车险返利信息发生错误:参数信息不正确");
            }
            else
            {
                try
                {
                    var query = from o in YJBDSFOrderInfo.ObjectSet()
                                join r in YJBCarInsuranceRebate.ObjectSet() on o.OrderNo equals r.OrderNo
                                where OrderNos.Contains(o.OrderNo)
                                select new Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO
                                {
                                    OrderNo = o.OrderNo,
                                    OrderPayMoney = o.OrderPayMoney.Value,
                                    OrderPayState = o.OrderPayState,
                                    OrderPayDate = o.OrderPayDate.Value,
                                    PlatformName = o.PlatformName,
                                    Commodity = o.Commodity,
                                    RebateMoney = r.RebateMoney.Value,
                                    RebateNum = r.RebateNum.Value,
                                    RebateDate = r.RebateDate.Value,
                                    RebateState = (Jinher.AMP.YJB.Deploy.Enums.CarInsuranceStateEnum)r.RebateState.Value,
                                    RemittanceNo = r.RemittanceNo,
                                    AuditDate = r.AuditDate.Value,
                                    PhoneNum = r.PhoneNum,
                                    InsuranceAmount = r.InsuranceAmount.Value
                                };
                    result.ResultCode = query.Count();
                    result.Data = query.ToList();
                }
                catch (Exception ex)
                {
                    result.isSuccess = false;
                    result.Message = "fail";
                    LogHelper.Error("查询第三方订单发生错误:参数信息:" + JsonHelper.JsonSerializer(OrderNos), ex);
                }
            }
            return result;
        }
        /// <summary>
        /// 获取保险返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>> GetCarInsuranceRebateExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>>()
            {
                isSuccess = true,
                Message = "sucess"
            };

            if (arg == null)
            {
                result.isSuccess = false;
                result.Message = "参数信息不正确";

                LogHelper.Error("查询车险返利信息发生错误:参数信息不正确");
            }

            LogHelper.Debug("查询车险返利信息;查询依据" + JsonHelper.JsonSerializer(arg));

            try
            {
                var query = YJBCarInsuranceRebate.ObjectSet()
                    .GroupBy(x =>
                        new { x.RemittanceNo }).Select(x =>
                     new CarInsurancePolymerizationDTO()
                     {
                         RemittanceNo = x.Key.RemittanceNo,
                         RebateDate = x.Max(o => o.RebateDate),
                         RebateMoney = x.Sum(o => o.RebateMoney.Value),
                         RebateNum = x.Sum(o => o.RebateNum.Value),
                         AfterTaxMoney = Math.Round(x.Sum(o => o.DouRebateMoney) * (decimal)1.06, 2),
                         AuditFlag = x.Max(o => o.AuditFlag ?? 0),
                         DouRebateMoney = x.Sum(o => o.DouRebateMoney),
                         //DouRebatePersent = Math.Round(x.Sum(o => o.DouRebateMoney) / x.Sum(o => o.InsuranceAmount.Value), 2),
                         DouRebatePersent = x.Max(o=>o.DouRebatePersent),
                         CompanyRemittanceMoney = Math.Round(x.Sum(o => o.CompanyRebateMoney) * (decimal)1.06, 2),
                         CompanyRebateMoney = x.Sum(o => o.CompanyRebateMoney),
                         InsuranceCompanyCode = x.Max(o => o.InsuranceCompanyCode),
                         BusinessInsuranceAmount = x.Sum(o => o.BusinessInsuranceAmount),
                         StrongInsuranceAmount = x.Sum(o => o.StrongInsuranceAmount),
                         CarShipAmount = x.Sum(o => o.CarShipAmount),
                         InsuranceAmount = x.Sum(o => o.InsuranceAmount.Value),
                         //CompanyRebatePersent = Math.Round(x.Sum(o => o.CompanyRebateMoney) / x.Sum(o => o.InsuranceAmount.Value), 2)
                         CompanyRebatePersent = x.Max(o => o.CompanyRebatePersent)
                     });
                var count = query.Count();
                var Data = query.OrderByDescending(o => o.RebateDate).Skip((arg.PageIndex - 1) * arg.PageSize).Take(arg.PageSize).ToList();

                result.ResultCode = count;
                result.Data = Data;

            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "fail";
                LogHelper.Error("查询车险返利信息发生错误:参数信息:" + JsonHelper.JsonSerializer(arg), ex);
            }
            return result;
        }
        /// <summary>
        /// 根据汇款单号获取返利数据
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>> GetCarRebateByRemittanceNoExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            var result = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>>()
            {
                isSuccess = true,
                Message = "sucess"
            };

            if (arg == null)
            {
                result.isSuccess = false;
                result.Message = "参数信息不正确";

                LogHelper.Error("根据汇款单号查询车险返利信息发生错误:参数信息不正确");
            }

            LogHelper.Debug("根据汇款单号查询车险返利信息;查询依据" + JsonHelper.JsonSerializer(arg));

            try
            {
                var query = YJBCarInsuranceRebate.ObjectSet()
                   .Select(x =>
                    new Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO()
                    {
                        OrderNo = x.OrderNo,
                        RebateDate = x.RebateDate.Value,
                        RebateNum = x.RebateNum.Value,
                        RebateMoney = x.RebateMoney.Value,
                        RemittanceNo = x.RemittanceNo,
                        PhoneNum = x.PhoneNum,
                        AuditDate = x.AuditDate.Value,
                        InsuranceAmount = x.InsuranceAmount.Value,
                        RebateState = (CarInsuranceStateEnum)x.RebateState,
                        DouRemittanceAmount = Math.Round(x.DouRebateMoney * (decimal)1.06, 2),
                        CompanyRemittanceMoney = Math.Round(x.CompanyRebateMoney * (decimal)1.06, 2),
                        BusinessInsuranceAmount = x.BusinessInsuranceAmount,
                        StrongInsuranceAmount = x.StrongInsuranceAmount,
                        CarShipAmount = x.CarShipAmount,
                        InsuranceCompanyCode = x.InsuranceCompanyCode,
                        DouRebateMoney = x.DouRebateMoney,
                        DouRebatePersent = x.DouRebatePersent,
                        CompanyRebateMoney = x.CompanyRebateMoney,
                        CompanyRebatePersent = x.CompanyRebatePersent
                    });

                if (!string.IsNullOrEmpty(arg.RemittanceNo))
                {
                    query = query.Where(x => x.RemittanceNo == arg.RemittanceNo);
                }
                var count = query.Count();
                var Data = new List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>();
                if (arg.PageIndex > 0)
                {
                    Data = query.OrderByDescending(o => o.RebateDate).Skip((arg.PageIndex - 1) * arg.PageSize).Take(arg.PageSize).ToList();
                }
                else
                {
                    Data = query.OrderByDescending(o => o.RebateDate).ToList();
                }
                result.ResultCode = count;
                result.Data = Data;
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "fail";
                LogHelper.Error("根据汇款单号查询车险返利信息发生错误:参数信息:" + JsonHelper.JsonSerializer(arg), ex);
            }
            return result;
        }
        /// <summary>
        /// 查询统计报表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>> GetCarInsuranceReportExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            var result = new ResultDTO<List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>>()
            {
                isSuccess = true,
                Message = "sucess"
            };

            if (arg == null)
            {
                result.isSuccess = false;
                result.Message = "参数信息不正确";

                LogHelper.Error("查询车险报表信息发生错误:参数信息不正确");
            }

            LogHelper.Debug("查询车险报表信息查询依据" + JsonHelper.JsonSerializer(arg));

            try
            {
                var query = from r in YJBCarInsuranceReport.ObjectSet()
                            join d in YJBCarInsReportDetail.ObjectSet() on r.DetailId equals d.Id
                            select new Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO
                            {
                                OrderNo = r.OrderNo,
                                MemberPhone = r.MemberPhone,
                                CustomPhone = r.CustomPhone,
                                InsuranceAmount = r.InsuranceAmount.Value,
                                InsuranceTime = r.InsuranceTime,
                                State = r.State,
                                RecommendName = r.RecommendName,
                                RecommendAmount = SqlFunctions.StringConvert(r.RecommendAmount.Value),
                                CustomAmount = r.CustomAmount.Value,
                                SinopecAmount = r.SinopecAmount.Value,
                                RebateState = (CarInsuranceStateEnum)r.RebateState.Value,
                                StrongInsuranceAmount = d.StrongInsuranceAmount.Value,
                                BusinessAmount = d.BusinessAmount.Value,
                                StrongInsuranceOrderId = d.StrongInsuranceOrderId,
                                StrongInsuranceStartTime = d.StrongInsuranceStartTime,
                                StrongInsuranceEndTime = d.StrongInsuranceEndTime,
                                BusinessOrderId = d.BusinessOrderId,
                                BusinessStartTime = d.BusinessStartTime,
                                BusinessEndTime = d.BusinessEndTime,
                                PlateNumber = d.PlateNumber,
                                ChassisNumber = d.ChassisNumber,
                                EngineNumber = d.EngineNumber,
                                CarTypeName = d.CarTypeName,
                                RegisterTime = d.RegisterTime,
                                IsTransfer = d.IsTransfer,
                                CarOwnerName = d.CarOwnerName,
                                CarOwnerIdType = d.CarOwnerIdType,
                                CarOwnerId = d.CarOwnerId,
                                CarOwnerAddress = d.CarOwnerAddress,
                                CarOwnerPhone = d.CarOwnerPhone,
                                PolicyHolderName = d.PolicyHolderName,
                                PolicyHolderIdType = d.PolicyHolderIdType,
                                PolicyHolderId = d.PolicyHolderId,
                                PolicyHolderPhone = d.PolicyHolderPhone,
                                PolicyHolderAddress = d.PolicyHolderAddress,
                                StrongInsurance_SI = d.StrongInsurance_SI,
                                StrongInsurance_Car = d.StrongInsurance_Car,
                                Business_Car = d.Business_Car,
                                Business_Three = d.Business_Three,
                                Business_Driver = d.Business_Driver,
                                Business_Passenger = d.Business_Passenger,
                                Business_AllCar = d.Business_AllCar,
                                Business_Glass = d.Business_Glass,
                                Business_Body = d.Business_Body,
                                Business_Engine = d.Business_Engine,
                                Business_Natural = d.Business_Natural,
                                Business_Garage = d.Business_Garage,
                                Business_Third = d.Business_Third,
                                Business_Spirit = d.Business_Spirit,
                                NoDeductibles_Car = d.NoDeductibles_Car,
                                NoDeductibles_Three = d.NoDeductibles_Three,
                                NoDeductibles_Driver = d.NoDeductibles_Driver,
                                NoDeductibles_Passenger = d.NoDeductibles_Passenger,
                                NoDeductibles_AllCar = d.NoDeductibles_AllCar,
                                NoDeductibles_Body = d.NoDeductibles_Body,
                                NoDeductibles_Engine = d.NoDeductibles_Engine,
                                NoDeductibles_Natural = d.NoDeductibles_Natural,
                                NoDeductibles__Spirit = d.NoDeductibles__Spirit,
                                InsuranceCompanyCode = r.InsuranceCompanyCode,
                                SubTime = r.SubTime
                            };
                if (arg.BeginTime != null)
                {
                    //DateTime begintime = DateTime.Parse(arg.BeginTime);
                    query = query.Where(x => x.SubTime >= arg.BeginTime);
                }
                if (arg.EndTime != null)
                {
                    DateTime endtime = arg.EndTime.Value.AddDays(1);
                    query = query.Where(x => x.SubTime < endtime);
                }
                if (arg.CompanyCode!=null&&arg.CompanyCode.Any())
                {
                    query = query.Where(x => x.InsuranceCompanyCode != null && x.InsuranceCompanyCode != "");
                    query = query.Where(x => arg.CompanyCode.Contains(x.InsuranceCompanyCode));
                    //var ss = LinqHelper.GetEFCommandSql(query);
                }
                if (!string.IsNullOrEmpty(arg.Status) && arg.Status != "-1")
                {
                    query = query.Where(x => x.State.Equals(arg.Status));
                }
                var count = query.Count();

                var Data = query.OrderByDescending(o => o.InsuranceTime).Skip((arg.PageIndex - 1) * arg.PageSize).Take(arg.PageSize).ToList();
                result.isSuccess = true;
                result.ResultCode = count;
                result.Data = Data;

            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.Message = "fail";
                LogHelper.Error("查询车险报表信息发生错误:参数信息:" + JsonHelper.JsonSerializer(arg), ex);
            }
            return result;
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCarRebateStateExt(string OrderNO,int State)
        {
            var result = new ResultDTO
            {
                isSuccess = true
            };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var rebate = YJBCarInsuranceRebate.ObjectSet().Where(x => x.OrderNo == OrderNO).FirstOrDefault();
                if (rebate != null)
                {
                    //Deploy.YJBCarInsuranceRebateDTO dto = new Deploy.YJBCarInsuranceRebateDTO();
                    //dto.Id = rebate.Id;
                    //dto.EntityState = System.Data.EntityState.Modified;
                    //dto.OrderNo = rebate.OrderNo;
                    //dto.RebateDate = rebate.RebateDate;
                    //dto.RebateMoney = rebate.RebateMoney;
                    //dto.RebateNum = rebate.RebateNum;
                    //dto.RemittanceNo = rebate.RemittanceNo;
                    //dto.PhoneNum = rebate.PhoneNum;
                    //dto.RebateState = State;
                    //dto.AuditDate = DateTime.Now;
                    //dto.InsuranceAmount = rebate.InsuranceAmount;
                    //dto.AuditFlag = 1;
                    //var dbmodel = YJBCarInsuranceRebate.FromDTO(dto);
                    //contextSession.SaveObject(dbmodel);
                    rebate.RebateState = State;
                    rebate.AuditFlag = 1;
                    rebate.AuditDate = DateTime.Now;
                }

                var report = YJBCarInsuranceReport.ObjectSet().Where(x => x.OrderNo == OrderNO).FirstOrDefault();
                if (report != null)
                {
                    //Deploy.YJBCarInsuranceReportDTO dto = new BTP.Deploy.YJBCarInsuranceReportDTO();
                    //dto.Id = report.Id;
                    //dto.EntityState = System.Data.EntityState.Modified;
                    //dto.DetailId = report.DetailId;
                    //dto.ModifiedOn = DateTime.Now;

                    //dto.OrderNo = report.OrderNo;
                    //dto.MemberPhone = report.MemberPhone;
                    //dto.CustomPhone = report.CustomPhone;
                    //dto.InsuranceAmount = report.InsuranceAmount;

                    //dto.InsuranceTime = report.InsuranceTime;
                    //dto.State = report.State;
                    //dto.RecommendName = report.RecommendName;
                    //dto.RecommendAmount = report.CustomAmount;
                    //dto.CustomAmount = report.CustomAmount;
                    //dto.SinopecAmount = report.SinopecAmount;
                    //dto.SubId = Guid.Empty;
                    //dto.RebateState = (short)State;

                    //var dbreport = YJBCarInsuranceReport.FromDTO(dto);

                    //contextSession.SaveObject(dbreport);
                    report.RebateState = (short)State;
                    report.ModifiedOn = DateTime.Now;

                }

                var count = contextSession.SaveChanges();
                if (count <= 0)
                {
                    result.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
            }
            return result;
        }
    }
}