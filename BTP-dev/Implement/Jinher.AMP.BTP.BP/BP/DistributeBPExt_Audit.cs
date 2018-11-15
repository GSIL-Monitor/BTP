
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
using System.Diagnostics;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.Enum;
using Jinher.JAP.Common.Loging;
using MongoDB.Bson.Serialization.Serializers;
using NPOI.HSSF.Model;
using CommodityDistributionDTO = Jinher.AMP.BTP.Deploy.CommodityDistributionDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DistributeBP : BaseBP, IDistribute
    {

        /// <summary>
        /// 查询 分销商申请 的身份设置
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public List<DistributionIdentityDTO> GetDistributorApplyIdentityValsExt(Guid applyId)
        {
            var identityEntities = DistributionIdentity.ObjectSet()
                .Where(t => t.ApplyId == applyId)
                .OrderBy(x => x.NameCategory)
                .ToList();
            var identityDtos = identityEntities.Select(x => x.ToEntityData()).ToList();

            return identityDtos;
        }

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="applyId"></param>
        /// <returns></returns>
        public DistributionApplyDTO GetDistributionApplyExt(Guid applyId)
        {
            var applyEntity = DistributionApply.ObjectSet().FirstOrDefault(x => x.Id == applyId);
            return applyEntity == null ? null : applyEntity.ToEntityData();
        }

        /// <summary>
        /// 查询一条 分销商申请 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributionApplyDTO GetDistributionApplyExt(Guid ruleId, Guid userId)
        {
            var applyEntity = DistributionApply.ObjectSet().FirstOrDefault(x => x.RuleId == ruleId && x.UserId == userId);
            return applyEntity == null ? null : applyEntity.ToEntityData();
        }

        /// <summary>
        /// 查询一条 分销商申请资料设置 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public DistributionRuleDTO GetDistributionRuleExt(Guid ruleId)
        {
            var applyEntity = DistributionRule.ObjectSet().FirstOrDefault(x => x.Id == ruleId);
            return applyEntity == null ? null : applyEntity.ToEntityData();
        }

        /// <summary>
        /// 查询 分销商申请资料设置 的身份设置
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public List<DistributionIdentitySetDTO> GetDistributionIdentitySetsExt(Guid ruleId)
        {
            var setEntities = DistributionIdentitySet.ObjectSet()
                .Where(t => t.RuleId == ruleId)
                .OrderBy(x => x.NameCategory)
                .ToList();
            var setDtos = setEntities.Select(x => x.ToEntityData()).ToList();

            return setDtos;
        }


        /// <summary>
        /// 查询某个app的 分销商申请 列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<DistributionApplyResultDTO> GetDistributionApplyListExt(Guid appId, int pageSize, int pageIndex,
            out int rowCount)
        {
            return GetDistributionApplyListByWhereExt(appId, pageSize, pageIndex, out rowCount, "", -1);
        }

        /// <summary>
        /// 查询某个app符合条件的 分销商申请 列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="userName"></param> 
        /// <param name="state"></param>
        /// <returns></returns>
        public List<DistributionApplyResultDTO> GetDistributionApplyListByWhereExt(Guid appId, int pageSize,
            int pageIndex,
            out int rowCount, string userName, int state)
        {
            //昵称的NameCategory
            var nameCategory_Name = 0;
            var query = from apply in DistributionApply.ObjectSet().Where(x => x.RuleId == appId)
                        join setval in DistributionIdentity.ObjectSet().Where(x => x.NameCategory == nameCategory_Name) on
                            apply.Id equals setval.ApplyId into setval2
                        from setval in setval2.DefaultIfEmpty()
                        select new { Apply = apply, SetVal = setval };

            if (state == 1)
                query = query.Where(x => x.Apply.State.Value == 1 || x.Apply.State.Value == 4);
            else if (state > 0)
                query = query.Where(x => x.Apply.State.Value == state);
            else
                query = query.Where(x => x.Apply.State.Value != 0);//不显示0状态

            if (!string.IsNullOrWhiteSpace(userName))
                query = query.Where(x => x.Apply.UserName.Contains(userName) || x.Apply.UserCode.Contains(userName) || x.SetVal.Value.Contains(userName));

            rowCount = query.Count();
            var queryOrdered = query.OrderByDescending(x => x.Apply.SubTime);
            var entities = queryOrdered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            LogHelper.Debug(string.Format("GetDistributionApplyListByWhereExt(userName:{0},state:{1})，查询结果：{2}条记录。", userName, state, rowCount));

            if (entities.Any())
            {
                var applyIds = string.Join(",", entities.Select(x => "Guid'" + x.Apply.Id + "'").ToArray());

                var allapplysIdentityVals = DistributionIdentity.ObjectSet()
                    .Where("it.ApplyId in {" + applyIds + "}")
                    .Where(x => !string.IsNullOrEmpty(x.Value))//排除value为空的设置
                    .ToList();

                //根据applyId取identity
                Func<Guid, List<DistributionIdentityDTO>> identityVals = (Guid applyId) =>
                    allapplysIdentityVals.Where(x => x.ApplyId == applyId)
                    .Select(x => x.ToEntityData()).ToList();

                var dtos =
                    entities.Select(x => new DistributionApplyResultDTO(x.Apply.ToEntityData(), identityVals(x.Apply.Id))).ToList();
                return dtos;
            }
            else
            {
                return new List<DistributionApplyResultDTO>();
            }
        }

        /// <summary>
        /// 获取分销商的所有身份设置
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public List<DistributorsHasIdentityResultDTO> GetDistributorsIdentitysExt(List<Guid> distributorList)
        {
            var result = new List<DistributorsHasIdentityResultDTO>();
            var distributorIds = string.Join(",", distributorList.Select(x => "Guid'" + x + "'").ToArray());

            if (distributorIds.Any())
            {
                var distributorObjectSet = Distributor.ObjectSet()
                    .Where("it.Id in {" + distributorIds + "}");

                var stateAudit = (int)DistributeApplyStateEnum.Audit;
                var applyList = (from distributor in distributorObjectSet
                                 join apply in DistributionApply.ObjectSet() on
                                     new { UserId = distributor.UserId, AppId = distributor.EsAppId } equals
                                     new { UserId = apply.UserId, AppId = apply.RuleId }
                                 where apply.State.Value == stateAudit
                                 select new { DistributorId = distributor.Id, ApplyId = apply.Id, PicturePath = apply.PicturePath })
                    .Distinct()
                    .ToList();

                if (applyList.Any())
                {
                    var applyIds = string.Join(",",
                        applyList.Select(x => x.ApplyId).Select(x => "Guid'" + x + "'").ToArray());
                    var identitysList =
                        DistributionIdentity.ObjectSet().Where("it.ApplyId in {" + applyIds + "}").ToList();

                    Func<Guid, Guid> getApplyFunc = (distributorId) =>
                    {
                        var first = applyList.FirstOrDefault(x => x.DistributorId == distributorId);
                        return first == null ? Guid.Empty : first.ApplyId;
                    };
                    Func<Guid, string> getPicturePathFunc = (distributorId) =>
                    {
                        var first = applyList.FirstOrDefault(x => x.DistributorId == distributorId);
                        return first == null ? "" : first.PicturePath;
                    };
                    Func<Guid, bool> hasIdentityFunc = (applyId) =>
                    {
                        var identitys = identitysList.Where(x => x.ApplyId == applyId);
                        return identitys.Any();
                    };
                    Func<Guid, List<DistributionIdentityDTO>> getIdentitysFunc = (applyId) =>
                    {
                        var identitys = identitysList.Where(x => x.ApplyId == applyId);
                        return identitys.Select(x => x.ToEntityData()).ToList();
                    };

                    distributorList.ForEach(x => result.Add(new DistributorsHasIdentityResultDTO
                    {
                        DistributorId = x,
                        ApplyId = getApplyFunc(x),
                        PicturePath = getPicturePathFunc(x),
                        HasIdentity = hasIdentityFunc(getApplyFunc(x)),
                        Identitys = getIdentitysFunc(getApplyFunc(x))
                    }));
                }
            }

            return result;
        }

        /// <summary>
        /// 获取分销商们的备注信息
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> GetDistributorsRemarksExt(List<Guid> distributorList)
        {
            var result = new Dictionary<Guid, string>();
            if (distributorList.Any())
            {
                var distributorIds = string.Join(",", distributorList.Select(x => "Guid'" + x + "'").ToArray());

                var distributors = Distributor.ObjectSet()
                    .Where("it.Id in {" + distributorIds + "}")
                    .Select(x => new { Id = x.Id, Remarks = x.Remarks })
                    .ToList();

                distributors.ForEach(x => result.Add(x.Id, x.Remarks));
            }

            return result;
        }

        /// <summary>
        /// 备注 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public ResultDTO RemarkDistributionApplyExt(Guid id, string remarks)
        {
            var applyId = id;
            return UpdateDistributionApplyAndAudit(applyId, false, false, remarks);
        }

        /// <summary>
        /// 获取分销商申请的审批历史记录
        /// </summary>
        /// <param name="applyId"></param>
        public List<DistributionApplyAuditListDTO> GetApplyAuditListExt(Guid applyId)
        {
            var audits = DistributionApplyAudit.ObjectSet()
                .Where(x => x.ApplyId == applyId)
                .OrderByDescending(x => x.SubTime)
                .ToList();

            if (audits.Count > 0)
            {
                var users = CBCSV.Instance.GetUserNameAccountsByIds(audits.Select(x => x.SubId).ToList());
                Func<Guid, string> getUserName = (subId) =>
                {
                    if (users == null)
                        return subId.ToString();
                    var user = users.FirstOrDefault(x => x.userId == subId);
                    return user == null ? subId.ToString() : user.userName;
                };
                var auditDtos = audits.Select(x => new DistributionApplyAuditListDTO(x.ToEntityData(), getUserName(x.SubId)))
                    .ToList();
                return auditDtos;
            }
            return new List<DistributionApplyAuditListDTO>();
        }

        /// <summary>
        /// 备注 分销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public ResultDTO RemarkDistributorExt(Guid id, string remarks)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var now = DateTime.Now;

            var apply = Distributor.ObjectSet().FirstOrDefault(t => t.Id == id);
            if (apply == null)
                return new ResultDTO<DistributionApply> { isSuccess = false, ResultCode = 0, Message = "没有找到ID='" + id + "'的数据！", Data = null };

            apply.Remarks = remarks;
            apply.ModifiedOn = now;
            apply.EntityState = EntityState.Modified;

            contextSession.SaveChanges();

            return new ResultDTO { isSuccess = true, ResultCode = 1 };
        }

        /// <summary>
        /// 审核 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPass"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public ResultDTO<AuditDistributionResultDTO> AuditingDistributionApplyExt(Guid id, bool isPass,
            string refuseReason)
        {
            var applyRet = UpdateDistributionApplyAndAudit(id, true, isPass, refuseReason);
            if (!applyRet.isSuccess)
                return new ResultDTO<AuditDistributionResultDTO>
                {
                    isSuccess = false,
                    ResultCode = 0,
                    Message = applyRet.Message,
                    Data = null
                };

            var apply = applyRet.Data;
            var appId = apply.RuleId; //Apply的RuleId是DistributionRule的Id，而它就是AppId
            List<AuditRet4Wx> auditRet4Wxs;
            var appDistributorCount = 0;
            AuditDistributionResultDTO result;

            if (isPass)
            {
                var afterPass = PassAuditing_DistributorAndMicroshop(apply);
                var distributor = afterPass.Item1;
                var microshop = afterPass.Item2;

                auditRet4Wxs = GetAuditRet4WxsWhenPass(apply, distributor, microshop.Url, out appDistributorCount);

                result = new AuditDistributionResultDTO
                {
                    ApplyId = appId,
                    IsPass = true,
                    DistributorId = distributor.Id,
                    MicroShopId = microshop.Id,
                    MicroShopUrl = microshop.Url,
                    MicroShopLogo = microshop.Logo
                };
            }
            else
            {
                var auditRet4Wx = new AuditRet4Wx(apply.UserId, apply.UserName, apply.UserCode, apply.RefuseReason);
                auditRet4Wxs = new List<AuditRet4Wx> { auditRet4Wx };

                result = new AuditDistributionResultDTO { ApplyId = appId, IsPass = false };
            }

            new Task((x) =>
            {
                var p = (Tuple<Guid, List<AuditRet4Wx>, int, bool>)x;
                var data4SendMessage = GetData4SendMessage(p.Item1, p.Item2, p.Item3, p.Item4);
                var data4Wx = data4SendMessage.Item1;
                var data4App = data4SendMessage.Item2;

                SendMessageToWx(appId, data4Wx);
                SendMessageToApp(data4App);

            }, new Tuple<Guid, List<AuditRet4Wx>, int, bool>(appId, auditRet4Wxs, appDistributorCount, isPass)).Start();

            return new ResultDTO<AuditDistributionResultDTO>
            {
                isSuccess = true,
                ResultCode = 0,
                Message = "OK",
                Data = result
            };
        }

        /// <summary>
        /// 更新DistributionApply表，插入DistributionApplyAudit记录
        /// </summary>
        /// <param name="applyId"></param>
        /// <param name="isAuditing"></param>
        /// <param name="isAuditingPass"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        private ResultDTO<DistributionApply> UpdateDistributionApplyAndAudit(Guid applyId, bool isAuditing, bool isAuditingPass, string txt)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var now = DateTime.Now;

            var apply = DistributionApply.ObjectSet().FirstOrDefault(t => t.Id == applyId);
            if (apply == null)
                return new ResultDTO<DistributionApply> { isSuccess = false, ResultCode = 0, Message = "没有找到ID='" + applyId + "'的数据！", Data = null };

            var refuseTxt = "";
            var detailTxt = "";
            if (isAuditing)
            {
                if (isAuditingPass)
                {
                    if (apply.State.EnumValue == DistributeApplyStateEnum.Audit)
                        return new ResultDTO<DistributionApply> { isSuccess = false, ResultCode = 0, Message = "此申请已经通过审核，不能重复通过审核！", Data = null };

                    apply.State = new DistributionApplySate
                    {
                        EnumValue = DistributeApplyStateEnum.Audit,
                        Value = (int)DistributeApplyStateEnum.Audit
                    };
                }
                else
                {
                    apply.State = new DistributionApplySate
                    {
                        EnumValue = DistributeApplyStateEnum.AuditRefuse,
                        Value = (int)DistributeApplyStateEnum.AuditRefuse
                    };
                    apply.RefuseReason = txt;
                    refuseTxt = txt;
                }
            }
            else
            {
                apply.Remarks = txt;
                detailTxt = txt;
            }
            apply.ModifiedOn = now;
            apply.EntityState = EntityState.Modified;

            //IsPass取当前entity的状态
            var isPass = apply.State.EnumValue == DistributeApplyStateEnum.Audit;
            var audit = new DistributionApplyAudit
            {
                Id = Guid.NewGuid(),
                ApplyId = applyId,
                SubTime = now,
                SubId = ContextDTO.LoginUserID,
                Details = detailTxt,
                RefuseReason = refuseTxt,
                IsPass = isPass,
                EntityState = EntityState.Added
            };
            contextSession.SaveObject(audit);

            contextSession.SaveChanges();

            return new ResultDTO<DistributionApply> { isSuccess = true, ResultCode = 1, Data = apply };
        }

        private Tuple<Distributor, Microshop> PassAuditing_DistributorAndMicroshop(DistributionApply apply)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            var now = DateTime.Now;

            var appId = apply.RuleId;//Apply的RuleId是DistributionRule的Id，而它就是AppId
            var newDistributorId = Guid.NewGuid();

            var level = apply.ParentId.Equals(Guid.Empty) ? 1
                : Distributor.ObjectSet().First(x => x.Id == apply.ParentId).Level + 1;
            var parentDistributorIds = new List<Guid>
            {
                GetDistributorGrandFatherId(apply.ParentId),
                apply.ParentId,
                newDistributorId
            }.Where(x => !x.Equals(Guid.Empty))
            .Select(x => x.ToString()).ToArray();

            var distributorKey = string.Join(",", parentDistributorIds);
            var remarks = "";
            var distributor = new Distributor
            {
                Id = newDistributorId,
                UserId = apply.UserId,
                UserName = apply.UserName,
                UserCode = apply.UserCode,
                SubTime = now,
                ModifiedOn = now,
                EsAppId = appId,
                ParentId = apply.ParentId,
                Level = level,
                Key = distributorKey,
                UserSubTime = apply.SubTime,
                PicturePath = apply.PicturePath,
                Remarks = remarks,
                EntityState = EntityState.Added
            };
            contextSession.SaveObject(distributor);

            var logo = "";
            var userInfo = CBCSV.Instance.GetUserInfoWithAccountList(new List<Guid> { apply.UserId });
            if (userInfo != null && userInfo.Count > 0)
            {
                logo = userInfo[0].HeadIcon;
            }
            else
            {
                LogHelper.Error(string.Format("调用CBCSV.Instance.GetUserInfoWithAccountList({0})，没有返回有效值值！", apply.UserId));
            }

            var qrCodeUrl = "";//暂时为空，后续通过update动作赋值
            var newMicroshopId = Guid.NewGuid();
            var microshopType = MicroshopTypeEnum.Distribution;
            var microshopUrl = CreateMicroShopUrl(appId, newMicroshopId, distributor.Id);
            var microshop = new Microshop
            {
                Id = newMicroshopId,
                SubTime = now,
                ModifiedOn = now,
                AppId = appId,
                UserId = apply.UserId,
                Logo = logo,
                QRCodeUrl = qrCodeUrl,
                Name = string.Format("{0}的小店", apply.UserName),
                Url = microshopUrl,
                Type = microshopType,
                Key = distributor.Id,
                EntityState = EntityState.Added
            };
            contextSession.SaveObject(microshop);

            contextSession.SaveChanges();

            return new Tuple<Distributor, Microshop>(distributor, microshop);
        }

        private string CreateMicroShopUrl(Guid appId, Guid microShopId, Guid distributorId)
        {
            //微小店地址要求：必要参数appId（商城的Id），微小店Id，distributorid（分销商Id）
            return string.Format("{0}/Distribute/MicroshopIndex?appid={1}&microshopId={2}&distributorId={3}"
                , Common.CustomConfig.BtpDomain, appId, microShopId, distributorId);
        }

        /// <summary>
        /// 获取一个分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributorDTO GetDistributorByExt(Guid appId, Guid userId)
        {
            var entity = Distributor.ObjectSet().FirstOrDefault(x => x.EsAppId == appId && x.UserId == userId);
            return entity == null ? null : entity.ToEntityData();
        }

        /// <summary>
        /// 查询 分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<DistributorDTO> GetDistributorsExt(Guid appId, int pageSize, int pageIndex, out int rowCount)
        {
            var query = Distributor.ObjectSet().Where(x => x.EsAppId == appId);
            rowCount = query.Count();
            var queryOrdered = query.OrderByDescending(x => x.SubTime);
            var entities = queryOrdered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return entities.Select(x => x.ToEntityData()).ToList();
        }


        /// <summary>
        /// 更新微小店QrCode
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopQrCodeExt(UpdateQrCodeRequestDTO dto)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            var microShop = Microshop.ObjectSet().FirstOrDefault(t => t.Id == dto.MicroShopId);
            if (microShop == null)
            {
                return new ResultDTO { isSuccess = false, ResultCode = 0, Message = "没有找到ID='" + dto.MicroShopId + "'的数据" };
            }

            microShop.QRCodeUrl = dto.QRCodeUrl;
            microShop.EntityState = EntityState.Modified;

            contextSession.SaveChanges();

            return new ResultDTO { isSuccess = true, ResultCode = 1, Message = "OK" };
        }

        /// <summary>
        /// 新增分销商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributorExt(DistributorDTO dto)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            dto.EntityState = EntityState.Added;
            contextSession.SaveObject(dto);

            contextSession.SaveChanges();

            return new ResultDTO { isSuccess = true, ResultCode = 1, Message = "OK" };
        }

        /// <summary>
        /// 根据Id获取微小店
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshopExt(Guid microshopId)
        {
            var entity = Microshop.ObjectSet().FirstOrDefault(x => x.Id == microshopId);
            return entity == null ? null : entity.ToEntityData();
        }

        /// <summary>
        /// 获取微小店
        /// </summary>
        /// <param name="appId"></param>
        /// /// <param name="userId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshopExt(Guid appId, Guid userId)
        {
            var entity = Microshop.ObjectSet().FirstOrDefault(x => x.AppId == appId && x.UserId == userId);
            return entity == null ? null : entity.ToEntityData();
        }

        /// <summary>
        /// 获取某应用的所有加入微小店的商品Id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<CommodityDistributionDTO> GetAppAllMicroshopCommoditysExt(Guid appId)
        {
            var entitys = (from commodity in Commodity.ObjectSet()
                           where commodity.AppId == appId
                           join commodityDistribution in CommodityDistribution.ObjectSet() on commodity.Id equals
                               commodityDistribution.Id
                           select commodityDistribution).ToList();
            return entitys.Select(x => x.ToEntityData()).ToList();
        }

        /// <summary>
        /// 获取某微小店的所有下架商品Id
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public List<Guid> GetMicroshopOfflineCommodityIdsExt(Guid microshopId)
        {
            var strMicroshopId = microshopId;
            var commodityIds =
                MicroshopCom.ObjectSet()
                    .Where(x => x.MicroshopId == strMicroshopId)
                    .Select(x => x.CommodityId)
                    .ToList();
            return commodityIds;
        }

        private Guid GetDistributorGrandFatherId(Guid fatherId)
        {
            if (fatherId.Equals(Guid.Empty))
                return Guid.Empty;

            return (from d2 in Distributor.ObjectSet()
                    where d2.Id == fatherId
                    join d1 in Distributor.ObjectSet() on d2.ParentId equals d1.Id
                    select d1.Id).FirstOrDefault();
        }

        private List<AuditRet4Wx> GetAuditRet4WxsWhenPass(DistributionApply apply, Distributor l3Distributor, string microShopUrl, out int appDistributorCount)
        {
            if (apply.State == DistributeApplyStateEnum.Audit && (l3Distributor == null || string.IsNullOrEmpty(microShopUrl)))
                throw new Exception("通过审核时，参数distributor,l3MircoShopId不能为空！");

            var result = new List<AuditRet4Wx>();
            var appId = apply.RuleId;//Apply的RuleId是DistributionRule的Id，而它就是AppId

            switch (apply.State.EnumValue)
            {
                case DistributeApplyStateEnum.Audit:
                    var threeLevels = (from d3 in Distributor.ObjectSet()
                                       where d3.Id == l3Distributor.Id
                                       select d3
                ).Union(from d2 in Distributor.ObjectSet()
                        where d2.Id == l3Distributor.ParentId
                        select d2
                ).Union(from d2 in Distributor.ObjectSet()
                        where d2.Id == l3Distributor.ParentId
                        join d1 in Distributor.ObjectSet() on d2.ParentId equals d1.Id
                        select d1)
                        .ToList();

                    threeLevels.ForEach(x => result.Add(new AuditRet4Wx(x.UserId, x.UserName, x.UserCode, x.Level)));
                    var l3 = result.OrderByDescending(x => x.Level).FirstOrDefault();
                    if (l3 != null)
                        l3.SetMicroShopUrl(microShopUrl);
                    break;
                case DistributeApplyStateEnum.AuditRefuse:
                    result.Add(new AuditRet4Wx(apply.UserId, apply.UserName, apply.UserCode, apply.RefuseReason));
                    break;
            }

            appDistributorCount = Distributor.ObjectSet().Count(x => x.EsAppId == appId);
            return result;
        }

        private ResultDTO SendMessageToWx(Guid appId, List<PayWxMessageDTO> data4Wx)
        {
            var isSuccess = true;
            var errMsg = "";

            var msgSv = new BTPMessageSV();
            foreach (var msg in data4Wx)
            {
                msgSv.SendNewsMessageToWx(appId, msg);
            }
            return new ResultDTO { isSuccess = isSuccess, Message = errMsg, ResultCode = isSuccess ? 1 : 0 };
        }

        private ResultDTO SendMessageToApp(List<MobileMsgDTO> data4Mobile)
        {
            var isSuccess = true;
            var errMsg = "";

            var msgSv = new BTPMessageSV();
            foreach (var msg in data4Mobile)
            {
                msgSv.SendMobileMessage(msg);
            }
            return new ResultDTO { isSuccess = isSuccess, Message = errMsg, ResultCode = isSuccess ? 1 : 0 };
        }

        private Tuple<List<PayWxMessageDTO>, List<MobileMsgDTO>> GetData4SendMessage(Guid appId, List<AuditRet4Wx> distributors, int appDistributorCount = 0,
            bool isPass = false)
        {
            var l3RefuseTitle = "抱歉，由于{0}原因，您尚未能成为分销商~";
            var l1PassTitle = "恭喜您，您的下级{0}推荐的{1}成为{2}的第{3}位分销商";
            var l2PassTitle = "恭喜您，推荐的{0}成为{1}的第{2}位分销商";
            var l3PassTitle = "恭喜您，加入分销申请审核通过，您已经正式成为{0}的第{1}位分销商，赶快开启推广您的微小店吧~";
            var l3PassDescript = "点此打开您的微小店";
            var l3PassUrl = "http://btp/{0}";
            var applicationName = "正品O2O";

            if (isPass && appDistributorCount == 0)
                throw new Exception("AppDistributorCount不应该为0！");

            if (!distributors.Any())
                return null;

            var maxLevel = distributors.Max(x => x.Level);
            distributors.ForEach(x => x.Level -= (maxLevel - 3));

            var data4Wx = new List<PayWxMessageDTO>();
            var data4App = new List<MobileMsgDTO>();

            Func<Guid, Guid, string, MobileMsgDTO> createMobileMsgDto =
                (Guid applicationId, Guid userId, string title) =>
                    new MobileMsgDTO(applicationId + ":" + userId, applicationId, new List<Guid> { userId }, title,
                        MobileMsgSecondTypeEnum.DistributionAudit, MobileMsgCodeEnum.Common);

            var l3 = distributors.FirstOrDefault(x => x.Level == 3);
            if (l3 != null)
            {
                var openid = CBCSV.Instance.GetThirdBind(l3.UserId);
                string title;

                if (!isPass)
                {
                    title = string.Format(l3RefuseTitle, l3.RefuseReaon);
                    data4Wx.Add(CreateWxMessageDto(openid, title));
                    data4App.Add(createMobileMsgDto(appId, l3.UserId, title));
                }
                else
                {
                    title = string.Format(l3PassTitle, applicationName, appDistributorCount);
                    var descript = l3PassDescript;
                    var url = string.Format(l3PassUrl, l3.MicroShopUrl);

                    data4Wx.Add(CreateWxMessageDto(openid, title, descript, url));
                    data4App.Add(createMobileMsgDto(appId, l3.UserId, title));

                    var l2 = distributors.FirstOrDefault(x => x.Level == 2);
                    if (l2 != null)
                    {
                        openid = CBCSV.Instance.GetThirdBind(l2.UserId);
                        title = string.Format(l2PassTitle, l2.UserName, applicationName, appDistributorCount);

                        data4Wx.Add(CreateWxMessageDto(openid, title, descript, url));
                        data4App.Add(createMobileMsgDto(appId, l2.UserId, title));
                    }

                    var l1 = distributors.FirstOrDefault(x => x.Level == 1);
                    if (l1 != null)
                    {
                        openid = CBCSV.Instance.GetThirdBind(l1.UserId);
                        title = string.Format(l1PassTitle, l1.UserName, l1.UserName, applicationName,
                            appDistributorCount);

                        data4Wx.Add(CreateWxMessageDto(openid, title, descript, url));
                        data4App.Add(createMobileMsgDto(appId, l1.UserId, title));
                    }
                }
            }

            return new Tuple<List<PayWxMessageDTO>, List<MobileMsgDTO>>(data4Wx, data4App);
        }

        private PayWxMessageDTO CreateWxMessageDto(string openid, string title, string description = "", string url = "", string picurl = "")
        {
            var message = new PayWxMessageDTO()
            {
                touser = openid,
                msgtype = "news"
            };
            message.news.articles.Add(new WxArticles()
            {
                title = title,
                description = description,
                url = url,
                picurl = picurl
            });
            return message;
        }
    }

    class AuditRet4Wx
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public int Level { get; set; }
        public string MicroShopUrl { get; private set; }
        public string RefuseReaon { get; set; }

        /// <summary>
        /// 拒绝通过时
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="userCode"></param>
        /// <param name="refuseReaon"></param>
        public AuditRet4Wx(Guid userId, string userName, string userCode, string refuseReaon)
        {
            UserId = userId;
            UserName = userName;
            UserCode = userCode;
            Level = 1;
            MicroShopUrl = "";
            RefuseReaon = refuseReaon;
        }
        /// <summary>
        /// 审核通过时
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="userCode"></param>
        /// <param name="level"></param>
        public AuditRet4Wx(Guid userId, string userName, string userCode, int level)
        {
            UserId = userId;
            UserName = userName;
            UserCode = userCode;
            Level = level;
            RefuseReaon = "";
        }

        public void SetMicroShopUrl(string microShopUrl)
        {
            MicroShopUrl = microShopUrl;
        }
    }
}