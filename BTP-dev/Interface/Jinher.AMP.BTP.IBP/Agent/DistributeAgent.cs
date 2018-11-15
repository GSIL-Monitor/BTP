
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/1/31 18:19:21
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using CommodityDistributionDTO = Jinher.AMP.BTP.Deploy.CommodityDistributionDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class DistributeAgent : BaseBpAgent<IDistribute>, IDistribute
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ManageNumDTO ManageNc(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ManageNumDTO result;
             
            try
            {
                //调用代理方法
                result = base.Channel.ManageNc(manaDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ManageDTO ManageInfo(Jinher.AMP.BTP.Deploy.CustomDTO.ManageVM manaDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ManageDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ManageInfo(manaDTO);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public DistributRuleFullDTO GetDistributRuleFull(DistributionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributRuleFull(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public DistributRuleFullDTO GetDistributRuleFullDTOByAppId_Mobile(DistributionSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributRuleFullDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributRuleFullDTOByAppId_Mobile(search);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public ResultDTO ModifyDistributRuleFull(DistributRuleFullDTO distributRuleFullDto)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.ModifyDistributRuleFull(distributRuleFullDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public ResultDTO UpdateDistributionApplyState(Guid appId, Guid userId, DistributeApplyStateEnum distributeApplyStateEnum)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateDistributionApplyState(appId, userId, distributeApplyStateEnum);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public List<DistributionIdentityDTO> GetDistributorApplyIdentityVals(Guid applyId)
        {
            //定义返回值
            List<DistributionIdentityDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorApplyIdentityVals(applyId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public DistributionApplyDTO GetDistributionApply(Guid applyId)
        {
            //定义返回值
            DistributionApplyDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionApply(applyId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public DistributionApplyDTO GetDistributionApply(Guid ruleId, Guid userId)
        {
            //定义返回值
            DistributionApplyDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionApply(ruleId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 查询一条 分销商申请资料设置 记录
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public DistributionRuleDTO GetDistributionRule(Guid ruleId)
        {
            //定义返回值
            DistributionRuleDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionRule(ruleId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 查询 分销商申请资料设置 的身份设置
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public List<DistributionIdentitySetDTO> GetDistributionIdentitySets(Guid ruleId)
        {
            //定义返回值
            List<DistributionIdentitySetDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionIdentitySets(ruleId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 查询某个app的 分销商申请 列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<DistributionApplyResultDTO> GetDistributionApplyList(Guid appId, int pageSize, int pageIndex,
            out int rowCount)
        {
            //定义返回值
            List<DistributionApplyResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionApplyList(appId, pageSize, pageIndex, out rowCount);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
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
        public List<DistributionApplyResultDTO> GetDistributionApplyListByWhere(Guid appId, int pageSize, int pageIndex,
            out int rowCount, string userName, int state)
        {
            //定义返回值
            List<DistributionApplyResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionApplyListByWhere(appId, pageSize, pageIndex, out rowCount, userName, state);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 备注 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public ResultDTO RemarkDistributionApply(Guid id, string remarks)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RemarkDistributionApply(id, remarks);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

         /// <summary>
        /// 获取分销商申请的审批历史记录
        /// </summary>
        /// <param name="applyId"></param>
        public List<DistributionApplyAuditListDTO> GetApplyAuditList(Guid applyId)
        {
            //定义返回值
            List<DistributionApplyAuditListDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetApplyAuditList(applyId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 备注 分销商
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public ResultDTO RemarkDistributor(Guid id, string remarks)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.RemarkDistributor(id, remarks);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 审核 分销商申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isPass"></param>
        /// <param name="refuseReason"></param>
        /// <returns></returns>
        public ResultDTO<AuditDistributionResultDTO> AuditingDistributionApply(Guid id, bool isPass, string refuseReason)
        {
            //定义返回值
            ResultDTO<AuditDistributionResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.AuditingDistributionApply(id, isPass, refuseReason);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取一个分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributorDTO GetDistributorBy(Guid appId, Guid userId)
        {
            //定义返回值
            DistributorDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorBy(appId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        
        /// <summary>
        /// 更新微小店QrCode
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopQrCode(UpdateQrCodeRequestDTO dto)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateMicroshopQrCode(dto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        
        /// <summary>
        /// 新增分销商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributor(DistributorDTO dto)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddDistributor(dto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 添加申请成为分销商资料
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="userId"></param>
        /// <param name="ruleId"></param>
        /// <param name="distributApplyFullDto"></param>
        /// <returns></returns>
        public ResultDTO AddDistributionIdentityInfo(string userCode, string userId, string ruleId, DistributApplyFullDTO distributApplyFullDto)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddDistributionIdentityInfo(userCode, userId, ruleId, distributApplyFullDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取审核资料状态
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultDTO GetDistributeState(Guid appId, Guid userId)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributeState(appId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取分销商的所有身份设置
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public List<DistributorsHasIdentityResultDTO> GetDistributorsIdentitys(List<Guid> distributorList)
        {
            //定义返回值
            List<DistributorsHasIdentityResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorsIdentitys(distributorList);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 查询 分销商
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<DistributorDTO> GetDistributors(Guid appId, int pageSize,int pageIndex,out int rowCount)
        {
            //定义返回值
            List<DistributorDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributors(appId, pageSize, pageIndex, out rowCount);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 获取分销商们的备注信息
        /// </summary>
        /// <param name="distributorList"></param>
        /// <returns></returns>
        public Dictionary<Guid,string> GetDistributorsRemarks(List<Guid> distributorList)
        {
            //定义返回值
            Dictionary<Guid, string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributorsRemarks(distributorList);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        /// <summary>
        /// 获取分销商微小店信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DistributionMicroShopDTO GetDistributionMicroShop(Guid appId, Guid userId)
        {
            //定义返回值
            DistributionMicroShopDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDistributionMicroShop(appId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 修改微小店信息
        /// </summary>
        /// <param name="microshopDto"></param>
        /// <returns></returns>
        public ResultDTO UpdateDistributionMicroShop(MicroshopDTO microshopDto)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateDistributionMicroShop(microshopDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 微小店 下架商品
        /// </summary>
        /// <param name="microshopComDto"></param>
        /// <returns></returns>
        public ResultDTO SaveMicroshopCom(MicroshopComDTO microshopComDto)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveMicroshopCom(microshopComDto);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 微小店 上架商品
        /// </summary>
        /// <param name="microshopComId"></param>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public ResultDTO UpdateMicroshopCom(Guid microshopComId, Guid microshopId)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateMicroshopCom(microshopComId, microshopId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 根据Id获取微小店
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshop(Guid microshopId)
        {
            //定义返回值
            MicroshopDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMicroshop(microshopId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取微小店
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MicroshopDTO GetMicroshop(Guid appId, Guid userId)
        {
            //定义返回值
            MicroshopDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMicroshop(appId, userId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取某应用的所有加入微小店的商品Id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<CommodityDistributionDTO> GetAppAllMicroshopCommoditys(Guid appId)
        {
            //定义返回值
            List<CommodityDistributionDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppAllMicroshopCommoditys(appId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        /// <summary>
        /// 获取某微小店的所有下架商品Id
        /// </summary>
        /// <param name="microshopId"></param>
        /// <returns></returns>
        public List<Guid> GetMicroshopOfflineCommodityIds(Guid microshopId)
        {
            //定义返回值
            List<Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMicroshopOfflineCommodityIds(microshopId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
    }
}
