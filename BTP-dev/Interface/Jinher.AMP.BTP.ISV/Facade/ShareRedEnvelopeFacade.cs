using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class ShareRedEnvelopeFacade : BaseFacade<IShareRedEnvelope>
    {

        /// <summary>
        /// 佣金结算
        /// </summary>
        public void SettleCommossion()
        {
            base.Do();
            this.Command.SettleCommossion();
        }

        /// <summary>
        /// 发送红包
        /// </summary>
        public void SendRedEnvelope()
        {
            base.Do();
            this.Command.SendRedEnvelope();
        }


        /// <summary>
        /// 处理过期红包
        /// </summary>
        public void HandleInValidRedEnvelope()
        {
            base.Do();
            this.Command.HandleInValidRedEnvelope();
        }

        /// <summary>
        /// 处理众筹过期红包
        /// </summary>
        public void HandleCfInValidRedEnvelope()
        {
            base.Do();
            this.Command.HandleCfInValidRedEnvelope();
        }

        /// <summary>
        /// 获取红包
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO GetRedEnvelope(Guid redEnvelopeId)
        {
            base.Do();
            return this.Command.GetRedEnvelope(redEnvelopeId);
        }

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="userRedEnvelopeId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawRedEnvelope(Guid userRedEnvelopeId)
        {
            base.Do();
            return this.Command.DrawRedEnvelope(userRedEnvelopeId);
        }


        /// <summary>
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyRedEnvelope(Guid userId, int type, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetMyRedEnvelope(userId, type, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyOrgRedEnvelope(Guid userId, int type, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetMyOrgRedEnvelope(userId, type, pageIndex, pageSize);
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UseRuleDescription(Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO ruleDescriptionDTO)
        {
            base.Do();
            return this.Command.UseRuleDescription(ruleDescriptionDTO);
        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO GetRuleDescription(Guid appId)
        {
            base.Do();
            return this.Command.GetRuleDescription(appId);
        }
        /// <summary>
        /// 发送红包
        /// </summary>
        public void SendCfRedEnvelope()
        {
            base.Do();
            this.Command.SendCfRedEnvelope();
        }

        /// <summary>
        /// 发送红包
        /// </summary>
        public ShareListResult GetShareList(int pageSize, int pageIndex)
        {
            base.Do();
            return this.Command.GetShareList(pageSize, pageIndex);
        }

    }
}
