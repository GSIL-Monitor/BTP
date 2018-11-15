using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.AMP.SNS.ISV.Facade;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BaseApp.MessageCenter.Deploy.CustomDTO;
using System.Runtime.Serialization.Json;
using System.IO;
using Jinher.JAP.BaseApp.MessageCenter.ISV.Facade;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.Finance.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using System.Data;
using ReturnInfoDTO = Jinher.AMP.Finance.Deploy.CustomDTO.ReturnInfoDTO;


namespace Jinher.AMP.BTP.SV
{

    public partial class ShareRedEnvelopeSV : BaseSv, IShareRedEnvelope
    {
        private static Object getRedLock = new Object();
        /// <summary>
        /// 佣金结算
        /// </summary>
        public void SettleCommossionExt()
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                bool isContinue = true;
                int pageSize = 100;
                while (true)
                {
                    int shareFacadeErrCnt = 0;
                    var shares = (from data in ShareDividend.ObjectSet()
                                  join order in CommodityOrder.ObjectSet() on data.CommodityOrderId equals order.Id
                                  join data1 in OrderShareMess.ObjectSet() on data.CommodityOrderId equals data1.OrderId into oshares
                                  from oshare in oshares.DefaultIfEmpty()
                                  join commodity in Commodity.ObjectSet() on order.SrcTagId equals commodity.Id into coms
                                  from com in coms.DefaultIfEmpty()
                                  where data.State == 0 && data.SharerMoney > 0
                                  select new
                                  {
                                      ShareDividend = data,
                                      OrderSrcType = order.SrcType,
                                      OrderSrcTagId = order.SrcTagId,
                                      OrderCode = order.Code,
                                      ShareId = oshare.ShareId,
                                      CommodityName = com.Name,
                                      OrderSrcAppId = order.SrcAppId,
                                      ShareType = data.ShareType,
                                      OrderId = order.Id


                                  }).Take(pageSize).ToList();
                    if (!shares.Any())
                        break;

                    var shareOrderIds = shares.Where(x => x.OrderSrcType == 33).Select(x => x.OrderSrcTagId).ToList();

                    Dictionary<Guid, string> shareOrderCodes = CommodityOrder.ObjectSet()
                        .Where(x => shareOrderIds.Contains(x.Id))
                        .Select(x => new
                        {
                            Code = x.Code,
                            Id = x.Id
                        }).ToDictionary(x => x.Id, y => y.Code);

                    for (int i = 0; i < shares.Count; i++)
                    {
                        var share = shares[i];
                        Guid userId = Guid.Empty;
                        int roleType = 1;
                        string description = null;
                        long money = share.ShareDividend.SharerMoney;
                        if (share.ShareType == 1) //应用主分成
                        {
                            try
                            {
                                if (share.OrderSrcAppId.HasValue && share.OrderSrcAppId != Guid.Empty)
                                {
                                    var result = APPSV.Instance.GetAppLevelInfo(share.OrderSrcAppId.Value.ToString());
                                    if (result == null)
                                        continue;
                                    var appScore = result.LevelScore;
                                    if (result.OwnerId == Guid.Empty || appScore < CustomConfig.ShareOwner.AppMinCalcScore)
                                    {
                                        share.ShareDividend.State = 1;
                                        share.ShareDividend.EntityState = EntityState.Modified;
                                        continue;
                                    }

                                    if (result.OwnerTypeId == (int)App.Deploy.Enum.AppOwnerTypeEnum.Org)
                                    {
                                        roleType = 2;
                                    }
                                    userId = result.OwnerId;

                                    //发送消息
                                    if (result.OwnerTypeId == (int)App.Deploy.Enum.AppOwnerTypeEnum.Org)
                                    {
                                        List<Guid> OrgUserIds = Jinher.AMP.BTP.TPS.EBCSV.Instance.GetUserIdsByOrgIdAndCode(result.OwnerId, "ReceiveRed");

                                        CommodityOrderSV.SendMessageToPayment(OrgUserIds, "affirm", share.ShareDividend.SharerMoney.ToString(CultureInfo.InvariantCulture), null, 0);
                                    }
                                    else if (result.OwnerTypeId == (int)App.Deploy.Enum.AppOwnerTypeEnum.Personal)
                                    {
                                        List<Guid> userIds = new List<Guid> { result.OwnerId };
                                        CommodityOrderSV.SendMessageToPayment(userIds, "affirm", share.ShareDividend.SharerMoney.ToString(CultureInfo.InvariantCulture), null, 0);
                                    }

                                }
                                description = string.Format("正品o2o 商品销售分红获得金币{0}个", money);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("佣金结算服务异常:获取应用信息异常。", ex);
                                continue;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(share.ShareId) || share.ShareId.ToLower() == "null" || share.ShareId.ToLower() == "undefined")
                            {
                                share.ShareDividend.State = 1;
                                share.ShareDividend.EntityState = EntityState.Modified;
                                continue;
                            }
                            Jinher.AMP.SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid> shareServiceResult = new SNS.Deploy.CustomDTO.ReturnInfoDTO<Guid>();

                            try
                            {
                                shareServiceResult = Jinher.AMP.BTP.TPS.SNSSV.Instance.GetShareUserId(share.ShareId);
                            }
                            catch (Exception ex)
                            {
                                shareFacadeErrCnt++;
                                LogHelper.Error(string.Format("佣金结算服务异常。调用服务：\"根据分享Id获取分享人Id\" 错误,分享Id={0}", share.ShareId), ex);
                                if (i > 5 && i == shareFacadeErrCnt - 1)
                                {
                                    isContinue = false;
                                    break;
                                }
                                continue;
                            }

                            if (shareServiceResult.Code != "0")
                            {
                                LogHelper.Error(string.Format("佣金结算服务异常。调用服务：\"根据分享Id获取分享人Id\" 不成功,分享Id={0}，返回结果={1}", share.ShareId, JsonHelper.JsonSerializer(shareServiceResult)));
                                shareFacadeErrCnt++;
                                //调用服务都为异常，停止本次操作
                                if (i > 5 && i == shareFacadeErrCnt - 1)
                                {
                                    isContinue = false;
                                    break;
                                }
                                continue;
                            }
                            userId = shareServiceResult.Content;
                            switch (share.OrderSrcType)
                            {
                                case 33:
                                    //description = string.Format("分享{0}商品获得金币{1}个", share.CommodityName, money);

                                    if (share.OrderSrcTagId.HasValue && shareOrderCodes.ContainsKey(share.OrderSrcTagId.Value))
                                    {
                                        description = string.Format("分享{0}订单获得金币{1}个", shareOrderCodes[share.OrderSrcTagId.Value], money);
                                    }
                                    else
                                    {
                                        description = string.Format("分享订单获得金币{0}个", money);
                                    }
                                    break;
                                case 34:
                                    //description = string.Format("分享{0}订单获得金币{1}个", share.OrderCode, money);
                                    description = string.Format("分享{0}商品获得金币{1}个", share.CommodityName, money);
                                    break;

                            }
                        }


                        ShareDividendDetail model = new ShareDividendDetail
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            Money = money,
                            SettlementDate = share.ShareDividend.SettlementDate,
                            AppId = share.ShareDividend.AppId,
                            RoleType = roleType,
                            ShareDivedendId = share.ShareDividend.Id,
                            Description = description
                        };
                        model.EntityState = EntityState.Added;
                        contextSession.SaveObject(model);

                        share.ShareDividend.State = 1;
                        share.ShareDividend.EntityState = EntityState.Modified;

                    }
                    if (!isContinue)
                        break;
                    contextSession.SaveChanges();
                    LogHelper.Info(string.Format("佣金结算Job处理了ShareDividend表{0}条记录", shares.Count));
                    if (shares.Count < pageSize)
                    {
                        isContinue = false;
                        break;
                    }
                }

