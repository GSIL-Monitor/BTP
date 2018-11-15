using System;
using System.Linq;
using System.Threading;
using Jinher.JAP.PL;

namespace Jinher.AMP.BTP.BE
{
    public partial class UserCodeForHaiXin
    {
        private static object syncRoot = new object();

        #region 基类抽象方法重载
        public override void BusinessRuleValidate()
        {

        }
        #endregion

        #region 基类虚方法重写
        public override void SetDefaultValue()
        {
            base.SetDefaultValue();
        }
        #endregion

        /// <summary>
        /// 获取用户编号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetUserCode(Guid userId)
        {
            return "0000000000";
            lock (syncRoot)
            {
                var entity = UserCodeForHaiXin.ObjectSet().Where(p => p.UserId == userId).FirstOrDefault();
                if (entity == null)
                {
                    entity = new UserCodeForHaiXin
                    {
                        UserId = userId,
                        UserCode = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString(),
                        EntityState = System.Data.EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(entity);
                    int count = ContextFactory.CurrentThreadContext.SaveChanges();
                    if (count == 0)
                    {
                        Thread.Sleep(1000);
                        entity.UserCode = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
                        ContextFactory.CurrentThreadContext.SaveObject(entity);
                        count = ContextFactory.CurrentThreadContext.SaveChanges();
                        if (count == 0) entity.UserCode = string.Empty;
                    }
                }
                return entity.UserCode;
            }
        }
    }
}



