using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.Deploy.CustomDTO
{
    /// <summary>
    /// BTP的App消息类
    /// </summary>
    [Serializable()]
    [DataContract]
    public class MobileMsgDTO
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [DataMemberAttribute()]
        public string MsgId { get; set; }
        /// <summary>
        /// App ID
        /// </summary>
        [DataMemberAttribute()]
        public Guid AppId { get; set; }
        /// <summary>
        /// 接收消息的UserId列表
        /// </summary>
        [DataMemberAttribute()]
        public List<Guid> ToUsers { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [DataMemberAttribute()]
        public string MsgContent { get; set; }
        /// <summary>
        /// Product Second Type
        /// </summary>
        [DataMemberAttribute()]
        public MobileMsgSecondTypeEnum SecondType { get; set; }
        /// <summary>
        /// ContentCode
        /// </summary>
        [DataMemberAttribute()]
        public MobileMsgCodeEnum ContentCode { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        [DataMemberAttribute()]
        public MobileMsgType MobileMsgType { get; set; }

        public MobileMsgDTO()
        {
            
        }

        public MobileMsgDTO(string msgId,
            Guid appId,
            List<Guid> toUsers,
            string msgContent,
            MobileMsgSecondTypeEnum secondType,
            MobileMsgCodeEnum contentCode )
            : this(msgId, appId, toUsers, msgContent, secondType, contentCode, MobileMsgType.BUSI_MSG)
        {
            
        }

        public MobileMsgDTO(string msgId,
            Guid appId,
            List<Guid> toUsers,
            string msgContent,
            MobileMsgSecondTypeEnum secondType,
            MobileMsgCodeEnum contentCode,
            MobileMsgType mobileMsgType)
        {
            MsgId = msgId;
            AppId = appId;
            ToUsers = toUsers;
            MsgContent = msgContent;
            SecondType = secondType;
            ContentCode = contentCode;
            MobileMsgType = mobileMsgType;
        }
    }

    // 摘要:
    //     消息类型
    /// <summary>
    /// 消息类型
    /// 与Jinher.JAP.BaseApp.MessageCenter.Deploy.Enum保持一致
    /// 避免了BTP.Deploy引用程序集 Jinher.JAP.BaseApp.MessageCenter.Deploy.dll
    /// </summary>
    public enum MobileMsgType
    {
        // 摘要:
        //     业务消息，手机端业务系统处理的消息
        BUSI_MSG = 0,
        //
        // 摘要:
        //     系统消息，手机端公共组件处理的消息
        SYS_MSG = 1,
        //
        // 摘要:
        //     升级消息，手机端公共组件处理的消息
        UP_MSG = 2,
        //
        // 摘要:
        //     用户消息，手机端公共组件处理的消息
        USER_MSG = 3,
        //
        // 摘要:
        //     广告消息，手机端公共组件处理的消息
        APPAD_MSG = 4,
        SQUARE_MSG = 5,
        PrecisionMarketing_Msg = 6,
        //
        // 摘要:
        //     在线状态通知消息，已弃用
        [Obsolete("已弃用")]
        ONLINESTAS_MSG = 7,
        RedDot_MSG = 8,
    }
}
