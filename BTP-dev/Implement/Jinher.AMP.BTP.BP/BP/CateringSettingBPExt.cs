
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/12/6 16:45:10
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CateringSettingBP : BaseBP, ICateringSetting
    {

        public ResultDTO AddCateringSettingExt(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            try
            {
                var setting = CateringSetting.CreateCateringSetting();
                setting.Id = settingDTO.CateringSetting.Id;
                setting.AppId = settingDTO.CateringSetting.AppId;
                setting.StoreId = settingDTO.CateringSetting.StoreId;
                setting.DeliveryAmount = settingDTO.CateringSetting.DeliveryAmount;
                setting.DeliveryFee = settingDTO.CateringSetting.DeliveryFee;
                setting.DeliveryRange = settingDTO.CateringSetting.DeliveryRange;
                setting.MostCoupon = settingDTO.CateringSetting.MostCoupon;
                setting.Unit = settingDTO.CateringSetting.Unit;
                setting.Specification = settingDTO.CateringSetting.Specification;

                setting.DeliveryFeeDiscount = settingDTO.CateringSetting.DeliveryFeeDiscount;
                setting.DeliveryFeeEndT = settingDTO.CateringSetting.DeliveryFeeEndT;
                setting.DeliveryFeeStartT = settingDTO.CateringSetting.DeliveryFeeStartT;
                setting.FreeAmount = settingDTO.CateringSetting.FreeAmount;

                setting.IsDel = false;
                setting.SubTime = DateTime.Now;
                setting.SubId = this.ContextDTO.LoginUserID;
                setting.ModifiedOn = DateTime.Now;
                setting.EntityState = EntityState.Added;
                ContextFactory.CurrentThreadContext.SaveObject(setting);
                if (settingDTO.CYBusinessHours != null && settingDTO.CYBusinessHours.Count > 0)
                {
                    settingDTO.CYBusinessHours.ForEach(r =>
                    {
                        var hour = CateringBusinessHours.CreateCateringBusinessHours();
                        hour.OpeningTime = r.openingTime;
                        hour.ClosingTime = r.closingTime;
                        hour.Id = Guid.NewGuid();
                        hour.CateringSettingId = setting.Id;
                        hour.SubTime = DateTime.Now;
                        hour.SubId = this.ContextDTO.LoginUserID;
                        hour.ModifiedOn = DateTime.Now;
                        hour.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(hour);
                    });
                }
                if (settingDTO.CYShiftTime != null && settingDTO.CYShiftTime.Count > 0)
                {
                    settingDTO.CYShiftTime.ForEach(r =>
                    {
                        var hour = CateringShiftTime.CreateCateringShiftTime();
                        hour.ShiftTime = r.shiftTime;
                        hour.Id = Guid.NewGuid();
                        hour.CateringSettingId = setting.Id;
                        hour.SubTime = DateTime.Now;
                        hour.SubId = this.ContextDTO.LoginUserID;
                        hour.ModifiedOn = DateTime.Now;
                        hour.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(hour);
                    });
                }
                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("添加门店异常。commodityAndCategoryDTO：{0}", settingDTO), ex);
            }
            return new ResultDTO { ResultCode =1, Message = "Error" };
        }

        public ResultDTO UpdateCateringSettingExt(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            try
            {
                var setting = CateringSetting.ObjectSet().SingleOrDefault(r => r.Id == settingDTO.CateringSetting.Id && r.AppId == settingDTO.CateringSetting.AppId);
                if (setting == null) return new ResultDTO { ResultCode = 3, Message = "未找到该店铺！" };

                setting.DeliveryAmount = settingDTO.CateringSetting.DeliveryAmount;
                setting.DeliveryFee = settingDTO.CateringSetting.DeliveryFee;
                setting.DeliveryRange = settingDTO.CateringSetting.DeliveryRange;
                setting.MostCoupon = settingDTO.CateringSetting.MostCoupon;
                setting.Unit = settingDTO.CateringSetting.Unit;
                setting.Specification = settingDTO.CateringSetting.Specification;

                setting.DeliveryFeeDiscount = settingDTO.CateringSetting.DeliveryFeeDiscount;
                setting.DeliveryFeeEndT = settingDTO.CateringSetting.DeliveryFeeEndT;
                setting.DeliveryFeeStartT = settingDTO.CateringSetting.DeliveryFeeStartT;
                setting.FreeAmount = settingDTO.CateringSetting.FreeAmount;

                setting.IsDel = false;
                setting.ModifiedOn = DateTime.Now;
                setting.EntityState = EntityState.Modified;
                ContextFactory.CurrentThreadContext.SaveObject(setting);

                var hours = CateringBusinessHours.ObjectSet().Where(r => r.CateringSettingId == setting.Id).ToList();
                if (hours != null && hours.Count > 0)
                {
                    hours.ForEach(r =>
                    {
                        r.EntityState = EntityState.Deleted;
                        ContextFactory.CurrentThreadContext.SaveObject(r);
                    });
                }
                if (settingDTO.CYBusinessHours != null && settingDTO.CYBusinessHours.Count > 0)
                {
                    settingDTO.CYBusinessHours.ForEach(r =>
                    {
                        var hour = CateringBusinessHours.CreateCateringBusinessHours();
                        hour.OpeningTime = r.openingTime;
                        hour.ClosingTime = r.closingTime;
                        hour.Id = Guid.NewGuid();
                        hour.CateringSettingId = setting.Id;
                        hour.SubTime = hour.ModifiedOn = DateTime.Now;
                        hour.SubId = this.ContextDTO.LoginUserID;
                        hour.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(hour);
                    });
                }

                var shiftTimes = CateringShiftTime.ObjectSet().Where(r => r.CateringSettingId == setting.Id).ToList();
                if (shiftTimes != null && shiftTimes.Count > 0)
                {
                    shiftTimes.ForEach(r =>
                    {
                        r.EntityState = EntityState.Deleted;
                        ContextFactory.CurrentThreadContext.SaveObject(r);
                    });
                }

                if (settingDTO.CYShiftTime != null && settingDTO.CYShiftTime.Count > 0)
                {
                    settingDTO.CYShiftTime.ForEach(r =>
                    {
                        var hour = CateringShiftTime.CreateCateringShiftTime();
                        hour.ShiftTime = r.shiftTime;
                        hour.Id = Guid.NewGuid();
                        hour.CateringSettingId = setting.Id;
                        hour.SubTime = hour.ModifiedOn = DateTime.Now;
                        hour.SubId = this.ContextDTO.LoginUserID;
                        hour.EntityState = EntityState.Added;
                        ContextFactory.CurrentThreadContext.SaveObject(hour);
                    });
                }
                if (ContextFactory.CurrentThreadContext.SaveChanges() > 0)
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("修改门店异常。commodityAndCategoryDTO：{0}", settingDTO), ex);
            }
            return new ResultDTO { ResultCode = 1, Message = "Error" };
        }

        public Deploy.CustomDTO.FCYSettingCDTO GetCateringSettingExt(Guid storeId)
        {
            Deploy.CustomDTO.FCYSettingCDTO CYSetting = new Deploy.CustomDTO.FCYSettingCDTO();
            Deploy.CustomDTO.FCateringSettingCDTO fcs = (from cs in BE.CateringSetting.ObjectSet()
                                                         where cs.StoreId == storeId && cs.IsDel == false
                                                         select new Deploy.CustomDTO.FCateringSettingCDTO
                                                         {
                                                             Unit = cs.Unit,
                                                             Specification = cs.Specification,
                                                             DeliveryAmount = cs.DeliveryAmount,
                                                             DeliveryRange = cs.DeliveryRange,
                                                             DeliveryFee = cs.DeliveryFee,
                                                             MostCoupon = cs.MostCoupon,
                                                             AppId = cs.AppId,
                                                             StoreId = cs.StoreId,
                                                             Id = cs.Id,
                                                             DeliveryFeeDiscount = cs.DeliveryFeeDiscount,
                                                             DeliveryFeeEndT = cs.DeliveryFeeEndT,
                                                             DeliveryFeeStartT = cs.DeliveryFeeStartT,
                                                             FreeAmount = cs.FreeAmount
                                                         }).FirstOrDefault();

            CYSetting.CateringSetting = fcs;
            CYSetting.CYBusinessHours = (from cbh in BE.CateringBusinessHours.ObjectSet()
                                         where cbh.CateringSettingId == fcs.Id
                                         select new Deploy.CustomDTO.FCateringBusinessHoursCDTO
                                         {
                                             openingTime = cbh.OpeningTime,
                                             closingTime = cbh.ClosingTime
                                         }).ToList();

            CYSetting.CYShiftTime = (from shift in BE.CateringShiftTime.ObjectSet()
                                     where shift.CateringSettingId == fcs.Id
                                     select new Deploy.CustomDTO.FCateringShiftTimeCDTO()
                                     {
                                         shiftTime = shift.ShiftTime
                                     }).ToList();
            return CYSetting;
        }

        public Deploy.CustomDTO.FCYSettingCDTO GetCateringSettingByAppIdExt(System.Guid appId)
        {
            Deploy.CustomDTO.FCYSettingCDTO CYSetting = new Deploy.CustomDTO.FCYSettingCDTO();
            Deploy.CustomDTO.FCateringSettingCDTO fcs = (from cs in BE.CateringSetting.ObjectSet()
                                                         where cs.AppId == appId && cs.IsDel == false
                                                         select new Deploy.CustomDTO.FCateringSettingCDTO
                                                         {
                                                             Unit = cs.Unit,
                                                             Specification = cs.Specification,
                                                             DeliveryAmount = cs.DeliveryAmount,
                                                             DeliveryRange = cs.DeliveryRange,
                                                             DeliveryFee = cs.DeliveryFee,
                                                             MostCoupon = cs.MostCoupon,
                                                             AppId = cs.AppId,
                                                             StoreId = cs.StoreId,
                                                             Id = cs.Id,
                                                             DeliveryFeeDiscount = cs.DeliveryFeeDiscount,
                                                             DeliveryFeeStartT= cs.DeliveryFeeStartT,
                                                             DeliveryFeeEndT = cs.DeliveryFeeEndT,
                                                             FreeAmount = cs.FreeAmount
                                                         }).FirstOrDefault();

            CYSetting.CateringSetting = fcs;
            CYSetting.CYBusinessHours = (from cbh in BE.CateringBusinessHours.ObjectSet()
                                         where cbh.CateringSettingId == fcs.Id
                                         select new Deploy.CustomDTO.FCateringBusinessHoursCDTO
                                         {
                                             openingTime = cbh.OpeningTime,
                                             closingTime = cbh.ClosingTime
                                         }).ToList();

            CYSetting.CYShiftTime = (from shift in BE.CateringShiftTime.ObjectSet()
                                     where shift.CateringSettingId == fcs.Id
                                     select new Deploy.CustomDTO.FCateringShiftTimeCDTO()
                                     {
                                         shiftTime = shift.ShiftTime
                                     }).ToList();
            return CYSetting;
        }

        /// <summary>
        /// 根据门店ID集合获取对应的门店设置信息
        /// </summary>
        /// <param name="storeIds"></param>
        /// <returns></returns>
        public List<Deploy.CustomDTO.FCYSettingCDTO> GetCateringSettingByStoreIdsExt(List<Guid> storeIds)
        {
            var _CYSettings = new List<Deploy.CustomDTO.FCYSettingCDTO>();

            var fcs = (from se in storeIds
                       join cs in BE.CateringSetting.ObjectSet() on se equals cs.StoreId
                       where cs.IsDel == false
                       select new Deploy.CustomDTO.FCateringSettingCDTO
                       {
                           Unit = cs.Unit,
                           Specification = cs.Specification,
                           DeliveryAmount = cs.DeliveryAmount,
                           DeliveryRange = cs.DeliveryRange,
                           DeliveryFee = cs.DeliveryFee,
                           MostCoupon = cs.MostCoupon,
                           AppId = cs.AppId,
                           StoreId = cs.StoreId,
                           Id = cs.Id,
                           DeliveryFeeDiscount = cs.DeliveryFeeDiscount,
                           DeliveryFeeStartT = cs.DeliveryFeeStartT,
                           DeliveryFeeEndT = cs.DeliveryFeeEndT,
                           FreeAmount = cs.FreeAmount
                       }).ToList();

            if (fcs == null || fcs.Count == 0) return _CYSettings;

            var settingIds = fcs.Select(r => r.Id).ToList();

            //营业时间段
            var busHours = (from f in settingIds
                            join cbh in BE.CateringBusinessHours.ObjectSet() on f equals cbh.CateringSettingId
                            select new
                            {
                                id = f,
                                openingTime = cbh.OpeningTime,
                                closingTime = cbh.ClosingTime
                            }).ToList();


            //交班时间段
            var shiftTimes = (from f in settingIds
                              join shift in BE.CateringShiftTime.ObjectSet() on f equals shift.CateringSettingId
                              select new
                              {
                                  id = f,
                                  shiftTime = shift.ShiftTime
                              }).ToList();


            fcs.ForEach(r =>
            {
                //基本设置
                var _setting = new Jinher.AMP.BTP.Deploy.CustomDTO.FCYSettingCDTO()
                {
                    CateringSetting = r
                };
                //营业时间段
                if (busHours != null && busHours.Count > 0)
                {
                    _setting.CYBusinessHours = new List<Deploy.CustomDTO.FCateringBusinessHoursCDTO>();
                    var _busHour = busHours.Where(b => b.id == r.Id).ToList();
                    if (_busHour != null && _busHour.Count > 0)
                    {
                        _busHour.ForEach(u =>
                        {
                            _setting.CYBusinessHours.Add(new Deploy.CustomDTO.FCateringBusinessHoursCDTO()
                               {
                                   closingTime = u.closingTime,
                                   openingTime = u.openingTime
                               });
                        });
                    }
                }

                //交班时间段
                if (shiftTimes != null && shiftTimes.Count > 0)
                {
                    _setting.CYShiftTime = new List<Deploy.CustomDTO.FCateringShiftTimeCDTO>();
                    var _sTime = shiftTimes.Where(b => b.id == r.Id).ToList();
                    if (_sTime != null && _sTime.Count > 0)
                    {
                        _sTime.ForEach(u =>
                        {
                            _setting.CYShiftTime.Add(new Deploy.CustomDTO.FCateringShiftTimeCDTO()
                            {
                                shiftTime = u.shiftTime
                            });
                        });
                    }
                }
                _CYSettings.Add(_setting);
            });

            return _CYSettings;
        }
    }
}