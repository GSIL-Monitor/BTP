using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.Metadata;
using Jinher.JAP.Metadata.Description;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.BF.BE.Base;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.Common.Exception;
using Jinher.JAP.Common.Exception.ComExpDefine;
using Jinher.JAP.Common;
using Jinher.JAP.PL;
namespace Jinher.AMP.BTP.BE
{
    public partial class BehaviorRecord
    {
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
        /// 异步记录行为记录
        /// </summary>
        /// <param name="userId">用户Id，匿名用户使用Guid.Empty</param>
        /// <param name="behaviorType">行为类型  1：浏览商品，2：收藏商品，3：删除收藏，4：添加购物车，5：删除购物车商品，6：确认支付</param>
        /// <param name="behaviorKey">行为关键字，与行为类型对应 1：浏览商品：商品Id，2：收藏商品：商品Id，3：删除收藏：商品Id，4：添加购物车：商品Id，5：删除购物车商品：商品Id，6：确认支付：订单Id</param>
        public static void SaveBehaviorAsync(Guid userId, BehaviorTypeEnum behaviorType, string behaviorKey)
        {

            var statik = new StackTrace();

            StackFrame[] stackFrames = statik.GetFrames();
            string method = string.Empty;
            if (stackFrames != null && stackFrames.Count() > 1)
            {
                var methodInfo = stackFrames[1].GetMethod();
                if (methodInfo.DeclaringType != null)
                {
                    method = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
                }
            }
            //行为记录
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(
                    a =>
                    {
                        ContextSession contextSession = ContextFactory.CurrentThreadContext;
                        BehaviorRecord commodityBehavior = CreateBehaviorRecord();
                        commodityBehavior.UserId = userId;
                        commodityBehavior.Method = method;
                        commodityBehavior.Params = string.Empty;
                        commodityBehavior.BehaviorType = (int)behaviorType;
                        commodityBehavior.BehaviorKey = behaviorKey;
                        contextSession.SaveObject(commodityBehavior);
                        contextSession.SaveChanges();
                    });
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format(method + " 行为记录记录异常,\r\nMessage：{0}，StackTrace：{1}", ex.Message, ex.StackTrace));
            }
        }


    }
}