                LogHelper.Info("佣金结算Job处理成功");
            }
            catch (Exception ex)
            {
                LogHelper.Error("佣金结算服务异常。 ", ex);
            }

        }

        /// <summary>
        /// 发送红包
        /// </summary>
        public void SendRedEnvelopeExt()
        {
            int pageIndex = 0;
            int pageSize = 500;

            DateTime today = DateTime.Now.Date;
            ContextSession session = ContextFactory.CurrentThreadContext;
            bool isContinue = true;
            do
            {
                var userDividents = (from s in ShareDividendDetail.ObjectSet()
                                     where s.SettlementDate < today && s.State == 0
                                     group s by s.UserId into g
                                     select g
                        ).OrderBy(a => a.Key).Take(pageSize).Skip(pageIndex)
                        .ToDictionary(x => x.Key, y => y.ToList());


                foreach (Guid userId in userDividents.Keys)
                {
                    StringBuilder strDescription = new StringBuilder(50);
                    foreach (ShareDividendDetail userDivident in userDividents[userId])
                    {
                        userDivident.State = 1;
                        userDivident.EntityState = System.Data.EntityState.Modified;

                        strDescription.Append(userDivident.Description).Append(",");
                    }
                    strDescription.Remove(strDescription.Length - 1, 1);
                    var goldNum = userDividents[userId].Sum(x => x.Money);
                    UserRedEnvelope redEnvelope = UserRedEnvelope.CreateUserRedEnvelope();
                    redEnvelope.EntityState = System.Data.EntityState.Added;
                    redEnvelope.AppId = userDividents[userId][0].AppId;
                    redEnvelope.Content = CustomConfig.SaleShare.DividentContent;
                    redEnvelope.Description = strDescription.ToString();
                    double dueDateAdd = 24;
                    double.TryParse(CustomConfig.SaleShare.DividentDue, out dueDateAdd);
                    redEnvelope.DueDate = DateTime.Now.AddHours(dueDateAdd);
                    redEnvelope.GoldCount = goldNum;
                    redEnvelope.UserId = userId;
                    redEnvelope.RoleType = userDividents[userId][0].RoleType;

                    ShareRedMessageDTO contentCDTO = new ShareRedMessageDTO();
                    contentCDTO.message = redEnvelope.Content;
                    contentCDTO.userName = redEnvelope.Content;
                    contentCDTO.msgId = redEnvelope.Id;
                    contentCDTO.url = string.Format("{0}ShareRedEnvelope/ShareRedEnvelopesDetail?msgId={1}", CustomConfig.BtpDomain, redEnvelope.Id);

                    double dueMessDateAdd = 48;
                    double.TryParse(CustomConfig.SaleShare.DividentMessageDue, out dueMessDateAdd);

                    if (userDividents[userId][0].RoleType == 2)
                    {
                        try
                        {
                            List<Guid> userIds = Jinher.AMP.BTP.TPS.EBCSV.Instance.GetUserIdsByOrgIdAndCode(userId, "ReceiveRed");
                            int msgCount = 0;
                            foreach (Guid id in userIds)
                            {
                                SendMessage(redEnvelope.Id, redEnvelope.AppId, id, DateTime.Now.AddHours(dueMessDateAdd), contentCDTO);
                                //最多选择20个代领者
                                msgCount += 1;
                                if (msgCount > 20)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("发送红包Job服务异常:EBC.ISV.Facade.OrganizationQueryFacade。", ex);

                            continue;
                        }
                    }
                    else
                    {
                        SendMessage(redEnvelope.Id, redEnvelope.AppId, redEnvelope.UserId, DateTime.Now.AddHours(dueMessDateAdd), contentCDTO);
                    }

                    session.SaveObject(redEnvelope);
                }

                try
                {
                    session.SaveChanges();
                    if (userDividents.Count < pageSize)
                    {
                        isContinue = false;
                    }
                    LogHelper.Info(string.Format("发送红包Job处理了ShareDividendDetail表{0}条记录", userDividents.Count));
                }
                catch (Exception ex)
                {
                    LogHelper.Error("发送红包Job服务异常。", ex);
                    isContinue = false;
                }
            }
            while (isContinue);
        }

        /// <summary>
        /// 发送众筹红包
        /// </summary>
        public void SendCfRedEnvelopeExt()
        {
            int pageIndex = 0;
            int pageSize = 500;

            DateTime today = DateTime.Now.Date;
            ContextSession session = ContextFactory.CurrentThreadContext;
            bool isContinue = true;
            do
            {
                var userDividents = (from s in CfDividend.ObjectSet()
                                     where s.SettlementDate < today && s.State == 0
                                     group s by s.UserId into g
                                     select g
                        ).OrderBy(a => a.Key).Take(pageSize).Skip(pageIndex)
                        .ToDictionary(x => x.Key, y => y.ToList());


                foreach (Guid userId in userDividents.Keys)
                {
                    StringBuilder strDescription = new StringBuilder(50);
                    foreach (CfDividend userDivident in userDividents[userId])
                    {
                        userDivident.State = 1;
                        userDivident.EntityState = System.Data.EntityState.Modified;

                        strDescription.Append(userDivident.AppName);
                        strDescription.Append("  持股");
                        strDescription.Append(userDivident.ShareCount);
                        strDescription.Append("股  获得分红");
                        strDescription.Append(userDivident.Gold);
                        strDescription.Append("个金币").Append(",");
                    }
                    strDescription.Remove(strDescription.Length - 1, 1);
                    var goldNum = userDividents[userId].Sum(x => x.Gold);
                    UserRedEnvelope redEnvelope = UserRedEnvelope.CreateUserRedEnvelope();
                    redEnvelope.EntityState = System.Data.EntityState.Added;
                    redEnvelope.AppId = userDividents[userId][0].AppId;
                    redEnvelope.Content = CustomConfig.SaleShare.DividentContent;
                    redEnvelope.Description = strDescription.ToString();
                    double dueDateAdd = CustomConfig.CrowdfundingConfig.DividentDue;
                    redEnvelope.DueDate = DateTime.Now.AddHours(dueDateAdd);
                    redEnvelope.GoldCount = goldNum;
                    redEnvelope.UserId = userId;
                    redEnvelope.RedEnvelopeType = 1;

                    session.SaveObject(redEnvelope);

                    ShareRedMessageDTO contentCDTO = new ShareRedMessageDTO();
                    contentCDTO.message = redEnvelope.Content;
                    contentCDTO.userName = redEnvelope.Content;
                    contentCDTO.msgId = redEnvelope.Id;
                    contentCDTO.url = string.Format("{0}ShareRedEnvelope/ShareRedEnvelopesDetail?msgId={1}", CustomConfig.BtpDomain, redEnvelope.Id);

                    double dueMessDateAdd = CustomConfig.CrowdfundingConfig.DividentMessageDue;
                    SendMessage(redEnvelope.Id, redEnvelope.AppId, redEnvelope.UserId, DateTime.Now.AddHours(dueMessDateAdd), contentCDTO);
                }

                try
                {
                    session.SaveChanges();
                    if (userDividents.Count < pageSize)
                    {
                        isContinue = false;
                    }
                    LogHelper.Info(string.Format("发送众筹红包Job处理了CfDividend表{0}条记录", userDividents.Count));
                }
                catch (Exception ex)
                {
                    LogHelper.Error("发送众筹红包Job服务异常。", ex);
                    isContinue = false;
                }
            }
            while (isContinue);
        }

        /// <summary>
        /// 处理过期红包
        /// </summary>
        public void HandleInValidRedEnvelopeExt()
        {
            DateTime now = DateTime.Now;
            int pageIndex = 0;
            int pageSize = 500;


            ContextSession session = ContextFactory.CurrentThreadContext;
            bool isContinue = true;
            do
            {
                var userRedEnvelopes = (from u in UserRedEnvelope.ObjectSet()
                                        where u.DueDate < now && u.State == 0 && u.RedEnvelopeType == 0
                                        select u)
                                        .OrderBy(a => a.DueDate).Take(pageSize).Skip(pageIndex).ToList();

                if (!userRedEnvelopes.Any())
                    return;
                if (userRedEnvelopes.Count < pageSize)
                    isContinue = false;

                var totalGoldNum = userRedEnvelopes.Sum(x => x.GoldCount);

                MultiPayeeTradeByPasswordArg arg = new MultiPayeeTradeByPasswordArg();
                arg.PayeeComments = new List<string>();
                arg.PayorComments = new List<string>();

                arg.AppId = userRedEnvelopes[0].AppId;
                arg.PayorId = CustomConfig.ShareGoldAccout.BTPShareGoldAccount;
                arg.UsageId = CustomConfig.ShareGoldAccout.BTPGlodUsageId;
                arg.Golds = new List<long>() { totalGoldNum };
                arg.Payees = new List<Tuple<Guid, bool>>();
                arg.Payees.Add(new Tuple<Guid, bool>(CustomConfig.ShareGoldAccout.JHShareGoldAccount, true));
                arg.BizSystem = "BTP";
                arg.BizId = userRedEnvelopes[0].Id;
                arg.BizType = "BTP_InvalidRed_Auto";

                arg.PayorComments.Add("电商过期红包支出");
                arg.PayeeComments.Add("电商过期红包收益");
                arg.PayorPassword = CustomConfig.ShareGoldAccout.BTPShareAccountPwd;

                ReturnInfoDTO gReturnDTO = new ReturnInfoDTO();
                try
                {
                    gReturnDTO = Jinher.AMP.BTP.TPS.Finance.Instance.MultiPayeeTradeByPassword(arg);

                    if (gReturnDTO.IsSuccess)
                    {
                        foreach (var userRedEnvelope in userRedEnvelopes)
                        {
                            userRedEnvelope.State = 2;
                            userRedEnvelope.EntityState = System.Data.EntityState.Modified;
                        }
                        session.SaveChanges();
                        LogHelper.Info(string.Format("电商过期红包Job处理了UserRedEnvelope表{0}条记录", userRedEnvelopes.Count));
                    }
                    else
                    {
                        JAP.Common.Loging.LogHelper.Error("电商过期红包MultiPayeeTrade fail：" + gReturnDTO.Code + ":" + gReturnDTO.Info);
                        isContinue = false;
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error("电商过期红包Job服务异常。", ex);
                    isContinue = false;
                }

            }
            while (isContinue);
        }

        /// <summary>
        /// 处理众筹过期红包
        /// </summary>
        public void HandleCfInValidRedEnvelopeExt()
        {
            DateTime now = DateTime.Now;
            int pageIndex = 0;
            int pageSize = 500;


            ContextSession session = ContextFactory.CurrentThreadContext;
            bool isContinue = true;
            do
            {
                var userRedEnvelopes = (from u in UserRedEnvelope.ObjectSet()
                                        where u.DueDate < now && u.State == 0 && u.RedEnvelopeType == 1
                                        select u)
                                        .OrderBy(a => a.DueDate).Take(pageSize).Skip(pageIndex).ToList();
                if (!userRedEnvelopes.Any())
                    break;
                if (userRedEnvelopes.Count < pageSize)
                    isContinue = false;


                var totalGoldNum = userRedEnvelopes.Sum(x => x.GoldCount);
                MultiPayeeTradeByPasswordArg arg = new MultiPayeeTradeByPasswordArg();
                arg.PayeeComments = new List<string>();
                arg.PayorComments = new List<string>();

                arg.AppId = userRedEnvelopes[0].AppId;
                arg.PayorId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount;
                arg.UsageId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingUsageId;
                arg.Golds = new List<long>() { totalGoldNum };
                arg.Payees = new List<Tuple<Guid, bool>>();
                arg.Payees.Add(new Tuple<Guid, bool>(CustomConfig.CrowdfundingAccount.JhCrowdfundingGoldAccount, true));
                arg.BizSystem = "BTP";
                arg.BizId = userRedEnvelopes[0].Id;
                arg.BizType = "BTP_Cf_InvalidRed_Auto";

                arg.PayorComments.Add("众筹活动电商过期红包支出");
                arg.PayeeComments.Add("众筹活动电商过期红包收益");
                arg.PayorPassword = CustomConfig.CrowdfundingAccount.BTPCrowdfundingPwd;

                ReturnInfoDTO gReturnDTO = new ReturnInfoDTO();
                try
                {
                    gReturnDTO = Jinher.AMP.BTP.TPS.Finance.Instance.MultiPayeeTradeByPassword(arg);

                    if (gReturnDTO.IsSuccess)
                    {
                        foreach (var userRedEnvelope in userRedEnvelopes)
                        {
                            userRedEnvelope.State = 2;
                            userRedEnvelope.EntityState = System.Data.EntityState.Modified;
                        }
                        session.SaveChanges();
                        LogHelper.Info(string.Format("众筹活动电商过期红包Job处理了UserRedEnvelope表{0}条记录", userRedEnvelopes.Count));
                    }
                    else
                    {
                        JAP.Common.Loging.LogHelper.Error("众筹活动电商过期红包MultiPayeeTrade fail：" + gReturnDTO.Code + ":" + gReturnDTO.Info);
                        isContinue = false;
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error("众筹活动电商过期红包Job服务异常。", ex);
                    isContinue = false;
                }

            }
            while (isContinue);
        }

        void SendMessage(Guid msgId, Guid appId, Guid userId, DateTime? endTime, ShareRedMessageDTO content)
        {
            //给邀请人发送系统消息
            //调用消息中心发消息
            //定义消息内容 
            MobileMessageDTO messageDTO = new MobileMessageDTO();

            messageDTO.MessageType = JAP.BaseApp.MessageCenter.Deploy.Enum.MessageType.BUSI_MSG;

            messageDTO.AppId = appId.ToString().ToLower();
            messageDTO.BasicContentDTO = new BasicContentDTO();
            messageDTO.BasicContentDTO.Code = "AdDividend";//"BTPShareRedEnvelope";

            messageDTO.ProductType = JAP.BaseApp.MessageCenter.Deploy.Enum.ProductType.BTP;
            messageDTO.ProductSecondType = 3;//新红包

            DataContractJsonSerializer json = new DataContractJsonSerializer(content.GetType());
            string szJson = "";
            //序列化
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, content);
                szJson = Encoding.UTF8.GetString(stream.ToArray());
            }

            messageDTO.BasicContentDTO.Content = szJson.Replace("\"", "\\\"");
            //messageDTO.Content = szJson.Replace("\"", "\\\""); ;

            if (endTime != null)
                messageDTO.EndTime = (DateTime)endTime;
            //接收人邀请人Id
            string strRelation = "[";

            strRelation += "\"" + userId.ToString() + "\",";

            strRelation = strRelation.Substring(0, strRelation.Length - 1);
            strRelation += "]";

            messageDTO.UserIds = strRelation;
            try
            {
                Jinher.AMP.BTP.TPS.MessageCenter.Instance.AddMessage(messageDTO);
                JAP.Common.Loging.LogHelper.Info("红包消息内容：" + userId.ToString() + ":" + ":" + messageDTO.AppId.ToString() + ":" + messageDTO.BasicContentDTO.Content);
            }
            catch (Exception ex)
            {
                JAP.Common.Loging.LogHelper.Error("Error_LogKey:RedEnvelopesProcessor.SendMessage:BaseApp.MessageCenter.ISV.Facade.PublishMobileMessageFacade.AddMessage", ex);
            }
        }

        /// <summary>
        /// 获取红包
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO GetRedEnvelopeExt(Guid redEnvelopeId)
        {

            try
            {
                Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO userRedEnvelopeDTO = new UserRedEnvelopeDTO();

                var query = UserRedEnvelope.ObjectSet().Where(q => q.Id == redEnvelopeId).FirstOrDefault();

                if (query != null)
                {

                    userRedEnvelopeDTO.Id = query.Id;
                    userRedEnvelopeDTO.AppId = query.AppId;
                    userRedEnvelopeDTO.Content = query.Content;
                    userRedEnvelopeDTO.Description = query.Description;
                    userRedEnvelopeDTO.DueDate = query.DueDate;
                    userRedEnvelopeDTO.GoldCount = query.GoldCount;
                    userRedEnvelopeDTO.SubTime = query.SubTime;
                    userRedEnvelopeDTO.ModifiedOn = query.ModifiedOn;
                    userRedEnvelopeDTO.State = query.State;
                    userRedEnvelopeDTO.UserId = query.UserId;
                    userRedEnvelopeDTO.RedEnvelopeType = query.RedEnvelopeType;

                }
                return userRedEnvelopeDTO;
            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("获取我的红包UserRedEnvelopeSV-GetRedEnvelopeExt,参数redEnvelopeId:{0}", redEnvelopeId), ex);
                return null;
            }

        }

        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="userRedEnvelopeId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DrawRedEnvelopeExt(Guid userRedEnvelopeId)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new ResultDTO();

            UserRedEnvelope query = null;

            lock (getRedLock)
            {
                //用户分红表
                query = UserRedEnvelope.ObjectSet().Where(q => q.Id == userRedEnvelopeId).FirstOrDefault();

                if (query == null)
                {
                    result.ResultCode = 1;
                    result.Message = "没有红包可领";
                    return result;
                }
                else if (query.State == 0 && DateTime.Now > query.DueDate)
                {
                    result.ResultCode = 1;
                    result.Message = "红包已过期";
                    return result;
                }
                else if (query.State == 1)
                {
                    result.ResultCode = 1;
                    result.Message = "红包已领";
                    return result;
                }
                int num = UpdateRedState(query, 1);
                if (num > 0)
                {
                    result.ResultCode = 0;
                    result.Message = "领取红包成功";

                }
                else
                {
                    result.ResultCode = 1;
                    result.Message = "领取红包失败";
                    return result;
                }
            }

            MultiPayeeTradeByPasswordArg arg = new MultiPayeeTradeByPasswordArg();
            ReturnInfoDTO gReturnDTO = new ReturnInfoDTO();
            arg.PayeeComments = new List<string>();
            arg.PayorComments = new List<string>();
            arg.AppId = query.AppId;
            arg.Payees = new List<Tuple<Guid, bool>>();
            arg.Payees.Add(new Tuple<Guid, bool>(query.UserId, true));
            arg.BizSystem = "BTP";
            arg.BizId = query.Id;
            arg.Golds = new List<long> { query.GoldCount };
            arg.PayorPassword = CustomConfig.ShareGoldAccout.BTPShareAccountPwd;
            //众销
            if (query.RedEnvelopeType == 0)
            {

                arg.PayorId = CustomConfig.ShareGoldAccout.BTPShareGoldAccount;
                arg.UsageId = CustomConfig.ShareGoldAccout.BTPGlodUsageId;
                arg.BizType = "BTP_ShareDividend_Auto";
                arg.PayorComments.Add("电商分享红包支出");
                arg.PayeeComments.Add("电商分享红包收益");

            }
            else
            {
                //众筹
                arg.PayorId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingAcount;
                arg.UsageId = CustomConfig.CrowdfundingAccount.BTPCrowdfundingUsageId;
                arg.BizType = "BTP_CrowdfundingDividend";
                arg.PayorComments.Add("电商众筹分红支出");
                arg.PayeeComments.Add("电商众筹分红收益");

                //计算用户已得分红
                var uc = UserCrowdfunding.ObjectSet().Where(q => q.AppId == query.AppId && q.UserId == query.UserId).FirstOrDefault();

                uc.RealGetDividend += query.GoldCount;
                uc.EntityState = EntityState.Modified;
                contextSession.SaveObject(uc);

            }
            try
            {
                gReturnDTO = Jinher.AMP.BTP.TPS.Finance.Instance.MultiPayeeTradeByPassword(arg);
            }
            catch (Exception ex)
            {
                int num = UpdateRedState(query, 0);
                LogHelper.Error(string.Format("获取我的红包UserRedEnvelopeSV-DrawRedEnvelopeExt-MultiPayeeTrade,参数redEnvelopeId:{0},红包状态恢复{1}", userRedEnvelopeId, (num > 0 ? "成功" : "失败")), ex);
                result.ResultCode = 1;
                result.Message = "调用金币接口失败";
                return result;
            }

            if (gReturnDTO == null || !gReturnDTO.IsSuccess)
            {
                int num = UpdateRedState(query, 0);
                result.ResultCode = 1;
                result.Message = "分享红包支付失败";

                LogHelper.Error(string.Format("调用金币MultiPayeeTradeByPassword方法失败,code:{0},错误消息:{1},参数redEnvelopeId:{2},红包状态恢复{3}", gReturnDTO.Code, gReturnDTO.Info, userRedEnvelopeId, (num > 0 ? "成功" : "失败")));

                return result;

            }
            return result;
        }

        private static int UpdateRedState(UserRedEnvelope query, int state)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            query.State = state;
            query.EntityState = EntityState.Modified;
            int num = contextSession.SaveChanges();
            return num;
        }


        /// <summary>
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyRedEnvelopeExt(Guid userId, int type, int pageIndex, int pageSize)
        {
            pageSize = pageSize == 0 ? 10 : pageSize;
            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> userRedEnvelopeDTOList = new List<UserRedEnvelopeDTO>();
            try
            {

                var qaueryList = UserRedEnvelope.ObjectSet().Where(q => q.UserId == userId && q.RedEnvelopeType == type).OrderByDescending(q => q.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();


                foreach (var item in qaueryList)
                {
                    Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO userRedEnvelopeDTO = new UserRedEnvelopeDTO();

                    userRedEnvelopeDTO.Id = item.Id;
                    userRedEnvelopeDTO.AppId = item.AppId;
                    userRedEnvelopeDTO.Content = item.Content;
                    userRedEnvelopeDTO.Description = item.Description;
                    userRedEnvelopeDTO.DueDate = item.DueDate;
                    userRedEnvelopeDTO.GoldCount = item.GoldCount;
                    userRedEnvelopeDTO.SubTime = item.SubTime;
                    userRedEnvelopeDTO.ModifiedOn = item.ModifiedOn;
                    userRedEnvelopeDTO.State = item.State;
                    userRedEnvelopeDTO.UserId = item.UserId;
                    userRedEnvelopeDTO.RedEnvelopeType = item.RedEnvelopeType;
                    userRedEnvelopeDTOList.Add(userRedEnvelopeDTO);
                }

                return userRedEnvelopeDTOList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取我的红包UserRedEnvelopeSV-GetMyRedEnvelopeExt,userId：{0}，type：{1}，pageIndex：{2}，pageSize：{3}，", userId, type, pageIndex, pageSize), ex);
                return null;
            }


        }

        /// <summary>
        /// 获取我的红包
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> GetMyOrgRedEnvelopeExt(Guid userId, int type, int pageIndex, int pageSize)
        {
            pageSize = pageSize == 0 ? 10 : pageSize;
            List<Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO> userRedEnvelopeDTOList = new List<UserRedEnvelopeDTO>();
            try
            {
                List<Guid> orgIdList = Jinher.AMP.BTP.TPS.EBCSV.Instance.GetOrgIdsByUserIdAndCode(userId, "ReceiveRed");

                var qaueryList = UserRedEnvelope.ObjectSet().Where(q => orgIdList.Contains(q.UserId) && q.RedEnvelopeType == type).OrderByDescending(q => q.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();


                foreach (var item in qaueryList)
                {
                    Jinher.AMP.BTP.Deploy.CustomDTO.UserRedEnvelopeDTO userRedEnvelopeDTO = new UserRedEnvelopeDTO();

                    userRedEnvelopeDTO.Id = item.Id;
                    userRedEnvelopeDTO.AppId = item.AppId;
                    userRedEnvelopeDTO.Content = item.Content;
                    userRedEnvelopeDTO.Description = item.Description;
                    userRedEnvelopeDTO.DueDate = item.DueDate;
                    userRedEnvelopeDTO.GoldCount = item.GoldCount;
                    userRedEnvelopeDTO.SubTime = item.SubTime;
                    userRedEnvelopeDTO.ModifiedOn = item.ModifiedOn;
                    userRedEnvelopeDTO.State = item.State;
                    userRedEnvelopeDTO.UserId = item.UserId;
                    userRedEnvelopeDTO.RedEnvelopeType = item.RedEnvelopeType;
                    userRedEnvelopeDTOList.Add(userRedEnvelopeDTO);
                }

                return userRedEnvelopeDTOList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("获取我的组织红包UserRedEnvelopeSV-GetMyRedEnvelopeExt,userId：{0}，type：{1}，pageIndex：{2}，pageSize：{3}，", userId, type, pageIndex, pageSize), ex);
                return null;
            }


        }
        /// <summary>
        /// 操作规则说明
        /// </summary>
        /// <param name="ruleDescriptionDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UseRuleDescriptionExt(Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO ruleDescriptionDTO)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result = new ResultDTO();
            try
            {

                ContextSession contextSession = ContextFactory.CurrentThreadContext;


                var query = RuleDescription.ObjectSet().Where(q => q.Id == ruleDescriptionDTO.Id).FirstOrDefault();
                if (query == null)
                {
                    RuleDescription rd = new RuleDescription();
                    rd.Id = ruleDescriptionDTO.Id;
                    rd.Description = ruleDescriptionDTO.Description;
                    rd.AppId = ruleDescriptionDTO.appId;
                    rd.EntityState = EntityState.Added;
                    contextSession.SaveObject(rd);


                }
                else
                {
                    query.Description = ruleDescriptionDTO.Description;
                    query.EntityState = EntityState.Modified;
                    contextSession.SaveObject(query);
                }
                int num = contextSession.SaveChanges();
                if (num > 0)
                {
                    result.ResultCode = 0;
                    result.Message = "成功";
                    return result;
                }
                else
                {
                    result.ResultCode = 1;
                    result.Message = "失败";
                    return result;
                }

            }
            catch (Exception ex)
            {

                LogHelper.Error(string.Format("操作规则说明UserRedEnvelopeSV-UseRuleDescriptionExt,参数ruleDescriptionDTO:{0}", JsonHelper.JsonSerializer(ruleDescriptionDTO)), ex);
                result.ResultCode = 1;
                result.Message = ex.Message;
                return result;
            }

        }


        public Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO GetRuleDescriptionExt(Guid appId)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.RuleDescriptionDTO ruleDescriptionDTO = new RuleDescriptionDTO();

            var query = RuleDescription.ObjectSet().FirstOrDefault();
            if (appId != Guid.Empty)
            {
                query = RuleDescription.ObjectSet().FirstOrDefault(t => t.AppId == appId);
            }
            if (query != null)
            {
                ruleDescriptionDTO.Id = query.Id;
                ruleDescriptionDTO.Description = query.Description;
                ruleDescriptionDTO.appId = query.AppId == null ? Guid.Empty : (Guid)query.AppId;
            }
            return ruleDescriptionDTO;

        }
        /// <summary>
        /// 获取众销明细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ShareListResult GetShareListExt(int pageSize, int pageIndex)
        {
            ShareListResult result = new ShareListResult();
            int start = (pageIndex - 1) * pageSize;

            result.Count = ShareDividendDetail.ObjectSet().Count();

            if (result.Count > 0)
            {
                result.ShareItems = new List<ShareItemDTO>();

                var orders = (from order in CommodityOrder.ObjectSet()
                              join dividend in ShareDividend.ObjectSet() on order.Id equals dividend.CommodityOrderId
                              join dividendDetail in ShareDividendDetail.ObjectSet() on dividend.Id equals
                                  dividendDetail.ShareDivedendId
                              where (order.SrcType == 33 || order.SrcType == 34) && order.State == 3
                              orderby order.SubTime
                              select new ShareItemDTO
                              {
                                  ShareUserId = dividendDetail.UserId,
                                  OrderCode = order.Code,
                                  DividendGold = dividendDetail.Money
                              }).Skip(start).Take(pageSize).ToList();
                if (orders.Any())
                {
                    var userIds = orders.Select(c => c.ShareUserId).Distinct().ToList();

                    List<UserNameAccountsDTO> userNamelist =
                    CBCSV.Instance.GetUserNameAccountsByIds(userIds) ?? new List<UserNameAccountsDTO>();
                    foreach (var shareItemDto in orders)
                    {
                        string userCode = "--";
                        string userName = "--";

                        var userInfo = userNamelist.FirstOrDefault(c => c.userId == shareItemDto.ShareUserId);
                        if (userInfo != null)
                        {
                            userName = userInfo.userName;
                            if (userInfo.Accounts != null && userInfo.Accounts.Any())
                            {
                                var account = userInfo.Accounts.FirstOrDefault(c => !string.IsNullOrEmpty(c.Account));
                                if (account != null)
                                {
                                    userCode = account.Account;
                                }
                            }
                        }
                        result.ShareItems.Add(new ShareItemDTO
                        {

                            DividendGold = shareItemDto.DividendGold,
                            OrderCode = shareItemDto.OrderCode,
                            ShareUserCode = userCode,
                            ShareUserName = userName,
                            ShareDate = DateTime.Now,
                            ShareUserId = shareItemDto.ShareUserId
                        });
                    }

                    result.SumUserCount = ShareDividendDetail.ObjectSet().Select(c => c.UserId).Distinct().Count();
                    result.SumDividendGold = ShareDividendDetail.ObjectSet().Sum(c => c.Money);
                }
            }
            return result;
        }
    }


}
