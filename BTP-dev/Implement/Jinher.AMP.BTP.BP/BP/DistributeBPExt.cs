
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/1/28 10:08:13
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DistributeBP : BaseBP, IDistribute
    {

        /// <summary>
        /// 分销商数量和层级
        /// </summary>
        /// <returns></returns>
        public ManageNumDTO ManageNcExt(ManageVM search)
        {
            ManageNumDTO manageNumDTO = new ManageNumDTO();
            if (search == null || search.AppId == Guid.Empty)
                return manageNumDTO;
            var query = Distributor.ObjectSet().Where(c => c.EsAppId == search.AppId);

            if (query.Any())
            {
                manageNumDTO.Count = query.Count();
                manageNumDTO.MaxLevel = query.Max(c => c.Level);
            }
            else
            {
                manageNumDTO.Count = 0;
                manageNumDTO.MaxLevel = 0;
            }
            return manageNumDTO;
        }

        /// <summary>
        /// 分销商列表信息
        /// </summary>
        /// <param name="manaDTO"></param>
        /// <returns></returns>
        public ManageDTO ManageInfoExt(ManageVM manaDTO)
        {

            ManageDTO result = new ManageDTO();
            if (manaDTO == null)
            {
                return result;
            }

            if (manaDTO.Ynos == 1 && manaDTO.ParentId != Guid.Empty)
            {
                var parent = Distributor.ObjectSet().FirstOrDefault(c => c.Id == manaDTO.ParentId);
                if (parent != null)
                {
                    var tuple = CBCSV.GetUserNameAndCode(parent.UserId);
                    result.ParentName = tuple.Item1;
                    result.ParentCode = tuple.Item2;
                }
            }
            var resultData = DSSBP.Instance.ManageInfo(manaDTO);
            if (resultData != null && resultData.Data != null)
            {
                result.Count = resultData.Data.Count;
                result.Manager = resultData.Data.Manager;
            }

            LogHelper.Info(
                string.Format("DistributeBP.ManageInfoExt(AppId='{0}',ParentId='{1}')，返回：Count={2},Manager.Count={3}。",
                    manaDTO.AppId, manaDTO.ParentId, result.Count, result.Manager.Count));

            return result;
        }

        /// <summary>
        /// 获取分销商申请设置 后台
        /// </summary>
        /// <returns></returns>
        public DistributRuleFullDTO GetDistributRuleFullExt(DistributionSearchDTO search)
        {
            DistributRuleFullDTO distributRuleFullDto = new DistributRuleFullDTO();
            try
            {
                var temp = (from dr in DistributionRule.ObjectSet()
                            where dr.Id == search.AppId
                            select new DistributRuleFullDTO

                            {
                                Id = dr.Id,
                                SubTime = dr.SubTime,
                                ModifiedOn = dr.ModifiedOn,
                                SubId = dr.SubId,
                                ModifiedId = dr.ModifiedId,
                                HasCondition = dr.HasCondition,
                                NeedIdentity = dr.NeedIdentity,
                                OrderTimeLimit = dr.OrderTimeLimit,
                                OrderAmountLimit = dr.OrderAmountLimit,
                                HasCustomComs = dr.HasCustomComs,
                                Title = dr.Title,
                                TiApprovalType = (ApprovalTypeEnum)dr.ApprovalType.Value,
                                StartCalcState = (DistributeApplyStateEnum)dr.StartCalcState.Value,
                                RuleDesc = dr.RuleDesc
                            });
                distributRuleFullDto = temp.FirstOrDefault();
                if (distributRuleFullDto != null)
                {
                    List<DistributionIdentitySetFullDTO> vDbIdentitySets =
                        (from dis in DistributionIdentitySet.ObjectSet()
                         where dis.RuleId == search.AppId && !dis.IsDel
                         orderby dis.NameCategory
                         select new DistributionIdentitySetFullDTO
                         {
                             Id = dis.Id,
                             SubTime = dis.SubTime,
                             ModifiedOn = dis.ModifiedOn,
                             SubId = dis.SubId,
                             ModifiedId = dis.ModifiedId,
                             Name = dis.Name,
                             IsRequired = dis.IsRequired,
                             RuleId = dis.RuleId,
                             ValueType = (ApplyInfoTypeEnum)dis.ValueType.Value,
                             IsDel = dis.IsDel,
                             NameCategory = dis.NameCategory,
                         }).ToList();
                    distributRuleFullDto.DbIdentitySets = vDbIdentitySets;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return distributRuleFullDto;
        }

        /// <summary>
        /// 获取用户申请资料信息 前端
        /// </summary>
        /// <returns></returns>
        public DistributRuleFullDTO GetDistributRuleFullDTOByAppId_MobileExt(DistributionSearchDTO search)
        {
            DistributRuleFullDTO distributRuleFullDto = new DistributRuleFullDTO();
            try
            {
                var temp = (from dr in DistributionRule.ObjectSet()
                    where dr.Id == search.AppId
                    select new DistributRuleFullDTO

                    {
                        Id = dr.Id,
                        SubTime = dr.SubTime,
                        ModifiedOn = dr.ModifiedOn,
                        SubId = dr.SubId,
                        ModifiedId = dr.ModifiedId,
                        HasCondition = dr.HasCondition,
                        NeedIdentity = dr.NeedIdentity,
                        OrderTimeLimit = dr.OrderTimeLimit,
                        OrderAmountLimit = dr.OrderAmountLimit,
                        HasCustomComs = dr.HasCustomComs,
                        Title = dr.Title,
                        TiApprovalType = (ApprovalTypeEnum) dr.ApprovalType.Value,
                        StartCalcState = (DistributeApplyStateEnum) dr.StartCalcState.Value,
                        RuleDesc = dr.RuleDesc
                    });
                distributRuleFullDto = temp.FirstOrDefault();
                if (distributRuleFullDto != null)
                {
                    List<DistributionIdentitySetFullDTO> vDbIdentitySets =
                        (from dis in DistributionIdentitySet.ObjectSet()
                            join di in DistributionIdentity.ObjectSet() on dis.Id equals di.IdentitySetId
                            where dis.RuleId == search.AppId && di.SubId == search.UserId
                            orderby dis.NameCategory
                            select new DistributionIdentitySetFullDTO
                            {
                                Id = dis.Id,
                                SubTime = dis.SubTime,
                                ModifiedOn = dis.ModifiedOn,
                                SubId = dis.SubId,
                                ModifiedId = dis.ModifiedId,
                                Name = dis.Name,
                                IsRequired = dis.IsRequired,
                                RuleId = dis.RuleId,
                                ValueType = (ApplyInfoTypeEnum) dis.ValueType.Value,
                                IsDel = dis.IsDel,
                                NameCategory = dis.NameCategory,
                                Value = di.Value
                            }).ToList();
                    distributRuleFullDto.DbIdentitySets = vDbIdentitySets;

                    //后台规则修改
                    if (CheckIsChange(search.AppId, search.UserId) && !search.IsLook)
                    {
                        List<DistributionIdentitySetFullDTO> dbIdentitySets =
                            (from dis in DistributionIdentitySet.ObjectSet()
                                where dis.RuleId == search.AppId && !dis.IsDel
                                orderby dis.NameCategory
                                select new DistributionIdentitySetFullDTO
                                {
                                    Id = dis.Id,
                                    SubTime = dis.SubTime,
                                    ModifiedOn = dis.ModifiedOn,
                                    SubId = dis.SubId,
                                    ModifiedId = dis.ModifiedId,
                                    Name = dis.Name,
                                    IsRequired = dis.IsRequired,
                                    RuleId = dis.RuleId,
                                    ValueType = (ApplyInfoTypeEnum) dis.ValueType.Value,
                                    IsDel = dis.IsDel,
                                    NameCategory = dis.NameCategory,
                                    Value = ""
                                }).ToList();
                        distributRuleFullDto.DbIdentitySets = dbIdentitySets;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return distributRuleFullDto;
        }

        /// <summary>
        /// 验证用户编辑的时候 时候后台规则有变化
        /// </summary>
        /// <returns></returns>
        public bool CheckIsChange(Guid appId, Guid userId)
        {
            bool isChange = false;
            List<DistributionIdentitySetFullDTO> vDbIdentitySets =
                (from dis in DistributionIdentitySet.ObjectSet()
                    join di in DistributionIdentity.ObjectSet() on dis.Id equals di.IdentitySetId
                    where dis.RuleId == appId && di.SubId == userId
                    orderby dis.NameCategory
                    select new DistributionIdentitySetFullDTO
                    {
                        Id = dis.Id,
                        SubTime = dis.SubTime,
                        ModifiedOn = dis.ModifiedOn,
                        SubId = dis.SubId,
                        ModifiedId = dis.ModifiedId,
                        Name = dis.Name,
                        IsRequired = dis.IsRequired,
                        RuleId = dis.RuleId,
                        ValueType = (ApplyInfoTypeEnum) dis.ValueType.Value,
                        IsDel = dis.IsDel,
                        NameCategory = dis.NameCategory,
                        Value = di.Value
                    }).ToList();

            List<DistributionIdentitySetFullDTO> dbIdentitySets =
                (from dis in DistributionIdentitySet.ObjectSet()
                    where dis.RuleId == appId && !dis.IsDel
                    orderby dis.NameCategory
                    select new DistributionIdentitySetFullDTO
                    {
                        Id = dis.Id,
                        SubTime = dis.SubTime,
                        ModifiedOn = dis.ModifiedOn,
                        SubId = dis.SubId,
                        ModifiedId = dis.ModifiedId,
                        Name = dis.Name,
                        IsRequired = dis.IsRequired,
                        RuleId = dis.RuleId,
                        ValueType = (ApplyInfoTypeEnum) dis.ValueType.Value,
                        IsDel = dis.IsDel,
                        NameCategory = dis.NameCategory,
                        Value = ""
                    }).ToList();

            if (vDbIdentitySets.Count != dbIdentitySets.Count)
            {
                isChange = true;
            }
            else
            {
                //字段名字或者字段类型 做变更
                if (vDbIdentitySets.Where((t, i) => t.Name != dbIdentitySets[i].Name || t.ValueType != dbIdentitySets[i].ValueType).Any())
                {
                    isChange = true;
                }
            }
            return isChange;
        }

        /// <summary>
        /// 保存分销商申请设置
        /// </summary>
        /// <returns></returns>
        public ResultDTO ModifyDistributRuleFullExt(DistributRuleFullDTO distributRuleFullDto)
        {
            var result = new ResultDTO { isSuccess = false };
            try
            {
                if (distributRuleFullDto == null)
                {
                    result.Message = "参数不能为空";
                    return result;
                }

                //先处理该appid下的历史数据
                ResultDTO ddr = DeleteDistributionRule(distributRuleFullDto.Id);
                if (ddr.isSuccess == false)
                {
                    result.Message = ddr.Message;
                    return result;
                }

                ApprovalTypeVO approval = new ApprovalTypeVO() { Value = (int)distributRuleFullDto.TiApprovalType };
                CalcOrderStateVO calc = new CalcOrderStateVO() { Value = (int)distributRuleFullDto.StartCalcState };

                var distributionRule = new DistributionRule
                {
                    Id = distributRuleFullDto.Id,
                    SubTime = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    SubId = ContextDTO.LoginUserID,
                    ModifiedId = ContextDTO.LoginUserID,
                    HasCondition = distributRuleFullDto.HasCondition,
                    NeedIdentity = distributRuleFullDto.NeedIdentity,
                    OrderTimeLimit = distributRuleFullDto.OrderTimeLimit,
                    OrderAmountLimit = distributRuleFullDto.OrderAmountLimit,
                    HasCustomComs = distributRuleFullDto.HasCustomComs,
                    Title = distributRuleFullDto.Title ?? "",
                    ApprovalType = approval,
                    StartCalcState = calc,
                    RuleDesc = distributRuleFullDto.RuleDesc,
                    EntityState = EntityState.Added
                };

                ContextFactory.CurrentThreadContext.SaveObject(distributionRule);
                if (distributRuleFullDto.DbIdentitySets != null)
                {
                    ResultDTO resultDto = SaveDistributionIdentitySet(distributRuleFullDto.Id, distributRuleFullDto.DbIdentitySets);
                    if (resultDto.isSuccess == false)
                    {
                        result.Message = resultDto.Message;
                        return result;
                    }
                }

                int esActivityCount = ContextFactory.CurrentThreadContext.SaveChanges();
                if (esActivityCount > 0)
                {
                    result.isSuccess = true;
                    result.Message = "添加成功";
                }
                else
                {
                    result.Message = "添加失败";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ModifyDistributRuleFullExt异常，异常信息：", ex);

                result.isSuccess = false;
                result.Message = "出现异常";
                return result;
            }
        }

        /// <summary>
        /// 保存相应的审核资料
        /// </summary>
        public ResultDTO SaveDistributionIdentitySet(Guid drId, List<DistributionIdentitySetFullDTO> distributionIdentitySetFullDtos)
        {
            var result = new ResultDTO { isSuccess = true };
            int i = 0;
            foreach (var d in distributionIdentitySetFullDtos)
            {
                var dis = new DistributionIdentitySet
                {
                    Id = Guid.NewGuid(),
                    SubTime = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    SubId = ContextDTO.LoginUserID,
                    ModifiedId = ContextDTO.LoginUserID,
                    Name = d.Name,
                    IsRequired = d.IsRequired,
                    RuleId = drId,
                    ValueType = d.ValueType,
                    IsDel = false,
                    NameCategory = i,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(dis);
                i++;
            }
            return result;
        }

        /// <summary>
        /// 变更审核状态
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="distributeApplyStateEnum"></param>
        /// <returns></returns>
        public ResultDTO UpdateDistributionApplyStateExt(Guid appId, Guid userId, DistributeApplyStateEnum distributeApplyStateEnum)
        {
            var result = new ResultDTO { isSuccess = false };
            var distributionApply = DistributionApply.ObjectSet().FirstOrDefault(t => t.UserId == userId && t.RuleId == appId);
            if (distributionApply != null)
            {
                distributionApply.State = distributeApplyStateEnum;
                distributionApply.ModifiedOn = DateTime.Now;
                distributionApply.EntityState = EntityState.Modified;
            }

            ContextFactory.CurrentThreadContext.SaveObject(distributionApply);
            int daCount = ContextFactory.CurrentThreadContext.SaveChanges();
            if (daCount > 0)
            {
                result.isSuccess = true;
                LogHelper.Info("UpdateDistributionApplyState变更状态成功appId：" + appId + ",userId:" + userId + ",distributeApplyStateEnum:" + distributeApplyStateEnum + "");
            }
            else
            {
                LogHelper.Info("UpdateDistributionApplyState变更状态失败appId：" + appId + ",userId:" + userId + ",distributeApplyStateEnum:" + distributeApplyStateEnum + "");
            }
            return result;
        }

        /// <summary>
        /// 删除相关的设置以及资料
        /// </summary>
        /// <param name="drId"></param>
        /// <returns></returns>
        public ResultDTO DeleteDistributionRule(Guid drId)
        {
            var result = new ResultDTO { isSuccess = true };
            var dis = DistributionIdentitySet.ObjectSet().Where(t => t.RuleId == drId);
            foreach (var d in dis)
            {
                d.IsDel = true;
                d.EntityState = EntityState.Modified;
                ContextFactory.CurrentThreadContext.SaveObject(d);
            }

            var dr = DistributionRule.ObjectSet().FirstOrDefault(t => t.Id == drId);
            if (dr != null)
            {
                dr.EntityState = EntityState.Deleted;
                ContextFactory.CurrentThreadContext.SaveObject(dr);
            }
            return result;
        }

        /// <summary>
        /// 添加或编辑申请成为分销商资料
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userId"></param>
        /// <param name="ruleId"></param>
        /// <param name="distributApplyFullDto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributionIdentityInfoExt(string userCode, string userId, string ruleId, DistributApplyFullDTO distributApplyFullDto)
        {
            var result = new ResultDTO { isSuccess = false };
            try
            {
                if (string.IsNullOrEmpty(userId) || (string.IsNullOrEmpty(ruleId) || distributApplyFullDto == null))
                {
                    result.Message = "参数不能为空";
                    return result;
                }

                var drId = Guid.NewGuid();
                var distributionApply = DistributionApply.ObjectSet().FirstOrDefault(t => t.RuleId == new Guid(ruleId) && t.UserId == new Guid(userId));
                //新增
                if (distributionApply == null)
                {
                    var dr = DistributionRule.ObjectSet().FirstOrDefault(t => t.Id == new Guid(ruleId));
                    List<Guid> users = new List<Guid> { new Guid(userId) };
                    var user = CBCSV.Instance.GetUserInfoWithAccountList(users)[0];

                    var da = new DistributionApply()
                    {
                        Id = drId,
                        SubTime = distributApplyFullDto.ModifiedOn = DateTime.Now,
                        HasIdentity = dr != null && dr.NeedIdentity,
                        RuleId = new Guid(ruleId),
                        UserId = new Guid(userId),
                        UserCode = userCode,
                        UserName = user.Name,
                        PicturePath = user.HeadIcon,
                        Remarks = "",
                        ParentId = distributApplyFullDto.ParentId,
                        State = DistributeApplyStateEnum.Other,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(da);
                }
                //修改
                else
                {
                    drId = distributionApply.Id;
                }

                //先删除相关审核内容
                var distributionIdentityList = DistributionIdentity.ObjectSet().Where(t => t.RuleId == new Guid(ruleId) && t.SubId == new Guid(userId));
                foreach (var distributionIdentity in distributionIdentityList)
                {
                    distributionIdentity.EntityState = EntityState.Deleted;
                    ContextFactory.CurrentThreadContext.SaveObject(distributionIdentity);
                }
                if (distributApplyFullDto.DistributionIdentityFullDtos != null)
                {
                    if (distributionApply != null)
                    {
                        distributionApply.State = !distributApplyFullDto.IsModified ? DistributeApplyStateEnum.PendingAudit : DistributeApplyStateEnum.AuditAgain;
                        distributionApply.ModifiedOn = DateTime.Now;
                        ContextFactory.CurrentThreadContext.SaveObject(distributionApply);
                    }

                    ResultDTO resultDto = SaveDistributionIdentity(drId, new Guid(ruleId), distributApplyFullDto.DistributionIdentityFullDtos);
                    if (resultDto.isSuccess == false)
                    {
                        result.Message = "保存申请信息失败";
                        return result;
                    }
                }
                else
                {
                    //添加不同用户的审核资料
                    var distributionIdentitySetList = DistributionIdentitySet.ObjectSet().Where(t => t.RuleId == new Guid(ruleId) && !t.IsDel);
                    var i = 0;
                    foreach (var distributionIdentitySet in distributionIdentitySetList)
                    {
                        var di = new DistributionIdentity
                        {
                            Id = Guid.NewGuid(),
                            SubTime = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            SubId = ContextDTO.LoginUserID,
                            ModifiedId = ContextDTO.LoginUserID,
                            Name = distributionIdentitySet.Name,
                            IdentitySetId = distributionIdentitySet.Id,
                            RuleId = new Guid(ruleId),
                            ApplyId = drId,
                            ValueType = distributionIdentitySet.ValueType,
                            Value = "",
                            NameCategory = i,
                            EntityState = EntityState.Added
                        };
                        ContextFactory.CurrentThreadContext.SaveObject(di);
                        i++;
                    }
                }

                int esActivityCount = ContextFactory.CurrentThreadContext.SaveChanges();
                if (esActivityCount > 0)
                {
                    result.isSuccess = true;
                    result.Message = Convert.ToString(drId);
                }
                else
                {
                    result.Message = "添加失败";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Error("AddDistributionIdentityInfoExt异常，异常信息：", ex);

                result.isSuccess = false;
                result.Message = "出现异常";
                return result;
            }
        }

        /// <summary>
        /// 保存相应的申请信息
        /// </summary>
        public ResultDTO SaveDistributionIdentity(Guid daId, Guid ruleId, List<DistributionIdentityFullDTO> distributionIdentityFullDtos)
        {
            var result = new ResultDTO { isSuccess = true };
            int i = 0;
            foreach (var d in distributionIdentityFullDtos)
            {
                var di = new DistributionIdentity
                {
                    Id = Guid.NewGuid(),
                    SubTime = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    SubId = ContextDTO.LoginUserID,
                    ModifiedId = ContextDTO.LoginUserID,
                    Name = d.Name,
                    IdentitySetId = d.IdentitySetId,
                    RuleId = ruleId,
                    ApplyId = daId,
                    ValueType = d.ValueType,
                    Value = d.Value,
                    NameCategory = i,
                    EntityState = EntityState.Added
                };
                ContextFactory.CurrentThreadContext.SaveObject(di);
                i++;
            }
            return result;
        }

        /// <summary>
        /// 获取用户审核状态
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultDTO GetDistributeStateExt(Guid appId, Guid userId)
        {
            ResultDTO result = new ResultDTO() { isSuccess = true };
            var da = DistributionApply.ObjectSet().FirstOrDefault(t => t.RuleId == appId && t.UserId == userId);
            if (da != null)
            {
                result.ResultCode = da.State.Value;
                if (result.ResultCode == (int)DistributeApplyStateEnum.AuditRefuse)
                {
                    result.Message = da.RefuseReason;
                }
                return result;
            }
            result.ResultCode = -1;
            return result;
        }

        /// <summary>
        /// 获取分销商微小店信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributionMicroShopDTO GetDistributionMicroShopExt(Guid appId, Guid userId)
        {
            DistributionMicroShopDTO distributionMicroShopDto = new DistributionMicroShopDTO();
            try
            {
                var temp = (from ms in Microshop.ObjectSet()
                            where ms.AppId == appId && ms.UserId == userId
                            select new DistributionMicroShopDTO
                            {
                                Id = ms.Id,
                                SubTime = ms.SubTime,
                                ModifiedOn = ms.ModifiedOn,
                                AppId = ms.AppId,
                                UserId = ms.UserId,
                                Logo = ms.Logo,
                                QRCodeUrl = ms.QRCodeUrl,
                                Name = ms.Name,
                                Url = ms.Url,
                                Type = (MicroshopTypeEnum)ms.Type.Value,
                                Key = ms.Key
                            });
                distributionMicroShopDto = temp.FirstOrDefault();
                if (distributionMicroShopDto != null)
                {
                    //获取下架商品
                    var commodityIds = MicroshopCom.ObjectSet().Where(t => !t.IsDel && t.MicroshopId == distributionMicroShopDto.AppId).Select(c => c.CommodityId).Distinct().ToArray();
                    var dc = (from cd in CommodityDistribution.ObjectSet()
                              join c in Commodity.ObjectSet() on cd.Id equals c.Id
                              where commodityIds.Contains(c.Id)
                              select new CommodityVM
                              {
                                  Id = c.Id,
                                  Name = c.Name,
                                  Price = c.Price,
                                  MarketPrice = c.MarketPrice,
                                  Pic = c.PicturesPath,
                                  L1Percent = Math.Round((decimal)(cd.L1Percent * c.Price), 2),
                                  L2Percent = Math.Round((decimal)(cd.L2Percent * c.Price), 2),
                                  L3Percent = Math.Round((decimal)(cd.L3Percent * c.Price), 2)
                              });
                    List<CommodityVM> commodityVmsD = new List<CommodityVM>();
                    foreach (var commodityVm in dc)
                    {
                        if (commodityVm.L1Percent == null || commodityVm.L2Percent == null || commodityVm.L3Percent == null)
                        {
                            var model = AppExtension.ObjectSet().FirstOrDefault(t => t.Id == appId);
                            commodityVm.L1Percent = Math.Round((decimal)(model.DistributeL1Percent*commodityVm.Price), 2);
                            commodityVm.L2Percent = Math.Round((decimal)(model.DistributeL2Percent*commodityVm.Price), 2);
                            commodityVm.L3Percent = Math.Round((decimal)(model.DistributeL3Percent*commodityVm.Price), 2);
                        }
                        commodityVmsD.Add(commodityVm);
                    }
                    distributionMicroShopDto.DownCommodityDtos = commodityVmsD.ToList();

                    //获取符合条件的上架商品
                    var uc = (from cd in CommodityDistribution.ObjectSet()
                              join c in Commodity.ObjectSet() on cd.Id equals c.Id
                              where c.AppId == appId && !c.IsDel && c.CommodityType == 0 && c.State == 0 && !commodityIds.Contains(c.Id)
                              select new CommodityVM
                                            {
                                                Id = c.Id,
                                                Name = c.Name,
                                                Price = c.Price,
                                                MarketPrice = c.MarketPrice,
                                                Pic = c.PicturesPath,
                                                L1Percent = Math.Round((decimal)(cd.L1Percent * c.Price), 2),
                                                L2Percent = Math.Round((decimal)(cd.L2Percent * c.Price), 2),
                                                L3Percent = Math.Round((decimal)(cd.L3Percent * c.Price), 2)
                                            });
                    List<CommodityVM> commodityVmsU = new List<CommodityVM>();
                    foreach (var commodityVm in uc)
                    {
                        if (commodityVm.L1Percent == null || commodityVm.L2Percent == null || commodityVm.L3Percent == null)
                        {
                            var model = AppExtension.ObjectSet().FirstOrDefault(t => t.Id == appId);
                            commodityVm.L1Percent = Math.Round((decimal)(model.DistributeL1Percent * commodityVm.Price), 2);
                            commodityVm.L2Percent = Math.Round((decimal)(model.DistributeL2Percent * commodityVm.Price), 2);
                            commodityVm.L3Percent = Math.Round((decimal)(model.DistributeL3Percent * commodityVm.Price), 2);
                        }
                        commodityVmsU.Add(commodityVm);
                    }
                    distributionMicroShopDto.UpCommodityDtos = commodityVmsU.ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
            return distributionMicroShopDto;
        }

        /// <summary>
        /// 修改微小店信息
        /// </summary>
        public ResultDTO UpdateDistributionMicroShopExt(MicroshopDTO microshopDto)
        {
            var result = new ResultDTO { isSuccess = true };

            var microshop = Microshop.ObjectSet().FirstOrDefault(t => t.Id == microshopDto.Id);
            if (microshop == null)
            {
                result.isSuccess = true;
                result.Message = "获取数据失败";
                return result;
            }

            if (!string.IsNullOrEmpty(microshopDto.Logo))
            {
                microshop.Logo = microshopDto.Logo;
                microshop.QRCodeUrl = microshopDto.QRCodeUrl;
            }
            if (!string.IsNullOrEmpty(microshopDto.Name))
            {
                microshop.Name = microshopDto.Name;
            }
            microshop.ModifiedOn = DateTime.Now;
            microshop.EntityState = EntityState.Modified;
            ContextFactory.CurrentThreadContext.SaveObject(microshop);

            int mcCount = ContextFactory.CurrentThreadContext.SaveChanges();
            if (mcCount > 0)
            {
                result.isSuccess = true;
                result.Message = "修改成功";
            }
            else
            {
                result.Message = "修改失败";
            }
            return result;
        }

        /// <summary>
        /// 微小店 下架商品
        /// </summary>
        public ResultDTO SaveMicroshopComExt(MicroshopComDTO microshopComDto)
        {
            var result = new ResultDTO { isSuccess = false };
            var mc = new MicroshopCom
            {
                Id = Guid.NewGuid(),
                SubTime = DateTime.Now,
                ModifiedOn = DateTime.Now,
                CommodityId = microshopComDto.CommodityId,
                MicroshopId = microshopComDto.MicroshopId,
                IsDel = false,
                EntityState = EntityState.Added
            };
            ContextFactory.CurrentThreadContext.SaveObject(mc);

            int mcCount = ContextFactory.CurrentThreadContext.SaveChanges();
            if (mcCount > 0)
            {
                result.isSuccess = true;
                result.Message = "添加成功";
            }
            else
            {
                result.Message = "添加失败";
            }
            return result;
        }

        /// <summary>
        /// 微小店 上架商品
        /// </summary>
        public ResultDTO UpdateMicroshopComExt(Guid microshopComId, Guid microshopId)
        {
            var result = new ResultDTO { isSuccess = false };

            var microshopCom = MicroshopCom.ObjectSet().FirstOrDefault(t => t.CommodityId == microshopComId && t.MicroshopId == microshopId && !t.IsDel);
            if (microshopCom == null)
            {
                result.isSuccess = true;
                result.Message = "获取数据失败";
                return result;
            }

            microshopCom.ModifiedOn = DateTime.Now;
            microshopCom.IsDel = true;
            microshopCom.EntityState = EntityState.Modified;
            ContextFactory.CurrentThreadContext.SaveObject(microshopCom);

            int mcCount = ContextFactory.CurrentThreadContext.SaveChanges();
            if (mcCount > 0)
            {
                result.isSuccess = true;
                result.Message = "上架成功";
            }
            else
            {
                result.Message = "上架失败";
            }
            return result;
        }
    }
}