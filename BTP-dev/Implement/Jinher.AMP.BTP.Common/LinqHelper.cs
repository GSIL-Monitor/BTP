using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    public class LinqHelper
    {
        /// <summary>
        /// 获取linq对应的sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string GetEFCommandSql<T>(IQueryable<T> query)
        {
            try
            {
                string sql = "";
                ObjectQuery<T> parents = query as ObjectQuery<T>;
                if (parents != null)
                {
                    sql = parents.ToTraceString();
                }
                return sql;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString(), ex);
            }
            return string.Empty;
        }
    }
}