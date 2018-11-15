using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jinher.AMP.BTP.Common
{
    public class PhoneHelper
    {
        /// <summary>
        /// 校验手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool CheckPhoneIsAble(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            if (phone.Length != 11) return false;
            var rx = new Regex("^(13[0-9]|14[579]|15[0-3,5-9]|16[6]|17[0135678]|18[0-9]|19[89])\\d{8}$");
            return rx.IsMatch(phone);
        }


        [System.Runtime.Serialization.DataContract]
        public class UserPhone
        {
            [System.Runtime.Serialization.DataMember]
            public string userPhone;
        }
    }
}
