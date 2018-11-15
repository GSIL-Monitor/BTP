using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.UI.Models
{
    public class UserModel
    {
        /// <summary>
        /// 获取用户信息中的用户名和昵称。
        /// </summary>
        public static Tuple<string, string> GetUserNameAndCode(Guid userId, string invoke)
        {
            string uName = "--";
            string uCode = "--";
            Tuple<string, string> tuple = new Tuple<string, string>(uName, uCode);

            var userNamelist = CBCSV.Instance.GetUserNameAccountsByIds(new List<Guid> { userId });
            if (userNamelist == null)
            {
                return tuple;
            }
            if (!userNamelist.Any())
            {
                return tuple;
            }

            var user = userNamelist.First();
            uName = user.userName;
            if (user.Accounts == null || (!user.Accounts.Any()))
            {
                return tuple;
            }
            //取手机号，如果手机号为空取 邮箱， 还为空，随便取
            var acc = user.Accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && !c.Account.Contains('@'));
            if (acc == null)
            {
                acc = user.Accounts.FirstOrDefault(c => c.AccountType == CBC.Deploy.Enum.AccountSrcEnum.System && !string.IsNullOrEmpty(c.Account) && c.Account.Contains('@'));
                if (acc == null)
                {
                    acc = user.Accounts.FirstOrDefault(c => !string.IsNullOrEmpty(c.Account));
                }
            }

            if (acc != null)
            {
                uCode = acc.Account;
            }
            tuple = new Tuple<string, string>(uName, uCode);
            return tuple;

        }

    }
}