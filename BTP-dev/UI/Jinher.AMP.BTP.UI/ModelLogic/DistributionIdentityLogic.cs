using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;

namespace Jinher.AMP.BTP.UI.ModelLogic
{
 
    /// <summary>
    /// 分销商的身份信息
    /// </summary>
    public class DistributionIdentityLogic
    {
        const int NameCategoryUserUserName = 0;
        const int NameCategoryUserUserCode = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apply"></param>
        public static void DecorateDistributionApplyDto(DistributionApplyResultDTO apply)
        {
            var identitys = apply.IdentityVals;

            if (apply.IdentityVals != null)
            {
                var userNameIdentity = identitys.FirstOrDefault(x => x.NameCategory == NameCategoryUserUserName);
                if (userNameIdentity != null && !string.IsNullOrEmpty(userNameIdentity.Value))
                    apply.UserName = userNameIdentity.Value;

                if (string.IsNullOrWhiteSpace(apply.UserName))
                    apply.UserName = "无";

                var userCodeIdentity = identitys.FirstOrDefault(x => x.NameCategory == NameCategoryUserUserCode);
                if (userCodeIdentity != null && !string.IsNullOrEmpty(userCodeIdentity.Value))
                    apply.UserCode = userCodeIdentity.Value;

                if (string.IsNullOrWhiteSpace(apply.UserCode))
                    apply.UserCode = "无";

                if (string.IsNullOrWhiteSpace(apply.PicturePath))
                    apply.PicturePath = "/Content/images/touxiang.png";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="identitys"></param>
        public static void DecorateManagerSDto(ManagerSDTO apply, List<DistributionIdentityDTO> identitys)
        {

            var userNameIdentity = identitys.FirstOrDefault(x => x.NameCategory == NameCategoryUserUserName);
            if (userNameIdentity != null && !string.IsNullOrEmpty(userNameIdentity.Value))
                apply.Name = userNameIdentity.Value;

            if (string.IsNullOrWhiteSpace(apply.Name)||apply.Name=="null")
                apply.Name = "无";

            var userCodeIdentity = identitys.FirstOrDefault(x => x.NameCategory == NameCategoryUserUserCode);
            if (userCodeIdentity != null && !string.IsNullOrEmpty(userCodeIdentity.Value))
                apply.UserCode = userCodeIdentity.Value;

            if (string.IsNullOrWhiteSpace(apply.UserCode) || apply.UserCode == "null")
                apply.UserCode = "无";

            if (string.IsNullOrWhiteSpace(apply.Pic) || apply.Pic == "null")
                apply.Pic = "/Content/images/touxiang.png";
        }
    }
}