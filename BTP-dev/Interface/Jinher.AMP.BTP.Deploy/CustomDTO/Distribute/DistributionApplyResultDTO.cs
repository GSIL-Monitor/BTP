using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// 分销商身份设置值 
    /// </summary>
    [Serializable]
    [DataContract]
    public class DistributionApplyResultDTO:DistributionApplyDTO
    {
        /// <summary>
        /// 状态说明
        /// </summary>
        [DataMemberAttribute()]
        public string StateName { get; set; }

        /// <summary>
        /// 编码后的“备注”
        /// </summary>
        [DataMemberAttribute()]
        public string RemarksEncoded
        {
            get { return string.IsNullOrEmpty(Remarks) ? "" : Uri.EscapeDataString(Remarks); }
        }

        /// <summary>
        /// 编码后的“拒绝原因”
        /// </summary>
        [DataMemberAttribute()]
        public string RefuseReasonEncoded {
            get { return string.IsNullOrEmpty(RefuseReason) ? "" : Uri.EscapeDataString(RefuseReason); }
        }
        
        /// <summary>
        /// 身份设置值
        /// </summary>
        [DataMemberAttribute()]
        public List<DistributionIdentityDTO> IdentityVals { get; set; }
        
        /// <summary>
        /// 是否有身份设置值
        /// </summary>
        [DataMemberAttribute()]
        public bool HasIdentityVals {
            get { return IdentityVals.Count > 0; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DistributionApplyResultDTO()
        {
            
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="identityVals"></param>
        public DistributionApplyResultDTO(DistributionApplyDTO dto, List<DistributionIdentityDTO> identityVals)
        {
            Id = dto.Id;
            AuditTime = dto.AuditTime;
            ModifiedOn = dto.ModifiedOn;
            SubTime = dto.SubTime;
            AuditorId = dto.AuditorId;
            HasIdentity = dto.HasIdentity;
            PicturePath = dto.PicturePath;
            RefuseReason = dto.RefuseReason;
            Remarks = dto.Remarks;
            RuleId = dto.RuleId;
            State = dto.State;
            UserCode = dto.UserCode;
            UserId = dto.UserId;
            UserName = dto.UserName;

            foreach (var identityVal in identityVals)
            {
                if (string.IsNullOrWhiteSpace(identityVal.Value))
                    identityVal.Value = "无";
            }
            IdentityVals = identityVals;
        }
    }
}
