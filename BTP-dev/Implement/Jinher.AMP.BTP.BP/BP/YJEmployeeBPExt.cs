
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/4/4 15:29:38
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

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class YJEmployeeBP : BaseBP, IYJEmployee
    {
        /// <summary>
        /// 查询员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> GetYJEmployeeListExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO input)
        {
            var query = YJEmployee.ObjectSet().Where(_ => _.AppId == input.AppId && _.IsDel == 0).AsQueryable();
            if (!string.IsNullOrWhiteSpace(input.UserCode))
            {
                query = query.Where(_ => _.UserCode == input.UserCode);
            }
            if (!string.IsNullOrWhiteSpace(input.UserAccount))
            {
                query = query.Where(_ => _.UserAccount == input.UserAccount);
            }
            if (!string.IsNullOrWhiteSpace(input.UserName))
            {
                query = query.Where(_ => _.UserName.Contains(input.UserName) || input.Phone.Contains(_.UserName));
            }
            if (!string.IsNullOrWhiteSpace(input.Phone))
            {
                query = query.Where(_ => _.Phone == input.Phone);
            }
            if (!string.IsNullOrWhiteSpace(input.Area))
            {
                query = query.Where(_ => input.Area.Contains(_.Area) || _.Area.Contains(input.Area));
            }
            if (!string.IsNullOrWhiteSpace(input.StationCode))
            {
                query = query.Where(_ => _.StationCode.Contains(input.StationCode) || input.StationCode.Contains(_.StationCode));
            }
            if (!string.IsNullOrWhiteSpace(input.StationName))
            {
                query = query.Where(_ => _.StationName.Contains(input.StationName) || input.StationName.Contains(_.StationName));
            }
            if (!string.IsNullOrWhiteSpace(input.IdentityNum))
            {
                query = query.Where(_ => _.IdentityNum == input.IdentityNum);
            }
            if (input.IsManager > 0)
            {
                query = query.Where(_ => _.IsManager == input.IsManager);
            }
            if (!string.IsNullOrWhiteSpace(input.Department))
            {
                query = query.Where(_ => input.Department.Contains(_.Department) || _.Department.Contains(input.Department));
            }
            if (!string.IsNullOrWhiteSpace(input.Station))
            {
                query = query.Where(_ => input.Station.Contains(_.Station) || _.Station.Contains(input.Station));
            }
            var count = query.Count();
            var data = (from n in query
                        select new Jinher.AMP.BTP.Deploy.YJEmployeeDTO
                        {
                            Id = n.Id,
                            UserCode = n.UserCode,
                            UserAccount = n.UserAccount,
                            UserName = n.UserName,
                            IdentityNum = n.IdentityNum,
                            Phone = n.Phone,
                            Area = n.Area,
                            StationCode = n.StationCode,
                            StationName = n.StationName,
                            SubOn = n.SubOn,
                            IsManager = n.IsManager,
                            Department = n.Department,
                            Station = n.Station
                        }).OrderByDescending(q => q.SubOn).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList();
            return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>>
            {
                isSuccess = true,
                Data = new ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> { List = data, Count = count }
            };
        }

        /// <summary>
        /// 根据条件查询员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> GetYJEmployeeListExtBySearch(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO input)
        {
            var query = YJEmployee.ObjectSet().Where(_ => _.IsDel == 0).AsQueryable();
            if (input.AppId != null && input.AppId != Guid.Empty)
            {
                query = query.Where(_ => _.AppId == input.AppId);
            }
            if (!string.IsNullOrWhiteSpace(input.UserCode))
            {
                query = query.Where(_ => _.UserCode == input.UserCode);
            }
            if (!string.IsNullOrWhiteSpace(input.UserAccount))
            {
                query = query.Where(_ => _.UserAccount == input.UserAccount);
            }
            if (!string.IsNullOrWhiteSpace(input.UserName))
            {
                query = query.Where(_ => _.UserName.Contains(input.UserName) || input.Phone.Contains(_.UserName));
            }
            if (!string.IsNullOrWhiteSpace(input.Phone))
            {
                query = query.Where(_ => _.Phone == input.Phone);
            }
            if (!string.IsNullOrWhiteSpace(input.Area))
            {
                query = query.Where(_ => input.Area.Contains(_.Area) || _.Area.Contains(input.Area));
            }
            if (!string.IsNullOrWhiteSpace(input.StationCode))
            {
                query = query.Where(_ => _.StationCode.Contains(input.StationCode) || input.StationCode.Contains(_.StationCode));
            }
            if (!string.IsNullOrWhiteSpace(input.StationName))
            {
                query = query.Where(_ => _.StationName.Contains(input.StationName) || input.StationName.Contains(_.StationName));
            }
            if (!string.IsNullOrWhiteSpace(input.IdentityNum))
            {
                query = query.Where(_ => _.IdentityNum == input.IdentityNum);
            }
            if (input.IsManager > 0)
            {
                query = query.Where(_ => _.IsManager == input.IsManager);
            }
            if (!string.IsNullOrWhiteSpace(input.Department))
            {
                query = query.Where(_ => input.Department.Contains(_.Department) || _.Department.Contains(input.Department));
            }
            if (!string.IsNullOrWhiteSpace(input.Station))
            {
                query = query.Where(_ => input.Station.Contains(_.Station) || _.Station.Contains(input.Station));
            }
            var count = query.Count();
            var data = (from n in query
                        select new Jinher.AMP.BTP.Deploy.YJEmployeeDTO
                        {
                            Id = n.Id,
                            UserCode = n.UserCode,
                            UserAccount = n.UserAccount,
                            UserName = n.UserName,
                            IdentityNum = n.IdentityNum,
                            Phone = n.Phone,
                            Area = n.Area,
                            StationCode = n.StationCode,
                            StationName = n.StationName,
                            SubOn = n.SubOn,
                            IsManager = n.IsManager,
                            Department = n.Department,
                            Station = n.Station
                        }).OrderByDescending(q => q.SubOn).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList();
            return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>>
            {
                isSuccess = true,
                Data = new ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> { List = data, Count = count }
            };
        }


        /// <summary>
        /// 新建员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddYJEmployeeExt(Jinher.AMP.BTP.Deploy.YJEmployeeDTO input)
        {
            try
            {
                Guid UserId = CBCSV.GetUserIdByAccount(input.UserAccount);
                //if (UserId == Guid.Empty || UserId == null)
                //{
                //    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "该用户不是易捷用户!" };
                //}
                var UserInfo = YJEmployee.ObjectSet().FirstOrDefault(p => (p.UserAccount == input.UserAccount||p.IdentityNum==input.IdentityNum )&& p.AppId == p.AppId && p.IsDel == 0);
                if (UserInfo != null)
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "该用户已存在!" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                YJEmployee YJEmp = YJEmployee.CreateYJEmployee();
                YJEmp.UserId = UserId;
                YJEmp.UserCode = input.UserCode;
                YJEmp.UserAccount = input.UserAccount;
                YJEmp.UserName = input.UserName;
                YJEmp.IdentityNum = input.IdentityNum;
                YJEmp.Phone = input.Phone;
                YJEmp.Area = input.Area;
                YJEmp.StationCode = input.StationCode;
                YJEmp.StationName = input.StationName;
                YJEmp.IsDel = 0;
                YJEmp.SubId = ContextDTO.LoginUserID;
                YJEmp.SubOn = DateTime.Now;
                YJEmp.AppId = input.AppId;
                YJEmp.IsManager = input.IsManager;
                if (YJEmp.IsManager == 1)
                {
                    YJEmp.Department = input.Department;
                    YJEmp.Station = input.Station;
                }
                else
                {
                    YJEmp.Department = "";
                    YJEmp.Station = "";
                }
                contextSession.SaveObject(YJEmp);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.AddYJEmployeeExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "添加成功" };
        }
        /// <summary>
        /// 编辑员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateYJEmployeeExt(Jinher.AMP.BTP.Deploy.YJEmployeeDTO input)
        {
            try
            {
                Guid UserId = CBCSV.GetUserIdByAccount(input.UserAccount);
                //if (UserId == Guid.Empty || UserId == null)
                //{
                //    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "该用户不是易捷用户!" };
                //}
                var UserInfo = YJEmployee.ObjectSet().FirstOrDefault(p => p.Id != input.Id & p.AppId == input.AppId & p.UserAccount == input.UserAccount & p.IsDel != 1);
                if (UserInfo != null)
                {
                    return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "该用户已存在!" };
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var YJEmp = YJEmployee.ObjectSet().FirstOrDefault(_ => _.Id == input.Id);
                YJEmp.UserId = UserId;
                YJEmp.UserCode = input.UserCode;
                YJEmp.UserAccount = input.UserAccount;
                YJEmp.UserName = input.UserName;
                YJEmp.IdentityNum = input.IdentityNum;
                YJEmp.Phone = input.Phone;
                YJEmp.Area = input.Area;
                YJEmp.StationCode = input.StationCode;
                YJEmp.StationName = input.StationName;
                YJEmp.ModifiedId = ContextDTO.LoginUserID;
                YJEmp.ModifiedOn = DateTime.Now;
                YJEmp.IsManager = input.IsManager;
                if (YJEmp.IsManager == 1)
                {
                    YJEmp.Department = input.Department;
                    YJEmp.Station = input.Station;
                }
                else
                {
                    YJEmp.Department = "";
                    YJEmp.Station = "";
                }
                YJEmp.EntityState = EntityState.Modified;
                contextSession.SaveObject(YJEmp);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdateYJEmployeeExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = ex.Message };
            }
            LogHelper.Info("员工修改日志" + JsonHelper.JsonSerializer(input) + "修改人" + ContextDTO.LoginUserID + "修改时间" + DateTime.Now);
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "修改成功" };
        }
        /// <summary>
        /// 获取员工详情
        /// </summary>
        public Jinher.AMP.BTP.Deploy.YJEmployeeDTO GetYJEmployeeInfoExt(System.Guid Id)
        {
            try
            {
                var Query = YJEmployee.ObjectSet().FirstOrDefault(p => p.Id == Id);
                return Query.ToEntityData();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdateYJEmployeeExt 异常", ex);
                return new YJEmployeeDTO();
            }
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelYJEmployeeExt(System.Guid Id)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var YJEmp = YJEmployee.ObjectSet().FirstOrDefault(p => p.Id == Id);
                YJEmp.IsDel = 1;
                YJEmp.ModifiedId = ContextDTO.LoginUserID;
                YJEmp.ModifiedOn = DateTime.Now;
                YJEmp.EntityState = EntityState.Modified;
                contextSession.SaveObject(YJEmp);
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdateYJEmployeeExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "删除失败" };
            }
            LogHelper.Info("员工修改日志 Id:" + Id + "删除人:" + ContextDTO.LoginUserID + "删除时间:" + DateTime.Now);
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "删除成功" };
        }
        /// <summary>
        /// 批量删除员工
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DelYJEmployeeAllExt(System.Collections.Generic.List<System.Guid> Ids)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var YJEmpList = YJEmployee.ObjectSet().Where(p => Ids.Contains(p.Id)).ToList();
                foreach (var YJEmp in YJEmpList)
                {
                    YJEmp.IsDel = 1;
                    YJEmp.ModifiedId = ContextDTO.LoginUserID;
                    YJEmp.ModifiedOn = DateTime.Now;
                    YJEmp.EntityState = EntityState.Modified;
                    contextSession.SaveObject(YJEmp);
                }
                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdateYJEmployeeExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "删除失败" };
            }
            LogHelper.Info("员工修改日志 Ids:" + JsonHelper.JsonSerializer(Ids) + "删除人:" + ContextDTO.LoginUserID + "删除时间:" + DateTime.Now);
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "删除成功" };
        }
        /// <summary>
        /// 导出员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> ExportYJEmployeeListExt(Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO input)
        {
            try
            {
                var query = YJEmployee.ObjectSet().Where(_ => _.IsDel == 0 && _.AppId == input.AppId).AsQueryable();
                if (!string.IsNullOrWhiteSpace(input.UserCode))
                {
                    query = query.Where(_ => _.UserCode == input.UserCode);
                }
                if (!string.IsNullOrWhiteSpace(input.UserAccount))
                {
                    query = query.Where(_ => _.UserAccount == input.UserAccount);
                }
                if (!string.IsNullOrWhiteSpace(input.UserName))
                {
                    query = query.Where(_ => _.UserName.Contains(input.UserName) || input.Phone.Contains(_.UserName));
                }
                if (!string.IsNullOrWhiteSpace(input.Phone))
                {
                    query = query.Where(_ => _.Phone == input.Phone);
                }
                if (!string.IsNullOrWhiteSpace(input.Area))
                {
                    query = query.Where(_ => _.Area == input.Area);
                }
                if (!string.IsNullOrWhiteSpace(input.Area))
                {
                    query = query.Where(_ => _.Area == input.Area);
                }
                if (!string.IsNullOrWhiteSpace(input.StationCode))
                {
                    query = query.Where(_ => _.StationCode.Contains(input.StationCode) || input.StationCode.Contains(_.StationCode));
                }
                if (!string.IsNullOrWhiteSpace(input.StationName))
                {
                    query = query.Where(_ => _.StationName.Contains(input.StationName) || input.StationName.Contains(_.StationName));
                }
                if (input.IsManager > 0)
                {
                    query = query.Where(_ => _.IsManager == input.IsManager);
                }
                if (!string.IsNullOrWhiteSpace(input.Department))
                {
                    query = query.Where(_ => _.Department == input.Department);
                }
                if (!string.IsNullOrWhiteSpace(input.Station))
                {
                    query = query.Where(_ => _.Station == input.Station);
                }
                var count = query.Count();
                var data = (from n in query
                            select new Jinher.AMP.BTP.Deploy.YJEmployeeDTO
                            {
                                Id = n.Id,
                                UserCode = n.UserCode,
                                UserAccount = n.UserAccount,
                                UserName = n.UserName,
                                IdentityNum = n.IdentityNum,
                                Phone = n.Phone,
                                Area = n.Area,
                                StationCode = n.StationCode,
                                StationName = n.StationName,
                                SubOn = n.SubOn,
                                IsManager = n.IsManager,
                                Department = n.Department,
                                Station = n.Station
                            }).OrderByDescending(q => q.SubOn).ToList();
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>>
                {
                    isSuccess = true,
                    Data = new ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> { List = data, Count = count }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.ExportYJEmployeeListExt 异常", ex);
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> { isSuccess = false, Data = null };
            }
        }
        /// <summary>
        /// 导出无效账户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> ExportInvalidDataExt(System.Collections.Generic.List<string> UserAccounts, System.Guid AppId)
        {
            try
            {
                var query = YJEmployee.ObjectSet().Where(p => UserAccounts.Contains(p.UserAccount) && p.AppId == AppId && p.IsDel == 0 && p.UserId == Guid.Empty).AsQueryable();
                var count = query.Count();
                var data = (from n in query
                            select new Jinher.AMP.BTP.Deploy.YJEmployeeDTO
                            {
                                Id = n.Id,
                                UserCode = n.UserCode,
                                UserAccount = n.UserAccount,
                                UserName = n.UserName,
                                IdentityNum = n.IdentityNum,
                                Phone = n.Phone,
                                Area = n.Area,
                                StationCode = n.StationCode,
                                StationName = n.StationName,
                                SubOn = n.SubOn,
                                IsManager = n.IsManager,
                                Department = n.Department,
                                Station = n.Station
                            }).OrderByDescending(q => q.SubOn).ToList();
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>>
                {
                    isSuccess = true,
                    Data = new ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> { List = data, Count = count }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.ExportYJEmployeeListExt 异常", ex);
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> { isSuccess = false, Data = null };
            }
        }
        /// <summary>
        /// 导出全部无效账户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> ExportInvalidDataByAppidExt(System.Guid AppId)
        {
            try
            {
                var query = YJEmployee.ObjectSet().Where(p =>p.AppId == AppId && p.IsDel == 0 && p.UserId == Guid.Empty).AsQueryable();
                var count = query.Count();
                var data = (from n in query
                            select new Jinher.AMP.BTP.Deploy.YJEmployeeDTO
                            {
                                Id = n.Id,
                                UserCode = n.UserCode,
                                UserAccount = n.UserAccount,
                                UserName = n.UserName,
                                IdentityNum = n.IdentityNum,
                                Phone = n.Phone,
                                Area = n.Area,
                                StationCode = n.StationCode,
                                StationName = n.StationName,
                                SubOn = n.SubOn,
                                IsManager = n.IsManager,
                                Department = n.Department,
                                Station = n.Station
                            }).OrderByDescending(q => q.SubOn).ToList();
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>>
                {
                    isSuccess = true,
                    Data = new ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> { List = data, Count = count }
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.ExportInvalidDataByAppidExt 异常", ex);
                return new ResultDTO<ListResult<Jinher.AMP.BTP.Deploy.YJEmployeeDTO>> { isSuccess = false, Data = null };
            }
        }
        /// <summary>
        /// 导入员工信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> ImportYJEmployeeListExt(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> YJEmpList, System.Guid AppId)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                YJEmployeeSearchDTO YJEmpl = new YJEmployeeSearchDTO();
                var YJEmpListNoRepeat = YJEmpList.GroupBy(P => P.UserAccount).Select(P => P.FirstOrDefault()).ToList();
                var IcardList = YJEmpList.Select(s => s.IdentityNum).ToList();//身份证号

                List<string> UserAccountList = YJEmpListNoRepeat.Select(S => S.UserAccount).ToList();//用户账号 
                //验证重复账号   
                var  ExistIcardDate= YJEmployee.ObjectSet().Where(p => IcardList.Contains(p.IdentityNum) && p.IsDel == 0 && p.AppId == AppId).Select(s=>s.IdentityNum).ToList();
                var ExistDate= YJEmployee.ObjectSet().Where(p => UserAccountList.Contains(p.UserAccount) && p.IsDel == 0 && p.AppId == AppId).ToList();
                var RepeatData = ExistDate.Select(s => s.UserAccount).ToList();
                YJEmpl.RepeatData = YJEmpListNoRepeat.Where(p => RepeatData.Contains(p.UserAccount)).ToList();
                YJEmpl.RepeatIcardData = YJEmpList.Where(p => ExistIcardDate.Contains(p.IdentityNum)).ToList();
                if (YJEmpl.RepeatIcardData.Any())
                {
                    return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { Data = YJEmpl, ResultCode = 1, isSuccess = false, Message = "数据库中已存在,请核对后再导入~" };
                }
                //取出无效数据  
                List<string> Invalid = new List<string>();
                foreach (var item in YJEmpListNoRepeat)
                {
                    Guid UserId = CBCSV.GetUserIdByAccount(item.UserAccount);
                    item.UserId = UserId;
                    if (UserId == Guid.Empty || UserId == null)
                    {
                        Invalid.Add(item.UserAccount);
                    }
                }
                YJEmpl.InvalidData = YJEmpListNoRepeat.Where(p => Invalid.Contains(p.UserAccount)).Select(s => s.UserAccount).ToList();
                foreach (var item in YJEmpListNoRepeat)
                {
                    var ExistEmpolyee = ExistDate.FirstOrDefault(p => p.UserAccount == item.UserAccount);
                    if (ExistEmpolyee == null)
                    {
                        YJEmployee YJEmp = YJEmployee.CreateYJEmployee();
                        YJEmp.UserId = item.UserId;
                        YJEmp.UserCode = item.UserCode;
                        YJEmp.UserAccount = item.UserAccount;
                        YJEmp.UserName = item.UserName;
                        YJEmp.IdentityNum = item.IdentityNum;
                        YJEmp.Phone = item.Phone;
                        YJEmp.Area = item.Area;
                        YJEmp.StationCode = item.StationCode;
                        YJEmp.StationName = item.StationName;
                        YJEmp.IsDel = 0;
                        YJEmp.SubId = ContextDTO.LoginUserID;
                        YJEmp.SubOn = DateTime.Now;
                        YJEmp.AppId = item.AppId;
                        YJEmp.IsManager = item.IsManager;
                        YJEmp.Department = item.Department;
                        YJEmp.Station = item.Station;
                        contextSession.SaveObject(YJEmp);
                    }
                    else
                    {
                        ExistEmpolyee.UserId = item.UserId;
                        ExistEmpolyee.UserCode = item.UserCode;
                        ExistEmpolyee.UserAccount = item.UserAccount;
                        ExistEmpolyee.UserName = item.UserName;
                        ExistEmpolyee.IdentityNum = item.IdentityNum;
                        ExistEmpolyee.Phone = item.Phone;
                        ExistEmpolyee.Area = item.Area;
                        ExistEmpolyee.StationCode = item.StationCode;
                        ExistEmpolyee.StationName = item.StationName;
                        ExistEmpolyee.IsDel = 0;
                        ExistEmpolyee.SubId = ContextDTO.LoginUserID;
                        ExistEmpolyee.SubOn = DateTime.Now;
                        ExistEmpolyee.AppId = item.AppId;
                        ExistEmpolyee.IsManager = item.IsManager;
                        ExistEmpolyee.Department = item.Department;
                        ExistEmpolyee.Station = item.Station;
                        ExistEmpolyee.EntityState = EntityState.Modified;
                        contextSession.SaveObject(ExistEmpolyee);
                    }
                }
                contextSession.SaveChanges();
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { Data = YJEmpl, ResultCode = 0, isSuccess = true, Message = "导入成功" };
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.AddYJEmployeeExt 异常", ex);
                return new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.YJEmployeeSearchDTO> { isSuccess = false, ResultCode = 2, Message = ex.Message };
            }
        }
        /// <summary>
        /// 定时更新无效用户信息
        /// </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataYJEmployeeInfoExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                //取出表中多有的无效员工
                var InvalidYJEmployeeList = YJEmployee.ObjectSet().Where(p => p.UserId == Guid.Empty && p.IsDel == 0).ToList();
                foreach (var item in InvalidYJEmployeeList)
                {
                    Guid UserId = CBCSV.GetUserIdByAccount(item.UserAccount.Trim());
                    if (UserId != Guid.Empty)
                    {
                        item.UserId = UserId;
                        item.ModifiedOn = DateTime.Now;
                        item.EntityState = EntityState.Modified;
                        contextSession.SaveObject(item);
                    }

                }
                int count = contextSession.SaveChanges();
                LogHelper.Info("成功更新无效用户条数:" + count);
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdataYJEmployeeInfoExt 异常", ex);
                return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = false, Message = "更新失败" };
            }
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO { isSuccess = true, Message = "更新成功" };
        }
        /// <summary>
        /// 获取易捷员工所属区域信息
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.YJEmployeeDTO> GetAreaInfoExt(System.Guid AppId)
        {
            try
            {
                var Query = YJEmployee.ObjectSet().Where(p => p.AppId == AppId).GroupBy(p => p.Area).Select(p => p.FirstOrDefault()).Select(s => new YJEmployeeDTO
                {
                    Id = s.Id
                    ,
                    Area = s.Area
                }).ToList();
                return Query;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.GetAreaInfoExt 异常", ex);
                return new List<YJEmployeeDTO>();
            }
        }
        /// <summary>
        /// 根据站编码修改站名称
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataStationNameByCodeExt()
        {
            ResultDTO result = new ResultDTO() { ResultCode = 1, isSuccess = false };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var stationList = YJEmTemp.ObjectSet().ToList();
                var yjEmployee = YJEmployee.ObjectSet().ToList();
                foreach (var item in yjEmployee)
                {
                    var station = stationList.FirstOrDefault(p => p.stationcode == item.StationCode);
                    if (station != null)
                    {
                        item.StationName = station.station ?? item.StationName;
                        item.EntityState = EntityState.Modified;
                    }
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    result.isSuccess = true;
                    result.Message = "更新成功";
                }
                else
                {
                    result.isSuccess = false;
                    result.Message = "更新失败";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdataStationNameByCodeExt 异常", ex);
                return result;
            }
        }
        /// <summary>
        /// 根据员工账号更新员工编码
        ///  </summary>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdataUserCodeByUserAccountExt()
        {
            ResultDTO result = new ResultDTO() { ResultCode = 1, isSuccess = false };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var YJEmList = YJEmTemp.ObjectSet().ToList();
                var yjEmployee = YJEmployee.ObjectSet().ToList();
                foreach (var item in yjEmployee)
                {
                    var YJEmInfo = YJEmList.FirstOrDefault(p => p.stationcode == item.IdentityNum&&p.station==item.UserName);
                    if (YJEmInfo != null)
                    {
                        item.UserCode = YJEmInfo.UserCode ?? "";
                        item.EntityState = EntityState.Modified;
                    }
                }
                int count = contextSession.SaveChanges();
                if (count > 0)
                {
                    result.isSuccess = true;
                    result.Message = "更新成功";
                }
                else
                {
                    result.isSuccess = false;
                    result.Message = "更新失败";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("YJEmployeeBP.UpdataUserCodeByUserAccountExt 异常", ex);
                return result;
            }
        }

    }
}