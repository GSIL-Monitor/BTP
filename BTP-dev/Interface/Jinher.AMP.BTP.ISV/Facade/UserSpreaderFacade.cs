
/***************
功能描述: BTP-OPTFacade
作    者: 
创建时间: 2015/7/17 18:19:52
***************/
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
    public class UserSpreaderFacade : BaseFacade<IUserSpreader>
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
            base.Do();
            return this.Command.SaveSpreaderAndBuyerWxRel(sbwxDto);
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
            return this.Command.UpdateOrderSpreader(sbwxDto);
        }
        /// <summary>
        /// 更新用户为推广主
        /// </summary>
        /// <param name="spreaderDto">推广者dto</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateToSpreader(SpreaderAndBuyerWxDTO spreaderDto)
        {
            base.Do();
            return this.Command.UpdateToSpreader(spreaderDto);
        }
        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <param name="userSpreaderBindDTO">参数只传SpreadCode、UserID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUserSpreaderCode(Jinher.AMP.BTP.Deploy.CustomDTO.UserSpreaderBindDTO userSpreaderBindDTO)
        {
            base.Do();
            return this.Command.SaveUserSpreaderCode(userSpreaderBindDTO);
        }
    }
}