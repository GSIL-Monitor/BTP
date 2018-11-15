using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.SNS.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.TPS
{
    [BTPAopLog]
    public class SpreadSV : ContextBoundObject
    {
        /// <summary>
        /// 单例
        /// </summary>
        public static SpreadSV Instance = new SpreadSV();
        private SpreadSV()
        {

        }
        /// <summary>
        /// 更新推广主绑定关系，如果推广主之前与其他账户绑定，解绑重新绑定自身
        /// </summary>
        /// <param name="spreadId">推广主Id</param>
        /// <param name="spreadCode">推广码</param>
        /// <param name="spreadType">推广类型 0：推广主，1：电商馆，2：总代，3企业</param>
        public void UpdateRalationUserSpread(Guid spreadId, Guid spreadCode, int spreadType)
        {
            ContextSession contextSession = ContextFactory.CurrentThreadContext;

            var createOrgUserId = EBCSV.GetOrgCreateUser(spreadId);

            var userSpreader = UserSpreader.ObjectSet().FirstOrDefault(c => c.UserId == createOrgUserId);
            if (userSpreader != null)
            {
                var createUsers = EBCSV.GetMyCreateAccountList(createOrgUserId);
                if (createUsers != null && createUsers.Contains(userSpreader.SpreaderId))
                    return;
                contextSession.Delete(userSpreader);
            }
            UserSpreader uSpreaderNew = UserSpreader.CreateUserSpreader();
            uSpreaderNew.UserId = createOrgUserId;
            uSpreaderNew.SpreaderId = spreadId;
            uSpreaderNew.SpreadCode = spreadCode;
            uSpreaderNew.IsDel = false;
            uSpreaderNew.CreateOrderId = new Guid("00000000-0000-0000-0000-000000000000");
            uSpreaderNew.SubTime = DateTime.Now;
            uSpreaderNew.ModifiedOn = DateTime.Now;
            uSpreaderNew.WxOpenId = "";
            contextSession.SaveObject(uSpreaderNew);
        }

        /// <summary>
        /// 保存spreadinfo
        /// </summary>
        /// <param name="spreadInfo"></param>
        /// <param name="isDeleteOld">是否删除同类型的旧数据</param>
        public void BuildSaveSpreadInfo(SpreadInfoDTO spreadInfo, bool isDeleteOld = false)
        {
            if (spreadInfo == null || spreadInfo.SpreadId == Guid.Empty || spreadInfo.SpreadCode == Guid.Empty)
                return;
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            if (isDeleteOld)
            {
                //插入SpreadInfo数据库表  
                var oldSpreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.SpreadId == spreadInfo.SpreadId && c.IsDel == 0);

                if (oldSpreadInfo != null)
                {
                    if ((oldSpreadInfo.SpreadId == spreadInfo.SpreadId) && (oldSpreadInfo.SpreadType == spreadInfo.SpreadType))
                        oldSpreadInfo.EntityState = EntityState.Deleted;
                }
            }
            SpreadInfo newSpreadInfo = SpreadInfo.CreateSpreadInfo();
            newSpreadInfo.SpreadId = spreadInfo.SpreadId;
            newSpreadInfo.SpreadCode = spreadInfo.SpreadCode;
            newSpreadInfo.SpreadUrl = spreadInfo.SpreadUrl;
            newSpreadInfo.IsDel = spreadInfo.IsDel;
            newSpreadInfo.SpreadType = spreadInfo.SpreadType;
            newSpreadInfo.SpreadDesc = spreadInfo.SpreadDesc;
            newSpreadInfo.AppId = spreadInfo.AppId;
            contextSession.SaveObject(newSpreadInfo);
            UpdateRalationUserSpread(spreadInfo.SpreadId, spreadInfo.SpreadCode, spreadInfo.SpreadType);
        }
        /// <summary>
        /// 生成微信二维码名称
        /// </summary>
        /// <param name="spreadInfo"></param>
        /// <param name="appDict"></param>
        /// <returns></returns>
        public string BuildQrCodeName(SpreadInfo spreadInfo, Dictionary<Guid, string> appDict)
        {
            string result = string.Empty;
            if (spreadInfo == null)
                return result;
            if (appDict == null || !appDict.Any())
                return result;
            StringBuilder sb = new StringBuilder();
            sb.Append(spreadInfo.Name);
            if (!string.IsNullOrEmpty(spreadInfo.UserCode))
            {
                sb.Append("_");
                sb.Append(spreadInfo.UserCode);
            }
            if (spreadInfo.SpreadAppId != Guid.Empty && appDict.ContainsKey(spreadInfo.SpreadAppId))
            {
                sb.Append("_");
                sb.Append(appDict[spreadInfo.SpreadAppId]);
            }
            if (spreadInfo.HotshopId != Guid.Empty && appDict.ContainsKey(spreadInfo.HotshopId))
            {
                sb.Append("_");
                sb.Append(appDict[spreadInfo.HotshopId]);
            }
            result = sb.ToString();
            if (string.IsNullOrEmpty(result))
            {
                result = "推广主";
            }
            return result;
        }
    }
}
