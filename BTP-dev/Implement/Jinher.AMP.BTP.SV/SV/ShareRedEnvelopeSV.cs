using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;
using System.Diagnostics;

namespace Jinher.AMP.BTP.SV
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class ShareRedEnvelopeSV : BaseSv, IShareRedEnvelope
    {

        /// <summary>
        /// 佣金结算
        /// </summary>
        public void SettleCommossion()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.SettleCommossionExt();
            timer.Stop();
            LogHelper.Info(string.Format("ShareRedEnvelopeSV.SettleCommossion：耗时：{0}。", timer.ElapsedMilliseconds));
        }

        /// <summary>
        /// 发送红包
        /// </summary>
        public void SendRedEnvelope()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.SendRedEnvelopeExt();
            timer.Stop();
            LogHelper.Info(string.Format("ShareRedEnvelopeSV.SendRedEnvelope：耗时：{0}。", timer.ElapsedMilliseconds));
        }


        /// <summary>
        /// 处理过期红包
        /// </summary>
        public void HandleInValidRedEnvelope()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.HandleInValidRedEnvelopeExt();
            timer.Stop();
            LogHelper.Info(string.Format("ShareRedEnvelopeSV.HandleInValidRedEnvelope：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        /// <summary>
        /// 处理众筹过期红包
        /// </summary>
        public void HandleCfInValidRedEnvelope()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            HandleCfInValidRedEnvelopeExt();
            timer.Stop();
            LogHelper.Info(string.Format("ShareRedEnvelopeSV.HandleCfInValidRedEnvelope：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        /// <summary>
        /// 获取红包
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO GetRedEnvelope(Guid redEnvelopeId)
        {
            base.Do(false);
            return this.GetRedEnvelopeExt(redEnvelopeId);
        }

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="userRedEnvelopeId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawRedEnvelope(Guid userRedEnvelopeId)
        {
            base.Do(false);
            return this.DrawRedEnvelopeExt(userRedEnvelopeId);
        }



        /// <summary>
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyRedEnvelope(Guid userId, int type, int pageIndex, int pageSize)
        {
            base.Do(false);
            return this.GetMyRedEnvelopeExt(userId, type, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取我的组织红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyOrgRedEnvelope(Guid userId, int type, int pageIndex, int pageSize)
        {
            base.Do(false);
            return this.GetMyOrgRedEnvelopeExt(userId, type, pageIndex, pageSize);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UseRuleDescription(Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO ruleDescriptionDTO)
        {
            base.Do(false);
            return this.UseRuleDescriptionExt(ruleDescriptionDTO);
        }

        public Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO GetRuleDescription(Guid appId)
        {
            base.Do(false);
            return this.GetRuleDescriptionExt(appId);
        }

        /// <summary>
        /// 发送众筹红包
        /// </summary>
        public void SendCfRedEnvelope()
        {
            base.Do(false);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            this.SendCfRedEnvelopeExt();
            timer.Stop();
            LogHelper.Info(string.Format("ShareRedEnvelopeSV.SendCfRedEnvelope：耗时：{0}。", timer.ElapsedMilliseconds));
        }
        /// <summary>
        /// 获取众销明细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ShareListResult GetShareList(int pageSize, int pageIndex)
        {
            base.Do(false);
            return this.GetShareListExt(pageSize, pageIndex);
        }

    }
}
