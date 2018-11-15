using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.Game.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.Game.IBP.Facade;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class GameSV : OutSideServiceBase<GameSVFacade>
    {

    }

    public class GameSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// 更新用户为购买状态。（客户端接口）
        /// </summary>
        /// <param name="lotteryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public bool UpdateWinPlayerBuyed(Guid lotteryId, Guid userId)
        {
            try
            {
                LotteryInfoFacade lotteryInfoFacade = new LotteryInfoFacade();
                lotteryInfoFacade.ContextDTO = AuthorizeHelper.CoinInitAuthorizeInfo();
                ReturnInfoDTO rDTO = lotteryInfoFacade.UpdateWinPlayerBuyed(lotteryId, userId);
                if (rDTO.Code != 1)
                {
                    LogHelper.Error(string.Format("更新好运来用户购买状态异常,lotteryId：{0},userId：{1}，错误信息：{2}", lotteryId, userId, rDTO.Message));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("GameSV.UpdateWinPlayerBuyed服务异常:获取应用信息异常。 lotteryId：{0},userId{1}", lotteryId, userId), ex);
                return false;
            }
        }
    }
}
