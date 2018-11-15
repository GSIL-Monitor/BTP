
/***************
功能描述: BTP-OPTSV
作    者: 
创建时间: 2015/7/17 18:19:53
***************/
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class UserSpreaderSV : BaseSv, IUserSpreader
    {

        /// <summary>
        /// 保存买家微信和推广者（推广码）之间的关系。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.UserSpreaderSV.svc/SaveSpreaderAndBuyerWxRel
        /// </para>
        /// </summary>
        /// <param name="sbwxDto">参数只传SpreadCode、WxOpenId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSpreaderAndBuyerWxRel(Jinher.AMP.BTP.Deploy.CustomDTO.SpreaderAndBuyerWxDTO sbwxDto)
        {
            base.Do(false);
            LogHelper.Info(string.Format("保存买家微信和推广者（推广码）之间的关系。UserSpreaderSV.SaveSpreaderAndBuyerWxRel, sbwxDto:{0}", JsonHelper.JsonSerializer(sbwxDto)));
            return this.SaveSpreaderAndBuyerWxRelExt(sbwxDto);

        }
        /// <summary>
        /// 更新订单推广者信息。
        /// <para>Service Url: http://devbtp.iuoooo.com/Jinher.AMP.BTP.SV.UserSpreaderSV.svc/UpdateOrderSpreader
        /// </para>
        /// </summary>
        /// <param name="sbwxDto">参数只传WxOpenId、BuyerId、OrderId</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateOrderSpreader(Jinher.AMP.BTP.Deploy.CustomDTO.SpreaderAndBuyerWxDTO sbwxDto)
        {
            base.Do();
            LogHelper.Info(string.Format("更新订单推广者信息。UserSpreaderSV.UpdateOrderSpreaderExt, sbwxDto:{0}", JsonHelper.JsonSerializer(sbwxDto)));
            return this.UpdateOrderSpreaderExt(sbwxDto);

        }

        /// <summary>
        /// 更新用户为推广主
        /// </summary>
        /// <param name="spreaderDto">推广者dto</param>
        /// <returns></returns>
        public ResultDTO UpdateToSpreader(SpreaderAndBuyerWxDTO spreaderDto)
        {
            base.Do(false);
            LogHelper.Debug(string.Format("更新用户为推广主:UserSpreaderSV.UpdateToSpreader, spreaderDto:{0}", JsonHelper.JsonSerializer(spreaderDto)));
            return this.UpdateToSpreaderExt(spreaderDto);
        }

        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <param name="userSpreaderBindDTO">参数只传SpreadCode、UserID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUserSpreaderCode(Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO)
        {
            base.Do(false);
            LogHelper.Debug(string.Format("绑定关系:UserSpreaderSV.SaveUserSpreaderCode, userSpreaderBindDTO:{0}", JsonHelper.JsonSerializer(userSpreaderBindDTO)));
            return this.SaveUserSpreaderCodeExt(userSpreaderBindDTO);
        }
    }
}
